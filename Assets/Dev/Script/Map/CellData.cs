using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maps {
    /// <summary>
    /// 地图上每个格子的信息
    /// </summary>
    public class CellData : IDisposable {
        #region Common Field/Property
        private Vector3Int position;
        private MapObject mapObject;
        private CellStatus status = CellStatus.None;

        /// <summary>
        /// 坐标位置
        /// </summary>
        /// <value>The vector3 int.</value>
        public Vector3Int Position { get => position; }

        /// <summary>
        /// 是否有 Tile
        /// </summary>
        /// <value><c>true</c> if has tile; otherwise, <c>false</c>.</value>
        public bool HasTile {
            get { return CheckStatus(CellStatus.TerrainTile, false); }
            set { SwitchStatus(CellStatus.TerrainTile, value); }
        }

        /// <summary>
        /// 是否有 Cursor
        /// </summary>
        /// <value><c>true</c> if has cursor; otherwise, <c>false</c>.</value>
        public bool HasCursor {
            get { return CheckStatus(CellStatus.MoveCursor | CellStatus.AttackCursor, true); }
            set { SwitchStatus(CellStatus.MoveCursor | CellStatus.AttackCursor, value); }
        }

        /// <summary>
        /// 是否有移动范围光标
        /// </summary>
        /// <value><c>true</c> if has move cursor; otherwise, <c>false</c>.</value>
        public bool HasMoveCursor {
            get { return CheckStatus(CellStatus.MoveCursor, false); }
            set { SwitchStatus(CellStatus.MoveCursor, value); }
        }

        /// <summary>
        /// 是否有攻击范围光标
        /// </summary>
        /// <value><c>true</c> if has attack cursor; otherwise, <c>false</c>.</value>
        public bool HasAttackCursor {
            get { return CheckStatus(CellStatus.AttackCursor, false); }
            set { SwitchStatus(CellStatus.AttackCursor, value); }
        }

        /// <summary>
        /// 地图对象
        /// </summary>
        /// <value>The map object.</value>
        public MapObject MapObject {
            get => mapObject;
            set {
                mapObject = value;
                SwitchStatus(CellStatus.MapObject, value != null);
            }
        }

        /// <summary>
        /// 是否有地图对象
        /// </summary>
        /// <value><c>true</c> if has map object; otherwise, <c>false</c>.</value>
        public bool HasMapObject => MapObject != null;

        /// <summary>
        /// 是否可移动
        /// </summary>
        /// <value><c>true</c> if can move; otherwise, <c>false</c>.</value>
        public bool CanMove {
            get { return HasTile && !HasMapObject; }
        }

        /// <summary>
        /// 获取状态开关
        /// </summary>
        /// <returns>The status.</returns>
        public CellStatus GetStatus() {
            return status;
        }
        #endregion

        #region Constructor
        public CellData(Vector3Int position) {
            this.position = position;
        }

        public CellData(int x, int y) {
            position = new Vector3Int(x, y, 0);
        }
        #endregion

        #region AStar Field/Property
        /// <summary>
        /// 邻居 CellData
        /// </summary>
        /// <value>The adjacents.</value>
        public List<CellData> Adjacents { get; set; } = new List<CellData>();

        /// <summary>
        /// 寻找的前一个 CellData
        /// </summary>
        /// <value>The previous.</value>
        public CellData Previous { get; set; }

        /// <summary>
        /// AStar 的 G 值, 移动消耗
        /// </summary>
        /// <value>The g.</value>
        public float G { get; set; }

        /// <summary>
        /// AStar 的 H 值, 预计消耗
        /// </summary>
        /// <value>The h.</value>
        public float H { get; set; }

        /// <summary>
        /// AStar 的 F 值, F = G + H
        /// </summary>
        /// <value>The f.</value>
        public float F => G + H;
        #endregion

        #region Reset AStar Method
        public void ResetAStar() {
            Previous = null;
            G = 0;
            H = 0;
        }
        #endregion

        public void Dispose() {
            position = Vector3Int.zero;
            MapObject = null;
            Adjacents = null;
            ResetAStar();
        }

        /// <summary>
        /// 设置状态开关
        /// </summary>
        /// <param name="status">Status.</param>
        /// <param name="isOn">If set to <c>true</c> is on.</param>
        public void SwitchStatus(CellStatus status, bool isOn) {
            if (isOn) {
                this.status |= status;
            } else {
                this.status &= ~status;
            }
        }

        /// <summary>
        /// 开关是否开启
        /// any:
        ///     true 表示, 判断 status 中是否存在开启项
        ///     false 表示, 判断 status 中是否全部开启
        /// </summary>
        /// <returns><c>true</c>, if status was checked, <c>false</c> otherwise.</returns>
        /// <param name="status">Status.</param>
        /// <param name="any">If set to <c>true</c> any.</param>
        public bool CheckStatus(CellStatus status, bool any) {
            return any ? (this.status & status) != 0 : (this.status & status) == status;
        }
    }
}
