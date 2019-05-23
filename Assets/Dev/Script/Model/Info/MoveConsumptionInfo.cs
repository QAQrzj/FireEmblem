using System;

namespace Models {
    [Serializable]
    public class MoveConsumptionInfo {
        /// <summary>
        ///  职业类型
        /// </summary>
        public ClassType type;

        /// <summary>
        /// 移动消耗具体数值
        /// </summary>
        public float[] consumptions;
    }
}
