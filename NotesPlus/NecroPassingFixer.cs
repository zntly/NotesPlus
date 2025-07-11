using System;
using System.Linq;
using Game.Interface;
using HarmonyLib;
using Server.Shared.Extensions;
using UnityEngine;

namespace NotesPlus
{
	// Token: 0x02000007 RID: 7
	[HarmonyPatch(typeof(NecroPassingElementsPanel), "AddVoteEntry")]
	public class NecroPassingFixer
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00002389 File Offset: 0x00000589
		[HarmonyPostfix]
		public static void Postfix(NecroPassingElementsPanel __instance)
		{
			NecroPassingFixer.NecroPassingPanel = __instance;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000048E8 File Offset: 0x00002AE8
		public static void RemoveFromNecroPassing(int pos)
		{
			if (!NecroPassingFixer.NecroPassingPanel)
				return;
			if (NecroPassingFixer.NecroPassingPanel.voteEntries.Any((NecroPassingVoteEntry v) => v.Position == pos))
			{
				NecroPassingVoteEntry necroPassingVoteEntry = NecroPassingFixer.NecroPassingPanel.voteEntries.Find((NecroPassingVoteEntry v) => v.Position == pos);
				NecroPassingFixer.NecroPassingPanel.voteEntries.Remove(necroPassingVoteEntry);
				UnityEngine.Object.Destroy(necroPassingVoteEntry);
				var a = from v in NecroPassingFixer.NecroPassingPanel.voteEntries
				orderby v.Position
				select v;
				for (int i = 0; i < NecroPassingFixer.NecroPassingPanel.voteEntries.Count; i++)
				{
					NecroPassingFixer.NecroPassingPanel.voteEntries[i].transform.SetSiblingIndex(i);
				}
			}
		}

		// Token: 0x04000013 RID: 19
		public static NecroPassingElementsPanel NecroPassingPanel;
	}
}
