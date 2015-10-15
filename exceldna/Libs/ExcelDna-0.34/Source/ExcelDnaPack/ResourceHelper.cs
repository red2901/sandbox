﻿//  Copyright (c) Govert van Drimmelen. All rights reserved.
//  Excel-DNA is licensed under the zlib license. See LICENSE.txt for details.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using SevenZip.Compression.LZMA;
using System.Threading;

internal unsafe static class ResourceHelper
{
    internal enum TypeName
    {
        CONFIG = -1,
        ASSEMBLY = 0,
        DNA = 1,
        IMAGE = 2,
        SOURCE = 3,
    }

    // TODO: Learn about locales
    private const ushort localeNeutral		= 0;
	private const ushort localeEnglishUS	= 1033;
	private const ushort localeEnglishSA	= 7177;

	[DllImport("kernel32.dll")]
	private static extern IntPtr BeginUpdateResource(
		string pFileName,
		bool bDeleteExistingResources);

	[DllImport("kernel32.dll")]
	private static extern bool EndUpdateResource(
		IntPtr hUpdate,
		bool fDiscard);
	
	//, EntryPoint="UpdateResourceA", CharSet=CharSet.Ansi,
	[DllImport("kernel32.dll", SetLastError=true)]
	private static extern bool UpdateResource(
		IntPtr hUpdate,
		string lpType,
		string lpName,
		ushort wLanguage,
		IntPtr lpData,
		uint cbData);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool UpdateResource(
        IntPtr hUpdate,
        string lpType,
        IntPtr intResource,
        ushort wLanguage,
        IntPtr lpData,
        uint cbData);

	[DllImport("kernel32.dll")]
	private static extern uint GetLastError();

	internal unsafe class ResourceUpdater
	{        
        int typelibIndex = 0;
		IntPtr _hUpdate;
		List<object> updateData = new List<object>();

        object lockResource = new object();

        List<ManualResetEvent> finishedTask = new List<ManualResetEvent>();

        public ResourceUpdater(string fileName)
		{
			_hUpdate = BeginUpdateResource(fileName, false);
			if (_hUpdate == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
		}

        private void CompressDoUpdateHelper(byte[] content, string name, TypeName typeName, bool compress)
        {
            if (compress)
                content = SevenZipHelper.Compress(content);
            DoUpdateResource(typeName.ToString() + (compress ? "_LZMA" : ""), name, content);
        }

        public string AddFile(byte[] content, string name, TypeName typeName, bool compress, bool multithreading)
        {
            Debug.Assert(name == name.ToUpperInvariant());

            if (multithreading)
            {
                var mre = new ManualResetEvent(false);
                finishedTask.Add(mre);
                ThreadPool.QueueUserWorkItem(delegate
                    {
                        CompressDoUpdateHelper(content, name, typeName, compress);
                        mre.Set();
                    }
                );
            }
            else
                CompressDoUpdateHelper(content, name, typeName, compress);

            return name;
        }

        public string AddAssembly(string path, bool compress, bool multithreading)
		{
			try
			{
				byte[] assBytes = File.ReadAllBytes(path);
				// Not just into the Reflection context, because this Load is used to get the name and also to 
				// check that the assembly can Load from bytes (mixed assemblies can't).
				Assembly ass = Assembly.Load(assBytes);
				string name = ass.GetName().Name.ToUpperInvariant(); // .ToUpperInvariant().Replace(".", "_");

                AddFile(assBytes, name, TypeName.ASSEMBLY, compress, multithreading);				
				return name;
			}
			catch (Exception e)
			{
				Console.WriteLine("Assembly at " + path + " could not be packed. Possibly a mixed assembly? (These are not supported yet.)\r\nException: " + e);
				return null;
			}
		}

        public int AddTypeLib(byte[] data)
        {
            lock (lockResource)
            {
                string typeName = "TYPELIB";
                typelibIndex++;

                Console.WriteLine(string.Format("  ->  Updating typelib: Type: {0}, Index: {1}, Length: {2}", typeName, typelibIndex, data.Length));
                GCHandle pinHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                updateData.Add(pinHandle);

                bool result = ResourceHelper.UpdateResource(_hUpdate, typeName, (IntPtr)typelibIndex, localeNeutral, pinHandle.AddrOfPinnedObject(), (uint)data.Length);
                if (!result)
                {
                    throw new Win32Exception();
                }

            }
            return typelibIndex;
        }

		public void DoUpdateResource(string typeName, string name, byte[] data)
		{
            lock (lockResource)
            {
                Console.WriteLine(string.Format("  ->  Updating resource: Type: {0}, Name: {1}, Length: {2}", typeName, name, data.Length));
                GCHandle pinHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                updateData.Add(pinHandle);

                bool result = ResourceHelper.UpdateResource(_hUpdate, typeName, name, localeNeutral, pinHandle.AddrOfPinnedObject(), (uint)data.Length);
                if (!result)
                {
                    throw new Win32Exception();
                }
            }
        }

        public void RemoveResource(string typeName, string name)
        {
            lock (lockResource)
            {
                bool result = ResourceHelper.UpdateResource(_hUpdate, typeName, name, localeEnglishUS, IntPtr.Zero, 0);
                if (!result)
                {
                    throw new Win32Exception();
                }
            }
        }

		public void EndUpdate()
		{
			EndUpdate(false);
		}

		public void EndUpdate(bool discard)
		{
            if (finishedTask.Count > 0)
            {
                ManualResetEvent[] mre = new ManualResetEvent[finishedTask.Count];
                for (int i = 0; i < finishedTask.Count; i++)
                    mre[i] = finishedTask[i];

                WaitHandle.WaitAll(mre);
            }

            bool result = EndUpdateResource(_hUpdate, discard);
			if (!result)
			{
				throw new Win32Exception();
			}
		}
	}
}