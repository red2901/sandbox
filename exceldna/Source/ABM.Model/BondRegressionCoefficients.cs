// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BondRegressionCoefficients.cs" company="">
//   
// </copyright>
// <summary>
//   The bond regression coefficients.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using MathNet.Numerics.LinearAlgebra;

    /// <summary>
    ///     The bond regression coefficients.
    /// </summary>
    public class BondRegressionCoefficients : IEquatable<BondRegressionCoefficients>, IBondRegressionCoefficients
    {
        #region Constants

        /// <summary>
        /// The amount outstanding idx.
        /// </summary>
        private const int AmountOutstandingIdx = 0;

        /// <summary>
        /// The benchmark idx.
        /// </summary>
        private const int BenchmarkIdx = 1;

        /// <summary>
        /// The bid ask spread idx.
        /// </summary>
        private const int BidAskSpreadIdx = 3;

        /// <summary>
        /// The ctd idx.
        /// </summary>
        private const int CtdIdx = 2;

        /// <summary>
        /// The issue date year fraction idx.
        /// </summary>
        private const int IssueDateYearFractionIdx = 4;

        /// <summary>
        /// The number coeffs.
        /// </summary>
        private const int NumberCoeffs = 5;

        #endregion

        #region Fields

        /// <summary>
        /// The coeffs.
        /// </summary>
        private readonly IList<double> coeffs;

        /// <summary>
        /// The coeff flags.
        /// </summary>
        private IList<bool> coeffFlags;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BondRegressionCoefficients" /> class.
        /// </summary>
        public BondRegressionCoefficients()
        {
            this.coeffs = new List<double>(NumberCoeffs) { 0, 0, 0, 0, 0 };
            this.coeffFlags = new List<bool>(NumberCoeffs) { false, false, false, false, false };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BondRegressionCoefficients"/> class.
        /// </summary>
        /// <param name="coeffSwitchboard">
        /// The coeff Switchboard.
        /// </param>
        public BondRegressionCoefficients(IList<bool> coeffSwitchboard)
        {
            this.coeffs = new List<double>(NumberCoeffs) { 0, 0, 0, 0, 0 };
            this.InitialiseCoeffs(coeffSwitchboard);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the amount outstanding.
        /// </summary>
        public double AmountOutstanding
        {
            get
            {
                return this.coeffs[AmountOutstandingIdx];
            }

            set
            {
                this.coeffs[AmountOutstandingIdx] = value;
            }
        }

        /// <summary>
        ///     Gets the on count.
        /// </summary>
        public int Available
        {
            get
            {
                int n = 0;
                for (int i = 0; i < NumberCoeffs; i++)
                {
                    if (Math.Abs(this.coeffs[i]) > 0.000000001)
                    {
                        n += 1;
                    }
                }

                return n;
            }
        }

        /// <summary>
        /// Gets or sets the benchmark.
        /// </summary>
        public double Benchmark
        {
            get
            {
                return this.coeffs[BenchmarkIdx];
            }

            set
            {
                this.coeffs[BenchmarkIdx] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether benchmark on.
        /// </summary>
        public bool BenchmarkOn
        {
            get
            {
                return Math.Abs(this.coeffs[BenchmarkIdx]) > 0.00000001;
            }
        }

        /// <summary>
        /// Gets or sets the bid ask spread.
        /// </summary>
        public double BidAskSpread
        {
            get
            {
                return this.coeffs[BidAskSpreadIdx];
            }

            set
            {
                this.coeffs[BidAskSpreadIdx] = value;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.coeffs.Count;
            }
        }

        /// <summary>
        /// Gets or sets the ctd.
        /// </summary>
        public double Ctd
        {
            get
            {
                return this.coeffs[CtdIdx];
            }

            set
            {
                this.coeffs[CtdIdx] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether ctd on.
        /// </summary>
        public bool CtdOn
        {
            get
            {
                return Math.Abs(this.coeffs[BenchmarkIdx]) > 0.00000001;
            }
        }

        /// <summary>
        /// Gets or sets the issue date year fraction.
        /// </summary>
        public double IssueDateYearFraction
        {
            get
            {
                return this.coeffs[IssueDateYearFractionIdx];
            }

            set
            {
                this.coeffs[IssueDateYearFractionIdx] = value;
            }
        }

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
        public bool Equals(BondRegressionCoefficients other)
        {
            if (!this.AmountOutstanding.Equals(other.AmountOutstanding))
            {
                return false;
            }

            if (!this.Benchmark.Equals(other.Benchmark))
            {
                return false;
            }

            if (!this.BidAskSpread.Equals(other.BidAskSpread))
            {
                return false;
            }

            if (!this.Ctd.Equals(other.Ctd))
            {
                return false;
            }

            if (!this.IssueDateYearFraction.Equals(other.IssueDateYearFraction))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     The to list.
        /// </summary>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        public IList<double> ToList()
        {
            return this.coeffs;
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0},{1}\n", "AmountOutstanding", this.AmountOutstanding);
            sb.AppendFormat("{0},{1}\n", "Benchmark", this.Benchmark);
            sb.AppendFormat("{0},{1}\n", "Ctd", this.Ctd);
            sb.AppendFormat("{0},{1}\n", "BidAskSpread", this.BidAskSpread);
            sb.AppendFormat("{0},{1}\n", "IssueDateYearFraction", this.IssueDateYearFraction);
            return sb.ToString();
        }

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
        public bool Update(
            double amountOutstandingVar, 
            double benchmarkVar, 
            double bidAskSpreadVar, 
            double ctdVar, 
            double issueDateYearFractionVar)
        {
            bool haschanged = false;
            if (!amountOutstandingVar.Equals(this.AmountOutstanding))
            {
                this.AmountOutstanding = amountOutstandingVar;
                haschanged = true;
            }

            if (!benchmarkVar.Equals(this.Benchmark))
            {
                this.Benchmark = benchmarkVar;
                haschanged = true;
            }

            if (!bidAskSpreadVar.Equals(this.BidAskSpread))
            {
                this.BidAskSpread = bidAskSpreadVar;
                haschanged = true;
            }

            if (!ctdVar.Equals(this.Ctd))
            {
                this.Ctd = ctdVar;
                haschanged = true;
            }

            if (!issueDateYearFractionVar.Equals(this.IssueDateYearFraction))
            {
                this.IssueDateYearFraction = issueDateYearFractionVar;
                haschanged = true;
            }

            return haschanged;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="startidx">
        /// The startidx.
        /// </param>
        public void Update(Vector<double> parameters, int startidx)
        {
            int j = startidx;
            for (int i = 0; i < this.coeffFlags.Count; i++)
            {
                if (this.coeffFlags[i])
                {
                    this.coeffs[i] = parameters[j];
                    j++;
                }
                else
                {
                    this.coeffs[i] = 0.0;
                }
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Update(IList<bool> parameters)
        {
            this.InitialiseCoeffs(parameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialise coeffs.
        /// </summary>
        /// <param name="coeffSwitchboard">
        /// The coeff switchboard.
        /// </param>
        private void InitialiseCoeffs(IList<bool> coeffSwitchboard)
        {
            for (int i = 0; i < coeffSwitchboard.Count; i++)
            {
                if (coeffSwitchboard[i])
                {
                    this.coeffs[i] = 0.0001;
                }
                else
                {
                    this.coeffs[i] = 0.0;
                }
            }

            this.coeffFlags = coeffSwitchboard;
        }

        #endregion
    }
}