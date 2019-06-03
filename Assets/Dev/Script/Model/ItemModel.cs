using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dev;
using UnityEngine;

namespace Models {
    public class ItemModel : ModelBase, IDictionary<ulong, Item> {
        #region Static
        /// <summary>
        /// 生成的下一个物品的 GUID
        /// </summary>
        public static ulong NextItemGUID { get; private set; }
        #endregion

        #region Field
        private Dictionary<ulong, Item> items;
        private Dictionary<int, ItemData> itemTemplates;
        #endregion

        #region IDictionary<int, Item> Properties
        public ICollection<ulong> Keys => items.Keys;

        public ICollection<Item> Values => items.Values;

        public int Count => items.Count;

        bool ICollection<KeyValuePair<ulong, Item>>.IsReadOnly => ((ICollection<KeyValuePair<ulong, Item>>)items).IsReadOnly;

        public Item this[ulong guid] {
            get => !items.TryGetValue(guid, out Item data) ? null : data;
            set => throw new NotImplementedException("Readonly.");
        }
        #endregion

        #region Load
        protected override void OnLoad() {
            NextItemGUID = 1UL;
            items = new Dictionary<ulong, Item>();
            itemTemplates = new Dictionary<int, ItemData>();
        }

        protected override void OnDispose() {
            ulong[] keys = items.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++) {
                items[keys[i]].Dispose();
            }
            items = null;
            itemTemplates = null;
        }
        #endregion

        #region IDictionary<int, Item> Method
        void IDictionary<ulong, Item>.Add(ulong guid, Item value) => throw new NotImplementedException("Readonly.");

        public bool ContainsKey(ulong guid) {
            return items.ContainsKey(guid);
        }

        public bool Remove(ulong guid) {
            return items.Remove(guid);
        }

        public bool TryGetValue(ulong guid, out Item value) {
            return items.TryGetValue(guid, out value);
        }

        void ICollection<KeyValuePair<ulong, Item>>.Add(KeyValuePair<ulong, Item> item) => throw new NotImplementedException("Readonly.");

        void ICollection<KeyValuePair<ulong, Item>>.Clear() => throw new NotImplementedException("Readonly.");

        bool ICollection<KeyValuePair<ulong, Item>>.Contains(KeyValuePair<ulong, Item> item) {
            return ((ICollection<KeyValuePair<ulong, Item>>)items).Contains(item);
        }

        void ICollection<KeyValuePair<ulong, Item>>.CopyTo(KeyValuePair<ulong, Item>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<ulong, Item>>)items).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<ulong, Item>>.Remove(KeyValuePair<ulong, Item> item) {
            return ((ICollection<KeyValuePair<ulong, Item>>)items).Remove(item);
        }

        public IEnumerator<KeyValuePair<ulong, Item>> GetEnumerator() {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// 获取物品信息
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private ItemInfo GetItemInfo(int itemId) {
            ItemInfoConfig config = DR.Book.SRPG_Dev.Framework.ConfigFile.Get<ItemInfoConfig>();
            ItemInfo info = config[itemId];

            if (info == null) {
                Debug.LogErrorFormat("ItemModel -> ItemInfo key `{0}` is not found.", itemId.ToString());
                return null;
            }
            return info;
        }

        /// <summary>
        /// 获取物品数据模版, 如果不存在则创建后获取
        /// </summary>
        /// <returns></returns>
        public ItemData GetOrCreateItemTemplate(int itemId) {
            if (!itemTemplates.TryGetValue(itemId, out ItemData data)) {
                ItemInfo info = GetItemInfo(itemId);
                if (info == null) {
                    return null;
                }

                data = new ItemData {
                    itemId = info.id
                };

                switch (info.itemType) {
                    case ItemType.Weapon:
                        WeaponUniqueInfo weapon = info.uniqueInfo as WeaponUniqueInfo;
                        data.durability = weapon.durability;
                        break;
                    case ItemType.Ornament:
                        break;
                    case ItemType.Consumable:
                        ConsumableUniqueInfo consumable = info.uniqueInfo as ConsumableUniqueInfo;
                        data.durability = consumable.stackingNumber == 1 ? consumable.amountUsed : consumable.stackingNumber;
                        break;
                }

                itemTemplates.Add(itemId, data);
            }
            return data;
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Item CreateItem(ItemInfo info, ItemData data, bool isTemplate) {
            Item item;
            switch (info.itemType) {
                case ItemType.Weapon:
                    item = new Weapon(info, isTemplate ? NextItemGUID++ : data.guid);
                    break;
                case ItemType.Ornament:
                    item = new Ornament(info, isTemplate ? NextItemGUID++ : data.guid);
                    break;
                case ItemType.Consumable:
                    item = new Consumable(info, isTemplate ? NextItemGUID++ : data.guid);
                    break;
                default:
                    Debug.LogError("ItemModel -> Create item: unknow type.");
                    item = null;
                    break;
            }

            if (item != null) {
                item.Load(data);
            }

            return item;
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public Item CreateItem(int itemId) {
            ItemInfo info = GetItemInfo(itemId);
            if (info == null) {
                return null;
            }

            ItemData template = GetOrCreateItemTemplate(itemId);
            if (template == null) {
                return null;
            }

            Item item = CreateItem(info, template, true);
            items.Add(item.Guid, item);
            return item;
        }

        /// <summary>
        /// 读取存档用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Item CreateItemFromSaver(ItemData data) {
            if (data == null) {
                return null;
            }

            ItemInfo info = GetItemInfo(data.itemId);
            if (info == null) {
                return null;
            }

            if (items.ContainsKey(data.guid)) {
                Debug.LogErrorFormat("ItemModel -> Create item from saver ERROR. GUID {0} is exist.", data.guid);
                return null;
            }

            // 判断 guid
            if (data.guid >= NextItemGUID) {
                NextItemGUID = data.guid + 1UL;
            }

            Item item = CreateItem(info, data, false);
            items.Add(item.Guid, item);
            return item;
        }
    }
}
