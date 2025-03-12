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
		// Token: 0x06000021 RID: 33 RVA: 0x00002158 File Offset: 0x00000358
		public static bool BTOS2Exists()
		{
			return ModStates.IsEnabled("curtis.tuba.better.tos2");
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00004120 File Offset: 0x00002320
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

		// Token: 0x06000023 RID: 35 RVA: 0x00002164 File Offset: 0x00000364
		private static bool IsBTOS2Bypass()
		{
			return Utils.BTOS2Exists() && BTOSInfo.IS_MODDED;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002174 File Offset: 0x00000374
		public static bool IsPandora()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)222);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000021A7 File Offset: 0x000003A7
		public static bool IsCompliance()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)221);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000021DA File Offset: 0x000003DA
		public static bool IsCTT()
		{
			return Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.TOWN_TRAITOR);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002204 File Offset: 0x00000404
		public static bool IsATT()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.FOUR_HORSEMEN);
		}
	}
}
