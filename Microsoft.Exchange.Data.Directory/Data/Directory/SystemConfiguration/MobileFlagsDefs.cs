using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum MobileFlagsDefs
	{
		PasswordEnabled = 2,
		AlphanumericPasswordRequired = 4,
		PasswordRecoveryEnabled = 16,
		RequireStorageCardEncryption = 32,
		AttachmentsEnabled = 64,
		IsDefault = 4096,
		AllowNonProvisionableDevices = 131072,
		AllowSimplePassword = 262144,
		WSSAccessEnabled = 1048576,
		UNCAccessEnabled = 2097152,
		DenyMicrosoftPushNotifications = 16777216,
		DenyApplePushNotifications = 33554432,
		DenyGooglePushNotifications = 67108864,
		AllowPendingGetNotifications = 134217728
	}
}
