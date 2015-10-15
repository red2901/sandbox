// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelDnaOutputLocationCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The output location collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Common.ExcelDna
{
    using System.Collections.Concurrent;

    using global::ExcelDna.Integration;

    /// <summary>
    ///     The output location collection.
    /// </summary>
    public class ExcelDnaOutputLocationCollection : ConcurrentDictionary<ExcelReference, object>, 
                                                    IOutputLocationCollection
    {
        #region Public Methods and Operators

        /// <summary>
        /// The clear range.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <param name="dataArray">
        /// The data array.
        /// </param>
        public void ClearRange(ExcelReference reference, object[,] dataArray)
        {
            if (this.EmptyOrScalar(dataArray))
            {
                return;
            }

            this.ClearReferenceValues(dataArray);

            this.SetReferenceValues(reference, dataArray);
        }

        /// <summary>
        /// The clear reference values.
        /// </summary>
        /// <param name="dataArray">
        /// The data array.
        /// </param>
        public void ClearReferenceValues(object[,] dataArray)
        {
            int rows = dataArray.GetLength(0);
            int cols = dataArray.GetLength(1);

            // clear the output range
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    dataArray[i, j] = null;
                }
            }
        }

        /// <summary>
        /// The empty or scalar.
        /// </summary>
        /// <param name="dataArray">
        /// The data array.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool EmptyOrScalar(object[,] dataArray)
        {
            int rows = dataArray.GetLength(0);
            int cols = dataArray.GetLength(1);

            // output location is zero
            if (rows == 0 || cols == 0)
            {
                return true;
            }

            // if caller and a single object then let the caller handle the value settings
            if (rows == 1 && cols == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The get formula.
        /// </summary>
        /// <param name="firstCell">
        /// The first cell.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFormula(ExcelReference firstCell)
        {
            var formula = (string)XlCall.Excel(XlCall.xlfGetCell, 41, firstCell);
            var isR1C1Mode = (bool)XlCall.Excel(XlCall.xlfGetWorkspace, 4);
            string formulaR1C1 = formula;
            if (!isR1C1Mode)
            {
                object formulaR1C1Obj;
                XlCall.XlReturn formulaR1C1Return = XlCall.TryExcel(
                    XlCall.xlfFormulaConvert, 
                    out formulaR1C1Obj, 
                    formula, 
                    true, 
                    false, 
                    ExcelMissing.Value, 
                    firstCell);
                if (formulaR1C1Return != XlCall.XlReturn.XlReturnSuccess || formulaR1C1Obj is ExcelError)
                {
                    var firstCellAddress = (string)XlCall.Excel(XlCall.xlfReftext, firstCell, true);
                    XlCall.Excel(
                        XlCall.xlcAlert, 
                        "Cannot resize array formula at " + firstCellAddress
                        + " - formula might be too long when converted to R1C1 format.");
                    firstCell.SetValue("'" + formula);
                    return string.Empty;
                }

                formulaR1C1 = (string)formulaR1C1Obj;
            }

            return formulaR1C1;
        }

        /// <summary>
        /// The last result.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object LastResult()
        {
            var reference = XlCall.Excel(XlCall.xlfCaller) as ExcelReference;

            if (reference == null)
            {
                return null;
            }

            object currentData = null;
            if (this.ContainsKey(reference))
            {
                this.TryGetValue(reference, out currentData);
            }

            return currentData;
        }

        /// <summary>
        /// The result.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Result(object data)
        {
            var reference = XlCall.Excel(XlCall.xlfCaller) as ExcelReference;

            if (reference == null)
            {
                return null;
            }

            bool clearData = false;
            bool setData = true;
            object currentData = null;
            if (this.ContainsKey(reference))
            {
                this.TryGetValue(reference, out currentData);
                if (this.ResultChanged(currentData, data))
                {
                    this.TryRemove(reference, out currentData);
                    clearData = true;
                }
                else
                {
                    setData = false;
                }
            }

            this.TryAdd(reference, data);

            if (data as object[,] == null)
            {
                return data;
            }

            if (setData)
            {
                ExcelAsyncUtil.QueueAsMacro(
                    delegate
                    {
                        var firstCell = new ExcelReference(
                            reference.RowFirst,
                            reference.RowFirst,
                            reference.ColumnFirst,
                            reference.ColumnFirst,
                            reference.SheetId);
                        string formula = this.GetFormula(firstCell);

                        var dataArray = data as object[,];
                        if (clearData)
                        {
                            dataArray = this.AdjustArray(data as object[,], currentData as object[,]);
                        }

                        this.SetRange(reference, dataArray);
                        this.SetFirstCell(firstCell, formula);
                    });
            }

            return data;
        }

        /// <summary>
        /// The result changed.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ResultChanged(object a, object b)
        {
            if (a is object[,] && b is object[,])
            {
                var objectArray_a = a as object[,];
                var objectArray_b = b as object[,];

                int rows_a = objectArray_a.GetLength(0);
                int cols_a = objectArray_a.GetLength(1);

                int rows_b = objectArray_b.GetLength(0);
                int cols_b = objectArray_b.GetLength(1);

                if (rows_a != rows_b)
                {
                    return true;
                }

                if (cols_a != cols_b)
                {
                    return true;
                }

                for (int i = 0; i < rows_a; i++)
                {
                    for (int j = 0; j < cols_a; j++)
                    {
                        if (objectArray_a[i, j] == null && objectArray_b[i, j] == null)
                        {
                            return true;
                        }

                        if (objectArray_a[i, j] != null && objectArray_b[i, j] == null)
                        {
                            return false;
                        }

                        if (objectArray_a[i, j] == null && objectArray_b[i, j] != null)
                        {
                            return false;
                        }

                        if (!objectArray_a[i, j].Equals(objectArray_b[i, j]))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// The set first cell.
        /// </summary>
        /// <param name="firstCell">
        /// The first cell.
        /// </param>
        /// <param name="formula">
        /// The formula.
        /// </param>
        public void SetFirstCell(ExcelReference firstCell, string formula)
        {
            XlCall.Excel(XlCall.xlcFormula, formula, firstCell);
        }

        /// <summary>
        /// The set range.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <param name="dataArray">
        /// The data array.
        /// </param>
        public void SetRange(ExcelReference reference, object[,] dataArray)
        {
            if (this.EmptyOrScalar(dataArray))
            {
                return;
            }

            this.SetReferenceValues(reference, dataArray);
        }

        /// <summary>
        /// The set reference values.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <param name="dataArray">
        /// The data array.
        /// </param>
        public void SetReferenceValues(ExcelReference reference, object[,] dataArray)
        {
            int rows = dataArray.GetLength(0);
            int cols = dataArray.GetLength(1);

            var xlr = new ExcelReference(
                reference.RowFirst, 
                reference.RowLast + rows - 1, 
                reference.ColumnFirst, 
                reference.ColumnLast + cols - 1, 
                reference.SheetId);

            using (new ExcelEchoOffHelper())
            using (new ExcelCalculationManualHelper())
            {
                xlr.SetValue(dataArray);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The adjust array.
        /// </summary>
        /// <param name="newdata">
        /// The newdata.
        /// </param>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        private object[,] AdjustArray(object[,] newdata, object[,] current)
        {
            int nrows = newdata.GetLength(0);
            int ncols = newdata.GetLength(1);

            int orows = current.GetLength(0);
            int ocols = current.GetLength(1);

            if (nrows >= orows)
            {
                return newdata;
            }

            for (int i = 0; i < orows; i++)
            {
                for (int j = 0; j < ocols; j++)
                {
                    if (i < nrows && j < ncols)
                    {
                        current[i, j] = newdata[i, j];
                    }
                    else
                    {
                        current[i, j] = string.Empty;
                    }
                }
            }

            return current;
        }

        #endregion
    }
}