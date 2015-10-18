// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BondFields.cs" company="">
//   
// </copyright>
// <summary>
//   The bloomberg bond.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Data.Services.Bloomberg
{
    using System.Collections.Generic;

    using Bloomberglp.Blpapi;

    using ABM.Model;

    /// <summary>
    ///     The bloomberg bond.
    /// </summary>
    public class BondFields
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get cash flow from element.
        /// </summary>
        /// <param name="bulkElement">
        /// The bulk element.
        /// </param>
        /// <returns>
        /// The <see cref="CashFlow"/>.
        /// </returns>
        public static CashFlow GetCashFlowFromElement(Element bulkElement)
        {
            int numofBulkElements = bulkElement.NumElements;
            var cashflow = new CashFlow();
            bool hasDetails = false;
            for (int n = 0; n < numofBulkElements; n++)
            {
                Element elem = bulkElement.GetElement(n);
                switch (elem.Datatype)
                {
                    case Schema.Datatype.DATE:
                        hasDetails = true;
                        Datetime paymentDate = elem.GetValueAsDate();
                        cashflow.Date = (double)paymentDate.ToSystemDateTime().ToOADate();
                        break;
                    case Schema.Datatype.FLOAT64:
                        if (n == 1)
                        {
                            hasDetails = true;
                            cashflow.Amount = (double)elem.GetValueAsFloat64();
                        }

                        if (n == 2)
                        {
                            hasDetails = true;
                            cashflow.Principle = (double)elem.GetValueAsFloat64();
                        }

                        break;
                }
            }

            return hasDetails ? cashflow : null;
        }

        /// <summary>
        /// Map fields to bond object.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="bond">
        /// The bond.
        /// </param>
        public static void Map(Element security, Bond bond)
        {
            ICashFlowStream cashflows = bond.CashFlows();

            bond.RequestKey = security.GetElementAsString(Names.SECURITY);
            Element fields = security.GetElement(Names.FIELD_DATA);
            if (fields.NumElements > 0)
            {
                int numElements = fields.NumElements;
                for (int j = 0; j < numElements; ++j)
                {
                    Element field = fields.GetElement(j);

                    if (field.Datatype == Schema.Datatype.SEQUENCE)
                    {
                        for (int n = 0; n < field.NumValues; n++)
                        {
                            CashFlow cashflow = GetCashFlowFromElement(field.GetValueAsElement(n));
                            if (cashflow != null)
                            {
                                cashflows.Add(cashflow);
                            }
                        }
                    }
                    else
                    {
                        if (field.Name.ToString().ToUpper().Equals(Fields.AMT_OUTSTANDING))
                        {
                            bond.AmountOutstanding = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.INT_ACC))
                        {
                            bond.AccruedInterest = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.COUPON))
                        {
                            bond.Coupon = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.DAYS_TO_SETTLE))
                        {
                            bond.DaysToSettle = field.GetValueAsInt32();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.ISSUE_DT))
                        {
                            bond.IssueDate = field.GetValueAsDatetime().ToSystemDateTime();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.MATURITY))
                        {
                            bond.Maturity = field.GetValueAsDatetime().ToSystemDateTime();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.SETTLEMENT_DATE))
                        {
                            bond.SettlementDate = field.GetValueAsDatetime().ToSystemDateTime();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.TICKER))
                        {
                            bond.Ticker = field.GetValueAsString();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.SECURITY_DES))
                        {
                            bond.ShortName = field.GetValueAsString();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.ID_BB_UNIQUE))
                        {
                            bond.UniqueIdString = field.GetValueAsString();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.PX_BID))
                        {
                            bond.Bid = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.PX_ASK))
                        {
                            bond.Ask = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.YLD_YTM_ASK))
                        {
                            bond.YieldAsk = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.YLD_YTM_BID))
                        {
                            bond.YieldBid = field.GetValueAsFloat64();
                        }

                        if (field.Name.ToString().ToUpper().Equals(Fields.YLD_YTM_MID))
                        {
                            bond.YieldMid = field.GetValueAsFloat64();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The market prices field list.
        /// </summary>
        /// <returns>
        /// List of market data fields.
        /// </returns>
        public static IEnumerable<string> MarketDataFieldList()
        {
            return new List<string> { Fields.PX_ASK, Fields.PX_BID };
        }

        /// <summary>
        /// The reference field list.
        /// </summary>
        /// <returns>
        /// List of reference data fields.
        /// </returns>
        public static IEnumerable<string> ReferenceFieldList()
        {
            return new List<string>
                       {
                           Fields.COUPON, 
                           Fields.DAYS_TO_SETTLE, 
                           Fields.DES_CASH_FLOW, 
                           Fields.INT_ACC, 
                           Fields.MATURITY, 
                           Fields.SETTLEMENT_DATE, 
                           Fields.TICKER,
                           Fields.SECURITY_DES,
                           Fields.ID_BB_UNIQUE
                       };
        }

        #endregion
    }
}