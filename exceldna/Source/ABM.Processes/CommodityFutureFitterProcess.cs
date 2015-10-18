namespace QMALibConsole
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;

    using log4net;

    using QMA.Analytics.Solvers;
    using QMA.Common;
    using QMA.Data.Services;
    using QMA.Model;
    using QMA.Model.Events;

    public class CommodityFutureFitterProcess : FittableProcess
    {
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
        ///     The solver result.
        /// </summary>
        private ISolverResult solverResult;

        public CommodityFutureFitterProcess(ILog logger, IInstrumentFactory instrumentFactory, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.instrumentFactory = instrumentFactory;
            this.eventAggregator = eventAggregator;

            this.InitialiseSubscriptions();
        }

        public override void Run()
        {
            // this is the fittable object thread
            this.RunAsyncThread();

            // add the commodity futures
            // open a file with the data, read and create the fittable objects
            this.instrumentFactory.BuildAndPublish<CommodityFuture>("NGH5 Comdty");
        }
    }
}