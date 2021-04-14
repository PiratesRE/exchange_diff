using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class TraceFileLogger
	{
		internal TraceFileLogger(bool forcedLogging)
		{
			this.isForcedLogging = forcedLogging;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.processName = currentProcess.MainModule.ModuleName;
				this.processId = currentProcess.Id;
			}
			this.isProcessAllowedInDatacenter = !this.processName.Contains("MSExchangeMailboxAssistants");
		}

		internal void TraceDebug(object context, string callId, string message)
		{
			this.TraceToLog(TraceFileLogger.LogLevel.Debug, context, callId, message);
		}

		internal void TraceError(object context, string callId, string message)
		{
			this.TraceToLog(TraceFileLogger.LogLevel.Error, context, callId, message);
		}

		internal void TracePfd(object context, string callId, string message)
		{
			this.TraceToLog(TraceFileLogger.LogLevel.Pfd, context, callId, message);
		}

		internal void TraceWarning(object context, string callId, string message)
		{
			this.TraceToLog(TraceFileLogger.LogLevel.Warning, context, callId, message);
		}

		internal void TracePerformance(object context, string callId, string message)
		{
			this.TraceToLog(TraceFileLogger.LogLevel.Performance, context, callId, message);
		}

		internal void Flush()
		{
			if (this.log != null)
			{
				this.log.Flush();
			}
		}

		private void LazyInitialize()
		{
			if (this.log == null)
			{
				lock (this.lockObject)
				{
					if (this.log == null)
					{
						this.logSchema = new LogSchema("Microsoft Exchange Server", CommonConstants.ApplicationVersion, "Unified Messaging Trace", this.logFields);
						this.log = new Log(this.processName + "-", new LogHeaderFormatter(this.logSchema, LogHeaderCsvOption.CsvCompatible), "UnifiedMessaging");
						this.log.Configure(Path.Combine(Utils.GetExchangeDirectory(), "Logging\\UMLogs"), this.LogMaxAge, 5368709120L, 10485760L, 1048576, this.LogFlushInterval);
					}
				}
			}
		}

		private void TraceToLog(TraceFileLogger.LogLevel level, object context, string callId, string message)
		{
			if ((VariantConfiguration.InvariantNoFlightingSnapshot.UM.AlwaysLogTraces.Enabled && this.isProcessAllowedInDatacenter) || this.isForcedLogging)
			{
				this.LazyInitialize();
				LogRowFormatter row = new LogRowFormatter(this.logSchema);
				row[1] = Interlocked.Increment(ref this.sequenceNumber);
				row[2] = this.processName;
				row[3] = level;
				row[4] = this.processId;
				row[5] = Thread.CurrentThread.ManagedThreadId;
				row[6] = ((context == null) ? "" : string.Concat(context.GetHashCode()));
				row[7] = ((callId == null) ? "" : callId);
				row[8] = message;
				Task.Factory.StartNew(delegate()
				{
					this.log.Append(row, 0);
				}, CancellationToken.None, TaskCreationOptions.PreferFairness, this.logSchedulerPair.ExclusiveScheduler);
			}
		}

		private const string SoftwareName = "Microsoft Exchange Server";

		private const string LogTypeName = "Unified Messaging Trace";

		private const string LogComponent = "UnifiedMessaging";

		private const string LogPath = "Logging\\UMLogs";

		private const long LogMaxDirectorySize = 5368709120L;

		private const long LogMaxFileSize = 10485760L;

		private const int LogBufferSize = 1048576;

		private readonly TimeSpan LogMaxAge = TimeSpan.FromDays(14.0);

		private readonly TimeSpan LogFlushInterval = TimeSpan.FromSeconds(30.0);

		private readonly string[] logFields = new string[]
		{
			"Timestamp",
			"SequenceNumber",
			"ProcessName",
			"Level",
			"ProcessId",
			"ThreadId",
			"Context",
			"CallId",
			"Message"
		};

		private Log log;

		private LogSchema logSchema;

		private readonly string processName;

		private readonly int processId;

		private long sequenceNumber;

		private readonly bool isForcedLogging;

		private readonly bool isProcessAllowedInDatacenter;

		private object lockObject = new object();

		private ConcurrentExclusiveSchedulerPair logSchedulerPair = new ConcurrentExclusiveSchedulerPair();

		private enum LogField
		{
			Timestamp,
			SequenceNumber,
			ProcessName,
			Level,
			ProcessId,
			ThreadId,
			Context,
			CallId,
			Message
		}

		private enum LogLevel
		{
			Pfd,
			Debug,
			Warning,
			Error,
			Performance
		}
	}
}
