using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal interface IGlobalInfo
	{
		int? LastPolicyXMLHash { get; set; }

		ExDateTime? NextTimeToClearMailboxLogs { get; set; }

		uint PolicyKeyNeeded { get; set; }

		uint PolicyKeyWaitingAck { get; set; }

		uint PolicyKeyOnDevice { get; set; }

		bool ProvisionSupported { get; set; }

		ExDateTime? LastPolicyTime { get; set; }

		ExDateTime? LastSyncAttemptTime { get; set; }

		ExDateTime? LastSyncSuccessTime { get; set; }

		ExDateTime? RemoteWipeRequestedTime { get; set; }

		ExDateTime? RemoteWipeSentTime { get; set; }

		ExDateTime? RemoteWipeAckTime { get; set; }

		string DeviceModel { get; set; }

		string DeviceImei { get; set; }

		string DeviceFriendlyName { get; set; }

		string DeviceOS { get; set; }

		string DeviceOSLanguage { get; set; }

		string DevicePhoneNumber { get; set; }

		string UserAgent { get; set; }

		bool DeviceEnableOutboundSMS { get; set; }

		string DeviceMobileOperator { get; set; }

		string RecoveryPassword { get; set; }

		DeviceAccessState DeviceAccessState { get; set; }

		DeviceAccessStateReason DeviceAccessStateReason { get; set; }

		DevicePolicyApplicationStatus DevicePolicyApplicationStatus { get; set; }

		ADObjectId DevicePolicyApplied { get; set; }

		ADObjectId DeviceAccessControlRule { get; set; }

		string LastDeviceWipeRequestor { get; set; }

		string DeviceActiveSyncVersion { get; set; }

		string[] RemoteWipeConfirmationAddresses { get; set; }

		int? ADDeviceInfoHash { get; set; }

		bool HaveSentBoostrapMailForWM61 { get; set; }

		ExDateTime? BootstrapMailForWM61TriggeredTime { get; set; }

		bool DeviceInformationReceived { get; set; }

		ExDateTime? SyncStateUpgradeTime { get; set; }

		ExDateTime? ADCreationTime { get; set; }

		ADObjectId DeviceADObjectId { get; set; }

		ADObjectId UserADObjectId { get; set; }

		bool IsSyncStateJustUpgraded { get; }

		StoreObjectId ABQMailId { get; set; }

		ABQMailState ABQMailState { get; set; }

		bool DeviceInformationPromoted { get; set; }

		string DevicePhoneNumberForSms { get; set; }

		bool SmsSearchFolderCreated { get; set; }

		DeviceBehavior DeviceBehavior { get; set; }
	}
}
