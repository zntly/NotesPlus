using Game.Interface;
using HarmonyLib;
using Server.Shared.State;
using Services;
using System;
using System.Collections.Generic;

namespace NotesPlus
{
    [HarmonyPatch(typeof(TosAbilityPanel), "FilterList")]
    public class FilterByFactionFixer
    {
        public static void Postfix(TosAbilityPanel __instance)
        {
            if (__instance.selectedFilter == TosAbilityPanel.FilterType.SHOW_FACTION)
            {
                using (List<TosAbilityPanelListItem>.Enumerator enumerator3 = __instance.playerListPlayers.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                        if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().TryGetValue(enumerator3.Current.characterPosition, out Tuple<Role, FactionType> roleFactionTuple))
                            enumerator3.Current.gameObject.SetActive(__instance.IsRoleKnown(roleFactionTuple.Item1) && Pepper.GetMyFaction() == roleFactionTuple.Item2);
                }
            }
        }
    }
}
