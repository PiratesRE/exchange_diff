using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "MobileDeviceMailboxPolicy", SupportsShouldProcess = true)]
	public class NewMobilePolicy : NewMailboxPolicyBase<MobileMailboxPolicy>
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
		public bool PasswordEnabled
		{
			internal get
			{
				return this.DataObject.PasswordEnabled;
			}
			set
			{
				this.DataObject.PasswordEnabled = value;
			}
		}

		[Parameter]
		public bool AlphanumericPasswordRequired
		{
			internal get
			{
				return this.DataObject.AlphanumericPasswordRequired;
			}
			set
			{
				this.DataObject.AlphanumericPasswordRequired = value;
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
		public int? MinPasswordLength
		{
			internal get
			{
				return this.DataObject.MinPasswordLength;
			}
			set
			{
				this.DataObject.MinPasswordLength = value;
			}
		}

		[Parameter]
		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeLock
		{
			internal get
			{
				return this.DataObject.MaxInactivityTimeLock;
			}
			set
			{
				this.DataObject.MaxInactivityTimeLock = value;
			}
		}

		[Parameter]
		public Unlimited<int> MaxPasswordFailedAttempts
		{
			internal get
			{
				return this.DataObject.MaxPasswordFailedAttempts;
			}
			set
			{
				this.DataObject.MaxPasswordFailedAttempts = value;
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
		public bool AllowSimplePassword
		{
			internal get
			{
				return this.DataObject.AllowSimplePassword;
			}
			set
			{
				this.DataObject.AllowSimplePassword = value;
			}
		}

		[Parameter]
		public Unlimited<EnhancedTimeSpan> PasswordExpiration
		{
			internal get
			{
				return this.DataObject.PasswordExpiration;
			}
			set
			{
				this.DataObject.PasswordExpiration = value;
			}
		}

		[Parameter]
		public int PasswordHistory
		{
			internal get
			{
				return this.DataObject.PasswordHistory;
			}
			set
			{
				this.DataObject.PasswordHistory = value;
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
		public int MinPasswordComplexCharacters
		{
			internal get
			{
				return this.DataObject.MinPasswordComplexCharacters;
			}
			set
			{
				this.DataObject.MinPasswordComplexCharacters = value;
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

		[Parameter(Mandatory = false)]
		public bool AllowMicrosoftPushNotifications
		{
			internal get
			{
				return this.DataObject.AllowMicrosoftPushNotifications;
			}
			set
			{
				this.DataObject.AllowMicrosoftPushNotifications = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowGooglePushNotifications
		{
			internal get
			{
				return this.DataObject.AllowGooglePushNotifications;
			}
			set
			{
				this.DataObject.AllowGooglePushNotifications = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.updateExistingDefaultPolicies)
				{
					return Strings.ConfirmationMessageNewMobileMailboxDefaultPolicy(base.Name.ToString());
				}
				return base.ConfirmationMessage;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsDefault)
			{
				this.existingDefaultPolicies = DefaultMobileMailboxPolicyUtility<MobileMailboxPolicy>.GetDefaultPolicies((IConfigurationSession)base.DataSession);
				if (this.existingDefaultPolicies.Count > 0)
				{
					this.updateExistingDefaultPolicies = true;
				}
			}
			if (!DefaultMobileMailboxPolicyUtility<MobileMailboxPolicy>.ValidateLength(this.DataObject.UnapprovedInROMApplicationList, 5120, 2048))
			{
				base.WriteError(new ArgumentException(Strings.MobileDevicePolicyApplicationListTooLong(5120, 2048), "UnapprovedInROMApplicationList"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (!DefaultMobileMailboxPolicyUtility<MobileMailboxPolicy>.ValidateLength(this.DataObject.ApprovedApplicationList, 7168, 2048))
			{
				base.WriteError(new ArgumentException(Strings.MobileDevicePolicyApplicationListTooLong(7168, 2048), "ApprovedApplicationList"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.updateExistingDefaultPolicies)
			{
				try
				{
					DefaultMailboxPolicyUtility<MobileMailboxPolicy>.ClearDefaultPolicies(base.DataSession as IConfigurationSession, this.existingDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
		}
	}
}
