// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectManagerService.cs" company="">
//   
// </copyright>
// <summary>
//   The object data service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using ABM.Common;

    /// <summary>
    ///     The object data service.
    /// </summary>
    public class ObjectManagerService : IObjectManagerService
    {
        #region Fields

        /// <summary>
        ///     The data.
        /// </summary>
        private readonly ConcurrentDictionary<string, ManagedObject> data;

        /// <summary>
        ///     The initialise date.
        /// </summary>
        private DateTime initialiseDate;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectManagerService" /> class.
        /// </summary>
        public ObjectManagerService()
        {
            this.data = new ConcurrentDictionary<string, ManagedObject>();
            this.initialiseDate = DateTime.Today;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.data.Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
            this.data.Clear();
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(string key)
        {
            if (!this.IsValid())
            {
                this.data.Clear();
                return false;
            }

            return this.data.ContainsKey(key);
        }

        /// <summary>
        /// The key list.
        /// </summary>
        /// <returns>
        /// The <see cref="ICollection"/>.
        /// </returns>
        public IList<string> KeyList()
        {
            var keyList = new List<string>();
            foreach (var key in this.data.Keys)
            {
                ManagedObject o;
                this.data.TryGetValue(key, out o);
                keyList.Add(o.ToString());
            }
            return keyList;
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public object Remove(string key)
        {
            ManagedObject o = null;
            this.data.TryRemove(key, out o);
            return o.Object;
        }

        /// <summary>
        /// The replace.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        public ManagedObject Replace(string key, object o)
        {
            ManagedObject managedObject = this.RetrieveManagedObject(key);
            managedObject.Object = o;
            managedObject.UpdateVersion();
            return managedObject;
        }

        /// <summary>
        /// The retrieve.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public object Retrieve(string key)
        {
            ManagedObject o = null;

            if (this.data.ContainsKey(key))
            {
                this.data.TryGetValue(key, out o);
            }

            return o == null ? null : o.Object;
        }

        /// <summary>
        /// The retrieve managed object.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        public ManagedObject RetrieveManagedObject(string key)
        {
            ManagedObject o = null;

            if (this.data.ContainsKey(key))
            {
                this.data.TryGetValue(key, out o);
            }

            return o;
        }

        /// <summary>
        /// The retrieve using ref.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object RetrieveUsingRef(string reference)
        {
            return this.Retrieve(ManagedObject.KeyString(reference));
        }

        /// <summary>
        /// The store.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public ManagedObject Store(string key, object o)
        {
            ManagedObject managedObject = null;
            if (this.Contains(key))
            {
                ManagedObject existingObject = this.RetrieveManagedObject(key);
                if (o is object[,])
                {
                    var omatrix = o as object[,];
                    var existingOMatrix = existingObject.Object as object[,];
                    if (omatrix.GetModifiedHashCode() != existingOMatrix.GetModifiedHashCode())
                    {
                        return this.Replace(key, o);
                    }
                }
                else
                {
                    if (o.GetHashCode() != existingObject.Object.GetHashCode())
                    {
                        return this.Replace(key, o);
                    }
                }

                return existingObject;
            }

            managedObject = new ManagedObject(key, o);
            this.data.TryAdd(key, managedObject);

            return managedObject;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The is valid.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool IsValid()
        {
            if (this.initialiseDate.Equals(DateTime.Today))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}