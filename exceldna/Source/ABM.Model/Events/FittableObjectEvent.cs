// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFittableObjectEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The add fittable object event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model.Events
{
    /// <summary>
    ///     The add fittable object event.
    /// </summary>
    public class FittableObjectEvent
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFittableObjectEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event Type.
        /// </param>
        /// <param name="fittableObject">
        /// The fittable object.
        /// </param>
        public FittableObjectEvent(EventType eventType, IFittableObject fittableObject)
        {
            this.EventType = eventType;
            this.FittableObject = fittableObject;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        ///     Gets or sets the fittable object.
        /// </summary>
        public IFittableObject FittableObject { get; set; }

        #endregion
    }
}