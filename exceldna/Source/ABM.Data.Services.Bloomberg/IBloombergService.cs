// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBloombergService.cs" company="">
//   
// </copyright>
// <summary>
//   The BloombergService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Data.Services
{
    using Bloomberglp.Blpapi;

    /// <summary>
    /// The BloombergService interface.
    /// </summary>
    public interface IBloombergService
    {
        #region Public Properties

        /// <summary>
        ///     The reference data service.
        /// </summary>
        Service ReferenceDataService { get; }

        /// <summary>
        ///     The DAPI Session.
        /// </summary>
        Session Session { get; }

        /// <summary>
        /// Gets a value indicating whether started.
        /// </summary>
        bool Started { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        void Start();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();

        #endregion
    }
}