// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimiserInitialVaraibles.cs" company="">
//   
// </copyright>
// <summary>
//   The optimiser initial varaibles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The optimiser initial varaibles.
    /// </summary>
    public class OptimiserInitialVariables : IOptimiserInitialVariables
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimiserInitialVariables"/> class.
        /// </summary>
        public OptimiserInitialVariables()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimiserInitialVariables"/> class.
        /// </summary>
        /// <param name="x0">
        /// The x 0.
        /// </param>
        public OptimiserInitialVariables(IEnumerable<double> x0)
        {
            this.X0 = x0.ToArray();
            this.LowerBound = this.InitialiseArray(this.X0.Length, -9999999.0);
            this.UpperBound = this.InitialiseArray(this.X0.Length, 9999999.0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimiserInitialVariables"/> class.
        /// </summary>
        /// <param name="x0">
        /// The x 0.
        /// </param>
        /// <param name="lower">
        /// The lower.
        /// </param>
        /// <param name="upper">
        /// The upper.
        /// </param>
        public OptimiserInitialVariables(IEnumerable<double> x0, double lower, double upper)
        {
            this.X0 = x0.ToArray();
            this.LowerBound = this.InitialiseArray(this.X0.Length, lower);
            this.UpperBound = this.InitialiseArray(this.X0.Length, upper);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the lower bound.
        /// </summary>
        public double[] LowerBound { get; set; }

        /// <summary>
        ///     Gets or sets the upper bound.
        /// </summary>
        public double[] UpperBound { get; set; }

        /// <summary>
        ///     Gets or sets the x 0.
        /// </summary>
        public double[] X0 { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The initialise array.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        private double[] InitialiseArray(int n, double x)
        {
            var array = new double[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = x;
            }

            return array;
        }

        #endregion
    }
}