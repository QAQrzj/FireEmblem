using UnityEngine;

namespace Maps.FindPath {
    [CreateAssetMenu(fileName = "FindMoveRange.asset", menuName = "SRPG/How to find move range")]
    public class FindMoveRange : FindRange {
        public override CellData ChoseCell(PathFinding search) {
            if (search.reachable.Count == 0) {
                return null;
            }

            // 取得 F 最小的节点
            search.reachable.Sort((cell1, cell2) => -cell1.F.CompareTo(cell2.F));
            int index = search.reachable.Count - 1;
            CellData chose = search.reachable[index];
            search.reachable.RemoveAt(index);
            return chose;
        }

        public override float CalcGPerCell(PathFinding search, CellData adjacent) {
            // 获取邻居的 Tile
            SRPGTile tile = search.map.GetTile(adjacent.Position);

            // 返回本格子的消耗
            return search.GetMoveConsumption(tile.terrainType);
        }

        public override bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent) {
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

            // 不在范围内
            if (g < 0f || g > search.range.y) {
                return false;
            }

            adjacent.G = g;
            adjacent.Previous = search.currentCell;
            return true;
        }
    }
}
