// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericalDerivative.cs" company="">
//   
// </copyright>
// <summary>
//   The numerical derivative.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System;

    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    ///     The numerical derivative.
    /// </summary>
    public class NumericalDerivative
    {
        #region Static Fields

        /// <summary>
        ///     The h.
        /// </summary>
        public static double h = Math.Sqrt(Precision.DoublePrecision);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The backward.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Backward(Func<double, double> f, double x)
        {
            return (f(x) - f(x - h)) / h;
        }

        /// <summary>
        /// The central.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Central(Func<double, double> f, double x)
        {
            double half_local_h = h * 0.5;
            return (f(x + half_local_h) - f(x - half_local_h)) / h;
        }

        /// <summary>
        /// The forward.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double Forward(Func<double, double> f, double x)
        {
            return (f(x + h) - f(x)) / h;
        }

        /// <summary>
        /// The forward numerical derivative.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        public static Vector<double> Forward(Func<Vector<double>, double> f, Vector<double> x)
        {
            Vector<double> gradient = new DenseVector(x.Count);
            for (int i = 0; i < x.Count; i++)
            {
                Vector<double> xph = x.Clone();

                // forward derivative
                xph[i] = xph[i] + h;
                gradient[i] = (f(xph) - f(x)) / h;
            }

            return gradient;
        }

        #endregion
    }
}