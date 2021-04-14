using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataLatencyDetailSchema
	{
		public static readonly HygienePropertyDefinition Identity = new HygienePropertyDefinition("Identity", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition TemporalPartition = new HygienePropertyDefinition("TemporalPartition", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition RowCount = new HygienePropertyDefinition("RowCount", typeof(long), -1L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition StartDateParameter = new HygienePropertyDefinition("startDate", typeof(DateTime?));

		public static readonly HygienePropertyDefinition PartitionCountParameter = new HygienePropertyDefinition("partitionCount", typeof(int), 7, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
