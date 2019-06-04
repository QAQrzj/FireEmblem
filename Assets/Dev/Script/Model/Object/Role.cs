using Dev;

namespace Models {
    public abstract class Role {
        #region Field
        protected Weapon equipedWeapon;
        protected readonly Item[] items = new Item[SettingVars.roleItemCount];
        #endregion

        #region Property
        protected RoleData Self { get; set; }

        public ulong Guid => Self.guid;

        public abstract RoleType RoleType { get; }

        public int CharacterId => Self.characterId;

        public Character Character {
            get {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateCharacter(Self.characterId);
            }
        }

        public int ClassId => Self.classId;

        public Class Cls {
            get {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateClass(Self.classId);
            }
        }

        public MoveConsumption MoveConsumption => Cls.MoveConsumption;

        public AttitudeTowards AttitudeTowards {
            get { return Self.attitudeTowards; }
            set { Self.attitudeTowards = value; }
        }

        public int Level {
            get { return Self.level; }
            set { Self.level = value; }
        }

        public virtual int Exp => Self.exp;

        public virtual FightProperties FightProperties => Self.fightProperties;

        public virtual int MaxHp => Self.fightProperties.hp;

        public int Hp => Self.hp;

        public virtual int Luk => Self.luk;

        public int Money => Self.money;

        public float MovePoint => Self.movePoint;

        //public bool Holding {
        //    get { return Self.holding; }
        //    set { Self.holding = value; }
        //}

        public virtual Weapon EquipedWeapon => equipedWeapon;

        public Item[] Items => items;

        public bool IsDead => Self.hp <= 0;
        #endregion

        #region Combat Property
        /// <summary>
        /// 物理攻击力
        /// </summary>
        public int Attack {
            get {
                if (equipedWeapon == null) {
                    return 0;
                }

                int atk = equipedWeapon.UniqueInfo.attack;
                atk += FightProperties[FightPropertyType.STR];
                atk += GetItemFightPropertySum(FightPropertyType.STR);
                return atk;
            }
        }

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public int MageAttack {
            get {
                if (equipedWeapon == null) {
                    return 0;
                }

                int mag = equipedWeapon.UniqueInfo.attack;
                mag += FightProperties[FightPropertyType.MAG];
                mag += GetItemFightPropertySum(FightPropertyType.MAG);
                return mag;
            }
        }

        /// <summary>
        /// 物理防御力
        /// </summary>
        public int Defence {
            get {
                int def = FightProperties[FightPropertyType.DEF];
                def += GetItemFightPropertySum(FightPropertyType.DEF);
                return def;
            }
        }

        /// <summary>
        /// 魔法防御力
        /// </summary>
        public int MageDefence {
            get {
                int mdf = FightProperties[FightPropertyType.MDF];
                mdf += GetItemFightPropertySum(FightPropertyType.MDF);
                return mdf;
            }
        }

        /// <summary>
        /// 攻速
        /// </summary>
        public int Speed {
            get {
                if (equipedWeapon == null) {
                    return 0;
                }

                int spd = FightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                spd -= equipedWeapon.UniqueInfo.weight;
                return spd;
            }
        }

        /// <summary>
        /// 命中率
        /// </summary>
        public int Hit {
            get {
                if (equipedWeapon == null) {
                    return 0;
                }

                int skl = FightProperties[FightPropertyType.SKL];
                skl += GetItemFightPropertySum(FightPropertyType.SKL);
                int hit = equipedWeapon.UniqueInfo.hit + skl * 2;
                return hit;
            }
        }

        /// <summary>
        /// 回避率
        /// </summary>
        public int Avoidance {
            get {
                int spd = FightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                int avd = spd * 2 + Luk + GetItemLukSum();

                return avd;
            }
        }
        #endregion

        #region Constructor
        protected Role() {
            Self = new RoleData();
        }

        protected Role(ulong guid) : this() {
            Self.guid = guid;
        }
        #endregion

        #region Load Method
        public virtual bool Load(RoleData data) {
            if (data == null) {
                return false;
            }
            data.CopyTo(Self);
            return true;
        }
        #endregion

        #region Level UP or Property Control
        public void AddFightProperty(FightPropertyType type, int value) {
            Self.fightProperties[type] += value;
        }

        public virtual void LevelUp() {

        }
        #endregion

        #region
        /// <summary>
        /// 获取物品空位
        /// </summary>
        /// <returns></returns>
        protected int GetNullItemIndex() {
            int index = -1;
            for (int i = 0; i < SettingVars.roleItemCount; i++) {
                if (items[i] == null) {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int AddItem(Item item) {
            if (item == null) {
                return -1;
            }

            int index = GetNullItemIndex();

            if (index != -1) {
                items[index] = item;

                // 如果是武器, 判断装备的武器是否为 null
                if (item.ItemType == ItemType.Weapon && equipedWeapon == null) {
                    equipedWeapon = item as Weapon;
                }
            }

            return index;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Item RemoveItem(int index) {
            if (index < 0 || index >= SettingVars.roleItemCount || items[index] == null) {
                return null;
            }

            Item item = items[index];
            items[index] = null;

            // 如果是装备的武器
            if (item.ItemType == ItemType.Weapon && equipedWeapon == item) {
                equipedWeapon = null;

                for (int i = 0; i < SettingVars.roleItemCount; i++) {
                    if (items[i] != null && items[i].ItemType == ItemType.Weapon) {
                        equipedWeapon = items[i] as Weapon;
                        break;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// 交换物品
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public bool SwapItem(int index1, int index2) {
            if (index1 < 0 || index1 >= SettingVars.roleItemCount || index2 < 0 || index2 >= SettingVars.roleItemCount) {
                return false;
            }

            Item tmp = items[index1];
            items[index1] = items[index2];
            items[index2] = tmp;
            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// 物品属性叠加
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetItemFightPropertySum(FightPropertyType type) {
            if (type == FightPropertyType.Length) {
                return 0;
            }

            int value = 0;

            // 如果装备武器不为 null, 则叠加武器属性
            if (equipedWeapon != null) {
                value += equipedWeapon.UniqueInfo.fightProperties[type];
            }

            // 叠加所有饰品属性
            foreach (Item item in items) {
                if (item != null && item.ItemType == ItemType.Ornament) {
                    value += (item as Ornament).UniqueInfo.fightProperties[type];
                }
            }

            return value;
        }

        /// <summary>
        /// 物品幸运叠加
        /// </summary>
        /// <returns></returns>
        public int GetItemLukSum() {
            int value = 0;

            // 如果装备武器不为 null, 则叠加武器幸运
            if (equipedWeapon != null) {
                value += equipedWeapon.UniqueInfo.luk;
            }

            // 叠加所有饰品幸运
            foreach (Item item in items) {
                if (item != null && item.ItemType == ItemType.Ornament) {
                    value += (item as Ornament).UniqueInfo.luk;
                }
            }

            return value;
        }
        #endregion

        #region Combat Method

        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="durability"></param>
        public void OnBattleEnd(int hp, int durability) {
            Self.hp = hp;
            if (AttitudeTowards == AttitudeTowards.Player) {
                equipedWeapon.Durability = durability;
            }
        }
        #endregion

        #region Property Methods
        public void OnMoveEnd(float consume) {
            Self.movePoint -= consume;
        }

        public void ResetMovePoint() {
            Self.movePoint = Cls.Info.movePoint;
        }
        #endregion
    }

    public class UniqueRole : Role {
        public sealed override RoleType RoleType => RoleType.Unique;
    }

    public class FollowingRole : Role {
        public sealed override RoleType RoleType => RoleType.Following;

        public FollowingRole(ulong guid) : base(guid) {

        }
    }
}
