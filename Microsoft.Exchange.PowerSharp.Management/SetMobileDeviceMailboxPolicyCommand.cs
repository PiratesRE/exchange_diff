using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMobileDeviceMailboxPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<MobileMailboxPolicy>
	{
		private SetMobileDeviceMailboxPolicyCommand() : base("Set-MobileDeviceMailboxPolicy")
		{
		}

		public SetMobileDeviceMailboxPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMobileDeviceMailboxPolicyCommand SetParameters(SetMobileDeviceMailboxPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMobileDeviceMailboxPolicyCommand SetParameters(SetMobileDeviceMailboxPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AllowNonProvisionableDevices
			{
				set
				{
					base.PowerSharpParameters["AllowNonProvisionableDevices"] = value;
				}
			}

			public virtual bool AlphanumericPasswordRequired
			{
				set
				{
					base.PowerSharpParameters["AlphanumericPasswordRequired"] = value;
				}
			}

			public virtual bool AttachmentsEnabled
			{
				set
				{
					base.PowerSharpParameters["AttachmentsEnabled"] = value;
				}
			}

			public virtual bool DeviceEncryptionEnabled
			{
				set
				{
					base.PowerSharpParameters["DeviceEncryptionEnabled"] = value;
				}
			}

			public virtual bool RequireStorageCardEncryption
			{
				set
				{
					base.PowerSharpParameters["RequireStorageCardEncryption"] = value;
				}
			}

			public virtual bool PasswordEnabled
			{
				set
				{
					base.PowerSharpParameters["PasswordEnabled"] = value;
				}
			}

			public virtual bool PasswordRecoveryEnabled
			{
				set
				{
					base.PowerSharpParameters["PasswordRecoveryEnabled"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval
			{
				set
				{
					base.PowerSharpParameters["DevicePolicyRefreshInterval"] = value;
				}
			}

			public virtual bool AllowSimplePassword
			{
				set
				{
					base.PowerSharpParameters["AllowSimplePassword"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MaxAttachmentSize
			{
				set
				{
					base.PowerSharpParameters["MaxAttachmentSize"] = value;
				}
			}

			public virtual bool WSSAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessEnabled"] = value;
				}
			}

			public virtual bool UNCAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessEnabled"] = value;
				}
			}

			public virtual int? MinPasswordLength
			{
				set
				{
					base.PowerSharpParameters["MinPasswordLength"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> MaxInactivityTimeLock
			{
				set
				{
					base.PowerSharpParameters["MaxInactivityTimeLock"] = value;
				}
			}

			public virtual Unlimited<int> MaxPasswordFailedAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxPasswordFailedAttempts"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> PasswordExpiration
			{
				set
				{
					base.PowerSharpParameters["PasswordExpiration"] = value;
				}
			}

			public virtual int PasswordHistory
			{
				set
				{
					base.PowerSharpParameters["PasswordHistory"] = value;
				}
			}

			public virtual bool IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual bool AllowApplePushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowApplePushNotifications"] = value;
				}
			}

			public virtual bool AllowMicrosoftPushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowMicrosoftPushNotifications"] = value;
				}
			}

			public virtual bool AllowGooglePushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowGooglePushNotifications"] = value;
				}
			}

			public virtual bool AllowStorageCard
			{
				set
				{
					base.PowerSharpParameters["AllowStorageCard"] = value;
				}
			}

			public virtual bool AllowCamera
			{
				set
				{
					base.PowerSharpParameters["AllowCamera"] = value;
				}
			}

			public virtual bool RequireDeviceEncryption
			{
				set
				{
					base.PowerSharpParameters["RequireDeviceEncryption"] = value;
				}
			}

			public virtual bool AllowUnsignedApplications
			{
				set
				{
					base.PowerSharpParameters["AllowUnsignedApplications"] = value;
				}
			}

			public virtual bool AllowUnsignedInstallationPackages
			{
				set
				{
					base.PowerSharpParameters["AllowUnsignedInstallationPackages"] = value;
				}
			}

			public virtual bool AllowWiFi
			{
				set
				{
					base.PowerSharpParameters["AllowWiFi"] = value;
				}
			}

			public virtual bool AllowTextMessaging
			{
				set
				{
					base.PowerSharpParameters["AllowTextMessaging"] = value;
				}
			}

			public virtual bool AllowPOPIMAPEmail
			{
				set
				{
					base.PowerSharpParameters["AllowPOPIMAPEmail"] = value;
				}
			}

			public virtual bool AllowIrDA
			{
				set
				{
					base.PowerSharpParameters["AllowIrDA"] = value;
				}
			}

			public virtual bool RequireManualSyncWhenRoaming
			{
				set
				{
					base.PowerSharpParameters["RequireManualSyncWhenRoaming"] = value;
				}
			}

			public virtual bool AllowDesktopSync
			{
				set
				{
					base.PowerSharpParameters["AllowDesktopSync"] = value;
				}
			}

			public virtual bool AllowHTMLEmail
			{
				set
				{
					base.PowerSharpParameters["AllowHTMLEmail"] = value;
				}
			}

			public virtual bool RequireSignedSMIMEMessages
			{
				set
				{
					base.PowerSharpParameters["RequireSignedSMIMEMessages"] = value;
				}
			}

			public virtual bool RequireEncryptedSMIMEMessages
			{
				set
				{
					base.PowerSharpParameters["RequireEncryptedSMIMEMessages"] = value;
				}
			}

			public virtual bool AllowSMIMESoftCerts
			{
				set
				{
					base.PowerSharpParameters["AllowSMIMESoftCerts"] = value;
				}
			}

			public virtual bool AllowBrowser
			{
				set
				{
					base.PowerSharpParameters["AllowBrowser"] = value;
				}
			}

			public virtual bool AllowConsumerEmail
			{
				set
				{
					base.PowerSharpParameters["AllowConsumerEmail"] = value;
				}
			}

			public virtual bool AllowRemoteDesktop
			{
				set
				{
					base.PowerSharpParameters["AllowRemoteDesktop"] = value;
				}
			}

			public virtual bool AllowInternetSharing
			{
				set
				{
					base.PowerSharpParameters["AllowInternetSharing"] = value;
				}
			}

			public virtual BluetoothType AllowBluetooth
			{
				set
				{
					base.PowerSharpParameters["AllowBluetooth"] = value;
				}
			}

			public virtual CalendarAgeFilterType MaxCalendarAgeFilter
			{
				set
				{
					base.PowerSharpParameters["MaxCalendarAgeFilter"] = value;
				}
			}

			public virtual EmailAgeFilterType MaxEmailAgeFilter
			{
				set
				{
					base.PowerSharpParameters["MaxEmailAgeFilter"] = value;
				}
			}

			public virtual SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm
			{
				set
				{
					base.PowerSharpParameters["RequireSignedSMIMEAlgorithm"] = value;
				}
			}

			public virtual EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm
			{
				set
				{
					base.PowerSharpParameters["RequireEncryptionSMIMEAlgorithm"] = value;
				}
			}

			public virtual SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation
			{
				set
				{
					base.PowerSharpParameters["AllowSMIMEEncryptionAlgorithmNegotiation"] = value;
				}
			}

			public virtual int MinPasswordComplexCharacters
			{
				set
				{
					base.PowerSharpParameters["MinPasswordComplexCharacters"] = value;
				}
			}

			public virtual Unlimited<int> MaxEmailBodyTruncationSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailBodyTruncationSize"] = value;
				}
			}

			public virtual Unlimited<int> MaxEmailHTMLBodyTruncationSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailHTMLBodyTruncationSize"] = value;
				}
			}

			public virtual MultiValuedProperty<string> UnapprovedInROMApplicationList
			{
				set
				{
					base.PowerSharpParameters["UnapprovedInROMApplicationList"] = value;
				}
			}

			public virtual ApprovedApplicationCollection ApprovedApplicationList
			{
				set
				{
					base.PowerSharpParameters["ApprovedApplicationList"] = value;
				}
			}

			public virtual bool AllowExternalDeviceManagement
			{
				set
				{
					base.PowerSharpParameters["AllowExternalDeviceManagement"] = value;
				}
			}

			public virtual MobileOTAUpdateModeType MobileOTAUpdateMode
			{
				set
				{
					base.PowerSharpParameters["MobileOTAUpdateMode"] = value;
				}
			}

			public virtual bool AllowMobileOTAUpdate
			{
				set
				{
					base.PowerSharpParameters["AllowMobileOTAUpdate"] = value;
				}
			}

			public virtual bool IrmEnabled
			{
				set
				{
					base.PowerSharpParameters["IrmEnabled"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool AllowNonProvisionableDevices
			{
				set
				{
					base.PowerSharpParameters["AllowNonProvisionableDevices"] = value;
				}
			}

			public virtual bool AlphanumericPasswordRequired
			{
				set
				{
					base.PowerSharpParameters["AlphanumericPasswordRequired"] = value;
				}
			}

			public virtual bool AttachmentsEnabled
			{
				set
				{
					base.PowerSharpParameters["AttachmentsEnabled"] = value;
				}
			}

			public virtual bool DeviceEncryptionEnabled
			{
				set
				{
					base.PowerSharpParameters["DeviceEncryptionEnabled"] = value;
				}
			}

			public virtual bool RequireStorageCardEncryption
			{
				set
				{
					base.PowerSharpParameters["RequireStorageCardEncryption"] = value;
				}
			}

			public virtual bool PasswordEnabled
			{
				set
				{
					base.PowerSharpParameters["PasswordEnabled"] = value;
				}
			}

			public virtual bool PasswordRecoveryEnabled
			{
				set
				{
					base.PowerSharpParameters["PasswordRecoveryEnabled"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval
			{
				set
				{
					base.PowerSharpParameters["DevicePolicyRefreshInterval"] = value;
				}
			}

			public virtual bool AllowSimplePassword
			{
				set
				{
					base.PowerSharpParameters["AllowSimplePassword"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> MaxAttachmentSize
			{
				set
				{
					base.PowerSharpParameters["MaxAttachmentSize"] = value;
				}
			}

			public virtual bool WSSAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessEnabled"] = value;
				}
			}

			public virtual bool UNCAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessEnabled"] = value;
				}
			}

			public virtual int? MinPasswordLength
			{
				set
				{
					base.PowerSharpParameters["MinPasswordLength"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> MaxInactivityTimeLock
			{
				set
				{
					base.PowerSharpParameters["MaxInactivityTimeLock"] = value;
				}
			}

			public virtual Unlimited<int> MaxPasswordFailedAttempts
			{
				set
				{
					base.PowerSharpParameters["MaxPasswordFailedAttempts"] = value;
				}
			}

			public virtual Unlimited<EnhancedTimeSpan> PasswordExpiration
			{
				set
				{
					base.PowerSharpParameters["PasswordExpiration"] = value;
				}
			}

			public virtual int PasswordHistory
			{
				set
				{
					base.PowerSharpParameters["PasswordHistory"] = value;
				}
			}

			public virtual bool IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual bool AllowApplePushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowApplePushNotifications"] = value;
				}
			}

			public virtual bool AllowMicrosoftPushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowMicrosoftPushNotifications"] = value;
				}
			}

			public virtual bool AllowGooglePushNotifications
			{
				set
				{
					base.PowerSharpParameters["AllowGooglePushNotifications"] = value;
				}
			}

			public virtual bool AllowStorageCard
			{
				set
				{
					base.PowerSharpParameters["AllowStorageCard"] = value;
				}
			}

			public virtual bool AllowCamera
			{
				set
				{
					base.PowerSharpParameters["AllowCamera"] = value;
				}
			}

			public virtual bool RequireDeviceEncryption
			{
				set
				{
					base.PowerSharpParameters["RequireDeviceEncryption"] = value;
				}
			}

			public virtual bool AllowUnsignedApplications
			{
				set
				{
					base.PowerSharpParameters["AllowUnsignedApplications"] = value;
				}
			}

			public virtual bool AllowUnsignedInstallationPackages
			{
				set
				{
					base.PowerSharpParameters["AllowUnsignedInstallationPackages"] = value;
				}
			}

			public virtual bool AllowWiFi
			{
				set
				{
					base.PowerSharpParameters["AllowWiFi"] = value;
				}
			}

			public virtual bool AllowTextMessaging
			{
				set
				{
					base.PowerSharpParameters["AllowTextMessaging"] = value;
				}
			}

			public virtual bool AllowPOPIMAPEmail
			{
				set
				{
					base.PowerSharpParameters["AllowPOPIMAPEmail"] = value;
				}
			}

			public virtual bool AllowIrDA
			{
				set
				{
					base.PowerSharpParameters["AllowIrDA"] = value;
				}
			}

			public virtual bool RequireManualSyncWhenRoaming
			{
				set
				{
					base.PowerSharpParameters["RequireManualSyncWhenRoaming"] = value;
				}
			}

			public virtual bool AllowDesktopSync
			{
				set
				{
					base.PowerSharpParameters["AllowDesktopSync"] = value;
				}
			}

			public virtual bool AllowHTMLEmail
			{
				set
				{
					base.PowerSharpParameters["AllowHTMLEmail"] = value;
				}
			}

			public virtual bool RequireSignedSMIMEMessages
			{
				set
				{
					base.PowerSharpParameters["RequireSignedSMIMEMessages"] = value;
				}
			}

			public virtual bool RequireEncryptedSMIMEMessages
			{
				set
				{
					base.PowerSharpParameters["RequireEncryptedSMIMEMessages"] = value;
				}
			}

			public virtual bool AllowSMIMESoftCerts
			{
				set
				{
					base.PowerSharpParameters["AllowSMIMESoftCerts"] = value;
				}
			}

			public virtual bool AllowBrowser
			{
				set
				{
					base.PowerSharpParameters["AllowBrowser"] = value;
				}
			}

			public virtual bool AllowConsumerEmail
			{
				set
				{
					base.PowerSharpParameters["AllowConsumerEmail"] = value;
				}
			}

			public virtual bool AllowRemoteDesktop
			{
				set
				{
					base.PowerSharpParameters["AllowRemoteDesktop"] = value;
				}
			}

			public virtual bool AllowInternetSharing
			{
				set
				{
					base.PowerSharpParameters["AllowInternetSharing"] = value;
				}
			}

			public virtual BluetoothType AllowBluetooth
			{
				set
				{
					base.PowerSharpParameters["AllowBluetooth"] = value;
				}
			}

			public virtual CalendarAgeFilterType MaxCalendarAgeFilter
			{
				set
				{
					base.PowerSharpParameters["MaxCalendarAgeFilter"] = value;
				}
			}

			public virtual EmailAgeFilterType MaxEmailAgeFilter
			{
				set
				{
					base.PowerSharpParameters["MaxEmailAgeFilter"] = value;
				}
			}

			public virtual SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm
			{
				set
				{
					base.PowerSharpParameters["RequireSignedSMIMEAlgorithm"] = value;
				}
			}

			public virtual EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm
			{
				set
				{
					base.PowerSharpParameters["RequireEncryptionSMIMEAlgorithm"] = value;
				}
			}

			public virtual SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation
			{
				set
				{
					base.PowerSharpParameters["AllowSMIMEEncryptionAlgorithmNegotiation"] = value;
				}
			}

			public virtual int MinPasswordComplexCharacters
			{
				set
				{
					base.PowerSharpParameters["MinPasswordComplexCharacters"] = value;
				}
			}

			public virtual Unlimited<int> MaxEmailBodyTruncationSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailBodyTruncationSize"] = value;
				}
			}

			public virtual Unlimited<int> MaxEmailHTMLBodyTruncationSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailHTMLBodyTruncationSize"] = value;
				}
			}

			public virtual MultiValuedProperty<string> UnapprovedInROMApplicationList
			{
				set
				{
					base.PowerSharpParameters["UnapprovedInROMApplicationList"] = value;
				}
			}

			public virtual ApprovedApplicationCollection ApprovedApplicationList
			{
				set
				{
					base.PowerSharpParameters["ApprovedApplicationList"] = value;
				}
			}

			public virtual bool AllowExternalDeviceManagement
			{
				set
				{
					base.PowerSharpParameters["AllowExternalDeviceManagement"] = value;
				}
			}

			public virtual MobileOTAUpdateModeType MobileOTAUpdateMode
			{
				set
				{
					base.PowerSharpParameters["MobileOTAUpdateMode"] = value;
				}
			}

			public virtual bool AllowMobileOTAUpdate
			{
				set
				{
					base.PowerSharpParameters["AllowMobileOTAUpdate"] = value;
				}
			}

			public virtual bool IrmEnabled
			{
				set
				{
					base.PowerSharpParameters["IrmEnabled"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
