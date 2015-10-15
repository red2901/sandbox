using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABM.Analytics.UnitTests
{
    [TestClass]
    public class UTNumericalDerivative
    {

        public static double X2(double x)
        {
            return x * x;
        }

        [TestMethod]
        public void TestX2()
        {
            // by heuriscally checking we get this type of precision which is confirmed by
            // http://stackoverflow.com/questions/1559695/implementing-the-derivative-in-c-c
            // http://en.wikipedia.org/wiki/Numerical_differentiation
            int numericalprecision = 6;

            var dx1 = NumericalDerivative.Backward(X2, 1.0);
            var dx2 = NumericalDerivative.Forward(X2, 1.0);
            var dx3 = NumericalDerivative.Central(X2, 1.0);

            Assert.AreEqual(2.0, Math.Round(dx1, numericalprecision));
            Assert.AreEqual(2.0, Math.Round(dx2, numericalprecision));
            Assert.AreEqual(2.0, Math.Round(dx3, numericalprecision));

            dx1 = NumericalDerivative.Backward(X2, 2.0);
            dx2 = NumericalDerivative.Forward(X2, 2.0);
            dx3 = NumericalDerivative.Central(X2, 2.0);

            Assert.AreEqual(4.0, Math.Round(dx1, numericalprecision));
            Assert.AreEqual(4.0, Math.Round(dx2, numericalprecision));
            Assert.AreEqual(4.0, Math.Round(dx3, numericalprecision));
            
        }
    }
}
