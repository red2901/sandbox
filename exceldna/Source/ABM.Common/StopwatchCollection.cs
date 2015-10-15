// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopwatchCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The stopwatch collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;

    /// <summary>
    ///     The stopwatch collection.
    /// </summary>
    public class StopwatchCollection
    {
        #region Fields

        /// <summary>
        ///     The stopwatch collection.
        /// </summary>
        private readonly Dictionary<string, Stopwatch> stopwatchCollection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StopwatchCollection" /> class.
        /// </summary>
        public StopwatchCollection()
        {
            this.stopwatchCollection = new Dictionary<string, Stopwatch>();
            this.Keys = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the keys.
        /// </summary>
        public List<string> Keys { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Stopwatch"/>.
        /// </returns>
        public Stopwatch Item(string key)
        {
            if (!this.stopwatchCollection.ContainsKey(key))
            {
                this.Keys.Add(key);
                this.stopwatchCollection[key] = new Stopwatch();
            }

            return this.stopwatchCollection[key];
        }

        /// <summary>
        /// The result.
        /// </summary>
        /// <returns>
        /// The <see cref="OrderedDictionary"/>.
        /// </returns>
        public OrderedDictionary Result()
        {
            var resultDictionary = new OrderedDictionary();
            foreach (string key in this.Keys)
            {
                resultDictionary[key] = this.stopwatchCollection[key].ElapsedMilliseconds;
            }

            return resultDictionary;
        }

        #endregion
    }
}