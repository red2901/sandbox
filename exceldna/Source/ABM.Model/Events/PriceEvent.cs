// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PriceEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The price event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    /// <summary>
    /// The price event.
    /// </summary>
    public class PriceEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceEvent"/> class.
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
        public PriceEvent(EventType eventType, string securityKey, double priceUpdate)
        {
            this.EventType = eventType;
            this.SecurityKey = securityKey;
            this.PriceUpdate = priceUpdate;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the price update.
        /// </summary>
        public double PriceUpdate { get; set; }

        /// <summary>
        /// Gets or sets the security key.
        /// </summary>
        public string SecurityKey { get; set; }

        #endregion
    }
}