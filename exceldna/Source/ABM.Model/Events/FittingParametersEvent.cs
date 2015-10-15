// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FittingParametersEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The fitting parameters event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model.Events
{
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    /// The fitting parameters event.
    /// </summary>
    public class FittingParametersEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FittingParametersEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="fittingParameters">
        /// The fitting parameters.
        /// </param>
        public FittingParametersEvent(EventType eventType, DenseVector fittingParameters)
        {
            this.EventType = eventType;
            this.FittingParameters = fittingParameters;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the fitting parameters.
        /// </summary>
        public DenseVector FittingParameters { get; set; }

        #endregion
    }
}