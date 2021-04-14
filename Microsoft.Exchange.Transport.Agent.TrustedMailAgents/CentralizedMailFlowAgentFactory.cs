using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	public sealed class CentralizedMailFlowAgentFactory : RoutingAgentFactory
	{
		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			bool enabled = CentralizedMailFlowAgentFactory.isEdge && TrustedMailUtils.AcceptAnyRecipientOnPremises && TrustedMailUtils.TrustedMailAgentsEnabled;
			return new CentralizedMailFlowAgent(enabled);
		}

		private static bool isEdge = !(Components.Configuration.LocalServer.IsBridgehead | Components.Configuration.LocalServer.TransportServer.IsFrontendTransportServer);
	}
}
