// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
//   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The copy new.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] CopyNew(this object[,] current)
        {
            int currentRows = current.GetLength(0);
            int currentCols = current.GetLength(1);

            var newArrary = new object[currentRows, currentCols];

            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentCols; j++)
                {
                    newArrary[i, j] = current[i, j];
                }
            }

            return newArrary;
        }

        /// <summary>
        /// The copy new.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] CopyNew(this object[,] current, int rows, int cols)
        {
            int currentRows = current.GetLength(0);
            int currentCols = current.GetLength(1);

            int maxrows = currentRows;
            if (currentRows < rows)
            {
                maxrows = rows;
            }

            int maxcols = currentCols;
            if (currentCols < cols)
            {
                maxcols = cols;
            }

            var newArrary = new object[maxrows, maxcols];

            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentCols; j++)
                {
                    newArrary[i, j] = current[i, j];
                }
            }

            return newArrary;
        }

        /// <summary>
        /// The fill null.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] FillNull(this object[,] current, object value)
        {
            int currentRows = current.GetLength(0);
            int currentCols = current.GetLength(1);

            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentCols; j++)
                {
                    if (current[i, j] == null)
                    {
                        current[i, j] = value;
                    }
                }
            }

            return current;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetModifiedHashCode(this double[] array)
        {
            int hc = array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                hc = unchecked((hc * 314159) + array[i].GetHashCode());
            }

            return hc;
        }

        /// <summary>
        /// The get modified hash code.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetModifiedHashCode(this object[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int hc = rows + cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    hc = unchecked((hc * 314159) + matrix[r, c].GetHashCode());
                }
            }

            return hc;
        }

        /// <summary>
        /// The has changed.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasChanged(this object[,] current, object[,] other)
        {
            int currentRows = current.GetLength(0);
            int currentCols = current.GetLength(1);

            int otherRows = other.GetLength(0);
            int otherCols = other.GetLength(1);

            if (currentRows != otherRows)
            {
                return true;
            }

            if (currentCols != otherCols)
            {
                return true;
            }

            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentCols; j++)
                {
                    if (!current[i, j].Equals(other[i, j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The is different from index list.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static IList<int> IsDifferentFromIndexList(this IList<string> current, IList<string> other)
        {
            var indexList = new List<int>();

            var imax = current.Count > other.Count ? other.Count : current.Count;

            for (int i = 0; i < imax; i++)
            {
                if (!current[i].Equals(other[i]))
                {
                    indexList.Add(i);
                }
            }

            return indexList;
        }

        /// <summary>
        /// The is different from index list.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static IList<int> IsDifferentFromIndexList(this IList<double> current, IList<double> other)
        {
            var indexList = new List<int>();

            var imax = current.Count > other.Count ? other.Count : current.Count;

            for (int i = 0; i < imax; i++)
            {
                if (Math.Abs(current[i] - other[i]) > 0.000001)
                {
                    indexList.Add(i);
                }
            }

            return indexList;
        }

        /// <summary>
        /// The is different from index list.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static bool HasChanged(this IList<bool> current, IList<bool> other)
        {
            for (int i = 0; i < current.Count; i++)
            {
                if (!current[i].Equals(other[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The to array.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ToArray(this Tuple<double, double> t)
        {
            var tuple = new double[2];

            tuple[0] = t.Item1;
            tuple[1] = t.Item2;
            return tuple;
        }

        /// <summary>
        /// The to boolean list from object matrix.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<bool> ToBooleanListFromObjectMatrix(this object[,] o)
        {
            List<bool> returnVector = null;

            int rows = o.GetLength(0);
            int cols = o.GetLength(1);

            returnVector = new List<bool>(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector.Add(Convert.ToBoolean(o[i, 0]));
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        /// <summary>
        /// The to double list.
        /// </summary>
        /// <param name="doubleList">
        /// The double list.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<decimal> ToDecimalList(this IList<double> doubleList)
        {
            foreach (double d in doubleList)
            {
                yield return Convert.ToDecimal(d);
            }
        }

        /// <summary>
        /// The to double list from column vector.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="removeTrailingZeros">
        /// The remove Trailing Zeros.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<decimal> ToDecimalListFromDoubleArray(this double[] o, bool removeTrailingZeros = false)
        {
            List<decimal> returnVector = null;

            int rows = o.GetLength(0);

            if (removeTrailingZeros)
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    if (Math.Abs(o[i]) < 0.0000000001)
                    {
                        rows = i;
                    }
                }
            }

            returnVector = new List<decimal>(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector.Add(Convert.ToDecimal(o[i]));
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        /// <summary>
        /// The to double list from object matrix.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="removeTrailingZeros">
        /// The remove trailing zeros.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<decimal> ToDecimalListFromObjectMatrix(this object[,] o, bool removeTrailingZeros = false)
        {
            List<decimal> returnVector = null;

            int rows = o.GetLength(0);
            int cols = o.GetLength(1);

            if (removeTrailingZeros)
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    try
                    {
                        if (Math.Abs(Convert.ToDouble(o[i, 0])) < 0.0000000001)
                        {
                            rows = i;
                        }
                    }
                    catch (Exception)
                    {
                        // continue as this maybe a bad value
                    }
                }
            }

            returnVector = new List<decimal>(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector.Add(Convert.ToDecimal(o[i, 0]));
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        /// <summary>
        /// The to dense object.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="DenseMatrix"/>.
        /// </returns>
        public static DenseMatrix ToDenseObject(this double[,] matrix)
        {
            return DenseMatrix.OfArray(matrix);
        }

        /// <summary>
        /// The to dense object.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="DenseVector"/>.
        /// </returns>
        public static DenseVector ToDenseObject(this double[] vector)
        {
            return DenseVector.OfArray(vector);
        }

        /// <summary>
        /// The to dense vector.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="DenseVector"/>.
        /// </returns>
        public static DenseVector ToDenseVector(this object[,] o)
        {
            int rows = o.GetLength(0);
            int cols = o.GetLength(1);

            var returnVector = new DenseVector(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector[i] = Convert.ToDouble(o[i, 0]);
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        /// <summary>
        /// The to double list from object matrix.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="removeTrailingZeros">
        /// The remove trailing zeros.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<double> ToDoubleListFromObjectMatrix(this object[,] o, bool removeTrailingZeros = false)
        {
            List<double> returnVector = null;

            int rows = o.GetLength(0);
            int cols = o.GetLength(1);

            if (removeTrailingZeros)
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    try
                    {
                        if (Math.Abs(Convert.ToDouble(o[i, 0])) < 0.0000000001)
                        {
                            rows = i;
                        }
                    }
                    catch (Exception)
                    {
                        // continue as this maybe a bad value
                    }
                }
            }

            returnVector = new List<double>(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector.Add(Convert.ToDouble(o[i, 0]));
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        /// <summary>
        /// The to enumerable double.
        /// </summary>
        /// <param name="enumerableDecimals">
        /// The enumerable decimals.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<double> ToEnumerableDouble(this IEnumerable<double> enumerableDecimals)
        {
            return enumerableDecimals.Select(Convert.ToDouble);
        }

        /// <summary>
        /// The to object array.
        /// </summary>
        /// <param name="decimalList">
        /// The double list.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ToObjectArray(this IList<double> decimalList)
        {
            var doubleArray = new object[decimalList.Count, 1];

            for (int i = 0; i < decimalList.Count; i++)
            {
                doubleArray[i, 0] = Convert.ToDouble(decimalList[i]);
            }

            return doubleArray;
        }

        /// <summary>
        /// The to object array.
        /// </summary>
        /// <param name="stringList">
        /// The string list.
        /// </param>
        /// <param name="sort">
        /// The sort.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ToObjectArray(this IList<string> stringList, bool sort = false)
        {
            var localStringList = (List<string>)stringList;
            if (sort)
            {
                localStringList.Sort();
            }

            var stringArray = new object[localStringList.Count, 1];

            int i = 0;
            foreach (string s in localStringList)
            {
                stringArray[i, 0] = s;
                i++;
            }

            return stringArray;
        }

        /// <summary>
        /// The to real array.
        /// </summary>
        /// <param name="v">
        /// The v.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ToRealArray(this Vector<Complex> v)
        {
            var ret = new double[v.Count];
            int index = 0;
            foreach (Complex complex in v)
            {
                ret[index] = complex.Real;
                index += 1;
            }

            return ret;
        }

        /// <summary>
        /// The to string list from object matrix.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> ToStringListFromObjectMatrix(this object[,] o)
        {
            List<string> returnVector = null;

            int rows = o.GetLength(0);

            returnVector = new List<string>(rows);
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    returnVector.Add(Convert.ToString(o[i, 0]));
                }
                catch (Exception)
                {
                    // log it and just return
                }
            }

            return returnVector;
        }

        #endregion
    }
}