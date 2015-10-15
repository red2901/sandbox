// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IYieldCurve.cs" company="">
//   
// </copyright>
// <summary>
//   The YieldCurve interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The YieldCurve interface.
    /// </summary>
    public interface IYieldCurve : IEquatable<IYieldCurve>
    {
        #region Public Properties

        /// <summary>
        ///     The curve dates.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        IList<double> CurveDates { get; set; }

        /// <summary>
        /// Gets or sets the discount factors.
        /// </summary>
        IList<double> DiscountFactors { get; set; }

        /// <summary>
        ///     The yields.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        IList<double> Yields { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double DiscountFactor(double date);

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double DiscountFactor(DateTime date);

        /// <summary>
        /// The discount factors.
        /// </summary>
        /// <param name="inputDates">
        /// The input dates.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IList<double> DiscountFactorList(IList<double> inputDates);

        /// <summary>
        ///     The length.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int Length();

        /// <summary>
        ///     The to dense vector.
        /// </summary>
        /// <returns>
        ///     The <see cref="DenseVector" />.
        /// </returns>
        DenseVector ToDenseVector();

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="curveDates">
        /// The curve dates.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Update(IList<double> curveDates, IList<double> values);

        /// <summary>
        /// The update yields.
        /// </summary>
        /// <param name="iteration">
        /// The iteration.
        /// </param>
        void UpdateYields(double[] iteration);

        /// <summary>
        /// The update yields.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void UpdateYields(Vector<double> parameters);

        /// <summary>
        /// The yield.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double Yield(DateTime date);

        /// <summary>
        /// The yield.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double Yield(double date);

        /// <summary>
        /// The yields.
        /// </summary>
        /// <param name="inputDates">
        /// The input dates.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IList<double> YieldList(IList<double> inputDates);

        #endregion
    }
}