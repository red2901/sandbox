﻿//------------------------------------------------------------------------------
// <copyright project="Examples" file="HistoricalDataRequest.cs" company="Jordan Robinson">
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

    public static class HistoricalDataRequest
    {
        //Using a Name object allows the API to pre-compute the hash values of strings to more quickly retrieve values
        //  See <WAPI> -> API Best Practices -> Data Request Topics -> #2
        //  I can't find this information in the publicly available documentation.
        //Two equivalent ways to create a Name (new Name(string) or Name.GetName(string))
        private static readonly Name _nAsk = new Name("ASK");
        private static readonly Name _nBid = Name.GetName("BID");
        private static readonly Name _nSecurities = Name.GetName("securities");

        public static void RunExample()
        {
            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = "127.0.0.1";
            sessionOptions.ServerPort = 8194;

            Session session = new Session(sessionOptions);
            if (session.Start() && session.OpenService("//blp/refdata"))
            {
                Service service = session.GetService("//blp/refdata");
                if (service == null)
                {
                    Console.WriteLine("Service is null");
                }
                else
                {
                    Request request = service.CreateRequest("HistoricalDataRequest");

                    //request information for the following securities
                    request.Append(HistoricalDataRequest._nSecurities, "MSFT US EQUITY");
                    request.Append("securities", "C A COMDTY");
                    request.Append("securities", "AAPL 150117C00600000 EQUITY"); //this is a stock option: TICKER yyMMdd[C/P]\d{8} EQUITY

                    //uncomment the following line to see what a request for a nonexistent security looks like
                    //request.Append("securities", "ZIBM US EQUITY");
                    //  My code treats all securities that start with a 'Z' as a nonexistent security

                    //include the following simple fields in the result
                    request.Append("fields", "BID"); //Note that the API will not allow you to use the HistoricalDataRequest._nBid name as a value here.  It expects a string.
                    request.Append("fields", "ASK"); //ditto

                    //uncomment the following line to see what a request for an invalid field looks like
                    //request.Append("fields", "ZBID");
                    //  My code treats all fields that start with a 'Z' as an invalid field

                    //Historical requests allow a few overrides.  See the developer's guide A.2.4 for more information.

                    request.Set("startDate", DateTime.Today.AddMonths(-1).ToString("yyyyMMdd")); //Request that the information start three months ago from today.  This override is required.
                    request.Set("endDate", DateTime.Today.AddDays(10).ToString("yyyyMMdd")); //Request that the information end three days before today.  This is an optional override.  The default is today.

                    //Determine the frequency and calendar type of the output. To be used in conjunction with Period Selection.
                    request.Set("periodicityAdjustment", "CALENDAR"); //Optional string.  Valid values are ACTUAL (default), CALENDAR, and FISCAL.

                    //Determine the frequency of the output. To be used in conjunction with Period Adjustment.
                    request.Set("periodicitySelection", "DAILY"); //Optional string.  Valid values are DAILY (default), WEEKLY, MONTHLY, QUARTERLY, SEMI_ANNUALLY, and YEARLY

                    //Sets quote to Price or Yield for a debt instrument whose default value is quoted in yield (depending on pricing source).
                    request.Set("pricingOption", "PRICING_OPTION_PRICE"); //Optional string.  Valid values are PRICING_OPTION_PRICE (default) and PRICING_OPTION_YIELD

                    //Adjust for "change on day"
                    request.Set("adjustmentNormal", true); //Optional bool. Valid values are true and false (default = false)

                    //Adjusts for Anormal Cash Dividends
                    request.Set("adjustmentAbnormal", false); //Optional bool. Valid values are true and false (default = false)

                    //Capital Changes Defaults
                    request.Set("adjustmentSplit", true); //Optional bool. Valid values are true and false (default = false)

                    //The maximum number of data points to return, starting from the startDate
                    //request.Set("maxDataPoints", 5); //Optional integer.  Valid values are positive integers.  The default is unspecified in which case the response will have all data points between startDate and endDate

                    //Indicates whether to use the average or the closing price in quote calculation.
                    request.Set("overrideOption", "OVERRIDE_OPTION_CLOSE"); //Optional string.  Valid values are OVERRIDE_OPTION_GPA for an average and OVERRIDE_OPTION_CLOSE (default) for the closing price

                    CorrelationID requestID = new CorrelationID(1);
                    session.SendRequest(request, requestID);

                    bool continueToLoop = true;
                    while (continueToLoop)
                    {
                        Event eventObj = session.NextEvent();
                        switch (eventObj.Type)
                        {
                            case Event.EventType.RESPONSE: // final event
                                continueToLoop = false;
                                HistoricalDataRequest.handleResponseEvent(eventObj);
                                break;
                            case Event.EventType.PARTIAL_RESPONSE:
                                HistoricalDataRequest.handleResponseEvent(eventObj);
                                break;
                            default:
                                HistoricalDataRequest.handleOtherEvent(eventObj);
                                break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Cannot connect to server.  Check that the server host is \"localhost\" or \"127.0.0.1\" and that the server port is 8194.");
            }
        }

        private static void handleResponseEvent(Event eventObj)
        {
            Console.WriteLine("EventType = " + eventObj.Type);
            foreach (Message message in eventObj.GetMessages())
            {
                Console.WriteLine();
                Console.WriteLine("correlationID= " + message.CorrelationID);
                Console.WriteLine("messageType = " + message.MessageType);
                
                Element elmSecurityData = message["securityData"];

                Element elmSecurity = elmSecurityData["security"];
                string security = elmSecurity.GetValueAsString();
                Console.WriteLine(security);

                if (elmSecurityData.HasElement("securityError", true))
                {
                    Element elmSecError = elmSecurityData["securityError"];
                    string source = elmSecError.GetElementAsString("source");
                    int code = elmSecError.GetElementAsInt32("code");
                    string category = elmSecError.GetElementAsString("category");
                    string errorMessage = elmSecError.GetElementAsString("message");
                    string subCategory = elmSecError.GetElementAsString("subcategory");

                    Console.Error.WriteLine("security error");
                    Console.Error.WriteLine(string.Format("source = {0}", source));
                    Console.Error.WriteLine(string.Format("code = {0}", code));
                    Console.Error.WriteLine(string.Format("category = {0}", category));
                    Console.Error.WriteLine(string.Format("errorMessage = {0}", errorMessage));
                    Console.Error.WriteLine(string.Format("subcategory = {0}", subCategory));
                }
                else
                {
                    bool hasFieldErrors = elmSecurityData.HasElement("fieldExceptions", true);
                    if (hasFieldErrors)
                    {
                        Element elmFieldErrors = elmSecurityData["fieldExceptions"];
                        for (int errorIndex = 0; errorIndex < elmFieldErrors.NumValues; errorIndex++)
                        {
                            Element fieldError = elmFieldErrors.GetValueAsElement(errorIndex);
                            string fieldId = fieldError.GetElementAsString("fieldId");

                            Element errorInfo = fieldError["errorInfo"];
                            string source = errorInfo.GetElementAsString("source");
                            int code = errorInfo.GetElementAsInt32("code");
                            string category = errorInfo.GetElementAsString("category");
                            string strMessage = errorInfo.GetElementAsString("message");
                            string subCategory = errorInfo.GetElementAsString("subcategory");

                            Console.Error.WriteLine();
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("field error: ");
                            Console.Error.WriteLine(string.Format("\tfieldId = {0}", fieldId));
                            Console.Error.WriteLine(string.Format("\tsource = {0}", source));
                            Console.Error.WriteLine(string.Format("\tcode = {0}", code));
                            Console.Error.WriteLine(string.Format("\tcategory = {0}", category));
                            Console.Error.WriteLine(string.Format("\terrorMessage = {0}", strMessage));
                            Console.Error.WriteLine(string.Format("\tsubCategory = {0}", subCategory));
                        }
                    }

                    Element elmFieldData = elmSecurityData["fieldData"];
                    for (int valueIndex = 0; valueIndex < elmFieldData.NumValues; valueIndex++)
                    {
                        Element elmValues = elmFieldData.GetValueAsElement(valueIndex);
                        DateTime date = elmValues.GetElementAsDate("date").ToSystemDateTime();

                        //You can use either a Name or a string to get elements.
                        double bid = elmValues.GetElementAsFloat64(HistoricalDataRequest._nBid);
                        double ask = elmValues.GetElementAsFloat64(HistoricalDataRequest._nAsk);

                        Console.WriteLine(string.Format("{0:yyyy-MM-dd}: BID = {1}, ASK = {2}", date, bid, ask));
                    }
                }
            }
        }

        private static void handleOtherEvent(Event eventObj)
        {
            Console.WriteLine("EventType=" + eventObj.Type);
            foreach (Message message in eventObj.GetMessages())
            {
                Console.WriteLine("correlationID=" + message.CorrelationID);
                Console.WriteLine("messageType=" + message.MessageType);
                Console.WriteLine(message.ToString());
                if (Event.EventType.SESSION_STATUS == eventObj.Type && message.MessageType.Equals("SessionTerminated"))
                {
                    Console.WriteLine("Terminating: " + message.MessageType);
                }
            }
        }
    }
}
