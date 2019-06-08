using UnityEditor;
using UnityEngine;

namespace Models {
    [CustomPropertyDrawer(typeof(MoveConsumptionInfoConfig))]
    public class MoveConsumptionInfoConfigPropertyDrawer : PropertyDrawer {
        private const float padding = 2f;
        private const float tabWidth = 16f;
        private const int arraySize = (int)ClassType.Length;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // 标题高度
            float height = EditorGUIUtility.singleLineHeight + padding;

            if (property.isExpanded) {
                SerializedProperty datas = property.FindPropertyRelative("datas");

                // datas 每一个属性高度
                for (int i = 0; i < datas.arraySize; i++) {
                    SerializedProperty data = datas.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(data, true) + padding;
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // 设置初始位置与高度
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            // 渲染标题 Foldout
            bool expanded = EditorGUI.PropertyField(rect, property, label);
            rect.y += EditorGUIUtility.singleLineHeight + padding;

            // 设置长度与 ClassType 一样
            SerializedProperty datas = property.FindPropertyRelative("datas");
            datas.arraySize = arraySize;

            if (expanded) {
                // 设置偏移
                rect.x += tabWidth;
                rect.width -= tabWidth;

                for (int i = 0; i < datas.arraySize; i++) {
                    SerializedProperty data = datas.GetArrayElementAtIndex(i);

                    // 强制保持数组顺序为 ClassType 的 Enum 顺序
                    SerializedProperty classType = data.FindPropertyRelative("classType");
                    classType.enumValueIndex = i;

                    // 渲染每一个 MoveConsumpotionInfo
                    rect.height = EditorGUI.GetPropertyHeight(data, true);
                    EditorGUI.PropertyField(rect, data, true);
                    rect.y += rect.height + padding;
                }
            }
        }
    }
}
