namespace Maps.FindPath {
    public interface IHowToFind {
        /// <summary>
        /// 获取检测的 Cell
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="search">Search.</param>
        CellData ChoseCell(PathFinding search);

        /// <summary>
        /// 选择 Cell 后, 是否结束
        /// </summary>
        /// <returns><c>true</c>, if finished on chose was ised, <c>false</c> otherwise.</returns>
        /// <param name="search">Search.</param>
        bool IsFinishedOnChose(PathFinding search);

        /// <summary>
        /// 计算移动到下一格的消耗
        /// </summary>
        /// <returns>The GP er cell.</returns>
        /// <param name="search">Search.</param>
        /// <param name="adjacent">Adjacent.</param>
        float CalcGPerCell(PathFinding search, CellData adjacent);

        /// <summary>
        /// 无视范围, 直接寻路用, 计算预计消耗值(这里用距离)
        /// </summary>
        /// <returns>The h.</returns>
        /// <param name="search">Search.</param>
        /// <param name="adjacent">Adjacent.</param>
        float CalcH(PathFinding search, CellData adjacent);

        /// <summary>
        /// 是否能把邻居加入到检测列表中
        /// </summary>
        /// <returns><c>true</c>, if add adjacent to reachable was caned, <c>false</c> otherwise.</returns>
        /// <param name="search">Search.</param>
        /// <param name="adjacent">Adjacent.</param>
        bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent);

        /// <summary>
        /// 生成最终显示的范围
        /// </summary>
        /// <param name="search">Search.</param>
        void BuildResult(PathFinding search);
    }
}
