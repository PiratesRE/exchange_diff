using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ActiveSyncMailboxPolicyObject : ActiveSyncMailboxPolicyRow
	{
		public ActiveSyncMailboxPolicyObject(ActiveSyncMailboxPolicy policy) : base(policy)
		{
		}

		[DataMember]
		public string Name
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowNonProvisionableDevices
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowNonProvisionableDevices;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllowNonProvisionableDevicesString
		{
			get
			{
				if (this.AllowNonProvisionableDevices)
				{
					return Strings.AllowNonProvisionable;
				}
				return Strings.DisallowNonProvisionable;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DisplayTitle
		{
			get
			{
				if (base.IsDefault)
				{
					return string.Format(Strings.DefaultEASPolicyDetailTitle, base.ActiveSyncMailboxPolicy.Name);
				}
				return string.Format(Strings.EASPolicyDetailTitle, base.ActiveSyncMailboxPolicy.Name);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PasswordRequirementsString
		{
			get
			{
				if (!this.DevicePasswordEnabled)
				{
					return Strings.PasswordNotRequired;
				}
				if (this.IsMaxInactivityTimeDeviceLockSet)
				{
					if (this.AlphanumericDevicePasswordRequired)
					{
						return string.Format(Strings.RequiredAlphaLockingPassword, base.ActiveSyncMailboxPolicy.MinDevicePasswordLength, base.ActiveSyncMailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalMinutes);
					}
					if (this.IsMinDevicePasswordLengthSet)
					{
						return string.Format(Strings.RequiredPinLockingPassword, base.ActiveSyncMailboxPolicy.MinDevicePasswordLength, base.ActiveSyncMailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalMinutes);
					}
					return string.Format(Strings.RequiredLockingPassword, base.ActiveSyncMailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalMinutes);
				}
				else
				{
					if (this.AlphanumericDevicePasswordRequired)
					{
						return string.Format(Strings.RequiredAlphaPassword, base.ActiveSyncMailboxPolicy.MinDevicePasswordLength);
					}
					if (this.IsMinDevicePasswordLengthSet)
					{
						return string.Format(Strings.RequiredPinPassword, base.ActiveSyncMailboxPolicy.MinDevicePasswordLength);
					}
					return Strings.PasswordRequired;
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool DevicePasswordEnabled
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.DevicePasswordEnabled;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public bool AllowSimpleDevicePassword
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowSimpleDevicePassword;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public bool AlphanumericDevicePasswordRequired
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AlphanumericDevicePasswordRequired;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public bool IsMinDevicePasswordLengthSet
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.MinDevicePasswordLength != null;
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string MinDevicePasswordLength
		{
			get
			{
				if (this.IsMinDevicePasswordLengthSet)
				{
					return base.ActiveSyncMailboxPolicy.MinDevicePasswordLength.Value.ToString();
				}
				return "4";
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public bool IsMaxDevicePasswordFailedAttemptsSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.MaxDevicePasswordFailedAttempts.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxDevicePasswordFailedAttempts
		{
			get
			{
				if (this.IsMaxDevicePasswordFailedAttemptsSet)
				{
					return base.ActiveSyncMailboxPolicy.MaxDevicePasswordFailedAttempts.Value.ToString(CultureInfo.InvariantCulture);
				}
				return "8";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxDevicePasswordFailedAttemptsString
		{
			get
			{
				if (this.IsMaxDevicePasswordFailedAttemptsSet)
				{
					return string.Format(Strings.MaxDevicePasswordFailedAttempts, base.ActiveSyncMailboxPolicy.MaxDevicePasswordFailedAttempts);
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsMaxInactivityTimeDeviceLockSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.MaxInactivityTimeDeviceLock.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxInactivityTimeDeviceLock
		{
			get
			{
				if (this.IsMaxInactivityTimeDeviceLockSet)
				{
					return base.ActiveSyncMailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture);
				}
				return "15";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool RequireDeviceEncryption
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.RequireDeviceEncryption;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RequireDeviceEncryptionString
		{
			get
			{
				if (this.RequireDeviceEncryption)
				{
					return Strings.DeviceEncryptionRequired;
				}
				return Strings.DeviceEncryptionNotRequired;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool RequireStorageCardEncryption
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.RequireStorageCardEncryption;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllowStorageCardString
		{
			get
			{
				if (base.ActiveSyncMailboxPolicy.AllowStorageCard)
				{
					return Strings.StorageCardAllowed;
				}
				return Strings.StorageCardNotAllowed;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllowCameraString
		{
			get
			{
				if (base.ActiveSyncMailboxPolicy.AllowCamera)
				{
					return Strings.CameraAllowed;
				}
				return Strings.CameraNotAllowed;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllowBrowserString
		{
			get
			{
				if (base.ActiveSyncMailboxPolicy.AllowBrowser)
				{
					return Strings.BrowserAllowed;
				}
				return Strings.BrowserNotAllowed;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RequireManualSyncWhenRoamingString
		{
			get
			{
				if (base.ActiveSyncMailboxPolicy.RequireManualSyncWhenRoaming)
				{
					return Strings.ManualRoamingSyncRequired;
				}
				return Strings.ManualRoamingSyncNotRequired;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsDevicePolicyRefreshIntervalSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.DevicePolicyRefreshInterval.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DevicePolicyRefreshInterval
		{
			get
			{
				if (this.IsDevicePolicyRefreshIntervalSet)
				{
					return base.ActiveSyncMailboxPolicy.DevicePolicyRefreshInterval.Value.TotalHours.ToString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool PasswordRecoveryEnabled
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.PasswordRecoveryEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int MinDevicePasswordComplexCharacters
		{
			get
			{
				if (this.AlphanumericDevicePasswordRequired)
				{
					return base.ActiveSyncMailboxPolicy.MinDevicePasswordComplexCharacters;
				}
				return 3;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsDevicePasswordExpirationSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.DevicePasswordExpiration.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DevicePasswordExpiration
		{
			get
			{
				if (this.IsDevicePasswordExpirationSet)
				{
					return base.ActiveSyncMailboxPolicy.DevicePasswordExpiration.Value.Days.ToString();
				}
				return "90";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int DevicePasswordHistory
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.DevicePasswordHistory;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxCalendarAgeFilter
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.MaxCalendarAgeFilter.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxEmailAgeFilter
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.MaxEmailAgeFilter.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsMaxEmailBodyTruncationSizeSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.MaxEmailBodyTruncationSize.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxEmailBodyTruncationSize
		{
			get
			{
				if (this.IsMaxEmailBodyTruncationSizeSet)
				{
					return base.ActiveSyncMailboxPolicy.MaxEmailBodyTruncationSize.Value.ToString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool RequireManualSyncWhenRoaming
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.RequireManualSyncWhenRoaming;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowHTMLEmail
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowHTMLEmail;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AttachmentsEnabled
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AttachmentsEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsMaxAttachmentSizeSet
		{
			get
			{
				return !base.ActiveSyncMailboxPolicy.MaxAttachmentSize.IsUnlimited;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MaxAttachmentSize
		{
			get
			{
				if (this.IsMaxAttachmentSizeSet)
				{
					return base.ActiveSyncMailboxPolicy.MaxAttachmentSize.Value.ToKB().ToString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowTextMessaging
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowTextMessaging;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowStorageCard
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowStorageCard;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowCamera
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowCamera;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowWiFi
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowWiFi;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowIrDA
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowIrDA;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowInternetSharing
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowInternetSharing;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool AllowBrowser
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowBrowser;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllowBluetooth
		{
			get
			{
				return base.ActiveSyncMailboxPolicy.AllowBluetooth.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		internal const string DefaultMinDevicePasswordLength = "4";

		internal const string DefaultMaxDevicePasswordFailedAttempts = "8";

		internal const string DefaultMaxInactivityTimeDeviceLock = "15";

		internal const int DefaultMinDevicePasswordComplexCharacters = 3;

		internal const string DefaultDevicePasswordExpiration = "90";
	}
}
