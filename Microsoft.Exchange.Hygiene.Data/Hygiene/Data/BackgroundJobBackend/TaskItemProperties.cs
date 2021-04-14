using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal static class TaskItemProperties
	{
		internal static readonly BackgroundJobBackendPropertyDefinition ActiveJobIdProperty = new BackgroundJobBackendPropertyDefinition("ActiveJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskIdProperty = new BackgroundJobBackendPropertyDefinition("TaskId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdProperty = new BackgroundJobBackendPropertyDefinition("ScheduleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = new BackgroundJobBackendPropertyDefinition("BackgroundJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = new BackgroundJobBackendPropertyDefinition("RoleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition InstanceIdProperty = new BackgroundJobBackendPropertyDefinition("InstanceId", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskExecutionStateProperty = new BackgroundJobBackendPropertyDefinition("TaskExecutionState", typeof(byte), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TaskCompletionStatusProperty = new BackgroundJobBackendPropertyDefinition("TaskCompletionStatus", typeof(byte?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition ParentTaskIdProperty = new BackgroundJobBackendPropertyDefinition("ParentTaskId", typeof(Guid), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition ExecutionAttemptProperty = new BackgroundJobBackendPropertyDefinition("ExecutionAttempt", typeof(short), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition BJMOwnerIdProperty = new BackgroundJobBackendPropertyDefinition("BJMOwnerId", typeof(Guid?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition OwnerFitnessScoreProperty = new BackgroundJobBackendPropertyDefinition("OwnerFitnessScore", typeof(int?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition StartTimeProperty = new BackgroundJobBackendPropertyDefinition("StartTime", typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition EndTimeProperty = new BackgroundJobBackendPropertyDefinition("EndTime", typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = new BackgroundJobBackendPropertyDefinition("Heartbeat", typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition InsertTimeStampProperty = new BackgroundJobBackendPropertyDefinition("InsertTimeStamp", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));
	}
}
