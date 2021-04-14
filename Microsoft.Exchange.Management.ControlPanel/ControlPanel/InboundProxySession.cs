using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class InboundProxySession : LocalSession
	{
		private InboundProxySession(RbacContext context) : base(context, InboundProxySession.sessionPerfCounters, InboundProxySession.esoSessionPerfCounters)
		{
			this.originalCallerName = context.Settings.InboundProxyCallerName;
		}

		protected override void WriteInitializationLog()
		{
			ExTraceGlobals.RBACTracer.TraceInformation<InboundProxySession, string>(0, 0L, "{0} created to handle calls from {1}.", this, this.originalCallerName);
			EcpEventLogConstants.Tuple_InboundProxySessionInitialize.LogEvent(new object[]
			{
				base.NameForEventLog,
				this.originalCallerName
			});
		}

		private static PerfCounterGroup sessionsCounters = new PerfCounterGroup(EcpPerfCounters.InboundProxySessions, EcpPerfCounters.InboundProxySessionsPeak, EcpPerfCounters.InboundProxySessionsTotal);

		private static PerfCounterGroup requestsCounters = new PerfCounterGroup(EcpPerfCounters.InboundProxyRequests, EcpPerfCounters.InboundProxyRequestsPeak, EcpPerfCounters.InboundProxyRequestsTotal);

		private static PerfCounterGroup esoSessionsCounters = new PerfCounterGroup(EcpPerfCounters.EsoInboundProxySessions, EcpPerfCounters.EsoInboundProxySessionsPeak, EcpPerfCounters.EsoInboundProxySessionsTotal);

		private static PerfCounterGroup esoRequestsCounters = new PerfCounterGroup(EcpPerfCounters.EsoInboundProxyRequests, EcpPerfCounters.EsoInboundProxyRequestsPeak, EcpPerfCounters.EsoInboundProxyRequestsTotal);

		private static SessionPerformanceCounters sessionPerfCounters = new SessionPerformanceCounters(InboundProxySession.sessionsCounters, InboundProxySession.requestsCounters);

		private static EsoSessionPerformanceCounters esoSessionPerfCounters = new EsoSessionPerformanceCounters(InboundProxySession.sessionsCounters, InboundProxySession.requestsCounters, InboundProxySession.esoSessionsCounters, InboundProxySession.esoRequestsCounters);

		private string originalCallerName;

		public sealed class ProxyLogonNeededFactory : RbacSession.Factory
		{
			public ProxyLogonNeededFactory(RbacContext context) : base(context)
			{
			}

			protected override bool CanCreateSession()
			{
				return base.Settings.IsInboundProxyRequest && !base.Settings.IsProxyLogon;
			}

			protected override RbacSession CreateNewSession()
			{
				Util.EndResponse(HttpContext.Current.Response, (HttpStatusCode)441);
				throw new InvalidOperationException();
			}
		}

		public new sealed class Factory : RbacSession.Factory
		{
			public Factory(RbacContext context) : base(context)
			{
			}

			protected override bool CanCreateSession()
			{
				return base.Settings.IsProxyLogon;
			}

			protected override RbacSession CreateNewSession()
			{
				return new InboundProxySession(base.Context);
			}
		}
	}
}
