using System;
using System.Xml.Serialization;

namespace Models {
    [Serializable]
    public class CharacterInfoConfig : BaseXmlConfig<int, CharacterInfo> {

    }

    [Serializable]
    public class CharacterInfo : IConfigData<int> {
        /// <summary>
        /// 人物 ID
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// 头像
        /// </summary>
        [XmlAttribute]
        public string profile;

        /// <summary>
        /// 人物的职业 ID
        /// </summary>
        [XmlAttribute]
        public int classId;

        /// <summary>
        /// 基本等级
        /// </summary>
        [XmlAttribute]
        public int level;

        /// <summary>
        /// 基本生命值
        /// </summary>
        [XmlAttribute]
        public int hp;

        /// <summary>
        /// 战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 幸运值(作用待定)
        /// </summary>
        [XmlAttribute]
        public int luk;

        /// <summary>
        /// TODO 包含的技能 IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        /// <summary>
        /// TODO 起始包含的物品 IDs
        /// </summary>
        //[XmlElement]
        //public int[] items;

        public int GetKey() {
            return id;
        }
    }
}
