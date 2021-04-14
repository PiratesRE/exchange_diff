using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrSearchManager : ActivityManager
	{
		internal AsrSearchManager(ActivityManager manager, AsrSearchManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool StarOutToDialPlanEnabled
		{
			get
			{
				SpeechAutoAttendantManager speechAutoAttendantManager = base.Manager as SpeechAutoAttendantManager;
				return speechAutoAttendantManager != null && speechAutoAttendantManager.StarOutToDialPlanEnabled;
			}
		}

		internal string BusinessName
		{
			get
			{
				SpeechAutoAttendantManager speechAutoAttendantManager = base.Manager as SpeechAutoAttendantManager;
				if (speechAutoAttendantManager == null)
				{
					return null;
				}
				return speechAutoAttendantManager.BusinessName;
			}
		}

		internal bool RepeatMainMenu
		{
			get
			{
				SpeechAutoAttendantManager speechAutoAttendantManager = base.Manager as SpeechAutoAttendantManager;
				return speechAutoAttendantManager == null || speechAutoAttendantManager.RepeatMainMenu;
			}
		}

		internal AutoAttendantContext AAContext
		{
			get
			{
				SpeechAutoAttendantManager speechAutoAttendantManager = base.Manager as SpeechAutoAttendantManager;
				if (speechAutoAttendantManager == null)
				{
					return null;
				}
				return speechAutoAttendantManager.AAContext;
			}
		}

		internal bool MaxPersonalContactsExceeded
		{
			get
			{
				PersonalContactsGrammarFile personalContactsGrammarFile = this.GlobalManager.PersonalContactsGrammarFile;
				return personalContactsGrammarFile != null && personalContactsGrammarFile.MaxEntriesExceeded;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.searchContext = (AsrSearchContext)this.ReadVariable("searchContext");
			base.WriteVariable("mode", refInfo);
			this.ClearState();
			base.Start(vo, null);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "AsrSearchManager::ExecuteAction({0}) PreviousState: {1}.", new object[]
			{
				action,
				this.menuState
			});
			string input = null;
			if (action.StartsWith("init", StringComparison.OrdinalIgnoreCase))
			{
				this.previousState = this.menuState;
				if (action != null)
				{
					if (!(action == "initAskAgainQA"))
					{
						if (!(action == "initConfirmAgainQA"))
						{
							if (!(action == "initConfirmQA"))
							{
								if (!(action == "initConfirmViaListQA"))
								{
									if (action == "initNameCollisionQA")
									{
										this.menuState = AsrSearchManager.MenuState.CollisionQA;
									}
								}
								else
								{
									this.menuState = AsrSearchManager.MenuState.ConfirmViaListQA;
									this.confirmViaListQA = true;
								}
							}
							else
							{
								this.menuState = AsrSearchManager.MenuState.ConfirmQA;
							}
						}
						else
						{
							this.menuState = AsrSearchManager.MenuState.ConfirmAgainQA;
						}
					}
					else
					{
						this.menuState = AsrSearchManager.MenuState.AskAgainQA;
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "AsrSearchManager::ExecuteAction({0}) PreviousState: {1} NextState: {2}.", new object[]
				{
					action,
					this.previousState,
					this.menuState
				});
			}
			else if (string.Equals(action, "setExtensionNumber", StringComparison.OrdinalIgnoreCase))
			{
				AsrSearchResult varValue = AsrSearchResult.Create(base.DtmfDigits);
				base.Manager.WriteVariable("searchResult", varValue);
			}
			else if (string.Equals(action, "handleRecognition", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "AsrSearchManager::HandleRecognition().", new object[0]);
				List<List<IUMRecognitionPhrase>> speechRecognitionResults = base.RecoResult.GetSpeechRecognitionResults();
				List<List<IUMRecognitionPhrase>> list = this.searchContext.ProcessMultipleResults(speechRecognitionResults);
				if (list == null || list.Count == 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "AsrSearchManager::HandleRecognition - did not find any valid results. Doing fallback.", new object[0]);
					input = "invalidSearchResult";
				}
				else
				{
					input = this.currentMenu.HandleRecognition(vo, list);
					this.searchContext.OnNameSpoken();
				}
			}
			else if (string.Equals(action, "handleYes", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleYes(vo);
			}
			else if (string.Equals(action, "handleNo", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleNo(vo);
			}
			else if (string.Equals(action, "handleNotListed", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleNotListed(vo);
			}
			else if (string.Equals(action, "handleNotSure", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleNotSure(vo);
			}
			else if (string.Equals(action, "handleChoice", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleChoice(vo);
			}
			else if (string.Equals(action, "handleDtmfChoice", StringComparison.OrdinalIgnoreCase))
			{
				input = this.currentMenu.HandleDtmfChoice(vo);
			}
			else
			{
				if (!string.Equals(action, "resetSearchState", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				this.ClearState();
				input = null;
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			base.SetNavigationFailure("Hangup during AsrSearch");
			base.OnUserHangup(vo, voiceEventArgs);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsrSearchManager>(this);
		}

		private void ChangeState(AsrSearchManager.MenuBase newMenu)
		{
			this.currentMenu = newMenu;
		}

		private void ClearState()
		{
			this.nbestPhase = NBestPhase.Phase1;
			this.menuState = AsrSearchManager.MenuState.OpeningMenu;
			this.previousState = AsrSearchManager.MenuState.None;
			this.confirmViaListQA = false;
			this.ChangeState(new AsrSearchManager.OpeningMenu(this, this.searchContext));
		}

		internal const float MaxConfidence = 1f;

		internal const float HighConfidence = 0.7f;

		internal const float LowConfidence = 0.2f;

		internal const float DeltaConfidence = 0.3f;

		private const int NumberOfResultsToShow = 9;

		private AsrSearchManager.MenuState menuState;

		private AsrSearchManager.MenuState previousState;

		private AsrSearchManager.MenuBase currentMenu;

		private AsrSearchContext searchContext;

		private NBestPhase nbestPhase;

		private bool confirmViaListQA;

		internal enum MenuState
		{
			None,
			OpeningMenu,
			ConfirmQA,
			ConfirmAgainQA,
			ConfirmViaListQA,
			AskAgainQA,
			CollisionQA
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing ASR search activity manager.", new object[0]);
				return new AsrSearchManager(manager, this);
			}
		}

		private abstract class MenuBase
		{
			protected MenuBase(AsrSearchManager manager, AsrSearchContext context)
			{
				this.manager = manager;
				this.context = context;
			}

			protected AsrSearchManager Manager
			{
				get
				{
					return this.manager;
				}
			}

			protected AsrSearchContext Context
			{
				get
				{
					return this.context;
				}
			}

			internal static List<List<IUMRecognitionPhrase>> ConvertToListOfLists(List<IUMRecognitionPhrase> results)
			{
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				foreach (IUMRecognitionPhrase item in results)
				{
					list.Add(new List<IUMRecognitionPhrase>
					{
						item
					});
				}
				return list;
			}

			internal virtual void Initialize(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Initialize", new object[0]);
			}

			internal virtual string HandleYes(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "ENTER: HandleYes(context.ResultsToPlay.Count={0}[{1}]).", new object[]
				{
					this.Context.ResultsToPlay.Count,
					this.Context.ResultsToPlay[0].Count
				});
				return this.CommonYesHandler(vo, this.Context.ResultsToPlay[0]);
			}

			internal virtual string HandleNo(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleNo().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string HandleNotSure(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleNotSure().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string HandleNotListed(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleNotListed().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string HandleRecognition(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleRecognition().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string HandleChoice(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string HandleDtmfChoice(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleDtmfChoice().", new object[0]);
				throw new NotImplementedException();
			}

			internal virtual string CommonNameCollisionHandler(List<IUMRecognitionPhrase> alternates)
			{
				string text;
				if (alternates.Count > Constants.DirectorySearch.MaxResultsToDisplay)
				{
					text = "resultsMoreThanAllowed";
				}
				else if (this.Context.CanShowExactMatches())
				{
					this.Manager.ChangeState(new AsrSearchManager.CollisionQA(this.Manager, this.Context));
					text = "collision";
					this.Context.PrepareForCollisionQA(AsrSearchManager.MenuBase.ConvertToListOfLists(alternates));
				}
				else
				{
					this.Context.PrepareForPromptForAliasQA(alternates);
					this.Manager.ChangeState(new AsrSearchManager.PromptForAliasQA(this.Manager, this.Context));
					text = "promptForAlias";
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonNameCollisionHandler(#homohones = {0}) returning autoEvent {1}.", new object[]
				{
					alternates.Count,
					text
				});
				return text;
			}

			internal virtual string CommonConfirmViaListHandler(List<List<IUMRecognitionPhrase>> alternates)
			{
				string text;
				if (this.Context.CanShowExactMatches())
				{
					if (alternates[0].Count > Constants.DirectorySearch.MaxResultsToDisplay)
					{
						text = "resultsMoreThanAllowed";
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonConfirmViaListHandler(alternates[0].Count = {0}) returning autoEvent {1}.", new object[]
						{
							alternates[0].Count,
							text
						});
						return text;
					}
					List<List<IUMRecognitionPhrase>> results = AsrSearchManager.MenuBase.FlattenList(alternates);
					this.Context.PrepareForConfirmViaListQA(results);
					this.Manager.ChangeState(new AsrSearchManager.ConfirmViaListQA(this.Manager, this.Context));
					text = "confirmViaList";
				}
				else
				{
					this.Context.PrepareForConfirmViaListQA(alternates);
					this.Manager.ChangeState(new AsrSearchManager.ConfirmViaListQA(this.Manager, this.Context));
					text = "confirmViaList";
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonConfirmViaListHandler(#homophones = {0}) returning autoEvent {1}.", new object[]
				{
					alternates.Count,
					text
				});
				return text;
			}

			protected static List<List<IUMRecognitionPhrase>> GetTopNEntries(List<List<IUMRecognitionPhrase>> alternates, int n)
			{
				int num = 0;
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				foreach (List<IUMRecognitionPhrase> item in alternates)
				{
					num++;
					if (num > n)
					{
						break;
					}
					list.Add(item);
				}
				return list;
			}

			protected static bool TryParseBookmark(string bookmark, out int index)
			{
				index = -1;
				bool result = false;
				if (bookmark != null && bookmark.StartsWith("user", StringComparison.Ordinal))
				{
					string s = bookmark.Substring(4);
					result = int.TryParse(s, out index);
				}
				return result;
			}

			protected string DoNBest_Phase2(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "DoNBest_Phase2() alternates.Count = {0}.", new object[]
				{
					(alternates != null) ? alternates.Count : -1
				});
				this.Context.PrepareForNBestPhase2();
				string result = null;
				List<List<IUMRecognitionPhrase>> list = this.RemoveRejectedPhaseOneNames(alternates);
				if (list.Count == 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "No results after removing rejected phase-1 names. Returning to search opening menu.", new object[0]);
					return "retrySearch";
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "DoNBest_Phase2() currentResults.Count = {0}.", new object[]
				{
					list.Count
				});
				list.Sort(RecognitionPhraseListComparer.StaticInstance);
				if (this.Manager.confirmViaListQA)
				{
					List<List<IUMRecognitionPhrase>> topNEntries = AsrSearchManager.MenuBase.GetTopNEntries(list, 1);
					this.Manager.ChangeState(new AsrSearchManager.ConfirmAgainQA(this.Manager, this.Context));
					if (!this.Context.PrepareForConfirmAgainQA(topNEntries))
					{
						result = "invalidSearchResult";
					}
					return result;
				}
				List<List<IUMRecognitionPhrase>> resultsInConfidenceRange = AsrSearchManager.MenuBase.GetResultsInConfidenceRange(list, 0.7f, 1f);
				if (resultsInConfidenceRange.Count > 0)
				{
					List<List<IUMRecognitionPhrase>> topNEntries2 = AsrSearchManager.MenuBase.GetTopNEntries(resultsInConfidenceRange, 9);
					if (topNEntries2.Count > 1)
					{
						return this.CommonConfirmViaListHandler(topNEntries2);
					}
					if (!this.Context.PrepareForConfirmAgainQA(topNEntries2))
					{
						result = "invalidSearchResult";
					}
					this.Manager.ChangeState(new AsrSearchManager.ConfirmAgainQA(this.Manager, this.Context));
					return result;
				}
				else
				{
					List<List<IUMRecognitionPhrase>> resultsInConfidenceRange2 = AsrSearchManager.MenuBase.GetResultsInConfidenceRange(list, 0.2f, 0.7f);
					if (resultsInConfidenceRange2.Count > 0)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Got Medium confidence results (count={0}), working with the topmost result.", new object[]
						{
							resultsInConfidenceRange2.Count
						});
						list = resultsInConfidenceRange2.GetRange(0, 1);
						if (!this.Context.PrepareForConfirmAgainQA(list))
						{
							result = "invalidSearchResult";
						}
						this.Manager.ChangeState(new AsrSearchManager.ConfirmAgainQA(this.Manager, this.Context));
						return result;
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Phase2 processing did not return any results for the user. Doing fallback processing.", new object[0]);
					return "doFallback";
				}
			}

			protected string CommonYesHandler(BaseUMCallSession vo, List<IUMRecognitionPhrase> resultList)
			{
				string text = null;
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonYesHandler(): resultList.Count: {0}.", new object[]
				{
					resultList.Count
				});
				if (this.Context.ResultsToPlay.Count == 1 && resultList.Count == 1)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonYesHandler(): have one result in ResultsToPlay. Going to transfer.", new object[0]);
					AsrSearchResult varValue = AsrSearchResult.Create(resultList[0], vo.CurrentCallContext.CallerInfo, vo.CurrentCallContext.TenantGuid);
					this.Manager.Manager.WriteVariable("searchResult", varValue);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "CommonYesHandler(): have more than one result in ResultsToPlay. Going to NameCollision.", new object[0]);
					text = this.CommonNameCollisionHandler(resultList);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "LEAVE: CommonYesHandler() returning autoEvent = {0}.", new object[]
				{
					text
				});
				return text;
			}

			private static List<List<IUMRecognitionPhrase>> FlattenList(List<List<IUMRecognitionPhrase>> alternates)
			{
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				int num = Constants.DirectorySearch.MaxResultsToDisplay;
				int num2 = 0;
				while (num2 < alternates.Count && alternates[num2].Count <= num)
				{
					for (int i = 0; i < alternates[num2].Count; i++)
					{
						list.Add(new List<IUMRecognitionPhrase>
						{
							alternates[num2][i]
						});
						num--;
						if (num == 0)
						{
							break;
						}
					}
					if (num == 0)
					{
						break;
					}
					num2++;
				}
				return list;
			}

			private static bool IsPhraseInConfidenceRange(IUMRecognitionPhrase phrase, float low, float high)
			{
				return phrase.Confidence >= low && phrase.Confidence <= high;
			}

			private static List<List<IUMRecognitionPhrase>> GetResultsInConfidenceRange(List<List<IUMRecognitionPhrase>> alternates, float low, float high)
			{
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				foreach (List<IUMRecognitionPhrase> list2 in alternates)
				{
					IUMRecognitionPhrase iumrecognitionPhrase = list2[0];
					if (iumrecognitionPhrase != null && AsrSearchManager.MenuBase.IsPhraseInConfidenceRange(iumrecognitionPhrase, low, high))
					{
						list.Add(list2);
					}
				}
				return list;
			}

			private List<List<IUMRecognitionPhrase>> RemoveRejectedPhaseOneNames(List<List<IUMRecognitionPhrase>> alternates)
			{
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				Dictionary<string, IUMRecognitionPhrase> dictionary = new Dictionary<string, IUMRecognitionPhrase>();
				foreach (List<IUMRecognitionPhrase> list2 in this.Context.RejectedResults)
				{
					foreach (IUMRecognitionPhrase iumrecognitionPhrase in list2)
					{
						string text = (string)iumrecognitionPhrase["ResultType"];
						string a;
						if ((a = text) != null)
						{
							if (!(a == "DirectoryContact"))
							{
								if (!(a == "PersonalContact"))
								{
									if (a == "Department")
									{
										string key = (string)iumrecognitionPhrase["DepartmentName"];
										if (!dictionary.ContainsKey(key))
										{
											dictionary.Add(key, iumrecognitionPhrase);
										}
									}
								}
								else
								{
									string key2 = (string)iumrecognitionPhrase["ContactId"];
									if (!dictionary.ContainsKey(key2))
									{
										dictionary.Add(key2, iumrecognitionPhrase);
									}
								}
							}
							else
							{
								string key3 = (string)iumrecognitionPhrase["ObjectGuid"];
								if (!dictionary.ContainsKey(key3))
								{
									dictionary.Add(key3, iumrecognitionPhrase);
								}
							}
						}
					}
				}
				foreach (List<IUMRecognitionPhrase> list3 in alternates)
				{
					bool flag = false;
					List<IUMRecognitionPhrase> list4 = null;
					foreach (IUMRecognitionPhrase iumrecognitionPhrase2 in list3)
					{
						string text2 = (string)iumrecognitionPhrase2["ResultType"];
						string a2;
						if ((a2 = text2) != null)
						{
							if (!(a2 == "DirectoryContact"))
							{
								if (!(a2 == "PersonalContact"))
								{
									if (a2 == "Department")
									{
										string text3 = (string)iumrecognitionPhrase2["DepartmentName"];
										flag = dictionary.ContainsKey(text3);
										CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Department: {0} Rejected: {1}.", new object[]
										{
											text3,
											flag
										});
									}
								}
								else
								{
									string text4 = (string)iumrecognitionPhrase2["ContactId"];
									flag = dictionary.ContainsKey(text4);
									PIIMessage data = PIIMessage.Create(PIIType._PII, text4);
									CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, data, "ContactId: _PII Rejected: {0}.", new object[]
									{
										flag
									});
								}
							}
							else
							{
								string text5 = (string)iumrecognitionPhrase2["ObjectGuid"];
								string value = (string)iumrecognitionPhrase2["SMTP"];
								flag = dictionary.ContainsKey(text5);
								PIIMessage data2 = PIIMessage.Create(PIIType._EmailAddress, value);
								CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, data2, "Guid: {0} Email:_EmailAddress Rejected: {1}.", new object[]
								{
									text5,
									flag
								});
							}
						}
						if (!flag)
						{
							if (list4 == null)
							{
								list4 = new List<IUMRecognitionPhrase>();
							}
							list4.Add(iumrecognitionPhrase2);
						}
					}
					if (list4 != null && list4.Count > 0)
					{
						list.Add(list4);
					}
				}
				return list;
			}

			private AsrSearchManager manager;

			private AsrSearchContext context;
		}

		private class OpeningMenu : AsrSearchManager.MenuBase
		{
			internal OpeningMenu(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleRecognition(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "RecoResult = {0}[{1}].", new object[]
				{
					base.Manager.RecoResult.Confidence,
					base.Manager.RecoResult.Text
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Alternates.Count = {0}.", new object[]
				{
					alternates.Count
				});
				return this.DoNBest_Phase1(vo, alternates);
			}

			internal string DoNBest_Phase1(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "NBest_Phase1::(Alternates.Count == {0}).", new object[]
				{
					alternates.Count
				});
				List<List<IUMRecognitionPhrase>> list = null;
				if (alternates.Count > 1)
				{
					list = this.GetResultsInDeltaConfidenceRange(alternates, 0.3f);
				}
				List<List<IUMRecognitionPhrase>> topNEntries;
				if (list != null && list.Count > 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "NBest_Phase1::Got Delta Confidence Results, Count == {0}.", new object[]
					{
						list.Count
					});
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Getting max: {0} results from the deltaconfidence results", new object[]
					{
						9
					});
					topNEntries = AsrSearchManager.MenuBase.GetTopNEntries(list, 9);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Did not get deltaconfidence results, just getting the top 1.", new object[0]);
					topNEntries = AsrSearchManager.MenuBase.GetTopNEntries(alternates, 1);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "NBest_Phase1::Final Results (count = {0}).", new object[]
				{
					topNEntries.Count
				});
				string result;
				if (topNEntries.Count > 1)
				{
					result = this.CommonConfirmViaListHandler(topNEntries);
				}
				else
				{
					result = null;
					base.Context.ResultsToPlay = topNEntries.GetRange(0, 1);
					base.Manager.ChangeState(new AsrSearchManager.ConfirmQA(base.Manager, base.Context));
					if (!base.Context.PrepareForConfirmQA(base.Context.ResultsToPlay))
					{
						result = "invalidSearchResult";
					}
				}
				return result;
			}

			protected List<List<IUMRecognitionPhrase>> GetResultsInDeltaConfidenceRange(List<List<IUMRecognitionPhrase>> alternates, float deltaConfidence)
			{
				List<List<IUMRecognitionPhrase>> list = new List<List<IUMRecognitionPhrase>>();
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "GetResultsInDeltaConfidenceRange(alternates.Count = {0}, deltaConf = {1}).", new object[]
				{
					alternates.Count,
					deltaConfidence
				});
				float num = -1f;
				float num2 = -1f;
				float num3 = -1f;
				foreach (List<IUMRecognitionPhrase> list2 in alternates)
				{
					IUMRecognitionPhrase iumrecognitionPhrase = list2[0];
					PIIMessage data = PIIMessage.Create(PIIType._PII, iumrecognitionPhrase.Text);
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, data, "Root Phrase: {0}[_PII] Alternates.Count ={1}.", new object[]
					{
						iumrecognitionPhrase.Confidence,
						list2.Count
					});
					if (num == -1f)
					{
						num = iumrecognitionPhrase.Confidence;
						num2 = num + deltaConfidence;
						num2 = Math.Min(num2, 1f);
						num3 = Math.Max(0f, num - deltaConfidence);
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, data, "Phrase: {0}[_PII] Expected Range ={1}-{2}.", new object[]
					{
						iumrecognitionPhrase.Confidence,
						num3,
						num2
					});
					if (iumrecognitionPhrase.Confidence >= num3 && iumrecognitionPhrase.Confidence <= num2)
					{
						list.Add(list2);
					}
				}
				return list;
			}
		}

		private class ConfirmQA : AsrSearchManager.MenuBase
		{
			internal ConfirmQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleRecognition(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "ConfirmQA::handleNo - setting rejectedResults (count = {0}).", new object[]
				{
					base.Context.ResultsToPlay.Count
				});
				base.Manager.nbestPhase = NBestPhase.Phase2;
				base.Context.RejectedResults = base.Context.ResultsToPlay;
				return base.DoNBest_Phase2(vo, alternates);
			}

			internal override string HandleYes(BaseUMCallSession vo)
			{
				return base.HandleYes(vo);
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				base.Context.RejectedResults = base.Context.ResultsToPlay;
				base.Context.ResultsToPlay = null;
				base.Manager.ChangeState(new AsrSearchManager.AskAgainQA(base.Manager, base.Context));
				return null;
			}

			internal override string HandleNotSure(BaseUMCallSession vo)
			{
				return "doFallback";
			}
		}

		private class ConfirmAgainQA : AsrSearchManager.MenuBase
		{
			internal ConfirmAgainQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleYes(BaseUMCallSession vo)
			{
				return base.HandleYes(vo);
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				return "doFallback";
			}

			internal override string HandleNotSure(BaseUMCallSession vo)
			{
				return "doFallback";
			}
		}

		private class ConfirmViaListQA : AsrSearchManager.MenuBase
		{
			internal ConfirmViaListQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override void Initialize(BaseUMCallSession vo)
			{
				base.Manager.confirmViaListQA = true;
			}

			internal override string HandleNotListed(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Enter:HandleNotListed().", new object[0]);
				string text = null;
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int num = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out num))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() bookmark \"{0}\" was invalid.", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleNotListed(): User said Not Listed while list was being played. Stopping playback.", new object[0]);
					vo.StopPlayback();
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() returning AutoEvent: <null>", new object[0]);
					return null;
				}
				switch (base.Manager.nbestPhase)
				{
				case NBestPhase.Phase1:
					base.Context.RejectedResults = base.Context.ResultsToPlay;
					base.Context.ResultsToPlay = null;
					text = "doAskAgainQA";
					base.Manager.ChangeState(new AsrSearchManager.AskAgainQA(base.Manager, base.Context));
					break;
				case NBestPhase.Phase2:
					text = "doFallback";
					break;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() returning AutoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}

			internal override string HandleYes(BaseUMCallSession vo)
			{
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int choiceIndex = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out choiceIndex))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleYes() bookmark \"{0}\" was invalid", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleYes(): User said Yes while list was being played. Stopping playback", new object[0]);
					vo.StopPlayback();
					string text = null;
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleYes() returning AutoEvent: {0}", new object[]
					{
						text
					});
					return text;
				}
				return this.HandleChoice(vo, choiceIndex);
			}

			internal override string HandleChoice(BaseUMCallSession vo)
			{
				string s = base.Manager.RecoResult["Choice"] as string;
				int choiceIndex = int.Parse(s, CultureInfo.InvariantCulture);
				return this.HandleChoice(vo, choiceIndex);
			}

			internal override string HandleDtmfChoice(BaseUMCallSession vo)
			{
				int choiceIndex = int.Parse(base.Manager.DtmfDigits, CultureInfo.InvariantCulture);
				return this.HandleChoice(vo, choiceIndex);
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Enter:HandleNo().", new object[0]);
				string text = null;
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int num = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out num))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() bookmark \"{0}\" was invalid.", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() returning AutoEvent: <null>", new object[0]);
					return null;
				}
				switch (base.Manager.nbestPhase)
				{
				case NBestPhase.Phase1:
					base.Context.RejectedResults = base.Context.ResultsToPlay;
					base.Context.ResultsToPlay = null;
					text = "doAskAgainQA";
					base.Manager.ChangeState(new AsrSearchManager.AskAgainQA(base.Manager, base.Context));
					break;
				case NBestPhase.Phase2:
					text = "doFallback";
					break;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() returning AutoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}

			private string HandleChoice(BaseUMCallSession vo, int choiceIndex)
			{
				choiceIndex--;
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice: User Input: {0}.", new object[]
				{
					choiceIndex
				});
				if (choiceIndex >= base.Context.ResultsToPlay.Count)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Choice entered was wrong...", new object[0]);
					return "invalidSelection";
				}
				List<IUMRecognitionPhrase> list = base.Context.ResultsToPlay[choiceIndex];
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice: Alternates.Count: {0}.", new object[]
				{
					list.Count
				});
				string text;
				if (list.Count == 1)
				{
					AsrSearchResult varValue = AsrSearchResult.Create(list[0], vo.CurrentCallContext.CallerInfo, vo.CurrentCallContext.TenantGuid);
					base.Manager.Manager.WriteVariable("searchResult", varValue);
					text = "validChoice";
				}
				else
				{
					text = this.CommonNameCollisionHandler(list);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice returning autoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}
		}

		private class AskAgainQA : AsrSearchManager.MenuBase
		{
			internal AskAgainQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleRecognition(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				base.Manager.nbestPhase = NBestPhase.Phase2;
				return base.DoNBest_Phase2(vo, alternates);
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				return "doFallback";
			}
		}

		private class CollisionQA : AsrSearchManager.MenuBase
		{
			internal CollisionQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleNotListed(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Enter:HandleNotListed().", new object[0]);
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int num = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out num))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() bookmark \"{0}\" was invalid.", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleNotListed(): User said Not Listed while list was being played. Stopping playback.", new object[0]);
					vo.StopPlayback();
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() returning AutoEvent: <null>", new object[0]);
					return null;
				}
				string text = "doFallback";
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNotListed() returning AutoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}

			internal override string HandleYes(BaseUMCallSession vo)
			{
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int choiceIndex = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out choiceIndex))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleYes() bookmark \"{0}\" was invalid", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleYes(): User said Yes/Number-X at end of list. Stopping playback", new object[0]);
					vo.StopPlayback();
					string text = null;
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleYes() returning AutoEvent: {0}", new object[]
					{
						text
					});
					return text;
				}
				return this.HandleChoice(vo, choiceIndex);
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Enter:HandleNo().", new object[0]);
				string lastBookmarkReached = base.Manager.LastBookmarkReached;
				int num = -1;
				if (!AsrSearchManager.MenuBase.TryParseBookmark(lastBookmarkReached, out num))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() bookmark \"{0}\" was invalid.", new object[]
					{
						lastBookmarkReached
					});
					return null;
				}
				if (vo.IsDuringPlayback())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() returning AutoEvent: <null>", new object[0]);
					return null;
				}
				string text = "doFallback";
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Exit:HandleNo() returning AutoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}

			internal override string HandleChoice(BaseUMCallSession vo)
			{
				string s = base.Manager.RecoResult["Choice"] as string;
				int choiceIndex = int.Parse(s, CultureInfo.InvariantCulture);
				return this.HandleChoice(vo, choiceIndex);
			}

			internal override string HandleDtmfChoice(BaseUMCallSession vo)
			{
				int choiceIndex = int.Parse(base.Manager.DtmfDigits, CultureInfo.InvariantCulture);
				return this.HandleChoice(vo, choiceIndex);
			}

			private string HandleChoice(BaseUMCallSession vo, int choiceIndex)
			{
				string text = "validChoice";
				choiceIndex--;
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice: User Input: {0}.", new object[]
				{
					choiceIndex
				});
				if (choiceIndex >= base.Context.ResultsToPlay.Count)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Choice entered was wrong...", new object[0]);
					return "invalidSelection";
				}
				List<IUMRecognitionPhrase> list = base.Context.ResultsToPlay[choiceIndex];
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice: Alternates.Count: {0}.", new object[]
				{
					list.Count
				});
				AsrSearchResult varValue = AsrSearchResult.Create(list[0], vo.CurrentCallContext.CallerInfo, vo.CurrentCallContext.TenantGuid);
				base.Manager.Manager.WriteVariable("searchResult", varValue);
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "HandleChoice returning autoEvent: {0}.", new object[]
				{
					text
				});
				return text;
			}
		}

		private class PromptForAliasQA : AsrSearchManager.MenuBase
		{
			internal PromptForAliasQA(AsrSearchManager manager, AsrSearchContext context) : base(manager, context)
			{
			}

			internal override string HandleRecognition(BaseUMCallSession vo, List<List<IUMRecognitionPhrase>> alternates)
			{
				string text = null;
				PIIMessage data = PIIMessage.Create(PIIType._PII, base.Manager.RecoResult.Text);
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, data, "RecoResult = {0}[_PII].", new object[]
				{
					base.Manager.RecoResult.Confidence
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "Alternates.Count = {0}.", new object[]
				{
					alternates.Count
				});
				if (alternates.Count > 1)
				{
					text = "resultsMoreThanAllowed";
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "PromptForAliasQA::HandleRecognition() returning autoEvent = {0}", new object[]
					{
						text
					});
					return text;
				}
				base.Context.ResultsToPlay = alternates.GetRange(0, 1);
				base.Manager.ChangeState(new AsrSearchManager.ConfirmQA(base.Manager, base.Context));
				if (!base.Context.PrepareForConfirmQA(base.Context.ResultsToPlay))
				{
					text = "invalidSearchResult";
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrSearchTracer, this, "PromptForAliasQA::HandleRecognition() returning autoEvent = {0}", new object[]
				{
					text
				});
				return text;
			}

			internal override string HandleYes(BaseUMCallSession vo)
			{
				throw new NotImplementedException();
			}

			internal override string HandleChoice(BaseUMCallSession vo)
			{
				throw new NotImplementedException();
			}

			internal override string HandleDtmfChoice(BaseUMCallSession vo)
			{
				throw new NotImplementedException();
			}

			internal override string HandleNotListed(BaseUMCallSession vo)
			{
				throw new NotImplementedException();
			}

			internal override string HandleNo(BaseUMCallSession vo)
			{
				throw new NotImplementedException();
			}
		}
	}
}
