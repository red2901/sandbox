// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFittableObjectCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The FittableObjectCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model
{
    using System;

    using ABM.Analytics;

    /// <summary>
    /// The FittableObjectCollection interface.
    /// </summary>
    public interface IFittableObjectCollection : IObjectiveValueCollection, IEventAggregatorHandler
    {
        #region Public Methods and Operators
        /// <summary>
        /// The start fitting loop.
        /// </summary>
        bool Fit();

        #endregion
    }
}