// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedObjectRepositoryService.cs" company="">
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
    public class ManagedObjectRepositoryService : IManagedObjectRepositoryService
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
        ///     Initializes a new instance of the <see cref="ManagedObjectRepositoryService" /> class.
        /// </summary>
        public ManagedObjectRepositoryService()
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
        public ManagedObject Add(string key, object o)
        {
            var managedObject = new ManagedObject(key, o);
            return this.Add(managedObject);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="mo">
        /// The mo.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        public ManagedObject Add(ManagedObject mo)
        {
            string key = mo.Name;
            ManagedObject existingObject = this.RetrieveExistingObject(mo);
            if (existingObject == null)
            {
                this.data.TryAdd(key, mo);
            }
            else
            {
                return existingObject;
            }

            return mo;
        }

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
        ///     The key list.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IEnumerable<string> Keys()
        {
            foreach (string key in this.data.Keys)
            {
                ManagedObject o;
                this.data.TryGetValue(key, out o);
                yield return o.KeyString();
            }
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public object Remove(string reference)
        {
            ManagedObject o = null;
            string key = ManagedObject.NameString(reference);
            this.data.TryRemove(key, out o);
            return o.RawObject;
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
            managedObject.Update(o);
            return managedObject;
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
        public object Retrieve(string reference)
        {
            return this.RetrieveRawObject(ManagedObject.NameString(reference));
        }

        /// <summary>
        /// The retrieve managed object.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        public ManagedObject RetrieveManagedObject(string reference)
        {
            ManagedObject o = null;

            string key = ManagedObject.NameString(reference);

            if (this.data.ContainsKey(key))
            {
                this.data.TryGetValue(key, out o);
            }

            return o;
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

        /// <summary>
        /// The retrieve existing object.
        /// </summary>
        /// <param name="mo">
        /// The mo.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        private ManagedObject RetrieveExistingObject(ManagedObject mo)
        {
            string key = mo.Name;
            object o = mo.RawObject;
            if (this.Contains(key))
            {
                ManagedObject existingObject = this.RetrieveManagedObject(key);
                if (o is object[,])
                {
                    var omatrix = o as object[,];
                    var existingOMatrix = existingObject.RawObject as object[,];
                    if (omatrix.GetModifiedHashCode() != existingOMatrix.GetModifiedHashCode())
                    {
                        return this.Replace(key, o);
                    }
                }
                else
                {
                    if (o.GetHashCode() != existingObject.RawObject.GetHashCode())
                    {
                        return this.Replace(key, o);
                    }
                }

                return existingObject;
            }

            return null;
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
        private object RetrieveRawObject(string key)
        {
            ManagedObject o = null;

            if (this.data.ContainsKey(key))
            {
                this.data.TryGetValue(key, out o);
            }

            return o == null ? null : o.RawObject;
        }

        #endregion
    }
}