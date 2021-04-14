using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal static class FailureItemPublisherHelper
	{
		internal static void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest<FailureTag>(4033228093U, ref failureTag);
			FailureItemPublisherHelper.PublishFailureItem(failureTag, databaseGuid, dbInstanceName, null);
		}

		internal static void PublishActionAndLogEvent(FailureTag failureTag, Guid databaseGuid, string dbInstanceName, ExEventLog.EventTuple errorEventTuple, params string[] eventParams)
		{
			FailureItemPublisherHelper.Trace("Replay Publishing event {0} Action {1} Database {2}", new object[]
			{
				DiagCore.GetEventViewerEventId(errorEventTuple),
				failureTag,
				databaseGuid
			});
			errorEventTuple.LogEvent(null, eventParams);
			FailureItemPublisherHelper.PublishAction(failureTag, databaseGuid, dbInstanceName);
		}

		internal static void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName, IoErrorInfo ioErrorInfo)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest<FailureTag>(4033228093U, ref failureTag);
			FailureItemPublisherHelper.Trace("Replay Publishing Action {0} Database {1}", new object[]
			{
				failureTag,
				databaseGuid
			});
			FailureItemPublisherHelper.PublishFailureItem(failureTag, databaseGuid, dbInstanceName, ioErrorInfo);
		}

		internal static void PublishFailureItem(FailureTag failureTag, Guid databaseGuid, string dbInstanceName, IoErrorInfo ioErrorInfo)
		{
			FailureItemPublisherHelper.PublishFailureItem(FailureNameSpace.Replay, failureTag, databaseGuid, dbInstanceName, ioErrorInfo);
		}

		internal static void PublishFailureItem(FailureNameSpace nameSpace, FailureTag failureTag, Guid databaseGuid, string dbInstanceName, IoErrorInfo ioErrorInfo)
		{
			DatabaseFailureItem databaseFailureItem = new DatabaseFailureItem(nameSpace, failureTag, databaseGuid);
			databaseFailureItem.InstanceName = dbInstanceName;
			databaseFailureItem.IoError = ioErrorInfo;
			try
			{
				databaseFailureItem.Publish();
			}
			catch (ExDbApiException ex)
			{
				FailureItemPublisherHelper.Trace("Failed to publish failure item {0} (error:{1})", new object[]
				{
					databaseFailureItem,
					ex.ToString()
				});
				ReplayEventLogConstants.Tuple_FailedToPublishFailureItem.LogEvent(null, new object[]
				{
					databaseFailureItem.ToString(),
					ex.Message
				});
			}
		}

		private static void Trace(string formatString, params object[] args)
		{
			if (ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string formatString2 = "[FIP] " + formatString;
				ExTraceGlobals.FailureItemTracer.TraceDebug(0L, formatString2, args);
			}
		}
	}
}
