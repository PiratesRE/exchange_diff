using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	internal class ExMonitoringRequestTracker
	{
		internal static ExMonitoringRequestTracker Instance
		{
			get
			{
				return ExMonitoringRequestTracker.instance;
			}
		}

		internal string MonitoringInstanceId
		{
			get
			{
				return this.monitoringInstanceId;
			}
		}

		private ExMonitoringRequestTracker()
		{
			using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				byte[] array = new byte[16];
				rngcryptoServiceProvider.GetBytes(array);
				this.monitoringInstanceId = new Guid(array).ToString();
			}
		}

		internal void ReportMonitoringRequest(HttpRequest request)
		{
			string text = request.Headers["X-MonitoringInstance"];
			if (!string.IsNullOrEmpty(text))
			{
				if (this.knownMonitoringIds.Count > 1000)
				{
					ExTraceGlobals.WebHealthTracer.TraceDebug<int>((long)this.GetHashCode(), "ExMonitoringRequestTracker::ReportMonitoringRequest() - Resetting list of known monitoring IDs since there were more than {0} elements", 1000);
					this.knownMonitoringIds.Clear();
				}
				this.knownMonitoringIds.GetOrAdd(text, true);
			}
		}

		internal bool IsKnownMonitoringRequest(HttpRequest request)
		{
			string text = request.Headers["X-MonitoringInstance"];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.WebHealthTracer.TraceDebug((long)this.GetHashCode(), "ExMonitoringRequestTracker::IsKnownMonitoringRequest() - No monitoring ID header present");
				return false;
			}
			if (this.knownMonitoringIds.ContainsKey(text))
			{
				ExTraceGlobals.WebHealthTracer.TraceDebug<string>((long)this.GetHashCode(), "ExMonitoringRequestTracker::IsKnownMonitoringRequest() - Request contained a valid ID: {0}", text);
				return true;
			}
			ExTraceGlobals.WebHealthTracer.TraceDebug<string>((long)this.GetHashCode(), "ExMonitoringRequestTracker::IsKnownMonitoringRequest() - Request contained a unknown ID: {0}", text);
			return false;
		}

		private const int MaxNumberOfKnownMonitoringClients = 1000;

		internal const string MonitoringInstanceIdHeaderName = "X-MonitoringInstance";

		private static ExMonitoringRequestTracker instance = new ExMonitoringRequestTracker();

		private readonly string monitoringInstanceId;

		private readonly ConcurrentDictionary<string, bool> knownMonitoringIds = new ConcurrentDictionary<string, bool>();
	}
}
