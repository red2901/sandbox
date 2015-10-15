// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FittedBondCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The background bond collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Analytics;
    using ABM.Analytics.Solvers;
    using ABM.Common;
    using ABM.Model;

    /// <summary>
    ///     The background bond collection.
    /// </summary>
    public class FittedBondCollection : IFittedBondCollection
    {
        #region Fields

        /// <summary>
        ///     The bond collection.
        /// </summary>
        private readonly IBondCollection bondCollection;

        /// <summary>
        ///     The collectionlock.
        /// </summary>
        private readonly object collectionlock = new object();

        /// <summary>
        ///     The fitted coeffs lock.
        /// </summary>
        private readonly object fittedCoeffsLock = new object();

        /// <summary>
        ///     The fitted yield curve lock.
        /// </summary>
        private readonly object fittedYieldCurveLock = new object();

        /// <summary>
        ///     The last fit length lock.
        /// </summary>
        private readonly object lastFitLengthLock = new object();

        /// <summary>
        ///     The last fit time lock.
        /// </summary>
        private readonly object lastFitTimeLock = new object();

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        ///     The solver result.
        /// </summary>
        private readonly ISolverResult solverResult;

        /// <summary>
        ///     The solver result lock.
        /// </summary>
        private readonly object solverResultLock = new object();

        /// <summary>
        ///     The status message lock.
        /// </summary>
        private readonly object statusMessageLock = new object();

        /// <summary>
        ///     The update queue.
        /// </summary>
        private readonly ConcurrentQueue<object> updateQueue;

        /// <summary>
        ///     The anchor dates.
        /// </summary>
        private IList<double> anchorDates;

        /// <summary>
        ///     The ask price list.
        /// </summary>
        private IList<double> askPriceList;

        /// <summary>
        ///     The asof.
        /// </summary>
        private object asof;

        /// <summary>
        ///     The bench mark list.
        /// </summary>
        private IList<double> benchMarkList;

        /// <summary>
        ///     The bid price list.
        /// </summary>
        private IList<double> bidPriceList;

        /// <summary>
        ///     The calculate yields.
        /// </summary>
        private bool calculateYields;

        /// <summary>
        ///     The extra parameters.
        /// </summary>
        private IList<bool> coeffFlags;

        /// <summary>
        ///     The ctd list.
        /// </summary>
        private IList<double> ctdList;

        /// <summary>
        ///     The endfitloop.
        /// </summary>
        private bool endfitloop;

        /// <summary>
        ///     The fitted coefficients.
        /// </summary>
        private IBondRegressionCoefficients fittedCoefficients;

        /// <summary>
        ///     The fitted yield curve.
        /// </summary>
        private IYieldCurve fittedYieldCurve;

        /// <summary>
        ///     The fitting thread.
        /// </summary>
        private Thread fittingThread;

        /// <summary>
        ///     The haschanged.
        /// </summary>
        private bool haschanged;

        /// <summary>
        ///     The initialised.
        /// </summary>
        private bool initialised;

        /// <summary>
        ///     The last fit length.
        /// </summary>
        private double lastFitLength;

        /// <summary>
        ///     The last fit time.
        /// </summary>
        private DateTime lastFitTime;

        /// <summary>
        ///     The solver result available.
        /// </summary>
        private bool solverResultAvailable;

        /// <summary>
        ///     The status message.
        /// </summary>
        private string statusMessage;

        /// <summary>
        ///     The stopped.
        /// </summary>
        private bool stopped;

        /// <summary>
        ///     The ticker string list.
        /// </summary>
        private IList<string> tickerStringList;

        /// <summary>
        ///     The weight list.
        /// </summary>
        private IList<double> weightList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FittedBondCollection"/> class.
        /// </summary>
        /// <param name="loghandle">
        /// The loghandle.
        /// </param>
        /// <param name="bondCollection">
        /// The bond collection.
        /// </param>
        /// <param name="solverResult">
        /// The solver result.
        /// </param>
        public FittedBondCollection(ILog loghandle, IBondCollection bondCollection, ISolverResult solverResult)
        {
            this.logger = loghandle;
            this.bondCollection = bondCollection;
            this.solverResult = solverResult;
            this.endfitloop = false;
            this.stopped = false;
            this.haschanged = true;
            this.initialised = false;
            this.lastFitTime = DateTime.Today;
            this.lastFitLength = 0.0;
            this.solverResultAvailable = false;

            this.updateQueue = new ConcurrentQueue<object>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The cost.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double Cost
        {
            get
            {
                double cost = 0.0;
                lock (this.solverResultLock)
                {
                    cost = this.solverResult.ValueNew;
                }

                return cost;
            }
        }

        /// <summary>
        ///     The evals.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Evals
        {
            get
            {
                int evals = 0;
                lock (this.solverResultLock)
                {
                    evals = this.solverResult.IterationResults.Count;
                }

                return evals;
            }
        }

        /// <summary>
        ///     The fitted parameters.
        /// </summary>
        /// <returns>
        ///     The <see cref="Vector" />.
        /// </returns>
        public IList<double> FittedAnchorYields
        {
            get
            {
                IList<double> result;
                lock (this.fittedYieldCurveLock)
                {
                    result = this.fittedYieldCurve.Yields;
                }

                return result;
            }
        }

        /// <summary>
        ///     Gets or sets the last error message.
        /// </summary>
        public string LastErrorMessage { get; set; }

        /// <summary>
        ///     The last fit length.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double LastFitLength
        {
            get
            {
                double length = 0;
                lock (this.lastFitLengthLock)
                {
                    length = this.lastFitLength;
                }

                return length;
            }
        }

        /// <summary>
        ///     The last fit time.
        /// </summary>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public DateTime LastFitTime
        {
            get
            {
                DateTime retdt = DateTime.Today;
                lock (this.lastFitTimeLock)
                {
                    retdt = this.lastFitTime;
                }

                return retdt;
            }
        }

        /// <summary>
        ///     Gets or sets the n fit loop.
        /// </summary>
        public int NFitLoops { get; set; }

        /// <summary>
        ///     Gets the size.
        /// </summary>
        public int Size
        {
            get
            {
                return this.bondCollection.Count;
            }
        }

        /// <summary>
        ///     The status.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string Status
        {
            get
            {
                string status = string.Empty;

                if (this.solverResultAvailable)
                {
                    lock (this.solverResultLock)
                    {
                        status = this.solverResult.Status.ToString();
                    }
                }
                else
                {
                    status = this.StatusMessage;
                }

                return status;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the status message.
        /// </summary>
        private string StatusMessage
        {
            get
            {
                string result;
                lock (this.statusMessageLock)
                {
                    result = this.statusMessage;
                }

                return result;
            }

            set
            {
                lock (this.statusMessageLock)
                {
                    this.statusMessage = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public void Add(Bond bond)
        {
            lock (this.collectionlock)
            {
                this.bondCollection.Add(bond);
            }
        }

        /// <summary>
        ///     The amount outstanding in billions.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> AmountOutstandingInBillions()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.AmountOutstanding / 1000000000.0);
            }

            return result;
        }

        /// <summary>
        ///     The bid ask spread.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> BidAskSpread()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.BidAskSpread);
            }

            return result;
        }

        /// <summary>
        ///     The cheapness.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> Cheapness()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.Cheapness);
            }

            return result;
        }

        /// <summary>
        ///     The coeffs.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> Coeffs()
        {
            var result = new List<double>(this.fittedCoefficients.Count);

            lock (this.fittedCoeffsLock)
            {
                result.AddRange(this.fittedCoefficients.ToList());
            }

            return result;
        }

        /// <summary>
        ///     The fit.
        /// </summary>
        public void Fit()
        {
            var sw = new Stopwatch();
            sw.Start();

            // we should make these both injected
            var solverOptions = new SolverOptions();
            lock (this.solverResultLock)
            {
                this.solverResult.Clear();
            }

            var solver = new LevenbergMarquardt(this.logger, solverOptions, this.solverResult);

            // initial vector
            DenseVector initialVector;
            lock (this.fittedCoeffsLock)
            {
                lock (this.fittedYieldCurveLock)
                {
                    initialVector = this.CreateInitialVector(this.fittedYieldCurve, this.fittedCoefficients);
                }
            }

            if (this.bondCollection.Count < initialVector.Count)
            {
                this.solverResultAvailable = false;
                this.StatusMessage = string.Format(
                    "No enough bonds to find solution {0} < {1}", 
                    this.bondCollection.Count, 
                    initialVector.Count);
                return;
            }

            // perform the fit
            this.StatusMessage = string.Format("Fitting");

            lock (this.collectionlock)
            {
                try
                {
                    solver.Estimate((IObjectiveValueCollection)this.bondCollection, initialVector);

                    // assign back to the bonds
                    this.UpdateParameters(solver.SolverResult);

                    // calculate the relevant yields
                    var dataFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();
                    dataFactory.GetModelYield(this.bondCollection);
                    this.solverResultAvailable = true;
                }
                catch (Exception e)
                {
                    this.solverResultAvailable = false;
                    this.StatusMessage = e.Message;
                }
            }

            lock (this.lastFitTimeLock)
            {
                this.lastFitTime = DateTime.Now;
            }

            lock (this.lastFitLengthLock)
            {
                sw.Stop();
                this.lastFitLength = sw.ElapsedMilliseconds;
            }
        }

        /// <summary>
        ///     The fit loop.
        /// </summary>
        public void FitLoop()
        {
            this.NFitLoops = 0;
            try
            {
                while (!this.endfitloop)
                {
                    if (this.UpdateFromQueue())
                    {
                        this.NFitLoops += 1;
                        this.Fit();
                        this.haschanged = false;
                    }

                    // if (this.haschanged)
                    // {
                    // this.NFitLoops += 1;
                    // this.Fit();
                    // this.haschanged = false;
                    // }
                }
            }
            catch (Exception e)
            {
                this.endfitloop = true;
                this.solverResultAvailable = false;
                this.LastErrorMessage = e.Message;
            }

            this.stopped = true;
        }

        /// <summary>
        ///     The fitted parameters.
        /// </summary>
        /// <returns>
        ///     The <see cref="Vector" />.
        /// </returns>
        public Vector<double> FittedParameters()
        {
            Vector<double> result;
            lock (this.solverResultLock)
            {
                result = this.solverResult.ParametersNew == null ? null : this.solverResult.ParametersNew.Clone();
            }

            return result;
        }

        /// <summary>
        /// The get bond.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="Bond"/>.
        /// </returns>
        public Bond GetBond(string ticker)
        {
            return this.bondCollection.GetBond(ticker);
        }

        /// <summary>
        /// The initialise.
        /// </summary>
        /// <param name="localTickerStringList">
        /// The local ticker string list.
        /// </param>
        /// <param name="localBenchMarkList">
        /// The local bench mark list.
        /// </param>
        /// <param name="localCtdList">
        /// The local ctd list.
        /// </param>
        /// <param name="localWeightList">
        /// The local weight list.
        /// </param>
        /// <param name="localBidPriceList">
        /// The local bid price list.
        /// </param>
        /// <param name="localAskPriceList">
        /// The local ask price list.
        /// </param>
        /// <param name="localasof">
        /// The localasof.
        /// </param>
        /// <param name="localCalculateYields">
        /// The local Calculate Yields.
        /// </param>
        /// <param name="localAnchorDates">
        /// The local anchor dates.
        /// </param>
        /// <param name="localCoeffFlags">
        /// The local Extra Parameters.
        /// </param>
        public void Initialise(
            IList<string> localTickerStringList, 
            IList<double> localBenchMarkList, 
            IList<double> localCtdList, 
            IList<double> localWeightList, 
            IList<double> localBidPriceList, 
            IList<double> localAskPriceList, 
            object localasof, 
            bool localCalculateYields, 
            IList<double> localAnchorDates, 
            IList<bool> localCoeffFlags)
        {
            this.tickerStringList = localTickerStringList;
            this.benchMarkList = localBenchMarkList;
            this.ctdList = localCtdList;
            this.weightList = localWeightList;
            this.bidPriceList = localBidPriceList;
            this.askPriceList = localAskPriceList;
            this.anchorDates = localAnchorDates;
            this.asof = localasof;
            this.calculateYields = localCalculateYields;
            this.coeffFlags = localCoeffFlags;

            this.InitialiseCollection();
        }

        /// <summary>
        /// The initialise extra parameters.
        /// </summary>
        /// <param name="coeffFlags">
        /// The extra parameters.
        /// </param>
        public void InitialiseCoeffFlags(IList<bool> coeffFlags)
        {
            lock (this.fittedCoeffsLock)
            {
                this.fittedCoefficients = new BondRegressionCoefficients(coeffFlags);

                lock (this.collectionlock)
                {
                    this.bondCollection.SetRegressionCoefficients(this.fittedCoefficients);
                }
            }
        }

        /// <summary>
        ///     The initialise collection.
        /// </summary>
        public void InitialiseCollection()
        {
            this.solverResultAvailable = false;
            var dataFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();

            lock (this.collectionlock)
            {
                this.bondCollection.Clear();

                for (int i = 0; i < this.tickerStringList.Count; i++)
                {
                    this.StatusMessage = string.Format("Initialising bond {0} - {1}", i + 1, this.tickerStringList[i]);
                    Bond bond = this.InitliaseBond(
                        this.tickerStringList[i], 
                        Convert.ToDouble(this.asof), 
                        this.askPriceList[i], 
                        this.bidPriceList[i], 
                        Convert.ToBoolean(this.benchMarkList[i]), 
                        Convert.ToBoolean(this.ctdList[i]), 
                        this.weightList[i], 
                        null, 
                        null);
                    if (this.calculateYields)
                    {
                        dataFactory.GetMarketYields(bond);
                    }

                    this.bondCollection.Add(bond);
                }

                // ok now initialise the yield curve
                this.StatusMessage = "Initialising yield curve";
                this.InitialiseYieldCurve(this.anchorDates);

                // ok now initialise the coeffs
                this.StatusMessage = "Initialising coefficients";
                this.InitialiseCoeffFlags(this.coeffFlags);

                this.initialised = true;
            }
        }

        /// <summary>
        /// The initialise yield curve.
        /// </summary>
        /// <param name="anchorDates">
        /// The anchor Dates.
        /// </param>
        public void InitialiseYieldCurve(IList<double> anchorDates)
        {
            var curveDates = new List<double>();
            var curveYields = new List<double>();

            foreach (double dt in anchorDates)
            {
                curveDates.Add(dt);
                curveYields.Add(0.5);
            }

            lock (this.fittedYieldCurveLock)
            {

                this.fittedYieldCurve = ServiceLocator.Current.GetInstance<IYieldCurve>(YieldCurveType.Yield.ToString()); 
                this.fittedYieldCurve.CurveDates = curveDates;
                this.fittedYieldCurve.Yields = curveYields;
            }

            lock (this.collectionlock)
            {
                this.bondCollection.SetYieldCurve(this.fittedYieldCurve);
            }
        }

        /// <summary>
        ///     The maturity.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        public IList<double> Maturity()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.Maturity.ToOADate());
            }

            return result;
        }

        /// <summary>
        ///     The model clean price.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        public IList<double> ModelCleanPrice()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.ModelCleanPriceMid);
            }

            return result;
        }

        /// <summary>
        /// The model clean price.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double ModelCleanPrice(string ticker)
        {
            double result = 0.0;

            lock (this.collectionlock)
            {
                foreach (Bond bond in this.bondCollection)
                {
                    if (bond.RequestKey.Equals(ticker))
                    {
                        result = bond.ModelCleanPriceMid;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     The model yield.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        public IList<double> ModelYield()
        {
            var result = new List<double>(this.bondCollection.Count);

            foreach (Bond bond in this.bondCollection)
            {
                result.Add(bond.ModelYieldMid);
            }

            return result;
        }

        /// <summary>
        ///     The start.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Start()
        {
            // create a thread
            this.fittingThread = new Thread(this.FitLoop);
            this.fittingThread.Start();

            // start the thread
            return true;
        }

        /// <summary>
        ///     The stop.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Stop()
        {
            this.endfitloop = true;

            int tries = 0;
            int maxtries = 1000;
            while (!this.stopped && tries < maxtries)
            {
                Thread.Sleep(1);
                tries += 1;
            }

            this.stopped = false;
            this.endfitloop = false;

            if (tries < maxtries)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="localTickerStringList">
        /// The local ticker string list.
        /// </param>
        /// <param name="localBenchMarkList">
        /// The local bench mark list.
        /// </param>
        /// <param name="localCtdList">
        /// The local ctd list.
        /// </param>
        /// <param name="localWeightList">
        /// The local weight list.
        /// </param>
        /// <param name="localBidPriceList">
        /// The local bid price list.
        /// </param>
        /// <param name="localAskPriceList">
        /// The local ask price list.
        /// </param>
        /// <param name="localasof">
        /// The localasof.
        /// </param>
        /// <param name="localCalculateYields">
        /// The local Calculate Yields.
        /// </param>
        /// <param name="localAnchorDates">
        /// The local anchor dates.
        /// </param>
        /// <param name="localExtraParameters">
        /// The local Extra Parameters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Update(
            IList<string> localTickerStringList, 
            IList<double> localBenchMarkList, 
            IList<double> localCtdList, 
            IList<double> localWeightList, 
            IList<double> localBidPriceList, 
            IList<double> localAskPriceList, 
            object localasof, 
            bool localCalculateYields, 
            IList<double> localAnchorDates, 
            IList<bool> localExtraParameters)
        {
            if (!this.initialised)
            {
                return false;
            }

            var localTickerIndex = new Dictionary<int, string>(localTickerStringList.Count);
            for (int i = 0; i < localTickerStringList.Count; i++)
            {
                localTickerIndex[i] = localTickerStringList[i];
            }

            bool updated = false;

            if (!this.asof.Equals(localasof) || this.tickerStringList.Count != localTickerStringList.Count
                || this.tickerStringList.IsDifferentFromIndexList(localTickerStringList).Count > 0)
            {
                lock (this.collectionlock)
                {
                    // add any new ones
                    for (int i = 0; i < localTickerStringList.Count; i++)
                    {
                        double asof = Convert.ToDouble(localasof);
                        string ticker = localTickerStringList[i];
                        bool benchmark = Convert.ToBoolean(localBenchMarkList[i]);
                        bool ctd = Convert.ToBoolean(localCtdList[i]);
                        double weight = localWeightList[i];
                        double bid = localBidPriceList[i];
                        double ask = localAskPriceList[i];

                        if (!this.bondCollection.ContainsKey(ticker))
                        {
                            Bond bond = this.InitliaseBond(
                                ticker, 
                                asof, 
                                ask, 
                                bid, 
                                benchmark, 
                                ctd, 
                                weight, 
                                this.fittedYieldCurve, 
                                this.fittedCoefficients);
                            this.bondCollection.Add(bond);
                        }
                    }

                    this.bondCollection.Keep(localTickerStringList);
                }

                this.tickerStringList = localTickerStringList;
                this.benchMarkList = localBenchMarkList;
                this.ctdList = localCtdList;
                this.askPriceList = localAskPriceList;
                this.bidPriceList = localBidPriceList;
                this.asof = localasof;
                this.weightList = localWeightList;

                updated = true;
            }

            if (this.anchorDates.Count != localAnchorDates.Count
                || this.anchorDates.IsDifferentFromIndexList(localAnchorDates).Count > 0)
            {
                this.anchorDates = localAnchorDates;
                this.InitialiseYieldCurve(localAnchorDates);
                updated = true;
            }

            IList<int> bidPriceChanges = this.bidPriceList.IsDifferentFromIndexList(localBidPriceList);
            if (bidPriceChanges.Count > 0)
            {
                foreach (int idx in bidPriceChanges)
                {
                    double bid = localBidPriceList[idx];
                    string requestKey = localTickerIndex[idx];
                    this.updateQueue.Enqueue(new BidPrice(requestKey, bid));
                }

                this.bidPriceList = localBidPriceList;
            }

            // IList<int> askPriceChanges = this.askPriceList.IsDifferentFromIndexList(localAskPriceList);
            // if (bidPriceChanges.Count > 0 || askPriceChanges.Count > 0)
            // {
            // IEnumerable<int> indexChanges = bidPriceChanges.Union(askPriceChanges).Distinct();
            // foreach (int idx in indexChanges)
            // {
            // double bid = localBidPriceList[idx];
            // double ask = localAskPriceList[idx];
            // BondCalculations bondCalcs = this.bondCollection.GetBond(localTickerIndex[idx]).Calculations();
            // bondCalcs.Ask = localAskPriceList[idx];
            // bondCalcs.Bid = localBidPriceList[idx];
            // }

            // this.bidPriceList = localBidPriceList;
            // this.askPriceList = localAskPriceList;
            // updated = true;
            // }
            IList<int> benchmarkChanges = this.benchMarkList.IsDifferentFromIndexList(localBenchMarkList);
            if (benchmarkChanges.Count > 0)
            {
                foreach (int idx in benchmarkChanges)
                {
                    this.bondCollection.GetBond(localTickerIndex[idx]).Benchmark =
                        Convert.ToBoolean(localBenchMarkList[idx]);
                }

                this.benchMarkList = localBenchMarkList;
                updated = true;
            }

            IList<int> ctdChanges = this.benchMarkList.IsDifferentFromIndexList(localCtdList);
            if (ctdChanges.Count > 0)
            {
                foreach (int idx in ctdChanges)
                {
                    this.bondCollection.GetBond(localTickerIndex[idx]).CTD = Convert.ToBoolean(localCtdList[idx]);
                }

                this.ctdList = localCtdList;
                updated = true;
            }

            IList<int> weightChanges = this.weightList.IsDifferentFromIndexList(localWeightList);
            if (weightChanges.Count > 0)
            {
                foreach (int idx in weightChanges)
                {
                    this.bondCollection.GetBond(localTickerIndex[idx]).Weight = localWeightList[idx];
                }

                this.weightList = localWeightList;
                updated = true;
            }

            lock (this.fittedCoeffsLock)
            {
                if (this.coeffFlags.HasChanged(localExtraParameters))
                {
                    this.coeffFlags = localExtraParameters;
                    this.fittedCoefficients.Update(localExtraParameters);

                    if (this.fittedCoefficients.BenchmarkOn)
                    {
                        bool nobenchmarks = true;

                        // check that we have some benchmarks set otherwise just return an error
                        foreach (Bond bond in this.bondCollection)
                        {
                            if (bond.Benchmark)
                            {
                                nobenchmarks = false;
                                break;
                            }
                        }

                        if (nobenchmarks)
                        {
                            this.solverResultAvailable = false;
                            this.StatusMessage = "No benchmarks set";
                            return false;
                        }
                    }

                    if (this.fittedCoefficients.CtdOn)
                    {
                        bool noctd = true;

                        // check that we have some benchmarks set otherwise just return an error
                        foreach (Bond bond in this.bondCollection)
                        {
                            if (bond.CTD)
                            {
                                noctd = false;
                                break;
                            }
                        }

                        if (noctd)
                        {
                            this.solverResultAvailable = false;
                            this.StatusMessage = "No CTDs set";
                            return false;
                        }
                    }

                    // reset the yield curve
                    this.InitialiseYieldCurve(this.anchorDates);

                    updated = true;
                }
            }

            this.haschanged = updated;

            return updated;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create initial vector.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        /// <param name="bondRegressionCoefficients">
        /// The bond regression coefficients.
        /// </param>
        /// <returns>
        /// The <see cref="DenseVector"/>.
        /// </returns>
        private DenseVector CreateInitialVector(
            IYieldCurve yieldCurve, 
            IBondRegressionCoefficients bondRegressionCoefficients)
        {
            IList<double> yields = yieldCurve.Yields;
            int yieldCurveSize = yields.Count;
            int initialVectorSize = yieldCurveSize;

            initialVectorSize += bondRegressionCoefficients.Available;
            var denseVector = new DenseVector(initialVectorSize);

            for (int i = 0; i < yields.Count; i++)
            {
                denseVector[i] = 0.5;
            }

            int j = 0;
            for (int i = 0; i < bondRegressionCoefficients.Available; i++)
            {
                denseVector[yieldCurveSize + j] = 0.0001;
            }

            return denseVector;
        }

        /// <summary>
        /// The initliase bond.
        /// </summary>
        /// <param name="requestTicker">
        /// The request ticker.
        /// </param>
        /// <param name="asof">
        /// The asof.
        /// </param>
        /// <param name="ask">
        /// The ask.
        /// </param>
        /// <param name="bid">
        /// The bid.
        /// </param>
        /// <param name="benchmark">
        /// The benchmark.
        /// </param>
        /// <param name="ctd">
        /// The ctd.
        /// </param>
        /// <param name="weight">
        /// The weight.
        /// </param>
        /// <param name="yc">
        /// The yc.
        /// </param>
        /// <param name="bondRegressionCoefficients">
        /// The bond regression coefficients.
        /// </param>
        /// <returns>
        /// The <see cref="Bond"/>.
        /// </returns>
        private Bond InitliaseBond(
            string requestTicker, 
            double asof, 
            double ask, 
            double bid, 
            bool benchmark, 
            bool ctd, 
            double weight, 
            IYieldCurve yc, 
            IBondRegressionCoefficients bondRegressionCoefficients)
        {
            var dataFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();
            Bond bond = dataFactory.GetBond(requestTicker);

            if (this.calculateYields)
            {
                dataFactory.GetMarketYields(bond);
            }

            if (bondRegressionCoefficients != null)
            {
                bond.SetRegressionCoefficients(bondRegressionCoefficients);
            }

            return bond;
        }

        /// <summary>
        /// The update from queue.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool UpdateFromQueue()
        {
            while (!this.updateQueue.IsEmpty)
            {
                bool retry = false;
                object result = null;
                do
                {
                    retry = this.updateQueue.TryDequeue(out result);
                    if (retry)
                    {
                        // wait a milli second
                        Thread.Sleep(1);
                    }
                }
                while (!retry);

                if (result is BidPrice)
                {
                    var bidPrice = (BidPrice)result;
                    this.bondCollection.GetBond(bidPrice.RequestKey).Bid = bidPrice.Value;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// The update parameters.
        /// </summary>
        /// <param name="solverResult">
        /// The solver result.
        /// </param>
        private void UpdateParameters(ISolverResult solverResult)
        {
            lock (this.solverResultLock)
            {
                Vector<double> parameters = solverResult.ParametersNew;
                lock (this.fittedYieldCurveLock)
                {
                    this.fittedYieldCurve.UpdateYields(parameters);
                }

                lock (this.fittedCoeffsLock)
                {
                    this.fittedCoefficients.Update(parameters, this.fittedYieldCurve.Length());
                }
            }
        }

        #endregion
    }
}