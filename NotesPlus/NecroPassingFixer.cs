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
		// Token: 0x06000028 RID: 40 RVA: 0x00002211 File Offset: 0x00000411
		[HarmonyPostfix]
		public static void Postfix(NecroPassingElementsPanel __instance)
		{
			NecroPassingFixer.NecroPassingPanel = __instance;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003EF4 File Offset: 0x000020F4
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

		// Token: 0x04000011 RID: 17
		public static NecroPassingElementsPanel NecroPassingPanel;
	}
}
