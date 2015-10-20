// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectProperty.cs" company="">
//   
// </copyright>
// <summary>
//   The object property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Functions
{
    using System.Collections.Generic;
    using System.Reflection;

    using ABM.Common;
    using ABM.Data.Services;

    /// <summary>
    ///     The object property.
    /// </summary>
    public class ObjectProperty
    {
        #region Public Methods and Operators

        /// <summary>
        /// The qma object property list.
        /// </summary>
        /// <param name="objRef">
        /// The obj Ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMObjectPropertyList(string objRef)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(objRef);

            if (mo == null)
            {
                return null;
            }

            object o = mo.RawObject;
            var plist = new List<string>();
            foreach (PropertyInfo prop in o.GetType().GetProperties())
            {
                plist.Add(prop.Name);
            }

            return plist.ToObjectArray();
        }

        /// <summary>
        /// The qma object property value.
        /// </summary>
        /// <param name="objRef">
        /// The obj ref.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMObjectPropertyValue(string objRef, string property)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(objRef);

            if (mo == null)
            {
                return null;
            }

            object o = mo.RawObject;
            foreach (PropertyInfo prop in o.GetType().GetProperties())
            {
                if (prop.Name.Equals(property))
                {
                    var result = prop.GetValue(o, null);

                    if (result is IList<double>)
                    {
                        return (result as IList<double>).ToObjectArray();
                    }

                    if (result is IList<string>)
                    {
                        return (result as IList<string>).ToObjectArray();
                    }

                    return result;
                }
            }

            return null;
        }

        #endregion
    }
}