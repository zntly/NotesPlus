using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BMG.UI;
using Game.Interface;
using Game.Simulation;
using HarmonyLib;
using Home.Common.Tooltips;
using Home.Shared;
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
	[HarmonyPatch(typeof(GameSimulation), "HandleOnGameInfoChanged")]
	public class DoYourThing
	{
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
						if ((bool)Settings.SettingsCache.GetValue("Manual Locking/Unlocking") && Pepper.GetMyPosition() != i - 1)
						{
                            BMG_Button lockButton = UnityEngine.Object.Instantiate<BMG_Button>(DoYourThing.PlayerNotesSendToChat, new Vector2(-0.06f, 0.305f), Quaternion.identity, input.transform.parent);
							DoYourThing.lockButtons.SetValue(i - 1, lockButton);
							RectTransform lockTransform = lockButton.transform as RectTransform;
							TooltipTrigger tooltip = lockButton.GetComponent<TooltipTrigger>();
                            tooltip.LookupKey = "";
                            if (DoYourThing.lockedplayers.ContainsKey(i - 1))
							{
								lockButton.DoSpriteSwap(Main.locked);
								Tuple<Role, FactionType> tuple = DoYourThing.lockedplayers.GetValue(i - 1);
								tooltip.NonLocalizedString = "Unlock Role (Currently locked to " + (tuple.Item2 != FactionType.NONE && tuple.Item2 != FactionType.UNKNOWN && tuple.Item2 != tuple.Item1.GetFaction() ? Utils.RoleDisplayString(tuple.Item1, tuple.Item2) + "$" + tuple.Item2.ToDisplayString() : Utils.RoleDisplayString(tuple.Item1, tuple.Item2)) + ")";
                            } else
							{
                                lockButton.DoSpriteSwap(Main.unlocked);
								tooltip.NonLocalizedString = "Lock Role";
                            }
							lockTransform.position = new Vector3(-0.06f, lockTransform.parent.GetChild(0).transform.position.y - 0.0265f, 0f);
							lockTransform.sizeDelta = new Vector2(35f, 35f);
							lockButton.autoClickDelay = 1f;
                            lockButton.onClick.RemoveAllListeners();
							void onClickLock()
							{
                                if (DoYourThing.lockedplayers.ContainsKey(i - 1))
								{
									DoYourThing.lockedplayers.Remove(i - 1);
                                    lockButton.DoSpriteSwap(Main.unlocked);
                                    tooltip.NonLocalizedString = "Lock Role";
                                } else if (DoYourThing.ourknown.Get().ContainsKey(i - 1))
								{
                                    Tuple<Role, FactionType> tuple = DoYourThing.ourknown.Get().GetValue(i - 1, null);
                                    DoYourThing.lockedplayers.SetValue(i - 1, tuple);
                                    lockButton.DoSpriteSwap(Main.locked);
                                    tooltip.NonLocalizedString = "Unlock Role (Currently locked to " + (tuple.Item2 != FactionType.NONE && tuple.Item2 != FactionType.UNKNOWN && tuple.Item2 != tuple.Item1.GetFaction() ? Utils.RoleDisplayString(tuple.Item1, tuple.Item2) + "$" + tuple.Item2.ToDisplayString() : Utils.RoleDisplayString(tuple.Item1, tuple.Item2)) + ")";
                                }
							}
							lockButton.onClick.AddListener(new UnityAction(onClickLock));
                        }
                        DoYourThing.alreadydone.Add(i);
                    }
                }
                catch
                {
                    current = 16;
                }
            }
        }
		public static void SharedHandleNPlus()
		{
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
                DoYourThing.PlayerNotesCopyToClipboard.DoSpriteSwap(Main.copyToClipboard);
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

		[HarmonyPostfix]
		public static void Postfix(GameInfo gameInfo)
		{
			if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DAY)
			{
				ClaimspaceVisualizer.ready = false;
				ClaimspaceVisualizer.allRoleListItems = new List<RoleListItem>();
                DoYourThing.playerList = GameObject.Find("Hud/AbilityMenuElementsUI(Clone)/MainCanvasGroup/MainPanel/TosAbilityMenu/PlayerList/Players");
				DoYourThing.inputHolder = null;
				DoYourThing.JANCanCode = false;
				DoYourThing.alreadydone = new List<int>();
                DoYourThing.ourknown = new StateProperty<Dictionary<int, Tuple<Role, FactionType>>>(new Dictionary<int, Tuple<Role, FactionType>>());
				DoYourThing.lockedplayers = new Dictionary<int, Tuple<Role, FactionType>>();
                DoYourThing.lockButtons = new Dictionary<int, BMG_Button>();
                foreach (KeyValuePair<int, Tuple<Role, FactionType>> keyValuePair in Service.Game.Sim.simulation.knownRolesAndFactions.Get())
				{
					DoYourThing.ourknown.Get().SetValue(keyValuePair.Key, keyValuePair.Value);
					DoYourThing.lockedplayers.TryAdd(keyValuePair.Key, keyValuePair.Value);
				}
				StateProperty<Dictionary<int, Tuple<Role, FactionType>>> knownRolesAndFactions = Service.Game.Sim.simulation.knownRolesAndFactions;
				knownRolesAndFactions.OnChanged = (Action<Dictionary<int, Tuple<Role, FactionType>>>)Delegate.Combine(knownRolesAndFactions.OnChanged, new Action<Dictionary<int, Tuple<Role, FactionType>>>(DoYourThing.DetectChanges));
				DoYourThing.notepad = UnityEngine.Object.FindAnyObjectByType<NotepadPanel>();
				DoYourThing.SharedHandleNPlus();
                DoYourThing.HandleInputFields();
            }
			else if (gameInfo.gamePhase == GamePhase.PLAY && gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION)
			{
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
				if (!DoYourThing.JANCanCode)
				{
					DoYourThing.SharedHandleNPlus();
                    DoYourThing.HandleInputFields();
                }
			}
		}
		public static void DetectChanges(Dictionary<int, Tuple<Role, FactionType>> data)
		{
			if ((bool)Settings.SettingsCache.GetValue("Claimspace Visualizer"))
				ClaimspaceVisualizer.SortRoles(data);
			for (int i = 0; i < 15; i++)
			{
				try
				{
					if ((!DoYourThing.ourknown.Get().ContainsKey(i) && Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i)) || (Service.Game.Sim.simulation.knownRolesAndFactions.Get().ContainsKey(i) && DoYourThing.ourknown.Get().ContainsKey(i) && (Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item1 != DoYourThing.ourknown.Get().GetValue(i, null).Item1 || Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null).Item2 != DoYourThing.ourknown.Get().GetValue(i, null).Item2)))
					{ 
						Tuple<Role, FactionType> tuple = Service.Game.Sim.simulation.knownRolesAndFactions.Get().GetValue(i, null);
                        DoYourThing.lockedplayers.SetValue(i, tuple);
						try
						{
							BMG_Button lockButton = DoYourThing.lockButtons.GetValue(i);
                            TooltipTrigger tooltip = lockButton.GetComponent<TooltipTrigger>();
                            if (lockButton)
							{
                                lockButton.DoSpriteSwap(Main.locked);
                                tooltip.NonLocalizedString = "Unlock Role (Currently locked to " + (tuple.Item2 != FactionType.NONE && tuple.Item2 != FactionType.UNKNOWN && tuple.Item2 != tuple.Item1.GetFaction() ? Utils.RoleDisplayString(tuple.Item1, tuple.Item2) + "$" + tuple.Item2.ToDisplayString() : Utils.RoleDisplayString(tuple.Item1, tuple.Item2)) + ")";
                            }
							GameObject gameObject = DoYourThing.playerList.transform.GetChild(i + 1).Find("LayoutGroup").Find("PlayerRoleLabel").gameObject;
							if (gameObject)
							{
								RectTransform component = gameObject.GetComponent<RectTransform>();
								float multi = ModStates.IsEnabled("alchlcsystm.fancy.ui") ? 1f : 0.34718204f;
								float maxsize = multi == 1f ? 9999f : 150f;
								float x = Mathf.Min(gameObject.GetComponent<TextMeshProUGUI>().GetPreferredValues("(" + Utils.RoleDisplayString(tuple.Item1, tuple.Item2) + ")").x * multi, maxsize);
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
		public static Tuple<Role, FactionType> AlignmentToFaction(string align)
		{
			string a = align.ToLower();
			bool isBtos = Utils.IsBTOS2();
			Role theRole = Role.UNKNOWN;
			FactionType theFaction = FactionType.UNKNOWN;
			foreach (KeyValuePair<List<string>, FactionType> kvp in AlignmentToFactionStrings)
				if (kvp.Key.Contains(a))
				{
					theRole = GetRoleOfAbbreviationList(kvp.Key, isBtos);
					theFaction = kvp.Value;
				}
			if (isBtos)
			{
				if (Utils.IsPandora() && (theFaction == FactionType.COVEN || theFaction == FactionType.APOCALYPSE))
					theFaction = Btos2Faction.Pandora;
				if (Utils.IsCompliance() && theRole == Btos2Role.NeutralKilling)
					theFaction = Btos2Faction.Compliance;
			}
			return new Tuple<Role, FactionType>(theRole, theFaction);
		}
        public static FactionType AlignmentToFaction(Role role)
        {
            bool isBtos = Utils.IsBTOS2();
            FactionType theFaction = FactionType.UNKNOWN;
			List<string> abbreviationList = GetAbbreviationListOfRole(role, isBtos);
            foreach (KeyValuePair<List<string>, FactionType> kvp in AlignmentToFactionStrings)
                if (kvp.Key == abbreviationList)
                    theFaction = kvp.Value;
            if (isBtos)
            {
                if (Utils.IsPandora() && (theFaction == FactionType.COVEN || theFaction == FactionType.APOCALYPSE))
                    theFaction = Btos2Faction.Pandora;
                if (Utils.IsCompliance() && role == Btos2Role.NeutralKilling)
                    theFaction = Btos2Faction.Compliance;
            }
            return theFaction;
        }
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
								if (role >= Role.RANDOM_TOWN && role < (Role)200)
									factionType = DoYourThing.AlignmentToFaction(role);
								else
									factionType = role.GetFaction();
								if (Utils.IsBTOS2())
								{
									if ((factionType == FactionType.COVEN || factionType == FactionType.APOCALYPSE) && Utils.IsPandora())
									{
										factionType = Btos2Faction.Pandora;
									}
									else if (factionType >= FactionType.SERIALKILLER && factionType <= FactionType.SHROUD && Utils.IsCompliance())
									{
										factionType = Btos2Faction.Compliance;
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
					Match match4 = DoYourThing.AlignmentRegex.Match(str);
                    if (match4.Success)
					{
						Tuple<Role, FactionType> tuple2 = DoYourThing.AlignmentToFaction(match4.Value);
						Role item = tuple2.Item1;
						FactionType factionType2 = FactionType.UNKNOWN;
                        flag = true;
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
		public static FactionType TraitorFaction(string str)
		{
			string a = str.ToLower();
            if (a == "traitor" || a == "tt")
            {
                if (Utils.IsPandora())
                    return Btos2Faction.Pandora;
                if (Utils.IsCTT())
                    return FactionType.COVEN;
                if (Utils.IsATT())
                    return FactionType.APOCALYPSE;
                return FactionType.COVEN;
            }
            FactionType theFaction = FactionType.UNKNOWN;
            foreach (KeyValuePair<FactionType, List<string>> kvp in TraitorAbbreviationStrings)
				if (kvp.Value.Contains(a))
					theFaction = kvp.Key;
			if (Utils.IsBTOS2())
			{
				if (Utils.IsPandora() && (theFaction == FactionType.COVEN || theFaction == FactionType.APOCALYPSE))
					theFaction = Btos2Faction.Pandora;
				if (Utils.IsCompliance() && theFaction >= FactionType.SERIALKILLER && theFaction <= FactionType.SHROUD)
					theFaction = Btos2Faction.Compliance;
			}
			return theFaction;
		}
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
		public static GameObject GetNotesLabel(int num)
		{
			return DoYourThing.playerList.transform.GetChild(num + 1).Find("LayoutGroup").Find("NotesPlusLabel").gameObject;
		}
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
		public static List<string> GetAbbreviationListOfRoleTuple(Role vanillaRole, Role btosRole)
		{
            foreach (KeyValuePair<Tuple<Role, Role>, List<string>> kvp in AlignmentAbbreviationStrings)
				if (kvp.Key.Item1 == vanillaRole && kvp.Key.Item2 == btosRole)
					return kvp.Value;
            return new List<string>();
		}
        public static List<string> GetAbbreviationListOfRole(Role role, bool isBtos)
        {
            foreach (KeyValuePair<Tuple<Role, Role>, List<string>> kvp in AlignmentAbbreviationStrings)
                if (!isBtos && kvp.Key.Item1 == role || isBtos && kvp.Key.Item2 == role)
                    return kvp.Value;
            return new List<string>();
        }
		public static Tuple<Role, Role> GetRoleTupleOfAbbreviationList(List<string> abbreviationList)
		{
            foreach (KeyValuePair<Tuple<Role, Role>, List<string>> kvp in AlignmentAbbreviationStrings)
                if (kvp.Value == abbreviationList)
                    return kvp.Key;
            return new Tuple<Role, Role>(Role.UNKNOWN, Role.UNKNOWN);
        }
        public static Role GetRoleOfAbbreviationList(List<string> abbreviationList, bool isBtos)
        {
            foreach (KeyValuePair<Tuple<Role, Role>, List<string>> kvp in AlignmentAbbreviationStrings)
                if (kvp.Value == abbreviationList)
                    return isBtos ? kvp.Key.Item2 : kvp.Key.Item1;
            return Role.UNKNOWN;
        }
        static DoYourThing()
        {
            string alignments = "(";
            bool firstDone = false;
            foreach (KeyValuePair<Tuple<Role, Role>, List<string>> kvp in AlignmentAbbreviationStrings)
                foreach (string alignment in kvp.Value)
                {
                    if (!firstDone)
                    {
                        firstDone = true;
                        alignments += alignment;
                        continue;
                    }
                    alignments += "|" + alignment;
                }
            alignments += ")";
            DoYourThing.AlignmentRegex = new Regex("(?<=^|\\s|\\*)" + alignments + "(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
            string factions = "(";
			firstDone = false;
            foreach (KeyValuePair<FactionType, List<string>> kvp in TraitorAbbreviationStrings)
                foreach (string faction in kvp.Value)
                {
                    if (!firstDone)
                    {
                        firstDone = true;
                        factions += faction;
                        continue;
                    }
                    factions += "|" + faction;
                }
            factions += ")";
            DoYourThing.TraitorRegex = new Regex("(?<=^|\\s|\\*)" + factions + "(?=$|\\s|\\*)", RegexOptions.IgnoreCase);
        }
        public static StateProperty<Dictionary<int, Tuple<Role, FactionType>>> ourknown;

		public static Dictionary<int, BMG_Button> lockButtons;

        public static Regex RoleRegex = new Regex("\\[\\[#\\d+]]|\\[\\[#\\d+,\\d+]]|<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		public static Regex RoleIdRegex = new Regex("\\d+");

		public static Regex FactionIdRegex = new Regex("(?<=,)\\d+");

		public static Dictionary<Tuple<Role, Role>, List<string>> AlignmentAbbreviationStrings = new Dictionary<Tuple<Role, Role>, List<string>>()
		{
			{
				Tuple.Create(Role.TOWN_INVESTIGATIVE, Btos2Role.TownInvestigative),
				new List<string>()
				{
					"ti"
				}
			},
            {
                Tuple.Create(Role.TOWN_PROTECTIVE, Btos2Role.TownProtective),
                new List<string>()
                {
                    "tp"
                }
            },
            {
                Tuple.Create(Role.TOWN_SUPPORT, Btos2Role.TownSupport),
                new List<string>()
                {
                    "ts"
                }
            },
            {
                Tuple.Create(Role.TOWN_KILLING, Btos2Role.TownKilling),
                new List<string>()
                {
                    "tk"
                }
            },
            {
                Tuple.Create(Role.TOWN_POWER, Btos2Role.Unknown),
                new List<string>()
                {
                    "tpow", "tpower"
                }
            },
            {
                Tuple.Create(Role.UNKNOWN, Btos2Role.TownGovernment),
                new List<string>()
                {
                    "tg", "tgov"
                }
            },
            {
                Tuple.Create(Role.UNKNOWN, Btos2Role.TownExecutive),
                new List<string>()
                {
                    "te", "texe"
                }
            },
            {
                Tuple.Create(Role.COVEN_KILLING, Btos2Role.CovenKilling),
                new List<string>()
                {
                    "ck"
                }
            },
            {
                Tuple.Create(Role.COVEN_POWER, Btos2Role.CovenPower),
                new List<string>()
                {
                    "cp", "cpow", "cpower"
                }
            },
            {
                Tuple.Create(Role.COVEN_DECEPTION, Btos2Role.CovenDeception),
                new List<string>()
                {
                    "cd"
                }
            },
            {
                Tuple.Create(Role.COVEN_UTILITY, Btos2Role.CovenUtility),
                new List<string>()
                {
                    "cu"
                }
            },
            {
                Tuple.Create(Role.NEUTRAL_EVIL, Btos2Role.NeutralEvil),
                new List<string>()
                {
                    "ne"
                }
            },
            {
                Tuple.Create(Role.NEUTRAL_KILLING, Btos2Role.NeutralKilling),
                new List<string>()
                {
                    "nk"
                }
            },
            {
                Tuple.Create(Role.NEUTRAL_APOCALYPSE, Btos2Role.RandomApocalypse),
                new List<string>()
                {
                    "na", "ra", "apoc", "apocalypse", "horseman", "horsemen"
                }
            },
            {
                Tuple.Create(Role.RANDOM_TOWN, Btos2Role.RandomTown),
                new List<string>()
                {
                    "rt", "town", "townie"
                }
            },
            {
                Tuple.Create(Role.COMMON_TOWN, Btos2Role.CommonTown),
                new List<string>()
                {
                    "ct"
                }
            },
            {
                Tuple.Create(Role.RANDOM_COVEN, Btos2Role.RandomCoven),
                new List<string>()
                {
                    "rc", "cov", "coven"
                }
            },
            {
                Tuple.Create(Role.RANDOM_NEUTRAL, Btos2Role.RandomNeutral),
                new List<string>()
                {
                    "rn", "neut", "neutral"
                }
            },
            {
                Tuple.Create(Role.UNKNOWN, Btos2Role.NeutralPariah),
                new List<string>()
                {
                    "np", "pariah"
                }
            },
            {
                Tuple.Create(Role.UNKNOWN, Btos2Role.NeutralOutlier),
                new List<string>()
                {
                    "no", "nout", "ns", "nspec"
                }
            },
        };

		public static Dictionary<FactionType, List<string>> TraitorAbbreviationStrings = new Dictionary<FactionType, List<string>>()
		{
			{
				FactionType.TOWN,
				new List<string>()
				{
					"town", "townie"
				}
			},
            {
                FactionType.COVEN,
                new List<string>()
                {
                    "cov", "coven", "ctt", "covtt", "coventt"
                }
            },
            {
                FactionType.SERIALKILLER,
                new List<string>()
                {
                    "sk"
                }
            },
            {
                FactionType.ARSONIST,
                new List<string>()
                {
                    "arso", "arsonist"
                }
            },
            {
                FactionType.WEREWOLF,
                new List<string>()
                {
                    "ww", "werewolf"
                }
            },
            {
                FactionType.SHROUD,
                new List<string>()
                {
                    "shroud"
                }
            },
            {
                FactionType.APOCALYPSE,
                new List<string>()
                {
                    "apoc", "apocalypse", "horseman", "horsemen", "att", "apoctt", "apocalypsett"
                }
            },
            {
                FactionType.EXECUTIONER,
                new List<string>()
                {
                    "exe", "executioner"
                }
            },
            {
                FactionType.JESTER,
                new List<string>()
                {
                    "jest", "jester"
                }
            },
            {
                FactionType.PIRATE,
                new List<string>()
                {
                    "pirate"
                }
            },
            {
                FactionType.DOOMSAYER,
                new List<string>()
                {
                    "doom", "doomsayer", "random", "randomass", "random ass"
                }
            },
            {
                FactionType.VAMPIRE,
                new List<string>()
                {
                    "vamp", "vampire", "vampd", "vampired", "bit", "bitten", "conv", "convert", "converted"
                }
            },
            {
                FactionType.CURSED_SOUL,
                new List<string>()
                {
                    "cs", "ws", "cursed", "wander", "wandering", "soul"
                }
            },
            {
                Btos2Faction.Jackal,
                new List<string>()
                {
                    "rec", "recruit", "recd", "recruited"
                }
            },
            {
                Btos2Faction.Egotist,
                new List<string>()
                {
                    "ego", "egoist", "egotist", "egotown", "egotownie"
                }
            },
            {
                Btos2Faction.Pandora,
                new List<string>()
                {
                    "pan", "pand", "pandora", "ptt", "pantt", "pandtt", "pandoratt"
                }
            },
            {
                Btos2Faction.Compliance,
                new List<string>()
                {
                    "comp", "compliant", "compliance", "comk", "comkiller", "compkiller"
                }
            },
            {
                FactionType.UNKNOWN,
                new List<string>()
                {
                    "tt", "traitor"
                }
            },
        };

		public static Dictionary<List<string>, FactionType> AlignmentToFactionStrings = new Dictionary<List<string>, FactionType>()
		{
			{
				GetAbbreviationListOfRoleTuple(Role.TOWN_INVESTIGATIVE, Btos2Role.TownInvestigative), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.TOWN_PROTECTIVE, Btos2Role.TownProtective), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.TOWN_SUPPORT, Btos2Role.TownSupport), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.TOWN_KILLING, Btos2Role.TownKilling), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.TOWN_POWER, Btos2Role.Unknown), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.UNKNOWN, Btos2Role.TownGovernment), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.UNKNOWN, Btos2Role.TownExecutive), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.RANDOM_TOWN, Btos2Role.RandomTown), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COMMON_TOWN, Btos2Role.CommonTown), FactionType.TOWN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COVEN_KILLING, Btos2Role.CovenKilling), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COVEN_DECEPTION, Btos2Role.CovenDeception), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COVEN_POWER, Btos2Role.CovenPower), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COVEN_UTILITY, Btos2Role.CovenUtility), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.RANDOM_COVEN, Btos2Role.RandomCoven), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.COMMON_COVEN, Btos2Role.CommonCoven), FactionType.COVEN
			},
			{
				GetAbbreviationListOfRoleTuple(Role.NEUTRAL_EVIL, Btos2Role.NeutralEvil), FactionType.NONE
			},
			{
				GetAbbreviationListOfRoleTuple(Role.NEUTRAL_KILLING, Btos2Role.NeutralKilling), FactionType.NONE
			},
			{
				GetAbbreviationListOfRoleTuple(Role.UNKNOWN, Btos2Role.NeutralPariah), FactionType.NONE
			},
			{
				GetAbbreviationListOfRoleTuple(Role.UNKNOWN, Btos2Role.NeutralOutlier), FactionType.NONE
			},
			{
				GetAbbreviationListOfRoleTuple(Role.NEUTRAL_APOCALYPSE, Btos2Role.RandomApocalypse), FactionType.APOCALYPSE
			},
		};
		public static Regex AlignmentRegex;

        public static BMG_Button PlayerNotesSendToChat;

		public static BMG_Button PlayerNotesCopyToClipboard;

		public static MentionPanel mentionsPanel;

		public static NotepadPanel notepad;

		public static Regex PostChatRegex = new Regex("<style=Mention>(.*?<color=#......>.*?\\/style>)|<style=Mention>(.*?\\/style>)");

		public static Regex LinkRoleRegex = new Regex("<link=\"r\\d+\">|<link=\"r\\d+,\\d+\">");

		public static List<int> alreadydone;

		public static Regex TraitorRegex;

		public static bool JANCanCode = false;

		public static Dictionary<int, Tuple<Role, FactionType>> lockedplayers;

		public static Regex AdditionalNotesRegex = new Regex("(?<=(?<!\\[)\\[)[^\\[\\]]*(?=\\](?!\\]))");

		public static GameObject playerList;

		public static GameObject inputHolder;
	}
}
