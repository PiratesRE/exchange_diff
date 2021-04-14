using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OfficeGraph
{
	internal static class OfficeGraphMonitoringHelper
	{
		static OfficeGraphMonitoringHelper()
		{
			MonitoringLogConfiguration configuration = new MonitoringLogConfiguration(ExchangeComponent.OfficeGraph.Name, "Monitoring");
			OfficeGraphMonitoringHelper.monitoringLogger = new MonitoringLogger(configuration);
		}

		internal static long GetPerformanceCounterValue(string categoryName, string counterName)
		{
			long result = 0L;
			using (PerformanceCounter performanceCounter = new PerformanceCounter(categoryName, counterName, true))
			{
				result = performanceCounter.RawValue;
			}
			return result;
		}

		internal static long GetPerformanceCounterValue(string categoryName, string counterName, string instanceName)
		{
			long result = 0L;
			using (PerformanceCounter performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName, true))
			{
				result = performanceCounter.RawValue;
			}
			return result;
		}

		internal static ProbeResult GetLastProbeResult(ProbeWorkItem probe, IProbeWorkBroker broker, CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = null;
			if (broker != null)
			{
				IOrderedEnumerable<ProbeResult> query = from r in broker.GetProbeResults(probe.Definition, probe.Result.ExecutionStartTime.AddSeconds((double)(-5 * probe.Definition.RecurrenceIntervalSeconds)))
				orderby r.ExecutionStartTime descending
				select r;
				Task<int> task = broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult r)
				{
					if (lastProbeResult == null)
					{
						lastProbeResult = r;
					}
				}, cancellationToken, OfficeGraphMonitoringHelper.traceContext);
				task.Wait(cancellationToken);
				return lastProbeResult;
			}
			if (ExEnvironment.IsTest)
			{
				return null;
			}
			throw new ArgumentNullException("broker");
		}

		internal static MailboxDatabaseInfo GetDatabaseInfo(string databaseName)
		{
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentException("databaseName");
			}
			lock (OfficeGraphMonitoringHelper.databaseInfoDict)
			{
				if (OfficeGraphMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					return OfficeGraphMonitoringHelper.databaseInfoDict[databaseName];
				}
			}
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForBackend = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			MailboxDatabaseInfo result;
			lock (OfficeGraphMonitoringHelper.databaseInfoDict)
			{
				if (!OfficeGraphMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					OfficeGraphMonitoringHelper.databaseInfoDict.Clear();
					foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForBackend)
					{
						OfficeGraphMonitoringHelper.databaseInfoDict.Add(mailboxDatabaseInfo.MailboxDatabaseName, mailboxDatabaseInfo);
					}
				}
				if (OfficeGraphMonitoringHelper.databaseInfoDict.ContainsKey(databaseName))
				{
					result = OfficeGraphMonitoringHelper.databaseInfoDict[databaseName];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal static long GetDirectorySize(string directory)
		{
			return new DirectoryInfo(directory).GetFiles("*.*", SearchOption.AllDirectories).Sum((FileInfo file) => file.Length);
		}

		internal static void LogInfo(string message, params object[] messageArgs)
		{
			OfficeGraphMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, message, messageArgs);
		}

		internal static void LogInfo(WorkItem workItem, string message, params object[] messageArgs)
		{
			OfficeGraphMonitoringHelper.monitoringLogger.LogEvent(DateTime.UtcNow, string.Format("{0}/{1}: ", workItem.Definition.Name, workItem.Definition.TargetResource) + message, messageArgs);
		}

		internal const string MSExchangeMailboxTransportDeliveryServiceName = "MSExchangeDelivery";

		internal const string MSMessageTracingClientServiceName = "MSMessageTracingClient";

		internal const string MSExchangeDeliveryExtensibilityAgentsCounterCategoryName = "MSExchange Delivery Extensibility Agents";

		internal const string AverageAgentProcessingTimeCounterName = "Average Agent Processing Time (sec)";

		internal const string OfficeGraphAgentInstanceName = "office graph agent";

		internal const string OfficeGraphWriterMessageTracingPluginCounterCategoryName = "Office Graph Writer - Message Tracing Plugin";

		internal const string AverageSignalProcessingTimeCounterName = "Average Signal Processing Time";

		internal const string MessageTracingPluginLogDirectory = "D:\\OfficeGraph";

		private static readonly TracingContext traceContext = TracingContext.Default;

		private static MonitoringLogger monitoringLogger;

		private static Dictionary<string, MailboxDatabaseInfo> databaseInfoDict = new Dictionary<string, MailboxDatabaseInfo>(StringComparer.OrdinalIgnoreCase);
	}
}
