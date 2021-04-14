using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Microsoft.Exchange.Diagnostics.Components.SystemProbe;
using Microsoft.Exchange.Diagnostics.Internal;
using Microsoft.Exchange.SystemProbe;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class SystemProbe
	{
		public static Guid ActivityId
		{
			get
			{
				object obj = CallContext.LogicalGetData("SystemProbe.ActivityID");
				if (obj != null)
				{
					return (Guid)obj;
				}
				return Guid.Empty;
			}
			set
			{
				CallContext.LogicalSetData("SystemProbe.ActivityID", value);
			}
		}

		public static bool Enabled
		{
			get
			{
				return SystemProbe.enabled;
			}
		}

		internal static Log Log
		{
			get
			{
				return SystemProbe.log;
			}
		}

		internal static ExEventLog EventLogger
		{
			get
			{
				if (SystemProbe.eventLogger == null)
				{
					SystemProbe.eventLogger = new ExEventLog(ExTraceGlobals.ProbeTracer.Category, "FfoSystemProbe");
				}
				return SystemProbe.eventLogger;
			}
		}

		internal static bool IsTraceEnabled()
		{
			return SystemProbe.IsTraceEnabled(SystemProbe.ActivityId);
		}

		internal static bool IsTraceEnabled(Guid activityId)
		{
			return SystemProbe.enabled && SystemProbe.log != null && activityId != Guid.Empty && activityId.ToString().IndexOf("-0000-0000-0000-") == -1;
		}

		internal static bool IsTraceEnabled(ISystemProbeTraceable activityIdHolder)
		{
			return activityIdHolder != null && SystemProbe.IsTraceEnabled(activityIdHolder.SystemProbeId);
		}

		public static void Start()
		{
			SystemProbe.Start("SYSPRB", null);
		}

		public static void Start(string logFilePrefix, string logFileSubfolder = null)
		{
			SystemProbe.sysProbeSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "System Probe Log", SystemProbe.Fields);
			SystemProbe.log = new Log(logFilePrefix, new LogHeaderFormatter(SystemProbe.sysProbeSchema), "SystemProbeLogs");
			SystemProbe.startTime = DateTime.UtcNow;
			SystemProbe.stopwatch.Start();
			try
			{
				SystemProbe.hostName = Dns.GetHostName();
			}
			catch (SocketException)
			{
				SystemProbe.hostName = Environment.MachineName;
			}
			string text = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("SystemProbeLogPath");
					if (value != null)
					{
						text = value.ToString();
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeLogPathNotConfigured, null, new object[0]);
				return;
			}
			if (!string.IsNullOrEmpty(logFileSubfolder))
			{
				text = Path.Combine(text, logFileSubfolder);
			}
			SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeStarted, null, new object[0]);
			long num = 10485760L;
			long maxDirectorySize = 50L * num;
			int bufferSize = 4096;
			TimeSpan maxAge = TimeSpan.FromDays(5.0);
			TimeSpan streamFlushInterval = TimeSpan.FromSeconds(30.0);
			SystemProbe.log.Configure(text, maxAge, maxDirectorySize, num, bufferSize, streamFlushInterval);
			SystemProbe.enabled = true;
			SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeConfigured, null, new object[]
			{
				text
			});
		}

		public static void Stop()
		{
			if (SystemProbe.log != null)
			{
				SystemProbe.log.Close();
				SystemProbe.log = null;
			}
			SystemProbe.enabled = false;
			SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeStopped, null, new object[0]);
		}

		public static void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval)
		{
			SystemProbe.log.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, false, bufferSize, streamFlushInterval);
			SystemProbe.enabled = true;
			SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeConfigured, null, new object[]
			{
				path
			});
		}

		public static void FlushBuffer()
		{
			if (SystemProbe.log != null)
			{
				SystemProbe.log.Flush();
			}
		}

		public static void TracePass(string component, string formatString, params object[] args)
		{
			SystemProbe.Trace(component, SystemProbe.Status.Pass, formatString, args);
		}

		public static void TraceFail(string component, string formatString, params object[] args)
		{
			SystemProbe.Trace(component, SystemProbe.Status.Fail, formatString, args);
		}

		public static void Trace(string component, SystemProbe.Status status, string formatString, params object[] args)
		{
			if (!SystemProbe.IsTraceEnabled())
			{
				return;
			}
			SystemProbe.AddToLog(SystemProbe.ActivityId, component, status, formatString, args);
		}

		public static void TracePass(Guid activityId, string component, string message)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Pass, message, new object[0]);
		}

		public static void TracePass<T0>(Guid activityId, string component, string formatString, T0 arg0)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0
			});
		}

		public static void TracePass<T0, T1>(Guid activityId, string component, string formatString, T0 arg0, T1 arg1)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0,
				arg1
			});
		}

		public static void TracePass<T0, T1, T2>(Guid activityId, string component, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public static void TracePass(Guid activityId, string component, string formatString, params object[] args)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Pass, formatString, args);
		}

		public static void TraceFail(Guid activityId, string component, string message)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Fail, message, new object[0]);
		}

		public static void TraceFail<T0>(Guid activityId, string component, string formatString, T0 arg0)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0
			});
		}

		public static void TraceFail<T0, T1>(Guid activityId, string component, string formatString, T0 arg0, T1 arg1)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0,
				arg1
			});
		}

		public static void TraceFail<T0, T1, T2>(Guid activityId, string component, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public static void TraceFail(Guid activityId, string component, string formatString, params object[] args)
		{
			if (!SystemProbe.IsTraceEnabled(activityId))
			{
				return;
			}
			SystemProbe.AddToLog(activityId, component, SystemProbe.Status.Fail, formatString, args);
		}

		public static void TracePass(ISystemProbeTraceable activityIdHolder, string component, string message)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Pass, message, new object[0]);
		}

		public static void TracePass<T0>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0
			});
		}

		public static void TracePass<T0, T1>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0, T1 arg1)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0,
				arg1
			});
		}

		public static void TracePass<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Pass, formatString, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public static void TracePass(ISystemProbeTraceable activityIdHolder, string component, string formatString, params object[] args)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Pass, formatString, args);
		}

		public static void TraceFail(ISystemProbeTraceable activityIdHolder, string component, string message)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Fail, message, new object[0]);
		}

		public static void TraceFail<T0>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0
			});
		}

		public static void TraceFail<T0, T1>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0, T1 arg1)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0,
				arg1
			});
		}

		public static void TraceFail<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, string component, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Fail, formatString, new object[]
			{
				arg0,
				arg1,
				arg2
			});
		}

		public static void TraceFail(ISystemProbeTraceable activityIdHolder, string component, string formatString, params object[] args)
		{
			if (!SystemProbe.IsTraceEnabled(activityIdHolder))
			{
				return;
			}
			SystemProbe.AddToLog(activityIdHolder.SystemProbeId, component, SystemProbe.Status.Fail, formatString, args);
		}

		private static void AddToLog(Guid activityId, string component, SystemProbe.Status status, string formatString, params object[] args)
		{
			string value = string.Empty;
			try
			{
				value = string.Format(CultureInfo.InvariantCulture, formatString, args);
			}
			catch (ArgumentNullException)
			{
				SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeFormatArgumentNullException, component, new object[0]);
				return;
			}
			catch (FormatException ex)
			{
				SystemProbe.EventLogger.LogEvent(FfoSystemProbeEventLogConstants.Tuple_SystemProbeFormatException, component, new object[]
				{
					ex.Message
				});
				return;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("SysProbe", string.Empty));
			list.Add(new KeyValuePair<string, string>("Server", SystemProbe.hostName));
			list.Add(new KeyValuePair<string, string>("Component", component));
			list.Add(new KeyValuePair<string, string>("Status", status.ToString()));
			list.Add(new KeyValuePair<string, string>("Message", value));
			List<List<KeyValuePair<string, string>>> list2 = new List<List<KeyValuePair<string, string>>>();
			list2.Add(list);
			LogRowFormatter logRowFormatter = new LogRowFormatter(SystemProbe.sysProbeSchema);
			logRowFormatter[0] = (SystemProbe.startTime + SystemProbe.stopwatch.Elapsed).ToString("yyyy-MM-ddTHH\\:mm\\:ss.ffffffZ", DateTimeFormatInfo.InvariantInfo);
			logRowFormatter[10] = activityId;
			logRowFormatter[11] = activityId;
			logRowFormatter[8] = MessageTrackingEvent.SYSPROBEINFO;
			logRowFormatter[7] = MessageTrackingSource.AGENT;
			logRowFormatter[23] = SystemProbeConstants.TenantID;
			logRowFormatter[26] = LoggingFormatter.GetAgentInfoString(list2);
			logRowFormatter[12] = new string[]
			{
				"sysprb@contoso.com"
			};
			if (SystemProbe.log != null)
			{
				SystemProbe.log.Append(logRowFormatter, -1);
			}
		}

		private static string[] InitializeFields()
		{
			string[] array = new string[Enum.GetValues(typeof(SystemProbe.SysProbeField)).Length];
			array[0] = "date-time";
			array[1] = "client-ip";
			array[2] = "client-hostname";
			array[3] = "server-ip";
			array[4] = "server-hostname";
			array[5] = "source-context";
			array[6] = "connector-id";
			array[7] = "source";
			array[8] = "event-id";
			array[9] = "internal-message-id";
			array[10] = "message-id";
			array[11] = "network-message-id";
			array[12] = "recipient-address";
			array[13] = "recipient-status";
			array[14] = "total-bytes";
			array[15] = "recipient-count";
			array[16] = "related-recipient-address";
			array[17] = "reference";
			array[18] = "message-subject";
			array[19] = "sender-address";
			array[20] = "return-path";
			array[21] = "message-info";
			array[22] = "directionality";
			array[23] = "tenant-id";
			array[24] = "original-client-ip";
			array[25] = "original-server-ip";
			array[26] = "custom-data";
			return array;
		}

		private const string LogComponentName = "SystemProbeLogs";

		private const string LogPathRegistryKey = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string LogPathRegistryValue = "SystemProbeLogPath";

		private const string SysProbeRecipient = "sysprb@contoso.com";

		private static readonly string[] Fields = SystemProbe.InitializeFields();

		private static LogSchema sysProbeSchema;

		private static Log log;

		private static bool enabled;

		private static string hostName;

		private static ExEventLog eventLogger = null;

		private static DateTime startTime;

		private static Stopwatch stopwatch = new Stopwatch();

		private enum SysProbeField
		{
			Time,
			ClientIP,
			ClientHostName,
			ServerIP,
			ServerHostName,
			SourceContext,
			ConnectorId,
			Source,
			EventID,
			InternalMsgID,
			MessageID,
			NetworkMsgID,
			RecipientAddress,
			RecipStatus,
			TotalBytes,
			RecipientCount,
			RelatedRecipientAddress,
			Reference,
			Subject,
			Sender,
			ReturnPath,
			MessageInfo,
			Directionality,
			TenantID,
			OriginalClientIP,
			OriginalServerIP,
			CustomData
		}

		public enum Status
		{
			Pass,
			Fail
		}
	}
}
