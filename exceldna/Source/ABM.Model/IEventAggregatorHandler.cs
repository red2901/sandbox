namespace ABM.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IEventAggregatorHandler : IDisposable
    {
        IList<IDisposable> Subscriptions { get; }

        void InitialiseSubscriptions();
    }
}