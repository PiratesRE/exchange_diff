using System;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueSchema
	{
		internal static readonly SimpleProviderPropertyDefinition SnapshotDatetimeProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("SnapshotDatetime", typeof(DateTime), DateTime.MinValue, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition ServerIdProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ServerId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition ServerNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ServerName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition DagIdProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DagId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition DagNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DagName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition ADSiteIdProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ADSiteId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition ADSiteNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ADSiteName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition ForestIdProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ForestId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition ForestNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("ForestName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition QueueIdProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("QueueId", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition QueueNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("QueueName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition TlsDomainProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("TlsDomain", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition NextHopDomainProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("NextHopDomain", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition NextHopKeyProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("NextHopKey", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition NextHopConnectorProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("NextHopConnector", typeof(Guid));

		internal static readonly SimpleProviderPropertyDefinition NextHopCategoryProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("NextHopCategory", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition DeliveryTypeProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DeliveryType", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition RiskLevelProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("RiskLevel", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition OutboundIPPoolProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("OutboundIPPool", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition StatusProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("Status", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition LastErrorProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("LastError", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition QueueCountProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("QueueCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition MessageCountProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("MessageCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition DeferredMessageCountProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("DeferredMessageCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition LockedMessageCountProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("LockedMessageCount", typeof(int), 0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition IncomingRateProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("IncomingRate", typeof(double), 0.0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition OutgoingRateProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("OutgoingRate", typeof(double), 0.0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition VelocityProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("Velocity", typeof(double), 0.0, PropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly SimpleProviderPropertyDefinition FilterNameProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("FilterName", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition OperatorProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("Operator", typeof(string));

		internal static readonly SimpleProviderPropertyDefinition FilterValueProperty = TransportQueueSchemaHelper.CreatePropertyDefinition("FilterValue", typeof(string));
	}
}
