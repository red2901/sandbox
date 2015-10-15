// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListOfVectors.cs" company="">
//   
// </copyright>
// <summary>
//   The list of vectors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using MathNet.Numerics.LinearAlgebra;

    /// <summary>
    ///     The list of vectors.
    /// </summary>
    public class ListOfVectors : List<Vector<double>>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="ListOfVectors"/>.
        /// </returns>
        public ListOfVectors Clone()
        {
            var newListOfVector = new ListOfVectors();
            foreach (var v in this)
            {
                newListOfVector.Add(v.Clone());
            }

            return newListOfVector;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <param name="columnFormat">
        /// The column format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToDisplayString()
        {
            var headerBuilder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                headerBuilder.AppendFormat("Iter{0},", i);
            }

            headerBuilder.Append(Environment.NewLine);
            int vectorLength = this[0].Count;
            var dataBuilder = new StringBuilder();
            for (int i = 0; i < vectorLength; i++)
            {
                for (int j = 0; j < this.Count; j++)
                {
                    dataBuilder.AppendFormat("{0}, ", this[j][i]);
                }

                dataBuilder.Append(Environment.NewLine);
            }

            return string.Format("{0}{1}", headerBuilder, dataBuilder);
        }

        #endregion
    }
}