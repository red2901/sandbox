// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnchorDateEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The anchor date event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    /// <summary>
    /// The anchor date event.
    /// </summary>
    public class AnchorDateEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorDateEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="anchorDate">
        /// The anchor date.
        /// </param>
        public AnchorDateEvent(EventType eventType, double anchorDate)
        {
            this.EventType = eventType;
            this.AnchorDate = anchorDate;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the anchor date.
        /// </summary>
        public double AnchorDate { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        #endregion
    }
}