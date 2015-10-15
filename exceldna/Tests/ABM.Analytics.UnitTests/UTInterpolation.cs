namespace ABM.Analytics.UnitTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UTInterpolation
    {
        private TestData testData = new TestData();

        public UTInterpolation()
        {
            this.testData.Initialise();
        }

        [TestMethod]
        public void OnePoint()
        {
            int idx = 2609;
            var result = Interpolation.MultiPoint(this.testData.TestVector[idx], this.testData.XVector, this.testData.YVector);
            var targetResult = this.testData.TestVectorResult[idx];
          
            var error = Math.Abs(targetResult - result);
            Assert.IsTrue(error < 0.0001);            
        }

        [TestMethod]
        public void MultiPoint()
        {
            int idx = 0;
            foreach (var testPoint in this.testData.TestVector)
            {
                var result = Interpolation.MultiPoint(testPoint, this.testData.XVector, this.testData.YVector);
                var targetResult = this.testData.TestVectorResult[idx];
                idx += 1;
                var error = Math.Abs(targetResult - result);
                Assert.IsTrue(error < 0.0001);
            }
        }
    }
}
