using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
using SceneView = UnityEditor.SceneView;
#endif

namespace Maps {
    using Dev;
    using DR.Book.SRPG_Dev.Framework;
    using FindPath;
    using Models;

    [RequireComponent(typeof(Grid))]
    public class MapGraph : MonoBehaviour {

#if UNITY_EDITOR
        #region Gizmos
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

        /// <summary>
        /// 绘制 Border 的 Gizmos
        /// </summary>
        protected void EditorDrawBorderGizmos() {
            Color old = Gizmos.color;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = editorBorderColor;

            // 获取边框左下角与右上角的世界坐标
            Vector3 leftDown = Grid.GetCellCenterWorld(LeftDownPosition) - HalfCellSize;
            Vector3 rightUp = Grid.GetCellCenterWorld(RightUpPosition) + HalfCellSize;

            // 绘制左下角 Cell 与右上角 Cell 的 Position
            Handles.Label(leftDown, new Vector2Int(LeftDownPosition.x, LeftDownPosition.y).ToString(), textStyle);
            Handles.Label(rightUp, new Vector2Int(RightUpPosition.x, RightUpPosition.y).ToString(), textStyle);

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

        /// <summary>
        /// 绘制 Cell 的 Gizmos
        /// </summary>
        protected void EditorDrawCellGizmos() {
            // 用于获取鼠标位置
            Event e = Event.current;
            if (e.type != EventType.Repaint) {
                return;
            }

            // 获取当前操作 Scene 面板
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null) {
                return;
            }

            Color old = Gizmos.color;

            /// 获取鼠标世界坐标:
            /// Event 是从左上角(Left Up)开始,
            /// 而 Camera 是从左下角(Left Down),
            /// 需要转换才能使用 Camera 的 ScreenToWorldPoint 方法
            Vector2 screenPosition = new Vector2(e.mousePosition.x, sceneView.camera.pixelHeight - e.mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在 Cell 的 Position
            Vector3Int cellPostion = Grid.WorldToCell(worldPosition);
            // 当前鼠标所在 Cell 的 Center 坐标
            Vector3 cellCenter = Grid.GetCellCenterWorld(cellPostion);

            /// 绘制当前鼠标下的 Cell 边框与 Position
            /// 如果包含 Cell, 正常绘制
            /// 如果不包含 Cell, 改变颜色, 并多绘制一个叉
            if (Contains(cellPostion)) {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = editorCellColor;
                Gizmos.color = editorCellColor;

                Handles.Label(cellCenter - HalfCellSize, new Vector2Int(cellPostion.x, cellPostion.y).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, Grid.cellSize);
            } else {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = editorErrorColor;
                Gizmos.color = editorErrorColor;

                Handles.Label(cellCenter - HalfCellSize, new Vector2Int(cellPostion.x, cellPostion.y).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, Grid.cellSize);

                // 绘制 Cell 对角线
                Vector3 from = cellCenter - HalfCellSize;
                Vector3 to = cellCenter + HalfCellSize;
                Gizmos.DrawLine(from, to);
                float tmpX = from.x;
                from.x = to.x;
                to.x = tmpX;
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = old;
        }
        #endregion
#endif

        #region Map Setting Field
        [Header("Map Setting")]
        [SerializeField]
        private string mapName;
        [SerializeField]
        private RectInt mapRect = new RectInt(0, 0, 10, 10);
        [SerializeField]
        private Tilemap terrainTilemap;

        /// <summary>
        /// 地图每个格子的信息
        /// </summary>
        private Dictionary<Vector3Int, CellData> dataDict = new Dictionary<Vector3Int, CellData>();

        private Grid grid;
        #endregion

        #region Map Setting Property
        /// <summary>
        /// 地图的名称
        /// </summary>
        public string MapName {
            get => mapName;
            set => mapName = value;
        }

        /// <summary>
        /// 地图的矩形框
        /// </summary>
        public RectInt MapRect {
            get { return mapRect; }
            set { mapRect = value; }
        }

        /// <summary>
        /// 地图左下角 Position
        /// </summary>
        public Vector3Int LeftDownPosition => new Vector3Int(mapRect.xMin, mapRect.yMin, 0);

        /// <summary>
        /// 地图右上角 Position
        /// </summary>
        public Vector3Int RightUpPosition => new Vector3Int(mapRect.xMax - 1, mapRect.yMax - 1, 0);

        /// <summary>
        /// 地图宽
        /// </summary>
        public int Width => mapRect.width;

        /// <summary>
        /// 地图高
        /// </summary>
        public int Height => mapRect.height;

        /// <summary>
        /// 计算的 Tilemap
        /// </summary>
        public Tilemap TerrainTilemap {
            get => terrainTilemap;
            set => terrainTilemap = value;
        }

        /// <summary>
        /// Grid 组件
        /// </summary>
        public Grid Grid {
            get {
                if (grid == null) {
                    grid = GetComponent<Grid>();
                }
                return grid;
            }
        }

        /// <summary>
        /// 地图每个 cell 尺寸的一半
        /// </summary>
        public Vector3 HalfCellSize => Grid.cellSize / 2f;
        #endregion

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
        /// 生成的 MapMouseCursor
        /// </summary>
        private MapMouseCursor mouseCursor;

        /// <summary>
        /// 运行时, MapCursor 的预制体
        /// </summary>
        private MapCursor runtimeCursorPrefab;

        ///// <summary>
        ///// 移动范围光标集合
        ///// </summary>
        //private List<MapCursor> m_MapMoveCursors = new List<MapCursor>();

        ///// <summary>
        ///// 攻击范围光标集合
        ///// </summary>
        //private List<MapCursor> m_MapAttackCursors = new List<MapCursor>();

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
        public ObjectPool MapObjectPool {
            get => mapObjectPool;
            set => mapObjectPool = value;
        }

        /// <summary>
        /// MapCursor 父对象
        /// </summary>
        public ObjectPool MapCursorPool {
            get => mapCursorPool;
            set => mapCursorPool = value;
        }

        /// <summary>
        /// 默认 mouse cursor 的 prefab
        /// </summary>
        public MapMouseCursor MouseCursorPrefab {
            get => mouseCursorPrefab;
            set => mouseCursorPrefab = value;
        }

        /// <summary>
        /// 默认 cursor 的 prefab
        /// </summary>
        public MapCursor CursorPrefab {
            get => cursorPrefab;
            set => cursorPrefab = value;
        }

        /// <summary>
        /// 用户光标
        /// </summary>
        public MapMouseCursor MouseCursor {
            get {
                // 只有在测试时, 才会都使用默认 prefab 创建
                // 正式游戏, 这里不会为 null
                // 将在初始化地图时创建用户光标
                // 如果游戏无法初始化光标, 则需要检查代码是否正确
                if (mouseCursor == null) {
                    mouseCursor = CreateMapObject(MouseCursorPrefab) as MapMouseCursor;
                }
                return mouseCursor;
            }
            private set => mouseCursor = value;
        }


        /// <summary>
        /// 运行时, MapCursor 的预制体
        /// </summary>
        public MapCursor RuntimeCursorPrefab {
            get {
                // 只有在测试时, 才会都使用默认 prefab
                // 正式游戏, 这里不会为 null
                // 将在初始化地图时会加载预制体
                // 如果游戏无法加载预制体, 则需要检查代码是否正确
                if (runtimeCursorPrefab == null) {
                    runtimeCursorPrefab = CursorPrefab;
                }
                return runtimeCursorPrefab;
            }
            private set => runtimeCursorPrefab = value;
        }
        #endregion

        #region Path Finding Field
        /// <summary>
        /// 寻路核心
        /// </summary>

        [Header("Path Finding")]
        [SerializeField]
        private FindRange findAttackRange;

        [SerializeField]
        private FindRange findMoveRange;

        [SerializeField]
        private FindRange findPathDirect;

        private CellPositionEqualityComparer cellPositionEqualityComparer = new CellPositionEqualityComparer();
        #endregion

        #region Path Finding Property
        /// <summary>
        /// 寻路核心
        /// </summary>
        public PathFinding searchPath;

        /// <summary>
        /// 寻找攻击范围
        /// </summary>
        public FindRange FindAttackRange {
            get => findAttackRange;
            set => findAttackRange = value;
        }

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        public FindRange FindMoveRange {
            get => findMoveRange;
            set => findMoveRange = value;
        }

        /// <summary>
        /// 无视移动力, 直接寻找路径
        /// </summary>
        public FindRange FindPathDirect {
            get => findPathDirect;
            set => findPathDirect = value;
        }

        /// <summary>
        /// 判断两个 Cell 的 Position 是否相等
        /// </summary>
        public CellPositionEqualityComparer GetcellPositionEqualityComparer() {
            if (cellPositionEqualityComparer == null) {
                cellPositionEqualityComparer = new CellPositionEqualityComparer();
            }
            return cellPositionEqualityComparer;
        }
        #endregion

        #region Unity Callback
        private void OnApplicationQuit() {
            if (MapObjectPool != null && MapObjectPool.gameObject != null) {
                MapObjectPool.DespawnAll();
                DestroyImmediate(MapObjectPool.gameObject);
            }
        }

        private void OnDestroy() {
            ClearCellDatas();
        }
        #endregion

        #region Init Map Method
        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <returns></returns>
        public void InitMap(bool reinit = false) {
            if (!reinit && dataDict.Count > 0) {
                return;
            }

            InitCellDatas();

            InitPathfinding();

            InitMapObjectsInMap();

            // TODO other init
        }

        private void InitCellDatas() {
            ClearCellDatas();
            CreateCellDatas();
        }

        /// <summary>
        /// 删除已有的 CellData
        /// </summary>
        public void ClearCellDatas() {
            if (dataDict.Count > 0) {
                if (MapObjectPool != null) {
                    MapObjectPool.DespawnAll();
                }
                foreach (CellData cell in dataDict.Values) {
                    cell.Dispose();
                }
                dataDict.Clear();
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
        /// <param name="cell"></param>
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
        /// 初始化寻路
        /// </summary>
        private void InitPathfinding() {
            if (searchPath == null) {
                searchPath = new PathFinding(this);
            }

            if (Application.isPlaying) {
                if (findAttackRange == null) {
                    findAttackRange = ScriptableObject.CreateInstance<FindRange>();
                }

                if (findMoveRange == null) {
                    findMoveRange = ScriptableObject.CreateInstance<FindMoveRange>();
                }

                if (findPathDirect == null) {
                    findPathDirect = ScriptableObject.CreateInstance<FindPathDirect>();
                }
            }

            if (cellPositionEqualityComparer == null) {
                cellPositionEqualityComparer = new CellPositionEqualityComparer();
            }
        }

        /// <summary>
        /// 初始化加载地图时地图对象
        /// </summary>
        private void InitMapObjectsInMap() {
            if (MapObjectPool == null) {
                Debug.LogError("MapGraph -> MapObject Pool is null.");
                return;
            }

            MapObject[] mapObjects = MapObjectPool.gameObject.GetComponentsInChildren<MapObject>();
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
                    Vector3Int cellPosition = Grid.WorldToCell(world);
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

                    // 如果是 Class
                    // 可选项(可忽略):
                    //      请将 Prefab 加入到 mapObjectPool 的 PrePrefabs 中防止动态重复读取 Prefab
                    //      为 Prefab 添加 RuntimePrePoolObject 组件:
                    //          如果此 Prefab 也用于动态读取, 一定将组件 enable 设置为 false;
                    //          将 Prefab Name 设置成对应的 Prefab 名称;
                    //          删除时会 Despawn 回池子
                    //      这些不是必须的, 一些需要这样干的情况:
                    //          绘制此 Prefab 的实例(比如某些杂兵)数量非常多(一般 > 20),
                    //          且在消灭之后会由于事件触发再次生成大量此 Prefab 的实例
                    if (mapObject.MapObjectType == MapObjectType.Class) {
                        RuntimePrePoolObject runtime = mapObject.GetComponent<RuntimePrePoolObject>();
                        if (runtime != null && !runtime.enabled) {
                            runtime.m_PoolName = MapObjectPool.poolName;
                            runtime.enabled = true;
                        }
                        MapClass cls = mapObject as MapClass;
                        cls.Load(0, RoleType.Following); // TODO Load Data
                        if (!classes.Contains(cls)) {
                            classes.Add(cls);
                        }
                    }

                    mapObject.gameObject.name += mapObject.CellPosition.ToString();
                }
            }
        }
        #endregion

        #region Map Object Method
        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(MapObject prefab) {
            if (prefab == null) {
                Debug.LogErrorFormat("MapGraph -> CreateMapObject Error! Prefab is null.");
                return null;
            }

            MapObjectType type = prefab.MapObjectType;

            // 用户光标在整个地图中只能有且只有一个
            if (type == MapObjectType.MouseCursor && mouseCursor != null) {
                ObjectPool.DespawnUnsafe(mouseCursor.gameObject, true);
                mouseCursor = null;
            }

            // 实例化 map object
            GameObject instance;
            if (type == MapObjectType.Cursor || type == MapObjectType.MouseCursor) {
                instance = MapCursorPool.Spawn(prefab.gameObject);
            } else {
                instance = MapObjectPool.Spawn(prefab.gameObject);
            }

            MapObject mapObject = instance.GetComponent<MapObject>();

            mapObject.InitMapObject(this);

            if (type == MapObjectType.MouseCursor) {
                mouseCursor = mapObject as MapMouseCursor;
            }

            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(string prefabName) {
            MapObject prefab = LoadMapObjectPrefab(prefabName);
            return CreateMapObject(prefab);
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
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
        /// <param name="cellPosition"></param>
        /// <returns></returns>
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
        /// <param name="prefabName"></param>
        /// <returns></returns>
        private MapObject LoadMapObjectPrefab(string prefabName) {
            if (string.IsNullOrEmpty(prefabName)) {
                Debug.LogError("MapGraph -> LoadMapObjectPrefab Error! Prefab name is null or empty.");
                return null;
            }

            // TODO ResourcesManager 读取 prefab
            MapObject prefab = Resources.Load<MapObject>(prefabName);
            return prefab;
        }

        /// <summary>
        /// 显示 cursor
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="type"></param>
        public void ShowRangeCursors(IEnumerable<CellData> cells, MapCursor.MapCursorType type) {
            if (type == MapCursor.MapCursorType.Mouse) {
                return;
            }

            foreach (CellData cell in cells) {
                MapCursor cursor = CreateMapObject(RuntimeCursorPrefab, cell.Position) as MapCursor;
                if (cursor != null) {
                    //cursor.name = string.Format(
                    //    "{0} Cursor {1}",
                    //    type.ToString(),
                    //    cell.position.ToString());
                    cursor.CursorType = type;
                    if (type == MapCursor.MapCursorType.Move) {
                        //m_MapMoveCursors.Add(cursor);
                        cell.HasMoveCursor = true;
                    } else {
                        cell.HasAttackCursor |= type == MapCursor.MapCursorType.Attack;
                    }

                    cursors.Add(cursor);
                }
            }
        }

        /// <summary>
        /// 隐藏cursor
        /// </summary>
        public void HideRangeCursors() {
            //if (m_MapMoveCursors.Count > 0)
            //{
            //    for (int i = 0; i < m_MapMoveCursors.Count; i++)
            //    {
            //        ObjectPool.DespawnUnsafe(m_MapMoveCursors[i].gameObject, true);
            //    }
            //    m_MapMoveCursors.Clear();
            //}

            //if (m_MapAttackCursors.Count > 0)
            //{
            //    for (int i = 0; i < m_MapAttackCursors.Count; i++)
            //    {
            //        ObjectPool.DespawnUnsafe(m_MapAttackCursors[i].gameObject, true);
            //    }
            //    m_MapAttackCursors.Clear();
            //}

            if (cursors.Count > 0) {
                foreach (MapCursor cursor in cursors) {
                    //CellData cellData = GetCellData(cursor.cellPosition);
                    //if (cellData != null)
                    //{
                    //    cellData.hasCursor = false;
                    //}
                    ObjectPool.DespawnUnsafe(cursor.gameObject, true);
                }
                cursors.Clear();
            }
        }
        #endregion

        #region Path Finding Method
        /// <summary>
        /// 搜寻移动范围
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="movePoint"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public List<CellData> SearchMoveRange(CellData cell, float movePoint, MoveConsumption consumption) {
            if (FindMoveRange == null) {
                Debug.LogError("Error: Find move range is null.");
                return null;
            }

            if (!searchPath.SearchMoveRange(FindMoveRange, cell, movePoint, consumption)) {
                Debug.LogErrorFormat("Error: Move Range({0}) is Not Found.", 5f);
                return null;
            }

            return searchPath.result;
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <returns></returns>
        public List<CellData> SearchAttackRange(CellData cell, int minRange, int maxRange, bool useEndCell = false) {
            if (FindAttackRange == null) {
                Debug.LogError("Error: Find attack range is null.");
                return null;
            }

            if (!searchPath.SearchAttackRange(FindAttackRange, cell, minRange, maxRange, useEndCell)) {
                Debug.LogErrorFormat("Error: Attack Range({0} - {1}) is Not Found.", 2, 3);
                return null;
            }

            return searchPath.result;
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public List<CellData> SearchPath(CellData startCell, CellData endCell, MoveConsumption consumption) {
            if (FindPathDirect == null) {
                Debug.LogError("Error: Find path is null.");
                return null;
            }

            if (!searchPath.SearchPath(FindPathDirect, startCell, endCell, consumption)) {
                Debug.LogError("Error: Search Path Error. Maybe some cells are out of range.");
                return null;
            }

            return searchPath.result;
        }

        /// <summary>
        /// 搜寻移动范围与攻击范围
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="nAtk">是否包含攻击范围</param>
        /// <param name="moveCells"></param>
        /// <param name="atkCells"></param>
        /// <returns></returns>
        public bool SearchMoveRange(
            MapClass cls,
            bool nAtk,
            out IEnumerable<CellData> moveCells,
            out IEnumerable<CellData> atkCells) {
            moveCells = null;
            atkCells = null;

            if (cls == null) {
                Debug.LogError("MapGraph -> SearchMoveRange: `cls` is null.");
                return false;
            }

            CellData cell = GetCellData(cls.CellPosition);
            if (cell == null) {
                Debug.LogError("MapGraph -> SearchMoveRange: `cls.cellPosition` is out of range.");
                return false;
            }

            // 搜索移动范围, 从 MapClass 中读取数据
            Role role = cls.Role;
            if (role == null) {
                Debug.LogErrorFormat("MapGraph -> SearchMoveRange: `cls.role` is null. Pos: {0}", cell.Position.ToString());
                return false;
            }

            float movePoint = role.MovePoint;
            MoveConsumption consumption = role.Cls.MoveConsumption;

            List<CellData> rangeCells = SearchMoveRange(cell, movePoint, consumption);
            if (rangeCells == null) {
                return false;
            }

            HashSet<CellData> moveRangeCells = new HashSet<CellData>(rangeCells, GetcellPositionEqualityComparer());
            moveCells = moveRangeCells;

            if (nAtk && role.EquipedWeapon != null /* 是否有武器 */) {
                // 搜索攻击范围, 从 MapClass 中读取数据
                Vector2Int atkRange = new Vector2Int(
                    role.EquipedWeapon.UniqueInfo.minRange,
                    role.EquipedWeapon.UniqueInfo.maxRange);

                HashSet<CellData> atkRangeCells = new HashSet<CellData>(GetcellPositionEqualityComparer());
                foreach (CellData moveCell in moveRangeCells) {
                    rangeCells = SearchAttackRange(moveCell, atkRange.x, atkRange.y, true);

                    if (rangeCells == null) {
                        return false;
                    }

                    if (rangeCells.Count > 0) {
                        atkRangeCells.UnionWith(rangeCells.Where(c => !moveRangeCells.Contains(c)));
                    }
                }

                atkCells = atkRangeCells;
            }

            return true;
        }

        /// <summary>
        /// 搜寻和显示范围
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="nAtk">包含攻击范围</param>
        /// <returns></returns>
        public bool SearchAndShowMoveRange(MapClass cls, bool nAtk) {
            if (!SearchMoveRange(cls, nAtk, out IEnumerable<CellData> moveCells, out IEnumerable<CellData> atkCells)) {
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
        #endregion

        #region Helper Method
        /// <summary>
        /// 地图是否包含 Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position) {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }
        public bool Contains(int x, int y) {
            return mapRect.Contains(new Vector2Int(x, y));
        }

        /// <summary>
        /// 获取 Cell 的位置
        /// </summary>
        /// <param name="cellPosition">网格坐标</param>
        /// <param name="center">是否是中心位置</param>
        /// <param name="world">是否是世界坐标</param>
        /// <returns></returns>
        public Vector3 GetCellPosition(Vector3Int cellPosition, bool world = true, bool center = false) {
            Vector3 pos;

            if (world) {
                pos = Grid.GetCellCenterWorld(cellPosition);
            } else {
                pos = Grid.GetCellCenterLocal(cellPosition);
            }

            if (!center) {
                pos.y -= HalfCellSize.y;
            }

            return pos;
        }

        /// <summary>
        /// 获取 Terrain 层的 Tile
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SRPGTile GetTile(Vector3Int position) {
            return TerrainTilemap.GetTile<SRPGTile>(position);
        }

        /// <summary>
        /// 改变地形
        /// </summary>
        /// <param name="position"></param>
        /// <param name="newTile"></param>
        public void ChangeTile(Vector3Int position, SRPGTile newTile) {
            SRPGTile old = GetTile(position);
            if (old == newTile) {
                return;
            }

            TerrainTilemap.SetTile(position, newTile);
            TerrainTilemap.RefreshTile(position);

            if (Contains(position)) {
                dataDict[position].HasTile = newTile != null;
            }
        }

        /// <summary>
        /// 获取 CellData
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CellData GetCellData(Vector3Int position) {
            if (!Contains(position)) {
                return null;
            }
            return dataDict[position];
        }
        #endregion

        #region class CellPositionEqualityComparer
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
