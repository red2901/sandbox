// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedObject.cs" company="">
//   
// </copyright>
// <summary>
//   The managed object wrapper. Object envelope which enables tracking of the object in the object manager service.
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
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        public ManagedObject(string name, object o)
        {
            this.Name = name;
            this.RawObject = o;
            this.Version = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the object.
        /// </summary>
        public object RawObject { get; set; }

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
        public static string NameString(string reference)
        {
            return reference.Split('#')[0];
        }

        /// <summary>
        ///     Returns the string representation of the storage key.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string KeyString()
        {
            return string.Format("{0}#{1}", this.Name, this.Version);
        }

        /// <summary>
        ///     Returns the string representation of the underlying object.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return this.RawObject.ToString();
        }

        /// <summary>
        /// Update the object which is pointed to.
        /// </summary>
        /// <param name="o">
        /// </param>
        public void Update(object o)
        {
            this.RawObject = o;
            this.UpdateVersion();
        }

        /// <summary>
        ///     Tick up the version of the object to be saved by 1.
        /// </summary>
        public void UpdateVersion()
        {
            this.Version += 1;
        }

        #endregion
    }
}