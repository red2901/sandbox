using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABM.Analytics.UnitTests
{
    using System.Linq;

    using MathNet.Numerics.LinearAlgebra.Double;

    [TestClass]
    public class Numerics
    {
        private double[,] testMatrix = new double[2,3];

        public Numerics()
        {
            testMatrix[0, 0] = 1.0;
            testMatrix[1, 0] = 2.0;
            testMatrix[0, 1] = 3.0;
            testMatrix[1, 1] = 4.0;

        }

        [TestMethod]
        public void Range()
        {
            var matrix = new double[2,2];
            var denseMatrix = DenseMatrix.OfArray(matrix);
            var range = denseMatrix.Range();
            var array = new double[range.Length];

            int i = 0;
            foreach (var vector in range)
            {
                array[i] = vector.Count;
                i += 1;
            }

            Assert.AreEqual(array.Length, 2);
        }
    }
}
