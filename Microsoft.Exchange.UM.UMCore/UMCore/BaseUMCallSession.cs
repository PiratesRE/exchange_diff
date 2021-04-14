using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BaseUMCallSession : DisposableBase
	{
		internal BaseUMCallSession()
		{
			this.sessionGuid = Guid.NewGuid();
			this.taskCallType = CommonConstants.TaskCallType.Voice;
			this.appState = CommonConstants.ApplicationState.Idle;
		}

		internal abstract event UMCallSessionHandler<EventArgs> OnCallConnected;

		internal abstract event UMCallSessionHandler<OutboundCallDetailsEventArgs> OnOutboundCallRequestCompleted;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnDtmf;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnHangup;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnStateInfoSent;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnComplete;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnTimeout;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnTransferComplete;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnHold;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnResume;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnError;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnFaxRequestReceive;

		internal abstract event UMCallSessionHandler<UMCallSessionEventArgs> OnCancelled;

		internal abstract event UMCallSessionHandler<HeavyBlockingOperationEventArgs> OnHeavyBlockingOperation;

		internal abstract event UMCallSessionHandler<InfoMessage.MessageReceivedEventArgs> OnMessageReceived;

		internal abstract event UMCallSessionHandler<EventArgs> OnMessageSent;

		internal abstract event UMCallSessionHandler<EventArgs> OnDispose;

		internal static MovingAverage AverageUserResponseLatency
		{
			get
			{
				return BaseUMCallSession.averageUserResponseLatency;
			}
		}

		internal string CallId
		{
			get
			{
				return this.callId;
			}
			set
			{
				this.callId = value;
			}
		}

		internal CommonConstants.ApplicationState AppState
		{
			get
			{
				return this.appState;
			}
			set
			{
				this.appState = value;
			}
		}

		internal CommonConstants.TaskCallType TaskCallType
		{
			get
			{
				return this.taskCallType;
			}
			set
			{
				this.taskCallType = value;
			}
		}

		internal CallContext CurrentCallContext
		{
			get
			{
				return this.currentCallContext;
			}
			set
			{
				this.currentCallContext = value;
			}
		}

		internal DependentSessionDetails DependentSessionDetails
		{
			get
			{
				return this.dependentSessionDetails;
			}
			set
			{
				this.dependentSessionDetails = value;
			}
		}

		internal Guid SessionGuid
		{
			get
			{
				return this.sessionGuid;
			}
		}

		internal string PlayOnPhoneSMTPAddress
		{
			get
			{
				return this.playOnPhoneSMTPAddress;
			}
			set
			{
				this.playOnPhoneSMTPAddress = value;
			}
		}

		internal string ToTag
		{
			get
			{
				return this.CurrentCallContext.CallInfo.ToTag;
			}
		}

		internal ReadOnlyCollection<PlatformSignalingHeader> RemoteHeaders
		{
			get
			{
				return this.CurrentCallContext.CallInfo.RemoteHeaders;
			}
		}

		internal string FromTag
		{
			get
			{
				return this.CurrentCallContext.CallInfo.FromTag;
			}
		}

		internal bool WaitForSourcePartyInfo
		{
			get
			{
				return this.CurrentCallContext.GatewayConfig.DelayedSourcePartyInfoEnabled && this.CurrentCallContext.IsAnonymousCaller && this.CurrentCallContext.DialPlan.URIType == UMUriType.TelExtn && string.IsNullOrEmpty(this.CurrentCallContext.Extension);
			}
		}

		internal abstract UMCallState State { get; }

		internal abstract bool IsClosing { get; }

		internal abstract string CallLegId { get; }

		protected abstract bool IsDependentSession { get; }

		internal abstract IUMSpeechRecognizer SpeechRecognizer { get; }

		internal abstract UMLoggingManager LoggingManager { get; }

		protected X509Certificate RemoteCertificate { get; set; }

		protected string RemoteMatchedFQDN { get; set; }

		protected List<BaseUMAsyncTimer> Timers
		{
			get
			{
				return this.timerList;
			}
		}

		public abstract void RebufferDigits(byte[] dtmfDigits);

		internal abstract BaseUMAsyncTimer StartTimer(BaseUMAsyncTimer.UMTimerCallback callback, int dueTime);

		internal abstract void TransferAsync();

		internal abstract void TransferAsync(string phoneNumber);

		internal abstract void TransferAsync(PlatformSipUri target, IList<PlatformSignalingHeader> headers);

		internal abstract void TransferAsync(PlatformSipUri target);

		internal abstract void CancelPendingOperations();

		internal void DisconnectCall()
		{
			this.DisconnectCall(null);
		}

		internal abstract void DisconnectCall(PlatformSignalingHeader diagnosticHeader);

		internal abstract void CloseSession();

		internal abstract void Redirect(string host, int port, int code);

		internal abstract void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, StopPatterns stopPatterns, int startIdx, TimeSpan offset, bool stopPromptOnBargeIn, string turnName, int initalSilenceTimeout);

		internal abstract void PlayUninterruptiblePrompts(ArrayList prompts);

		internal abstract void RecordFile(string fileName, int maxSilence, int maxSeconds, string stopTones, bool append, ArrayList comfortPrompts);

		internal abstract void SendStateInfo(string callId, string state);

		internal abstract void ClearDigits(int sleepMsec);

		internal abstract void AcceptFax();

		internal abstract void RunHeavyBlockingOperation(IUMHeavyBlockingOperation operation, ArrayList prompts);

		internal abstract void SendMessage(InfoMessage message);

		internal abstract void SendDtmf(string dtmfSequence, TimeSpan initialSilence);

		internal abstract bool IsDuringPlayback();

		internal abstract void StopPlayback();

		internal abstract void StopPlaybackAndCancelRecognition();

		internal abstract void Skip(TimeSpan timeToSkip);

		internal abstract void InitializeConnectedCall(OutboundCallDetailsEventArgs args);

		internal void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout)
		{
			this.PlayPrompts(prompts, minDigits, maxDigits, timeout, stopTones, interDigitTimeout, StopPatterns.Empty, 0, TimeSpan.Zero, stopPromptOnBargeIn, turnName, initialSilenceTimeout);
		}

		internal void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, StopPatterns stopPatterns, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout)
		{
			this.PlayPrompts(prompts, minDigits, maxDigits, timeout, stopTones, interDigitTimeout, stopPatterns, 0, TimeSpan.Zero, stopPromptOnBargeIn, turnName, initialSilenceTimeout);
		}

		internal void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, int startIdx, TimeSpan offset, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout)
		{
			this.PlayPrompts(prompts, minDigits, maxDigits, timeout, stopTones, interDigitTimeout, StopPatterns.Empty, startIdx, offset, stopPromptOnBargeIn, turnName, initialSilenceTimeout);
		}

		internal void HandleFailedOutboundCall(OutboundCallDetailsEventArgs eventArgs)
		{
			this.CurrentCallContext.ReasonForDisconnect = DropCallReason.OutboundFailedCall;
			UmServiceGlobals.VoipPlatform.DisconnectedOutboundCalls[this.SessionGuid] = eventArgs.CallInfoEx;
			this.DisconnectCall();
		}

		internal void HandleIncomingCall(IList<PlatformSignalingHeader> headers)
		{
			this.CurrentCallContext.OnSessionReceived(headers);
			if (this.CurrentCallContext.OfferResult == OfferResult.Redirect)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "HandleIncomingCall: Call will be redirected, CallId = {0}", new object[]
				{
					this.CallId
				});
				this.RedirectCallToDifferentServer();
				return;
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallReceived, null, new object[]
			{
				CommonUtil.ToEventLogString(this.CurrentCallContext.CallerId.ToDial),
				CommonUtil.ToEventLogString(this.CurrentCallContext.Extension),
				this.CallId
			});
			this.LoggingManager.LogApplicationInformation("HandleIncomingCall: CallId={0}, CalleeInfo={1}, CallerInfo={2}", new object[]
			{
				this.CallLegId,
				(this.CurrentCallContext.CalleeInfo != null) ? this.CurrentCallContext.CalleeInfo.DisplayName : this.CurrentCallContext.CalleeId.ToDial,
				(this.CurrentCallContext.CallerInfo != null) ? this.CurrentCallContext.CallerInfo.DisplayName : this.CurrentCallContext.CallerId.ToDial
			});
			this.CurrentCallContext.OfferResult = OfferResult.Answer;
			this.SetContactUriForAccept();
			this.InternalAcceptCall();
		}

		internal void OpenAsync(PlatformSipUri toAddress, PlatformSipUri fromAddress, UMSipPeer outboundProxy, IList<PlatformSignalingHeader> headers)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), "toaddr {0} fromaddr {1} proxy {2}", new object[]
			{
				toAddress,
				fromAddress,
				outboundProxy
			});
			BaseUMCallSession.OutboundCallInfo info = new BaseUMCallSession.OutboundCallInfo
			{
				CalledParty = toAddress.ToString(),
				CallingParty = fromAddress.ToString(),
				Gateway = outboundProxy
			};
			this.IncrementCounter(AvailabilityCounters.TotalWorkerProcessCallCount);
			this.InternalOpenAsync(info, headers);
		}

		internal BaseUMCallSession CreateDependentSession(UMCallSessionHandler<OutboundCallDetailsEventArgs> onoutboundCallRequestCompleted, UMSubscriber caller, string callerId, PhoneNumber numberToCall)
		{
			ExAssert.RetailAssert(!this.IsDependentSession, "CreateDependentSession called on dependent session!");
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._Caller, callerId),
				PIIMessage.Create(PIIType._PhoneNumber, numberToCall.ToDial)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), data, "CreateDependentSession . Call-ID: {0}, caller-id=_Caller, numberToCall=_PhoneNumber", new object[]
			{
				this.CallId
			});
			UMIPGatewaySipPeer umipgatewaySipPeer = new UMIPGatewaySipPeer(this.CurrentCallContext.GatewayConfig, this.CurrentCallContext.DialPlan);
			CallContext callContext = new CallContext();
			callContext.DialPlan = this.CurrentCallContext.DialPlan;
			callContext.GatewayConfig = this.CurrentCallContext.GatewayConfig;
			callContext.CallLoggingHelper.ParentCallIdentifier = this.CurrentCallContext.CallId;
			if (umipgatewaySipPeer.UseMutualTLS && umipgatewaySipPeer.Address.IsIPAddress && !string.IsNullOrEmpty(this.RemoteMatchedFQDN))
			{
				umipgatewaySipPeer.Address = new UMSmartHost(this.RemoteMatchedFQDN);
			}
			if (!this.CurrentCallContext.RoutingHelper.SupportsMsOrganizationRouting && SipRoutingHelper.UseGlobalSBCSettingsForOutbound(this.CurrentCallContext.GatewayConfig))
			{
				umipgatewaySipPeer.NextHopForOutboundRouting = SipPeerManager.Instance.SBCService;
			}
			this.DisposeDependentSession();
			this.DependentSessionDetails = new DependentSessionDetails(onoutboundCallRequestCompleted, caller, callerId, umipgatewaySipPeer, numberToCall, this);
			BaseUMCallSession baseUMCallSession = this.InternalCreateDependentSession(this.DependentSessionDetails, callContext);
			this.DependentSessionDetails.DependentUMCallSession = baseUMCallSession;
			return baseUMCallSession;
		}

		internal void DisconnectDependentUMCallSession()
		{
			ExAssert.RetailAssert(!this.IsDependentSession, "DisconnectDependentSession called on dependent session");
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), "DisconnectDependentUMCallSession . Call-ID: {0}", new object[]
			{
				this.CallId
			});
			this.DependentSessionDetails.DependentUMCallSession.DisconnectCall();
		}

		internal void TerminateDependentSession()
		{
			ExAssert.RetailAssert(!this.IsDependentSession, "TerminateDependentSession called on dependent session");
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), "TerminateDependentSession . Call-ID: {0}", new object[]
			{
				this.CallId
			});
			try
			{
				this.DependentSessionDetails.DependentUMCallSession.CloseSession();
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), "TerminateDependentSession . Call-ID: {0}. The call is already disconnected. Exception={1}", new object[]
				{
					this.CallId,
					ex.ToString()
				});
			}
		}

		internal void MakeNewDependentSessionCall()
		{
			ExAssert.RetailAssert(this.IsDependentSession, "MakeNewDependentSessionCall called on non-dependent session");
			IList<PlatformSignalingHeader> list = new List<PlatformSignalingHeader>();
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), "MakeCallOnDependentSession fired.", new object[0]);
			list.Add(Platform.Builder.CreateSignalingHeader("X-MSUM-Originating-Session-Call-Id", this.DependentSessionDetails.ParentUMCallSession.CallLegId));
			if (this.DependentSessionDetails.Caller.DialPlan.URIType == UMUriType.SipName)
			{
				string str = this.DependentSessionDetails.Caller.IsVirtualNumberEnabled ? this.DependentSessionDetails.Caller.VirtualNumber.Trim() : this.DependentSessionDetails.Caller.Extension.Trim();
				PlatformSipUri platformSipUri = Platform.Builder.CreateSipUri("SIP:" + str);
				list.Add(Platform.Builder.CreateSignalingHeader("Referred-By", "<" + platformSipUri.ToString() + ">"));
			}
			OutCallingHandlerForUser outCallingHandlerForUser = new OutCallingHandlerForUser(this.DependentSessionDetails.Caller, this, this.DependentSessionDetails.RemotePeerToUse, TypeOfOutboundCall.FindMe);
			outCallingHandlerForUser.MakeCall(this.DependentSessionDetails.CallerID, this.DependentSessionDetails.NumberToCall.ToDial, list);
		}

		internal void BlindTransferToPhone(PhoneNumber phone, ContactInfo contactInfo)
		{
			this.SetStateForTransfer("BlindTransfer", phone, contactInfo);
			BlindTransferToPhone blindTransferToPhone = new BlindTransferToPhone(this, this.CurrentCallContext, phone);
			blindTransferToPhone.Transfer();
		}

		internal void BlindTransferToHost(PhoneNumber phone, PlatformSipUri referredByUri)
		{
			this.SetStateForTransfer("BlindTransferToHost", phone, null);
			BlindTransferToHost blindTransferToHost = new BlindTransferToHost(this, this.CurrentCallContext, phone, referredByUri);
			blindTransferToHost.Transfer();
		}

		internal void SupervisedTransfer()
		{
			this.SetStateForTransfer("SupervisedTransfer", this.DependentSessionDetails.NumberToCall, null);
			SupervisedTransfer supervisedTransfer = new SupervisedTransfer(this, this.CurrentCallContext, this.DependentSessionDetails.NumberToCall, this.DependentSessionDetails.Caller);
			supervisedTransfer.Transfer();
		}

		internal void IncrementCounter(ExPerformanceCounter counter)
		{
			this.IncrementCounter(counter, 1L);
		}

		internal void IncrementCounter(ExPerformanceCounter counter, long count)
		{
			if (this.CurrentCallContext != null)
			{
				this.CurrentCallContext.IncrementCounter(counter, count);
				return;
			}
			Util.IncrementCounter(counter, count);
		}

		internal void DecrementCounter(ExPerformanceCounter counter)
		{
			if (this.CurrentCallContext != null)
			{
				this.CurrentCallContext.DecrementCounter(counter);
				return;
			}
			Util.DecrementCounter(counter);
		}

		internal void SetCounter(ExPerformanceCounter counter, long value)
		{
			if (this.CurrentCallContext != null)
			{
				this.CurrentCallContext.SetCounter(counter, value);
				return;
			}
			Util.SetCounter(counter, value);
		}

		protected static UMEventCause GetUMEventCause(int sipCode)
		{
			if (sipCode == 480)
			{
				return UMEventCause.NoAnswer;
			}
			switch (sipCode)
			{
			case 486:
				return UMEventCause.UserBusy;
			case 487:
				return UMEventCause.None;
			default:
				if (sipCode != 600)
				{
					return UMEventCause.Other;
				}
				return UMEventCause.UserBusy;
			}
		}

		protected abstract void InternalAcceptCall();

		protected abstract void SetContactUriForAccept();

		protected abstract void DeclineCall(PlatformSignalingHeader diagnosticHeader);

		protected abstract BaseUMCallSession InternalCreateDependentSession(DependentSessionDetails details, CallContext context);

		protected abstract void InternalOpenAsync(BaseUMCallSession.OutboundCallInfo info, IList<PlatformSignalingHeader> headers);

		protected virtual void SetStateForTransfer(string transferType, PhoneNumber number, ContactInfo contactInfo)
		{
			if (this.CurrentCallContext != null)
			{
				this.CurrentCallContext.CallLoggingHelper.SetTransferTarget(number, contactInfo);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "BaseUMCallSession disposing.", new object[0]);
				UmServiceGlobals.VoipPlatform.HandleCallDisposed(this);
				this.DisposeDependentSession();
				this.TeardownCurrentCall();
				UmServiceGlobals.VoipPlatform.UnRegisterCall(this);
			}
		}

		protected virtual void TeardownCurrentCall()
		{
			TempFileFactory.DisposeSessionFiles(this.CallId);
			this.TeardownTimers();
			this.TeardownContext();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BaseUMCallSession>(this);
		}

		protected void ConditionalUpdateCallerId(InfoMessage.MessageReceivedEventArgs args)
		{
			if (this.CurrentCallContext.GatewayConfig != null && this.CurrentCallContext.GatewayConfig.DelayedSourcePartyInfoEnabled && this.CurrentCallContext.DialPlan != null && this.CurrentCallContext.DialPlan.URIType == UMUriType.TelExtn)
			{
				this.CurrentCallContext.UpdateCallerInfo(args);
			}
		}

		protected void UpdateCallerId(PlatformTelephonyAddress callerAddress)
		{
			this.CurrentCallContext.InitializeCallerId(callerAddress, false);
			if (PhoneNumber.IsNullOrEmpty(this.CurrentCallContext.CallerId))
			{
				this.CurrentCallContext.CallerId = PhoneNumber.Empty;
				return;
			}
			this.CurrentCallContext.ConsumeUpdateForCallerInformation();
		}

		protected void LogOutboundCallFailed(BaseUMCallSession.OutboundCallInfo outboundCallInfo, string sipError, string exceptionMessage)
		{
			switch (this.CurrentCallContext.DialPlan.GlobalCallRoutingScheme)
			{
			case UMGlobalCallRoutingScheme.None:
			case UMGlobalCallRoutingScheme.E164:
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_OutboundCallFailed, null, new object[]
				{
					CommonUtil.ToEventLogString(outboundCallInfo.CalledParty),
					CommonUtil.ToEventLogString(outboundCallInfo.Gateway),
					CommonUtil.ToEventLogString(sipError),
					CommonUtil.ToEventLogString(outboundCallInfo.CallingParty),
					CommonUtil.ToEventLogString(exceptionMessage)
				});
				return;
			case UMGlobalCallRoutingScheme.GatewayGuid:
				UmGlobals.ExEvent.LogEvent(this.CurrentCallContext.DialPlan.OrganizationId, UMEventLogConstants.Tuple_OutboundCallFailedForOnPremiseGateway, outboundCallInfo.Gateway.Name, new object[]
				{
					CommonUtil.ToEventLogString(outboundCallInfo.CalledParty),
					CommonUtil.ToEventLogString(outboundCallInfo.Gateway),
					CommonUtil.ToEventLogString(sipError),
					CommonUtil.ToEventLogString(outboundCallInfo.CallingParty),
					CommonUtil.ToEventLogString(exceptionMessage)
				});
				return;
			default:
				throw new InvalidOperationException("Invalid value for DialPlan.GlobalCallRoutingScheme: " + this.CurrentCallContext.DialPlan.GlobalCallRoutingScheme.ToString());
			}
		}

		protected string BuildPromptInfoMessageString(string turnName, ArrayList prompts)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("Call-Id: {0}\r\nCall-Prompts: \r\n<State>{1}</State>\r\n", this.CallId, turnName);
			for (int i = 0; i < prompts.Count; i++)
			{
				stringBuilder.AppendFormat("<Prompt>{0}</Prompt>\r\n", Uri.EscapeUriString(prompts[i].ToString()));
			}
			return stringBuilder.ToString();
		}

		protected PlatformSipUri GetRedirectContactUri(string host, int port)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this, "Using RoutingHelper:{0}", new object[]
			{
				this.CurrentCallContext.RoutingHelper.GetType().Name
			});
			PlatformSipUri redirectContactUri = RouterUtils.GetRedirectContactUri(this.CurrentCallContext.RequestUriOfCall, this.CurrentCallContext.RoutingHelper, host, port, this.CurrentCallContext.IsSecuredCall ? TransportParameter.Tls : TransportParameter.Tcp, this.CurrentCallContext.TenantGuid.ToString());
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "FormatContactUri: {0}", new object[]
			{
				redirectContactUri
			});
			return redirectContactUri;
		}

		private static void LogCallRedirected(CallContext cc, string callId, string serverFqdn)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRedirectedToServer, null, new object[]
			{
				cc.ServerPicker.SubscriberLogId,
				serverFqdn,
				callId
			});
		}

		private void TeardownContext()
		{
			if (this.currentCallContext != null)
			{
				this.currentCallContext.Dispose();
				this.currentCallContext = null;
			}
		}

		private void DisposeDependentSession()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "DisposeDependentSession", new object[0]);
			if (this.DependentSessionDetails != null)
			{
				BaseUMCallSession dependentUMCallSession = this.DependentSessionDetails.DependentUMCallSession;
				if (dependentUMCallSession != null)
				{
					dependentUMCallSession.Dispose();
				}
			}
			this.DependentSessionDetails = null;
		}

		private void TeardownTimers()
		{
			foreach (BaseUMAsyncTimer baseUMAsyncTimer in this.timerList)
			{
				baseUMAsyncTimer.Dispose();
			}
			this.timerList.Clear();
		}

		private void RedirectCallToDifferentServer()
		{
			IRedirectTargetChooser serverPicker = this.CurrentCallContext.ServerPicker;
			ExAssert.RetailAssert(serverPicker != null, "serverPicker is null");
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "MSSCallSession::RedirectCallToDifferentServer() - user/extension '{0}'", new object[]
			{
				serverPicker.SubscriberLogId
			});
			string text = null;
			int port;
			if (!serverPicker.GetTargetServer(out text, out port))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Did not find a valid server to redirect the call. The call will be rejected", new object[0]);
				serverPicker.HandleServerNotFound();
				this.DeclineCall(CallRejectedException.RenderDiagnosticHeader(CallEndingReason.UserRoutingIssue, null, new object[0]));
				this.DisconnectCall();
				return;
			}
			int redirectResponseCode = this.CurrentCallContext.RoutingHelper.RedirectResponseCode;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Redirecting the call to server {0} - code:{1}", new object[]
			{
				text,
				redirectResponseCode
			});
			BaseUMCallSession.LogCallRedirected(this.CurrentCallContext, this.CallId, text);
			this.Redirect(text, port, redirectResponseCode);
			this.DisconnectCall();
		}

		private static MovingAverage averageUserResponseLatency = new MovingAverage(25);

		private CommonConstants.ApplicationState appState;

		private CommonConstants.TaskCallType taskCallType;

		private string callId = string.Empty;

		private CallContext currentCallContext;

		private Guid sessionGuid;

		private DependentSessionDetails dependentSessionDetails;

		private string playOnPhoneSMTPAddress;

		private List<BaseUMAsyncTimer> timerList = new List<BaseUMAsyncTimer>();

		internal struct OutboundCallInfo
		{
			internal OutboundCallInfo(string callingParty, string calledParty, UMSipPeer gateway)
			{
				this.Gateway = gateway;
				this.CalledParty = calledParty;
				this.CallingParty = callingParty;
			}

			internal UMSipPeer Gateway;

			internal string CalledParty;

			internal string CallingParty;
		}
	}
}
