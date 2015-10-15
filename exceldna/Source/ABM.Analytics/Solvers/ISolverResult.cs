// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISolverResult.cs" company="">
//   
// </copyright>
// <summary>
//   The SolverResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using System;
    using System.Collections.Specialized;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using ABM.Common;

    /// <summary>
    ///     The SolverResult interface.
    /// </summary>
    public interface ISolverResult
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the calculation profile.
        /// </summary>
        OrderedDictionary CalculationProfile { get; set; }

        /// <summary>
        ///     Gets or sets the fitting time.
        /// </summary>
        double FittingTime { get; set; }

        /// <summary>
        ///     Gets or sets the iteration results.
        /// </summary>
        ListOfVectors IterationResults { get; set; }

        /// <summary>
        ///     Gets or sets the parameters current.
        /// </summary>
        Vector<double> ParametersCurrent { get; set; }

        /// <summary>
        ///     Gets or sets the parameters new.
        /// </summary>
        Vector<double> ParametersNew { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        DateTime Start { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        SolverResultStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the stop.
        /// </summary>
        DateTime Stop { get; set; }

        /// <summary>
        ///     Gets or sets the value current.
        /// </summary>
        double ValueCurrent { get; set; }

        /// <summary>
        ///     Gets or sets the value new.
        /// </summary>
        double ValueNew { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The clear.
        /// </summary>
        void Clear();

        /// <summary>
        ///     The copy.
        /// </summary>
        /// <returns>
        ///     The <see cref="ISolverResult" />.
        /// </returns>
        ISolverResult Clone();

        /// <summary>
        /// The constrain.
        /// </summary>
        /// <param name="lowerBounds">
        /// The lower bounds.
        /// </param>
        /// <param name="upperBounds">
        /// The upper bounds.
        /// </param>
        void Constrain(DenseVector lowerBounds, DenseVector upperBounds);

        /// <summary>
        ///     The save iteration.
        /// </summary>
        void SaveIteration(params object[] objectArray);

        /// <summary>
        ///     The to display string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string ToDisplayString();

        /// <summary>
        ///     The update parameters.
        /// </summary>
        void UpdateParameters();

        /// <summary>
        ///     The value change.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ValueChange();

        #endregion
    }
}