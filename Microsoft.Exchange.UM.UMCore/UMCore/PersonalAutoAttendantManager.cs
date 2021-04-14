using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PersonalAutoAttendantManager : PAAManagerBase, IPAAParent, IPAACommonInterface
	{
		internal bool FindMeSuccessful { get; private set; }

		internal bool IsFirstFindMeTry { get; private set; }

		internal bool AreThereMoreFindMeNumbers
		{
			get
			{
				return this.index < this.findMeNumbers.Count;
			}
		}

		internal IPAAChild SubscriberManager
		{
			get
			{
				return (IPAAChild)base.CallSession.CurrentCallContext.LinkedManagerPointer;
			}
			set
			{
				base.CallSession.CurrentCallContext.LinkedManagerPointer = value;
			}
		}

		public object GetCallerRecordedName()
		{
			if (this.RecordedNameOfCaller != null)
			{
				return this.RecordedNameOfCaller;
			}
			return this.ReadVariable("recording");
		}

		public object GetCalleeRecordedName()
		{
			return base.RecordedName;
		}

		public void TerminateFindMe()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: TerminateFindMe() ", new object[0]);
			this.SubscriberManager = null;
			this.TransferToPAAMainMenu();
		}

		public void DisconnectChildCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: DisconnectChildCall() ", new object[0]);
			base.CallSession.TerminateDependentSession();
		}

		public void SetPointerToChild(IPAAChild pointer)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: SetPointerToChild()", new object[0]);
			base.CallSession.CurrentCallContext.LinkedManagerPointer = pointer;
		}

		public void ContinueFindMe()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: ContinueFindMe() ", new object[0]);
			if (this.state != FindMeState.FindMe || this.FindMeSuccessful)
			{
				return;
			}
			this.SubscriberManager = null;
			this.DoNextFindMe();
		}

		public void AcceptCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: AcceptCall() ", new object[0]);
			base.TargetPhoneNumber = this.currentFindmeNumber;
			this.FindMeSuccessful = true;
			base.CallSession.StopPlaybackAndCancelRecognition();
		}

		internal string StartFindMe(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: StartFindMe() ", new object[0]);
			this.currentFindmeNumber = null;
			this.IsFirstFindMeTry = true;
			this.GetCalleeRecordedName();
			this.index = 0;
			FindMe givenNumber;
			if (!this.TryGetNextFindMeNumber(out givenNumber))
			{
				throw new InvalidOperationException();
			}
			return this.FindMe(givenNumber);
		}

		internal string TerminateFindMe(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: TerminateFindMe(vo) ", new object[0]);
			this.state = FindMeState.None;
			this.CancelOutBoundCallTimer();
			if (this.SubscriberManager == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: calling vo.TerminateDependentSession() ", new object[0]);
				this.DropChildCall();
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: calling this.SubscriberManager.TerminateCall() ", new object[0]);
				this.SubscriberManager.TerminateCall();
				this.SubscriberManager = null;
			}
			return null;
		}

		internal string CleanupFindMe(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: CleanupFindMe() ", new object[0]);
			if (this.state != FindMeState.FindMe)
			{
				return null;
			}
			return this.TerminateFindMe(null);
		}

		internal string ContinueFindMe(BaseUMCallSession vo)
		{
			string result = "moreFindMeNumbersLeft";
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: ContinueFindMe(vo) ", new object[0]);
			bool findMeSuccessful = this.FindMeSuccessful;
			this.FindMeSuccessful = false;
			this.IsFirstFindMeTry = false;
			if (findMeSuccessful || !this.AreThereMoreFindMeNumbers)
			{
				this.state = FindMeState.None;
				result = null;
			}
			this.SubscriberManager.TerminateCallToTryNextNumberTransfer();
			this.SubscriberManager = null;
			return result;
		}

		internal override void OnOutBoundCallRequestCompleted(BaseUMCallSession outboundCallSession, OutboundCallDetailsEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: OnOutBoundCallRequestCompleted(), mystate = {0}, outcome ={1}", new object[]
			{
				this.state,
				callSessionEventArgs.CallOutcome
			});
			if (this.state != FindMeState.FindMe)
			{
				outboundCallSession.DisconnectCall();
				return;
			}
			this.CancelOutBoundCallTimer();
			outboundCallSession.CurrentCallContext.CallType = 9;
			if (callSessionEventArgs.CallOutcome == OutboundCallDetailsEventArgs.OutboundCallOutcome.Failure)
			{
				outboundCallSession.HandleFailedOutboundCall(callSessionEventArgs);
				this.state = FindMeState.None;
				this.DoNextFindMe();
				return;
			}
			outboundCallSession.CurrentCallContext.LinkedManagerPointer = this;
			outboundCallSession.CurrentCallContext.CallerInfo = new UMSubscriber(base.CallSession.CurrentCallContext.CalleeInfo.ADRecipient);
			outboundCallSession.CurrentCallContext.CallerInfo.IsAuthenticated = true;
			outboundCallSession.CurrentCallContext.DialPlan = base.CallSession.CurrentCallContext.DialPlan;
			outboundCallSession.InitializeConnectedCall(callSessionEventArgs);
		}

		private void DropChildCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: DropChildCall() ", new object[0]);
			base.CallSession.DisconnectDependentUMCallSession();
		}

		private void CancelOutBoundCallTimer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: CancelOutBoundCallTimer() ", new object[0]);
			if (this.outboundCallTimer != null && this.outboundCallTimer.IsActive)
			{
				this.outboundCallTimer.Dispose();
				this.outboundCallTimer = null;
			}
		}

		private void MaximumOutboundCallConnectTimeExceeded(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: MaximumOutboundCallConnectTimeExceeded(vo) ", new object[0]);
			this.CancelOutBoundCallTimer();
			this.DisconnectChildCall();
		}

		private string FindMe(FindMe givenNumber)
		{
			this.state = FindMeState.FindMe;
			bool flag = false;
			FindMe findMe = givenNumber;
			BaseUMCallSession baseUMCallSession = null;
			while (!flag)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: FindMe(), number= {0}, Timeout ={1} secs, currentCalls ={2}, maxcallsAllowed={3}", new object[]
				{
					findMe.Number,
					findMe.Timeout,
					GeneralCounters.CurrentCalls.RawValue,
					(CommonConstants.MaxCallsAllowed != null) ? CommonConstants.MaxCallsAllowed.Value.ToString() : "not set"
				});
				if (Util.MaxCallLimitExceeded())
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FindMeFailedSinceMaximumCallsLimitReached, null, new object[]
					{
						CommonConstants.MaxCallsAllowed.Value,
						base.CallSession.CallId
					});
					this.state = FindMeState.None;
					return "maxAllowedCallsLimitReached";
				}
				this.currentFindmeNumber = findMe.PhoneNumber;
				try
				{
					UmServiceGlobals.VoipPlatform.CreateAndMakeCallOnDependentSession(base.CallSession, new UMCallSessionHandler<OutboundCallDetailsEventArgs>(this.OnOutBoundCallRequestCompleted), base.Subscriber, this.GetCallerIdToUseForFindMe(), this.currentFindmeNumber, out baseUMCallSession);
					baseUMCallSession.CurrentCallContext.CallLoggingHelper.FindMeDialedString = this.currentFindmeNumber.ToDial;
					flag = true;
				}
				catch (InvalidPhoneNumberException)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FindMeInvalidPhoneNumber, null, new object[]
					{
						CommonUtil.ToEventLogString(base.CallSession.CurrentCallContext.CalleeInfo.ADRecipient.Name),
						CommonUtil.ToEventLogString(this.currentFindmeNumber.ToDial),
						base.CallSession.CallId
					});
					if (!this.HandleDialingFailed(out findMe))
					{
						return "dialingRulesCheckFailed";
					}
				}
				catch (DialingRulesException)
				{
					if (!this.HandleDialingFailed(out findMe))
					{
						return "dialingRulesCheckFailed";
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: Creating outboundCall Timer", new object[0]);
			this.outboundCallTimer = base.CallSession.StartTimer(new BaseUMAsyncTimer.UMTimerCallback(this.MaximumOutboundCallConnectTimeExceeded), findMe.Timeout);
			return null;
		}

		private bool HandleDialingFailed(out FindMe number)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: HandleDialingFailed", new object[0]);
			if (!this.TryGetNextFindMeNumber(out number))
			{
				base.CallSession.TerminateDependentSession();
				this.state = FindMeState.None;
				return false;
			}
			return true;
		}

		private void DropFindMeOutboundCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: DropFindMeOutboundCall", new object[0]);
			if (this.state == FindMeState.FindMe)
			{
				this.TerminateFindMe(null);
			}
		}

		private bool TryGetNextFindMeNumber(out FindMe number)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: TryGetNextFindMeNumber()", new object[0]);
			if (this.index >= this.findMeNumbers.Count)
			{
				number = null;
				return false;
			}
			PhoneNumber phoneNumber;
			bool flag;
			for (;;)
			{
				number = this.findMeNumbers[this.index];
				flag = PhoneNumber.TryParse(base.CallSession.CurrentCallContext.DialPlan, number.Number, out phoneNumber);
				this.index++;
				if (flag)
				{
					break;
				}
				if (this.index >= this.findMeNumbers.Count)
				{
					return flag;
				}
			}
			number.PhoneNumber = phoneNumber;
			return flag;
		}

		private string GetCallerIdToUseForFindMe()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: GetCallerIdToUseForFindMe()", new object[0]);
			if (base.CallSession.CurrentCallContext.IsVirtualNumberCall)
			{
				UMSubscriber umsubscriber = base.CallSession.CurrentCallContext.CalleeInfo as UMSubscriber;
				if (!string.IsNullOrEmpty(umsubscriber.VirtualNumber))
				{
					return umsubscriber.VirtualNumber;
				}
			}
			if (base.CallSession.CurrentCallContext.DialPlan.URIType == UMUriType.SipName)
			{
				return base.CallSession.CurrentCallContext.FromUriOfCall.ToString();
			}
			if (base.CallSession.CurrentCallContext.IsAnonymousCaller)
			{
				return "Anonymous";
			}
			return base.CallSession.CurrentCallContext.CallerId.ToDial;
		}

		private void DoNextFindMe()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: DoNextFindMe()", new object[0]);
			FindMe givenNumber;
			if (!this.TryGetNextFindMeNumber(out givenNumber))
			{
				this.TransferToPAAMainMenu();
				return;
			}
			if (this.FindMe(givenNumber) != null)
			{
				this.TransferToPAAMainMenu();
			}
		}

		private void TransferToPAAMainMenu()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager: TransferToPAAMainMenu()", new object[0]);
			this.state = FindMeState.None;
			this.CancelOutBoundCallTimer();
			base.CallSession.StopPlaybackAndCancelRecognition();
		}

		internal PersonalAutoAttendantManager(ActivityManager manager, PersonalAutoAttendantManager.ConfigClass config) : base(manager, config)
		{
			this.findMeEnabled = false;
			this.state = FindMeState.None;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::ctor()", new object[0]);
		}

		public bool Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		public bool HavePersonalOperator
		{
			get
			{
				return this.operatorNumber != null;
			}
		}

		public bool ExecuteBlindTransfer
		{
			get
			{
				return this.executeBlindTransfer;
			}
			set
			{
				this.executeBlindTransfer = value;
			}
		}

		public bool FindMeEnabled
		{
			get
			{
				return this.findMeEnabled;
			}
		}

		public bool ExecuteTransferToMailbox
		{
			get
			{
				return this.executeTransferToMailbox;
			}
		}

		public bool ExecuteTransferToVoiceMessage
		{
			get
			{
				return this.executeTransferToVoiceMessage;
			}
			set
			{
				this.executeTransferToVoiceMessage = value;
			}
		}

		public bool PermissionCheckFailure
		{
			get
			{
				return this.permissionCheckFailure;
			}
			set
			{
				this.permissionCheckFailure = value;
			}
		}

		public bool InvalidADContact
		{
			get
			{
				return this.invalidADContact;
			}
			set
			{
				this.invalidADContact = value;
			}
		}

		internal string EvaluationStatus
		{
			get
			{
				return this.evaluationStatus.ToString();
			}
		}

		internal object RecordedNameOfCaller
		{
			get
			{
				return this.recordedNameOfCaller;
			}
			set
			{
				this.recordedNameOfCaller = value;
			}
		}

		internal bool CallerIsResolvedToADContact
		{
			get
			{
				return this.callerIsResolvedToADContact;
			}
			set
			{
				this.callerIsResolvedToADContact = value;
			}
		}

		internal bool TimeOut
		{
			get
			{
				return this.timeout;
			}
		}

		internal bool TargetHasValidPAA
		{
			get
			{
				return this.targetHasValidPAA;
			}
		}

		internal bool TargetPAAInDifferentSite
		{
			get
			{
				return this.targetPAAInDifferentSite && !this.targetHasValidPAA;
			}
		}

		internal static bool TryGetTargetPAA(CallContext callcontext, ADRecipient mailboxTransferTarget, UMDialPlan originatorDialPlan, PhoneNumber callerId, out PersonalAutoAttendant paa, out bool requiresTransferToAnotherServer, out BricksRoutingBasedServerChooser serverPicker)
		{
			paa = null;
			requiresTransferToAnotherServer = false;
			serverPicker = null;
			bool result;
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(mailboxTransferTarget))
			{
				if (umsubscriber == null || !umsubscriber.DialPlan.Identity.Equals(originatorDialPlan.Identity))
				{
					result = false;
				}
				else
				{
					serverPicker = new BricksRoutingBasedServerChooser(callcontext, umsubscriber, 4);
					if (serverPicker.IsRedirectionNeeded)
					{
						requiresTransferToAnotherServer = true;
						result = false;
					}
					else
					{
						using (IPAAEvaluator ipaaevaluator = EvaluateUserPAA.Create(umsubscriber, callerId, umsubscriber.Extension))
						{
							if (!ipaaevaluator.GetEffectivePAA(out paa))
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, null, "PersonalAutoAttendantManager::TryGetTargetPAA: Target = {0} did not find valid PAA. Transferring to voicemail maybe", new object[]
								{
									mailboxTransferTarget.DisplayName
								});
								result = false;
							}
							else
							{
								PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, mailboxTransferTarget.DisplayName);
								CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, null, data, "PersonalAutoAttendantManager::TryGetTargetPAA: Target = _UserDisplayName found valid PAA {0}", new object[]
								{
									paa.Identity.ToString()
								});
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		public override void SetADTransferTarget(ADRecipient mailboxTransferTarget)
		{
			this.mailboxTransferTarget = mailboxTransferTarget;
		}

		public override void SetBlindTransferEnabled(bool enabled, PhoneNumber target)
		{
			this.executeBlindTransfer = enabled;
			base.TargetPhoneNumber = target;
		}

		public override void SetPermissionCheckFailure()
		{
			this.permissionCheckFailure = true;
		}

		public override void SetTransferToMailboxEnabled()
		{
			this.executeTransferToMailbox = true;
		}

		public override void SetTransferToVoiceMessageEnabled()
		{
			this.executeTransferToVoiceMessage = true;
		}

		public override void SetInvalidADContact()
		{
			this.invalidADContact = true;
		}

		public override void SetFindMeNumbers(FindMe[] numbers)
		{
			List<FindMe> list = new List<FindMe>();
			foreach (FindMe findMe in numbers)
			{
				if (findMe.ValidationResult == PAAValidationResult.Valid)
				{
					list.Add(findMe);
				}
			}
			if (list.Count > 0)
			{
				this.findMeEnabled = true;
				this.findMeNumbers = list;
				return;
			}
			this.findMeEnabled = false;
			this.SetPermissionCheckFailure();
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::Start()", new object[0]);
			this.paaEvents = new PersonalAutoAttendantManager.PAAEvents(vo);
			base.Subscriber = (UMSubscriber)vo.CurrentCallContext.CalleeInfo;
			this.callId = vo.CallId;
			this.operatorNumber = Util.SA_GetOperatorNumber(vo.CurrentCallContext.DialPlan, base.Subscriber);
			base.Start(vo, refInfo);
		}

		internal string Reset(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::Reset()", new object[0]);
			this.executeBlindTransfer = false;
			this.permissionCheckFailure = false;
			this.executeTransferToVoiceMessage = false;
			this.findMeEnabled = false;
			this.executeTransferToMailbox = false;
			this.invalidADContact = false;
			this.findMeNumbers = null;
			this.keySelected = -1;
			base.RecordContext.Reset();
			return null;
		}

		internal string GetAutoAttendant(BaseUMCallSession vo)
		{
			this.evaluationStatus = PAAEvaluationStatus.Failure;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetAutoAttendant()", new object[0]);
			if (!base.Subscriber.IsPAAEnabled)
			{
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, vo.CurrentCallContext.CalleeInfo.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PersonalAutoAttendantManager::GetAutoAttendant() user _UserDisplayName is not enabled for PAA", new object[0]);
				this.evaluationStatus = PAAEvaluationStatus.Failure;
				return null;
			}
			PersonalAutoAttendant personalAutoAttendant = (PersonalAutoAttendant)this.GlobalManager.ReadVariable("TargetPAA");
			if (personalAutoAttendant == null)
			{
				personalAutoAttendant = vo.CurrentCallContext.UmSubscriberData.PersonalAutoAttendant;
				if (personalAutoAttendant == null)
				{
					this.evaluationStatus = (vo.CurrentCallContext.UmSubscriberData.TimedOut ? PAAEvaluationStatus.Timeout : PAAEvaluationStatus.Failure);
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetAutoAttendant() Did not get PAA. EvaluationStatus = {0}", new object[]
					{
						this.evaluationStatus
					});
				}
				else
				{
					this.evaluationStatus = PAAEvaluationStatus.Success;
					base.PersonalAutoAttendant = personalAutoAttendant;
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallAnsweredByPAA, null, new object[]
					{
						vo.CallId,
						personalAutoAttendant.Identity.ToString(),
						vo.CurrentCallContext.Extension
					});
					this.reader = new NonBlockingReader(new NonBlockingReader.Operation(this.GetGreetingCallback), personalAutoAttendant, PAAConstants.PAAGreetingDownloadTimeout, new NonBlockingReader.TimeoutCallback(this.TimedOutGetGreeting));
					this.reader.StartAsyncOperation();
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetAutoAttendant() Got PAA ID={0} Enabled={1} Version={2} Valid={3}", new object[]
					{
						base.PersonalAutoAttendant.Identity.ToString(),
						base.PersonalAutoAttendant.Enabled,
						base.PersonalAutoAttendant.Version.ToString(),
						base.PersonalAutoAttendant.Valid
					});
				}
				this.paaEvents.EvaluatingPAAComplete(this.evaluationStatus, vo.CurrentCallContext.UmSubscriberData.SubscriberHasPAAConfigured, vo.CurrentCallContext.UmSubscriberData.PAAEvaluationTime);
				return null;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetAutoAttendant() [PAA->PAA TRANSFER] Got PAA ID={0} Enabled={1} Version={2} Valid={3}", new object[]
			{
				personalAutoAttendant.Identity.ToString(),
				personalAutoAttendant.Enabled,
				personalAutoAttendant.Version.ToString(),
				personalAutoAttendant.Valid
			});
			if (PAAUtils.IsCompatible(personalAutoAttendant.Version) && personalAutoAttendant.Enabled)
			{
				this.evaluationStatus = PAAEvaluationStatus.Success;
				base.PersonalAutoAttendant = personalAutoAttendant;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallAnsweredByPAA, null, new object[]
				{
					vo.CallId,
					personalAutoAttendant.Identity.ToString(),
					vo.CurrentCallContext.Extension
				});
				this.reader = new NonBlockingReader(new NonBlockingReader.Operation(this.GetGreetingCallback), personalAutoAttendant, PAAConstants.PAAGreetingDownloadTimeout, new NonBlockingReader.TimeoutCallback(this.TimedOutGetGreeting));
				this.reader.StartAsyncOperation();
				return null;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetAutoAttendant() [PAA->PAA TRANSFER] PAA ID={0} Is not compatible, or not valid", new object[]
			{
				personalAutoAttendant.Identity.ToString()
			});
			this.evaluationStatus = PAAEvaluationStatus.Failure;
			base.PersonalAutoAttendant = null;
			return null;
		}

		internal void TimedOutGetGreeting(object state)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TimedOutRetrievingMailboxData, null, new object[]
			{
				base.Subscriber.MailAddress,
				this.callId,
				base.Subscriber.ExchangePrincipal.MailboxInfo.Location.ServerFqdn,
				base.Subscriber.ExchangePrincipal.MailboxInfo.MailboxDatabase.ToString(),
				CommonUtil.ToEventLogString(new StackTrace(true))
			});
		}

		internal override string GetGreeting(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::GetGreeting()", new object[0]);
			this.reader.WaitForCompletion();
			return null;
		}

		internal string ResolveCallingLineIdToGalContact(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::ResolveCallingLineIdToGalContact()", new object[0]);
			this.RecordedNameOfCaller = null;
			this.CallerIsResolvedToADContact = false;
			IADRecipient iadrecipient = null;
			if (vo.CurrentCallContext.CallerInfo == null)
			{
				ADContactInfo adcontactInfo = null;
				ADContactInfo.TryFindCallerByCallerId(base.Subscriber, vo.CurrentCallContext.CallerId, out adcontactInfo);
				if (adcontactInfo != null)
				{
					iadrecipient = adcontactInfo.ADOrgPerson;
				}
			}
			else
			{
				iadrecipient = vo.CurrentCallContext.CallerInfo.ADRecipient;
			}
			if (iadrecipient == null)
			{
				return null;
			}
			this.CallerIsResolvedToADContact = true;
			this.recordedNameOfCaller = base.GetRecordedName(iadrecipient);
			return null;
		}

		internal string SetOperatorNumber(BaseUMCallSession vo)
		{
			base.TargetPhoneNumber = this.operatorNumber;
			return null;
		}

		internal string PrepareForVoiceMail(BaseUMCallSession vo)
		{
			return null;
		}

		internal string ProcessSelection(BaseUMCallSession vo)
		{
			string dtmfDigits = base.DtmfDigits;
			int key;
			if (dtmfDigits == "#")
			{
				key = 10;
			}
			else
			{
				key = int.Parse(dtmfDigits, CultureInfo.InvariantCulture);
			}
			this.SelectMenu(key);
			return null;
		}

		internal string SelectNextAction(BaseUMCallSession vo)
		{
			if (this.actionsIterator == null)
			{
				this.actionsIterator = base.PersonalAutoAttendant.AutoActionsList.SortedMenu.GetEnumerator();
			}
			if (this.actionsIterator.MoveNext())
			{
				KeyMappingBase keyMappingBase = this.actionsIterator.Current;
				this.SelectMenu(keyMappingBase.Key);
				return null;
			}
			return "noActionLeft";
		}

		internal string HandleTimeout(BaseUMCallSession vo)
		{
			this.numFailures++;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::HandleTimeout() NumFailures={0} MaxAllowed = {1}.", new object[]
			{
				this.numFailures,
				3
			});
			if (this.numFailures >= 3)
			{
				return "menuRetriesExceeded";
			}
			return null;
		}

		internal string PrepareForTransfer(BaseUMCallSession vo)
		{
			return null;
		}

		internal string PrepareForFindMe(BaseUMCallSession vo)
		{
			return this.ResolveCallingLineIdToGalContact(vo);
		}

		internal string TransferToPAASiteFailed(BaseUMCallSession vo)
		{
			this.GlobalManager.WriteVariable("directorySearchResult", ContactSearchItem.CreateFromRecipient(this.mailboxTransferTarget));
			return null;
		}

		internal string PrepareForTransferToMailbox(BaseUMCallSession vo)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, this.mailboxTransferTarget.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PersonalAutoAttendantManager::PrepareForTransferToMailbox: Target = _UserDisplayName", new object[0]);
			PersonalAutoAttendant personalAutoAttendant = null;
			BricksRoutingBasedServerChooser bricksRoutingBasedServerChooser = null;
			this.targetHasValidPAA = PersonalAutoAttendantManager.TryGetTargetPAA(vo.CurrentCallContext, this.mailboxTransferTarget, base.Subscriber.DialPlan, vo.CurrentCallContext.CallerId, out personalAutoAttendant, out this.targetPAAInDifferentSite, out bricksRoutingBasedServerChooser);
			this.SendMissedCall(vo);
			if (!this.targetHasValidPAA)
			{
				if (!this.targetPAAInDifferentSite)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PersonalAutoAttendantManager::PrepareForTransferToMailbox: Target = _DisplayName did not find valid PAA. Transferring to voicemail", new object[0]);
					this.GlobalManager.WriteVariable("directorySearchResult", ContactSearchItem.CreateFromRecipient(this.mailboxTransferTarget));
				}
				else
				{
					ExAssert.RetailAssert(bricksRoutingBasedServerChooser != null, "ServerPicker cannot be null if transferToAnotherServer is needed");
					vo.CurrentCallContext.ServerPicker = bricksRoutingBasedServerChooser;
					string referredByHostUri = null;
					if (vo.CurrentCallContext.CallInfo.DiversionInfo.Count > 0)
					{
						referredByHostUri = vo.CurrentCallContext.CallInfo.DiversionInfo[0].UserAtHost;
					}
					UserTransferWithContext userTransferWithContext = new UserTransferWithContext(referredByHostUri);
					this.GlobalManager.WriteVariable("ReferredByUri", userTransferWithContext.SerializeCACallTransferWithContextUri(this.mailboxTransferTarget.UMExtension, vo.CurrentCallContext.DialPlan.PhoneContext));
				}
			}
			else
			{
				this.targetPAA = personalAutoAttendant;
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PersonalAutoAttendantManager::PrepareForTransferToMailbox: Target = _DisplayName found valid PAA {0}", new object[]
				{
					this.targetPAA.Identity.ToString()
				});
				vo.CurrentCallContext.CalleeInfo = UMRecipient.Factory.FromADRecipient<UMRecipient>(this.mailboxTransferTarget);
			}
			return null;
		}

		internal string PrepareForTransferToPaa(BaseUMCallSession vo)
		{
			if (this.targetPAA == null)
			{
				throw new InvalidOperationException("Got a NULL TargetPAA");
			}
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, this.mailboxTransferTarget.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PersonalAutoAttendantManager::PrepareForTransferToPaa: Target Mailbox = _UserDisplayName, Target PAA = {0}", new object[]
			{
				this.targetPAA.Identity.ToString()
			});
			this.GlobalManager.WriteVariable("TargetPAA", this.targetPAA);
			return null;
		}

		internal string PrepareForTransferToVoicemail(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::PrepareForTransferToVoicemail()", new object[0]);
			return null;
		}

		internal override void OnTransferComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager : OnTransferComplete() called, state = {0} Error = {1}", new object[]
			{
				this.state,
				callSessionEventArgs.Error
			});
			if (this.state == FindMeState.FindMe)
			{
				this.SubscriberManager.TerminateCall();
				this.SubscriberManager = null;
				this.state = FindMeState.None;
			}
			base.OnTransferComplete(vo, callSessionEventArgs);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager : DropCall() called, state = {0} ", new object[]
			{
				this.state
			});
			this.DropFindMeOutboundCall();
			this.SendMissedCall(vo);
			base.DropCall(vo, reason);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager : OnUserHangup() called, state = {0} ", new object[]
			{
				this.state
			});
			this.SendMissedCall(vo);
			this.DropFindMeOutboundCall();
			base.OnUserHangup(vo, voiceEventArgs);
		}

		internal override void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::OnTimeout()", new object[0]);
			this.timeout = true;
			base.OnTimeout(vo, callSessionEventArgs);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::OnInput()", new object[0]);
			this.timeout = false;
			base.OnInput(vo, callSessionEventArgs);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.reader != null)
					{
						this.reader.Dispose();
					}
					if (this.actionsIterator != null)
					{
						this.actionsIterator.Dispose();
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
			return DisposeTracker.Get<PersonalAutoAttendantManager>(this);
		}

		private void SendMissedCall(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::SendMissedCall() this.sendMissedCall = {0}", new object[]
			{
				this.sentMissedCall
			});
			int num = Interlocked.Increment(ref this.sentMissedCall);
			if (num == 1)
			{
				RecordVoicemailManager.MessageSubmissionHelper messageSubmissionHelper = RecordVoicemailManager.MessageSubmissionHelper.Create(vo);
				messageSubmissionHelper.SubmitMissedCall(false);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonalAutoAttendantManager::SendMissedCall() MissedCall was already sent. Returning.", new object[0]);
		}

		private void GetGreetingCallback(object state)
		{
			using (new CallId(this.callId))
			{
				this.GetGreetingCallbackWorker(state);
			}
		}

		private void GetGreetingCallbackWorker(object state)
		{
			PersonalAutoAttendant personalAutoAttendant = (PersonalAutoAttendant)state;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "GetGreetingCallback() PAA {0}", new object[]
			{
				personalAutoAttendant.Identity
			});
			base.LoadGreetingForPAA(personalAutoAttendant);
		}

		private void SelectMenu(int key)
		{
			this.keySelected = key;
			PAAMenuItem paamenuItem = base.PAAMenuItems[this.keySelected];
			this.findMeEnabled = false;
			PAAManagerBase.PAAPresentationObject paapresentationObject = base.Menu[key];
			paapresentationObject.SetVariablesForTransfer(this);
		}

		private const int MaxAllowedFailures = 3;

		private int index;

		private BaseUMAsyncTimer outboundCallTimer;

		private PhoneNumber currentFindmeNumber;

		private FindMeState state;

		private int numFailures;

		private PhoneNumber operatorNumber;

		private bool findMeEnabled;

		private bool executeBlindTransfer;

		private bool executeTransferToMailbox;

		private bool executeTransferToVoiceMessage;

		private bool permissionCheckFailure;

		private bool invalidADContact;

		private PAAEvaluationStatus evaluationStatus;

		private bool targetHasValidPAA;

		private bool targetPAAInDifferentSite;

		private bool timeout;

		private ADRecipient mailboxTransferTarget;

		private PersonalAutoAttendant targetPAA;

		private int keySelected;

		private PersonalAutoAttendantManager.PAAEvents paaEvents;

		private List<FindMe> findMeNumbers;

		private NonBlockingReader reader;

		private string callId;

		private object recordedNameOfCaller;

		private bool callerIsResolvedToADContact;

		private int sentMissedCall;

		private IEnumerator<KeyMappingBase> actionsIterator;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Constructing PersonalAutoAttendant activity manager.", new object[0]);
				return new PersonalAutoAttendantManager(manager, this);
			}

			internal override void Load(XmlNode rootNode)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Loading a new PersonalAutoAttendantConfig.", new object[0]);
				base.Load(rootNode);
			}
		}

		internal class PAAEvents : IPAAEvents
		{
			internal PAAEvents(BaseUMCallSession vo)
			{
				this.vo = vo;
			}

			public void OnBeginEvaluatingPAA()
			{
				this.startTime = ExDateTime.UtcNow;
			}

			public void OnEndEvaluatingPAA(PAAEvaluationStatus status, bool subscriberHasConfiguredPAA)
			{
				TimeSpan elapsedTime = ExDateTime.UtcNow - this.startTime;
				this.EvaluatingPAAComplete(status, subscriberHasConfiguredPAA, elapsedTime);
			}

			public void EvaluatingPAAComplete(PAAEvaluationStatus status, bool subscriberHasConfiguredPAA, TimeSpan elapsedTime)
			{
				if (!subscriberHasConfiguredPAA)
				{
					return;
				}
				this.vo.IncrementCounter(CallAnswerCounters.CallsForSubscribersHavingOneOrMoreCARConfigured);
				switch (status)
				{
				case PAAEvaluationStatus.Success:
					this.vo.IncrementCounter(CallAnswerCounters.TotalCARCalls);
					this.vo.SetCounter(CallAnswerCounters.CAREvaluationAverageTime, PersonalAutoAttendantManager.PAAEvents.averageEvaluationTime.Update((long)elapsedTime.TotalSeconds));
					return;
				case PAAEvaluationStatus.Failure:
					break;
				case PAAEvaluationStatus.Timeout:
					this.vo.IncrementCounter(CallAnswerCounters.CARTimedOutEvaluations);
					break;
				default:
					return;
				}
			}

			private static MovingAverage averageEvaluationTime = new MovingAverage(50);

			private BaseUMCallSession vo;

			private ExDateTime startTime;
		}
	}
}
