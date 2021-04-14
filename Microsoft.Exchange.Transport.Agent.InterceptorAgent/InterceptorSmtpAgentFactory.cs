using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public sealed class InterceptorSmtpAgentFactory : SmtpReceiveAgentFactory, IDiagnosable
	{
		public InterceptorSmtpAgentFactory()
		{
			Archiver.CreateArchiver(InterceptorAgentSettings.ArchivePath);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new InterceptorSmtpAgent(this.filteredRuleCache);
		}

		public string GetDiagnosticComponentName()
		{
			return "InterceptorSmtpAgent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return InterceptorDiagnosticHelper.GetDiagnosticInfo(parameters, "InterceptorSmtpAgent", this.filteredRuleCache);
		}

		internal const string DiagnosticComponentName = "InterceptorSmtpAgent";

		private readonly FilteredRuleCache filteredRuleCache = new FilteredRuleCache(InterceptorAgentEvent.OnMailFrom | InterceptorAgentEvent.OnRcptTo | InterceptorAgentEvent.OnEndOfHeaders | InterceptorAgentEvent.OnEndOfData);
	}
}
