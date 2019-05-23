using UnityEngine;

namespace Maps {
    using DR.Book.SRPG_Dev.Framework;

    [DisallowMultipleComponent]
    public abstract class MapObject : MonoBehaviour, IReusableComponent {
        #region Field/Property
        [SerializeField]
        public SpriteRenderer Renderer;
        private Vector3Int cellPosition;

        /// <summary>
        /// 所属地图
        /// </summary>
        /// <value>The map.</value>
        public MapGraph Map { get; set; }

        /// <summary>
        /// 地图中网格的位置
        /// </summary>
        /// <value>The cell position.</value>
        public Vector3Int CellPosition {
            get => cellPosition;
            set {
                cellPosition = value;
                if (Renderer != null) {
                    Renderer.sortingOrder = MapObject.CalcSortingOrder(Map, value);
                }
            }
        }

        /// <summary>
        /// 地图对象类型
        /// </summary>
        /// <value>The type of the map object.</value>
        public abstract MapObjectType MapObjectType { get; }
        #endregion

        #region Method
        public void InitMapObject(MapGraph map) {
            Map = map;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="world">If set to <c>true</c> world.</param>
        /// <param name="center">If set to <c>true</c> center.</param>
        public void UpdatePosition(bool world = true, bool center = false) {
            if (Map == null) {
                Debug.LogError(name + " Map is null.");
                return;
            }

            Vector3 pos = Map.GetCellPosition(CellPosition, world, center);
            if (world) {
                transform.position = pos;
            } else {
                transform.localPosition = pos;
            }
        }

        public void UpdatePosition(Vector3Int cellPosition, bool world = true, bool center = false) {
            CellPosition = cellPosition;
            UpdatePosition(world, center);
        }

        /// <summary>
        /// 计算 sortingOrder
        /// </summary>
        /// <returns>The sorting order.</returns>
        /// <param name="map">Map.</param>
        /// <param name="cellPosition">Cell position.</param>
        public static int CalcSortingOrder(MapGraph map, Vector3Int cellPosition) {
            if (map == null) {
                return 0;
            }

            // 相对零点坐标
            Vector3Int relative = cellPosition - map.LeftDownPosition;

            return -(map.Width * relative.y + (map.Width - relative.x));
        }
        #endregion

        #region IReusableComponent 对象池组件
        public virtual void OnSpawn() { }

        public virtual void OnDespawn() {
            Map = null;
        }
        #endregion
    }
}
