using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class AutomaticProtectionAgentFactory : RoutingAgentFactory
	{
		internal AgentInstanceController InstanceController
		{
			get
			{
				return AgentInstanceController.Instance;
			}
		}

		public AutomaticProtectionAgentFactory()
		{
			ApaAgentPerfCounters.TotalMessagesEncrypted.RawValue = 0L;
			ApaAgentPerfCounters.TotalMessagesFailed.RawValue = 0L;
			ApaAgentPerfCounters.TotalDeferrals.RawValue = 0L;
			ApaAgentPerfCounters.TotalMessagesReencrypted.RawValue = 0L;
			ApaAgentPerfCounters.TotalMessagesFailedToReencrypt.RawValue = 0L;
			ApaAgentPerfCounters.Percentile95FailedToEncrypt.RawValue = 0L;
			AgentInstanceController.Initialize();
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new AutomaticProtectionAgent(this);
		}

		internal void UpdatePercentileCounters(bool success)
		{
			if (success)
			{
				AutomaticProtectionAgentFactory.percentileCounter.AddValue(0L);
			}
			else
			{
				AutomaticProtectionAgentFactory.percentileCounter.AddValue(1L);
			}
			ApaAgentPerfCounters.Percentile95FailedToEncrypt.RawValue = AutomaticProtectionAgentFactory.percentileCounter.PercentileQuery(95.0);
		}

		internal static readonly ExEventLog Logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

		private static PercentileCounter percentileCounter = new PercentileCounter(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(1.0), 1L, 2L);
	}
}
