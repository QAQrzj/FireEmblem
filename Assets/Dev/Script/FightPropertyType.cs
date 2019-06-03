using System;

namespace Dev {
    [Serializable]
    public enum FightPropertyType {
        /// <summary>
        /// 生命值
        /// </summary>
        HP = 0,

        /// <summary>
        /// 力量
        /// </summary>
        STR,

        /// <summary>
        /// 魔力
        /// </summary>
        MAG,

        /// <summary>
        /// 技巧
        /// </summary>
        SKL,

        /// <summary>
        /// 速度
        /// </summary>
        SPD,

        /// <summary>
        /// 防御
        /// </summary>
        DEF,

        /// <summary>
        /// 魔防
        /// </summary>
        MDF,

        /// <summary>
        /// 长度
        /// </summary>
        Length
    }
}
