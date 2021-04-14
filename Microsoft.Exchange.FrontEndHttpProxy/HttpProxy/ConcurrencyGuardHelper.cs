using System;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class ConcurrencyGuardHelper
	{
		public static void IncrementTargetBackendDagAndForest(ProxyRequestHandler request)
		{
			string bucketName;
			string text;
			string text2;
			if (ConcurrencyGuardHelper.TryGetBackendDagAndForest(request, out bucketName, out text, out text2))
			{
				ConcurrencyGuards.TargetBackend.Increment(bucketName, request.Logger);
				ConcurrencyGuards.TargetForest.Increment(text2, request.Logger);
				ConcurrencyGuardHelper.GetPerfCounter(text2).OutstandingProxyRequestsToForest.Increment();
				PerfCounters.HttpProxyCountersInstance.OutstandingProxyRequests.Increment();
			}
		}

		public static void DecrementTargetBackendDagAndForest(ProxyRequestHandler request)
		{
			string bucketName;
			string text;
			string text2;
			if (ConcurrencyGuardHelper.TryGetBackendDagAndForest(request, out bucketName, out text, out text2))
			{
				ConcurrencyGuards.TargetBackend.Decrement(bucketName, request.Logger);
				ConcurrencyGuards.TargetForest.Decrement(text2, request.Logger);
				ConcurrencyGuardHelper.GetPerfCounter(text2).OutstandingProxyRequestsToForest.Decrement();
				PerfCounters.HttpProxyCountersInstance.OutstandingProxyRequests.Decrement();
			}
		}

		private static bool TryGetBackendDagAndForest(ProxyRequestHandler request, out string targetFqdn, out string dag, out string forestFqdn)
		{
			dag = null;
			forestFqdn = null;
			targetFqdn = request.ServerRequest.Address.Host.ToLower();
			if (string.IsNullOrWhiteSpace(targetFqdn))
			{
				return false;
			}
			forestFqdn = Utilities.GetForestFqdnFromServerFqdn(targetFqdn);
			return !string.IsNullOrWhiteSpace(forestFqdn);
		}

		private static HttpProxyCountersInstance GetPerfCounter(string instanceName)
		{
			return HttpProxyCounters.GetInstance(HttpProxyGlobals.ProtocolType + "_" + instanceName);
		}
	}
}
