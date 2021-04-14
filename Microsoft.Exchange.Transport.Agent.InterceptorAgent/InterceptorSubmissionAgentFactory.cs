using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorSubmissionAgentFactory : StoreDriverAgentFactory, IDiagnosable
	{
		public InterceptorSubmissionAgentFactory()
		{
			Archiver.CreateArchiver(InterceptorAgentSettings.ArchivePath);
		}

		public override StoreDriverAgent CreateAgent(SmtpServer server)
		{
			return new InterceptorSubmissionAgent(this.filteredRuleCache);
		}

		public string GetDiagnosticComponentName()
		{
			return "InterceptorSubmissionAgent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return InterceptorDiagnosticHelper.GetDiagnosticInfo(parameters, "InterceptorSubmissionAgent", this.filteredRuleCache);
		}

		internal const string DiagnosticComponentName = "InterceptorSubmissionAgent";

		private readonly FilteredRuleCache filteredRuleCache = new FilteredRuleCache(InterceptorAgentEvent.OnDemotedMessage);
	}
}
