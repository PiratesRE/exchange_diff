using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ActiveSyncMailboxPolicySchema : MobileMailboxPolicySchema
	{
		public static readonly ADPropertyDefinition MaxDevicePasswordFailedAttempts = MobileMailboxPolicySchema.MaxPasswordFailedAttempts;

		public static readonly ADPropertyDefinition MaxInactivityTimeDeviceLock = MobileMailboxPolicySchema.MaxInactivityTimeLock;

		public static readonly ADPropertyDefinition DevicePasswordExpiration = MobileMailboxPolicySchema.PasswordExpiration;

		public static readonly ADPropertyDefinition DevicePasswordHistory = MobileMailboxPolicySchema.PasswordHistory;

		public static readonly ADPropertyDefinition MinDevicePasswordLength = MobileMailboxPolicySchema.MinPasswordLength;

		public static readonly ADPropertyDefinition MinDevicePasswordComplexCharacters = MobileMailboxPolicySchema.MinPasswordComplexCharacters;

		public static readonly ADPropertyDefinition AlphanumericDevicePasswordRequired = MobileMailboxPolicySchema.AlphanumericPasswordRequired;

		public static readonly ADPropertyDefinition DevicePasswordEnabled = MobileMailboxPolicySchema.PasswordEnabled;

		public static readonly ADPropertyDefinition AllowSimpleDevicePassword = MobileMailboxPolicySchema.AllowSimplePassword;

		public static readonly ADPropertyDefinition IsDefaultPolicy = MobileMailboxPolicySchema.IsDefault;
	}
}
