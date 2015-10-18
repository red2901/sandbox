// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DapiModelBase.cs" company="">
//   
// </copyright>
// <summary>
//   Base class for models that receive data via DAPI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Data.Services.Bloomberg
{
    using Bloomberglp.Blpapi;

    using global::Bloomberg.ExcelAppPortal;

    /// <summary>
    ///     Base class for models that receive data via DAPI.
    /// </summary>
    public abstract class DapiModelBase : BlpModel
    {
        #region Public Methods and Operators

        /// <summary>
        /// Called to process a message from a DAPI event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public abstract void ProcessDapiMessage(Message message);

        #endregion
    }
}