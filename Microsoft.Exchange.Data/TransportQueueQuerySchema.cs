using System;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueQuerySchema
	{
		internal static readonly SimpleProviderPropertyDefinition ForestIdQueryProperty = TransportQueueSchema.ForestIdProperty;

		internal static readonly SimpleProviderPropertyDefinition AggregatedByQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("AggregatedBy", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition DetailsLevelQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DetailsLevel", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition PageSizeQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("PageSize", typeof(int), 100, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition DetailsResultSizeQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DetailsResultSize", typeof(int), 20, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition FreshnessCutoffTimeSecondsProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("cutOffTimeSeconds", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition TlsDomainQueryProperty = TransportQueueSchema.TlsDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopDomainQueryProperty = TransportQueueSchema.NextHopDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopKeyQueryProperty = TransportQueueSchema.NextHopKeyProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopConnectorQueryProperty = TransportQueueSchema.NextHopConnectorProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopCategoryQueryProperty = TransportQueueSchema.NextHopCategoryProperty;

		internal static readonly SimpleProviderPropertyDefinition DeliveryTypeQueryProperty = TransportQueueSchema.DeliveryTypeProperty;

		internal static readonly SimpleProviderPropertyDefinition RiskLevelQueryProperty = TransportQueueSchema.RiskLevelProperty;

		internal static readonly SimpleProviderPropertyDefinition OutboundIPPoolQueryProperty = TransportQueueSchema.OutboundIPPoolProperty;

		internal static readonly SimpleProviderPropertyDefinition StatusQueryProperty = TransportQueueSchema.StatusProperty;

		internal static readonly SimpleProviderPropertyDefinition LastErrorQueryProperty = TransportQueueSchema.LastErrorProperty;

		internal static readonly SimpleProviderPropertyDefinition MessageCountQueryProperty = TransportQueueSchema.MessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition DeferredMessageCountQueryProperty = TransportQueueSchema.DeferredMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition LockedMessageCountQueryProperty = TransportQueueSchema.LockedMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition IncomingRateQueryProperty = TransportQueueSchema.IncomingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition OutgoingRateQueryProperty = TransportQueueSchema.OutgoingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition VelocityQueryProperty = TransportQueueSchema.VelocityProperty;
	}
}
