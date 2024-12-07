using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BMG.UI;
using Game.Simulation;
using HarmonyLib;
using Server.Shared.Extensions;
using Server.Shared.Info;
using Server.Shared.State;
using Services;
using SML;
using UnityEngine;
using UnityEngine.Events;

namespace NotesPlus
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(GameSimulation), "HandleOnGameInfoChanged")]
	public class DoYourThing
	{
		// Token: 0x06000008 RID: 8
		public static BMG_InputField GetInput(int num)
		{
			return GameObject.Find("Hud/NotepadElementsUI(Clone)/MainPanel/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground/Scroll View/Viewport/Content").transform.GetChild(num).gameObject.GetComponentInChildren<BMG_InputField>();
		}

		// Token: 0x06000009 RID: 9
		[HarmonyPostfix]
		public static void Postfix(GameInfo gameInfo)
		{
			if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION)
			{
				for (int i = 1; i < 16; i++)
				{
					try
					{
						BMG_InputField input = DoYourThing.GetInput(i);
						input.characterLimit = 99999;
						switch (i)
						{
						case 1:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing1));
							break;
						case 2:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing2));
							break;
						case 3:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing3));
							break;
						case 4:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing4));
							break;
						case 5:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing5));
							break;
						case 6:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing6));
							break;
						case 7:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing7));
							break;
						case 8:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing8));
							break;
						case 9:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing9));
							break;
						case 10:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing10));
							break;
						case 11:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing11));
							break;
						case 12:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing12));
							break;
						case 13:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing13));
							break;
						case 14:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing14));
							break;
						case 15:
							input.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing15));
							break;
						}
					}
					catch
					{
						i = 16;
					}
				}
				DoYourThing.ourknown = new StateProperty<Dictionary<int, Tuple<Role, FactionType>>>(new Dictionary<int, Tuple<Role, FactionType>>());
				DoYourThing.lockedplayers = new Dictionary<int, bool>();
				foreach (KeyValuePair<int, Tuple<Role, FactionType>> keyValuePair in Service.Game.Sim.simulation.knownRolesAndFactions.Get())
				{
					DoYourThing.ourknown.Get().SetValue(keyValuePair.Key, keyValuePair.Value);
					DoYourThing.lockedplayers.TryAdd(keyValuePair.Key, true);
				}
				for (int j = 0; j < 15; j++)
				{
					DoYourThing.ourknown.Get().TryAdd(j, new Tuple<Role, FactionType>(Role.NONE, FactionType.NONE));
					DoYourThing.lockedplayers.TryAdd(j, false);
				}
				StateProperty<Dictionary<int, Tuple<Role, FactionType>>> knownRolesAndFactions = Service.Game.Sim.simulation.knownRolesAndFactions;
				knownRolesAndFactions.OnChanged = (Action<Dictionary<int, Tuple<Role, FactionType>>>)Delegate.Combine(knownRolesAndFactions.OnChanged, new Action<Dictionary<int, Tuple<Role, FactionType>>>(DoYourThing.DetectChanges));
			}
		}

		// Token: 0x0600000A RID: 10
		public static void DetectChanges(Dictionary<int, Tuple<Role, FactionType>> data)
		{
			for (int i = 0; i < 15; i++)
			{
				try
				{
					if (Service.Game.Sim.simulation.knownRolesAndFactions.Data.ContainsKey(i) && (Service.Game.Sim.simulation.knownRolesAndFactions.Data[i].Item1 != DoYourThing.ourknown.Data[i].Item1 || Service.Game.Sim.simulation.knownRolesAndFactions.Data[i].Item2 != DoYourThing.ourknown.Data[i].Item2))
					{
						DoYourThing.lockedplayers[i] = true;
					}
				}
				catch
				{
					DoYourThing.lockedplayers.TryAdd(i, true);
				}
			}
		}

		// Token: 0x0600000C RID: 12
		public static void DoingTheThing1(string str)
		{
			DoYourThing.TheGoodStuff(str, 0);
		}

		// Token: 0x0600000D RID: 13
		public static Tuple<Role, FactionType> AlignmentToFaction(string align)
		{
			string a = align.ToLower();
			if (!Utils.IsBTOS2())
			{
				if (a == "ti")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_INVESTIGATIVE, FactionType.TOWN);
				}
				if (a == "tp")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_PROTECTIVE, FactionType.TOWN);
				}
				if (a == "ts")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_SUPPORT, FactionType.TOWN);
				}
				if (a == "tpow" || a == "tpower")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_POWER, FactionType.TOWN);
				}
				if (a == "tk")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_KILLING, FactionType.TOWN);
				}
				if (a == "rt" || a == "town" || a == "townie")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_TOWN, FactionType.TOWN);
				}
				if (a == "ct")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_TOWN, FactionType.TOWN);
				}
				if (a == "cd")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_DECEPTION, FactionType.COVEN);
				}
				if (a == "ck")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_KILLING, FactionType.COVEN);
				}
				if (a == "cp" || a == "cpow" || a == "cpower")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_POWER, FactionType.COVEN);
				}
				if (a == "cu")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_UTILITY, FactionType.COVEN);
				}
				if (a == "rc" || a == "cov" || a == "coven")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_COVEN, FactionType.COVEN);
				}
				if (a == "cc")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_COVEN, FactionType.COVEN);
				}
				if (a == "ne")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_EVIL, FactionType.NONE);
				}
				if (a == "nk")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_KILLING, FactionType.NONE);
				}
				if (a == "rn" || a == "neut" || a == "neutral")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_NEUTRAL, FactionType.NONE);
				}
				if (a == "na" || a == "ra" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_APOCALYPSE, FactionType.APOCALYPSE);
				}
				if (a == "cn")
				{
					return new Tuple<Role, FactionType>((Role)121, FactionType.NONE);
				}
				if (a == "np" || a == "pariah")
				{
					return new Tuple<Role, FactionType>((Role)119, FactionType.NONE);
				}
				if (a == "ns")
				{
					return new Tuple<Role, FactionType>((Role)120, FactionType.NONE);
				}
			}
			else
			{
				if (a == "ti")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_SUPPORT, FactionType.TOWN);
				}
				if (a == "tp")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_COVEN, FactionType.TOWN);
				}
				if (a == "ts")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_UTILITY, FactionType.TOWN);
				}
				if (a == "tpow" || a == "tpower")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_POWER, FactionType.TOWN);
				}
				if (a == "tk")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_KILLING, FactionType.TOWN);
				}
				if (a == "rt" || a == "town" || a == "townie")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_PROTECTIVE, FactionType.TOWN);
				}
				if (a == "ct")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_KILLING, FactionType.TOWN);
				}
				bool Pandora = Utils.IsPandora();
				FactionType covfac = FactionType.COVEN;
				if (Pandora)
				{
					covfac = (FactionType)43;
				}
				if (a == "cd")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_NEUTRAL, covfac);
				}
				if (a == "ck")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_KILLING, covfac);
				}
				if (a == "cp" || a == "cpow" || a == "cpower")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_EVIL, covfac);
				}
				if (a == "cu")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_APOCALYPSE, covfac);
				}
				if (a == "rc" || a == "cov" || a == "coven")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_DECEPTION, covfac);
				}
				if (a == "cc")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_POWER, covfac);
				}
				if (a == "ne")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_COVEN, FactionType.NONE);
				}
				bool flag = Utils.IsCompliance();
				FactionType nkfac = FactionType.NONE;
				if (flag)
				{
					nkfac = (FactionType)44;
				}
				if (a == "nk")
				{
					return new Tuple<Role, FactionType>((Role)118, nkfac);
				}
				if (a == "rn" || a == "neut" || a == "neutral")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_TOWN, FactionType.NONE);
				}
				FactionType apocfac = FactionType.APOCALYPSE;
				if (Pandora)
				{
					apocfac = (FactionType)43;
				}
				if (a == "na" || a == "ra" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")
				{
					return new Tuple<Role, FactionType>(Role.ANY, apocfac);
				}
				if (a == "cn")
				{
					return new Tuple<Role, FactionType>((Role)121, FactionType.NONE);
				}
				if (a == "np" || a == "pariah")
				{
					return new Tuple<Role, FactionType>((Role)119, FactionType.NONE);
				}
				if (a == "ns")
				{
					return new Tuple<Role, FactionType>((Role)120, FactionType.NONE);
				}
			}
			return new Tuple<Role, FactionType>(Role.NONE, FactionType.NONE);
		}

		// Token: 0x0600000E RID: 14
		public static void DoingTheThing2(string str)
		{
			DoYourThing.TheGoodStuff(str, 1);
		}

		// Token: 0x0600000F RID: 15
		public static void DoingTheThing15(string str)
		{
			DoYourThing.TheGoodStuff(str, 14);
		}

		// Token: 0x06000010 RID: 16
		public static void DoingTheThing14(string str)
		{
			DoYourThing.TheGoodStuff(str, 13);
		}

		// Token: 0x06000011 RID: 17
		public static void DoingTheThing13(string str)
		{
			DoYourThing.TheGoodStuff(str, 12);
		}

		// Token: 0x06000012 RID: 18
		public static void DoingTheThing12(string str)
		{
			DoYourThing.TheGoodStuff(str, 11);
		}

		// Token: 0x06000013 RID: 19
		public static void DoingTheThing11(string str)
		{
			DoYourThing.TheGoodStuff(str, 10);
		}

		// Token: 0x06000014 RID: 20
		public static void DoingTheThing10(string str)
		{
			DoYourThing.TheGoodStuff(str, 9);
		}

		// Token: 0x06000015 RID: 21
		public static void DoingTheThing9(string str)
		{
			DoYourThing.TheGoodStuff(str, 8);
		}

		// Token: 0x06000016 RID: 22
		public static void DoingTheThing8(string str)
		{
			DoYourThing.TheGoodStuff(str, 7);
		}

		// Token: 0x06000017 RID: 23
		public static void DoingTheThing7(string str)
		{
			DoYourThing.TheGoodStuff(str, 6);
		}

		// Token: 0x06000018 RID: 24
		public static void DoingTheThing6(string str)
		{
			DoYourThing.TheGoodStuff(str, 5);
		}

		// Token: 0x06000019 RID: 25
		public static void DoingTheThing5(string str)
		{
			DoYourThing.TheGoodStuff(str, 4);
		}

		// Token: 0x0600001A RID: 26
		public static void DoingTheThing4(string str)
		{
			DoYourThing.TheGoodStuff(str, 3);
		}

		// Token: 0x0600001B RID: 27
		public static void DoingTheThing3(string str)
		{
			DoYourThing.TheGoodStuff(str, 2);
		}

		// Token: 0x0600001C RID: 28
		public static void TheGoodStuff(string str, int key)
		{
			bool flag;
			DoYourThing.lockedplayers.TryGetValue(key, out flag);
			if (!flag && (!ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") || (ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") && str.IndexOf('*') != -1)))
			{
				Match match = DoYourThing.RoleRegex.Match(str);
				bool flag2 = false;
				if (match.Success)
				{
					string value = match.Value;
					Role role = (Role)int.Parse(DoYourThing.RoleIdRegex.Match(value).Value);
					Match match2 = DoYourThing.FactionIdRegex.Match(value);
					FactionType factionType = FactionType.UNKNOWN;
					if (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Always" || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Marked" && str.IndexOf('*') != -1) || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Roles" && role < Role.RANDOM_TOWN))
					{
						if (match2.Success)
						{
							factionType = (FactionType)int.Parse(match2.Value);
						}
						else if ((!Utils.IsBTOS2() && ((role >= Role.RANDOM_TOWN && role <= Role.TOWN_POWER) || role == Role.COMMON_TOWN)) || (Utils.IsBTOS2() && role >= Role.TOWN_PROTECTIVE && role <= Role.COVEN_UTILITY))
						{
							factionType = FactionType.TOWN;
						}
						else if ((!Utils.IsBTOS2() && ((role >= Role.RANDOM_COVEN && role <= Role.COVEN_POWER) || role == Role.COMMON_COVEN)) || (Utils.IsBTOS2() && role >= Role.COVEN_DECEPTION && role <= Role.NEUTRAL_APOCALYPSE))
						{
							factionType = FactionType.COVEN;
						}
						else if ((!Utils.IsBTOS2() && role >= Role.RANDOM_NEUTRAL && role <= Role.NEUTRAL_EVIL) || (Utils.IsBTOS2() && role >= Role.COMMON_TOWN && role <= (Role)121))
						{
							factionType = FactionType.NONE;
						}
						else if ((Utils.IsBTOS2() && role == Role.ANY) || (!Utils.IsBTOS2() && role == Role.NEUTRAL_APOCALYPSE))
						{
							factionType = FactionType.APOCALYPSE;
						}
						else
						{
							factionType = role.GetFaction();
						}
						if (!match2.Success && Utils.IsBTOS2())
						{
							if ((factionType == FactionType.COVEN || factionType == FactionType.APOCALYPSE) && Utils.IsPandora())
							{
								factionType = (FactionType)43;
							}
							else if ((role == (Role)118 || factionType == FactionType.SERIALKILLER || factionType == FactionType.ARSONIST || factionType == FactionType.WEREWOLF || factionType == FactionType.SHROUD) && Utils.IsCompliance())
							{
								factionType = (FactionType)44;
							}
						}
					}
					bool flag3 = false;
					if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) == null || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item1 != role || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item2 != factionType)
					{
						flag3 = true;
					}
					DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(role, factionType));
					Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(role, factionType));
					if (flag3)
					{
						Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
						return;
					}
				}
				else if (ModSettings.GetBool("Alignment Abbreviations", "synapsium.notes.plus"))
				{
					Match match3;
					if (Utils.IsBTOS2())
					{
						match3 = DoYourThing.AlignmentRegexBTOS.Match(str);
					}
					else
					{
						match3 = DoYourThing.AlignmentRegex.Match(str);
					}
					if (match3.Success)
					{
						flag2 = true;
						Tuple<Role, FactionType> tuple = DoYourThing.AlignmentToFaction(match3.Value);
						Role item = tuple.Item1;
						FactionType factionType2 = FactionType.UNKNOWN;
						if (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Always" || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Marked" && str.IndexOf('*') != -1))
						{
							factionType2 = tuple.Item2;
						}
						bool flag4 = false;
						if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) == null || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item1 != item || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item2 != factionType2)
						{
							flag4 = true;
						}
						DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(item, factionType2));
						Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(item, factionType2));
						if (flag4)
						{
							Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
							return;
						}
					}
				}
				if (!match.Success && !flag2)
				{
					bool flag5 = false;
					if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) != null)
					{
						flag5 = true;
					}
					if (!flag5)
					{
						DoYourThing.ourknown.Get().Remove(key);
						Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
						return;
					}
					DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(Role.UNKNOWN, FactionType.UNKNOWN));
					Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(Role.UNKNOWN, FactionType.UNKNOWN));
					Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
					DoYourThing.ourknown.Get().Remove(key);
					Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
					Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
					return;
				}
			}
			else if (!flag && ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") && str.IndexOf('*') != -1)
			{
				bool flag6 = false;
				if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) != null)
				{
					flag6 = true;
				}
				if (!flag6)
				{
					DoYourThing.ourknown.Get().Remove(key);
					Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
					return;
				}
				DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(Role.UNKNOWN, FactionType.UNKNOWN));
				Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(Role.UNKNOWN, FactionType.UNKNOWN));
				Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
				DoYourThing.ourknown.Get().Remove(key);
				Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
				Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
				return;
			}
		}

		// Token: 0x04000002 RID: 2
		public static StateProperty<Dictionary<int, Tuple<Role, FactionType>>> ourknown;

		// Token: 0x04000003 RID: 3
		public static Regex RoleRegex = new Regex("\\[\\[#\\d+]]|\\[\\[#\\d+,\\d+]]|<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		// Token: 0x04000004 RID: 4
		public static Regex RoleIdRegex = new Regex("\\d+");

		// Token: 0x04000005 RID: 5
		public static Regex FactionIdRegex = new Regex("(?<=,)\\d+");

		// Token: 0x04000006 RID: 6
		public static Regex AlignmentRegex = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tpow|tpower|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cc|cov|coven|rn|neut|neutral)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);

		// Token: 0x04000007 RID: 7
		public static Regex AlignmentRegexBTOS = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tpow|tpower|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cov|cc|coven|rn|neut|neutral|cn|np|ns|pariah)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);

		// Token: 0x04000008 RID: 8
		public static Dictionary<int, bool> lockedplayers;
	}
}
