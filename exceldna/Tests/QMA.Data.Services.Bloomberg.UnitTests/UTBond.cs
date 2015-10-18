using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QMA.Data.Services.Bloomberg.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.Practices.ServiceLocation;

    using QMA.Functions;

    [TestClass]
    public class UTBond
    {
        private IInstrumentFactory bloombergInstrumentFactory;

        public UTBond()
        {
            Setup.Initialise();
        }

        /// <summary>
        /// The get bonds.
        /// </summary>
        [TestMethod]
        public void GetBond()
        {
            this.bloombergInstrumentFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();
            var bond = this.bloombergInstrumentFactory.GetBond("EK109396 Corp");

            Assert.IsTrue(bond != null);
            Assert.AreEqual("UKT", bond.Ticker);
        }

        [TestMethod]
        public void GetBondMarketData()
        {
            this.bloombergInstrumentFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();
            var bond = this.bloombergInstrumentFactory.GetBond("EK109396 Corp");
            this.bloombergInstrumentFactory.GetBondCalculationData(bond);

            Assert.IsTrue(bond != null);
            Assert.AreEqual("UKT", bond.Ticker);
            Assert.IsTrue(bond.Bid > 0);
            Assert.IsTrue(bond.YieldBid > 0);
        }
    }
}
