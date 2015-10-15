// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOutputLocationCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The OutputLocationCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    /// <summary>
    ///     The OutputLocationCollection interface.
    /// </summary>
    public interface IOutputLocationCollection
    {
        #region Public Methods and Operators

        /// <summary>
        /// The last result.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object LastResult();

        /// <summary>
        /// The result.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Result(object data);

        #endregion
    }
}