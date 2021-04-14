using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal class FlowControlLog : IFlowControlLog
	{
		public event Action<string> TrackSummary;

		public void Configure(IFlowControlLogConfig config, Server serverConfig)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("serverConfig", serverConfig);
			if (!serverConfig.FlowControlLogEnabled)
			{
				this.enabled = false;
				return;
			}
			if (serverConfig.FlowControlLogPath == null || string.IsNullOrEmpty(serverConfig.FlowControlLogPath.PathName))
			{
				this.enabled = false;
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Flow Control Log path was set to null, Flow Control Log is disabled");
				return;
			}
			FlowControlLog.schema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Transport Flow Control Log", FlowControlLog.Fields);
			this.log = new AsyncLog("FlowControlLog", new LogHeaderFormatter(FlowControlLog.schema), "FlowControlLog");
			this.log.Configure(serverConfig.FlowControlLogPath.PathName, serverConfig.FlowControlLogMaxAge, (long)(serverConfig.FlowControlLogMaxDirectorySize.IsUnlimited ? 0UL : serverConfig.FlowControlLogMaxDirectorySize.Value.ToBytes()), (long)(serverConfig.FlowControlLogMaxFileSize.IsUnlimited ? 0UL : serverConfig.FlowControlLogMaxFileSize.Value.ToBytes()), config.BufferSize, config.FlushInterval, config.AsyncInterval);
			this.enabled = true;
			this.LogReset();
			this.logSummaryTimer = new GuardedTimer(new TimerCallback(this.RaiseSummaryEvent), null, config.SummaryLoggingInterval, config.SummaryLoggingInterval);
		}

		public void Stop()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
			if (this.logSummaryTimer != null)
			{
				this.logSummaryTimer.Dispose(true);
			}
		}

		public void LogThrottle(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = ThrottlingEvent.Throttle;
			logRowFormatter[2] = this.GetNextSequenceNumber();
			logRowFormatter[3] = resource;
			logRowFormatter[4] = action;
			logRowFormatter[5] = threshold;
			logRowFormatter[6] = thresholdInterval;
			logRowFormatter[8] = scope;
			if (externalOrganizationId != Guid.Empty)
			{
				logRowFormatter[9] = externalOrganizationId;
			}
			logRowFormatter[10] = sender;
			logRowFormatter[11] = recipient;
			logRowFormatter[12] = subject;
			logRowFormatter[13] = source;
			logRowFormatter[14] = loggingMode;
			this.LogCustomData(logRowFormatter, extraData);
			this.Append(logRowFormatter);
		}

		public void LogUnthrottle(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, int impact, int observedValue, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = ThrottlingEvent.Unthrottle;
			logRowFormatter[2] = this.GetNextSequenceNumber();
			logRowFormatter[3] = resource;
			logRowFormatter[4] = action;
			logRowFormatter[5] = threshold;
			logRowFormatter[6] = thresholdInterval;
			logRowFormatter[7] = impact;
			logRowFormatter[8] = scope;
			if (externalOrganizationId != Guid.Empty)
			{
				logRowFormatter[9] = externalOrganizationId;
			}
			logRowFormatter[10] = sender;
			logRowFormatter[11] = recipient;
			logRowFormatter[12] = subject;
			logRowFormatter[13] = source;
			logRowFormatter[14] = loggingMode;
			this.LogCustomData(logRowFormatter, extraData, "observedValue", observedValue);
			this.Append(logRowFormatter);
		}

		public void LogSummary(string sequenceNumber, ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, int observedValue, int impact, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = ThrottlingEvent.SummaryThrottle;
			logRowFormatter[2] = sequenceNumber;
			logRowFormatter[3] = resource;
			logRowFormatter[4] = action;
			logRowFormatter[5] = threshold;
			logRowFormatter[6] = thresholdInterval;
			logRowFormatter[7] = impact;
			logRowFormatter[8] = scope;
			if (externalOrganizationId != Guid.Empty)
			{
				logRowFormatter[9] = externalOrganizationId;
			}
			logRowFormatter[10] = sender;
			logRowFormatter[11] = recipient;
			logRowFormatter[12] = subject;
			logRowFormatter[13] = source;
			logRowFormatter[14] = loggingMode;
			this.LogCustomData(logRowFormatter, extraData, "observedValue", observedValue);
			this.Append(logRowFormatter);
		}

		public void LogSummaryWarning(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			this.LogWarningInternal(ThrottlingEvent.SummaryWarning, resource, action, threshold, thresholdInterval, scope, externalOrganizationId, sender, recipient, subject, source, loggingMode, extraData);
		}

		public void LogWarning(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			this.LogWarningInternal(ThrottlingEvent.Warning, resource, action, threshold, thresholdInterval, scope, externalOrganizationId, sender, recipient, subject, source, loggingMode, extraData);
		}

		public void LogMaxLinesExceeded(string sequenceNumber, ThrottlingSource source, int threshold, int observedValue, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = ThrottlingEvent.SummaryThrottle;
			logRowFormatter[2] = sequenceNumber;
			logRowFormatter[3] = ThrottlingResource.MaxLinesReached;
			logRowFormatter[5] = threshold;
			this.LogCustomData(logRowFormatter, extraData, "observedValue", observedValue);
			this.Append(logRowFormatter);
		}

		private static string[] InitializeHeaders()
		{
			string[] array = new string[Enum.GetValues(typeof(FlowControlLog.Field)).Length];
			array[0] = "dateTime";
			array[1] = "eventId";
			array[2] = "sequenceNumber";
			array[3] = "resource";
			array[4] = "action";
			array[5] = "threshold";
			array[6] = "thresholdInterval";
			array[7] = "impact";
			array[9] = "organizationId";
			array[8] = "scope";
			array[10] = "sender";
			array[11] = "recipient";
			array[12] = "subject";
			array[13] = "source";
			array[14] = "loggingMode";
			array[15] = "customData";
			return array;
		}

		private void LogWarningInternal(ThrottlingEvent warningEventType, ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = warningEventType;
			logRowFormatter[2] = this.GetNextSequenceNumber();
			logRowFormatter[3] = resource;
			logRowFormatter[4] = action;
			logRowFormatter[5] = threshold;
			logRowFormatter[6] = thresholdInterval;
			logRowFormatter[8] = scope;
			if (externalOrganizationId != Guid.Empty)
			{
				logRowFormatter[9] = externalOrganizationId;
			}
			logRowFormatter[10] = sender;
			logRowFormatter[11] = recipient;
			logRowFormatter[12] = subject;
			logRowFormatter[13] = source;
			logRowFormatter[14] = loggingMode;
			this.LogCustomData(logRowFormatter, extraData);
			this.Append(logRowFormatter);
		}

		private void LogCustomData(LogRowFormatter row, IEnumerable<KeyValuePair<string, object>> extraData, string property, object value)
		{
			List<KeyValuePair<string, object>> first = new List<KeyValuePair<string, object>>(1)
			{
				new KeyValuePair<string, object>(property, value)
			};
			IEnumerable<KeyValuePair<string, object>> enumerable = first.Concat(this.defaultProperties);
			if (extraData != null)
			{
				enumerable = extraData.Concat(enumerable);
			}
			row[15] = enumerable;
		}

		private void LogCustomData(LogRowFormatter row, IEnumerable<KeyValuePair<string, object>> extraData)
		{
			IEnumerable<KeyValuePair<string, object>> value = this.defaultProperties;
			if (extraData != null)
			{
				value = extraData.Concat(this.defaultProperties);
			}
			row[15] = value;
		}

		private void LogReset()
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(FlowControlLog.schema);
			logRowFormatter[1] = ThrottlingEvent.Reset;
			this.Append(logRowFormatter);
		}

		private void Append(LogRowFormatter row)
		{
			try
			{
				this.log.Append(row, 0);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Appending to queue quota log failed with ObjectDisposedException");
			}
		}

		private void RaiseSummaryEvent(object state)
		{
			if (this.TrackSummary != null)
			{
				this.TrackSummary(this.GetNextSequenceNumber());
			}
		}

		private string GetNextSequenceNumber()
		{
			this.sequenceNumber += 1L;
			return this.sequenceNumber.ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		internal const string ObservedValue = "observedValue";

		internal const string ThrottledDuration = "throttledDuration";

		internal const string ScopeValue = "ScopeValue";

		private const string LogComponentName = "FlowControlLog";

		private const string LogFileName = "FlowControlLog";

		private const string AssemblyVersionName = "Version";

		private static readonly string[] Fields = FlowControlLog.InitializeHeaders();

		private static LogSchema schema;

		private AsyncLog log;

		private bool enabled;

		private long sequenceNumber = DateTime.UtcNow.Ticks;

		private GuardedTimer logSummaryTimer;

		private List<KeyValuePair<string, object>> defaultProperties = new List<KeyValuePair<string, object>>
		{
			new KeyValuePair<string, object>("Version", ExWatson.ApplicationVersion.ToString())
		};

		private enum Field
		{
			Time,
			EventId,
			SequenceNumber,
			Resource,
			Action,
			Threshold,
			ThresholdInterval,
			Impact,
			Scope,
			OrganizationId,
			Sender,
			Recipient,
			Subject,
			Source,
			LoggingMode,
			CustomData
		}
	}
}
