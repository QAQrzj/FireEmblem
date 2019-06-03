using System;

namespace Models {
    public interface IModel : IDisposable {
        void Load();
    }
}
