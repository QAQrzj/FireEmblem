using System;

namespace Maps {
    /// <summary>
    /// 格子状态
    /// </summary>
    [Serializable, Flags]
    public enum CellStatus : byte {
        /// <summary>
        /// 没有任何东西, 0000 0000
        /// </summary>
        None = 0,

        /// <summary>
        /// 有 TerrainTile, 0000 0001
        /// </summary>
        TerrainTile = 0x01,

        /// <summary>
        /// 移动光标, 0000 0010
        /// </summary>
        MoveCursor = 0x02,

        /// <summary>
        /// 攻击光标, 0000 0100
        /// </summary>
        AttackCursor = 0x04,

        /// <summary>
        /// 地图对象, 0000 1000
        /// </summary>
        MapObject = 0x08,

        //如果有其他需求, 在这里添加其余 4 个开关属性

        /// <summary>
        /// 全部 8 个开关, 1111 1111
        /// </summary>
        All = byte.MaxValue
    }
}
