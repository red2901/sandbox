namespace ABMLib
{
    using ExcelDna.Integration;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using ABM.Common;
    using ABM.Common.ExcelDna;
    using ABM.Functions;

    public class ABMLibAddinSetup
    {
        public static void Initialise()
        {
            Setup.Initialise();

            // specific for Excel Dna
            Setup.UnityContainer.RegisterType<IOutputLocationCollection, ExcelDnaOutputLocationCollection>(
                new ContainerControlledLifetimeManager());

            Setup.UnityContainer.RegisterType<ILocalClientLogger, ExcelDnaClientLogger>(
                new ContainerControlledLifetimeManager());

            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            // logger.Show();
            logger.On();

            // register error logging
            ExcelIntegration.RegisterUnhandledExceptionHandler(Utils.ErrorHandler);            
        }

        public static void TearDown()
        {
            Setup.Close();
        }
    }
}