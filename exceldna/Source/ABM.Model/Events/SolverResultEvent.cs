// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverResultEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The solver result event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    using ABM.Analytics.Solvers;

    /// <summary>
    /// The solver result event.
    /// </summary>
    public class SolverResultEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverResultEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="solverResult">
        /// The solver result.
        /// </param>
        public SolverResultEvent(EventType eventType, ISolverResult solverResult)
        {
            this.EventType = eventType;
            this.SolverResult = solverResult;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the solver result.
        /// </summary>
        public ISolverResult SolverResult { get; set; }

        #endregion
    }
}