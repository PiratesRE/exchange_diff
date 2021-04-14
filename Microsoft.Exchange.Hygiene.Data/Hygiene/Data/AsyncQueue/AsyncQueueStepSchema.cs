using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueStepSchema
	{
		internal static readonly HygienePropertyDefinition PriorityProperty = AsyncQueueCommonSchema.PriorityProperty;

		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition DependantRequestIdProperty = AsyncQueueCommonSchema.DependantRequestIdProperty;

		internal static readonly HygienePropertyDefinition RequestCookieProperty = AsyncQueueCommonSchema.RequestCookieProperty;

		internal static readonly HygienePropertyDefinition RequestStepIdProperty = AsyncQueueCommonSchema.RequestStepIdProperty;

		internal static readonly HygienePropertyDefinition OwnerIdProperty = AsyncQueueCommonSchema.OwnerIdProperty;

		internal static readonly HygienePropertyDefinition FriendlyNameProperty = AsyncQueueCommonSchema.FriendlyNameProperty;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition StepNumberProperty = AsyncQueueCommonSchema.StepNumberProperty;

		internal static readonly HygienePropertyDefinition StepNameProperty = AsyncQueueCommonSchema.StepNameProperty;

		internal static readonly HygienePropertyDefinition StepOrdinalProperty = AsyncQueueCommonSchema.StepOrdinalProperty;

		internal static readonly HygienePropertyDefinition FlagsProperty = AsyncQueueCommonSchema.StepFlagsProperty;

		internal static readonly HygienePropertyDefinition StepStatusProperty = AsyncQueueCommonSchema.StepStatusProperty;

		internal static readonly HygienePropertyDefinition FetchCountProperty = AsyncQueueCommonSchema.FetchCountProperty;

		internal static readonly HygienePropertyDefinition ErrorCountProperty = AsyncQueueCommonSchema.ErrorCountProperty;

		internal static readonly HygienePropertyDefinition ProcessInstanceNameProperty = AsyncQueueCommonSchema.ProcessInstanceNameProperty;

		internal static readonly HygienePropertyDefinition MaxExecutionTimeProperty = AsyncQueueCommonSchema.MaxExecutionTimeProperty;

		internal static readonly HygienePropertyDefinition CookieProperty = AsyncQueueCommonSchema.CookieProperty;

		internal static readonly HygienePropertyDefinition ExecutionStateProperty = new HygienePropertyDefinition("ExecutionState", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NextFetchDatetimeProperty = new HygienePropertyDefinition("NextFetchDatetime", typeof(DateTime?));
	}
}
