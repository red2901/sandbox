// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFittedBondCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The BackgroundFittedBondCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    using System;
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra;

    using ABM.Model;

    /// <summary>
    ///     The BackgroundFittedBondCollection interface.
    /// </summary>
    public interface IFittedBondCollection
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the last error message.
        /// </summary>
        string LastErrorMessage { get; set; }

        /// <summary>
        ///     Gets or sets the n fit loop.
        /// </summary>
        int NFitLoops { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        void Add(Bond bond);

        /// <summary>
        ///     The amount outstanding in billions.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IList<double> AmountOutstandingInBillions();

        /// <summary>
        ///     The bid ask spread.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IList<double> BidAskSpread();

        /// <summary>
        ///     The cheapness.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IList<double> Cheapness();

        /// <summary>
        ///     The coeffs.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IList<double> Coeffs();

        /// <summary>
        ///     The cost.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        double Cost { get; }

        /// <summary>
        /// The count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>

        int Size { get; }

        /// <summary>
        ///     The evals.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int Evals { get; }

        /// <summary>
        ///     The fitted parameters.
        /// </summary>
        /// <returns>
        ///     The <see cref="Vector" />.
        /// </returns>
        Vector<double> FittedParameters();

        /// <summary>
        ///     The fitted yield curve.
        /// </summary>
        /// <returns>
        ///     The <see cref="Vector" />.
        /// </returns>
        IList<double> FittedAnchorYields { get; }

        /// <summary>
        /// The initialise.
        /// </summary>
        /// <param name="tickerStringList">
        /// The ticker string list.
        /// </param>
        /// <param name="benchMarkList">
        /// The bench mark list.
        /// </param>
        /// <param name="ctdList">
        /// The ctd list.
        /// </param>
        /// <param name="weightList">
        /// The weight list.
        /// </param>
        /// <param name="bidPriceList">
        /// The bid price list.
        /// </param>
        /// <param name="askPriceList">
        /// The ask price list.
        /// </param>
        /// <param name="asof">
        /// The asof.
        /// </param>
        /// <param name="calculateYields">
        /// The calculate yields.
        /// </param>
        /// <param name="anchorDates">
        /// The anchor dates.
        /// </param>
        /// <param name="localCoeffFlags">
        /// The extra Parameters.
        /// </param>
        void Initialise(
            IList<string> tickerStringList, 
            IList<double> benchMarkList, 
            IList<double> ctdList, 
            IList<double> weightList, 
            IList<double> bidPriceList, 
            IList<double> askPriceList, 
            object asof, 
            bool calculateYields, 
            IList<double> anchorDates, 
            IList<bool> localCoeffFlags);

        /// <summary>
        /// The initialise extra parameters.
        /// </summary>
        /// <param name="coeffFlags">
        /// The extra parameters.
        /// </param>
        void InitialiseCoeffFlags(IList<bool> coeffFlags);

        /// <summary>
        /// The initialise yield curve.
        /// </summary>
        /// <param name="anchorDays">
        /// The anchor days.
        /// </param>
        void InitialiseYieldCurve(IList<double> anchorDays);

        /// <summary>
        ///     The last fit length.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        double LastFitLength { get; }

        /// <summary>
        ///     The last fit time.
        /// </summary>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        DateTime LastFitTime { get; }

        /// <summary>
        ///     The maturity.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        IList<double> Maturity();

        /// <summary>
        ///     The model clean price.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        IList<double> ModelCleanPrice();

        /// <summary>
        /// The model clean price.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double ModelCleanPrice(string ticker);

        /// <summary>
        ///     The model yield.
        /// </summary>
        /// <returns>
        ///     The <see cref="double[]" />.
        /// </returns>
        IList<double> ModelYield();

        /// <summary>
        ///     The start.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool Start();

        /// <summary>
        ///     The status.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string Status { get; }

        /// <summary>
        ///     The stop.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool Stop();

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="tickerStringList">
        /// The ticker string list.
        /// </param>
        /// <param name="bencchMarkList">
        /// The bencch mark list.
        /// </param>
        /// <param name="ctdList">
        /// The ctd list.
        /// </param>
        /// <param name="weightList">
        /// The weight list.
        /// </param>
        /// <param name="bidPriceList">
        /// The bid price list.
        /// </param>
        /// <param name="askPriceList">
        /// The ask price list.
        /// </param>
        /// <param name="asof">
        /// The asof.
        /// </param>
        /// <param name="calculateYields">
        /// The calculate yields.
        /// </param>
        /// <param name="anchorDates">
        /// The anchor dates.
        /// </param>
        /// <param name="extraParameters">
        /// The extra Parameters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Update(
            IList<string> tickerStringList, 
            IList<double> bencchMarkList, 
            IList<double> ctdList, 
            IList<double> weightList, 
            IList<double> bidPriceList, 
            IList<double> askPriceList, 
            object asof, 
            bool calculateYields, 
            IList<double> anchorDates, 
            IList<bool> extraParameters);

        Bond GetBond(string ticker);

        #endregion
    }
}