using System;

namespace NQuantLib.Util {
    public abstract class LazyObject : IObservable, IObserver {
        protected bool calculated;
        protected bool frozen;

        /* This method must implement any calculations which must be (re)done 
         * in order to calculate the desired results. */

        protected LazyObject() {
            calculated = false;
            frozen = false;
        }

        /*! This method forces recalculation of any results which would otherwise be cached.
        * It needs to call the <i><b>LazyCalculationEvent</b></i> event.
          Explicit invocation of this method is <b>not</b> necessary if the object has registered itself as
          observer with the structures on which such results depend.  It is strongly advised to follow this
          policy when possible. */

        public event Action notifyObserversEvent;

        public void registerWith(Action handler) {
            notifyObserversEvent += handler;
        }

        public void unregisterWith(Action handler) {
            notifyObserversEvent -= handler;
        }

        public virtual void update() {
            if (!frozen && calculated)
                notifyObservers();

            calculated = false;
        }

        protected abstract void performCalculations();

        public virtual void recalculate() {
            var wasFrozen = frozen;
            calculated = frozen = false;
            try {
                calculate();
            }
            finally {
                frozen = wasFrozen;
                notifyObservers();
            }
        }

        protected void notifyObservers() {
            var handler = notifyObserversEvent;

            if (handler != null)
                handler();
        }


        /*! This method constrains the object to return the presently cached results on successive invocations,
         * even if arguments upon which they depend should change. */

        public void freeze() {
            frozen = true;
        }

        // This method reverts the effect of the <i><b>freeze</b></i> method, thus re-enabling recalculations.
        public void unfreeze() {
            frozen = false;
            notifyObservers(); // send notification, just in case we lost any
        }

        /*! This method performs all needed calculations by calling the <i><b>performCalculations</b></i> method.
            Objects cache the results of the previous calculation. Such results will be returned upon
            later invocations of <i><b>calculate</b></i>. When the results depend
            on arguments which could change between invocations, the lazy object must register itself
            as observer of such objects for the calculations to be performed again when they change.
            Should this method be redefined in derived classes, LazyObject::calculate() should be called
            in the overriding method. */

        protected virtual void calculate() {
            if (!calculated && !frozen) {
                calculated = true; // prevent infinite recursion in case of bootstrapping
                try {
                    performCalculations();
                }
                catch {
                    calculated = false;
                    throw;
                }
            }
        }
    }
}