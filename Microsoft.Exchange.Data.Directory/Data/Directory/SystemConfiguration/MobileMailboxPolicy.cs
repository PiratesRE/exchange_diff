using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MobileMailboxPolicy : MailboxPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MobileMailboxPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MobileMailboxPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return MobileMailboxPolicy.parentPath;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(MobileMailboxPolicySchema.AssociatedUsers)
			});
			MobileMailboxPolicy[] array = base.Session.Find<MobileMailboxPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		[Parameter(Mandatory = false)]
		public bool AllowNonProvisionableDevices
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowNonProvisionableDevices];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowNonProvisionableDevices] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AlphanumericPasswordRequired
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AlphanumericPasswordRequired];
			}
			set
			{
				this[MobileMailboxPolicySchema.AlphanumericPasswordRequired] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttachmentsEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AttachmentsEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.AttachmentsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeviceEncryptionEnabled
		{
			get
			{
				return this.RequireStorageCardEncryption;
			}
			set
			{
				this.RequireStorageCardEncryption = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireStorageCardEncryption
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.RequireStorageCardEncryption];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireStorageCardEncryption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PasswordEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.PasswordEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.PasswordEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PasswordRecoveryEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.PasswordRecoveryEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.PasswordRecoveryEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[MobileMailboxPolicySchema.DevicePolicyRefreshInterval];
			}
			set
			{
				this[MobileMailboxPolicySchema.DevicePolicyRefreshInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSimplePassword
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowSimplePassword];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowSimplePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxAttachmentSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MobileMailboxPolicySchema.MaxAttachmentSize];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxAttachmentSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WSSAccessEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.WSSAccessEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.WSSAccessEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UNCAccessEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.UNCAccessEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.UNCAccessEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MinPasswordLength
		{
			get
			{
				return (int?)this[MobileMailboxPolicySchema.MinPasswordLength];
			}
			set
			{
				this[MobileMailboxPolicySchema.MinPasswordLength] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeLock
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[MobileMailboxPolicySchema.MaxInactivityTimeLock];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxInactivityTimeLock] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxPasswordFailedAttempts
		{
			get
			{
				return (Unlimited<int>)this[MobileMailboxPolicySchema.MaxPasswordFailedAttempts];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxPasswordFailedAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> PasswordExpiration
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[MobileMailboxPolicySchema.PasswordExpiration];
			}
			set
			{
				this[MobileMailboxPolicySchema.PasswordExpiration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PasswordHistory
		{
			get
			{
				return (int)this[MobileMailboxPolicySchema.PasswordHistory];
			}
			set
			{
				this[MobileMailboxPolicySchema.PasswordHistory] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsDefault
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.IsDefault];
			}
			set
			{
				this[MobileMailboxPolicySchema.IsDefault] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowApplePushNotifications
		{
			get
			{
				return !(bool)this[MobileMailboxPolicySchema.DenyApplePushNotifications];
			}
			set
			{
				this[MobileMailboxPolicySchema.DenyApplePushNotifications] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowMicrosoftPushNotifications
		{
			get
			{
				return !(bool)this[MobileMailboxPolicySchema.DenyMicrosoftPushNotifications];
			}
			set
			{
				this[MobileMailboxPolicySchema.DenyMicrosoftPushNotifications] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowGooglePushNotifications
		{
			get
			{
				return !(bool)this[MobileMailboxPolicySchema.DenyGooglePushNotifications];
			}
			set
			{
				this[MobileMailboxPolicySchema.DenyGooglePushNotifications] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowStorageCard
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowStorageCard];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowStorageCard] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowCamera
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowCamera];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowCamera] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireDeviceEncryption
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.RequireDeviceEncryption];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireDeviceEncryption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowUnsignedApplications
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowUnsignedApplications];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowUnsignedApplications] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowUnsignedInstallationPackages
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowUnsignedInstallationPackages];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowUnsignedInstallationPackages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowWiFi
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowWiFi];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowWiFi] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTextMessaging
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowTextMessaging];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowTextMessaging] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowPOPIMAPEmail
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowPOPIMAPEmail];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowPOPIMAPEmail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowIrDA
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowIrDA];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowIrDA] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireManualSyncWhenRoaming
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.RequireManualSyncWhenRoaming];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireManualSyncWhenRoaming] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowDesktopSync
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowDesktopSync];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowDesktopSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowHTMLEmail
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowHTMLEmail];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowHTMLEmail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSignedSMIMEMessages
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.RequireSignedSMIMEMessages];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireSignedSMIMEMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireEncryptedSMIMEMessages
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.RequireEncryptedSMIMEMessages];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireEncryptedSMIMEMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSMIMESoftCerts
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowSMIMESoftCerts];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowSMIMESoftCerts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowBrowser
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowBrowser];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowBrowser] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowConsumerEmail
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowConsumerEmail];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowConsumerEmail] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowRemoteDesktop
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowRemoteDesktop];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowRemoteDesktop] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowInternetSharing
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowInternetSharing];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowInternetSharing] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public BluetoothType AllowBluetooth
		{
			get
			{
				return (BluetoothType)this[MobileMailboxPolicySchema.AllowBluetooth];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowBluetooth] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CalendarAgeFilterType MaxCalendarAgeFilter
		{
			get
			{
				return (CalendarAgeFilterType)this[MobileMailboxPolicySchema.MaxCalendarAgeFilter];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxCalendarAgeFilter] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EmailAgeFilterType MaxEmailAgeFilter
		{
			get
			{
				return (EmailAgeFilterType)this[MobileMailboxPolicySchema.MaxEmailAgeFilter];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxEmailAgeFilter] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm
		{
			get
			{
				return (SignedSMIMEAlgorithmType)this[MobileMailboxPolicySchema.RequireSignedSMIMEAlgorithm];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireSignedSMIMEAlgorithm] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm
		{
			get
			{
				return (EncryptionSMIMEAlgorithmType)this[MobileMailboxPolicySchema.RequireEncryptionSMIMEAlgorithm];
			}
			set
			{
				this[MobileMailboxPolicySchema.RequireEncryptionSMIMEAlgorithm] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation
		{
			get
			{
				return (SMIMEEncryptionAlgorithmNegotiationType)this[MobileMailboxPolicySchema.AllowSMIMEEncryptionAlgorithmNegotiation];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowSMIMEEncryptionAlgorithmNegotiation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinPasswordComplexCharacters
		{
			get
			{
				return (int)this[MobileMailboxPolicySchema.MinPasswordComplexCharacters];
			}
			set
			{
				this[MobileMailboxPolicySchema.MinPasswordComplexCharacters] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxEmailBodyTruncationSize
		{
			get
			{
				return (Unlimited<int>)this[MobileMailboxPolicySchema.MaxEmailBodyTruncationSize];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxEmailBodyTruncationSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxEmailHTMLBodyTruncationSize
		{
			get
			{
				return (Unlimited<int>)this[MobileMailboxPolicySchema.MaxEmailHTMLBodyTruncationSize];
			}
			set
			{
				this[MobileMailboxPolicySchema.MaxEmailHTMLBodyTruncationSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UnapprovedInROMApplicationList
		{
			get
			{
				return (MultiValuedProperty<string>)this[MobileMailboxPolicySchema.UnapprovedInROMApplicationList];
			}
			set
			{
				this[MobileMailboxPolicySchema.UnapprovedInROMApplicationList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ApprovedApplicationCollection ApprovedApplicationList
		{
			get
			{
				return (ApprovedApplicationCollection)this[MobileMailboxPolicySchema.ADApprovedApplicationList];
			}
			set
			{
				this[MobileMailboxPolicySchema.ADApprovedApplicationList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowExternalDeviceManagement
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowExternalDeviceManagement];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowExternalDeviceManagement] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MobileOTAUpdateModeType MobileOTAUpdateMode
		{
			get
			{
				return (MobileOTAUpdateModeType)this[MobileMailboxPolicySchema.MobileOTAUpdateMode];
			}
			set
			{
				this[MobileMailboxPolicySchema.MobileOTAUpdateMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowMobileOTAUpdate
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.AllowMobileOTAUpdate];
			}
			set
			{
				this[MobileMailboxPolicySchema.AllowMobileOTAUpdate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IrmEnabled
		{
			get
			{
				return (bool)this[MobileMailboxPolicySchema.IrmEnabled];
			}
			set
			{
				this[MobileMailboxPolicySchema.IrmEnabled] = value;
			}
		}

		private static MobileMailboxPolicySchema schema = ObjectSchema.GetInstance<MobileMailboxPolicySchema>();

		private static string mostDerivedClass = "msExchMobileMailboxPolicy";

		private static ADObjectId parentPath = new ADObjectId("CN=Mobile Mailbox Policies");
	}
}
