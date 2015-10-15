namespace ABM.Data.Services
{
    using System.Collections.Generic;

    using log4net;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Common;
    using ABM.Model;
    using ABM.Model.Events;

    public class DataFileInstrumentFactory : IInstrumentFactory
    {
        private readonly IEventAggregator eventAggregator;

        private readonly ILog logger;

        private readonly IDictionary<string, string[]> dataStore; 

        public DataFileInstrumentFactory(ILog logger, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.logger = logger;

            this.dataStore = new Dictionary<string, string[]>();
        }

        public void Initialise(string dataFileName)
        {
            var dataLines = FileUtil.ReadFileIntoStringList(dataFileName);

            bool header = true;
            foreach (var dataLine in dataLines)
            {
                if (header)
                {
                    // skip
                }
                else
                {
                    var elts = dataLine.Split(',');
                    this.dataStore[elts[0]] = elts;
                }

                // first one is the header
                header = false;
            }
        }

        public bool BuildAndPublish<T>(string key)
        {
            if (this.dataStore.ContainsKey(key))
            {
                if (typeof(T) == typeof(CommodityFuture))
                {
                    var newCommodityFuture = ServiceLocator.Current.GetInstance<CommodityFuture>();

                    this.eventAggregator.Publish(new FittableObjectEvent(EventType.New, newCommodityFuture));
                    return true;
                }
            }

            return false;
        }

        public T Build<T>(string requestKey)
        {
            throw new System.NotImplementedException();
        }

        public IFittedBondCollection CreateBackgroundBondObjectCollection(
            IList<string> tickerStringList,
            IList<double> bencchMarkList,
            IList<double> ctdList,
            IList<double> weightList,
            IList<double> bidPriceList,
            IList<double> askPriceList,
            object asof,
            bool calculateYields,
            IList<double> anchorDates,
            IList<bool> extraParameters)
        {
            throw new System.NotImplementedException();
        }

        public double GetAskPrice(string ticker)
        {
            throw new System.NotImplementedException();
        }

        public void GetBidAskPrice(Bond bond)
        {
            throw new System.NotImplementedException();
        }

        public double GetBidPrice(string ticker)
        {
            throw new System.NotImplementedException();
        }

        public Bond GetBond(string requestTicker)
        {
            throw new System.NotImplementedException();
        }

        public void GetBondCalculationData(Bond bond)
        {
            throw new System.NotImplementedException();
        }

        public void GetBondCalculationData(IBondCollection bonds, IPriceCollection bidCollection, IPriceCollection askCollection)
        {
            throw new System.NotImplementedException();
        }

        public IList<Bond> GetBonds(IEnumerable<string> requestTickers)
        {
            throw new System.NotImplementedException();
        }

        public void GetMarketYields(Bond bond)
        {
            throw new System.NotImplementedException();
        }

        public void GetModelYield(IBondCollection bonds)
        {
            throw new System.NotImplementedException();
        }
    }
}