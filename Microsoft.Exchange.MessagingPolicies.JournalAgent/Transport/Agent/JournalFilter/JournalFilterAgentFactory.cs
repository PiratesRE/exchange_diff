using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MessagingPolicies.Journaling;

namespace Microsoft.Exchange.Transport.Agent.JournalFilter
{
	public sealed class JournalFilterAgentFactory : SmtpReceiveAgentFactory
	{
		public JournalFilterAgentFactory()
		{
			PerfCounters.IncomingJournalReportsDropped.RawValue = 0L;
			PerfCounters.IncomingJournalReportsDroppedPerHour.RawValue = 0L;
			this.perfCountersWrapper = new JournalPerfCountersWrapper(new Tuple<ExPerformanceCounter, ExPerformanceCounter>[]
			{
				new Tuple<ExPerformanceCounter, ExPerformanceCounter>(PerfCounters.IncomingJournalReportsDropped, PerfCounters.IncomingJournalReportsDroppedPerHour)
			});
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new JournalFilterAgent(this.perfCountersWrapper);
		}

		private JournalPerfCountersWrapper perfCountersWrapper;
	}
}
