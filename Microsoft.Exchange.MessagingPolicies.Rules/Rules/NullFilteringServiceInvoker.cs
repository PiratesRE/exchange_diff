using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class NullFilteringServiceInvoker : FilteringServiceInvoker
	{
		private NullFilteringServiceInvoker()
		{
		}

		internal static NullFilteringServiceInvoker Factory()
		{
			return new NullFilteringServiceInvoker();
		}

		public override FilteringServiceInvoker.BeginScanResult BeginScan(FilteringServiceInvokerRequest filteringServiceInvokerRequest, ITracer tracer, Dictionary<string, string> classificationsToLookFor, FilteringServiceInvoker.ScanCompleteCallback scanCompleteCallback)
		{
			return FilteringServiceInvoker.BeginScanResult.Failure;
		}
	}
}
