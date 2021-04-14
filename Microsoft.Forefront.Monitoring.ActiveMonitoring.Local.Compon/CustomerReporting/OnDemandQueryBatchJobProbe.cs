using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data.AsyncQueue;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.CustomerReporting
{
	public class OnDemandQueryBatchJobProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("OnDemandQueryBatchJobProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			DateTime utcNow = DateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			try
			{
				AsyncQueueSession asyncQueueSession = new AsyncQueueSession();
				List<AsyncQueueRequest> source = asyncQueueSession.GetAsyncQueueRequests("OnDemandQueryBatchRequest", null, null, new DateTime?(utcNow - OnDemandQueryBatchJobProbe.requestMonitoringRange), null, 500).ToList<AsyncQueueRequest>();
				IEnumerable<AsyncQueueRequest> source2 = from request in source
				where request.RequestStatus == AsyncQueueStatus.Failed
				select request;
				if (source2.Any<AsyncQueueRequest>())
				{
					stringBuilder.Append("Failed OnDemandQueryBatchRequest found.");
					stringBuilder2.AppendLine(string.Format("{0} {1}, first 10 of the failed requests:", "Failed OnDemandQueryBatchRequest found.", source2.Count<AsyncQueueRequest>()));
					foreach (AsyncQueueRequest request3 in source2.Take(10))
					{
						stringBuilder2.AppendLine(OnDemandQueryBatchJobProbe.FormatAsyncQueueRequest(request3));
					}
				}
				IEnumerable<AsyncQueueRequest> source3 = from request in source
				where (request.RequestStatus == AsyncQueueStatus.InProgress || request.RequestStatus == AsyncQueueStatus.NotStarted) && request.CreationTime < utcNow - OnDemandQueryBatchJobProbe.requestStaleRange
				select request;
				if (source3.Any<AsyncQueueRequest>())
				{
					stringBuilder.Append("Stale OnDemandQueryBatchRequest found in the past 6 hours and it is over 4 hours old.");
					stringBuilder2.AppendLine(string.Format("{0} {1}, first 10 of the stale requests:", "Stale OnDemandQueryBatchRequest found in the past 6 hours and it is over 4 hours old.", source3.Count<AsyncQueueRequest>()));
					foreach (AsyncQueueRequest request2 in source3.Take(10))
					{
						stringBuilder2.AppendLine(OnDemandQueryBatchJobProbe.FormatAsyncQueueRequest(request2));
					}
				}
			}
			catch (Exception arg)
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("Exception encounter when iterating OnDemandQueryBatchJobProbe AsyncQueue requests: {0}", arg);
				throw;
			}
			finally
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += string.Format("OnDemandQueryBatchJobProbe ended at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			}
			if (stringBuilder.Length > 0)
			{
				base.Result.FailureContext = stringBuilder2.ToString();
				throw new Exception(stringBuilder.ToString());
			}
		}

		private static string FormatAsyncQueueRequest(AsyncQueueRequest request)
		{
			return string.Format("RequestId: {0}, Name: {1}, Status: {2}, CreationTime: {3}", new object[]
			{
				request.RequestId,
				request.FriendlyName,
				request.RequestStatus,
				request.CreationTime
			});
		}

		private const string FailedOnDemandQueryBatchRequestFound = "Failed OnDemandQueryBatchRequest found.";

		private const string StaleOnDemandQueryBatchRequestFound = "Stale OnDemandQueryBatchRequest found in the past 6 hours and it is over 4 hours old.";

		private const string OnDemandQueryBatchRequestOwnerId = "OnDemandQueryBatchRequest";

		private static TimeSpan requestMonitoringRange = TimeSpan.FromHours(6.0);

		private static TimeSpan requestStaleRange = TimeSpan.FromHours(4.0);
	}
}
