using System;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaCallRouterApplicationEndpoint : ApplicationEndpoint
	{
		public event EventHandler<SessionReceivedEventArgs> LegacyLyncNotificationCallReceived;

		public UcmaCallRouterApplicationEndpoint(CollaborationPlatform platform, ApplicationEndpointSettings settings) : base(platform, settings)
		{
		}

		protected override bool HandleSignalingSession(SessionReceivedEventArgs args)
		{
			bool result = false;
			if (UcmaCallRouterApplicationEndpoint.IsUserEventNotificationCall(args) && this.LegacyLyncNotificationCallReceived != null)
			{
				this.LegacyLyncNotificationCallReceived(this, args);
				result = true;
			}
			return result;
		}

		private static bool IsUserEventNotificationCall(SessionReceivedEventArgs args)
		{
			return args.RequestUri.IndexOf("opaque=app:rtcevent", StringComparison.OrdinalIgnoreCase) > 0;
		}
	}
}
