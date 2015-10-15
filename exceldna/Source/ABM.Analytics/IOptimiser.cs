// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptimiser.cs" company="">
//   
// </copyright>
// <summary>
//   The Optimiser interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System.Collections.Generic;

    using Cureos.Numerics.Optimizers;

    /// <summary>
    ///     The Optimiser interface.
    /// </summary>
    public interface IOptimiser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the optimiser initial variables.
        /// </summary>
        IOptimiserInitialVariables OptimiserInitialVariables { get; set; }

        /// <summary>
        ///     Gets or sets the optimiser objective function.
        /// </summary>
        IOptimiserObjectiveFunction OptimiserObjectiveFunction { get; set; }

        /// <summary>
        ///     Gets or sets the optimization summary.
        /// </summary>
        OptimizationSummary OptimizationSummary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether single pass.
        /// </summary>
        bool SinglePass { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find minimum.
        /// </summary>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        IEnumerable<double> FindMinimum();

        #endregion
    }
}