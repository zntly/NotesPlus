﻿using System;
using System.Collections.Generic;
using SML;

namespace NotesPlus
{
	// Token: 0x02000003 RID: 3
	[DynamicSettings]
	public class Settings
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2
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

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4
		public ModSettings.CheckboxSetting DidUpdateTraitorDetections
		{
			get
			{
				return new ModSettings.CheckboxSetting
				{
					Name = "Did Update Traitor Detections",
					Description = "This is an internal setting used to tell if the player has already had their old setting migrated to the new one (Traitor Detections -> Faction Abbreviations).",
					DefaultValue = false,
					AvailableInGame = false,
					Available = false
				};
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600004E RID: 78
		public ModSettings.CheckboxSetting AdditionalNotes
		{
			get
			{
				return new ModSettings.CheckboxSetting
				{
					Name = "Additional Notes",
					Description = "Have additional notes appear next to a person's name besides just a role - encase what you would like to show in [single brackets]",
					DefaultValue = true,
					AvailableInGame = false,
					Available = true
				};
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000053 RID: 83
		public ModSettings.ColorPickerSetting AdditionalNotesColor
		{
			get
			{
				return new ModSettings.ColorPickerSetting
				{
					Name = "Additional Notes Color",
					Description = "The color of additional notes",
					DefaultValue = "#FFFFFF",
					AvailableInGame = false,
					Available = ModSettings.GetBool("Additional Notes")
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
