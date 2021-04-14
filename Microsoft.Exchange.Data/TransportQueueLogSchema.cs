using System;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueLogSchema : ObjectSchema
	{
		internal static readonly SimpleProviderPropertyDefinition ServerIdProperty = TransportQueueSchema.ServerIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ServerNameProperty = TransportQueueSchema.ServerNameProperty;

		internal static readonly SimpleProviderPropertyDefinition DagIdProperty = TransportQueueSchema.DagIdProperty;

		internal static readonly SimpleProviderPropertyDefinition DagNameProperty = TransportQueueSchema.DagNameProperty;

		internal static readonly SimpleProviderPropertyDefinition ADSiteIdProperty = TransportQueueSchema.ADSiteIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ADSiteNameProperty = TransportQueueSchema.ADSiteNameProperty;

		internal static readonly SimpleProviderPropertyDefinition ForestIdProperty = TransportQueueSchema.ForestIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ForestNameProperty = TransportQueueSchema.ForestNameProperty;

		internal static readonly SimpleProviderPropertyDefinition QueueIdProperty = TransportQueueSchema.QueueIdProperty;

		internal static readonly SimpleProviderPropertyDefinition QueueNameProperty = TransportQueueSchema.QueueNameProperty;

		internal static readonly SimpleProviderPropertyDefinition SnapshotDatetimeProperty = TransportQueueSchema.SnapshotDatetimeProperty;

		internal static readonly SimpleProviderPropertyDefinition TlsDomainProperty = TransportQueueSchema.TlsDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopDomainProperty = TransportQueueSchema.NextHopDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopKeyProperty = TransportQueueSchema.NextHopKeyProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopConnectorProperty = TransportQueueSchema.NextHopConnectorProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopCategoryProperty = TransportQueueSchema.NextHopCategoryProperty;

		internal static readonly SimpleProviderPropertyDefinition DeliveryTypeProperty = TransportQueueSchema.DeliveryTypeProperty;

		internal static readonly SimpleProviderPropertyDefinition RiskLevelProperty = TransportQueueSchema.RiskLevelProperty;

		internal static readonly SimpleProviderPropertyDefinition OutboundIPPoolProperty = TransportQueueSchema.OutboundIPPoolProperty;

		internal static readonly SimpleProviderPropertyDefinition StatusProperty = TransportQueueSchema.StatusProperty;

		internal static readonly SimpleProviderPropertyDefinition LastErrorProperty = TransportQueueSchema.LastErrorProperty;

		internal static readonly SimpleProviderPropertyDefinition MessageCountProperty = TransportQueueSchema.MessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition DeferredMessageCountProperty = TransportQueueSchema.DeferredMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition LockedMessageCountProperty = TransportQueueSchema.LockedMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition IncomingRateProperty = TransportQueueSchema.IncomingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition OutgoingRateProperty = TransportQueueSchema.OutgoingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition VelocityProperty = TransportQueueSchema.VelocityProperty;
	}
}
