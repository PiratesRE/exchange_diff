using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class AmServerNameCacheLogEvent : AmServerNameCache
	{
		public AmServerNameCacheLogEvent()
		{
			base.Enable();
		}

		public override string GetFqdn(string shortNodeName, bool throwException)
		{
			string result;
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3081121085U);
				result = base.GetFqdn(shortNodeName, true);
			}
			catch (AmServerNameResolveFqdnException ex)
			{
				AmTrace.Error("FQDN resolution of the short name {0} failed. Error: {1}", new object[]
				{
					shortNodeName,
					ex.Message
				});
				ReplayEventLogConstants.Tuple_FqdnResolutionFailure.LogEvent(ExTraceGlobals.ActiveManagerTracer.GetHashCode().ToString(), new object[]
				{
					shortNodeName,
					ex.Message
				});
				result = AmServerNameCache.GetFqdnWithLocalDomainSuffix(shortNodeName);
			}
			return result;
		}
	}
}
