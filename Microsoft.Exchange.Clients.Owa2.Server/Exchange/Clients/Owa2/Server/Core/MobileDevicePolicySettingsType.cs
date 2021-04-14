using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MobileDevicePolicySettingsType
	{
		public MobileDevicePolicySettingsType()
		{
			this.AlphanumericDevicePasswordRequired = false;
			this.DeviceEncryptionRequired = false;
			this.DevicePasswordRequired = false;
			this.MaxDevicePasswordExpirationString = string.Empty;
			this.MaxDevicePasswordFailedAttemptsString = string.Empty;
			this.MaxInactivityTimeDeviceLockString = string.Empty;
			this.MinDevicePasswordComplexCharacters = 1;
			this.MinDevicePasswordHistory = 0;
			this.MinDevicePasswordLength = null;
			this.policyIdentifier = string.Empty;
			this.simpleDevicePasswordAllowed = true;
			this.allowApplePushNotifications = true;
			this.allowMicrosoftPushNotifications = true;
			this.allowGooglePushNotifications = true;
		}

		[DataMember]
		public bool AlphanumericDevicePasswordRequired
		{
			get
			{
				return this.alphanumericDevicePasswordRequired;
			}
			set
			{
				this.alphanumericDevicePasswordRequired = value;
			}
		}

		[DataMember]
		public bool DeviceEncryptionRequired
		{
			get
			{
				return this.deviceEncryptionRequired;
			}
			set
			{
				this.deviceEncryptionRequired = value;
			}
		}

		[DataMember]
		public bool DevicePasswordRequired
		{
			get
			{
				return this.devicePasswordRequired;
			}
			set
			{
				this.devicePasswordRequired = value;
			}
		}

		[DataMember(Name = "MaxDevicePasswordExpiration")]
		public string MaxDevicePasswordExpirationString
		{
			get
			{
				return this.maxDevicePasswordExpirationString;
			}
			set
			{
				this.maxDevicePasswordExpirationString = value;
			}
		}

		[DataMember(Name = "MaxDevicePasswordFailedAttempts")]
		public string MaxDevicePasswordFailedAttemptsString
		{
			get
			{
				return this.maxDevicePasswordFailedAttemptsString;
			}
			set
			{
				this.maxDevicePasswordFailedAttemptsString = value;
			}
		}

		[DataMember(Name = "MaxInactivityTimeDeviceLock")]
		public string MaxInactivityTimeDeviceLockString
		{
			get
			{
				return this.maxInactivityTimeDeviceLockString;
			}
			set
			{
				this.maxInactivityTimeDeviceLockString = value;
			}
		}

		[DataMember]
		public int MinDevicePasswordComplexCharacters
		{
			get
			{
				return this.minDevicePasswordComplexCharacters;
			}
			set
			{
				this.minDevicePasswordComplexCharacters = value;
			}
		}

		[DataMember]
		public int MinDevicePasswordHistory
		{
			get
			{
				return this.minDevicePasswordHistory;
			}
			set
			{
				this.minDevicePasswordHistory = value;
			}
		}

		public int? MinDevicePasswordLength
		{
			get
			{
				return this.minDevicePasswordLength;
			}
			set
			{
				this.minDevicePasswordLength = value;
			}
		}

		[DataMember(Name = "MinDevicePasswordLength")]
		public string MinDevicePasswordLengthString
		{
			get
			{
				if (this.MinDevicePasswordLength == null)
				{
					return string.Empty;
				}
				return this.MinDevicePasswordLength.Value.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.MinDevicePasswordLength = null;
					return;
				}
				this.MinDevicePasswordLength = new int?(int.Parse(value));
			}
		}

		[DataMember]
		public string PolicyIdentifier
		{
			get
			{
				return this.policyIdentifier;
			}
			set
			{
				this.policyIdentifier = value;
			}
		}

		[DataMember]
		public bool SimpleDevicePasswordAllowed
		{
			get
			{
				return this.simpleDevicePasswordAllowed;
			}
			set
			{
				this.simpleDevicePasswordAllowed = value;
			}
		}

		[DataMember]
		public bool AllowApplePushNotifications
		{
			get
			{
				return this.allowApplePushNotifications;
			}
			set
			{
				this.allowApplePushNotifications = value;
			}
		}

		[DataMember]
		public bool AllowMicrosoftPushNotifications
		{
			get
			{
				return this.allowMicrosoftPushNotifications;
			}
			set
			{
				this.allowMicrosoftPushNotifications = value;
			}
		}

		[DataMember]
		public bool AllowGooglePushNotifications
		{
			get
			{
				return this.allowGooglePushNotifications;
			}
			set
			{
				this.allowGooglePushNotifications = value;
			}
		}

		private bool alphanumericDevicePasswordRequired;

		private bool deviceEncryptionRequired;

		private bool devicePasswordRequired;

		private string maxDevicePasswordExpirationString;

		private string maxDevicePasswordFailedAttemptsString;

		private string maxInactivityTimeDeviceLockString;

		private int minDevicePasswordComplexCharacters;

		private int minDevicePasswordHistory;

		private int? minDevicePasswordLength;

		private string policyIdentifier;

		private bool simpleDevicePasswordAllowed;

		private bool allowApplePushNotifications;

		private bool allowMicrosoftPushNotifications;

		private bool allowGooglePushNotifications;
	}
}
