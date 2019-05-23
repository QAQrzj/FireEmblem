using UnityEngine;

namespace Maps {
    [AddComponentMenu("SRPG/Map Object/Map Obstacle")]
    public class MapObstacle : MapObject {
        [SerializeField]
        private Animator animator;

        public override MapObjectType MapObjectType => MapObjectType.Obstacle;

        public Animator Animator { get => animator; set => animator = value; }
    }
}
