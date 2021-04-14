using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PlayOnPhoneHandler : RequestHandler
	{
		protected abstract CallType OutgoingCallType { get; }

		internal static void OnOutBoundCallRequestCompleted(BaseUMCallSession vo, OutboundCallDetailsEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "PlayOnPhoneHandler: OnOutBoundCallRequestCompleted(), outcome ={0}", new object[]
			{
				callSessionEventArgs.CallOutcome
			});
			if (callSessionEventArgs.CallOutcome == OutboundCallDetailsEventArgs.OutboundCallOutcome.Failure)
			{
				vo.HandleFailedOutboundCall(callSessionEventArgs);
				return;
			}
			vo.InitializeConnectedCall(callSessionEventArgs);
		}

		protected UMSipPeer GetSipPeer(UMDialPlan dialPlan)
		{
			int num;
			IMwiTarget mwiTarget = UMIPGatewayOutboundTargetPicker.Instance.PickNextServer(dialPlan, out num);
			if (mwiTarget == null)
			{
				UmGlobals.ExEvent.LogEvent(dialPlan.OrganizationId, UMEventLogConstants.Tuple_NoOutboundGatewaysForDialPlanWithId, dialPlan.Name, dialPlan.Id);
				throw new IPGatewayNotFoundException();
			}
			SipNotifyMwiTarget sipNotifyMwiTarget = (SipNotifyMwiTarget)mwiTarget;
			return sipNotifyMwiTarget.Peer;
		}

		protected override ResponseBase Execute(RequestBase requestBase)
		{
			UMSubscriber umsubscriber = null;
			CallContext callContext = null;
			ResponseBase result;
			try
			{
				PlayOnPhoneUserRequest playOnPhoneUserRequest = requestBase as PlayOnPhoneUserRequest;
				umsubscriber = new UMSubscriber(playOnPhoneUserRequest.CallerInfo);
				if (string.IsNullOrEmpty(playOnPhoneUserRequest.DialString) || !umsubscriber.IsPlayOnPhoneEnabled || string.IsNullOrEmpty(umsubscriber.Extension))
				{
					PIIMessage[] data = new PIIMessage[]
					{
						PIIMessage.Create(PIIType._User, umsubscriber.ADRecipient.Name),
						PIIMessage.Create(PIIType._PhoneNumber, umsubscriber.Extension),
						PIIMessage.Create(PIIType._PII, playOnPhoneUserRequest.DialString ?? "<null>")
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "PlayOnPhoneHandler: DialingRulesException - UmUser = '_User', IsPlayOnPhoneEnabled = '{0}', Extension = '_PhoneNumber', request.DialString = '_PII'", new object[]
					{
						umsubscriber.IsPlayOnPhoneEnabled
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UserNotEnabledForPlayOnPhone, null, new object[]
					{
						umsubscriber.ADRecipient.Name,
						playOnPhoneUserRequest.DialString
					});
					throw new DialingRulesException();
				}
				string mailAddress = umsubscriber.MailAddress;
				int phoneCallCount = UmServiceGlobals.VoipPlatform.UsersPhoneCalls.GetPhoneCallCount(mailAddress);
				if (phoneCallCount >= 2)
				{
					PIIMessage data2 = PIIMessage.Create(PIIType._SmtpAddress, mailAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data2, "PlayOnPhoneHandler: smtpAddress = '_SmtpAddress', CurrentNumberOfCalls = '{0}'", new object[]
					{
						phoneCallCount.ToString(CultureInfo.InvariantCulture)
					});
					throw new OverPlayOnPhoneCallLimitException();
				}
				UMSipPeer sipPeer = this.GetSipPeer(umsubscriber.DialPlan);
				callContext = new CallContext();
				callContext.WebServiceRequest = playOnPhoneUserRequest;
				callContext.CallType = this.OutgoingCallType;
				callContext.CallerInfo = umsubscriber;
				callContext.CallerInfo.IsAuthenticated = true;
				callContext.DialPlan = umsubscriber.DialPlan;
				callContext.GatewayConfig = sipPeer.ToUMIPGateway(umsubscriber.DialPlan.OrganizationId);
				BaseUMCallSession baseUMCallSession = UmServiceGlobals.VoipPlatform.MakeCallForUser(umsubscriber, playOnPhoneUserRequest.DialString, sipPeer, callContext, new UMCallSessionHandler<OutboundCallDetailsEventArgs>(PlayOnPhoneHandler.OnOutBoundCallRequestCompleted));
				PlayOnPhoneResponse playOnPhoneResponse = new PlayOnPhoneResponse();
				playOnPhoneResponse.CallId = baseUMCallSession.SessionGuid.ToString();
				UmServiceGlobals.VoipPlatform.UsersPhoneCalls.AddPhoneCall(mailAddress, baseUMCallSession.SessionGuid);
				result = playOnPhoneResponse;
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "PlayOnPhoneHandler: Got exception {0}. Setting CallType=None", new object[]
				{
					ex
				});
				if (callContext != null)
				{
					callContext.CallType = 0;
				}
				if (umsubscriber != null)
				{
					umsubscriber.Dispose();
				}
				throw;
			}
			return result;
		}
	}
}
