using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsAggregation;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal class DiagnosticsAggregationLog
	{
		public void Start(DiagnosticsAggregationServiceletConfig config)
		{
			if (!config.LoggingEnabled)
			{
				return;
			}
			this.log = new Log("DiagnosticsAggregationLog", new LogHeaderFormatter(DiagnosticsAggregationLog.schema), "DiagnosticsAggregationLog");
			this.log.Configure(config.LogFileDirectoryPath, config.LogFileMaxAge, (long)config.LogFileMaxDirectorySize.ToBytes(), (long)config.LogFileMaxSize.ToBytes());
			this.loggingEnabled = true;
			this.Log(DiagnosticsAggregationEvent.LogStarted, "Webservice started", new object[0]);
		}

		public void Stop()
		{
			if (this.log == null)
			{
				return;
			}
			lock (this)
			{
				this.loggingEnabled = false;
				this.log.Close();
				this.log = null;
			}
		}

		public void Log(DiagnosticsAggregationEvent evt, string format, params object[] parameters)
		{
			this.Log(evt, null, null, string.Empty, string.Empty, null, string.Empty, string.Format(format, parameters));
		}

		public void LogOperationFromClient(DiagnosticsAggregationEvent evt, ClientInformation clientInfo, TimeSpan? duration = null, string description = "")
		{
			this.Log(evt, new uint?(clientInfo.SessionId), duration, (clientInfo != null) ? clientInfo.ClientMachineName : string.Empty, (clientInfo != null) ? clientInfo.ClientProcessName : string.Empty, (clientInfo != null) ? new int?(clientInfo.ClientProcessId) : null, string.Empty, description);
		}

		public void LogOperationToServer(DiagnosticsAggregationEvent evt, uint sessionId, string serverName, TimeSpan? duration = null, string description = "")
		{
			this.Log(evt, new uint?(sessionId), duration, string.Empty, string.Empty, null, serverName, description);
		}

		private void Log(DiagnosticsAggregationEvent evt, uint? sessionId, TimeSpan? duration, string clientHostName, string clientProcessName, int? clientProcessId, string serverHostName, string description)
		{
			if (!this.loggingEnabled)
			{
				return;
			}
			LogSchema logSchema = DiagnosticsAggregationLog.schema;
			uint? num = sessionId;
			DiagnosticsAggregationLogRow row = new DiagnosticsAggregationLogRow(logSchema, evt, (num != null) ? new long?((long)((ulong)num.GetValueOrDefault())) : null, duration, clientHostName, clientProcessName, clientProcessId, serverHostName, description);
			try
			{
				lock (this)
				{
					if (this.loggingEnabled)
					{
						this.log.Append(row, 0);
					}
				}
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError(0L, "Appending to Diagnostics Aggregation log failed with ObjectDisposedException");
			}
		}

		private const string LogComponentName = "DiagnosticsAggregationLog";

		private const string LogFileName = "DiagnosticsAggregationLog";

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Diagnostics Aggregation Log", DiagnosticsAggregationLogRow.Fields);

		private Log log;

		private bool loggingEnabled;
	}
}
