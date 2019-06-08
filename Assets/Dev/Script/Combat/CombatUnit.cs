using System;

namespace CombatManagement {
    using Maps;
    using Models;
    using Weapon;

    public class CombatUnit : IDisposable {
        public MapClass MapClass { get; private set; }

        public Role Role => MapClass.Role;

        /// <summary>
        /// 战斗中的位置
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// 生命值
        /// </summary>
        public int Hp { get; private set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHp { get; private set; }

        /// <summary>
        /// 攻击
        /// </summary>
        public int Atk { get; private set; }

        /// <summary>
        /// 魔法攻击
        /// </summary>
        public int MageAtk { get; private set; }

        /// <summary>
        /// 防御
        /// </summary>
        public int Def { get; private set; }

        /// <summary>
        /// 魔法防御
        /// </summary>
        public int MageDef { get; private set; }

        /// <summary>
        /// 攻速
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// 命中率
        /// </summary>
        public int Hit { get; private set; }

        /// <summary>
        /// 暴击率
        /// </summary>
        public int Crit { get; private set; }

        /// <summary>
        /// 回避率
        /// </summary>
        public int Avoidance { get; private set; }

        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType WeaponType { get; private set; }

        /// <summary>
        /// 武器耐久度
        /// </summary>
        public int Durability { get; private set; }

        public CombatUnit(int position) => Position = position;

        public bool Load(MapClass mapClass) {
            if (mapClass == null) {
                return false;
            }


            if (mapClass.Role == null) {
                return false;
            }

            MapClass = mapClass;

            Hp = Role.Hp;
            MaxHp = Role.MaxHp;
            Atk = Role.Attack;
            MageAtk = Role.MageAttack;
            Def = Role.Defence;
            MageDef = Role.MageDefence;
            Speed = Role.Speed;
            Hit = Role.Hit;
            //Crit = Role.crit;
            Avoidance = Role.Avoidance;
            if (Role.EquipedWeapon == null) {
                WeaponType = WeaponType.Unknow;
                Durability = 0;
            } else {
                WeaponType = Role.EquipedWeapon.UniqueInfo.weaponType;
                Durability = Role.EquipedWeapon.Durability;
            }
            return true;
        }

        public void Dispose() {
            MapClass = null;
            Position = -1;
        }

        public void ClearMapClass() {
            MapClass = null;
        }
    }
}
