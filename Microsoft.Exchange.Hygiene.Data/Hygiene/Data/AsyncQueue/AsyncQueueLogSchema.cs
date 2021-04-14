using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLogSchema
	{
		internal static readonly HygienePropertyDefinition StepTransactionIdProperty = new HygienePropertyDefinition("StepTransactionId", typeof(Guid));

		internal static readonly HygienePropertyDefinition PriorityProperty = AsyncQueueCommonSchema.PriorityProperty;

		internal static readonly HygienePropertyDefinition RequestStepIdProperty = AsyncQueueCommonSchema.RequestStepIdProperty;

		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition DependantRequestIdProperty = AsyncQueueCommonSchema.DependantRequestIdProperty;

		internal static readonly HygienePropertyDefinition OwnerIdProperty = AsyncQueueCommonSchema.OwnerIdProperty;

		internal static readonly HygienePropertyDefinition FriendlyNameProperty = AsyncQueueCommonSchema.FriendlyNameProperty;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition StepNumberProperty = AsyncQueueCommonSchema.StepNumberProperty;

		internal static readonly HygienePropertyDefinition StepNameProperty = AsyncQueueCommonSchema.StepNameProperty;

		internal static readonly HygienePropertyDefinition StepOrdinalProperty = AsyncQueueCommonSchema.StepOrdinalProperty;

		internal static readonly HygienePropertyDefinition StepFromStatusProperty = new HygienePropertyDefinition("FromStatus", typeof(AsyncQueueStatus));

		internal static readonly HygienePropertyDefinition StepStatusProperty = new HygienePropertyDefinition("Status", typeof(AsyncQueueStatus));

		internal static readonly HygienePropertyDefinition FetchCountProperty = AsyncQueueCommonSchema.FetchCountProperty;

		internal static readonly HygienePropertyDefinition ErrorCountProperty = AsyncQueueCommonSchema.ErrorCountProperty;

		internal static readonly HygienePropertyDefinition ProcessInstanceNameProperty = AsyncQueueCommonSchema.ProcessInstanceNameProperty;

		internal static readonly HygienePropertyDefinition ProcessStartDatetimeProperty = new HygienePropertyDefinition("ProcessStartDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProcessEndDatetimeProperty = new HygienePropertyDefinition("ProcessEndDatetime", typeof(DateTime?));
	}
}
