using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AuditLogSearchQuery
	{
		public static IEnumerable<T> SearchAuditLogs<T, TFilter>(IAuditLogCollection logCollection, TFilter queryFilter, int maximumResultsCount, TimeSpan timeout, IAuditQueryStrategy<T> strategy, Trace tracer)
		{
			if (maximumResultsCount <= 0)
			{
				if (AuditLogSearchQuery.IsTraceEnabled(tracer))
				{
					tracer.TraceDebug<int>((long)logCollection.GetHashCode(), "SearchAuditLogs: Maximum number of records requested is {0}. No records returned.", maximumResultsCount);
				}
			}
			else
			{
				int totalCount = 0;
				foreach (IAuditLog auditLog in logCollection.GetAuditLogs())
				{
					using (IAuditQueryContext<TFilter> queryContext = auditLog.CreateAuditQueryContext<TFilter>())
					{
						if (AuditLogSearchQuery.IsTraceEnabled(tracer))
						{
							tracer.TraceDebug<DateTime, DateTime, string>((long)logCollection.GetHashCode(), "SearchAuditLogs: Begin async query. Log=[{0},{1}], query=[{2}]", auditLog.EstimatedLogStartTime, auditLog.EstimatedLogEndTime, (queryFilter != null) ? queryFilter.ToString() : string.Empty);
						}
						IAsyncResult asyncResult = queryContext.BeginAuditLogQuery(queryFilter, maximumResultsCount);
						bool queryCompleted = asyncResult.IsCompleted;
						if (!queryCompleted)
						{
							if (AuditLogSearchQuery.IsTraceEnabled(tracer))
							{
								tracer.TraceDebug<TimeSpan>((long)logCollection.GetHashCode(), "SearchAuditLogs: waiting for async query to complete. Timeout=[{0}]", timeout);
							}
							queryCompleted = asyncResult.AsyncWaitHandle.WaitOne(timeout, false);
						}
						else if (AuditLogSearchQuery.IsTraceEnabled(tracer))
						{
							tracer.TraceDebug((long)logCollection.GetHashCode(), "SearchAuditLogs: query completed synchronously");
						}
						if (!queryCompleted)
						{
							if (AuditLogSearchQuery.IsTraceEnabled(tracer))
							{
								tracer.TraceDebug((long)logCollection.GetHashCode(), "SearchAuditLogs: query execution has timed out");
							}
							Exception timeoutException = strategy.GetTimeoutException(timeout);
							if (timeoutException != null)
							{
								throw timeoutException;
							}
						}
						else
						{
							if (AuditLogSearchQuery.IsTraceEnabled(tracer))
							{
								tracer.TraceDebug((long)logCollection.GetHashCode(), "SearchAuditLogs: query execution complete");
							}
							int count = 0;
							foreach (T result in queryContext.EndAuditLogQuery<T>(asyncResult, strategy))
							{
								totalCount++;
								count++;
								yield return result;
								if (totalCount >= maximumResultsCount)
								{
									if (AuditLogSearchQuery.IsTraceEnabled(tracer))
									{
										tracer.TraceDebug<int>((long)logCollection.GetHashCode(), "SearchAuditLogs: maximum total number of records reached. Total records returned={0}", totalCount);
									}
									yield break;
								}
							}
							if (AuditLogSearchQuery.IsTraceEnabled(tracer))
							{
								tracer.TraceDebug<int>((long)logCollection.GetHashCode(), "SearchAuditLogs: query result set has {0} rows", count);
							}
						}
					}
				}
			}
			yield break;
		}

		private static bool IsTraceEnabled(Trace tracer)
		{
			return tracer != null && tracer.IsTraceEnabled(TraceType.DebugTrace);
		}
	}
}
