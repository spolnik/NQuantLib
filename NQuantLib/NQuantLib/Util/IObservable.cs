using System;

namespace NQuantLib.Util {

    public interface IObservable {
        event Action notifyObserversEvent;
        void registerWith(Action handler);
        void unregisterWith(Action handler);
    }
}