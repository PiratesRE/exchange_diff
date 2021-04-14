using System;
using System.Diagnostics;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class Globals
	{
		public static ExEventLog EventLog
		{
			get
			{
				return Globals.eventLog;
			}
		}

		public static bool IsMultiProcess
		{
			get
			{
				return Globals.isMultiProcess;
			}
		}

		public static void Initialize(Guid? databaseGuid, Guid? dagOrServerGuid)
		{
			LockManager.Initialize();
			WatsonOnUnhandledException.Initialize();
			ErrorHelper.Initialize(databaseGuid);
			Globals.isMultiProcess = (databaseGuid != null);
			Globals.databaseGuid = databaseGuid;
			ConfigurationSchema.SetDatabaseContext(databaseGuid, dagOrServerGuid);
			ThreadManager.Initialize();
			TempStream.Configure(null);
			Statistics.Initialize();
			NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(null);
		}

		public static void Terminate()
		{
			Statistics.Terminate();
			LoggerManager.Terminate();
			ThreadManager.Terminate();
		}

		[Conditional("DEBUG")]
		internal static void Assert(bool assertCondition, string message)
		{
		}

		[Conditional("INTERNAL")]
		internal static void AssertInternal(bool assertCondition, string message)
		{
			ErrorHelper.AssertRetail(assertCondition, message);
		}

		internal static void AssertRetail(bool assertCondition, string message)
		{
			ErrorHelper.AssertRetail(assertCondition, message);
		}

		internal static void LogEvent(ExEventLog.EventTuple tuple, params object[] messageArgs)
		{
			if (!Globals.testOnlyEvents.Contains(tuple.EventId) || StoreEnvironment.IsTestEnvironment)
			{
				Globals.TracePIDAndDatabaseGuid(tuple.EventId);
				Globals.LogPeriodicEvent(null, tuple, messageArgs);
			}
		}

		private static void TracePIDAndDatabaseGuid(uint eventId)
		{
			if (Globals.tracedForThisEventId != eventId)
			{
				Globals.tracedForThisEventId = eventId;
				if (Globals.databaseGuid != null)
				{
					DiagnosticContext.TraceGuid((LID)41344U, Globals.databaseGuid.GetValueOrDefault());
				}
				DiagnosticContext.TraceDword((LID)35200U, (uint)DiagnosticsNativeMethods.GetCurrentProcessId());
			}
		}

		internal static void LogPeriodicEvent(string periodicKey, ExEventLog.EventTuple tuple, params object[] messageArgs)
		{
			byte[] array = DiagnosticContext.PackInfo();
			byte[] array2 = new byte["[DIAG_CTX]".Length + 2 + 4 + array.Length];
			int num = 0;
			num += ExBitConverter.Write("[DIAG_CTX]", "[DIAG_CTX]".Length, false, array2, num) - 1;
			num += ExBitConverter.Write(0, array2, num);
			num += ExBitConverter.Write(DiagnosticContext.Size + 14, array2, num);
			Buffer.BlockCopy(array, 0, array2, num, array.Length);
			Globals.eventLog.LogEventWithExtraData(tuple, periodicKey, array2, messageArgs);
		}

		public static readonly Guid EventLogGuid = new Guid("e1cda82c-0202-4901-8dfb-7b1993298b60");

		private static readonly ExEventLog eventLog = new ExEventLog(Globals.EventLogGuid, "MSExchangeIS");

		private static bool isMultiProcess;

		private static Guid? databaseGuid = null;

		private static uint tracedForThisEventId = 0U;

		private static HashSet<uint> testOnlyEvents = new HashSet<uint>
		{
			MSExchangeISEventLogConstants.Tuple_InvalidMailboxGlobcntAllocation.EventId,
			MSExchangeISEventLogConstants.Tuple_LeakedException.EventId,
			MSExchangeISEventLogConstants.Tuple_PropertyPromotion.EventId
		};
	}
}
