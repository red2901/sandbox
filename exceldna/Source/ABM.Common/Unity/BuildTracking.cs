// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildTracking.cs" company="">
//   
// </copyright>
// <summary>
//   The build tracking.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// The build tracking.
    /// </summary>
    public class BuildTracking : UnityContainerExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get policy.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IBuildTrackingPolicy"/>.
        /// </returns>
        public static IBuildTrackingPolicy GetPolicy(IBuilderContext context)
        {
            return context.Policies.Get<IBuildTrackingPolicy>(context.BuildKey, true);
        }

        /// <summary>
        /// The set policy.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IBuildTrackingPolicy"/>.
        /// </returns>
        public static IBuildTrackingPolicy SetPolicy(IBuilderContext context)
        {
            IBuildTrackingPolicy policy = new BuildTrackingPolicy();
            context.Policies.SetDefault(policy);
            return policy;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize.
        /// </summary>
        protected override void Initialize()
        {
            this.Context.Strategies.AddNew<BuildTrackingStrategy>(UnityBuildStage.TypeMapping);
        }

        #endregion
    }
}