using Models;
using UnityEngine;

namespace CombatManagement {
    [CreateAssetMenu(fileName = "CombatAttackAction.asset", menuName = "SRPG/Combat Attack Action")]
    public class AttackAction : BattleAction {
        public sealed override BattleActionType ActionType => BattleActionType.Attack;

        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal) {
            CombatUnit atker = combat.GetCombatUnit(atkVal.position);
            CombatUnit defer = combat.GetCombatUnit(defVal.position);

            atkVal.animaType = CombatAnimaType.Attack;

            // 真实命中率 = 攻击者命中 - 防守者回避
            int realHit = atker.Hit - defer.Avoidance;

            // 概率是否击中
            int hitRate = Random.Range(0, 100);
            bool isHit = hitRate <= realHit;
            if (isHit) {
                bool crit = false; // TODO 是否暴击
                int realAtk = atker.Atk;

                ///////////////////
                // TODO 触发伤害技能
                // 这里写触发技能后伤害变化(比如武器特效等)
                // 或者触发某些状态(比如中毒等)
                //////////////////

                if (crit) {
                    realAtk *= 2; // 假定暴击造成双倍伤害
                }

                // 掉血 = 攻击者攻击力 - 防守者防御力
                // 最少掉一滴血
                int damageHp = Mathf.Max(1, realAtk - defer.Def);
                if (damageHp > defVal.hp) {
                    damageHp = defVal.hp;
                }
                defVal.hp -= damageHp;

                atkVal.crit = crit;
                defVal.animaType = CombatAnimaType.Damage;

                // 更新此次攻击信息
                this.Message = string.Format(
                    "{0} 对 {1} 的攻击造成了 {2} 点伤害{3}。",
                    atker.Role.Character.Info.name,
                    defer.Role.Character.Info.name,
                    damageHp,
                    crit ? "(暴击)" : string.Empty);

                if (defVal.IsDead) {
                    Message += string.Format(" {0}被击败了。", defer.Role.Character.Info.name);
                }
            } else {
                defVal.animaType = CombatAnimaType.Evade;

                // 更新此次躲闪信息
                Message = string.Format(
                    "{1} 躲闪了 {0} 的攻击。",
                    atker.Role.Character.Info.name,
                    defer.Role.Character.Info.name);
            }

            // 只有玩家才会减低耐久度
            if (atker.Role.AttitudeTowards == AttitudeTowards.Player) {
                // 攻击者武器耐久度 -1
                atkVal.durability = Mathf.Max(0, atkVal.durability - 1);
            }

            // 攻击者行动过了
            atkVal.action = true;

            CombatStep step = new CombatStep(atkVal, defVal);
            return step;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal) {
            // 防守者死亡
            if (defVal.IsDead) {
                return true;
            }

            // 如果防守者行动过了
            if (defVal.action) {
                //CombatUnit atker = GetCombatUnit(atkVal.position);
                //CombatUnit defer = GetCombatUnit(defVal.position);

                // TODO 是否继续攻击, 必要时需要在 CombatVariable 加入其它控制变量
                // 比如, 触发过技能或物品了
                // atker.role.skill/item 包含继续战斗的技能或物品
                // defer.role.skill/item 包含继续战斗的技能或物品
                //if (触发继续战斗) {
                //    return false;
                //}

                return true;
            }

            return false;
        }
    }
}
