using System;

namespace CombatManagement {
    [Serializable]
    public struct CombatVariable {
        /// <summary>
        /// 位置
        /// </summary>
        public int position;

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp;

        /// <summary>
        /// 是否可攻击
        /// </summary>
        public bool canAtk;

        /// <summary>
        /// 武器耐久度
        /// </summary>
        public int durability;

        /// <summary>
        /// 动画类型
        /// </summary>
        public CombatAnimaType animaType;

        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool crit;

        /// <summary>
        /// 是否行动过
        /// </summary>
        public bool action;

        /// <summary>
        /// 是否已经死亡
        /// </summary>
        public bool IsDead => hp <= 0;

        public CombatVariable(int position, int hp, bool canAtk, CombatAnimaType animaType) {
            this.position = position;
            this.hp = hp;
            this.canAtk = canAtk;
            durability = 0;
            this.animaType = animaType;
            crit = false;
            action = false;
        }

        public CombatVariable(int position, int hp, bool canAtk, int durability, CombatAnimaType animaType) {
            this.position = position;
            this.hp = hp;
            this.canAtk = canAtk;
            this.durability = durability;
            this.animaType = animaType;
            crit = false;
            action = false;
        }

        public void ResetAnima() {
            animaType = CombatAnimaType.Unknow;
            crit = false;
        }

        public CombatVariable Clone() {
            CombatVariable variable = new CombatVariable {
                position = position,
                hp = hp,
                canAtk = canAtk,
                durability = durability,
                animaType = animaType,
                crit = crit,
                action = action
            };
            return variable;
        }
    }
}
