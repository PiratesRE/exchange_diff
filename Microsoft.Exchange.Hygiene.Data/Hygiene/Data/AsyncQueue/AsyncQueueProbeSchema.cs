using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueProbeSchema
	{
		internal static readonly HygienePropertyDefinition OwnerID = AsyncQueueCommonSchema.OwnerIdProperty;

		internal static readonly HygienePropertyDefinition Priority = AsyncQueueCommonSchema.PriorityProperty;

		internal static readonly HygienePropertyDefinition RequestId = new HygienePropertyDefinition("RequestId", typeof(Guid));

		internal static readonly HygienePropertyDefinition StepName = AsyncQueueCommonSchema.StepNameProperty;

		internal static readonly HygienePropertyDefinition StepNumber = AsyncQueueCommonSchema.StepNumberProperty;

		internal static readonly HygienePropertyDefinition BitFlags = AsyncQueueCommonSchema.StepFlagsProperty;

		internal static readonly HygienePropertyDefinition Ordinal = AsyncQueueCommonSchema.StepOrdinalProperty;

		internal static readonly HygienePropertyDefinition Status = AsyncQueueCommonSchema.StepStatusProperty;

		internal static readonly HygienePropertyDefinition FetchCount = AsyncQueueCommonSchema.FetchCountProperty;

		internal static readonly HygienePropertyDefinition ErrorCount = AsyncQueueCommonSchema.ErrorCountProperty;

		internal static readonly HygienePropertyDefinition NextFetchDatetime = new HygienePropertyDefinition("NextFetchDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition CreatedDatetime = new HygienePropertyDefinition("CreatedDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition ChangedDatetime = new HygienePropertyDefinition("ChangedDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition InprogressBatchSize = new HygienePropertyDefinition("InprogressBatchSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition BatchSize = new HygienePropertyDefinition("BatchSize", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProocessInprogressBackInSeconds = new HygienePropertyDefinition("ProocessInprogressBackInSeconds", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProocessBackInSeconds = new HygienePropertyDefinition("ProocessBackInSeconds", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
