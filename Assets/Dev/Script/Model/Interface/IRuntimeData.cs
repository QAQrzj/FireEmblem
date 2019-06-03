using System;

namespace Models {
    public interface IRuntimeData<T> : ICloneable where T : class, IRuntimeData<T>, new() {
        void CopyTo(T data);
        new T Clone();
    }
}
