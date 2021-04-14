using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorStoreDriverDeliveryAgentFactory : StoreDriverDeliveryAgentFactory, IDiagnosable
	{
		public InterceptorStoreDriverDeliveryAgentFactory()
		{
			Archiver.CreateArchiver(InterceptorAgentSettings.ArchivePath);
		}

		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new InterceptorStoreDriverDeliveryAgent(this.filteredRuleCache);
		}

		public string GetDiagnosticComponentName()
		{
			return "InterceptorDeliveryAgent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return InterceptorDiagnosticHelper.GetDiagnosticInfo(parameters, "InterceptorDeliveryAgent", this.filteredRuleCache);
		}

		internal const string DiagnosticComponentName = "InterceptorDeliveryAgent";

		private readonly FilteredRuleCache filteredRuleCache = new FilteredRuleCache(InterceptorAgentEvent.OnInitMsg | InterceptorAgentEvent.OnPromotedMessage | InterceptorAgentEvent.OnCreatedMessage);
	}
}
