// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISolverOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The SolverOptions interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The SolverOptions interface.
    /// </summary>
    public interface ISolverOptions
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether contrain.
        /// </summary>
        bool Constrain { get; set; }

        /// <summary>
        /// Gets or sets the lower bounds.
        /// </summary>
        DenseVector LowerBounds { get; set; }

        /// <summary>
        ///     Number of iterations to stop processing.
        /// </summary>
        int MaximumIterations { get; set; }

        /// <summary>
        ///     Change in model function parameters to stop iteration.
        /// </summary>
        double MinimumDeltaParameters { get; set; }

        /// <summary>
        ///     Change in objective function value to stop iteration.
        /// </summary>
        double MinimumDeltaValue { get; set; }

        /// <summary>
        /// Gets or sets the upper bounds.
        /// </summary>
        DenseVector UpperBounds { get; set; }

        #endregion
    }
}