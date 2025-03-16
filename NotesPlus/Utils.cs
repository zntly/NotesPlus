using System;
using BetterTOS2;
using Server.Shared.State;
using Services;
using SML;

namespace NotesPlus
{
	// Token: 0x02000006 RID: 6
	public static class Utils
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000226A File Offset: 0x0000046A
		public static bool BTOS2Exists()
		{
			return ModStates.IsEnabled("curtis.tuba.better.tos2");
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000477C File Offset: 0x0000297C
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

		// Token: 0x06000028 RID: 40 RVA: 0x00002276 File Offset: 0x00000476
		private static bool IsBTOS2Bypass()
		{
			return Utils.BTOS2Exists() && BTOSInfo.IS_MODDED;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002286 File Offset: 0x00000486
		public static bool IsPandora()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)222);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000022B9 File Offset: 0x000004B9
		public static bool IsCompliance()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)221);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000022EC File Offset: 0x000004EC
		public static bool IsCTT()
		{
			return Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.TOWN_TRAITOR);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002316 File Offset: 0x00000516
		public static bool IsATT()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.FOUR_HORSEMEN);
		}
	}
}
