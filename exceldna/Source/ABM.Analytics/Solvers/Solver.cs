// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Solver.cs" company="">
//   
// </copyright>
// <summary>
//   The solver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using System;

    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The solver.
    /// </summary>
    public abstract class Solver : ISolver
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether debug iterations.
        /// </summary>
        public bool DebugCalculations { get; set; }

        /// <summary>
        ///     Gets or sets the solver options.
        /// </summary>
        public ISolverOptions SolverOptions { get; set; }

        /// <summary>
        ///     Gets or sets the solver result.
        /// </summary>
        public ISolverResult SolverResult { get; set; }

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
        public abstract void Estimate(IObjectiveValueCollection objectiveValueCollection, DenseVector initialParameters);

        /// <summary>
        ///     The should terminate.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ShouldTerminate()
        {
            if (Math.Abs(this.SolverResult.ValueNew - this.SolverResult.ValueCurrent)
                <= this.SolverOptions.MinimumDeltaValue)
            {
                this.SolverResult.Status = SolverResultStatus.MinimumDeltaValueConverged;
                return true;
            }

            double result = this.SolverResult.ParametersNew.Subtract(this.SolverResult.ParametersCurrent).Norm(2.0);
            if (result <= this.SolverOptions.MinimumDeltaParameters)
            {
                this.SolverResult.Status = SolverResultStatus.MinimumDeltaParametersConverged;
                return true;
            }

            if (this.SolverResult.IterationResults.Count >= this.SolverOptions.MaximumIterations)
            {
                this.SolverResult.Status = SolverResultStatus.MaximumIterationsReached;
                return true;
            }

            return false;
        }

        #endregion
    }
}