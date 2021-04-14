using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaCobrandingRedirProxyRequestHandler : ProxyRequestHandler
	{
		internal static bool IsCobrandingRedirRequest(HttpRequest request)
		{
			return request.Url.LocalPath.EndsWith("cobrandingredir.aspx", StringComparison.OrdinalIgnoreCase);
		}

		protected override AnchoredRoutingTarget TryDirectTargetCalculation()
		{
			BackEndServer randomDownLevelClientAccessServer = DownLevelServerManager.Instance.GetRandomDownLevelClientAccessServer();
			return new AnchoredRoutingTarget(new AnonymousAnchorMailbox(this), randomDownLevelClientAccessServer);
		}
	}
}
