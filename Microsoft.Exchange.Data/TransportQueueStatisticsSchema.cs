using System;
using System.Data;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueStatisticsSchema : ObjectSchema
	{
		internal static readonly SimpleProviderPropertyDefinition ServerNameProperty = TransportQueueSchema.ServerNameProperty;

		internal static readonly SimpleProviderPropertyDefinition TlsDomainProperty = TransportQueueSchema.TlsDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopDomainProperty = TransportQueueSchema.NextHopDomainProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopKeyProperty = TransportQueueSchema.NextHopKeyProperty;

		internal static readonly SimpleProviderPropertyDefinition NextHopCategoryProperty = TransportQueueSchema.NextHopCategoryProperty;

		internal static readonly SimpleProviderPropertyDefinition DeliveryTypeProperty = TransportQueueSchema.DeliveryTypeProperty;

		internal static readonly SimpleProviderPropertyDefinition RiskLevelProperty = TransportQueueSchema.RiskLevelProperty;

		internal static readonly SimpleProviderPropertyDefinition OutboundIPPoolProperty = TransportQueueSchema.OutboundIPPoolProperty;

		internal static readonly SimpleProviderPropertyDefinition StatusProperty = TransportQueueSchema.StatusProperty;

		internal static readonly SimpleProviderPropertyDefinition LastErrorProperty = TransportQueueSchema.LastErrorProperty;

		internal static readonly SimpleProviderPropertyDefinition QueueCountProperty = TransportQueueSchema.QueueCountProperty;

		internal static readonly SimpleProviderPropertyDefinition MessageCountProperty = TransportQueueSchema.MessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition DeferredMessageCountProperty = TransportQueueSchema.DeferredMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition LockedMessageCountProperty = TransportQueueSchema.LockedMessageCountProperty;

		internal static readonly SimpleProviderPropertyDefinition IncomingRateProperty = TransportQueueSchema.IncomingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition OutgoingRateProperty = TransportQueueSchema.OutgoingRateProperty;

		internal static readonly SimpleProviderPropertyDefinition VelocityProperty = TransportQueueSchema.VelocityProperty;

		internal static readonly SimpleProviderPropertyDefinition TransportQueueLogsProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("QueueLogs", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition ForestIdQueryProperty = TransportQueueSchema.ForestIdProperty;

		internal static readonly SimpleProviderPropertyDefinition ServerQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ServerFilters", typeof(DataTable));

		internal static readonly SimpleProviderPropertyDefinition DagQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DagFilters", typeof(DataTable));

		internal static readonly SimpleProviderPropertyDefinition ADSiteQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ADSiteFilters", typeof(DataTable));

		internal static readonly SimpleProviderPropertyDefinition DataFilterQueryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DataFilters", typeof(DataTable));

		internal static readonly SimpleProviderPropertyDefinition AggregatedByQueryProperty = TransportQueueQuerySchema.AggregatedByQueryProperty;

		internal static readonly SimpleProviderPropertyDefinition DetailsLevelQueryProperty = TransportQueueQuerySchema.DetailsLevelQueryProperty;

		internal static readonly SimpleProviderPropertyDefinition PageSizeQueryProperty = TransportQueueQuerySchema.PageSizeQueryProperty;

		internal static readonly SimpleProviderPropertyDefinition DetailsResultSizeQueryProperty = TransportQueueQuerySchema.DetailsResultSizeQueryProperty;

		internal static readonly SimpleProviderPropertyDefinition FreshnessCutoffTimeSecondsProperty = TransportQueueQuerySchema.FreshnessCutoffTimeSecondsProperty;
	}
}
