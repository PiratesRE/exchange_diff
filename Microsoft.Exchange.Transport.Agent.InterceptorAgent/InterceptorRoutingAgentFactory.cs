using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public sealed class InterceptorRoutingAgentFactory : RoutingAgentFactory, IDiagnosable
	{
		public InterceptorRoutingAgentFactory()
		{
			Archiver.CreateArchiver(InterceptorAgentSettings.ArchivePath);
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new InterceptorRoutingAgent(this.filteredRuleCache);
		}

		public string GetDiagnosticComponentName()
		{
			return "InterceptorRoutingAgent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return InterceptorDiagnosticHelper.GetDiagnosticInfo(parameters, "InterceptorRoutingAgent", this.filteredRuleCache);
		}

		internal const string DiagnosticComponentName = "InterceptorRoutingAgent";

		private readonly FilteredRuleCache filteredRuleCache = new FilteredRuleCache(InterceptorAgentEvent.OnSubmittedMessage | InterceptorAgentEvent.OnResolvedMessage | InterceptorAgentEvent.OnRoutedMessage | InterceptorAgentEvent.OnCategorizedMessage);
	}
}
