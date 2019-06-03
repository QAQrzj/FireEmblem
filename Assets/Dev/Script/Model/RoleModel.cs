using System.Collections.Generic;
using Dev;
using UnityEngine;

namespace Models {
    public class RoleModel : ModelBase {
        #region Static
        /// <summary>
        /// 生成的下一个角色 GUID
        /// </summary>
        public static ulong NextRoleGUID { get; private set; }
        #endregion

        #region Field
        private Dictionary<ClassType, MoveConsumption> moveConsumptions;
        private Dictionary<int, Class> classes;
        private Dictionary<int, Character> characters;

        /// <summary>
        /// 独有角色
        /// </summary>
        private Dictionary<int, UniqueRole> uniqueRoles;

        /// <summary>
        /// 部下杂兵模版
        /// </summary>
        private Dictionary<int, RoleData> followingTemplates;

        /// <summary>
        /// 部下杂兵角色
        /// </summary>
        private Dictionary<ulong, FollowingRole> followingRoles;
        #endregion

        #region Load
        protected override void OnLoad() {
            moveConsumptions = new Dictionary<ClassType, MoveConsumption>();
            classes = new Dictionary<int, Class>();
            characters = new Dictionary<int, Character>();

            NextRoleGUID = 1UL;
            uniqueRoles = new Dictionary<int, UniqueRole>();
            followingTemplates = new Dictionary<int, RoleData>();
            followingRoles = new Dictionary<ulong, FollowingRole>();
        }

        protected override void OnDispose() {
            moveConsumptions = null;
            classes = null;
            characters = null;

            uniqueRoles = null;
            followingTemplates = null;
            followingRoles = null;
        }
        #endregion

        /// <summary>
        /// 获取或创建移动消耗
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public MoveConsumption GetOrCreateMoveConsumption(ClassType classType) {
            if (!moveConsumptions.TryGetValue(classType, out MoveConsumption consumption)) {
                MoveConsumptionInfoConfig config = DR.Book.SRPG_Dev.Framework.ConfigFile.Get<MoveConsumptionInfoConfig>();
                MoveConsumptionInfo info = config[classType];
                if (info == null) {
                    Debug.LogErrorFormat("RoleModel -> MoveConsumption key `{0}` is not found.", classType.ToString());
                    return null;
                }
                consumption = new MoveConsumption(info);
                moveConsumptions.Add(classType, consumption);
            }

            return consumption;
        }

        /// <summary>
        /// 获取或创建职业
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public Class GetOrCreateClass(int classId) {
            if (!classes.TryGetValue(classId, out Class cls)) {
                ClassInfoConfig config = DR.Book.SRPG_Dev.Framework.ConfigFile.Get<ClassInfoConfig>();
                ClassInfo info = config[classId];
                if (info == null) {
                    Debug.LogErrorFormat("RoleModel -> Class key `{0}` is not found.", classId.ToString());
                    return null;
                }
                cls = new Class(info);
                classes.Add(classId, cls);

            }
            return cls;
        }

        /// <summary>
        /// 获取或创建独有人物
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public Character GetOrCreateCharacter(int characterId) {
            if (!characters.TryGetValue(characterId, out Character character)) {
                CharacterInfoConfig config = DR.Book.SRPG_Dev.Framework.ConfigFile.Get<CharacterInfoConfig>();
                CharacterInfo info = config[characterId];
                if (info == null) {
                    Debug.LogErrorFormat("RoleModel -> CharacterInfo key `{0}` is not found.", characterId.ToString());
                    return null;
                }
                character = new Character(info);
                characters.Add(characterId, character);
            }
            return character;
        }

        /// <summary>
        /// 获取或创建杂兵模板
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public RoleData GetOrCreateFollowingTemplate(int classId) {
            Class cls = GetOrCreateClass(classId);

            if (cls == null || cls.Info == null) {
                return null;
            }

            ClassInfo info = cls.Info;

            if (!followingTemplates.TryGetValue(info.id, out RoleData data)) {
                data = new RoleData {
                    classId = info.id
                };

                // TODO 计算公式，计算 npc 出生数据

                followingTemplates.Add(data.classId, data);
            }

            return data;
        }

        /// <summary>
        /// 获取或创建角色
        /// 独有角色: characterId
        /// 部下杂兵角色: classId
        /// </summary>
        /// <returns></returns>
        public Role GetOrCreateRole(int id, RoleType roleType) {
            if (roleType == RoleType.Unique) {
                if (!uniqueRoles.TryGetValue(id, out UniqueRole role)) {
                    role = CreateUniqueRole(id);
                    if (role != null) {
                        uniqueRoles.Add(role.CharacterId, role);
                    }
                }
                return role;
            }
            FollowingRole followingRole = CreateFollowingRole(id);
            if (followingRole != null) {
                followingRoles.Add(followingRole.Guid, followingRole);
            }
            return followingRole;
        }

        /// <summary>
        /// 创建独有角色
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        private UniqueRole CreateUniqueRole(int characterId) {
            UniqueRole role = new UniqueRole();
            RoleData data = CreateUniqueRoleData(characterId);
            if (!role.Load(data)) {
                return null;
            }
            return role;
        }

        /// <summary>
        /// 创建部下角色
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        private FollowingRole CreateFollowingRole(int classId) {
            FollowingRole role = new FollowingRole(NextRoleGUID++);
            RoleData template = GetOrCreateFollowingTemplate(classId);
            if (!role.Load(template)) {
                return null;
            }
            return role;
        }

        /// <summary>
        /// 创建独有角色数据
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        private RoleData CreateUniqueRoleData(int characterId) {
            Character character = GetOrCreateCharacter(characterId);
            if (character == null) {
                return null;
            }

            Class cls = GetOrCreateClass(character.Info.classId);
            if (cls == null) {
                return null;
            }

            RoleData self = new RoleData {
                characterId = characterId,
                classId = character.Info.classId,
                level = Mathf.Clamp(character.Info.level, 0, SettingVars.MaxLevel),
                exp = 0,
                fightProperties = FightProperties.Clamp(character.Info.fightProperties + cls.Info.fightProperties, cls.Info.maxFightProperties)
            };
            self.hp = self.fightProperties.hp;
            self.luk = Mathf.Clamp(character.Info.luk, 0, SettingVars.MaxLuk);
            //self.money = character.info.money;
            self.movePoint = cls.Info.movePoint;

            return self;
        }
    }
}
