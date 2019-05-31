using UnityEditor;
using UnityEngine;

namespace Models {
    using Dev.Editor;
    using Maps;

    [CustomPropertyDrawer(typeof(MoveConsumptionInfo))]
    public class MoveConsumptionInfoPropertyDrawer : PropertyDrawer {
        private const int arraySize = (int)TerrainType.Length;
        private static readonly GUIContent classTypeContent = new GUIContent("Class Type");

        private const float padding = 2f;
        private const float tabWidth = 16f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // 标题高度
            float height = EditorGUIUtility.singleLineHeight + padding;

            // 如果有展开，加上 ClassType 高度 + 单个消耗高度 * 消耗长度(arraySize)
            if (property.isExpanded) {
                height += (arraySize + 1) * (EditorGUIUtility.singleLineHeight + padding);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // 设置初始位置与高度
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty classType = property.FindPropertyRelative("classType");

            // 渲染标题
            string tmpTitle = label.text;
            label.text = string.Format("{0} {1}", tmpTitle, EnumGUIContents.classTypeContents[classType.enumValueIndex].text);
            bool expanded = EditorGUI.PropertyField(rect, property, label);
            label.text = tmpTitle;
            rect.y += EditorGUIUtility.singleLineHeight + padding;

            if (expanded) {
                // 设置偏移
                rect.x += tabWidth;
                rect.width -= tabWidth;

                // 渲染 ClassType
                EditorGUI.PropertyField(rect, classType, classTypeContent);
                rect.y += EditorGUIUtility.singleLineHeight + padding;

                // 强制保持长度与 TerrainType 一样
                SerializedProperty consumptions = property.FindPropertyRelative("consumptions");
                consumptions.arraySize = arraySize;

                // 渲染每一个消耗
                for (int i = 0; i < consumptions.arraySize; i++) {
                    SerializedProperty consumption = consumptions.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, consumption, EnumGUIContents.terrainTypeContents[i], true);
                    rect.y += EditorGUIUtility.singleLineHeight + padding;
                }
            }
        }
    }
}
