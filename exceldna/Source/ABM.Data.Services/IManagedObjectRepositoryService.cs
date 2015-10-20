// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManagedObjectRepositoryService.cs" company="">
//   
// </copyright>
// <summary>
//   The ObjectManagerService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     The ObjectManagerService interface.
    /// </summary>
    public interface IManagedObjectRepositoryService
    {
        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        int Count { get; }

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
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        ManagedObject Add(string key, object o);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="mo">
        /// The mo.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        ManagedObject Add(ManagedObject mo);

        /// <summary>
        ///     The clear.
        /// </summary>
        void Clear();

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Contains(string key);

        /// <summary>
        ///     The key list.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IEnumerable<string> Keys();

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Remove(string reference);

        /// <summary>
        /// The retrieve using ref.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Retrieve(string reference);

        /// <summary>
        /// The retrieve managed object.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="ManagedObject"/>.
        /// </returns>
        ManagedObject RetrieveManagedObject(string reference);

        #endregion
    }
}