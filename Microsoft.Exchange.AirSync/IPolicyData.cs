using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal interface IPolicyData
	{
		ADObjectId Identity { get; }

		bool AllowNonProvisionableDevices { get; }

		bool AlphanumericDevicePasswordRequired { get; }

		bool AttachmentsEnabled { get; }

		bool RequireStorageCardEncryption { get; }

		bool DevicePasswordEnabled { get; }

		bool PasswordRecoveryEnabled { get; }

		Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval { get; }

		bool AllowSimpleDevicePassword { get; }

		Unlimited<ByteQuantifiedSize> MaxAttachmentSize { get; }

		bool WSSAccessEnabled { get; }

		bool UNCAccessEnabled { get; }

		int? MinDevicePasswordLength { get; }

		Unlimited<EnhancedTimeSpan> MaxInactivityTimeDeviceLock { get; }

		Unlimited<int> MaxDevicePasswordFailedAttempts { get; }

		Unlimited<EnhancedTimeSpan> DevicePasswordExpiration { get; }

		int DevicePasswordHistory { get; }

		bool IsDefault { get; }

		bool AllowStorageCard { get; }

		bool AllowCamera { get; }

		bool RequireDeviceEncryption { get; }

		bool AllowUnsignedApplications { get; }

		bool AllowUnsignedInstallationPackages { get; }

		bool AllowWiFi { get; }

		bool AllowTextMessaging { get; }

		bool AllowPOPIMAPEmail { get; }

		bool AllowIrDA { get; }

		bool RequireManualSyncWhenRoaming { get; }

		bool AllowDesktopSync { get; }

		bool AllowHTMLEmail { get; }

		bool RequireSignedSMIMEMessages { get; }

		bool RequireEncryptedSMIMEMessages { get; }

		bool AllowSMIMESoftCerts { get; }

		bool AllowBrowser { get; }

		bool AllowConsumerEmail { get; }

		bool AllowRemoteDesktop { get; }

		bool AllowInternetSharing { get; }

		BluetoothType AllowBluetooth { get; }

		CalendarAgeFilterType MaxCalendarAgeFilter { get; }

		EmailAgeFilterType MaxEmailAgeFilter { get; }

		SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm { get; }

		EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm { get; }

		SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation { get; }

		int MinDevicePasswordComplexCharacters { get; }

		Unlimited<int> MaxEmailBodyTruncationSize { get; }

		Unlimited<int> MaxEmailHTMLBodyTruncationSize { get; }

		MultiValuedProperty<string> UnapprovedInROMApplicationList { get; }

		ApprovedApplicationCollection ApprovedApplicationList { get; }

		bool AllowExternalDeviceManagement { get; }

		bool IsIrmEnabled { get; }

		MobileOTAUpdateModeType MobileOTAUpdateMode { get; }

		bool AllowMobileOTAUpdate { get; }

		bool GetVersionCompatibility(int version);

		int GetHashCode(int version);
	}
}
