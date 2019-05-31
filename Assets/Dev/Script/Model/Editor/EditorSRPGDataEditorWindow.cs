using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DR.Book.SRPG_Dev.Framework;
using UnityEditor;
using UnityEngine;

namespace Models {
    public class EditorSRPGDataEditorWindow : EditorWindow {
        private static EditorSRPGDataEditorWindow window;
        public static EditorSRPGDataEditorWindow OpenEditorSrpgDataEditorWindow() {
            if (window != null) {
                window.Focus();
                return window;
            }
            window = GetWindow<EditorSRPGDataEditorWindow>(false, "SRPG Data");
            window.minSize = new Vector2(480, 480);
            window.Show();
            return window;
        }

        private EditorSRPGData srpgData;
        private SerializedObject serializedObject;

        public EditorSRPGData SRPGData {
            get { return srpgData; }
            set {
                if (srpgData == value) {
                    return;
                }
                srpgData = value;

                // 删除以前的
                if (serializedObject != null) {
                    serializedObject.Dispose();
                    serializedObject = null;
                }

                // 重新建立
                if (srpgData != null) {
                    serializedObject = new SerializedObject(srpgData);
                }
            }
        }

        private void OnDestroy() {
            SRPGData = null;
            window = null;
        }

        private Vector2Int selectedRange;
        private Vector2 scroll;

        private GUILayoutOption btnWidth = GUILayout.MaxWidth(120);

        /// <summary>
        /// 绘制按钮
        /// </summary>
        private bool DoDrawButtons() {
            IEditorConfigSerializer config = srpgData.GetCurConfig();
            if (config == null) {
                EditorGUILayout.HelpBox(string.Format("{0} Config is not found.", srpgData.currentConfig.ToString()), MessageType.Error);
                return false;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save To File", btnWidth)) {
                    SaveToFile(config);
                }

                if (GUILayout.Button("Load From File", btnWidth)) {
                    LoadFromFile(config);
                }

                if (GUILayout.Button("Check Keys", btnWidth)) {
                    CheckDuplicateKeys(config);
                }

                if (GUILayout.Button("Sort Datas", btnWidth)) {
                    SortWithKeys(config);
                }
            }
            EditorGUILayout.EndHorizontal();

            return true;
        }

        /// <summary>
        /// 绘制具体信息
        /// </summary>
        private bool DoDrawDatas() {
            SerializedProperty curConfigProperty = GetConfigProperty();
            if (curConfigProperty == null) {
                EditorGUILayout.HelpBox(string.Format("{0} Config Property is not found.", srpgData.currentConfig.ToString()), MessageType.Error);
                return false;
            }

            SerializedProperty curArrayDatasProperty = curConfigProperty.FindPropertyRelative("datas");

            EditorGUI.BeginChangeCheck();

            // 设置数量
            int arraySize = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", curArrayDatasProperty.arraySize));
            curArrayDatasProperty.arraySize = arraySize;

            if (arraySize != 0) {
                // 最少显示 20 个
                selectedRange = EditorGUILayout.Vector2IntField("Index Range", selectedRange);
                selectedRange.x = Mathf.Max(0, Mathf.Min(selectedRange.x, arraySize - 20));
                selectedRange.y = Mathf.Min(arraySize - 1, Mathf.Max(selectedRange.x + 19, selectedRange.y));
                Vector2 range = selectedRange;
                EditorGUILayout.MinMaxSlider(ref range.x, ref range.y, 0, arraySize - 1);
                selectedRange.x = (int)range.x;
                selectedRange.y = (int)range.y;

                // 绘制数据
                scroll = EditorGUILayout.BeginScrollView(scroll, "box");
                {
                    for (int i = selectedRange.x; i <= selectedRange.y; i++) {
                        SerializedProperty property = curArrayDatasProperty.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(property, true);
                    }
                }
                EditorGUILayout.EndScrollView();
            }

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(srpgData);
            }

            return true;
        }

        private void SaveToFile(IEditorConfigSerializer config) {
            string ext = (config is XmlConfigFile) ? "xml" : "txt";
            string path = EditorUtility.SaveFilePanel("Save", Application.streamingAssetsPath, config.GetType().Name, ext);

            if (!string.IsNullOrEmpty(path)) {
                if (!CheckDuplicateKeys(config)) {
                    Debug.LogError("Config to save has some `Duplicate Keys`. Save Failure.");
                    return;
                }

                try {
                    byte[] bytes = config.EditorSerializeToBytes();
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                } catch (Exception e) {
                    Debug.LogError("Save Error: " + e);
                    return;
                }
            }
        }

        private void LoadFromFile(IEditorConfigSerializer config) {
            string ext = (config is XmlConfigFile) ? "xml" : "txt";
            string path = EditorUtility.OpenFilePanel("Load", Application.streamingAssetsPath, ext);

            if (!string.IsNullOrEmpty(path)) {
                try {
                    byte[] bytes = File.ReadAllBytes(path);
                    config.EditorDeserializeToObject(bytes);
                    EditorUtility.SetDirty(srpgData);
                    Repaint();
                } catch (Exception e) {
                    Debug.LogError("Load Error: " + e);
                    return;
                }

                if (!CheckDuplicateKeys(config)) {
                    Debug.LogError("Loaded File has some `Duplicate Keys`.");
                    return;
                }
            }
        }

        /// <summary>
        /// 检查重复的 Key
        /// </summary>
        /// <returns></returns>
        private bool CheckDuplicateKeys(IEditorConfigSerializer config) {
            // 获取所有 key
            Array keys = config.EditorGetKeys();

            // key: index
            Dictionary<object, int> keySet = new Dictionary<object, int>();

            // dumplicate [key: indexes]
            Dictionary<object, HashSet<string>> duplicateKeys = new Dictionary<object, HashSet<string>>();

            for (int i = 0; i < keys.Length; i++) {
                object key = keys.GetValue(i);

                // 如果 key 重复了
                if (keySet.ContainsKey(key)) {
                    // 如果重复 key 的 set 没有建立
                    if (!duplicateKeys.ContainsKey(key)) {
                        // 建立 set, 并加入最初的下标
                        duplicateKeys[key] = new HashSet<string>{
                            keySet[key].ToString()
                        };
                    }

                    // 加入当前下标
                    duplicateKeys[key].Add(i.ToString());
                } else {
                    keySet.Add(key, i);
                }
            }

            if (duplicateKeys.Count != 0) {
                // 打印所有重复的 keys
                foreach (var kvp in duplicateKeys) {
                    Debug.LogErrorFormat("Duplicate Keys \"{0}\": Index [{1}]", kvp.Key.ToString(), string.Join(", ", kvp.Value.ToArray()));
                }
                return false;
            }

            return true;
        }

        private void SortWithKeys(IEditorConfigSerializer config) {
            config.EditorSortDatas();
        }

        private void OnGUI() {
            EditorGUI.BeginDisabledGroup(true);
            srpgData = (EditorSRPGData)EditorGUILayout.ObjectField("SRPG Data Editor", srpgData, typeof(EditorSRPGData), false);
            EditorGUI.EndDisabledGroup();
            if (srpgData == null || serializedObject == null) {
                EditorGUILayout.HelpBox("Please re-open a SRPG Data Editor Window.", MessageType.Info);
                return;
            }

            serializedObject.Update();

            // 绘制选择类型
            SerializedProperty curConfigTypeProperty = serializedObject.FindProperty("currentConfig");
            EditorGUILayout.PropertyField(curConfigTypeProperty, true);
            EditorGUILayout.Space();

            // 绘制按钮
            if (!DoDrawButtons()) {
                return;
            }

            // 绘制数据
            if (!DoDrawDatas()) {
                return;
            }
        }

        /// <summary>
        /// 获取当前 config
        /// </summary>
        /// <returns></returns>
        private SerializedProperty GetConfigProperty() {
            switch (srpgData.currentConfig) {
                case EditorSRPGData.ConfigType.MoveConsumption:
                    return serializedObject.FindProperty("moveConsumptionConfig");
                case EditorSRPGData.ConfigType.Class:
                    return serializedObject.FindProperty("classConfig");
                case EditorSRPGData.ConfigType.Character:
                    return serializedObject.FindProperty("characterInfoConfig");
                case EditorSRPGData.ConfigType.Item:
                    return serializedObject.FindProperty("itemInfoConfig");
                case EditorSRPGData.ConfigType.Text:
                    return serializedObject.FindProperty("textInfoConfig");
                default:
                    return null;
            }
        }
    }
}
