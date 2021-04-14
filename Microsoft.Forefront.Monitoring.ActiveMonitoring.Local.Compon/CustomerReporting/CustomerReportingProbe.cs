using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data.AsyncQueue;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.CustomerReporting
{
	public class CustomerReportingProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("CustomerReportingProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			DateTime utcNow = DateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			try
			{
				AsyncQueueSession asyncQueueSession = new AsyncQueueSession();
				foreach (Guid organizationalUnitRoot in CustomerReportingProbe.schedulerGuids)
				{
					AsyncQueueRequest asyncQueueRequest = asyncQueueSession.GetAsyncQueueRequests(organizationalUnitRoot, "EOPScheduler", null, null, new AsyncQueueStatus?(AsyncQueueStatus.InProgress), null, null, 1).FirstOrDefault<AsyncQueueRequest>();
					if (asyncQueueRequest == null)
					{
						return;
					}
					if (asyncQueueRequest.LastModifiedTime < utcNow - CustomerReportingProbe.requestMonitoringRange)
					{
						stringBuilder.Append("Scheduler instance has not been running for 6 hours.");
						stringBuilder2.Append("Scheduler instance has not been running for 6 hours.");
						stringBuilder2.AppendLine(CustomerReportingProbe.FormatAsyncQueueRequest(asyncQueueRequest));
					}
				}
				List<AsyncQueueRequest> list = asyncQueueSession.GetAsyncQueueRequests(CustomerReportingProbe.customerReportingJobTenantId, "EOPSchedulerCustomerReportJob", null, null, null, new DateTime?(utcNow - CustomerReportingProbe.requestMonitoringRange), null, 1000).ToList<AsyncQueueRequest>();
				List<AsyncQueueRequest> collection = asyncQueueSession.GetAsyncQueueRequests(CustomerReportingProbe.complianceReportingJobTenantId, "EOPSchedulerCustomerReportJob", null, null, null, new DateTime?(utcNow - CustomerReportingProbe.requestMonitoringRange), null, 1000).ToList<AsyncQueueRequest>();
				list.AddRange(collection);
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("CustomerReportingProbe complete iterating AsyncQueue requests at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
				if (!list.Any<AsyncQueueRequest>())
				{
					stringBuilder.Append("No new CustomerReporting requests found in the past 6 hours.");
					stringBuilder2.AppendLine("No new CustomerReporting requests found in the past 6 hours.");
				}
				else
				{
					IEnumerable<AsyncQueueRequest> source = from request in list
					where request.RequestStatus == AsyncQueueStatus.Failed
					select request;
					if (source.Any<AsyncQueueRequest>())
					{
						stringBuilder.Append("Failed CustomerReporting requests found in the past 6 hours.");
						stringBuilder2.AppendLine(string.Format("{0} {1}, first 10 of the failed requests:", "Failed CustomerReporting requests found in the past 6 hours.", source.Count<AsyncQueueRequest>()));
						foreach (AsyncQueueRequest request3 in source.Take(10))
						{
							stringBuilder2.AppendLine(CustomerReportingProbe.FormatAsyncQueueRequest(request3));
						}
					}
					IEnumerable<AsyncQueueRequest> source2 = from request in list
					where (request.RequestStatus == AsyncQueueStatus.InProgress || request.RequestStatus == AsyncQueueStatus.NotStarted) && request.CreationTime < utcNow - CustomerReportingProbe.requestStaleRange
					select request;
					if (source2.Any<AsyncQueueRequest>())
					{
						stringBuilder.Append("Stale CustomerReporting requests found in the past 6 hours and it is over 4 hours old.");
						stringBuilder2.AppendLine(string.Format("{0} {1}, first 10 of the stale requests:", "Stale CustomerReporting requests found in the past 6 hours and it is over 4 hours old.", source2.Count<AsyncQueueRequest>()));
						foreach (AsyncQueueRequest request2 in source2.Take(10))
						{
							stringBuilder2.AppendLine(CustomerReportingProbe.FormatAsyncQueueRequest(request2));
						}
					}
				}
			}
			catch (Exception arg)
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += string.Format("Exception encounter when iterating CustomerReporting AsyncQueue requests: {0}", arg);
				throw;
			}
			finally
			{
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += string.Format("CustomerReportingProbe ended at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
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

		private const string SchedulerNotRunning = "Scheduler instance has not been running for 6 hours.";

		private const string NoNewCustomerReportingRequestFound = "No new CustomerReporting requests found in the past 6 hours.";

		private const string FailedCustomerReportingRequestFound = "Failed CustomerReporting requests found in the past 6 hours.";

		private const string StaleCustomerReportingRequestFound = "Stale CustomerReporting requests found in the past 6 hours and it is over 4 hours old.";

		private const string CustomerReportingJobOwnerId = "EOPSchedulerCustomerReportJob";

		private const string CustomerReportingSchedulerOwnerId = "EOPScheduler";

		private static Guid customerReportingJobTenantId = new Guid("4947cd3c-5f10-4507-b924-75ba2493de37");

		private static Guid complianceReportingJobTenantId = new Guid("0E12795A-4963-4E3F-B7C6-8AA627B109FB");

		private static Guid[] schedulerGuids = new Guid[]
		{
			new Guid("23abf60f-5b3b-4b8f-836d-b6f3574ba074"),
			new Guid("39150751-790f-4179-9e4c-549ed9a24454"),
			new Guid("1d040574-1eab-4bef-be7a-c6dffc03b13d"),
			new Guid("080FA8AC-754F-4C13-887E-E8FF7E6D02F7")
		};

		private static TimeSpan requestMonitoringRange = TimeSpan.FromHours(6.0);

		private static TimeSpan requestStaleRange = TimeSpan.FromHours(4.0);
	}
}
