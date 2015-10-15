// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
//   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.ExcelDna
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using global::ExcelDna.Integration;

    using Microsoft.Office.Interop.Excel;

    /// <summary>
    ///     The extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods and Operators

        public static Range ToRange(this ExcelReference excelReference)
        {
            object app = ExcelDnaUtil.Application;
            object refText = XlCall.Excel(XlCall.xlfReftext, excelReference, true);
            return (Range)app.GetType().InvokeMember("Range", BindingFlags.Public | BindingFlags.GetProperty, null, app, new object[] { refText });
        }

        /// <summary>
        /// The to decimal list.
        /// </summary>
        /// <param name="excelReference">
        /// The excel reference.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<decimal> ToDecimalList(this ExcelReference excelReference)
        {
            return new List<decimal>();
        }

        /// <summary>
        /// The to string key.
        /// </summary>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToReferenceKey(this object arg)
        {
            if (arg is double)
            {
                return arg.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }

            if (arg is string)
            {
                return arg.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }

            if (arg is bool)
            {
                return arg.GetHashCode().ToString();
            }

            if (arg is ExcelError)
            {
                return "ExcelError: " + arg.ToString();
            }

            if (arg is object[,])
            {
                // The object array returned here may contain a mixture of different types,
                // reflecting the different cell contents.
                var matrix = ((object[,])arg);
                var rows = matrix.GetLength(0);
                var cols = matrix.GetLength(1);

                int hc = rows + cols;
                for (int r = 1; r <= rows; r++)
                {
                    for (int c = 1; c <= cols; c++)
                    {
                        hc = unchecked((hc * 314159) + matrix[r, c].GetHashCode());
                    }
                }

                return hc.ToString(CultureInfo.InvariantCulture);

                // return string.Format("Array[{0},{1}]", ((object[,])arg).GetLength(0), ((object[,])arg).GetLength(1));
            }

            if (arg is ExcelMissing)
            {
                return "<<Missing>>"; // Would have been System.Reflection.Missing in previous versions of ExcelDna
            }

            if (arg is ExcelEmpty)
            {
                return "<<Empty>>"; // Would have been null
            }

            if (arg is ExcelReference)
            {
                var excelReference = (ExcelReference)arg;
                var range = excelReference.ToRange();

                // Calling xlfRefText here requires IsMacroType=true for this function.
                return string.Format("{0}[{1}]", XlCall.Excel(XlCall.xlfReftext, arg, true), ToReferenceKey((object)range.Value2));
            }

            return "!? Unheard Of ?!";
        }

        #endregion
    }
}