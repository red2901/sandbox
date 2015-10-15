// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Analytics.cs" company="">
//   
// </copyright>
// <summary>
//   The analytics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Factorization;
    using MathNet.Numerics.LinearRegression;

    using Microsoft.CSharp;
    using Microsoft.Practices.ServiceLocation;

    using ABM.Analytics;
    using ABM.Common;
    using ABM.Data.Services;
    using ABM.Model;

    /// <summary>
    ///     The analytics.
    /// </summary>
    public class Analytics
    {
        #region Public Methods and Operators

        

        /// <summary>
        /// The qma bond object.
        /// </summary>
        /// <param name="bondName">
        /// The bond Name.
        /// </param>
        /// <param name="requestTicker">
        /// The request Ticker.
        /// </param>
        /// <param name="calcObjectRef">
        /// The calc Object Ref.
        /// </param>
        /// <param name="coeffObjectRef">
        /// The coeff Object Ref.
        /// </param>
        /// <param name="calculateYields">
        /// The calculate Yields.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMCreateBondObject(
            string bondName, 
            string requestTicker,
            string coeffObjectRef, 
            bool calculateYields)
        {
            IBondRegressionCoefficients coeffObject = null;


            if (coeffObjectRef != null)
            {
                coeffObject = (IBondRegressionCoefficients)ObjectManager.ABMObjectManagerRetrieve(coeffObjectRef);
            }

            if (coeffObject == null)
            {
                return null;
            }

            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(bondName);

            var dataFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();
            if (mo == null)
            {
                Bond bond = dataFactory.GetBond(requestTicker);
                bond.SetRegressionCoefficients(coeffObject);

                if (calculateYields)
                {
                    dataFactory.GetMarketYields(bond);
                }

                return ObjectManager.ABMObjectManagerCreate(bondName, bond);
            }
            else
            {
                var bond = (Bond)mo.Object;

                bool haschanged = false;
                if (calculateYields && bond.YieldInputsChanged)
                {
                    dataFactory.GetMarketYields(bond);
                }

                if (bond.Coefficients() != null && !bond.Coefficients().Equals(coeffObject))
                {
                    bond.SetRegressionCoefficients(coeffObject);
                    haschanged = true;
                }

                if (haschanged)
                {
                    mo.UpdateVersion();
                }
            }

            return mo.ToString();
        }

        /// <summary>
        /// The qma bond regression coeffients object.
        /// </summary>
        /// <param name="coeffName">
        /// The coeff name.
        /// </param>
        /// <param name="amountOutstanding">
        /// The amount outstanding.
        /// </param>
        /// <param name="benchmark">
        /// The benchmark.
        /// </param>
        /// <param name="bidAskSpread">
        /// The bid ask spread.
        /// </param>
        /// <param name="ctd">
        /// The ctd.
        /// </param>
        /// <param name="issueDate">
        /// The issue date.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMCreateBondRegressionCoeffientsObject(
            string coeffName, 
            object amountOutstanding, 
            object benchmark, 
            object bidAskSpread, 
            object ctd, 
            object issueDate)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(coeffName);

            double amountOutstandingVar = Convert.ToDouble(amountOutstanding);
            double benchmarkVar = Convert.ToDouble(benchmark);
            double bidAskSpreadVar = Convert.ToDouble(bidAskSpread);
            double ctdVar = Convert.ToDouble(ctd);
            double issueDateYearFractionVar = Convert.ToDouble(issueDate);

            if (mo == null)
            {
                return ObjectManager.ABMObjectManagerCreate(
                    coeffName, 
                    new BondRegressionCoefficients
                        {
                            AmountOutstanding = amountOutstandingVar, 
                            Benchmark = benchmarkVar, 
                            BidAskSpread = bidAskSpreadVar, 
                            Ctd = ctdVar, 
                            IssueDateYearFraction = issueDateYearFractionVar
                        });
            }

            var bondRregressionCoeffs = (BondRegressionCoefficients)mo.Object;
            if (bondRregressionCoeffs.Update(
                amountOutstandingVar, 
                benchmarkVar, 
                bidAskSpreadVar, 
                ctdVar, 
                issueDateYearFractionVar))
            {
                mo.UpdateVersion();
            }

            return mo.ToString();
        }

        /// <summary>
        /// The qma discount curve object.
        /// </summary>
        /// <param name="curveName">
        /// The curve name.
        /// </param>
        /// <param name="curveDates">
        /// The curve dates.
        /// </param>
        /// <param name="discountFactors">
        /// The discount factors.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMCreateDiscountCurveObject(
            string curveName, 
            object[,] curveDates, 
            object[,] discountFactors)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(curveName);

            List<double> curveDateList = curveDates.ToDoubleListFromObjectMatrix();
            List<double> discountFactorList = discountFactors.ToDoubleListFromObjectMatrix();

            if (curveDateList.Count != discountFactorList.Count)
            {
                return null;
            }

            if (mo == null)
            {
                var discountCurveObject = ServiceLocator.Current.GetInstance<IYieldCurve>(YieldCurveType.Discount.ToString());
                discountCurveObject.CurveDates = curveDateList;
                discountCurveObject.DiscountFactors = discountFactorList;
                return ObjectManager.ABMObjectManagerCreate(curveName, discountCurveObject);
            }

            var discountCurve = (IYieldCurve)mo.Object;

            if (discountCurve.Update(curveDateList, discountFactorList))
            {
                mo.UpdateVersion();
            }

            return mo.ToString();
        }

        /// <summary>
        /// The qma create background bond object collection.
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
        /// The extra Parameters.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMCreateFittedBondCollection(
            string collectionName, 
            object[,] tickers, 
            object[,] benchMarks, 
            object[,] ctds, 
            object[,] weights, 
            object[,] bidPrices, 
            object[,] askPrices, 
            object asof, 
            bool calculateYields, 
            object[,] anchorPointDates, 
            object[,] coeffFlags)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(collectionName);
            var dataFactory = ServiceLocator.Current.GetInstance<IInstrumentFactory>();

            // check that all the arrays are of the same size
            int rows = tickers.GetLength(0);
            if (rows != benchMarks.GetLength(0))
            {
                return null;
            }

            if (rows != ctds.GetLength(0))
            {
                return null;
            }

            if (rows != weights.GetLength(0))
            {
                return null;
            }

            if (rows != bidPrices.GetLength(0))
            {
                return null;
            }

            if (rows != askPrices.GetLength(0))
            {
                return null;
            }

            // convert to list
            IList<string> tickerStringList;
            IList<double> benchMarkList;
            IList<double> ctdList;
            IList<double> weightList;
            IList<double> bidPriceList;
            IList<double> askPriceList;
            IList<double> anchorDates;
            IList<bool> coeffFlagList;

            try
            {
                tickerStringList = tickers.ToStringListFromObjectMatrix();
                benchMarkList = benchMarks.ToDoubleListFromObjectMatrix();
                ctdList = ctds.ToDoubleListFromObjectMatrix();
                weightList = weights.ToDoubleListFromObjectMatrix();
                bidPriceList = bidPrices.ToDoubleListFromObjectMatrix();
                askPriceList = askPrices.ToDoubleListFromObjectMatrix();
                anchorDates = anchorPointDates.ToDoubleListFromObjectMatrix();
                coeffFlagList = coeffFlags.ToBooleanListFromObjectMatrix();
            }
            catch (Exception e)
            {
                return string.Format("Input error : {0}", e.Message);
            }

            if (mo == null)
            {
                IFittedBondCollection bondCollection = dataFactory.CreateBackgroundBondObjectCollection(
                    tickerStringList, 
                    benchMarkList, 
                    ctdList, 
                    weightList, 
                    bidPriceList, 
                    askPriceList, 
                    asof, 
                    calculateYields, 
                    anchorDates, 
                    coeffFlagList);

                bondCollection.Start();

                return ObjectManager.ABMObjectManagerCreate(collectionName, bondCollection);
            }

            var backgroundBondCollection = (Data.Services.FittedBondCollection)mo.Object;

            try
            {
                bool haschanged = backgroundBondCollection.Update(
                    tickerStringList, 
                    benchMarkList, 
                    ctdList, 
                    weightList, 
                    bidPriceList, 
                    askPriceList, 
                    asof, 
                    calculateYields, 
                    anchorDates, 
                    coeffFlagList);

                if (haschanged)
                {
                    mo.UpdateVersion();
                }
            }
            catch (Exception e)
            {
                return string.Format("Update Failed : {0}", e.Message);
            }

            return mo.ToString();
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
        public static object ABMCreateYieldCurveObject(string curveName, object[,] curveDates, object[,] yields)
        {
            var mo = (ManagedObject)ObjectManager.ABMObjectManagerRetrieveManagedObject(curveName);

            List<double> curveDateList = curveDates.ToDoubleListFromObjectMatrix();
            List<double> yieldList = yields.ToDoubleListFromObjectMatrix();

            if (curveDateList.Count != yieldList.Count)
            {
                return null;
            }

            if (mo == null)
            {
                var newYieldCurve = ServiceLocator.Current.GetInstance<IYieldCurve>(YieldCurveType.Yield.ToString());
                newYieldCurve.CurveDates = curveDateList;
                newYieldCurve.Yields = yieldList;
                return ObjectManager.ABMObjectManagerCreate(curveName, newYieldCurve);
            }

            var yieldCurve = (YieldCurve)mo.Object;
            if (yieldCurve.Update(curveDateList, yieldList))
            {
                mo.UpdateVersion();
            }

            return mo.ToString();
        }

        /// <summary>
        /// The qma discount curve discount factor.
        /// </summary>
        /// <param name="curveReference">
        /// The curve Reference.
        /// </param>
        /// <param name="curveDates">
        /// The curve Dates.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMDiscountCurveDiscountFactors(string curveReference, object[,] curveDates)
        {
            var discountCurve = (DiscountCurve)ObjectManager.ABMObjectManagerRetrieve(curveReference);
            List<double> inputDateList = curveDates.ToDoubleListFromObjectMatrix();
            IEnumerable<double> discountFactors = discountCurve.DiscountFactorList(inputDateList);

            var returnObject = new object[inputDateList.Count, 1];

            int i = 0;
            foreach (double df in discountFactors)
            {
                returnObject[i, 0] = df;
                i++;
            }

            return returnObject;
        }

        /// <summary>
        /// The qma background fitted bond collection amount outstanding.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionAmountOutstanding(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.AmountOutstandingInBillions().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection bid ask spread.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionBidAskSpread(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.BidAskSpread().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection cheapness.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionCheapness(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.Cheapness().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection coeffs.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The coeffs.
        /// </returns>
        public static object[,] ABMFittedBondCollectionCoeffs(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);

            if (collection == null)
            {
                return null;
            }

            if (collection.Coeffs() == null)
            {
                return null;
            }

            return collection.Coeffs().ToObjectArray();
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
        public static object ABMFittedBondCollectionCost(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.Cost;
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
        public static object ABMFittedBondCollectionEvals(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.Evals;
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
        public static object[,] ABMFittedBondCollectionFittedParameters(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.FittedParameters().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection fitted yield curve.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionFittedYieldCurve(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.FittedAnchorYields.ToObjectArray();
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
        public static object ABMFittedBondCollectionLastErrorMessage(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.LastErrorMessage;
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
        public static object ABMFittedBondCollectionLastFitLength(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.LastFitLength / 1000.0;
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
        public static object ABMFittedBondCollectionLastFitTime(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.LastFitTime.ToOADate();
        }

        /// <summary>
        /// The qma background fitted bond collection maturity.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionMaturity(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.Maturity().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection model clean price.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionModelCleanPrice(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.ModelCleanPrice().ToObjectArray();
        }

        /// <summary>
        /// The qma background fitted bond collection model yield.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object[,]"/>.
        /// </returns>
        public static object[,] ABMFittedBondCollectionModelYield(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.ModelYield().ToObjectArray();
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
        public static object ABMFittedBondCollectionNFitLoops(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.NFitLoops;
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
        public static object ABMFittedBondCollectionSize(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            return collection.Size;
        }

        /// <summary>
        /// The qma background fitted bond collection start.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMFittedBondCollectionStart(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            try
            {
                return collection.Start() ? DateTime.Now.ToString("hh:mm:ss tt") : "Start Failed";
            }
            catch (Exception e)
            {
                return string.Format("Start Failed : {0}", e.Message);
            }
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
        public static object ABMFittedBondCollectionStatus(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? null : collection.Status;
        }

        public static object ABMFittedBondCollectionProperty(string collectionNameRef, string property)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            var propInfo = collection.GetType().GetProperty(property);

            if (propInfo == null)
            {
                return null;
            }

            return propInfo.GetValue(collection, null);
        }

        public static object ABMFittedBondProperty(string collectionNameRef, string bondRef, string property)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            var bond = collection.GetBond(bondRef);

            var propInfo = bond.GetType().GetProperty(property);

            if (propInfo == null)
            {
                return null;
            }

            return propInfo.GetValue(bond, null);
        }

        public static object ABMFittedBondPropertyList(string collectionNameRef, string bondRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            var bond = collection.GetBond(bondRef);

            var propInfos = bond.GetType().GetProperties();

            var result = new List<string>();
            foreach (var propertyInfo in propInfos)
            {
                result.Add(propertyInfo.Name);
            }

            return result;
        }

        public static object ABMFittedBondCollectionPropertyList(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            var propInfos = collection.GetType().GetProperties();

            var result = new List<string>();
            foreach (var propertyInfo in propInfos)
            {
                result.Add(propertyInfo.Name);
            }

            return result;
        }

        /// <summary>
        /// The qma background fitted bond collection stop.
        /// </summary>
        /// <param name="collectionNameRef">
        /// The collection name ref.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMFittedBondCollectionStop(string collectionNameRef)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            if (collection == null)
            {
                return null;
            }

            try
            {
                return collection.Stop() ? DateTime.Now.ToString("hh:mm:ss tt") : "Stop Failed";
            }
            catch (Exception e)
            {
                return string.Format("Stop Failed : {0}", e.Message);
            }
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
        public static object ABMFittedBondModelCleanPrice(string collectionNameRef, string ticker)
        {
            var collection = (IFittedBondCollection)ObjectManager.ABMObjectManagerRetrieve(collectionNameRef);
            return collection == null ? (object)null : collection.ModelCleanPrice(ticker);
        }

        /// <summary>
        /// The qma linear equation solve.
        /// </summary>
        /// <param name="A">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMLinearEquationSolve(double[,] A, double[] b)
        {
            return A.ToDenseObject().Solve(b.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix add matrix.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixAddMatrix(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().Add(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix add scalar.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixAddScalar(double[,] matrix, double scalar)
        {
            return matrix.ToDenseObject().Add(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix column absolute sums.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixColumnAbsoluteSums(double[,] matrix)
        {
            return matrix.ToDenseObject().ColumnAbsoluteSums().ToArray();
        }

        /// <summary>
        /// The qma matrix column norms.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="norm">
        /// The norm.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixColumnNorms(double[,] matrix, double norm)
        {
            return matrix.ToDenseObject().ColumnNorms(norm).ToArray();
        }

        /// <summary>
        /// The qma matrix column sums.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixColumnSums(double[,] matrix)
        {
            return matrix.ToDenseObject().ColumnSums().ToArray();
        }

        /// <summary>
        /// The qma matrix condition number.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixConditionNumber(double[,] matrix)
        {
            return matrix.ToDenseObject().ConditionNumber();
        }

        /// <summary>
        /// The qma matrix conjugate.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixConjugate(double[,] matrix)
        {
            return matrix.ToDenseObject().Conjugate().ToArray();
        }

        /// <summary>
        /// The qma matrix conjugate transpose.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixConjugateTranspose(double[,] matrix)
        {
            return matrix.ToDenseObject().ConjugateTranspose().ToArray();
        }

        /// <summary>
        /// The qma matrix conjugate transpose and multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixConjugateTransposeAndMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().ConjugateTransposeAndMultiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix conjugate transpose this and multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixConjugateTransposeThisAndMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().ConjugateTransposeThisAndMultiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix create.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixCreate(int rows, int cols)
        {
            return Matrix<double>.Build.Dense(rows, cols).ToArray();
        }

        /// <summary>
        /// The qma matrix create and fill diagonal.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixCreateAndFillDiagonal(int rows, int cols, double value)
        {
            return Matrix<double>.Build.DenseDiagonal(rows, cols, value).ToArray();
        }

        /// <summary>
        /// The qma matrix create and fill with random standard distribution.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixCreateAndFillWithRandomStandardDistribution(int rows, int cols)
        {
            return Matrix<double>.Build.Random(rows, cols).ToArray();
        }

        /// <summary>
        /// The qma matrix determinant.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixDeterminant(double[,] matrix)
        {
            try
            {
                return matrix.ToDenseObject().Determinant();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// The qma matrix divide.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixDivide(double[,] matrix1, double scalar)
        {
            return matrix1.ToDenseObject().Divide(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix divide by this.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixDivideByThis(double[,] matrix1, double scalar)
        {
            return matrix1.ToDenseObject().DivideByThis(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix eigen values.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixEigenValues(double[,] matrix)
        {
            Evd<double> evd = matrix.ToDenseObject().Evd();
            return evd.EigenValues.ToRealArray();
        }

        /// <summary>
        /// The qma matrix eigen vectors.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixEigenVectors(double[,] matrix)
        {
            return matrix.ToDenseObject().Evd().EigenVectors.ToArray();
        }

        /// <summary>
        /// The qma matrix frobenius norm.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static double ABMMatrixFrobeniusNorm(double[,] matrix)
        {
            return matrix.ToDenseObject().FrobeniusNorm();
        }

        /// <summary>
        /// The qma matrix infinity norm.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static double ABMMatrixInfinityNorm(double[,] matrix)
        {
            return matrix.ToDenseObject().InfinityNorm();
        }

        /// <summary>
        /// The qma matrix invert.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixInvert(double[,] matrix)
        {
            return matrix.ToDenseObject().Inverse().ToArray();
        }

        /// <summary>
        /// The qma matrix kronecker product.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixKroneckerProduct(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().KroneckerProduct(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix l 1 norm.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixL1Norm(double[,] matrix)
        {
            return matrix.ToDenseObject().L1Norm();
        }

        /// <summary>
        /// The qma matrix l 2 norm.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixL2Norm(double[,] matrix)
        {
            return matrix.ToDenseObject().L2Norm();
        }

        /// <summary>
        /// The qma matrix left multiply vector.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixLeftMultiplyVector(double[,] matrix1, double[] vector)
        {
            return matrix1.ToDenseObject().LeftMultiply(vector.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix modulus.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixModulus(double[,] matrix, double scalar)
        {
            return matrix.ToDenseObject().Modulus(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix modulus by this.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixModulusByThis(double[,] matrix, double divisor)
        {
            return matrix.ToDenseObject().ModulusByThis(divisor).ToArray();
        }

        /// <summary>
        /// The qma matrix multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().Multiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix multiply scalar.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixMultiplyScalar(double[,] matrix1, double scalar)
        {
            return matrix1.ToDenseObject().Multiply(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix multiply vector.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixMultiplyVector(double[,] matrix1, double[] vector)
        {
            return matrix1.ToDenseObject().Multiply(vector.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix negate.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixNegate(double[,] matrix)
        {
            return matrix.ToDenseObject().Negate().ToArray();
        }

        /// <summary>
        /// The qma matrix normalize columns.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="norm">
        /// The norm.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixNormalizeColumns(double[,] matrix, double norm)
        {
            return matrix.ToDenseObject().NormalizeColumns(norm).ToArray();
        }

        /// <summary>
        /// The qma matrix normalize rows.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="norm">
        /// The norm.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixNormalizeRows(double[,] matrix, double norm)
        {
            return matrix.ToDenseObject().NormalizeRows(norm).ToArray();
        }

        /// <summary>
        /// The qma matrix nullity.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixNullity(double[,] matrix)
        {
            return matrix.ToDenseObject().Nullity();
        }

        /// <summary>
        /// The qma matrix pointwise divide.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseDivide(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().PointwiseDivide(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise exp.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseExp(double[,] matrix)
        {
            return matrix.ToDenseObject().PointwiseExp().ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise log.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseLog(double[,] matrix)
        {
            return matrix.ToDenseObject().PointwiseLog().ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise modulus.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseModulus(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().PointwiseModulus(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().PointwiseMultiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise power.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwisePower(double[,] matrix, int exponent)
        {
            return matrix.ToDenseObject().PointwisePower(exponent).ToArray();
        }

        /// <summary>
        /// The qma matrix pointwise remainder.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPointwiseRemainder(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().PointwiseRemainder(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix power.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixPower(double[,] matrix, int exponent)
        {
            return matrix.ToDenseObject().Power(exponent).ToArray();
        }

        /// <summary>
        /// The qma matrix rank.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ABMMatrixRank(double[,] matrix)
        {
            return matrix.ToDenseObject().Rank();
        }

        /// <summary>
        /// The qma matrix remainder.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixRemainder(double[,] matrix, double divisor)
        {
            return matrix.ToDenseObject().Remainder(divisor).ToArray();
        }

        /// <summary>
        /// The qma matrix remainder by this.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="divisor">
        /// The divisor.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixRemainderByThis(double[,] matrix, double divisor)
        {
            return matrix.ToDenseObject().RemainderByThis(divisor).ToArray();
        }

        /// <summary>
        /// The qma matrix row absolute sums.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixRowAbsoluteSums(double[,] matrix)
        {
            return matrix.ToDenseObject().RowAbsoluteSums().ToArray();
        }

        /// <summary>
        /// The qma matrix row norms.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="norm">
        /// The norm.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixRowNorms(double[,] matrix, double norm)
        {
            return matrix.ToDenseObject().RowNorms(norm).ToArray();
        }

        /// <summary>
        /// The qma matrix row sums.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMMatrixRowSums(double[,] matrix)
        {
            return matrix.ToDenseObject().RowSums().ToArray();
        }

        /// <summary>
        /// The qma matrix subtract.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixSubtract(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().Subtract(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix subtract scalar.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixSubtractScalar(double[,] matrix1, double scalar)
        {
            return matrix1.ToDenseObject().Subtract(scalar).ToArray();
        }

        /// <summary>
        /// The qma matrix trace.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static double ABMMatrixTrace(double[,] matrix)
        {
            return matrix.ToDenseObject().Trace();
        }

        /// <summary>
        /// The qma matrix transpose and multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixTransposeAndMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().TransposeAndMultiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma matrix transpose this and multiply.
        /// </summary>
        /// <param name="matrix1">
        /// The matrix 1.
        /// </param>
        /// <param name="matrix2">
        /// The matrix 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMatrixTransposeThisAndMultiply(double[,] matrix1, double[,] matrix2)
        {
            return matrix1.ToDenseObject().TransposeThisAndMultiply(matrix2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma mtrix create and fill.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMtrixCreateAndFill(int rows, int cols, double value)
        {
            return Matrix<double>.Build.Dense(rows, cols, value).ToArray();
        }

        /// <summary>
        /// The qma mtrix create identity.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMMtrixCreateIdentity(int size)
        {
            return Matrix<double>.Build.DenseIdentity(size).ToArray();
        }

        /// <summary>
        /// The qma multi point interpolation.
        /// </summary>
        /// s
        /// <param name="targetvector">
        /// The tickers.
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
        public static object[,] ABMMultiPointInterpolation(object targetvector, object[,] xvector, object[,] yvector)
        {
            List<double> targetList = null;

            if (targetvector is object[,])
            {
                targetList = ((object[,])targetvector).ToDoubleListFromObjectMatrix(true);
            }

            if (targetvector is double)
            {
                targetList = new List<double> { Convert.ToDouble(targetvector) };
            }

            List<double> xList = xvector.ToDoubleListFromObjectMatrix(true);
            List<double> yList = yvector.ToDoubleListFromObjectMatrix(false);
            var returnVector = new object[targetList.Count, 1];

            for (int i = 0; i < targetList.Count; i++)
            {
                returnVector[i, 0] = Interpolation.MultiPoint(targetList[i], xList, yList);
            }

            return returnVector;
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
        public static object ABMObjectProperty(string objectReference, string property)
        {
            object objectInstance = ObjectManager.ABMObjectManagerRetrieve(objectReference);

            if (objectInstance == null)
            {
                return null;
            }

            PropertyInfo propertyInfo = objectInstance.GetType().GetProperty(property);

            if (propertyInfo == null)
            {
                return null;
            }

            return propertyInfo.GetValue(objectInstance, null);
        }

        /// <summary>
        /// The qma regression simple.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionLine(double[] vector1, double[] vector2)
        {
            return Fit.Line(vector1, vector2).ToArray();
        }

        /// <summary>
        /// The qma regression multi dim normal equations.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="intecept">
        /// The intecept.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultiDimNormalEquations(double[][] X, double[] y, bool intecept)
        {
            return Fit.MultiDim(X, y, intecept, DirectRegressionMethod.NormalEquations).ToArray();
        }

        /// <summary>
        /// The qma regression multi dim qr.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="intecept">
        /// The intecept.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultiDimQR(double[][] X, double[] y, bool intecept)
        {
            return Fit.MultiDim(X, y, intecept, DirectRegressionMethod.QR).ToArray();
        }

        /// <summary>
        /// The qma regression multi dim svd.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="intecept">
        /// The intecept.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultiDimSvd(double[][] X, double[] y, bool intecept)
        {
            return Fit.MultiDim(X, y, intecept, DirectRegressionMethod.Svd).ToArray();
        }

        /// <summary>
        /// The qma regression multiple direct method.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultipleDirectMethod(double[,] X, double[] y)
        {
            return MultipleRegression.DirectMethod(X.ToDenseObject(), y.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma regression multiple normal equations.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultipleNormalEquations(double[,] X, double[] y)
        {
            return MultipleRegression.NormalEquations(X.ToDenseObject(), y.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma regression multiple qr.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultipleQR(double[,] X, double[] y)
        {
            return MultipleRegression.QR(X.ToDenseObject(), y.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma regression multiple svd.
        /// </summary>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionMultipleSVD(double[,] X, double[] y)
        {
            return MultipleRegression.Svd(X.ToDenseObject(), y.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma regression polynomial.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionPolynomial(double[] x, double[] y, int order)
        {
            return Fit.Polynomial(x, y, order);
        }

        /// <summary>
        /// The qma regression polynomial weighted.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="w">
        /// The w.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMRegressionPolynomialWeighted(double[] x, double[] y, double[] w, int order)
        {
            return Fit.PolynomialWeighted(x, y, w, order);
        }

        /// <summary>
        /// The qma scalar subtract from matrix.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMScalarSubtractFromMatrix(double scalar, double[,] matrix)
        {
            return matrix.ToDenseObject().SubtractFrom(scalar).ToArray();
        }

        /// <summary>
        /// The qma scalar subtract from vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMScalarSubtractFromVector(double scalar, double[] vector)
        {
            return vector.ToDenseObject().SubtractFrom(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector absolute maximum.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorAbsoluteMaximum(double[] vector)
        {
            return vector.ToDenseObject().AbsoluteMaximum();
        }

        /// <summary>
        /// The qma vector absolute maximum index.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ABMVectorAbsoluteMaximumIndex(double[] vector)
        {
            return vector.ToDenseObject().AbsoluteMaximumIndex();
        }

        /// <summary>
        /// The qma vector absolute minimum.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorAbsoluteMinimum(double[] vector)
        {
            return vector.ToDenseObject().AbsoluteMinimum();
        }

        /// <summary>
        /// The qma vector absolute minimum index.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ABMVectorAbsoluteMinimumIndex(double[] vector)
        {
            return vector.ToDenseObject().AbsoluteMinimumIndex();
        }

        /// <summary>
        /// The qma vector add scalar.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorAddScalar(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Add(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector add vector.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorAddVector(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().Add(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector canberra.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorCanberra(double[] vector1, double[] vector2)
        {
            return Distance.Canberra(vector1, vector2);
        }

        /// <summary>
        /// The qma vector chebyshev.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorChebyshev(double[] vector1, double[] vector2)
        {
            return Distance.Chebyshev(vector1, vector2);
        }

        /// <summary>
        /// The qma vector conjugate.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorConjugate(double[] vector)
        {
            return vector.ToDenseObject().Conjugate().ToArray();
        }

        /// <summary>
        /// The qma vector conjugate dot product.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorConjugateDotProduct(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().ConjugateDotProduct(vector2.ToDenseObject());
        }

        /// <summary>
        /// The qma vector cosine.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorCosine(double[] vector1, double[] vector2)
        {
            return Distance.Cosine(vector1, vector2);
        }

        /// <summary>
        /// The qma vector create.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorCreate(int rows)
        {
            return Vector<double>.Build.Dense(rows).ToArray();
        }

        /// <summary>
        /// The qma vector create and fill.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="cols">
        /// The cols.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorCreateAndFill(int rows, int cols, double value)
        {
            return Vector<double>.Build.Dense(rows, value).ToArray();
        }

        /// <summary>
        /// The qma vector create and fill with random standard distribution.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorCreateAndFillWithRandomStandardDistribution(int rows)
        {
            return Vector<double>.Build.Random(rows).ToArray();
        }

        /// <summary>
        /// The qma vector divide.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorDivide(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Divide(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector divide by this.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorDivideByThis(double[] vector, double scalar)
        {
            return vector.ToDenseObject().DivideByThis(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector dot product.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorDotProduct(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().DotProduct(vector2.ToDenseObject());
        }

        /// <summary>
        /// The qma vector euclidean.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorEuclidean(double[] vector1, double[] vector2)
        {
            return Distance.Euclidean(vector1, vector2);
        }

        /// <summary>
        /// The qma vector hamming.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorHamming(double[] vector1, double[] vector2)
        {
            return Distance.Hamming(vector1, vector2);
        }

        /// <summary>
        /// The qma vector infinity norm.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorInfinityNorm(double[] vector)
        {
            return vector.ToDenseObject().InfinityNorm();
        }

        /// <summary>
        /// The qma vector jaccard.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorJaccard(double p, double[] vector1, double[] vector2)
        {
            return Distance.Jaccard(vector1, vector2);
        }

        /// <summary>
        /// The qma vector l 1 norm.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorL1Norm(double[] vector)
        {
            return vector.ToDenseObject().L1Norm();
        }

        /// <summary>
        /// The qma vector l 2 norm.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorL2Norm(double[] vector)
        {
            return vector.ToDenseObject().L2Norm();
        }

        /// <summary>
        /// The qma vector manhattan.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorManhattan(double[] vector1, double[] vector2)
        {
            return Distance.Manhattan(vector1, vector2);
        }

        /// <summary>
        /// The qma vector maximum.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorMaximum(double[] vector)
        {
            return vector.ToDenseObject().Maximum();
        }

        /// <summary>
        /// The qma vector maximum index.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ABMVectorMaximumIndex(double[] vector)
        {
            return vector.ToDenseObject().MaximumIndex();
        }

        /// <summary>
        /// The qma vector mean absolute error.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorMeanAbsoluteError(double[] vector1, double[] vector2)
        {
            return Distance.MAE(vector1, vector2);
        }

        /// <summary>
        /// The qma vector mean squared error.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorMeanSquaredError(double[] vector1, double[] vector2)
        {
            return Distance.MSE(vector1, vector2);
        }

        /// <summary>
        /// The qma vector minimum.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorMinimum(double[] vector)
        {
            return vector.ToDenseObject().Minimum();
        }

        /// <summary>
        /// The qma vector minimum index.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ABMVectorMinimumIndex(double[] vector)
        {
            return vector.ToDenseObject().MinimumIndex();
        }

        /// <summary>
        /// The qma vector minkowski.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorMinkowski(double p, double[] vector1, double[] vector2)
        {
            return Distance.Minkowski(p, vector1, vector2);
        }

        /// <summary>
        /// The qma vector modulus.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorModulus(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Modulus(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector modulus by this.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorModulusByThis(double[] vector, double scalar)
        {
            return vector.ToDenseObject().ModulusByThis(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector multiply.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorMultiply(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Multiply(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector negate.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorNegate(double[] vector)
        {
            return vector.ToDenseObject().Negate().ToArray();
        }

        /// <summary>
        /// The qma vector norm.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorNorm(double[] vector, double p)
        {
            return vector.ToDenseObject().Norm(p);
        }

        /// <summary>
        /// The qma vector normalize.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorNormalize(double[] vector, double p)
        {
            return vector.ToDenseObject().Normalize(p).ToArray();
        }

        /// <summary>
        /// The qma vector outer product.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[,]"/>.
        /// </returns>
        public static double[,] ABMVectorOuterProduct(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().OuterProduct(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector pearson.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorPearson(double[] vector1, double[] vector2)
        {
            return Distance.Pearson(vector1, vector2);
        }

        /// <summary>
        /// The qma vector pointwise divide.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseDivide(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().PointwiseDivide(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector pointwise exp.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseExp(double[] vector)
        {
            return vector.ToDenseObject().PointwiseExp().ToArray();
        }

        /// <summary>
        /// The qma vector pointwise log.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseLog(double[] vector)
        {
            return vector.ToDenseObject().PointwiseLog().ToArray();
        }

        /// <summary>
        /// The qma vector pointwise modulus.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseModulus(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().PointwiseModulus(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector pointwise multiply.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseMultiply(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().PointwiseMultiply(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector pointwise power.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="exponent">
        /// The exponent.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwisePower(double[] vector, double exponent)
        {
            return vector.ToDenseObject().PointwisePower(exponent).ToArray();
        }

        /// <summary>
        /// The qma vector pointwise remainder.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorPointwiseRemainder(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().PointwiseRemainder(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector remainder.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorRemainder(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Remainder(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector remainder by this.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorRemainderByThis(double[] vector, double scalar)
        {
            return vector.ToDenseObject().RemainderByThis(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector subtract scalar.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorSubtractScalar(double[] vector, double scalar)
        {
            return vector.ToDenseObject().Subtract(scalar).ToArray();
        }

        /// <summary>
        /// The qma vector subtract vector.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] ABMVectorSubtractVector(double[] vector1, double[] vector2)
        {
            return vector1.ToDenseObject().Subtract(vector2.ToDenseObject()).ToArray();
        }

        /// <summary>
        /// The qma vector sum.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorSum(double[] vector)
        {
            return vector.ToDenseObject().Sum();
        }

        /// <summary>
        /// The qma vector sum magnitudes.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorSumMagnitudes(double[] vector)
        {
            return vector.ToDenseObject().SumMagnitudes();
        }

        /// <summary>
        /// The qma vector sum of absolute difference.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorSumOfAbsoluteDifference(double[] vector1, double[] vector2)
        {
            return Distance.SAD(vector1.ToDenseObject(), vector2.ToDenseObject());
        }

        /// <summary>
        /// The qma vector sum of squared difference.
        /// </summary>
        /// <param name="vector1">
        /// The vector 1.
        /// </param>
        /// <param name="vector2">
        /// The vector 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ABMVectorSumOfSquaredDifference(double[] vector1, double[] vector2)
        {
            return Distance.SSD(vector1, vector2);
        }

        /// <summary>
        /// The qma yield curve discount factors.
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
        public static object[,] ABMYieldCurveDiscountFactors(string curveReference, object[,] inputDates)
        {
            var yieldCurve = (YieldCurve)ObjectManager.ABMObjectManagerRetrieve(curveReference);
            List<double> inputDateList = inputDates.ToDoubleListFromObjectMatrix();
            IEnumerable<double> discountFactors = yieldCurve.DiscountFactorList(inputDateList);

            var returnObject = new object[inputDateList.Count, 1];

            int i = 0;
            foreach (double discountFactor in discountFactors)
            {
                returnObject[i, 0] = discountFactor;
                i++;
            }

            return returnObject;
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
        public static object[,] ABMYieldCurveYields(string curveReference, object[,] inputDates)
        {
            var yieldCurve = (YieldCurve)ObjectManager.ABMObjectManagerRetrieve(curveReference);
            if (yieldCurve == null)
            {
                return null;
            }

            List<double> inputDateList = inputDates.ToDoubleListFromObjectMatrix();
            return yieldCurve.YieldList(inputDateList).ToObjectArray();
        }

        #endregion
    }
}