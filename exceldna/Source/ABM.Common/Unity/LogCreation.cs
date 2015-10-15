// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCreation.cs" company="">
//   
// </copyright>
// <summary>
//   The log creation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.Unity
{
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    ///     The log creation.
    /// </summary>
    public class LogCreation : UnityContainerExtension
    {
        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        protected override void Initialize()
        {
            this.Context.Strategies.AddNew<LogCreationStrategy>(UnityBuildStage.PreCreation);
        }

        #endregion
    }
}