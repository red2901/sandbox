// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuildTrackingPolicy .cs" company="">
//   
// </copyright>
// <summary>
//   The BuildTrackingPolicy interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.Unity
{
    using System.Collections.Generic;

    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// The BuildTrackingPolicy interface.
    /// </summary>
    public interface IBuildTrackingPolicy : IBuilderPolicy
    {
        #region Public Properties

        /// <summary>
        /// Gets the build keys.
        /// </summary>
        Stack<object> BuildKeys { get; }

        #endregion
    }
}