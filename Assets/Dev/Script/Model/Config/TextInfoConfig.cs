using System;
using UnityEngine;

namespace Models {
    [Serializable]
    public class TextInfoConfig : BaseTxtConfig<int, TextInfo> {

    }

    [Serializable]
    public class TextInfo : ITxtConfigData<int> {
        public int id;
        public string text;

        public bool FormatText(string line) {
            string[] words = line.Split('\t');
            if (words.Length != 2) {
                Debug.LogErrorFormat("{0} -> `FormatText` Error: Length error.", GetType().Name);
                return false;
            }

            try {
                id = int.Parse(words[0]);
                text = words[1].Trim();
            } catch (Exception e) {
                Debug.LogErrorFormat("{0} -> `FormatText` Error: {1}", GetType().Name, e.ToString());
                return false;
            }
            return true;
        }

        public int GetKey() {
            return id;
        }
    }
}
