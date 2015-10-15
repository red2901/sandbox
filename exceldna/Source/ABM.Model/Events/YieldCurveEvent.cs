// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewYieldCurveEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The new yield curve event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model.Events
{
    /// <summary>
    ///     The new yield curve event.
    /// </summary>
    public class YieldCurveEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YieldCurveEvent"/> class. 
        /// </summary>
        /// <param name="eventType">
        /// The event Type.
        /// </param>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        public YieldCurveEvent(EventType eventType, IYieldCurve yieldCurve)
        {
            this.EventType = eventType;
            this.YieldCurve = yieldCurve;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        ///     Gets or sets the yield curve.
        /// </summary>
        public IYieldCurve YieldCurve { get; set; }

        #endregion
    }
}