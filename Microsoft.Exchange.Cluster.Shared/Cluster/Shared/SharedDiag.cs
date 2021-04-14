using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class SharedDiag
	{
		public static TimeSpan DefaultEventSuppressionInterval
		{
			get
			{
				return SharedDiag.s_defaultEventSuppressionInterval;
			}
		}

		private static ExEventLog EventLog
		{
			get
			{
				return SharedDiag.s_eventLog;
			}
		}

		public static void TestResetFromDependencies()
		{
			SharedDiag.s_eventLog = SharedDependencies.DiagCoreImpl.EventLog;
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
			SharedDependencies.AssertRtl(condition, formatString, parameters);
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
			return string.Format(SharedDiag.s_exceptionMessageWithHrFormatString, ex.Message, Marshal.GetHRForException(ex));
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			bool flag;
			tuple.LogEvent(periodicKey, out flag, messageArgs);
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, string periodicKey, out bool fEventSuppressed, params object[] messageArgs)
		{
			SharedDiag.EventLog.LogEvent(tuple, periodicKey, out fEventSuppressed, messageArgs);
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
						SharedDiag.GetEventViewerEventId(tuple),
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
			if (SharedDiag.s_clusmsgFullPath == null)
			{
				SharedDiag.s_clusmsgFullPath = Path.Combine(SharedDiag.GetFolderPathOfExchangeBinaries(), "clusmsg.dll");
			}
			if (!SharedDiag.FormatMessageFromModule(SharedDiag.s_clusmsgFullPath, eventTuple.EventId, out result, out gle, arguments))
			{
				return null;
			}
			return result;
		}

		internal static string DbMsgEventTupleToString(ExEventLog.EventTuple eventTuple, params object[] arguments)
		{
			string result = null;
			if (SharedDiag.s_exdbmsgFullPath == null)
			{
				SharedDiag.s_exdbmsgFullPath = Path.Combine(SharedDiag.GetFolderPathOfExchangeBinaries(), "exdbmsg.dll");
			}
			int num;
			if (!SharedDiag.FormatMessageFromModule(SharedDiag.s_exdbmsgFullPath, eventTuple.EventId, out result, out num, arguments))
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
			return SharedDiag.FormatMessageFromModule(moduleName, msgId, out msg, out num, arguments);
		}

		internal static bool FormatMessageFromModule(string moduleName, uint msgId, out string msg, out int gle, params object[] objArguments)
		{
			string[] array = null;
			if (objArguments != null && objArguments.Length > 0)
			{
				array = new string[objArguments.Length];
				int num = 0;
				foreach (object obj in objArguments)
				{
					string text = string.Empty;
					if (obj != null)
					{
						text = obj.ToString();
					}
					array[num++] = text;
				}
			}
			bool result = false;
			msg = null;
			gle = 0;
			ModuleHandle moduleHandle = NativeMethods.LoadLibraryEx(moduleName, IntPtr.Zero, 2U);
			if (moduleHandle == null || moduleHandle.IsInvalid)
			{
				gle = Marshal.GetLastWin32Error();
				ExTraceGlobals.ReplayApiTracer.TraceError<string, uint, int>(0L, "Failed to LoadLibraryEx( {0} ) when interpreting {1}. Error code: {2}.", moduleName, msgId, gle);
				return false;
			}
			IntPtr[] array2 = null;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				uint dwFlags;
				if (array != null)
				{
					int num2 = array.Length;
					array2 = new IntPtr[num2];
					for (int j = 0; j < num2; j++)
					{
						array2[j] = Marshal.StringToHGlobalUni(array[j]);
					}
					intPtr = Marshal.AllocHGlobal(num2 * IntPtr.Size);
					Marshal.Copy(array2, 0, intPtr, num2);
					dwFlags = 10496U;
				}
				else
				{
					dwFlags = 2816U;
				}
				IntPtr zero = IntPtr.Zero;
				uint num3 = NativeMethods.FormatMessage(dwFlags, moduleHandle, msgId, 0U, ref zero, 0U, intPtr);
				if (num3 != 0U && zero != IntPtr.Zero)
				{
					msg = Marshal.PtrToStringUni(zero);
					Marshal.FreeHGlobal(zero);
					result = true;
					ExTraceGlobals.ReplayApiTracer.TraceDebug<uint, string>(0L, "FormatMessage( {0} ) was successful. Message: {1}.", msgId, msg);
				}
				else
				{
					gle = Marshal.GetLastWin32Error();
					ExTraceGlobals.ReplayApiTracer.TraceError<uint, int>(0L, "FormatMessage( {0} ) failed with GLE = {1} .", msgId, gle);
				}
			}
			finally
			{
				if (array2 != null)
				{
					for (int k = 0; k < array2.Length; k++)
					{
						Marshal.FreeHGlobal(array2[k]);
					}
					Marshal.FreeHGlobal(intPtr);
				}
				moduleHandle.Close();
			}
			return result;
		}

		internal static int GetThreadId()
		{
			return DiagnosticsNativeMethods.GetCurrentThreadId();
		}

		private static TimeSpan s_defaultEventSuppressionInterval = TimeSpan.FromSeconds((double)RegistryParameters.CrimsonPeriodicLoggingIntervalInSec);

		private static ExEventLog s_eventLog = SharedDependencies.DiagCoreImpl.EventLog;

		private static string s_clusmsgFullPath = null;

		private static string s_exceptionMessageWithHrFormatString = "{0} [HResult: 0x{1:x}].";

		private static string s_exdbmsgFullPath = null;
	}
}
