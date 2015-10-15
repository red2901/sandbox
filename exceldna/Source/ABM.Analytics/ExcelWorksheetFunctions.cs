// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelWorksheetFunctions.cs" company="">
//   
// </copyright>
// <summary>
//   The excel worksheet functions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Analytics
{
    using System.Collections.Generic;

    /// <summary>
    /// The excel worksheet functions.
    /// </summary>
    public class ExcelWorksheetFunctions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Equivalent to the Excel match function.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="matchType">
        /// The match type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Match(double target, IList<double> vector, int matchType = 1)
        {
            int idx = 0;
            foreach (double @double in vector)
            {
                switch (matchType)
                {
                    case 0:
                        if (target == @double)
                        {
                            return idx;
                        }

                        break;
                    case 1:
                        if (target <= @double)
                        {
                            return idx;
                        }

                        break;
                    case -1:
                        if (target >= @double)
                        {
                            return idx;
                        }

                        break;
                }

                idx += 1;
            }

            return idx;
        }

        #endregion
    }
}