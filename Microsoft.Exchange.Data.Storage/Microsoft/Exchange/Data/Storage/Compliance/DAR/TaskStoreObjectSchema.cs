using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskStoreObjectSchema : EwsStoreObjectSchema
	{
		public static GuidNamePropertyDefinition CreateStorePropertyDefinition(EwsStoreObjectPropertyDefinition ewsStorePropertyDefinition)
		{
			return GuidNamePropertyDefinition.CreateCustom(ewsStorePropertyDefinition.Name, ewsStorePropertyDefinition.IsMultivalued ? ewsStorePropertyDefinition.Type.MakeArrayType() : ewsStorePropertyDefinition.Type, ((ExtendedPropertyDefinition)ewsStorePropertyDefinition.StorePropertyDefinition).PropertySetId.Value, ewsStorePropertyDefinition.Name, PropertyFlags.None);
		}

		public static EwsStoreObjectPropertyDefinition CreateEwsPropertyDefinition(ExtendedPropertyDefinition definition, object defaultValue = null)
		{
			if (defaultValue == null && definition.Type.IsValueType)
			{
				defaultValue = Activator.CreateInstance(definition.Type);
			}
			return new EwsStoreObjectPropertyDefinition(definition.Name, ExchangeObjectVersion.Exchange2012, definition.Type, PropertyDefinitionFlags.ReturnOnBind, defaultValue, defaultValue, definition);
		}

		public static EwsStoreObjectPropertyDefinition CreateEwsDefinitionFromServiceDefinition(ServiceObjectPropertyDefinition definition, string name, object defaultValue = null)
		{
			return new EwsStoreObjectPropertyDefinition(name, ExchangeObjectVersion.Exchange2012, definition.Type, PropertyDefinitionFlags.ReadOnly, defaultValue, defaultValue, definition);
		}

		internal const int CurrentSchemaVersion = 0;

		public static readonly EwsStoreObjectPropertyDefinition Id = EwsStoreObjectSchema.AlternativeId;

		public static readonly EwsStoreObjectPropertyDefinition LastModifiedTime = TaskStoreObjectSchema.CreateEwsDefinitionFromServiceDefinition(ItemSchema.LastModifiedTime, "LastModifiedTime", null);

		public static readonly EwsStoreObjectPropertyDefinition Category = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.Category, null);

		public static readonly EwsStoreObjectPropertyDefinition Priority = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.Priority, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskState = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskState, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskType = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskType, null);

		public static readonly EwsStoreObjectPropertyDefinition SerializedTaskData = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.SerializedTaskData, null);

		public static readonly EwsStoreObjectPropertyDefinition TenantId = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TenantId, null);

		public static readonly EwsStoreObjectPropertyDefinition MinTaskScheduleTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.MinTaskScheduleTime, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskCompletionTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskCompletionTime, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskExecutionStartTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskExecutionStartTime, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskQueuedTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskQueuedTime, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskScheduledTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskScheduledTime, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskRetryInterval = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskRetryInterval, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskRetryCurrentCount = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskRetryCurrentCount, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskRetryTotalCount = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskRetryTotalCount, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskSynchronizationOption = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskSynchronizationOption, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskSynchronizationKey = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskSynchronizationKey, null);

		public static readonly EwsStoreObjectPropertyDefinition ExecutionContainer = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.ExecutionContainer, null);

		public static readonly EwsStoreObjectPropertyDefinition ExecutionTarget = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.ExecutionTarget, null);

		public static readonly EwsStoreObjectPropertyDefinition ExecutionLockExpiryTime = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.ExecutionLockExpiryTime, null);

		public static readonly EwsStoreObjectPropertyDefinition SchemaVersion = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.SchemaVersion, 0);
	}
}
