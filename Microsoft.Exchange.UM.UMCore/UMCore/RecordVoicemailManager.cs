using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecordVoicemailManager : CAMessageSubmissionManager
	{
		internal RecordVoicemailManager(ActivityManager manager, RecordVoicemailManager.ConfigClass config) : base(manager, config)
		{
			this.submitMessage = true;
			base.IsQuickMessage = manager.IsQuickMessage;
			manager.IsQuickMessage = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "RecordVoicemailManager constructed with fQuickMessage={0}.", new object[]
			{
				base.IsQuickMessage
			});
		}

		internal bool DrmIsEnabled
		{
			get
			{
				return this.drmIsEnabled;
			}
		}

		internal bool VoiceMailAnalysisWarningRequired { get; set; }

		internal bool AllowMarkAsPrivate { get; set; }

		private TranscriptionEnabledSetting TranscriptionEnabledInMailboxConfig { get; set; }

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			if (!base.IsQuickMessage && !vo.CurrentCallContext.IsCallAnswerCallsCounterIncremented)
			{
				vo.IncrementCounter(CallAnswerCounters.CallAnsweringCalls);
			}
			this.InitOperatorNumber(vo);
			base.WriteVariable("mode", refInfo);
			UMSubscriber umsubscriber = vo.CurrentCallContext.CalleeInfo as UMSubscriber;
			this.drmIsEnabled = (umsubscriber != null && umsubscriber.DRMPolicyForCA != DRMProtectionOptions.None);
			this.TranscriptionEnabledInMailboxConfig = ((vo.CurrentCallContext.UmSubscriberData != null) ? vo.CurrentCallContext.UmSubscriberData.TranscriptionEnabledInMailboxConfig : TranscriptionEnabledSetting.Disabled);
			bool flag;
			bool voiceMailAnalysisWarningRequired;
			RecordVoicemailManager.GetVoiceMailAnalysisSettings(vo.CurrentCallContext.CalleeInfo, this.TranscriptionEnabledInMailboxConfig, out flag, out voiceMailAnalysisWarningRequired);
			this.VoiceMailAnalysisWarningRequired = voiceMailAnalysisWarningRequired;
			this.AllowMarkAsPrivate = (this.drmIsEnabled || flag);
			base.IsSentImportant = false;
			base.MessageMarkedPrivate = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "RecordVoicemailManager::Start(refInfo={0}) DrmEnabled={1} VoiceMailAnalysisWarningRequired={2} AllowMarkAsPrivate={3}", new object[]
			{
				refInfo,
				this.drmIsEnabled,
				this.VoiceMailAnalysisWarningRequired,
				this.AllowMarkAsPrivate
			});
			base.Start(vo, refInfo);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs eventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Received DTMF input.", new object[0]);
			base.NumFailures = 0;
			string @string = Encoding.ASCII.GetString(eventArgs.DtmfDigits);
			if (string.Equals(@string, "faxtone", StringComparison.InvariantCulture))
			{
				this.faxCall = true;
			}
			base.CurrentActivity.OnInput(vo, eventArgs);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs eventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "User hangup.", new object[0]);
			if (this.submitMessage)
			{
				if (base.CurrentActivity is Record)
				{
					base.RecordContext.TotalSeconds += (int)eventArgs.RecordTime.TotalSeconds;
				}
				this.SubmitVoiceMail(vo);
			}
			base.OnUserHangup(vo, eventArgs);
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Disconnecting the call.", new object[0]);
			if (this.submitMessage)
			{
				this.SubmitVoiceMail(vo);
			}
			base.DropCall(vo, reason);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Record Voicemail Manager asked to do action {0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "getGreeting", StringComparison.OrdinalIgnoreCase))
			{
				if (!(vo.CurrentCallContext.CalleeInfo is UMSubscriber))
				{
					PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, vo.CurrentCallContext.CalleeInfo.DisplayName);
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "GetGreeting returning {0}, as user _DisplayName is not UM enabled, or has an incompatible version of the mailbox.", new object[]
					{
						"noGreeting"
					});
					input = "noGreeting";
				}
				else if (vo.CurrentCallContext.UmSubscriberData != null)
				{
					ITempWavFile greetingFile = vo.CurrentCallContext.UmSubscriberData.GreetingFile;
					if (greetingFile == null)
					{
						input = (vo.CurrentCallContext.UmSubscriberData.IsOOF ? "noGreetingOof" : "noGreeting");
					}
					this.UpdateCallSessionWithGreeting(vo, greetingFile, vo.CurrentCallContext.UmSubscriberData.TimedOut);
				}
				else
				{
					input = "noGreeting";
				}
			}
			else
			{
				if (string.Equals(action, "getName", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						object obj = this.GlobalManager.ReadVariable("userName");
						PIIMessage data2 = PIIMessage.Create(PIIType._Callee, vo.CurrentCallContext.CalleeInfo);
						if (obj != null && vo.CurrentCallContext.CallType == 2)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data2, "RecordVoicemainManager using recorded name set by AutoAttendant for user = _Callee.", new object[0]);
						}
						else
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data2, "Getting recorded name for user=_Callee.", new object[0]);
							base.SetRecordedName("userName", vo.CurrentCallContext.CalleeInfo.ADRecipient);
						}
						goto IL_1EF;
					}
					catch (ADTransientException ex)
					{
						base.SetTextPartVariable("userName", vo.CurrentCallContext.CalleeInfo.ADRecipient.DisplayName);
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Failed to retrieve the named greeting information. Exception: {0}.", new object[]
						{
							ex
						});
						goto IL_1EF;
					}
				}
				if (!string.Equals(action, "submitVoiceMail", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				this.SubmitVoiceMail(vo);
			}
			IL_1EF:
			return base.CurrentActivity.GetTransition(input);
		}

		internal string TogglePrivacy(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "TogglePrivacy", new object[0]);
			base.MessageMarkedPrivate = !base.MessageMarkedPrivate;
			return null;
		}

		internal string ToggleImportance(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "ToggleImportance", new object[0]);
			base.IsSentImportant = !base.IsSentImportant;
			return null;
		}

		internal string ClearSelection(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "ClearSelection", new object[0]);
			base.IsSentImportant = false;
			base.MessageMarkedPrivate = false;
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RecordVoicemailManager>(this);
		}

		private static void GetVoiceMailAnalysisSettings(UMRecipient recipient, TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig, out bool voiceMailAnalysisEnabled, out bool voiceMailAnalysisWarningRequired)
		{
			voiceMailAnalysisEnabled = false;
			voiceMailAnalysisWarningRequired = false;
			UMSubscriber umsubscriber = recipient as UMSubscriber;
			if (umsubscriber != null)
			{
				TranscriptionEnabledSetting transcriptionEnabledSetting = UMSubscriber.IsPartnerTranscriptionEnabled(umsubscriber.UMMailboxPolicy, transcriptionEnabledInMailboxConfig);
				bool flag = transcriptionEnabledSetting == TranscriptionEnabledSetting.Enabled;
				bool flag2 = umsubscriber.UMMailboxPolicy.AllowVoiceMailAnalysis && umsubscriber.ADUMMailboxSettings.VoiceMailAnalysisEnabled;
				voiceMailAnalysisEnabled = ((flag || flag2) && umsubscriber.DRMPolicyForCA != DRMProtectionOptions.All);
				voiceMailAnalysisWarningRequired = (voiceMailAnalysisEnabled && umsubscriber.UMMailboxPolicy.InformCallerOfVoiceMailAnalysis);
			}
		}

		private void InitOperatorNumber(BaseUMCallSession vo)
		{
			UMSubscriber targetUser = vo.CurrentCallContext.CalleeInfo as UMSubscriber;
			UMDialPlan dialPlan = vo.CurrentCallContext.DialPlan;
			PhoneNumber varValue = Util.SA_GetOperatorNumber(dialPlan, targetUser);
			base.WriteVariable("operatorNumber", varValue);
		}

		private void SubmitVoiceMail(BaseUMCallSession vo)
		{
			ITempWavFile recording = base.RecordContext.Recording;
			if (recording == null || base.RecordContext.TotalSeconds <= 0)
			{
				this.SubmitVoiceMail_MissedCall(vo);
			}
			else
			{
				this.SubmitVoiceMail_VoiceMessage(vo);
			}
			base.RecordContext.Reset();
		}

		private void SubmitVoiceMail_VoiceMessage(BaseUMCallSession vo)
		{
			ITempWavFile recording = base.RecordContext.Recording;
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "SubmitVoiceMail_VoiceMessage Importance = {0} IsFaxCall = {1} MessageDuration = {2}s. Privacy = {3}", new object[]
			{
				base.IsSentImportant,
				this.faxCall,
				base.RecordContext.TotalSeconds,
				base.MessageMarkedPrivate
			});
			if (this.faxCall && base.RecordContext.TotalSeconds <= 2)
			{
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "submitting voicemail message.  Context wav={0}, seconds={1}", new object[]
			{
				recording,
				base.RecordContext.TotalSeconds
			});
			RecordVoicemailManager.MessageSubmissionHelper messageSubmissionHelper = RecordVoicemailManager.MessageSubmissionHelper.Create(vo);
			messageSubmissionHelper.SubmitVoiceMessage(base.IsSentImportant, base.MessageMarkedPrivate, recording.FilePath, base.RecordContext.TotalSeconds, base.IsQuickMessage, this.TranscriptionEnabledInMailboxConfig);
			this.submitMessage = false;
			vo.CurrentCallContext.HasVoicemailBeenSubmitted = true;
			vo.SetCounter(CallAnswerCounters.AverageVoiceMessageSize, RecordVoicemailManager.averageMessageSize.Update((long)base.RecordContext.TotalSeconds));
			vo.SetCounter(CallAnswerCounters.AverageRecentVoiceMessageSize, RecordVoicemailManager.averageRecentMessageSize.Update((long)base.RecordContext.TotalSeconds));
		}

		private void SubmitVoiceMail_MissedCall(BaseUMCallSession vo)
		{
			if (!this.CanSendMissedCall())
			{
				return;
			}
			RecordVoicemailManager.MessageSubmissionHelper messageSubmissionHelper = RecordVoicemailManager.MessageSubmissionHelper.Create(vo);
			this.submitMessage = !messageSubmissionHelper.SubmitMissedCall(base.IsSentImportant);
		}

		private bool CanSendMissedCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CanSendMissedCall() PipelineHealthy = {0} IsMailboxOverQuota = {1} IsFaxCall = {2} IsQuickMessage={3}", new object[]
			{
				base.PipelineHealthy,
				base.IsMailboxOverQuota,
				this.faxCall,
				base.IsQuickMessage
			});
			if (base.IsQuickMessage || this.faxCall || !base.PipelineHealthy || base.IsMailboxOverQuota)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CanSendMissedCall() Missed call will not be sent", new object[0]);
				return false;
			}
			return true;
		}

		private void UpdateCallSessionWithGreeting(BaseUMCallSession vo, ITempWavFile tmpFile, bool timedOut)
		{
			if (timedOut)
			{
				vo.IncrementCounter(CallAnswerCounters.FetchGreetingTimedOut);
			}
			if (tmpFile == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "No greeting found.", new object[0]);
				vo.IncrementCounter(CallAnswerCounters.CallsWithoutPersonalGreetings);
				return;
			}
			base.WriteVariable("greeting", tmpFile);
			FileInfo fileInfo = new FileInfo(tmpFile.FilePath);
			vo.SetCounter(CallAnswerCounters.AverageGreetingSize, RecordVoicemailManager.averageGreetingSize.Update((long)Math.Round((double)fileInfo.Length / 16000.0)));
		}

		private static Average averageMessageSize = new Average();

		private static MovingAverage averageRecentMessageSize = new MovingAverage(50);

		private static Average averageGreetingSize = new Average();

		private bool submitMessage;

		private bool faxCall;

		private bool drmIsEnabled;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing Record Voicemail activity manager.", new object[0]);
				return new RecordVoicemailManager(manager, this);
			}
		}

		internal class MessageSubmissionHelper
		{
			private MessageSubmissionHelper(BaseUMCallSession callSession)
			{
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, callSession.CurrentCallContext.CalleeInfo.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "MessageSubmissionHelper::ctor() CallType = {0} Callee = _UserDisplayName", new object[]
				{
					callSession.CurrentCallContext.CallType
				});
				this.callSession = callSession;
				this.callContext = callSession.CurrentCallContext;
				this.callerId = this.callContext.CallerId;
				this.callerIdDisplayName = this.callContext.CallerIdDisplayName;
				this.codec = this.callContext.DialPlan.AudioCodec;
				ADUser aduser = this.callContext.CalleeInfo.ADRecipient as ADUser;
				if (aduser != null && aduser.CallAnsweringAudioCodec != null)
				{
					this.codec = aduser.CallAnsweringAudioCodec.Value;
				}
			}

			internal static RecordVoicemailManager.MessageSubmissionHelper Create(BaseUMCallSession callSession)
			{
				return new RecordVoicemailManager.MessageSubmissionHelper(callSession);
			}

			internal bool SubmitVoiceMessage(bool important, bool markedPrivate, string wavFilePath, int messageDuration, bool quickMessage, TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "MessageSubmissionHelper::SubmitVoiceMessage() Important = {0} WavFile = {1}, Duration = {2} IsQuickMessage = {3} Private = {4} Transcription Enabled In Mbx = {5}", new object[]
				{
					important,
					wavFilePath,
					messageDuration,
					quickMessage,
					markedPrivate,
					transcriptionEnabledInMailboxConfig
				});
				CallIdTracer.TracePfd(ExTraceGlobals.PFDUMCallAcceptanceTracer, this, "PFD UMC {0} - Submitting Message for Transport.", new object[]
				{
					15674
				});
				UMMessageSubmission.SubmitVoiceMail(this.callSession.CallId, this.callerId, this.callSession.CurrentCallContext.CallerInfo, this.callSession.CurrentCallContext.CalleeInfo, this.callSession.CurrentCallContext.DialPlan.DefaultLanguage.Culture, this.codec, messageDuration, wavFilePath, this.GetImportance(important), this.callSession.CurrentCallContext.OCFeature.Subject, !quickMessage, markedPrivate, transcriptionEnabledInMailboxConfig, null, this.callerIdDisplayName, this.callSession.CurrentCallContext.TenantGuid);
				this.callContext.CallLoggingHelper.IsCallAnsweringVoiceMessage = true;
				return true;
			}

			internal bool SubmitMissedCall(bool important)
			{
				UMSubscriber umsubscriber = (UMSubscriber)this.callContext.CalleeInfo;
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, this.callSession.CurrentCallContext.CalleeInfo.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, data, "MessageSubmissionHelper::SubmitMissedCall(CallType = {0}) Callee = _UserDisplayName", new object[]
				{
					this.callContext.CallType
				});
				if (this.callContext.CallType != 4 && this.callContext.CallType != 2)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid calltype '{0}' for missed call submission. Expected a calltype of CallAnswer or AutoAttendant", new object[]
					{
						this.callContext.CallType
					}));
				}
				bool flag = umsubscriber.IsMissedCallNotificationEnabled && this.callContext.OCFeature.FeatureType != OCFeatureType.VoiceMemo;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "MessageSubmissionHelper::SubmitMissedCall() IsMissedCallNotificationEnabled={0} OCFeature={1} fSendMissedCall={2}", new object[]
				{
					umsubscriber.IsMissedCallNotificationEnabled,
					this.callContext.OCFeature,
					flag
				});
				if (!flag)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "MessageSubmissionHelper::SubmitMissedCall() Missed call will not be sent", new object[0]);
					return false;
				}
				CallIdTracer.TracePfd(ExTraceGlobals.PFDUMCallAcceptanceTracer, this, "PFD UMC {0} - Submitting Message for Transport.", new object[]
				{
					15674
				});
				UMMessageSubmission.SubmitMissedCall(this.callSession.CallId, this.callerId, this.callContext.CallerInfo, umsubscriber, this.GetImportance(important), this.callContext.OCFeature.Subject, this.callerIdDisplayName, this.callContext.TenantGuid);
				return true;
			}

			private bool GetImportance(bool imp)
			{
				return imp || this.callContext.OCFeature.IsUrgent;
			}

			private BaseUMCallSession callSession;

			private CallContext callContext;

			private string callerIdDisplayName;

			private PhoneNumber callerId;

			private AudioCodecEnum codec;
		}
	}
}
