// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UTEventAggregator.cs" company="">
//   
// </copyright>
// <summary>
//   The ut event aggregator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ABM.Common.UnitTests
{
    using System;
    using System.Reactive.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The ut event aggregator.
    /// </summary>
    [TestClass]
    public class UTEventAggregator
    {
        [TestMethod]
        public void Test1()
        {
            IObservable<int> source = Observable.Range(1, 10);
            IDisposable subscription = source.Subscribe(
                x => Console.WriteLine("OnNext: {0}", x),
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnCompleted"));
            Console.WriteLine("Press ENTER to unsubscribe...");
            Console.ReadLine();
            subscription.Dispose();
        }
    }
}