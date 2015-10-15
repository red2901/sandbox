// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPriceCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The PriceCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    /// <summary>
    ///     The PriceCollection interface.
    /// </summary>
    public interface IPriceCollection
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void Add(string key, double value);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double Get(string key);

        /// <summary>
        ///     The has values.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool HasValues();

        #endregion
    }
}