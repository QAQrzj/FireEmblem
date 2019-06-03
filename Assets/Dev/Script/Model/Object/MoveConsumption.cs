using UnityEngine;
using Utility;

namespace Models {
    using Maps;

    public class MoveConsumption {
        private MoveConsumptionInfo info;

        public ClassType ClassType {
            get { return info.classType; }
        }

        public float this[TerrainType terrainType] {
            get {
                if (terrainType == TerrainType.Length) {
                    Debug.LogError("MoveConsumption -> TerrainType can not be Length.");
                    return 255f;
                }
                return info.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(MoveConsumptionInfo info) {
            this.info = info;
        }
    }
}
