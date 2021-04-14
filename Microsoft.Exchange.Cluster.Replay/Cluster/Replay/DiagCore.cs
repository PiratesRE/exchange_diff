using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class DiagCore
	{
		public static TimeSpan DefaultEventSuppressionInterval
		{
			get
			{
				return DiagCore.s_defaultEventSuppressionInterval;
			}
		}

		private static ExEventLog EventLog
		{
			get
			{
				return DiagCore.s_eventLog;
			}
		}

		public static ExEventLog TestGetEventLog()
		{
			return DiagCore.s_eventLog;
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString, params object[] parameters)
		{
		}

		public static void RetailAssert(bool condition, string formatString, params object[] parameters)
		{
			Dependencies.AssertRtl(condition, formatString, parameters);
		}

		public static void AssertOrWatson(bool condition, string formatString, params object[] parameters)
		{
			if (condition)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder("ASSERT_WATSON: ");
			if (formatString != null)
			{
				if (parameters != null)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, formatString, parameters);
				}
				else
				{
					stringBuilder.Append(formatString);
				}
			}
			string text = stringBuilder.ToString();
			ExTraceGlobals.ReplayApiTracer.TraceError<string>(0L, "AssertOrWatson: Sending Watson report. {0}", text);
			Exception exception = null;
			try
			{
				throw new ExAssertException(text);
			}
			catch (ExAssertException ex)
			{
				exception = ex;
			}
			ExWatson.SendReport(exception, ReportOptions.None, "AssertOrWatson");
		}

		public static string GetMessageWithHResult(this Exception ex)
		{
			return string.Format(DiagCore.s_exceptionMessageWithHrFormatString, ex.Message, Marshal.GetHRForException(ex));
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			bool flag;
			tuple.LogEvent(periodicKey, out flag, messageArgs);
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			DiagCore.EventLog.LogEvent(tuple, periodicKey, out fEventSuppressed, messageArgs);
			if (!fEventSuppressed)
			{
				string text = tuple.EventLogToString(messageArgs);
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = string.Empty;
					if (messageArgs.Length > 0)
					{
						text2 = string.Join(",\n", messageArgs);
					}
					EventLogEntryType entryType = tuple.EntryType;
					ReplayCrimsonEvent replayCrimsonEvent;
					switch (entryType)
					{
					case EventLogEntryType.Error:
						goto IL_7C;
					case EventLogEntryType.Warning:
						replayCrimsonEvent = ReplayCrimsonEvents.AppLogMirrorWarning;
						goto IL_8A;
					case (EventLogEntryType)3:
						goto IL_84;
					case EventLogEntryType.Information:
						break;
					default:
						if (entryType != EventLogEntryType.SuccessAudit)
						{
							if (entryType != EventLogEntryType.FailureAudit)
							{
								goto IL_84;
							}
							goto IL_7C;
						}
						break;
					}
					replayCrimsonEvent = ReplayCrimsonEvents.AppLogMirrorInformational;
					goto IL_8A;
					IL_7C:
					replayCrimsonEvent = ReplayCrimsonEvents.AppLogMirrorError;
					goto IL_8A;
					IL_84:
					replayCrimsonEvent = ReplayCrimsonEvents.AppLogMirrorInformational;
					IL_8A:
					replayCrimsonEvent.LogGeneric(new object[]
					{
						DiagCore.GetEventViewerEventId(tuple),
						text,
						text2
					});
				}
			}
		}

		internal static string EventLogToString(this ExEventLog.EventTuple eventTuple, params object[] arguments)
		{
			int num;
			return eventTuple.EventLogToString(out num, arguments);
		}

		internal static string EventLogToString(this ExEventLog.EventTuple eventTuple, out int gle, params object[] arguments)
		{
			string result = null;
			if (DiagCore.s_clusmsgFullPath == null)
			{
				DiagCore.s_clusmsgFullPath = Path.Combine(DiagCore.GetFolderPathOfExchangeBinaries(), "clusmsg.dll");
			}
			if (!DiagCore.FormatMessageFromModule(DiagCore.s_clusmsgFullPath, eventTuple.EventId, out result, out gle, arguments))
			{
				return null;
			}
			return result;
		}

		internal static string DbMsgEventTupleToString(ExEventLog.EventTuple eventTuple, params object[] arguments)
		{
			string result = null;
			if (DiagCore.s_exdbmsgFullPath == null)
			{
				DiagCore.s_exdbmsgFullPath = Path.Combine(DiagCore.GetFolderPathOfExchangeBinaries(), "exdbmsg.dll");
			}
			int num;
			if (!DiagCore.FormatMessageFromModule(DiagCore.s_exdbmsgFullPath, eventTuple.EventId, out result, out num, arguments))
			{
				return null;
			}
			return result;
		}

		internal static string GetFolderPathOfExchangeBinaries()
		{
			string result = null;
			try
			{
				result = ExchangeSetupContext.BinPath;
			}
			catch (SetupVersionInformationCorruptException)
			{
				result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			return result;
		}

		internal static uint GetEventViewerEventId(ExEventLog.EventTuple eventTuple)
		{
			return eventTuple.EventId & 65535U;
		}

		internal static bool FormatMessageFromModule(string moduleName, uint msgId, out string msg, params object[] arguments)
		{
			int num;
			return DiagCore.FormatMessageFromModule(moduleName, msgId, out msg, out num, arguments);
		}

		internal static bool FormatMessageFromModule(string moduleName, uint msgId, out string msg, out int gle, params object[] objArguments)
		{
			return SharedDiag.FormatMessageFromModule(moduleName, msgId, out msg, out gle, objArguments);
		}

		internal static int GetThreadId()
		{
			return DiagnosticsNativeMethods.GetCurrentThreadId();
		}

		private static TimeSpan s_defaultEventSuppressionInterval = TimeSpan.FromSeconds((double)RegistryParameters.CrimsonPeriodicLoggingIntervalInSec);

		private static ExEventLog s_eventLog = new ExEventLog(ExTraceGlobals.ReplayApiTracer.Category, "MSExchangeRepl");

		private static string s_clusmsgFullPath = null;

		private static string s_exceptionMessageWithHrFormatString = "{0} [HResult: 0x{1:x}].";

		private static string s_exdbmsgFullPath = null;
	}
}
