using DR.Book.SRPG_Dev.Framework;
using Models;
using UnityEngine;

public class Test : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // 我们保存配置文件根目录
        ConfigLoader.rootDirectory = Application.streamingAssetsPath + "/Config";

        CharacterInfoConfig character = ConfigFile.Get<CharacterInfoConfig>();
        Debug.Log(character[0].name);

        TextInfoConfig text = ConfigFile.Get<TextInfoConfig>();
        TextInfo info = text.datas[0];
        Debug.Log(info.id + ": " + info.text);
    }
}
