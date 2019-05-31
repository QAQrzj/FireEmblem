using System;
using System.Xml.Serialization;
using Weapon;

namespace Models {
    [Serializable]
    public class WeaponUniqueInfo : OrnamentUniqueInfo {
        /// <summary>
        /// 武器类型
        /// </summary>
        [XmlAttribute]
        public WeaponType weaponType;

        /// <summary>
        /// 武器等级
        /// </summary>
        [XmlAttribute]
        public int level;

        /// <summary>
        /// 攻击力
        /// </summary>
        [XmlAttribute]
        public int attack;

        /// <summary>
        /// 最小攻击范围
        /// </summary>
        [XmlAttribute]
        public int minRange;

        /// <summary>
        /// 最大攻击范围
        /// </summary>
        [XmlAttribute]
        public int maxRange;

        /// <summary>
        /// 重量
        /// </summary>
        [XmlAttribute]
        public int weight;

        /// <summary>
        /// 耐久度
        /// </summary>
        [XmlAttribute]
        public int durability;
    }
}
