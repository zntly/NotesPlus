using BetterTOS2;
using HarmonyLib;
using Home.Shared;
using Server.Shared.State;
using Services;
using SML;
using System;
using UnityEngine;

namespace NotesPlus
{
	// Token: 0x02000006 RID: 6
	public static class Utils
	{
		// Token: 0x06000028 RID: 40
		public static bool BTOS2Exists()
		{
			return ModStates.IsEnabled("curtis.tuba.better.tos2");
		}

		// Token: 0x06000029 RID: 41
		public static bool IsBTOS2()
		{
			bool result;
			try
			{
				result = Utils.IsBTOS2Bypass();
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600002A RID: 42
		private static bool IsBTOS2Bypass()
		{
			return Utils.BTOS2Exists() && BTOSInfo.IS_MODDED;
		}

		// Token: 0x0600002B RID: 43
		public static bool IsPandora()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Btos2Role.PandorasBox);
		}

		// Token: 0x0600002C RID: 44
		public static bool IsCompliance()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Btos2Role.CompliantKillers);
		}

		// Token: 0x0600002D RID: 45
		public static bool IsCTT()
		{
			return Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.TOWN_TRAITOR);
		}

		// Token: 0x0600002E RID: 46
		public static bool IsATT()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Btos2Role.ApocTownTraitor);
		}

		// Token: 0x0600003F RID: 63
		public static string FancyUIString(Role role, FactionType factionType) => FancyUI.Utils.ToRoleFactionDisplayString(role, factionType);

		public static string FancyUIFactionColorString(FactionType factionType, Role role)
		{
			if (factionType == Btos2Faction.Jackal)
				role = Btos2Role.Jackal;
			if (factionType == FactionType.VAMPIRE)
				role = Role.VAMPIRE;
			Gradient grad = FancyUI.Gradients.GetChangedGradient(factionType, role);
			string nstr = "#" + ColorUtility.ToHtmlStringRGB(grad.Evaluate(0f));
			if (factionType == Btos2Faction.Pandora && nstr == "#B545FF")
				nstr = "#DA22A6";
			if (factionType == Btos2Faction.Egotist && nstr == "#359F3F")
				nstr = "#3F359F";
			if (factionType == FactionType.CURSED_SOUL && (nstr == "#B545FF" || nstr == "#B54FFF" || nstr == "#B24CFF"))
				nstr = "#" + ColorUtility.ToHtmlStringRGB(grad.Evaluate(1f));
            return nstr;
		}
		// Token: 0x06000040 RID: 64
		public static string RoleDisplayString(Role role, FactionType factionType)
		{
            if (ModStates.IsEnabled("alchlcsystm.fancy.ui"))
                return Utils.FancyUIString(role, factionType);
			return role.ToDisplayString();
		}

		public static string FactionDisplayColor(this FactionType factionType, Role role)
		{
            if (ModStates.IsEnabled("alchlcsystm.fancy.ui"))
                return Utils.FancyUIFactionColorString(factionType, role);
			if (factionType == Btos2Faction.Pandora)
				return "#DA22A6";
			if (factionType == Btos2Faction.Egotist)
				return "#3F359F";
			if (!Utils.IsBTOS2() && factionType == FactionType.CURSED_SOUL)
				return "#E9957E";
            return factionType.GetFactionColor();
        }
    }
}
