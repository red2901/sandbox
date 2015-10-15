// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CashFlow.cs" company="">
//   
// </copyright>
// <summary>
//   The cash flow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;

    /// <summary>
    ///     The cash flow.
    /// </summary>
    public class CashFlow : IMergable<CashFlow>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the amount.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        public double Date { get; set; }

        /// <summary>
        ///     Gets or sets the premium.
        /// </summary>
        public double Principle { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="referenceInstrument">
        /// The reference instrument.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Merge(CashFlow referenceInstrument)
        {
            if (!this.Amount.Equals(referenceInstrument.Amount))
            {
                this.Amount = referenceInstrument.Amount;
            }

            if (!this.Date.Equals(referenceInstrument.Date))
            {
                this.Date = referenceInstrument.Date;
            }

            if (!this.Principle.Equals(referenceInstrument.Principle))
            {
                this.Principle = referenceInstrument.Principle;
            }
        }

        /// <summary>
        ///     The net amount which is amount + premium.
        /// </summary>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        public double NetAmount()
        {
            return this.Amount + this.Principle;
        }

        #endregion
    }
}