namespace ABM.Common
{
    using System;

    using log4net;

    using Microsoft.Practices.ServiceLocation;

    public class BasicLocalClientLogger : LocalClientLogger, ILocalClientLogger
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

            var logger = ServiceLocator.Current.GetInstance<ILog>();

            logger.Info(string.Format("{0} - {1} - {2}", DateTime.Now.ToString("hh:mm:ss"), count, message));
        }

        /// <summary>
        ///     The show.
        /// </summary>
        public override void Show()
        {
            // TODO : need to create a logging pane in excel do nothing for now
        }

        #endregion
    }
}