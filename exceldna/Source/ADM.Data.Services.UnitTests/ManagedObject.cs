// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedObject.cs" company="">
//   
// </copyright>
// <summary>
//   The managed object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ADM.Data.Services.UnitTests
{
    using ABM.Data.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     The managed object.
    /// </summary>
    [TestClass]
    public class ManagedObjectTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The test method 1.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            double floatvalue = 99.9;
            var floatvalueManagedObject = new ManagedObject("floatvalue", floatvalue);
            Assert.AreEqual(floatvalueManagedObject.KeyString(), "floatvalue#1");
            Assert.AreEqual(floatvalueManagedObject.ToString(), "99.9");
            Assert.AreEqual(floatvalueManagedObject.RawObject, 99.9);

            floatvalueManagedObject.Update(100.0);

            Assert.AreEqual(floatvalueManagedObject.RawObject, 100.0);

            floatvalueManagedObject.UpdateVersion();
            Assert.AreEqual(floatvalueManagedObject.Version, 3);
            Assert.AreEqual(floatvalueManagedObject.Name, "floatvalue");
            Assert.AreEqual(ManagedObject.NameString(floatvalueManagedObject.KeyString()), "floatvalue");
        }

        #endregion
    }
}