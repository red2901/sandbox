// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBondRegressionCoefficients.cs" company="">
//   
// </copyright>
// <summary>
//   The BondRegressionCoefficients interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra;

    /// <summary>
    ///     The BondRegressionCoefficients interface.
    /// </summary>
    public interface IBondRegressionCoefficients
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the amount outstanding.
        /// </summary>
        double AmountOutstanding { get; set; }

        /// <summary>
        ///     Gets the on count.
        /// </summary>
        int Available { get; }

        /// <summary>
        ///     Gets or sets the benchmark.
        /// </summary>
        double Benchmark { get; set; }

        /// <summary>
        /// Gets a value indicating whether benchmark on.
        /// </summary>
        bool BenchmarkOn { get; }

        /// <summary>
        ///     Gets or sets the bid ask spread.
        /// </summary>
        double BidAskSpread { get; set; }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets or sets the ctd.
        /// </summary>
        double Ctd { get; set; }

        /// <summary>
        /// Gets a value indicating whether ctd on.
        /// </summary>
        bool CtdOn { get; }

        /// <summary>
        ///     Gets or sets the issue date year fraction.
        /// </summary>
        double IssueDateYearFraction { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Equals(BondRegressionCoefficients other);

        /// <summary>
        ///     The to list.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        IList<double> ToList();

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="amountOutstandingVar">
        /// The amount outstanding var.
        /// </param>
        /// <param name="benchmarkVar">
        /// The benchmark var.
        /// </param>
        /// <param name="bidAskSpreadVar">
        /// The bid ask spread var.
        /// </param>
        /// <param name="ctdVar">
        /// The ctd var.
        /// </param>
        /// <param name="issueDateYearFractionVar">
        /// The issue date year fraction var.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Update(
            double amountOutstandingVar, 
            double benchmarkVar, 
            double bidAskSpreadVar, 
            double ctdVar, 
            double issueDateYearFractionVar);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="startidx">
        /// The startidx.
        /// </param>
        void Update(Vector<double> parameters, int startidx);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="localExtraParameters">
        /// The local extra parameters.
        /// </param>
        void Update(IList<bool> localExtraParameters);

        #endregion
    }
}