using UnityEngine;

namespace CombatManagement {
    /// <summary>
    /// 战斗每一步结果
    /// </summary>
    public class CombatStep {
        // 如果你要在 Inspector 面板显示每次战斗结果, 在这里更改
        // 将属性改成字段, 加上 [SerializeField]
        // 同时更改 Combat 类 List<CombatStep> 也需要这样改
        // 在此类和 CombatVariable 上加上 [Serializable]
        // 也可以自定义 UnityEditor
        // 例如:
        //[SerializeField]
        //private CombatVariable atkVal;
        //public CombatVariable AtkVal {
        //    get => atkVal;
        //    private set => atkVal = value;
        //}

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
