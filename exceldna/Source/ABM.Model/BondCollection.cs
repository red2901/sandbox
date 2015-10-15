// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BondCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The bond collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using ABM.Analytics;

    /// <summary>
    ///     The bond collection.
    /// </summary>
    public class BondCollection : IObjectiveValueCollection, IBondCollection
    {
        #region Fields

        /// <summary>
        /// The bond dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, Bond> bondDictionary;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BondCollection"/> class.
        /// </summary>
        /// <param name="loghandle">
        /// The loghandle.
        /// </param>
        public BondCollection(ILog loghandle)
        {
            this.logger = loghandle;
            this.bondDictionary = new ConcurrentDictionary<string, Bond>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.bondDictionary.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is read only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public void Add(Bond bond)
        {
            if (!this.bondDictionary.ContainsKey(bond.RequestKey))
            {
                this.bondDictionary.TryAdd(bond.RequestKey, bond);
            }
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            this.bondDictionary.Clear();
            
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(Bond item)
        {
            return this.bondDictionary.ContainsKey(item.RequestKey);
        }

        /// <summary>
        /// The contains key.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ContainsKey(string ticker)
        {
            return this.bondDictionary.ContainsKey(ticker);
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// The array index.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CopyTo(Bond[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get bond.
        /// </summary>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="Bond"/>.
        /// </returns>
        public Bond GetBond(string ticker)
        {
            return this.bondDictionary[ticker];
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<Bond> GetEnumerator()
        {
            return this.bondDictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// The keep.
        /// </summary>
        /// <param name="localTickerStringList">
        /// The local ticker string list.
        /// </param>
        public void Keep(IList<string> localTickerStringList)
        {
            var localTickerDict = new Dictionary<string, string>(localTickerStringList.Count);

            foreach (string ticker in localTickerStringList)
            {
                localTickerDict[ticker] = ticker;
            }

            var removeTickerList = new List<string>();
            foreach (string ticker in this.bondDictionary.Keys)
            {
                if (!localTickerDict.ContainsKey(ticker))
                {
                    removeTickerList.Add(ticker);
                }
            }

            foreach (string ticker in removeTickerList)
            {
                Bond currentBond = null;
                this.bondDictionary.TryRemove(ticker, out currentBond);
            }
        }

        /// <summary>
        /// The objective value.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double ObjectiveValue(Vector<double> parameters)
        {
            double objectiveValue = 0.0;
            foreach (Bond bond in this.bondDictionary.Values)
            {
                double y = bond.ObjectiveValue(parameters);
                this.logger.DebugFormat("{0} - {1} -> {2}", bond.ShortName, y, y * y);
                objectiveValue += y * y;
            }

            return 0.5 * objectiveValue;
        }

        /// <summary>
        /// The objective value jacobian.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public Matrix<double> ObjectiveValueJacobian(Vector<double> parametersCurrent)
        {
            Matrix<double> jacobian = new DenseMatrix(this.bondDictionary.Count, parametersCurrent.Count);

            int i = 0;
            foreach (Bond bond in this.bondDictionary.Values)
            {
                Vector<double> gradient = bond.Gradient(parametersCurrent);
                jacobian.SetRow(i, gradient);
                i += 1;
            }

            return jacobian;
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Remove(Bond item)
        {
            Bond bond;
            return this.bondDictionary.TryRemove(item.RequestKey, out bond);
        }

        /// <summary>
        ///     The identifiers.
        /// </summary>
        /// <returns>
        ///     Returns the list os request keys.
        /// </returns>
        public IEnumerable<string> RequestKeys()
        {
            return this.bondDictionary.Keys;
        }

        /// <summary>
        /// The residual.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        public Vector<double> Residual(Vector<double> parametersCurrent)
        {
            Vector<double> residual = new DenseVector(this.bondDictionary.Count);
            int i = 0;
            foreach (Bond bond in this.bondDictionary.Values)
            {
                residual[i] = bond.ObjectiveValue(parametersCurrent);
                i += 1;
            }

            return residual;
        }

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="parametersNew">
        /// The parameters new.
        /// </param>
        public void SetParameters(Vector<double> parametersNew)
        {
            foreach (Bond bond in this.bondDictionary.Values)
            {
                bond.SetParameters(parametersNew);
            }
        }

        /// <summary>
        /// The set extra parameters.
        /// </summary>
        /// <param name="bondRegressionCoefficients">
        /// The bond Regression Coefficients.
        /// </param>
        public void SetRegressionCoefficients(IBondRegressionCoefficients bondRegressionCoefficients)
        {
            foreach (Bond bond in this.bondDictionary.Values)
            {
                bond.SetRegressionCoefficients(bondRegressionCoefficients);
            }
        }

        /// <summary>
        /// The set yield curve.
        /// </summary>
        /// <param name="yieldCurve">
        /// The yield curve.
        /// </param>
        public void SetYieldCurve(IYieldCurve yieldCurve)
        {
            foreach (Bond bond in this.bondDictionary.Values)
            {
                bond.SetYieldCurve(yieldCurve);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="detail">
        /// The detail.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToString(bool detail)
        {
            var msgBuilder = new StringBuilder();
            foreach (Bond bond in this.bondDictionary.Values)
            {
                msgBuilder.AppendFormat("{0}{1}", bond.ToString(detail), Environment.NewLine);
            }

            return msgBuilder.ToString();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.bondDictionary.GetEnumerator();
        }

        #endregion
    }
}