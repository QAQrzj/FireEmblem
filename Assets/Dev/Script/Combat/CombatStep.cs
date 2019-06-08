using UnityEngine;

namespace CombatManagement {
    /// <summary>
    /// 战斗每一步结果
    /// </summary>
    public class CombatStep {
        /// <summary>
        /// 当前进攻方
        /// </summary>
        public CombatVariable AtkVal { get; private set; }

        /// <summary>
        /// 当前防守方
        /// </summary>
        public CombatVariable DefVal { get; private set; }

        public CombatStep(CombatVariable atker, CombatVariable defer) {
            AtkVal = atker;
            DefVal = defer;
        }

        /// <summary>
        /// 根据位置获取战斗变量
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CombatVariable GetCombatVariable(int position) {
            if (AtkVal.position == position) {
                return AtkVal;
            }

            if (DefVal.position == position) {
                return DefVal;
            }

            Debug.LogError("CombatStep -> position is out of range.");
            return default;
        }
    }
}
