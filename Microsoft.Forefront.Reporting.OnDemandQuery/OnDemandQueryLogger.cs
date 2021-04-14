using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	internal class OnDemandQueryLogger : DisposeTrackableBase
	{
		static OnDemandQueryLogger()
		{
			string[] names = Enum.GetNames(typeof(OnDemandQueryLogFields));
			OnDemandQueryLogger.OnDemandQueryLogSchema = new LogSchema("Microsoft.Forefront.Reporting.Common", "15.00.1497.015", "OnDemandQueryLogs", names);
			string path = "D:\\OnDemandQueryLogs";
			TimeSpan maxAge = TimeSpan.FromDays(5.0);
			long maxDirectorySize = 50000000L;
			long maxLogFileSize = 5000000L;
			OnDemandQueryLogger.onDemandQueryLog = new Log("OnDemandQueryLogs_", new LogHeaderFormatter(OnDemandQueryLogger.OnDemandQueryLogSchema), "OnDemandQueryLogs");
			OnDemandQueryLogger.onDemandQueryLog.Configure(path, maxAge, maxDirectorySize, maxLogFileSize);
		}

		public static void Log(OnDemandQueryRequest queryRequest, OnDemandQueryLogEvent eventType, string exceptionStr = null)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(OnDemandQueryLogger.OnDemandQueryLogSchema);
			logRowFormatter[0] = DateTime.UtcNow;
			logRowFormatter[1] = eventType.ToString();
			logRowFormatter[2] = queryRequest.RequestId;
			logRowFormatter[3] = queryRequest.TenantId;
			logRowFormatter[4] = queryRequest.Region;
			logRowFormatter[5] = queryRequest.SubmissionTime;
			logRowFormatter[6] = queryRequest.QueryType;
			logRowFormatter[7] = queryRequest.QueryPriority;
			logRowFormatter[8] = queryRequest.CallerType;
			logRowFormatter[9] = queryRequest.QueryDefinition;
			logRowFormatter[10] = queryRequest.BatchId;
			logRowFormatter[11] = queryRequest.InBatchQueryId;
			logRowFormatter[12] = queryRequest.CosmosJobId;
			logRowFormatter[13] = queryRequest.MatchRowCounts;
			logRowFormatter[14] = queryRequest.ResultRowCounts;
			logRowFormatter[15] = queryRequest.ResultSize;
			logRowFormatter[16] = queryRequest.ViewCounts;
			logRowFormatter[17] = queryRequest.RetryCount;
			logRowFormatter[18] = queryRequest.ResultLocale;
			logRowFormatter[19] = exceptionStr;
			OnDemandQueryLogger.onDemandQueryLog.Append(logRowFormatter, 0);
		}

		public static void Log(IEnumerable<OnDemandQueryRequest> requests, OnDemandQueryLogEvent eventType, string exceptionStr = null)
		{
			foreach (OnDemandQueryRequest queryRequest in requests)
			{
				OnDemandQueryLogger.Log(queryRequest, eventType, exceptionStr);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OnDemandQueryLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && OnDemandQueryLogger.onDemandQueryLog != null)
			{
				OnDemandQueryLogger.onDemandQueryLog.Close();
			}
		}

		private const string LogType = "OnDemandQueryLogs";

		private const string LogComponentName = "Microsoft.Forefront.Reporting.Common";

		private static readonly LogSchema OnDemandQueryLogSchema;

		private static readonly Log onDemandQueryLog;
	}
}
