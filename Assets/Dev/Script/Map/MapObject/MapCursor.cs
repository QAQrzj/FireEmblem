using UnityEngine;

namespace Maps {
    [AddComponentMenu("SRPG/Map Object/Map Cursor")]
    public class MapCursor : MapObject {
        public enum MapCursorType {
            Mouse = 0,
            Move = 1,
            Attack = 2,
        }

        public Animator animator;

        [SerializeField]
        public Sprite[] cursorSprites;
        public virtual MapCursorType CursorType {
            set {
                if (Renderer == null) {
                    Debug.LogError("Cursor: SpriteRenderer was not found.");
                    return;
                }

                if (cursorSprites == null || cursorSprites.Length == 0) {
                    Debug.LogError("Cursor: There is no sprite.");
                    return;
                }

                int index = (int)value;

                if (index < 0 || index >= cursorSprites.Length) {
                    Debug.LogError("Cursor: Index is out of range.");
                    return;
                }

                Renderer.sprite = cursorSprites[index];
            }
        }

        public override MapObjectType MapObjectType => MapObjectType.Cursor;

        public override void OnSpawn() {
            if (Renderer == null) {
                Renderer = gameObject.GetComponentInChildren<SpriteRenderer>(true);
                if (Renderer == null) {
                    Debug.LogError("Cursor: SpriteRenderer was not found.");
                    return;
                }
            }
        }

        public override void OnDespawn() {
            if (Map != null && MapObjectType == MapObjectType.Cursor) {
                CellData cellData = Map.GetCellData(CellPosition);
                if (cellData != null) {
                    cellData.HasCursor = false;
                }
            }

            base.OnDespawn();
        }
    }
}
