// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FittableProcess.cs" company="">
//   
// </copyright>
// <summary>
//   The fittable process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace QMALibConsole
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading;

    using log4net;

    using Microsoft.Practices.ServiceLocation;

    using QMA.Analytics.Solvers;
    using QMA.Common;
    using QMA.Data.Services;
    using QMA.Model;
    using QMA.Model.Events;

    /// <summary>
    ///     The fittable process.
    /// </summary>
    public abstract class FittableProcess : IFittableProcess
    {
        #region Fields

        /// <summary>
        ///     The event aggregator.
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        ///     The instrument factory.
        /// </summary>
        private readonly IInstrumentFactory instrumentFactory;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        ///     The fitting thread.
        /// </summary>
        private Thread fittingThread;

        /// <summary>
        ///     The solver result.
        /// </summary>
        private ISolverResult solverResult;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the subscriptions.
        /// </summary>
        public IList<IDisposable> Subscriptions { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable subscription in this.Subscriptions)
            {
                subscription.Dispose();
            }
        }

        /// <summary>
        ///     The initialise subscriptions.
        /// </summary>
        public void InitialiseSubscriptions()
        {
            this.Subscriptions = new List<IDisposable>
                                     {
                                         this.eventAggregator.GetEvent<SolverResultEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(
                                                 ev => this.solverResult = ev.SolverResult.Clone())
                                     };
        }

        /// <summary>
        /// The run.
        /// </summary>
        public abstract void Run();

        /// <summary>
        ///     The run async.
        /// </summary>
        public void RunAsync()
        {
            var fittableObjectCollection = ServiceLocator.Current.GetInstance<IFittableObjectCollection>();

            // continue forever
            while (true)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// The run async thread.
        /// </summary>
        public void RunAsyncThread()
        {
            // start the fitting thread
            this.logger.Info("Starting the fitter thread");

            // create a thread
            this.fittingThread = new Thread(this.RunAsync);
            this.fittingThread.Start();

            this.logger.Info("Running ... ");
        }

        #endregion
    }
}