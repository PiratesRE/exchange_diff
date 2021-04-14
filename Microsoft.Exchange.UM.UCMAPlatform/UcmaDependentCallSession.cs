using System;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Collaboration;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaDependentCallSession : UcmaCallSession
	{
		internal UcmaDependentCallSession(DependentSessionDetails details, ISessionSerializer serializer, ApplicationEndpoint localEndpoint, CallContext cc) : base(serializer, localEndpoint, cc)
		{
			base.DependentSessionDetails = details;
			this.OnOutboundCallRequestCompleted += details.OutBoundCallConnectedHandler;
			this.IgnoreBye = false;
		}

		protected override bool IsDependentSession
		{
			get
			{
				return true;
			}
		}

		internal bool IgnoreBye { get; set; }
	}
}
