// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UTOptimiser.cs" company="">
//   
// </copyright>
// <summary>
//   The ut optimiser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Analytics.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Cureos.Numerics.Optimizers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ABM.Analytics.ObjectiveFunctions;

    /// <summary>
    /// The ut optimiser.
    /// </summary>
    [TestClass]
    public class UTOptimiser
    {
        #region Public Methods and Operators

        private int N = 100;

        private IEnumerable<double> Linear()
        {
            for (int i = 0; i < N; i++)
            {
                yield return i - N/2;
            }
        }

        private IEnumerable<double> X2()
        {
            for (int i = 0; i < N; i++)
            {
                yield return i * i - (this.N / 2) * (this.N / 2);
            }
        }

        private IEnumerable<double> SqrtX()
        {
            for (int i = 0; i < N; i++)
            {
                yield return Math.Sqrt(i) - Math.Sqrt(50);
            }
        }

        private IEnumerable<double> InitialList(double val)
        {
            for (int i = 0; i < N; i++)
            {
                yield return val;
            }
        }

            /// <summary>
        /// The linear.
        /// </summary>
        [TestMethod]
        public void Fit()
        {
            var startingCondition = this.InitialList(0.0);

            var objectiveFunction = new SumSquares();
            var optimiser = new Optimiser();
            optimiser.OptimiserInitialVariables = new OptimiserInitialVariables(startingCondition);
            optimiser.OptimiserObjectiveFunction = objectiveFunction;

            objectiveFunction.Reference = this.Linear().ToArray();
            optimiser.FindMinimum();
            Assert.AreEqual(optimiser.OptimizationSummary.Status, OptimizationStatus.Normal);
            Assert.IsTrue(Math.Abs(optimiser.OptimizationSummary.F) < 0.000001);

            objectiveFunction.Reference = this.X2().ToArray();
            optimiser.FindMinimum();
            Assert.AreEqual(optimiser.OptimizationSummary.Status, OptimizationStatus.Normal);
            Assert.IsTrue(Math.Abs(optimiser.OptimizationSummary.F) < 0.000001);

            objectiveFunction.Reference = this.SqrtX().ToArray();
            optimiser.FindMinimum();
            Assert.AreEqual(optimiser.OptimizationSummary.Status, OptimizationStatus.Normal);
            Assert.IsTrue(Math.Abs(optimiser.OptimizationSummary.F) < 0.000001);
        }

        [TestMethod]
        public void SinglePass()
        {
            var targetList = new List<double>();
            targetList.Add(1.0);
            targetList.Add(2.0);
            targetList.Add(3.0);
            targetList.Add(4.0);
            targetList.Add(5.0);

            var startingCondition = new List<double>();
            startingCondition.Add(2.0);
            startingCondition.Add(2.0);
            startingCondition.Add(2.0);
            startingCondition.Add(2.0);
            startingCondition.Add(2.0);

            var intialVariables = new OptimiserInitialVariables(startingCondition);
            var objectiveFunction = new SumSquares(targetList);
            var optimiser = new Optimiser(intialVariables, objectiveFunction);
            optimiser.SinglePass = true;
            optimiser.FindMinimum();

            Assert.AreEqual(optimiser.OptimizationSummary.Status, OptimizationStatus.MAXFUN_Reached);
        }

        #endregion
    }
}