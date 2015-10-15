// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriceCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The price collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System.Collections.Generic;

    /// <summary>
    ///     The price collection.
    /// </summary>
    public class PriceCollection : IPriceCollection
    {
        #region Fields

        /// <summary>
        ///     The collection.
        /// </summary>
        private readonly Dictionary<string, double> collection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriceCollection" /> class.
        /// </summary>
        public PriceCollection()
        {
            this.collection = new Dictionary<string, double>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCollection"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public PriceCollection(string key, double value)
        {
            this.collection = new Dictionary<string, double> { { key, value } };
        }

        #endregion

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
        public void Add(string key, double value)
        {
            this.collection.Add(key, value);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Get(string key)
        {
            double value;
            this.collection.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        ///     The has values.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool HasValues()
        {
            return this.collection.Count != 0;
        }

        #endregion
    }
}