using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ActivityManager : ActivityBase
	{
		internal ActivityManager(ActivityManager manager, ActivityManagerConfig config) : base(manager, config)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, null, "ActivityManager being constructed manager={0}, config={1}.", new object[]
			{
				manager,
				config
			});
			this.contextVariables = new Hashtable();
			this.currentActivity = config.InitialActivity.CreateActivity(this);
			this.playBackCtx = new PlayBackContext();
			this.recCtx = new RecordContext();
			this.msgPlayerCtx = new MessagePlayerContext();
			if (manager != null)
			{
				this.RecoResult = manager.RecoResult;
				this.WriteVariable("lastActivity", base.Manager.LastActivity);
				this.WriteVariable("lastRecoEvent", base.Manager.LastRecoEvent);
				this.WriteVariable("useAsr", base.Manager.UseASR);
			}
		}

		internal bool IsSentImportant
		{
			get
			{
				return this.isSentImportant;
			}
			set
			{
				this.isSentImportant = value;
			}
		}

		internal bool MessageMarkedPrivate
		{
			get
			{
				return this.messageMarkedPrivate;
			}
			set
			{
				this.messageMarkedPrivate = value;
			}
		}

		internal bool IsQuickMessage
		{
			get
			{
				return this.isQuickMessage;
			}
			set
			{
				this.isQuickMessage = value;
			}
		}

		internal bool MessageHasBeenSentWithHighImportance { get; set; }

		internal PhoneNumber TargetPhoneNumber { get; set; }

		internal int NumericInput
		{
			get
			{
				return this.numberInput;
			}
			set
			{
				this.numberInput = value;
			}
		}

		internal string DtmfDigits
		{
			get
			{
				return this.dtmfDigits;
			}
			set
			{
				this.dtmfDigits = value;
				if (base.Manager != null)
				{
					base.Manager.DtmfDigits = value;
				}
			}
		}

		internal EncryptedBuffer Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		internal PlayBackContext PlayBackContext
		{
			get
			{
				return this.playBackCtx;
			}
			set
			{
				this.playBackCtx = value;
			}
		}

		internal RecordContext RecordContext
		{
			get
			{
				return this.recCtx;
			}
			set
			{
				this.recCtx = value;
			}
		}

		internal MessagePlayerContext MessagePlayerContext
		{
			get
			{
				return this.msgPlayerCtx;
			}
			set
			{
				this.msgPlayerCtx = value;
			}
		}

		internal Shortcut LastShortcut
		{
			get
			{
				return this.lastShortcut;
			}
			set
			{
				this.lastShortcut = value;
			}
		}

		internal BaseUMCallSession CallSession
		{
			get
			{
				return this.callSession;
			}
		}

		internal IUMRecognitionResult RecoResult
		{
			get
			{
				return this.recoResult;
			}
			set
			{
				this.recoResult = value;
				this.LastRecoEvent = ((value == null) ? string.Empty : (value["RecoEvent"] as string));
				if (base.Manager != null)
				{
					base.Manager.RecoResult = value;
				}
			}
		}

		internal string LastBookmarkReached
		{
			get
			{
				return this.lastBookmarkReached;
			}
			set
			{
				this.lastBookmarkReached = value;
				if (base.Manager != null)
				{
					base.Manager.LastBookmarkReached = value;
				}
			}
		}

		internal string LastRecoEvent
		{
			get
			{
				string text = this.ReadVariable("lastRecoEvent") as string;
				return text ?? string.Empty;
			}
			set
			{
				string text = value ?? string.Empty;
				this.WriteVariable("lastRecoEvent", text);
				if (base.Manager != null)
				{
					base.Manager.LastRecoEvent = text;
				}
			}
		}

		internal string LastActivity
		{
			get
			{
				string text = this.ReadVariable("lastActivity") as string;
				return text ?? string.Empty;
			}
			set
			{
				string text = value ?? string.Empty;
				this.WriteVariable("lastActivity", text);
				if (base.Manager != null)
				{
					base.Manager.LastActivity = text;
				}
			}
		}

		internal bool UseASR
		{
			get
			{
				object obj = this.ReadVariable("useAsr");
				return obj == null || (bool)obj;
			}
			set
			{
				this.WriteVariable("useAsr", value);
				if (base.Manager != null)
				{
					base.Manager.UseASR = value;
				}
			}
		}

		internal virtual DirectoryGrammarHandler DirectoryGrammarHandler
		{
			get
			{
				return this.GlobalManager.DirectoryGrammarHandler;
			}
		}

		internal virtual GlobalActivityManager GlobalManager
		{
			get
			{
				return base.Manager.GlobalManager;
			}
		}

		internal virtual bool LargeGrammarsNeeded
		{
			get
			{
				return false;
			}
		}

		internal virtual float ProsodyRate
		{
			get
			{
				return this.GlobalManager.ProsodyRate;
			}
			set
			{
				this.GlobalManager.ProsodyRate = value;
			}
		}

		internal int LastInputNum
		{
			get
			{
				int result = 0;
				if (int.TryParse((string)this.ReadVariable("lastInput"), NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return 0;
			}
		}

		protected ActivityBase CurrentActivity
		{
			get
			{
				return this.currentActivity;
			}
			set
			{
				this.currentActivity = value;
			}
		}

		protected bool PlayingLastMenu
		{
			get
			{
				return this.playingLastMenu;
			}
			set
			{
				this.playingLastMenu = value;
			}
		}

		protected bool PlayingSystemPrompt
		{
			get
			{
				return this.playingSystemPrompt;
			}
			set
			{
				this.playingSystemPrompt = value;
			}
		}

		protected IntroType MessageIntroType
		{
			get
			{
				return this.messageIntroType;
			}
			set
			{
				this.messageIntroType = value;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.navigationState = UMNavigationState.Success;
			this.navigationStateMessage = string.Empty;
			this.navigationLogger = vo.LoggingManager;
			this.navigationLogger.EnterTask(base.UniqueId);
			this.FetchGalGrammar(vo);
			this.callSession = vo;
			this.CheckAuthorization(vo.CurrentCallContext.CallerInfo);
			this.currentActivity.Start(vo, refInfo);
		}

		internal virtual void CheckAuthorization(UMSubscriber u)
		{
			if (!u.IsAuthenticated)
			{
				PIIMessage data = PIIMessage.Create(PIIType._User, u);
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, data, "User=_User unauthenticated in activity={0}.", new object[]
				{
					this
				});
				throw new UmAuthorizationException(u.ToString(), this.ToString());
			}
			if (this.GlobalManager.LimitedOVAAccess)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._User, u);
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, data2, "Activity {0} is not allowed in limited OVA Access mode. User = _User", new object[]
				{
					this
				});
				throw new UmAuthorizationException(u.ToString(), this.ToString());
			}
		}

		internal override void OnSpeech(object sender, UMSpeechEventArgs args)
		{
			BaseUMCallSession baseUMCallSession = (BaseUMCallSession)sender;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager={0} received speech.", new object[]
			{
				base.GetType()
			});
			this.currentActivity.OnSpeech(sender, args);
		}

		internal override void OnOutBoundCallRequestCompleted(BaseUMCallSession vo, OutboundCallDetailsEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Activity Manager : OnOutBoundCallRequestCompleted", new object[0]);
			this.currentActivity.OnOutBoundCallRequestCompleted(vo, callSessionEventArgs);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received DTMF input.", new object[0]);
			this.currentActivity.OnInput(vo, callSessionEventArgs);
		}

		internal override void OnCancelled(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "All operations cancelled.", new object[0]);
			this.currentActivity.OnCancelled(vo, callSessionEventArgs);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "User hangup.", new object[0]);
			if (this.playingLastMenu)
			{
				vo.DisconnectCall();
				return;
			}
			if (this.playingSystemPrompt)
			{
				this.playingSystemPrompt = false;
			}
			this.currentActivity.OnUserHangup(vo, callSessionEventArgs);
		}

		internal override void OnDispose(BaseUMCallSession vo, EventArgs e)
		{
			this.currentActivity.OnDispose(vo, e);
			this.Dispose();
		}

		internal override void OnComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Prompt completed.", new object[0]);
			if (this.playingLastMenu)
			{
				vo.DisconnectCall();
				return;
			}
			if (this.playingSystemPrompt)
			{
				this.playingSystemPrompt = false;
				this.currentActivity.Start(vo, null);
				return;
			}
			this.currentActivity.OnComplete(vo, callSessionEventArgs);
		}

		internal override void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Timeout.", new object[0]);
			if (this.playingLastMenu)
			{
				vo.DisconnectCall();
				return;
			}
			if (this.playingSystemPrompt)
			{
				this.playingSystemPrompt = false;
				this.currentActivity.Start(vo, null);
				return;
			}
			this.currentActivity.OnTimeout(vo, callSessionEventArgs);
		}

		internal override void OnStateInfoSent(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			base.OnStateInfoSent(vo, callSessionEventArgs);
			this.currentActivity.OnStateInfoSent(vo, callSessionEventArgs);
		}

		internal override void OnError(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			if (callSessionEventArgs.Error is UmAuthorizationException)
			{
				ExceptionHandling.SendWatsonWithoutDump(callSessionEventArgs.Error);
				callSessionEventArgs.Error = null;
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Manager {0} received UMauthorizationException. Dropping the call. ", new object[]
				{
					this
				});
				this.DropCall(vo, DropCallReason.SystemError);
				this.GetTransition("stopEvent").Execute(this, vo);
				return;
			}
			if (this.playingLastMenu)
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Manager {0} got an exception trying to play the last menu prompt. Giving up.", new object[]
				{
					this
				});
				vo.DisconnectCall();
				return;
			}
			this.currentActivity.OnError(vo, callSessionEventArgs);
			if (callSessionEventArgs.Error != null)
			{
				this.SetNavigationFailure((callSessionEventArgs.Error != null) ? callSessionEventArgs.Error.Message : string.Empty);
				if (this.HandleError(vo, callSessionEventArgs))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Manager {0} is handling error.", new object[]
					{
						this
					});
					return;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Manager {0} is unable to handles error. Bubbling Up.", new object[]
				{
					this
				});
			}
		}

		internal override void OnHold(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received Hold input.", new object[0]);
			this.currentActivity.OnHold(vo, callSessionEventArgs);
		}

		internal override void OnResume(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Received Hold input.", new object[0]);
			this.currentActivity.OnResume(vo, callSessionEventArgs);
		}

		internal virtual void PreActionExecute(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager.PreActionExecute()", new object[0]);
			UMMailboxRecipient ummailboxRecipient = vo.CurrentCallContext.CalleeInfo as UMMailboxRecipient;
			UMMailboxRecipient callerInfo = vo.CurrentCallContext.CallerInfo;
			if (ummailboxRecipient != null)
			{
				this.calleeSessionGuard = ummailboxRecipient.CreateConnectionGuard();
			}
			if (callerInfo != null)
			{
				this.callerSessionGuard = callerInfo.CreateConnectionGuard();
			}
		}

		internal virtual void PostActionExecute(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager.PostActionExecute()", new object[0]);
			if (this.calleeSessionGuard != null)
			{
				this.calleeSessionGuard.Dispose();
				this.calleeSessionGuard = null;
			}
			if (this.callerSessionGuard != null)
			{
				this.callerSessionGuard.Dispose();
				this.callerSessionGuard = null;
			}
		}

		internal virtual bool HandleError(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleError called.", new object[0]);
			TransitionBase transition = this.GetTransition(this.GetExceptionCategory(callSessionEventArgs));
			if (transition == null)
			{
				return false;
			}
			callSessionEventArgs.Error = null;
			transition.Execute(this, vo);
			return true;
		}

		internal override void OnTransferComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			base.OnTransferComplete(vo, callSessionEventArgs);
			this.currentActivity.OnTransferComplete(vo, callSessionEventArgs);
		}

		internal override void OnMessageReceived(BaseUMCallSession vo, InfoMessage.MessageReceivedEventArgs e)
		{
			base.OnMessageReceived(vo, e);
			this.currentActivity.OnMessageReceived(vo, e);
		}

		internal override void OnMessageSent(BaseUMCallSession vo, EventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Message sent.", new object[0]);
			this.currentActivity.OnMessageSent(vo, e);
		}

		internal override void OnHeavyBlockingOperation(BaseUMCallSession vo, HeavyBlockingOperationEventArgs hboea)
		{
			this.currentActivity.OnHeavyBlockingOperation(vo, hboea);
		}

		internal virtual void ChangeActivity(ActivityBase next, BaseUMCallSession vo, string refInfo)
		{
			if (this.currentActivity.UniqueId != next.UniqueId)
			{
				this.WriteVariable("more", false);
			}
			this.WriteVariable("currentActivity", next.ActivityId);
			this.LastActivity = this.currentActivity.ActivityId;
			this.currentActivity.Dispose();
			this.currentActivity = next;
			next.Start(vo, refInfo);
		}

		internal virtual void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			if (base.Manager != null)
			{
				base.Manager.DropCall(vo, reason);
			}
		}

		internal override TransitionBase GetTransition(string input)
		{
			TransitionBase transition = this.currentActivity.GetTransition(input);
			if (transition == null)
			{
				transition = base.Config.GetTransition(input, this);
			}
			return transition;
		}

		internal virtual TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager::ExecuteAction({0})", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "disconnect", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager dropping Call.", new object[0]);
				this.DropCall(vo, DropCallReason.GracefulHangup);
				input = "stopEvent";
			}
			else if (string.Equals(action, "more", StringComparison.OrdinalIgnoreCase))
			{
				object obj = this.ReadVariable("more");
				bool flag = obj == null || !(bool)obj;
				this.WriteVariable("more", flag);
			}
			else if (string.Equals(action, "stopASR", StringComparison.OrdinalIgnoreCase))
			{
				this.UseASR = false;
			}
			else if (string.Equals(action, "clearRecording", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Clearing current recording.", new object[0]);
				this.WriteVariable("recording", null);
				this.RecordContext.Reset();
			}
			else if (string.Equals(action, "appendRecording", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "setting append recording flag.", new object[0]);
				this.RecordContext.Append = true;
			}
			else if (string.Equals(action, "pause", StringComparison.OrdinalIgnoreCase))
			{
				this.PlayBackContext.Commit();
				this.PlayBackContext.Offset = this.PlayBackContext.Offset - TimeSpan.FromSeconds(5.0);
				if (this.PlayBackContext.Offset < TimeSpan.Zero)
				{
					this.PlayBackContext.Offset = TimeSpan.Zero;
				}
			}
			else if (string.Equals(action, "rewind", StringComparison.OrdinalIgnoreCase))
			{
				ActivityManager.Rewind(vo);
			}
			else if (string.Equals(action, "fastForward", StringComparison.OrdinalIgnoreCase))
			{
				ActivityManager.FastForward(vo);
			}
			else if (string.Equals(action, "speedUp", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SpeedUp(vo);
			}
			else if (string.Equals(action, "slowDown", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SlowDown(vo);
			}
			else if (string.Equals(action, "resetPlayback", StringComparison.OrdinalIgnoreCase))
			{
				this.PlayBackContext.Reset();
			}
			else if (string.Equals(action, "saveRecoEvent", StringComparison.OrdinalIgnoreCase))
			{
				this.WriteVariable("savedRecoEvent", this.LastRecoEvent);
			}
			else
			{
				if (!string.Equals(action, "setSpeechError", StringComparison.OrdinalIgnoreCase))
				{
					return base.Manager.ExecuteAction(action, vo);
				}
				base.Manager.LastRecoEvent = "recoFailure";
			}
			return this.currentActivity.GetTransition(input);
		}

		internal void RunHeavyBlockingOperation(BaseUMCallSession vo, HeavyBlockingOperation hbo)
		{
			CultureInfo culture = vo.CurrentCallContext.Culture;
			vo.RunHeavyBlockingOperation(hbo, GlobCfg.DefaultPromptHelper.Build(this, culture, GlobCfg.DefaultPrompts.ComfortNoise));
		}

		internal virtual object ReadVariable(string varName)
		{
			object obj = this.contextVariables[varName];
			if (obj == null && base.Manager != null)
			{
				obj = base.Manager.ReadVariable(varName);
			}
			return obj;
		}

		internal void WriteVariable(string varName, object varValue)
		{
			this.contextVariables[varName] = varValue;
		}

		internal void PlaySystemPrompt(ArrayList prompts, BaseUMCallSession vo)
		{
			if (vo.IsDuringPlayback())
			{
				vo.StopPlayback();
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Activity Manager playing system prompt.", new object[0]);
			this.playingSystemPrompt = true;
			if (vo.CurrentCallContext.IsTestCall)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < prompts.Count; i++)
				{
					stringBuilder.Append("\n");
					stringBuilder.Append(prompts[i].ToString());
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PromptsPlayed, null, new object[]
				{
					base.Config.ActivityId,
					stringBuilder.ToString()
				});
			}
			vo.PlayUninterruptiblePrompts(prompts);
		}

		internal void SetNavigationFailure(string message)
		{
			this.navigationState = UMNavigationState.Failure;
			this.navigationStateMessage = message;
		}

		internal void SetRecordedName(string variableName, string recipientName, IADRecipient r, bool disambiguate, DisambiguationFieldEnum disambiguationField, ref bool haveNameRecording)
		{
			object recordedName = this.GetRecordedName(recipientName, r, disambiguate, disambiguationField, ref haveNameRecording);
			this.WriteVariable(variableName, recordedName);
		}

		internal object GetRecordedName(string recipientName, IADRecipient r, bool disambiguate, DisambiguationFieldEnum disambiguationField, ref bool haveNameRecording)
		{
			haveNameRecording = false;
			string text = null;
			string text2 = null;
			object result = null;
			UMCoreADUtil.GetDisambiguatedNameForRecipient(r, recipientName, disambiguate, disambiguationField, out text, out text2);
			using (GreetingBase greetingBase = new ADGreeting(r as ADRecipient, "RecordedName"))
			{
				ITempWavFile tempWavFile = greetingBase.Get();
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, text);
				if (tempWavFile == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "Did not find a recorded name for aduser=_UserDisplayName.  Using tts.", new object[0]);
					this.CallSession.IncrementCounter(AvailabilityCounters.NameTTSed);
					if (text2 != null)
					{
						DisambiguatedName disambiguatedName = new DisambiguatedName(text, text2, disambiguationField);
						result = disambiguatedName;
					}
					else
					{
						result = text;
					}
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "Found a recorded name for aduser=_UserDisplayName.", new object[0]);
					this.CallSession.IncrementCounter(AvailabilityCounters.SpokenNameAccessed);
					result = tempWavFile;
					tempWavFile.ExtraInfo = text;
					haveNameRecording = true;
				}
			}
			return result;
		}

		internal string Repeat(BaseUMCallSession vo)
		{
			this.WriteVariable("repeat", true);
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityManager::Dispose.", new object[0]);
					this.contextVariables.Clear();
					this.Password = null;
					this.DtmfDigits = null;
					this.NumericInput = 0;
					this.PlayBackContext.Reset();
					this.RecordContext.Reset();
					if (this.navigationLogger != null)
					{
						this.navigationLogger.ExitTask(this.navigationState, this.navigationStateMessage);
					}
					if (this.currentActivity != null)
					{
						this.currentActivity.Dispose();
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityManager>(this);
		}

		protected object GetRecordedName(IADRecipient r)
		{
			bool flag = false;
			string recipientName;
			if ((recipientName = r.DisplayName) == null)
			{
				recipientName = (r.Alias ?? string.Empty);
			}
			return this.GetRecordedName(recipientName, r, false, DisambiguationFieldEnum.None, ref flag);
		}

		protected void SetTextPartVariable(string variableName, string recordedName)
		{
			this.WriteVariable(variableName, recordedName);
			this.WriteVariable("textPart", true);
			this.WriteVariable("wavePart", false);
		}

		protected void SetWavePartVariable(string variableName, ITempWavFile recordedName)
		{
			this.WriteVariable(variableName, recordedName);
			this.WriteVariable("textPart", false);
			this.WriteVariable("wavePart", true);
		}

		protected void SetRecordedName(string variableName, IADRecipient r)
		{
			bool flag = false;
			this.SetRecordedName(variableName, r, ref flag);
		}

		protected void SetRecordedName(string variableName, IADRecipient r, ref bool haveNameRecording)
		{
			string recipientName;
			if ((recipientName = r.DisplayName) == null)
			{
				recipientName = (r.Alias ?? string.Empty);
			}
			this.SetRecordedName(variableName, recipientName, r, false, DisambiguationFieldEnum.None, ref haveNameRecording);
		}

		protected string SelectLanguage()
		{
			CultureInfo language = CultureInfo.GetCultureInfo(this.RecoResult["Language"] as string);
			language = UmCultures.GetBestSupportedPromptCulture(language);
			return this.SetLanguage(language);
		}

		protected string NextLanguage(BaseUMCallSession vo)
		{
			CultureInfo cultureInfo = this.MessagePlayerContext.Language;
			if (cultureInfo == null)
			{
				cultureInfo = vo.CurrentCallContext.Culture;
			}
			List<CultureInfo> sortedSupportedPromptCultures = UmCultures.GetSortedSupportedPromptCultures(vo.CurrentCallContext.Culture);
			CultureInfo language = sortedSupportedPromptCultures[0];
			CultureInfo objA = null;
			foreach (CultureInfo cultureInfo2 in sortedSupportedPromptCultures)
			{
				if (object.Equals(objA, cultureInfo))
				{
					language = cultureInfo2;
					break;
				}
				objA = cultureInfo2;
			}
			return this.SetLanguage(language);
		}

		protected string SetLanguage(CultureInfo language)
		{
			string result = null;
			if (language != null)
			{
				this.MessagePlayerContext.Language = language;
				this.WriteVariable("messageLanguage", this.MessagePlayerContext.Language);
				this.WriteVariable("languageDetected", null);
			}
			else
			{
				result = "unknownLanguage";
			}
			return result;
		}

		protected void WriteReplyIntroType(IntroType introType)
		{
			this.WriteVariable("declineIntro", introType == IntroType.Decline);
			this.WriteVariable("replyIntro", introType == IntroType.Reply);
			this.WriteVariable("replyAllIntro", introType == IntroType.ReplyAll);
			this.WriteVariable("cancelIntro", introType == IntroType.Cancel || introType == IntroType.ClearCalendar);
			this.WriteVariable("forwardIntro", introType == IntroType.Forward);
			this.WriteVariable("clearCalendarIntro", introType == IntroType.ClearCalendar);
			this.messageIntroType = introType;
		}

		private static void Rewind(BaseUMCallSession vo)
		{
			vo.Skip(-Constants.SeekTime);
		}

		private static void FastForward(BaseUMCallSession vo)
		{
			vo.Skip(Constants.SeekTime);
		}

		private void FetchGalGrammar(BaseUMCallSession vo)
		{
			if (this.LargeGrammarsNeeded && this.UseASR && Util.IsSpeechCulture(vo.CurrentCallContext.Culture))
			{
				this.WriteVariable("namesGrammar", null);
			}
		}

		private string SpeedUp(BaseUMCallSession vo)
		{
			string result = null;
			float prosodyRate = this.ProsodyRate;
			this.ProsodyRate += 0.15f;
			if (this.ProsodyRate > 0.6f)
			{
				this.ProsodyRate = 0.6f;
				result = "maxProsodyRate";
			}
			this.PlayBackContext.Commit();
			this.PlayBackContext.Offset = this.PlayBackContext.Offset - TimeSpan.FromSeconds(2.0);
			if (this.PlayBackContext.Offset < TimeSpan.Zero)
			{
				this.PlayBackContext.Offset = TimeSpan.Zero;
			}
			return result;
		}

		private string SlowDown(BaseUMCallSession vo)
		{
			string result = null;
			float prosodyRate = this.ProsodyRate;
			this.ProsodyRate -= 0.15f;
			if (this.ProsodyRate < -0.6f)
			{
				this.ProsodyRate = -0.6f;
				result = "minProsodyRate";
			}
			this.PlayBackContext.Commit();
			this.PlayBackContext.Offset = this.PlayBackContext.Offset + TimeSpan.FromSeconds(2.0);
			if (this.PlayBackContext.Offset < TimeSpan.Zero)
			{
				this.PlayBackContext.Offset = TimeSpan.Zero;
			}
			return result;
		}

		private bool isSentImportant;

		private bool messageMarkedPrivate;

		private Shortcut lastShortcut;

		private bool isQuickMessage;

		private IUMRecognitionResult recoResult;

		private string lastBookmarkReached;

		private UMNavigationState navigationState;

		private string navigationStateMessage;

		private UMLoggingManager navigationLogger;

		private ActivityBase currentActivity;

		private Hashtable contextVariables;

		private int numberInput;

		private string dtmfDigits = string.Empty;

		private EncryptedBuffer password;

		private bool playingLastMenu;

		private bool playingSystemPrompt;

		private PlayBackContext playBackCtx;

		private RecordContext recCtx;

		private MessagePlayerContext msgPlayerCtx;

		private BaseUMCallSession callSession;

		private IntroType messageIntroType;

		private UMMailboxRecipient.MailboxConnectionGuard calleeSessionGuard;

		private UMMailboxRecipient.MailboxConnectionGuard callerSessionGuard;
	}
}
