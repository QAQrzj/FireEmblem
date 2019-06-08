using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DR.Book.SRPG_Dev.Framework;
using UnityEngine;

namespace Models {
    [Serializable]
    public abstract class BaseXmlConfig<TKey, TData> : XmlConfigFile, IEditorConfigSerializer where TData : class, IConfigData<TKey> {
        /// <summary>
        /// 能够序列化的数据
        /// </summary>
        [XmlArray, XmlArrayItem]
        public TData[] datas;

        /// <summary>
        /// 读取后存储的数据
        /// </summary>
        private Dictionary<TKey, TData> dataDict = new Dictionary<TKey, TData>();

        [XmlIgnore]
        public TData this[TKey key] {
            get {
                if (!dataDict.TryGetValue(key, out TData data)) {
                    Debug.LogErrorFormat("{0} -> Key '{1}' was not found.", GetType().Name, key);
                    return null;
                }
                return data;
            }
        }

        /// <summary>
        /// 文件信息构造
        /// </summary>
        /// <param name="info">Info.</param>
        protected override void ConstructInfo(ref Info info) {
            base.ConstructInfo(ref info);
            info.name = GetType().Name + ".xml";
            info.loadType = LoadType.WWW;
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <returns>The buffer.</returns>
        /// <param name="buffer">Buffer.</param>
        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer) {
            // 如果直接使用数组或字典序列化, 可以直接返回 buffer
            //return buffer;

            BaseXmlConfig<TKey, TData> config = buffer as BaseXmlConfig<TKey, TData>;
            foreach (TData data in config.datas) {
                if (dataDict.ContainsKey(data.GetKey())) {
                    Debug.LogWarningFormat("{0} -> Key '{1}' is exist. Pass.", GetType().Name, data.GetKey());
                    continue;
                }
                dataDict.Add(data.GetKey(), data);
            }
            return this;
        }

        /// <summary>
        /// 获取所有 Key
        /// </summary>
        /// <returns></returns>
        Array IEditorConfigSerializer.EditorGetKeys() {
            if (datas == null) {
                return default;
            }
            return datas.Select(data => data.GetKey()).ToArray();
        }

        void IEditorConfigSerializer.EditorSortDatas() {
            if (datas != null) {
                Array.Sort(datas, (data1, data2) => {
                    return data1.GetKey().GetHashCode().CompareTo(data2.GetKey().GetHashCode());
                });
            }
        }

        public virtual byte[] EditorSerializeToBytes() {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream()) {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8)) {
                    XmlSerializer xs = new XmlSerializer(GetType());
                    XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                    xsn.Add("", "");
                    xs.Serialize(sw, this, xsn);
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }

        public virtual void EditorDeserializeToObject(byte[] bytes) {
            XmlConfigFile config;
            using (MemoryStream ms = new MemoryStream(bytes)) {
                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8)) {
                    XmlSerializer xs = new XmlSerializer(GetType());
                    config = xs.Deserialize(sr) as XmlConfigFile;
                }
            }
            datas = (config as BaseXmlConfig<TKey, TData>).datas;
        }
    }
}
