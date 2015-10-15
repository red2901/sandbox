// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FittableObjectCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The fittable object collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ABM.Model
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading;

    using log4net;

    using MathNet.Numerics.LinearAlgebra;
    using MathNet.Numerics.LinearAlgebra.Double;

    using Microsoft.Practices.ServiceLocation;

    using ABM.Analytics.Solvers;
    using ABM.Common;
    using ABM.Model.Events;

    /// <summary>
    ///     The fittable object collection.
    /// </summary>
    public class FittableObjectCollection : IFittableObjectCollection
    {
        #region Fields

        /// <summary>
        ///     The event aggregator.
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        ///     The fittable object collection.
        /// </summary>
        private readonly Dictionary<string, IFittableObject> fittableObjectCollection;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILog logger;

        private DenseVector fittingParameters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FittableObjectCollection"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        public FittableObjectCollection(ILog logger, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.fittableObjectCollection = new Dictionary<string, IFittableObject>();

            this.InitialiseSubscriptions();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the fitting parameters.
        /// </summary>
        public DenseVector FittingParameters 
        {
            get
            {
                return this.fittingParameters;
            }
            set
            {
                this.fittingParameters = (DenseVector)value.Clone();
            } 
        }

        /// <summary>
        /// Gets or sets the solver result.
        /// </summary>
        public ISolverResult SolverResult { get; set; }

        /// <summary>
        ///     Gets the subscriptions.
        /// </summary>
        public IList<IDisposable> Subscriptions { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="fittableObject">
        /// The fittable object.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Add(IFittableObject fittableObject)
        {
            string key = fittableObject.ToString();
            if (this.fittableObjectCollection.ContainsKey(key))
            {
                this.logger.DebugFormat("We already have a fittable object : {0}", key);
                return false;
            }

            this.logger.DebugFormat("Adding fittable object : {0}", key);
            this.fittableObjectCollection.Add(key, fittableObject);
            return true;
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable subscription in this.Subscriptions)
            {
                subscription.Dispose();
            }
        }

        private bool HasFittableValues()
        {
            // check that the collection has fittable values
            foreach (var value in this.fittableObjectCollection.Values)
            {
                if (!value.HasFittableValues())
                {
                    this.logger.DebugFormat("{0} -  has no fittable values", value);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The fit.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Fit()
        {
            try
            {
                if (!this.HasFittableValues())
                {
                    return false;
                }

                this.logger.DebugFormat("Running solver");
                var solver = ServiceLocator.Current.GetInstance<ISolver>();
                solver.Estimate(this, this.FittingParameters);

                this.SolverResult = solver.SolverResult.Clone();
                this.logger.DebugFormat("Solver Complete");

                // publish solver result
                this.eventAggregator.Publish(new SolverResultEvent(EventType.Update, this.SolverResult));

                return true;
            }
            catch (Exception e)
            {
                this.logger.DebugFormat("Solver exception : {0}", e.Message);
                return false;
            }
        }

        /// <summary>
        ///     The subscribe to events.
        /// </summary>
        public void InitialiseSubscriptions()
        {
            this.Subscriptions = new List<IDisposable>
                                     {
                                         // add and remove fittable objects
                                         this.eventAggregator.GetEvent<FittableObjectEvent>()
                                             .Where(ev => ev.EventType == EventType.Add)
                                             .Subscribe(ev => this.Add(ev.FittableObject)), 
                                         this.eventAggregator.GetEvent<FittableObjectEvent>()
                                             .Where(ev => ev.EventType == EventType.Remove)
                                             .Subscribe(ev => this.Remove(ev.FittableObject)), 

                                         // update fittable object prices
                                         this.eventAggregator.GetEvent<AskPriceEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(
                                                 ev =>
                                                 this.UpdateAskPrice(ev.SecurityKey, ev.PriceUpdate)), 
                                         this.eventAggregator.GetEvent<BidPriceEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(
                                                 ev =>
                                                 this.UpdateBidPrice(ev.SecurityKey, ev.PriceUpdate)), 
                                         this.eventAggregator.GetEvent<PriceEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(
                                                 ev =>
                                                 this.UpdateBidAskPrice(ev.SecurityKey, ev.PriceUpdate)), 

                                         // update fitting parameters
                                         this.eventAggregator.GetEvent<FittingParametersEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(
                                                 ev => this.FittingParameters = ev.FittingParameters), 

                                         // run the fitter
                                         this.eventAggregator.GetEvent<FittingEvent>()
                                             .Where(ev => ev.EventType == EventType.Update)
                                             .Subscribe(ev => this.Fit()), 
                                     };
        }

        /// <summary>
        /// The objective value.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double ObjectiveValue(Vector<double> parameters)
        {
            double objectiveValue = 0.0;
            foreach (IFittableObject fittableObject in this.fittableObjectCollection.Values)
            {
                double y = fittableObject.ObjectiveValue(parameters);
                // this.logger.DebugFormat("{0} - {1} -> {2}", fittableObject, y, y * y);
                objectiveValue += y * y;
            }

            return 0.5 * objectiveValue;
        }

        /// <summary>
        /// The objective value jacobian.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public Matrix<double> ObjectiveValueJacobian(Vector<double> parametersCurrent)
        {
            Matrix<double> jacobian = new DenseMatrix(this.fittableObjectCollection.Count, parametersCurrent.Count);

            int i = 0;
            foreach (var fittableObject in this.fittableObjectCollection.Values)
            {
                Vector<double> gradient = fittableObject.Gradient(parametersCurrent);
                jacobian.SetRow(i, gradient);
                i += 1;
            }

            return jacobian;
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="fittableObject">
        /// The fittable object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Remove(IFittableObject fittableObject)
        {
            string key = fittableObject.ToString();
            if (this.fittableObjectCollection.ContainsKey(key))
            {
                this.logger.DebugFormat("Removing fittable object : {0}", key);
                this.fittableObjectCollection.Remove(key);
                return true;
            }

            this.logger.DebugFormat("There is no fittable object to remove : {0}", key);
            return false;
        }

        /// <summary>
        /// The residual.
        /// </summary>
        /// <param name="parametersCurrent">
        /// The parameters current.
        /// </param>
        /// <returns>
        /// The <see cref="Vector"/>.
        /// </returns>
        public Vector<double> Residual(Vector<double> parametersCurrent)
        {
            Vector<double> residual = new DenseVector(this.fittableObjectCollection.Count);
            int i = 0;
            foreach (Bond bond in this.fittableObjectCollection.Values)
            {
                residual[i] = bond.ObjectiveValue(parametersCurrent);
                i += 1;
            }

            return residual;
        }


        #endregion

        #region Methods

        /// <summary>
        /// The update ask price.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        private void UpdateAskPrice(string key, double price)
        {
            if (this.fittableObjectCollection.ContainsKey(key))
            {
                IFittableObject currentObject;
                this.fittableObjectCollection.TryGetValue(key, out currentObject);
                if (currentObject != null)
                {
                    currentObject.Ask = price;
                }
            }
        }

        /// <summary>
        /// The update bid price.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        private void UpdateBidPrice(string key, double price)
        {
            if (this.fittableObjectCollection.ContainsKey(key))
            {
                IFittableObject currentObject;
                this.fittableObjectCollection.TryGetValue(key, out currentObject);
                if (currentObject != null)
                {
                    currentObject.Bid = price;
                }
            }
        }

        private void UpdateBidAskPrice(string key, double price)
        {
            if (this.fittableObjectCollection.ContainsKey(key))
            {
                IFittableObject currentObject;
                this.fittableObjectCollection.TryGetValue(key, out currentObject);
                this.logger.DebugFormat("Setting fittable object price : {0} = {1}", key, price);
                if (currentObject != null)
                {
                    currentObject.Bid = price;
                    currentObject.Ask = price;
                }
            }
        }

        #endregion
    }
}