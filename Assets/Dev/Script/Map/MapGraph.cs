using UnityEngine;
using System.Collections.Generic;
using Maps.FindPath;
using UnityEngine.Tilemaps;
using Models;
using DR.Book.SRPG_Dev.Framework;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maps {
    public class MapGraph : MonoBehaviour {
#if UNITY_EDITOR
        [Header("Editor Gizmos")]
        public bool editorDrawGizmos = true;
        public Color editorBorderColor = Color.white;
        public Color editorCellColor = Color.green;
        public Color editorErrorColor = Color.red;

        private void OnDrawGizmos() {
            if (editorDrawGizmos) {
                EditorDrawBorderGizmos();
            }
        }

        private void OnDrawGizmosSelected() {
            if (editorDrawGizmos) {
                EditorDrawCellGizmos();
            }
        }

        protected void EditorDrawBorderGizmos() {
            Color old = Gizmos.color;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = editorBorderColor;

            // 获取边框左下角与右上角的世界坐标
            Vector3 leftDown = Grid.GetCellCenterWorld(LeftDownPosition) - HalfCellSize;
            Vector3 rightUp = Grid.GetCellCenterWorld(RightUpPosition) + HalfCellSize;

            // 绘制左下角 Cell 与右上角 Cell 的 Position
            Handles.Label(leftDown, (new Vector2Int(LeftDownPosition.x, LeftDownPosition.y)).ToString(), textStyle);
            Handles.Label(rightUp, (new Vector2Int(RightUpPosition.x, RightUpPosition.y)).ToString(), textStyle);

            if (mapRect.width > 0 && mapRect.height > 0) {
                Gizmos.color = editorBorderColor;

                // 边框的长与宽
                Vector3 size = Vector3.Scale(new Vector3(Width, Height), Grid.cellSize);

                // 边框的中心坐标
                Vector3 center = leftDown + size / 2f;

                // 绘制边框
                Gizmos.DrawWireCube(center, size);
            }

            Gizmos.color = old;
        }

        protected void EditorDrawCellGizmos() {
            Event e = Event.current;
            Vector2 mousePosition = e.mousePosition;

            // 处理缩放
            mousePosition.Scale(new Vector2(2f, 2f));

            // 获取当前操作的 Scene 面板
            SceneView sceneView = SceneView.currentDrawingSceneView;

            /// 获取世界坐标
            /// Event 是从左上角 (Left Up) 开始
            /// 而 Camera 是从左下角 (Left Down)
            /// 需要转换才能使用 Camera 的 ScreenToWorldPoint 方法
            Vector2 screenPosition = new Vector2(mousePosition.x, sceneView.camera.pixelHeight - mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在 Cell 的 Position
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            // 当前鼠标所在 Cell 的 Center 坐标
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);

            /// 绘制当前鼠标下的 Cell 边框与 Position
            /// 如果包含 Cell, 正常绘制
            /// 如果不包含 Cell, 改变颜色, 并多绘制一个叉
            GUIStyle textStyle = new GUIStyle();
            if (Contains(cellPosition)) {
                textStyle.normal.textColor = editorCellColor;
                Gizmos.color = editorCellColor;
            } else {
                textStyle.normal.textColor = editorErrorColor;
                Gizmos.color = editorErrorColor;

                // 绘制 Cell 对角线
                Vector3 from = cellCenter - HalfCellSize;
                Vector3 to = cellCenter + HalfCellSize;
                Gizmos.DrawLine(from, to);
                float tmpX = from.x;
                from.x = to.x;
                to.x = tmpX;
                Gizmos.DrawLine(from, to);
            }
            Handles.Label(cellCenter - HalfCellSize, (new Vector2Int(cellPosition.x, cellPosition.y)).ToString(), textStyle);
            Gizmos.DrawWireCube(cellCenter, grid.cellSize);
        }
#endif

        public string mapName = "MapGraph";

        public RectInt mapRect = new RectInt(0, 0, 24, 24);

        private Grid grid;
        public Grid Grid {
            get {
                if (grid == null) {
                    grid = GetComponent<Grid>();
                }
                return grid;
            }
        }

        public Vector3 HalfCellSize {
            get { return Grid.cellSize / 2f; }
        }

        public Vector3Int LeftDownPosition {
            get { return new Vector3Int(mapRect.xMin, mapRect.yMin, 0); }
        }

        public Vector3Int RightUpPosition {
            get { return new Vector3Int(mapRect.xMax - 1, mapRect.yMax - 1, 0); }
        }

        public int Width {
            get { return mapRect.width; }
        }

        public int Height {
            get { return mapRect.height; }
        }

        /// <summary>
        ///  计算的 Tilemap
        /// </summary>
        /// <value>The terrain tilemap.</value>
        public Tilemap TerrainTilemap;

        /// <summary>
        /// 地图每个格子的信息
        /// </summary>
        private Dictionary<Vector3Int, CellData> dataDict = new Dictionary<Vector3Int, CellData>();

        /// <summary>
        /// 获取 CellData
        /// </summary>
        /// <returns>The cell data.</returns>
        /// <param name="position">Position.</param>
        public CellData GetCellData(Vector3Int position) {
            if (!Contains(position)) {
                return null;
            }
            return dataDict[position];
        }

        /// <summary>
        /// 地图是否包含 Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position) {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }

        #region Map Object Field
        [Header("Map Object Setting")]
        [SerializeField]
        private ObjectPool mapObjectPool;
        [SerializeField]
        private ObjectPool mapCursorPool;
        [SerializeField]
        private MapMouseCursor mouseCursorPrefab;
        [SerializeField]
        private MapCursor cursorPrefab;

        /// <summary>
        /// MapCursor 父对象
        /// </summary>
        /// <value>The map cursor pool.</value>
        public ObjectPool MapCursorPool {
            get => mapCursorPool;
            set => mapCursorPool = value;
        }

        /// <summary>
        /// 生成的 MapMouseCursor
        /// </summary>
        private MapMouseCursor mouseCursor;

        /// <summary>
        /// 运行时, MapCursor 的预制体
        /// </summary>
        private MapCursor runtimeCursorPrefab;

        /// <summary>
        /// 移动范围光标集合
        /// </summary>
        //private List<MapCursor> mapMoveCursors = new List<MapCursor>();

        /// <summary>
        /// 攻击范围光标集合
        /// </summary>
        //private List<MapCursor> mapAttackCursors = new List<MapCursor>();

        /// <summary>
        /// 光标集合
        /// </summary>
        private HashSet<MapCursor> cursors = new HashSet<MapCursor>();

        /// <summary>
        /// 职业集合
        /// </summary>
        private List<MapClass> classes = new List<MapClass>();
        #endregion

        #region Map Object Property
        /// <summary>
        /// MapObject 父对象
        /// </summary>
        /// <value>The map object pool.</value>
        public ObjectPool MapObjectPool {
            get => mapObjectPool;
            set => mapObjectPool = value;
        }

        /// <summary>
        /// 默认 mouse cursor 的 prefab
        /// </summary>
        /// <value>The mouse cursor prefab.</value>
        public MapMouseCursor MouseCursorPrefab {
            get => mouseCursorPrefab;
            set => mouseCursorPrefab = value;
        }

        /// <summary>
        /// 默认 cursor 的 prefab
        /// </summary>
        /// <value>The cursor prefab.</value>
        public MapCursor CursorPrefab {
            get => cursorPrefab;
            set => cursorPrefab = value;
        }

        /// <summary>
        /// 用户光标
        /// </summary>
        /// <value>The mouse cursor.</value>
        public MapMouseCursor MouseCursor {
            get {

                // 只有在测试时, 才都会使用默认 prefab  创建
                // 正式游戏, 这里不会为 null
                // 将在初始化地图时创建用户光标
                // 如果游戏无法初始化光标, 则需要检查代码是否正确
                if (mouseCursor == null) {
                    mouseCursor = CreateMapObject(mouseCursorPrefab) as MapMouseCursor;
                }
                return mouseCursor;
            }
            private set => mouseCursor = value;
        }

        /// <summary>
        /// 运行时, MapCursor 的预制体
        /// </summary>
        /// <value>The runtime cursor prefab.</value>
        public MapCursor RuntimeCursorPrefab {
            get {
                // 只有在测试时, 才会都使用默认 prefab
                // 正式游戏, 这里不会为 null
                // 将在初始化地图时加载预制体
                // 如果游戏无法加载预制体, 则需要检查代码是否正确
                if (runtimeCursorPrefab == null) {
                    runtimeCursorPrefab = cursorPrefab;
                }
                return runtimeCursorPrefab;
            }
            private set => runtimeCursorPrefab = value;
        }

        /// <summary>
        /// 显示 cursor
        /// </summary>
        /// <param name="cells">Cells.</param>
        /// <param name="type">Type.</param>
        public void ShowRangeCursors(IEnumerable<CellData> cells, MapCursor.MapCursorType type) {
            if (type == MapCursor.MapCursorType.Mouse) {
                return;
            }

            foreach (CellData cell in cells) {
                MapCursor cursor = CreateMapObject(runtimeCursorPrefab, cell.Position) as MapCursor;
                if (cursor != null) {
                    cursor.name = string.Format(
                        "{0} Cursor {1}",
                        type.ToString(),
                        cell.Position.ToString());
                    cursor.CursorType = type;
                    if (type == MapCursor.MapCursorType.Move) {
                        //mapMoveCursors.Add(cursor);
                        cell.HasMoveCursor = true;
                    } else if (type == MapCursor.MapCursorType.Attack) {
                        //mapAttackCursors.Add(cursor);
                        cell.HasAttackCursor = true;
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏 cursor
        /// </summary>
        public void HideRangeCursors() {
            //if (mapMoveCursors.Count > 0) {
            //    for (int i = 0; i < mapMoveCursors.Count; i++) {
            //        ObjectPool.DespawnUnsafe(mapMoveCursors[i].gameObject, true);
            //    }
            //    mapMoveCursors.Clear();
            //}

            //if (mapAttackCursors.Count > 0) {
            //    for (int i = 0; i < mapAttackCursors.Count; i++) {
            //        ObjectPool.DespawnUnsafe(mapAttackCursors[i].gameObject, true);
            //    }
            //    mapAttackCursors.Clear();
            //}

            if (cursors.Count > 0) {
                foreach (MapCursor cursor in cursors) {
                    ObjectPool.DespawnUnsafe(cursor.gameObject, true);
                }
                cursors.Clear();
            }
        }
        #endregion

        #region Map Object Method
        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <returns>The map object.</returns>
        /// <param name="prefab">Prefab.</param>
        public MapObject CreateMapObject(MapObject prefab) {
            if (prefab == null) {
                Debug.LogError("MapGraph -> CreateMapObject Error! Prefab is null.");
                return null;
            }

            MapObjectType type = prefab.MapObjectType;

            // 用户光标在整个地图中只能有且只有一个
            if (type == MapObjectType.MouseCursor && MouseCursor != null) {
                ObjectPool.DespawnUnsafe(MouseCursor.gameObject, true);
            }

            //实例化 map object
            GameObject instance;
            if (type == MapObjectType.Cursor || type == MapObjectType.MouseCursor) {
                instance = mapCursorPool.Spawn(prefab.gameObject);
            } else {
                instance = mapObjectPool.Spawn(prefab.gameObject);
            }

            MapObject mapObject = instance.GetComponent<MapObject>();

            mapObject.InitMapObject(this);

            if (type == MapObjectType.MouseCursor) {
                MouseCursor = mapObject as MapMouseCursor;
            }

            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <returns>The map object.</returns>
        /// <param name="prefab">Prefab.</param>
        /// <param name="cellPosition">Cell position.</param>
        public MapObject CreateMapObject(MapObject prefab, Vector3Int cellPosition) {
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null) {
                mapObject.UpdatePosition(cellPosition);
            }
            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <returns>The map object.</returns>
        /// <param name="prefabName">Prefab name.</param>
        public MapObject CreateMapObject(string prefabName) {
            MapObject prefab = LoadMapObjectPrefab(prefabName);
            return CreateMapObject(prefab);
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <returns>The map object.</returns>
        /// <param name="prefabName">Prefab name.</param>
        /// <param name="cellPosition">Cell position.</param>
        public MapObject CreateMapObject(string prefabName, Vector3Int cellPosition) {
            MapObject prefab = LoadMapObjectPrefab(prefabName);
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null) {
                mapObject.UpdatePosition(cellPosition);
            }
            return mapObject;
        }

        /// <summary>
        /// 读取 Prefab
        /// </summary>
        /// <returns>The map object prefab.</returns>
        /// <param name="prefabName">Prefab name.</param>
        private MapObject LoadMapObjectPrefab(string prefabName) {
            if (string.IsNullOrEmpty(prefabName)) {
                Debug.LogError("MapGraph -> LoadMapObjectPrefab error! Prefab name is null or empty");
                return null;
            }

            // TODO ResourcesManager 读取 prefab
            MapObject prefab = Resources.Load<MapObject>(prefabName);
            return prefab;
        }
        #endregion

        #region Path Finding Field
        /// <summary>
        ///  寻路核心
        /// </summary>
        public PathFinding searchPath;

        /// <summary>
        /// 寻找攻击范围
        /// </summary>
        [Header("Path Finding")]
        [SerializeField]
        public FindRange findAttackRange;

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        [SerializeField]
        public FindRange findMoveRange;

        /// <summary>
        /// 无视移动力, 直接寻找路径
        /// </summary>
        [SerializeField]
        public FindRange findPathDirect;
        #endregion

        #region Unity Callback
        private void OnDestroy() {
            ClearCellDatas();
        }
        #endregion

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="reinitCellDatas">If set to <c>true</c> reinit cell datas.</param>
        public void InitMap(bool reinitCellDatas = false) {
            if (reinitCellDatas) {
                ClearCellDatas();
            }

            CreateCellDatas();

            if (searchPath == null) {
                searchPath = new PathFinding(this);
            }

            // TODO other init
        }

        /// <summary>
        /// 初始化加载地图时地图对象
        /// </summary>
        private void InitMapObjectsInMap() {
            if (mapObjectPool == null) {
                Debug.LogError("MapGraph -> MapObject Pool is null.");
                return;
            }

            MapObject[] mapObjects = mapObjectPool.gameObject.GetComponentsInChildren<MapObject>();
            if (mapObjects != null) {
                foreach (MapObject mapObject in mapObjects) {
                    // 我们的地图对象不应包含 Cursor 相关的物体
                    if (mapObject.MapObjectType == MapObjectType.MouseCursor || mapObject.MapObjectType == MapObjectType.Cursor) {
                        Destroy(mapObject.gameObject);
                        continue;
                    }

                    // 初始化
                    mapObject.InitMapObject(this);

                    // 更新坐标
                    Vector3 world = mapObject.transform.position;
                    Vector3Int cellPosition = grid.WorldToCell(world);
                    mapObject.CellPosition = cellPosition;

                    // 设置 CellData
                    CellData cellData = GetCellData(cellPosition);
                    if (cellData != null) {
                        if (cellData.HasMapObject) {
                            Debug.LogErrorFormat("MapObject in Cell {0} already exists.", cellPosition.ToString());
                            continue;
                        }
                        cellData.MapObject = mapObject;
                    }

                    if (mapObject.MapObjectType == MapObjectType.Class) {
                        RuntimePrePoolObject runtime = mapObject.GetComponent<RuntimePrePoolObject>();
                        if (runtime != null && !runtime.enabled) {
                            runtime.m_PoolName = mapObjectPool.poolName;
                            runtime.enabled = true;
                        }
                        MapClass cls = mapObject as MapClass;
                        cls.Load(0); // TODO Load Data
                        if (!classes.Contains(cls)) {
                            classes.Add(cls);
                        }
                    }

                    mapObject.gameObject.name += mapObject.CellPosition.ToString();
                }
            }
        }

        /// <summary>
        /// 建立 CellData
        /// </summary>
        private void CreateCellDatas() {
            if (dataDict.Count != 0) {
                return;
            }

            for (int y = mapRect.yMin; y < mapRect.yMax; y++) {
                for (int x = mapRect.xMin; x < mapRect.xMax; x++) {
                    CellData cell = new CellData(x, y);
                    dataDict.Add(cell.Position, cell);

                }
            }

            foreach (CellData cell in dataDict.Values) {
                cell.HasTile = GetTile(cell.Position) != null;
                FindAdjacents(cell);
            }
        }

        /// <summary>
        /// 添加邻居
        /// </summary>
        /// <param name="cell">Cell.</param>
        private void FindAdjacents(CellData cell) {
            cell.Adjacents.Clear();
            Vector3Int position = cell.Position;
            Vector3Int pos;

            // up
            pos = new Vector3Int(position.x, position.y + 1, position.z);
            if (Contains(pos)) {
                cell.Adjacents.Add(dataDict[pos]);
            }

            // right
            pos = new Vector3Int(position.x + 1, position.y, position.z);
            if (Contains(pos)) {
                cell.Adjacents.Add(dataDict[pos]);
            }

            // down
            pos = new Vector3Int(position.x, position.y - 1, position.z);
            if (Contains(pos)) {
                cell.Adjacents.Add(dataDict[pos]);
            }

            // left
            pos = new Vector3Int(position.x - 1, position.y, position.z);
            if (Contains(pos)) {
                cell.Adjacents.Add(dataDict[pos]);
            }
        }

        /// <summary>
        /// 获取 Terrain 层的 Tile
        /// </summary>
        /// <returns>The tile.</returns>
        /// <param name="position">Position.</param>
        public SRPGTile GetTile(Vector3Int position) {
            return TerrainTilemap.GetTile<SRPGTile>(position);
        }

        /// <summary>
        /// 获取 Cell 的位置
        /// </summary>
        /// <returns>The cell position.</returns>
        /// <param name="cellPosition">Cell position.</param>
        /// <param name="world">If set to <c>true</c> world.</param>
        /// <param name="center">If set to <c>true</c> center.</param>
        public Vector3 GetCellPosition(Vector3Int cellPosition, bool world = true, bool center = false) {
            Vector3 pos;

            if (world) {
                pos = grid.GetCellCenterWorld(cellPosition);
            } else {
                pos = grid.GetCellCenterLocal(cellPosition);
            }

            if (!center) {
                pos.y -= HalfCellSize.y;
            }

            return pos;
        }

        /// <summary>
        /// 删除已有的 CellData
        /// </summary>
        public void ClearCellDatas() {
            if (dataDict.Count > 0) {
                if (mapObjectPool != null) {
                    mapObjectPool.DespawnAll();
                }
                foreach (CellData cell in dataDict.Values) {
                    cell.Dispose();
                }
                dataDict.Clear();
            }
        }

        /// <summary>
        /// 搜寻移动范围
        /// </summary>
        /// <returns>The move range.</returns>
        /// <param name="cell">Cell.</param>
        /// <param name="movePoint">Move point.</param>
        /// <param name="consumption">Consumption.</param>
        public List<CellData> SearchMoveRange(CellData cell, float movePoint, MoveConsumption consumption) {
            if (findMoveRange == null) {
                Debug.LogError("Error: Find move range is null.");
                return null;
            }

            if (!searchPath.SearchMoveRange(findMoveRange, cell, movePoint, consumption)) {
                Debug.LogErrorFormat("Error: Move range({0}) is not found.", 5f);
                return null;
            }

            return searchPath.result;
        }

        /// <summary>
        /// 搜寻移动范围与攻击范围
        /// </summary>
        /// <returns><c>true</c>, if move range was searched, <c>false</c> otherwise.</returns>
        /// <param name="cls">Cls.</param>
        /// <param name="nAtk">是否包含攻击范围</param>
        /// <param name="moveCells">Move cells.</param>
        /// <param name="atkCells">Atk cells.</param>
        public bool SearchMoveRange(MapClass cls, bool nAtk, out IEnumerable<CellData> moveCells, out IEnumerable<CellData> atkCells) {
            moveCells = null;
            atkCells = null;

            if (cls == null) {
                Debug.LogError("MapGraph -> SearchMoveRange: 'cls' is null.");
                return false;
            }

            CellData cell = GetCellData(cls.CellPosition);
            if (cell == null) {
                Debug.LogError("MapGraph -> SearchMoveRange: 'cls.CellPosition' is out of range.");
                return false;
            }

            // TODO 搜索移动范围, 从 MapClass 中读取数据
            float movePoint = 0;
            MoveConsumption consumption = null;

            List<CellData> rangeCells = SearchMoveRange(cell, movePoint, consumption);
            if (rangeCells == null) {
                return false;
            }

            moveCells = rangeCells.ToArray();

            if (nAtk /* TODO && 是否有武器*/) {
                // TODO 搜索攻击范围, 从 MapClass 中读取数据
                Vector2Int atkRange = Vector2Int.one;

                HashSet<CellData> atkRangeCells = new HashSet<CellData>(cellPositionEqualityComparer);
                foreach (CellData moveCell in moveCells) {
                    rangeCells = SearchAttackRange(moveCell, atkRange.x, atkRange.y, true);
                    if (rangeCells != null && rangeCells.Count > 0) {
                        atkRangeCells.UnionWith(rangeCells.Where(c => !c.HasCursor));
                    }
                }

                atkCells = atkRangeCells;
            }

            return true;
        }

        /// <summary>
        /// 搜寻和显示范围
        /// </summary>
        /// <returns><c>true</c>, if and show move range was searched, <c>false</c> otherwise.</returns>
        /// <param name="cls">Cls.</param>
        /// <param name="nAtk">是否包含攻击范围</param>
        public bool SearchAndShowMoveRange(MapClass cls, bool nAtk) {
            IEnumerable<CellData> moveCells, atkCells;
            if (!SearchMoveRange(cls, nAtk, out moveCells, out atkCells)) {
                return false;
            }

            if (moveCells != null) {
                ShowRangeCursors(moveCells, MapCursor.MapCursorType.Move);
            }

            if (atkCells != null) {
                ShowRangeCursors(atkCells, MapCursor.MapCursorType.Attack);
            }

            return true;
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <returns>The attack range.</returns>
        /// <param name="cell">Cell.</param>
        /// <param name="minRange">Minimum range.</param>
        /// <param name="maxRange">Max range.</param>
        /// <param name="useEndCell">If set to <c>true</c> use end cell.</param>
        public List<CellData> SearchAttackRange(CellData cell, int minRange, int maxRange, bool useEndCell = false) {
            if (findAttackRange == null) {
                Debug.LogError("Error: Find attack range is null.");
                return null;
            }

            if (!searchPath.SearchAttackRange(findAttackRange, cell, minRange, maxRange, useEndCell)) {
                Debug.LogErrorFormat("Error: Attack range({0}) - ({1}) is not found.", 2, 3);
                return null;
            }

            return searchPath.result;
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="startCell">Start cell.</param>
        /// <param name="endCell">End cell.</param>
        /// <param name="consumption">Consumption.</param>
        public List<CellData> SearchPath(CellData startCell, CellData endCell, MoveConsumption consumption) {
            if (findPathDirect == null) {
                Debug.LogError("Error: Find path is null.");
                return null;
            }

            if (!searchPath.SearchPath(findPathDirect, startCell, endCell, consumption)) {
                Debug.LogError("Error: Search path error. Maybe some cells are out of range.");
                return null;
            }

            return searchPath.result;
        }

        #region class CellPositionEqualityComparer
        private CellPositionEqualityComparer cellPositionEqualityComparer = new CellPositionEqualityComparer();

        /// <summary>
        /// 判断两个 Cell 的 Position 是否相等
        /// </summary>
        public CellPositionEqualityComparer GetCellPositionEqualityComparer() {
            if (cellPositionEqualityComparer == null) {
                cellPositionEqualityComparer = new CellPositionEqualityComparer();
            }
            return cellPositionEqualityComparer;
        }

        /// <summary>
        /// 判断两个 Cell 的 Position 是否相等
        /// </summary>
        public class CellPositionEqualityComparer : IEqualityComparer<CellData> {
            public bool Equals(CellData x, CellData y) {
                return x.Position == y.Position;
            }

            public int GetHashCode(CellData obj) {
                return obj.Position.GetHashCode();
            }
        }
        #endregion
    }
}
