using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maps {
    using Dev;
    using Models;

    [AddComponentMenu("SRPG/Map Object/Map Class")]
    [RequireComponent(typeof(ClassAnimatorController))]
    public class MapClass : MapObstacle {
        #region Delegate
        public delegate void OnMovingEndDelegate(CellData endCell);
        public event OnMovingEndDelegate OnMovingEnd;
        #endregion

        #region Field
        [SerializeField]
        private float moveTimePerCell = 1f;
        private ClassAnimatorController animatorController;
        #endregion

        #region Property
        public sealed override MapObjectType MapObjectType => MapObjectType.Class;

        /// <summary>
        /// 每一格子移动时间
        /// </summary>
        public float MoveTimePerCell {
            get => moveTimePerCell;
            set => moveTimePerCell = value;
        }

        /// <summary>
        /// 是否在移动状态
        /// </summary>
        public bool Moving { get; private set; }

        /// <summary>
        /// 职业动画控制器
        /// </summary>
        public ClassAnimatorController AnimatorController {
            get {
                if (animatorController == null) {
                    animatorController = GetComponent<ClassAnimatorController>();
                }
                return animatorController;
            }
        }

        public Role Role { get; private set; }
        #endregion

        #region Unity Callback
        protected virtual void Awake() {
            if (animatorController == null) {
                animatorController = GetComponent<ClassAnimatorController>();
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Load(int id, RoleType roleType) {
            RoleModel model = ModelManager.models.Get<RoleModel>();
            Role = model.GetOrCreateRole(id, roleType);
            return Role != null;
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="movePath"></param>
        public void StartMove(Stack<CellData> movePath) {
            if (movePath == null || movePath.Count == 0) {
                Debug.LogError(name + " StartMove Error, path is null or empty.");
                return;
            }

            if (Moving) {
                Debug.LogError(name + " is in moving.");
                return;
            }

            if (Map == null) {
                Debug.LogError(name + " map is null.");
                return;
            }

            StartCoroutine(Move(movePath));
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="movePath"></param>
        /// <returns></returns>
        private IEnumerator Move(Stack<CellData> movePath) {
            Moving = true;

            // 获取当前节点
            CellData current = movePath.Pop();
            CellData next;
            while (movePath.Count > 0) {
                // 获取下一个节点
                next = movePath.Pop();
                if (next == null) {
                    Debug.LogError("MapClass -> Moving Error. Next CellData is null. Current Cell Data is " + current.Position.ToString());
                    continue;
                }

                // 等待移动一个格子完成
                yield return MovingTo(current, next);

                // 设置当前节点
                current = next;
            }

            Moving = false;

            // 移动完成事件
            OnMovingEnd?.Invoke(current);
        }

        /// <summary>
        /// 移动每一格
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private IEnumerator MovingTo(CellData current, CellData next) {
            // 设置方向
            Direction direction = GetMovingDirection(current, next);
            AnimatorController.SetMoveDirection(direction);

            // 计算 sortingOrder
            if (GetComponent<Renderer>() != null) {
                GetComponent<Renderer>().sortingOrder = CalcSortingOrder(Map, next.Position);
            }

            if (moveTimePerCell <= 0f) {
                Debug.LogError(name + " Move time can not be less equal zero.");
                transform.position = Map.GetCellPosition(next.Position);
                yield break;
            }

            // 获取起始与结束坐标
            Vector3 start = Map.GetCellPosition(current.Position);
            Vector3 end = Map.GetCellPosition(next.Position);

            // 开始移动
            Vector3 pos;
            float time = 0f;
            while (true) {
                time += Time.deltaTime;
                pos = Vector3.Lerp(start, end, time / moveTimePerCell);
                transform.position = pos;
                yield return null;
                if (pos == end) {
                    break;
                }
            }
        }

        /// <summary>
        /// 获取移动中的方向
        /// </summary>
        /// <returns></returns>
        private Direction GetMovingDirection(CellData current, CellData next) {
            Vector3Int offset = next.Position - current.Position;

            if (offset.x == 0) {
                if (offset.y == 1) {
                    return Direction.Up;
                }
                if (offset.y == -1) {
                    return Direction.Down;
                }
            } else if (offset.y == 0) {
                if (offset.x == 1) {
                    return Direction.Right;
                }
                if (offset.x == -1) {
                    return Direction.Left;
                }
            }

            Debug.LogError("MapClass -> GetMovingDirection: Check the code. Offset: " + offset.ToString());
            return Direction.Down;
        }
        #endregion

        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="durability"></param>
        public void OnBattleEnd(int hp, int durability) {
            Role.OnBattleEnd(hp, durability);

            if (Role.IsDead) {
                // TODO 死亡
            }
        }

        #region Pool Method
        public override void OnDespawn() {
            base.OnDespawn();

            if (Moving) {
                StopAllCoroutines();
                Moving = false;
            }

            Role = null;
        }
        #endregion
    }
}
