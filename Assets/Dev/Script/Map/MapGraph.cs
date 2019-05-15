using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maps {
    public class MapGraph : MonoBehaviour {
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
        /// 地图是否包含 Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position) {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }

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
    }
}
