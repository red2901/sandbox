// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverType.cs" company="">
//   
// </copyright>
// <summary>
//   The solver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Analytics.Solvers
{
    /// <summary>
    /// The solver type.
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        ///     Solver based on normal equations and Cholesky decomposition.
        /// </summary>
        Normal, 

        /// <summary>
        ///     Solver based on QR decomposition.
        /// </summary>
        Qr, 

        /// <summary>
        ///     Solver based on singular value decomposition (SVD).
        /// </summary>
        Svd, 

        /// <summary>
        ///     Solver based on steepest descent iteration.
        /// </summary>
        SteepestDescent, 

        /// <summary>
        ///     Solver based on Gauss-Newton iteration.
        /// </summary>
        GaussNewton, 

        /// <summary>
        ///     Solver based on Levenberg-Marquardt iteration.
        /// </summary>
        LevenbergMarquardt
    }
}