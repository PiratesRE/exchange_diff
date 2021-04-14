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
	public sealed class ActiveSyncMailboxPolicy : MobileMailboxPolicy
	{
		[Parameter(Mandatory = false)]
		public bool AlphanumericDevicePasswordRequired
		{
			get
			{
				return (bool)this[ActiveSyncMailboxPolicySchema.AlphanumericDevicePasswordRequired];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.AlphanumericDevicePasswordRequired] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DevicePasswordEnabled
		{
			get
			{
				return (bool)this[ActiveSyncMailboxPolicySchema.DevicePasswordEnabled];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.DevicePasswordEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowSimpleDevicePassword
		{
			get
			{
				return (bool)this[ActiveSyncMailboxPolicySchema.AllowSimpleDevicePassword];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.AllowSimpleDevicePassword] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MinDevicePasswordLength
		{
			get
			{
				return (int?)this[ActiveSyncMailboxPolicySchema.MinDevicePasswordLength];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.MinDevicePasswordLength] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDefaultPolicy
		{
			get
			{
				return (bool)this[ActiveSyncMailboxPolicySchema.IsDefaultPolicy];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.IsDefaultPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> MaxInactivityTimeDeviceLock
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[ActiveSyncMailboxPolicySchema.MaxInactivityTimeDeviceLock];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.MaxInactivityTimeDeviceLock] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxDevicePasswordFailedAttempts
		{
			get
			{
				return (Unlimited<int>)this[ActiveSyncMailboxPolicySchema.MaxDevicePasswordFailedAttempts];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.MaxDevicePasswordFailedAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> DevicePasswordExpiration
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[ActiveSyncMailboxPolicySchema.DevicePasswordExpiration];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.DevicePasswordExpiration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int DevicePasswordHistory
		{
			get
			{
				return (int)this[ActiveSyncMailboxPolicySchema.DevicePasswordHistory];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.DevicePasswordHistory] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MinDevicePasswordComplexCharacters
		{
			get
			{
				return (int)this[ActiveSyncMailboxPolicySchema.MinDevicePasswordComplexCharacters];
			}
			set
			{
				this[ActiveSyncMailboxPolicySchema.MinDevicePasswordComplexCharacters] = value;
			}
		}

		private new bool AlphanumericPasswordRequired
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new bool PasswordEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new bool AllowSimplePassword
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new int? MinPasswordLength
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new Unlimited<EnhancedTimeSpan> MaxInactivityTimeLock
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new Unlimited<int> MaxPasswordFailedAttempts
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new Unlimited<EnhancedTimeSpan> PasswordExpiration
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new int PasswordHistory
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new int MinPasswordComplexCharacters
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private new bool AllowMicrosoftPushNotifications
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private new bool AllowGooglePushNotifications
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
