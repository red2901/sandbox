﻿//------------------------------------------------------------------------------
// <copyright project="Examples" file="MarketDataRequest.cs" company="Jordan Robinson">
//     Copyright (c) 2013 Jordan Robinson. All rights reserved.
//
//     The use of this software is governed by the Microsoft Public License
//     which is included with this distribution.
// </copyright>
//------------------------------------------------------------------------------

namespace Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Bloomberglp.Blpapi;

    //This declaration specifies that EventHandler refers to Bloomberglp.Blpapi.EventHandler and not System.EventHandler.  The Bloomberg API named this ambiguously.
    using EventHandler = Bloomberglp.Blpapi.EventHandler;

    public static class MarketDataRequest
    {
        public static void RunExample()
        {
            MarketDataRequest._fields = new List<String>();
            MarketDataRequest._fields.Add("BID");
            MarketDataRequest._fields.Add("ASK");

            //uncomment the following line to see what a request for a nonexistent field looks like
            //MarketDataRequest._fields.Add("ZASK");
            //  My code treats all fields that start with a 'Z' as a nonexistent security

            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = "localhost";
            sessionOptions.ServerPort = 8194;

            Session session = new Session(sessionOptions, new EventHandler(MarketDataRequest.ProcessEvent));
            session.StartAsync();
        }

        private static List<string> _fields;

        private static void ProcessEvent(Event evt, Session session)
        {
            switch (evt.Type)
            {
                case Event.EventType.SESSION_STATUS: //use this to open the service
                    foreach (Message message in evt.GetMessages())
                    {
                        if (message.MessageType.Equals("SessionStarted"))
                        {
                            try
                            {
                                session.OpenServiceAsync("//blp/mktdata", new CorrelationID(-9999));
                            }
                            catch (Exception)
                            {
                                Console.Error.WriteLine("Could not open //blp/mktdata for async");
                            }
                        }
                    }
                    break;

                case Event.EventType.SERVICE_STATUS: //use this to subscribe to ticker feeds
                    List<Subscription> slist = new List<Subscription>();
                    
                    //Conflate the data to show every two seconds.
                    //  Please note that the Bloomberg API Emulator code does not treat this exactly correct: individual subscriptions should each have their own interval setting.
                    //  I have not coded that in the emulator.
                    List<string> options = new string[] { "interval=2", "start_time=16:22", "end_time=16:23" }.ToList(); //2 seconds.  //Comment this line to receive a subscription data event whenever it happens in the market.

                    //uncomment the following line to see what a request for a nonexistent security looks like
                    //slist.Add(new Subscription("ZYZZ US EQUITY", MarketDataRequest._fields, options));
                    //  My code treats all securities that start with a 'Z' as a nonexistent security

                    slist.Add(new Subscription("SPY US EQUITY", MarketDataRequest._fields, options));
                    slist.Add(new Subscription("AAPL 150117C00600000 EQUITY", MarketDataRequest._fields, options));

                    session.Subscribe(slist);
                    break;

                case Event.EventType.SUBSCRIPTION_DATA:
                case Event.EventType.RESPONSE:
                case Event.EventType.PARTIAL_RESPONSE:
                    MarketDataRequest.ProcessEvent(evt);
                    break;

                case Event.EventType.SUBSCRIPTION_STATUS:
                    foreach (var msg in evt.GetMessages())
                    {
                        bool fieldExceptionsExist = msg.MessageType.ToString() == "SubscriptionStarted" && msg.HasElement("exceptions", true);
                        bool securityError = msg.MessageType.ToString() == "SubscriptionFailure" && msg.HasElement("reason", true);

                        if (fieldExceptionsExist)
                        {
                            Element elmExceptions = msg["exceptions"];
                            for (int i = 0; i < elmExceptions.NumValues; i++)
                            {
                                Element elmException = elmExceptions.GetValueAsElement(i);
                                string fieldId = elmException.GetElementAsString("fieldId");

                                Element elmReason = elmException["reason"];
                                string source = elmReason.GetElementAsString("source");
                                //int errorCode = elmReason.GetElementAsInt32("errorCode");
                                string category = elmReason.GetElementAsString("category");
                                string description = elmReason.GetElementAsString("description");

                                Console.Error.WriteLine("field error: ");
                                Console.Error.WriteLine(string.Format("\tfieldId = {0}", fieldId));
                                Console.Error.WriteLine(string.Format("\tsource = {0}", source));
                                //Console.Error.WriteLine(string.Format("\terrorCode = {0}", errorCode));
                                Console.Error.WriteLine(string.Format("\tcategory = {0}", category));
                                Console.Error.WriteLine(string.Format("\tdescription = {0}", description));
                            }
                        }
                        else if (securityError)
                        {
                            string security = msg.TopicName;

                            Element elmReason = msg["reason"];
                            string source = elmReason.GetElementAsString("source");
                            int errorCode = elmReason.GetElementAsInt32("errorCode");
                            string category = elmReason.GetElementAsString("category");
                            string description = elmReason.GetElementAsString("description");

                            Console.Error.WriteLine("security not found: ");
                            Console.Error.WriteLine(string.Format("\tsecurity = {0}", security));
                            Console.Error.WriteLine(string.Format("\tsource = {0}", source));
                            Console.Error.WriteLine(string.Format("\terrorCode = {0}", errorCode));
                            Console.Error.WriteLine(string.Format("\tcategory = {0}", category));
                            Console.Error.WriteLine(string.Format("\tdescription = {0}", description));
                        }
                    }
                    break;

                default:
                    foreach (var msg in evt.GetMessages())
                    {
                        Console.WriteLine(msg.ToString());
                    }
                    break;
            }
        }

        private static void ProcessEvent(Event evt)
        {
            const bool excludeNullElements = true;
            foreach (Message message in evt.GetMessages())
            {
                string security = message.TopicName;
                foreach (var field in MarketDataRequest._fields)
                {
                    //This ignores the extraneous fields in the response
                    if (message.HasElement(field, excludeNullElements)) //be careful, excludeNullElements is false by default
                    {
                        Element elmField = message[field];

                        Console.WriteLine(string.Format("{0:HH:mm:ss}: {1}, {2}", 
                            DateTime.Now,
                            security, 
                            elmField.ToString().Trim()));
                    }
                }
            }
            Console.WriteLine();
        }

    }
}