using UnityEngine;

namespace Maps {
    [AddComponentMenu("SRPG/Map Object/Map Mouse Cursor")]
    public class MapMouseCursor : MapCursor {
        public override MapObjectType MapObjectType => MapObjectType.MouseCursor;

        public override MapCursorType CursorType {
            set {
                if (value != MapCursorType.Mouse) {
                    return;
                }
                base.CursorType = value;
            }
        }

        public override void OnSpawn() {
            base.OnSpawn();

            CursorType = MapCursorType.Mouse;
        }
    }
}
