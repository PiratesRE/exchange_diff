using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SpeechMenu : Menu
	{
		internal SpeechMenu(ActivityManager manager, SpeechMenuConfig config) : base(manager, config)
		{
			this.config = config;
			this.state = SpeechMenuState.Main;
			this.silenceCount = (this.mumbleCount = (this.speechFailureCount = 0));
			this.shouldIgnoreShortCircuit = true;
			base.IsTestCall = false;
		}

		internal override void StartActivity(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Starting {0}.", new object[]
			{
				this
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FsmActivityStart, null, new object[]
			{
				this.ToString(),
				vo.CallId
			});
			if (!string.IsNullOrEmpty(refInfo))
			{
				this.SetInitialState(refInfo);
			}
			base.IsTestCall = vo.CurrentCallContext.IsTestCall;
			this.SpeechMenuStart(vo);
		}

		internal override void OnSpeech(object sender, UMSpeechEventArgs args)
		{
			base.Manager.PlayBackContext.Update(base.UniqueId, args.LastPrompt, args.PlayTime);
			base.Manager.DtmfDigits = string.Empty;
			base.Manager.NumericInput = 0;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SpeechMenu with id={0} received speech input.", new object[]
			{
				base.UniqueId
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SpeechMenu [id = {0}, Confidence={1}] result = {2} Confidence = {3} Bookmark={4}.", new object[]
			{
				base.UniqueId,
				this.config.Confidence,
				(args.Result != null) ? args.Result.ToString() : "<null>",
				(args.Result != null) ? args.Result.Confidence : -1f,
				args.BookmarkReached
			});
			BaseUMCallSession vo = (BaseUMCallSession)sender;
			if (args.Result == null || args.Result.Confidence < this.config.Confidence)
			{
				this.HandleMumble(vo, args);
				return;
			}
			this.HandleRecognition(vo, args);
		}

		internal override void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			base.Manager.PlayBackContext.Update(base.UniqueId, 0, TimeSpan.Zero);
			if (this.state != SpeechMenuState.DtmfFallback && this.state != SpeechMenuState.GoodbyeConfirm)
			{
				this.HandleSilence(vo);
				return;
			}
			this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
			this.state = SpeechMenuState.Main;
			this.SpeechMenuStart(vo);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			base.Manager.PlayBackContext.Update(base.UniqueId, voiceObjectEventArgs.LastPrompt, voiceObjectEventArgs.PlayTime);
			if (this.state == SpeechMenuState.GoodbyeConfirm)
			{
				this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
				this.state = SpeechMenuState.Main;
				this.SpeechMenuStart(vo);
				return;
			}
			if (string.Compare(base.Manager.LastRecoEvent, "recoFailure", StringComparison.OrdinalIgnoreCase) != 0 && this.state != SpeechMenuState.DtmfFallback)
			{
				TransitionBase transitionBase = base.ChooseTransition(vo, voiceObjectEventArgs);
				if (transitionBase != null)
				{
					base.TakeTransition(vo, voiceObjectEventArgs, transitionBase);
					return;
				}
				if (vo.IsDuringPlayback())
				{
					vo.StopPlayback();
					return;
				}
				this.state = SpeechMenuState.DtmfFallback;
				ArrayList prompts = GlobCfg.DefaultPromptHelper.Build(base.Manager, vo.CurrentCallContext.Culture, new PromptConfigBase[]
				{
					GlobCfg.DefaultPrompts.DtmfFallback
				});
				vo.PlayPrompts(prompts, 1, 1, 10, "0123456789#*ABCD", 1, StopPatterns.AnyKeyOnly, true, base.UniqueId, 6);
				return;
			}
			else
			{
				if (voiceObjectEventArgs.DtmfDigits[0] == Constants.SpeechMenu.DtmfFallbackKey)
				{
					base.Manager.SetNavigationFailure(base.UniqueId);
					base.Manager.UseASR = false;
					TransitionBase transition = this.config.GetTransition("dtmfFallback", base.Manager);
					transition.Execute(base.Manager, vo);
					return;
				}
				this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
				this.state = SpeechMenuState.Main;
				this.SpeechMenuStart(vo);
				return;
			}
		}

		protected override void PlayPrompts(ArrayList prompts, BaseUMCallSession vo)
		{
			int maxDigits = string.Equals(this.config.DtmfInputValue, "extension", StringComparison.OrdinalIgnoreCase) ? vo.CurrentCallContext.DialPlan.NumberOfDigitsInExtension : ((int)this.config.MaxDtmfSize);
			List<UMGrammar> grammars = this.config.GetGrammars(base.Manager, vo.CurrentCallContext.Culture);
			IUMSpeechRecognizer iumspeechRecognizer = (IUMSpeechRecognizer)vo;
			if (string.Compare(base.Manager.PlayBackContext.Id, base.UniqueId, true, CultureInfo.InvariantCulture) != 0)
			{
				this.InitializePrompts(prompts, base.IsTestCall);
				base.Manager.PlayBackContext.Update(prompts);
				iumspeechRecognizer.PlayPrompts(prompts, (int)this.config.MinDtmfSize, maxDigits, this.config.InputTimeout, this.config.DtmfStopTones, this.config.InterDigitTimeout, this.config.ComputeStopPatterns(base.Manager), 0, TimeSpan.Zero, grammars, 0 < grammars.Count, this.config.BabbleSeconds, this.config.StopPromptOnBargeIn, string.Equals(this.config.DtmfInputValue, "password", StringComparison.OrdinalIgnoreCase) ? null : base.UniqueId, this.config.InitialSilenceTimeout);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayBackContext={0} being applied to activity={1}.", new object[]
			{
				base.Manager.PlayBackContext,
				this
			});
			prompts = base.Manager.PlayBackContext.Prompts;
			int lastPrompt = base.Manager.PlayBackContext.LastPrompt;
			TimeSpan offset = base.Manager.PlayBackContext.Offset;
			base.SetProsodyRates(prompts);
			base.SetLanguages(prompts);
			base.Manager.PlayBackContext.Reset();
			base.Manager.PlayBackContext.Update(prompts);
			if (base.Manager.PlayBackContext.LastPrompt < prompts.Count)
			{
				iumspeechRecognizer.PlayPrompts(prompts, (int)this.config.MinDtmfSize, maxDigits, this.config.InputTimeout, this.config.DtmfStopTones, this.config.InterDigitTimeout, this.config.ComputeStopPatterns(base.Manager), lastPrompt, offset, grammars, 0 < grammars.Count, this.config.BabbleSeconds, this.config.StopPromptOnBargeIn, string.Equals(this.config.DtmfInputValue, "password", StringComparison.OrdinalIgnoreCase) ? null : base.UniqueId, this.config.InitialSilenceTimeout);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayBackActivity firing OnComplete event to itself.", new object[0]);
			this.OnComplete(vo, new UMCallSessionEventArgs
			{
				LastPrompt = lastPrompt
			});
		}

		private void SpeechMenuStart(BaseUMCallSession vo)
		{
			base.Manager.DtmfDigits = string.Empty;
			base.Manager.NumericInput = 0;
			if (this.state == SpeechMenuState.Goodbye)
			{
				base.Manager.ExecuteAction("disconnect", vo);
				return;
			}
			TransitionBase transitionBase = null;
			if (!this.shouldIgnoreShortCircuit)
			{
				transitionBase = this.GetSpeechStateTransition();
			}
			this.shouldIgnoreShortCircuit = false;
			if (transitionBase != null)
			{
				transitionBase.Execute(base.Manager, vo);
				return;
			}
			if (vo.IsDuringPlayback())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SpeechMenu with id={0} stopping playback.", new object[]
				{
					base.UniqueId
				});
				vo.StopPlayback();
				return;
			}
			IPromptCounter promptCounter = Menu.GetPromptCounter(vo);
			ArrayList arrayList = this.config.GetSpeechPrompts(base.Manager, this.state, vo.CurrentCallContext.Culture, promptCounter);
			if (promptCounter != null)
			{
				promptCounter.SavePromptCount();
			}
			if (this.isRepeatMode)
			{
				this.isRepeatMode = false;
				ArrayList c = arrayList;
				arrayList = GlobCfg.DefaultPromptHelper.Build(base.Manager, vo.CurrentCallContext.Culture, new PromptConfigBase[]
				{
					GlobCfg.DefaultPrompts.Repeat
				});
				arrayList.AddRange(c);
			}
			base.Manager.WriteVariable("repeat", false);
			int num = (int)(base.Manager.ReadVariable(base.ActivityId) ?? 0);
			base.Manager.WriteVariable(base.ActivityId, num + 1);
			this.PlayPrompts(arrayList, vo);
		}

		private void HandleRecognition(BaseUMCallSession vo, UMSpeechEventArgs args)
		{
			IUMRecognitionResult recoResult = base.Manager.RecoResult;
			base.Manager.RecoResult = args.Result;
			base.Manager.LastBookmarkReached = args.BookmarkReached;
			string text = args.Result["RecoEvent"] as string;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SpeechMenu handling recognition with result={0}.", new object[]
			{
				text
			});
			if (this.state == SpeechMenuState.GoodbyeConfirm)
			{
				if (string.Compare(text, "recoYes", StringComparison.Ordinal) != 0)
				{
					base.Manager.RecoResult = null;
					this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
					this.state = SpeechMenuState.Main;
					this.SpeechMenuStart(vo);
					return;
				}
				if (vo.IsDuringPlayback())
				{
					vo.StopPlayback();
					return;
				}
				this.state = SpeechMenuState.Goodbye;
				ArrayList prompts = GlobCfg.DefaultPromptHelper.Build(base.Manager, vo.CurrentCallContext.Culture, new PromptConfigBase[]
				{
					GlobCfg.DefaultPrompts.GoodBye
				});
				base.Manager.RecoResult = null;
				base.Manager.LastRecoEvent = "recoGoodbye";
				base.Manager.PlaySystemPrompt(prompts, vo);
				return;
			}
			else
			{
				if (string.Compare(text, "recoHelp", StringComparison.Ordinal) == 0)
				{
					this.mumbleCount = (this.silenceCount = (this.speechFailureCount = (base.NumFailures = 0)));
					this.state = SpeechMenuState.Help;
					this.SpeechMenuStart(vo);
					return;
				}
				if (string.Compare(text, "recoRepeat", StringComparison.Ordinal) == 0)
				{
					base.Manager.RecoResult = recoResult;
					this.mumbleCount = (this.silenceCount = (this.speechFailureCount = (base.NumFailures = 0)));
					this.isRepeatMode = true;
					base.Manager.WriteVariable("repeat", true);
					this.SpeechMenuStart(vo);
					return;
				}
				if (string.Compare(text, "recoMainMenu", StringComparison.Ordinal) == 0)
				{
					this.mumbleCount = (this.silenceCount = (this.speechFailureCount = (base.NumFailures = 0)));
					TransitionBase transition = this.config.GetTransition("recoMainMenu", base.Manager);
					transition.Execute(base.Manager, vo);
					return;
				}
				if (string.Compare(text, "recoGoodbye", StringComparison.Ordinal) == 0)
				{
					if (vo.IsDuringPlayback())
					{
						vo.StopPlayback();
						return;
					}
					base.Manager.ExecuteAction("pause", vo);
					this.state = SpeechMenuState.GoodbyeConfirm;
					this.mumbleCount = (this.silenceCount = (this.speechFailureCount = (base.NumFailures = 0)));
					base.Manager.RecoResult = null;
					ArrayList prompts2 = GlobCfg.DefaultPromptHelper.Build(base.Manager, vo.CurrentCallContext.Culture, new PromptConfigBase[]
					{
						GlobCfg.DefaultPrompts.GoodByeConfirmation
					});
					List<UMGrammar> list = new List<UMGrammar>();
					list.Add(GlobCfg.DefaultGrammars.GoodbyeConfirmation.GetGrammar(base.Manager, vo.CurrentCallContext.Culture));
					IUMSpeechRecognizer iumspeechRecognizer = (IUMSpeechRecognizer)vo;
					this.InitializePrompts(prompts2, base.IsTestCall);
					iumspeechRecognizer.PlayPrompts(prompts2, 0, 1, 10, string.Empty, 1, null, 0, TimeSpan.Zero, list, true, 10, true, base.UniqueId, 6);
					return;
				}
				else
				{
					TransitionBase transition;
					if ((transition = this.config.GetTransition(text, base.Manager)) != null)
					{
						this.mumbleCount = (this.silenceCount = (this.speechFailureCount = (base.NumFailures = 0)));
						transition.Execute(base.Manager, vo);
						return;
					}
					if (!args.WasPlaybackStopped)
					{
						this.speechFailureCount++;
					}
					if (this.speechFailureCount >= 3)
					{
						this.HandleFailure(vo);
						return;
					}
					this.state = SpeechMenuState.InvalidCommand;
					this.SpeechMenuStart(vo);
					return;
				}
			}
		}

		private void HandleMumble(BaseUMCallSession vo, UMSpeechEventArgs args)
		{
			base.Manager.RecoResult = null;
			if (this.state == SpeechMenuState.GoodbyeConfirm)
			{
				this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
				this.state = SpeechMenuState.Main;
				this.SpeechMenuStart(vo);
				return;
			}
			if (!args.WasPlaybackStopped)
			{
				this.mumbleCount++;
				this.speechFailureCount++;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleMumble sets mumbleCount={0}, speechFailureCount={1}, from state={2}.", new object[]
			{
				this.mumbleCount,
				this.speechFailureCount,
				this.state
			});
			if (this.speechFailureCount >= 3)
			{
				this.HandleFailure(vo);
				return;
			}
			if (1 == this.mumbleCount)
			{
				this.state = SpeechMenuState.Mumble1;
				this.SpeechMenuStart(vo);
				return;
			}
			this.state = SpeechMenuState.Mumble2;
			this.SpeechMenuStart(vo);
		}

		private void HandleSilence(BaseUMCallSession vo)
		{
			base.Manager.RecoResult = null;
			if (this.state == SpeechMenuState.GoodbyeConfirm)
			{
				this.silenceCount = (this.mumbleCount = (this.speechFailureCount = (base.NumFailures = 0)));
				this.state = SpeechMenuState.Main;
				this.SpeechMenuStart(vo);
				return;
			}
			this.silenceCount++;
			this.speechFailureCount++;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSilence sets silenceCount={0}, speechFailureCount={1}, from state={2}.", new object[]
			{
				this.silenceCount,
				this.speechFailureCount,
				this.state
			});
			if (this.speechFailureCount >= 3)
			{
				this.HandleFailure(vo);
				return;
			}
			if (1 == this.silenceCount)
			{
				this.state = SpeechMenuState.Silence1;
				this.SpeechMenuStart(vo);
				return;
			}
			this.state = SpeechMenuState.Silence2;
			this.SpeechMenuStart(vo);
		}

		private void HandleFailure(BaseUMCallSession vo)
		{
			if (vo.IsDuringPlayback())
			{
				vo.StopPlayback();
				return;
			}
			base.Manager.SetNavigationFailure(base.UniqueId);
			this.state = SpeechMenuState.SpeechError;
			IPromptCounter promptCounter = Menu.GetPromptCounter(vo);
			ArrayList speechPrompts = this.config.GetSpeechPrompts(base.Manager, this.state, vo.CurrentCallContext.Culture, promptCounter);
			if (promptCounter != null)
			{
				promptCounter.SavePromptCount();
			}
			base.Manager.RecoResult = null;
			base.Manager.LastRecoEvent = "recoFailure";
			base.Manager.PlaySystemPrompt(speechPrompts, vo);
		}

		private TransitionBase GetSpeechStateTransition()
		{
			TransitionBase transitionBase = null;
			switch (this.state)
			{
			case SpeechMenuState.Main:
				if (this.isRepeatMode)
				{
					transitionBase = this.config.GetTransition("repeat", base.Manager);
				}
				break;
			case SpeechMenuState.Help:
				transitionBase = this.config.GetTransition("help", base.Manager);
				break;
			case SpeechMenuState.Mumble1:
				transitionBase = this.config.GetTransition("mumble1", base.Manager);
				break;
			case SpeechMenuState.Mumble2:
				transitionBase = this.config.GetTransition("mumble2", base.Manager);
				break;
			case SpeechMenuState.Silence1:
				transitionBase = this.config.GetTransition("silence1", base.Manager);
				break;
			case SpeechMenuState.Silence2:
				transitionBase = this.config.GetTransition("silence2", base.Manager);
				break;
			case SpeechMenuState.SpeechError:
				transitionBase = this.config.GetTransition("speechError", base.Manager);
				if (transitionBase == null)
				{
					transitionBase = this.config.GetTransition("recoMainMenu", base.Manager);
				}
				else
				{
					base.Manager.LastRecoEvent = string.Empty;
				}
				break;
			case SpeechMenuState.InvalidCommand:
				transitionBase = this.config.GetTransition("invalidCommand", base.Manager);
				break;
			default:
				transitionBase = null;
				break;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SpeechMenu::GetSpeechStateTransition returns transition t={0}.", new object[]
			{
				transitionBase
			});
			return transitionBase;
		}

		private void SetInitialState(string stateName)
		{
			this.silenceCount = (this.mumbleCount = (this.speechFailureCount = 0));
			if (string.Compare(stateName, "main", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Main;
				return;
			}
			if (string.Compare(stateName, "help", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Help;
				return;
			}
			if (string.Compare(stateName, "mumble1", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Mumble1;
				this.mumbleCount = 1;
				this.speechFailureCount = 1;
				return;
			}
			if (string.Compare(stateName, "mumble2", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Mumble2;
				this.mumbleCount = 2;
				this.speechFailureCount = 2;
				return;
			}
			if (string.Compare(stateName, "silence1", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Silence1;
				this.silenceCount = 1;
				this.speechFailureCount = 1;
				return;
			}
			if (string.Compare(stateName, "silence2", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.Silence2;
				this.silenceCount = 2;
				this.speechFailureCount = 2;
				return;
			}
			if (string.Compare(stateName, "invalidCommand", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.state = SpeechMenuState.InvalidCommand;
				return;
			}
			throw new ArgumentException(stateName + " is not a valid intial state for a speech menu");
		}

		private const int MaxErrors = 3;

		private SpeechMenuConfig config;

		private SpeechMenuState state;

		private int silenceCount;

		private int mumbleCount;

		private int speechFailureCount;

		private bool isRepeatMode;

		private bool shouldIgnoreShortCircuit;
	}
}
