using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public static class Util
	{
		internal static ExEventLog EventLog
		{
			get
			{
				return Util.eventLogger;
			}
		}

		internal static InterceptorAgentPerfCountersInstance PerfCountersTotalInstance
		{
			get
			{
				return InterceptorAgentPerfCounters.GetInstance(Util.TotalInstanceName);
			}
		}

		internal static InterceptorAgentPerfCountersInstance GetPerfCountersInstance(string instanceName, out bool existed)
		{
			string instanceName2 = Util.PerfCounterPrefix + instanceName;
			existed = InterceptorAgentPerfCounters.InstanceExists(instanceName2);
			return InterceptorAgentPerfCounters.GetInstance(instanceName2);
		}

		public static readonly string PerfCounterPrefix = Process.GetCurrentProcess().ProcessName + "_";

		public static readonly string TotalInstanceName = Util.PerfCounterPrefix + "Total";

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.InterceptorAgentTracer.Category, TransportEventLog.GetEventSource());
	}
}
