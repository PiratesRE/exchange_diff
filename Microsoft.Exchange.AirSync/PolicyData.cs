using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal class PolicyData : IPolicyData
	{
		public PolicyData(MobileMailboxPolicy policy) : this(policy, GlobalSettings.IrmEnabled)
		{
		}

		public PolicyData(MobileMailboxPolicy policy, bool useIrmEnabledPolicySetting)
		{
			this.identity = policy.OriginalId;
			this.allowNonProvisionableDevices = policy.AllowNonProvisionableDevices;
			this.alphanumericDevicePasswordRequired = policy.AlphanumericPasswordRequired;
			this.attachmentsEnabled = policy.AttachmentsEnabled;
			this.requireStorageCardEncryption = policy.RequireStorageCardEncryption;
			this.devicePasswordEnabled = policy.PasswordEnabled;
			this.passwordRecoveryEnabled = policy.PasswordRecoveryEnabled;
			this.devicePolicyRefreshInterval = policy.DevicePolicyRefreshInterval;
			this.allowSimpleDevicePassword = policy.AllowSimplePassword;
			this.maxAttachmentSize = policy.MaxAttachmentSize;
			this.wssAccessEnabled = policy.WSSAccessEnabled;
			this.uncAccessEnabled = policy.UNCAccessEnabled;
			this.minDevicePasswordLength = policy.MinPasswordLength;
			this.maxInactivityTimeDeviceLock = policy.MaxInactivityTimeLock;
			this.maxDevicePasswordFailedAttempts = policy.MaxPasswordFailedAttempts;
			this.devicePasswordExpiration = policy.PasswordExpiration;
			this.devicePasswordHistory = policy.PasswordHistory;
			this.isDefault = policy.IsDefault;
			this.allowStorageCard = policy.AllowStorageCard;
			this.allowCamera = policy.AllowCamera;
			this.requireDeviceEncryption = policy.RequireDeviceEncryption;
			this.allowUnsignedApplications = policy.AllowUnsignedApplications;
			this.allowUnsignedInstallationPackages = policy.AllowUnsignedInstallationPackages;
			this.allowWiFi = policy.AllowWiFi;
			this.allowTextMessaging = policy.AllowTextMessaging;
			this.allowPOPIMAPEmail = policy.AllowPOPIMAPEmail;
			this.allowIrDA = policy.AllowIrDA;
			this.requireManualSyncWhenRoaming = policy.RequireManualSyncWhenRoaming;
			this.allowDesktopSync = policy.AllowDesktopSync;
			this.allowHTMLEmail = policy.AllowHTMLEmail;
			this.requireSignedSMIMEMessages = policy.RequireSignedSMIMEMessages;
			this.requireEncryptedSMIMEMessages = policy.RequireEncryptedSMIMEMessages;
			this.allowSMIMESoftCerts = policy.AllowSMIMESoftCerts;
			this.allowBrowser = policy.AllowBrowser;
			this.allowConsumerEmail = policy.AllowConsumerEmail;
			this.allowRemoteDesktop = policy.AllowRemoteDesktop;
			this.allowInternetSharing = policy.AllowInternetSharing;
			this.allowBluetooth = policy.AllowBluetooth;
			this.maxCalendarAgeFilter = policy.MaxCalendarAgeFilter;
			this.maxEmailAgeFilter = policy.MaxEmailAgeFilter;
			this.requireSignedSMIMEAlgorithm = policy.RequireSignedSMIMEAlgorithm;
			this.requireEncryptionSMIMEAlgorithm = policy.RequireEncryptionSMIMEAlgorithm;
			this.allowSMIMEEncryptionAlgorithmNegotiation = policy.AllowSMIMEEncryptionAlgorithmNegotiation;
			this.minDevicePasswordComplexCharacters = policy.MinPasswordComplexCharacters;
			this.maxEmailBodyTruncationSize = policy.MaxEmailBodyTruncationSize;
			this.maxEmailHTMLBodyTruncationSize = policy.MaxEmailHTMLBodyTruncationSize;
			this.unapprovedInROMApplicationList = policy.UnapprovedInROMApplicationList;
			this.approvedApplicationList = policy.ApprovedApplicationList;
			this.allowExternalDeviceManagement = policy.AllowExternalDeviceManagement;
			this.MobileOTAUpdateMode = policy.MobileOTAUpdateMode;
			this.AllowMobileOTAUpdate = policy.AllowMobileOTAUpdate;
			this.isIrmEnabled = (useIrmEnabledPolicySetting && policy.IrmEnabled);
			string policyXml = ProvisionCommandPhaseOne.BuildEASProvisionDoc(20, out this.preversion121DeviceCompatibility, this) + policy.AllowNonProvisionableDevices;
			this.preversion121HashCode = PolicyData.GetPolicyHashCode(policyXml);
			policyXml = ProvisionCommandPhaseOne.BuildEASProvisionDoc(121, out this.version121DeviceCompatibility, this) + policy.AllowNonProvisionableDevices;
			this.version121HashCode = PolicyData.GetPolicyHashCode(policyXml);
		}

		public ADObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public bool AllowNonProvisionableDevices
		{
			get
			{
				return this.allowNonProvisionableDevices;
			}
		}

		public bool AlphanumericDevicePasswordRequired
		{
			get
			{
				return this.alphanumericDevicePasswordRequired;
			}
		}

		public bool AttachmentsEnabled
		{
			get
			{
				return this.attachmentsEnabled;
			}
		}

		public bool RequireStorageCardEncryption
		{
			get
			{
				return this.requireStorageCardEncryption;
			}
		}

		public bool DevicePasswordEnabled
		{
			get
			{
				return this.devicePasswordEnabled;
			}
		}

		public bool PasswordRecoveryEnabled
		{
			get
			{
				return this.passwordRecoveryEnabled;
			}
		}

		public Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval
		{
			get
			{
				return this.devicePolicyRefreshInterval;
			}
		}

		public bool AllowSimpleDevicePassword
		{
			get
			{
				return this.allowSimpleDevicePassword;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxAttachmentSize
		{
			get
			{
				return this.maxAttachmentSize;
			}
		}

		public bool WSSAccessEnabled
		{
			get
			{
				return this.wssAccessEnabled;
			}
		}

		public bool UNCAccessEnabled
		{
			get
			{
				return this.uncAccessEnabled;
			}
		}

		public int? MinDevicePasswordLength
		{
			get
			{
				return this.minDevicePasswordLength;
			}
		}

		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeDeviceLock
		{
			get
			{
				return this.maxInactivityTimeDeviceLock;
			}
		}

		public Unlimited<int> MaxDevicePasswordFailedAttempts
		{
			get
			{
				return this.maxDevicePasswordFailedAttempts;
			}
		}

		public Unlimited<EnhancedTimeSpan> DevicePasswordExpiration
		{
			get
			{
				return this.devicePasswordExpiration;
			}
		}

		public int DevicePasswordHistory
		{
			get
			{
				return this.devicePasswordHistory;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		public bool AllowStorageCard
		{
			get
			{
				return this.allowStorageCard;
			}
		}

		public bool AllowCamera
		{
			get
			{
				return this.allowCamera;
			}
		}

		public bool RequireDeviceEncryption
		{
			get
			{
				return this.requireDeviceEncryption;
			}
		}

		public bool AllowUnsignedApplications
		{
			get
			{
				return this.allowUnsignedApplications;
			}
		}

		public bool AllowUnsignedInstallationPackages
		{
			get
			{
				return this.allowUnsignedInstallationPackages;
			}
		}

		public bool AllowWiFi
		{
			get
			{
				return this.allowWiFi;
			}
		}

		public bool AllowTextMessaging
		{
			get
			{
				return this.allowTextMessaging;
			}
		}

		public bool AllowPOPIMAPEmail
		{
			get
			{
				return this.allowPOPIMAPEmail;
			}
		}

		public bool AllowIrDA
		{
			get
			{
				return this.allowIrDA;
			}
		}

		public bool RequireManualSyncWhenRoaming
		{
			get
			{
				return this.requireManualSyncWhenRoaming;
			}
		}

		public bool AllowDesktopSync
		{
			get
			{
				return this.allowDesktopSync;
			}
		}

		public bool AllowHTMLEmail
		{
			get
			{
				return this.allowHTMLEmail;
			}
		}

		public bool RequireSignedSMIMEMessages
		{
			get
			{
				return this.requireSignedSMIMEMessages;
			}
		}

		public bool RequireEncryptedSMIMEMessages
		{
			get
			{
				return this.requireEncryptedSMIMEMessages;
			}
		}

		public bool AllowSMIMESoftCerts
		{
			get
			{
				return this.allowSMIMESoftCerts;
			}
		}

		public bool AllowBrowser
		{
			get
			{
				return this.allowBrowser;
			}
		}

		public bool AllowConsumerEmail
		{
			get
			{
				return this.allowConsumerEmail;
			}
		}

		public bool AllowRemoteDesktop
		{
			get
			{
				return this.allowRemoteDesktop;
			}
		}

		public bool AllowInternetSharing
		{
			get
			{
				return this.allowInternetSharing;
			}
		}

		public BluetoothType AllowBluetooth
		{
			get
			{
				return this.allowBluetooth;
			}
		}

		public CalendarAgeFilterType MaxCalendarAgeFilter
		{
			get
			{
				return this.maxCalendarAgeFilter;
			}
		}

		public EmailAgeFilterType MaxEmailAgeFilter
		{
			get
			{
				return this.maxEmailAgeFilter;
			}
		}

		public SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm
		{
			get
			{
				return this.requireSignedSMIMEAlgorithm;
			}
		}

		public EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm
		{
			get
			{
				return this.requireEncryptionSMIMEAlgorithm;
			}
		}

		public SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation
		{
			get
			{
				return this.allowSMIMEEncryptionAlgorithmNegotiation;
			}
		}

		public int MinDevicePasswordComplexCharacters
		{
			get
			{
				return this.minDevicePasswordComplexCharacters;
			}
		}

		public Unlimited<int> MaxEmailBodyTruncationSize
		{
			get
			{
				return this.maxEmailBodyTruncationSize;
			}
		}

		public Unlimited<int> MaxEmailHTMLBodyTruncationSize
		{
			get
			{
				return this.maxEmailHTMLBodyTruncationSize;
			}
		}

		public MultiValuedProperty<string> UnapprovedInROMApplicationList
		{
			get
			{
				return this.unapprovedInROMApplicationList;
			}
		}

		public ApprovedApplicationCollection ApprovedApplicationList
		{
			get
			{
				return this.approvedApplicationList;
			}
		}

		public bool AllowExternalDeviceManagement
		{
			get
			{
				return this.allowExternalDeviceManagement;
			}
		}

		public bool IsIrmEnabled
		{
			get
			{
				return this.isIrmEnabled;
			}
		}

		public MobileOTAUpdateModeType MobileOTAUpdateMode { get; private set; }

		public bool AllowMobileOTAUpdate { get; private set; }

		public static int GetPolicyHashCode(string policyXml)
		{
			return policyXml.GetHashCode();
		}

		public bool GetVersionCompatibility(int version)
		{
			if (version >= 121)
			{
				return this.version121DeviceCompatibility;
			}
			return this.preversion121DeviceCompatibility;
		}

		public int GetHashCode(int version)
		{
			if (version >= 121)
			{
				return this.version121HashCode;
			}
			return this.preversion121HashCode;
		}

		private readonly bool isDefault;

		private int preversion121HashCode;

		private int version121HashCode;

		private bool preversion121DeviceCompatibility;

		private bool version121DeviceCompatibility;

		private ADObjectId identity;

		private bool allowNonProvisionableDevices;

		private bool alphanumericDevicePasswordRequired;

		private bool attachmentsEnabled;

		private bool requireStorageCardEncryption;

		private bool devicePasswordEnabled;

		private bool passwordRecoveryEnabled;

		private Unlimited<EnhancedTimeSpan> devicePolicyRefreshInterval;

		private bool allowSimpleDevicePassword;

		private Unlimited<ByteQuantifiedSize> maxAttachmentSize;

		private bool wssAccessEnabled;

		private bool uncAccessEnabled;

		private int? minDevicePasswordLength;

		private Unlimited<EnhancedTimeSpan> maxInactivityTimeDeviceLock;

		private Unlimited<int> maxDevicePasswordFailedAttempts;

		private Unlimited<EnhancedTimeSpan> devicePasswordExpiration;

		private int devicePasswordHistory;

		private bool allowStorageCard;

		private bool allowCamera;

		private bool requireDeviceEncryption;

		private bool allowUnsignedApplications;

		private bool allowUnsignedInstallationPackages;

		private bool allowWiFi;

		private bool allowTextMessaging;

		private bool allowPOPIMAPEmail;

		private bool allowIrDA;

		private bool requireManualSyncWhenRoaming;

		private bool allowDesktopSync;

		private bool allowHTMLEmail;

		private bool requireSignedSMIMEMessages;

		private bool requireEncryptedSMIMEMessages;

		private bool allowSMIMESoftCerts;

		private bool allowBrowser;

		private bool allowConsumerEmail;

		private bool allowRemoteDesktop;

		private bool allowInternetSharing;

		private BluetoothType allowBluetooth;

		private CalendarAgeFilterType maxCalendarAgeFilter;

		private EmailAgeFilterType maxEmailAgeFilter;

		private SignedSMIMEAlgorithmType requireSignedSMIMEAlgorithm;

		private EncryptionSMIMEAlgorithmType requireEncryptionSMIMEAlgorithm;

		private SMIMEEncryptionAlgorithmNegotiationType allowSMIMEEncryptionAlgorithmNegotiation;

		private int minDevicePasswordComplexCharacters;

		private Unlimited<int> maxEmailBodyTruncationSize;

		private Unlimited<int> maxEmailHTMLBodyTruncationSize;

		private MultiValuedProperty<string> unapprovedInROMApplicationList;

		private ApprovedApplicationCollection approvedApplicationList;

		private bool allowExternalDeviceManagement;

		private bool isIrmEnabled;
	}
}
