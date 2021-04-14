using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal static class BrokerSubscriptionSchema
	{
		private static GuidNamePropertyDefinition CreatePropertyDefinition(string propertyName, Type propertyType)
		{
			return GuidNamePropertyDefinition.CreateCustom(propertyName, propertyType, BrokerSubscriptionSchema.PropertySetGuid, propertyName, PropertyFlags.None);
		}

		public static readonly Guid PropertySetGuid = new Guid("4CB44F05-36EA-40E2-87E1-9581CCE7CC6F");

		public static readonly GuidNamePropertyDefinition SubscriptionId = BrokerSubscriptionSchema.CreatePropertyDefinition("SubscriptionId", typeof(Guid));

		public static readonly GuidNamePropertyDefinition ConsumerId = BrokerSubscriptionSchema.CreatePropertyDefinition("ConsumerId", typeof(string));

		public static readonly GuidNamePropertyDefinition ChannelId = BrokerSubscriptionSchema.CreatePropertyDefinition("ChannelId", typeof(string));

		public static readonly GuidNamePropertyDefinition Expiration = BrokerSubscriptionSchema.CreatePropertyDefinition("Expiration", typeof(ExDateTime));

		public static readonly GuidNamePropertyDefinition ReceiverMailboxGuid = BrokerSubscriptionSchema.CreatePropertyDefinition("ReceiverMailboxGuid", typeof(Guid));

		public static readonly GuidNamePropertyDefinition ReceiverMailboxSmtp = BrokerSubscriptionSchema.CreatePropertyDefinition("ReceiverMailboxSmtp", typeof(string));

		public static readonly GuidNamePropertyDefinition ReceiverUrl = BrokerSubscriptionSchema.CreatePropertyDefinition("ReceiverUrl", typeof(string));

		public static readonly GuidNamePropertyDefinition Parameters = BrokerSubscriptionSchema.CreatePropertyDefinition("Parameters", typeof(string));
	}
}
