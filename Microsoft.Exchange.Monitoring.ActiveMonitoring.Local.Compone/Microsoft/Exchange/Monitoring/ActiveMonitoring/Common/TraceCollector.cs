using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class TraceCollector
	{
		internal void FlushToFile(string logInstanceName, MonitoringLogConfiguration logConfiguration)
		{
			MonitoringLogger loggerInstance = this.GetLoggerInstance(logInstanceName, logConfiguration);
			lock (this.traceTimes)
			{
				loggerInstance.LogEvent(DateTime.UtcNow, "---------------------------------------------------------------------------------------------", null);
				loggerInstance.LogEvents(this.traceTimes, this.traceMessages, this.traceParameters);
				loggerInstance.LogEvent(DateTime.UtcNow, "---------------------------------------------------------------------------------------------", null);
			}
		}

		internal void TraceInformation(Trace tracer, TracingContext tracingContext, string message, params object[] parameters)
		{
			string message2 = message;
			if (parameters != null && parameters.Length > 0)
			{
				message2 = string.Format(message, parameters);
			}
			WTFDiagnostics.TraceInformation(tracer, tracingContext, message2, null, "TraceInformation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Logging\\TraceCollector.cs", 79);
			lock (this.traceTimes)
			{
				this.traceTimes.Add(DateTime.UtcNow);
				this.traceMessages.Add(message);
				this.traceParameters.Add(parameters);
			}
		}

		internal void DisposeLoggerInstance(string instanceName)
		{
			lock (TraceCollector.syncLogCache)
			{
				if (TraceCollector.syncLogCache.ContainsKey(instanceName))
				{
					MonitoringLogger monitoringLogger = TraceCollector.syncLogCache[instanceName];
					TraceCollector.syncLogCache.Remove(instanceName);
					monitoringLogger.Dispose();
				}
			}
		}

		private MonitoringLogger GetLoggerInstance(string instanceName, MonitoringLogConfiguration logConfiguration)
		{
			MonitoringLogger result;
			lock (TraceCollector.syncLogCache)
			{
				if (TraceCollector.syncLogCache.ContainsKey(instanceName))
				{
					result = TraceCollector.syncLogCache[instanceName];
				}
				else
				{
					MonitoringLogger monitoringLogger = new MonitoringLogger(logConfiguration);
					TraceCollector.syncLogCache.Add(instanceName, monitoringLogger);
					result = monitoringLogger;
				}
			}
			return result;
		}

		private static readonly Dictionary<string, MonitoringLogger> syncLogCache = new Dictionary<string, MonitoringLogger>();

		private List<DateTime> traceTimes = new List<DateTime>();

		private List<string> traceMessages = new List<string>();

		private List<object[]> traceParameters = new List<object[]>();
	}
}
