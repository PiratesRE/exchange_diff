using System;
using System.Web;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MessageTrackingRequestHandler : EwsProxyRequestHandler
	{
		internal static bool IsMessageTrackingRequest(HttpRequest request)
		{
			return request.UserAgent != null && request.UserAgent.StartsWith(MessageTrackingRequestHandler.MessageTrackingUserAgentString);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			return new LocalSiteAnchorMailbox(this);
		}

		private static readonly string MessageTrackingUserAgentString = WellKnownUserAgent.GetEwsNegoAuthUserAgent("Microsoft.Exchange.InfoWorker.Common.MessageTracking");
	}
}
