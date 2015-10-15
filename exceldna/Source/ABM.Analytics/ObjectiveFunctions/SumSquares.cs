// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SumSquares.cs" company="">
//   
// </copyright>
// <summary>
//   The sum squares.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.ObjectiveFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The sum squares.
    /// </summary>
    public class SumSquares : IOptimiserObjectiveFunction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SumSquares"/> class.
        /// </summary>
        public SumSquares()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SumSquares"/> class.
        /// </summary>
        /// <param name="referenceList">
        /// The reference list.
        /// </param>
        public SumSquares(IEnumerable<double> referenceList)
        {
            this.Reference = referenceList.ToArray();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the reference.
        /// </summary>
        public double[] Reference { get; set; }

        #endregion

        #region Public Methods and Operators

        public double Cost { get; set; }

        /// <summary>
        /// The objective function.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="iteration">
        /// The iteration.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public double ObjectiveFunction(int n, double[] iteration)
        {
            this.Cost = 0.0;

            for (int i = 0; i < n; i++)
            {
                this.Cost += (this.Reference[i] - iteration[i]) * (this.Reference[i] - iteration[i]);
            }

            return this.Cost;
        }

        #endregion
    }
}