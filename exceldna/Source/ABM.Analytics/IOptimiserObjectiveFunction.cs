// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptimiserObjectiveFunction.cs" company="">
//   
// </copyright>
// <summary>
//   The OptimiserObjectiveFunction interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    /// <summary>
    ///     The OptimiserObjectiveFunction interface.
    /// </summary>
    public interface IOptimiserObjectiveFunction
    {
        #region Public Properties

        /// <summary>
        ///     The cost.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        double Cost { get; set; }

        #endregion

        #region Public Methods and Operators

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
        double ObjectiveFunction(int n, double[] iteration);

        #endregion
    }
}