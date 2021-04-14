using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallLoggingHelper
	{
		public long CallsInProgressWhenStarted { get; set; }

		public PhoneNumber TransferTarget { get; set; }

		public ContactInfo TransferContactInfo { get; set; }

		public string ParentCallIdentifier { get; set; }

		public string FindMeDialedString { get; set; }

		public bool IsCallAnsweringVoiceMessage { get; set; }

		public AudioQuality AudioMetrics { get; set; }

		public CallLoggingHelper(CallContext callContext)
		{
			ValidateArgument.NotNull(callContext, "callContext");
			this.callContext = callContext;
			this.AudioMetrics = AudioQuality.CreateDefaultAudioQuality();
		}

		public void SetTransferTarget(PhoneNumber transferTarget, ContactInfo transferContactInfo)
		{
			this.TransferTarget = transferTarget;
			this.TransferContactInfo = transferContactInfo;
		}

		private void ClearCountersData()
		{
			this.successfulLogons = 0L;
			this.logonFailures = 0L;
			this.voiceMessagesCreated = 0L;
			this.directoryAccessed = 0L;
			this.directoryAccessedByExtension = 0L;
			this.directoryAccessedByDialByName = 0L;
			this.directoryAccessedSuccessfullyByDialByName = 0L;
			this.directoryAccessedBySpeakingAName = 0L;
			this.directoryAccessedSuccessfullyBySpeakingAName = 0L;
			this.voiceMessagesHeard = 0L;
			this.protectedVoiceMessagesHeard = 0L;
			this.voiceMessagesDeleted = 0L;
			this.emailMessagesHeard = 0L;
			this.emailMessagesDeleted = 0L;
			this.meetingsAccepted = 0L;
			this.meetingsDeclinedOrCanceled = 0L;
			this.repliedToMeetingOrganizer = 0L;
			this.contactsAccessed = 0L;
			this.customMenuOptionTaken = 0L;
		}

		public void LogCallData()
		{
			this.PullCallDataFromCounters();
			this.callDuration = this.callContext.CalcCallDuration();
			string callData = this.GetCallData();
			ExEventLog.EventTuple tuple;
			switch (this.callContext.CallType)
			{
			case 2:
				tuple = UMEventLogConstants.Tuple_CallDataAutoAttendant;
				goto IL_86;
			case 3:
				tuple = UMEventLogConstants.Tuple_CallDataSubscriberAccess;
				goto IL_86;
			case 4:
				tuple = UMEventLogConstants.Tuple_CallDataCallAnswer;
				goto IL_86;
			case 5:
			case 9:
			case 10:
				tuple = UMEventLogConstants.Tuple_CallDataOutbound;
				goto IL_86;
			case 6:
				tuple = UMEventLogConstants.Tuple_CallDataFax;
				goto IL_86;
			}
			tuple = UMEventLogConstants.Tuple_CallData;
			IL_86:
			UmGlobals.ExEvent.LogEvent(tuple, null, new object[]
			{
				CommonUtil.ToEventLogString(callData)
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Call data: {0}.", new object[]
			{
				callData
			});
			this.SendSubscriberCallLogs();
			this.SendCDREmail();
			this.LogCallStatistics();
		}

		private void LogCallStatistics()
		{
			CDRData cdrdata = this.CreateAndFillCDRData(Guid.Empty);
			CallStatisticsLogger.CallStatisticsLogRow callStatisticsLogRow = new CallStatisticsLogger.CallStatisticsLogRow();
			callStatisticsLogRow.CallStartTime = cdrdata.CallStartTime;
			callStatisticsLogRow.CallType = cdrdata.CallType;
			callStatisticsLogRow.CallIdentity = cdrdata.CallIdentity;
			callStatisticsLogRow.ParentCallIdentity = cdrdata.ParentCallIdentity;
			callStatisticsLogRow.UMServerName = cdrdata.UMServerName;
			callStatisticsLogRow.DialPlanGuid = cdrdata.DialPlanGuid;
			callStatisticsLogRow.DialPlanName = cdrdata.DialPlanName;
			callStatisticsLogRow.CallDuration = cdrdata.CallDuration;
			callStatisticsLogRow.IPGatewayAddress = cdrdata.IPGatewayAddress;
			callStatisticsLogRow.GatewayGuid = cdrdata.GatewayGuid;
			callStatisticsLogRow.CalledPhoneNumber = cdrdata.CalledPhoneNumber;
			callStatisticsLogRow.CallerPhoneNumber = cdrdata.CallerPhoneNumber;
			callStatisticsLogRow.OfferResult = cdrdata.OfferResult;
			callStatisticsLogRow.DropCallReason = cdrdata.DropCallReason;
			callStatisticsLogRow.ReasonForCall = cdrdata.ReasonForCall;
			callStatisticsLogRow.TransferredNumber = cdrdata.TransferredNumber;
			callStatisticsLogRow.DialedString = cdrdata.DialedString;
			callStatisticsLogRow.CallerMailboxAlias = cdrdata.CallerMailboxAlias;
			callStatisticsLogRow.CalleeMailboxAlias = cdrdata.CalleeMailboxAlias;
			callStatisticsLogRow.AutoAttendantName = cdrdata.AutoAttendantName;
			callStatisticsLogRow.OrganizationId = Util.GetTenantName(this.callContext.DialPlan);
			callStatisticsLogRow.AudioCodec = cdrdata.AudioQualityMetrics.AudioCodec;
			callStatisticsLogRow.AudioQualityBurstDensity = cdrdata.AudioQualityMetrics.BurstDensity;
			callStatisticsLogRow.AudioQualityBurstDuration = cdrdata.AudioQualityMetrics.BurstDuration;
			callStatisticsLogRow.AudioQualityJitter = cdrdata.AudioQualityMetrics.Jitter;
			callStatisticsLogRow.AudioQualityNMOS = cdrdata.AudioQualityMetrics.NMOS;
			callStatisticsLogRow.AudioQualityNMOSDegradation = cdrdata.AudioQualityMetrics.NMOSDegradation;
			callStatisticsLogRow.AudioQualityNMOSDegradationJitter = cdrdata.AudioQualityMetrics.NMOSDegradationJitter;
			callStatisticsLogRow.AudioQualityNMOSDegradationPacketLoss = cdrdata.AudioQualityMetrics.NMOSDegradationPacketLoss;
			callStatisticsLogRow.AudioQualityPacketLoss = cdrdata.AudioQualityMetrics.PacketLoss;
			callStatisticsLogRow.AudioQualityRoundTrip = cdrdata.AudioQualityMetrics.RoundTrip;
			CallStatisticsLogger.Instance.Append(callStatisticsLogRow);
		}

		private void SendSubscriberCallLogs()
		{
			if (this.callContext.IsSubscriberAccessCall)
			{
				UMSubscriber callerInfo = this.callContext.CallerInfo;
				if (callerInfo.IsVirtualNumberEnabled && !PhoneNumber.IsNullOrEmpty(this.TransferTarget))
				{
					UMMessageSubmission.SubmitOutgoingCallLog(this.callContext.CallId, this.TransferTarget, this.TransferContactInfo, callerInfo, this.callContext.TenantGuid);
					return;
				}
			}
			else if (this.callContext.IsVirtualNumberCall)
			{
				UMSubscriber recipient = (UMSubscriber)this.callContext.CalleeInfo;
				UMMessageSubmission.SubmitIncomingCallLog(this.callContext.CallId, this.callContext.CallerId, recipient, this.callContext.TenantGuid);
			}
		}

		private void SendCDREmail()
		{
			if (!AppConfig.Instance.Service.CDRLoggingEnabled || this.callContext.DialPlan == null || this.callContext.DialPlan.SubscriberType == UMSubscriberType.Consumer || (this.callContext.GatewayConfig != null && this.callContext.GatewayConfig.Simulator) || this.callContext.CallType == 7 || this.callContext.IsTroubleshootingToolCall || this.callContext.IsActiveMonitoringCall || this.callContext.OfferResult == OfferResult.Redirect)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Will not log a CDR. UMDialPlanId = {0}, Calltype={1}, CDRLoggingEnabled = {2}, TroubleshootingCall = {3}, UMIPGateway.Simulator = {4}, OfferResult = {5}", new object[]
				{
					this.GetDialPlanId(),
					this.callContext.CallType,
					AppConfig.Instance.Service.CDRLoggingEnabled,
					this.callContext.IsTroubleshootingToolCall,
					(this.callContext.GatewayConfig != null) ? this.callContext.GatewayConfig.Simulator.ToString() : "gateway object null",
					this.callContext.OfferResult
				});
				return;
			}
			try
			{
				OrganizationId organizationId = this.callContext.DialPlan.OrganizationId;
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(organizationId, null);
				ADUser umdataStorageMailbox = iadrecipientLookup.GetUMDataStorageMailbox();
				Guid objectGuid = umdataStorageMailbox.Database.ObjectGuid;
				PipelineSubmitStatus pipelineSubmitStatus = PipelineDispatcher.Instance.CanSubmitWorkItem(objectGuid.ToString(), PipelineDispatcher.ThrottledWorkItemType.CDRWorkItem);
				if (pipelineSubmitStatus != PipelineSubmitStatus.Ok)
				{
					PIIMessage data = PIIMessage.Create(PIIType._User, umdataStorageMailbox.DistinguishedName);
					CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, data, "Unable to save CDR message: pipeline is full. Mailbox _User, Database Guid {0}, Submit status {1}", new object[]
					{
						objectGuid,
						pipelineSubmitStatus
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToSaveCDR, null, new object[]
					{
						Strings.PipelineFullWithCDRMessages(umdataStorageMailbox.DistinguishedName, umdataStorageMailbox.Database.ToDNString())
					});
				}
				else
				{
					CDRData cdrData = this.CreateAndFillCDRData(umdataStorageMailbox.Guid);
					UMMessageSubmission.SubmitCDREmail(cdrData);
				}
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "Caught an exception while logging CDR. Exception {0}.", new object[]
				{
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToSaveCDR, null, new object[]
				{
					CommonUtil.ToEventLogString(ex)
				});
				if (!(ex is ObjectNotFoundException) && !(ex is NonUniqueRecipientException))
				{
					throw;
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToFindEDiscoveryMailbox, null, new object[]
				{
					this.callContext.DialPlan.OrganizationId,
					CommonUtil.ToEventLogString(ex)
				});
			}
		}

		public CDRData CreateAndFillCDRData(Guid ediscoveryUserObjectGuid)
		{
			return new CDRData(this.callContext.CallReceivedTime.UniversalTime, this.GetCallTypeDescription(), Utils.CheckString(this.callContext.CallId), Utils.CheckString(this.ParentCallIdentifier), Utils.GetLocalHostName(), this.GetDialPlanId(), this.GetDialPlanName(), this.callDuration, this.GetUMIPGatewayAddress(), this.GetUMIPGatewayName(), this.GetGatewayId(), Utils.CheckString(this.callContext.Extension), (this.callContext.CallerId != null) ? this.callContext.CallerId.ToDial : string.Empty, this.GetOfferResultDescription(), this.GetDropCallReason(), this.GetReasonForCallDescription(), (this.TransferTarget != null) ? this.TransferTarget.ToDial : string.Empty, this.GetDialedString(), this.GetCallerAlias(), this.GetCalleeAlias(), this.GetCallerLegacyExchangeDN(), this.GetCalleeLegacyExchangeDN(), this.GetAutoAttendantName(), ediscoveryUserObjectGuid, this.AudioMetrics, this.callContext.TenantGuid);
		}

		private string GetUMIPGatewayAddress()
		{
			if (this.callContext.GatewayConfig != null && this.callContext.GatewayConfig.Address != null)
			{
				return this.callContext.GatewayConfig.Address.ToString();
			}
			return string.Empty;
		}

		private string GetUMIPGatewayName()
		{
			if (this.callContext.GatewayConfig != null)
			{
				return this.callContext.GatewayConfig.Name;
			}
			return string.Empty;
		}

		private string GetCallTypeDescription()
		{
			switch (this.callContext.CallType)
			{
			case 0:
				return "None";
			case 1:
				return "UnAuthenticatedPilotNumber";
			case 2:
				return "AutoAttendant";
			case 3:
				return "SubscriberAccess";
			case 4:
				if (this.callContext.IsVirtualNumberCall)
				{
					return "VirtualNumberCall";
				}
				if (this.IsCallAnsweringVoiceMessage)
				{
					return "CallAnsweringVoiceMessage";
				}
				return "CallAnsweringMissedCall";
			case 5:
				return "PlayOnPhone";
			case 6:
				return "Fax";
			case 7:
				return "Diagnostics";
			case 8:
				return "PromptProvisioning";
			case 9:
				return "FindMe";
			case 10:
				return "PlayOnPhonePAAGreeting";
			default:
				throw new NotImplementedException("Unknown enum value");
			}
		}

		private string GetDuration()
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, this.callDuration);
			return timeSpan.ToString();
		}

		private string GetReasonForCallDescription()
		{
			switch (this.callContext.ReasonForCall)
			{
			case ReasonForCall.None:
				return "None";
			case ReasonForCall.Direct:
				return "Direct";
			case ReasonForCall.DivertNoAnswer:
				return "DivertNoAnswer";
			case ReasonForCall.DivertBusy:
				return "DivertBusy";
			case ReasonForCall.DivertForward:
				return "DivertForward";
			case ReasonForCall.Outbound:
				return "Outbound";
			default:
				throw new NotImplementedException("Unknown enum value");
			}
		}

		private string GetDropCallReason()
		{
			switch (this.callContext.ReasonForDisconnect)
			{
			case DropCallReason.None:
				return "None";
			case DropCallReason.UserError:
				return "UserError";
			case DropCallReason.SystemError:
				return "SystemError";
			case DropCallReason.GracefulHangup:
				return "GracefulHangup";
			case DropCallReason.OutboundFailedCall:
				return "OutboundFailedCall";
			default:
				throw new NotImplementedException("Unknown enum value");
			}
		}

		private string GetOfferResultDescription()
		{
			switch (this.callContext.OfferResult)
			{
			case OfferResult.None:
				return "None";
			case OfferResult.Answer:
				return "Answer";
			case OfferResult.Reject:
				return "Reject";
			case OfferResult.Redirect:
				return "Redirect";
			default:
				throw new NotImplementedException("Unknown enum value");
			}
		}

		private Guid GetDialPlanId()
		{
			if (this.callContext.DialPlan != null)
			{
				return this.callContext.DialPlan.Guid;
			}
			return Guid.Empty;
		}

		private Guid GetGatewayId()
		{
			if (this.callContext.GatewayConfig != null)
			{
				return this.callContext.GatewayConfig.Guid;
			}
			return Guid.Empty;
		}

		private string GetDialPlanName()
		{
			if (this.callContext.DialPlan != null)
			{
				return this.callContext.DialPlan.Name;
			}
			return string.Empty;
		}

		private string GetAutoAttendantId()
		{
			if (this.callContext.AutoAttendantInfo != null)
			{
				return this.callContext.AutoAttendantInfo.Id.ObjectGuid.ToString();
			}
			return string.Empty;
		}

		private string GetAutoAttendantName()
		{
			if (this.callContext.AutoAttendantInfo != null)
			{
				return this.callContext.AutoAttendantInfo.Name;
			}
			return string.Empty;
		}

		private string GetCallerUserId()
		{
			if (this.callContext.CallerInfo != null && this.callContext.CallerInfo.ADRecipient != null)
			{
				return this.callContext.CallerInfo.ADRecipient.Id.ObjectGuid.ToString();
			}
			return string.Empty;
		}

		private string GetCalleeUserId()
		{
			if (this.callContext.CalleeInfo != null && this.callContext.CalleeInfo.ADRecipient != null)
			{
				return this.callContext.CalleeInfo.ADRecipient.Id.ObjectGuid.ToString();
			}
			return string.Empty;
		}

		private string GetCalleeAlias()
		{
			if (this.callContext.CalleeInfo != null && this.callContext.CalleeInfo.ADRecipient != null)
			{
				return this.callContext.CalleeInfo.ADRecipient.Alias;
			}
			return string.Empty;
		}

		private string GetCalleeLegacyExchangeDN()
		{
			if (this.callContext.CalleeInfo != null && this.callContext.CalleeInfo.ADRecipient != null)
			{
				return this.callContext.CalleeInfo.ADRecipient.LegacyExchangeDN;
			}
			return string.Empty;
		}

		private string GetCallerLegacyExchangeDN()
		{
			if (this.callContext.CallerInfo != null && this.callContext.CallerInfo.ADRecipient != null)
			{
				return this.callContext.CallerInfo.ADRecipient.LegacyExchangeDN;
			}
			return string.Empty;
		}

		private string GetCallerAlias()
		{
			if (this.callContext.CallerInfo != null && this.callContext.CallerInfo.ADRecipient != null)
			{
				return this.callContext.CallerInfo.ADRecipient.Alias;
			}
			return string.Empty;
		}

		private string GetDialedString()
		{
			if (this.callContext.WebServiceRequest != null)
			{
				PlayOnPhoneRequest webServiceRequest = this.callContext.WebServiceRequest;
				if (webServiceRequest != null)
				{
					return webServiceRequest.DialString;
				}
			}
			else if (this.FindMeDialedString != null)
			{
				return this.FindMeDialedString;
			}
			return string.Empty;
		}

		private void PullCallDataItemFromCounter(ExPerformanceCounter counter, ref long callDataItem)
		{
			this.callContext.IncrementedCounters.TryGetValue(counter, out callDataItem);
		}

		private void PullAACallDataFromCounters(AACountersInstance aac)
		{
			this.PullCallDataItemFromCounter(aac.DirectoryAccessed, ref this.directoryAccessed);
			this.PullCallDataItemFromCounter(aac.DirectoryAccessedByExtension, ref this.directoryAccessedByExtension);
			this.PullCallDataItemFromCounter(aac.DirectoryAccessedByDialByName, ref this.directoryAccessedByDialByName);
			this.PullCallDataItemFromCounter(aac.DirectoryAccessedSuccessfullyByDialByName, ref this.directoryAccessedSuccessfullyByDialByName);
			this.PullCallDataItemFromCounter(aac.DirectoryAccessedBySpokenName, ref this.directoryAccessedBySpeakingAName);
			this.PullCallDataItemFromCounter(aac.DirectoryAccessedSuccessfullyBySpokenName, ref this.directoryAccessedSuccessfullyBySpeakingAName);
			long num = 0L;
			this.callContext.IncrementedCounters.TryGetValue(aac.OperatorTransfers, out num);
			this.customMenuOptionTaken = 0L;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption1, out num);
			this.customMenuOptionTaken += num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption2, out num);
			this.customMenuOptionTaken += 2L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption3, out num);
			this.customMenuOptionTaken += 3L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption4, out num);
			this.customMenuOptionTaken += 4L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption5, out num);
			this.customMenuOptionTaken += 5L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption6, out num);
			this.customMenuOptionTaken += 6L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption7, out num);
			this.customMenuOptionTaken += 7L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption8, out num);
			this.customMenuOptionTaken += 8L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOption9, out num);
			this.customMenuOptionTaken += 9L * num;
			this.callContext.IncrementedCounters.TryGetValue(aac.MenuOptionTimeout, out num);
			this.customMenuOptionTaken += -1L * num;
		}

		private void PullCallDataFromCounters()
		{
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.SubscriberLogons, ref this.successfulLogons);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.SubscriberAuthenticationFailures, ref this.logonFailures);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.VoiceMessagesSent, ref this.voiceMessagesCreated);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.VoiceMessagesHeard, ref this.voiceMessagesHeard);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.ProtectedVoiceMessagesHeard, ref this.protectedVoiceMessagesHeard);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.VoiceMessagesDeleted, ref this.voiceMessagesDeleted);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.EmailMessagesHeard, ref this.emailMessagesHeard);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.EmailMessagesDeleted, ref this.emailMessagesDeleted);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.MeetingsAccepted, ref this.meetingsAccepted);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.MeetingsDeclined, ref this.meetingsDeclinedOrCanceled);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.RepliedToOrganizer, ref this.repliedToMeetingOrganizer);
			this.PullCallDataItemFromCounter(SubscriberAccessCounters.ContactsAccessed, ref this.contactsAccessed);
			if (this.callContext.CallType == 6)
			{
				this.PullCallDataItemFromCounter(FaxCounters.TotalNumberOfValidFaxCalls, ref this.totalNumberOfValidFaxCalls);
				this.PullCallDataItemFromCounter(FaxCounters.TotalNumberOfSuccessfulValidFaxCalls, ref this.totalNumberOfSuccessfulValidFaxCalls);
			}
			if (this.callContext.AutoAttendantInfo != null && !string.IsNullOrEmpty(this.callContext.AutoAttendantInfo.Name) && !CommonConstants.UseDataCenterLogging)
			{
				AACountersInstance instance = AACounters.GetInstance(this.callContext.AutoAttendantInfo.Name);
				if (instance != null)
				{
					this.PullAACallDataFromCounters(instance);
					return;
				}
			}
			else
			{
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessed, ref this.directoryAccessed);
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessedByExtension, ref this.directoryAccessedByExtension);
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessedByDialByName, ref this.directoryAccessedByDialByName);
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessedSuccessfullyByDialByName, ref this.directoryAccessedSuccessfullyByDialByName);
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessedBySpokenName, ref this.directoryAccessedBySpeakingAName);
				this.PullCallDataItemFromCounter(SubscriberAccessCounters.DirectoryAccessedSuccessfullyBySpokenName, ref this.directoryAccessedSuccessfullyBySpeakingAName);
			}
		}

		private string GetCallData()
		{
			string value = (this.TransferTarget != null) ? this.TransferTarget.ToDial : string.Empty;
			StringBuilder stringBuilder = new StringBuilder(this.GetCallTypeDescription());
			stringBuilder.Append(",").Append(this.callContext.CallId);
			stringBuilder.Append(",").Append(Utils.GetLocalHostName());
			stringBuilder.Append(",").Append(this.GetDialPlanId().ToString());
			stringBuilder.Append(",").Append(this.GetDialPlanName());
			stringBuilder.Append(",").Append(this.callContext.CallReceivedTime.ToString("yyyy-MM-ddTHH:mm:szzz", CultureInfo.InvariantCulture));
			stringBuilder.Append(",").Append(this.CallsInProgressWhenStarted);
			stringBuilder.Append(",").Append(this.GetDuration());
			stringBuilder.Append(",").Append(this.GetGatewayId().ToString());
			stringBuilder.Append(",").Append((this.callContext.GatewayConfig != null) ? this.callContext.GatewayConfig.Name : string.Empty);
			stringBuilder.Append(",").Append("\"").Append((this.callContext.CallerId != null) ? this.callContext.CallerId.ToDial : string.Empty).Append("\"");
			stringBuilder.Append(",").Append("\"").Append(this.callContext.Extension).Append("\"");
			stringBuilder.Append(",").Append(this.GetReasonForCallDescription());
			stringBuilder.Append(",").Append(this.GetOfferResultDescription());
			stringBuilder.Append(",").Append(this.GetDropCallReason());
			stringBuilder.Append(",").Append("\"").Append(value).Append("\"");
			stringBuilder.Append(",").Append(this.logonFailures + this.successfulLogons);
			stringBuilder.Append(",").Append(this.logonFailures);
			stringBuilder.Append(",").Append(this.voiceMessagesCreated);
			stringBuilder.Append(",").Append(this.directoryAccessed);
			stringBuilder.Append(",").Append(this.directoryAccessedByExtension);
			stringBuilder.Append(",").Append(this.directoryAccessedByDialByName);
			stringBuilder.Append(",").Append(this.directoryAccessedSuccessfullyByDialByName);
			stringBuilder.Append(",").Append(this.directoryAccessedBySpeakingAName);
			stringBuilder.Append(",").Append(this.directoryAccessedSuccessfullyBySpeakingAName);
			switch (this.callContext.CallType)
			{
			case 2:
				stringBuilder.Append(",").Append(this.GetAutoAttendantId());
				stringBuilder.Append(",").Append(this.GetAutoAttendantName());
				stringBuilder.Append(",").Append(this.customMenuOptionTaken);
				stringBuilder.Append(",").Append(this.directoryAccessedBySpeakingAName);
				stringBuilder.Append(",").Append((this.directoryAccessedSuccessfullyBySpeakingAName > 0L) ? "T" : "F");
				break;
			case 3:
				stringBuilder.Append(",").Append(this.GetCallerUserId());
				stringBuilder.Append(",").Append(this.GetCallerAlias());
				stringBuilder.Append(",").Append(this.voiceMessagesHeard);
				stringBuilder.Append(",").Append(this.protectedVoiceMessagesHeard);
				stringBuilder.Append(",").Append(this.voiceMessagesDeleted);
				stringBuilder.Append(",").Append(this.emailMessagesHeard);
				stringBuilder.Append(",").Append(this.emailMessagesDeleted);
				stringBuilder.Append(",").Append(this.meetingsAccepted);
				stringBuilder.Append(",").Append(this.meetingsDeclinedOrCanceled);
				stringBuilder.Append(",").Append(this.repliedToMeetingOrganizer);
				stringBuilder.Append(",").Append(this.contactsAccessed);
				break;
			case 4:
				stringBuilder.Append(",").Append(this.GetCalleeUserId());
				stringBuilder.Append(",").Append(this.GetCalleeAlias());
				break;
			case 5:
				stringBuilder.Append(",").Append(this.GetCallerUserId());
				stringBuilder.Append(",").Append(this.GetCallerAlias());
				stringBuilder.Append(",").Append("\"").Append(this.GetDialedString()).Append("\"");
				break;
			case 6:
				stringBuilder.Append(",").Append(this.GetCalleeUserId());
				stringBuilder.Append(",").Append(this.GetCalleeAlias());
				stringBuilder.Append(",").Append(this.totalNumberOfValidFaxCalls);
				stringBuilder.Append(",").Append(this.totalNumberOfSuccessfulValidFaxCalls);
				break;
			}
			stringBuilder.Append(",").Append(this.AudioMetrics.AudioCodec);
			stringBuilder.Append(",").Append(this.AudioMetrics.BurstDensity);
			stringBuilder.Append(",").Append(this.AudioMetrics.BurstDuration);
			stringBuilder.Append(",").Append(this.AudioMetrics.Jitter);
			stringBuilder.Append(",").Append(this.AudioMetrics.NMOS);
			stringBuilder.Append(",").Append(this.AudioMetrics.NMOSDegradation);
			stringBuilder.Append(",").Append(this.AudioMetrics.PacketLoss);
			stringBuilder.Append(",").Append(this.AudioMetrics.RoundTrip);
			stringBuilder.Append(",").Append(this.AudioMetrics.NMOSDegradationJitter);
			stringBuilder.Append(",").Append(this.AudioMetrics.NMOSDegradationPacketLoss);
			stringBuilder.AppendLine();
			this.ClearCountersData();
			return stringBuilder.ToString();
		}

		private CallContext callContext;

		private int callDuration;

		private long successfulLogons;

		private long logonFailures;

		private long voiceMessagesCreated;

		private long directoryAccessed;

		private long directoryAccessedByExtension;

		private long directoryAccessedByDialByName;

		private long directoryAccessedSuccessfullyByDialByName;

		private long directoryAccessedBySpeakingAName;

		private long directoryAccessedSuccessfullyBySpeakingAName;

		private long voiceMessagesHeard;

		private long protectedVoiceMessagesHeard;

		private long voiceMessagesDeleted;

		private long emailMessagesHeard;

		private long emailMessagesDeleted;

		private long totalNumberOfValidFaxCalls;

		private long totalNumberOfSuccessfulValidFaxCalls;

		private long meetingsAccepted;

		private long meetingsDeclinedOrCanceled;

		private long repliedToMeetingOrganizer;

		private long contactsAccessed;

		private long customMenuOptionTaken;
	}
}
