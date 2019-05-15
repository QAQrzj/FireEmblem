using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Maps {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapGraph), true)]
    public class MapGraphEditor : Editor {
        [MenuItem("GameObject/SRPG/Map Graph", priority = -1)]
        public static MapGraph CreateMapGraphGameObject() {
            GameObject mapGraph = new GameObject("MapGraph", typeof(Grid));
            GameObject tilemap = new GameObject("Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
            tilemap.transform.SetParent(mapGraph.transform, false);
            Selection.activeObject = mapGraph;
            return mapGraph.AddComponent<MapGraph>();
        }

        public MapGraph Map {
            get { return target as MapGraph; }
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            // 检测地图长宽是否正确, 如果不正确就修正
            if (Map.mapRect.width < 2 || Map.mapRect.height < 2) {
                RectInt fix = Map.mapRect;
                fix.width = Mathf.Max(Map.mapRect.width, 2);
                fix.height = Mathf.Max(Map.mapRect.height, 2);
                Map.mapRect = fix;
            }
        }

        protected virtual void OnSceneGUI() {
            if (!Map.editorDrawGizmos) {
                return;
            }

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = Map.editorCellColor;

            // Scene 面板左上角显示信息
            Handles.BeginGUI();
            Rect areaRect = new Rect(50, 50, 200, 200);
            GUILayout.BeginArea(areaRect);
            DrawHorizontalLabel("Object Name:", Map.gameObject.name, textStyle);
            DrawHorizontalLabel("Map Name:", Map.mapName, textStyle);
            DrawHorizontalLabel("Map Size:", Map.Width + "x" + Map.Height, textStyle);
            DrawHorizontalLabel("Cell Size:", Map.Grid.cellSize.x + "x" + Map.Grid.cellSize.y, textStyle);
            GUILayout.EndArea();
            Handles.EndGUI();

            // 立即刷新 Scene 面板
            UpdateSceneGUI();
        }

        /// <summary>
        /// 立即刷新 Scene 面板, 这保证了每帧都运行(包括 Gizmos)
        /// 如果在 OnSceneGUI 或 Gizmos 里获取鼠标, 需要每帧都运行
        /// </summary>
        protected void UpdateSceneGUI() {
            HandleUtility.Repaint();
        }

        protected void DrawHorizontalLabel(string name, string value, GUIStyle style = null, int nameMaxWidth = 80, int valueMaxWidth = 120) {
            EditorGUILayout.BeginHorizontal();
            if (style == null) {
                EditorGUILayout.LabelField(name, GUILayout.MaxWidth(nameMaxWidth));
                EditorGUILayout.LabelField(value, GUILayout.MaxWidth(valueMaxWidth));
            } else {
                EditorGUILayout.LabelField(name, style, GUILayout.MaxWidth(nameMaxWidth));
                EditorGUILayout.LabelField(value, style, GUILayout.MaxWidth(valueMaxWidth));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
