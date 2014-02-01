using System;
using NQuantLib.Util;

namespace NQuantLib.PricingEngines {
    
    public class GenericEngine<TArg, TRes> : IPricingEngine, IObserver
        where TArg : IPricingEngineArguments, new()
        where TRes : IPricingEngineResults, new() {
        
        protected TArg arguments = new TArg();
        protected TRes results = new TRes();

        public IPricingEngineArguments getArguments() {
            return arguments;
        }

        public IPricingEngineResults getResults() {
            return results;
        }

        public void reset() {
            results.reset();
        }

        public virtual void calculate() {
            throw new NotSupportedException();
        }

        public event Action notifyObserversEvent;

        public void registerWith(Action handler) {
            notifyObserversEvent += handler;
        }

        public void unregisterWith(Action handler) {
            notifyObserversEvent -= handler;
        }

        public void update() {
            notifyObservers();
        }

        protected void notifyObservers() {
            var handler = notifyObserversEvent;
            if (handler != null)
                handler();
        }
    }
}