using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayOnPhoneAAGreetingRequestHandler : PlayOnPhoneHandler
	{
		protected override CallType OutgoingCallType
		{
			get
			{
				return 5;
			}
		}

		protected override ResponseBase Execute(RequestBase requestBase)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Processing a PlayOnPhone request for AA", new object[0]);
			PlayOnPhoneAAGreetingRequest playOnPhoneAAGreetingRequest = (PlayOnPhoneAAGreetingRequest)requestBase;
			if (playOnPhoneAAGreetingRequest.DialPlan == null)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "PlayOnPhoneAAGreetingRequestHandler::Execute() Could not find dialplan with id {0}", new object[]
				{
					playOnPhoneAAGreetingRequest.AutoAttendant.UMDialPlan
				});
				throw new IPGatewayNotFoundException();
			}
			UMAutoAttendant autoAttendant = playOnPhoneAAGreetingRequest.AutoAttendant;
			if (string.IsNullOrEmpty(playOnPhoneAAGreetingRequest.DialString))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "PlayOnPhoneAAGreetingRequestHandler: Execute= '{0}', CallSomeoneEnabled = '{1}', request.DialString = '{2}'", new object[]
				{
					autoAttendant.Name,
					autoAttendant.CallSomeoneEnabled,
					playOnPhoneAAGreetingRequest.DialString ?? "<null>"
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AAOutDialingFailure, null, new object[]
				{
					autoAttendant.Name,
					playOnPhoneAAGreetingRequest.DialString
				});
				throw new DialingRulesException();
			}
			this.ValidateFileName(playOnPhoneAAGreetingRequest.FileName);
			UMSipPeer sipPeer = base.GetSipPeer(playOnPhoneAAGreetingRequest.DialPlan);
			CallContext callContext = new CallContext();
			callContext.WebServiceRequest = playOnPhoneAAGreetingRequest;
			callContext.CallerInfo = null;
			callContext.DialPlan = playOnPhoneAAGreetingRequest.DialPlan;
			callContext.SetCurrentAutoAttendant(autoAttendant);
			callContext.CallType = this.OutgoingCallType;
			callContext.GatewayConfig = sipPeer.ToUMIPGateway(playOnPhoneAAGreetingRequest.DialPlan.OrganizationId);
			ResponseBase result;
			try
			{
				BaseUMCallSession baseUMCallSession = UmServiceGlobals.VoipPlatform.MakeCallForAA(playOnPhoneAAGreetingRequest.AutoAttendant, playOnPhoneAAGreetingRequest.DialString, sipPeer, callContext, new UMCallSessionHandler<OutboundCallDetailsEventArgs>(PlayOnPhoneHandler.OnOutBoundCallRequestCompleted));
				result = new PlayOnPhoneResponse
				{
					CallId = baseUMCallSession.SessionGuid.ToString()
				};
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "PlayOnPhoneAAGreetingRequestHandler: Got exception {0}. Setting CallType=None", new object[]
				{
					ex
				});
				callContext.CallType = 0;
				throw;
			}
			return result;
		}

		private void ValidateFileName(string filename)
		{
			string text = Utils.TrimSpaces(filename);
			if (text == null || text.Length > 128)
			{
				throw new InvalidFileNameException(128);
			}
			string extension;
			try
			{
				extension = Path.GetExtension(text);
			}
			catch (ArgumentException)
			{
				throw new InvalidFileNameException(128);
			}
			if (!string.Equals(extension, ".wav", StringComparison.OrdinalIgnoreCase) && !string.Equals(extension, ".wma", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidFileNameException(128);
			}
		}
	}
}
