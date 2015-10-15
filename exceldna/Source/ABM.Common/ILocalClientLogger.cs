// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalClientLogger.cs" company="">
//   
// </copyright>
// <summary>
//   The Logger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    /// <summary>
    ///     The Logger interface. This is mainly for client applications where external libraries like ExcelDna has its own
    ///     logger. Internally we should use ILog and log4net.
    /// </summary>
    public interface ILocalClientLogger
    {
        #region Public Methods and Operators

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Info(string message);

        /// <summary>
        ///     The off.
        /// </summary>
        void Off();

        /// <summary>
        ///     The on.
        /// </summary>
        void On();

        /// <summary>
        /// The show.
        /// </summary>
        void Show();

        /// <summary>
        ///     The status.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool Status();

        #endregion
    }
}