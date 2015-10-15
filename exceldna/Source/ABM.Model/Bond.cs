// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bond.cs" company="">
//   
// </copyright>
// <summary>
//   The bond.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Reactive.Linq;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Analytics;
    using ABM.Common;
    using ABM.Model.Events;

    /// <summary>
    ///     The bond.
    /// </summary>
    public class Bond : IBond, IMergable<Bond>
    {
        #region Fields

        /// <summary>
        ///     The cash flows.
        /// </summary>
        private readonly ICashFlowStream cashFlows;

        /// <summary>
        ///     The event aggregator.
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        ///     The coefficients.
        /// </summary>
        private IBondRegressionCoefficients coefficients;

        /// <summary>
        ///     The yield curve.
        /// </summary>
        private IYieldCurve yieldCurve;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bond" /> class.
        /// </summary>
        public Bond()
        {
            this.cashFlows = new CashFlowStream();
            this.coefficients = null;
            this.YieldInputsChanged = false;

            this.AsOf = DateTime.Today.ToOADate();
            this.Ask = -99999.0;
            this.Bid = -99999.0;
            this.ModelPrice = 0;
            this.Weight = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bond"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="cashFlowStream">
        /// The cash flow stream.
        /// </param>
        public Bond(IEventAggregator eventAggregator, ILog logger, ICashFlowStream cashFlowStream)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.cashFlows = cashFlowStream;

            this.coefficients = null;
            this.YieldInputsChanged = false;
            this.AsOf = DateTime.Today.ToOADate();
            this.Ask = -99999.0;
            this.Bid = -99999.0;
            this.ModelPrice = 0;
            this.Weight = 1.0;

            this.InitialiseSubscriptions();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the amount outstanding.
        /// </summary>
        public double AccruedInterest { get; set; }

        /// <summary>
        ///     Gets or sets the amount outstanding.
        /// </summary>
        public double AmountOutstanding { get; set; }

        /// <summary>
        ///     Gets or sets the as of.
        /// </summary>
        public double AsOf { get; set; }

        /// <summary>
        ///     Gets or sets the ask.
        /// </summary>
        public double Ask { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether benchmark.
        /// </summary>
        public bool Benchmark { get; set; }

        /// <summary>
        ///     Gets or sets the bid.
        /// </summary>
        public double Bid { get; set; }

        /// <summary>
        ///     Gets the bid ask spread.
        /// </summary>
        public double BidAskSpread
        {
            get
            {
                return this.Ask - this.Bid;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether ctd.
        /// </summary>
        public bool CTD { get; set; }

        /// <summary>
        ///     Gets the cheapness.
        /// </summary>
        public double Cheapness
        {
            get
            {
                return (this.ModelYieldMid - this.yieldCurve.Yield(this.Maturity)) * 100.0;
            }
        }

        /// <summary>
        ///     Gets or sets the coupon.
        /// </summary>
        public double Coupon { get; set; }

        /// <summary>
        ///     Gets or sets the days to settle.
        /// </summary>
        public int DaysToSettle { get; set; }

        /// <summary>
        ///     Gets the invoice price ask.
        /// </summary>
        public double InvoicePriceAsk
        {
            get
            {
                return this.Ask + this.AccruedInterest;
            }
        }

        /// <summary>
        ///     Gets the invoice price bid.
        /// </summary>
        public double InvoicePriceBid
        {
            get
            {
                return this.Bid + this.AccruedInterest;
            }
        }

        /// <summary>
        ///     Gets the invoice price mid.
        /// </summary>
        public double InvoicePriceMid
        {
            get
            {
                return (this.InvoicePriceAsk + this.InvoicePriceBid) / 2;
            }
        }

        /// <summary>
        ///     Gets or sets the issue date.
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        ///     Gets the issue date year fraction.
        /// </summary>
        public double IssueDateYearFraction
        {
            get
            {
                return (this.AsOf - this.SettlementDate.ToOADate()) / 365.25;
            }
        }

        /// <summary>
        ///     Gets the market price.
        /// </summary>
        public double MarketPrice
        {
            get
            {
                return this.Mid;
            }
        }

        /// <summary>
        ///     Gets the market to model price spread.
        /// </summary>
        public double MarketToModelPriceSpread
        {
            get
            {
                return this.InvoicePriceMid - this.ModelPrice;
            }
        }

        /// <summary>
        ///     Gets the market to model yield spread.
        /// </summary>
        public double MarketToModelYieldSpread
        {
            get
            {
                return this.YieldMid - this.ModelYieldMid;
            }
        }

        /// <summary>
        ///     Gets or sets the maturity.
        /// </summary>
        public DateTime Maturity { get; set; }

        /// <summary>
        ///     Gets the maturity date year fraction.
        /// </summary>
        public double MaturityDateYearFraction
        {
            get
            {
                return (this.Maturity.ToOADate() - this.AsOf) / 365.25;
            }
        }

        /// <summary>
        ///     Gets or sets the mid.
        /// </summary>
        public double Mid 
        {
            get
            {
                return (this.Ask + this.Bid) / 2;
            }
        }

        /// <summary>
        ///     Gets or sets the model clean price mid.
        /// </summary>
        public double ModelPrice { get; set; }

        /// <summary>
        ///     Gets or sets the model yield mid.
        /// </summary>
        public double ModelYieldMid { get; set; }

        /// <summary>
        ///     Gets or sets the RequestKey.
        /// </summary>
        public string RequestKey { get; set; }

        /// <summary>
        ///     Gets or sets the residual weight.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        ///     Gets or sets the settlement date.
        /// </summary>
        public DateTime SettlementDate { get; set; }

        /// <summary>
        ///     Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     Gets the subscriptions.
        /// </summary>
        public IList<IDisposable> Subscriptions { get; private set; }

        /// <summary>
        ///     Gets or sets the ticker.
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        ///     Gets or sets the unique id string.
        /// </summary>
        public string UniqueIdString { get; set; }

        /// <summary>
        ///     Gets or sets the yield ask.
        /// </summary>
        public double YieldAsk { get; set; }

        /// <summary>
        ///     Gets or sets the yield bid.
        /// </summary>
        public double YieldBid { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether yield inputs changed.
        /// </summary>
        public bool YieldInputsChanged { get; set; }

        /// <summary>
        ///     Gets or sets the yield mid.
        /// </summary>
        public double YieldMid { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The calculate model clean price mid.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double CalculateModelCleanPriceMid(Vector<double> parameters)
        {
            this.SetParameters(parameters);
            IBondRegressionCoefficients coeffs = this.Coefficients();

            if (coeffs == null)
            {
                return (this.NetPresentValue(parameters) / 10000) - this.AccruedInterest;
            }

            return (this.NetPresentValue(parameters) / 10000) - this.AccruedInterest
                   + ((this.AmountOutstanding / 1000000000) * coeffs.AmountOutstanding)
                   + (this.IssueDateYearFraction * coeffs.IssueDateYearFraction)
                   + (Convert.ToDouble(this.Benchmark) * coeffs.Benchmark) + (Convert.ToDouble(this.CTD) * coeffs.Ctd)
                   + (this.BidAskSpread * coeffs.BidAskSpread);
        }

        /// <summary>
        ///     The cash flows.
        /// </summary>
        /// <returns>
        ///     The <see cref="CashFlowStream" />.
        /// </returns>
        public ICashFlowStream CashFlows()
        {
            return this.cashFlows;
        }

        /// <summary>
        ///     The coefficients.
        /// </summary>
        /// <returns>
        ///     The <see cref="BondRegressionCoefficients" />.
        /// </returns>
        public IBondRegressionCoefficients Coefficients()
        {
            return this.coefficients;
        }

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
        /// The gradient.
        /// </summary>
        /// <param name="parametersInitial">
        /// The parameters initial.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        public Vector<double> Gradient(Vector<double> parametersInitial)
        {
            return NumericalDerivative.Forward(this.CalculateModelCleanPriceMid, parametersInitial);
        }

        /// <summary>
        /// The has prices.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasFittableValues()
        {
            if (Math.Abs(this.Ask - (-99999.0)) < 0.0001 || Math.Abs(this.Bid - (-99999.0)) < 0.0001)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     The initialise subscriptions.
        /// </summary>
        public void InitialiseSubscriptions()
        {
            this.Subscriptions = new List<IDisposable>
                                     {
                                         this.eventAggregator.GetEvent<YieldCurveEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(ev => this.SetYieldCurve(ev.YieldCurve)), 
                                     };
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        public void Merge(IBond reference)
        {
            this.Merge((Bond)reference);
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        public void Merge(Bond reference)
        {
            if (!this.AmountOutstanding.Equals(reference.AmountOutstanding))
            {
                this.AmountOutstanding = reference.AmountOutstanding;
            }

            if (!this.AccruedInterest.Equals(reference.AccruedInterest))
            {
                this.AccruedInterest = reference.AccruedInterest;
            }

            if (!this.Coupon.Equals(reference.Coupon))
            {
                this.Coupon = reference.Coupon;
            }

            if (!this.DaysToSettle.Equals(reference.DaysToSettle))
            {
                this.DaysToSettle = reference.DaysToSettle;
            }

            if (this.RequestKey == null || !this.RequestKey.Equals(reference.RequestKey))
            {
                this.RequestKey = reference.RequestKey;
            }

            if (this.ShortName == null || !this.ShortName.Equals(reference.ShortName))
            {
                this.ShortName = reference.ShortName;
            }

            if (this.Ticker == null || !this.Ticker.Equals(reference.Ticker))
            {
                this.Ticker = reference.Ticker;
            }

            if (!this.Maturity.Equals(reference.Maturity))
            {
                this.Maturity = reference.Maturity;
            }

            if (!this.IssueDate.Equals(reference.IssueDate))
            {
                this.IssueDate = reference.IssueDate;
            }

            if (!this.SettlementDate.Equals(reference.SettlementDate))
            {
                this.SettlementDate = reference.SettlementDate;
            }

            // cashflow stream copying
            this.CashFlows().Clear();

            foreach (CashFlow cashflow in reference.CashFlows())
            {
                this.CashFlows().Add(cashflow);
            }
        }

        /// <summary>
        ///     The net present value.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double NetPresentValue()
        {
            return this.CashFlows()
                .Sum(cashFlow => cashFlow.NetAmount() * this.yieldCurve.DiscountFactor(cashFlow.Date));
        }

        /// <summary>
        /// The net present value.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double NetPresentValue(Vector<double> parameters)
        {
            var currentYieldCurve = ServiceLocator.Current.GetInstance<IYieldCurve>(YieldCurveType.Yield.ToString());
            currentYieldCurve.CurveDates = this.yieldCurve.CurveDates;
            currentYieldCurve.Yields = parameters;

            return
                this.CashFlows().Sum(cashFlow => cashFlow.NetAmount() * currentYieldCurve.DiscountFactor(cashFlow.Date));
        }

        /// <summary>
        /// The objective value.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double ObjectiveValue(Vector<double> parameters)
        {
            double y = this.CalculateModelCleanPriceMid(parameters) - this.Mid;
            return y * this.Weight;
        }

        /// <summary>
        /// The set parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        public Vector<double> SetParameters(Vector<double> parameters)
        {
            Vector<double> yieldCurve;
            int yieldCurveLength = this.YieldCurve().Length();

            if (parameters.Count == yieldCurveLength)
            {
                yieldCurve = parameters;
            }
            else
            {
                yieldCurve = new DenseVector(yieldCurveLength);
                parameters.CopySubVectorTo(yieldCurve, 0, 0, yieldCurveLength);
                this.coefficients.Update(parameters, yieldCurveLength);
            }

            return yieldCurve;
        }

        /// <summary>
        /// The regression coefficients.
        /// </summary>
        /// <param name="bondRegressionCoefficients">
        /// The bond regression coefficients.
        /// </param>
        public void SetRegressionCoefficients(IBondRegressionCoefficients bondRegressionCoefficients)
        {
            this.coefficients = bondRegressionCoefficients;
        }

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="yc">
        /// The yc.
        /// </param>
        public void SetYieldCurve(IYieldCurve yc)
        {
            this.logger.DebugFormat("Setting yield curve in {0}", this.ShortName);
            this.yieldCurve = yc;
        }

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="parametersNew">
        /// The parameters new.
        /// </param>
        public void SetYieldCurve(Vector<double> parametersNew)
        {
            this.logger.DebugFormat("Setting yield curve with new parameters in {0}", this.ShortName);
            this.yieldCurve.UpdateYields(parametersNew.ToArray());
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return this.RequestKey;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToString(bool details)
        {
            string msg = string.Empty;
            if (details)
            {
                msg = string.Format(
                    "{0} : T={1}, T={2}, P={3}, P+AI={4}, Y={5}, NPV={6}, P(ASW)={7}", 
                    this.ShortName, 
                    this.Maturity.ToOADate(), 
                    this.MaturityDateYearFraction, 
                    this.Mid, 
                    this.InvoicePriceMid, 
                    this.YieldMid, 
                    this.ModelPrice, 
                    this.MarketToModelPriceSpread);
            }
            else
            {
                msg = string.Format(
                    "{0}={1},{2},{3},{4},{5},{6},{7}", 
                    this.ShortName, 
                    this.Maturity.ToOADate(), 
                    this.MaturityDateYearFraction, 
                    this.Mid, 
                    this.InvoicePriceMid, 
                    this.YieldMid, 
                    this.ModelPrice, 
                    this.MarketToModelPriceSpread);
            }

            return msg;
        }

        /// <summary>
        ///     The yield curve.
        /// </summary>
        /// <returns>
        ///     The <see cref="IYieldCurve" />.
        /// </returns>
        public IYieldCurve YieldCurve()
        {
            return this.yieldCurve;
        }

        #endregion
    }
}