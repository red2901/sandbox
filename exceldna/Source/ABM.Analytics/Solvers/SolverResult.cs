// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverResult.cs" company="">
//   
// </copyright>
// <summary>
//   The solver result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.Serialization.Formatters;
    using System.Text;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using ABM.Common;

    /// <summary>
    ///     The solver result.
    /// </summary>
    public class SolverResult : ISolverResult
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SolverResult" /> class.
        /// </summary>
        public SolverResult()
        {
            this.IterationResults = new ListOfVectors();
            this.CalculationProfile = new OrderedDictionary();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the calculation profile.
        /// </summary>
        public OrderedDictionary CalculationProfile { get; set; }

        /// <summary>
        ///     Gets or sets the fitting time.
        /// </summary>
        public double FittingTime { get; set; }

        /// <summary>
        ///     Gets or sets the iteration results.
        /// </summary>
        public ListOfVectors IterationResults { get; set; }

        /// <summary>
        ///     Gets or sets the parameters current.
        /// </summary>
        public Vector<double> ParametersCurrent { get; set; }

        /// <summary>
        ///     Gets or sets the parameters new.
        /// </summary>
        public Vector<double> ParametersNew { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        public SolverResultStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the stop.
        /// </summary>
        public DateTime Stop { get; set; }

        /// <summary>
        ///     Gets or sets the value current.
        /// </summary>
        public double ValueCurrent { get; set; }

        /// <summary>
        ///     Gets or sets the value new.
        /// </summary>
        public double ValueNew { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
            this.IterationResults.Clear();
        }

        /// <summary>
        ///     The clone.
        /// </summary>
        /// <returns>
        ///     The <see cref="ISolverResult" />.
        /// </returns>
        public ISolverResult Clone()
        {
            var newSolverResult = new SolverResult
                                      {
                                          IterationResults = this.IterationResults.Clone(),
                                          ParametersCurrent = this.ParametersCurrent.Clone(),
                                          ParametersNew = this.ParametersNew.Clone(),
                                          Status = this.Status,
                                          ValueCurrent = this.ValueCurrent,
                                          ValueNew = this.ValueNew,
                                          FittingTime = this.FittingTime,
                                          Start = this.Start,
                                          Stop = this.Stop
                                      };
            foreach (var key in this.CalculationProfile.Keys)
            {
                newSolverResult.CalculationProfile[key] = this.CalculationProfile[key];
            }
            return newSolverResult;
        }

        /// <summary>
        /// The constrain.
        /// </summary>
        /// <param name="lowerBounds">
        /// The lower bounds.
        /// </param>
        /// <param name="upperBounds">
        /// The upper bounds.
        /// </param>
        public void Constrain(DenseVector lowerBounds, DenseVector upperBounds)
        {
            Vector<double> constrainVector = this.ParametersNew;
            for (int i = 0; i < constrainVector.Count; i++)
            {
                if (lowerBounds != null && constrainVector[i] <= lowerBounds[i])
                {
                    constrainVector[i] = lowerBounds[i];
                }

                if (upperBounds != null && constrainVector[i] >= upperBounds[i])
                {
                    constrainVector[i] = upperBounds[i];
                }
            }
        }

        /// <summary>
        ///     The save iteration.
        /// </summary>
        public void SaveIteration(params object[] objectArray)
        {
            var v = new DenseVector(this.ParametersCurrent.Count + objectArray.Length);

            for (int i = 0; i < this.ParametersCurrent.Count; i++)
            {
                v[i] = this.ParametersCurrent[i];
            }

            for (int j = 0, i = this.ParametersCurrent.Count; j < objectArray.Length; j++, i++)
            {
                v[i] = Convert.ToDouble(objectArray[j]);
            }

            this.IterationResults.Add(v);
        }

        /// <summary>
        ///     The to display string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToDisplayString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} = {1}{2}", "Status", this.Status, Environment.NewLine);
            sb.AppendFormat("{0} = {1}{2}", "Iterations", this.IterationResults.Count, Environment.NewLine);
            sb.AppendFormat("{0} = {1}{2}", "Start", this.Start.ToString("HH:mm:ss"), Environment.NewLine);
            sb.AppendFormat("{0} = {1}{2}", "Stop", this.Stop.ToString("HH:mm:ss"), Environment.NewLine);
            sb.AppendFormat("{0} = {1}ms{2}", "FittingTime", this.FittingTime, Environment.NewLine);

            foreach (var key in this.CalculationProfile.Keys)
            {
                var ms = this.CalculationProfile[key];
                sb.AppendFormat("    {0} = {1}ms / {2}ms {3}", key, ms, Convert.ToInt32(Convert.ToDouble(ms) / Convert.ToDouble(this.IterationResults.Count)), Environment.NewLine);
            }

            sb.AppendFormat("{0} = {1}{2}", "ValueNew", this.ValueNew, Environment.NewLine);
            sb.AppendFormat("{0} = {1}{2}", "ValueCurrent", this.ValueCurrent, Environment.NewLine);
            sb.AppendFormat(
                "{0} = {1}{2}", 
                "ParametersNew", 
                this.ParametersNew.ToString(this.ParametersNew.Count, 20), 
                Environment.NewLine);
            sb.AppendFormat(
                "{0} = {1}{2}", 
                "ParametersCurrent", 
                this.ParametersCurrent.ToString(this.ParametersCurrent.Count, 20), 
                Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        ///     The update parameters.
        /// </summary>
        public void UpdateParameters()
        {
            this.ValueCurrent = this.ValueNew;
            this.ParametersNew.CopyTo(this.ParametersCurrent);
        }

        /// <summary>
        ///     The value change.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ValueChange()
        {
            return this.ValueNew < this.ValueCurrent;
        }

        #endregion
    }
}