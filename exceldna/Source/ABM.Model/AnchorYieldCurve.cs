namespace ABM.Model
{
    using System.Collections.Generic;

    using log4net;

    using ABM.Common;

    /// <summary>
    /// The anchor yield curve.
    /// </summary>
    public class AnchorYieldCurve : YieldCurve
    {
        private ILog logger;
        private IEventAggregator eventAggregator;

        public AnchorYieldCurve(ILog logger, IEventAggregator eventAggregator)
            : base(logger, eventAggregator)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }
    }
}