// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedObject.cs" company="">
//   
// </copyright>
// <summary>
//   The managed object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    /// <summary>
    ///     The managed object.
    /// </summary>
    public class ManagedObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedObject"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        public ManagedObject(string key, object o)
        {
            this.Key = key;
            this.Object = o;
            this.Version = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the object.
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        public int Version { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The key string.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string KeyString(string reference)
        {
            return reference.Split('#')[0];
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}#{1}", this.Key, this.Version);
        }

        /// <summary>
        ///     The update version.
        /// </summary>
        public void UpdateVersion()
        {
            this.Version += 1;
        }

        #endregion
    }
}