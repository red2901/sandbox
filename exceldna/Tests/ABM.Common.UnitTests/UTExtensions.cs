// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UTExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   Summary description for UTExtensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.UnitTests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     Summary description for UTExtensions
    /// </summary>
    [TestClass]
    public class UTExtensions
    {
        #region Fields

        /// <summary>
        /// The darray.
        /// </summary>
        private readonly double[] darray = new double[12];

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UTExtensions"/> class.
        /// </summary>
        public UTExtensions()
        {
            this.darray[0] = 1.0;
            this.darray[1] = 2.0;
            this.darray[2] = 3.0;
            this.darray[3] = 4.0;
            this.darray[4] = 5.0;
            this.darray[5] = 6.0;
            this.darray[6] = 7.0;
            this.darray[7] = 8.0;
            this.darray[8] = 9.0;
            this.darray[9] = 10.0;
            this.darray[10] = 0;
            this.darray[11] = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        #region Public Methods and Operators

        /// <summary>
        /// The double array.
        /// </summary>
        [TestMethod]
        public void DoubleArray()
        {
            List<decimal> decimalList = this.darray.ToDecimalListFromDoubleArray();
            Assert.IsTrue(decimalList.Count == 12);

            decimalList = this.darray.ToDecimalListFromDoubleArray(true);
            Assert.IsTrue(decimalList.Count == 10);
        }

        #endregion
    }
}