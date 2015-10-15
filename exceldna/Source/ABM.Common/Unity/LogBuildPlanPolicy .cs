// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogBuildPlanPolicy .cs" company="">
//   
// </copyright>
// <summary>
//   The log build plan policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.Unity
{
    using System;

    using log4net;

    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// The log build plan policy.
    /// </summary>
    public class LogBuildPlanPolicy : IBuildPlanPolicy
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogBuildPlanPolicy"/> class.
        /// </summary>
        /// <param name="logType">
        /// The log type.
        /// </param>
        public LogBuildPlanPolicy(Type logType)
        {
            this.LogType = logType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the log type.
        /// </summary>
        public Type LogType { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The build up.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                ILog log = LogManager.GetLogger(this.LogType);
                context.Existing = log;
            }
        }

        #endregion
    }
}