using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class MobileDevicePolicyData : SerializableDataBase, IEquatable<MobileDevicePolicyData>
	{
		[DataMember]
		public bool AlphanumericDevicePasswordRequired { get; set; }

		[DataMember]
		public bool DeviceEncryptionRequired { get; set; }

		[DataMember]
		public bool DevicePasswordRequired { get; set; }

		[DataMember]
		public int MinDevicePasswordComplexCharacters { get; set; }

		[DataMember]
		public int MinDevicePasswordHistory { get; set; }

		[DataMember]
		public int? MinDevicePasswordLength { get; set; }

		[DataMember]
		public bool SimpleDevicePasswordAllowed { get; set; }

		[DataMember]
		public bool AllowApplePushNotifications { get; set; }

		[DataMember]
		public bool AllowMicrosoftPushNotifications { get; set; }

		[DataMember]
		public bool AllowGooglePushNotifications { get; set; }

		[DataMember(Name = "MaxInactivityTimeDeviceLock")]
		public string MaxInactivityTimeDeviceLockString
		{
			get
			{
				if (this.MaxInactivityTimeDeviceLock.IsUnlimited)
				{
					return string.Empty;
				}
				return ((int)this.MaxInactivityTimeDeviceLock.Value.TotalSeconds).ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.MaxInactivityTimeDeviceLock = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
					return;
				}
				int num = int.Parse(value);
				this.MaxInactivityTimeDeviceLock = EnhancedTimeSpan.FromSeconds((double)num);
			}
		}

		[DataMember(Name = "MaxDevicePasswordExpiration")]
		public string MaxDevicePasswordExpirationString
		{
			get
			{
				if (this.MaxDevicePasswordExpiration.IsUnlimited)
				{
					return string.Empty;
				}
				return ((int)this.MaxDevicePasswordExpiration.Value.TotalDays).ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.MaxDevicePasswordExpiration = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
					return;
				}
				int num = int.Parse(value);
				this.MaxDevicePasswordExpiration = EnhancedTimeSpan.FromDays((double)num);
			}
		}

		[DataMember(Name = "MaxDevicePasswordFailedAttempts")]
		public string MaxDevicePasswordFailedAttemptsString
		{
			get
			{
				if (this.MaxDevicePasswordFailedAttempts.IsUnlimited)
				{
					return string.Empty;
				}
				return this.MaxDevicePasswordFailedAttempts.Value.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.MaxDevicePasswordFailedAttempts = Unlimited<int>.UnlimitedValue;
					return;
				}
				this.MaxDevicePasswordFailedAttempts = int.Parse(value);
			}
		}

		public string PolicyIdentifier
		{
			get
			{
				return this.GetHashCode().ToString();
			}
		}

		public Unlimited<EnhancedTimeSpan> MaxDevicePasswordExpiration { get; set; }

		public Unlimited<int> MaxDevicePasswordFailedAttempts { get; set; }

		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeDeviceLock { get; set; }

		public bool Equals(MobileDevicePolicyData obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj.AlphanumericDevicePasswordRequired == this.AlphanumericDevicePasswordRequired && obj.DeviceEncryptionRequired == this.DeviceEncryptionRequired && obj.DevicePasswordRequired == this.DevicePasswordRequired && obj.MaxDevicePasswordExpiration == this.MaxDevicePasswordExpiration && obj.MaxDevicePasswordFailedAttempts == this.MaxDevicePasswordFailedAttempts && obj.MaxInactivityTimeDeviceLock == this.MaxInactivityTimeDeviceLock && obj.MinDevicePasswordComplexCharacters == this.MinDevicePasswordComplexCharacters && obj.MinDevicePasswordHistory == this.MinDevicePasswordHistory && obj.MinDevicePasswordLength == this.MinDevicePasswordLength && obj.SimpleDevicePasswordAllowed == this.SimpleDevicePasswordAllowed && obj.AllowApplePushNotifications == this.AllowApplePushNotifications && obj.AllowMicrosoftPushNotifications == this.AllowMicrosoftPushNotifications && obj.AllowGooglePushNotifications == this.AllowGooglePushNotifications));
		}

		public override string ToString()
		{
			if (this.toStringValue == null)
			{
				this.toStringValue = string.Join(",", new object[]
				{
					this.AlphanumericDevicePasswordRequired,
					this.DeviceEncryptionRequired,
					this.DevicePasswordRequired,
					this.MaxDevicePasswordExpiration,
					this.MaxDevicePasswordFailedAttempts,
					this.MaxInactivityTimeDeviceLock,
					this.MinDevicePasswordComplexCharacters,
					this.MinDevicePasswordHistory,
					this.MinDevicePasswordLength,
					this.SimpleDevicePasswordAllowed,
					this.AllowApplePushNotifications,
					this.AllowMicrosoftPushNotifications,
					this.AllowGooglePushNotifications
				});
			}
			return this.toStringValue;
		}

		protected override int InternalGetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as MobileDevicePolicyData);
		}

		private string toStringValue;
	}
}
