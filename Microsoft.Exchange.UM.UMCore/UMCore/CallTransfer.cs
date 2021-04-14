using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallTransfer : CommonActivity
	{
		internal CallTransfer(ActivityManager manager, ActivityConfig config) : base(manager, config)
		{
		}

		internal override void StartActivity(BaseUMCallSession callSession, string refInfo)
		{
			CallTransferConfig callTransferConfig = base.Config as CallTransferConfig;
			this.InitializeMessageStrings();
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FsmActivityStart, null, new object[]
			{
				this.ToString(),
				callSession.CallId
			});
			PhoneNumber phoneNumber = null;
			if (string.Compare(callTransferConfig.PhoneNumberType, "variable", StringComparison.OrdinalIgnoreCase) == 0)
			{
				phoneNumber = callTransferConfig.GetPhoneNumberVariable(base.Manager);
			}
			else if (string.Compare(callTransferConfig.PhoneNumberType, "literal", StringComparison.OrdinalIgnoreCase) == 0)
			{
				PhoneNumber.TryParse(callTransferConfig.PhoneNumber, out phoneNumber);
			}
			if (PhoneNumber.IsNullOrEmpty(phoneNumber))
			{
				throw new ArgumentNullException("phone");
			}
			this.transferPhoneNumber = phoneNumber;
			if (phoneNumber.UriType == UMUriType.SipName && !Util.IsSipUriValid(phoneNumber.ToDial))
			{
				throw new InvalidSipUriException();
			}
			this.transferContactInfo = (base.Manager.ReadVariable("targetContactInfo") as ContactInfo);
			if (callSession.CurrentCallContext.DialPlan.URIType == UMUriType.E164 && phoneNumber.UriType == UMUriType.TelExtn)
			{
				PhoneNumber phoneNumber2 = PhoneNumber.Parse(Util.FormatE164Number(phoneNumber.Number));
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._PhoneNumber, phoneNumber.ToDial),
					PIIMessage.Create(PIIType._PhoneNumber, phoneNumber2.ToDial)
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "CallTransfer::Start() converting TelExtn number _PhoneNumber1 to E164 number _PhoneNumber2", new object[0]);
				this.transferPhoneNumber = phoneNumber2;
			}
			PIIMessage data2 = PIIMessage.Create(PIIType._PhoneNumber, this.transferPhoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data2, "Call transfer to: _PhoneNumber", new object[0]);
			this.CallTransferStart(callSession);
		}

		internal override void OnTransferComplete(BaseUMCallSession callSession, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CallTransfer::OnTransferComplete called.", new object[0]);
			ActivityConfig config = base.Config;
			CallType callType = callSession.CurrentCallContext.CallType;
			if (callType != 3)
			{
				if (callType == 6)
				{
					CallContext.UpdateCountersAndPercentages(true, FaxCounters.TotalNumberOfSuccessfulValidFaxCalls, FaxCounters.TotalNumberOfValidFaxCalls, FaxCounters.PercentageSuccessfulValidFaxCalls, FaxCounters.PercentageSuccessfulValidFaxCalls_Base);
				}
			}
			else
			{
				callSession.IncrementCounter(SubscriberAccessCounters.LaunchedCalls);
			}
			base.Manager.DropCall(callSession, DropCallReason.GracefulHangup);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "CallTransfer activity finishing.", new object[0]);
		}

		internal void HandleFailedFaxTransfer(BaseUMCallSession callSession)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Handle Failed Fax Transfer", new object[0]);
			CallContext.UpdateCountersAndPercentages(false, FaxCounters.TotalNumberOfSuccessfulValidFaxCalls, FaxCounters.TotalNumberOfValidFaxCalls, FaxCounters.PercentageSuccessfulValidFaxCalls, FaxCounters.PercentageSuccessfulValidFaxCalls_Base);
			UMSubscriber umsubscriber = callSession.CurrentCallContext.CalleeInfo as UMSubscriber;
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FaxTransferFailure, null, new object[]
			{
				umsubscriber.UMMailboxPolicy.FaxServerURI
			});
			UMMessageSubmission.SubmitMissedCall(callSession.CallId, callSession.CurrentCallContext.CallerId, callSession.CurrentCallContext.CallerInfo, umsubscriber, false, null, callSession.CurrentCallContext.CallerIdDisplayName, callSession.CurrentCallContext.TenantGuid);
		}

		protected override string GetExceptionCategory(UMCallSessionEventArgs callSessionEventArgs)
		{
			if (callSessionEventArgs.Error != null)
			{
				ActivityConfig config = base.Config;
				if (!base.Manager.CallSession.IsClosing && base.Manager.CallSession.State != UMCallState.Disconnected)
				{
					PlatformSipUri obj = (PlatformSipUri)base.Manager.ReadVariable("ReferredByUri");
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallTransferFailed, null, new object[]
					{
						this.transferType,
						this.transferTarget,
						this.transferPhoneNumber.ToDial,
						base.Manager.CallSession.CallId,
						CommonUtil.ToEventLogString(obj)
					});
				}
				return "transferFailed";
			}
			return base.GetExceptionCategory(callSessionEventArgs);
		}

		private void CallTransferStart(BaseUMCallSession callSession)
		{
			CallTransferConfig callTransferConfig = base.Config as CallTransferConfig;
			PlatformSipUri platformSipUri = (PlatformSipUri)base.Manager.ReadVariable("ReferredByUri");
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallTransfer, null, new object[]
			{
				this.transferType,
				this.transferTarget,
				this.transferPhoneNumber.ToDial,
				base.Manager.CallSession.CallId,
				CommonUtil.ToEventLogString(platformSipUri)
			});
			switch (callTransferConfig.TransferType)
			{
			case CallTransferType.BlindTransferToPhone:
				callSession.BlindTransferToPhone(this.transferPhoneNumber, this.transferContactInfo);
				return;
			case CallTransferType.Supervised:
				callSession.SupervisedTransfer();
				return;
			case CallTransferType.BlindTransferToHost:
				if (platformSipUri == null)
				{
					throw new ArgumentNullException("ReferredByUri");
				}
				callSession.BlindTransferToHost(this.transferPhoneNumber, platformSipUri);
				return;
			default:
				throw new ArgumentException(callTransferConfig.TransferType.ToString());
			}
		}

		private void InitializeMessageStrings()
		{
			this.transferType = null;
			this.transferTarget = null;
			CallTransferConfig callTransferConfig = base.Config as CallTransferConfig;
			switch (callTransferConfig.TransferType)
			{
			case CallTransferType.BlindTransferToPhone:
				this.transferType = Strings.Blind;
				this.transferTarget = Strings.TransferTargetPhone;
				return;
			case CallTransferType.Supervised:
				this.transferType = Strings.Supervised;
				this.transferTarget = Strings.TransferTargetPhone;
				return;
			case CallTransferType.BlindTransferToHost:
				this.transferType = Strings.Blind;
				this.transferTarget = Strings.TransferTargetHost;
				return;
			default:
				throw new ArgumentException(callTransferConfig.TransferType.ToString());
			}
		}

		private PhoneNumber transferPhoneNumber;

		private ContactInfo transferContactInfo;

		private string transferType;

		private string transferTarget;
	}
}
