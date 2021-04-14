using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class BackgroundJobBackendSession : HygieneSession
	{
		public BackgroundJobBackendSession()
		{
			this.dataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.BackgroundJobBackend);
		}

		public RoleDefinition[] FindRoleByNameVersion(string roleName, string roleVersion)
		{
			QueryFilter filter = null;
			int num = 0;
			if (roleName != null)
			{
				num++;
			}
			if (roleVersion != null)
			{
				num++;
			}
			if (num > 0)
			{
				QueryFilter[] array = new QueryFilter[num];
				int num2 = 0;
				if (roleName != null)
				{
					array[num2++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleNameQueryProperty, roleName);
				}
				if (roleVersion != null)
				{
					array[num2++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleVersionQueryProperty, roleVersion);
				}
				if (num > 1)
				{
					filter = new OrFilter(array);
				}
				else
				{
					filter = array[0];
				}
			}
			IConfigurable[] array2 = this.dataProvider.Find<RoleDefinition>(filter, null, false, null);
			if (array2 == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<RoleDefinition>(array2);
		}

		public RegionDefinition[] FindRegionIdByName(string regionName)
		{
			QueryFilter filter = null;
			if (regionName != null)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.NameQueryProperty, regionName);
			}
			IConfigurable[] array = this.dataProvider.Find<RegionDefinition>(filter, null, false, null);
			if (array == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<RegionDefinition>(array);
		}

		public DataCenterDefinition[] FindDataCenterIdByName(string dcName)
		{
			QueryFilter filter = null;
			if (dcName != null)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.NameQueryProperty, dcName);
			}
			IConfigurable[] array = this.dataProvider.Find<DataCenterDefinition>(filter, null, false, null);
			if (array == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<DataCenterDefinition>(array);
		}

		public BackgroundJobMgrInstance[] FindBackgroundJobMgrInstances(Guid roleId, string machineName)
		{
			QueryFilter[] array = new QueryFilter[2];
			array[0] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId);
			IConfigurable[] array2;
			if (machineName != null)
			{
				array[1] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.MachineNameQueryProperty, machineName);
				OrFilter filter = new OrFilter(array);
				array2 = this.dataProvider.Find<BackgroundJobMgrInstance>(filter, null, false, null);
			}
			else
			{
				array2 = this.dataProvider.Find<BackgroundJobMgrInstance>(array[0], null, false, null);
			}
			if (array2 == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<BackgroundJobMgrInstance>(array2);
		}

		public BackgroundJobMgrInstance FindSchedulerInstance(Guid roleId, DateTime heartbeatThreshold, Regions region, long? datacenter = null)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId),
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.heartbeatDatetimeThresholdQueryProperty, heartbeatThreshold),
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RegionSelectionSetQueryProperty, region),
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.DataCenterIdQueryProperty, datacenter)
			});
			IConfigurable[] array = this.dataProvider.Find<BackgroundJobMgrInstance>(filter, null, false, null);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableToT<BackgroundJobMgrInstance>(array[0]);
		}

		public BackgroundJobMgrInstance[] FindBackgroundJobMgrInstances(Guid roleId, DateTime lastCheckedDatetime, bool active)
		{
			OrFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId),
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.LastCheckedDatetimeQueryProperty, lastCheckedDatetime),
				new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.ActiveQueryProperty, active)
			});
			IConfigurable[] array = this.dataProvider.Find<BackgroundJobMgrInstance>(filter, null, false, null);
			if (array == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<BackgroundJobMgrInstance>(array);
		}

		public JobDefinition[] FindJobDefinitions(Guid? roleId = null, Guid? jobId = null, string jobName = null)
		{
			QueryFilter filter = null;
			if (roleId != null || jobId != null || jobName != null)
			{
				List<QueryFilter> list = new List<QueryFilter>(3);
				if (roleId != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId));
				}
				if (jobId != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.JobIdQueryProperty, jobId));
				}
				if (jobName != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.JobNameQueryProperty, jobName));
				}
				if (list.Count == 1)
				{
					filter = list[0];
				}
				else
				{
					filter = new OrFilter(list.ToArray());
				}
			}
			IConfigurable[] array = this.dataProvider.Find<JobDefinition>(filter, null, false, null);
			if (array == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<JobDefinition>(array);
		}

		public ScheduleItem[] FindScheduleItems(Guid roleId, Guid? scheduleId = null, bool? active = null, int? regionSelectionSet = null, IEnumerable<long> dataCenterIdCollection = null, Guid? jobId = null)
		{
			int num = 2;
			if (scheduleId != null)
			{
				num++;
			}
			if (active != null)
			{
				num++;
			}
			if (regionSelectionSet != null)
			{
				num++;
			}
			if (jobId != null)
			{
				num++;
			}
			QueryFilter[] array = new QueryFilter[num];
			num = 0;
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId);
			if (scheduleId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.ScheduleIdQueryProperty, scheduleId);
			}
			if (active != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.ActiveQueryProperty, active);
			}
			if (regionSelectionSet != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RegionSelectionSetQueryProperty, regionSelectionSet);
			}
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			if (dataCenterIdCollection != null)
			{
				foreach (long num2 in dataCenterIdCollection)
				{
					batchPropertyTable.AddPropertyValue(Guid.NewGuid(), BackgroundJobBackendSession.DCSelectionSetProperty2, num2);
				}
			}
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TvpDCIdList, batchPropertyTable);
			if (jobId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.JobIdQueryProperty, jobId);
			}
			QueryFilter filter;
			if (num > 1)
			{
				filter = new OrFilter(array);
			}
			else
			{
				filter = array[0];
			}
			IConfigurable[] array2 = this.dataProvider.Find<ScheduleItem>(filter, null, false, null);
			if (array2 == null)
			{
				return null;
			}
			return BackgroundJobBackendSession.IConfigurableArrayTo<ScheduleItem>(array2);
		}

		public TaskItem[] FindTasks(Guid roleId, Guid? ownerId = null, TaskExecutionStateType? taskExecutionState = null, SchedulingType? schedulingType = null, int? regionSelectionSet = null, Guid? scheduleId = null, Guid? taskId = null, TaskCompletionStatusType? taskCompletionStatus = null, IEnumerable<long> dataCenterIdCollection = null, Guid? jobId = null, Guid? activeJobId = null)
		{
			if (taskExecutionState == null)
			{
				IPagedReader<TaskItem> pagedReader = this.FindTasks(roleId, ownerId, null, null, schedulingType, regionSelectionSet, scheduleId, taskId, taskCompletionStatus, dataCenterIdCollection, 1000, jobId, activeJobId);
				return pagedReader.ReadAllPages();
			}
			IPagedReader<TaskItem> pagedReader2 = this.FindTasks(roleId, ownerId, new TaskExecutionStateType[]
			{
				taskExecutionState.Value
			}, null, schedulingType, regionSelectionSet, scheduleId, taskId, taskCompletionStatus, dataCenterIdCollection, 1000, jobId, activeJobId);
			return pagedReader2.ReadAllPages();
		}

		public TaskItem[] FindTasks(Guid roleId, Guid? ownerId, IEnumerable<TaskExecutionStateType> taskExecutionStates, IEnumerable<TaskExecutionStateType> taskExecutionExclusionStates, SchedulingType? schedulingType, int? regionSelectionSet, Guid? scheduleId, Guid? taskId, TaskCompletionStatusType? taskCompletionStatus, IEnumerable<long> dataCenterIdCollection, Guid? jobId = null, Guid? activeJobId = null)
		{
			IPagedReader<TaskItem> pagedReader = this.FindTasks(roleId, ownerId, taskExecutionStates, taskExecutionExclusionStates, schedulingType, regionSelectionSet, scheduleId, taskId, taskCompletionStatus, dataCenterIdCollection, 1000, jobId, activeJobId);
			return pagedReader.ReadAllPages();
		}

		public IPagedReader<TaskItem> FindTasks(Guid roleId, Guid? ownerId, IEnumerable<TaskExecutionStateType> taskExecutionStates, IEnumerable<TaskExecutionStateType> taskExecutionExclusionStates, SchedulingType? schedulingType, int? regionSelectionSet, Guid? scheduleId, Guid? taskId, TaskCompletionStatusType? taskCompletionStatus, IEnumerable<long> dataCenterIdCollection, int pageSize, Guid? jobId = null, Guid? activeJobId = null)
		{
			int num = 5;
			if (jobId != null)
			{
				num++;
			}
			if (activeJobId != null)
			{
				num++;
			}
			if (ownerId != null)
			{
				num++;
			}
			if (schedulingType != null)
			{
				num++;
			}
			if (regionSelectionSet != null)
			{
				num++;
			}
			if (scheduleId != null)
			{
				num++;
			}
			if (taskId != null)
			{
				num++;
			}
			if (taskCompletionStatus != null)
			{
				num++;
			}
			QueryFilter[] array = new QueryFilter[num];
			num = 0;
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RoleIdQueryProperty, roleId);
			if (jobId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.JobIdQueryProperty, jobId);
			}
			if (activeJobId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.ActiveJobIdQueryProperty, activeJobId);
			}
			if (ownerId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.OwnerIdQueryProperty, ownerId);
			}
			if (schedulingType != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.SchedulingTypeQueryProperty, (byte)schedulingType.Value);
			}
			if (regionSelectionSet != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.RegionSelectionSetQueryProperty, regionSelectionSet);
			}
			if (scheduleId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.ScheduleIdQueryProperty, scheduleId);
			}
			if (taskId != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TaskIdQueryProperty, taskId);
			}
			if (taskCompletionStatus != null)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TaskCompletionStatusQueryProperty, taskCompletionStatus);
			}
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			if (dataCenterIdCollection != null)
			{
				foreach (long num2 in dataCenterIdCollection)
				{
					batchPropertyTable.AddPropertyValue(Guid.NewGuid(), BackgroundJobBackendSession.DCSelectionSetProperty2, num2);
				}
			}
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TvpDCIdList, batchPropertyTable);
			PropertyTable propertyTable = new PropertyTable();
			MultiValuedProperty<int> multiValuedProperty = new MultiValuedProperty<int>();
			if (taskExecutionStates != null)
			{
				foreach (TaskExecutionStateType item in taskExecutionStates)
				{
					multiValuedProperty.Add((int)item);
				}
				propertyTable.AddPropertyValue(BackgroundJobBackendSession.TaskExecutionStateQueryMvpProperty, multiValuedProperty);
			}
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TvpTaskExecutionStates, propertyTable);
			PropertyTable propertyTable2 = new PropertyTable();
			MultiValuedProperty<int> multiValuedProperty2 = new MultiValuedProperty<int>();
			if (taskExecutionExclusionStates != null)
			{
				foreach (TaskExecutionStateType item2 in taskExecutionExclusionStates)
				{
					multiValuedProperty2.Add((int)item2);
				}
				propertyTable2.AddPropertyValue(BackgroundJobBackendSession.TaskExecutionStateQueryMvpProperty, multiValuedProperty2);
			}
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.TvpTaskExecutionExclusionStates, propertyTable2);
			array[num++] = new ComparisonFilter(ComparisonOperator.Equal, BackgroundJobBackendSession.PageSizeQueryProperty, pageSize);
			QueryFilter queryFilter = new OrFilter(array);
			return this.GetPagedReader<TaskItem>(queryFilter, pageSize, jobId == null);
		}

		public void Save(DataCenterDefinition dcDef)
		{
			if (dcDef == null)
			{
				throw new ArgumentNullException("dcDef");
			}
			AuditHelper.ApplyAuditProperties(dcDef, default(Guid), null);
			this.dataProvider.Save(dcDef);
		}

		public void Save(RoleDefinition roleDef)
		{
			if (roleDef == null)
			{
				throw new ArgumentNullException("roleDef");
			}
			AuditHelper.ApplyAuditProperties(roleDef, default(Guid), null);
			this.dataProvider.Save(roleDef);
		}

		public void Save(RegionDefinition regionDef)
		{
			if (regionDef == null)
			{
				throw new ArgumentNullException("regionDef");
			}
			AuditHelper.ApplyAuditProperties(regionDef, default(Guid), null);
			this.dataProvider.Save(regionDef);
		}

		public void Save(BackgroundJobMgrInstance bjmInstance)
		{
			if (bjmInstance == null)
			{
				throw new ArgumentNullException("bjmInstance");
			}
			AuditHelper.ApplyAuditProperties(bjmInstance, default(Guid), null);
			this.dataProvider.Save(bjmInstance);
		}

		public void UpdateBackgroundJobMgrAsTimedOut(BackgroundJobMgrTimedOutInstance bjmTimedOutInstance)
		{
			if (bjmTimedOutInstance == null)
			{
				throw new ArgumentNullException("bjmTimedOutInstance");
			}
			AuditHelper.ApplyAuditProperties(bjmTimedOutInstance, default(Guid), null);
			this.dataProvider.Save(bjmTimedOutInstance);
		}

		public void UpdateScheduleItemActive(ScheduleItemActiveUpdate scheduleItemActiveUpdate)
		{
			if (scheduleItemActiveUpdate == null)
			{
				throw new ArgumentNullException("scheduleItemActiveUpdate");
			}
			AuditHelper.ApplyAuditProperties(scheduleItemActiveUpdate, default(Guid), null);
			this.dataProvider.Save(scheduleItemActiveUpdate);
		}

		public void UpdateBackgroundJobMgrHeartBeat(BackgroundJobMgrHeartBeatUpdate bjmHeartBeatUpdate)
		{
			if (bjmHeartBeatUpdate == null)
			{
				throw new ArgumentNullException("bjmHeartBeatUpdate");
			}
			AuditHelper.ApplyAuditProperties(bjmHeartBeatUpdate, default(Guid), null);
			this.dataProvider.Save(bjmHeartBeatUpdate);
		}

		public void Save(ScheduleItem scheduleItem)
		{
			if (scheduleItem == null)
			{
				throw new ArgumentNullException("scheduleItem");
			}
			AuditHelper.ApplyAuditProperties(scheduleItem, default(Guid), null);
			this.dataProvider.Save(scheduleItem);
		}

		public void Save(JobDefinition jobDefinition)
		{
			if (jobDefinition == null)
			{
				throw new ArgumentNullException("jobDefinition");
			}
			AuditHelper.ApplyAuditProperties(jobDefinition, default(Guid), null);
			this.dataProvider.Save(jobDefinition);
		}

		public void SaveTasksSynchronized(SaveTaskItemBatch saveTaskItemBatch)
		{
			if (saveTaskItemBatch == null)
			{
				throw new ArgumentNullException("saveTaskItemBatch");
			}
			AuditHelper.ApplyAuditProperties(saveTaskItemBatch, default(Guid), null);
			this.dataProvider.Save(saveTaskItemBatch);
		}

		public void UpdateTaskStatus(TaskStatusUpdate taskStatusUpdate)
		{
			if (taskStatusUpdate == null)
			{
				throw new ArgumentNullException("taskStatusUpdate");
			}
			AuditHelper.ApplyAuditProperties(taskStatusUpdate, default(Guid), null);
			this.dataProvider.Save(taskStatusUpdate);
		}

		public void SyncTaskStatusUpdate(SyncTaskStatusUpdate syncTaskStatusUpdate)
		{
			if (syncTaskStatusUpdate == null)
			{
				throw new ArgumentNullException("syncTaskStatusUpdate");
			}
			AuditHelper.ApplyAuditProperties(syncTaskStatusUpdate, default(Guid), null);
			this.dataProvider.Save(syncTaskStatusUpdate);
		}

		public void TryTakeTaskOwnership(TakeTaskOwnership takeTaskOwnership)
		{
			if (takeTaskOwnership == null)
			{
				throw new ArgumentNullException("takeTaskOwnership");
			}
			AuditHelper.ApplyAuditProperties(takeTaskOwnership, default(Guid), null);
			this.dataProvider.Save(takeTaskOwnership);
		}

		private IPagedReader<T> GetPagedReader<T>(QueryFilter queryFilter, int pageSize, bool queryAllPartition) where T : IConfigurable, new()
		{
			if (!queryAllPartition)
			{
				return new ConfigDataProviderPagedReader<T>(this.dataProvider, null, queryFilter, null, pageSize);
			}
			object[] allPhysicalPartitions = ((IPartitionedDataProvider)this.dataProvider).GetAllPhysicalPartitions();
			IPagedReader<T>[] array = new IPagedReader<T>[allPhysicalPartitions.Length];
			for (int i = 0; i < allPhysicalPartitions.Length; i++)
			{
				array[i] = new ConfigDataProviderPagedReader<T>(this.dataProvider, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, allPhysicalPartitions[i]),
					queryFilter
				}), null, pageSize);
			}
			return new CompositePagedReader<T>(array);
		}

		private static T IConfigurableToT<T>(IConfigurable configurable) where T : IConfigurable
		{
			return (T)((object)configurable);
		}

		private static T[] IConfigurableArrayTo<T>(IConfigurable[] configurables) where T : IConfigurable
		{
			T[] array = new T[configurables.Length];
			for (int i = 0; i < configurables.Length; i++)
			{
				array[i] = BackgroundJobBackendSession.IConfigurableToT<T>(configurables[i]);
			}
			return array;
		}

		internal static readonly BackgroundJobBackendPropertyDefinition RoleNameQueryProperty = new BackgroundJobBackendPropertyDefinition("roleName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleVersionQueryProperty = new BackgroundJobBackendPropertyDefinition("roleVersion", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdQueryProperty = new BackgroundJobBackendPropertyDefinition("roleId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition JobIdQueryProperty = new BackgroundJobBackendPropertyDefinition("jobId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveJobIdQueryProperty = new BackgroundJobBackendPropertyDefinition("activeJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition NameQueryProperty = new BackgroundJobBackendPropertyDefinition("name", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition JobNameQueryProperty = new BackgroundJobBackendPropertyDefinition("jobName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition MachineNameQueryProperty = new BackgroundJobBackendPropertyDefinition("machineName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition LastCheckedDatetimeQueryProperty = new BackgroundJobBackendPropertyDefinition("lastCheckedDatetime", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveQueryProperty = new BackgroundJobBackendPropertyDefinition("active", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);

		internal static readonly BackgroundJobBackendPropertyDefinition RegionSelectionSetQueryProperty = new BackgroundJobBackendPropertyDefinition("regionSelectionSet", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition OwnerIdQueryProperty = new BackgroundJobBackendPropertyDefinition("ownerId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdQueryProperty = new BackgroundJobBackendPropertyDefinition("scheduleId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateQueryProperty = new BackgroundJobBackendPropertyDefinition("taskExecutionState", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingTypeQueryProperty = new BackgroundJobBackendPropertyDefinition("schedulingType", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdQueryProperty = new BackgroundJobBackendPropertyDefinition("taskId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskCompletionStatusQueryProperty = new BackgroundJobBackendPropertyDefinition("taskCompletionStatus", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition DataCenterIdQueryProperty = new BackgroundJobBackendPropertyDefinition("DCId", typeof(long?), PropertyDefinitionFlags.None, null);

		internal static readonly BackgroundJobBackendPropertyDefinition TvpDCIdList = new BackgroundJobBackendPropertyDefinition("tvp_DCIdList", typeof(DataTable), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition DCSelectionSetProperty2 = new BackgroundJobBackendPropertyDefinition("DCSelectionSet2", typeof(long), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0L);

		internal static readonly BackgroundJobBackendPropertyDefinition TvpTaskExecutionStates = new BackgroundJobBackendPropertyDefinition("tvp_TaskExecutionStates", typeof(DataTable), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition TvpTaskExecutionExclusionStates = new BackgroundJobBackendPropertyDefinition("tvp_TaskExecutionExclusionStates", typeof(DataTable), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = new HygienePropertyDefinition("PageSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly BackgroundJobBackendPropertyDefinition heartbeatDatetimeThresholdQueryProperty = new BackgroundJobBackendPropertyDefinition("heartbeatDatetimeThreshold", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateQueryMvpProperty = new BackgroundJobBackendPropertyDefinition("taskExecutionState", typeof(int), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.Mandatory, null);

		private readonly IConfigDataProvider dataProvider;
	}
}
