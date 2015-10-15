// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectiveValue.cs" company="">
//   
// </copyright>
// <summary>
//   The ObjectiveValue interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Analytics
{
    using MathNet.Numerics.LinearAlgebra;

    /// <summary>
    /// The ObjectiveValue interface.
    /// </summary>
    public interface IObjectiveValue
    {
        #region Public Methods and Operators

        /// <summary>
        /// The objective value.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double ObjectiveValue(Vector<double> parameters);

        #endregion
    }
}