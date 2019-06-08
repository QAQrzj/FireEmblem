using UnityEngine;
using Utility;
using Weapon;

namespace Maps {
    [RequireComponent(typeof(Animator))]
    public class ClassAnimatorController : MonoBehaviour {
        #region Field
        private Animator animator;
        #endregion

        #region Property
        public Animator Animator {
            get {
                if (animator == null) {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }
        #endregion

        #region Unity Callback
        private void Awake() {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }
        }
        #endregion

        #region Clip Length Method
        public float GetAttackAnimationLength(Direction direction, WeaponType weapon) {
            string clipName = GetAttackAnimationName(direction, weapon);
            return Animator.GetClipLength(clipName);
        }
        #endregion

        #region Play Method
        public void SetMoveDirection(Direction direction) {
            if (Animator.GetBool("PrepareAttack")) {
                return;
            }

            Animator.SetInteger("Direction", direction.ToInteger());
        }

        public void PlayMove() {
            if (!Animator.GetBool("PrepareAttack") && !Animator.GetBool("Move")) {
                Animator.SetBool("Move", true);
            }
        }

        public void StopMove() {
            if (Animator.GetBool("Move")) {
                Animator.SetBool("Move", false);
            }
        }

        public void PlayPrepareAttack(Direction direction, WeaponType weapon) {
            StopMove();

            if (!Animator.GetBool("PrepareAttack")) {
                Animator.SetInteger("Direction", direction.ToInteger());
                Animator.SetInteger("Weapon", weapon.ToInteger());
                Animator.SetBool("PrepareAttack", true);
            }
        }

        public void StopPrepareAttack() {
            if (Animator.GetBool("PrepareAttack")) {
                Animator.ResetTrigger("Attack");
                Animator.ResetTrigger("Evade");
                Animator.ResetTrigger("Damage");
                Animator.SetBool("PrepareAttack", false);
            }
        }

        public void PlayAttack() {
            if (!Animator.GetBool("PrepareAttack")) {
                return;
            }

            Animator.SetTrigger("Attack");
        }

        public void PlayEvade() {
            if (!Animator.GetBool("PrepareAttack")) {
                return;
            }

            Animator.SetTrigger("Evade");
        }

        public void PlayDamage() {
            if (!Animator.GetBool("PrepareAttack")) {
                return;
            }

            Animator.SetTrigger("Damage");
        }
        #endregion

        #region Static Method
        public static string GetMoveAnimationName(Direction direction) {
            return "Move" + direction.ToString();
        }

        public static string GetAttackAnimationName(Direction direction, WeaponType weapon) {
            return weapon.ToString() + direction.ToString();
        }

        public static string GetEvadeAnimationName(Direction direction) {
            return "Evade" + direction.ToString();
        }

        public static string GetDamageAnimationName(Direction direction) {
            return "Damage" + direction.ToString();
        }
        #endregion
    }
}
