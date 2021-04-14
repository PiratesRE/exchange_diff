using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class StandardSession : LocalSession
	{
		private StandardSession(RbacContext context) : base(context, StandardSession.sessionPerfCounters, StandardSession.esoSessionPerfCounters)
		{
		}

		protected override void WriteInitializationLog()
		{
			EcpEventLogConstants.Tuple_StandardSessionInitialize.LogEvent(new object[]
			{
				base.NameForEventLog,
				EcpEventLogExtensions.GetFlightInfoForLog()
			});
		}

		private static PerfCounterGroup sessionsCounters = new PerfCounterGroup(EcpPerfCounters.StandardSessions, EcpPerfCounters.StandardSessionsPeak, EcpPerfCounters.StandardSessionsTotal);

		private static PerfCounterGroup requestsCounters = new PerfCounterGroup(EcpPerfCounters.StandardRequests, EcpPerfCounters.StandardRequestsPeak, EcpPerfCounters.StandardRequestsTotal);

		private static PerfCounterGroup esoSessionsCounters = new PerfCounterGroup(EcpPerfCounters.EsoStandardSessions, EcpPerfCounters.EsoStandardSessionsPeak, EcpPerfCounters.EsoStandardSessionsTotal);

		private static PerfCounterGroup esoRequestsCounters = new PerfCounterGroup(EcpPerfCounters.EsoStandardRequests, EcpPerfCounters.EsoStandardRequestsPeak, EcpPerfCounters.EsoStandardRequestsTotal);

		private static SessionPerformanceCounters sessionPerfCounters = new SessionPerformanceCounters(StandardSession.sessionsCounters, StandardSession.requestsCounters);

		private static EsoSessionPerformanceCounters esoSessionPerfCounters = new EsoSessionPerformanceCounters(StandardSession.sessionsCounters, StandardSession.requestsCounters, StandardSession.esoSessionsCounters, StandardSession.esoRequestsCounters);

		public new sealed class Factory : RbacSession.Factory
		{
			public Factory(RbacContext context) : base(context)
			{
			}

			protected override bool CanCreateSession()
			{
				return true;
			}

			protected override RbacSession CreateNewSession()
			{
				return new StandardSession(base.Context);
			}
		}
	}
}
