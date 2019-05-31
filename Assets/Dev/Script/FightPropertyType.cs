using System;

namespace Dev {
    [Serializable]
    public enum FightPropertyType {
        /// <summary>
        /// 力量
        /// </summary>
        STR = 0,

        /// <summary>
        /// 魔力
        /// </summary>
        MAG = 1,

        /// <summary>
        /// 技巧
        /// </summary>
        SKL = 2,

        /// <summary>
        /// 速度
        /// </summary>
        SPD = 3,

        /// <summary>
        /// 防御
        /// </summary>
        DEF = 4,

        /// <summary>
        /// 魔防
        /// </summary>
        MDF = 5,

        /// <summary>
        /// 长度
        /// </summary>
        Length
    }
}
