using System;
using SML;

namespace NotesPlus
{
	// Token: 0x02000004 RID: 4
	[Mod.SalemMod]
	public class Main
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000022BC File Offset: 0x000004BC
		public void Start()
		{
			Console.WriteLine("anyone can be an enchanter if they try hard enough");
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
		}
	}
}
