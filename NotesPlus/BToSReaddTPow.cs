using BetterTOS2;
using Game.Interface;
using HarmonyLib;
using Server.Shared.State;
using Services;
using SML;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace NotesPlus
{
    public class BToSReaddTPow
    {
        public static void NewPrefix(Type type, string methodName, string func, Type[] parameters = null)
        {
            MethodInfo mOriginal = AccessTools.Method(type, methodName, parameters);
            MethodInfo mPrefix = AccessTools.Method(typeof(BToSReaddTPow), func);
            BToSReaddTPow.harmonyInstance.Patch(mOriginal, new HarmonyMethod(mPrefix));
        }
        public static void NewPostfix(Type type, string methodName, string func, Type[] parameters = null)
        {
            MethodInfo mOriginal = AccessTools.Method(type, methodName, parameters);
            MethodInfo mPostfix = AccessTools.Method(typeof(BToSReaddTPow), func);
            BToSReaddTPow.harmonyInstance.Patch(mOriginal, null, new HarmonyMethod(mPostfix));
        }
        public static void BToSRoleBucketsPostfix()
        {
            ModdedRoleBucket roleBucket = normalObject as ModdedRoleBucket;
            RoleInfo roleInfo = new RoleInfo
            {
                role = roleBucket.Role,
                sprite = BTOSInfo.sprites[roleBucket.RoleBucketIconPath],
                shortRoleName = roleBucket.Name
            };
            Service.Game.Roles.roleInfos.Add(roleInfo);
            Service.Game.Roles.roleInfoLookup.Add(roleBucket.Role, roleInfo);
            RoleBucket roleBucket2 = new RoleBucket
            {
                role = roleBucket.Role,
                limit = 100,
                roleName = roleBucket.Name,
                roles = roleBucket.Roles,
                roleAlignment = roleBucket.RoleAlignment,
                subAlignment = roleBucket.SubAlignment
            };
            SharedRoleData.roleBuckets.Add(roleBucket2);
            SharedRoleData.roleBucketLookup.Add(roleBucket.Role, roleBucket2);
            SharedRoleData.clientOnlyRoleBuckets.Add(roleBucket2);
            SharedRoleData.clientOnlyRoleBucketLookup.Add(roleBucket.Role, roleBucket2);
            SharedRoleData.modifierCards.Add(new RoleData(Btos2Role.TownPower, "Town Power", FactionType.TOWN, RoleAlignment.TOWN, SubAlignment.POWER, 3, 67, 1));
            UIRoleData.UIRoleDataInstance uiroleDataInstance = new UIRoleData.UIRoleDataInstance
            {
                role = roleBucket.Role,
                roleDesc = roleBucket.Name,
                elementName = roleBucket.Name,
                roleName = roleBucket.Name,
                roleIcon = BTOSInfo.sprites[roleBucket.RoleBucketIconPath]
            };
            ConfigureModData.uiRoleData.Add(uiroleDataInstance);
            if (!added)
            {
                MaterialReferenceManager.TryGetSpriteAsset(TMP_TextUtilities.GetHashCode("BTOSRoleIcons"), out TMP_SpriteAsset BTOSRoleIcons);
                Debug.LogWarning(BTOSRoleIcons);
                int noneIndex = BTOSRoleIcons.GetSpriteIndexFromHashcode(TMP_TextUtilities.GetHashCode("Role0"));
                TMP_SpriteGlyph noneGlyph = BTOSRoleIcons.spriteGlyphTable[noneIndex];
                int count = BTOSRoleIcons.spriteGlyphTable.Count;
                TMP_SpriteGlyph glyph = new TMP_SpriteGlyph((uint)count, noneGlyph.metrics, noneGlyph.glyphRect, 1, 0, noneGlyph.sprite);
                TMP_SpriteCharacter character = new TMP_SpriteCharacter(65534, BTOSRoleIcons, glyph)
                {
                    name = "Role242",
                    glyphIndex = (uint)count
                };
                BTOSRoleIcons.spriteCharacterTable.Add(character);
                BTOSRoleIcons.spriteGlyphTable.Add(glyph);
                BTOSRoleIcons.UpdateLookupTables();
            }
        }
        public static bool ToSpritePrefix(Role role, Sprite __result)
        {
            if (role == Btos2Role.TownPower)
            {
                __result = BTOSInfo.sprites["RoleCard_None"];
                return false;
            }
            return true;
        }
        public static void PopulateListItemsPostfix(RoleSelectionPanel __instance)
        {
            foreach (RoleCardListItem listItem in __instance.roleCardListItems)
                if (listItem.role == Btos2Role.TownPower)
                    listItem.transform.SetParent(null);
        }

        public static void DoYourThing()
        {
            normalObject = new ModdedRoleBucket();
            ModdedRoleBucket roleBucket = normalObject as ModdedRoleBucket;
            roleBucket.Role = Btos2Role.TownPower;
            roleBucket.RoleAlignment = RoleAlignment.TOWN;
            roleBucket.SubAlignment = SubAlignment.POWER;
            roleBucket.Name = "Town Power";
            roleBucket.RoleBucketIconPath = "RoleCard_None";
            roleBucket.Roles = new List<Role>()
            {
                Btos2Role.Jailor, Btos2Role.Marshal, Btos2Role.Prosecutor, Btos2Role.Mayor, Btos2Role.Monarch, Btos2Role.Pacifist
            };
            string stringTableString = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? Service.Home.LocalizationService.GetLocalizedString("FANCY_BUCKETS_TOWN") + " " + Service.Home.LocalizationService.GetLocalizedString("FANCY_BUCKETS_POWER") : Service.Home.LocalizationService.GetLocalizedString("GUI_ROLENAME_105");
            Service.Home.LocalizationService.stringTable_.Add("BTOS_ROLENAME_242", stringTableString);
            Service.Home.LocalizationService.stringTable_.Add("BTOS_ROLEBUCKET_242", stringTableString);
            Service.Home.LocalizationService.stringTable_.Add("Town Power", "[Player Notes+] Ignore this box, just close it.");
            BToSReaddTPow.NewPostfix(typeof(ConfigureModData), nameof(ConfigureModData.InitRoleBuckets), nameof(BToSRoleBucketsPostfix));
            BToSReaddTPow.NewPostfix(typeof(RoleSelectionPanel), nameof(RoleSelectionPanel.PopulateListItems), nameof(PopulateListItemsPostfix));
            BToSReaddTPow.NewPrefix(typeof(ToSpriteConversion), nameof(ToSpriteConversion.ToSprite), nameof(ToSpritePrefix));
        }
        public static Harmony harmonyInstance = new Harmony("synapsium.notes.plus.additional");
        public static object normalObject;
        public static bool added = false;
    }
}
