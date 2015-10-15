// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
//   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     The extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The is available.
        /// </summary>
        /// <param name="priceCollection">
        /// The price collection.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasValues<T>(this T collection) where T : ICollection
        {
            if (collection == null)
            {
                return false;
            }

            if (collection.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The to object array.
        /// </summary>
        /// <param name="cashflows">
        /// The cashflows.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ToObjectArray(this CashFlowStream cashflows)
        {
            var objectArray = new object[cashflows.Count, 3];

            for (int i = 0; i < cashflows.Count; i++)
            {
                objectArray[i, 0] = cashflows[i].Date;
                objectArray[i, 1] = cashflows[i].Amount;
                objectArray[i, 2] = cashflows[i].Principle;
            }

            return objectArray;
        }

        /// <summary>
        /// The to object array.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ToObjectArray(this Bond bond)
        {
            return ToObjectArray<Bond>(bond);
        }

        /// <summary>
        /// The to object array.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ToObjectArray<T>(T obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            var objectArray = new object[properties.Length, 2];

            for (int i = 0; i < properties.Length; i++)
            {
                objectArray[i, 0] = properties[i].Name;
                objectArray[i, 1] = properties[i].GetValue(obj, null);
            }

            return objectArray;
        }

        #endregion
    }
}