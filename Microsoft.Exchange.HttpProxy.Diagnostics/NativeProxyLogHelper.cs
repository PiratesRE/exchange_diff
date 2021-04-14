using System;
using System.Web;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class NativeProxyLogHelper
	{
		public static void PublishNativeProxyStatistics(HttpContextBase httpContext)
		{
			RequestLogger logger = RequestLogger.GetLogger(httpContext);
			string value = httpContext.Response.Headers[NativeProxyLogHelper.NativeProxyStatusHeaders.BackEndHttpStatus];
			if (!string.IsNullOrEmpty(value))
			{
				logger.LogField(LogKey.BackendStatus, value);
			}
			string text = httpContext.Response.Headers[NativeProxyLogHelper.NativeProxyStatusHeaders.ProxyErrorHResult];
			string arg = httpContext.Response.Headers[NativeProxyLogHelper.NativeProxyStatusHeaders.ProxyErrorLabel];
			string arg2 = httpContext.Response.Headers[NativeProxyLogHelper.NativeProxyStatusHeaders.ProxyErrorMessage];
			if (!string.IsNullOrEmpty(text))
			{
				logger.LogField(LogKey.ErrorCode, text);
				string value2 = string.Format("[{0}] [{1}] {2}", text, arg, arg2);
				logger.AppendErrorInfo("ProxyError", value2);
			}
			HttpWorkerRequest httpWorkerRequest = (HttpWorkerRequest)((IServiceProvider)httpContext).GetService(typeof(HttpWorkerRequest));
			bool hasWinHttpQuery = NativeProxyLogHelper.PublishTimestamps(httpWorkerRequest, logger);
			NativeProxyLogHelper.PublishLatencies(httpWorkerRequest, logger, hasWinHttpQuery);
			NativeProxyLogHelper.PublishCounters(httpWorkerRequest, logger);
			NativeProxyLogHelper.PublishStreamStats(httpWorkerRequest, logger);
			NativeProxyLogHelper.PublishGenericStats(httpWorkerRequest, logger, NativeProxyLogHelper.NativeProxyStatisticsVariables.RequestBufferSizeFootprints);
			NativeProxyLogHelper.PublishGenericStats(httpWorkerRequest, logger, NativeProxyLogHelper.NativeProxyStatisticsVariables.ResponseBufferSizeFootprints);
			if (NativeProxyLogHelper.LogBufferCopyStats.Value)
			{
				NativeProxyLogHelper.PublishGenericStats(httpWorkerRequest, logger, NativeProxyLogHelper.NativeProxyStatisticsVariables.BufferCopyStatsClientUpload);
				NativeProxyLogHelper.PublishGenericStats(httpWorkerRequest, logger, NativeProxyLogHelper.NativeProxyStatisticsVariables.BufferCopyStatsServerQuery);
				NativeProxyLogHelper.PublishGenericStats(httpWorkerRequest, logger, NativeProxyLogHelper.NativeProxyStatisticsVariables.BufferCopyStatsServerDownload);
			}
		}

		private static bool PublishTimestamps(HttpWorkerRequest httpWorkerRequest, RequestLogger logger)
		{
			bool result = false;
			string serverVariable = httpWorkerRequest.GetServerVariable(NativeProxyLogHelper.NativeProxyStatisticsVariables.Timestamps);
			if (!string.IsNullOrEmpty(serverVariable))
			{
				logger.AppendGenericInfo(NativeProxyLogHelper.NativeProxyStatisticsVariables.Timestamps, serverVariable);
				long[] array = NativeProxyLogHelper.ConvertStatisticsDataArray(serverVariable);
				logger.LogField(LogKey.ModuleToHandlerSwitchingLatency, array[1] - array[0]);
				long num = array[14] - array[3];
				logger.LogField(LogKey.BackendProcessingLatency, num);
				long num2 = num;
				if (array[22] >= 0L)
				{
					num2 = array[22] - array[3];
				}
				logger.LogField(LogKey.ProxyTime, num2);
				result = (array[15] >= 0L);
			}
			return result;
		}

		private static void PublishLatencies(HttpWorkerRequest httpWorkerRequest, RequestLogger logger, bool hasWinHttpQuery)
		{
			string serverVariable = httpWorkerRequest.GetServerVariable(NativeProxyLogHelper.NativeProxyStatisticsVariables.Latencies);
			if (!string.IsNullOrEmpty(serverVariable))
			{
				logger.AppendGenericInfo(NativeProxyLogHelper.NativeProxyStatisticsVariables.Latencies, serverVariable);
				long[] array = NativeProxyLogHelper.ConvertStatisticsDataArray(serverVariable);
				logger.LogField(LogKey.ClientReqStreamLatency, array[0]);
				logger.LogField(LogKey.ClientRespStreamLatency, array[1]);
				logger.LogField(LogKey.BackendReqStreamLatency, array[2]);
				if (hasWinHttpQuery)
				{
					logger.LogField(LogKey.BackendRespStreamLatency, array[3]);
					return;
				}
				logger.LogField(LogKey.BackendRespStreamLatency, array[4]);
			}
		}

		private static void PublishCounters(HttpWorkerRequest httpWorkerRequest, RequestLogger logger)
		{
			string serverVariable = httpWorkerRequest.GetServerVariable(NativeProxyLogHelper.NativeProxyStatisticsVariables.Counters);
			if (!string.IsNullOrEmpty(serverVariable))
			{
				logger.AppendGenericInfo(NativeProxyLogHelper.NativeProxyStatisticsVariables.Counters, serverVariable);
			}
		}

		private static void PublishStreamStats(HttpWorkerRequest httpWorkerRequest, RequestLogger logger)
		{
			string serverVariable = httpWorkerRequest.GetServerVariable(NativeProxyLogHelper.NativeProxyStatisticsVariables.StreamStats);
			if (!string.IsNullOrEmpty(serverVariable))
			{
				logger.AppendGenericInfo(NativeProxyLogHelper.NativeProxyStatisticsVariables.StreamStats, serverVariable);
				long[] array = NativeProxyLogHelper.ConvertStatisticsDataArray(serverVariable);
				logger.LogField(LogKey.RequestBytes, array[0]);
				logger.LogField(LogKey.ResponseBytes, array[2]);
			}
		}

		private static void PublishGenericStats(HttpWorkerRequest httpWorkerRequest, RequestLogger logger, string statsDataName)
		{
			string serverVariable = httpWorkerRequest.GetServerVariable(statsDataName);
			if (!string.IsNullOrEmpty(serverVariable))
			{
				logger.AppendGenericInfo(statsDataName, serverVariable);
			}
		}

		private static long[] ConvertStatisticsDataArray(string dataLine)
		{
			string[] array = dataLine.Split(new char[]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			return Array.ConvertAll<string, long>(array, (string x) => long.Parse(x));
		}

		public static readonly BoolAppSettingsEntry LogBufferCopyStats = new BoolAppSettingsEntry("NativeHttpProxy.LogBufferCopyStats", false, null);

		private enum ProxyEventTimestamps
		{
			Timestamp_IIS_MapHandler,
			Timestamp_IIS_ExecuteHandler,
			Timestamp_WinHttp_Pre_SendRequest,
			Timestamp_WinHttp_Post_SendRequest,
			Timestamp_IIS_BeginRead_First,
			Timestamp_IIS_BeginRead_Last,
			Timestamp_IIS_ReadComplete_First,
			Timestamp_IIS_ReadComplete_Last,
			Timestamp_WinHttp_SendRequestComplete,
			Timestamp_WinHttp_BeginWrite_First,
			Timestamp_WinHttp_BeginWrite_Last,
			Timestamp_WinHttp_WriteComplete_First,
			Timestamp_WinHttp_WriteComplete_Last,
			Timestamp_WinHttp_ReceiveResponse,
			Timestamp_WinHttp_HeadersAvailable,
			Timestamp_WinHttp_QueryData_First,
			Timestamp_WinHttp_QueryData_Last,
			Timestamp_WinHttp_DataAvailable_First,
			Timestamp_WinHttp_DataAvailable_Last,
			Timestamp_WinHttp_BeginRead_First,
			Timestamp_WinHttp_BeginRead_Last,
			Timestamp_WinHttp_ReadComplete_First,
			Timestamp_WinHttp_ReadComplete_Last,
			Timestamp_IIS_BeginWrite_First,
			Timestamp_IIS_BeginWrite_Last,
			Timestamp_IIS_WriteComplete_First,
			Timestamp_IIS_WriteComplete_Last,
			Timestamp_WinHttp_RequestError,
			Timestamp_Proxy_CompleteRequest,
			Timestamp_IIS_CompleteRequest
		}

		private enum ProxyStreamingLatencies
		{
			Latency_IIS_Read,
			Latency_IIS_Write,
			Latency_WinHttp_Write,
			Latency_WinHttp_Query,
			Latency_WinHttp_Read
		}

		private enum ProxyEventCounters
		{
			Count_IIS_Read,
			Count_IIS_Write,
			Count_WinHttp_Write,
			Count_WinHttp_DataAvailable,
			Count_WinHttp_Read
		}

		private enum ProxyStreamStats
		{
			Request_BytesLength,
			Request_Chunked,
			Response_BytesLength,
			Response_Chunked
		}

		internal static class NativeProxyStatusHeaders
		{
			public static readonly string BackEndHttpStatus = "X-BackEndHttpStatus";

			public static readonly string ProxyErrorHResult = "X-ProxyErrorHResult";

			public static readonly string ProxyErrorLabel = "X-ProxyErrorLabel";

			public static readonly string ProxyErrorMessage = "X-ProxyErrorMessage";
		}

		internal static class NativeProxyStatisticsVariables
		{
			public static readonly string Timestamps = "ProxyStats_Event_Timestamps";

			public static readonly string Counters = "ProxyStats_Event_Counters";

			public static readonly string Latencies = "ProxyStats_Streaming_Latencies";

			public static readonly string StreamStats = "ProxyStats_Stream_Stats";

			public static readonly string RequestBufferSizeFootprints = "ProxyStats_BufferSizeFootprints_Request";

			public static readonly string ResponseBufferSizeFootprints = "ProxyStats_BufferSizeFootprints_Response";

			public static readonly string BufferCopyStatsClientUpload = "ProxyStats_BufferCopyStats_Client_Upload";

			public static readonly string BufferCopyStatsServerQuery = "ProxyStats_BufferCopyStats_Server_Query";

			public static readonly string BufferCopyStatsServerDownload = "ProxyStats_BufferCopyStats_Server_Download";
		}
	}
}
