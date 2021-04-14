using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum MobileAdditionalFlagsDefs
	{
		AllowStorageCard = 1,
		AllowCamera = 2,
		RequireDeviceEncryption = 4,
		AllowUnsignedApplications = 8,
		AllowUnsignedInstallationPackages = 16,
		MinPasswordComplexCharacters = 32,
		AllowWiFi = 64,
		AllowTextMessaging = 128,
		AllowPOPIMAPEmail = 256,
		AllowIrDA = 512,
		RequireManualSyncWhenRoaming = 1024,
		AllowDesktopSync = 2048,
		AllowHTMLEmail = 4096,
		RequireSignedSMIMEMessages = 8192,
		RequireEncryptedSMIMEMessages = 16384,
		AllowSMIMESoftCerts = 32768,
		AllowBrowser = 65536,
		AllowConsumerEmail = 131072,
		AllowRemoteDesktop = 262144,
		AllowInternetSharing = 524288,
		AllowExternalDeviceManagement = 1048576,
		AllowMobileOTAUpdate = 2097152,
		IrmEnabled = 4194304
	}
}
