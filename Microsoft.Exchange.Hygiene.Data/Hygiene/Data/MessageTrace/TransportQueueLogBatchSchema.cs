using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class TransportQueueLogBatchSchema
	{
		internal static readonly SimpleProviderPropertyDefinition SnapshotDatetimeProperty = TransportQueueSchema.SnapshotDatetimeProperty;

		internal static readonly SimpleProviderPropertyDefinition ServerIdProperty = TransportQueueSchema.ServerIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ServerNameProperty = TransportQueueSchema.ServerNameProperty;

		internal static readonly SimpleProviderPropertyDefinition DagIdProperty = TransportQueueSchema.DagIdProperty;

		internal static readonly SimpleProviderPropertyDefinition DagNameProperty = TransportQueueSchema.DagNameProperty;

		internal static readonly SimpleProviderPropertyDefinition ADSiteIdProperty = TransportQueueSchema.ADSiteIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ADSiteNameProperty = TransportQueueSchema.ADSiteNameProperty;

		internal static readonly SimpleProviderPropertyDefinition ForestIdProperty = TransportQueueSchema.ForestIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ForestNameProperty = TransportQueueSchema.ForestNameProperty;

		internal static readonly HygienePropertyDefinition QueueLogProperty = new HygienePropertyDefinition("queueLogProperties", typeof(object), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
