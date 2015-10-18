// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideElement.cs" company="">
//   
// </copyright>
// <summary>
//   The override element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services.Bloomberg
{
    using System;

    using Bloomberglp.Blpapi;

    /// <summary>
    ///     The override element.
    /// </summary>
    public class OverrideElement
    {
        #region Constants



        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideElement"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public OverrideElement(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideElement"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public OverrideElement(string key, double value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideElement"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public OverrideElement(string key, decimal value)
        {
            this.Key = key;
            this.Value = Convert.ToDouble(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideElement"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public OverrideElement(string key, DateTime value)
        {
            this.Key = key;
            this.Value = value.ToString("yyyyMMdd");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        public object Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The set override.
        /// </summary>
        /// <param name="overridesElement">
        /// The overrides element.
        /// </param>
        public void SetOverride(Element overridesElement)
        {
            Element ore = overridesElement.AppendElement();
            ore.SetElement("fieldId", this.Key);
            if (this.Value is double)
            {
                ore.SetElement("value", Convert.ToDouble(this.Value));
            }

            if (this.Value is string)
            {
                ore.SetElement("value", Convert.ToString(this.Value));
            }
        }

        #endregion
    }
}