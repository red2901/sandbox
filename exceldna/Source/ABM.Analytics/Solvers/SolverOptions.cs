// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The solver options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The solver options.
    /// </summary>
    public class SolverOptions : ISolverOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverOptions"/> class.
        /// </summary>
        public SolverOptions()
        {
            this.MinimumDeltaValue = 0.000001;
            this.MinimumDeltaParameters = 0.000001;
            this.MaximumIterations = 5000;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether constrain.
        /// </summary>
        public bool Constrain { get; set; }

        /// <summary>
        ///     Gets or sets the lower bounds.
        /// </summary>
        public DenseVector LowerBounds { get; set; }

        /// <summary>
        ///     Number of iterations to stop processing.
        /// </summary>
        public int MaximumIterations { get; set; }

        /// <summary>
        ///     Change in model function parameters to stop iteration.
        /// </summary>
        public double MinimumDeltaParameters { get; set; }

        /// <summary>
        ///     Change in objective function value to stop iteration.
        /// </summary>
        public double MinimumDeltaValue { get; set; }

        /// <summary>
        ///     Gets or sets the upper bounds.
        /// </summary>
        public DenseVector UpperBounds { get; set; }

        #endregion
    }
}