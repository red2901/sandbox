// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BloombergService.cs" company="">
//   
// </copyright>
// <summary>
//   The bloomberg service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services.Bloomberg
{
    using System;

    using Bloomberglp.Blpapi;

    /// <summary>
    ///     The bloomberg service.
    /// </summary>
    public class BloombergService : IBloombergService
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BloombergService"/> class.
        /// </summary>
        public BloombergService()
        {
            this.Start();
            this.Started = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The reference data service.
        /// </summary>
        public Service ReferenceDataService { get; private set; }

        /// <summary>
        ///     The DAPI Session.
        /// </summary>
        public Session Session { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether started.
        /// </summary>
        public bool Started { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The start.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// </exception>
        public void Start()
        {
            // create Session
            this.Session =
                new Session(
                    new SessionOptions
                        {
                            ServerHost = "localhost", 
                            ServerPort = 8194, 
                            ClientMode = SessionOptions.ClientModeType.DAPI, 
                            AutoRestartOnDisconnection = true, 
                            ConnectTimeout = 60000
                        });

            // start Session
            if (!this.Session.Start())
            {
                throw new ApplicationException("Unable to start Bloomberg API Session.");
            }

            try
            {
                // open reference data service
                if (!this.Session.OpenService("//blp/refdata"))
                {
                    throw new ApplicationException("Unable to open Bloomberg Reference Data Service.");
                }

                // get reference data service
                this.ReferenceDataService = this.Session.GetService("//blp/refdata");
                if (this.ReferenceDataService == null)
                {
                    throw new ApplicationException("Unable to get Bloomberg Reference Data Service.");
                }
            }
            catch
            {
                // an error occurred -- stop the Session
                this.Session.Stop();

                // rethrow error
                throw;
            }

            this.Started = true;
        }

        /// <summary>
        ///     Stops the DAPI Session.
        /// </summary>
        public void Stop()
        {
            this.Session.Stop();
            this.Started = false;
        }

        #endregion
    }
}