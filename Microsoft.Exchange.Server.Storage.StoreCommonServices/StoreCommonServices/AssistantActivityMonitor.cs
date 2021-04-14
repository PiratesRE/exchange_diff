using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class AssistantActivityMonitor
	{
		private AssistantActivityMonitor(StoreDatabase database)
		{
			this.database = database;
			this.assistantActivityStates = new AssistantActivityState[5];
			for (int i = 0; i < 5; i++)
			{
				this.assistantActivityStates[i] = new AssistantActivityState((RequiredMaintenanceResourceType)i);
			}
		}

		public static TimeSpan MaintenanceControlPeriod
		{
			get
			{
				return AssistantActivityMonitor.maintenanceControlPeriod;
			}
		}

		public AssistantActivityState[] AssistantActivityStates
		{
			get
			{
				return this.assistantActivityStates;
			}
		}

		public AssistantActivityState this[RequiredMaintenanceResourceType requiredMaintenanceResourceType]
		{
			get
			{
				return this.assistantActivityStates[(int)requiredMaintenanceResourceType];
			}
		}

		public static AssistantActivityMonitor Instance(StoreDatabase database)
		{
			return database.ComponentData[AssistantActivityMonitor.assistantStateTrackerSlot] as AssistantActivityMonitor;
		}

		public static IEnumerable<AssistantActivityState> GetAssistantActivitySnapshot(Context context)
		{
			return AssistantActivityMonitor.Instance(context.Database).assistantActivityStates;
		}

		public static void PublishActiveMonitoringNotification(RequiredMaintenanceResourceType resourceType, string databaseName, ResultSeverityLevel resultSeverityLevel)
		{
			NotificationItem notificationItem = new EventNotificationItem(ExchangeComponent.Store.Name, string.Format("{0}.{1}", "StoreMaintenanceHandler", resourceType), databaseName, AssistantActivityMonitor.MaintenanceControlPeriod.ToString(), resultSeverityLevel);
			notificationItem.Publish(false);
		}

		public void UpdateAssistantActivityState(RequiredMaintenanceResourceType requiredMaintenanceResourceType, bool maintenanceRequested)
		{
			this.assistantActivityStates[(int)requiredMaintenanceResourceType].AssistantIsActiveInLastMonitoringPeriod = true;
			if (maintenanceRequested)
			{
				this.assistantActivityStates[(int)requiredMaintenanceResourceType].LastTimeRequested = DateTime.UtcNow;
				return;
			}
			this.assistantActivityStates[(int)requiredMaintenanceResourceType].LastTimePerformed = DateTime.UtcNow;
		}

		internal static void Initialize()
		{
			if (AssistantActivityMonitor.assistantStateTrackerSlot == -1)
			{
				AssistantActivityMonitor.assistantStateTrackerSlot = StoreDatabase.AllocateComponentDataSlot();
			}
			AssistantActivityMonitor.maintenanceControlPeriod = ConfigurationSchema.MaintenanceControlPeriod.Value;
		}

		internal static void MountHandler(Context context, StoreDatabase database)
		{
			AssistantActivityMonitor value = new AssistantActivityMonitor(database);
			database.ComponentData[AssistantActivityMonitor.assistantStateTrackerSlot] = value;
		}

		internal static void MountedHandler(Context context, StoreDatabase database)
		{
			AssistantActivityMonitor assistantActivityMonitor = database.ComponentData[AssistantActivityMonitor.assistantStateTrackerSlot] as AssistantActivityMonitor;
			RecurringTask<AssistantActivityMonitor> task = new RecurringTask<AssistantActivityMonitor>(TaskExecutionWrapper<AssistantActivityMonitor>.WrapExecute(new TaskDiagnosticInformation(TaskTypeId.MaintenanceIdleCheck, ClientType.System, context.Database.MdbGuid), new TaskExecutionWrapper<AssistantActivityMonitor>.TaskCallback<Context>(assistantActivityMonitor.MaintenanceIdleCheckTask)), assistantActivityMonitor, AssistantActivityMonitor.MaintenanceControlPeriod, false);
			context.Database.TaskList.Add(task, true);
		}

		internal static void DismountHandler(StoreDatabase database)
		{
			database.ComponentData[AssistantActivityMonitor.assistantStateTrackerSlot] = null;
		}

		private void MaintenanceIdleCheckTask(Context context, AssistantActivityMonitor thisObject, Func<bool> shouldCallbackContinue)
		{
			using (context.AssociateWithDatabase(this.database))
			{
				if (this.database.IsOnlineActive)
				{
					for (int i = 0; i < 5; i++)
					{
						if (this.assistantActivityStates[i].AssistantIsActiveInLastMonitoringPeriod)
						{
							this.assistantActivityStates[i].AssistantIsActiveInLastMonitoringPeriod = false;
							AssistantActivityMonitor.PublishActiveMonitoringNotification((RequiredMaintenanceResourceType)i, this.database.MdbName, ResultSeverityLevel.Informational);
						}
						else if (this.database.MountTime < DateTime.UtcNow - AssistantActivityMonitor.MaintenanceControlPeriod)
						{
							Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MaintenanceProcessorIsIdle, new object[]
							{
								(RequiredMaintenanceResourceType)i,
								this.database.MdbName,
								AssistantActivityMonitor.MaintenanceControlPeriod.ToString()
							});
						}
					}
				}
			}
		}

		public const string MaintenanceAssistantTriggerString = "StoreMaintenanceHandler";

		private static int assistantStateTrackerSlot = -1;

		private static TimeSpan maintenanceControlPeriod;

		private StoreDatabase database;

		private AssistantActivityState[] assistantActivityStates;
	}
}
