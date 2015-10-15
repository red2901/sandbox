// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YieldCurve.cs" company="">
//   
// </copyright>
// <summary>
//   The discount curve.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Analytics;
    using ABM.Common;

    /// <summary>
    ///     The discount curve.
    /// </summary>
    public class YieldCurve : IYieldCurve
    {
        private ILog logger;

        private IEventAggregator eventAggregator;

        #region Fields

        /// <summary>
        ///     The curve dates.
        /// </summary>
        public IList<double> CurveDates { get; set; }

        IList<double> IYieldCurve.DiscountFactors { get; set; }

        /// <summary>
        ///     The discount factors.
        /// </summary>
        public IList<double> Yields { get; set; }

        #endregion

        #region Constructors and Destructors

        public YieldCurve(ILog logger, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YieldCurve"/> class.
        /// </summary>
        /// <param name="otherYieldCurve">
        /// The other yield curve.
        /// </param>
        public YieldCurve(IYieldCurve otherYieldCurve)
        {
            int maxN = otherYieldCurve.CurveDates.Count;
            this.CurveDates = new List<double>(maxN);
            for (int i = 0; i < maxN; i++)
            {
                this.CurveDates.Add(otherYieldCurve.CurveDates[i]);
            }

            this.Yields = new List<double>(maxN);
            for (int i = 0; i < maxN; i++)
            {
                this.Yields.Add(otherYieldCurve.Yields[i]);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="inputDate">
        /// The input Date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double DiscountFactor(double inputDate)
        {
            return Math.Exp(-1.0 * this.Yield(inputDate) * this.YearFraction(inputDate) / 100.0);
        }

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double DiscountFactor(DateTime date)
        {
            return this.DiscountFactor(date.ToOADate());
        }

        public IList<double> DiscountFactorList(IList<double> inputDates)
        {
            var discountFactors = new List<double>(inputDates.Count);
            foreach (double inputDate in inputDates)
            {
                discountFactors.Add(this.DiscountFactor(inputDate));
            }

            return discountFactors;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(IYieldCurve other)
        {
            IList<double> thisCurveDates = this.CurveDates;
            IList<double> otherCurveDates = other.CurveDates;

            if (thisCurveDates.Count != otherCurveDates.Count)
            {
                return false;
            }

            IList<double> thisYields = this.Yields;
            IList<double> otherYields = other.Yields;

            if (thisYields.Count != otherYields.Count)
            {
                return false;
            }

            for (int i = 0; i < thisYields.Count; i++)
            {
                if (!thisYields[i].Equals(otherYields[i]))
                {
                    return false;
                }
            }

            for (int i = 0; i < thisCurveDates.Count; i++)
            {
                if (!thisCurveDates[i].Equals(otherCurveDates[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     The length.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Length()
        {
            return this.CurveDates.Count;
        }

        /// <summary>
        ///     The to dense vector.
        /// </summary>
        /// <returns>
        ///     The <see cref="DenseVector" />.
        /// </returns>
        public DenseVector ToDenseVector()
        {
            IList<double> yields = this.Yields;
            var denseVector = new DenseVector(yields.Count);

            for (int i = 0; i < yields.Count; i++)
            {
                denseVector[i] = yields[i];
            }

            return denseVector;
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            var msgBuilder = new StringBuilder();
            for (int i = 0; i < this.CurveDates.Count; i++)
            {
                msgBuilder.AppendFormat("{0},{1}{2}", this.CurveDates[i], this.Yields[i], Environment.NewLine);
            }

            return msgBuilder.ToString();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="curveDates">
        /// The curve dates.
        /// </param>
        /// <param name="yields">
        /// The yields.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Update(IList<double> curveDates, IList<double> yields)
        {
            bool haschanged = false;

            if (this.CurveDates.Count != curveDates.Count)
            {
                this.CurveDates = curveDates;
                this.Yields = yields;
                return true;
            }


            for (int i = 0; i < curveDates.Count; i++)
            {
                if (!this.CurveDates[i].Equals(curveDates[i]))
                {
                    this.CurveDates[i] = curveDates[i];
                    haschanged = true;
                }
            }

            for (int i = 0; i < yields.Count; i++)
            {
                if (!this.Yields[i].Equals(yields[i]))
                {
                    this.Yields[i] = yields[i];
                    haschanged = true;
                }
            }

            return haschanged;
        }

        /// <summary>
        /// The update yields.
        /// </summary>
        /// <param name="iteration">
        /// The iteration.
        /// </param>
        public void UpdateYields(double[] iteration)
        {
            for (int i = 0; i < iteration.Length; i++)
            {
                this.Yields[i] = iteration[i];
            }
        }

        /// <summary>
        /// The update yields.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void UpdateYields(Vector<double> parameters)
        {
            int length = this.Yields.Count;
            for (int i = 0; i < length; i++)
            {
                this.Yields[i] = parameters[i];
            }
        }

        /// <summary>
        /// The year fraction.
        /// </summary>
        /// <param name="inputDate">
        /// The input date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double YearFraction(double inputDate)
        {
            return (inputDate - this.CurveDates[0]) / 365.25;
        }

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Yield(double date)
        {
            return Interpolation.MultiPoint(date, this.CurveDates, this.Yields);
        }

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Yield(DateTime date)
        {
            return this.Yield(date.ToOADate());
        }

        /// <summary>
        /// The extrapolate.
        /// </summary>
        /// <param name="inputDates">
        /// The input dates.
        /// </param>
        /// <returns>
        /// Yield series.
        /// </returns>
        public IList<double> YieldList(IList<double> inputDates)
        {
            var yields = new List<double>(inputDates.Count);
            foreach (double inputDate in inputDates)
            {
                yields.Add(Interpolation.MultiPoint(inputDate, this.CurveDates, this.Yields));
            }

            return yields;
        }

        #endregion
    }
}