﻿using System;
using BetterTOS2;
using Server.Shared.State;
using Services;
using SML;

namespace NotesPlus
{
	// Token: 0x02000006 RID: 6
	public static class Utils
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002153 File Offset: 0x00000353
		public static bool BTOS2Exists()
		{
			return ModStates.IsEnabled("curtis.tuba.better.tos2");
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003A24 File Offset: 0x00001C24
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

		// Token: 0x06000021 RID: 33 RVA: 0x0000215F File Offset: 0x0000035F
		private static bool IsBTOS2Bypass()
		{
			return Utils.BTOS2Exists() && BTOSInfo.IS_MODDED;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000216F File Offset: 0x0000036F
		public static bool IsPandora()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)222);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000021A2 File Offset: 0x000003A2
		public static bool IsCompliance()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains((Role)221);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000021D5 File Offset: 0x000003D5
		public static bool IsCTT()
		{
			return Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.TOWN_TRAITOR);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000021FF File Offset: 0x000003FF
		public static bool IsATT()
		{
			return Utils.IsBTOS2() && Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards.Contains(Role.FOUR_HORSEMEN);
		}
	}
}
