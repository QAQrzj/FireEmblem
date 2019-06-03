using System;
using Dev;

namespace Models {
    public abstract class Item : IDisposable {
        #region enum OwnerType
        [Serializable]
        public enum OwnerTypeEnum {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0,

            /// <summary>
            /// 角色
            /// </summary>
            Role,

            /// <summary>
            /// 商店
            /// </summary>
            Shop,

            /// <summary>
            /// 地图
            /// </summary>
            Map
        }

        #endregion

        #region Property
        protected ItemData Self { get; set; }

        public ItemInfo Info { get; private set; }

        public UniqueInfo UniqueInfo => Info.uniqueInfo;

        public abstract ItemType ItemType { get; }

        public ulong Guid => Self.guid;

        public int ItemId => Self.itemId;

        public int Durability => Self.durability;

        public bool IsBroken => Self.durability <= 0;

        public OwnerTypeEnum OwnerType {
            get => Self.ownerType;
            set => Self.ownerType = value;
        }

        public int OwnerId {
            get => Self.ownerId;
            set => Self.ownerId = value;
        }
        #endregion

        #region Constructor
        protected Item(ItemInfo info, ulong guid) {
            Info = info;
            Self = new ItemData {
                guid = guid
            };
        }
        #endregion

        #region Load
        public virtual void Load(ItemData data) {
            data.CopyTo(Self);
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            if (ModelManager.models.ContainsKey(typeof(ItemModel))) {
                ItemModel model = ModelManager.models.Get<ItemModel>();
                if (model.ContainsKey(Guid)) {
                    model.Remove(Guid);
                }
            }

            Self = null;
        }
        #endregion
    }

    public class Weapon : Item {
        public sealed override ItemType ItemType => ItemType.Weapon;

        public new WeaponUniqueInfo UniqueInfo => Info.uniqueInfo as WeaponUniqueInfo;

        #region Constructor
        public Weapon(ItemInfo info, ulong guid) : base(info, guid) {

        }
        #endregion
    }

    public class Consumable : Item {
        public sealed override ItemType ItemType => ItemType.Consumable;

        public new ConsumableUniqueInfo UniqueInfo => Info.uniqueInfo as ConsumableUniqueInfo;

        #region Constructor
        public Consumable(ItemInfo info, ulong guid) : base(info, guid) {

        }
        #endregion
    }

    public class Ornament : Item {
        public sealed override ItemType ItemType => ItemType.Ornament;

        public new OrnamentUniqueInfo UniqueInfo => Info.uniqueInfo as OrnamentUniqueInfo;

        #region Constructor
        public Ornament(ItemInfo info, ulong guid) : base(info, guid) {

        }
        #endregion
    }
}
