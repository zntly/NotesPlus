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
					Description = "When will a role show its faction color (if it shouldn't, it will show as gray)\nAlways - Roles will always use a faction color\nOnly Marked - Roles will only use their faction color if you mark them with an asterisk (*)\nOnly Roles - Roles will use their faction color, alignments will not\nOnly On Override - Roles will only use a faction color if a faction override is specified (e.g. [[#24,2]] for Coven-Vigilante)\nNever - Roles will always be gray and never use a faction color",
					Options = this.FactionSettings,
					AvailableInGame = false,
					Available = true
				};
			}
		}

		// Token: 0x04000001 RID: 1
		private readonly List<string> FactionSettings = new List<string>(5)
		{
			"Always",
			"Only Marked",
			"Only Roles",
			"Only On Override",
			"Never"
		};
	}
}
