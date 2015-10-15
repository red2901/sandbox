// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFittableObject.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IFittableObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using MathNet.Numerics.LinearAlgebra;

    using ABM.Analytics;

    /// <summary>
    ///     The FittableObject interface.
    /// </summary>
    public interface IFittableObject : IObjectiveValue
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the ask.
        /// </summary>
        double Ask { get; set; }

        /// <summary>
        ///     Gets or sets the bid.
        /// </summary>
        double Bid { get; set; }

        double ModelPrice { get; set; }

        double Weight { get; set; }

        double AsOf { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The gradient.
        /// </summary>
        /// <param name="parametersInitial">
        /// The parameters initial.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        Vector<double> Gradient(Vector<double> parametersInitial);

        /// <summary>
        ///     The has fittable values.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool HasFittableValues();

        #endregion
    }
}