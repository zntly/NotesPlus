using Game.Interface;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Server.Shared.State;
using Server.Shared.Extensions;
using System.Linq;
using Home.Shared;

namespace NotesPlus
{
    [HarmonyPatch(typeof(HudRoleListPanel), "ShowRoles")]
    public class ClaimspaceVisualizer
    {
        [HarmonyPostfix]
        public static void Postfix(HudRoleListPanel __instance)
        {
            if (!(bool)Settings.SettingsCache.GetValue("Claimspace Visualizer"))
                return;
            if (allRoleListItems.Count == 0)
            {
                isBtos = Utils.IsBTOS2();
                allRoleListItems = __instance.roleListItems;
                addedNumbers = new Dictionary<RoleListItem, List<Tuple<int, bool>>>();
                categorizedRoleListItems = new Dictionary<int, List<RoleListItem>>()
                {
                    {1, new List<RoleListItem>()},
                    {2, new List<RoleListItem>()},
                    {3, new List<RoleListItem>()},
                    {4, new List<RoleListItem>()},
                    {5, new List<RoleListItem>()}
                };
                bool allAny = true;
                foreach (RoleListItem roleListItem in allRoleListItems)
                {
                    addedNumbers.Add(roleListItem, new List<Tuple<int, bool>>());
                    Role role1 = roleListItem.role;
                    if (allAny && !isBtos && role1 != Role.ANY || isBtos && role1 != Btos2Role.Any)
                        allAny = false;
                    Role role2 = roleListItem.role2;
                    int role1Category = 0;
                    int role2Category = 0;

                    // Role 1 handling
                    if (role1.IsResolved())
                        role1Category = 1; // First category (Roles)
                    else
                    {
                        RoleBucket roleBucket = role1.GetRoleBucket();
                        if (roleBucket.subAlignment != SubAlignment.ANY)
                            role1Category = 2; // Second category (Sub-alignment buckets)
                        else if (!isBtos && role1 == Role.ANY || isBtos && role1 == Btos2Role.Any)
                            role1Category = 5; // Fifth category (Any)
                        else if (roleBucket.roleName.Contains("Random"))
                            role1Category = 4; // Fourth cateogry (Random X)
                        else
                            role1Category = 3; // Third category (Common X)
                    }

                    // Role 2 handling, only if it is a dual bucket
                    if (role2 != Role.NONE)
                        if (role2.IsResolved())
                            role2Category = 1;
                        else
                        {
                            RoleBucket roleBucket = role2.GetRoleBucket();
                            if (roleBucket.subAlignment != SubAlignment.ANY)
                                role2Category = 2;
                            else if (!isBtos && role2 == Role.ANY || isBtos && role2 == Btos2Role.Any)
                                role2Category = 5;
                            else if (roleBucket.roleName.Contains("Random"))
                                role2Category = 4;
                            else
                                role2Category = 3;
                        }

                    // Add list item to the least specific category possible
                    categorizedRoleListItems.GetValue(Math.Max(role1Category, role2Category)).Add(roleListItem);
                }
                if (allAny && (bool)Settings.SettingsCache.GetValue("Disable Claimspace Visualizer in All Any"))
                    return;
                ClaimspaceVisualizer.instance = __instance;
            }
            else
                foreach (RoleListItem roleListItem in allRoleListItems)
                    roleListItem.RefreshLabel();
        }

        public static void SortRoles(Dictionary<int, Tuple<Role, FactionType>> data)
        {
            if (allRoleListItems.Count == 0 || ClaimspaceVisualizer.instance == null)
                return; // Ignore if there's no list items here
            List<int> markedForReplacement = new List<int>();
            bool goToReplacements = false;
            for (int i = 0; i < 16; i++)
            {
                if (i == 15 && markedForReplacement.Count > 0)
                {
                    goToReplacements = true;
                    i = 0;
                }
                if (goToReplacements && !markedForReplacement.Contains(i))
                    continue;
                if (goToReplacements)
                    markedForReplacement.Remove(i);
                bool isKnown = data.ContainsKey(i);
                if (!isKnown)
                {
                    Tuple<List<Tuple<int, bool>>, Tuple<int, bool>> queueForDeletion = null;
                    foreach (KeyValuePair<RoleListItem, List<Tuple<int, bool>>> kvp in addedNumbers)
                        foreach (Tuple<int, bool> tuple in kvp.Value)
                            if (tuple.Item1 == i)
                            {
                                queueForDeletion = Tuple.Create(kvp.Value, tuple);
                                break;
                            }
                    if (queueForDeletion != null)
                        queueForDeletion.Item1.Remove(queueForDeletion.Item2);
                    continue;
                }
                Tuple<Role, FactionType> roleFactionData = data.GetValue(i);
                Tuple<int, bool> confirmedData = new Tuple<int, bool>(i, roleFactionData.Item2 != FactionType.UNKNOWN);
                int highestCategory = 0;
                int endCategory = 0;
                if (roleFactionData.Item1.IsResolved())
                    highestCategory = 1; // First category (Roles)
                else
                {
                    RoleBucket roleBucket = roleFactionData.Item1.GetRoleBucket();
                    if (roleBucket.subAlignment != SubAlignment.ANY)
                        highestCategory = 2; // Second category (Sub-alignment buckets)
                    else if (!isBtos && roleFactionData.Item1 == Role.ANY || isBtos && roleFactionData.Item1 == Btos2Role.Any)
                        highestCategory = 5; // Fifth category (Any)
                    else if (roleBucket.roleName.Contains("Random"))
                        highestCategory = 4; // Fourth cateogry (Random X)
                    else
                        highestCategory = 3; // Third category (Common X)
                }
                bool placed = false;
                int lowestFit = 6;
                foreach (KeyValuePair<int, List<RoleListItem>> kvp in categorizedRoleListItems)
                    if (kvp.Key < highestCategory)
                        foreach (RoleListItem roleListItem in kvp.Value)
                        {
                            List<Tuple<int, bool>> addedNumberList = addedNumbers.GetValue(roleListItem);
                            Tuple<int, bool> queueForDeletion = null;
                            foreach (Tuple<int, bool> tuple in addedNumberList)
                                if (tuple.Item1 == i)
                                    queueForDeletion = tuple;
                            if (queueForDeletion != null)
                                addedNumberList.Remove(queueForDeletion);
                        }
                    else
                        foreach (RoleListItem roleListItem in kvp.Value)
                        {
                            if (!placed && kvp.Key == 1 && (roleListItem.role == roleFactionData.Item1 || roleListItem.role2 == roleFactionData.Item1 || roleFactionData.Item1 > (Role)249 && IsHorsemanInAcolyte(roleListItem.role, roleListItem.role2, roleFactionData.Item1)))
                            {
                                lowestFit = 1;
                                List<Tuple<int, bool>> addedNumberList = addedNumbers.GetValue(roleListItem);
                                if (addedNumberList.Count == 0)
                                {
                                    placed = true;
                                    endCategory = 1;
                                    addedNumberList.Add(confirmedData);
                                }
                                else
                                {
                                    Tuple<int, bool> queueForDeletion = null;
                                    foreach (Tuple<int, bool> tuple in addedNumberList)
                                        if (tuple.Equals(confirmedData))
                                        {
                                            placed = true;
                                            endCategory = 1;
                                            break;
                                        }
                                        else if (tuple.Item2 == false && confirmedData.Item2 == true)
                                        {
                                            placed = true;
                                            endCategory = 1;
                                            markedForReplacement.Add(tuple.Item1);
                                            queueForDeletion = tuple;
                                            addedNumberList.Add(confirmedData);
                                            break;
                                        }
                                        else if (tuple.Item1 == i)
                                        {
                                            queueForDeletion = tuple;
                                            if (addedNumberList.Count - 1 == 0)
                                            {
                                                placed = true;
                                                endCategory = 1;
                                                addedNumberList.Add(confirmedData);
                                            }
                                            break;
                                        }
                                    if (queueForDeletion != null)
                                        addedNumberList.Remove(queueForDeletion);
                                }
                            }
                            else if (placed)
                            {
                                List<Tuple<int, bool>> addedNumberList = addedNumbers.GetValue(roleListItem);
                                Tuple<int, bool> queueForDeletion = null;
                                foreach (Tuple<int, bool> tuple in addedNumberList)
                                    if (tuple.Item1 == i)
                                        queueForDeletion = tuple;
                                if (queueForDeletion != null)
                                    addedNumberList.Remove(queueForDeletion);
                            }
                            else if (kvp.Key > 1)
                            {
                                RoleBucket roleBucket1 = roleListItem.role.GetRoleBucket();
                                RoleBucket roleBucket2 = roleListItem.role2.GetRoleBucket();
                                if (roleBucket1.roles.Contains(roleFactionData.Item1) || roleBucket2.roles.Contains(roleFactionData.Item1) || roleListItem.role == roleFactionData.Item1 || roleListItem.role2 == roleFactionData.Item1 || IsBucketInBucket(roleListItem.role, roleFactionData.Item1) || IsBucketInBucket(roleListItem.role2, roleFactionData.Item1) || IsTGTETPow(roleListItem.role, roleListItem.role2, roleFactionData.Item1) || roleFactionData.Item1 > (Role)249 && IsHorsemanInAcolyte(roleListItem.role, roleListItem.role2, roleFactionData.Item1))
                                {
                                    lowestFit = kvp.Key;
                                    List<Tuple<int, bool>> addedNumberList = addedNumbers.GetValue(roleListItem);
                                    if (addedNumberList.Count == 0)
                                    {
                                        placed = true;
                                        endCategory = kvp.Key;
                                        addedNumberList.Add(confirmedData);
                                    }
                                    else
                                    {
                                        Tuple<int, bool> queueForDeletion = null;
                                        foreach (Tuple<int, bool> tuple in addedNumberList)
                                            if (tuple.Equals(confirmedData))
                                            {
                                                placed = true;
                                                endCategory = kvp.Key;
                                                break;
                                            }
                                            else if (tuple.Item2 == false && confirmedData.Item2 == true)
                                            {
                                                placed = true;
                                                endCategory = kvp.Key;
                                                markedForReplacement.Add(tuple.Item1);
                                                queueForDeletion = tuple;
                                                addedNumberList.Add(confirmedData);
                                                break;
                                            }
                                            else if (tuple.Item1 == i)
                                            {
                                                queueForDeletion = tuple;
                                                if (addedNumberList.Count - 1 == 0)
                                                {
                                                    placed = true;
                                                    endCategory = kvp.Key;
                                                    addedNumberList.Add(confirmedData);
                                                }
                                                break;
                                            }
                                        if (queueForDeletion != null)
                                            addedNumberList.Remove(queueForDeletion);
                                    }
                                }
                            }
                        }
                if (!placed && lowestFit != 6)
                {
                    List<RoleListItem> reversedList = new List<RoleListItem>(categorizedRoleListItems.GetValue(lowestFit));
                    reversedList.Reverse();
                    foreach (RoleListItem roleListItem in reversedList)
                    {
                        RoleBucket roleBucket1 = roleListItem.role.GetRoleBucket();
                        RoleBucket roleBucket2 = roleListItem.role2.GetRoleBucket();
                        if (roleBucket1.roles.Contains(roleFactionData.Item1) || roleBucket2.roles.Contains(roleFactionData.Item1) || roleListItem.role == roleFactionData.Item1 || roleListItem.role2 == roleFactionData.Item1 || IsBucketInBucket(roleListItem.role, roleFactionData.Item1) || IsBucketInBucket(roleListItem.role2, roleFactionData.Item1) || IsTGTETPow(roleListItem.role, roleListItem.role2, roleFactionData.Item1) || roleFactionData.Item1 > (Role)249 && IsHorsemanInAcolyte(roleListItem.role, roleListItem.role2, roleFactionData.Item1))
                        {
                            placed = true;
                            endCategory = lowestFit;
                            addedNumbers.GetValue(roleListItem).Add(confirmedData);
                            break;
                        }
                    }
                }
                List<Tuple<int, bool>> theQueueForDeletion = new List<Tuple<int, bool>>();
                bool foundOne = false;
                foreach (KeyValuePair<int, List<RoleListItem>> kvp in categorizedRoleListItems)
                    foreach (RoleListItem roleListItem in kvp.Value)
                    {
                        List<Tuple<int, bool>> addedNumberList = addedNumbers.GetValue(roleListItem);
                        foreach (Tuple<int, bool> tuple in addedNumberList)
                            if (tuple.Item1 == i && tuple.Item2 != confirmedData.Item2)
                                theQueueForDeletion.Add(tuple);
                            else if (tuple.Equals(confirmedData))
                            {
                                RoleBucket roleBucket1 = roleListItem.role.GetRoleBucket();
                                RoleBucket roleBucket2 = roleListItem.role2.GetRoleBucket();
                                if (foundOne || !(roleBucket1.roles.Contains(roleFactionData.Item1) || roleBucket2.roles.Contains(roleFactionData.Item1) || roleListItem.role == roleFactionData.Item1 || roleListItem.role2 == roleFactionData.Item1 || IsBucketInBucket(roleListItem.role, roleFactionData.Item1) || IsBucketInBucket(roleListItem.role2, roleFactionData.Item1) || IsTGTETPow(roleListItem.role, roleListItem.role2, roleFactionData.Item1) || roleFactionData.Item1 > (Role)249 && IsHorsemanInAcolyte(roleListItem.role, roleListItem.role2, roleFactionData.Item1)))
                                    theQueueForDeletion.Add(tuple);
                                else
                                    foundOne = true;
                            }
                        if (theQueueForDeletion.Count > 0)
                        {
                            foreach (Tuple<int, bool> queuedForDeletion in theQueueForDeletion)
                                addedNumberList.Remove(queuedForDeletion);
                            theQueueForDeletion.Clear();
                        }
                    }
            }
            ready = true;
            foreach (RoleListItem roleListItem in allRoleListItems)
                roleListItem.RefreshLabel();
        }
        public static bool IsHorsemanInAcolyte(Role role1, Role role2, Role horseman)
        {
            if (horseman == Role.PESTILENCE)
                return role1 == Role.PLAGUEBEARER || role2 == Role.PLAGUEBEARER;
            if (horseman == Role.FAMINE)
                return role1 == Role.BAKER || role2 == Role.BAKER;
            if (horseman == Role.WAR)
                return role1 == Role.BERSERKER || role2 == Role.BERSERKER;
            if (horseman == Role.DEATH)
                if (isBtos)
                    return role1 == Btos2Role.SoulCollector || role2 == Btos2Role.SoulCollector || role1 == Btos2Role.Warlock || role2 == Btos2Role.Warlock;
                else
                    return role1 == Role.SOULCOLLECTOR || role2 == Role.SOULCOLLECTOR;
            return false;
        }
        public static bool IsBucketInBucket(Role bucket1, Role bucket2)
        {
            if (!isBtos)
            {
                if (bucket1 == Role.COMMON_TOWN)
                    return bucket2 == Role.TOWN_INVESTIGATIVE || bucket2 == Role.TOWN_PROTECTIVE || bucket2 == Role.TOWN_SUPPORT || bucket2 == Role.TOWN_KILLING;
                else if (bucket1 == Role.RANDOM_TOWN)
                    return bucket2 == Role.TOWN_INVESTIGATIVE || bucket2 == Role.TOWN_PROTECTIVE || bucket2 == Role.TOWN_SUPPORT || bucket2 == Role.TOWN_KILLING || bucket2 == Role.TOWN_POWER || bucket2 == Role.COMMON_TOWN;
                else if (bucket1 == Role.COMMON_COVEN)
                    return bucket2 == Role.COVEN_DECEPTION || bucket2 == Role.COVEN_UTILITY;
                else if (bucket1 == Role.RANDOM_COVEN)
                    return bucket2 == Role.COVEN_DECEPTION || bucket2 == Role.COVEN_UTILITY || bucket2 == Role.COVEN_KILLING || bucket2 == Role.COVEN_POWER || bucket2 == Role.COMMON_COVEN;
                else if (bucket1 == Role.RANDOM_NEUTRAL)
                    return bucket2 == Role.NEUTRAL_EVIL || bucket2 == Role.NEUTRAL_KILLING || bucket2 == Role.NEUTRAL_APOCALYPSE;
                else if (bucket1 == Role.ANY)
                    return bucket2.IsBucket();
            }
            else if (bucket1 == Btos2Role.CommonTown)
                return bucket2 == Btos2Role.TownInvestigative || bucket2 == Btos2Role.TownProtective || bucket2 == Btos2Role.TownSupport || bucket2 == Btos2Role.TownKilling;
            else if (bucket1 == Btos2Role.RandomTown)
                return bucket2 == Btos2Role.TownInvestigative || bucket2 == Btos2Role.TownProtective || bucket2 == Btos2Role.TownSupport || bucket2 == Btos2Role.TownKilling || bucket2 == Btos2Role.TownGovernment || bucket2 == Btos2Role.TownExecutive || bucket2 == Btos2Role.TownPower || bucket2 == Btos2Role.CommonTown;
            else if (bucket1 == Btos2Role.CommonCoven)
                return bucket2 == Btos2Role.CovenDeception || bucket2 == Btos2Role.CovenUtility;
            else if (bucket1 == Btos2Role.RandomCoven)
                return bucket2 == Btos2Role.CovenDeception || bucket2 == Btos2Role.CovenUtility || bucket2 == Btos2Role.CovenKilling || bucket2 == Btos2Role.CovenPower || bucket2 == Btos2Role.CommonCoven;
            else if (bucket1 == Btos2Role.RandomNeutral)
                return bucket2 == Btos2Role.NeutralEvil || bucket2 == Btos2Role.NeutralKilling || bucket2 == Btos2Role.NeutralPariah || bucket2 == Btos2Role.NeutralOutlier;
            else if (bucket1 == Btos2Role.Any)
                return bucket2.IsBucket();
            return false;
        }
        public static bool IsTGTETPow(Role bucket1, Role bucket2, Role bucket3) => Utils.IsBTOS2() && bucket1 == Btos2Role.TownExecutive && bucket2 == Btos2Role.TownGovernment && bucket3 == Btos2Role.TownPower;

        public static List<RoleListItem> allRoleListItems = new List<RoleListItem>();
        public static Dictionary<int, List<RoleListItem>> categorizedRoleListItems;
        public static Dictionary<RoleListItem, List<Tuple<int, bool>>> addedNumbers;
        public static HudRoleListPanel instance;
        public static bool isBtos = false;
        public static bool ready = false;
    }

    [HarmonyPatch(typeof(RoleListItem), "RefreshLabel")]
    public class TextUpdater
    {
        [HarmonyPostfix]
        public static void Postfix(RoleListItem __instance)
        {
            if (!ClaimspaceVisualizer.ready || ClaimspaceVisualizer.instance == null || ClaimspaceVisualizer.instance.panel == null || !ClaimspaceVisualizer.instance.panel.activeSelf || ClaimspaceVisualizer.instance.isShowingGameModifiers)
                return;
            List<Tuple<int, bool>> addedNumberList = ClaimspaceVisualizer.addedNumbers.GetValue(__instance);
            string addText = "";
            if (addedNumberList.Count > 0)
            {
                addText = " <size=80%>";
                foreach (Tuple<int, bool> tuple in addedNumberList.OrderByDescending(tuple => tuple.Item2).ThenBy(tuple => tuple.Item1))
                {
                    addText += $"<sprite=\"PlayerNumbers\" name=\"PlayerNumbers_{tuple.Item1 + 1}\" color=#{(tuple.Item2 ? "FFFFFF" : "606060")}>";
                }
                addText += "</size>";
            }
            string finalText;
            if ((bool)Settings.SettingsCache.GetValue("Shorten Role Names to Fit Numbers") && addedNumberList.Count > 2 && __instance.role2 == Role.NONE)
                finalText = __instance.role.GetTMPSprite() + __instance.role.ToColorizedShortenedDisplayString() + addText;
            else
                finalText = __instance.roleLabel.text + addText;
            __instance.roleLabel.text = finalText;
        }
    }
}
