using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public sealed class InterceptorBootScannerAgentFactory : StorageAgentFactory, IDiagnosable
	{
		public string GetDiagnosticComponentName()
		{
			return "InterceptorBootScannerAgent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return InterceptorDiagnosticHelper.GetDiagnosticInfo(parameters, "InterceptorBootScannerAgent", this.filteredRuleCache);
		}

		internal override StorageAgent CreateAgent(SmtpServer server)
		{
			return new InterceptorBootScannerAgent(this.filteredRuleCache);
		}

		internal const string DiagnosticComponentName = "InterceptorBootScannerAgent";

		private readonly FilteredRuleCache filteredRuleCache = new FilteredRuleCache(InterceptorAgentEvent.OnLoadedMessage);
	}
}
