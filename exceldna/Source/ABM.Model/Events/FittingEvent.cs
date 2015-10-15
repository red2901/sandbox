// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The solver event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    /// <summary>
    /// The solver event.
    /// </summary>
    public class FittingEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FittingEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        public FittingEvent(EventType eventType)
        {
            this.EventType = eventType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        #endregion
    }
}