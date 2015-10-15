// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCreationStrategy.cs" company="">
//   
// </copyright>
// <summary>
//   The log creation strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.Unity
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using log4net;

    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    ///     The log creation strategy.
    /// </summary>
    public class LogCreationStrategy : BuilderStrategy
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether is policy set.
        /// </summary>
        public bool IsPolicySet { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The post build up.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void PostBuildUp(IBuilderContext context)
        {
            if (this.IsPolicySet)
            {
                context.Policies.Clear<IBuildPlanPolicy>(context.BuildKey);
                this.IsPolicySet = false;
            }
        }

        /// <summary>
        /// The pre build up.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Type typeToBuild = context.BuildKey.Type;
            if (typeof(ILog).Equals(typeToBuild))
            {
                if (context.Policies.Get<IBuildPlanPolicy>(context.BuildKey) == null)
                {
                    Type typeForLog = GetLogType(context);
                    IBuildPlanPolicy policy = new LogBuildPlanPolicy(typeForLog);
                    context.Policies.Set(policy, context.BuildKey);

                    this.IsPolicySet = true;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get log type.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        private static Type GetLogType(IBuilderContext context)
        {
            Type logType = typeof(ILog);
            IBuildTrackingPolicy buildTrackingPolicy = BuildTracking.GetPolicy(context);
            if ((buildTrackingPolicy != null) && (buildTrackingPolicy.BuildKeys.Count >= 2))
            {
                logType = ((NamedTypeBuildKey)buildTrackingPolicy.BuildKeys.ElementAt(1)).Type;
            }
            else
            {
                var stackTrace = new StackTrace();

                // first two are in the log creation strategy, can skip over them
                for (int i = 2; i < stackTrace.FrameCount; i++)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    logType = frame.GetMethod().DeclaringType;

                    // Console.WriteLine(logType.FullName);
                    if (!logType.FullName.StartsWith("Microsoft.Practices"))
                    {
                        break;
                    }
                }
            }

            return logType;
        }

        #endregion
    }
}