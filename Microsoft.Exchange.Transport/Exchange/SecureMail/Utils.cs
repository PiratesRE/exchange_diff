using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.SecureMail
{
	internal static class Utils
	{
		internal static SecureMailTransportPerfCountersInstance SecureMailPerfCounters
		{
			get
			{
				return Utils.secureMailPerfCounters;
			}
		}

		internal static void InitPerfCounters()
		{
			Utils.secureMailPerfCounters = SecureMailTransportPerfCounters.GetInstance("_Total");
		}

		internal const string DefaultPerfCountersInstance = "_Total";

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.SecureMailTracer.Category, TransportEventLog.GetEventSource());

		private static SecureMailTransportPerfCountersInstance secureMailPerfCounters;
	}
}
