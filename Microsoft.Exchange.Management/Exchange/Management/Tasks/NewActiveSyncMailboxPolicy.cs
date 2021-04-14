using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "ActiveSyncMailboxPolicy", SupportsShouldProcess = true)]
	public class NewActiveSyncMailboxPolicy : NewMailboxPolicyBase<ActiveSyncMailboxPolicy>
	{
		[Parameter]
		public bool AttachmentsEnabled
		{
			internal get
			{
				return this.DataObject.AttachmentsEnabled;
			}
			set
			{
				this.DataObject.AttachmentsEnabled = value;
			}
		}

		[Parameter]
		public bool DevicePasswordEnabled
		{
			internal get
			{
				return this.DataObject.DevicePasswordEnabled;
			}
			set
			{
				this.DataObject.DevicePasswordEnabled = value;
			}
		}

		[Parameter]
		public bool AlphanumericDevicePasswordRequired
		{
			internal get
			{
				return this.DataObject.AlphanumericDevicePasswordRequired;
			}
			set
			{
				this.DataObject.AlphanumericDevicePasswordRequired = value;
			}
		}

		[Parameter]
		public bool PasswordRecoveryEnabled
		{
			internal get
			{
				return this.DataObject.PasswordRecoveryEnabled;
			}
			set
			{
				this.DataObject.PasswordRecoveryEnabled = value;
			}
		}

		[Parameter]
		public bool DeviceEncryptionEnabled
		{
			internal get
			{
				return this.DataObject.RequireStorageCardEncryption;
			}
			set
			{
				this.DataObject.RequireStorageCardEncryption = value;
			}
		}

		[Parameter]
		public int? MinDevicePasswordLength
		{
			internal get
			{
				return this.DataObject.MinDevicePasswordLength;
			}
			set
			{
				this.DataObject.MinDevicePasswordLength = value;
			}
		}

		[Parameter]
		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeDeviceLock
		{
			internal get
			{
				return this.DataObject.MaxInactivityTimeDeviceLock;
			}
			set
			{
				this.DataObject.MaxInactivityTimeDeviceLock = value;
			}
		}

		[Parameter]
		public Unlimited<int> MaxDevicePasswordFailedAttempts
		{
			internal get
			{
				return this.DataObject.MaxDevicePasswordFailedAttempts;
			}
			set
			{
				this.DataObject.MaxDevicePasswordFailedAttempts = value;
			}
		}

		[Parameter]
		public bool AllowNonProvisionableDevices
		{
			internal get
			{
				return this.DataObject.AllowNonProvisionableDevices;
			}
			set
			{
				this.DataObject.AllowNonProvisionableDevices = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize> MaxAttachmentSize
		{
			internal get
			{
				return this.DataObject.MaxAttachmentSize;
			}
			set
			{
				this.DataObject.MaxAttachmentSize = value;
			}
		}

		[Parameter]
		public bool AllowSimpleDevicePassword
		{
			internal get
			{
				return this.DataObject.AllowSimpleDevicePassword;
			}
			set
			{
				this.DataObject.AllowSimpleDevicePassword = value;
			}
		}

		[Parameter]
		public Unlimited<EnhancedTimeSpan> DevicePasswordExpiration
		{
			internal get
			{
				return this.DataObject.DevicePasswordExpiration;
			}
			set
			{
				this.DataObject.DevicePasswordExpiration = value;
			}
		}

		[Parameter]
		public int DevicePasswordHistory
		{
			internal get
			{
				return this.DataObject.DevicePasswordHistory;
			}
			set
			{
				this.DataObject.DevicePasswordHistory = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> DevicePolicyRefreshInterval
		{
			internal get
			{
				return this.DataObject.DevicePolicyRefreshInterval;
			}
			set
			{
				this.DataObject.DevicePolicyRefreshInterval = value;
			}
		}

		[Parameter]
		public bool WSSAccessEnabled
		{
			internal get
			{
				return this.DataObject.WSSAccessEnabled;
			}
			set
			{
				this.DataObject.WSSAccessEnabled = value;
			}
		}

		[Parameter]
		public bool UNCAccessEnabled
		{
			internal get
			{
				return this.DataObject.UNCAccessEnabled;
			}
			set
			{
				this.DataObject.UNCAccessEnabled = value;
			}
		}

		[Parameter]
		public bool IsDefault
		{
			internal get
			{
				return this.DataObject.IsDefault;
			}
			set
			{
				this.DataObject.IsDefault = value;
			}
		}

		[Parameter]
		public bool IsDefaultPolicy
		{
			internal get
			{
				return this.DataObject.IsDefault;
			}
			set
			{
				this.DataObject.IsDefault = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowStorageCard
		{
			internal get
			{
				return this.DataObject.AllowStorageCard;
			}
			set
			{
				this.DataObject.AllowStorageCard = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowCamera
		{
			internal get
			{
				return this.DataObject.AllowCamera;
			}
			set
			{
				this.DataObject.AllowCamera = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireStorageCardEncryption
		{
			internal get
			{
				return this.DataObject.RequireStorageCardEncryption;
			}
			set
			{
				this.DataObject.RequireStorageCardEncryption = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireDeviceEncryption
		{
			internal get
			{
				return this.DataObject.RequireDeviceEncryption;
			}
			set
			{
				this.DataObject.RequireDeviceEncryption = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowUnsignedApplications
		{
			internal get
			{
				return this.DataObject.AllowUnsignedApplications;
			}
			set
			{
				this.DataObject.AllowUnsignedApplications = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowUnsignedInstallationPackages
		{
			internal get
			{
				return this.DataObject.AllowUnsignedInstallationPackages;
			}
			set
			{
				this.DataObject.AllowUnsignedInstallationPackages = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinDevicePasswordComplexCharacters
		{
			internal get
			{
				return this.DataObject.MinDevicePasswordComplexCharacters;
			}
			set
			{
				this.DataObject.MinDevicePasswordComplexCharacters = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowWiFi
		{
			internal get
			{
				return this.DataObject.AllowWiFi;
			}
			set
			{
				this.DataObject.AllowWiFi = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowTextMessaging
		{
			internal get
			{
				return this.DataObject.AllowTextMessaging;
			}
			set
			{
				this.DataObject.AllowTextMessaging = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowPOPIMAPEmail
		{
			internal get
			{
				return this.DataObject.AllowPOPIMAPEmail;
			}
			set
			{
				this.DataObject.AllowPOPIMAPEmail = value;
			}
		}

		[Parameter(Mandatory = false)]
		public BluetoothType AllowBluetooth
		{
			internal get
			{
				return this.DataObject.AllowBluetooth;
			}
			set
			{
				this.DataObject.AllowBluetooth = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowIrDA
		{
			internal get
			{
				return this.DataObject.AllowIrDA;
			}
			set
			{
				this.DataObject.AllowIrDA = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireManualSyncWhenRoaming
		{
			internal get
			{
				return this.DataObject.RequireManualSyncWhenRoaming;
			}
			set
			{
				this.DataObject.RequireManualSyncWhenRoaming = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowDesktopSync
		{
			internal get
			{
				return this.DataObject.AllowDesktopSync;
			}
			set
			{
				this.DataObject.AllowDesktopSync = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CalendarAgeFilterType MaxCalendarAgeFilter
		{
			internal get
			{
				return this.DataObject.MaxCalendarAgeFilter;
			}
			set
			{
				this.DataObject.MaxCalendarAgeFilter = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowHTMLEmail
		{
			internal get
			{
				return this.DataObject.AllowHTMLEmail;
			}
			set
			{
				this.DataObject.AllowHTMLEmail = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EmailAgeFilterType MaxEmailAgeFilter
		{
			internal get
			{
				return this.DataObject.MaxEmailAgeFilter;
			}
			set
			{
				this.DataObject.MaxEmailAgeFilter = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxEmailBodyTruncationSize
		{
			internal get
			{
				return this.DataObject.MaxEmailBodyTruncationSize;
			}
			set
			{
				this.DataObject.MaxEmailBodyTruncationSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxEmailHTMLBodyTruncationSize
		{
			internal get
			{
				return this.DataObject.MaxEmailHTMLBodyTruncationSize;
			}
			set
			{
				this.DataObject.MaxEmailHTMLBodyTruncationSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSignedSMIMEMessages
		{
			internal get
			{
				return this.DataObject.RequireSignedSMIMEMessages;
			}
			set
			{
				this.DataObject.RequireSignedSMIMEMessages = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireEncryptedSMIMEMessages
		{
			internal get
			{
				return this.DataObject.RequireEncryptedSMIMEMessages;
			}
			set
			{
				this.DataObject.RequireEncryptedSMIMEMessages = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SignedSMIMEAlgorithmType RequireSignedSMIMEAlgorithm
		{
			internal get
			{
				return this.DataObject.RequireSignedSMIMEAlgorithm;
			}
			set
			{
				this.DataObject.RequireSignedSMIMEAlgorithm = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EncryptionSMIMEAlgorithmType RequireEncryptionSMIMEAlgorithm
		{
			internal get
			{
				return this.DataObject.RequireEncryptionSMIMEAlgorithm;
			}
			set
			{
				this.DataObject.RequireEncryptionSMIMEAlgorithm = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SMIMEEncryptionAlgorithmNegotiationType AllowSMIMEEncryptionAlgorithmNegotiation
		{
			internal get
			{
				return this.DataObject.AllowSMIMEEncryptionAlgorithmNegotiation;
			}
			set
			{
				this.DataObject.AllowSMIMEEncryptionAlgorithmNegotiation = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSMIMESoftCerts
		{
			internal get
			{
				return this.DataObject.AllowSMIMESoftCerts;
			}
			set
			{
				this.DataObject.AllowSMIMESoftCerts = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowBrowser
		{
			internal get
			{
				return this.DataObject.AllowBrowser;
			}
			set
			{
				this.DataObject.AllowBrowser = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowConsumerEmail
		{
			internal get
			{
				return this.DataObject.AllowConsumerEmail;
			}
			set
			{
				this.DataObject.AllowConsumerEmail = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowRemoteDesktop
		{
			internal get
			{
				return this.DataObject.AllowRemoteDesktop;
			}
			set
			{
				this.DataObject.AllowRemoteDesktop = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowInternetSharing
		{
			internal get
			{
				return this.DataObject.AllowInternetSharing;
			}
			set
			{
				this.DataObject.AllowInternetSharing = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UnapprovedInROMApplicationList
		{
			internal get
			{
				return this.DataObject.UnapprovedInROMApplicationList;
			}
			set
			{
				this.DataObject.UnapprovedInROMApplicationList = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ApprovedApplicationCollection ApprovedApplicationList
		{
			internal get
			{
				return this.DataObject.ApprovedApplicationList;
			}
			set
			{
				this.DataObject.ApprovedApplicationList = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowExternalDeviceManagement
		{
			internal get
			{
				return this.DataObject.AllowExternalDeviceManagement;
			}
			set
			{
				this.DataObject.AllowExternalDeviceManagement = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MobileOTAUpdateModeType MobileOTAUpdateMode
		{
			internal get
			{
				return this.DataObject.MobileOTAUpdateMode;
			}
			set
			{
				this.DataObject.MobileOTAUpdateMode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowMobileOTAUpdate
		{
			internal get
			{
				return this.DataObject.AllowMobileOTAUpdate;
			}
			set
			{
				this.DataObject.AllowMobileOTAUpdate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IrmEnabled
		{
			internal get
			{
				return this.DataObject.IrmEnabled;
			}
			set
			{
				this.DataObject.IrmEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowApplePushNotifications
		{
			internal get
			{
				return this.DataObject.AllowApplePushNotifications;
			}
			set
			{
				this.DataObject.AllowApplePushNotifications = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.updateExistingDefaultPolicies)
				{
					return Strings.ConfirmationMessageNewActiveSyncMailboxDefaultPolicy(base.Name.ToString());
				}
				return base.ConfirmationMessage;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsDefault)
			{
				this.existingDefaultPolicies = DefaultMobileMailboxPolicyUtility<ActiveSyncMailboxPolicy>.GetDefaultPolicies((IConfigurationSession)base.DataSession);
				if (this.existingDefaultPolicies.Count > 0)
				{
					this.updateExistingDefaultPolicies = true;
				}
			}
			if (!DefaultMobileMailboxPolicyUtility<ActiveSyncMailboxPolicy>.ValidateLength(this.DataObject.UnapprovedInROMApplicationList, 5120, 2048))
			{
				base.WriteError(new ArgumentException(Strings.ActiveSyncPolicyApplicationListTooLong(5120, 2048), "UnapprovedInROMApplicationList"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (!DefaultMobileMailboxPolicyUtility<ActiveSyncMailboxPolicy>.ValidateLength(this.DataObject.ApprovedApplicationList, 7168, 2048))
			{
				base.WriteError(new ArgumentException(Strings.ActiveSyncPolicyApplicationListTooLong(7168, 2048), "ApprovedApplicationList"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("New-ActiveSyncMailboxPolicy", "New-MobileDeviceMailboxPolicy"));
			base.InternalProcessRecord();
			if (this.updateExistingDefaultPolicies)
			{
				try
				{
					DefaultMailboxPolicyUtility<ActiveSyncMailboxPolicy>.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.existingDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
		}
	}
}
