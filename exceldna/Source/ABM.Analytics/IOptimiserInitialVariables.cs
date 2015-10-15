namespace ABM.Analytics
{
    public interface IOptimiserInitialVariables
    {
        /// <summary>
        ///     Gets or sets the lower bound.
        /// </summary>
        double[] LowerBound { get; set; }

        /// <summary>
        ///     Gets or sets the upper bound.
        /// </summary>
        double[] UpperBound { get; set; }

        /// <summary>
        ///     Gets or sets the x 0.
        /// </summary>
        double[] X0 { get; set; }
    }
}