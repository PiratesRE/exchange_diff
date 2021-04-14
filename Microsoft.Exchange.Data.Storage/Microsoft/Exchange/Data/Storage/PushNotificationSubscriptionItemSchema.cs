using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PushNotificationSubscriptionItemSchema : ItemSchema
	{
		public new static PushNotificationSubscriptionItemSchema Instance
		{
			get
			{
				if (PushNotificationSubscriptionItemSchema.instance == null)
				{
					lock (PushNotificationSubscriptionItemSchema.syncObj)
					{
						if (PushNotificationSubscriptionItemSchema.instance == null)
						{
							PushNotificationSubscriptionItemSchema.instance = new PushNotificationSubscriptionItemSchema();
						}
					}
				}
				return PushNotificationSubscriptionItemSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition SubscriptionId = InternalSchema.PushNotificationSubscriptionId;

		[Autoload]
		public static readonly StorePropertyDefinition LastUpdateTimeUTC = InternalSchema.PushNotificationSubscriptionLastUpdateTimeUTC;

		[Autoload]
		public static readonly StorePropertyDefinition SerializedNotificationSubscription = InternalSchema.PushNotificationSubscription;

		private static readonly object syncObj = new object();

		private static PushNotificationSubscriptionItemSchema instance = null;
	}
}
