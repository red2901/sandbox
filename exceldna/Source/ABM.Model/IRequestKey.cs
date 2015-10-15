// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestKey.cs" company="">
//   
// </copyright>
// <summary>
//   The RequestKey interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Model
{
    /// <summary>
    /// The RequestKey interface.
    /// </summary>
    public interface IRequestKey
    {
        #region Public Properties

        /// <summary>
        /// Gets the request key.
        /// </summary>
        string RequestKey { get; set; }

        #endregion
    }
}