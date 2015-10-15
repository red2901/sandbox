// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverResultStatus.cs" company="">
//   
// </copyright>
// <summary>
//   The solver result status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    /// <summary>
    ///     The solver result status.
    /// </summary>
    public enum SolverResultStatus
    {
        /// <summary>
        /// The minimum delta value converged.
        /// </summary>
        MinimumDeltaValueConverged, 

        /// <summary>
        /// The minimum delta parameters converged.
        /// </summary>
        MinimumDeltaParametersConverged, 

        /// <summary>
        /// The maximum iterations reached.
        /// </summary>
        MaximumIterationsReached
    }
}