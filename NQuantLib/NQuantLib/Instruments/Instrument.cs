using System;
using System.Collections.Generic;
using NQuantLib.PricingEngines;
using NQuantLib.Util;

namespace NQuantLib.Instruments {
    public abstract class Instrument : LazyObject {
// ReSharper disable once InconsistentNaming
        protected decimal? NPV = null;
        protected Dictionary<string, object> additionalResultsMap = new Dictionary<string, object>();
        protected decimal? errorEstimateValue = null;
        //use money at all (with currency)

        protected IPricingEngine pricingEngine;
        //TODO: protected Date valuationDate;
        // https://github.com/lballabio/quantlib/blob/master/QuantLib/ql/instrument.hpp

        public decimal npv() {
            calculate();
            if (NPV == null) throw new ArgumentException("NPV not provided");
            //TODO: QL.require(!Double.isNaN(this.NPV) , "NPV not provided");
            return NPV.GetValueOrDefault();
        }

        protected override void calculate() {
            if (isExpired()) {
                setupExpired();
                calculated = true;
            }
            else {
                base.calculate();
            }
        }

        protected virtual void setupExpired() {
            NPV = errorEstimateValue = null;
            additionalResultsMap.Clear();
        }

        public decimal errorEstimate() {
            calculate();
            if (errorEstimateValue == null) throw new ArgumentException("error estimate not provided");
            // TODO: QL.require(!Double.isNaN(this.errorEstimate) , "error estimate not provided");
            return errorEstimateValue.GetValueOrDefault();
        }

        //! returns whether the instrument is still tradable.
        public virtual bool isExpired() {
            throw new NotSupportedException();
        }

        public virtual void setupArguments(IPricingEngineArguments a) {
            throw new NotImplementedException();
        }

        public void setPricingEngine(IPricingEngine engine) {
            if (pricingEngine != null)
                pricingEngine.unregisterWith(update);

            pricingEngine = engine;

            if (pricingEngine != null)
                pricingEngine.registerWith(update);

            update(); // trigger (lazy) recalculation and notify observers
        }

        /*! When a derived result structure is defined for an instrument,
         * this method should be overridden to read from it.
         * This is mandatory in case a pricing engine is used.  */

        public virtual void fetchResults(IPricingEngineResults r) {
            var results = r as Results;
            if (results == null) throw new ArgumentException("no results returned from pricing engine");
            NPV = results.value;
            errorEstimateValue = results.errorEstimate;
            additionalResultsMap = results.additionalResults;
        }

        /* In case a pricing engine is not used, this method must be overridden to perform the actual
           calculations and set any needed results.
         * In case a pricing engine is used, the default implementation can be used. */

        protected override void performCalculations() {
            if (pricingEngine == null)
                throw new ArgumentException("null pricing engine");
            //TODO: QL.require(engine != null, SHOULD_DEFINE_PRICING_ENGINE); // QA:[RG]::verified

            pricingEngine.reset();
            setupArguments(pricingEngine.getArguments());
            pricingEngine.getArguments().validate();
            pricingEngine.calculate();
            fetchResults(pricingEngine.getResults());
        }

        // returns any additional result returned by the pricing engine.
        public object result(string tag) {
            calculate();
            try {
                return additionalResultsMap[tag];
            }
            catch (KeyNotFoundException) {
                throw new ArgumentException(tag + " not provided");
            }
        }

        // returns all additional result returned by the pricing engine.
        public Dictionary<string, object> additionalResults() {
            return additionalResultsMap;
        }

        public class Results : IPricingEngineResults {
            public Dictionary<string, object> additionalResults = new Dictionary<string, object>();
            public decimal? errorEstimate;
            public decimal? value;

            public virtual void reset() {
                value = errorEstimate = null;
                additionalResults.Clear();
            }
        }
    }
}