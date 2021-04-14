using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class IndexDeliveryAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public IndexDeliveryAgentFactory()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("IndexDeliveryAgentFactory", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexDeliveryAgentTracer, (long)this.GetHashCode());
			this.enabled = new SearchConfig().IndexAgentEnabled;
		}

		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			if (this.enabled)
			{
				return new IndexDeliveryAgent();
			}
			return new IndexDeliveryAgentFactory.NoopDeliveryAgent();
		}

		public override void Close()
		{
			this.diagnosticsSession.TraceDebug("Factory closed", new object[0]);
		}

		private const string ComponentName = "IndexDeliveryAgentFactory";

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly bool enabled;

		private class NoopDeliveryAgent : StoreDriverDeliveryAgent
		{
		}
	}
}
