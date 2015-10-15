// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpolation.cs" company="">
//   
// </copyright>
// <summary>
//   Interpolation analytics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics
{
    using System.Collections.Generic;

    /// <summary>
    ///     Interpolation analytics.
    /// </summary>
    public class Interpolation
    {
        #region Public Methods and Operators

        /// <summary>
        /// The four point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double FourPoint(double target, IList<double> x, IList<double> y)
        {
            if (target < x[1])
            {
                return -999;
            }

            if (target > x[2])
            {
                return -999;
            }

            if (target == x[1])
            {
                return y[1];
            }

            if (target == x[2])
            {
                return y[2];
            }

            /*
                xVector(1) = xInputVector(1)
                xVector(2) = xInputVector(2)
                xVector(3) = xValue
                xVector(4) = xInputVector(3)
                xVector(5) = xInputVector(4)
    
                yVector(1) = yInputVector(1)
                yVector(2) = yInputVector(2)
    
                yVector(4) = yInputVector(3)
                yVector(5) = yInputVector(4)
        
                alpha = 1 / (xVector(3) - xVector(2))
                beta = (-yVector(2) / (xVector(3) - xVector(2))) - ((yVector(2) - yVector(1)) / (xVector(2) - xVector(1)))
                gamma = (-1 / (xVector(4) - xVector(3))) - (1 / (xVector(3) - xVector(2)))
                lamda = (yVector(4) / (xVector(4) - xVector(3))) + (yVector(2) / (xVector(3) - xVector(2)))
                theta = 1 / (xVector(4) - xVector(3))
                omega = (-yVector(4) / (xVector(4) - xVector(3))) + ((yVector(5) - yVector(4)) / (xVector(5) - xVector(4)))
             */
            double x1 = x[0];
            double x2 = x[1];
            double x3 = target;
            double x4 = x[2];
            double x5 = x[3];

            double y1 = y[0];
            double y2 = y[1];

            double y4 = y[2];
            double y5 = y[3];

            double alpha = 1 / (x3 - x2);
            double beta = (-y2 / (x3 - x2)) - ((y2 - y1) / (x2 - x1));
            double gamma = (-1 / (x4 - x3)) - (1 / (x3 - x2));
            double lambda = (y4 / (x4 - x3)) + (y2 / (x3 - x2));
            double theta = 1 / (x4 - x3);
            double omega = (-y4 / (x4 - x3)) + ((y5 - y4) / (x5 - x4));

            // ((-alpha * beta) - (gamma * lamda) - (theta * omega)) / ((alpha ^ 2) + (gamma ^ 2) + (theta ^ 2))
            return ((-alpha * beta) - (gamma * lambda) - (theta * omega))
                   / ((alpha * alpha) + (gamma * gamma) + (theta * theta));
        }

        /// <summary>
        /// The multi point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double MultiPoint(double target, IList<double> x, IList<double> y)
        {
            /*
                N = Application.WorksheetFunction.Count(xVector)
                If N = 2 Then
                    multiPointInterpolation = linearInterpolation(xValue, xVector, yVector)
                    GoTo Finish
                ElseIf N = 3 Then
                    multiPointInterpolation = threePointInterpolation(xValue, xVector, yVector)
                    GoTo Finish
                ElseIf xValue < xVector(2) Then
                    ReDim xVectorTemp(1 To 3) As Double
                    ReDim yVectorTemp(1 To 3) As Double
                        For i = 1 To 3
                            xVectorTemp(i) = xVector(i)
                            yVectorTemp(i) = yVector(i)
                        Next i
                    multiPointInterpolation = threePointInterpolation(xValue, xVectorTemp, yVectorTemp)
                    GoTo Finish
                ElseIf xValue >= xVector(N - 1) Then
                    ReDim xVectorTemp(1 To 3) As Double
                    ReDim yVectorTemp(1 To 3) As Double
                        For i = 1 To 3
                            xVectorTemp(i) = xVector(N - 3 + i)
                            yVectorTemp(i) = yVector(N - 3 + i)
                        Next i
                    multiPointInterpolation = threePointInterpolation(xValue, xVectorTemp, yVectorTemp)
                    GoTo Finish
                Else
                    ReDim xVectorTemp(1 To 4) As Double
                    ReDim yVectorTemp(1 To 4) As Double
                    Dim M As Integer
                    M = Application.WorksheetFunction.Match(xValue, xVector) + 1
                    For i = 1 To 4
                        xVectorTemp(i) = xVector(M - 3 + i)
                        yVectorTemp(i) = yVector(M - 3 + i)
                    Next i
                    multiPointInterpolation = fourPointInterpolation(xValue, xVectorTemp, yVectorTemp)
                End If
            */
            if (x.Count == 2)
            {
                return TwoPoint(target, x, y);
            }

            if (x.Count == 3)
            {
                return ThreePoint(target, x, y);
            }

            if (target <= x[1])
            {
                return ThreePoint(target, x, y);
            }

            // because count is base 0 not 1
            if (target >= x[x.Count - 1 - 1])
            {
                var xTemp = new List<double>(3);
                var yTemp = new List<double>(3);
                for (int i = 0; i < 3; i++)
                {
                    int idx = x.Count - 3 + i;
                    xTemp.Add(x[idx]);
                    yTemp.Add(y[idx]);
                }

                return ThreePoint(target, xTemp, yTemp);
            }
            else
            {
                int matchIndex = ExcelWorksheetFunctions.Match(target, x) + 1;

                var xTemp = new List<double>(4);
                var yTemp = new List<double>(4);
                for (int i = 0; i < 4; i++)
                {
                    int idx = matchIndex - 3 + i;
                    xTemp.Add(x[idx]);
                    yTemp.Add(y[idx]);
                }

                return FourPoint(target, xTemp, yTemp);
            }
        }

        /// <summary>
        /// The three point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ThreePoint(double target, IList<double> x, IList<double> y)
        {
            if (target < x[0])
            {
                return y[0];
            }

            if (target > x[2])
            {
                return y[2];
            }

            if (target == x[0])
            {
                return y[0];
            }

            if (target == x[1])
            {
                return y[1];
            }

            if (target == x[2])
            {
                return y[2];
            }

            double alpha, beta, gamma, lambda;

            if (target > x[1])
            {
                /*
                    xVector(1) = xInputVector(1)
                    xVector(2) = xInputVector(2)
                    xVector(3) = xValue
                    xVector(4) = xInputVector(3)
                    yVector(1) = yInputVector(1)
                    yVector(2) = yInputVector(2)
                    yVector(4) = yInputVector(3)
            
                    alpha = 1 / (xVector(3) - xVector(2))
                    beta = (-yVector(2) / (xVector(3) - xVector(2))) - ((yVector(2) - yVector(1)) / (xVector(2) - xVector(1)))
                    gamma = (-1 / (xVector(4) - xVector(3))) - (1 / (xVector(3) - xVector(2)))
                    lamda = (yVector(4) / (xVector(4) - xVector(3))) + (yVector(2) / (xVector(3) - xVector(2)))
                 */
                double x1 = x[0];
                double x2 = x[1];
                double x3 = target;
                double x4 = x[2];
                double y1 = y[0];
                double y2 = y[1];
                double y4 = y[2];

                alpha = 1 / (x3 - x2);
                beta = (-y2 / (x3 - x2)) - ((y2 - y1) / (x2 - x1));
                gamma = (-1 / (x4 - x3)) - (1 / (x3 - x2));
                lambda = (y4 / (x4 - x3)) + (y2 / (x3 - x2));
            }
            else
            {
                /*
                    xVector(1) = xInputVector(1)
                    xVector(3) = xInputVector(2)
                    xVector(2) = xValue
                    xVector(4) = xInputVector(3)
                    yVector(1) = yInputVector(1)
                    yVector(3) = yInputVector(2)
                    yVector(4) = yInputVector(3)
            
                    alpha = (-1 / (xVector(3) - xVector(2))) - (1 / (xVector(2) - xVector(1)))
                    beta = (yVector(3) / (xVector(3) - xVector(2))) + (yVector(1) / (xVector(2) - xVector(1)))
                    gamma = 1 / (xVector(3) - xVector(2))
                    lamda = ((yVector(4) - yVector(3)) / (xVector(4) - xVector(3))) - (yVector(3) / (xVector(3) - xVector(2)))
                 */
                double x1 = x[0];
                double x3 = x[1];
                double x2 = target;
                double x4 = x[2];
                double y1 = y[0];
                double y3 = y[1];
                double y4 = y[2];

                alpha = (-1 / (x3 - x2)) - (1 / (x2 - x1));
                beta = (y3 / (x3 - x2)) + (y1 / (x2 - x1));
                gamma = 1 / (x3 - x2);
                lambda = ((y4 - y3) / (x4 - x3)) - (y3 / (x3 - x2));
            }

            // ((-alpha * beta) - (gamma * lamda)) / ((alpha ^ 2) + (gamma ^ 2))
            return ((-alpha * beta) - (gamma * lambda)) / ((alpha * alpha) + (gamma * gamma));
        }

        /// <summary>
        /// The two point or linear interpolation.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        /// <exception cref="AnalyticException">
        /// </exception>
        public static double TwoPoint(double target, IList<double> x, IList<double> y)
        {
            if (target < x[0])
            {
                return y[0];
            }

            if (target > x[1])
            {
                return y[1];
            }

            if (target == x[0])
            {
                return y[0];
            }

            if (target == x[1])
            {
                return y[1];
            }

            // yInputVector(1) + ((yInputVector(2) - yInputVector(1)) * (xValue - xInputVector(1)) / (xInputVector(2) - xInputVector(1)))
            return y[0] + ((y[1] - y[0]) * ((target - x[0]) / (x[1] - x[0])));
        }

        #endregion
    }
}