using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GetCallInfoRequestHandler : RequestHandler
	{
		protected override ResponseBase Execute(RequestBase requestBase)
		{
			GetCallInfoRequest getCallInfoRequest = requestBase as GetCallInfoRequest;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Processing a GetCallInfo request.", new object[0]);
			Guid sessionGuid;
			try
			{
				sessionGuid = new Guid(getCallInfoRequest.CallId);
			}
			catch (FormatException innerException)
			{
				throw new InvalidCallIdException(innerException);
			}
			UMCallInfoEx callInfo = UmServiceGlobals.VoipPlatform.GetCallInfo(sessionGuid);
			if (callInfo != null)
			{
				if ((callInfo.CallState != UMCallState.Connected && callInfo.CallState != UMCallState.Disconnected) || (callInfo.CallState == UMCallState.Disconnected && callInfo.EventCause == UMEventCause.Unavailable))
				{
					callInfo.CallState = UMCallState.Connecting;
					callInfo.EventCause = UMEventCause.None;
					callInfo.EndResult = UMOperationResult.InProgress;
				}
				return new GetCallInfoResponse
				{
					CallInfo = callInfo
				};
			}
			throw new InvalidCallIdException();
		}
	}
}
