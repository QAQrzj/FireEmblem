using System;
using System.Xml.Serialization;
using UnityEngine;

namespace Models {
    [Serializable]
    public enum AttitudeTowards {
        /// <summary>
        /// 玩家
        /// </summary>
        Player,

        /// <summary>
        /// 敌人
        /// </summary>
        Enemy,

        /// <summary>
        /// 盟友
        /// </summary>
        Ally,

        /// <summary>
        /// 中立
        /// </summary>
        Neutral
    }

    [Serializable]
    public class RoleData : RuntimeData<RoleData> {
        [XmlAttribute]
        public ulong guid;

        [XmlAttribute]
        public int characterId;

        [XmlAttribute]
        public int classId;

        [XmlAttribute]
        public AttitudeTowards attitudeTowards;

        [XmlAttribute]
        public int level = 1;

        [XmlAttribute]
        public int exp = 0;

        [XmlAttribute]
        public int hp = 1;

        [XmlElement]
        public FightProperties fightProperties;

        [XmlAttribute]
        public int luk;

        [XmlAttribute]
        public int money;

        [XmlAttribute]
        public float movePoint;

        public override void CopyTo(RoleData data) {
            if (data == null) {
                Debug.LogError("RuntimeData -> CopyTo: data is null.");
                return;
            }

            if (data == this) {
                return;
            }

            data.characterId = characterId;
            data.classId = classId;
            data.level = level;
            data.exp = exp;
            data.hp = hp;
            data.fightProperties = fightProperties;
            data.luk = luk;
            data.money = money;
            data.movePoint = movePoint;
        }
    }
}
