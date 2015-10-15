// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectManager.cs" company="">
//   
// </copyright>
// <summary>
//   The object manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Functions
{
    using System.Collections.Generic;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Data.Services;

    /// <summary>
    ///     The object manager.
    /// </summary>
    public class ObjectManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The qma object manager clear.
        /// </summary>
        public static void ABMObjectManagerClear()
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();
            objectManagerService.Clear();
        }

        /// <summary>
        ///     The qma object count.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static object ABMObjectManagerCount()
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();
            return objectManagerService.Count;
        }

        /// <summary>
        /// The qma object create.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ABMObjectManagerCreate(string key, object data)
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();

            ManagedObject managedObject = objectManagerService.Store(key, data);
            return managedObject.ToString();
        }

        /// <summary>
        ///     The qma object manager list.
        /// </summary>
        /// <returns>
        ///     The <see cref="ICollection" />.
        /// </returns>
        public static IList<string> ABMObjectManagerList()
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();
            return objectManagerService.KeyList();
        }

        /// <summary>
        /// The qma object manager retrieve.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMObjectManagerRetrieve(string key)
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();
            return objectManagerService.RetrieveUsingRef(key);
        }

        /// <summary>
        /// The qma object manager retrieve managed object.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMObjectManagerRetrieveManagedObject(string ticker)
        {
            var objectManagerService = ServiceLocator.Current.GetInstance<IObjectManagerService>();
            return objectManagerService.RetrieveManagedObject(ticker);
        }

        #endregion
    }
}