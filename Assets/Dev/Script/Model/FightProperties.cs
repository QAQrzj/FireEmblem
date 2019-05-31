using System;
using System.Xml.Serialization;
using Dev;

namespace Models {
    [Serializable]
    public struct FightProperties {
        /// <summary>
        /// 力量
        /// </summary>
        [XmlAttribute]
        public int str;

        /// <summary>
        /// 魔力
        /// </summary>
        [XmlAttribute]
        public int mag;

        /// <summary>
        /// 技巧
        /// </summary>
        [XmlAttribute]
        public int skl;

        /// <summary>
        /// 速度
        /// </summary>
        [XmlAttribute]
        public int spd;

        /// <summary>
        /// 防御
        /// </summary>
        [XmlAttribute]
        public int def;

        /// <summary>
        /// 魔防
        /// </summary>
        [XmlAttribute]
        public int mdf;

        [XmlIgnore]
        public int this[FightPropertyType type] {
            get {
                switch (type) {
                    case FightPropertyType.STR:
                        return str;
                    case FightPropertyType.MAG:
                        return mag;
                    case FightPropertyType.SKL:
                        return skl;
                    case FightPropertyType.SPD:
                        return spd;
                    case FightPropertyType.DEF:
                        return def;
                    case FightPropertyType.MDF:
                        return mdf;
                    default:
                        return 0;
                }
            }
        }

        public static FightProperties operator +(FightProperties lhs, FightProperties rhs) {
            FightProperties fight = new FightProperties {
                str = lhs.str + rhs.str,
                mag = lhs.mag + rhs.mag,
                skl = lhs.skl + rhs.skl,
                spd = lhs.spd + rhs.spd,
                def = lhs.def + rhs.def,
                mdf = lhs.mdf + rhs.mdf
            };
            return fight;
        }
    }
}
