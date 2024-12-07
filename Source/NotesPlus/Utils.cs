using System;
using BetterTOS2;
using SML;

namespace NotesPlus
{
	// Token: 0x02000006 RID: 6
	public static class Utils
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002193 File Offset: 0x00000393
		public static bool BTOS2Exists()
		{
			return ModStates.IsEnabled("curtis.tuba.better.tos2");
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003200 File Offset: 0x00001400
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

		// Token: 0x0600001F RID: 31 RVA: 0x0000219F File Offset: 0x0000039F
		private static bool IsBTOS2Bypass()
		{
			return Utils.BTOS2Exists() && BTOSInfo.IS_MODDED;
		}
	}
}
