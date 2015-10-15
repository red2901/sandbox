// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BidPrice.cs" company="">
//   
// </copyright>
// <summary>
//   The bid price.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model
{
    /// <summary>
    /// The bid price.
    /// </summary>
    public class BidPrice
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BidPrice"/> class.
        /// </summary>
        /// <param name="requestKey">
        /// The request key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public BidPrice(string requestKey, double value)
        {
            this.RequestKey = requestKey;
            this.Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the request key.
        /// </summary>
        public string RequestKey { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; private set; }

        #endregion
    }
}