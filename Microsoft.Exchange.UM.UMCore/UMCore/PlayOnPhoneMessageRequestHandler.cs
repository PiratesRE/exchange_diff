using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayOnPhoneMessageRequestHandler : PlayOnPhoneHandler
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
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "Processing a PlayOnPhone request for voice messages.", new object[0]);
			return base.Execute(requestBase);
		}
	}
}
