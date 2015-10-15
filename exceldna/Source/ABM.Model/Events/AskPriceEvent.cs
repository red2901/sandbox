// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AskPriceEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The ask price event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    /// <summary>
    /// The ask price event.
    /// </summary>
    public class AskPriceEvent : PriceEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AskPriceEvent"/> class.
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
        public AskPriceEvent(EventType eventType, string securityKey, double priceUpdate)
            : base(eventType, securityKey, priceUpdate)
        {
        }

        #endregion
    }
}