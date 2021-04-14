using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class GlobalActivityManager : ActivityManager
	{
		internal GlobalActivityManager(GlobalActivityManager.ConfigClass globalConfig) : base(null, globalConfig)
		{
			this.ProsodyRate = 0f;
		}

		internal bool IsPromptProvisioningCall
		{
			get
			{
				return base.CallSession.CurrentCallContext.IsPromptProvisioningCall;
			}
		}

		internal bool SkipInitialGreetings
		{
			get
			{
				return this.skipInitialGreetings;
			}
		}

		internal bool ContactsAccessEnabled { get; private set; }

		internal bool DirectoryAccessEnabled { get; private set; }

		internal bool VoiceResponseToOtherMessageTypesEnabled { get; private set; }

		internal bool AddressBookEnabled { get; private set; }

		internal bool LimitedOVAAccess { get; private set; }

		internal bool ConsumerDialPlan { get; private set; }

		internal string DialPlanType { get; private set; }

		internal PersonalContactsGrammarFile PersonalContactsGrammarFile { get; set; }

		internal override DirectoryGrammarHandler DirectoryGrammarHandler
		{
			get
			{
				return this.directoryGrammarHandler;
			}
		}

		internal override GlobalActivityManager GlobalManager
		{
			get
			{
				return this;
			}
		}

		internal override float ProsodyRate
		{
			get
			{
				return this.prosodyRate;
			}
			set
			{
				this.prosodyRate = value;
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Global Manager executing action={0}.", new object[]
			{
				action
			});
			UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
			string text = null;
			if (string.Equals(action, "getExtension", StringComparison.OrdinalIgnoreCase))
			{
				if (vo.CurrentCallContext.IsTroubleshootingToolCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Troubleshooting Tool Call ", new object[0]);
					text = "troubleshootingToolCall";
				}
				else if (vo.CurrentCallContext.IsVirtualNumberCall)
				{
					PIIMessage data = PIIMessage.Create(PIIType._Callee, vo.CurrentCallContext.CalleeInfo.ToString());
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "Virtual Number call for : _Callee ", new object[0]);
					text = "virtualNumberCall";
				}
				else if (vo.CurrentCallContext.CalleeInfo != null)
				{
					PIIMessage data2 = PIIMessage.Create(PIIType._PhoneNumber, vo.CurrentCallContext.Extension);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data2, "Valid extension _PhoneNumber found.", new object[0]);
					text = "extensionFound";
				}
				else if (vo.CurrentCallContext.IsAutoAttendantCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Valid autoattendant {0} (SpeechEnabled = {1}) found.", new object[]
					{
						vo.CurrentCallContext.Extension,
						vo.CurrentCallContext.AutoAttendantInfo.SpeechEnabled
					});
					if (Util.UseAsrAutoAttendant(vo.CurrentCallContext.AutoAttendantInfo))
					{
						text = "validSpeechAutoAttendant";
						this.directoryGrammarHandler = DirectoryGrammarHandler.CreateHandler(DirectoryGrammarHandler.GetOrganizationId(vo.CurrentCallContext));
						this.directoryGrammarHandler.PrepareGrammarAsync(vo.CurrentCallContext, DirectoryGrammarHandler.GrammarType.User);
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Started Grammar Prefetch for Speech AA", new object[0]);
					}
					else
					{
						text = "validDtmfAutoAttendant";
					}
				}
				else if (vo.CurrentCallContext.IsDiagnosticCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "UmDiagnostic detected.", new object[0]);
					text = "umdiagnosticCall";
				}
				else if (vo.CurrentCallContext.IsPlayOnPhoneCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhoneCall detected.", new object[0]);
					text = "playOnPhone";
				}
				else if (vo.CurrentCallContext.IsFindMeSubscriberCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "FindMeSubscriberCall detected.", new object[0]);
					text = "findMeSubscriberCall";
				}
				else if (vo.CurrentCallContext.IsPlayOnPhonePAAGreetingCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PlayOnPhonePAACall detected", new object[0]);
					text = "playOnPhonePAAGreeting";
				}
				else if (!vo.CurrentCallContext.DivertedExtensionAllowVoiceMail)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DivertedExtensionNotAllowVoiceMail detected", new object[0]);
					text = "divertedExtensionNotAllowVoiceMail";
				}
			}
			else if (string.Equals(action, "createCallee", StringComparison.OrdinalIgnoreCase))
			{
				PIIMessage data3 = PIIMessage.Create(PIIType._PhoneNumber, vo.CurrentCallContext.CallerInfo.Extension);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "Initializing CalleeInfo from extension _PhoneNumber.", new object[0]);
				vo.CurrentCallContext.CalleeInfo = vo.CurrentCallContext.CallerInfo;
			}
			else if (string.Equals(action, "getName", StringComparison.OrdinalIgnoreCase))
			{
				object obj = this.ReadVariable("userName");
				PIIMessage data4 = PIIMessage.Create(PIIType._User, callerInfo);
				if (obj != null && vo.CurrentCallContext.CallType == 2)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data4, "GlobalManager using recorded name set by AutoAttendant for user = _User.", new object[0]);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data4, "Getting recorded name for user=_User.", new object[0]);
					base.SetRecordedName("userName", callerInfo.ADRecipient);
				}
			}
			else if (string.Equals(action, "doLogon", StringComparison.OrdinalIgnoreCase))
			{
				bool flag;
				text = this.DoLogon(callerInfo, vo, out flag);
				if (flag)
				{
					TransitionBase transitionBase = null;
					if (this.postLogonPendingTransition != null && (string.Equals(text, "logonAsr", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "logonOk", StringComparison.OrdinalIgnoreCase)))
					{
						transitionBase = this.postLogonPendingTransition;
					}
					this.postLogonPendingTransition = null;
					if (transitionBase != null)
					{
						return transitionBase;
					}
				}
			}
			else if (string.Equals(action, "validateMailbox", StringComparison.OrdinalIgnoreCase))
			{
				text = this.ValidateMailboxIsSubscriber(vo);
			}
			else if (string.Equals(action, "validateCaller", StringComparison.OrdinalIgnoreCase))
			{
				text = this.ValidateCallerIsSubscriber(vo);
			}
			else if (string.Equals(action, "clearCaller", StringComparison.OrdinalIgnoreCase))
			{
				UMSubscriber callerInfo2 = vo.CurrentCallContext.CallerInfo;
				if (callerInfo2 != null && !object.ReferenceEquals(callerInfo2, this.originalCaller))
				{
					PIIMessage[] data5 = new PIIMessage[]
					{
						PIIMessage.Create(PIIType._Caller, callerInfo2),
						PIIMessage.Create(PIIType._Caller, this.originalCaller)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data5, "reverting callerInfo from=_Caller1 to=_Caller2.", new object[0]);
					callerInfo2.Dispose();
					vo.CurrentCallContext.CallerInfo = this.originalCaller;
				}
				this.BlockLimitedOVAAccess();
				if (this.LimitedOVAAccess)
				{
					vo.CurrentCallContext.CallType = 1;
				}
				this.postLogonPendingTransition = null;
			}
			else if (string.Equals(action, "quickMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.QuickMessage(vo);
			}
			else if (string.Equals(action, "oofShortcut", StringComparison.OrdinalIgnoreCase))
			{
				base.LastShortcut = Shortcut.OOF;
				text = this.HandleIsPinRequired(vo);
			}
			else if (string.Equals(action, "getSummaryInfo", StringComparison.OrdinalIgnoreCase))
			{
				this.GetSummaryInformation(callerInfo);
			}
			else if (string.Equals(action, "handleCallSomeone", StringComparison.OrdinalIgnoreCase))
			{
				text = this.HandleCallSomeone(vo);
			}
			else if (string.Equals(action, "setInitialSearchTargetContacts", StringComparison.OrdinalIgnoreCase))
			{
				vo.IncrementCounter(SubscriberAccessCounters.ContactsAccessed);
				base.WriteVariable("initialSearchTarget", SearchTarget.PersonalContacts.ToString());
				text = this.HandleIsPinRequired(vo);
			}
			else if (string.Equals(action, "setInitialSearchTargetGAL", StringComparison.OrdinalIgnoreCase))
			{
				base.WriteVariable("initialSearchTarget", SearchTarget.GlobalAddressList.ToString());
				text = this.HandleIsPinRequired(vo);
			}
			else
			{
				if (!string.Equals(action, "setPromptProvContext", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				base.WriteVariable("promptProvContext", "DialPlan");
			}
			return base.CurrentActivity.GetTransition(text);
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			if (vo.CurrentCallContext.DialPlan != null)
			{
				this.DialPlanType = vo.CurrentCallContext.DialPlan.URIType.ToString();
				this.ConsumerDialPlan = (vo.CurrentCallContext.DialPlan.SubscriberType == UMSubscriberType.Consumer);
			}
			if (!this.isRegisteredForEvents)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Global Manager registering for BaseUMCallSession events.", new object[0]);
				vo.OnDtmf += this.OnInput;
				vo.OnComplete += this.OnComplete;
				vo.OnHangup += this.OnUserHangup;
				vo.OnTimeout += this.OnTimeout;
				vo.OnStateInfoSent += this.OnStateInfoSent;
				vo.OnError += this.OnError;
				vo.OnTransferComplete += this.OnTransferComplete;
				vo.OnFaxRequestReceive += this.OnFaxRequestReceive;
				vo.OnCancelled += this.OnCancelled;
				vo.OnHeavyBlockingOperation += this.OnHeavyBlockingOperation;
				vo.OnMessageReceived += this.OnMessageReceived;
				vo.OnMessageSent += this.OnMessageSent;
				vo.OnDispose += this.OnDispose;
				vo.OnOutboundCallRequestCompleted += this.OnOutBoundCallRequestCompleted;
				vo.OnHold += this.OnHold;
				vo.OnResume += this.OnResume;
				IUMSpeechRecognizer iumspeechRecognizer = vo as IUMSpeechRecognizer;
				if (iumspeechRecognizer != null)
				{
					iumspeechRecognizer.OnSpeech += new UMCallSessionHandler<UMSpeechEventArgs>(this.OnSpeech);
				}
				this.isRegisteredForEvents = true;
			}
			if (vo.CurrentCallContext.IsTestCall)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::Start: Test Call - sleeping for 2s to enable the test code to check the call connected event", new object[0]);
				Thread.Sleep(2000);
			}
			if (vo.CurrentCallContext.IsDiagnosticCall)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::Start: Diagnostic session", new object[0]);
				this.callTimer = vo.StartTimer(new BaseUMAsyncTimer.UMTimerCallback(this.MaximumCallTimeExceeded), 1800);
			}
			else
			{
				CallType callType = vo.CurrentCallContext.CallType;
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::Start: creating call timer.", new object[0]);
				this.callTimer = vo.StartTimer(new BaseUMAsyncTimer.UMTimerCallback(this.MaximumCallTimeExceeded), vo.CurrentCallContext.DialPlan.MaxCallDuration * 60);
				this.originalCaller = vo.CurrentCallContext.CallerInfo;
				base.WriteVariable("lastRecoEvent", string.Empty);
				base.WriteVariable("lastActivity", string.Empty);
				base.WriteVariable("defaultLanguage", vo.CurrentCallContext.Culture);
				List<CultureInfo> sortedSupportedPromptCultures = UmCultures.GetSortedSupportedPromptCultures(vo.CurrentCallContext.Culture);
				if (sortedSupportedPromptCultures.Count > 1)
				{
					base.WriteVariable("selectableLanguages", sortedSupportedPromptCultures);
				}
				UMDialPlan dialPlan = vo.CurrentCallContext.DialPlan;
				bool flag = callType == 3 || callType == 1;
				bool flag2 = flag && dialPlan.WelcomeGreetingEnabled && !string.IsNullOrEmpty(dialPlan.WelcomeGreetingFilename);
				base.WriteVariable("pilotNumberWelcomeGreetingEnabled", flag2);
				if (flag2)
				{
					base.WriteVariable("pilotNumberWelcomeGreetingFilename", vo.CurrentCallContext.UMConfigCache.GetPrompt<UMDialPlan>(dialPlan, dialPlan.WelcomeGreetingFilename));
				}
				flag2 = (flag && dialPlan.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False && !string.IsNullOrEmpty(dialPlan.InfoAnnouncementFilename));
				base.WriteVariable("pilotNumberInfoAnnouncementEnabled", flag2);
				if (flag2)
				{
					base.WriteVariable("pilotNumberInfoAnnouncementFilename", vo.CurrentCallContext.UMConfigCache.GetPrompt<UMDialPlan>(dialPlan, dialPlan.InfoAnnouncementFilename));
				}
				flag2 = (flag2 && dialPlan.InfoAnnouncementEnabled == InfoAnnouncementEnabledEnum.True);
				base.WriteVariable("pilotNumberInfoAnnouncementInterruptible", flag2);
				base.WriteVariable("tuiPromptEditingEnabled", dialPlan.TUIPromptEditingEnabled);
				bool waitForSourcePartyInfo = vo.WaitForSourcePartyInfo;
				base.WriteVariable("waitForSourcePartyInfo", waitForSourcePartyInfo);
				base.WriteVariable("contactSomeoneEnabled", dialPlan.CallSomeoneEnabled || dialPlan.SendVoiceMsgEnabled);
				base.WriteVariable("ocFeature", (vo.CurrentCallContext.OCFeature.FeatureType != OCFeatureType.None) ? vo.CurrentCallContext.OCFeature.FeatureType.ToString() : null);
			}
			base.WriteVariable("skipPinCheck", false);
			this.skipInitialGreetings = vo.CurrentCallContext.CallIsOVATransferForUMSubscriber;
			this.LimitedOVAAccess = false;
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::DropCall() reason = {0}", new object[]
			{
				reason
			});
			base.DropCall(vo, reason);
			CultureInfo culture = CommonConstants.DefaultCulture;
			if (vo == null)
			{
				return;
			}
			if (vo.CurrentCallContext != null)
			{
				vo.CurrentCallContext.ReasonForDisconnect = reason;
				culture = vo.CurrentCallContext.Culture;
			}
			if (reason != DropCallReason.GracefulHangup)
			{
				GlobalActivityManager.IncrementDroppedCallsCounter(vo, reason == DropCallReason.SystemError);
			}
			switch (reason)
			{
			case DropCallReason.UserError:
				break;
			case DropCallReason.SystemError:
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::DropCall: Playing system error prompts.", new object[0]);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SystemError, null, new object[]
				{
					vo.CallId
				});
				base.PlayingLastMenu = true;
				try
				{
					base.PlaySystemPrompt(GlobCfg.DefaultPromptHelper.Build(this, culture, new PromptConfigBase[]
					{
						GlobCfg.DefaultPrompts.SorrySystemError
					}), vo);
					return;
				}
				catch (InvalidOperationException ex)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::DropCall: Exception playing the final system error prompt {0}.", new object[]
					{
						ex
					});
					return;
				}
				break;
			case DropCallReason.GracefulHangup:
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::DropCall: due to GracefulHangup, skipping final prompts.", new object[0]);
				vo.DisconnectCall();
				return;
			default:
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::DropCall: Playing user error prompts.", new object[0]);
			base.PlayingLastMenu = true;
			base.PlaySystemPrompt(GlobCfg.DefaultPromptHelper.Build(this, culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPrompts.SorryTryAgainLater
			}), vo);
		}

		internal override void OnCancelled(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			if (this.maxCallSecondsExceeded)
			{
				base.PlayingLastMenu = true;
				base.PlaySystemPrompt(GlobCfg.DefaultPromptHelper.Build(this, vo.CurrentCallContext.Culture, GlobCfg.DefaultPrompts.MaxCallSecondsExceeded), vo);
				return;
			}
			base.OnCancelled(vo, voiceObjectEventArgs);
		}

		internal void OnFaxRequestReceive(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			FaxManager faxManager = (FaxManager)base.CurrentActivity;
			if (this.callTimer != null && this.callTimer.IsActive)
			{
				this.callTimer.Dispose();
				this.callTimer = null;
			}
			faxManager.OnFaxRequestReceive(vo, voiceObjectEventArgs);
		}

		internal override bool HandleError(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			if (!base.HandleError(vo, voiceObjectEventArgs))
			{
				this.DropCall(vo, DropCallReason.SystemError);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Global Manager unable to handle error. Dropping call.", new object[0]);
			}
			return true;
		}

		internal string HandleIsPinRequired(BaseUMCallSession vo)
		{
			if (this.LimitedOVAAccess)
			{
				base.WriteVariable("skipPinCheck", false);
				PIIMessage data = PIIMessage.Create(PIIType._Caller, vo.CurrentCallContext.CallerInfo);
				if (base.UseASR)
				{
					this.postLogonPendingTransition = base.CurrentActivity.GetTransition(base.LastRecoEvent);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "LimitedOVAAccess require login. Capturing ASR State. Caller=_Caller", new object[0]);
				}
				else
				{
					this.postLogonPendingTransition = base.CurrentActivity.GetTransition(base.LastInputNum.ToString(CultureInfo.InvariantCulture));
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "LimitedOVAAccess require login. Capturing DTMF State. Caller=_Caller", new object[0]);
				}
				return "forcePinLogin";
			}
			return null;
		}

		internal string FillCallerInfo(BaseUMCallSession vo)
		{
			vo.IncrementCounter(CallAnswerCounters.CallAnsweringEscapes);
			vo.CurrentCallContext.CallType = 1;
			this.BlockLimitedOVAAccess();
			try
			{
				vo.CurrentCallContext.CallerInfo = UMRecipient.Factory.FromExtension<UMSubscriber>(vo.CurrentCallContext.Extension, vo.CurrentCallContext.DialPlan, null);
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Unable to validate entered mailbox digits '{0}' due to exception e '{1}'", new object[]
				{
					vo.CurrentCallContext.Extension,
					ex
				});
			}
			return null;
		}

		internal string HandleCallSomeone(BaseUMCallSession vo)
		{
			UMDialPlan dialPlan = vo.CurrentCallContext.DialPlan;
			string text = null;
			if (CallSomeoneScopeEnum.Extension == dialPlan.ContactScope)
			{
				string extension = dialPlan.Extension;
				if (!string.IsNullOrEmpty(extension))
				{
					PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, extension);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "HandleCallSomeone::Found extension _PhoneNumber on dialplan.", new object[0]);
					PhoneUtil.SetTransferTargetPhone(this, TransferExtension.Operator, PhoneNumber.Parse(extension));
					text = "runCallExtension";
				}
			}
			else if (CallSomeoneScopeEnum.AutoAttendantLink == dialPlan.ContactScope)
			{
				ADObjectId umautoAttendant = dialPlan.UMAutoAttendant;
				string text2 = (umautoAttendant != null) ? umautoAttendant.DistinguishedName : null;
				if (!string.IsNullOrEmpty(text2))
				{
					PIIMessage data2 = PIIMessage.Create(PIIType._PhoneNumber, text2);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data2, "HandleCallSomeone::Found default autoattendant _PhoneNumber on dialplan.", new object[0]);
					if (vo.CurrentCallContext.SwitchToDefaultAutoAttendant())
					{
						bool speechEnabled = vo.CurrentCallContext.AutoAttendantInfo.SpeechEnabled;
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleCallSomeone::Executing autoattendant {0}, Speech={1}.", new object[]
						{
							text2,
							speechEnabled
						});
						if (speechEnabled)
						{
							text = "validSpeechAutoAttendant";
							this.directoryGrammarHandler = DirectoryGrammarHandler.CreateHandler(DirectoryGrammarHandler.GetOrganizationId(vo.CurrentCallContext));
							this.directoryGrammarHandler.PrepareGrammarAsync(vo.CurrentCallContext, DirectoryGrammarHandler.GrammarType.User);
							CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Started Grammar Prefetch for HandleCallSomeone : Switching to Speech AA", new object[0]);
						}
						else
						{
							text = "validDtmfAutoAttendant";
						}
					}
					else
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleCallSomeone::An unexpected error occured while transferring to the default AA in dialplan.", new object[0]);
					}
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleCallSomeone::Did not find a default autoattendant on dialplan {0} ", new object[]
					{
						dialPlan.Id.DistinguishedName
					});
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleCallSomeone:: Returning {0}.", new object[]
			{
				text
			});
			return text;
		}

		internal string HandleSourcePartyInfoDiversion(PlatformDiversionInfo diversionInfo)
		{
			string text = "noKey";
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSourcePartyInfoDiversion: Processing {0}", new object[]
			{
				diversionInfo.DiversionHeader
			});
			CallContext currentCallContext = base.CallSession.CurrentCallContext;
			currentCallContext.SetDiversionInfo(diversionInfo);
			if (currentCallContext.CallType == 4)
			{
				UMRecipient calleeInfo = currentCallContext.CalleeInfo;
				string extension = currentCallContext.Extension;
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._Callee, calleeInfo),
					PIIMessage.Create(PIIType._PhoneNumber, extension)
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "HandleSourcePartyInfoDiversion: Recipient:_Callee. Extension:_PhoneNumber", new object[0]);
				PlatformSipUri platformSipUri = null;
				IRedirectTargetChooser serverPicker = null;
				if (RouterUtils.TryGetReferDataForSourceSourcePartyInfoDiversion(currentCallContext, calleeInfo, extension, out platformSipUri, out serverPicker))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSourcePartyInfoDiversion: REFER needed. Referred-By:{0}", new object[]
					{
						platformSipUri
					});
					currentCallContext.ServerPicker = serverPicker;
					text = "mailboxNotSupported";
					base.WriteVariable("ReferredByUri", platformSipUri);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSourcePartyInfoDiversion: We can handle this call locally.", new object[0]);
				}
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSourcePartyInfoDiversion: Diversion didn't resolve to a CallAnswer call.", new object[0]);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HandleSourcePartyInfoDiversion: Returning {0}.", new object[]
			{
				text
			});
			return text;
		}

		internal string PrepareForTransferToServer(BaseUMCallSession vo)
		{
			PlatformSipUri platformSipUri;
			if (!RouterUtils.TryGetServerToServerReferTargetUri(vo.CurrentCallContext.DialPlan, vo.CurrentCallContext.ServerPicker, vo.CurrentCallContext.RequestUriOfCall, vo.CurrentCallContext.ToUriOfCall, vo.CurrentCallContext.IsSecuredCall, out platformSipUri))
			{
				return "noMoreServers";
			}
			string sipUri = platformSipUri.ToString();
			string phone = Utils.RemoveSIPPrefix(sipUri);
			PhoneNumber phoneNumber = new PhoneNumber(phone, PhoneNumberKind.Unknown, UMUriType.SipName);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PrepareForTransferToServer() setting URI={0}", new object[]
			{
				phoneNumber.ToDial
			});
			base.TargetPhoneNumber = phoneNumber;
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::Dispose.", new object[0]);
					if (this.callTimer != null && this.callTimer.IsActive)
					{
						this.callTimer.Dispose();
						this.callTimer = null;
					}
					if (0 < this.numLogonFailures)
					{
						base.CallSession.IncrementCounter(SubscriberAccessCounters.SubscriberLogonFailures);
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
			return DisposeTracker.Get<GlobalActivityManager>(this);
		}

		private static int GetNewVoicemailCount(UMSubscriber user)
		{
			int unreadCount;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = user.CreateSessionLock())
			{
				using (UMSearchFolder umsearchFolder = UMSearchFolder.Get(mailboxSessionLock.Session, UMSearchFolder.Type.VoiceMail))
				{
					unreadCount = umsearchFolder.UnreadCount;
				}
			}
			return unreadCount;
		}

		private static void IncrementDroppedCallsCounter(BaseUMCallSession vo, bool systemError)
		{
			if (!systemError)
			{
				vo.IncrementCounter(GeneralCounters.CallsDisconnectedByUserFailure);
				return;
			}
			vo.IncrementCounter(AvailabilityCounters.CallsDisconnectedOnIrrecoverableExternalError);
			switch (vo.CurrentCallContext.CallType)
			{
			case 2:
			{
				AutoAttendantCountersUtil autoAttendantCountersUtil = new AutoAttendantCountersUtil(vo);
				vo.IncrementCounter(autoAttendantCountersUtil.GetInstance().CallsDisconnectedOnIrrecoverableExternalError);
				return;
			}
			case 3:
				vo.IncrementCounter(SubscriberAccessCounters.CallsDisconnectedOnIrrecoverableExternalError);
				return;
			case 4:
				vo.IncrementCounter(CallAnswerCounters.CallsDisconnectedOnIrrecoverableExternalError);
				return;
			default:
				return;
			}
		}

		private void BlockLimitedOVAAccess()
		{
			if (!this.forbidLimitedOVAAccess)
			{
				this.forbidLimitedOVAAccess = true;
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "BlockLimitedOVAAccess() Block any future requests for Limited OVA Access.", new object[0]);
			}
		}

		private void MaximumCallTimeExceeded(BaseUMCallSession vo)
		{
			this.maxCallSecondsExceeded = true;
			vo.CancelPendingOperations();
			vo.IncrementCounter(GeneralCounters.CallDurationExceeded);
		}

		private string DoLogon(UMSubscriber user, BaseUMCallSession vo, out bool successfulLogon)
		{
			UmPasswordManager umPasswordManager = new UmPasswordManager(user);
			bool flag = (bool)this.ReadVariable("skipPinCheck");
			string result;
			if (flag)
			{
				successfulLogon = true;
				result = this.HandleSuccessfulLogon(user, vo, umPasswordManager);
			}
			else if (umPasswordManager.Authenticate(base.Password))
			{
				this.LimitedOVAAccess = false;
				successfulLogon = true;
				result = this.HandleSuccessfulLogon(user, vo, umPasswordManager);
			}
			else
			{
				successfulLogon = false;
				result = this.HandleFailedLogon(user, vo, umPasswordManager);
			}
			return result;
		}

		private string ValidateCallerIsSubscriber(BaseUMCallSession vo)
		{
			string result;
			try
			{
				string text = null;
				UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
				if (callerInfo != null)
				{
					PIIMessage[] data = new PIIMessage[]
					{
						PIIMessage.Create(PIIType._Caller, (callerInfo != null) ? callerInfo.ToString() : "<null>"),
						PIIMessage.Create(PIIType._PhoneNumber, (callerInfo != null) ? callerInfo.Extension : "<null>")
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "ValidateCallerIsSubscriber() ResolvedCaller = _Caller Extension = _PhoneNumber", new object[0]);
					PIIMessage data2 = PIIMessage.Create(PIIType._EmailAddress, callerInfo.MailAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data2, "ValidateCallerIsSubscriber() For Caller=_EmailAddress RequiresLegacyRedirectForSA={0}", new object[]
					{
						callerInfo.RequiresLegacyRedirectForSubscriberAccess
					});
				}
				PIIMessage data3 = PIIMessage.Create(PIIType._Caller, callerInfo);
				if (callerInfo == null || string.IsNullOrEmpty(callerInfo.Extension))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ValidateCallerIsSubscriber() Either caller was not found, is not supported on current server, or extension was not available", new object[0]);
					text = null;
				}
				else if (callerInfo.IsSubscriberAccessEnabled)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() Found caller=_Caller.", new object[0]);
					if (callerInfo.RequiresRedirectForSubscriberAccess())
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() caller=_Caller.requires redirect due to unsupported mailbox. This caller is not supported for callerid resolution for Subscriber access", new object[0]);
						text = null;
					}
					else if (!CommonUtil.ShouldAllowOVA(callerInfo.ADUser))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() caller=_Caller. Unable to create a ClientSecurityContext and we cannot login in his mailbox for this user.", new object[0]);
						text = null;
					}
					else
					{
						if (vo.CurrentCallContext.OCFeature.SkipPin)
						{
							base.WriteVariable("skipPinCheck", true);
							this.LimitedOVAAccess = false;
							CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() Found caller=_Caller. OC Pinless ", new object[0]);
						}
						else if (!this.forbidLimitedOVAAccess && (vo.CurrentCallContext.OCFeature.FeatureType == OCFeatureType.None || vo.CurrentCallContext.OCFeature.FeatureType == OCFeatureType.Voicemail) && callerInfo.IsLimitedOVAAccessAllowed(vo.CurrentCallContext.DialPlan, vo.CurrentCallContext.CallerId))
						{
							base.WriteVariable("skipPinCheck", true);
							this.LimitedOVAAccess = true;
							CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() Found caller=_Caller. VoicemailPinless ", new object[0]);
						}
						else
						{
							base.WriteVariable("skipPinCheck", false);
							this.LimitedOVAAccess = false;
							CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data3, "ValidateCallerIsSubscriber() Found caller=_Caller.", new object[0]);
						}
						text = "mailboxFound";
					}
				}
				result = text;
			}
			finally
			{
				this.BlockLimitedOVAAccess();
			}
			return result;
		}

		private string ValidateMailboxIsSubscriber(BaseUMCallSession vo)
		{
			string result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ValidateMailboxIsSubscriber() mailbox={0}.", new object[]
			{
				base.DtmfDigits
			});
			UMRecipient umrecipient = null;
			UMSubscriber umsubscriber = null;
			try
			{
				umrecipient = UMRecipient.Factory.FromExtension<UMRecipient>(base.DtmfDigits, vo.CurrentCallContext.DialPlan, null);
				umsubscriber = (umrecipient as UMSubscriber);
				if (umrecipient == null)
				{
					result = this.InvalidMailboxHandler(vo);
				}
				else
				{
					BricksRoutingBasedServerChooser bricksRoutingBasedServerChooser = new BricksRoutingBasedServerChooser(vo.CurrentCallContext, umrecipient, 3);
					PIIMessage data = PIIMessage.Create(PIIType._User, umrecipient);
					if (bricksRoutingBasedServerChooser.IsRedirectionNeeded)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "SetStateForTransferWithContext Recipient = '_User}'", new object[0]);
						result = "mailboxNotSupported";
						this.numMailboxFailures = 0;
						vo.CurrentCallContext.ServerPicker = bricksRoutingBasedServerChooser;
						UserTransferWithContext userTransferWithContext = new UserTransferWithContext(vo.CurrentCallContext.CallInfo.ApplicationAor);
						base.WriteVariable("ReferredByUri", userTransferWithContext.SerializeSACallTransferWithContextUri(base.DtmfDigits));
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ValidateMailboxIsSubscriber() unsupported mailbox ={0} call will now be referred to a UM server that supports this mailbox", new object[]
						{
							base.DtmfDigits
						});
					}
					else if (umsubscriber != null && umsubscriber.IsSubscriberAccessEnabled && CommonUtil.ShouldAllowOVA(umsubscriber.ADUser))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "ValidateMailboxIsSubscriber() Found caller=_User.", new object[0]);
						this.numMailboxFailures = 0;
						result = "mailboxFound";
					}
					else
					{
						result = this.InvalidMailboxHandler(vo);
					}
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Unable to validate entered mailbox digits '{0}' due to exception e '{1}'", new object[]
				{
					base.DtmfDigits,
					ex
				});
			}
			finally
			{
				if (umsubscriber != null)
				{
					vo.CurrentCallContext.CallerInfo = umsubscriber;
				}
				else if (umrecipient != null)
				{
					vo.CurrentCallContext.LegacySubscriber = umrecipient;
				}
			}
			return result;
		}

		private string InvalidMailboxHandler(BaseUMCallSession vo)
		{
			string result = null;
			vo.ClearDigits(1000);
			this.numMailboxFailures++;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Did not find user for mailbox={0}.  Failure count={1}.", new object[]
			{
				base.DtmfDigits,
				this.numMailboxFailures
			});
			int num = (vo.CurrentCallContext.DialPlan != null) ? vo.CurrentCallContext.DialPlan.InputFailuresBeforeDisconnect : 3;
			if (this.numMailboxFailures >= num)
			{
				result = "maxInvalidMailbox";
			}
			return result;
		}

		private string HandleSuccessfulLogon(UMSubscriber user, BaseUMCallSession vo, UmPasswordManager pwdManager)
		{
			user.IsAuthenticated = true;
			if (this.ReadVariable("promptProvContext") != null)
			{
				vo.CurrentCallContext.CallType = 8;
			}
			else
			{
				vo.CurrentCallContext.CallType = 3;
			}
			this.numLogonFailures = 0;
			base.WriteVariable("calendarAccessEnabled", user.HasCalendarFolder && user.IsCalenderAccessEnabled && user.IsSubscriberAccessEnabled);
			this.DirectoryAccessEnabled = (user.IsTUIAccessToDirectoryEnabled && user.IsSubscriberAccessEnabled);
			this.ContactsAccessEnabled = (user.HasContactsFolder && user.IsTUIAccessToContactsEnabled && user.IsSubscriberAccessEnabled);
			this.AddressBookEnabled = (this.ContactsAccessEnabled || this.DirectoryAccessEnabled);
			this.VoiceResponseToOtherMessageTypesEnabled = user.IsVoiceResponseToOtherMessageTypesEnabled;
			base.WriteVariable("emailAccessEnabled", user.IsEmailAccessEnabled && user.IsSubscriberAccessEnabled);
			PIIMessage data = PIIMessage.Create(PIIType._User, user);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User successfully authenticated to the um system.", new object[0]);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SuccessfulLogon, null, new object[]
			{
				user.ToString(),
				vo.CallId
			});
			vo.IncrementCounter(SubscriberAccessCounters.SubscriberLogons);
			bool flag = (bool)this.ReadVariable("skipPinCheck");
			if (this.ReadVariable("useAsr") == null)
			{
				base.UseASR = Util.UseAsrMenus(user, vo.CurrentCallContext.DialPlan);
			}
			string result;
			if (user.ConfigFolder.IsFirstTimeUser)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "User is a first time user.  Entering first time user dialog.", new object[0]);
				result = "firstTimeUserTask";
			}
			else if (pwdManager.IsExpired && !flag)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "User password is expired.  Entering change password dialog.", new object[0]);
				result = "changePasswordTask";
			}
			else if (base.Password != null && pwdManager.IsWeak(base.Password))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "User password is too weak for current policies. Entering change password dialog.", new object[0]);
				pwdManager.RequirePasswordToChangeAtFirstUse();
				result = "changePasswordTask";
			}
			else if (vo.CurrentCallContext.CallType == 8)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Finding logonPP transition.", new object[0]);
				result = "logonPP";
			}
			else if (base.UseASR)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Finding logonAsr transition.", new object[0]);
				result = "logonAsr";
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Finding logonOk transition.", new object[0]);
				result = "logonOk";
			}
			return result;
		}

		private string HandleFailedLogon(UMSubscriber user, BaseUMCallSession vo, UmPasswordManager pwdManager)
		{
			string result = null;
			this.numLogonFailures++;
			PIIMessage data = PIIMessage.Create(PIIType._User, user);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User logon failed.", new object[0]);
			vo.IncrementCounter(SubscriberAccessCounters.SubscriberAuthenticationFailures);
			if (pwdManager.IsLocked)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MailboxLocked, null, new object[]
				{
					user.ToString(),
					user.PasswordPolicy.LogonFailuresBeforeLockout.ToString(CultureInfo.InvariantCulture)
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User logon failed because mailbox is locked.  Disconnecting...", new object[0]);
				result = "badPasswordLockout";
			}
			else if (pwdManager.BadChecksum)
			{
				if (pwdManager.PinWasResetRecently)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User logon failed because their checksum is stale.", new object[0]);
					result = "staleChecksum";
				}
				else
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidChecksum, null, new object[]
					{
						user.ToString()
					});
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User logon failed because their checksum is invalid.", new object[0]);
					result = "badPasswordLockout";
				}
			}
			else if (pwdManager.PinResetNeeded)
			{
				Utils.ResetPassword(user, true, LockOutResetMode.None);
				result = "badPasswordReset";
			}
			else if (this.numLogonFailures >= vo.CurrentCallContext.DialPlan.LogonFailuresBeforeDisconnect)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_LogonDisconnect, null, new object[]
				{
					user.ToString(),
					this.numLogonFailures.ToString(CultureInfo.InvariantCulture)
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "User=_User logon failed {0} times.  Disconnecting...", new object[]
				{
					this.numLogonFailures
				});
				result = "badPasswordDisconnect";
			}
			return result;
		}

		private void QuickMessage(BaseUMCallSession vo)
		{
			base.IsQuickMessage = true;
			CallContext currentCallContext = vo.CurrentCallContext;
			ContactSearchItem contactSearchItem = (ContactSearchItem)this.ReadVariable("directorySearchResult");
			currentCallContext.CalleeInfo = UMRecipient.Factory.FromADRecipient<UMRecipient>(contactSearchItem.Recipient);
			currentCallContext.AsyncGetCallAnsweringData(false);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "GlobalManager::QuickMessage() creating async mailbox thread", new object[0]);
		}

		private void GetSummaryInformation(UMSubscriber user)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Calculating the number of messages.", new object[0]);
			base.WriteVariable("haveSummary", false);
			int newVoicemailCount = GlobalActivityManager.GetNewVoicemailCount(user);
			base.WriteVariable("numVoicemail", newVoicemailCount);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "numVoicemail={0}.", new object[]
			{
				newVoicemailCount
			});
			int newEmailCount = this.GetNewEmailCount(user);
			base.WriteVariable("numEmail", newEmailCount);
			base.WriteVariable("numEmailMax", newEmailCount - 1);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "numEmail={0}.", new object[]
			{
				newEmailCount
			});
			this.GetNextMeetingInfo(user);
			base.WriteVariable("Oof", user.ConfigFolder.IsOof);
			base.WriteVariable("haveSummary", true);
		}

		private int GetNewEmailCount(UMSubscriber user)
		{
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			};
			byte[] entryId = Convert.FromBase64String(user.ConfigFolder.TelephoneAccessFolderEmail);
			StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(entryId);
			int result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = user.CreateSessionLock())
			{
				using (Folder folder = Folder.Bind(mailboxSessionLock.Session, folderId))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, new PropertyDefinition[]
					{
						StoreObjectSchema.ItemClass,
						MessageItemSchema.IsRead
					}))
					{
						object[][] array = queryResult.GetRows(MessageItemList.PageSize);
						if (array == null || array.Length == 0)
						{
							result = 0;
						}
						else
						{
							int num = 0;
							int num2 = 0;
							base.WriteVariable("isMaxEmail", false);
							for (;;)
							{
								for (int i = 0; i < array.Length; i++)
								{
									string itemClass = (string)array[i][0];
									bool flag = (bool)array[i][1];
									if (EmailManager.CanReadMessageClassWithTui(itemClass) && (flag || ++num > MessageItemList.PageSize))
									{
										goto IL_E2;
									}
								}
								if (++num2 < 3)
								{
									array = queryResult.GetRows(MessageItemList.PageSize);
								}
								else
								{
									array = null;
									base.WriteVariable("isMaxEmail", true);
								}
								if (array == null || array.Length <= 0)
								{
									goto IL_14B;
								}
							}
							IL_E2:
							base.WriteVariable("isMaxEmail", num > MessageItemList.PageSize);
							return num;
							IL_14B:
							result = num;
						}
					}
				}
			}
			return result;
		}

		private void GetNextMeetingInfo(UMSubscriber user)
		{
			if (!user.HasCalendarFolder)
			{
				base.WriteVariable("numMeetings", 0);
				return;
			}
			CalendarNavigator calendarNavigator = new CalendarNavigator(user);
			CalendarNavigator.AgendaContext agendaContext = new CalendarNavigator.AgendaContext(calendarNavigator.CurrentAgenda, user, true, true);
			while (agendaContext.IsValid && agendaContext.Current.IsAllDayEvent)
			{
				agendaContext.Next();
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "skipped all day event.", new object[0]);
			}
			base.WriteVariable("numMeetings", 0);
			if (agendaContext.IsValid && agendaContext.Current.StartTime <= user.Now.AddHours(1.0))
			{
				base.WriteVariable("location", agendaContext.Current.Location);
				base.WriteVariable("startTime", agendaContext.Current.StartTime);
				bool flag = agendaContext.Current.StartTime <= user.Now && agendaContext.Current.EndTime > user.Now;
				base.WriteVariable("isInProgress", flag);
				base.WriteVariable("numMeetings", Math.Max(1, agendaContext.ConflictCount));
			}
		}

		private DirectoryGrammarHandler directoryGrammarHandler;

		private bool isRegisteredForEvents;

		private UMSubscriber originalCaller;

		private int numLogonFailures;

		private int numMailboxFailures;

		private BaseUMAsyncTimer callTimer;

		private bool maxCallSecondsExceeded;

		private float prosodyRate;

		private bool skipInitialGreetings;

		private TransitionBase postLogonPendingTransition;

		private bool forbidLimitedOVAAccess;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass() : base(null)
			{
			}

			public static Assembly CoreAssembly
			{
				get
				{
					return GlobalActivityManager.ConfigClass.coreAssembly;
				}
				set
				{
					GlobalActivityManager.ConfigClass.coreAssembly = value;
				}
			}

			internal static HashSet<string> RecordingFileNameCache
			{
				get
				{
					return GlobalActivityManager.ConfigClass.recordingFileNameCache.Value;
				}
			}

			internal static void ImportFsmModules(XmlDocument doc, string moduleDirectory)
			{
				XPathNavigator xpathNavigator = doc.CreateNavigator();
				Dictionary<string, XmlDocument> dictionary = new Dictionary<string, XmlDocument>(StringComparer.OrdinalIgnoreCase);
				while (xpathNavigator.MoveToFollowing("FsmImport", string.Empty))
				{
					string attribute = xpathNavigator.GetAttribute("href", string.Empty);
					string attribute2 = xpathNavigator.GetAttribute("module", string.Empty);
					string str = string.IsNullOrEmpty(attribute2) ? string.Empty : ("[@id='" + attribute2 + "']");
					string xpath = "/FiniteStateMachine/FsmModule" + str;
					XmlDocument xmlDocument = null;
					if (!dictionary.TryGetValue(attribute, out xmlDocument))
					{
						xmlDocument = new SafeXmlDocument();
						xmlDocument.PreserveWhitespace = true;
						xmlDocument.Load(Path.Combine(moduleDirectory, attribute));
						dictionary[attribute] = xmlDocument;
					}
					XPathNavigator xpathNavigator2 = xmlDocument.CreateNavigator();
					XPathNavigator xpathNavigator3 = xpathNavigator2.SelectSingleNode(xpath);
					if (xpathNavigator3 == null)
					{
						throw new FsmConfigurationException(Strings.FsmModuleNotFound(attribute2, attribute));
					}
					string str2 = attribute + ":" + attribute2;
					xpathNavigator.InsertBefore("<!-- BEGIN FsmImport " + str2 + " -->");
					xpathNavigator.InsertAfter("<!-- END FsmImport  " + str2 + " -->");
					XPathNodeIterator xpathNodeIterator = xpathNavigator3.SelectChildren(XPathNodeType.All);
					while (xpathNodeIterator.MoveNext())
					{
						XPathNavigator newSibling = xpathNodeIterator.Current;
						xpathNavigator.InsertBefore(newSibling);
					}
					xpathNavigator.DeleteSelf();
				}
			}

			internal ActivityManager CreateActivityManager()
			{
				return this.CreateActivityManager(null);
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				return new GlobalActivityManager(this);
			}

			internal void Load()
			{
				this.Load(GlobCfg.ConfigFile);
			}

			internal void Load(string xmlFileName)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Loading state machine from XML: {0}.", new object[]
				{
					xmlFileName
				});
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.PreserveWhitespace = true;
				Stream stream = null;
				try
				{
					xmlDocument.Load(xmlFileName);
					try
					{
						GlobalActivityManager.ConfigClass.ImportFsmModules(xmlDocument, Path.GetDirectoryName(xmlFileName));
					}
					catch (FileNotFoundException ex)
					{
						throw new FsmConfigurationException(Strings.FileNotFound(ex.FileName), ex);
					}
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					stream = executingAssembly.GetManifestResourceStream("umconfig.xsd");
					XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
					xmlSchemaSet.Add(null, SafeXmlFactory.CreateSafeXmlTextReader(stream));
					xmlDocument.Schemas.Add(xmlSchemaSet);
					xmlDocument.Validate(null);
					XmlNode rootNode = xmlDocument.SelectSingleNode("/FiniteStateMachine/FsmModule/GlobalActivityManager");
					base.Load(rootNode);
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Successfully loaded the state machine", new object[0]);
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FsmConfigurationInitialized, null, new object[]
					{
						xmlFileName
					});
				}
				catch (Exception ex2)
				{
					CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "The Um Global Manager failed to load: {0}.", new object[]
					{
						ex2
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FsmConfigurationError, null, new object[]
					{
						ex2
					});
					string outputFileName = Path.Combine(Path.GetDirectoryName(xmlFileName), "umconfig.err");
					using (XmlWriter xmlWriter = XmlWriter.Create(outputFileName, new XmlWriterSettings
					{
						Indent = true,
						CloseOutput = true
					}))
					{
						xmlDocument.WriteContentTo(xmlWriter);
					}
					throw;
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
					}
					ConditionParser.Release();
					ConstantValidator.Release();
					FsmVariableCache.Instance.Clear();
				}
			}

			private static HashSet<string> PopulateRecordingFileNameCache()
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.Create(CultureInfo.InvariantCulture, true));
				foreach (CultureInfo culture in UmCultures.GetSupportedPromptCultures())
				{
					string path = Util.WavPathFromCulture(culture);
					string[] files = Directory.GetFiles(path);
					foreach (string item in files)
					{
						hashSet.Add(item);
					}
				}
				return hashSet;
			}

			private static Assembly coreAssembly = Assembly.GetExecutingAssembly();

			private static readonly Lazy<HashSet<string>> recordingFileNameCache = new Lazy<HashSet<string>>(() => new LatencyStopwatch().Invoke<HashSet<string>>("PopulateRecordingFileNameCache", new Func<HashSet<string>>(GlobalActivityManager.ConfigClass.PopulateRecordingFileNameCache)), false);
		}
	}
}
