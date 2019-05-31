#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace Dev.Editor {
    using Maps;
    using Models;

    public static class EnumGUIContents {
        public static readonly GUIContent[] terrainTypeContents = GetEnumContents(typeof(TerrainType));
        public static readonly GUIContent[] classTypeContents = GetEnumContents(typeof(ClassType));

        private static GUIContent[] GetEnumContents(Type enumType) {
            return Enum.GetNames(enumType).Select(n => new GUIContent(n)).ToArray();
        }
    }
}
#endif
