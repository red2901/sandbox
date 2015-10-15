// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISolver.cs" company="">
//   
// </copyright>
// <summary>
//   The Solver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The Solver interface.
    /// </summary>
    public interface ISolver
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether debug iterations.
        /// </summary>
        bool DebugCalculations { get; set; }

        /// <summary>
        ///     Gets or sets the solver options.
        /// </summary>
        ISolverOptions SolverOptions { get; set; }

        /// <summary>
        ///     Gets or sets the solver result.
        /// </summary>
        ISolverResult SolverResult { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The estimate.
        /// </summary>
        /// <param name="objectiveValueCollection">
        /// The objective value collection.
        /// </param>
        /// <param name="initialParameters">
        /// The initial parameters.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        void Estimate(IObjectiveValueCollection objectiveValueCollection, DenseVector initialParameters);

        /// <summary>
        ///     The should terminate.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ShouldTerminate();

        #endregion
    }
}