using System.Linq;
using UnityEngine;

namespace Maps.FindPath {
    [CreateAssetMenu(fileName = "FindPathDirect.asset", menuName = "SRPG/How to find path")]
    public class FindPathDirect : FindMoveRange {
        public override void BuildResult(PathFinding search) {
            // 当没有达到目标点时, 已经建立过结果
            if (search.result.Count > 0) {
                return;
            }

            search.BuildPath(search.endCell, true);
        }

        public override float CalcH(PathFinding search, CellData adjacent) {
            Vector2 hVec;
            hVec.x = Mathf.Abs(adjacent.Position.x - search.endCell.Position.x);
            hVec.y = Mathf.Abs(adjacent.Position.y - search.endCell.Position.y);
            return hVec.x + hVec.y;
        }

        public override bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent) {
            // 没有 Tile
            //if (!adjacent.HasTile) {
            //    return false;
            //}

            // 已经有对象了
            //if (adjacent.HasMapObject) {
            //    return false;
            //}

            // 是否可移动
            if (!adjacent.CanMove) {
                return false;
            }

            // 如果已经在关闭集
            if (search.IsCellInExplored(adjacent)) {
                return false;
            }

            // 计算消耗 = 当前 cell 的消耗 + 邻居 cell 的消耗
            float g = search.currentCell.G + CalcGPerCell(search, adjacent);

            // 已经加入过开放集
            if (search.IsCellInReachable(adjacent)) {
                // 如果新消耗更低
                if (g < adjacent.G) {
                    adjacent.G = g;
                    adjacent.Previous = search.currentCell;
                }
                return false;
            }

            adjacent.G = g;
            adjacent.H = CalcH(search, adjacent);
            adjacent.Previous = search.currentCell;
            return true;
        }

        public override bool IsFinishedOnChose(PathFinding search) {
            // 如果开放集中已经空了, 则说明没有达到目标点
            if (search.currentCell == null) {
                // 使用 H 最小值建立结果
                CellData minCell = search.explored.First(cell => System.Math.Abs(cell.H - search.explored.Min(c => c.H)) < 1e-6);
                search.BuildPath(minCell, true);
                return true;
            }

            // 找到了目标点
            if (search.currentCell == search.endCell) {
                return true;
            }

            if (!search.IsCellInExplored(search.currentCell)) {
                search.explored.Add(search.currentCell);
            }

            return false;
        }
    }
}
