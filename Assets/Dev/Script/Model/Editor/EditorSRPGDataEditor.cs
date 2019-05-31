using UnityEngine;
using UnityEditor;

namespace Models {
    [CustomEditor(typeof(EditorSRPGData))]
    public class EditorSRPGDataEditor : Editor {
        #region Property
        public EditorSRPGData SRPGData {
            get { return target as EditorSRPGData; }
        }
        #endregion

        #region Unity Callback
        public override void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Edit Datas")) {
                EditorSRPGDataEditorWindow window = EditorSRPGDataEditorWindow.OpenEditorSrpgDataEditorWindow();
                window.SRPGData = SRPGData;
            }
        }
        #endregion
    }
}
