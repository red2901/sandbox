// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ABMLibAddIn.cs" company="">
//   
// </copyright>
// <summary>
//   The qma lib add in.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABMLib
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ExcelDna.Integration;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Common;
    using ABM.Common.ExcelDna;
    using ABM.Data.Services;
    using ABM.Functions;

    /// <summary>
    ///     The qma lib add in.
    /// </summary>
    public class ABMLibAddIn : IExcelAddIn
    {
        #region Static Fields

        /// <summary>
        /// The cached result.
        /// </summary>
        private static ConcurrentDictionary<string, object> CachedResult = new ConcurrentDictionary<string, object>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The qma bloomberg started.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "Status of Bloomberg service.")]
        public static bool ABMBloombergStarted()
        {
            ABMLog("ABMBloombergStarted");
            var bloombergService = ServiceLocator.Current.GetInstance<IBloombergService>();
            return bloombergService.Started;
        }

        /// <summary>
        /// The qma create background fitted bond collection.
        /// </summary>
        /// <param name="collectionName">
        /// The collection name.
        /// </param>
        /// <param name="tickers">
        /// The tickers.
        /// </param>
        /// <param name="benchMarks">
        /// The bench marks.
        /// </param>
        /// <param name="ctds">
        /// The ctds.
        /// </param>
        /// <param name="weights">
        /// The weights.
        /// </param>
        /// <param name="bidPrices">
        /// The bid prices.
        /// </param>
        /// <param name="askPrices">
        /// The ask prices.
        /// </param>
        /// <param name="asof">
        /// The asof.
        /// </param>
        /// <param name="calculateYields">
        /// The calculate yields.
        /// </param>
        /// <param name="anchorPointDates">
        /// The anchor point dates.
        /// </param>
        /// <param name="coeffFlags">
        /// The coeff Flags.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Create an object reference for a fitted bond object collection.")]
        public static object ABMCreateFittedBondCollection(
            [ExcelArgument("CollectionName")] string collectionName, 
            [ExcelArgument("Tickers")] object[,] tickers, 
            [ExcelArgument("BenchMarks")] object[,] benchMarks, 
            [ExcelArgument("CTDs")] object[,] ctds, 
            [ExcelArgument("Weights")] object[,] weights, 
            [ExcelArgument("BidPrices")] object[,] bidPrices, 
            [ExcelArgument("AskPrices")] object[,] askPrices, 
            [ExcelArgument("AsOf")] object asof, 
            [ExcelArgument("Anchor Point Dates")] object[,] anchorPointDates, 
            [ExcelArgument("Coeff Flags")] object[,] coeffFlags)
        {
            if (ABMIsError(
                collectionName, 
                tickers, 
                benchMarks, 
                ctds, 
                weights, 
                bidPrices, 
                askPrices, 
                asof, 
                anchorPointDates, 
                coeffFlags))
            {
                return null;
            }

            if (ExcelDnaUtil.IsInFunctionWizard())
            {
                return string.Empty;
            }

            ABMLog("ABMCreateFittedBondCollection");

            object result = ExcelAsyncUtil.Run(
                "ABMCreateFittedBondCollection", 
                new[]
                    {
                        collectionName, tickers, benchMarks, ctds, weights, bidPrices, askPrices, asof,
                        anchorPointDates, coeffFlags
                    }, 
                delegate
                    {
                        object bondObjCollection = Analytics.ABMCreateFittedBondCollection(
                            collectionName, 
                            tickers, 
                            benchMarks, 
                            ctds, 
                            weights, 
                            bidPrices, 
                            askPrices, 
                            asof, 
                            false, 
                            anchorPointDates, 
                            coeffFlags);

                        return bondObjCollection;
                    });

            return Utils.ReturnCheckedResult(result);
        }

        /// <summary>
        /// The qma yield curve object.
        /// </summary>
        /// <param name="curveName">
        /// The curve name.
        /// </param>
        /// <param name="curveDates">
        /// The curve dates.
        /// </param>
        /// <param name="yields">
        /// The yields.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Create an object reference for a yield curve.")]
        public static object ABMCreateYieldCurveObject(
            [ExcelArgument("Curve Name")] string curveName, 
            [ExcelArgument("Curve Dates")] object[,] curveDates, 
            [ExcelArgument("Curve Yields")] object[,] yields)
        {
            if (ABMIsError(curveName, curveDates, yields))
            {
                return null;
            }

            ABMLog("ABMCreateYieldCurveObject");
            return Analytics.ABMCreateYieldCurveObject(curveName, curveDates, yields);
        }

        /// <summary>
        /// The qma describe argument.
        /// </summary>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "Describe argument passed.", 
            IsMacroType = true)]
        public static string ABMDescribeArgument(object arg)
        {
            ABMLog("ABMDescribeArgument");
            return Utils.DescribeArgument(arg);
        }

        /// <summary>
        /// The qma background fitted bond collection amount outstanding.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the amount outstanding parameters.")]
        public static object ABMFittedBondCollectionAmountOutstanding(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionAmountOutstanding");

            return Analytics.ABMFittedBondCollectionAmountOutstanding(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection bid ask spread.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the bid ask spread parameters.")]
        public static object ABMFittedBondCollectionBidAskSpread(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionBidAskSpread");

            return Analytics.ABMFittedBondCollectionBidAskSpread(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection cheapness.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the cheapness parameters."
            )]
        public static object ABMFittedBondCollectionCheapness(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionCheapness");
            return Analytics.ABMFittedBondCollectionCheapness(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection coeffs.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the regression coefficient parameters.")]
        public static object ABMFittedBondCollectionCoeffs(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMBackgroundFittedBondCollectionCoeffs");

            return Analytics.ABMFittedBondCollectionCoeffs(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection cost.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the convergence cost.")]
        public static object ABMFittedBondCollectionCost(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionCost");

            return Analytics.ABMFittedBondCollectionCost(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection evals.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the number of iteration evaluations.")]
        public static object ABMFittedBondCollectionEvals(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionEvals");

            return Analytics.ABMFittedBondCollectionEvals(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection fitted parameters.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the estimated parameters."
            )]
        public static object ABMFittedBondCollectionFittedParameters(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionFittedParameters");

            return Analytics.ABMFittedBondCollectionFittedParameters(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection fitted yield curve.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the estimated yield curve parameters.")]
        public static object ABMFittedBondCollectionFittedYieldCurve(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionFittedYieldCurve");

            return Analytics.ABMFittedBondCollectionFittedYieldCurve(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection last error message.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the last error message.")]
        public static object ABMFittedBondCollectionLastErrorMessage(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionLastErrorMessage");

            return Analytics.ABMFittedBondCollectionLastErrorMessage(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection last fit length.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the length of time it took to fit.")]
        public static object ABMFittedBondCollectionLastFitLength(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionLastFitLength");
            return Analytics.ABMFittedBondCollectionLastFitLength(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection last fit time.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the last time it was fitted.")]
        public static object ABMFittedBondCollectionLastFitTime(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionLastFitTime");

            return Analytics.ABMFittedBondCollectionLastFitTime(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection maturity.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the bond model yield parameters.")]
        public static object ABMFittedBondCollectionMaturity(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionMaturity");

            return Analytics.ABMFittedBondCollectionMaturity(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection model clean price.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the bond model prices parameters.")]
        public static object ABMFittedBondCollectionModelCleanPrice(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionModelCleanPrice");

            return Analytics.ABMFittedBondCollectionModelCleanPrice(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection model yield.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the bond model yield parameters.")]
        public static object ABMFittedBondCollectionModelYield(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionModelYield");

            return Analytics.ABMFittedBondCollectionModelYield(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection n fit loops.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the number of times the loop has run.")]
        public static object ABMFittedBondCollectionNFitLoops(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionNFitLoops");

            return Analytics.ABMFittedBondCollectionNFitLoops(collectionNameRef);
        }

        /// <summary>
        /// The qma fitted bond collection property.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the convergence status.")]
        public static object ABMFittedBondCollectionProperty(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef, 
            [ExcelArgument("Property")] string property)
        {
            if (ABMIsError(collectionNameRef, property))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionProperty");

            object result = Utils.ReturnCachedResult("ABMFittedBondCollectionProperty", collectionNameRef, property);
            if (result != null)
            {
                return result;
            }

            result = ExcelAsyncUtil.Run(
                "ABMFittedBondCollectionProperty", 
                new[] { collectionNameRef, property }, 
                delegate
                    {
                        object ret = Analytics.ABMFittedBondCollectionProperty(collectionNameRef, property);
                        if (ret is DateTime)
                        {
                            return (ret is DateTime ? (DateTime)ret : new DateTime()).ToOADate();
                        }

                        if (ret is IList<double>)
                        {
                            return (ret is IList<double> ? (IList<double>)ret : new List<double>()).ToObjectArray();
                        }

                        return ret;
                    });

            return Utils.ReturnCheckedResultAndCache(
                result, 
                "ABMFittedBondCollectionProperty", 
                collectionNameRef, 
                property);
        }

        /// <summary>
        /// The qma fitted bond collection property list.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the convergence status.")]
        public static object ABMFittedBondCollectionPropertyList(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            return Analytics.ABMFittedBondCollectionPropertyList(collectionNameRef);
        }

        /// <summary>
        /// The qma fitted bond collection size.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Report the number of iteration evaluations.")]
        public static object ABMFittedBondCollectionSize(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionSize");

            return Analytics.ABMFittedBondCollectionSize(collectionNameRef);
        }

        /// <summary>
        /// The qma background fitted bond collection status.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the convergence status.")]
        public static object ABMFittedBondCollectionStatus(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef)
        {
            if (ABMIsError(collectionNameRef))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionStatus");

            object result = Utils.ReturnCachedResult("ABMFittedBondCollectionStatus", collectionNameRef);
            if (result != null)
            {
                return result;
            }

            result = ExcelAsyncUtil.Run(
                "ABMFittedBondCollectionStatus", 
                new[] { collectionNameRef }, 
                delegate { return Analytics.ABMFittedBondCollectionStatus(collectionNameRef); });

            return Utils.ReturnCheckedResultAndCache(result, "ABMFittedBondCollectionStatus", collectionNameRef);
        }

        /// <summary>
        /// The qma fitted bond model clean price.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <param name="ticker">
        /// The ticker.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the bond model price.")]
        public static object ABMFittedBondModelCleanPrice(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef, 
            [ExcelArgument("Ticker")] string ticker)
        {
            if (ABMIsError(collectionNameRef, ticker))
            {
                return null;
            }

            ABMLog("ABMFittedBondModelCleanPrice");

            object result = Utils.ReturnCachedResult("ABMFittedBondModelCleanPrice", collectionNameRef, ticker);
            if (result != null)
            {
                return result;
            }

            result = ExcelAsyncUtil.Run(
                "ABMFittedBondModelCleanPrice", 
                new[] { collectionNameRef, ticker }, 
                delegate { return Analytics.ABMFittedBondModelCleanPrice(collectionNameRef, ticker); });

            return Utils.ReturnCheckedResultAndCache(result, "ABMFittedBondModelCleanPrice", collectionNameRef, ticker);
        }

        /// <summary>
        /// The qma fitted bond property.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <param name="bondRef">
        /// The bond ref.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMFittedBondProperty(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef, 
            [ExcelArgument("Bond")] string bondRef, 
            [ExcelArgument("Property")] string property)
        {
            if (ABMIsError(collectionNameRef, bondRef, property))
            {
                return null;
            }

            ABMLog("ABMFittedBondCollectionProperty");

            object result = Utils.ReturnCachedResult("ABMFittedBondProperty", collectionNameRef, bondRef, property);
            if (result != null)
            {
                return result;
            }

            result = ExcelAsyncUtil.Run(
                "ABMFittedBondProperty", 
                new[] { collectionNameRef, bondRef, property }, 
                delegate
                    {
                        object ret = Analytics.ABMFittedBondProperty(collectionNameRef, bondRef, property);
                        if (ret is DateTime)
                        {
                            return (ret is DateTime ? (DateTime)ret : new DateTime()).ToOADate();
                        }

                        return ret;
                    });

            return Utils.ReturnCheckedResultAndCache(
                result, 
                "ABMFittedBondProperty", 
                collectionNameRef, 
                bondRef, 
                property);
        }

        /// <summary>
        /// The qma fitted bond property list.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <param name="bondRef">
        /// The bond ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Report the convergence status.")]
        public static object ABMFittedBondPropertyList(
            [ExcelArgument("CollectionNameReference")] string collectionNameRef, 
            [ExcelArgument("BondReference")] string bondRef)
        {
            return Analytics.ABMFittedBondPropertyList(collectionNameRef, bondRef);
        }

        /// <summary>
        /// The qma is error.
        /// </summary>
        /// <param name="objectParamsArray">
        /// The object params array.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ABMIsError(params object[] objectParamsArray)
        {
            for (int i = 0; i < objectParamsArray.Length; i++)
            {
                if (objectParamsArray[i] is ExcelError)
                {
                    return true;
                }

                if (objectParamsArray[i] is string)
                {
                    var s = objectParamsArray[i] as string;
                    if (s.First().Equals("#"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     The qma library client.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "ABMLib client")]
        public static string ABMLibraryClient()
        {
            return string.Format("ExcelDna");
        }

        /// <summary>
        /// The qma log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "Log message to logging output")]
        public static void ABMLog(string message)
        {
            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            logger.Info(message);
        }

        /// <summary>
        ///     The qma logging off.
        /// </summary>
        [ExcelCommand(MenuText = "Logging Off")]
        public static void ABMLoggingOff()
        {
            ABMLog("ABMLoggingOff");
            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            logger.Off();
        }

        /// <summary>
        ///     The toggle logging.
        /// </summary>
        [ExcelCommand(MenuText = "Logging On")]
        public static void ABMLoggingOn()
        {
            ABMLog("ABMLoggingOn");
            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            logger.On();
        }

        /// <summary>
        ///     The qma loggin status.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "Return logging status.")]
        public static bool ABMLoggingStatus()
        {
            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            return logger.Status();
        }

        /// <summary>
        /// The qma multi point interpolation.
        /// </summary>
        /// <param name="targetvector">
        /// The tickervector.
        /// </param>
        /// <param name="xvector">
        /// The xvector.
        /// </param>
        /// <param name="yvector">
        /// The yvector.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Find points in the Y vector which corresponds to the target in the X vector.")]
        public static object ABMMultiPointInterpolation(
            [ExcelArgument("Target Vector")] object targetvector, 
            [ExcelArgument("X Vector")] object[,] xvector, 
            [ExcelArgument("Y Vector")] object[,] yvector)
        {
            if (ABMIsError(targetvector, xvector, yvector))
            {
                return null;
            }

            ABMLog("ABMMultiPointInterpolation");
            return Analytics.ABMMultiPointInterpolation(targetvector, xvector, yvector);
        }

        /// <summary>
        ///     The qma object create.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, 
            Description = "Clear all the object in the object manager cache.")]
        public static void ABMObjectManagerClear()
        {
            ABMLog("ABMObjectManagerClear");
            ObjectManager.ABMObjectManagerClear();
        }

        /// <summary>
        ///     The qma object count.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, Description = "Number of objects stored")]
        public static object ABMObjectManagerCount()
        {
            ABMLog("ABMObjectManagerCount");
            return ObjectManager.ABMObjectManagerCount();
        }

        /// <summary>
        /// The qma object create.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, 
            Description = "Create an object from the excel references.")]
        public static string ABMObjectManagerCreate(
            [ExcelArgument("Key")] string key, 
            [ExcelArgument("Object Data")] object data)
        {
            if (ABMIsError(key, data))
            {
                return null;
            }

            ABMLog("ABMObjectManagerCreate");
            return ObjectManager.ABMObjectManagerCreate(key, data);
        }

        /// <summary>
        ///     The qma object manager list.
        /// </summary>
        /// <param name="outputLocation">
        ///     The output Location.
        /// </param>
        /// <returns>
        ///     Returns the list of stored objects.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, 
            Description = "Return a list of object manager names.")]
        public static object ABMObjectManagerList()
        {
            ABMLog("ABMObjectManagerList");

            return ObjectManager.ABMObjectManagerList().ToObjectArray(true);
        }

        /// <summary>
        /// The qma object manager retrieve.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, 
            Description = "Return of string representation of the object.")]
        public static object ABMObjectManagerRetrieve([ExcelArgument("Key")] string key)
        {
            if (ABMIsError(key))
            {
                return null;
            }

            ABMLog("ABMObjectManagerRetrieve");
            return ObjectManager.ABMObjectManagerRetrieve(key);
        }

        /// <summary>
        /// The qma object property.
        /// </summary>
        /// <param name="objectReference">
        /// The object reference.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, 
            Description = "Return the value of the object property.")]
        public static object ABMObjectProperty(
            [ExcelArgument("Object Reference")] string objectReference, 
            [ExcelArgument("Object Property")] string property)
        {
            if (ABMIsError(objectReference, property))
            {
                return null;
            }

            ABMLog("ABMObjectProperty");
            return Analytics.ABMObjectProperty(objectReference, property);
        }

        /// <summary>
        /// The qma object property list.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.ObjectManagerCategory, 
            Description = "Return of string representation of the object.")]
        public static object ABMObjectPropertyList([ExcelArgument("ObjectReference")] string key)
        {
            if (ABMIsError(key))
            {
                return null;
            }

            ABMLog("ABMObjectPropertyList");
            return ObjectProperty.ABMObjectPropertyList(key);
        }

        /// <summary>
        /// The resize.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "Dynamically resize an array function."
            )]
        public static object ABMResize(object array)
        {
            ABMLog("ABMResize");
            if (array is object[,])
            {
                var caller = XlCall.Excel(XlCall.xlfCaller) as ExcelReference;
                return Utils.Resize(array as object[,], caller);
            }

            return array;
        }

        /// <summary>
        ///     The show log.
        /// </summary>
        [ExcelCommand(MenuText = "Show Log Window")]
        public static void ABMShowLog()
        {
            ABMLog("ABMShowLog");
            var logger = ServiceLocator.Current.GetInstance<ILocalClientLogger>();
            logger.Show();
        }

        /// <summary>
        /// The qma smart solve.
        /// </summary>
        /// <param name="vectorSwitchboard">
        /// The vector switchboard.
        /// </param>
        /// <param name="inputMatrix">
        /// The input matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, Description = "Return the solution to ")]
        public static object ABMSmartSolve(
            [ExcelArgument("Array of 1's and 0's identifying the sub matrix assignment")] double[] vectorSwitchboard, 
            [ExcelArgument("Input Matrix")] double[,] inputMatrix)
        {
            if (ABMIsError(vectorSwitchboard, inputMatrix))
            {
                return null;
            }

            if (ExcelDnaUtil.IsInFunctionWizard())
            {
                return string.Empty;
            }

            DenseMatrix denseInputMatrix = DenseMatrix.OfArray(inputMatrix);

            var xlist = new List<Vector<double>>();
            var ylist = new List<Vector<double>>();
            int matrixSize = vectorSwitchboard.Length;
            int rowcount = 0;
            int colcount = 0;
            var doubleArray = new double[matrixSize, matrixSize];
            var colArrayIndex = new int[matrixSize];
            var rowArrayIndex = new int[matrixSize];
            int ridx = 0;
            int cidx = 0;

            for (int i = 0; i < matrixSize; i++)
            {
                if (Math.Abs(vectorSwitchboard[i]) > 0.001)
                {
                    xlist.Add(denseInputMatrix.Column(i));
                    rowcount += 1;
                    doubleArray[i, i] = 1;
                    colArrayIndex[i] = -1;
                    rowArrayIndex[i] = ridx;
                    ridx += 1;
                }
                else
                {
                    ylist.Add(denseInputMatrix.Column(i));
                    colcount += 1;
                    doubleArray[i, i] = 0;
                    colArrayIndex[i] = cidx;
                    cidx += 1;
                    rowArrayIndex[i] = -1;
                }
            }

            if (rowcount == matrixSize || colcount == matrixSize)
            {
                return doubleArray;
            }

            DenseMatrix X = DenseMatrix.OfColumnVectors(xlist);
            DenseMatrix Y = DenseMatrix.OfColumnVectors(ylist);
            Matrix<double> result = X.Solve(Y);

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    if (rowArrayIndex[i] >= 0 && colArrayIndex[j] >= 0)
                    {
                        doubleArray[i, j] = result[rowArrayIndex[i], colArrayIndex[j]];
                    }
                }
            }

            return doubleArray;
        }

        /// <summary>
        ///     The qma version.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.MiscCategory, Description = "ABMLib version number")]
        public static string ABMVersion()
        {
            ABMLog("ABMVersion");
            return string.Format("ABM Addin Version (ExcelDna) {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        /// <summary>
        /// The qma yield curve yields.
        /// </summary>
        /// <param name="curveReference">
        /// The curve reference.
        /// </param>
        /// <param name="inputDates">
        /// The input dates.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        [ExcelFunction(Category = FunctionCategories.AnalyticsCategory, 
            Description = "Return yields for input date range.")]
        public static object ABMYieldCurveYields(
            [ExcelArgument("Curve Name")] string curveReference, 
            [ExcelArgument("Input Dates")] object[,] inputDates)
        {
            if (ABMIsError(curveReference, inputDates))
            {
                return null;
            }

            if (ExcelDnaUtil.IsInFunctionWizard())
            {
                return string.Empty;
            }

            ABMLog("ABMYieldCurveYields");

            return Analytics.ABMYieldCurveYields(curveReference, inputDates);
        }

        /// <summary>
        ///     The auto close.
        /// </summary>
        public void AutoClose()
        {
            ABMLibAddinSetup.TearDown();
        }

        /// <summary>
        ///     The auto open.
        /// </summary>
        public void AutoOpen()
        {
            ABMLibAddinSetup.Initialise();
        }

        #endregion
    }
}