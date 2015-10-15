// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBond.cs" company="">
//   
// </copyright>
// <summary>
//   The Bond interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;

    using MathNet.Numerics.LinearAlgebra;

    using ABM.Common;

    /// <summary>
    ///     The Bond interface.
    /// </summary>
    public interface IBond : IFittableObject, IRequestKey, IMergable<IBond>, IEventAggregatorHandler
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the amount outstanding.
        /// </summary>
        double AccruedInterest { get; set; }

        /// <summary>
        ///     Gets or sets the amount outstanding.
        /// </summary>
        double AmountOutstanding { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether benchmark.
        /// </summary>
        bool Benchmark { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether ctd.
        /// </summary>
        bool CTD { get; set; }

        /// <summary>
        ///     Gets or sets the coupon.
        /// </summary>
        double Coupon { get; set; }

        /// <summary>
        ///     Gets or sets the days to settle.
        /// </summary>
        int DaysToSettle { get; set; }

        /// <summary>
        ///     Gets or sets the issue date.
        /// </summary>
        DateTime IssueDate { get; set; }

        /// <summary>
        ///     Gets or sets the maturity.
        /// </summary>
        DateTime Maturity { get; set; }

        /// <summary>
        ///     Gets or sets the settlement date.
        /// </summary>
        DateTime SettlementDate { get; set; }

        /// <summary>
        ///     Gets or sets the short name.
        /// </summary>
        string ShortName { get; set; }

        /// <summary>
        ///     Gets or sets the ticker.
        /// </summary>
        string Ticker { get; set; }

        /// <summary>
        ///     Gets or sets the unique id string.
        /// </summary>
        string UniqueIdString { get; set; }

        /// <summary>
        /// Gets or sets the yield ask.
        /// </summary>
        double YieldAsk { get; set; }

        /// <summary>
        /// Gets or sets the yield bid.
        /// </summary>
        double YieldBid { get; set; }

        /// <summary>
        /// Gets or sets the yield mid.
        /// </summary>
        double YieldMid { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The cash flows.
        /// </summary>
        /// <returns>
        ///     The <see cref="CashFlowStream" />.
        /// </returns>
        ICashFlowStream CashFlows();

        /// <summary>
        ///     The coefficients.
        /// </summary>
        /// <returns>
        ///     The <see cref="BondRegressionCoefficients" />.
        /// </returns>
        IBondRegressionCoefficients Coefficients();

        /// <summary>
        /// The regression coefficients.
        /// </summary>
        /// <param name="bondRegressionCoefficients">
        /// The bond regression coefficients.
        /// </param>
        void SetRegressionCoefficients(IBondRegressionCoefficients bondRegressionCoefficients);

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string ToString();

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ToString(bool details);

        #endregion
    }
}