using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maps {
    [AddComponentMenu("SRPG/Map Object/Map Class")]
    [RequireComponent(typeof(ClassAnimatorController))]
    public class MapClass : MapObstacle {
        public override MapObjectType MapObjectType => MapObjectType.Class;

        private ClassAnimatorController animatorController;
        /// <summary>
        /// 职业动画控制器
        /// </summary>
        /// <value>The animator controller.</value>
        public ClassAnimatorController AnimatorController {
            get {
                if (animatorController == null) {
                    animatorController = GetComponent<ClassAnimatorController>();
                }
                return animatorController;
            }
        }

        [SerializeField]
        private float moveTimePerCell = 1f;

        /// <summary>
        /// 每一格子移动时间
        /// </summary>
        /// <value>The move time per cell.</value>
        public float MoveTimePerCell {
            get => moveTimePerCell;
            set => moveTimePerCell = value;
        }

        /// <summary>
        /// 是否在移动状态
        /// </summary>
        /// <value><c>true</c> if moving; otherwise, <c>false</c>.</value>
        public bool Moving { get; set; } = false;

        private object role;

        /// <summary>
        /// 获取移动中的方向
        /// </summary>
        /// <returns>The moving direction.</returns>
        /// <param name="current">Current.</param>
        /// <param name="next">Next.</param>
        private Direction GetMovingDirection(CellData current, CellData next) {
            Vector3Int offset = next.Position - current.Position;

            if (offset.x == 0) {
                if (offset.y == 1) {
                    return Direction.Up;
                } else if (offset.y == -1) {
                    return Direction.Down;
                }
            } else if (offset.y == 0) {
                if (offset.x == 1) {
                    return Direction.Right;
                } else if (offset.x == -1) {
                    return Direction.Left;
                }
            }

            Debug.LogError("MapClass -> GetMovingDirection: Check the code. Offset: " + offset.ToString());
            return Direction.Down;
        }

        /// <summary>
        /// 移动每一格
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="current">Current.</param>
        /// <param name="next">Next.</param>
        private IEnumerator MovingTo(CellData current, CellData next) {
            // 设置方向
            Direction direction = GetMovingDirection(current, next);
            animatorController.SetMoveDirection(direction);

            // 计算 sortingOrder
            if (Renderer != null) {
                Renderer.sortingOrder = MapObject.CalcSortingOrder(Map, next.Position);
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
            Vector3 pos = start;
            float time = 0f;
            while (pos != end) {
                time += Time.deltaTime;
                pos = Vector3.Lerp(start, end, time / moveTimePerCell);
                transform.position = pos;
                yield return null;
            }
        }

        #region Delegate
        public delegate void OnMovingEndDelegate(CellData endCell);
        public event OnMovingEndDelegate OnMovingEnd;
        #endregion

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="movePath">Move path.</param>
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
        /// <returns>The move.</returns>
        /// <param name="movePath">Move path.</param>
        private IEnumerator Move(Stack<CellData> movePath) {
            Moving = true;

            // 获取当前节点
            CellData current = movePath.Pop();
            CellData next;
            while (movePath.Count > 0) {
                // 获取下一个节点
                next = movePath.Pop();
                if (next == null) {
                    Debug.LogError("MapClass -> Moving Error: Next CellData is null. Current CellData is " + current.Position.ToString());
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
        /// 读取信息
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="id">Identifier.</param>
        public virtual bool Load(int id) {
            // TODO
            return true;
        }

        public override void OnDespawn() {
            base.OnDespawn();

            if (Moving) {
                StopAllCoroutines();
                Moving = false;
            }
        }
    }
}
