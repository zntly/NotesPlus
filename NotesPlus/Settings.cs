using System;
using System.Collections.Generic;
using SML;
using UnityEngine;
using Server.Shared.Extensions;

namespace NotesPlus
{
	// Token: 0x02000003 RID: 3
	[DynamicSettings]
	public class Settings
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public ModSettings.CheckboxSetting AlignmentAbbreviations
		{
			get
			{
				return new ModSettings.CheckboxSetting
				{
					Name = "Alignment Abbreviations",
					Description = "Detect abbreviations for alignments (e.g. TK, CK, NE, TPow, etc) does not work on roles",
					DefaultValue = true,
					AvailableInGame = false,
					Available = true,
					OnChanged = delegate (bool s)
					{
						Settings.SettingsCache.SetValue("Alignment Abbreviations", s);
					}
				};
			}
		}
		public ModSettings.CheckboxSetting FactionAbbreviations
		{
			get
			{
				return new ModSettings.CheckboxSetting
				{
					Name = "Faction Abbreviations",
					Description = "Allows you to type common faction terms (Townie, Cov, CS, aliases of listed and more) or Traitor terms (TT, CTT, ATT, Rec, Ego, Converted, aliases of listed) to quickly change the shown faction of a role",
					DefaultValue = true,
					AvailableInGame = false,
					Available = true,
					OnChanged = delegate (bool s)
					{
						Settings.SettingsCache.SetValue("Faction Abbreviations", s);
					}
				};
			}
		}
		public ModSettings.CheckboxSetting OnlyDetectMarked
		{
			get
			{
				return new ModSettings.CheckboxSetting
				{
					Name = "Only Detect Marked",
					Description = "Only detects roles from players you mark (you can mark a player by putting an asterisk (*) in their textbox)",
					DefaultValue = false,
					AvailableInGame = false,
					Available = true,
					OnChanged = delegate (bool s)
					{
						Settings.SettingsCache.SetValue("Only Detect Marked", s);
					}
				};
			}
		}
		public ModSettings.DropdownSetting SetFactionSettings
		{
			get
			{
				return new ModSettings.DropdownSetting
				{
					Name = "Show Faction Color",
					Description = "When will a role show its faction color (if it shouldn't, it will show as white)\nAlways - Roles will always use a faction color\nOnly Marked - Roles will only use their faction color if you mark them with an asterisk (*)\nOnly Roles - Roles will use their faction color, alignments will not\nOnly On Override - Roles will only use a faction color if a faction override is specified (e.g. [[#24,2]] for Coven-Vigilante)\nNever - Roles will always be white and never use a faction color",
					Options = this.FactionSettings,
					AvailableInGame = false,
					Available = true,
					OnChanged = delegate (string s)
					{
						Settings.SettingsCache.SetValue("Show Faction Color", s);
					}
				};
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000208F File Offset: 0x0000028F
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020C1 File Offset: 0x000002C1
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
					Available = true,
					OnChanged = delegate (bool s)
					{
						Settings.SettingsCache.SetValue("Additional Notes", s);
					}
				};
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020F3 File Offset: 0x000002F3
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
					Available = (bool)Settings.SettingsCache.GetValue("Additional Notes"),
					OnChanged = delegate (Color s)
					{
						Settings.SettingsCache.SetValue("Additional Notes Color", s);
					}
				};
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002132 File Offset: 0x00000332
		public ModSettings.DropdownSetting AdditionalNotesStyle
		{
			get
			{
				return new ModSettings.DropdownSetting
				{
					Name = "Additional Notes Style",
					Description = "The way the additional notes look next to one's name (this does not change how you input additional notes, only how it appears in the playerlist)",
					Options = this.AdditionalNotesStyleList,
					AvailableInGame = true,
					Available = ModSettings.GetBool("Additional Notes"),
					OnChanged = delegate (string s)
                    {
						Settings.SettingsCache.SetValue("Additional Notes Style", s);
                    }
				};
			}
		}

		public ModSettings.DropdownSetting CopyToClipboardMode
		{
			get
			{
				return new ModSettings.DropdownSetting
				{
					Name = "Copy to Clipboard Mode",
					Description = "How the Copy to Clipboard button will separate players",
					Options = this.CopyToClipboardModes,
					AvailableInGame = true,
					Available = true,
					OnChanged = delegate (string s)
					{
						Settings.SettingsCache.SetValue("Copy to Clipboard Mode", s);
					}
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

		// Token: 0x04000002 RID: 2
		private readonly List<string> AdditionalNotesStyleList = new List<string>(5)
		{
			"(Note)",
			"[Note]",
			"{Note}",
			"- Note",
			"Note"
		};

		private readonly List<string> CopyToClipboardModes = new List<string>(2)
		{
			"Newlines",
			"Spaces"
		};

		public static Dictionary<string, object> SettingsCache = new Dictionary<string, object>()
		{
			{
				"Alignment Abbreviations",
				true
			},
			{
				"Faction Abbreviations",
				true
			},
			{
				"Only Detect Marked",
				false
			},
			{
				"Show Faction Color",
				"Always"
			},
			{
				"Additional Notes",
				true
			},
            {
				"Additional Notes Style",
				"(Note)"
            },
            {
				"Copy to Clipboard Mode",
				"Newlines"
            },
			{
				"Additional Notes Color",
				Color.white
            }
		};
	}
}
