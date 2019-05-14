using UnityEditor;

namespace Maps
{
    [CustomEditor(typeof(SRPGTile))]
    [CanEditMultipleObjects]
    public class SRPGTileEditor : RuleTileEditor
    {
        public SRPGTile SRPGTile
        {
            get
            {
                return target as SRPGTile;
            }
        }

        public override void OnInspectorGUI()
        {
            // 渲染新增的数据
            EditorGUI.BeginChangeCheck();
            SRPGTile.TerrainType = (TerrainType)EditorGUILayout.EnumPopup("地形", SRPGTile.TerrainType);
            SRPGTile.Defense = EditorGUILayout.IntSlider("守备", SRPGTile.Defense, -100, 100);
            SRPGTile.AvoidRate = EditorGUILayout.IntSlider("回避", SRPGTile.AvoidRate, -100, 100);
            SRPGTile.Treatment = EditorGUILayout.IntSlider("恢复", SRPGTile.Treatment, -100, 100);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            // 渲染 RuleTile 的内容
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}
