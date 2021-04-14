using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MwiDiagnostics
	{
		internal static MwiLoadBalancerPerformanceCountersInstance GetInstance(string instanceName)
		{
			MwiLoadBalancerPerformanceCountersInstance result = null;
			try
			{
				result = MwiLoadBalancerPerformanceCounters.GetInstance(instanceName);
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.MWITracer, 0, "MwiDiagnostics: Unable to get instance {0}: {1}", new object[]
				{
					instanceName,
					ex
				});
			}
			return result;
		}

		internal static void IncrementCounter(ExPerformanceCounter perfCounter)
		{
			if (perfCounter != null)
			{
				perfCounter.Increment();
			}
		}

		internal static void SetCounterValue(ExPerformanceCounter perfCounter, long counterValue)
		{
			if (perfCounter != null)
			{
				perfCounter.RawValue = counterValue;
			}
		}
	}
}
