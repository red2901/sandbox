// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUtil.cs" company="">
//   
// </copyright>
// <summary>
//   The file util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The file util.
    /// </summary>
    public class FileUtil
    {
        #region Public Methods and Operators

        /// <summary>
        /// The read file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<string> ReadFileIntoStringList(string fileName)
        {
            IList<string> fileContents = new List<string>();
            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    fileContents.Add(line);
                }
            }

            return fileContents;
        }

        #endregion
    }
}