using HarmonyLib;
using Server.Shared.Extensions;
using SML;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NotesPlus
{
	// Token: 0x02000004 RID: 4
	[Mod.SalemMod]
	public class Main
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002454 File Offset: 0x0000065
		public void Start()
		{
			Console.WriteLine("anyone can be an enchanter if they try hard enough");
			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			copyToClipboard = FromResources.LoadSprite("NotesPlus.resources.images.CopyToClipboard.png", thisAssembly);
            locked = FromResources.LoadSprite("NotesPlus.resources.images.Locked.png", thisAssembly);
            unlocked = FromResources.LoadSprite("NotesPlus.resources.images.Lock.png", thisAssembly);
            try
			{
				bool @bool = ModSettings.GetBool("Traitor Detections", "synapsium.notes.plus");
				if (!ModSettings.GetBool("Did Update Traitor Detections", "synapsium.notes.plus"))
				{
					ModSettings.SetBool("Faction Abbreviations", @bool, "synapsium.notes.plus");
					ModSettings.SetBool("Did Update Traitor Detections", true, "synapsium.notes.plus");
				}
			}
			catch
			{
				Console.WriteLine("maybe you're just better off as a plaguebearer");
			}
			try
            {
				Settings.SettingsCache.SetValue("Alignment Abbreviations", ModSettings.GetBool("Alignment Abbreviations", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Faction Abbreviations", ModSettings.GetBool("Faction Abbreviations", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Only Detect Marked", ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Show Faction Color", ModSettings.GetString("Show Faction Color", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Additional Notes", ModSettings.GetBool("Additional Notes", "synapsium.notes.plus"));
                Settings.SettingsCache.SetValue("Additional Notes Style", ModSettings.GetString("Additional Notes Style", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Copy to Clipboard Mode", ModSettings.GetString("Copy to Clipboard Mode", "synapsium.notes.plus"));
				Settings.SettingsCache.SetValue("Additional Notes Color", ModSettings.GetColor("Additional Notes Color", "synapsium.notes.plus"));
                Settings.SettingsCache.SetValue("Manual Locking/Unlocking", ModSettings.GetBool("Manual Locking/Unlocking", "synapsium.notes.plus"));
                Settings.SettingsCache.SetValue("Claimspace Visualizer", ModSettings.GetBool("Claimspace Visualizer", "synapsium.notes.plus"));
                Settings.SettingsCache.SetValue("Shorten Role Names to Fit Numbers", ModSettings.GetBool("Shorten Role Names to Fit Numbers", "synapsium.notes.plus"));
                Settings.SettingsCache.SetValue("Disable Claimspace Visualizer in All Any", ModSettings.GetBool("Disable Claimspace Visualizer in All Any", "synapsium.notes.plus"));
            } catch
            {
				Console.WriteLine("damn your plague is so strong you gave the mod a bug, contact synapsium");
			}
		}

        public static Sprite copyToClipboard;
		public static Sprite locked;
		public static Sprite unlocked;
	}
}
