using NQuantLib.Util;

namespace NQuantLib.PricingEngines {
    public interface IPricingEngine : IObservable {
        IPricingEngineArguments getArguments();
        IPricingEngineResults getResults();
        void reset();
        void calculate();
    }

    public interface IPricingEngineArguments {
        void validate();
    }

    public interface IPricingEngineResults {
        void reset();
    }
}