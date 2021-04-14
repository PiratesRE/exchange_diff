using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core
{
	internal static class PerformanceMonitor
	{
		static PerformanceMonitor()
		{
			PerformanceMonitor.BuildGenericCounterMap();
			PerformanceMonitor.BuildCustomCounterMap();
		}

		private static void BuildGenericCounterMap()
		{
			string[] names = Enum.GetNames(typeof(ResponseType));
			int num = names.Length;
			PerformanceMonitor.respTypeToRequestCountCounterMap = new Dictionary<ResponseType, ExPerformanceCounter>(num);
			PerformanceMonitor.respTypeToRequestSuccessCountCounterMap = new Dictionary<ResponseType, ExPerformanceCounter>(num);
			PerformanceMonitor.soapActionToLatencyCounterMap = new Dictionary<string, ExPerformanceCounter>(num * PerformanceMonitor.SchemaPatterns.Length);
			PerformanceMonitor.latencycounterToRunningAverageFloatMap = new Dictionary<string, RunningAverageFloat>(num + 2);
			PerformanceMonitor.missingCounters = new List<string>();
			foreach (string text in names)
			{
				ResponseType responseType = (ResponseType)Enum.Parse(typeof(ResponseType), text);
				string apiMethodName = text.Replace("ResponseMessage", string.Empty);
				PerformanceMonitor.AddGenericCounterForApiMethod(apiMethodName, responseType);
			}
			if (PerformanceMonitor.missingCounters.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("List of missing counters");
				stringBuilder.Append(Environment.NewLine);
				foreach (string value in PerformanceMonitor.missingCounters)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(Environment.NewLine);
				}
				string errorMessage = stringBuilder.ToString();
				PerformanceMonitor.LogFailure(errorMessage, true);
			}
		}

		private static void AddGenericCounterForApiMethod(string apiMethodName, ResponseType responseType)
		{
			PerformanceMonitor.AddCounterToCollection<ResponseType>(apiMethodName, "{0} Requests", responseType, ref PerformanceMonitor.respTypeToRequestCountCounterMap);
			PerformanceMonitor.AddCounterToCollection<ResponseType>(apiMethodName, "{0} Successful Requests", responseType, ref PerformanceMonitor.respTypeToRequestSuccessCountCounterMap);
			foreach (string format in PerformanceMonitor.SchemaPatterns)
			{
				PerformanceMonitor.AddCounterToCollection<string>(apiMethodName, "{0} Average Response Time", string.Format(format, apiMethodName), ref PerformanceMonitor.soapActionToLatencyCounterMap);
			}
			PerformanceMonitor.latencycounterToRunningAverageFloatMap.Add(string.Format("{0} Average Response Time", apiMethodName), new RunningAverageFloat(25));
		}

		private static void AddCounterToCollection<T>(string apiMethodName, string counterTypePattern, T key, ref Dictionary<T, ExPerformanceCounter> targetCollection)
		{
			string text = string.Format(counterTypePattern, apiMethodName);
			ExPerformanceCounter counterFromAllCountersCollection = PerformanceMonitor.GetCounterFromAllCountersCollection(text);
			if (counterFromAllCountersCollection != null)
			{
				targetCollection.Add(key, counterFromAllCountersCollection);
				return;
			}
			PerformanceMonitor.missingCounters.Add(text);
		}

		private static ExPerformanceCounter GetCounterFromAllCountersCollection(string counterName)
		{
			ExPerformanceCounter result = null;
			foreach (ExPerformanceCounter exPerformanceCounter in WsPerformanceCounters.AllCounters)
			{
				if (exPerformanceCounter.CounterName == counterName)
				{
					result = exPerformanceCounter;
					break;
				}
			}
			return result;
		}

		private static void BuildCustomCounterMap()
		{
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap = new Dictionary<ResponseType, ExPerformanceCounter>();
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.CreateFolderResponseMessage, WsPerformanceCounters.TotalFoldersCreated);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.CopyFolderResponseMessage, WsPerformanceCounters.TotalFoldersCopied);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.MoveFolderResponseMessage, WsPerformanceCounters.TotalFoldersMoved);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.UpdateFolderResponseMessage, WsPerformanceCounters.TotalFoldersUpdated);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.DeleteFolderResponseMessage, WsPerformanceCounters.TotalFoldersDeleted);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.SyncFolderHierarchyResponseMessage, WsPerformanceCounters.TotalFoldersSynced);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.GetFolderResponseMessage, WsPerformanceCounters.TotalFoldersRead);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.CreateItemResponseMessage, WsPerformanceCounters.TotalItemsCreated);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.UploadItemsResponseMessage, WsPerformanceCounters.TotalItemsCreated);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.CopyItemResponseMessage, WsPerformanceCounters.TotalItemsCopied);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.MoveItemResponseMessage, WsPerformanceCounters.TotalItemsMoved);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.UpdateItemResponseMessage, WsPerformanceCounters.TotalItemsUpdated);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.DeleteItemResponseMessage, WsPerformanceCounters.TotalItemsDeleted);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.SyncFolderItemsResponseMessage, WsPerformanceCounters.TotalItemsSynced);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.SendItemResponseMessage, WsPerformanceCounters.TotalItemsSent);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.GetItemResponseMessage, WsPerformanceCounters.TotalItemsRead);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.FindItemResponseMessage, WsPerformanceCounters.TotalItemsRead);
			PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.Add(ResponseType.ExportItemsResponseMessage, WsPerformanceCounters.TotalItemsRead);
			PerformanceMonitor.latencycounterToRunningAverageFloatMap.Add("Average Response Time", new RunningAverageFloat(25));
			PerformanceMonitor.latencycounterToRunningAverageFloatMap.Add("Proxy Average Response Time", new RunningAverageFloat(25));
		}

		public static void Initialize()
		{
			try
			{
				foreach (ExPerformanceCounter exPerformanceCounter in WsPerformanceCounters.AllCounters)
				{
					exPerformanceCounter.RawValue = 0L;
				}
				if (EWSSettings.IsWsPerformanceCountersEnabled)
				{
					foreach (ExPerformanceCounter exPerformanceCounter2 in WsDatacenterPerformanceCounters.AllCounters)
					{
						exPerformanceCounter2.RawValue = 0L;
					}
				}
				WsPerformanceCounters.PID.RawValue = (long)Process.GetCurrentProcess().Id;
				PerformanceMonitor.performanceCountersInitialized = true;
			}
			catch (InvalidOperationException exception)
			{
				PerformanceMonitor.performanceCountersInitialized = false;
				ServiceDiagnostics.LogExceptionWithTrace(ServicesEventLogConstants.Tuple_InitializePerformanceCountersFailed, null, ExTraceGlobals.PerformanceMonitorTracer, null, "Failed to initialize performance counters. Error: {0}.", exception);
			}
		}

		public static bool PerformanceCountersEnabled
		{
			get
			{
				return PerformanceMonitor.performanceCountersInitialized;
			}
		}

		public static void UpdateResponseTimePerformanceCounter(long latency)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					PerformanceMonitor.UpdateMovingAveragePerformanceCounter(WsPerformanceCounters.AverageResponseTime, latency);
					WsPerformanceCounters.TotalRequests.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update response time performance counter. Error: {0}.", arg);
				}
				PerformanceMonitor.UpdateResponseTimePerformanceCounterForSoapAction(latency);
			}
		}

		public static void UpdateProxyResponseTimePerformanceCounter(long newValue)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					PerformanceMonitor.UpdateMovingAveragePerformanceCounter(WsPerformanceCounters.ProxyAverageResponseTime, newValue);
					WsPerformanceCounters.TotalProxyRequests.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update proxy response time performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdatePushStatusCounter(bool success)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					ExPerformanceCounter exPerformanceCounter = success ? WsPerformanceCounters.TotalPushNotificationSuccesses : WsPerformanceCounters.TotalPushNotificationFailures;
					exPerformanceCounter.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update push status performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateUnsubscribeCounter()
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalUnsubscribeRequests.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update unsubscribe performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateFailedSubscriptionCounter()
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					ExPerformanceCounter totalFailedSubscriptions = WsPerformanceCounters.TotalFailedSubscriptions;
					totalFailedSubscriptions.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update failed subscription performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateStreamedEventsCounter(long eventCount)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					ExPerformanceCounter totalStreamedEvents = WsPerformanceCounters.TotalStreamedEvents;
					totalStreamedEvents.IncrementBy(eventCount);
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update streamedEvents performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateActiveSubscriptionsCounter(long count)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.ActiveSubscriptions.RawValue = count;
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update active subscription performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateActiveStreamingConnectionsCounter(long count)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.ActiveStreamingConnections.RawValue = count;
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update active streaming subscription connection performance counter. Error: {0}.", arg);
				}
			}
		}

		public static void UpdateTotalProxyRequestBytesCount(long byteCount)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalProxyRequestBytes.IncrementBy(byteCount);
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update TotalProxyRequestBytes performance counter.  Error {0}", arg);
				}
			}
		}

		public static void UpdateTotalProxyResponseBytesCount(long byteCount)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalProxyResponseBytes.IncrementBy(byteCount);
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update TotalProxyResponseBytes performance counter.  Error {0}", arg);
				}
			}
		}

		public static void UpdateTotalProxyFailoversCount()
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalProxyFailovers.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update TotalProxyFailovers performance counter.  Error {0}", arg);
				}
			}
		}

		public static void UpdateTotalRequestRejectionsCount()
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalRequestRejections.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update TotalRequestRejections performance counter.  Error {0}", arg);
				}
			}
		}

		public static void UpdateTotalCompletedRequestsCount()
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					WsPerformanceCounters.TotalCompletedRequests.Increment();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update TotalCompletedRequests performance counter.  Error {0}", arg);
				}
			}
		}

		public static void UpdateTotalRequestsReceivedWithPartnerToken()
		{
			PerformanceMonitor.SafeUpdatePerfCounter("TotalRequestsReceivedWithPartnerToken", delegate
			{
				WsDatacenterPerformanceCounters.TotalRequestsReceivedWithPartnerToken.Increment();
			});
		}

		public static void UpdateTotalUnauthorizedRequestsReceivedWithPartnerToken()
		{
			PerformanceMonitor.SafeUpdatePerfCounter("TotalUnauthorizedRequestsReceivedWithPartnerToken", delegate
			{
				WsDatacenterPerformanceCounters.TotalUnauthorizedRequestsReceivedWithPartnerToken.Increment();
			});
		}

		public static void UpdatePartnerTokenCacheEntries(int value)
		{
			PerformanceMonitor.SafeUpdatePerfCounter("PartnerTokenCacheEntries", delegate
			{
				WsDatacenterPerformanceCounters.PartnerTokenCacheEntries.RawValue = (long)value;
			});
		}

		private static void SafeUpdatePerfCounter(string counterName, Action updateAction)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					updateAction();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<string, InvalidOperationException>(0L, "Failed to update {0} performance counter. Error {1}", counterName, arg);
				}
			}
		}

		public static void UpdateResponseCounters(IExchangeWebMethodResponse response, int objectsChanged)
		{
			if (PerformanceMonitor.PerformanceCountersEnabled)
			{
				try
				{
					ResponseType responseType = response.GetResponseType();
					ExPerformanceCounter exPerformanceCounter;
					if (PerformanceMonitor.respTypeToRequestCountCounterMap.TryGetValue(responseType, out exPerformanceCounter))
					{
						exPerformanceCounter.Increment();
					}
					else
					{
						PerformanceMonitor.ReportAbsentCounter(responseType, "Request Count", true);
					}
					if (response.GetErrorCodeToLog() == ResponseCodeType.NoError)
					{
						ExPerformanceCounter exPerformanceCounter2;
						if (PerformanceMonitor.respTypeToRequestSuccessCountCounterMap.TryGetValue(responseType, out exPerformanceCounter2))
						{
							exPerformanceCounter2.Increment();
						}
						else
						{
							PerformanceMonitor.ReportAbsentCounter(responseType, "Request Success Count", true);
						}
					}
					ExPerformanceCounter exPerformanceCounter3;
					if (PerformanceMonitor.respTypeToRequestObjectsChangedCounterMap.TryGetValue(responseType, out exPerformanceCounter3))
					{
						if (objectsChanged > 0)
						{
							exPerformanceCounter3.IncrementBy((long)objectsChanged);
						}
					}
					else
					{
						PerformanceMonitor.ReportAbsentCounter(responseType, "Request Count", false);
					}
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update response performance counter. Error: {0}.", arg);
				}
			}
		}

		private static void UpdateResponseTimePerformanceCounterForSoapAction(long latency)
		{
			string soapAction = PerformanceMonitor.GetSoapAction();
			if (soapAction != null)
			{
				try
				{
					ExPerformanceCounter exPerformanceCounter;
					if (PerformanceMonitor.soapActionToLatencyCounterMap.TryGetValue(soapAction, out exPerformanceCounter))
					{
						exPerformanceCounter.Increment();
					}
					else
					{
						PerformanceMonitor.ReportAbsentCounter(soapAction, "Latency Counter", true);
					}
					PerformanceMonitor.UpdateMovingAveragePerformanceCounter(exPerformanceCounter, latency);
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.PerformanceMonitorTracer.TraceError<InvalidOperationException>(0L, "Failed to update response time performance counter. Error: {0}.", arg);
				}
			}
		}

		private static void ReportAbsentCounter(ResponseType responseType, string counterType, bool failRequest)
		{
			string errorMessage = string.Format("Performance counter not found for response type : {0}", responseType);
			PerformanceMonitor.LogFailure(errorMessage, failRequest);
		}

		private static void ReportAbsentCounter(string soapAction, string counterType, bool failRequest)
		{
			string errorMessage = string.Format("Performance counter not found for soap action : {0}", soapAction);
			PerformanceMonitor.LogFailure(errorMessage, failRequest);
		}

		private static void ReportAbsentRunningAverageFloatInstance(string counterName)
		{
			string errorMessage = string.Format("RunningAverageFloat instance not found for counter : {0}", counterName);
			PerformanceMonitor.LogFailure(errorMessage, true);
		}

		private static void LogFailure(string errorMessage, bool failRequest)
		{
			ExTraceGlobals.PerformanceMonitorTracer.TraceError(0L, errorMessage);
			if (failRequest)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		private static string GetSoapAction()
		{
			CallContext callContext = HttpContext.Current.Items["CallContext"] as CallContext;
			if (callContext != null)
			{
				return callContext.SoapAction;
			}
			return null;
		}

		private static void UpdateMovingAveragePerformanceCounter(ExPerformanceCounter performanceCounter, long newValue)
		{
			string counterName = performanceCounter.CounterName;
			lock (performanceCounter)
			{
				if (PerformanceMonitor.latencycounterToRunningAverageFloatMap.ContainsKey(counterName))
				{
					PerformanceMonitor.latencycounterToRunningAverageFloatMap[counterName].Update((float)newValue);
					performanceCounter.RawValue = (long)PerformanceMonitor.latencycounterToRunningAverageFloatMap[counterName].Value;
				}
				else
				{
					PerformanceMonitor.ReportAbsentRunningAverageFloatInstance(counterName);
				}
			}
		}

		private const int LatencyCounterNumberOfSamples = 25;

		private const string RequestCountCounterPattern = "{0} Requests";

		private const string RequestSuccessCountCounterPattern = "{0} Successful Requests";

		private const string LatencyCounterPattern = "{0} Average Response Time";

		private static readonly string[] SchemaPatterns = new string[]
		{
			"http://schemas.microsoft.com/exchange/services/2006/messages/{0}",
			"http://schemas.microsoft.com/exchange/services/2006a/messages/{0}"
		};

		private static bool performanceCountersInitialized;

		private static Dictionary<ResponseType, ExPerformanceCounter> respTypeToRequestObjectsChangedCounterMap;

		private static Dictionary<ResponseType, ExPerformanceCounter> respTypeToRequestCountCounterMap;

		private static Dictionary<ResponseType, ExPerformanceCounter> respTypeToRequestSuccessCountCounterMap;

		private static Dictionary<string, ExPerformanceCounter> soapActionToLatencyCounterMap;

		private static Dictionary<string, RunningAverageFloat> latencycounterToRunningAverageFloatMap;

		private static List<string> missingCounters;
	}
}
