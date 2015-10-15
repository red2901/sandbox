﻿//------------------------------------------------------------------------------
// <copyright project="BEmu_csh" file="BloombergTypes/Session.cs" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------

namespace Bloomberglp.Blpapi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Threading;

    public class Session
    {
        public enum SubscriptionStatus
        {
            SUBSCRIBING,
            WAITING_FOR_MESSAGES,
            RECEIVING_MESSAGES,
            UNSUBSCRIBED,
            RESUBSCRIBING
        }

        private SessionOptions _sessionOptions;

        #pragma warning disable 0414 //disables the "is assigned but its value is never used" warning
        private enum SessionResponseType { sync, async }
        private readonly SessionResponseType _sessionResponse;

        private enum SessionUriType { undefined, refData, mktData }
        private SessionUriType _sessionUri = SessionUriType.undefined;

        private enum SessionStateType { initialized, started, serviceOpened, connectionError }
        private SessionStateType _sessionState = SessionStateType.initialized;
        #pragma warning restore 0414

        private readonly Queue<Request> _sentRequests;
        private readonly EventHandler _asyncHandler;
        private CorrelationID _asyncOpenCorrelation;
        private readonly List<Subscription> _subscriptions;
        private readonly Timer _marketSimulatorTimer;
        private readonly object _syncroot = new object();

        #region SYNC
        public Session(SessionOptions sessionOptions) //for ReferenceData and HistoricalData (sync)
        {
            this._sessionResponse = SessionResponseType.sync;
            this._sessionOptions = sessionOptions;
            this._sentRequests = new Queue<Request>();
            this._asyncHandler = null;
            this._subscriptions = null;
            this._marketSimulatorTimer = null;
        }

        public bool Start()
        {
            if (this._sessionOptions.ServerPort == 8194 && (this._sessionOptions.ServerHost == "localhost" || this._sessionOptions.ServerHost == "127.0.0.1"))
            {
                this._sessionState = SessionStateType.started;
                return true;
            }
            else
            {
                this._sessionState = SessionStateType.connectionError;
                return false;
            }
        }

        public bool OpenService(string uri)
        {
            if (uri == "//blp/refdata")
            {
                this._sessionState = SessionStateType.serviceOpened;
                return true;
            }
            else
                return false;
        }

        public Service GetService(string uri)
        {
            if (uri == "//blp/refdata")
            {
                this._sessionUri = SessionUriType.refData;
                return new ServiceRefData();
            }
            else
                return null;
        }

        public CorrelationID SendRequest(Request request, CorrelationID correlationId)
        {
            if (request is HistoricalDataRequest.HistoricRequest)
            {
                if (!((HistoricalDataRequest.HistoricRequest)request).DtStart.HasValue)
                    throw new ArgumentException("Historic requests must have start dates");
            }

            request.correlationId = correlationId;
            this._sentRequests.Enqueue(request);
            return correlationId;
        }

        public CorrelationID SendRequest(Request request, EventQueue eventQueue, CorrelationID correlationId)
        {
            eventQueue.Session = this;
            return this.SendRequest(request, correlationId);
        }

        public Event NextEvent()
        {
            if (this._sentRequests.Any())
            {
                bool isLastRequest = this._sentRequests.Count == 1;
                return Event.EventFactory(this._sentRequests.Dequeue(), isLastRequest);
            }
            else
                return null;
        }

        //The actual API blocks until either another event happens or timeoutMillis passes.  I just ignore the timeoutMillis argument here.
        public Event NextEvent(long timeoutMillis)
        {
            return this.NextEvent();
        }
        #endregion

        #region ASYNC
        public Session(SessionOptions sessionOptions, EventHandler handler) //for MarketData (async)
        {
            this._sessionResponse = SessionResponseType.async;
            this._sessionOptions = sessionOptions;
            this._sentRequests = null;
            this._asyncHandler = handler;
            this._subscriptions = new List<Subscription>();
            this._marketSimulatorTimer = new Timer(new TimerCallback(this.MarketSimulatorTimerStep), null, (int)TimeSpan.FromSeconds(0).TotalMilliseconds, Timeout.Infinite);
        }

        public bool StartAsync()
        {
            this._sessionState = SessionStateType.started;
            MarketDataRequest.MarketEvent evtSessionStatus = new MarketDataRequest.MarketEvent(Event.EventType.SESSION_STATUS, null, null);
            MarketDataRequest.MarketEvent evtServiceStatus = new MarketDataRequest.MarketEvent(Event.EventType.SERVICE_STATUS, new CorrelationID(), null);

            if (this._asyncHandler != null)
            {
                this._asyncHandler(evtSessionStatus, this);
                this._asyncHandler(evtServiceStatus, this);
            }
            
            return true;
        }

        public void Stop()
        {
            if (this._marketSimulatorTimer != null)
                this._marketSimulatorTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            this._marketSimulatorTimer.Dispose();
        }

        public void OpenServiceAsync(string uri)
        {
            this._sessionUri = SessionUriType.mktData;
            this._sessionState = SessionStateType.serviceOpened;
            this._asyncOpenCorrelation = new CorrelationID();
        }

        public void OpenServiceAsync(string uri, CorrelationID correlationId)
        {
            this._sessionUri = SessionUriType.mktData;
            this._sessionState = SessionStateType.serviceOpened;
            this._asyncOpenCorrelation = correlationId;
        }

        public void Subscribe(IList<Subscription> subscriptionList)
        {
            lock (this._syncroot) //protect _subscriptions
                this._subscriptions.AddRange(subscriptionList);

            MarketDataRequest.MarketEvent evtSubStatus = new MarketDataRequest.MarketEvent(Event.EventType.SUBSCRIPTION_STATUS, null, subscriptionList);
            if (this._asyncHandler != null)
            {
                this._asyncHandler(evtSubStatus, this);
            }
        }

        public void Cancel(IList<CorrelationID> correlators)
        {
            foreach (var item in correlators)
            {
                this.Cancel(item);
            }
        }

        public void Cancel(CorrelationID corr)
        {
            lock (this._syncroot) //protect _subscriptions
            {
                for (int i = this._subscriptions.Count - 1; i >= 0; i--)
                {
                    if (this._subscriptions[i].CorrelationID.Value == corr.Value && this._subscriptions[i].CorrelationID.IsInternal == corr.IsInternal)
                    {
                        MarketDataRequest.MarketEvent evtSubCancel = new MarketDataRequest.MarketEvent(Event.EventType.SUBSCRIPTION_STATUS, this._subscriptions[i]);
                        if (this._asyncHandler != null)
                        {
                            this._asyncHandler(evtSubCancel, this);
                        }
                        this._subscriptions.RemoveAt(i);
                    }
                }
            }
        }

        [Obsolete("Deprecated as of 3.2.2 and use Cancel(CorrelationID) instead")]
        public void Unsubscribe(CorrelationID correlationId)
        {
            this.Cancel(correlationId);
        }

        public void Unsubscribe(IList<Subscription> subscriptionList)
        {
            this.Cancel(subscriptionList.Select(s => s.CorrelationID).ToList());
        }

        private void MarketSimulatorTimerStep(object arg)
        {
            int? conflationIntervalInMilleseconds = null;

            List<Subscription> subsToUse = new List<Subscription>();
            lock (this._syncroot) //protect _subscriptions
            {
                foreach (var item in this._subscriptions)
                {
                    if (Types.RandomDataGenerator.ShouldIncludeQuote()) //70% chance that I'll send a new quote for the current subscription (after the first response which contains all tickers)
                        subsToUse.Add(item);

                    if (item.ConflationInterval.HasValue)
                        conflationIntervalInMilleseconds = item.ConflationInterval.Value * 1000;
                }
            }

            if (subsToUse.Count > 0)
            {
                MarketDataRequest.MarketEvent evt = new MarketDataRequest.MarketEvent(Event.EventType.SUBSCRIPTION_DATA, null, subsToUse);

                if (this._asyncHandler != null)
                    this._asyncHandler(evt, this);
            }

            TimeSpan ts = Types.RandomDataGenerator.TimeBetweenMarketDataEvents();
            this._marketSimulatorTimer.Change(conflationIntervalInMilleseconds.GetValueOrDefault((int)ts.TotalMilliseconds), Timeout.Infinite);
        }
        #endregion

    }
}
