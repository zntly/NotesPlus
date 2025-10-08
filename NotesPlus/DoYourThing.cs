using System;
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
using Server.Shared.Utils;
using Services;
using SML;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NotesPlus
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(GameSimulation), "HandleOnGameInfoChanged")]
	public class DoYourThing
	{
		// Token: 0x0600000C RID: 12
		public static void HandleInputFields()
		{
            for (int current = 1; current < 16; current++)
            {
                try
                {
                    if (!DoYourThing.alreadydone.Contains(current))
                    {
                        int i = current;
                        BMG_InputField input = DoYourThing.GetInput(i);
                        input.characterLimit = 99999;
                        void DoingTheThing(string str)
                        {
                            DoYourThing.TheGoodStuff(str, i - 1);
                            DoYourThing.TheAdditionalStuff(str, i - 1);
                        }
                        input.onValueChanged.AddListener(new UnityAction<string>(DoingTheThing));
                        DoYourThing.CreateNotesLabel(i - 1);
                        DoYourThing.alreadydone.Add(i);
                    }
                }
                catch
                {
                    current = 16;
                }
            }
        }
		public static BMG_InputField GetInput(int num)
		{
			if (ModStates.IsEnabled("JAN.movablewills") && ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills"))
			{
				if (DoYourThing.inputHolder == null)
                {
					DoYourThing.inputHolder = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground/Scroll View/Viewport/Content");
				}
				return DoYourThing.inputHolder.transform.GetChild(num + Service.Game.Sim.simulation.validPlayerCount.Get()).gameObject.GetComponentInChildren<BMG_InputField>();
			}
			if (DoYourThing.inputHolder == null)
			{
				DoYourThing.inputHolder = GameObject.Find("Hud/NotepadElementsUI(Clone)/MainPanel/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground/Scroll View/Viewport/Content");
			}
			return DoYourThing.inputHolder.transform.GetChild(num).gameObject.GetComponentInChildren<BMG_InputField>();
		}

		// Token: 0x0600000D RID: 13
		[HarmonyPostfix]
		public static void Postfix(GameInfo gameInfo)
		{
			if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DAY)
			{
				DoYourThing.playerList = GameObject.Find("Hud/AbilityMenuElementsUI(Clone)/MainCanvasGroup/MainPanel/TosAbilityMenu/PlayerList/Players");
				DoYourThing.inputHolder = null;
				DoYourThing.JANCanCode = false;
				DoYourThing.alreadydone = new List<int>();
				DoYourThing.HandleInputFields();
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
					DoYourThing.PlayerNotesSendToChat.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
					DoYourThing.PlayerNotesSendToChat.onClick.RemoveAllListeners();
					DoYourThing.PlayerNotesCopyToClipboard = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.PlayerNotesSendToChat, new Vector2(0.3f, -0.5f), Quaternion.identity, gameObject.transform);
					DoYourThing.PlayerNotesSendToChat.transform.localPosition = new Vector3(220f, -365f, 0f);
					DoYourThing.PlayerNotesCopyToClipboard.transform.localPosition = new Vector3(150f, -365f, 0f);
					Texture2D clipboardsprite = Main.Textures["copy-icon"];
					DoYourThing.PlayerNotesCopyToClipboard.DoSpriteSwap(Sprite.Create(clipboardsprite, new Rect(0f, 0f, (float)clipboardsprite.width, (float)clipboardsprite.height), new Vector2(DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pivot.x / (float)clipboardsprite.width, DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pivot.y / (float)clipboardsprite.height), DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pixelsPerUnit, 0U, SpriteMeshType.Tight, DoYourThing.PlayerNotesCopyToClipboard.image.sprite.border));
					try
					{
						TooltipTrigger component = DoYourThing.PlayerNotesSendToChat.GetComponent<TooltipTrigger>();
						if (component != null)
						{
							component.LookupKey = "";
							component.NonLocalizedString = "Send Player Notes to Chat";
						}
						TooltipTrigger component2 = DoYourThing.PlayerNotesCopyToClipboard.GetComponent<TooltipTrigger>();
						if (component2 != null)
						{
							component2.LookupKey = "";
							component2.NonLocalizedString = "Copy to Clipboard";
						}
					}
					catch
					{
					}
					DoYourThing.PlayerNotesSendToChat.onClick.AddListener(new UnityAction(DoYourThing.OnSendToChat));
					DoYourThing.PlayerNotesCopyToClipboard.onClick.AddListener(new UnityAction(DoYourThing.OnCopyToClipboard));
					return;
				}
			}
			else if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION)
			{
                DoYourThing.HandleInputFields();
                foreach (KeyValuePair<int, Tuple<Role, FactionType>> keyValuePair2 in DoYourThing.lockedplayers)
				{
					try
					{
						GameObject gameObject2 = DoYourThing.playerList.transform.GetChild(keyValuePair2.Key + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
						if (gameObject2)
						{
							RectTransform component3 = gameObject2.GetComponent<RectTransform>();
                            float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
                            float maxsize = multi == 1f ? 9999f : 150f;
                            float x = Mathf.Min(gameObject2.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(keyValuePair2.Value.Item1, keyValuePair2.Value.Item2) + ")").x * multi, maxsize);
							component3.sizeDelta = new Vector2(x, 30f);
						}
					}
					catch
					{
					}
				}
				if (ModStates.IsEnabled("JAN.movablewills") && ModSettings.GetBool("Player Notes Standalone", "JAN.movablewills") && !DoYourThing.JANCanCode)
				{
					GameObject gameObject3 = GameObject.Find("Hud/NotepadElementsUI(Clone)/asd/NotepadCommonElements/Background/ScaledBackground/PlayerNoteBackground");
					if (DoYourThing.notepad && gameObject3)
					{
						DoYourThing.JANCanCode = true;
						DoYourThing.mentionsPanel = DoYourThing.notepad.mentionsPanel;
						DoYourThing.PlayerNotesSendToChat = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.notepad.SendToChatButton, new Vector2(0.3f, -0.5f), Quaternion.identity, gameObject3.transform);
						DoYourThing.PlayerNotesSendToChat.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
						DoYourThing.PlayerNotesSendToChat.onClick.RemoveAllListeners();
						DoYourThing.PlayerNotesCopyToClipboard = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.PlayerNotesSendToChat, new Vector2(0.3f, -0.5f), Quaternion.identity, gameObject3.transform);
						DoYourThing.PlayerNotesSendToChat.transform.localPosition = new Vector3(220f, -365f, 0f);
						DoYourThing.PlayerNotesCopyToClipboard.transform.localPosition = new Vector3(150f, -365f, 0f);
						Texture2D clipboardsprite = Main.Textures["copy-icon"];
						DoYourThing.PlayerNotesCopyToClipboard.DoSpriteSwap(Sprite.Create(clipboardsprite, new Rect(0f, 0f, (float)clipboardsprite.width, (float)clipboardsprite.height), new Vector2(DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pivot.x / (float)clipboardsprite.width, DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pivot.y / (float)clipboardsprite.height), DoYourThing.PlayerNotesCopyToClipboard.image.sprite.pixelsPerUnit, 0U, SpriteMeshType.Tight, DoYourThing.PlayerNotesCopyToClipboard.image.sprite.border));
						try
						{
							TooltipTrigger component = DoYourThing.PlayerNotesSendToChat.GetComponent<TooltipTrigger>();
							if (component != null)
							{
								component.LookupKey = "";
								component.NonLocalizedString = "Send Player Notes to Chat";
							}
							TooltipTrigger component2 = DoYourThing.PlayerNotesCopyToClipboard.GetComponent<TooltipTrigger>();
							if (component2 != null)
							{
								component2.LookupKey = "";
								component2.NonLocalizedString = "Copy to Clipboard";
							}
						}
						catch
						{
						}
						DoYourThing.PlayerNotesSendToChat.onClick.AddListener(new UnityAction(DoYourThing.OnSendToChat));
						DoYourThing.PlayerNotesCopyToClipboard.onClick.AddListener(new UnityAction(DoYourThing.OnCopyToClipboard));
						return;
					}
				}
			}
		}

		// Token: 0x0600000E RID: 14
		public static void DetectChanges(Dictionary<int, Tuple<Role, FactionType>> data)
		{
			for (int i = 0; i < 15; i++)
			{
				try
				{
					if ((!DoYourThing.ourknown.Get().ContainsKey(i) && Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i)) || (Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i) && DoYourThing.ourknown.Get().ContainsKey(i) && (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item1 != DoYourThing.ourknown.Get().GetValue(i, null).Item1 || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item2 != DoYourThing.ourknown.Get().GetValue(i, null).Item2)))
					{
						DoYourThing.lockedplayers.SetValue(i, Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null));
						try
						{
							GameObject gameObject = DoYourThing.playerList.transform.GetChild(i + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
							if (gameObject)
							{
								RectTransform component = gameObject.GetComponent<RectTransform>();
								Tuple<Role, FactionType> tiemp = Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null);
								float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
								float maxsize = multi == 1f ? 9999f : 150f;
								float x = Mathf.Min(gameObject.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(tiemp.Item1, tiemp.Item2) + ")").x * multi, maxsize);
								component.sizeDelta = new Vector2(x, 30f);
							}
						}
						catch
						{
						}
					}
				}
				catch
				{
					DoYourThing.lockedplayers.SetValue(i, new Tuple<Role, FactionType>(Role.ONE_TRIAL_PER_DAY, FactionType.CURSED_SOUL));
				}
			}
		}

		// Token: 0x0600000F RID: 15
		static DoYourThing()
		{
			DoYourThing.JANCanCode = false;
			DoYourThing.RoleIdRegex = new Regex("\\d+");
			DoYourThing.FactionIdRegex = new Regex("(?<=,)\\d+");
			DoYourThing.AlignmentRegex = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tpow|tpower|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cc|cov|coven|rn|neut|neutral)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
			DoYourThing.AlignmentRegexBTOS = new Regex("(?<=^|\\s|\\*)(ti|tp|tk|ts|tg|tgov|te|texe|ck|cp|cpow|cpower|cd|cu|ne|nk|na|ra|apoc|apocalypse|horseman|horsemen|rt|ct|town|townie|rc|cov|cc|coven|rn|neut|neutral|np|ns|no|nout|nspec|pariah)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
			DoYourThing.PostChatRegex = new Regex("<style=Mention>(.*?<color=#......>.*?\\/style>)|<style=Mention>(.*?\\/style>)");
		}

		// Token: 0x06000010 RID: 16

		// Token: 0x06000011 RID: 17
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
				if (a == "np" || a == "pariah")
				{
					return new Tuple<Role, FactionType>((Role)119, FactionType.NONE);
				}
			}
			else
			{
				if (a == "ti")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_KILLING, FactionType.TOWN);
				}
				if (a == "tp")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_SUPPORT, FactionType.TOWN);
				}
				if (a == "ts")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_COVEN, FactionType.TOWN);
				}
				if (a == "tg" || a == "tgov")
				{
					return new Tuple<Role, FactionType>((Role)120, FactionType.TOWN);
				}
                if (a == "te" || a == "texe")
                {
                    return new Tuple<Role, FactionType>((Role)119, FactionType.TOWN);
                }
                if (a == "tk")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_POWER, FactionType.TOWN);
				}
				if (a == "rt" || a == "town" || a == "townie")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_INVESTIGATIVE, FactionType.TOWN);
				}
				if (a == "ct")
				{
					return new Tuple<Role, FactionType>(Role.TOWN_PROTECTIVE, FactionType.TOWN);
				}
                bool flag = Utils.IsPandora();
				FactionType item = FactionType.COVEN;
				if (flag)
				{
					item = (FactionType)43;
				}
				if (a == "cd")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_DECEPTION, item);
				}
				if (a == "ck")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_POWER, item);
				}
				if (a == "cp" || a == "cpow" || a == "cpower")
				{
					return new Tuple<Role, FactionType>(Role.RANDOM_NEUTRAL, item);
				}
				if (a == "cu")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_KILLING, item);
				}
				if (a == "rc" || a == "cov" || a == "coven")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_KILLING, item);
				}
				if (a == "cc")
				{
					return new Tuple<Role, FactionType>(Role.COVEN_UTILITY, item);
				}
				if (a == "ne")
				{
					return new Tuple<Role, FactionType>(Role.ANY, FactionType.NONE);
				}
				bool flag2 = Utils.IsCompliance();
				FactionType item2 = FactionType.NONE;
				if (flag2)
				{
					item2 = (FactionType)44;
				}
				if (a == "nk")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_TOWN, item2);
				}
				if (a == "rn" || a == "neut" || a == "neutral")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_APOCALYPSE, FactionType.NONE);
				}
				FactionType item3 = FactionType.APOCALYPSE;
				if (flag)
				{
					item3 = (FactionType)43;
				}
				if (a == "na" || a == "ra" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")
				{
					return new Tuple<Role, FactionType>(Role.NEUTRAL_EVIL, item3);
				}
				if (a == "np" || a == "pariah")
				{
					return new Tuple<Role, FactionType>(Role.COMMON_COVEN, FactionType.NONE);
				}
                if (a == "ns" || a == "nspec" || a == "no" || a == "nout")
                {
                    return new Tuple<Role, FactionType>((Role)118, FactionType.NONE);
                }
            }
			return new Tuple<Role, FactionType>(Role.NONE, FactionType.NONE);
		}

		// Token: 0x06000012 RID: 18

		// Token: 0x06000020 RID: 32
		public static void TheGoodStuff(string str, int key)
		{
			Tuple<Role, FactionType> tuple;
			if ((bool)Settings.SettingsCache.GetValue("Additional Notes"))
            {
				str = DoYourThing.AdditionalNotesRegex.Replace(str, "");
            }
			DoYourThing.lockedplayers.TryGetValue(key, out tuple);
			if (tuple == null && (!(bool)Settings.SettingsCache.GetValue("Only Detect Marked") || ((bool)Settings.SettingsCache.GetValue("Only Detect Marked") && str.IndexOf('*') != -1)))
			{
				Match match = DoYourThing.RoleRegex.Match(str);
				bool flag = false;
				if (match.Success)
				{
					string value = match.Value;
					Role role = (Role)int.Parse(DoYourThing.RoleIdRegex.Match(value).Value);
					Match match2 = DoYourThing.FactionIdRegex.Match(value);
					FactionType factionType = FactionType.UNKNOWN;
					if ((string)Settings.SettingsCache.GetValue("Show Faction Color") == "Always" || ((string)Settings.SettingsCache.GetValue("Show Faction Color") == "Only Marked" && str.IndexOf('*') != -1) || ((string)Settings.SettingsCache.GetValue("Show Faction Color") == "Only Roles" && role < Role.RANDOM_TOWN) || (string)Settings.SettingsCache.GetValue("Show Faction Color") == "Only On Override")
					{
						if (match2.Success)
						{
							factionType = (FactionType)int.Parse(match2.Value);
						}
						if (!match2.Success)
						{
							bool flag2 = false;
							if ((bool)Settings.SettingsCache.GetValue("Faction Abbreviations"))
							{
								Match match3 = DoYourThing.TraitorRegex.Match(str);
								if (match3.Success)
								{
									flag2 = true;
									factionType = DoYourThing.TraitorFaction(match3.Value);
								}
							}
							if (!flag2 && (string)Settings.SettingsCache.GetValue("Show Faction Color") != "Only On Override")
							{
								if ((!Utils.IsBTOS2() && ((role >= Role.RANDOM_TOWN && role <= Role.TOWN_POWER) || role == Role.COMMON_TOWN)) || (Utils.IsBTOS2() && (role >= Role.TOWN_INVESTIGATIVE && role <= Role.RANDOM_COVEN) || role == (Role)119 || role == (Role)120))
								{
									factionType = FactionType.TOWN;
								}
								else if ((!Utils.IsBTOS2() && ((role >= Role.RANDOM_COVEN && role <= Role.COVEN_POWER) || role == Role.COMMON_COVEN)) || (Utils.IsBTOS2() && role >= Role.COVEN_KILLING && role <= Role.NEUTRAL_KILLING))
								{
									factionType = FactionType.COVEN;
								}
                                else if ((Utils.IsBTOS2() && role == Role.NEUTRAL_EVIL) || (!Utils.IsBTOS2() && role == Role.NEUTRAL_APOCALYPSE))
                                {
                                    factionType = FactionType.APOCALYPSE;
                                }
                                else if (role >= Role.RANDOM_TOWN && role < (Role)200)
								{
									factionType = FactionType.NONE;
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
									else if ((role == Role.COMMON_TOWN || factionType == FactionType.SERIALKILLER || factionType == FactionType.ARSONIST || factionType == FactionType.WEREWOLF || factionType == FactionType.SHROUD) && Utils.IsCompliance())
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
						try
						{
							GameObject gameObject = DoYourThing.playerList.transform.GetChild(key + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
							if (gameObject)
							{
								RectTransform component = gameObject.GetComponent<RectTransform>();
                                float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
                                float maxsize = multi == 1f ? 9999f : 150f;
                                float x = Mathf.Min(gameObject.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(role, factionType) + ")").x * multi, maxsize);
								component.sizeDelta = new Vector2(x, 30f);
							}
						}
						catch
						{
						}
						return;
					}
				}
				else if ((bool)Settings.SettingsCache.GetValue("Alignment Abbreviations"))
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
						if ((string)Settings.SettingsCache.GetValue("Show Faction Color") == "Always" || ((string)Settings.SettingsCache.GetValue("Show Faction Color") == "Only Marked" && str.IndexOf('*') != -1))
						{
							bool flag4 = false;
							if ((bool)Settings.SettingsCache.GetValue("Faction Abbreviations"))
							{
								MatchCollection matchCollection = DoYourThing.TraitorRegex.Matches(str);
								if (matchCollection.Count > 0)
								{
									for (int i = 0; i < matchCollection.Count; i++)
									{
										if (matchCollection[i].Value != match4.Value)
										{
											flag4 = true;
											factionType2 = DoYourThing.TraitorFaction(matchCollection[i].Value);
											i = matchCollection.Count;
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
							try
							{
								GameObject gameObject2 = DoYourThing.playerList.transform.GetChild(key + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
								if (gameObject2)
								{
									RectTransform component2 = gameObject2.GetComponent<RectTransform>();
                                    float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
                                    float maxsize = multi == 1f ? 9999f : 150f;
                                    float x2 = Mathf.Min(gameObject2.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(item, factionType2) + ")").x * multi, maxsize);
									component2.sizeDelta = new Vector2(x2, 30f);
								}
							}
							catch
							{
							}
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
				Match match5 = DoYourThing.RoleRegex.Match(str);
				if (match5.Success)
				{
					Match match6 = DoYourThing.FactionIdRegex.Match(match5.Value);
					if (match6.Success)
					{
						flag6 = true;
						factionType3 = (FactionType)int.Parse(match6.Value);
					}
				}
				if (!flag6 && (bool)Settings.SettingsCache.GetValue("Faction Abbreviations"))
				{
					Match match7 = DoYourThing.TraitorRegex.Match(str);
					if (match7.Success)
					{
						factionType3 = DoYourThing.TraitorFaction(match7.Value);
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
					try
					{
						GameObject gameObject3 = DoYourThing.playerList.transform.GetChild(key + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
						if (gameObject3)
						{
							RectTransform component3 = gameObject3.GetComponent<RectTransform>();
                            float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
                            float maxsize = multi == 1f ? 9999f : 150f;
                            float x3 = Mathf.Min(gameObject3.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(item2, factionType3) + ")").x * multi, maxsize);
							component3.sizeDelta = new Vector2(x3, 30f);
						}
					}
					catch
					{
					}
					return;
				}
			}
			else if (tuple == null && (bool)Settings.SettingsCache.GetValue("Only Detect Marked") && str.IndexOf('*') != -1 && Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(key, null) != null)
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

		// Token: 0x06000021 RID: 33
		public static void OnSendToChat()
		{
			string text = "";
			for (int i = 0; i < 16; i++)
			{
				try
				{
					BMG_InputField input = DoYourThing.GetInput(i);
					if (input.gameObject.activeInHierarchy)
					{
						string text2 = input.text.ResolveUnicodeSequences();
						MatchCollection matchCollection = DoYourThing.LinkRoleRegex.Matches(text2);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							string value = DoYourThing.RoleIdRegex.Match(matchCollection[j].Value).Value;
							Match match = DoYourThing.FactionIdRegex.Match(matchCollection[j].Value);
							if (match.Success && int.Parse(value) < 100)
							{
								text2 = DoYourThing.PostChatRegex.Replace(text2, "[[#" + value.ToString() + "," + match.Value.ToString() + "]]", 1);
							}
						}
						DoYourThing.mentionsPanel.mentionsProvider.ProcessEncodedText(text2);
						text2 = DoYourThing.mentionsPanel.mentionsProvider.EncodeText(text2);
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

		public static void OnCopyToClipboard()
		{
			string text = "";
			for (int i = 0; i < 16; i++)
			{
				try
				{
					BMG_InputField input = DoYourThing.GetInput(i);
					if (input.gameObject.activeInHierarchy)
					{
						string text2 = input.text.ResolveUnicodeSequences();
						MatchCollection matchCollection = DoYourThing.LinkRoleRegex.Matches(text2);
						for (int j = 0; j < matchCollection.Count; j++)
						{
							string value = DoYourThing.RoleIdRegex.Match(matchCollection[j].Value).Value;
							Match match = DoYourThing.FactionIdRegex.Match(matchCollection[j].Value);
							if (match.Success && int.Parse(value) < 100)
							{
								text2 = DoYourThing.PostChatRegex.Replace(text2, "[[#" + value.ToString() + "," + match.Value.ToString() + "]]", 1);
							}
						}
						DoYourThing.mentionsPanel.mentionsProvider.ProcessEncodedText(text2);
						text2 = DoYourThing.mentionsPanel.mentionsProvider.EncodeText(text2);
						text = string.Concat(new string[]
						{
							text,
							i.ToString(),
							" ",
							text2,
							(string)Settings.SettingsCache.GetValue("Copy to Clipboard Mode") == "Newlines" ? "\n" : " "
						});
					}
				}
				catch
				{
					i = 16;
				}
			}
			BMG_Clipboard.Clipboard = text;
		}

		// Token: 0x06000022 RID: 34
		public static FactionType TraitorFaction(string str)
		{
			string a = str.ToLower();
			if (Utils.IsBTOS2())
			{
				if (a == "ptt" || a == "pantt" || a == "pandtt" || a == "pandoratt" || a == "pand" || a == "pandora" || (Utils.IsPandora() && (a == "cov" || a == "coven" || a == "ctt" || a == "covtt" | a == "coventt" || a == "att" || a == "apoctt" || a == "apocalypsett" || a == "apoc" || a == "apocalypse" || a == "horseman" || a == "horsemen")))
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
				if (a == "comp" || a == "comk" || a == "compliance" || a == "compliant" || a == "comkiller" || a == "compkiller" || (Utils.IsCompliance() && (a == "sk" || a == "arso" || a == "arsonist" || a == "ww" || a == "werewolf" || a == "shroud")))
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
				if (a == "cov" || a == "coven" || a == "ctt" || a == "covtt" | a == "coventt")
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

		// Token: 0x06000023 RID: 35
		public static void DisableRoleLabel(int num)
		{
			try
			{
				DoYourThing.playerList.transform.GetChild(num + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject.SetActive(false);
			}
			catch
			{
			}
		}

		// Token: 0x06000024 RID: 36
		public static GameObject GetNotesLabel(int num)
		{
			return DoYourThing.playerList.transform.GetChild(num + 1).Find("LayoutGroup").Find("NotesPlusLabel").gameObject;
		}

		// Token: 0x06000025 RID: 37
		public static void CreateNotesLabel(int num)
		{
			GameObject gameObject = DoYourThing.playerList.transform.GetChild(num + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
			if (gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.name = "NotesPlusLabel";
				TextMeshProUGUI component = gameObject2.GetComponent<TextMeshProUGUI>();
				UnityEngine.Object.Destroy(gameObject2.GetComponent<RightClickable>());
				component.fontSizeMax = 18f;
				component.fontSizeMin = 9f;
				component.text = "()";
				gameObject2.SetActive(false);
				gameObject2.transform.SetParent(gameObject.transform.parent);
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
			}
		}

		// Token: 0x06000026 RID: 38
		public static void TheAdditionalStuff(string str, int key)
		{
			if ((bool)Settings.SettingsCache.GetValue("Additional Notes"))
			{
				Match match = DoYourThing.AdditionalNotesRegex.Match(str);
				GameObject notesLabel = DoYourThing.GetNotesLabel(key);
				if (match.Success)
				{
					notesLabel.GetComponent<TextMeshProUGUI>().text = DoYourThing.GetAdditionalNoteText(match.Value, ColorUtility.ToHtmlStringRGB((Color)Settings.SettingsCache.GetValue("Additional Notes Color")));
				}
				if (notesLabel.activeSelf != match.Success)
				{
					notesLabel.SetActive(match.Success);
				}
			}
		}

		// Token: 0x06000027 RID: 39
		public static string GetAdditionalNoteText(string str, string colorhex)
		{
			string @string = (string)Settings.SettingsCache.GetValue("Additional Notes Style");
			string text = (@string == "(Note)") ? "(" : ((@string == "[Note]") ? "[" : ((@string == "{Note}") ? "{" : ((@string == "- Note") ? "-" + " " : "")));
			string text2 = (text == "(") ? ")" : ((text == "[") ? "]" : ((text == "{") ? "}" : ""));
			return string.Concat(new string[]
			{
				"<color=#",
				colorhex,
				">",
				text,
				str,
				text2,
				"</color>"
			});
		}

		// Token: 0x04000003 RID: 3
		public static StateProperty<Dictionary<int, Tuple<Role, FactionType>>> ourknown;

		// Token: 0x04000004 RID: 4
		public static Regex RoleRegex = new Regex("\\[\\[#\\d+]]|\\[\\[#\\d+,\\d+]]|<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		// Token: 0x04000005 RID: 5
		public static Regex RoleIdRegex;

		// Token: 0x04000006 RID: 6
		public static Regex FactionIdRegex;

		// Token: 0x04000007 RID: 7
		public static Regex AlignmentRegex;

		// Token: 0x04000008 RID: 8
		public static Regex AlignmentRegexBTOS;

		// Token: 0x04000009 RID: 9
		public static BMG_Button PlayerNotesSendToChat;

		public static BMG_Button PlayerNotesCopyToClipboard;

		// Token: 0x0400000A RID: 10
		public static MentionPanel mentionsPanel;

		// Token: 0x0400000B RID: 11
		public static NotepadPanel notepad;

		// Token: 0x0400000C RID: 12
		public static Regex PostChatRegex;

		// Token: 0x0400000D RID: 13
		public static Regex LinkRoleRegex = new Regex("<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		// Token: 0x0400000E RID: 14
		public static List<int> alreadydone;

		// Token: 0x0400000F RID: 15
		public static Regex TraitorRegex = new Regex("(?<=^|\\s|\\*)(town|townie|cov|coven|tt|traitor|ctt|covtt|coventt|sk|arso|arsonist|ww|werewolf|shroud|apoc|apocalypse|horseman|horsemen|att|apoctt|apocalypsett|exe|executioner|jest|jester|cs|pand|pandora|ptt|pantt|pandtt|pandoratt|recruit|recruited|rec|recd|egotist|egoist|ego|egotown|egotownie|vamp|vampire|vampd|vampired|bit|bitten|converted|convert|conv|comk|comp|compliance|compliant|comkiller|compkiller)(?=$|\\s|\\*)", RegexOptions.IgnoreCase);

		// Token: 0x04000010 RID: 16
		public static bool JANCanCode;

		// Token: 0x04000011 RID: 17
		public static Dictionary<int, Tuple<Role, FactionType>> lockedplayers;

		// Token: 0x04000012 RID: 18
		public static Regex AdditionalNotesRegex = new Regex("(?<=(?<!\\[)\\[)[^\\[\\]]*(?=\\](?!\\]))");

		public static GameObject playerList;

		public static GameObject inputHolder;
	}
}
