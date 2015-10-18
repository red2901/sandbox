namespace QMALibConsole
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using QMA.Analytics.Solvers;
    using QMA.Common;
    using QMA.Data.Services;
    using QMA.Model;
    using QMA.Model.Events;

    /// <summary>
    ///     The fitter.
    /// </summary>
    public class BondFitterProcess : FittableProcess
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

        /// <summary>
        /// The yield curve.
        /// </summary>
        private IYieldCurve yieldCurve;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Fitter"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="instrumentFactory">
        /// The instrument factory.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        public BondFitterProcess(ILog logger, IInstrumentFactory instrumentFactory, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.instrumentFactory = instrumentFactory;
            this.eventAggregator = eventAggregator;

            this.InitialiseSubscriptions();
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The check anchor point change.
        /// </summary>
        public void CheckAnchorPointChange()
        {
        }

        /// <summary>
        /// The check bond change.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        public void CheckBondChange(IYieldCurve yieldCurve)
        {
            this.Refit(yieldCurve, 1);
            double valueCurrent = this.solverResult.ValueCurrent;
            Vector<double> parametersCurrent = this.solverResult.ParametersCurrent.Clone();

            string bondkey = "EJ407307 Corp";
            var bond = this.instrumentFactory.Build<IBond>(bondkey);
            this.eventAggregator.Publish(new FittableObjectEvent(EventType.Add, bond));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, bondkey, 123.855));

            this.Refit(yieldCurve, 2);
            this.eventAggregator.Publish(new FittableObjectEvent(EventType.Remove, bond));
            bond.Dispose();
            this.Refit(yieldCurve, 3);

            double valueCurrentAfterRefit = this.solverResult.ValueCurrent;
            Vector<double> parametersCurrentNew = this.solverResult.ParametersCurrent.Clone();

            Vector<double> parametersDiff = parametersCurrent.Subtract(parametersCurrentNew);

            if (Math.Abs(valueCurrent - valueCurrentAfterRefit) > 0.000001)
            {
                this.logger.ErrorFormat("Problem with refitting!!");
            }
            else
            {
                this.logger.InfoFormat("Bond changes OK");
            }

            foreach (double d in parametersDiff)
            {
                if (Math.Abs(d) > 0.0001)
                {
                    this.logger.ErrorFormat("Problem with refitting (diff = {0})!!", d);
                    return;
                }
            }

            this.logger.InfoFormat("Parameter change due to bond changes OK");
        }

        /// <summary>
        /// The check price change.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        public void CheckPriceChange(IYieldCurve yieldCurve)
        {
            this.Refit(yieldCurve, 1);
            double valueCurrent = this.solverResult.ValueCurrent;
            Vector<double> parametersCurrent = this.solverResult.ParametersCurrent.Clone();

            string bondkey = "EH887630 Corp";
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, bondkey, 111.3));
            this.Refit(yieldCurve, 2);
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, bondkey, 112.3));
            this.Refit(yieldCurve, 3);
            double valueCurrentAfterRefit = this.solverResult.ValueCurrent;
            Vector<double> parametersCurrentNew = this.solverResult.ParametersCurrent.Clone();

            Vector<double> parametersDiff = parametersCurrent.Subtract(parametersCurrentNew);

            if (Math.Abs(valueCurrent - valueCurrentAfterRefit) > 0.000001)
            {
                this.logger.ErrorFormat("Problem with refitting!!");
            }
            else
            {
                this.logger.InfoFormat("Price changes OK");
            }

            foreach (double d in parametersDiff)
            {
                if (Math.Abs(d) > 0.0001)
                {
                    this.logger.ErrorFormat("Problem with refitting (diff = {0})!!", d);
                    return;
                }
            }

            this.logger.InfoFormat("Parameter change due to price changes OK");
        }

        /// <summary>
        ///     The days.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> Days()
        {
            var days = new List<double>();

            days.Add(2 * 365.25);
            days.Add(5 * 365.25);
            days.Add(10 * 365.25);
            days.Add(30 * 365.25);

            return days;
        }

        /// <summary>
        /// The initial yield curve.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <returns>
        /// The <see cref="IYieldCurve"/>.
        /// </returns>
        public IYieldCurve InitialYieldCurve(double startDate)
        {
            var curveDates = new List<double>();
            var curveYields = new List<double>();
            curveDates.Add(startDate);
            curveYields.Add(0.05);

            foreach (double day in this.Days())
            {
                double dt = startDate + day;
                curveDates.Add(dt);
                curveYields.Add(0.05);
            }

            this.yieldCurve = ServiceLocator.Current.GetInstance<IYieldCurve>();

            this.yieldCurve.CurveDates = curveDates;
            this.yieldCurve.Yields = curveYields;
            return this.yieldCurve;
        }

        /// <summary>
        /// The refit.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        /// <param name="idx">
        /// The idx.
        /// </param>
        public void Refit(IYieldCurve yieldCurve, int idx)
        {
            this.logger.DebugFormat(
                "================================================== Refit {0} ======================================================", 
                idx);

            // update the yield curve, fitting parameters 
            yieldCurve.UpdateYields(this.solverResult.ParametersNew);
            this.eventAggregator.Publish(new YieldCurveEvent(EventType.Update, yieldCurve));
            this.eventAggregator.Publish(
                new FittingParametersEvent(EventType.Update, DenseVector.OfVector(this.solverResult.ParametersCurrent)));

            // fit again
            this.eventAggregator.Publish(new FittingEvent(EventType.Update));

            this.logger.DebugFormat("SolverResult : {0}{1}", Environment.NewLine, this.solverResult.ToDisplayString());
            this.logger.DebugFormat(
                "Iterations : {0}{1}", 
                Environment.NewLine, 
                this.solverResult.IterationResults.ToDisplayString());
        }

        /// <summary>
        ///     The run.
        /// </summary>
        public override void Run()
        {
            // start the fitting thread
            this.RunAsyncThread();

            // ok act like excel and add some bonds for fitting and test the results
            // add a fittable object
            foreach (string key in
                new List<string>
                    {
                        "EI450338 Corp", 
                        "EI778872 Corp", 
                        "EH887630 Corp", 
                        "ED860812 Corp", 
                        "EC453273 Corp", 
                        "EK333934 Corp", 
                        "EF396423 Corp"
                    })
            {
                var bond = this.instrumentFactory.Build<IBond>(key);
                this.eventAggregator.Publish(new FittableObjectEvent(EventType.Add, bond));
            }

            IYieldCurve yieldCurve = this.InitialYieldCurve(DateTime.Today.ToOADate());
            DenseVector fittingParameters = this.CreateInitialVector(yieldCurve);

            this.eventAggregator.Publish(new YieldCurveEvent(EventType.Update, yieldCurve));
            this.eventAggregator.Publish(new FittingParametersEvent(EventType.Update, fittingParameters));

            // add some prices
            this.UpdatePrices();

            // ok it runs - need to put in some prices to make it fit then cache the jacobians and hessian and show the iteration result matrix
            this.eventAggregator.Publish(new FittingEvent(EventType.Update));

            this.logger.DebugFormat("SolverResult : {0}{1}", Environment.NewLine, this.solverResult.ToDisplayString());
            this.logger.DebugFormat(
                "Iterations : {0}{1}", 
                Environment.NewLine, 
                this.solverResult.IterationResults.ToDisplayString());

            this.CheckPriceChange(yieldCurve);
            this.CheckBondChange(yieldCurve);
        }

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
        ///     The update prices.
        /// </summary>
        public void UpdatePrices()
        {
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EI450338 Corp", 101.65));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EI778872 Corp", 102.545));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EH887630 Corp", 112.3));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "ED860812 Corp", 118.145));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EC453273 Corp", 131.575));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EK333934 Corp", 123.855));
            this.eventAggregator.Publish(new PriceEvent(EventType.Update, "EF396423 Corp", 142.425));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create initial vector.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        /// <returns>
        /// The <see cref="DenseVector"/>.
        /// </returns>
        private DenseVector CreateInitialVector(IYieldCurve yieldCurve)
        {
            IList<double> yields = yieldCurve.Yields;
            int yieldCurveSize = yields.Count;
            int initialVectorSize = yieldCurveSize;

            var denseVector = new DenseVector(initialVectorSize);

            for (int i = 0; i < yields.Count; i++)
            {
                denseVector[i] = 1.0;
            }

            return denseVector;
        }

        #endregion
    }
}