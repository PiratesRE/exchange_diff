using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class TransportQueueLogSaveDataSetSchema
	{
		internal static readonly SimpleProviderPropertyDefinition ForestIdProperty = TransportQueueSchema.ForestIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ServerIdProperty = TransportQueueSchema.ServerIdProperty;

		internal static readonly SimpleProviderPropertyDefinition SnapshotDatetimeProperty = TransportQueueSchema.SnapshotDatetimeProperty;

		internal static readonly HygienePropertyDefinition ServerPropertiesTableProperty = new HygienePropertyDefinition("properties", typeof(DataTable));

		internal static readonly HygienePropertyDefinition QueueLogPropertiesTableProperty = new HygienePropertyDefinition("queueLogProperties", typeof(DataTable));
	}
}
