#if UNITY_EDITOR
using UnityEngine;

namespace Models {
    [CreateAssetMenu(fileName = "EditorSRPGData.asset", menuName = "SRPG/Editor SRPG Data")]
    public class EditorSRPGData : ScriptableObject {
        public enum ConfigType {
            MoveConsumption,
            Class,
            Character,
            Item,
            Text
        }

        [SerializeField]
        public ConfigType currentConfig = ConfigType.MoveConsumption;

        [SerializeField]
        public MoveConsumptionInfoConfig moveConsumptionConfig;

        [SerializeField]
        public ClassInfoConfig classConfig;

        [SerializeField]
        public CharacterInfoConfig characterInfoConfig;

        [SerializeField]
        public ItemInfoConfig itemInfoConfig;

        [SerializeField]
        public TextInfoConfig textInfoConfig;

        public IEditorConfigSerializer GetCurConfig() {
            switch (currentConfig) {
                case ConfigType.MoveConsumption:
                    return moveConsumptionConfig;
                case ConfigType.Class:
                    return classConfig;
                case ConfigType.Character:
                    return characterInfoConfig;
                case ConfigType.Item:
                    return itemInfoConfig;
                case ConfigType.Text:
                    return textInfoConfig;
                default:
                    return null;
            }
        }
    }
}
#endif
