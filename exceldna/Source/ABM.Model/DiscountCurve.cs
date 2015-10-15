// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscountCurve.cs" company="">
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
    using System.Text;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using ABM.Analytics;

    /// <summary>
    ///     The discount curve.
    /// </summary>
    public class DiscountCurve : IYieldCurve
    {


        #region Public Methods and Operators

        /// <summary>
        ///     The curve dates.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> CurveDates { get; set; }

        /// <summary>
        /// The discount factor.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double DiscountFactor(double date)
        {
            return Interpolation.MultiPoint(date, this.CurveDates, this.DiscountFactors);
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

        /// <summary>
        /// The extrapolate.
        /// </summary>
        /// <param name="inputDates">
        /// The input dates.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IList<double> DiscountFactorList(IList<double> inputDates)
        {
            var discountFactors = new List<double>(inputDates.Count);

            foreach (double inputDate in inputDates)
            {
                discountFactors.Add(Interpolation.MultiPoint(inputDate, this.CurveDates, this.DiscountFactors));
            }

            return discountFactors;
        }

        /// <summary>
        ///     The discount factors.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> DiscountFactors { get; set; }

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
            if (this.CurveDates.Count != other.CurveDates.Count)
            {
                return false;
            }

            if (this.DiscountFactors.Count != other.Yields.Count)
            {
                return false;
            }

            IList<double> discountFactors = this.DiscountFactors;
            IList<double> otherDiscountFactors = other.DiscountFactors;
            for (int i = 0; i < this.DiscountFactors.Count; i++)
            {
                if (!discountFactors[i].Equals(otherDiscountFactors[i]))
                {
                    return false;
                }
            }

            IList<double> thisCurveDates = this.CurveDates;
            IList<double> otherCurveDates = other.CurveDates;
            for (int i = 0; i < this.CurveDates.Count; i++)
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
            IList<double> dfs = this.DiscountFactors;
            var denseVector = new DenseVector(dfs.Count);

            for (int i = 0; i < dfs.Count; i++)
            {
                denseVector[i] = dfs[i];
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
                msgBuilder.AppendFormat(
                    "{0} {1}{2}", 
                    this.CurveDates[i], 
                    this.DiscountFactors[i], 
                    Environment.NewLine);
            }

            return msgBuilder.ToString();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="curveDates">
        /// The curve dates.
        /// </param>
        /// <param name="discountFactors">
        /// The discount factors.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Update(IList<double> curveDates, IList<double> discountFactors)
        {
            bool haschanged = false;

            for (int i = 0; i < curveDates.Count; i++)
            {
                if (!this.CurveDates[i].Equals(curveDates[i]))
                {
                    this.CurveDates[i] = curveDates[i];
                    haschanged = true;
                }
            }

            for (int i = 0; i < discountFactors.Count; i++)
            {
                if (!this.DiscountFactors[i].Equals(discountFactors[i]))
                {
                    this.DiscountFactors[i] = discountFactors[i];
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void UpdateYields(double[] iteration)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update yields.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void UpdateYields(Vector<double> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The yield.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public double Yield(DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The yield.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public double Yield(double date)
        {
            throw new NotImplementedException();
        }

        IList<double> IYieldCurve.Yields { get; set; }

        public IList<double> YieldList(IList<double> inputDates)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}