// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LevenbergMarquardt.cs" company="">
//   
// </copyright>
// <summary>
//   The levenbery marquardt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Analytics.Solvers
{
    using System;
    using System.Diagnostics;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Factorization;

    using ABM.Common;

    /// <summary>
    ///     The Levenberg Marquardt algorithm.
    /// </summary>
    public class LevenbergMarquardt : Solver
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LevenbergMarquardt"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="solverOptions">
        /// The solver Options.
        /// </param>
        /// <param name="solverResult">
        /// The solver Result.
        /// </param>
        public LevenbergMarquardt(ILog logger, ISolverOptions solverOptions, ISolverResult solverResult)
        {
            this.logger = logger;

            this.SolverOptions = solverOptions;
            this.SolverResult = solverResult;

            this.LambdaFactor = 10.0;
            this.LambdaInitial = 0.001;
            this.DebugCalculations = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the lambda factor.
        /// </summary>
        public double LambdaFactor { get; set; }

        /// <summary>
        ///     Gets or sets the lambda initial.
        /// </summary>
        public double LambdaInitial { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The estimate.
        /// </summary>
        /// <param name="objectiveValueCollection">
        /// The objective value collection.
        /// </param>
        /// <param name="initialParameters">
        /// The initial parameters.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public override void Estimate(IObjectiveValueCollection objectiveValueCollection, DenseVector initialParameters)
        {
            this.logger.DebugFormat("Estimating ...");
            var sw = new Stopwatch();
            sw.Start();
            this.SolverResult.Start = DateTime.Now;

            bool done = false;
            double lambda = this.LambdaInitial;
            int n = initialParameters.Count;

            this.SolverResult.ParametersCurrent = DenseVector.OfVector(initialParameters);
            this.SolverResult.ParametersNew = new DenseVector(n);

            if (this.DebugCalculations)
            {
                this.logger.Debug("Initial parameters");
                this.logger.Debug(
                    this.SolverResult.ParametersCurrent.ToString(this.SolverResult.ParametersCurrent.Count, 20));
            }

            // get the objective value
            this.SolverResult.ValueCurrent = objectiveValueCollection.ObjectiveValue(
                this.SolverResult.ParametersCurrent);

            this.SolverResult.SaveIteration(this.SolverResult.ValueCurrent, lambda);

            this.logger.DebugFormat("Initial cost {0}", this.SolverResult.ValueCurrent);

            // iterate until we find convergence
            var calculationProfiling = new StopwatchCollection();
            calculationProfiling.Item("iteration").Start();
            while (!done)
            {
                int iteration = this.SolverResult.IterationResults.Count;

                // get the  jacobian
                calculationProfiling.Item("jacobian").Start();
                Matrix<double> jacobian =
                    objectiveValueCollection.ObjectiveValueJacobian(this.SolverResult.ParametersCurrent);
                calculationProfiling.Item("jacobian").Stop();

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} Jacobian", iteration);
                    this.logger.Debug(jacobian.ToString(jacobian.RowCount, jacobian.ColumnCount));
                }

                // get the residuals
                calculationProfiling.Item("residual").Start();
                Vector<double> residual = objectiveValueCollection.Residual(this.SolverResult.ParametersCurrent);
                calculationProfiling.Item("residual").Stop();

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} Residual", iteration);
                    this.logger.Debug(residual.ToString(residual.Count, 20));
                }

                // compute the approximate hessian
                calculationProfiling.Item("hessian").Start();
                Matrix<double> hessian = jacobian.Transpose().Multiply(jacobian);
                calculationProfiling.Item("hessian").Stop();

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} Hessian", iteration);
                    this.logger.Debug(hessian.ToString(hessian.RowCount, hessian.ColumnCount));
                }

                // create diagonal matrix for proper scaling
                calculationProfiling.Item("diagonal").Start();
                var diagonal = new DiagonalMatrix(n, n, hessian.Diagonal().ToArray());
                calculationProfiling.Item("diagonal").Stop();

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} Diagonal", iteration);
                    this.logger.Debug(diagonal.ToString(diagonal.RowCount, diagonal.ColumnCount));
                }

                // compute Levenberg-Marquardt steps
                calculationProfiling.Item("scaledDiagonal").Start();
                Matrix<double> scaledDiagonal = diagonal.Multiply(lambda);
                calculationProfiling.Item("scaledDiagonal").Stop();

                calculationProfiling.Item("hessianAddScaledDiagonal").Start();
                Matrix<double> hessianAddScaledDiagonal = hessian.Add(scaledDiagonal);
                calculationProfiling.Item("hessianAddScaledDiagonal").Stop();

                calculationProfiling.Item("choleskyObject").Start();
                Cholesky<double> choleskyObject = hessianAddScaledDiagonal.Cholesky();
                calculationProfiling.Item("choleskyObject").Stop();

                calculationProfiling.Item("step").Start();
                Vector<double> step = choleskyObject.Solve(jacobian.Transpose().Multiply(residual));
                calculationProfiling.Item("step").Stop();

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} step", iteration);
                    this.logger.Debug(step.ToString(step.Count, 20));
                }

                // update estimated model parameters
                calculationProfiling.Item("subtract").Start();
                this.SolverResult.ParametersCurrent.Subtract(step, this.SolverResult.ParametersNew);
                calculationProfiling.Item("subtract").Stop();

                // contrain to upper and lower bounds
                if (this.SolverOptions.Constrain)
                {
                    this.SolverResult.Constrain(this.SolverOptions.LowerBounds, this.SolverOptions.UpperBounds);
                }

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("{0} Updated model parameters", iteration);
                    this.logger.Debug(
                        this.SolverResult.ParametersNew.ToString(this.SolverResult.ParametersNew.Count, 20));
                }

                calculationProfiling.Item("ObjectiveValue").Start();
                this.SolverResult.ValueNew = objectiveValueCollection.ObjectiveValue(this.SolverResult.ParametersNew);
                calculationProfiling.Item("ObjectiveValue").Stop();

                this.logger.DebugFormat(
                    "{0} - Cost change for lambda {1} => {2} - {3} = {4}",
                    this.SolverResult.IterationResults.Count,
                    lambda,
                    this.SolverResult.ValueCurrent,
                    this.SolverResult.ValueNew,
                    this.SolverResult.ValueCurrent - this.SolverResult.ValueNew);

                done = this.ShouldTerminate();

                if (this.DebugCalculations)
                {
                    switch (this.SolverResult.Status)
                    {
                        case SolverResultStatus.MaximumIterationsReached:
                            this.logger.DebugFormat("{0}", this.SolverResult.Status);
                            break;
                        case SolverResultStatus.MinimumDeltaParametersConverged:
                            double result =
                                this.SolverResult.ParametersNew.Subtract(this.SolverResult.ParametersCurrent).Norm(2.0);
                            this.logger.DebugFormat(
                                "{0} : {1} <= {2}",
                                this.SolverResult.Status,
                                result,
                                this.SolverOptions.MinimumDeltaParameters);
                            break;
                        case SolverResultStatus.MinimumDeltaValueConverged:
                            this.logger.DebugFormat(
                                "{0} : ({1} - {2} <= {3})",
                                this.SolverResult.Status,
                                this.SolverResult.ValueNew,
                                this.SolverResult.ValueCurrent,
                                this.SolverOptions.MinimumDeltaValue);
                            break;
                    }
                }

                if (done)
                {
                    this.logger.Debug("Done.");
                    break;
                }

                this.SolverResult.SaveIteration(this.SolverResult.ValueNew, lambda);
                if (this.SolverResult.ValueChange())
                {
                    // the step have decreased objective function value - decrease lambda
                    lambda = lambda / this.LambdaFactor;
                    this.SolverResult.UpdateParameters();                    
                }
                else
                {
                    // the step have not decreased objective function value - increase lambda
                    lambda = lambda * this.LambdaFactor;                    
                }
                

                if (this.DebugCalculations)
                {
                    this.logger.DebugFormat("New lambda {0}", lambda);
                }
            }
            calculationProfiling.Item("iteration").Stop();

            this.SolverResult.Stop = DateTime.Now;
            sw.Stop();
            this.SolverResult.FittingTime = sw.ElapsedMilliseconds;
            this.SolverResult.CalculationProfile = calculationProfiling.Result();
        }

        #endregion
    }
}