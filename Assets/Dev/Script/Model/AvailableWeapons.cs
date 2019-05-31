using System;
using System.Xml.Serialization;
using Weapon;

namespace Models {
    /// <summary>
    /// 可用武器等级:
    /// 0 不可用,
    /// 1 E, 2 D, 3 C, 4 B, 5 A, 6 S, 7 *
    /// </summary>
    [Serializable]
    public struct AvailableWeapons {
        /// <summary>
        /// 剑
        /// </summary>
        [XmlAttribute]
        public int sword;

        /// <summary>
        /// 枪
        /// </summary>
        [XmlAttribute]
        public int lance;

        /// <summary>
        /// 斧
        /// </summary>
        [XmlAttribute]
        public int axe;

        /// <summary>
        /// 弓
        /// </summary>
        [XmlAttribute]
        public int bow;

        /// <summary>
        /// 杖
        /// </summary>
        [XmlAttribute]
        public int staff;

        [XmlIgnore]
        public int this[WeaponType type] {
            get {
                switch (type) {
                    case WeaponType.Sword:
                        return sword;
                    case WeaponType.Lance:
                        return lance;
                    case WeaponType.Axe:
                        return axe;
                    case WeaponType.Bow:
                        return bow;
                    case WeaponType.Staff:
                        return staff;
                    default:
                        return 0;
                }
            }
        }
    }
}
