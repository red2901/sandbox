// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildTrackingPolicy.cs" company="">
//   
// </copyright>
// <summary>
//   The build tracking policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.Unity
{
    using System.Collections.Generic;

    /// <summary>
    ///     The build tracking policy.
    /// </summary>
    public class BuildTrackingPolicy : IBuildTrackingPolicy
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildTrackingPolicy" /> class.
        /// </summary>
        public BuildTrackingPolicy()
        {
            this.BuildKeys = new Stack<object>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the build keys.
        /// </summary>
        public Stack<object> BuildKeys { get; private set; }

        #endregion
    }
}