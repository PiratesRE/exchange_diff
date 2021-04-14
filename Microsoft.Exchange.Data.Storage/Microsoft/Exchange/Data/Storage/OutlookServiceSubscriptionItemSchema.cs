using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookServiceSubscriptionItemSchema : ItemSchema
	{
		public new static OutlookServiceSubscriptionItemSchema Instance
		{
			get
			{
				if (OutlookServiceSubscriptionItemSchema.instance == null)
				{
					lock (OutlookServiceSubscriptionItemSchema.syncObj)
					{
						if (OutlookServiceSubscriptionItemSchema.instance == null)
						{
							OutlookServiceSubscriptionItemSchema.instance = new OutlookServiceSubscriptionItemSchema();
						}
					}
				}
				return OutlookServiceSubscriptionItemSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition SubscriptionId = InternalSchema.OutlookServiceSubscriptionId;

		[Autoload]
		public static readonly StorePropertyDefinition LastUpdateTimeUTC = InternalSchema.OutlookServiceSubscriptionLastUpdateTimeUTC;

		[Autoload]
		public static readonly StorePropertyDefinition PackageId = InternalSchema.OutlookServicePackageId;

		[Autoload]
		public static readonly StorePropertyDefinition AppId = InternalSchema.OutlookServiceAppId;

		[Autoload]
		public static readonly StorePropertyDefinition DeviceNotificationId = InternalSchema.OutlookServiceDeviceNotificationId;

		[Autoload]
		public static readonly StorePropertyDefinition ExpirationTime = InternalSchema.OutlookServiceExpirationTime;

		[Autoload]
		public static readonly StorePropertyDefinition LockScreen = InternalSchema.OutlookServiceLockScreen;

		private static readonly object syncObj = new object();

		private static OutlookServiceSubscriptionItemSchema instance = null;
	}
}
