// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="">
//   
// </copyright>
// <summary>
//   The unit test 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The unit test 1.
    /// </summary>
    [TestClass]
    public class UTHasCodes
    {
        private double[] darray = new double[10];

        public UTHasCodes()
        {
            darray[0] = 1.0;
            darray[1] = 2.0;
            darray[2] = 3.0;
            darray[3] = 4.0;
            darray[4] = 5.0;
            darray[5] = 6.0;
            darray[6] = 7.0;
            darray[7] = 8.0;
            darray[8] = 9.0;
            darray[9] = 10.0;
        }

        #region Public Methods and Operators

        /// <summary>
        /// The test method 1.
        /// </summary>
        [TestMethod]
        public void DoubleTest()
        {
            double d = 123.4;
            Assert.AreEqual(-641253373, d.GetHashCode());
        }

        [TestMethod]
        public void DoubleArrayTest()
        {
            Assert.AreEqual(50119998, this.darray.GetHashCode());
            Assert.AreEqual(-1727024310, this.darray.GetModifiedHashCode());

            this.darray[0] = -999;

            // proof that the hashcod for an array doesn't change when the values change.
            Assert.AreEqual(50119998, this.darray.GetHashCode());

            Assert.AreNotEqual(-1727024310, this.darray.GetModifiedHashCode());
        }

        #endregion
    }
}