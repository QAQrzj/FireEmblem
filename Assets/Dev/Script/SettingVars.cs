using UnityEngine;

namespace Dev {
    public static class SettingVars {
        public const int levelUpExp = 100;
        public const int roleItemCount = 5;

        private static int maxLevel = 20;
        private static int maxHp = 60;
        private static int maxLuk = 30;

        public static int MaxLevel {
            get => maxLevel;
            set => maxLevel = Mathf.Max(1, value);
        }

        public static int MaxHp {
            get => maxHp;
            set => maxHp = Mathf.Max(1, value);
        }

        public static int MaxLuk {
            get => maxLuk;
            set => maxLuk = Mathf.Max(0, value);
        }
    }
}
