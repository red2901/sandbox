// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelCalculationManualHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The excel calculation manual helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.ExcelDna
{
    using System;

    using global::ExcelDna.Integration;

    /// <summary>
    /// The excel calculation manual helper.
    /// </summary>
    public class ExcelCalculationManualHelper : XlCall, IDisposable
    {
        #region Fields

        /// <summary>
        /// The old calculation mode.
        /// </summary>
        private readonly object oldCalculationMode;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCalculationManualHelper"/> class.
        /// </summary>
        public ExcelCalculationManualHelper()
        {
            this.oldCalculationMode = Excel(xlfGetDocument, 14);
            Excel(xlcOptionsCalculation, 3);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Excel(xlcOptionsCalculation, this.oldCalculationMode);
        }

        #endregion
    }
}