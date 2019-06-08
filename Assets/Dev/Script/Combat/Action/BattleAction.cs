using UnityEngine;

namespace CombatManagement {
    public enum BattleActionType {
        Unknow,
        Prepare,
        Attack,
        MageAttack,
        Heal,

        // 其它自定义类型
    }

    public abstract class BattleAction : ScriptableObject {
        public string Message { get; protected set; } = "Unknow battle message.";

        public abstract BattleActionType ActionType { get; }

        public abstract CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public abstract bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public sealed override string ToString() {
            return Message;
        }
    }
}
