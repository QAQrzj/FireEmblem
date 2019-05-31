using System;
using System.Xml.Serialization;

namespace Models {
    [Serializable]
    public class ConsumableUniqueInfo : UniqueInfo {
        /// <summary>
        /// 最大堆叠次数
        /// </summary>
        [XmlAttribute]
        public int stackingNumber;

        /// <summary>
        /// 使用次数
        /// </summary>
        [XmlAttribute]
        public int amountUsed;

        /// <summary>
        /// 使用效果 ID
        /// </summary>
        [XmlAttribute]
        public int usingEffectId;
    }
}
