using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class FullTextIndexLogger
	{
		public static bool IsLoggingEnabled
		{
			get
			{
				return FullTextIndexLogger.binaryLogger != null && FullTextIndexLogger.binaryLogger.IsLoggingEnabled;
			}
		}

		public static void Initialize()
		{
			FullTextIndexLogger.binaryLogger = LoggerManager.GetLogger(LoggerType.FullTextIndex);
			FullTextIndexLogger.minStringLogBufferSize = 6 + Guid.Empty.ToString().Length + 4 + 4 + 4 + 3;
		}

		public static void LogViewOperation(Guid databaseGuid, int mailboxNumber, int clientType, Guid correlationId, FullTextIndexLogger.ViewOperationType viewOperation, TimeSpan operationTime, int lazyIndexSeek, int lazyIndexRead, int messageSeek, int messageRead, bool isOptimizedInstantSearch)
		{
			if (!FullTextIndexLogger.IsLoggingEnabled)
			{
				return;
			}
			long num = (long)operationTime.TotalMilliseconds;
			string logString = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
			{
				DateTime.UtcNow.ToString("MM/dd/yyyy HH-mm-ss.fff"),
				(int)viewOperation,
				num,
				lazyIndexSeek,
				lazyIndexRead,
				messageSeek,
				messageRead,
				isOptimizedInstantSearch ? 1 : 0
			});
			FullTextIndexLogger.Log(FullTextIndexLogger.LogOperationType.ViewOperation, databaseGuid, mailboxNumber, clientType, correlationId, logString);
		}

		public static void LogOtherSuboperation(Guid databaseGuid, int mailboxNumber, int clientType, Guid correlationId, int suboperationType)
		{
			if (!FullTextIndexLogger.IsLoggingEnabled)
			{
				return;
			}
			FullTextIndexLogger.Log(FullTextIndexLogger.LogOperationType.OtherOperation, databaseGuid, mailboxNumber, clientType, correlationId, suboperationType.ToString());
		}

		public static void LogViewSetSearchCriteria(Guid databaseGuid, int mailboxNumber, int clientType, Guid correlationId)
		{
			if (!FullTextIndexLogger.IsLoggingEnabled)
			{
				return;
			}
			FullTextIndexLogger.Log(FullTextIndexLogger.LogOperationType.ViewSetSearchCriteria, databaseGuid, mailboxNumber, clientType, correlationId, DateTime.UtcNow.ToString("MM/dd/yyyy HH-mm-ss.fff"));
		}

		public static void LogSingleLineQuery(Guid correlationId, DateTime queryStartTime, DateTime queryEndTime, Guid databaseGuid, Guid mailboxGuid, int mailboxNumber, int clientType, string queryString, bool failed, string errorMessage, bool maxLinkCountReached, string storeResidual, int numberFastTrips, bool isPulsing, string firstScopeFolder, int maxCount, int scopeFolderCount, int initialSearchState, int finalSearchState, int searchCriteriaFlags, bool isNestedSearchFolder, int first1000FastResults, int firstNotificationFastResults, int fastResults, int totalResults, TimeSpan searchRestrictionTime, TimeSpan searchPlanTime, int first1000FastTime, int firstResultsFastTime, string fastTimes, int firstResultsTime, int fastTime, int expandedScopeFolderCount, string friendlyFolderName, uint totalRowsProcessed, string clientActionString, string scrubbedQuery, string replacements)
		{
			if (!FullTextIndexLogger.IsLoggingEnabled)
			{
				return;
			}
			using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.FullTextIndexSingleLine, true, true, 6, correlationId.ToString(), databaseGuid.ToString(), mailboxGuid.ToString(), queryStartTime.ToString("o"), queryEndTime.ToString("o"), mailboxNumber, clientType, queryString, failed ? 1 : 0, errorMessage, maxLinkCountReached ? 1 : 0, storeResidual, numberFastTrips, isPulsing ? 1 : 0, firstScopeFolder, maxCount, scopeFolderCount, initialSearchState, finalSearchState, searchCriteriaFlags, isNestedSearchFolder ? 1 : 0, first1000FastResults, firstNotificationFastResults, fastResults, totalResults, (int)searchRestrictionTime.TotalMilliseconds, (int)searchPlanTime.TotalMilliseconds, first1000FastTime, firstResultsFastTime, fastTimes, firstResultsTime, fastTime, expandedScopeFolderCount, friendlyFolderName, totalRowsProcessed, clientActionString, scrubbedQuery, replacements))
			{
				FullTextIndexLogger.binaryLogger.TryWrite(traceBuffer);
			}
		}

		private static void Log(FullTextIndexLogger.LogOperationType operationType, Guid databaseGuid, int mailboxNumber, int clientType, Guid correlationId, string logString)
		{
			string strValue = logString;
			if (logString.Length > 524288)
			{
				StringBuilder stringBuilder = new StringBuilder(logString);
				stringBuilder.Length = 524288 - "</Truncated>".Length;
				stringBuilder.Append("</Truncated>");
				strValue = stringBuilder.ToString();
			}
			using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.FullTextIndexQuery, true, false, 6, correlationId.ToString(), databaseGuid.ToString(), mailboxNumber, (int)operationType, clientType, strValue))
			{
				FullTextIndexLogger.binaryLogger.TryWrite(traceBuffer);
			}
		}

		private const int MaxLogStringLength = 524288;

		private const short TraceVersion = 6;

		private const string TruncatedLogSuffix = "</Truncated>";

		private const string DateTimeFormat = "MM/dd/yyyy HH-mm-ss.fff";

		private static IBinaryLogger binaryLogger;

		private static int minStringLogBufferSize;

		public enum LogOperationType
		{
			SearchFolderPopulationStart = 1,
			RequestSentToFAST,
			ResponseReceivedFromFAST,
			ReceviedErrorFromFAST,
			FirstResultsLinked,
			RequestCompleted,
			ViewOperation,
			ViewSetSearchCriteria,
			OtherOperation
		}

		public enum ViewOperationType
		{
			GetContentsTable = 1,
			GetContentsTableEx,
			QueryRows,
			QueryPosition,
			FindRow,
			SeekRow
		}
	}
}
