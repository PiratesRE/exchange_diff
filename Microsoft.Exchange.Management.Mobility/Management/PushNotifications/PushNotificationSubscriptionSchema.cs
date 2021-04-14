using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.PushNotifications
{
	internal class PushNotificationSubscriptionSchema : SimpleProviderObjectSchema
	{
		internal static SimpleProviderPropertyDefinition SubscriptionStoreId = new SimpleProviderPropertyDefinition("SubscriptionStoreId", ExchangeObjectVersion.Exchange2003, typeof(PushNotificationStoreId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition SubscriptionId = new SimpleProviderPropertyDefinition("SubscriptionId", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.None, string.Empty, new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyConstraint()
		}, new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyConstraint()
		});

		internal static SimpleProviderPropertyDefinition DeserializedSubscription = new SimpleProviderPropertyDefinition("DeserializedSubscription", ExchangeObjectVersion.Exchange2003, typeof(PushNotificationServerSubscription), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		internal static SimpleProviderPropertyDefinition AppId = new SimpleProviderPropertyDefinition("AppId", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PushNotificationSubscriptionSchema.DeserializedSubscription
		}, null, new GetterDelegate(PushNotificationSubscription.AppIdGetter), null);

		internal static SimpleProviderPropertyDefinition DeviceNotificationId = new SimpleProviderPropertyDefinition("DeviceNotificationId", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PushNotificationSubscriptionSchema.DeserializedSubscription
		}, null, new GetterDelegate(PushNotificationSubscription.DeviceNotificationIdGetter), null);

		internal static SimpleProviderPropertyDefinition DeviceNotificationType = new SimpleProviderPropertyDefinition("DeviceNotificationType", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PushNotificationSubscriptionSchema.DeserializedSubscription
		}, null, new GetterDelegate(PushNotificationSubscription.DeviceNotificationTypeGetter), null);

		internal static SimpleProviderPropertyDefinition InboxUnreadCount = new SimpleProviderPropertyDefinition("InboxUnreadCount", ExchangeObjectVersion.Exchange2003, typeof(long?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PushNotificationSubscriptionSchema.DeserializedSubscription
		}, null, new GetterDelegate(PushNotificationSubscription.InboxUnreadCountGetter), null);

		internal static SimpleProviderPropertyDefinition LastSubscriptionUpdate = new SimpleProviderPropertyDefinition("LastSubscriptionUpdate", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PushNotificationSubscriptionSchema.DeserializedSubscription
		}, null, new GetterDelegate(PushNotificationSubscription.LastSubscriptionUpdateGetter), null);
	}
}
