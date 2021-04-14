using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class BaseActiveSyncMailboxPolicyParams : SetObjectProperties
	{
		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[DataMember]
		public bool IsDefault
		{
			get
			{
				return (bool)(base["IsDefault"] ?? false);
			}
			set
			{
				base["IsDefault"] = value;
			}
		}

		[DataMember]
		public bool AllowNonProvisionableDevices
		{
			get
			{
				return (bool)(base["AllowNonProvisionableDevices"] ?? false);
			}
			set
			{
				base["AllowNonProvisionableDevices"] = value;
			}
		}

		[DataMember]
		public bool? IsDevicePolicyRefreshIntervalSet { get; set; }

		[DataMember]
		public string DevicePolicyRefreshInterval { get; set; }

		[DataMember]
		public bool? RequireDeviceEncryption { get; set; }

		[DataMember]
		public bool? RequireStorageCardEncryption { get; set; }

		[DataMember]
		public bool? DevicePasswordEnabled
		{
			get
			{
				return (bool?)base["DevicePasswordEnabled"];
			}
			set
			{
				base["DevicePasswordEnabled"] = value;
			}
		}

		[DataMember]
		public bool? AllowSimpleDevicePassword { get; set; }

		[DataMember]
		public bool? AlphanumericDevicePasswordRequired { get; set; }

		[DataMember]
		public bool? IsMinDevicePasswordLengthSet { get; set; }

		[DataMember]
		public string MinDevicePasswordLength { get; set; }

		[DataMember]
		public int? MinDevicePasswordComplexCharacters { get; set; }

		[DataMember]
		public bool? IsMaxDevicePasswordFailedAttemptsSet { get; set; }

		[DataMember]
		public string MaxDevicePasswordFailedAttempts { get; set; }

		[DataMember]
		public bool? IsMaxInactivityTimeDeviceLockSet { get; set; }

		[DataMember]
		public string MaxInactivityTimeDeviceLock { get; set; }

		[DataMember]
		public bool? PasswordRecoveryEnabled { get; set; }

		[DataMember]
		public bool? IsDevicePasswordExpirationSet { get; set; }

		[DataMember]
		public string DevicePasswordExpiration { get; set; }

		[DataMember]
		public int? DevicePasswordHistory { get; set; }

		[DataMember]
		public string MaxCalendarAgeFilter
		{
			get
			{
				return (string)base["MaxCalendarAgeFilter"];
			}
			set
			{
				base["MaxCalendarAgeFilter"] = value;
			}
		}

		[DataMember]
		public string MaxEmailAgeFilter
		{
			get
			{
				return (string)base["MaxEmailAgeFilter"];
			}
			set
			{
				base["MaxEmailAgeFilter"] = value;
			}
		}

		[DataMember]
		public bool? IsMaxEmailBodyTruncationSizeSet { get; set; }

		[DataMember]
		public string MaxEmailBodyTruncationSize { get; set; }

		[DataMember]
		public bool? IsMaxAttachmentSizeSet { get; set; }

		[DataMember]
		public string MaxAttachmentSize { get; set; }

		[DataMember]
		public bool RequireManualSyncWhenRoaming
		{
			get
			{
				return (bool)(base["RequireManualSyncWhenRoaming"] ?? false);
			}
			set
			{
				base["RequireManualSyncWhenRoaming"] = value;
			}
		}

		[DataMember]
		public bool AttachmentsEnabled
		{
			get
			{
				return (bool)(base["AttachmentsEnabled"] ?? false);
			}
			set
			{
				base["AttachmentsEnabled"] = value;
			}
		}

		[DataMember]
		public bool AllowHTMLEmail
		{
			get
			{
				return (bool)(base["AllowHTMLEmail"] ?? false);
			}
			set
			{
				base["AllowHTMLEmail"] = value;
			}
		}

		[DataMember]
		public bool AllowTextMessaging
		{
			get
			{
				return (bool)(base["AllowTextMessaging"] ?? false);
			}
			set
			{
				base["AllowTextMessaging"] = value;
			}
		}

		[DataMember]
		public bool AllowStorageCard
		{
			get
			{
				return (bool)(base["AllowStorageCard"] ?? false);
			}
			set
			{
				base["AllowStorageCard"] = value;
			}
		}

		[DataMember]
		public bool AllowCamera
		{
			get
			{
				return (bool)(base["AllowCamera"] ?? false);
			}
			set
			{
				base["AllowCamera"] = value;
			}
		}

		[DataMember]
		public bool AllowWiFi
		{
			get
			{
				return (bool)(base["AllowWiFi"] ?? false);
			}
			set
			{
				base["AllowWiFi"] = value;
			}
		}

		[DataMember]
		public bool AllowIrDA
		{
			get
			{
				return (bool)(base["AllowIrDA"] ?? false);
			}
			set
			{
				base["AllowIrDA"] = value;
			}
		}

		[DataMember]
		public bool AllowInternetSharing
		{
			get
			{
				return (bool)(base["AllowInternetSharing"] ?? false);
			}
			set
			{
				base["AllowInternetSharing"] = value;
			}
		}

		[DataMember]
		public bool AllowBrowser
		{
			get
			{
				return (bool)(base["AllowBrowser"] ?? false);
			}
			set
			{
				base["AllowBrowser"] = value;
			}
		}

		[DataMember]
		public string AllowBluetooth
		{
			get
			{
				return (string)base["AllowBluetooth"];
			}
			set
			{
				base["AllowBluetooth"] = value;
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		protected bool CheckAndParseParams<T>(bool isUnlimited, bool? isValueSet, bool? originalIsValueSet, string value, Func<string, T> convert, out object result) where T : struct, IComparable
		{
			bool result2 = false;
			result = null;
			if (isValueSet == false)
			{
				if (isUnlimited)
				{
					result = Unlimited<T>.UnlimitedString;
				}
				else
				{
					result = null;
				}
				result2 = true;
			}
			else if ((isValueSet == true || (isValueSet == null && originalIsValueSet == true)) && !string.IsNullOrEmpty(value))
			{
				try
				{
					if (isUnlimited)
					{
						result = new Unlimited<T>(convert(value));
					}
					else
					{
						result = convert(value);
					}
					result2 = true;
				}
				catch (Exception)
				{
					result2 = false;
				}
			}
			return result2;
		}

		public virtual void ProcessPolicyParams(ActiveSyncMailboxPolicyObject originalPolicy)
		{
			bool flag = originalPolicy == null;
			bool flag2 = this.DevicePasswordEnabled ?? (!flag && originalPolicy.DevicePasswordEnabled);
			object value;
			if (this.CheckAndParseParams<EnhancedTimeSpan>(true, this.IsDevicePolicyRefreshIntervalSet, flag ? null : new bool?(originalPolicy.IsDevicePolicyRefreshIntervalSet), this.DevicePolicyRefreshInterval, (string x) => EnhancedTimeSpan.FromHours(double.Parse(x)), out value))
			{
				base["DevicePolicyRefreshInterval"] = value;
			}
			if (this.CheckAndParseParams<int>(true, this.IsMaxEmailBodyTruncationSizeSet, flag ? null : new bool?(originalPolicy.IsMaxEmailBodyTruncationSizeSet), this.MaxEmailBodyTruncationSize, (string x) => int.Parse(x), out value))
			{
				base["MaxEmailBodyTruncationSize"] = value;
			}
			if (this.CheckAndParseParams<ByteQuantifiedSize>(true, this.IsMaxAttachmentSizeSet, flag ? null : new bool?(originalPolicy.IsMaxAttachmentSizeSet), this.MaxAttachmentSize, (string x) => ByteQuantifiedSize.FromKB(ulong.Parse(x)), out value))
			{
				base["MaxAttachmentSize"] = value;
			}
			if (flag2)
			{
				if (this.AlphanumericDevicePasswordRequired == true || (!flag && this.AlphanumericDevicePasswordRequired == null && originalPolicy.AlphanumericDevicePasswordRequired))
				{
					int? minDevicePasswordComplexCharacters = this.MinDevicePasswordComplexCharacters;
					int? num = (minDevicePasswordComplexCharacters != null) ? new int?(minDevicePasswordComplexCharacters.GetValueOrDefault()) : ((flag || originalPolicy.AlphanumericDevicePasswordRequired) ? this.MinDevicePasswordComplexCharacters : new int?(3));
					if (num != null)
					{
						base["MinDevicePasswordComplexCharacters"] = num;
					}
				}
				if (this.CheckAndParseParams<int>(false, this.IsMinDevicePasswordLengthSet, flag ? null : new bool?(originalPolicy.IsMinDevicePasswordLengthSet), this.MinDevicePasswordLength ?? ((flag || originalPolicy.IsMinDevicePasswordLengthSet) ? this.MinDevicePasswordLength : "4"), (string x) => int.Parse(x), out value))
				{
					base["MinDevicePasswordLength"] = value;
				}
				if (this.CheckAndParseParams<int>(false, this.IsMinDevicePasswordLengthSet, flag ? null : new bool?(originalPolicy.IsMinDevicePasswordLengthSet), this.MinDevicePasswordLength ?? ((flag || originalPolicy.IsMinDevicePasswordLengthSet) ? this.MinDevicePasswordLength : "4"), (string x) => int.Parse(x), out value))
				{
					base["MinDevicePasswordLength"] = value;
				}
				if (this.CheckAndParseParams<int>(true, this.IsMaxDevicePasswordFailedAttemptsSet, flag ? null : new bool?(originalPolicy.IsMaxDevicePasswordFailedAttemptsSet), this.MaxDevicePasswordFailedAttempts ?? ((flag || originalPolicy.IsMaxDevicePasswordFailedAttemptsSet) ? this.MaxDevicePasswordFailedAttempts : "8"), (string x) => int.Parse(x), out value))
				{
					base["MaxDevicePasswordFailedAttempts"] = value;
				}
				if (this.CheckAndParseParams<EnhancedTimeSpan>(true, this.IsDevicePasswordExpirationSet, flag ? null : new bool?(originalPolicy.IsDevicePasswordExpirationSet), this.DevicePasswordExpiration ?? ((flag || originalPolicy.IsDevicePasswordExpirationSet) ? this.DevicePasswordExpiration : "90"), (string x) => EnhancedTimeSpan.FromDays(double.Parse(x)), out value))
				{
					base["DevicePasswordExpiration"] = value;
				}
				if (this.CheckAndParseParams<EnhancedTimeSpan>(true, this.IsMaxInactivityTimeDeviceLockSet, flag ? null : new bool?(originalPolicy.IsMaxInactivityTimeDeviceLockSet), this.MaxInactivityTimeDeviceLock ?? ((flag || originalPolicy.IsMaxInactivityTimeDeviceLockSet) ? this.MaxInactivityTimeDeviceLock : "15"), (string x) => EnhancedTimeSpan.FromMinutes(double.Parse(x)), out value))
				{
					base["MaxInactivityTimeDeviceLock"] = value;
				}
				if (this.RequireDeviceEncryption != null)
				{
					base["RequireDeviceEncryption"] = this.RequireDeviceEncryption;
				}
				if (this.RequireStorageCardEncryption != null)
				{
					base["RequireStorageCardEncryption"] = this.RequireStorageCardEncryption;
				}
				if (this.AllowSimpleDevicePassword != null)
				{
					base["AllowSimpleDevicePassword"] = this.AllowSimpleDevicePassword;
				}
				if (this.AlphanumericDevicePasswordRequired != null)
				{
					base["AlphanumericDevicePasswordRequired"] = this.AlphanumericDevicePasswordRequired;
				}
				if (this.PasswordRecoveryEnabled != null)
				{
					base["PasswordRecoveryEnabled"] = this.PasswordRecoveryEnabled;
				}
				if (this.DevicePasswordHistory != null)
				{
					base["DevicePasswordHistory"] = this.DevicePasswordHistory;
				}
			}
		}
	}
}
