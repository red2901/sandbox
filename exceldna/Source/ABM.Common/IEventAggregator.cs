// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventAggregator.cs" company="">
//   
// </copyright>
// <summary>
//   The EventAggregator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    using System;

    /// <summary>
    ///     The EventAggregator interface.
    /// </summary>
    public interface IEventAggregator : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get event.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IObservable" />.
        /// </returns>
        IObservable<TEvent> GetEvent<TEvent>();

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="sampleEvent">
        /// The sample event.
        /// </param>
        /// <typeparam name="TEvent">
        /// </typeparam>
        void Publish<TEvent>(TEvent sampleEvent);

        #endregion
    }
}