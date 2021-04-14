using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public sealed class PrelicenseAgentFactory : RoutingAgentFactory
	{
		public PrelicenseAgentFactory()
		{
			PrelicenseAgentPerfCounters.TotalMessagesPreLicensed.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalMessagesFailedToPreLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalDeferralsToPreLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalRecipientsPreLicensed.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalRecipientsFailedToPreLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.Percentile95FailedToLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalMessagesLicensed.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalMessagesFailedToLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalDeferralsToLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalExternalMessagesPreLicensed.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalExternalMessagesFailedToPreLicense.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalRecipientsPreLicensedForExternalMessages.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalRecipientsFailedToPreLicenseForExternalMessages.RawValue = 0L;
			PrelicenseAgentPerfCounters.TotalDeferralsToPreLicenseForExternalMessages.RawValue = 0L;
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
			return new PrelicenseAgent(this, server);
		}

		internal void UpdatePercentileCounters(bool success)
		{
			if (success)
			{
				PrelicenseAgentFactory.percentileCounter.AddValue(0L);
			}
			else
			{
				PrelicenseAgentFactory.percentileCounter.AddValue(1L);
			}
			PrelicenseAgentPerfCounters.Percentile95FailedToLicense.RawValue = PrelicenseAgentFactory.percentileCounter.PercentileQuery(95.0);
		}

		internal static readonly ExEventLog Logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

		private static PercentileCounter percentileCounter = new PercentileCounter(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(1.0), 1L, 2L);
	}
}
