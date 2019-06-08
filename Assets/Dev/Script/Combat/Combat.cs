using System.Collections.Generic;
using UnityEngine;

namespace CombatManagement {
    using Maps;
    using Weapon;

    [DisallowMultipleComponent]
    [AddComponentMenu("SRPG/Combat System/Combat")]
    public class Combat : MonoBehaviour {
        #region Static
        public static CombatAnimaController GetOrAdd(GameObject gameObject) {
            if (gameObject == null) {
                Debug.Log("Combat -> argument named `gameObject` is null.");
                return null;
            }

            CombatAnimaController combat = gameObject.GetComponent<CombatAnimaController>();
            if (combat == null) {
                combat = gameObject.AddComponent<CombatAnimaController>();
                if (combat.Combat.battleActionDict.Count == 0) {
                    combat.Combat.battleActions = new BattleAction[] {
                        ScriptableObject.CreateInstance<PrepareAction>(),
                        ScriptableObject.CreateInstance<AttackAction>()
                    };
                    combat.Combat.InitBattleActions();
                }
            }

            return combat;
        }

        public static CombatAnimaController GetOrAdd(GameObject gameObject, MapClass role0, MapClass role1) {
            CombatAnimaController combat = GetOrAdd(gameObject);
            return combat == null || !combat.LoadCombatUnit(role0, role1) ? null : combat;
        }
        #endregion

        #region Field
        public BattleAction[] battleActions;
        private Dictionary<BattleActionType, BattleAction> battleActionDict = new Dictionary<BattleActionType, BattleAction>();
        #endregion

        #region Property

        public CombatUnit Unit0 { get; protected set; } // 如果是群攻, 所有 unit 用 List 或数组
        public CombatUnit Unit1 { get; protected set; } // 如果是群攻, 所有 unit 用 List 或数组
        public List<CombatStep> Steps { get; protected set; }

        public bool IsLoaded => Unit0.MapClass != null && Unit1.MapClass != null;

        public int StepCount => Steps.Count;

        #endregion

        #region Unity Callback
        private void Awake() {
            InitBattleActions();

            Unit0 = new CombatUnit(0);
            Unit1 = new CombatUnit(1);
            Steps = new List<CombatStep>();
        }

        private void InitBattleActions() {
            if (battleActions != null && battleActions.Length > 0) {
                for (int i = 0; i < battleActions.Length; i++) {
                    if (battleActions[i] == null) {
                        continue;
                    }

                    BattleAction action = battleActions[i];
                    if (battleActionDict.ContainsKey(action.ActionType)) {
                        Debug.LogWarningFormat("Battle Action {0} is exist. OVERRIDE.", action.ActionType.ToString());
                    }
                    battleActionDict[action.ActionType] = action;
                }
            }
        }

        private void OnDestroy() {
            Unit0.Dispose();
            Unit0 = null;
            Unit1.Dispose();
            Unit1 = null;
            Steps = null;
        }
        #endregion

        #region Load
        public bool LoadCombatUnit(MapClass mapClass0, MapClass mapClass1) {
            return Unit0.Load(mapClass0) && Unit1.Load(mapClass1);
        }
        #endregion

        #region Get Combat Unit
        public CombatUnit GetCombatUnit(int position) {
            switch (position) {
                case 0:
                    return Unit0;
                case 1:
                    return Unit1;
                default:
                    Debug.LogError("Combat -> GetCombatUnit: index is out of range.");
                    return null;
            }
        }
        #endregion

        #region Battle
        /// <summary>
        /// 开始战斗
        /// </summary>
        public void BattleBegin() {
            if (!IsLoaded) {
                Debug.LogError("Combat -> StartBattle: please load combat unit first.");
                return;
            }

            if (StepCount > 0) {
                Debug.LogError("Combat -> StartBattle: battle is not end.");
                return;
            }

            if (!battleActionDict.TryGetValue(BattleActionType.Prepare, out BattleAction action)) {
                Debug.LogError("Combat -> StartBattle: BattleActionType.Prepare is not found, check the code.");
                return;
            }

            // 准备阶段
            CombatStep firstStep = action.CalcBattle(this, default, default);
            // firstStep.message = action.message;
            Steps.Add(firstStep);

            if (!action.IsBattleEnd(this, firstStep.AtkVal, firstStep.DefVal)) {
                CalcBattle(firstStep.AtkVal, firstStep.DefVal);
            }
        }

        /// <summary>
        /// 计算战斗数据
        /// </summary>
        private void CalcBattle(CombatVariable atkVal, CombatVariable defVal) {
            CombatUnit atker = GetCombatUnit(atkVal.position);
            BattleActionType actionType = GetBattleActionType(atker.WeaponType);
            if (!battleActionDict.TryGetValue(actionType, out BattleAction action)) {
                Debug.LogErrorFormat("Combat -> StartBattle: BattleActionType.{0} is not found, check the code.", actionType.ToString());
                return;
            }


            CombatStep step = action.CalcBattle(this, atkVal, defVal);
            // step.message = action.message;
            Steps.Add(step);

            // 如果战斗没有结束, 交换攻击者与防守者
            if (!action.IsBattleEnd(this, step.AtkVal, step.DefVal)) {
                if (step.DefVal.canAtk) {
                    CalcBattle(step.DefVal, step.AtkVal);
                } else {
                    // 如果防守方不可反击
                    defVal = step.DefVal;
                    defVal.action = true;
                    if (!action.IsBattleEnd(this, defVal, step.AtkVal)) {
                        CalcBattle(step.AtkVal, defVal);
                    }
                }
            }
        }

        /// <summary>
        /// 战斗是否结束
        /// </summary>
        /// <param name="atkVal"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        private bool IsBattleEnd(ref CombatVariable atkVal, ref CombatVariable defVal) {
            // 攻击者死亡, 防守者死亡
            if (atkVal.IsDead || defVal.IsDead) {
                return true;
            }

            // 如果防守者可反击
            if (defVal.canAtk) {
                // 交换攻击者与防御者
                SwapCombatVariable(ref atkVal, ref defVal);
            }

            // 如果攻击者行动过了
            if (atkVal.action) {
                //CombatUnit atker = GetCombatUnit(atkVal.position);
                //CombatUnit defer = GetCombatUnit(defVal.position);

                // TODO 是否继续攻击, 必要时需要在 CombatVariable 加入其它控制变量
                // atker.role.skill 包含继续战斗的技能
                // defer.role.skill 包含继续战斗的技能
                //if (condition) {
                //    atkVal.action = false;
                //}
            }

            // 攻击者行动过了
            return atkVal.action;
        }

        /// <summary>
        /// 交换攻击者与防御者
        /// </summary>
        /// <param name="atkVal"></param>
        /// <param name="defVal"></param>
        public void SwapCombatVariable(ref CombatVariable atkVal, ref CombatVariable defVal) {
            CombatVariable tmp = atkVal;
            atkVal = defVal;
            defVal = tmp;
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd() {
            if (StepCount > 0) {
                CombatStep result = Steps[StepCount - 1];

                CombatVariable unit0Result = result.GetCombatVariable(0);
                CombatVariable unit1Result = result.GetCombatVariable(1);

                // TODO 经验值战利品
                Unit0.MapClass.OnBattleEnd(unit0Result.hp, Unit0.Durability);
                Unit1.MapClass.OnBattleEnd(unit1Result.hp, Unit1.Durability);

                Steps.Clear();
            }

            Unit0.ClearMapClass();
            Unit1.ClearMapClass();
        }

        /// <summary>
        /// 获取行动方式(如何计算战斗数据)
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        private BattleActionType GetBattleActionType(WeaponType weaponType) {
            // TODO 由于没有动画支持, 所以并没有其他武器
            // 你可以添加其他武器到这里

            switch (weaponType) {
                case WeaponType.Sword:
                    //case WeaponType.Lance:
                    //case WeaponType.Axe:
                    //case WeaponType.Bow:
                    return BattleActionType.Attack;
                //case WeaponType.Staff:
                //if (如果法杖是治疗) {
                //    return BattleActionType.Heal;
                //}
                //else if (法杖是其它等) {
                //    return BattleActionType.自定义类型;
                //}
                //case WeaponType.Fire:
                //case WeaponType.Thunder:
                //case WeaponType.Wind:
                //case WeaponType.Holy:
                //case WeaponType.Dark:
                //    return BattleActionType.MageAttack;
                default:
                    return BattleActionType.Unknow;
            }
        }
        #endregion
    }
}
