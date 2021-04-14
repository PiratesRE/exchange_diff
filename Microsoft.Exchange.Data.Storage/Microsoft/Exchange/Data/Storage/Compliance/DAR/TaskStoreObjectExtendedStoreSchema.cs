using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskStoreObjectExtendedStoreSchema : ObjectSchema
	{
		public static readonly ExtendedPropertyDefinition Priority = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "Priority", 14);

		public static readonly ExtendedPropertyDefinition Category = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "Category", 14);

		public static readonly ExtendedPropertyDefinition TaskState = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskState", 14);

		public static readonly ExtendedPropertyDefinition TaskType = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskType", 25);

		public static readonly ExtendedPropertyDefinition SerializedTaskData = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "SerializedTaskData", 25);

		public static readonly ExtendedPropertyDefinition TenantId = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TenantId", 2);

		public static readonly ExtendedPropertyDefinition MinTaskScheduleTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "MinTaskScheduleTime", 23);

		public static readonly ExtendedPropertyDefinition TaskCompletionTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskCompletionTime", 23);

		public static readonly ExtendedPropertyDefinition TaskExecutionStartTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskExecutionStartTime", 23);

		public static readonly ExtendedPropertyDefinition TaskQueuedTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskQueuedTime", 23);

		public static readonly ExtendedPropertyDefinition TaskScheduledTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskScheduledTime", 23);

		public static readonly ExtendedPropertyDefinition TaskRetryInterval = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskRetryInterval", 25);

		public static readonly ExtendedPropertyDefinition TaskRetryCurrentCount = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskRetryCurrentCount", 14);

		public static readonly ExtendedPropertyDefinition TaskRetryTotalCount = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskRetryTotalCount", 14);

		public static readonly ExtendedPropertyDefinition TaskSynchronizationOption = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskSynchronizationOption", 14);

		public static readonly ExtendedPropertyDefinition TaskSynchronizationKey = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "TaskSynchronizationKey", 25);

		public static readonly ExtendedPropertyDefinition ExecutionContainer = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "ExecutionContainer", 25);

		public static readonly ExtendedPropertyDefinition ExecutionTarget = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "ExecutionTarget", 25);

		public static readonly ExtendedPropertyDefinition ExecutionLockExpiryTime = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "ExecutionLockExpiryTime", 23);

		public static readonly ExtendedPropertyDefinition SchemaVersion = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "SchemaVersion", 14);
	}
}
