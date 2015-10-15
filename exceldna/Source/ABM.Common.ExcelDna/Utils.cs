// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="">
//   
// </copyright>
// <summary>
//   The utils.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.ExcelDna
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    using global::ExcelDna.Integration;

    using global::ExcelDna.Logging;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    ///     The utils.
    /// </summary>
    public class Utils
    {
        #region Static Fields

        /// <summary>
        /// The cached result.
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> CachedResult =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        ///     The resize jobs.
        /// </summary>
        private static readonly Queue<ExcelReference> ResizeJobs = new Queue<ExcelReference>();

        #endregion

        // This function returns a string that describes its argument.
        // For arguments defined as object type, this shows all the possible types that may be received.
        // Also try this function after changing the 
        // [ExcelArgument(AllowReference=true)] attribute.
        // In that case we allow references to be passed (registerd as type R). 
        // By default the function will be registered not
        // to receive references AllowReference=false (type P).
        #region Public Methods and Operators

        /// <summary>
        /// The describe.
        /// </summary>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [ExcelFunction(Description = "Describes the value passed to the function.", IsMacroType = true)]
        public static string DescribeArgument([ExcelArgument(AllowReference = false)] object arg)
        {
            if (arg is double)
            {
                return "Double: " + (double)arg;
            }
            else if (arg is string)
            {
                return "String: " + (string)arg;
            }
            else if (arg is bool)
            {
                return "Boolean: " + (bool)arg;
            }
            else if (arg is ExcelError)
            {
                return "ExcelError: " + arg.ToString();
            }
            else if (arg is object[,])
            {
                // The object array returned here may contain a mixture of different types,
                // reflecting the different cell contents.
                return string.Format("Array[{0},{1}]", ((object[,])arg).GetLength(0), ((object[,])arg).GetLength(1));
            }
            else if (arg is ExcelMissing)
            {
                return "<<Missing>>"; // Would have been System.Reflection.Missing in previous versions of ExcelDna
            }
            else if (arg is ExcelEmpty)
            {
                return "<<Empty>>"; // Would have been null
            }
            else if (arg is ExcelReference)
            {
                // Calling xlfRefText here requires IsMacroType=true for this function.
                return "Reference: " + XlCall.Excel(XlCall.xlfReftext, arg, true);
            }
            else
            {
                return "!? Unheard Of ?!";
            }
        }

        /// <summary>
        /// The error handler.
        /// </summary>
        /// <param name="exceptionObject">
        /// The exception object.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ErrorHandler(object exceptionObject)
        {
            var caller = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);

            // Calling reftext here requires all functions to be marked IsMacroType=true, which is undesirable.
            // A better plan would be to build the reference text oneself, using the RowFirst / ColumnFirst info
            // Not sure where to find the SheetName then....
            var callingName = (string)XlCall.Excel(XlCall.xlfReftext, caller, true);

            LogDisplay.WriteLine(callingName + " Error: " + exceptionObject);

            // return #VALUE into the cell anyway.
            return ExcelError.ExcelErrorValue;
        }

        /// <summary>
        /// The resize.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="arrayFunction">
        /// The array Function.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object Resize(object[,] array, ExcelReference caller, bool arrayFunction = true)
        {
            if (caller == null)
            {
                Debug.Print("Resize - Abandoning - No Caller");
                return array;
            }

            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            if ((caller.RowLast - caller.RowFirst + 1 != rows)
                || (caller.ColumnLast - caller.ColumnFirst + 1 != columns))
            {
                // Size problem: enqueue job, call async update and return #N/A
                EnqueueResize(caller, rows, columns);
                ExcelAsyncUtil.QueueAsMacro(DoResizing);
            }

            // Size is already OK - just return result
            return array;
        }

        /// <summary>
        /// The return cached result.
        /// </summary>
        /// <param name="objectParamsArray">
        /// The object params array.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ReturnCachedResult(params object[] objectParamsArray)
        {
            string key = ObjectParamsAsKey(objectParamsArray);
            if (CachedResult.ContainsKey(key))
            {
                return CachedResult[key];
            }

            return null;
        }

        /// <summary>
        /// The return checked result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ReturnCheckedResult(object result)
        {
            if (Equals(result, ExcelError.ExcelErrorNA))
            {
                return "#Working";
            }

            return result;
        }

        /// <summary>
        /// The return checked result and cache.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="objectParamsArray">
        /// The object params array.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ReturnCheckedResultAndCache(object result, params object[] objectParamsArray)
        {
            if (Equals(result, ExcelError.ExcelErrorNA))
            {
                return "#Working";
            }

            string key = ObjectParamsAsKey(objectParamsArray);

            CachedResult[key] = result;

            return result;
        }

        /// <summary>
        /// The return checked result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ReturnCheckedResultWithTracking(object result)
        {
            var olc = ServiceLocator.Current.GetInstance<IOutputLocationCollection>();
            if (Equals(result, ExcelError.ExcelErrorNA))
            {
                object res = olc.LastResult();
                if (res == null)
                {
                    return res;
                }
                else
                {
                    return "#Working";
                }
            }

            if (result is object[,])
            {
                return olc.Result(result);
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The do resize.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        private static void DoResize(ExcelReference target)
        {
            object oldEcho = XlCall.Excel(XlCall.xlfGetWorkspace, 40);
            object oldCalculationMode = XlCall.Excel(XlCall.xlfGetDocument, 14);
            try
            {
                // Get the current state for reset later
                XlCall.Excel(XlCall.xlcEcho, false);
                XlCall.Excel(XlCall.xlcOptionsCalculation, 3);

                // Get the formula in the first cell of the target
                var formula = (string)XlCall.Excel(XlCall.xlfGetCell, 41, target);
                var firstCell = new ExcelReference(
                    target.RowFirst, 
                    target.RowFirst, 
                    target.ColumnFirst, 
                    target.ColumnFirst, 
                    target.SheetId);

                var isFormulaArray = (bool)XlCall.Excel(XlCall.xlfGetCell, 49, target);
                if (isFormulaArray)
                {
                    object oldSelectionOnActiveSheet = XlCall.Excel(XlCall.xlfSelection);
                    object oldActiveCell = XlCall.Excel(XlCall.xlfActiveCell);

                    // Remember old selection and select the first cell of the target
                    var firstCellSheet = (string)XlCall.Excel(XlCall.xlSheetNm, firstCell);
                    XlCall.Excel(XlCall.xlcWorkbookSelect, new object[] { firstCellSheet });
                    object oldSelectionOnArraySheet = XlCall.Excel(XlCall.xlfSelection);
                    XlCall.Excel(XlCall.xlcFormulaGoto, firstCell);

                    // Extend the selection to the whole array and clear
                    XlCall.Excel(XlCall.xlcSelectSpecial, 6);
                    var oldArray = (ExcelReference)XlCall.Excel(XlCall.xlfSelection);

                    oldArray.SetValue(ExcelEmpty.Value);
                    XlCall.Excel(XlCall.xlcSelect, oldSelectionOnArraySheet);
                    XlCall.Excel(XlCall.xlcFormulaGoto, oldSelectionOnActiveSheet);
                }

                // Get the formula and convert to R1C1 mode
                var isR1C1Mode = (bool)XlCall.Excel(XlCall.xlfGetWorkspace, 4);
                string formulaR1C1 = formula;
                if (!isR1C1Mode)
                {
                    // Set the formula into the whole target
                    formulaR1C1 =
                        (string)
                        XlCall.Excel(XlCall.xlfFormulaConvert, formula, true, false, ExcelMissing.Value, firstCell);
                }

                // Must be R1C1-style references
                object ignoredResult;

                // Debug.Print("Resizing START: " + target.RowLast);
                XlCall.XlReturn retval = XlCall.TryExcel(XlCall.xlcFormulaArray, out ignoredResult, formulaR1C1, target);

                // Debug.Print("Resizing FINISH");

                // TODO: Dummy action to clear the undo stack
                if (retval != XlCall.XlReturn.XlReturnSuccess)
                {
                    // TODO: Consider what to do now!?
                    // Might have failed due to array in the way.
                    firstCell.SetValue("'" + formula);
                }
            }
            finally
            {
                XlCall.Excel(XlCall.xlcEcho, oldEcho);
                XlCall.Excel(XlCall.xlcOptionsCalculation, oldCalculationMode);
            }
        }

        /// <summary>
        ///     The do resizing.
        /// </summary>
        private static void DoResizing()
        {
            var uniqueJobSequence = new List<ExcelReference>();
            ExcelReference lastJob = null;
            while (ResizeJobs.Count > 0)
            {
                ExcelReference currentjob = ResizeJobs.Dequeue();
                if (!currentjob.Equals(lastJob))
                {
                    uniqueJobSequence.Add(currentjob);
                }

                lastJob = currentjob;
            }

            foreach (ExcelReference excelReference in uniqueJobSequence)
            {
                DoResize(excelReference);
            }
        }

        /// <summary>
        /// The enqueue resize.
        /// </summary>
        /// <param name="caller">
        /// The caller.
        /// </param>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        private static void EnqueueResize(ExcelReference caller, int rows, int columns)
        {
            var target = new ExcelReference(
                caller.RowFirst, 
                caller.RowFirst + rows - 1, 
                caller.ColumnFirst, 
                caller.ColumnFirst + columns - 1, 
                caller.SheetId);
            ResizeJobs.Enqueue(target);
        }

        /// <summary>
        /// The object params as key.
        /// </summary>
        /// <param name="objectParamsArray">
        /// The object params array.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ObjectParamsAsKey(object[] objectParamsArray)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < objectParamsArray.Length; i++)
            {
                sb.Append(objectParamsArray[i] as string);
            }

            return sb.ToString();
        }

        #endregion
    }
}