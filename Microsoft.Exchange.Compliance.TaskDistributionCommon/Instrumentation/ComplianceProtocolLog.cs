using System;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Compliance;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation
{
	internal class ComplianceProtocolLog : DisposeTrackableBase
	{
		public ComplianceProtocolLog(string path) : this(path, ComplianceProtocolLog.DefaultLogFileMaxAge, ComplianceProtocolLog.DefaultMaxDirectorySize, ComplianceProtocolLog.DefaultMaxFileSize, ComplianceProtocolLog.DefaultMaxCacheSize, ComplianceProtocolLog.DefaultLogFlushInterval)
		{
		}

		protected ComplianceProtocolLog()
		{
			this.logLock = new object();
			this.logFileMaxAge = TimeSpan.FromDays(30.0);
			this.maxDirectorySize = 100;
			this.maxFileSize = 100;
			this.maxCacheSize = 10;
			this.logFlushInterval = TimeSpan.FromSeconds(10.0);
			base..ctor();
			this.logSchema = new LogSchema(this.SoftwareName, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.LogTypeName, ComplianceProtocolLog.GetColumnArray(ComplianceProtocolLog.Fields));
		}

		protected ComplianceProtocolLog(string path, int logFileMaxAge, int maxDirectorySize, int maxFileSize, int maxCacheSize, int logFlushInterval)
		{
			this.logLock = new object();
			this.logFileMaxAge = TimeSpan.FromDays(30.0);
			this.maxDirectorySize = 100;
			this.maxFileSize = 100;
			this.maxCacheSize = 10;
			this.logFlushInterval = TimeSpan.FromSeconds(10.0);
			base..ctor();
			this.logPath = path;
			this.logFileMaxAge = TimeSpan.FromDays((double)logFileMaxAge);
			this.maxDirectorySize = maxDirectorySize * 1024 * 1024;
			this.maxFileSize = maxFileSize * 1024 * 1024;
			this.maxCacheSize = maxCacheSize * 1024 * 1024;
			this.logFlushInterval = TimeSpan.FromSeconds((double)logFlushInterval);
			this.logSchema = new LogSchema(this.SoftwareName, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.LogTypeName, ComplianceProtocolLog.GetColumnArray(ComplianceProtocolLog.Fields));
			this.log = new Log(this.FileNamePrefixName, new LogHeaderFormatter(this.logSchema, true), this.LogComponentName);
			this.Configure();
		}

		internal string SoftwareName
		{
			get
			{
				return "Microsoft Exchange";
			}
		}

		internal string LogComponentName
		{
			get
			{
				return "ComplianceProtocol";
			}
		}

		internal string LogTypeName
		{
			get
			{
				return "Compliance Protocol Log";
			}
		}

		internal string FileNamePrefixName
		{
			get
			{
				return "ComplianceProtocol_";
			}
		}

		protected static ComplianceProtocolLog Instance
		{
			set
			{
				ComplianceProtocolLog.instance = value;
			}
		}

		public static void CreateInstance(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				ComplianceProtocolLog.instance = null;
				return;
			}
			ComplianceProtocolLog.instance = new ComplianceProtocolLog(path);
		}

		public static ComplianceProtocolLog GetInstance()
		{
			return ComplianceProtocolLog.instance;
		}

		public static void Trace(long id, TraceType traceType, object parameter1)
		{
			if (ComplianceProtocolLog.GetInstance() == null)
			{
				return;
			}
			if (ExTraceGlobals.TaskDistributionSystemTracer.IsTraceEnabled(traceType))
			{
				string formatString = "{0}";
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceDebug(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceWarning(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceError(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TracePerformance(id, formatString, new object[]
					{
						parameter1
					});
					return;
				}
				ExTraceGlobals.TaskDistributionSystemTracer.Information(id, formatString, new object[]
				{
					parameter1
				});
			}
		}

		public static void Trace(long id, TraceType traceType, object parameter1, object parameter2)
		{
			if (ComplianceProtocolLog.GetInstance() == null)
			{
				return;
			}
			if (ExTraceGlobals.TaskDistributionSystemTracer.IsTraceEnabled(traceType))
			{
				string formatString = "{0} {1}";
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceDebug(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceWarning(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceError(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TracePerformance(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				}
				ExTraceGlobals.TaskDistributionSystemTracer.Information(id, formatString, new object[]
				{
					parameter1,
					parameter2
				});
			}
		}

		public static void Trace(long id, TraceType traceType, params object[] parameters)
		{
			if (ComplianceProtocolLog.GetInstance() == null)
			{
				return;
			}
			if (parameters != null && parameters.Length > 0 && ExTraceGlobals.TaskDistributionSystemTracer.IsTraceEnabled(traceType))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < parameters.Length; i++)
				{
					stringBuilder.AppendFormat("{{{0}}}", i);
				}
				string formatString = stringBuilder.ToString();
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceDebug(id, formatString, parameters);
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceWarning(id, formatString, parameters);
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TraceError(id, formatString, parameters);
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.TaskDistributionSystemTracer.TracePerformance(id, formatString, parameters);
					return;
				}
				ExTraceGlobals.TaskDistributionSystemTracer.Information(id, formatString, parameters);
			}
		}

		public static void LogIncomingMessage(ComplianceMessage complianceMessage)
		{
			ComplianceProtocolLog complianceProtocolLog = ComplianceProtocolLog.GetInstance();
			if (complianceProtocolLog != null && complianceMessage != null)
			{
				complianceProtocolLog.Log(complianceMessage, true);
			}
		}

		public static void LogOutgoingMessage(ComplianceMessage complianceMessage)
		{
			ComplianceProtocolLog complianceProtocolLog = ComplianceProtocolLog.GetInstance();
			if (complianceProtocolLog != null && complianceMessage != null)
			{
				complianceProtocolLog.Log(complianceMessage, false);
			}
		}

		public void Log(ComplianceMessage complianceMessage, bool incoming)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[1] = (incoming ? "In" : "Out");
			logRowFormatter[2] = complianceMessage.CorrelationId.ToString();
			logRowFormatter[3] = ((complianceMessage.TenantId != null) ? this.ByteArrayToString(complianceMessage.TenantId) : "NA");
			logRowFormatter[4] = ((!string.IsNullOrEmpty(complianceMessage.MessageId)) ? complianceMessage.MessageId : "NA");
			logRowFormatter[5] = ((!string.IsNullOrEmpty(complianceMessage.MessageSourceId)) ? complianceMessage.MessageSourceId : "NA");
			logRowFormatter[6] = ((complianceMessage.MessageTarget != null) ? this.TargetToString(complianceMessage.MessageTarget) : "NA");
			logRowFormatter[7] = ((complianceMessage.MessageSource != null) ? this.TargetToString(complianceMessage.MessageSource) : "NA");
			logRowFormatter[8] = complianceMessage.ComplianceMessageType.ToString();
			logRowFormatter[9] = complianceMessage.WorkDefinitionType.ToString();
			logRowFormatter[10] = ((complianceMessage.Payload != null) ? complianceMessage.Payload.Length.ToString() : "NA");
			logRowFormatter[11] = "NA";
			logRowFormatter[12] = "NA";
			logRowFormatter[13] = "NA";
			logRowFormatter[14] = "NA";
			logRowFormatter[15] = "NA";
			this.AppendLogRow(logRowFormatter);
		}

		public void Configure()
		{
			if (!base.IsDisposed)
			{
				lock (this.logLock)
				{
					this.log.Configure(this.logPath, this.logFileMaxAge, (long)this.maxDirectorySize, (long)this.maxFileSize, this.maxCacheSize, this.logFlushInterval, true);
				}
			}
		}

		public void Close()
		{
			if (!base.IsDisposed && this.log != null)
			{
				this.log.Close();
				this.log = null;
			}
		}

		protected static string[] GetColumnArray(ComplianceProtocolLog.FieldInfo[] fields)
		{
			string[] array = new string[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = fields[i].ColumnName;
			}
			return array;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ComplianceProtocolLog>(this);
		}

		protected virtual void AppendLogRow(LogRowFormatter row)
		{
			if (!base.IsDisposed)
			{
				this.log.Append(row, 0);
			}
		}

		private string ByteArrayToString(byte[] bytes)
		{
			string text = BitConverter.ToString(bytes);
			return text.Replace("-", string.Empty);
		}

		private string TargetToString(Target target)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append(string.Format("T:{0} I:{1} S:{2} D:{3} M:{4} F:{5}", new object[]
			{
				target.TargetType.ToString(),
				string.IsNullOrEmpty(target.Identifier) ? string.Empty : target.Identifier,
				string.IsNullOrEmpty(target.Server) ? string.Empty : target.Server,
				(target.Database == Guid.Empty) ? string.Empty : target.Database.ToString(),
				(target.Mailbox == Guid.Empty) ? string.Empty : target.Mailbox.ToString(),
				string.IsNullOrEmpty(target.Folder) ? string.Empty : target.Folder
			}));
			return stringBuilder.ToString();
		}

		private const string DefaultData = "NA";

		internal static readonly ComplianceProtocolLog.FieldInfo[] Fields = new ComplianceProtocolLog.FieldInfo[]
		{
			new ComplianceProtocolLog.FieldInfo(0, "date-time"),
			new ComplianceProtocolLog.FieldInfo(1, "incoming"),
			new ComplianceProtocolLog.FieldInfo(2, "correlationid"),
			new ComplianceProtocolLog.FieldInfo(3, "tenantid"),
			new ComplianceProtocolLog.FieldInfo(4, "messageid"),
			new ComplianceProtocolLog.FieldInfo(5, "messagesourceid"),
			new ComplianceProtocolLog.FieldInfo(6, "messagetarget"),
			new ComplianceProtocolLog.FieldInfo(7, "messagesource"),
			new ComplianceProtocolLog.FieldInfo(8, "compliancemessagetype"),
			new ComplianceProtocolLog.FieldInfo(9, "workdefinitiontype"),
			new ComplianceProtocolLog.FieldInfo(10, "payloadsize"),
			new ComplianceProtocolLog.FieldInfo(11, "receivedtime"),
			new ComplianceProtocolLog.FieldInfo(12, "processedtime"),
			new ComplianceProtocolLog.FieldInfo(13, "queuedtime"),
			new ComplianceProtocolLog.FieldInfo(14, "genericinfo"),
			new ComplianceProtocolLog.FieldInfo(15, "fault")
		};

		private static readonly int DefaultLogFileMaxAge = 30;

		private static readonly int DefaultMaxDirectorySize = 100;

		private static readonly int DefaultMaxFileSize = 100;

		private static readonly int DefaultMaxCacheSize = 10;

		private static readonly int DefaultLogFlushInterval = 10;

		private static ComplianceProtocolLog instance = null;

		private readonly LogSchema logSchema;

		private readonly object logLock;

		private readonly TimeSpan logFileMaxAge;

		private readonly int maxDirectorySize;

		private readonly int maxFileSize;

		private readonly int maxCacheSize;

		private readonly TimeSpan logFlushInterval;

		private readonly string logPath;

		private Log log;

		protected enum Field : byte
		{
			DateTime,
			Incoming,
			CorrelationId,
			TenantId,
			MessageId,
			MessageSourceId,
			MessageTarget,
			MessageSource,
			ComplianceMessageType,
			WorkDefinitionType,
			PayloadSize,
			ReceivedTime,
			ProcessedTime,
			QueuedTime,
			GenericInfo,
			Fault
		}

		internal struct FieldInfo
		{
			public FieldInfo(byte field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly byte Field;

			internal readonly string ColumnName;
		}
	}
}
