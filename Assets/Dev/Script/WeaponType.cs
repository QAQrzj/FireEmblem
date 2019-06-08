using System;

namespace Weapon {
    [Serializable]
    public enum WeaponType {
        Unknow = -1,

        /// <summary>
        /// 剑
        /// </summary>
        Sword = 0,

        /// <summary>
        /// 枪
        /// </summary>
        Lance = 1,

        /// <summary>
        /// 斧
        /// </summary>
        Axe = 2,

        /// <summary>
        /// 弓
        /// </summary>
        Bow = 3,

        /// <summary>
        /// 杖
        /// </summary>
        Staff = 4,
    }
}
