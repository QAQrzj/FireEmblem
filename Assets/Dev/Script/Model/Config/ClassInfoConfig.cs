using System;
using System.Xml.Serialization;

namespace Models {
    [Serializable]
    public class ClassInfoConfig : BaseXmlConfig<int, ClassInfo> {

    }

    [Serializable]
    public class ClassInfo : IConfigData<int> {
        /// <summary>
        /// 职业 ID
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 职业名称
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// 职业类型
        /// </summary>
        [XmlAttribute]
        public ClassType classType;

        /// <summary>
        /// 移动点数
        /// </summary>
        [XmlAttribute]
        public float movePoint;

        /// <summary>
        /// 战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 最大战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties maxFightProperties;

        /// <summary>
        /// 可用武器
        /// </summary>
        [XmlElement]
        public AvailableWeapons availableWeapons;

        /// <summary>
        /// TODO 包含的技能 IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        /// <summary>
        /// 预制体名称
        /// </summary>
        [XmlAttribute]
        public string prefab;

        /// <summary>
        /// 动画名称, 当使用同一个 prefab 时, 可以设置不同的动画
        /// </summary>
        [XmlAttribute]
        public string animator;

        public int GetKey() {
            return id;
        }
    }
}
