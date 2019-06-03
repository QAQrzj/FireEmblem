using System;
using System.Collections;
using System.Collections.Generic;

namespace Models {
    public static class ModelManager {
        public static readonly ModelDictionary models = new ModelDictionary();
    }

    public class ModelDictionary : IDictionary<Type, IModel> {
        #region Field
        private readonly Dictionary<Type, IModel> modelDict = new Dictionary<Type, IModel>();
        #endregion

        #region Method
        public T Get<T>() where T : class, IModel, new() {
            Type type = typeof(T);
            if (!modelDict.TryGetValue(type, out IModel model)) {
                RegisterModel<T>();
                model = modelDict[type];
            }
            return model as T;
        }

        public void RegisterModel<T>() where T : class, IModel, new() {
            Type type = typeof(T);
            if (!modelDict.ContainsKey(type)) {
                IModel model = new T();
                model.Load();
                modelDict.Add(type, model);
            }
        }

        public void UnregisterModel<T>() where T : class, IModel {
            Type type = typeof(T);
            if (modelDict.TryGetValue(type, out IModel model)) {
                model.Dispose();
                modelDict.Remove(type);
            }
        }

        public void UnregisterAllModels() {
            foreach (IModel model in modelDict.Values) {
                model.Dispose();
            }
            modelDict.Clear();
        }
        #endregion

        #region IDictionary<Type, ModelBase> Interface
        public IModel this[Type key] {
            get { return modelDict[key]; }
            set => throw new NotImplementedException("Read Only");
        }

        public ICollection<Type> Keys => modelDict.Keys;

        public ICollection<IModel> Values => modelDict.Values;

        public int Count => modelDict.Count;

        bool ICollection<KeyValuePair<Type, IModel>>.IsReadOnly => ((ICollection<KeyValuePair<Type, IModel>>)modelDict).IsReadOnly;

        void IDictionary<Type, IModel>.Add(Type key, IModel value) => throw new NotImplementedException("Not Supported.");

        void ICollection<KeyValuePair<Type, IModel>>.Add(KeyValuePair<Type, IModel> item) => throw new NotImplementedException("Not Supported.");

        void ICollection<KeyValuePair<Type, IModel>>.Clear() => throw new NotImplementedException("Not Supported.");

        bool ICollection<KeyValuePair<Type, IModel>>.Contains(KeyValuePair<Type, IModel> item) => throw new NotImplementedException("Not Supported.");

        public bool ContainsKey(Type key) {
            return modelDict.ContainsKey(key);
        }

        void ICollection<KeyValuePair<Type, IModel>>.CopyTo(KeyValuePair<Type, IModel>[] array, int arrayIndex) => throw new NotImplementedException("Not Supported.");

        public IEnumerator<KeyValuePair<Type, IModel>> GetEnumerator() {
            return modelDict.GetEnumerator();
        }

        bool IDictionary<Type, IModel>.Remove(Type key) => throw new NotImplementedException("Not Supported.");

        bool ICollection<KeyValuePair<Type, IModel>>.Remove(KeyValuePair<Type, IModel> item) => throw new NotImplementedException("Not Supported.");

        public bool TryGetValue(Type key, out IModel value) {
            return modelDict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return modelDict.GetEnumerator();
        }
        #endregion
    }
}
