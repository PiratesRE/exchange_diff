using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal static class SystemProbeHelper
	{
		public static SystemProbeTrace MessageResubmissionTracer
		{
			get
			{
				return SystemProbeHelper.messageResubmissionTracer;
			}
		}

		public static SystemProbeTrace ShadowRedundancyTracer
		{
			get
			{
				return SystemProbeHelper.shadowRedundancyTracer;
			}
		}

		public static SystemProbeTrace SmtpReceiveTracer
		{
			get
			{
				return SystemProbeHelper.smtpReceiveTracer;
			}
		}

		public static SystemProbeTrace SmtpSendTracer
		{
			get
			{
				return SystemProbeHelper.smtpSendTracer;
			}
		}

		public static SystemProbeTrace EtrTracer
		{
			get
			{
				return SystemProbeHelper.etrTracer;
			}
		}

		public static SystemProbeTrace SchedulerTracer
		{
			get
			{
				return SystemProbeHelper.schedulerTracer;
			}
		}

		private static SystemProbeTrace messageResubmissionTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.MessageResubmissionTracer, "MessageResubmission");

		private static SystemProbeTrace shadowRedundancyTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.ShadowRedundancyTracer, "ShadowRedundancy");

		private static SystemProbeTrace smtpReceiveTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpReceiveTracer, "SmtpReceive");

		private static SystemProbeTrace smtpSendTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer, "SmtpSend");

		private static SystemProbeTrace etrTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer, "exchangeTransportRules");

		private static SystemProbeTrace schedulerTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SchedulerTracer, "categorizer");
	}
}
