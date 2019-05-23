using System;

namespace Models {
    [Serializable]
    public enum ClassType {
        /// <summary>
        /// 领主
        /// </summary>
        Lord = 0,

        /// <summary>
        /// 轻骑士
        /// </summary>
        Socialknight,

        /// <summary>
        /// 圣骑士
        /// </summary>
        Paladin,

        /// <summary>
        /// 重甲骑士
        /// </summary>
        Armorknight,

        /// <summary>
        /// 天马骑士
        /// </summary>
        Pegasusknight,

        /// <summary>
        /// 魔法师
        /// </summary>
        Mage,

        /// <summary>
        /// 贤者
        /// </summary>
        Sage,

        /// <summary>
        /// 巫师
        /// </summary>
        Shaman,

        /// <summary>
        /// 剑士
        /// </summary>
        Myrmidon,

        /// <summary>
        /// 剑圣
        /// </summary>
        Swordmaster,

        /// <summary>
        /// 弓箭手
        /// </summary>
        Archer,

        /// <summary>
        /// 战士
        /// </summary>
        Fighter,

        /// <summary>
        /// 狂战士
        /// </summary>
        Berserker,

        /// <summary>
        /// 山贼
        /// </summary>
        Bandit,

        /// <summary>
        /// 海盗
        /// </summary>
        Pirate,

        /// <summary>
        /// 修女
        /// </summary>
        Sister,

        // Other Class Type

        /// <summary>
        /// 长度The length.
        /// </summary>
        Length
    }
}
