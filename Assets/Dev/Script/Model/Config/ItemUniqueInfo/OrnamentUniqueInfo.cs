using System;
using System.Xml.Serialization;

namespace Models {
    [Serializable]
    public class OrnamentUniqueInfo : UniqueInfo {
        /// <summary>
        /// 生命值加成
        /// </summary>
        [XmlElement]
        public int hp;

        /// <summary>
        /// 战斗属性加成
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 幸运加成
        /// </summary>
        [XmlElement]
        public int luk;

        /// <summary>
        /// 移动力加成
        /// </summary>
        [XmlElement]
        public float movePoint;

        /// <summary>
        /// TODO 包含的技能 IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        /// <summary>
        /// TODO 特殊效果 IDs
        /// </summary>
        //[XmlElement]
        //public int[] specialSkills;
    }
}
