using System;
using UnityEngine;

namespace Utility {
    public static class TypeExtension {
        public static int ToInteger(Enum value) {
            return Convert.ToInt32(value);
        }

        public static AnimationClip FindClip(Animator animator, string clipName) {
            if (animator == null || string.IsNullOrEmpty(clipName)) {
                return null;
            }

            if (animator.runtimeAnimatorController == null) {
                return null;
            }

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips == null || clips.Length == 0) {
                return null;
            }

            return Array.Find<AnimationClip>(clips, clip => clip != null && clip.name == clipName);
        }

        public static float GetClipLength(Animator animator, string clipName) {
            AnimationClip clip = FindClip(animator, clipName);
            if (clip == null) {
                return 0;
            }
            return clip.length;
        }
    }
}
