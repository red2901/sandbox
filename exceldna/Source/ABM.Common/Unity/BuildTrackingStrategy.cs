// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildTrackingStrategy.cs" company="">
//   
// </copyright>
// <summary>
//   The build tracking strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.Unity
{
    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// The build tracking strategy.
    /// </summary>
    public class BuildTrackingStrategy : BuilderStrategy
    {
        #region Public Methods and Operators

        /// <summary>
        /// The post build up.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void PostBuildUp(IBuilderContext context)
        {
            IBuildTrackingPolicy policy = BuildTracking.GetPolicy(context);
            if ((policy != null) && (policy.BuildKeys.Count > 0))
            {
                policy.BuildKeys.Pop();
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
            IBuildTrackingPolicy policy = BuildTracking.GetPolicy(context);
            if (policy == null)
            {
                policy = BuildTracking.SetPolicy(context);
            }

            policy.BuildKeys.Push(context.BuildKey);
        }

        #endregion
    }
}