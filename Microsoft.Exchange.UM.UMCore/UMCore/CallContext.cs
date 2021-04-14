using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class CallContext : DisposableBase, IRoutingContext
	{
		public CallContext()
		{
			base.SuppressDisposeTracker();
			this.callLogHelper = new CallLoggingHelper(this);
			this.IncrementCounter(AvailabilityCounters.TotalWorkerProcessCallCount);
		}

		public SipRoutingHelper RoutingHelper { get; private set; }

		public string CallId
		{
			get
			{
				return this.callId;
			}
		}

		public UMDialPlan DialPlan
		{
			get
			{
				return this.dialPlan;
			}
			set
			{
				this.dialPlan = value;
			}
		}

		public bool IsSecuredCall
		{
			get
			{
				return this.securedCall;
			}
		}

		public PlatformSipUri RequestUriOfCall
		{
			get
			{
				return this.requestUri;
			}
		}

		public Guid TenantGuid { get; private set; }

		internal bool IsTroubleshootingToolCall { get; private set; }

		internal bool IsActiveMonitoringCall { get; private set; }

		internal PlatformCallInfo CallInfo { get; private set; }

		internal bool DivertedExtensionAllowVoiceMail
		{
			get
			{
				return this.divertedExtensionAllowVoiceMail;
			}
		}

		internal UMRecipient LegacySubscriber
		{
			get
			{
				return this.unsupportedSubscriber;
			}
			set
			{
				this.UpdateDisposableInstance<UMRecipient>(ref this.unsupportedSubscriber, value, "LegacySubscriber");
			}
		}

		internal IRedirectTargetChooser ServerPicker
		{
			get
			{
				return this.serverPicker;
			}
			set
			{
				this.serverPicker = value;
			}
		}

		internal NonBlockingCallAnsweringData UmSubscriberData
		{
			get
			{
				return this.subscriberData;
			}
			set
			{
				this.UpdateDisposableInstance<NonBlockingCallAnsweringData>(ref this.subscriberData, value, "UmSubscriberData");
			}
		}

		internal bool IsPlayOnPhoneCall
		{
			get
			{
				return this.CallType == 5;
			}
		}

		internal bool IsAutoAttendantCall
		{
			get
			{
				return this.CallType == 2;
			}
		}

		internal bool IsSubscriberAccessCall
		{
			get
			{
				return this.CallType == 3;
			}
		}

		internal bool IsPromptProvisioningCall
		{
			get
			{
				return this.CallType == 8;
			}
		}

		internal bool IsVirtualNumberCall
		{
			get
			{
				return false;
			}
		}

		internal bool IsFindMeSubscriberCall
		{
			get
			{
				return this.CallType == 9;
			}
		}

		internal bool IsPlayOnPhonePAAGreetingCall
		{
			get
			{
				return this.CallType == 10;
			}
		}

		internal bool FaxToneReceived
		{
			get
			{
				return this.faxToneReceived;
			}
			set
			{
				this.faxToneReceived = value;
			}
		}

		internal string RemoteUserAgent
		{
			get
			{
				return this.CallInfo.RemoteUserAgent;
			}
		}

		internal PlayOnPhoneRequest WebServiceRequest
		{
			get
			{
				return this.webServiceRequest;
			}
			set
			{
				this.webServiceRequest = value;
			}
		}

		internal bool IsDiagnosticCall
		{
			get
			{
				return this.callIsDiagnostic;
			}
			set
			{
				this.callIsDiagnostic = value;
			}
		}

		internal bool IsLocalDiagnosticCall
		{
			get
			{
				return this.callIsLocalDiagnostic;
			}
		}

		internal bool IsTUIDiagnosticCall
		{
			get
			{
				return this.callIsTUIDiagnostic;
			}
			set
			{
				this.callIsTUIDiagnostic = value;
			}
		}

		internal bool IsAnonymousCaller
		{
			get
			{
				return this.callerId.IsEmpty;
			}
		}

		internal PhoneNumber CallerId
		{
			get
			{
				return this.callerId;
			}
			set
			{
				this.callerId = value;
			}
		}

		internal string CallerIdDisplayName
		{
			get
			{
				return this.callerIdDisplayName;
			}
		}

		internal PhoneNumber CalleeId
		{
			get
			{
				return this.calleeId;
			}
		}

		internal string Extension
		{
			get
			{
				return this.extnNumber;
			}
		}

		internal Exception CallRejectionException { get; set; }

		internal UMSubscriber CallerInfo
		{
			get
			{
				return this.callerInfo;
			}
			set
			{
				this.UpdateDisposableInstance<UMSubscriber>(ref this.callerInfo, value, "CallerInfo");
			}
		}

		internal UMRecipient CalleeInfo
		{
			get
			{
				return this.calleeInfo;
			}
			set
			{
				this.UpdateDisposableInstance<UMRecipient>(ref this.calleeInfo, value, string.Empty);
				UMSubscriber umsubscriber = value as UMSubscriber;
				if (umsubscriber != null && !string.IsNullOrEmpty(umsubscriber.Extension))
				{
					this.calleeId = new PhoneNumber(umsubscriber.Extension);
					this.extnNumber = this.calleeId.ToDial;
				}
				else
				{
					this.calleeId = PhoneNumber.Empty;
					this.extnNumber = string.Empty;
				}
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._Callee, this.CalleeInfo),
					PIIMessage.Create(PIIType._User, this.calleeId),
					PIIMessage.Create(PIIType._PhoneNumber, this.extnNumber)
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "Changed CalleeInfo to:_Callee, CalleeId:_User, Extension:_PhoneNumber", new object[0]);
			}
		}

		internal CultureInfo Culture
		{
			get
			{
				CultureInfo result = CommonConstants.DefaultCulture;
				if (this.autoAttendantInfo != null)
				{
					if (this.autoAttendantCulture == null)
					{
						this.autoAttendantCulture = this.autoAttendantInfo.Language.Culture;
						List<CultureInfo> supportedPromptCultures = UmCultures.GetSupportedPromptCultures();
						if (!supportedPromptCultures.Contains(this.autoAttendantCulture) && this.DialPlan != null)
						{
							CultureInfo defaultCulture = Util.GetDefaultCulture(this.DialPlan);
							UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AALanguageNotFound, null, new object[]
							{
								this.autoAttendantInfo.Name,
								this.autoAttendantCulture.Name,
								defaultCulture.Name,
								this.DialPlan.Name
							});
							this.autoAttendantCulture = defaultCulture;
						}
					}
					result = this.autoAttendantCulture;
				}
				else if (this.CallerInfo != null && this.CallerInfo.TelephonyCulture != null)
				{
					result = this.CallerInfo.TelephonyCulture;
				}
				else if (this.dialPlanCulture != null)
				{
					result = this.dialPlanCulture;
				}
				else if (this.DialPlan != null)
				{
					this.dialPlanCulture = Util.GetDefaultCulture(this.DialPlan);
					result = this.dialPlanCulture;
				}
				return result;
			}
		}

		internal bool IsTestCall
		{
			get
			{
				return this.callIsTest;
			}
		}

		internal bool HasVoicemailBeenSubmitted
		{
			get
			{
				return this.hasVoicemailBeenSubmitted;
			}
			set
			{
				this.hasVoicemailBeenSubmitted = value;
			}
		}

		internal OCFeature OCFeature
		{
			get
			{
				return this.officeCommunicatorFeature;
			}
		}

		internal UMAutoAttendant AutoAttendantInfo
		{
			get
			{
				return this.autoAttendantInfo;
			}
		}

		internal AutoAttendantSettings CurrentAutoAttendantSettings
		{
			get
			{
				return this.autoAttendantSettings;
			}
		}

		internal bool AutoAttendantBusinessHourCall
		{
			get
			{
				return this.businessHour;
			}
		}

		internal HolidaySchedule AutoAttendantHolidaySettings
		{
			get
			{
				return this.holidaySettings;
			}
		}

		internal ExDateTime CallReceivedTime
		{
			get
			{
				return this.callStartTime;
			}
		}

		internal ExDateTime? ConnectionTime
		{
			get
			{
				return this.connectionTime;
			}
			set
			{
				this.connectionTime = value;
			}
		}

		internal UMSmartHost GatewayDetails
		{
			get
			{
				return this.gatewayDetails;
			}
		}

		internal IPAddress ImmediatePeer
		{
			get
			{
				return this.immediatePeer;
			}
		}

		internal UMIPGateway GatewayConfig
		{
			get
			{
				return this.gatewayConfig;
			}
			set
			{
				this.gatewayConfig = value;
			}
		}

		internal string RemoteFQDN { get; private set; }

		internal bool IsCallAnswerCallsCounterIncremented
		{
			get
			{
				return this.IsCounterIncremented(CallAnswerCounters.CallAnsweringCalls);
			}
		}

		internal bool IsDelayedCallsCounterIncremented
		{
			get
			{
				return this.IsCounterIncremented(GeneralCounters.DelayedCalls);
			}
		}

		internal ReasonForCall ReasonForCall
		{
			get
			{
				return this.reasonForCall;
			}
			set
			{
				this.reasonForCall = value;
			}
		}

		internal DropCallReason ReasonForDisconnect
		{
			get
			{
				return this.reasonForDisconnect;
			}
			set
			{
				this.reasonForDisconnect = value;
			}
		}

		internal string ReferredByHeader
		{
			get
			{
				return this.referredByHeader;
			}
		}

		internal bool CallIsOVATransferForUMSubscriber
		{
			get
			{
				return this.callIsOVATransferForUMSubscriber;
			}
		}

		internal OfferResult OfferResult
		{
			get
			{
				return this.offerResult;
			}
			set
			{
				this.offerResult = value;
			}
		}

		internal CallType CallType
		{
			get
			{
				return this.callType;
			}
			set
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Setting CallType: Old {0}, New {1}.", new object[]
				{
					this.CallType,
					value
				});
				if (this.callType == null)
				{
					this.callLogHelper.CallsInProgressWhenStarted = (long)((int)GeneralCounters.CurrentCalls.RawValue);
				}
				this.callType = value;
			}
		}

		internal PlatformSipUri FromUriOfCall
		{
			get
			{
				if (this.fromAddress == null)
				{
					return null;
				}
				return this.fromAddress.Uri;
			}
		}

		internal PlatformSipUri ToUriOfCall
		{
			get
			{
				if (this.toAddress == null)
				{
					return null;
				}
				return this.toAddress.Uri;
			}
		}

		internal IPAACommonInterface LinkedManagerPointer
		{
			get
			{
				return this.linkedManagerPointer;
			}
			set
			{
				this.linkedManagerPointer = value;
			}
		}

		internal UMConfigCache UMConfigCache
		{
			get
			{
				return this.promptConfigCache;
			}
		}

		internal IDictionary<ExPerformanceCounter, long> IncrementedCounters
		{
			get
			{
				return this.incrementedCounters;
			}
		}

		internal CallLoggingHelper CallLoggingHelper
		{
			get
			{
				return this.callLogHelper;
			}
		}

		public string UMPodRedirectTemplate { get; set; }

		internal static void UpdateCountersAndPercentages(bool successfulAttempt, ExPerformanceCounter successCounter, ExPerformanceCounter attemptCounter, ExPerformanceCounter percentageCounter, ExPerformanceCounter percentageCounterBase)
		{
			if (!UmServiceGlobals.ArePerfCountersEnabled)
			{
				return;
			}
			Util.IncrementCounter(attemptCounter);
			if (successfulAttempt)
			{
				Util.IncrementCounter(successCounter);
			}
			if (attemptCounter.RawValue > 0L && successCounter.RawValue > 0L)
			{
				Util.SetCounter(percentageCounter, successCounter.RawValue);
				Util.SetCounter(percentageCounterBase, attemptCounter.RawValue);
				return;
			}
			Util.SetCounter(percentageCounter, 0L);
			Util.SetCounter(percentageCounterBase, 1L);
		}

		internal void PopulateCallContext(bool callIsOutbound, PlatformCallInfo callInfo)
		{
			if (callIsOutbound)
			{
				ExAssert.RetailAssert(this.GatewayConfig != null, "Gateway object was not set for an outbound call");
				ExAssert.RetailAssert(this.DialPlan != null, "DialPlan object was not set for an outbound call");
			}
			this.dialPlanCulture = null;
			this.callId = callInfo.CallId;
			this.CallInfo = callInfo;
			CallIdTracer.TracePfd(ExTraceGlobals.PFDUMCallAcceptanceTracer, this, "PFD UMC {0} - Attempt to Parse Information for Call.", new object[]
			{
				12282
			});
			this.RemoteFQDN = callInfo.RemoteMatchedFQDN;
			if (callIsOutbound)
			{
				this.PopulateTenantGuid(true);
				this.RoutingHelper = SipRoutingHelper.CreateForOutbound(this.DialPlan);
			}
			else
			{
				if (SipPeerManager.Instance.IsLocalDiagnosticCall(callInfo.RemotePeer, callInfo.RemoteHeaders))
				{
					this.callIsDiagnostic = true;
					this.callIsLocalDiagnostic = true;
					this.CallType = 7;
					this.TenantGuid = Guid.Empty;
				}
				else
				{
					this.PopulateTenantGuid(false);
				}
				this.RoutingHelper = SipRoutingHelper.Create(callInfo);
			}
			this.PopulateSIPHeaders(callIsOutbound, callInfo);
			if (this.FromUriOfCall == null)
			{
				throw new InvalidSIPHeaderException("INVITE", "from", string.Empty);
			}
			this.PopulateRemotePeer(callIsOutbound, callInfo.RemotePeer, callInfo.RemoteMatchedFQDN);
			if (this.CallInfo.DiversionInfo.Count > 0)
			{
				PlatformDiversionInfo platformDiversionInfo = this.CallInfo.DiversionInfo[0];
				this.extnNumber = platformDiversionInfo.OriginalCalledParty;
				this.originalCalledParty = platformDiversionInfo.OriginalCalledParty;
				this.diversionUserAtHost = platformDiversionInfo.UserAtHost;
				this.ReasonForCall = CallContext.GetReasonForCall(platformDiversionInfo);
			}
			this.Initialize(callInfo.RemoteHeaders);
		}

		internal void OnSessionReceived(IList<PlatformSignalingHeader> headers)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.OnSessionReceived. CallType: {0}.", new object[]
			{
				this.CallType
			});
			if (this.TryHandleTransferredCallFromAnotherServer())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.OnSessionReceived: Got transferred call from another server", new object[0]);
				OfferResult offerResult = this.offerResult;
				return;
			}
			if (this.offerResult == OfferResult.Redirect || this.IsDiagnosticCall || this.IsPlayOnPhoneCall || this.IsPlayOnPhonePAAGreetingCall)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.OnSessionReceived: No additional steps required.", new object[0]);
				return;
			}
			if (this.IsAutoAttendantCall)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.OnSessionReceived: AA call. No additional initialization required.", new object[0]);
				return;
			}
			if (this.CallInfo.DiversionInfo.Count > 0)
			{
				this.ProcessDiversionInformation();
				if (this.OfferResult == OfferResult.Redirect)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Redirect needed after processing diversion.", new object[0]);
					return;
				}
			}
			if (this.CallerInfo == null && !this.TryHandleCallWithCallerIdOfSubscriber())
			{
				return;
			}
			if (this.dialPlan.URIType == UMUriType.SipName && this.dialPlan.VoIPSecurity != UMVoIPSecurityType.Unsecured)
			{
				this.officeCommunicatorFeature.Parse(this, headers, this.localResourcePath);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "{0}: URIType={1}, VoIPSecurity={2}: will not attempt to process OC features", new object[]
				{
					this.dialPlan.Name,
					this.dialPlan.URIType,
					this.dialPlan.VoIPSecurity
				});
			}
			if (this.CallType == null)
			{
				this.CallType = 1;
			}
			else if (this.CallType == 4 && this.UmSubscriberData != null)
			{
				this.UmSubscriberData.WaitForCompletion();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Call Type: {0}.", new object[]
			{
				this.CallType
			});
		}

		internal IList<PlatformSignalingHeader> GetAcceptHeaders()
		{
			IList<PlatformSignalingHeader> list = null;
			if (this.dialPlan != null && this.dialPlan.URIType == UMUriType.SipName && !string.IsNullOrEmpty(this.Extension))
			{
				list = new List<PlatformSignalingHeader>();
				list.Add(Platform.Builder.CreateSignalingHeader("P-Asserted-Identity", string.Format(CultureInfo.InvariantCulture, "<sip:{0}>", new object[]
				{
					this.Extension
				})));
			}
			return list;
		}

		internal SetDiversionInfoResult SetDiversionInfo(PlatformDiversionInfo diversionInfo)
		{
			string b = null;
			string text = null;
			SetDiversionInfoResult setDiversionInfoResult = DiversionUtils.GetInitialDiversionInfo(diversionInfo, this.DialPlan, this.CallId, this.CallerId, out text, out b);
			if (setDiversionInfoResult != SetDiversionInfoResult.Invalid)
			{
				if (!string.Equals(this.CallerId.ToDial, b, StringComparison.OrdinalIgnoreCase))
				{
					this.extnNumber = text;
					this.diversionUserAtHost = diversionInfo.UserAtHost;
					this.ReasonForCall = CallContext.GetReasonForCall(diversionInfo);
					if (this.LookupAutoAttendantInDialPlan(text, true, this.dialPlan.Id))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found valid AA.", new object[0]);
						return SetDiversionInfoResult.ObjectFound;
					}
					try
					{
						PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, text);
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "AA not found, try to find a user with _PhoneNumber", new object[0]);
						UMRecipient umrecipient = UMRecipient.Factory.FromExtension<UMRecipient>(text, this.dialPlan, null);
						if (umrecipient != null)
						{
							PIIMessage data2 = PIIMessage.Create(PIIType._User, umrecipient);
							CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data2, "Found user _User", new object[0]);
							UMSubscriber umsubscriber = umrecipient as UMSubscriber;
							if (umrecipient.RequiresRedirectForCallAnswering())
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data2, "User _User requires a redirect for C/A and the dialplan supports it", new object[0]);
								this.SetCallAnsweringCallType(umrecipient, false);
							}
							else if (umsubscriber != null)
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data2, "User _User is a valid user for call answering on this server", new object[0]);
								this.SetCallAnsweringCallType(umsubscriber, true);
							}
							else
							{
								CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, data2, "User _User is NOT a valid user for call answering on this server", new object[0]);
								umrecipient.Dispose();
								umrecipient = null;
							}
						}
						return (umrecipient != null) ? SetDiversionInfoResult.ObjectFound : SetDiversionInfoResult.ObjectNotFound;
					}
					catch (LocalizedException ex)
					{
						CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "Failed to find the callee : {0}.", new object[]
						{
							ex
						});
						return setDiversionInfoResult;
					}
				}
				setDiversionInfoResult = SetDiversionInfoResult.UserCallingItself;
				PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, this.CallerId.ToDial);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, piimessage, "Bypassing diversion because callerId and diversion are same. _PhoneNumber. .", new object[0]);
				if (this.dialPlan.URIType == UMUriType.TelExtn && text.Length != this.callerId.ToDial.Length)
				{
					PIIMessage[] data3 = new PIIMessage[]
					{
						piimessage,
						PIIMessage.Create(PIIType._PhoneNumber, text)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data3, "Repopulating the callerId _PhoneNumber1 with _PhoneNumer2 for telex dial plan.", new object[0]);
					PhoneNumber.TryParse(text, out this.callerId);
				}
			}
			return setDiversionInfoResult;
		}

		internal void UpdateCallerInfo(InfoMessage.MessageReceivedEventArgs messageReceivedEventArgs)
		{
			if (this.GatewayConfig.DelayedSourcePartyInfoEnabled && this.DialPlan != null && this.DialPlan.URIType == UMUriType.TelExtn && messageReceivedEventArgs.Message.ContentType != null && messageReceivedEventArgs.Message.ContentType.Equals(Constants.ContentTypeSourceParty))
			{
				string text = (messageReceivedEventArgs.Message.Body != null) ? Encoding.UTF8.GetString(messageReceivedEventArgs.Message.Body).Trim() : null;
				if (!string.IsNullOrEmpty(text))
				{
					PIIMessage data = PIIMessage.Create(PIIType._Caller, text);
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "UpdateCallerInfo: Updating the calling party to _Caller.", new object[0]);
					if (!PhoneNumber.TryParse(text, out this.callerId))
					{
						this.callerId = PhoneNumber.Empty;
						return;
					}
					this.ConsumeUpdateForCallerInformation();
					return;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "UpdateCallerInfo: Received INFO with no calling party. Body={0}.", new object[]
					{
						messageReceivedEventArgs.Message.Body
					});
				}
			}
		}

		internal void ConsumeUpdateForCallerInformation()
		{
			UMRecipient umrecipient = this.ResolveCallerFromCallerId();
			PIIMessage data = PIIMessage.Create(PIIType._Callee, umrecipient);
			if (umrecipient == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ConsumeUpdateForCallerInformation: Calling party could not be resolved", new object[0]);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "ConsumeUpdateForCallerInformation: Resolved Party To: _Callee.", new object[0]);
			UMSubscriber umsubscriber = umrecipient as UMSubscriber;
			if (umsubscriber != null)
			{
				this.CallerInfo = umsubscriber;
				return;
			}
			CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, data, "ConsumeUpdateForCallerInformation: Resolved Party _Callee is not of type UMSubscriber", new object[0]);
			this.CallerInfo = null;
			if (umrecipient != null)
			{
				umrecipient.Dispose();
			}
		}

		internal bool TryHandlePlayOnPhonePAAGreetingCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext::HandlePlayOnPhonePAAGreetingCall CallType={0}", new object[]
			{
				this.CallType
			});
			return this.IsPlayOnPhonePAAGreetingCall;
		}

		internal bool HandleDirectAutoAttendantCallNoHuntGroup(string pilotNumber, PlatformSipUri requestUri, UMIPGateway gateway, bool gatewayInOnlyOneDialplan, ADObjectId gatewayDialPlanId)
		{
			UMAutoAttendant autoAttendant = AutoAttendantUtils.GetAutoAttendant(pilotNumber, requestUri, gateway, gatewayInOnlyOneDialplan, this.IsSecuredCall, gatewayDialPlanId, ADSystemConfigurationLookupFactory.CreateFromOrganizationId(gateway.OrganizationId));
			return this.CheckAndSetAA(autoAttendant, pilotNumber);
		}

		internal bool TryHandleTransferredCallFromAnotherServer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CallContext::TryHandleTransferredCallFromAnotherServer() header value = [{0}]", new object[]
			{
				this.ReferredByHeader ?? "<null>"
			});
			if (string.IsNullOrEmpty(this.ReferredByHeader))
			{
				return false;
			}
			UMRecipient caller = null;
			UserTransferWithContext.DeserializedReferredByHeader deserializedReferredByHeader = null;
			if (!UserTransferWithContext.TryParseReferredByHeader(this.ReferredByHeader, this.DialPlan, out caller, out deserializedReferredByHeader))
			{
				return false;
			}
			switch (deserializedReferredByHeader.TypeOfTransferredCall)
			{
			case 3:
				return this.HandleTransferredSubscriberAccessCall(caller);
			case 4:
				return this.HandleTransferredCallAnsweringCall(caller);
			default:
				return false;
			}
		}

		internal bool TryHandleCallWithCallerIdOfSubscriber()
		{
			UMRecipient umrecipient = this.ResolveCallerFromCallerId();
			PIIMessage data = PIIMessage.Create(PIIType._Callee, umrecipient);
			if (this.CallType == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): CallType = None, Checking if we need to redirect the recipient", new object[0]);
				if (umrecipient == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): recipient is null", new object[0]);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): Recipient=_Callee RequiresLegacySARedirect={0} ", new object[]
					{
						umrecipient.RequiresLegacyRedirectForSubscriberAccess
					});
					if (umrecipient.RequiresRedirectForSubscriberAccess())
					{
						this.OfferResult = OfferResult.Redirect;
						this.serverPicker = RedirectTargetChooserFactory.CreateFromRecipient(this, umrecipient);
						this.LegacySubscriber = umrecipient;
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): Recipient=_Callee will be redirected to an appropriate server", new object[0]);
						return false;
					}
				}
			}
			if (umrecipient != null)
			{
				UMSubscriber umsubscriber = umrecipient as UMSubscriber;
				if (umsubscriber != null)
				{
					if (!umsubscriber.IsLinkedToDialPlan(this.DialPlan))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): Subscriber=_Callee is not in the correct dialplan.", new object[0]);
						umsubscriber.Dispose();
					}
					else
					{
						PIIMessage[] data2 = new PIIMessage[]
						{
							PIIMessage.Create(PIIType._EmailAddress, umsubscriber.MailAddress),
							PIIMessage.Create(PIIType._UserDisplayName, umsubscriber.DisplayName),
							PIIMessage.Create(PIIType._PII, umsubscriber.ExchangeLegacyDN)
						};
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data2, "CallContext::TryHandleCallWithCallerIdOfSubscriber() Caller Info : _EmailAddress _UserDisplayName _PII.", new object[0]);
						this.CallerInfo = umsubscriber;
					}
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "CallContext::TryHandleCallWithCallerIdOfSubscriber(): Recipient=_Callee is not a subscriber", new object[0]);
					umrecipient.Dispose();
				}
			}
			return true;
		}

		internal void AsyncGetCallAnsweringData(bool evaluatePAA)
		{
			string diversion = this.originalCalledParty ?? this.Extension;
			this.UmSubscriberData = new NonBlockingCallAnsweringData(this.CalleeInfo, this.callId, this.CallerId, diversion, evaluatePAA);
		}

		internal void SwitchToCallAnswering(UMRecipient user)
		{
			this.SetCallAnsweringCallType(user, true);
		}

		internal bool SwitchToFallbackAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ENTER: SwitchToFallbackAutoAttendant().", new object[0]);
			ADObjectId dtmffallbackAutoAttendant = this.AutoAttendantInfo.DTMFFallbackAutoAttendant;
			if (dtmffallbackAutoAttendant == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "DTMF FallbackAA was null on SpeechAA: DN=\"{0}\".", new object[]
				{
					this.AutoAttendantInfo.Id.DistinguishedName
				});
				return false;
			}
			return this.SwitchToAutoAttendant(dtmffallbackAutoAttendant, this.AutoAttendantInfo.OrganizationId);
		}

		internal bool SwitchToDefaultAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ENTER: SwitchToDefaultAutoAttendant().", new object[0]);
			ADObjectId umautoAttendant = this.DialPlan.UMAutoAttendant;
			if (umautoAttendant == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Default AutoAttendant was null on DialPlan: DN=\"{0}\".", new object[]
				{
					this.DialPlan.Id.DistinguishedName
				});
				return false;
			}
			return this.SwitchToAutoAttendant(umautoAttendant, this.DialPlan.OrganizationId);
		}

		internal bool SwitchToAutoAttendant(ADObjectId autoAttendantIdentity, OrganizationId orgId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ENTER: SwitchToAutoAttendant({0}).", new object[]
			{
				autoAttendantIdentity.DistinguishedName
			});
			UMAutoAttendant umautoAttendant = this.UMConfigCache.Find<UMAutoAttendant>(autoAttendantIdentity, orgId);
			if (umautoAttendant == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "FallbackAA \"{0}\" was was not found. It may have been deleted while the server was handling call.", new object[]
				{
					autoAttendantIdentity.DistinguishedName
				});
				return false;
			}
			if (StatusEnum.Disabled == umautoAttendant.Status)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "FallbackAA \"{0}\" was not Enabled by admin.", new object[]
				{
					autoAttendantIdentity.DistinguishedName
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallToUnusableAA, null, new object[]
				{
					umautoAttendant.Name,
					Strings.DisabledAA
				});
				return false;
			}
			LocalizedString localizedString;
			if (!AutoAttendantCore.IsRunnableAutoAttendant(umautoAttendant, out localizedString))
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "AutoAttendant with Name {0} in dialplan {1} is not Enabled, or does not have any features enabled.", new object[]
				{
					umautoAttendant.Name,
					umautoAttendant.UMDialPlan.Name
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallToUnusableAA, null, new object[]
				{
					umautoAttendant.Name,
					localizedString
				});
				return false;
			}
			this.LogCallData();
			this.SetCurrentAutoAttendant(umautoAttendant);
			return true;
		}

		internal int CalcCallDuration()
		{
			if (this.connectionTime != null)
			{
				int num = (int)Math.Abs((ExDateTime.UtcNow - this.connectionTime.Value).TotalSeconds);
				return Math.Min(num, (this.DialPlan == null) ? num : (this.DialPlan.MaxCallDuration * 60));
			}
			return 0;
		}

		internal void IncrementHungUpAfterDelayCounter()
		{
			switch (this.CallType)
			{
			case 3:
				this.IncrementCounter(SubscriberAccessCounters.CallsDisconnectedByCallersDuringUMAudioHourglass);
				return;
			case 4:
				this.IncrementCounter(CallAnswerCounters.CallsDisconnectedByCallersDuringUMAudioHourglass);
				return;
			default:
				return;
			}
		}

		internal void IncrementCounter(ExPerformanceCounter counter)
		{
			this.IncrementCounter(counter, 1L);
		}

		internal void IncrementCounter(ExPerformanceCounter counter, long count)
		{
			if (this.IsDiagnosticCall)
			{
				return;
			}
			this.UpdateIncrementedCountersList(counter, count);
			Util.IncrementCounter(counter, count);
		}

		internal void DecrementCounter(ExPerformanceCounter counter)
		{
			if (this.IsDiagnosticCall)
			{
				return;
			}
			this.UpdateIncrementedCountersList(counter, -1L);
			Util.DecrementCounter(counter);
		}

		internal void SetCounter(ExPerformanceCounter counter, long value)
		{
			if (this.IsDiagnosticCall)
			{
				return;
			}
			Util.SetCounter(counter, value);
		}

		internal void TrackDirectoryAccessFailures(LocalizedException exception)
		{
			this.IncrementCounter(AvailabilityCounters.DirectoryAccessFailures);
		}

		internal void LogCallData()
		{
			this.callLogHelper.LogCallData();
		}

		internal void SetCurrentAutoAttendant(UMAutoAttendant aaconfig)
		{
			this.autoAttendantInfo = aaconfig;
			this.autoAttendantCulture = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "AutoAttendant Info : Name={0} DialPlan={1} TimeZone={2}.", new object[]
			{
				this.autoAttendantInfo.Name,
				this.autoAttendantInfo.UMDialPlan,
				this.autoAttendantInfo.TimeZone
			});
			if (string.IsNullOrEmpty(aaconfig.TimeZone))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "AutoAttendant Warning : The Unified Messaging auto attendant {0} has not been configured with a valid time zone.", new object[]
				{
					aaconfig.Name
				});
			}
			this.businessHour = true;
			this.autoAttendantSettings = aaconfig.GetCurrentSettings(out this.holidaySettings, ref this.businessHour);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Using AutoAttendantSettings: Name=\"{0}\" Dial-Plan: {1} Business-Hour: {2}.", new object[]
			{
				this.autoAttendantSettings.Parent.Name,
				this.autoAttendantSettings.Parent.UMDialPlan,
				this.businessHour
			});
			if (this.holidaySettings != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Using Holiday: Name=\"{0}\"", new object[]
				{
					this.holidaySettings.Name
				});
			}
			this.CallType = 2;
		}

		internal void AddDiagnosticsTimer(Timer timer)
		{
			if (timer != null)
			{
				this.diagnosticsTimers.Add(timer);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.IncrementCounter(GeneralCounters.TotalCalls);
				this.SetConnectionTimeCounters();
				CallRejectionCounterHelper.Instance.SetCounters(this.CallRejectionException, new Action<bool>(this.SetCallRejectionCounters), this.CallRejectionException != null, this.IsDiagnosticCall);
				this.LogCallData();
				if (this.CallerInfo != null)
				{
					this.CallerInfo.Dispose();
					this.CallerInfo = null;
				}
				if (this.CalleeInfo != null)
				{
					this.CalleeInfo.Dispose();
					this.CalleeInfo = null;
				}
				if (this.UmSubscriberData != null)
				{
					this.UmSubscriberData.Dispose();
					this.UmSubscriberData = null;
				}
				if (this.LegacySubscriber != null)
				{
					this.LegacySubscriber.Dispose();
					this.LegacySubscriber = null;
				}
				this.diagnosticsTimers.ForEach(delegate(Timer o)
				{
					o.Dispose();
				});
				this.diagnosticsTimers.Clear();
				this.CallType = 0;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CallContext>(this);
		}

		private static ReasonForCall GetReasonForCall(PlatformDiversionInfo diversionInfo)
		{
			ReasonForCall result = ReasonForCall.None;
			switch (diversionInfo.RedirectReason)
			{
			case RedirectReason.UserBusy:
				result = ReasonForCall.DivertBusy;
				break;
			case RedirectReason.NoAnswer:
				result = ReasonForCall.DivertNoAnswer;
				break;
			case RedirectReason.Unconditional:
				result = ReasonForCall.DivertForward;
				break;
			case RedirectReason.Deflection:
				result = ReasonForCall.DivertForward;
				break;
			}
			return result;
		}

		private void ProcessDiversionInformation()
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(1);
			SetDiversionInfoResult setDiversionInfoResult = SetDiversionInfoResult.Invalid;
			foreach (PlatformDiversionInfo platformDiversionInfo in this.CallInfo.DiversionInfo)
			{
				stringBuilder.Append(platformDiversionInfo.OriginalCalledParty).Append(" ");
				setDiversionInfoResult = this.SetDiversionInfo(platformDiversionInfo);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "SetDiversionInfo returned:{0} - Iteration:{1}", new object[]
				{
					setDiversionInfoResult,
					num
				});
				if (SetDiversionInfoResult.ObjectFound == setDiversionInfoResult)
				{
					this.PrepareForCallAnsweringRedirectIfNecessary();
					break;
				}
				if (SetDiversionInfoResult.UserCallingItself == setDiversionInfoResult)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext: User calling itself.", new object[0]);
					break;
				}
				if (SetDiversionInfoResult.ObjectNotFound == setDiversionInfoResult)
				{
					if (++num == 6)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext: MaxNumOfDiversionLookups reached.", new object[0]);
						break;
					}
				}
				else
				{
					if (setDiversionInfoResult == SetDiversionInfoResult.Invalid)
					{
						throw CallRejectedException.Create(Strings.InvalidDiversionReceived(platformDiversionInfo.DiversionHeader), CallEndingReason.InvalidDiversionFormat, "Diversion number: {0}.", new object[]
						{
							platformDiversionInfo.OriginalCalledParty
						});
					}
					throw new NotSupportedException(setDiversionInfoResult.ToString());
				}
			}
			if (SetDiversionInfoResult.ObjectNotFound == setDiversionInfoResult)
			{
				this.divertedExtensionAllowVoiceMail = false;
				this.IncrementCounter(CallAnswerCounters.DivertedExtensionNotProvisioned);
				string text = stringBuilder.ToString().Trim();
				UmGlobals.ExEvent.LogEvent(this.DialPlan.OrganizationId, UMEventLogConstants.Tuple_DivertedExtensionNotProvisioned, text, CommonUtil.ToEventLogString(text), CommonUtil.ToEventLogString(this.dialPlan.Name), CommonUtil.ToEventLogString(this.callId), CommonUtil.ToEventLogString(this.gatewayDetails));
			}
		}

		private void UpdateDisposableInstance<T>(ref T oldValue, T newValue, string propertyName) where T : DisposableBase
		{
			if (oldValue != null && !oldValue.IsDisposed && !object.ReferenceEquals(oldValue, newValue))
			{
				oldValue.Dispose();
			}
			oldValue = newValue;
			if (!string.IsNullOrEmpty(propertyName))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Changed {0} to: {1}", new object[]
				{
					propertyName,
					newValue
				});
			}
		}

		private UMRecipient ResolveCallerFromCallerId()
		{
			UMRecipient umrecipient = null;
			try
			{
				umrecipient = SubscriberAccessUtils.ResolveCaller(this.CallerId, this.CalleeInfo, this.dialPlan);
			}
			finally
			{
				this.UpdateCallerResolutionCounters(umrecipient != null);
			}
			return umrecipient;
		}

		private void SetCallAnsweringCallType(UMRecipient user, bool getCallAnsweringData)
		{
			this.CallType = 4;
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._EmailAddress, user.MailAddress),
				PIIMessage.Create(PIIType._UserDisplayName, user.DisplayName)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "SetCallAnsweringCallType() Callee Info : _EmailAddress _UserDisplayName", new object[0]);
			this.CalleeInfo = user;
			if (getCallAnsweringData)
			{
				this.AsyncGetCallAnsweringData(true);
			}
		}

		private bool HandleTransferredSubscriberAccessCall(UMRecipient caller)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CallContext::HandleTransferredSubscriberAccessCall caller type name {0}", new object[]
			{
				caller.GetType().Name
			});
			if (caller.RequiresRedirectForSubscriberAccess())
			{
				PIIMessage data = PIIMessage.Create(PIIType._Caller, caller);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "CallContext::HandleTransferredSubscriberAccessCall Found caller=_Caller RequiresRedirectForSubscriberAccess={0}.", new object[]
				{
					caller.RequiresRedirectForSubscriberAccess()
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CallContext::HandleTransferredSubscriberAccessCall call will be redirected", new object[0]);
				this.LegacySubscriber = caller;
				this.offerResult = OfferResult.Redirect;
				this.serverPicker = RedirectTargetChooserFactory.CreateFromRecipient(this, caller);
			}
			else
			{
				this.CallerInfo = (caller as UMSubscriber);
				this.callIsOVATransferForUMSubscriber = true;
				this.CallType = 1;
			}
			return true;
		}

		private bool HandleTransferredCallAnsweringCall(UMRecipient caller)
		{
			this.SetCallAnsweringCallType(caller, true);
			return true;
		}

		private void PopulateRemotePeer(bool callIsOutBound, IPAddress remoteEndp, string remoteMatchedFQDN)
		{
			if (callIsOutBound)
			{
				this.gatewayDetails = this.GatewayConfig.Address;
				this.securedCall = (this.DialPlan.VoIPSecurity != UMVoIPSecurityType.Unsecured);
				return;
			}
			this.immediatePeer = remoteEndp;
			if (string.IsNullOrEmpty(remoteMatchedFQDN))
			{
				this.securedCall = false;
				this.gatewayDetails = new UMSmartHost(this.immediatePeer.ToString());
				return;
			}
			this.securedCall = true;
			this.gatewayDetails = new UMSmartHost(remoteMatchedFQDN);
		}

		private void PopulateSIPHeaders(bool callIsOutbound, PlatformCallInfo callInfo)
		{
			this.referredByHeader = RouterUtils.GetReferredByHeader(callInfo.RemoteHeaders);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.ReferredByHeader() Value = \"{0}\"", new object[]
			{
				this.referredByHeader ?? "<null>"
			});
			if (!callIsOutbound)
			{
				this.fromAddress = callInfo.CallingParty;
				this.requestUri = callInfo.RequestUri;
				this.toAddress = callInfo.CalledParty;
				this.localResourcePath = callInfo.RequestUri.FindParameter("local-resource-path");
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InboundCallParams, null, new object[]
				{
					CommonUtil.ToEventLogString(callInfo.CallingParty.Uri),
					CommonUtil.ToEventLogString(callInfo.CalledParty.Uri),
					CommonUtil.ToEventLogString(RouterUtils.GetDiversionLogString(callInfo.DiversionInfo)),
					CommonUtil.ToEventLogString(this.ReferredByHeader),
					this.callId,
					callInfo.RemotePeer
				});
				this.ReasonForCall = ReasonForCall.Direct;
				return;
			}
			this.fromAddress = callInfo.CalledParty;
			this.toAddress = callInfo.CallingParty;
			this.requestUri = ((callInfo.CallingParty != null) ? callInfo.CallingParty.Uri : null);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_OutboundCallParams, null, new object[]
			{
				CommonUtil.ToEventLogString(callInfo.CallingParty),
				CommonUtil.ToEventLogString(callInfo.CalledParty.Uri),
				this.callId
			});
			this.ReasonForCall = ReasonForCall.Outbound;
		}

		private void Initialize(IList<PlatformSignalingHeader> sipInviteHeaders)
		{
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._Callee, this.FromUriOfCall),
				PIIMessage.Create(PIIType._PhoneNumber, this.extnNumber)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "Callee Info : _Callee, _PhoneNumber.", new object[0]);
			if (Util.MaxCallLimitExceeded())
			{
				int num = (CommonConstants.MaxCallsAllowed != null) ? CommonConstants.MaxCallsAllowed.Value : -1;
				throw CallRejectedException.Create(Strings.MaxCallsLimitReached(num), CallEndingReason.MaxCallsReached, "Maximum configured value: {0}.", new object[]
				{
					num
				});
			}
			this.CheckifTestCall();
			this.IsTroubleshootingToolCall = this.CheckifUmTroubleshootingToolCall(sipInviteHeaders);
			this.IsActiveMonitoringCall = SipPeerManager.Instance.IsActiveMonitoringCall(sipInviteHeaders);
			if (this.TryHandleLocalDiagnosticCall())
			{
				return;
			}
			if (this.TryHandlePlayOnPhoneCall())
			{
				return;
			}
			if (this.TryHandleFindMeSubscriberCall())
			{
				return;
			}
			if (this.TryHandlePlayOnPhonePAAGreetingCall())
			{
				return;
			}
			if (this.TryHandleAccessProxyCall())
			{
				return;
			}
			PIIMessage[] data2 = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._Caller, this.FromUriOfCall),
				PIIMessage.Create(PIIType._Callee, this.requestUri)
			};
			CallIdTracer.TracePfd(ExTraceGlobals.PFDUMCallAcceptanceTracer, this, data2, "PFD UMC {0} - Initializing Call from Gateway: {1} Caller: _Caller Callee: _Callee.", new object[]
			{
				8698,
				this.gatewayDetails
			});
			if (!this.TryExecuteDefaultCallHandling())
			{
				return;
			}
			this.InitializeCallerAndCalleeIds();
		}

		private void InitializeCallerAndCalleeIds()
		{
			this.InitializeCallerId(this.fromAddress, true);
			RouterUtils.ParseSipUri(this.requestUri, this.dialPlan, out this.calleeId);
		}

		internal void InitializeCallerId(PlatformTelephonyAddress callerAddress, bool throwIfCallerIdInValid)
		{
			PIIMessage data = PIIMessage.Create(PIIType._Caller, callerAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "InitializeCallerId : updating callerId to _Caller.", new object[0]);
			RouterUtils.ParseTelephonyAddress(callerAddress, this.dialPlan, throwIfCallerIdInValid, out this.callerId);
			this.callerIdDisplayName = this.GetCallerIdDisplayName(callerAddress);
		}

		private string GetCallerIdDisplayName(PlatformTelephonyAddress address)
		{
			if (address == null || string.IsNullOrEmpty(address.Name))
			{
				return null;
			}
			bool flag = UtilityMethods.IsAnonymousAddress(address);
			bool flag2 = PhoneNumber.IsValidPhoneNumber(address.Name);
			if (!flag && !flag2)
			{
				return address.Name;
			}
			return null;
		}

		private void CheckifTestCall()
		{
			if (GlobCfg.EnableRemoteGWAutomation)
			{
				this.callIsTest = true;
				return;
			}
			if (!string.IsNullOrEmpty(this.RemoteUserAgent))
			{
				string text = this.RemoteUserAgent.Replace(" ", string.Empty);
				string value = "Unified Messaging Test Client".Replace(" ", string.Empty);
				if (text.IndexOf(value, StringComparison.InvariantCulture) > 0)
				{
					this.callIsTest = true;
				}
			}
		}

		private bool CheckifUmTroubleshootingToolCall(IList<PlatformSignalingHeader> sipInviteHeaders)
		{
			foreach (PlatformSignalingHeader platformSignalingHeader in sipInviteHeaders)
			{
				if (platformSignalingHeader.Name.Equals("msexum-diagtool", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool TryHandleAccessProxyCall()
		{
			bool result = false;
			if (this.RoutingHelper.SupportsMsOrganizationRouting)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "TryHandleAccessProxyCall: {0} supports ms-organization routing.", new object[]
				{
					this.RemoteFQDN
				});
				string diversionUri = this.diversionUserAtHost;
				SipRoutingHelper.Context routingContext = this.RoutingHelper.GetRoutingContext(this.ToUriOfCall.SimplifiedUri, this.FromUriOfCall.SimplifiedUri, diversionUri, this.RequestUriOfCall);
				if (routingContext.AutoAttendant != null && this.CheckAndSetAA(routingContext.AutoAttendant, this.requestUri.Host))
				{
					this.InitializeCallerAndCalleeIds();
				}
				else
				{
					if (routingContext.DialPlanId == null)
					{
						throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.InvalidRequest, null, new object[0]);
					}
					this.DialPlan = this.UMConfigCache.Find<UMDialPlan>(routingContext.DialPlanId, routingContext.OrgId);
					this.InitializeCallerAndCalleeIds();
				}
				result = true;
			}
			return result;
		}

		private bool TryExecuteDefaultCallHandling()
		{
			ExAssert.RetailAssert(this.gatewayConfig != null, "Gateway object was not set for an in bound call");
			string text = (!UtilityMethods.IsAnonymousNumber(this.RequestUriOfCall.User)) ? this.RequestUriOfCall.User : null;
			IADSystemConfigurationLookup adSession = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.gatewayConfig.OrganizationId, false);
			ADObjectId gatewayDialPlanId;
			bool gatewayInOnlyOneDialplan;
			UMHuntGroup huntGroup = HuntGroupUtils.GetHuntGroup(text, this.gatewayConfig, this.RequestUriOfCall, this.GatewayDetails, adSession, this.IsSecuredCall, out gatewayDialPlanId, out gatewayInOnlyOneDialplan);
			if (this.TryHandleDirectAutoAttendantCall(text, huntGroup, this.gatewayConfig, gatewayInOnlyOneDialplan, gatewayDialPlanId))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found AA {0} that matched pilot# {1}.", new object[]
				{
					this.autoAttendantInfo.Id,
					text
				});
				return true;
			}
			if (huntGroup != null)
			{
				this.HandleSubscriberAccessCallWithHuntgroup(huntGroup);
				return true;
			}
			if (this.gatewayConfig.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.E164)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Global Gateway call ({0}), but no huntgroup, Virtual Number or AA found  for {1}.", new object[]
				{
					this.gatewayConfig.Address,
					text
				});
				throw CallRejectedException.Create(Strings.GlobalGatewayWithNoMatch(this.gatewayConfig.Address.ToString(), text), CallEndingReason.GlobalGatewayWithNoMatch, "UMIPGateway: {0}. Pilot number: {1}.", new object[]
				{
					this.gatewayConfig.Address.ToString(),
					text
				});
			}
			UMHuntGroup huntGroup2 = null;
			if (HuntGroupUtils.TryGetDefaultHuntGroup(this.gatewayConfig, text, out huntGroup2))
			{
				this.HandleSubscriberAccessCallWithHuntgroup(huntGroup2);
				return true;
			}
			throw CallRejectedException.Create(Strings.CallFromInvalidHuntGroup(text, this.gatewayDetails.ToString()), CallEndingReason.IncorrectHuntGroup, "Pilot number: {0}. UMIPGateway: {1}.", new object[]
			{
				text,
				this.gatewayDetails.ToString()
			});
		}

		private bool HandleSIPToHeaderLookups(CallContext.SIPToHeaderLookup lookupType, UMIPGateway gw)
		{
			string text = (!UtilityMethods.IsAnonymousNumber(this.ToUriOfCall.User)) ? this.ToUriOfCall.User : null;
			if (string.IsNullOrEmpty(text) || gw.GlobalCallRoutingScheme != UMGlobalCallRoutingScheme.E164)
			{
				return false;
			}
			if (lookupType == CallContext.SIPToHeaderLookup.DirectAA)
			{
				return this.TryFindAAWithNoDPKnowledge(text);
			}
			throw new ArgumentException("lookupType");
		}

		private void PrepareForCallAnsweringRedirectIfNecessary()
		{
			if (this.callType == 4 && this.CalleeInfo.RequiresRedirectForCallAnswering())
			{
				this.OfferResult = OfferResult.Redirect;
				this.serverPicker = RedirectTargetChooserFactory.CreateFromRecipient(this, this.CalleeInfo);
			}
		}

		private bool TryHandleDirectAutoAttendantCall(string pilotNumber, UMHuntGroup huntGroup, UMIPGateway gateway, bool gatewayInOnlyOneDialplan, ADObjectId gatewayDialPlanId)
		{
			return this.HandleSIPToHeaderLookups(CallContext.SIPToHeaderLookup.DirectAA, gateway) || this.TryHandleDirectAACallWithDPKnowledge(pilotNumber, huntGroup, gateway, gatewayInOnlyOneDialplan, gatewayDialPlanId);
		}

		private bool TryFindAAWithNoDPKnowledge(string aaNumber)
		{
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromTenantGuid(this.TenantGuid);
			UMAutoAttendant autoAttendantWithNoDialplanInformation = iadsystemConfigurationLookup.GetAutoAttendantWithNoDialplanInformation(aaNumber);
			return autoAttendantWithNoDialplanInformation != null && this.CheckAndSetAA(autoAttendantWithNoDialplanInformation, aaNumber);
		}

		private bool TryHandleDirectAACallWithDPKnowledge(string pilotNumber, UMHuntGroup huntGroup, UMIPGateway gateway, bool gatewayInOnlyOneDialplan, ADObjectId gatewayDialPlanId)
		{
			if (pilotNumber == null)
			{
				return false;
			}
			if (huntGroup == null)
			{
				return this.HandleDirectAutoAttendantCallNoHuntGroup(pilotNumber, this.RequestUriOfCall, gateway, gatewayInOnlyOneDialplan, gatewayDialPlanId);
			}
			UMAutoAttendant autoAttendant = AutoAttendantUtils.GetAutoAttendant(pilotNumber, huntGroup, this.RequestUriOfCall, this.IsSecuredCall, ADSystemConfigurationLookupFactory.CreateFromOrganizationId(gateway.OrganizationId));
			return this.CheckAndSetAA(autoAttendant, pilotNumber);
		}

		private void HandleSubscriberAccessCallWithHuntgroup(UMHuntGroup huntGroup)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Finding dialplan with huntgroup {0}.", new object[]
			{
				huntGroup.Id
			});
			this.dialPlan = this.UMConfigCache.Find<UMDialPlan>(huntGroup.UMDialPlan, huntGroup.OrganizationId);
			if (this.dialPlan != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found dialplan : {0}.", new object[]
				{
					this.dialPlan.Id
				});
				return;
			}
			CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "Dial plan object not found {0}", new object[]
			{
				huntGroup.UMDialPlan
			});
			throw CallRejectedException.Create(Strings.DialPlanNotFound(huntGroup.UMDialPlan.DistinguishedName), CallEndingReason.DialPlanNotFound, "UM dial plan: {0}.", new object[]
			{
				huntGroup.UMDialPlan.DistinguishedName
			});
		}

		private bool TryHandleFindMeSubscriberCall()
		{
			if (this.IsFindMeSubscriberCall)
			{
				PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, this.CallerInfo.MailAddress);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "CallContext created for FindMeSubscriberCall caller _EmailAddress.", new object[0]);
				return true;
			}
			return false;
		}

		private bool TryHandleLocalDiagnosticCall()
		{
			return this.IsLocalDiagnosticCall;
		}

		private bool TryHandlePlayOnPhoneCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext::HandlePlayOnPhoneCalll CallType={0}", new object[]
			{
				this.CallType
			});
			if (!this.IsPlayOnPhoneCall)
			{
				return false;
			}
			PlayOnPhoneAAGreetingRequest playOnPhoneAAGreetingRequest = this.webServiceRequest as PlayOnPhoneAAGreetingRequest;
			if (playOnPhoneAAGreetingRequest != null && this.UMConfigCache.Find<UMAutoAttendant>(playOnPhoneAAGreetingRequest.AutoAttendant.Id, playOnPhoneAAGreetingRequest.AutoAttendant.OrganizationId) == null)
			{
				throw new InvalidUMAutoAttendantException();
			}
			this.IncrementCounter(GeneralCounters.TotalPlayOnPhoneCalls);
			return true;
		}

		private bool LookupAutoAttendantInDialPlan(string pilotNumberOrName, bool numberIsPilot, ADObjectId dialPlanId)
		{
			UMAutoAttendant aa = AutoAttendantUtils.LookupAutoAttendantInDialPlan(pilotNumberOrName, numberIsPilot, dialPlanId, ADSystemConfigurationLookupFactory.CreateFromTenantGuid(this.TenantGuid));
			return this.CheckAndSetAA(aa, pilotNumberOrName);
		}

		private bool CheckAndSetAA(UMAutoAttendant aa, string pilotNumberOrName)
		{
			if (aa == null)
			{
				return false;
			}
			aa = this.UMConfigCache.Find<UMAutoAttendant>(aa.Id, aa.OrganizationId);
			if (aa == null)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "AutoAttendant with PilotNumberOrName '{0}' could not be loaded or does not have a valid configuration.", new object[]
				{
					pilotNumberOrName
				});
				return false;
			}
			if (!AutoAttendantUtils.IsAutoAttendantUsable(aa, pilotNumberOrName))
			{
				return false;
			}
			this.SetCurrentAutoAttendant(aa);
			this.dialPlan = this.UMConfigCache.Find<UMDialPlan>(aa.UMDialPlan, aa.OrganizationId);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Found Enabled AutoAttendant - Name/Number = [{0}] DP = [{1}].", new object[]
			{
				pilotNumberOrName,
				aa.UMDialPlan.Name
			});
			return true;
		}

		private void UpdateIncrementedCountersList(ExPerformanceCounter counter, long value)
		{
			if (this.incrementedCounters.ContainsKey(counter))
			{
				this.incrementedCounters[counter] = this.incrementedCounters[counter] + value;
			}
			else
			{
				this.incrementedCounters.Add(counter, value);
			}
			if (this.incrementedCounters[counter] < 0L)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "Counter '{0}' was decremented more times than it was incremented. Stack trace: {1}.", new object[]
				{
					counter.CounterName,
					new StackTrace()
				});
			}
		}

		private bool IsCounterIncremented(ExPerformanceCounter counter)
		{
			return this.incrementedCounters.ContainsKey(counter) && this.incrementedCounters[counter] > 0L;
		}

		private void SetCallRejectionCounters(bool isCallRejected)
		{
			if (isCallRejected)
			{
				this.IncrementCounter(AvailabilityCounters.UMWorkerProcessCallsRejected);
			}
			this.SetCounter(AvailabilityCounters.RecentUMWorkerProcessCallsRejected, (long)CallContext.recentPercentageRejectedCalls.Update(!isCallRejected));
		}

		private void SetConnectionTimeCounters()
		{
			if (this.connectionTime != null)
			{
				int num = this.CalcCallDuration();
				long num2 = CallContext.averageCallDuration.Update((long)num);
				long num3 = CallContext.averageRecentCallDuration.Update((long)num);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.Dispose(). Call Duration={0} (seconds). New Average={1}. New Recent Average={2}", new object[]
				{
					num,
					num2,
					num3,
					num3
				});
				this.SetCounter(GeneralCounters.AverageCallDuration, num2);
				this.SetCounter(GeneralCounters.AverageRecentCallDuration, num3);
				if (this.CallerInfo != null && this.CallerInfo.IsAuthenticated)
				{
					this.SetCounter(SubscriberAccessCounters.AverageSubscriberCallDuration, num2);
					this.SetCounter(SubscriberAccessCounters.AverageRecentSubscriberCallDuration, num3);
					return;
				}
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext::SetConnectionTimeCounters()  Call is not connected, skipping call duration counter updates", new object[0]);
			}
		}

		private void UpdateCallerResolutionCounters(bool callerLookupSuccessful)
		{
			if (7 == this.CallType)
			{
				return;
			}
			CallContext.UpdateCountersAndPercentages(callerLookupSuccessful, GeneralCounters.CallerResolutionsSucceeded, GeneralCounters.CallerResolutionsAttempted, GeneralCounters.PercentageSuccessfulCallerResolutions, GeneralCounters.PercentageSuccessfulCallerResolutions_Base);
			if (this.DialPlan != null && this.DialPlan.URIType == UMUriType.TelExtn && this.CallerId.UriType == UMUriType.TelExtn && this.CallerId.Number.Length == this.DialPlan.NumberOfDigitsInExtension)
			{
				CallContext.UpdateCountersAndPercentages(callerLookupSuccessful, GeneralCounters.ExtensionCallerResolutionsSucceeded, GeneralCounters.ExtensionCallerResolutionsAttempted, GeneralCounters.PercentageSuccessfulExtensionCallerResolutions, GeneralCounters.PercentageSuccessfulExtensionCallerResolutions_Base);
			}
		}

		private void PopulateTenantGuid(bool callIsOutbound)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.PopulateTenantGuid - callIsOutbound='{0}' IsLocalDiagnosticCall='{1}'", new object[]
			{
				callIsOutbound,
				this.IsLocalDiagnosticCall
			});
			ExAssert.RetailAssert(!this.IsLocalDiagnosticCall, "PopulateTenantGuid cannot be called for diagnostic calls.");
			this.TenantGuid = Guid.Empty;
			if (CommonConstants.UseDataCenterCallRouting)
			{
				if (callIsOutbound)
				{
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.DialPlan.OrganizationId);
					this.TenantGuid = iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId();
				}
				else
				{
					this.TenantGuid = Util.GetTenantGuid(this.CallInfo.RequestUri);
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "CallContext.PopulateTenantGuid - tenantGuid='{0}'", new object[]
			{
				this.TenantGuid
			});
		}

		private static Average averageCallDuration = new Average();

		private static MovingAverage averageRecentCallDuration = new MovingAverage(50);

		private static PercentageBooleanSlidingCounter recentPercentageRejectedCalls = PercentageBooleanSlidingCounter.CreateFailureCounter(1000, TimeSpan.FromHours(1.0));

		private CallLoggingHelper callLogHelper;

		private PhoneNumber callerId = PhoneNumber.Empty;

		private PhoneNumber calleeId = PhoneNumber.Empty;

		private string callerIdDisplayName;

		private string originalCalledParty;

		private string extnNumber;

		private string diversionUserAtHost;

		private UMSubscriber callerInfo;

		private PlatformTelephonyAddress fromAddress;

		private PlatformSipUri requestUri;

		private PlatformTelephonyAddress toAddress;

		private UMRecipient calleeInfo;

		private UMAutoAttendant autoAttendantInfo;

		private Dictionary<ExPerformanceCounter, long> incrementedCounters = new Dictionary<ExPerformanceCounter, long>();

		private bool callIsDiagnostic;

		private bool callIsLocalDiagnostic;

		private bool callIsTUIDiagnostic;

		private ExDateTime? connectionTime = null;

		private ExDateTime callStartTime = ExDateTime.UtcNow;

		private UMSmartHost gatewayDetails;

		private IPAddress immediatePeer;

		private bool securedCall;

		private UMDialPlan dialPlan;

		private CultureInfo dialPlanCulture;

		private CultureInfo autoAttendantCulture;

		private bool businessHour;

		private HolidaySchedule holidaySettings;

		private AutoAttendantSettings autoAttendantSettings;

		private CallType callType;

		private ReasonForCall reasonForCall;

		private DropCallReason reasonForDisconnect;

		private OfferResult offerResult;

		private string callId;

		private string localResourcePath;

		private bool callIsTest;

		private bool hasVoicemailBeenSubmitted;

		private PlayOnPhoneRequest webServiceRequest;

		private OCFeature officeCommunicatorFeature = new OCFeature();

		private IPAACommonInterface linkedManagerPointer;

		private bool faxToneReceived;

		private NonBlockingCallAnsweringData subscriberData;

		private string referredByHeader;

		private bool callIsOVATransferForUMSubscriber;

		private IRedirectTargetChooser serverPicker;

		private UMRecipient unsupportedSubscriber;

		private UMIPGateway gatewayConfig;

		private UMConfigCache promptConfigCache = new UMConfigCache();

		private bool divertedExtensionAllowVoiceMail = true;

		private List<Timer> diagnosticsTimers = new List<Timer>(2);

		private enum SIPToHeaderLookup
		{
			VirtualNumber,
			DirectAA
		}
	}
}
