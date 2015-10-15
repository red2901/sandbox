// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Optimiser.cs" company="">
//   
// </copyright>
// <summary>
//   The optimzer helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System.Collections.Generic;
    using System.Linq;

    using Cureos.Numerics.Optimizers;

    /// <summary>
    ///     The optimzer helper.
    /// </summary>
    public class Optimiser : IOptimiser
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Optimiser"/> class.
        /// </summary>
        public Optimiser()
        {
            this.SinglePass = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Optimiser"/> class.
        /// </summary>
        /// <param name="initialVariables">
        /// The initial Variables.
        /// </param>
        /// <param name="objectiveFunction">
        /// The objective Function.
        /// </param>
        public Optimiser(IOptimiserInitialVariables initialVariables, IOptimiserObjectiveFunction objectiveFunction)
        {
            this.OptimiserInitialVariables = initialVariables;
            this.OptimiserObjectiveFunction = objectiveFunction;
            this.SinglePass = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the optimiser initial variables.
        /// </summary>
        public IOptimiserInitialVariables OptimiserInitialVariables { get; set; }

        /// <summary>
        ///     Gets or sets the optimiser objective function.
        /// </summary>
        public IOptimiserObjectiveFunction OptimiserObjectiveFunction { get; set; }

        /// <summary>
        ///     Gets or sets the optimization summary.
        /// </summary>
        public OptimizationSummary OptimizationSummary { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether single pass.
        /// </summary>
        public bool SinglePass { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find minimum.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public IEnumerable<double> FindMinimum()
        {
            this.OptimizationSummary = this.Optimise();

            return this.OptimizationSummary.X;
        }

        /// <summary>
        ///     The optimise.
        /// </summary>
        /// <returns>
        ///     The <see cref="OptimizationSummary" />.
        /// </returns>
        public OptimizationSummary Optimise()
        {
            var optimizer = new Bobyqa(
                this.OptimiserInitialVariables.X0.Length, 
                this.OptimiserObjectiveFunction.ObjectiveFunction, 
                this.OptimiserInitialVariables.LowerBound, 
                this.OptimiserInitialVariables.UpperBound);

            if (this.SinglePass)
            {
                optimizer.MaximumFunctionCalls = 3;
            }

            return optimizer.FindMinimum(this.OptimiserInitialVariables.X0);
        }

        #endregion
    }
}