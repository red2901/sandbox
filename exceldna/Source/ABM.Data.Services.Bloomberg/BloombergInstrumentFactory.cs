// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BloombergInstrumentFactory.cs" company="">
//   
// </copyright>
// <summary>
//   The bloomberg instrument factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services.Bloomberg
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Bloomberglp.Blpapi;

    using log4net;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Model;

    /// <summary>
    ///     The bloomberg instrument factory.
    /// </summary>
    public class BloombergInstrumentFactory : IInstrumentFactory
    {
        #region Fields

        /// <summary>
        ///     The bloomberg service.
        /// </summary>
        private readonly IBloombergService bloombergService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        ///     The object manager service.
        /// </summary>
        private readonly IObjectManagerService objectManagerService;

        /// <summary>
        ///     The request lock.
        /// </summary>
        private readonly object requestLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BloombergInstrumentFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="bloombergService">
        /// The bloomberg service.
        /// </param>
        /// <param name="objectManagerService">
        /// The object Manager Service.
        /// </param>
        public BloombergInstrumentFactory(
            ILog logger, 
            IBloombergService bloombergService, 
            IObjectManagerService objectManagerService)
        {
            this.logger = logger;

            this.bloombergService = bloombergService;
            this.objectManagerService = objectManagerService;
        }

        #endregion

        #region Public Methods and Operators

        public bool BuildAndPublish<T>(string key)
        {
            throw new NotImplementedException();
        }

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
        /// <exception cref="Exception">
        /// </exception>
        public T Build<T>(string requestKey)
        {
            if (typeof(T) == typeof(IBond))
            {
                // TODO move this into unity
                var newBond = ServiceLocator.Current.GetInstance<IBond>();
                var currentBond = this.BuildBond(requestKey);
                newBond.Merge(currentBond);
                return (T)newBond;
            }

            throw new Exception("Unknown Instrument Type");
        }

        /// <summary>
        /// The create background bond object collection.
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
        /// <param name="extraParameters">
        /// The extra parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IFittedBondCollection"/>.
        /// </returns>
        public IFittedBondCollection CreateBackgroundBondObjectCollection(
            IList<string> tickerStringList, 
            IList<double> benchMarkList, 
            IList<double> ctdList, 
            IList<double> weightList, 
            IList<double> bidPriceList, 
            IList<double> askPriceList, 
            object asof, 
            bool calculateYields, 
            IList<double> anchorDates, 
            IList<bool> extraParameters)
        {
            var backgoundBondCollection = ServiceLocator.Current.GetInstance<IFittedBondCollection>();
            backgoundBondCollection.Initialise(
                tickerStringList, 
                benchMarkList, 
                ctdList, 
                weightList, 
                bidPriceList, 
                askPriceList, 
                asof, 
                calculateYields, 
                anchorDates, 
                extraParameters);
            return backgoundBondCollection;
        }

        /// <summary>
        /// The get ask price.
        /// </summary>
        /// <param name="requestTicker">
        /// The request ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetAskPrice(string requestTicker)
        {
            double result;
            lock (this.requestLock)
            {
                var bidAskRequestData = new Dictionary<string, Bond>();
                this.SendReferenceDataRequest(
                    new List<string> { requestTicker }, 
                    new List<string> { Fields.PX_ASK }, 
                    null);
                this.GetResults(bidAskRequestData, BondFields.Map);
                result = bidAskRequestData.First().Value.Ask;
            }

            return result;
        }

        /// <summary>
        /// The get bid ask price.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public void GetBidAskPrice(Bond bond)
        {
            var priceRequestData = new Dictionary<string, Bond>();

            this.SendReferenceDataRequest(
                new List<string> { bond.RequestKey }, 
                new List<string> { Fields.PX_BID, Fields.PX_ASK }, 
                null);
            this.GetResults(priceRequestData, BondFields.Map);
            bond.Bid = priceRequestData.First().Value.Bid;
            bond.Ask = priceRequestData.First().Value.Ask;
        }

        /// <summary>
        /// The get bid price.
        /// </summary>
        /// <param name="requestTicker">
        /// The request ticker.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetBidPrice(string requestTicker)
        {
            double result;
            lock (this.requestLock)
            {
                var bidAskRequestData = new Dictionary<string, Bond>();
                this.SendReferenceDataRequest(
                    new List<string> { requestTicker }, 
                    new List<string> { Fields.PX_BID }, 
                    null);
                this.GetResults(bidAskRequestData, BondFields.Map);
                result = bidAskRequestData.First().Value.Bid;
            }

            return result;
        }

        /// <summary>
        /// The get bond.
        /// </summary>
        /// <param name="requestTicker">
        /// The request ticker.
        /// </param>
        /// <returns>
        /// The <see cref="Bond"/>.
        /// </returns>
        public Bond GetBond(string requestTicker)
        {
            this.logger.DebugFormat("Getting bond {0}", requestTicker);

            IList<Bond> bonds = this.GetBonds(new List<string> { requestTicker });
            return bonds.Count == 1 ? bonds[0] : null;
        }

        /// <summary>
        /// The update bond market data.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public void GetBondCalculationData(Bond bond)
        {
            var bonds = ServiceLocator.Current.GetInstance<IBondCollection>();
            bonds.Add(bond);

            var bidCollection = ServiceLocator.Current.GetInstance<IPriceCollection>();
            var askCollection = ServiceLocator.Current.GetInstance<IPriceCollection>();

            bidCollection.Add(bond.RequestKey, bond.Bid);
            askCollection.Add(bond.RequestKey, bond.Ask);

            this.GetBondCalculationData(bonds, bidCollection, askCollection);
        }

        /// <summary>
        /// The get bond market data.
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
        public void GetBondCalculationData(
            IBondCollection bonds, 
            IPriceCollection bidCollection, 
            IPriceCollection askCollection)
        {
            lock (this.requestLock)
            {
                var bondMarketDataDictionary = new Dictionary<string, Bond>();

                if (!bidCollection.HasValues() && !askCollection.HasValues())
                {
                    // get the data from bloomberg
                    this.SendReferenceDataRequest(bonds.RequestKeys(), BondFields.MarketDataFieldList());
                    this.GetResults(bondMarketDataDictionary, BondFields.Map);
                }

                foreach (Bond bond in bonds)
                {
                    if (bidCollection.HasValues() && askCollection.HasValues())
                    {
                        double ask = askCollection.Get(bond.RequestKey);
                        double bid = bidCollection.Get(bond.RequestKey);
                        bond.Ask = ask;
                        bond.Bid = bid;
                    }
                    else
                    {
                        Bond localbond = null;
                        bondMarketDataDictionary.TryGetValue(bond.RequestKey, out localbond);
                        bond.Ask = localbond.Ask;
                        bond.Bid = localbond.Bid;
                    }

                    var marketAskOverride = new OverrideElement(Fields.PX_DISC_ASK, bond.Ask);
                    var marketBidOverride = new OverrideElement(Fields.PX_DISC_BID, bond.Bid);
                    var settlementDateOverrideElement = new OverrideElement(Fields.SETTLE_DT, bond.SettlementDate);

                    var marketAskOverrides = new List<OverrideElement>
                                                 {
                                                     marketAskOverride, 
                                                     settlementDateOverrideElement
                                                 };
                    var marketBidOverrides = new List<OverrideElement>
                                                 {
                                                     marketBidOverride, 
                                                     settlementDateOverrideElement
                                                 };
                    var marketMidOverrides = new List<OverrideElement>
                                                 {
                                                     marketBidOverride, 
                                                     marketAskOverride, 
                                                     settlementDateOverrideElement
                                                 };

                    var modelAskOverride = new OverrideElement(Fields.PX_ASK, bond.ModelPrice);
                    var modelBidOverride = new OverrideElement(Fields.PX_BID, bond.ModelPrice);
                    var modelMidOverrides = new List<OverrideElement>
                                                {
                                                    modelAskOverride, 
                                                    modelBidOverride, 
                                                    settlementDateOverrideElement
                                                };

                    var requestTicker = new List<string> { bond.RequestKey };
                    var yieldRequestData = new Dictionary<string, Bond>();

                    // market yields
                    yieldRequestData.Clear();
                    this.SendReferenceDataRequest(
                        requestTicker, 
                        new List<string> { Fields.YLD_YTM_ASK }, 
                        marketAskOverrides);
                    this.GetResults(yieldRequestData, BondFields.Map);
                    bond.YieldAsk = yieldRequestData.First().Value.YieldAsk;

                    yieldRequestData.Clear();
                    this.SendReferenceDataRequest(
                        requestTicker, 
                        new List<string> { Fields.YLD_YTM_BID }, 
                        marketBidOverrides);
                    this.GetResults(yieldRequestData, BondFields.Map);
                    bond.YieldBid = yieldRequestData.First().Value.YieldBid;

                    yieldRequestData.Clear();
                    this.SendReferenceDataRequest(
                        requestTicker, 
                        new List<string> { Fields.YLD_YTM_MID }, 
                        marketMidOverrides);
                    this.GetResults(yieldRequestData, BondFields.Map);
                    bond.YieldMid = yieldRequestData.First().Value.YieldMid;

                    // model yield
                    yieldRequestData.Clear();
                    this.SendReferenceDataRequest(
                        requestTicker, 
                        new List<string> { Fields.YLD_YTM_MID }, 
                        modelMidOverrides);
                    this.GetResults(yieldRequestData, BondFields.Map);
                    bond.ModelYieldMid = yieldRequestData.First().Value.YieldMid;
                }
            }
        }

        /// <summary>
        /// The create bonds.
        /// </summary>
        /// <param name="requestTickers">
        /// The tickers.
        /// </param>
        /// <returns>
        /// List of bond objects.
        /// </returns>
        public IList<Bond> GetBonds(IEnumerable<string> requestTickers)
        {
            IList<Bond> result;
            lock (this.requestLock)
            {
                var bonds = new Dictionary<string, Bond>();

                this.SendReferenceDataRequest(requestTickers, BondFields.ReferenceFieldList());
                this.GetResults(bonds, BondFields.Map);

                foreach (Bond bond in bonds.Values.Where(bond => !this.objectManagerService.Contains(bond.RequestKey)))
                {
                    this.objectManagerService.Store(bond.RequestKey, bond);
                }

                result = bonds.Values.ToList();
            }

            return result;
        }

        /// <summary>
        /// The get market yields.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public void GetMarketYields(Bond bond)
        {
            lock (this.requestLock)
            {
                var requestTicker = new List<string> { bond.RequestKey };
                var yieldRequestData = new Dictionary<string, Bond>();

                var marketAskOverride = new OverrideElement(Fields.PX_DISC_ASK, bond.Ask);
                var marketBidOverride = new OverrideElement(Fields.PX_DISC_BID, bond.Bid);
                var settlementDateOverrideElement = new OverrideElement(Fields.SETTLE_DT, bond.SettlementDate);

                var marketAskOverrides = new List<OverrideElement> { marketAskOverride, settlementDateOverrideElement };
                var marketBidOverrides = new List<OverrideElement> { marketBidOverride, settlementDateOverrideElement };
                var marketMidOverrides = new List<OverrideElement>
                                             {
                                                 marketBidOverride, 
                                                 marketAskOverride, 
                                                 settlementDateOverrideElement
                                             };

                yieldRequestData.Clear();
                this.SendReferenceDataRequest(
                    requestTicker, 
                    new List<string> { Fields.YLD_YTM_ASK }, 
                    marketAskOverrides);
                this.GetResults(yieldRequestData, BondFields.Map);
                bond.YieldAsk = yieldRequestData.First().Value.YieldAsk;

                yieldRequestData.Clear();
                this.SendReferenceDataRequest(
                    requestTicker, 
                    new List<string> { Fields.YLD_YTM_BID }, 
                    marketBidOverrides);
                this.GetResults(yieldRequestData, BondFields.Map);
                bond.YieldBid = yieldRequestData.First().Value.YieldBid;

                yieldRequestData.Clear();
                this.SendReferenceDataRequest(
                    requestTicker, 
                    new List<string> { Fields.YLD_YTM_MID }, 
                    marketMidOverrides);
                this.GetResults(yieldRequestData, BondFields.Map);
                bond.YieldMid = yieldRequestData.First().Value.YieldMid;
            }
        }

        /// <summary>
        /// The get model yield.
        /// </summary>
        /// <param name="bonds">
        /// The bonds.
        /// </param>
        public void GetModelYield(IBondCollection bonds)
        {
            lock (this.requestLock)
            {
                foreach (Bond bond in bonds)
                {
                    var settlementDateOverrideElement = new OverrideElement(Fields.SETTLE_DT, bond.SettlementDate);
                    var modelAskOverride = new OverrideElement(Fields.PX_ASK, bond.ModelPrice);
                    var modelBidOverride = new OverrideElement(Fields.PX_BID, bond.ModelPrice);
                    var modelMidOverrides = new List<OverrideElement>
                                                {
                                                    modelAskOverride, 
                                                    modelBidOverride, 
                                                    settlementDateOverrideElement
                                                };

                    var requestTicker = new List<string> { bond.RequestKey };
                    var yieldRequestData = new Dictionary<string, Bond>();

                    yieldRequestData.Clear();
                    this.SendReferenceDataRequest(
                        requestTicker, 
                        new List<string> { Fields.YLD_YTM_MID }, 
                        modelMidOverrides);
                    this.GetResults(yieldRequestData, BondFields.Map);
                    bond.ModelYieldMid = yieldRequestData.First().Value.YieldMid;
                }
            }
        }

        /// <summary>
        /// The send reference data request.
        /// </summary>
        /// <param name="tickers">
        /// The tickers.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="overrides">
        /// The overrides.
        /// </param>
        /// <returns>
        /// The <see cref="Request"/>.
        /// </returns>
        public Request SendReferenceDataRequest(
            IEnumerable<string> tickers, 
            IEnumerable<string> fields, 
            IEnumerable<OverrideElement> overrides = null)
        {
            Request request = this.bloombergService.ReferenceDataService.CreateRequest("ReferenceDataRequest");

            // add the security to the request
            Element securitiesElement = request.GetElement("securities");
            foreach (string ticker in tickers)
            {
                securitiesElement.AppendValue(ticker);
            }

            // set the fields we want
            Element fieldsElement = request.GetElement("fields");
            foreach (string field in fields)
            {
                fieldsElement.AppendValue(field.ToUpper());
            }

            if (overrides != null)
            {
                Element overridesElement = request["overrides"];
                foreach (OverrideElement or in overrides)
                {
                    or.SetOverride(overridesElement);
                }
            }

            // send request
            this.bloombergService.Session.SendRequest(request, null);

            return request;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The build bond.
        /// </summary>
        /// <param name="requestKey">
        /// The request key.
        /// </param>
        /// <returns>
        /// The <see cref="IBond"/>.
        /// </returns>
        private IBond BuildBond(string requestKey)
        {
            return this.GetBond(requestKey);
        }

        /// <summary>
        /// The get results.
        /// </summary>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <param name="Map">
        /// The Map.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private void GetResults<T>(IDictionary<string, T> objects, Action<Element, T> Map)
            where T : IMergable<T>, IRequestKey, new()
        {
            bool done = false;
            while (!done)
            {
                Event eventObj = this.bloombergService.Session.NextEvent();
                switch (eventObj.Type)
                {
                    case Event.EventType.PARTIAL_RESPONSE:
                        this.ProcessResponseEvent(eventObj, objects, Map);
                        break;
                    case Event.EventType.RESPONSE:
                        this.ProcessResponseEvent(eventObj, objects, Map);
                        done = true;
                        break;
                    default:
                        done = this.SessionTerminated(eventObj);
                        break;
                }
            }
        }

        /// <summary>
        /// The handle field exceptions.
        /// </summary>
        /// <param name="fieldExceptions">
        /// The field exceptions.
        /// </param>
        private void HandleFieldExceptions(Element fieldExceptions)
        {
            if (fieldExceptions.NumValues > 0)
            {
                for (int k = 0; k < fieldExceptions.NumValues; ++k)
                {
                    Element fieldException = fieldExceptions.GetValueAsElement(k);
                    this.PrintErrorInfo(
                        fieldException.GetElementAsString(Names.FIELD_ID) + "\t\t", 
                        fieldException.GetElement(Names.ERROR_INFO));
                }
            }
        }

        /// <summary>
        /// The print error info.
        /// </summary>
        /// <param name="leadingStr">
        /// The leading str.
        /// </param>
        /// <param name="errorInfo">
        /// The error info.
        /// </param>
        private void PrintErrorInfo(string leadingStr, Element errorInfo)
        {
            Console.WriteLine(
                leadingStr + errorInfo.GetElementAsString(Names.CATEGORY) + " ("
                + errorInfo.GetElementAsString(Names.MESSAGE) + ")");
        }

        /// <summary>
        /// The process response event.
        /// </summary>
        /// <param name="eventObj">
        /// The event obj.
        /// </param>
        /// <param name="objects">
        /// The objects.
        /// </param>
        /// <param name="Map">
        /// The Map.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private void ProcessResponseEvent<T>(Event eventObj, IDictionary<string, T> objects, Action<Element, T> Map)
            where T : IMergable<T>, IRequestKey, new()
        {
            foreach (Message msg in eventObj)
            {
                if (this.ResponseError(msg))
                {
                    continue;
                }

                Element securities = msg.GetElement(Names.SECURITY_DATA);
                int numSecurities = securities.NumValues;

                for (int i = 0; i < numSecurities; ++i)
                {
                    Element security = securities.GetValueAsElement(i);

                    if (this.SecurityError(security))
                    {
                        continue;
                    }

                    var securityObject = new T();
                    Map(security, securityObject);
                    if (!objects.ContainsKey(securityObject.RequestKey))
                    {
                        // add
                        objects.Add(securityObject.RequestKey, securityObject);
                    }
                    else
                    {
                        // merge
                        T currentObject;
                        objects.TryGetValue(securityObject.RequestKey, out currentObject);
                        currentObject.Merge(securityObject);
                    }

                    this.HandleFieldExceptions(security.GetElement(Names.FIELD_EXCEPTIONS));
                }
            }
        }

        /// <summary>
        /// The response error.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ResponseError(Message msg)
        {
            if (msg.HasElement(Names.RESPONSE_ERROR))
            {
                this.PrintErrorInfo("REQUEST FAILED: ", msg.GetElement(Names.RESPONSE_ERROR));
                return true;
            }

            return false;
        }

        /// <summary>
        /// The security error.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SecurityError(Element security)
        {
            if (security.HasElement("securityError"))
            {
                this.PrintErrorInfo("\tSECURITY FAILED: ", security.GetElement(Names.SECURITY_ERROR));
                return true;
            }

            return false;
        }

        /// <summary>
        /// The session terminated.
        /// </summary>
        /// <param name="eventObj">
        /// The event obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SessionTerminated(Event eventObj)
        {
            bool done = false;
            foreach (Message msg in eventObj)
            {
                if (eventObj.Type == Event.EventType.SESSION_STATUS)
                {
                    if (msg.MessageType.Equals("SessionTerminated"))
                    {
                        done = true;
                    }
                }
            }

            return done;
        }

        #endregion
    }
}