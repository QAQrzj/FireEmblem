using System;

namespace Maps {
    /// <summary>
    /// 地形类型
    /// </summary>
    [Serializable]
    public enum TerrainType : byte {
        /// <summary>
        /// 平地
        /// </summary>
        Plain,

        /// <summary>
        /// 森林
        /// </summary>
        Forest,

        /// <summary>
        /// 墙壁
        /// </summary>
        Wall,

        /// <summary>
        /// 长度
        /// </summary>
        Length
    }
}
