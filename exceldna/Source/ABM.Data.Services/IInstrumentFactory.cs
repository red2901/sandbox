// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentFactory.cs" company="">
//   
// </copyright>
// <summary>
//   The InstrumentFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services
{
    using System.Collections.Generic;

    using ABM.Model;

    /// <summary>
    ///     The InstrumentFactory interface.
    /// </summary>
    public interface IInstrumentFactory
    {
        #region Public Methods and Operators

        bool BuildAndPublish<T>(string key);

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="requestKey">
        /// The request key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Build<T>(string requestKey);

        /// <summary>
        /// The create background bond object collection.
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
        /// The <see cref="FittedBondCollection"/>.
        /// </returns>
        IFittedBondCollection CreateBackgroundBondObjectCollection(
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

        /// <summary>
        /// The get ask price.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double GetAskPrice(string ticker);

        /// <summary>
        /// The get bid ask price.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        void GetBidAskPrice(Bond bond);

        /// <summary>
        /// The get bid price.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double GetBidPrice(string ticker);

        /// <summary>
        /// The get bond.
        /// </summary>
        /// <param name="requestTicker">
        /// The requestTicker.
        /// </param>
        /// <returns>
        /// The <see cref="ABM.Model.Bond"/>.
        /// </returns>
        Bond GetBond(string requestTicker);

        /// <summary>
        /// The update bond market data.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        void GetBondCalculationData(Bond bond);

        /// <summary>
        /// The update bond market data.
        /// </summary>
        /// <param name="bonds">
        /// The bonds.
        /// </param>
        /// <param name="bidCollection">
        /// The bid Collection.
        /// </param>
        /// <param name="askCollection">
        /// The ask Collection.
        /// </param>
        void GetBondCalculationData(
            IBondCollection bonds, 
            IPriceCollection bidCollection, 
            IPriceCollection askCollection);

        /// <summary>
        /// The create bonds.
        /// </summary>
        /// <param name="requestTickers">
        /// The requestTickers.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IList<Bond> GetBonds(IEnumerable<string> requestTickers);

        /// <summary>
        /// The get market yields.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        void GetMarketYields(Bond bond);

        /// <summary>
        /// The get model yield.
        /// </summary>
        /// <param name="bonds">
        /// The bonds.
        /// </param>
        void GetModelYield(IBondCollection bonds);

        #endregion
    }
}