// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommodityFuture.cs" company="">
//   
// </copyright>
// <summary>
//   The commodity future.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections.Generic;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;

    using ABM.Common;

    /// <summary>
    ///     The commodity future.
    /// </summary>
    public class CommodityFuture : IFittableObject, IEventAggregatorHandler, IRequestKey
    {
        #region Fields

        /// <summary>
        ///     The event aggregator.
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        ///     The logger.
        /// </summary>
        private ILog logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommodityFuture"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CommodityFuture(IEventAggregator eventAggregator, ILog logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;

            this.AsOf = DateTime.Today.ToOADate();
            this.Ask = -99999.0;
            this.Bid = -99999.0;
            this.ModelPrice = 0;
            this.OpenInterest = 0;
            this.Weight = 1.0;

            this.InitialiseSubscriptions();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the as of.
        /// </summary>
        public double AsOf { get; set; }

        /// <summary>
        ///     Gets or sets the ask.
        /// </summary>
        public double Ask { get; set; }

        /// <summary>
        ///     Gets or sets the bid.
        /// </summary>
        public double Bid { get; set; }

        /// <summary>
        ///     Gets or sets the last trading date.
        /// </summary>
        public double LastTradingDate { get; set; }

        /// <summary>
        ///     Gets or sets the model price.
        /// </summary>
        public double ModelPrice { get; set; }

        /// <summary>
        ///     Gets or sets the open interest.
        /// </summary>
        public double OpenInterest { get; set; }

        /// <summary>
        ///     Gets or sets the request key.
        /// </summary>
        public string RequestKey { get; set; }

        /// <summary>
        ///     Gets the subscriptions.
        /// </summary>
        public IList<IDisposable> Subscriptions { get; private set; }

        /// <summary>
        ///     Gets or sets the weight.
        /// </summary>
        public double Weight { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Vector<double> Gradient(Vector<double> parametersInitial)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The has fittable values.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool HasFittableValues()
        {
            if (Math.Abs(this.Ask - (-99999.0)) < 0.0001 || Math.Abs(this.Bid - (-99999.0)) < 0.0001)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The imply weight from open interest.
        /// </summary>
        /// <param name="floor">
        /// The floor.
        /// </param>
        public void ImplyWeightFromOpenInterest(double floor)
        {
            this.Weight = this.OpenInterest > floor ? Math.Log(this.OpenInterest) : Math.Log(floor);
        }

        /// <summary>
        ///     The initialise subscriptions.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void InitialiseSubscriptions()
        {
            // subscribe to some events here
        }

        /// <summary>
        /// The market price.
        /// </summary>
        /// <param name="price">
        /// The price.
        /// </param>
        public void MarketPrice(double price)
        {
            this.Ask = price;
            this.Bid = price;
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public double ObjectiveValue(Vector<double> parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}