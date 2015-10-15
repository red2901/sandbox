// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBondCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The BondCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The BondCollection interface.
    /// </summary>
    public interface IBondCollection : IEnumerable<Bond>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        int Count { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        void Add(Bond item);

        /// <summary>
        ///     The clear.
        /// </summary>
        void Clear();

        /// <summary>
        /// The contains key.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ContainsKey(string ticker);

        /// <summary>
        /// The get bond.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="Bond"/>.
        /// </returns>
        Bond GetBond(string s);

        /// <summary>
        /// The keep.
        /// </summary>
        /// <param name="localTickerStringList">
        /// The local ticker string list.
        /// </param>
        void Keep(IList<string> localTickerStringList);

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
        /// The request keys.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<string> RequestKeys();

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

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="parametersNew">
        /// The parameters new.
        /// </param>
        void SetParameters(Vector<double> parametersNew);

        /// <summary>
        /// The set extra parameters.
        /// </summary>
        /// <param name="regressionCoefficients">
        /// The regression Coefficients.
        /// </param>
        void SetRegressionCoefficients(IBondRegressionCoefficients regressionCoefficients);

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        void SetYieldCurve(IYieldCurve yieldCurve);

        #endregion
    }
}