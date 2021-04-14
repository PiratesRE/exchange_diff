using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class Storage
	{
		public static TaskList TaskList
		{
			get
			{
				return Storage.taskList;
			}
		}

		internal static void Initialize()
		{
			Storage.taskList = new TaskList();
		}

		public static void Terminate()
		{
			if (Storage.taskList != null)
			{
				Storage.taskList.Dispose();
				Storage.taskList = null;
			}
		}

		public static void SetExiting(bool exiting)
		{
			Storage.exiting = exiting;
		}

		public static StoreDatabase FindDatabase(Guid mdbGuid)
		{
			StoreDatabase result = null;
			if (!Storage.databases.TryGetValue(mdbGuid, out result))
			{
				return null;
			}
			return result;
		}

		public static ICollection<StoreDatabase> GetDatabaseListSnapshot()
		{
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3536203069U);
			}
			return Storage.databases.Values;
		}

		public static ErrorCode MountDatabase(Context context, StoreDatabase database, MountFlags flags)
		{
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ErrorCodeValue errorCodeValue = ErrorCode.NoError;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<ErrorCodeValue>(3569757501U, ref errorCodeValue);
				if (errorCodeValue != ErrorCode.NoError)
				{
					return ErrorCode.CreateWithLid((LID)49704U, errorCodeValue);
				}
			}
			bool flag = false;
			int num = 0;
			while (!flag && (long)num < 100L)
			{
				StoreDatabase storeDatabase;
				if (Storage.databases.TryGetValue(database.MdbGuid, out storeDatabase))
				{
					database = storeDatabase;
					flag = true;
				}
				else if (Storage.databases.TryAdd(database.MdbGuid, database))
				{
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				return ErrorCode.CreateCancel((LID)37816U);
			}
			bool flag2 = false;
			try
			{
				ErrorCode first = database.MountDatabase(context, flags, ref flag2);
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)54200U);
				}
			}
			finally
			{
				if (flag2)
				{
					Storage.databases.Remove(database.MdbGuid);
				}
			}
			if ((flags & MountFlags.LogReplay) == MountFlags.None)
			{
				IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.ReferenceData);
				if (logger != null && logger.IsLoggingEnabled)
				{
					bool boolValue = false;
					bool boolValue2 = false;
					bool boolValue3 = false;
					bool boolValue4 = false;
					bool boolValue5 = false;
					try
					{
						database.GetSharedLock();
						boolValue = database.IsMaintenance;
						boolValue2 = database.IsOnlineActive;
						boolValue3 = database.IsOnlinePassive;
						boolValue4 = database.IsPublic;
						boolValue5 = database.IsRecovery;
					}
					finally
					{
						database.ReleaseSharedLock();
					}
					using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.DatabaseInfo, true, false, database.MdbGuid.GetHashCode(), database.MdbName, database.GetCurrentSchemaVersion(context).Value, boolValue, boolValue2, boolValue3, boolValue4, boolValue5, database.ForestName))
					{
						logger.TryWrite(traceBuffer);
					}
				}
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode PurgeDatabaseCache(Context context, Guid mdbGuid)
		{
			StoreDatabase storeDatabase;
			if (!Storage.databases.TryGetValue(mdbGuid, out storeDatabase))
			{
				return ErrorCode.NoError;
			}
			storeDatabase.GetExclusiveLock();
			try
			{
				if (storeDatabase.IsOnlineActive || storeDatabase.IsOnlinePassive)
				{
					storeDatabase.ResetDatabaseEngine();
				}
			}
			finally
			{
				storeDatabase.ReleaseExclusiveLock();
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode ExtendDatabase(Context context, Guid mdbGuid)
		{
			StoreDatabase storeDatabase;
			if (!Storage.databases.TryGetValue(mdbGuid, out storeDatabase))
			{
				return ErrorCode.CreateNotFound((LID)56072U);
			}
			using (context.AssociateWithDatabaseExclusive(storeDatabase))
			{
				if (storeDatabase.IsOnlineActive)
				{
					storeDatabase.ExtendDatabase(context);
				}
				else if (storeDatabase.IsOnlinePassive)
				{
					return ErrorCode.CreateInvalidParameter((LID)61032U);
				}
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode ShrinkDatabase(Context context, Guid mdbGuid)
		{
			StoreDatabase storeDatabase;
			if (!Storage.databases.TryGetValue(mdbGuid, out storeDatabase))
			{
				return ErrorCode.CreateNotFound((LID)36456U);
			}
			using (context.AssociateWithDatabaseExclusive(storeDatabase))
			{
				if (storeDatabase.IsOnlineActive)
				{
					storeDatabase.ShrinkDatabase(context);
				}
				else if (storeDatabase.IsOnlinePassive)
				{
					return ErrorCode.CreateInvalidParameter((LID)52840U);
				}
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode VersionStoreCleanup(Context context, Guid mdbGuid)
		{
			StoreDatabase storeDatabase;
			if (!Storage.databases.TryGetValue(mdbGuid, out storeDatabase))
			{
				return ErrorCode.CreateNotFound((LID)49612U);
			}
			using (context.AssociateWithDatabaseExclusive(storeDatabase))
			{
				if (storeDatabase.IsOnlineActive)
				{
					storeDatabase.VersionStoreCleanup(context);
				}
				else if (storeDatabase.IsOnlinePassive)
				{
					return ErrorCode.CreateInvalidParameter((LID)35836U);
				}
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode DismountDatabase(Context context, Guid mdbGuid)
		{
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2496015677U);
			}
			StoreDatabase storeDatabase;
			if (!Storage.databases.TryGetValue(mdbGuid, out storeDatabase))
			{
				return ErrorCode.CreateNotFound((LID)64968U);
			}
			ErrorCode errorCode = storeDatabase.DismountDatabase(context);
			if (errorCode != ErrorCode.NoError)
			{
				storeDatabase.DismountError = errorCode;
				return errorCode.Propagate((LID)41912U);
			}
			Storage.databases.Remove(storeDatabase.MdbGuid);
			return ErrorCode.NoError;
		}

		public static void DismountAllDatabases()
		{
			Storage.ForEachDatabaseExecuteAsyncAndWait(TaskTypeId.DismountDatabase, delegate(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue)
			{
				Storage.DismountDatabase(context, database.MdbGuid);
			});
		}

		public static bool WhileNotExiting()
		{
			return !Storage.exiting;
		}

		public static void ForEachDatabase(Context context, Storage.DatabaseEnumerationCallback enumCallback)
		{
			Storage.ForEachDatabase(context, true, enumCallback);
		}

		public static void ForEachDatabase(Context context, bool activeDatabasesOnly, Storage.DatabaseEnumerationCallback enumCallback)
		{
			foreach (StoreDatabase storeDatabase in Storage.GetDatabaseListSnapshot())
			{
				if (Storage.exiting)
				{
					break;
				}
				using (context.AssociateWithDatabase(storeDatabase))
				{
					if (storeDatabase.IsOnlineActive || !activeDatabasesOnly)
					{
						enumCallback(context, storeDatabase, new Func<bool>(Storage.WhileNotExiting));
					}
				}
			}
		}

		public static void ForEachDatabaseExecuteAsyncAndWait(TaskTypeId taskTypeId, TaskExecutionWrapper<StoreDatabase>.TaskCallback<Context> enumCallback)
		{
			using (TaskList taskList = new TaskList())
			{
				foreach (StoreDatabase storeDatabase in Storage.GetDatabaseListSnapshot())
				{
					SingleExecutionTask<StoreDatabase>.CreateSingleExecutionTask(taskList, TaskExecutionWrapper<StoreDatabase>.WrapExecute(new TaskDiagnosticInformation(taskTypeId, ClientType.System, storeDatabase.MdbGuid), enumCallback), storeDatabase, true);
				}
				taskList.WaitAndShutdown();
			}
		}

		private static bool exiting;

		private static TaskList taskList;

		private static LockFreeDictionary<Guid, StoreDatabase> databases = new LockFreeDictionary<Guid, StoreDatabase>();

		public delegate void DatabaseEnumerationCallback(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue);
	}
}
