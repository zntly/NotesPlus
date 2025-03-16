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
		// Token: 0x0600002E RID: 46 RVA: 0x00002349 File Offset: 0x00000549
		[HarmonyPostfix]
		public static void Postfix(NecroPassingElementsPanel __instance)
		{
			NecroPassingFixer.NecroPassingPanel = __instance;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000047A8 File Offset: 0x000029A8
		public static void RemoveFromNecroPassing(int pos)
		{
			if (NecroPassingFixer.NecroPassingPanel.voteEntries.Any((NecroPassingVoteEntry v) => v.Position == pos))
			{
				NecroPassingVoteEntry necroPassingVoteEntry = NecroPassingFixer.NecroPassingPanel.voteEntries.Find((NecroPassingVoteEntry v) => v.Position == pos);
				NecroPassingFixer.NecroPassingPanel.voteEntries.Remove(necroPassingVoteEntry);
				UnityEngine.Object.Destroy(necroPassingVoteEntry);
				from v in NecroPassingFixer.NecroPassingPanel.voteEntries
				orderby v.Position
				select v;
				for (int i = 0; i < NecroPassingFixer.NecroPassingPanel.voteEntries.Count; i++)
				{
					NecroPassingFixer.NecroPassingPanel.voteEntries[i].transform.SetSiblingIndex(i);
				}
			}
		}

		// Token: 0x04000012 RID: 18
		public static NecroPassingElementsPanel NecroPassingPanel;
	}
}
