// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedObjectRepositoryService.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for ObjectManagerService
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ADM.Data.Services.UnitTests
{
    using System.Linq;

    using ABM.Data.Services;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     Summary description for ObjectManagerService
    /// </summary>
    [TestClass]
    public class ManagedObjectRepositoryService
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedObjectRepositoryService"/> class.
        /// </summary>
        public ManagedObjectRepositoryService()
        {
            var unityContainer = new UnityContainer();

            var locator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);

            unityContainer
                .RegisterType<IManagedObjectRepositoryService, ABM.Data.Services.ManagedObjectRepositoryService>(
                    new ContainerControlledLifetimeManager());
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The test method 1.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var managedObjectRepositoryService = ServiceLocator.Current.GetInstance<IManagedObjectRepositoryService>();

            var floatvalueManagedObject = new ManagedObject("floatvalue", 99.0);
            managedObjectRepositoryService.Add(floatvalueManagedObject);
            managedObjectRepositoryService.Add("intvalue", 2);

            Assert.AreEqual(managedObjectRepositoryService.Count, 2);
            Assert.IsTrue(managedObjectRepositoryService.Contains("floatvalue"));
            Assert.IsTrue(managedObjectRepositoryService.Contains("intvalue"));
            Assert.AreEqual(managedObjectRepositoryService.Keys().ToList()[0], "floatvalue#1");
            Assert.AreEqual(managedObjectRepositoryService.Keys().ToList()[1], "intvalue#1");

            object o = managedObjectRepositoryService.Retrieve("floatvalue");
            Assert.AreEqual(o, 99.0);

            ManagedObject mo = managedObjectRepositoryService.RetrieveManagedObject("floatvalue");
            mo.Update(100.0);

            Assert.AreEqual(managedObjectRepositoryService.Retrieve("floatvalue"), 100.0);
            Assert.AreEqual(managedObjectRepositoryService.RetrieveManagedObject("floatvalue").Version, 2);

            managedObjectRepositoryService.Clear();
            Assert.AreEqual(managedObjectRepositoryService.Count, 0);
        }

        #endregion
    }
}