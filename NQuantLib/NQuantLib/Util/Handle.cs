using System;

namespace NQuantLib.Util {
    public class Handle<T> where T : class, IObservable, new() {
        protected readonly Link innerLink;

        public Handle() : this(new T()) {}
        public Handle(T h) : this(h, true) {}

        public Handle(T h, bool registerAsObserver) {
            innerLink = new Link(h, registerAsObserver);
        }

        public T link {
            get {
                if (empty())
                    throw new ApplicationException("empty Handle cannot be dereferenced");
                return innerLink.currentLink();
            }
        }

        //! dereferencing
        public T currentLink() {
            return link;
        }

        public static implicit operator T(Handle<T> impliedObject) {
            return impliedObject.link;
        }

        // dereferencing of the observable to the Link
        public void registerWith(Action handler) {
            innerLink.registerWith(handler);
        }

        public void unregisterWith(Action handler) {
            innerLink.unregisterWith(handler);
        }


        //! checks if the contained shared pointer points to anything
        public bool empty() {
            return innerLink.empty();
        }

        #region operator overload

        public static bool operator ==(Handle<T> here, Handle<T> there) {
            return here.Equals(there);
        }

        public static bool operator !=(Handle<T> here, Handle<T> there) {
            return !here.Equals(there);
        }

        public override bool Equals(object o) {
            return innerLink == ((Handle<T>) o).innerLink;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        #endregion

        protected class Link : IObservable, IObserver {
            private T handle;
            private bool isObserver;

            public Link(T handler, bool registerAsObserver) {
                linkTo(handler, registerAsObserver);
            }

            // Observable
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

            public void linkTo(T handler, bool registerAsObserver) {
                if (!handler.Equals(handle) || (isObserver != registerAsObserver)) {
                    if (handle != null && isObserver) {
                        handle.unregisterWith(update);
                    }

                    handle = handler;
                    isObserver = registerAsObserver;

                    if (handle != null && isObserver) {
                        handle.registerWith(update);
                    }

                    // finally, notify observers of this of the change in the underlying object
                    notifyObservers();
                }
            }

            public bool empty() {
                return handle == null;
            }

            public T currentLink() {
                return handle;
            }

            protected void notifyObservers() {
                Action handler = notifyObserversEvent;
                if (handler != null)
                    handler();
            }
        }
    }
}