using System;
using BetterTOS2;
using Home.Shared;
using MiscRoleCustomisation;
using FancyUI;
using Server.Shared.State;
using Services;
using SML;

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
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)222);
		}

		// Token: 0x0600002C RID: 44
		public static bool IsCompliance()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)221);
		}

		// Token: 0x0600002D RID: 45
		public static bool IsCTT()
		{
			return Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.TOWN_TRAITOR);
		}

		// Token: 0x0600002E RID: 46
		public static bool IsATT()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.FOUR_HORSEMEN);
		}

		// Token: 0x0600003F RID: 63
		public static string MRCString(Role role, FactionType factionType)
		{
			if (MiscRoleCustomisation.Main.FactionSpecificRoleNames)
			{
				return MiscRoleCustomisation.Utils.ToRoleFactionDisplayString(role, factionType);
			}
			return role.ToDisplayString();
		}
		public static string FancyUIString(Role role, FactionType factionType) => FancyUI.Utils.ToRoleFactionDisplayString(role, factionType);
		// Token: 0x06000040 RID: 64
		public static string RoleDisplayString(Role role, FactionType factionType)
		{
            if (ModStates.IsEnabled("alchlcsystm.fancy.ui"))
            {
                return Utils.FancyUIString(role, factionType);
            }
            if (ModStates.IsEnabled("det.rolecustomizationmod"))
			{
				return Utils.MRCString(role, factionType);
			}
			return role.ToDisplayString();
		}
	}
}
