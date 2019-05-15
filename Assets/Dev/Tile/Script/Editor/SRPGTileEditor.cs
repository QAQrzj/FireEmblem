using UnityEditor;

namespace Maps {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SRPGTile))]
    public class SRPGTileEditor : RuleTileEditor {
        public SRPGTile SRPGTile {
            get { return target as SRPGTile; }
        }

        public override void OnInspectorGUI() {
            // 渲染新增的数据
            EditorGUI.BeginChangeCheck();
            SRPGTile.terrainType = (TerrainType)EditorGUILayout.EnumPopup("地形类型", SRPGTile.terrainType);
            SRPGTile.defense = EditorGUILayout.IntSlider("守备", SRPGTile.defense, -100, 100);
            SRPGTile.avoidRate = EditorGUILayout.IntSlider("回避", SRPGTile.avoidRate, -100, 100);
            SRPGTile.treatment = EditorGUILayout.IntSlider("恢复", SRPGTile.treatment, -100, 100);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }
            // 渲染 RuleTile 的内容
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}
