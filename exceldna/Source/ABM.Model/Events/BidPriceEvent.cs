// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BidPriceEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The bid price event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    /// <summary>
    /// The bid price event.
    /// </summary>
    public class BidPriceEvent : PriceEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BidPriceEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="securityKey">
        /// The security key.
        /// </param>
        /// <param name="priceUpdate">
        /// The price update.
        /// </param>
        public BidPriceEvent(EventType eventType, string securityKey, double priceUpdate)
            : base(eventType, securityKey, priceUpdate)
        {
        }

        #endregion
    }
}