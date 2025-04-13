using System;
using SML;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Server.Shared.Extensions;

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
			AssetBundle assetBundleFromResources = FromAssetBundle.GetAssetBundleFromResources("NotesPlus.resources.assetbundles.notes.plus", Assembly.GetExecutingAssembly());
			assetBundleFromResources.LoadAllAssets<Texture2D>().ForEach(delegate (Texture2D s)
			{
				Main.Textures.Add(s.name, s);
			});
			if (assetBundleFromResources != null)
			{
				assetBundleFromResources.Unload(false);
			}
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
			} catch
            {
				Console.WriteLine("damn your plague is so strong you gave the mod a bug, contact synapsium");
			}
		}

		public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
	}
}
