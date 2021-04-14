using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class JournalReportDecryptionAgentFactory : RoutingAgentFactory
	{
		public JournalReportDecryptionAgentFactory()
		{
			JournalReportDecryptionAgentPerfCounters.TotalJRDecrypted.RawValue = 0L;
			JournalReportDecryptionAgentPerfCounters.TotalJRFailed.RawValue = 0L;
			JournalReportDecryptionAgentPerfCounters.TotalDeferrals.RawValue = 0L;
			AgentInstanceController.Initialize();
		}

		internal AgentInstanceController InstanceController
		{
			get
			{
				return AgentInstanceController.Instance;
			}
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new JournalReportDecryptionAgent(this, server);
		}

		internal static ExEventLog Logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");
	}
}
