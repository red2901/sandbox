// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelDnaClientLogger.cs" company="">
//   
// </copyright>
// <summary>
//   The excel dna client logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.ExcelDna
{
    using System;

    using global::ExcelDna.Logging;

    /// <summary>
    ///     The excel dna client logger.
    /// </summary>
    public class ExcelDnaClientLogger : LocalClientLogger, ILocalClientLogger
    {
        #region Public Methods and Operators

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public override void Info(string message)
        {
            int count = this.UpdateMessageTracker(message);

            LogDisplay.RecordLine(string.Format("{0} - {1} - {2}", DateTime.Now.ToString("hh:mm:ss"), count, message));
        }

        /// <summary>
        /// The show.
        /// </summary>
        public override void Show()
        {
            LogDisplay.Show();
        }

        #endregion
    }
}