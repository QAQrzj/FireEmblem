using System;
using System.Xml.Serialization;
using DR.Book.SRPG_Dev.Framework;
using Maps;

namespace Models {
    [Serializable]
    public class MoveConsumptionInfoConfig : BaseXmlConfig<ClassType, MoveConsumptionInfo> {
        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer) {
            // 长度要和 TerrainType 数量保持一致
            int length = (int)TerrainType.Length;
            MoveConsumptionInfoConfig config = buffer as MoveConsumptionInfoConfig;
            for (int i = 0; i < config.datas.Length; i++) {
                if (config.datas[i].consumptions.Length != length) {
                    int oldLength = config.datas[i].consumptions.Length;
                    Array.Resize(ref config.datas[i].consumptions, length);
                    for (int j = oldLength; j < length; j++) {
                        config.datas[i].consumptions[j] = 255f;
                    }
                }
            }

            return base.FormatBuffer(config);
        }
    }

    [Serializable]
    public class MoveConsumptionInfo : IConfigData<ClassType> {
        /// <summary>
        ///  职业类型
        /// </summary>
        [XmlAttribute]
        public ClassType classType;

        /// <summary>
        /// 在各个地形的移动消耗具体数值
        /// </summary>
        [XmlAttribute]
        public float[] consumptions;

        public ClassType GetKey() {
            return classType;
        }
    }
}
