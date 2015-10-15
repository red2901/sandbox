// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICashFlowStream.cs" company="">
//   
// </copyright>
// <summary>
//   The CashFlowStream interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System.Collections.Generic;

    /// <summary>
    ///     The CashFlowStream interface.
    /// </summary>
    public interface ICashFlowStream : IEnumerable<CashFlow>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The clear.
        /// </summary>
        void Clear();

        void Add(CashFlow cashflow);

        #endregion
    }
}