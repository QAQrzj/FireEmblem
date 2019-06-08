using UnityEngine;

namespace CombatManagement {
    using Models;

    [CreateAssetMenu(fileName = "CombatPrepareAction.asset", menuName = "SRPG/Combat Prepare Action")]
    public class PrepareAction : BattleAction {
        public override BattleActionType ActionType => BattleActionType.Prepare;

        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal) {
            CombatUnit atker = combat.GetCombatUnit(0);
            CombatUnit defer = combat.GetCombatUnit(1);

            // 防守者是否可反击
            bool canDeferAtk = false;
            if (defer.Role.EquipedWeapon != null) {
                Vector3Int offset = defer.MapClass.CellPosition - atker.MapClass.CellPosition;
                int dist = Mathf.Abs(offset.x) + Mathf.Abs(offset.y);
                WeaponUniqueInfo defInfo = defer.Role.EquipedWeapon.UniqueInfo;

                //if (defInfo.weaponType != WeaponType.Staff)
                {
                    // 如果在反击范围内
                    canDeferAtk |= dist >= defInfo.minRange && dist <= defInfo.maxRange;
                }
            }

            // 根据速度初始化攻击者与防守者
            if (canDeferAtk) {
                if (atker.Speed < defer.Speed) {
                    CombatUnit tmp = atker;
                    atker = defer;
                    defer = tmp;
                }
            }

            // 更新信息
            Message = "战斗开始";

            atkVal = new CombatVariable(atker.Position, atker.Hp, true, atker.Durability, CombatAnimaType.Prepare);
            defVal = new CombatVariable(defer.Position, defer.Hp, canDeferAtk, defer.Durability, CombatAnimaType.Prepare);

            // 准备阶段
            CombatStep firstStep = new CombatStep(atkVal, defVal);
            return firstStep;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal) {
            return false;
        }
    }
}
