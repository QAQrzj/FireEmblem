using System;
using UnityEngine;

namespace Maps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New SRPG Tile.asset", menuName = "SRPG/Tile")]
    public class SRPGTile : RuleTile
    {
        /// <summary>
        /// 地形类型
        /// </summary>
        public TerrainType TerrainType { get; set; } = TerrainType.Plain;

        /// <summary>
        /// 守备
        /// </summary>
        public int Defense { get; set; } = 0;

        /// <summary>
        /// 回避
        /// </summary>
        public int AvoidRate { get; set; } = 0;

        /// <summary>
        /// 恢复
        /// </summary>
        public int Treatment { get; set; } = 0;
    }
}
