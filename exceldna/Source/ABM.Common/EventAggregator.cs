// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventAggregator.cs" company="">
//   
// </copyright>
// <summary>
//   The event aggregator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    // based on http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/
    // and http://machadogj.com/2011/3/yet-another-event-aggregator-using-rx.html
    /// <summary>
    /// The event aggregator.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region Fields

        /// <summary>
        /// The subject.
        /// </summary>
        private readonly Subject<object> subject = new Subject<object>();

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The get event.
        /// </summary>
        /// <typeparam name="TEvent">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return this.subject.OfType<TEvent>().AsObservable();
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="sampleEvent">
        /// The sample event.
        /// </param>
        /// <typeparam name="TEvent">
        /// </typeparam>
        public void Publish<TEvent>(TEvent sampleEvent)
        {
            this.subject.OnNext(sampleEvent);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.subject.Dispose();

            this.disposed = true;
        }

        #endregion
    }
}