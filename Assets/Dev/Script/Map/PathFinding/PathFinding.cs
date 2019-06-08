using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Maps.FindPath {
    public class PathFinding {
        public MapGraph map;

        /// <summary>
        /// 开放列表
        /// </summary>
        public List<CellData> reachable = new List<CellData>();

        /// <summary>
        /// 关闭列表
        /// </summary>
        public List<CellData> explored = new List<CellData>();

        /// <summary>
        /// 结果
        /// </summary>
        public List<CellData> result = new List<CellData>();

        public Vector2 range;
        public CellData startCell;
        public CellData endCell;
        public CellData currentCell;
        public bool finished;
        public int searchCount;

        private IHowToFind howToFind;
        private MoveConsumption moveConsumption;

        // 最大迭代次数
        public int maxSearchCount = 2000;

        #region Constructor
        public PathFinding(MapGraph map) {
            this.map = map;
        }
        #endregion

        /// <summary>
        /// 搜寻下一次, return finished
        /// </summary>
        /// <returns><c>true</c>, if next was found, <c>false</c> otherwise.</returns>
        private bool FindNext() {
            if (result.Count > 0) {
                return true;
            }

            // 选择节点
            currentCell = howToFind.ChoseCell(this);

            // 判断是否搜索结束
            if (howToFind.IsFinishedOnChose(this)) {
                // 如果结束, 建立结果
                howToFind.BuildResult(this);
                return true;
            }

            // 当前选择的节点不为 null
            if (currentCell != null) {
                for (int i = 0; i < currentCell.Adjacents.Count; i++) {
                    // 是否可以加入到开放集中
                    if (howToFind.CanAddAdjacentToReachable(this, currentCell.Adjacents[i])) {
                        reachable.Add(currentCell.Adjacents[i]);
                    }
                }
            }
            return false;
        }

        private bool SearchRangeInternal() {
            while (!finished) {
                searchCount++;
                finished = FindNext();
                if (searchCount >= maxSearchCount) {
                    Debug.LogError("Search is timeout. MaxCount: " + maxSearchCount.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        /// <returns><c>true</c>, if move range was searched, <c>false</c> otherwise.</returns>
        /// <param name="howToFind">How to find.</param>
        /// <param name="start">Start.</param>
        /// <param name="movePoint">Move point.</param>
        /// <param name="consumption">Consumption.</param>
        public bool SearchMoveRange(IHowToFind howToFind, CellData start, float movePoint, MoveConsumption consumption) {
            if (howToFind == null || start == null || movePoint < 0) {
                return false;
            }

            Reset();

            this.howToFind = howToFind;
            moveConsumption = consumption;

            startCell = start;
            startCell.ResetAStar();
            range.y = movePoint;

            reachable.Add(startCell);

            return SearchRangeInternal();
        }

        /// <summary>
        /// 寻找攻击范围
        /// </summary>
        /// <returns><c>true</c>, if attack range was searched, <c>false</c> otherwise.</returns>
        /// <param name="howToFind">How to find.</param>
        /// <param name="start">Start.</param>
        /// <param name="minRange">Minimum range.</param>
        /// <param name="maxRange">Max range.</param>
        /// <param name="useEndCell">If set to <c>true</c> use end cell.</param>
        public bool SearchAttackRange(IHowToFind howToFind, CellData start, int minRange, int maxRange, bool useEndCell = false) {
            if (howToFind == null || start == null || minRange < 1 || maxRange < minRange) {
                return false;
            }

            Reset();

            this.howToFind = howToFind;
            range = new Vector2(minRange, maxRange);

            // 在重置时, 不重置父亲节点
            // 其一: 没有用到
            // 其二: 二次查找时不破坏路径, 否则路径将被破坏
            if (useEndCell) {
                endCell = start;
                endCell.G = 0f;
                reachable.Add(endCell);
            } else {
                startCell = start;
                startCell.G = 0f;
                reachable.Add(startCell);
            }

            return SearchRangeInternal();
        }

        public bool SearchPath(IHowToFind howToFind, CellData start, CellData end, MoveConsumption consumption) {
            if (howToFind == null || start == null || end == null) {
                return false;
            }

            Reset();

            this.howToFind = howToFind;
            moveConsumption = consumption;
            startCell = start;
            startCell.ResetAStar();
            endCell = end;
            endCell.ResetAStar();

            reachable.Add(startCell);

            startCell.H = this.howToFind.CalcH(this, startCell);

            return SearchRangeInternal();
        }

        /// <summary>
        /// 建立路径 List
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="endCell">End cell.</param>
        /// <param name="useResult">If set to <c>true</c> use result.</param>
        public List<CellData> BuildPath(CellData endCell, bool useResult) {
            if (endCell == null) {
                Debug.LogError("PathFinding -> Argument named 'endCell' is null.");
                return null;
            }

            List<CellData> path = useResult ? result : new List<CellData>();

            CellData current = endCell;
            path.Add(current);
            while (current.Previous != null) {
                current = current.Previous;
                path.Insert(0, current);
            }
            return path;
        }

        /// <summary>
        /// 建立路径 Stack
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="endCell">End cell.</param>
        public Stack<CellData> BuildPath(CellData endCell) {
            if (endCell == null) {
                Debug.LogError("PathFinding -> Argument named 'endCell' is null.");
                return null;
            }

            Stack<CellData> path = new Stack<CellData>();

            CellData current = endCell;
            path.Push(current);
            while (current.Previous != null) {
                current = current.Previous;
                path.Push(current);
            }
            return path;
        }

        /// <summary>
        /// 获取移动消耗
        /// </summary>
        /// <returns>The move consumption.</returns>
        /// <param name="terrainType">Terrain type.</param>
        public float GetMoveConsumption(TerrainType terrainType) {
            if (moveConsumption == null) {
                return 1f;
            }
            return moveConsumption[terrainType];
        }

        #region Reset Method
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() {
            reachable.Clear();
            explored.Clear();
            result.Clear();

            range = Vector2.zero;
            startCell = null;
            endCell = null;
            currentCell = null;
            finished = false;
            howToFind = null;
            moveConsumption = null;

            searchCount = 0;
        }
        #endregion

        #region Helper
        /// <summary>
        /// 是否在 Reachable 列表中
        /// </summary>
        /// <returns><c>true</c>, if cell in reachable was ised, <c>false</c> otherwise.</returns>
        /// <param name="cell">Cell.</param>
        public bool IsCellInReachable(CellData cell) {
            return reachable.Contains(cell);
        }

        /// <summary>
        /// 是否在 Explored 列表中
        /// </summary>
        /// <returns><c>true</c>, if cell in expored was ised, <c>false</c> otherwise.</returns>
        /// <param name="cell">Cell.</param>
        public bool IsCellInExplored(CellData cell) {
            return explored.Contains(cell);
        }
        #endregion
    }
}
