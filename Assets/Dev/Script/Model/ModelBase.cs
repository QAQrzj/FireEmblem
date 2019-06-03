using System;

namespace Models {
    public class ModelBase : IModel {
        public bool Loaded { get; private set; } = false;

        void IModel.Load() {
            if (Loaded) {
                return;
            }

            OnLoad();
            Loaded = true;
        }

        protected virtual void OnLoad() {

        }


        void IDisposable.Dispose() {
            if (!Loaded) {
                return;
            }

            OnDispose();
            Loaded = false;
        }

        protected virtual void OnDispose() {

        }
    }
}
