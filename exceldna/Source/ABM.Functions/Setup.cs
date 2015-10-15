// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.cs" company="">
//   
// </copyright>
// <summary>
//   The functions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Functions
{
    using System;

    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using ABM.Analytics.Solvers;
    using ABM.Common;
    using ABM.Common.Unity;
    using ABM.Data.Services;
    using ABM.Data.Services.Bloomberg;
    using ABM.Model;

    /// <summary>
    ///     The functions.
    /// </summary>
    public static class Setup
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the unity container.
        /// </summary>
        public static IUnityContainer UnityContainer { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The close.
        /// </summary>
        public static void Close()
        {
            // stop bloomberg
            var bloombergService = ServiceLocator.Current.GetInstance<IBloombergService>();
            bloombergService.Stop();
        }

        /// <summary>
        ///     The initialise.
        /// </summary>
        public static void Initialise()
        {
            // initialise log4net
            LoggerSetup();

            // initialise Unity
            UnityContainer = new UnityContainer();

            // these 2 for are log4net
            UnityContainer.AddNewExtension<BuildTracking>();
            UnityContainer.AddNewExtension<LogCreation>();

            // initialise the service locator
            var locator = new UnityServiceLocator(UnityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);

            // register the event aggregator
            UnityContainer.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

            // register some objects
            UnityContainer.RegisterType<ISolverResult, SolverResult>(new TransientLifetimeManager());
            UnityContainer.RegisterType<ISolverOptions, SolverOptions>(new TransientLifetimeManager());
            UnityContainer.RegisterType<IBondCollection, BondCollection>(new TransientLifetimeManager());
            UnityContainer.RegisterType<IPriceCollection, PriceCollection>(new TransientLifetimeManager());
            UnityContainer.RegisterType<IFittedBondCollection, Data.Services.FittedBondCollection>(new TransientLifetimeManager());

            UnityContainer.RegisterType<IFittableObjectCollection, FittableObjectCollection>(new TransientLifetimeManager());
            UnityContainer.RegisterType<ICashFlowStream, CashFlowStream>(new TransientLifetimeManager());
            UnityContainer.RegisterType<IBond, Bond>(new TransientLifetimeManager());

            UnityContainer.RegisterType<IYieldCurve, YieldCurve>(YieldCurveType.Yield.ToString(), new TransientLifetimeManager());
            UnityContainer.RegisterType<IYieldCurve, DiscountCurve>(YieldCurveType.Discount.ToString(), new TransientLifetimeManager());
            UnityContainer.RegisterType<IYieldCurve, AnchorYieldCurve>(YieldCurveType.Anchor.ToString(), new TransientLifetimeManager());

            // register solvers
            //UnityContainer.RegisterType<ISolver, LevenbergMarquardt>(SolverType.LevenbergMarquardt.ToString(), new TransientLifetimeManager());
            UnityContainer.RegisterType<ISolver, LevenbergMarquardt>(new TransientLifetimeManager());

            // ok register services
            UnityContainer.RegisterType<IBloombergService, BloombergService>(new ContainerControlledLifetimeManager());
            UnityContainer.RegisterType<IInstrumentFactory, BloombergInstrumentFactory>(new ContainerControlledLifetimeManager());
            UnityContainer.RegisterType<IObjectManagerService, ObjectManagerService>(
                new ContainerControlledLifetimeManager());

            // start bloomberg
            try
            {
                var bloombergService = ServiceLocator.Current.GetInstance<IBloombergService>();
                bloombergService.Start();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// The logger setup.
        /// </summary>
        public static void LoggerSetup()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var consoleAppender = new ConsoleAppender();
            var patternLayout = new PatternLayout
                                    {
                                        ConversionPattern =
                                            "%date [%thread] %-5level %logger - %message%newline"
                                    };
            patternLayout.ActivateOptions();

            consoleAppender.Layout = patternLayout;
            consoleAppender.ActivateOptions();

            hierarchy.Root.AddAppender(consoleAppender);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }

        #endregion
    }
}