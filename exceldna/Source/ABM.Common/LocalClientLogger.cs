// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalClientLogger.cs" company="">
//   
// </copyright>
// <summary>
//   The local client logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The local client logger.
    /// </summary>
    public abstract class LocalClientLogger : ILocalClientLogger
    {
        #region Fields

        /// <summary>
        ///     The message tracker.
        /// </summary>
        private readonly Dictionary<string, int> messageTracker;

        /// <summary>
        ///     The on.
        /// </summary>
        private bool on;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalClientLogger" /> class.
        /// </summary>
        protected LocalClientLogger()
        {
            this.on = false;
            this.messageTracker = new Dictionary<string, int>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void Info(string message)
        {
            this.UpdateMessageTracker(message);
        }

        /// <summary>
        ///     The off.
        /// </summary>
        public void Off()
        {
            this.on = false;
        }

        /// <summary>
        ///     The on.
        /// </summary>
        public void On()
        {
            this.on = true;
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
        public virtual void Show()
        {
            throw new Exception("Nothing to show");
        }

        /// <summary>
        ///     The status.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Status()
        {
            return this.on;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The update message tracker.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected int UpdateMessageTracker(string message)
        {
            if (this.messageTracker.ContainsKey(message))
            {
                this.messageTracker[message] += 1;
            }
            else
            {
                this.messageTracker[message] = 1;
            }

            return this.messageTracker[message];
        }

        #endregion
    }
}