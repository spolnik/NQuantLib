namespace NQuantLib.Util {
    public class RelinkableHandle<T> : Handle<T> where T : class, IObservable, new() {
        public RelinkableHandle() : base(new T(), true) {}
        public RelinkableHandle(T h) : base(h, true) {}
        public RelinkableHandle(T h, bool registerAsObserver) : base(h, registerAsObserver) {}

        public void linkTo(T h) {
            linkTo(h, true);
        }

        public void linkTo(T h, bool registerAsObserver) {
            innerLink.linkTo(h, registerAsObserver);
        }
    }
}