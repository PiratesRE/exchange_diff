using System;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.SystemProbeDrop
{
	internal sealed class SystemProbeDropRoutingAgent : RoutingAgent
	{
		public SystemProbeDropRoutingAgent()
		{
			base.OnCategorizedMessage += this.OnCategorizedMessageHandler;
		}

		public void OnCategorizedMessageHandler(CategorizedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (SystemProbeDropHelper.IsAgentEnabled() && SystemProbeDropHelper.CheckMailItemHeaders(args.MailItem) && SystemProbeDropHelper.ShouldDropMessage(args.MailItem.MimeDocument.RootPart.Headers, "OnCategorizedMessage"))
			{
				source.Delete();
			}
		}
	}
}
