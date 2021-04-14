using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy
{
	internal static class ExecutionWrapper
	{
		public static void DoRetriableWork(Action doWork, uint retryTimes, Func<Exception, bool> isRetriableError, bool mapGrayException = true)
		{
			uint num = 0U;
			try
			{
				IL_02:
				if (mapGrayException)
				{
					GrayException.MapAndReportGrayExceptions(doWork);
				}
				else
				{
					doWork();
				}
			}
			catch (Exception ex)
			{
				if (isRetriableError(ex) && num < retryTimes)
				{
					num += 1U;
					Thread.Sleep(1000);
					goto IL_02;
				}
				throw ex;
			}
		}

		public static void DoRetriableWorkAndLogIfFail(Action doWork, uint retryTimes, Func<Exception, bool> isRetriableError, ExecutionLog logger, string logClient, string tenantId, string correlationId, ExecutionLog.EventType eventType, string tag, string contextData, bool mapGrayException = true)
		{
			try
			{
				ExecutionWrapper.DoRetriableWork(doWork, retryTimes, isRetriableError, mapGrayException);
			}
			catch (Exception ex)
			{
				logger.LogOneEntry(logClient, tenantId, correlationId, eventType, tag, contextData, ex, new KeyValuePair<string, object>[0]);
				throw ex;
			}
		}
	}
}
