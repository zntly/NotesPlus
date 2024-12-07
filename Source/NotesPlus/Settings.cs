using System;
using System.Collections.Generic;
using SML;

namespace NotesPlus
{
	// Token: 0x02000003 RID: 3
	[DynamicSettings]
	public class Settings
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public ModSettings.DropdownSetting SetFactionSettings
		{
			get
			{
				return new ModSettings.DropdownSetting
				{
					Name = "Show Faction Color",
					Description = "When will a role show its faction color (if it shouldn't, it will show as grey)",
					Options = this.FactionSettings,
					AvailableInGame = false,
					Available = true
				};
			}
		}

		// Token: 0x04000001 RID: 1
		private readonly List<string> FactionSettings = new List<string>(4)
		{
			"Always",
			"Only Marked",
			"Only Roles",
			"Never"
		};
	}
}
