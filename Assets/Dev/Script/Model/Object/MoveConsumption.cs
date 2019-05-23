using UnityEngine;
using Utility;

namespace Models {
    using Maps;

    public class MoveConsumption {
        private readonly MoveConsumptionInfo moveConsumptionInfo;

        public float this[TerrainType terrainType] {
            get {
                if (terrainType == TerrainType.Length) {
                    Debug.LogError("MoveConsumption -> TerrainType can not be Length.");
                    return 0;
                }
                return moveConsumptionInfo.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(ClassType classType) {
            // TODO Load from config file
            moveConsumptionInfo = new MoveConsumptionInfo {
                type = classType,
                consumptions = new float[TerrainType.Length.ToInteger()]
            };
            for (int i = 0; i < moveConsumptionInfo.consumptions.Length; i++) {
                moveConsumptionInfo.consumptions[i] = UnityEngine.Random.Range(0.5f, 3f);
            }
        }

        // TODO Other
    }
}
