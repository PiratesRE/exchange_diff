using System;
using System.Collections.Generic;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class FilteringServiceInvoker
	{
		public string ErrorDescription { get; protected set; }

		public abstract FilteringServiceInvoker.BeginScanResult BeginScan(FilteringServiceInvokerRequest filteringServiceInvokerRequest, ITracer tracer, Dictionary<string, string> classificationsToLookFor, FilteringServiceInvoker.ScanCompleteCallback scanCompleteCallback);

		public enum ScanResult
		{
			Success,
			Failure,
			CrashFailure,
			Timeout,
			QueueTimeout
		}

		public enum BeginScanResult
		{
			Queued,
			Failure
		}

		public delegate void ScanCompleteCallback(FilteringServiceInvoker.ScanResult scanResult, IEnumerable<DiscoveredDataClassification> classifications, FilteringResults filteringResults, Exception exception);
	}
}
