﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BMG.UI;
using Game.Interface;
using Game.Simulation;
using HarmonyLib;
using Home.Common.Tooltips;
using Mentions.UI;
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
			if (ModStates.IsEnabled("JAN.movablewills") && ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills"))
			{
				return GameObject.Find("Hud/NotepadElementsUI(Clone)/asd/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground/Scroll View/Viewport/Content").transform.GetChild(num + Service.Game.Sim.simulation.validPlayerCount.Get()).gameObject.GetComponentInChildren<BMG_InputField>();
			}
			return GameObject.Find("Hud/NotepadElementsUI(Clone)/MainPanel/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground/Scroll View/Viewport/Content").transform.GetChild(num).gameObject.GetComponentInChildren<BMG_InputField>();
		}

		// Token: 0x06000009 RID: 9
		[HarmonyPostfix]
		public static void Postfix(GameInfo gameInfo)
		{
			if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DAY)
			{
				DoYourThing.JANCanCode = false;
				DoYourThing.alreadydone = new List<int>();
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
						DoYourThing.alreadydone.Add(i);
					}
					catch
					{
						i = 16;
					}
				}
				DoYourThing.ourknown = new StateProperty<Dictionary<int, Tuple<Role, FactionType>>>(new Dictionary<int, Tuple<Role, FactionType>>());
				DoYourThing.lockedplayers = new Dictionary<int, Tuple<Role, FactionType>>();
				foreach (KeyValuePair<int, Tuple<Role, FactionType>> keyValuePair in Service.Game.Sim.simulation.knownRolesAndFactions.Get())
				{
					DoYourThing.ourknown.Get().SetValue(keyValuePair.Key, keyValuePair.Value);
					DoYourThing.lockedplayers.TryAdd(keyValuePair.Key, keyValuePair.Value);
				}
				StateProperty<Dictionary<int, Tuple<Role, FactionType>>> knownRolesAndFactions = Service.Game.Sim.simulation.knownRolesAndFactions;
				knownRolesAndFactions.OnChanged = (Action<Dictionary<int, Tuple<Role, FactionType>>>)Delegate.Combine(knownRolesAndFactions.OnChanged, new Action<Dictionary<int, Tuple<Role, FactionType>>>(DoYourThing.DetectChanges));
				DoYourThing.notepad = UnityEngine.Object.FindAnyObjectByType<NotepadPanel>();
				GameObject gameObject;
				if (ModStates.IsEnabled("JAN.movablewills") && ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills"))
				{
					gameObject = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground");
				}
				else
				{
					gameObject = GameObject.Find("Hud/NotepadElementsUI(Clone)/MainPanel/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground");
				}
				if (DoYourThing.notepad && gameObject)
				{
					DoYourThing.JANCanCode = true;
					DoYourThing.mentionsPanel = DoYourThing.notepad.mentionsPanel;
					DoYourThing.PlayerNotesSendToChat = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.notepad.SendToChatButton, new Vector2(0.3f, -0.5f), Quaternion.identity, gameObject.transform);
					DoYourThing.PlayerNotesSendToChat.transform.localPosition = new Vector3(220f, -365f, 0f);
					try
					{
						TooltipTrigger component = DoYourThing.PlayerNotesSendToChat.GetComponent<TooltipTrigger>();
						if (component != null)
						{
							component.LookupKey = "";
							component.NonLocalizedString = "Send Player Notes to Chat";
						}
					}
					catch
					{
					}
					DoYourThing.PlayerNotesSendToChat.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
					DoYourThing.PlayerNotesSendToChat.onClick.RemoveAllListeners();
					DoYourThing.PlayerNotesSendToChat.onClick.AddListener(new UnityAction(DoYourThing.OnSendToChat));
					return;
				}
			}
			else if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION)
			{
				for (int j = 1; j < 16; j++)
				{
					try
					{
						if (!DoYourThing.alreadydone.Contains(j))
						{
							BMG_InputField input2 = DoYourThing.GetInput(j);
							input2.characterLimit = 99999;
							switch (j)
							{
							case 1:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing1));
								break;
							case 2:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing2));
								break;
							case 3:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing3));
								break;
							case 4:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing4));
								break;
							case 5:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing5));
								break;
							case 6:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing6));
								break;
							case 7:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing7));
								break;
							case 8:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing8));
								break;
							case 9:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing9));
								break;
							case 10:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing10));
								break;
							case 11:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing11));
								break;
							case 12:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing12));
								break;
							case 13:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing13));
								break;
							case 14:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing14));
								break;
							case 15:
								input2.onValueChanged.AddListener(new UnityAction<string>(DoYourThing.DoingTheThing15));
								break;
							}
							DoYourThing.alreadydone.Add(j);
						}
					}
					catch
					{
						j = 16;
					}
				}
				if (ModStates.IsEnabled("JAN.movablewills") && ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills") && !DoYourThing.JANCanCode)
				{
					GameObject gameObject2 = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground");
					if (DoYourThing.notepad && gameObject2)
					{
						DoYourThing.JANCanCode = true;
						DoYourThing.mentionsPanel = DoYourThing.notepad.mentionsPanel;
						DoYourThing.PlayerNotesSendToChat = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.notepad.SendToChatButton, new Vector2(0.3f, -0.5f), Quaternion.identity, gameObject2.transform);
						DoYourThing.PlayerNotesSendToChat.transform.localPosition = new Vector3(220f, -365f, 0f);
						try
						{
							TooltipTrigger component2 = DoYourThing.PlayerNotesSendToChat.GetComponent<TooltipTrigger>();
							if (component2 != null)
							{
								component2.LookupKey = "";
								component2.NonLocalizedString = "Send Player Notes to Chat";
							}
						}
						catch
						{
						}
						DoYourThing.PlayerNotesSendToChat.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
						DoYourThing.PlayerNotesSendToChat.onClick.RemoveAllListeners();
						DoYourThing.PlayerNotesSendToChat.onClick.AddListener(new UnityAction(DoYourThing.OnSendToChat));
						return;
					}
				}
			}
		}

		// Token: 0x0600000A RID: 10
		public static void DetectChanges(Dictionary<int, Tuple<Role, FactionType>> data)
		{
			for (int i = 0; i < 15; i++)
			{
				try
				{
					if ((!DoYourThing.ourknown.Get().ContainsKey(i) && Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i)) || (Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i) && DoYourThing.ourknown.Get().ContainsKey(i) && (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item1 != DoYourThing.ourknown.Get().GetValue(i, null).Item1 || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item2 != DoYourThing.ourknown.Get().GetValue(i, null).Item2)))
					{
						DoYourThing.lockedplayers.SetValue(i, Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null));
					}
				}
				catch
				{
					DoYourThing.lockedplayers.SetValue(i, new Tuple<Role, FactionType>(Role.ONE_TRIAL_PER_DAY, FactionType.CURSED_SOUL));
				}
			}
		}

		// Token: 0x0600000B RID: 11
		static DoYourThing()
		{
			DoYourThing.RoleIdRegex = new Regex("\\d+");
			DoYourThing.FactionIdRegex = new Regex("(?<=,)\\d+");
			DoYourThing.AlignmentRegex = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tpow|tpower|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cc|cov|coven|rn|neut|neutral)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
			DoYourThing.AlignmentRegexBTOS = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tpow|tpower|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cov|cc|coven|rn|neut|neutral|cn|np|ns|pariah)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
			DoYourThing.PostChatRegex = new Regex("<style=Mention>(.*?<color=#......>.*?\\/style>)|<style=Mention>(.*?\\/style>)");
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
				bool flag = Utils.IsPandora();
				FactionType item = FactionType.COVEN;
				if (flag)
				{
					item = (FactionType)43;
				}
				if (a == "cd")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_NEUTRAL, item);
				}
				if (a == "ck")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_KILLING, item);
				}
				if (a == "cp" || a == "cpow" || a == "cpower")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_EVIL, item);
				}
				if (a == "cu")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_APOCALYPSE, item);
				}
				if (a == "rc" || a == "cov" || a == "coven")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_DECEPTION, item);
				}
				if (a == "cc")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_POWER, item);
				}
				if (a == "ne")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_COVEN, FactionType.NONE);
				}
				bool flag2 = Utils.IsCompliance();
				FactionType item2 = FactionType.NONE;
				if (flag2)
				{
					item2 = (FactionType)44;
				}
				if (a == "nk")
				{
					return new Tuple<Role, FactionType>((Role)118, item2);
				}
				if (a == "rn" || a == "neut" || a == "neutral")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_TOWN, FactionType.NONE);
				}
				FactionType item3 = FactionType.APOCALYPSE;
				if (flag)
				{
					item3 = (FactionType)43;
				}
				if (a == "na" || a == "ra" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")
				{
					return new Tuple<Role, FactionType>(Role.ANY, item3);
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
			Tuple<Role, FactionType> tuple;
			DoYourThing.lockedplayers.TryGetValue(key, out tuple);
			if (tuple == null && (!ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") || (ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") && str.IndexOf('*') != -1)))
			{
				Match match = DoYourThing.RoleRegex.Match(str);
				bool flag = false;
				if (match.Success)
				{
					string value = match.Value;
					Role role = (Role)int.Parse(DoYourThing.RoleIdRegex.Match(value).Value);
					Match match2 = DoYourThing.FactionIdRegex.Match(value);
					FactionType factionType = FactionType.UNKNOWN;
					if (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Always" || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Marked" && str.IndexOf('*') != -1) || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Roles" && role < Role.RANDOM_TOWN) || ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only On Override")
					{
						if (match2.Success)
						{
							factionType = (FactionType)int.Parse(match2.Value);
						}
						if (!match2.Success && ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") != "Only On Override")
						{
							bool flag2 = false;
							if (ModSettings.GetBool("Faction Abbreviations", "synapsium.notes.plus"))
							{
								Match match3 = DoYourThing.TraitorRegex.Match(str);
								if (match3.Success)
								{
									flag2 = true;
									factionType = DoYourThing.TraitorFaction(match3.Value);
								}
							}
							if (!flag2)
							{
								if ((!Utils.IsBTOS2() && ((role >= Role.RANDOM_TOWN && role <= Role.TOWN_POWER) || role == Role.COMMON_TOWN)) || (Utils.IsBTOS2() && role >= Role.TOWN_PROTECTIVE && role <= Role.COVEN_UTILITY))
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
								if (Utils.IsBTOS2())
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
					Match match4;
					if (Utils.IsBTOS2())
					{
						match4 = DoYourThing.AlignmentRegexBTOS.Match(str);
					}
					else
					{
						match4 = DoYourThing.AlignmentRegex.Match(str);
					}
					if (match4.Success)
					{
						flag = true;
						Tuple<Role, FactionType> tuple2 = DoYourThing.AlignmentToFaction(match4.Value);
						Role item = tuple2.Item1;
						FactionType factionType2 = FactionType.UNKNOWN;
						if (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Always" || (ModSettings.GetString("Show Faction Color", "synapsium.notes.plus") == "Only Marked" && str.IndexOf('*') != -1))
						{
							bool flag4 = false;
							if (ModSettings.GetBool("Faction Abbreviations", "synapsium.notes.plus"))
							{
								MatchCollection match5 = DoYourThing.TraitorRegex.Matches(str);
								if (match5.Count > 0)
								{
									for (int jj = 0; jj < match5.Count; jj++)
									{
										if (match5[jj].Value != match4.Value)
										{
											flag4 = true;
											factionType2 = DoYourThing.TraitorFaction(match5[jj].Value);
											jj = match5.Count;
										}
									}
								}
							}
							if (!flag4)
							{
								factionType2 = tuple2.Item2;
							}
						}
						bool flag5 = false;
						if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) == null || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item1 != item || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item2 != factionType2)
						{
							flag5 = true;
						}
						DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(item, factionType2));
						Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(item, factionType2));
						if (flag5)
						{
							Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
							return;
						}
					}
				}
				if (tuple == null && !match.Success && !flag && Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) != null)
				{
					try
					{
						if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, new Tuple<Role, FactionType>(Role.NONE, FactionType.NONE)).Item1.IsCovenAligned())
						{
							NecroPassingFixer.RemoveFromNecroPassing(key);
						}
						DoYourThing.ourknown.Get().Remove(key);
						Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
						Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
						DoYourThing.DisableRoleLabel(key);
					}
					catch (Exception message)
					{
						Debug.LogWarning(message);
					}
					return;
				}
			}
			else if (tuple != null)
			{
				Role item2 = tuple.Item1;
				FactionType factionType3 = tuple.Item2;
				bool flag6 = false;
				Match match6 = DoYourThing.RoleRegex.Match(str);
				if (match6.Success)
				{
					Match match7 = DoYourThing.FactionIdRegex.Match(match6.Value);
					if (match7.Success)
					{
						flag6 = true;
						factionType3 = (FactionType)int.Parse(match7.Value);
					}
				}
				if (!flag6 && ModSettings.GetBool("Faction Abbreviations", "synapsium.notes.plus"))
				{
					Match match8 = DoYourThing.TraitorRegex.Match(str);
					if (match8.Success)
					{
						factionType3 = DoYourThing.TraitorFaction(match8.Value);
					}
				}
				bool flag7 = false;
				if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) == null || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item1 != item2 || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null).Item2 != factionType3)
				{
					flag7 = true;
				}
				DoYourThing.ourknown.Get().SetValue(key, new Tuple<Role, FactionType>(item2, factionType3));
				Service.Game.Sim.simulation.knownRolesAndFactions.Get().SetValue(key, new Tuple<Role, FactionType>(item2, factionType3));
				if (flag7)
				{
					Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
					return;
				}
			}
			else if (tuple == null && ModSettings.GetBool("Only Detect Marked", "synapsium.notes.plus") && str.IndexOf('*') != -1 && Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) != null)
			{
				try
				{
					if (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, new Tuple<Role, FactionType>(Role.NONE, FactionType.NONE)).Item1.IsCovenAligned())
					{
						NecroPassingFixer.RemoveFromNecroPassing(key);
					}
					DoYourThing.ourknown.Get().Remove(key);
					Service.Game.Sim.simulation.knownRolesAndFactions.Get().Remove(key);
					Service.Game.Sim.simulation.knownRolesAndFactions.Broadcast();
					DoYourThing.DisableRoleLabel(key);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600001D RID: 29
		public static void OnSendToChat()
		{
			string text = "";
			for (int i = 1; i < 16; i++)
			{
				try
				{
					BMG_InputField input = DoYourThing.GetInput(i);
					if (input.gameObject.activeInHierarchy)
					{
						string text2 = input.text;
						MatchCollection matchCollection = DoYourThing.LinkRoleRegex.Matches(text2);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							string value = DoYourThing.RoleIdRegex.Match(matchCollection[j].Value).Value;
							string text3 = "";
							Match match = DoYourThing.FactionIdRegex.Match(matchCollection[j].Value);
							if (match.Success && int.Parse(value) < 100)
							{
								text3 = match.Value;
							}
							string text4 = "[[#" + value.ToString();
							if (text3 != "")
							{
								text4 = text4 + "," + text3.ToString();
							}
							text4 += "]]";
							text2 = DoYourThing.PostChatRegex.Replace(text2, text4, 1);
						}
						text = string.Concat(new string[]
						{
							text,
							i.ToString(),
							" ",
							text2,
							" "
						});
					}
				}
				catch
				{
					i = 16;
				}
			}
			PasteTextController.FormatAndPasteToChat(text, DoYourThing.mentionsPanel);
		}

		// Token: 0x0600001E RID: 30
		public static FactionType TraitorFaction(string str)
		{
			string a = str.ToLower();
			if (Utils.IsBTOS2())
			{
				if (a == "ptt" || a == "pantt" || a == "pandtt" || a == "pandoratt" || a == "pand" || a == "pandora")
				{
					return (FactionType)43;
				}
				if (a == "rec" || a == "recruit" || a == "recruited" || a == "recd")
				{
					return (FactionType)33;
				}
				if (a == "ego" || a == "egotown" || a == "egotownie" || a == "egoist" || a == "egotist")
				{
					return (FactionType)42;
				}
				if (a == "comp" || a == "comk" || a == "compliance" || a == "compliant" || a == "comkiller" || a == "compkiller")
				{
					return (FactionType)44;
				}
			}
			if (a == "traitor" || a == "tt")
			{
				if (Utils.IsPandora())
				{
					return (FactionType)43;
				}
				if (Utils.IsCTT())
				{
					return FactionType.COVEN;
				}
				if (Utils.IsATT())
				{
					return FactionType.APOCALYPSE;
				}
				return FactionType.COVEN;
			}
			else
			{
				if (a == "town" || a == "townie")
				{
					return FactionType.TOWN;
				}
				if (a == "cov" || a == "coven" || a == "ctt" || (a == "covtt" | a == "coventt"))
				{
					return FactionType.COVEN;
				}
				if (a == "sk")
				{
					return FactionType.SERIALKILLER;
				}
				if (a == "arso" || a == "arsonist")
				{
					return FactionType.ARSONIST;
				}
				if (a == "ww" || a == "werewolf")
				{
					return FactionType.WEREWOLF;
				}
				if (a == "shroud")
				{
					return FactionType.SHROUD;
				}
				if (a == "exe" || a == "executioner")
				{
					return FactionType.EXECUTIONER;
				}
				if (a == "jest" || a == "jester")
				{
					return FactionType.JESTER;
				}
				if (a == "cs")
				{
					return FactionType.CURSED_SOUL;
				}
				if (a == "att" || a == "apoctt" || a == "apocalypsett" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")
				{
					return FactionType.APOCALYPSE;
				}
				if (a == "vamp" || a == "vampire" || a == "vampd" || a == "vampired" || a == "bit" || a == "bitten" || a == "converted" || a == "convert" || a == "conv")
				{
					return FactionType.VAMPIRE;
				}
				return FactionType.NONE;
			}
		}

		// Token: 0x0600001F RID: 31
		public static void DisableRoleLabel(int num)
		{
			try
			{
				GameObject.Find("Hud/AbilityMenuElementsUI(Clone)/MainCanvasGroup/MainPanel/TosAbilityMenu/PlayerList/Players").transform.GetChild(num + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject.SetActive(false);
			}
			catch
			{
			}
		}

		// Token: 0x04000002 RID: 2
		public static StateProperty<Dictionary<int, Tuple<Role, FactionType>>> ourknown;

		// Token: 0x04000003 RID: 3
		public static Regex RoleRegex = new Regex("\\[\\[#\\d+]]|\\[\\[#\\d+,\\d+]]|<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		// Token: 0x04000004 RID: 4
		public static Regex RoleIdRegex;

		// Token: 0x04000005 RID: 5
		public static Regex FactionIdRegex;

		// Token: 0x04000006 RID: 6
		public static Regex AlignmentRegex;

		// Token: 0x04000007 RID: 7
		public static Regex AlignmentRegexBTOS;

		// Token: 0x04000008 RID: 8
		public static BMG_Button PlayerNotesSendToChat;

		// Token: 0x04000009 RID: 9
		public static MentionPanel mentionsPanel;

		// Token: 0x0400000A RID: 10
		public static NotepadPanel notepad;

		// Token: 0x0400000B RID: 11
		public static Regex PostChatRegex;

		// Token: 0x0400000C RID: 12
		public static Regex LinkRoleRegex = new Regex("<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		// Token: 0x0400000D RID: 13
		public static List<int> alreadydone;

		// Token: 0x0400000E RID: 14
		public static Regex TraitorRegex = new Regex("(?<=^|\\s|\\*)(town|townie|cov|coven|tt|traitor|ctt|covtt|coventt|sk|arso|arsonist|ww|werewolf|shroud|apoc|apocalypse|horseman|horsemen|att|apoctt|apocalypsett|exe|executioner|jest|jester|cs|pand|pandora|ptt|pantt|pandtt|pandoratt|recruit|recruited|rec|recd|egotist|egoist|ego|egotown|egotownie|vamp|vampire|vampd|vampired|bit|bitten|converted|convert|conv|comk|comp|compliance|compliant|comkiller|compkiller)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);

		// Token: 0x0400000F RID: 15
		public static bool JANCanCode = false;

		// Token: 0x04000010 RID: 16
		public static Dictionary<int, Tuple<Role, FactionType>> lockedplayers;
	}
}
