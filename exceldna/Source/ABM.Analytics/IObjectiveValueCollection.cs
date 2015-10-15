// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectiveValueCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The MeasurableCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra;

    /// <summary>
    /// The MeasurableCollection interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IObjectiveValueCollection : IObjectiveValue
    {
        #region Public Methods and Operators

        /// <summary>
        /// The objective value jacobian.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        Matrix<double> ObjectiveValueJacobian(Vector<double> parametersCurrent);

        /// <summary>
        /// The residual.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        Vector<double> Residual(Vector<double> parametersCurrent);

        #endregion
    }
}