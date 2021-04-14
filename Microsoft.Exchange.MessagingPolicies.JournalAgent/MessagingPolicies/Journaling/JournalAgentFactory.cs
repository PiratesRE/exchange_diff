using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	public sealed class JournalAgentFactory : RoutingAgentFactory
	{
		public JournalAgentFactory()
		{
			PerfCounters.UsersJournaled.RawValue = 0L;
			PerfCounters.UsersJournaledPerHour.RawValue = 0L;
			PerfCounters.ReportsGenerated.RawValue = 0L;
			PerfCounters.ReportsGeneratedPerHour.RawValue = 0L;
			PerfCounters.ProcessingTime.RawValue = 0L;
			PerfCounters.MessagesProcessed.RawValue = 0L;
			PerfCounters.ReportsGeneratedWithRMSProtectedMessage.RawValue = 0L;
			PerfCounters.MessagesDeferredWithinJournalAgent.RawValue = 0L;
			PerfCounters.MessagesDeferredWithinJournalAgentPerHour.RawValue = 0L;
			PerfCounters.JournalReportsThatCouldNotBeCreated.RawValue = 0L;
			PerfCounters.JournalReportsThatCouldNotBeCreatedPerHour.RawValue = 0L;
			PerfCounters.MessagesDeferredWithinJournalAgentLawfulIntercept.RawValue = 0L;
			PerfCounters.MessagesDeferredWithinJournalAgentLawfulInterceptPerHour.RawValue = 0L;
			Components.PerfCountersLoaderComponent.AddCounterToGetExchangeDiagnostics(typeof(PerfCounters), "JournalAgentCounters");
			this.perfCountersWrapper = new JournalPerfCountersWrapper(new Tuple<ExPerformanceCounter, ExPerformanceCounter>[]
			{
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.UsersJournaled, PerfCounters.UsersJournaledPerHour),
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.ReportsGenerated, PerfCounters.ReportsGeneratedPerHour),
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.MessagesDeferredWithinJournalAgent, PerfCounters.MessagesDeferredWithinJournalAgentPerHour),
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.JournalReportsThatCouldNotBeCreated, PerfCounters.JournalReportsThatCouldNotBeCreatedPerHour),
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.MessagesDeferredWithinJournalAgentLawfulIntercept, PerfCounters.MessagesDeferredWithinJournalAgentLawfulInterceptPerHour)
			});
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new JournalAgent(server, this.perfCountersWrapper);
		}

		internal static JournalingDistibutionGroupCache JournalingDistributionGroupCacheInstance = new JournalingDistibutionGroupCache();

		private JournalPerfCountersWrapper perfCountersWrapper;
	}
}
