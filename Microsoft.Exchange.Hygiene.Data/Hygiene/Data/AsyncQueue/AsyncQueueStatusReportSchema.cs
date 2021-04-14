using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueStatusReportSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition FriendlyNameProperty = AsyncQueueCommonSchema.FriendlyNameProperty;

		internal static readonly HygienePropertyDefinition CreatedDatetimeProperty = AsyncQueueCommonSchema.CreatedDatetimeProperty;

		internal static readonly HygienePropertyDefinition StepNameProperty = AsyncQueueCommonSchema.StepNameProperty;

		internal static readonly HygienePropertyDefinition RequestStepIdProperty = AsyncQueueCommonSchema.RequestStepIdProperty;

		internal static readonly HygienePropertyDefinition StepStatusProperty = AsyncQueueCommonSchema.StepStatusProperty;

		internal static readonly HygienePropertyDefinition FetchCountProperty = AsyncQueueCommonSchema.FetchCountProperty;

		internal static readonly HygienePropertyDefinition ErrorCountProperty = AsyncQueueCommonSchema.ErrorCountProperty;

		internal static readonly HygienePropertyDefinition NextFetchDatetimeProperty = AsyncQueueStepSchema.NextFetchDatetimeProperty;

		internal static readonly HygienePropertyDefinition MaxCreationDateTimeQueryProperty = AsyncQueueCommonSchema.MaxCreationDateTimeQueryProperty;

		internal static readonly HygienePropertyDefinition MinCreationDateTimeQueryProperty = AsyncQueueCommonSchema.MinCreationDateTimeQueryProperty;

		internal static readonly HygienePropertyDefinition MinFetchCountProperty = new HygienePropertyDefinition("MinFetchCount", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OwnerIdQueryProperty = AsyncQueueCommonSchema.OwnerIdProperty;

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = AsyncQueueCommonSchema.PageSizeQueryProperty;
	}
}
