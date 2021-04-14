using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MaintenanceHandler
	{
		private MaintenanceHandler()
		{
			this.databaseMaintenanceStates = new MaintenanceHandler.MaintenanceState[MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count];
			this.databaseMaintenanceInProgress = new int[MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count];
		}

		public static bool AsyncMaintenanceSchedulingEnabled
		{
			get
			{
				return MaintenanceHandler.asyncMaintenanceSchedulingEnabled;
			}
			set
			{
				MaintenanceHandler.asyncMaintenanceSchedulingEnabled = value;
			}
		}

		public static TimeSpan MailboxLockTimeout
		{
			get
			{
				return MaintenanceHandler.mailboxLockTimeout;
			}
		}

		private static MaintenanceHandler.RegistrationStateObject RegistrationState
		{
			get
			{
				return MaintenanceHandler.registrationStateHookable.Value;
			}
		}

		public static IDatabaseMaintenance RegisterDatabaseMaintenance(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, MaintenanceHandler.DatabaseMaintenanceDelegate databaseMaintenanceDelegate, string maintenanceTaskName)
		{
			return MaintenanceHandler.RegisterDatabaseMaintenance(maintenanceId, requiredMaintenanceResourceType, databaseMaintenanceDelegate, maintenanceTaskName, 1);
		}

		public static IDatabaseMaintenance RegisterDatabaseMaintenance(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, MaintenanceHandler.DatabaseMaintenanceDelegate databaseMaintenanceDelegate, string maintenanceTaskName, int numberOfBatchesToSchedule)
		{
			MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition = new MaintenanceHandler.DatabaseMaintenanceTaskDefinition(maintenanceId, MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count, requiredMaintenanceResourceType, databaseMaintenanceDelegate, maintenanceTaskName, numberOfBatchesToSchedule);
			MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Add(databaseMaintenanceTaskDefinition);
			return databaseMaintenanceTaskDefinition;
		}

		public static IMailboxMaintenance RegisterMailboxMaintenance(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, bool checkMailboxIsIdle, MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate, string maintenanceTaskName)
		{
			return MaintenanceHandler.RegisterMailboxMaintenance(maintenanceId, requiredMaintenanceResourceType, checkMailboxIsIdle, mailboxMaintenanceDelegate, maintenanceTaskName, false);
		}

		public static IMailboxMaintenance RegisterMailboxMaintenance(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, bool checkMailboxIsIdle, MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate, string maintenanceTaskName, bool isFinal)
		{
			MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition = new MaintenanceHandler.MailboxMaintenanceTaskDefinition(maintenanceId, MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions.Count, requiredMaintenanceResourceType, checkMailboxIsIdle, mailboxMaintenanceDelegate, maintenanceTaskName, isFinal);
			MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions.Add(mailboxMaintenanceTaskDefinition);
			return mailboxMaintenanceTaskDefinition;
		}

		public static void DoDatabaseMaintenance(Context context, DatabaseInfo databaseInfo, Guid maintenanceId)
		{
			MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition = MaintenanceHandler.FindDatabaseMaintenanceDefinition(maintenanceId);
			if (databaseMaintenanceTaskDefinition == null)
			{
				throw new StoreException((LID)56584U, ErrorCodeValue.NotFound, "Invalid database maintenance ID");
			}
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			if (Interlocked.CompareExchange(ref maintenanceHandler.databaseMaintenanceInProgress[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex], 1, 0) != 0)
			{
				if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid>(52160L, "DoDatabaseMaintenance({0}) for database {1} is skipped because same maintenance is currently in progress.", databaseMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid);
				}
				return;
			}
			try
			{
				if (!AssistantActivityMonitor.Instance(context.Database)[databaseMaintenanceTaskDefinition.RequiredMaintenanceResourceType].AssistantIsActiveInLastMonitoringPeriod)
				{
					AssistantActivityMonitor.PublishActiveMonitoringNotification(databaseMaintenanceTaskDefinition.RequiredMaintenanceResourceType, context.Database.MdbName, ResultSeverityLevel.Informational);
				}
				AssistantActivityMonitor.Instance(context.Database).UpdateAssistantActivityState(databaseMaintenanceTaskDefinition.RequiredMaintenanceResourceType, false);
				if (!maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired)
				{
					if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid>(33208L, "DoDatabaseMaintenance({0}) for database {1} called, but maintenance is not marked as required.", databaseMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid);
					}
					DiagnosticContext.TraceLocation((LID)60680U);
				}
				else if (MaintenanceHandler.ShouldStopDatabaseMaintenanceTask(context, maintenanceId))
				{
					if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid>(0L, "DoDatabaseMaintenance({0}) for database {1} called, but database maintenance tasks are not permitted to proceed, so the task is being pre-empted.", databaseMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid);
					}
					DiagnosticContext.TraceLocation((LID)52960U);
				}
				else
				{
					context.PerfInstance.RateOfDatabaseMaintenances.Increment();
					bool flag;
					try
					{
						maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex].LastExecutionStarted = DateTime.UtcNow;
						databaseMaintenanceTaskDefinition.DatabaseMaintenanceDelegate(context, databaseInfo, out flag);
					}
					finally
					{
						maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex].LastExecutionFinished = DateTime.UtcNow;
					}
					if (flag)
					{
						maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired = false;
					}
					if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, bool>(49080L, "DoDatabaseMaintenance({0}) for database {1} finished, completed={2}.", databaseMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, flag);
					}
				}
			}
			finally
			{
				Interlocked.CompareExchange(ref maintenanceHandler.databaseMaintenanceInProgress[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex], 0, 1);
			}
		}

		public static string GetDatabaseMaintenanceTaskName(Guid maintenanceId)
		{
			MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition = MaintenanceHandler.FindDatabaseMaintenanceDefinition(maintenanceId);
			if (databaseMaintenanceTaskDefinition == null)
			{
				return null;
			}
			return databaseMaintenanceTaskDefinition.MaintenanceTaskName;
		}

		public static void DoMailboxMaintenance(Context context, Guid maintenanceId, int mailboxNumber)
		{
			MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition = MaintenanceHandler.FindMailboxMaintenanceDefinition(maintenanceId);
			if (mailboxMaintenanceTaskDefinition == null)
			{
				throw new StoreException((LID)44296U, ErrorCodeValue.NotFound, "Invalid mailbox maintenance ID");
			}
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			AssistantActivityMonitor.Instance(context.Database).UpdateAssistantActivityState(mailboxMaintenanceTaskDefinition.RequiredMaintenanceResourceType, false);
			context.InitializeMailboxExclusiveOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.MailboxMaintenance, mailboxMaintenanceTaskDefinition.CheckMailboxIsIdle ? TimeSpan.Zero : MaintenanceHandler.mailboxLockTimeout);
			bool commit = false;
			try
			{
				ErrorCode errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, true, true);
				if (errorCode != ErrorCode.NoError)
				{
					if (errorCode != ErrorCodeValue.Timeout)
					{
						if (errorCode == ErrorCodeValue.NotFound)
						{
							errorCode = ErrorCode.CreateUnknownMailbox((LID)40700U);
						}
						if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(49592L, "DoMailboxMaintenance({0}) for database {1} called with non existing mailbox. MailboxNumber={2}.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
						}
						throw new StoreException((LID)36104U, errorCode, "StartMailboxOperation failed");
					}
					if (mailboxMaintenanceTaskDefinition.CheckMailboxIsIdle)
					{
						DiagnosticContext.TraceLocation((LID)42844U);
						if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(49592L, "DoMailboxMaintenance({0}) for database {1}, MailboxNumber={2} skipped because mailbox is not idle.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
						}
					}
					else
					{
						DiagnosticContext.TraceLocation((LID)34652U);
						if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(44892L, "DoMailboxMaintenance({0}) for database {1}, MailboxNumber={2} skipped because mailbox lock contention.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
						}
					}
				}
				else
				{
					bool flag = false;
					MaintenanceHandler.MaintenanceState[] array = null;
					if (!context.LockedMailboxState.Quarantined)
					{
						array = (MaintenanceHandler.MaintenanceState[])context.LockedMailboxState.GetComponentData(MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot);
						if (array == null)
						{
							maintenanceHandler.mailboxNumberToMaintenanceStates.TryGetValue(context.LockedMailboxState.MailboxNumber, out array);
						}
						if (array == null || !array[mailboxMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired)
						{
							if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(65464L, "DoMailboxMaintenance({0}) for database {1}, mailbox {2} called, but maintenance is not marked as required.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
							}
							DiagnosticContext.TraceLocation((LID)52488U);
							return;
						}
						if (MaintenanceHandler.ShouldStopMailboxMaintenanceTask(context, context.LockedMailboxState, maintenanceId))
						{
							if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(0L, "DoMailboxMaintenance({0}) for database {1}, mailbox {2} called, but mailbox maintenance tasks are not permitted to proceed, so the task is being pre-empted.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
							}
							DiagnosticContext.TraceLocation((LID)46816U);
							return;
						}
						try
						{
							array[mailboxMaintenanceTaskDefinition.MaintenanceSlotIndex].LastExecutionStarted = DateTime.UtcNow;
							mailboxMaintenanceTaskDefinition.MailboxMaintenanceDelegate(context, context.LockedMailboxState, out flag);
							commit = true;
						}
						finally
						{
							if (context.IsMailboxOperationStarted)
							{
								array[mailboxMaintenanceTaskDefinition.MaintenanceSlotIndex].LastExecutionFinished = DateTime.UtcNow;
							}
						}
						context.PerfInstance.RateOfMailboxMaintenances.Increment();
					}
					else if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MaintenanceTracer.TraceDebug<string, Guid, int>(46720L, "DoMailboxMaintenance({0}) for database {1}, mailbox {2} called, but mailbox is quarantined.", mailboxMaintenanceTaskDefinition.MaintenanceTaskName, context.Database.MdbGuid, mailboxNumber);
					}
					if (!context.IsMailboxOperationStarted)
					{
						DiagnosticContext.TraceLocation((LID)59228U);
					}
					else
					{
						if (flag && array[mailboxMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired)
						{
							context.PerfInstance.MailboxMaintenances.Decrement();
							array[mailboxMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired = false;
							if (array.All((MaintenanceHandler.MaintenanceState state) => !state.MaintenanceRequired))
							{
								DiagnosticContext.TraceLocation((LID)57784U);
								MaintenanceHandler.MaintenanceState[] array2;
								maintenanceHandler.mailboxNumberToMaintenanceStates.TryRemove(mailboxNumber, out array2);
								context.LockedMailboxState.SetComponentData(MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot, null);
								context.PerfInstance.MailboxesWithMaintenances.Decrement();
							}
						}
						if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.MaintenanceTracer.TraceDebug(57272L, "DoMailboxMaintenance({0}) for database {1}, mailbox {2} finished, completed = {3}.", new object[]
							{
								mailboxMaintenanceTaskDefinition.MaintenanceTaskName,
								context.Database.MdbGuid,
								mailboxNumber,
								flag
							});
						}
					}
				}
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit, false);
				}
			}
		}

		public static string GetMailboxMaintenanceTaskName(Guid maintenanceId)
		{
			MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition = MaintenanceHandler.FindMailboxMaintenanceDefinition(maintenanceId);
			if (mailboxMaintenanceTaskDefinition == null)
			{
				return null;
			}
			return mailboxMaintenanceTaskDefinition.MaintenanceTaskName;
		}

		public static IEnumerable<MaintenanceHandler.QueryableMaintenanceState> GetDatabaseMaintenanceSnapshot(Context context)
		{
			MaintenanceHandler.QueryableMaintenanceState[] array = new MaintenanceHandler.QueryableMaintenanceState[MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count];
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			for (int i = 0; i < MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count; i++)
			{
				MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition = MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions[i];
				MaintenanceHandler.MaintenanceState maintenanceState = maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex];
				array[i] = new MaintenanceHandler.QueryableMaintenanceState(databaseMaintenanceTaskDefinition.MaintenanceId, databaseMaintenanceTaskDefinition.RequiredMaintenanceResourceType, databaseMaintenanceTaskDefinition.MaintenanceTaskName, maintenanceState.MaintenanceRequired, maintenanceState.LastRequested, maintenanceState.LastExecutionStarted, maintenanceState.LastExecutionFinished);
			}
			return array;
		}

		public static IEnumerable<MaintenanceHandler.QueryableMailboxMaintenanceState> GetMailboxMaintenanceSnapshot(Context context)
		{
			List<MaintenanceHandler.QueryableMailboxMaintenanceState> list = new List<MaintenanceHandler.QueryableMailboxMaintenanceState>(10 * MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions.Count);
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			foreach (KeyValuePair<int, MaintenanceHandler.MaintenanceState[]> keyValuePair in maintenanceHandler.mailboxNumberToMaintenanceStates)
			{
				for (int i = 0; i < keyValuePair.Value.Length; i++)
				{
					MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition = MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions[i];
					MaintenanceHandler.MaintenanceState maintenanceState = keyValuePair.Value[i];
					list.Add(new MaintenanceHandler.QueryableMailboxMaintenanceState(mailboxMaintenanceTaskDefinition.MaintenanceId, mailboxMaintenanceTaskDefinition.RequiredMaintenanceResourceType, mailboxMaintenanceTaskDefinition.MaintenanceTaskName, mailboxMaintenanceTaskDefinition.IsFinal, keyValuePair.Key, maintenanceState.MaintenanceRequired, maintenanceState.LastRequested, maintenanceState.LastExecutionStarted, maintenanceState.LastExecutionFinished));
				}
			}
			return list;
		}

		public static List<MaintenanceHandler.MaintenanceToSchedule> GetScheduledMaintenances(Context context, RequiredMaintenanceResourceType requestedMaintenanceResourceType)
		{
			List<MaintenanceHandler.MaintenanceToSchedule> list = new List<MaintenanceHandler.MaintenanceToSchedule>(10);
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			AssistantActivityMonitor.Instance(context.Database).UpdateAssistantActivityState(requestedMaintenanceResourceType, true);
			for (int i = 0; i < MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions.Count; i++)
			{
				MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition = MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions[i];
				if (maintenanceHandler.databaseMaintenanceStates[databaseMaintenanceTaskDefinition.MaintenanceSlotIndex].MaintenanceRequired && databaseMaintenanceTaskDefinition.RequiredMaintenanceResourceType == requestedMaintenanceResourceType)
				{
					for (int j = 0; j < databaseMaintenanceTaskDefinition.NumberOfBatchesToSchedule; j++)
					{
						list.Add(new MaintenanceHandler.MaintenanceToSchedule(databaseMaintenanceTaskDefinition.MaintenanceId, 0, Guid.Empty));
					}
				}
				if ((long)list.Count >= (long)((ulong)MaintenanceHandler.maximumMaintenanceRecordsToReturn.Value))
				{
					break;
				}
			}
			foreach (KeyValuePair<int, MaintenanceHandler.MaintenanceState[]> keyValuePair in maintenanceHandler.mailboxNumberToMaintenanceStates)
			{
				if ((long)(list.Count + MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions.Count) > (long)((ulong)MaintenanceHandler.maximumMaintenanceRecordsToReturn.Value))
				{
					break;
				}
				MailboxState mailboxState = MailboxStateCache.Get(context, keyValuePair.Key);
				if (mailboxState == null || mailboxState.Quarantined)
				{
					if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.MaintenanceTracer.TraceDebug<Guid, string, string>(40888L, "GetScheduledMaintenances on database {0} called for mailbox {1}, no maintenance record will be returned becuase of {2}.", context.Database.MdbGuid, (mailboxState == null) ? "<N/A>" : mailboxState.MailboxGuid.ToString(), (mailboxState == null) ? "mailbox does not exist" : "Quarantined");
					}
				}
				else
				{
					for (int k = 0; k < keyValuePair.Value.Length; k++)
					{
						MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition = MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions[k];
						if (keyValuePair.Value[k].MaintenanceRequired && mailboxMaintenanceTaskDefinition.RequiredMaintenanceResourceType == requestedMaintenanceResourceType)
						{
							list.Add(new MaintenanceHandler.MaintenanceToSchedule(mailboxMaintenanceTaskDefinition.MaintenanceId, keyValuePair.Key, mailboxState.MailboxGuid));
						}
					}
				}
			}
			if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MaintenanceTracer.TraceDebug<Guid, int>(63104L, "GetScheduledMaintenances for database {0} called. {1} maintenances returned.", context.Database.MdbGuid, list.Count);
			}
			return list;
		}

		public static bool ShouldStopDatabaseMaintenanceTask(Context context, Guid maintenanceId)
		{
			return context.Database.HasExclusiveLockContention() || context.Database.IsDatabaseEngineTooBusyForDatabaseMaintenanceTask(maintenanceId);
		}

		public static bool ShouldStopMailboxMaintenanceTask(Context context, MailboxState mailboxState, Guid maintenanceId)
		{
			return context.Database.HasExclusiveLockContention() || context.Database.IsDatabaseEngineTooBusyForMailboxMaintenanceTask(mailboxState, maintenanceId);
		}

		public static void ApplyMaintenanceToActiveAndDeletedMailboxes(Context context, ExecutionDiagnostics.OperationSource operationSource, Action<Context, MailboxState> maintenanceDelegate, out bool completed)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaCompare(mailboxTable.LastLogonTime, SearchCriteriaCompare.SearchRelOp.GreaterThan, Factory.CreateConstantColumn(DateTime.UtcNow.Subtract(TimeSpan.FromDays(2.0))));
			SearchCriteria searchCriteria2 = Factory.CreateSearchCriteriaCompare(mailboxTable.DeletedOn, SearchCriteriaCompare.SearchRelOp.GreaterThan, Factory.CreateConstantColumn(DateTime.UtcNow.Subtract(TimeSpan.FromDays(3.0))));
			IEnumerable<MailboxState> stateListSnapshot = MailboxStateCache.GetStateListSnapshot(context, Factory.CreateSearchCriteriaOr(new SearchCriteria[]
			{
				searchCriteria,
				searchCriteria2
			}));
			MaintenanceHandler.ApplyMaintenanceToMailboxes(context, stateListSnapshot, operationSource, true, maintenanceDelegate, out completed);
		}

		public static void ApplyMaintenanceToMailboxes(Context context, IEnumerable<MailboxState> mailboxList, ExecutionDiagnostics.OperationSource operationSource, bool checkMailboxIsAccessible, Action<Context, MailboxState> maintenanceDelegate, out bool completed)
		{
			completed = true;
			foreach (MailboxState mailboxState in mailboxList)
			{
				bool flag;
				MaintenanceHandler.ApplyMaintenanceToOneMailbox(context, mailboxState.MailboxNumber, operationSource, checkMailboxIsAccessible, maintenanceDelegate, out flag);
				if (!flag)
				{
					completed = false;
				}
			}
		}

		public static void ApplyMaintenanceToOneMailbox(Context context, int mailboxNumber, ExecutionDiagnostics.OperationSource operationSource, bool checkMailboxIsAccessible, Action<Context, MailboxState> maintenanceDelegate, out bool completed)
		{
			context.InitializeMailboxExclusiveOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.MailboxMaintenance, MaintenanceHandler.mailboxLockTimeout);
			bool commit = false;
			try
			{
				ErrorCode errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, true, true);
				if (errorCode != ErrorCode.NoError)
				{
					if (errorCode == ErrorCodeValue.NotFound)
					{
						completed = true;
					}
					else
					{
						completed = false;
					}
					return;
				}
				if (checkMailboxIsAccessible && !context.LockedMailboxState.IsAccessible)
				{
					completed = true;
					return;
				}
				context.LockedMailboxState.AddReference();
				try
				{
					maintenanceDelegate(context, context.LockedMailboxState);
				}
				finally
				{
					context.LockedMailboxState.ReleaseReference();
				}
				commit = true;
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit, false);
				}
			}
			completed = true;
		}

		internal static void Initialize()
		{
			if (MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot == -1)
			{
				MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot = MailboxState.AllocateComponentDataSlot(true);
			}
			if (MaintenanceHandler.RegistrationState.MaintenanceHanderDatabaseSlot == -1)
			{
				MaintenanceHandler.RegistrationState.MaintenanceHanderDatabaseSlot = StoreDatabase.AllocateComponentDataSlot();
			}
			if (MaintenanceHandler.registrationStateForTest.MailboxMaintenanceStatesSlot == -1)
			{
				MaintenanceHandler.registrationStateForTest.MailboxMaintenanceStatesSlot = MailboxState.AllocateComponentDataSlot(true);
			}
			if (MaintenanceHandler.registrationStateForTest.MaintenanceHanderDatabaseSlot == -1)
			{
				MaintenanceHandler.registrationStateForTest.MaintenanceHanderDatabaseSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static IDisposable SetRegistrationStateTestHook()
		{
			MaintenanceHandler.registrationStateForTest.DatabaseLevelMaintenanceDefinitions.Clear();
			MaintenanceHandler.registrationStateForTest.MailboxLevelMaintenanceDefinitions.Clear();
			return MaintenanceHandler.registrationStateHookable.SetTestHook(MaintenanceHandler.registrationStateForTest);
		}

		internal static IDisposable SetDatabaseTaskRegistrationHook(Action<Guid> action)
		{
			return MaintenanceHandler.databaseTaskRegistrationHook.SetTestHook(action);
		}

		internal static IDisposable SetDatabaseTaskExecutionHook(Action<Guid> action)
		{
			return MaintenanceHandler.databaseTaskExecutionHook.SetTestHook(action);
		}

		internal static void MountHandler(Context context)
		{
			StoreDatabase database = context.Database;
			MaintenanceHandler value = new MaintenanceHandler();
			database.ComponentData[MaintenanceHandler.RegistrationState.MaintenanceHanderDatabaseSlot] = value;
		}

		internal static void DismountHandler(StoreDatabase database)
		{
			database.ComponentData[MaintenanceHandler.RegistrationState.MaintenanceHanderDatabaseSlot] = null;
		}

		internal static void TestResetMailboxStateMaintenanceSlot(MailboxState mailboxState)
		{
			mailboxState.SetComponentData(MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot, null);
		}

		internal static IDisposable SetMaximumNumberOfMaintenanceRecordsForTest(uint maximumNumberOfMaintenanceRecords)
		{
			return MaintenanceHandler.maximumMaintenanceRecordsToReturn.SetTestHook(maximumNumberOfMaintenanceRecords);
		}

		[Conditional("DEBUG")]
		private static void AssertStaticInitialized()
		{
		}

		private static MaintenanceHandler GetMaintenanceHandler(Context context)
		{
			return (MaintenanceHandler)context.Database.ComponentData[MaintenanceHandler.RegistrationState.MaintenanceHanderDatabaseSlot];
		}

		private static MaintenanceHandler.DatabaseMaintenanceTaskDefinition FindDatabaseMaintenanceDefinition(Guid maintenanceId)
		{
			foreach (MaintenanceHandler.DatabaseMaintenanceTaskDefinition databaseMaintenanceTaskDefinition in MaintenanceHandler.RegistrationState.DatabaseLevelMaintenanceDefinitions)
			{
				if (maintenanceId == databaseMaintenanceTaskDefinition.MaintenanceId)
				{
					return databaseMaintenanceTaskDefinition;
				}
			}
			return null;
		}

		private static MaintenanceHandler.MailboxMaintenanceTaskDefinition FindMailboxMaintenanceDefinition(Guid maintenanceId)
		{
			foreach (MaintenanceHandler.MailboxMaintenanceTaskDefinition mailboxMaintenanceTaskDefinition in MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions)
			{
				if (maintenanceId == mailboxMaintenanceTaskDefinition.MaintenanceId)
				{
					return mailboxMaintenanceTaskDefinition;
				}
			}
			return null;
		}

		private static bool MarkDatabaseForMaintenance(Context context, MaintenanceHandler.DatabaseMaintenanceTaskDefinition maintenanceDefinition)
		{
			bool result = false;
			MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
			if (!maintenanceHandler.databaseMaintenanceStates[maintenanceDefinition.MaintenanceSlotIndex].MaintenanceRequired)
			{
				result = true;
				maintenanceHandler.databaseMaintenanceStates[maintenanceDefinition.MaintenanceSlotIndex].LastRequested = DateTime.UtcNow;
				maintenanceHandler.databaseMaintenanceStates[maintenanceDefinition.MaintenanceSlotIndex].MaintenanceRequired = true;
			}
			if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MaintenanceTracer.TraceDebug<Guid, Guid>(53176L, "Database {0} marked for maintenance {1}.", context.Database.MdbGuid, maintenanceDefinition.MaintenanceId);
			}
			return result;
		}

		private static bool MarkMailboxForMaintenance(Context context, MailboxState mailboxState, MaintenanceHandler.MailboxMaintenanceTaskDefinition maintenanceDefinition)
		{
			bool result = false;
			MaintenanceHandler.MaintenanceState[] array = (MaintenanceHandler.MaintenanceState[])mailboxState.GetComponentData(MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot);
			if (array == null)
			{
				MaintenanceHandler maintenanceHandler = MaintenanceHandler.GetMaintenanceHandler(context);
				if (!maintenanceHandler.mailboxNumberToMaintenanceStates.TryGetValue(mailboxState.MailboxNumber, out array))
				{
					array = new MaintenanceHandler.MaintenanceState[MaintenanceHandler.RegistrationState.MailboxLevelMaintenanceDefinitions.Count];
					context.PerfInstance.MailboxesWithMaintenances.Increment();
				}
				mailboxState.SetComponentData(MaintenanceHandler.RegistrationState.MailboxMaintenanceStatesSlot, array);
				maintenanceHandler.mailboxNumberToMaintenanceStates[mailboxState.MailboxNumber] = array;
			}
			if (maintenanceDefinition.IsFinal)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].MaintenanceRequired)
					{
						array[i].MaintenanceRequired = false;
						context.PerfInstance.MailboxMaintenances.Decrement();
					}
				}
			}
			if (!array[maintenanceDefinition.MaintenanceSlotIndex].MaintenanceRequired)
			{
				context.PerfInstance.MailboxMaintenances.Increment();
				array[maintenanceDefinition.MaintenanceSlotIndex].LastRequested = DateTime.UtcNow;
				array[maintenanceDefinition.MaintenanceSlotIndex].MaintenanceRequired = true;
				result = true;
			}
			if (ExTraceGlobals.MaintenanceTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MaintenanceTracer.TraceDebug<int, Guid, Guid>(36792L, "MailboxNumber {0} on database {1} marked for maintenance {2}.", mailboxState.MailboxNumber, context.Database.MdbGuid, maintenanceDefinition.MaintenanceId);
			}
			return result;
		}

		private static Hookable<MaintenanceHandler.RegistrationStateObject> registrationStateHookable = Hookable<MaintenanceHandler.RegistrationStateObject>.Create(false, new MaintenanceHandler.RegistrationStateObject());

		private static Hookable<uint> maximumMaintenanceRecordsToReturn = Hookable<uint>.Create(false, 1000U);

		private static Hookable<Action<Guid>> databaseTaskRegistrationHook = Hookable<Action<Guid>>.Create(true, delegate(Guid guid)
		{
		});

		private static Hookable<Action<Guid>> databaseTaskExecutionHook = Hookable<Action<Guid>>.Create(true, delegate(Guid guid)
		{
		});

		private static MaintenanceHandler.RegistrationStateObject registrationStateForTest = new MaintenanceHandler.RegistrationStateObject();

		private static bool asyncMaintenanceSchedulingEnabled = true;

		private static readonly TimeSpan mailboxLockTimeout = TimeSpan.FromMinutes(1.0);

		private readonly MaintenanceHandler.MaintenanceState[] databaseMaintenanceStates;

		private readonly int[] databaseMaintenanceInProgress;

		private readonly ConcurrentDictionary<int, MaintenanceHandler.MaintenanceState[]> mailboxNumberToMaintenanceStates = new ConcurrentDictionary<int, MaintenanceHandler.MaintenanceState[]>(50, 1000);

		private enum DatabaseMaintenanceStatus
		{
			Idle,
			InProgress
		}

		public delegate void DatabaseMaintenanceDelegate(Context context, DatabaseInfo databaseInfo, out bool completed);

		public delegate void MailboxMaintenanceDelegate(Context context, MailboxState lockedMailboxState, out bool completed);

		public class QueryableMaintenanceState
		{
			internal QueryableMaintenanceState(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, string maintenanceTaskName, bool maintenanceRequired, DateTime lastRequested, DateTime lastExecutionStarted, DateTime lastExecutionFinished)
			{
				this.maintenanceId = maintenanceId;
				this.requiredMaintenanceResourceType = requiredMaintenanceResourceType;
				this.maintenanceTaskName = maintenanceTaskName;
				this.maintenanceRequired = maintenanceRequired;
				this.lastRequested = lastRequested;
				this.lastExecutionStarted = lastExecutionStarted;
				this.lastExecutionFinished = lastExecutionFinished;
			}

			public Guid MaintenanceId
			{
				get
				{
					return this.maintenanceId;
				}
			}

			public RequiredMaintenanceResourceType RequiredMaintenanceResourceType
			{
				get
				{
					return this.requiredMaintenanceResourceType;
				}
			}

			public string MaintenanceTaskName
			{
				get
				{
					return this.maintenanceTaskName;
				}
			}

			public bool MaintenanceRequired
			{
				get
				{
					return this.maintenanceRequired;
				}
			}

			public DateTime LastRequested
			{
				get
				{
					return this.lastRequested;
				}
			}

			public DateTime LastExecutionStarted
			{
				get
				{
					return this.lastExecutionStarted;
				}
			}

			public DateTime LastExecutionFinished
			{
				get
				{
					return this.lastExecutionFinished;
				}
			}

			private readonly Guid maintenanceId;

			private readonly RequiredMaintenanceResourceType requiredMaintenanceResourceType;

			private readonly string maintenanceTaskName;

			private readonly bool maintenanceRequired;

			private readonly DateTime lastRequested;

			private readonly DateTime lastExecutionStarted;

			private readonly DateTime lastExecutionFinished;
		}

		public class QueryableMailboxMaintenanceState : MaintenanceHandler.QueryableMaintenanceState
		{
			internal QueryableMailboxMaintenanceState(Guid maintenanceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, string maintenanceTaskName, bool isFinal, int mailboxNumber, bool maintenanceRequired, DateTime lastRequested, DateTime lastExecutionStarted, DateTime lastExecutionFinished) : base(maintenanceId, requiredMaintenanceResourceType, maintenanceTaskName, maintenanceRequired, lastRequested, lastExecutionStarted, lastExecutionFinished)
			{
				this.isFinal = isFinal;
				this.mailboxNumber = mailboxNumber;
			}

			public bool IsFinal
			{
				get
				{
					return this.isFinal;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			private readonly bool isFinal;

			private readonly int mailboxNumber;
		}

		public struct MaintenanceToSchedule
		{
			public MaintenanceToSchedule(Guid maintenanceId, int mailboxNumber, Guid mailboxGuid)
			{
				this.maintenanceId = maintenanceId;
				this.mailboxNumber = mailboxNumber;
				this.mailboxGuid = mailboxGuid;
			}

			public Guid MaintenanceId
			{
				get
				{
					return this.maintenanceId;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public Guid MailboxGuid
			{
				get
				{
					return this.mailboxGuid;
				}
			}

			private Guid maintenanceId;

			private int mailboxNumber;

			private Guid mailboxGuid;
		}

		private struct MaintenanceState
		{
			internal bool MaintenanceRequired
			{
				get
				{
					return this.maintenanceRequired;
				}
				set
				{
					this.maintenanceRequired = value;
				}
			}

			internal DateTime LastExecutionStarted
			{
				get
				{
					return this.lastExecutionStarted;
				}
				set
				{
					this.lastExecutionStarted = value;
				}
			}

			internal DateTime LastExecutionFinished
			{
				get
				{
					return this.lastExecutionFinished;
				}
				set
				{
					this.lastExecutionFinished = value;
				}
			}

			internal DateTime LastRequested
			{
				get
				{
					return this.lastRequested;
				}
				set
				{
					this.lastRequested = value;
				}
			}

			private bool maintenanceRequired;

			private DateTime lastExecutionStarted;

			private DateTime lastExecutionFinished;

			private DateTime lastRequested;
		}

		private class MaintenanceTaskDefinition
		{
			protected MaintenanceTaskDefinition(Guid maintenanceId, int maintenanceSlotIndex, RequiredMaintenanceResourceType requiredMaintenanceResourceType, string maintenanceTaskName)
			{
				this.maintenanceId = maintenanceId;
				this.maintenanceSlotIndex = maintenanceSlotIndex;
				this.requiredMaintenanceResourceType = requiredMaintenanceResourceType;
				this.maintenanceTaskName = maintenanceTaskName;
			}

			internal Guid MaintenanceId
			{
				get
				{
					return this.maintenanceId;
				}
			}

			internal int MaintenanceSlotIndex
			{
				get
				{
					return this.maintenanceSlotIndex;
				}
			}

			internal RequiredMaintenanceResourceType RequiredMaintenanceResourceType
			{
				get
				{
					return this.requiredMaintenanceResourceType;
				}
			}

			internal string MaintenanceTaskName
			{
				get
				{
					return this.maintenanceTaskName;
				}
			}

			private readonly Guid maintenanceId;

			private readonly int maintenanceSlotIndex;

			private readonly RequiredMaintenanceResourceType requiredMaintenanceResourceType;

			private readonly string maintenanceTaskName;
		}

		private class DatabaseMaintenanceTaskDefinition : MaintenanceHandler.MaintenanceTaskDefinition, IDatabaseMaintenance
		{
			internal DatabaseMaintenanceTaskDefinition(Guid maintenanceId, int maintenanceSlotIndex, RequiredMaintenanceResourceType requiredMaintenanceResourceType, MaintenanceHandler.DatabaseMaintenanceDelegate databaseMaintenanceDelegate, string maintenanceTaskName, int numberOfBatchesToSchedule) : base(maintenanceId, maintenanceSlotIndex, requiredMaintenanceResourceType, maintenanceTaskName)
			{
				this.databaseMaintenanceDelegate = databaseMaintenanceDelegate;
				this.numberOfBatchesToSchedule = numberOfBatchesToSchedule;
			}

			internal MaintenanceHandler.DatabaseMaintenanceDelegate DatabaseMaintenanceDelegate
			{
				get
				{
					return this.databaseMaintenanceDelegate;
				}
			}

			internal int NumberOfBatchesToSchedule
			{
				get
				{
					return this.numberOfBatchesToSchedule;
				}
			}

			public bool MarkForMaintenance(Context context)
			{
				return MaintenanceHandler.MarkDatabaseForMaintenance(context, this);
			}

			public void ScheduleMarkForMaintenance(Context context, TimeSpan interval)
			{
				this.ScheduleMarkForMaintenance(context, TimeSpan.Zero, interval);
			}

			public void ScheduleMarkForMaintenance(Context context, TimeSpan initialDelay, TimeSpan interval)
			{
				MaintenanceHandler.databaseTaskRegistrationHook.Value(base.MaintenanceId);
				RecurringTask<StoreDatabase> task = new RecurringTask<StoreDatabase>(TaskExecutionWrapper<StoreDatabase>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.MarkForMaintenance, context.ClientType, context.Database.MdbGuid), new TaskExecutionWrapper<StoreDatabase>.TaskCallback<Context>(this.MarkForMaintenanceTask)), context.Database, initialDelay, interval, false);
				context.Database.TaskList.Add(task, true);
			}

			private void MarkForMaintenanceTask(Context context, StoreDatabase database, Func<bool> shouldCallbackContinue)
			{
				if (!MaintenanceHandler.AsyncMaintenanceSchedulingEnabled)
				{
					return;
				}
				using (context.AssociateWithDatabase(database))
				{
					if (database.IsOnlineActive)
					{
						MaintenanceHandler.databaseTaskExecutionHook.Value(base.MaintenanceId);
						this.MarkForMaintenance(context);
					}
				}
			}

			private readonly MaintenanceHandler.DatabaseMaintenanceDelegate databaseMaintenanceDelegate;

			private readonly int numberOfBatchesToSchedule;
		}

		private class MailboxMaintenanceTaskDefinition : MaintenanceHandler.MaintenanceTaskDefinition, IMailboxMaintenance
		{
			internal MailboxMaintenanceTaskDefinition(Guid maintenanceId, int maintenanceSlotIndex, RequiredMaintenanceResourceType requiredMaintenanceResourceType, bool checkMailboxIsIdle, MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate, string maintenanceTaskName, bool isFinal) : base(maintenanceId, maintenanceSlotIndex, requiredMaintenanceResourceType, maintenanceTaskName)
			{
				this.mailboxMaintenanceDelegate = mailboxMaintenanceDelegate;
				this.checkMailboxIsIdle = checkMailboxIsIdle;
				this.isFinal = isFinal;
			}

			internal MaintenanceHandler.MailboxMaintenanceDelegate MailboxMaintenanceDelegate
			{
				get
				{
					return this.mailboxMaintenanceDelegate;
				}
			}

			internal bool CheckMailboxIsIdle
			{
				get
				{
					return this.checkMailboxIsIdle;
				}
			}

			internal bool IsFinal
			{
				get
				{
					return this.isFinal;
				}
			}

			public bool MarkForMaintenance(Context context, MailboxState mailboxState)
			{
				return MaintenanceHandler.MarkMailboxForMaintenance(context, mailboxState, this);
			}

			private readonly MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate;

			private readonly bool checkMailboxIsIdle;

			private readonly bool isFinal;
		}

		private class RegistrationStateObject
		{
			internal int MailboxMaintenanceStatesSlot = -1;

			internal int MaintenanceHanderDatabaseSlot = -1;

			internal List<MaintenanceHandler.DatabaseMaintenanceTaskDefinition> DatabaseLevelMaintenanceDefinitions = new List<MaintenanceHandler.DatabaseMaintenanceTaskDefinition>();

			internal List<MaintenanceHandler.MailboxMaintenanceTaskDefinition> MailboxLevelMaintenanceDefinitions = new List<MaintenanceHandler.MailboxMaintenanceTaskDefinition>();
		}
	}
}
