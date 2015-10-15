// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelEchoOffHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The excel echo off helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.ExcelDna
{
    using System;

    using global::ExcelDna.Integration;

    /// <summary>
    /// The excel echo off helper.
    /// </summary>
    public class ExcelEchoOffHelper : XlCall, IDisposable
    {
        #region Fields

        /// <summary>
        /// The old echo.
        /// </summary>
        private readonly object oldEcho;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelEchoOffHelper"/> class.
        /// </summary>
        public ExcelEchoOffHelper()
        {
            this.oldEcho = Excel(xlfGetWorkspace, 40);
            Excel(xlcEcho, false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Excel(xlcEcho, this.oldEcho);
        }

        #endregion
    }
}