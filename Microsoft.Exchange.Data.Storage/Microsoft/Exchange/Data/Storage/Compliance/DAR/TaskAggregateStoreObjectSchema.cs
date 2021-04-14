using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskAggregateStoreObjectSchema : EwsStoreObjectSchema
	{
		public static readonly EwsStoreObjectPropertyDefinition Id = EwsStoreObjectSchema.AlternativeId;

		public static readonly EwsStoreObjectPropertyDefinition LastModifiedTime = TaskStoreObjectSchema.CreateEwsDefinitionFromServiceDefinition(ItemSchema.LastModifiedTime, "LastModifiedTime", null);

		public static readonly EwsStoreObjectPropertyDefinition Enabled = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskAggregateStoreObjectExtendedStoreSchema.Enabled, null);

		public static readonly EwsStoreObjectPropertyDefinition TaskType = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TaskType, null);

		public static readonly EwsStoreObjectPropertyDefinition ScopeId = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.TenantId, null);

		public static readonly EwsStoreObjectPropertyDefinition MaxRunningTasks = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskAggregateStoreObjectExtendedStoreSchema.MaxRunningTasks, null);

		public static readonly EwsStoreObjectPropertyDefinition RecurrenceType = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskAggregateStoreObjectExtendedStoreSchema.RecurrenceType, null);

		public static readonly EwsStoreObjectPropertyDefinition RecurrenceFrequency = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskAggregateStoreObjectExtendedStoreSchema.RecurrenceFrequency, null);

		public static readonly EwsStoreObjectPropertyDefinition RecurrenceInterval = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskAggregateStoreObjectExtendedStoreSchema.RecurrenceInterval, null);

		public static readonly EwsStoreObjectPropertyDefinition SchemaVersion = TaskStoreObjectSchema.CreateEwsPropertyDefinition(TaskStoreObjectExtendedStoreSchema.SchemaVersion, 0);
	}
}
