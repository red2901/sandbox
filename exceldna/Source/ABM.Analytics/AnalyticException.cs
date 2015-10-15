// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyticException.cs" company="">
//   
// </copyright>
// <summary>
//   The analytic exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System;

    /// <summary>
    ///     The analytic exception.
    /// </summary>
    public class AnalyticException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class.
        /// </summary>
        public AnalyticException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public AnalyticException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public AnalyticException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}