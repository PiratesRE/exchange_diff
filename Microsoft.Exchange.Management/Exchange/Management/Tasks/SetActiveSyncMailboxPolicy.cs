using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("set", "ActiveSyncMailboxPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetActiveSyncMailboxPolicy : SetMailboxPolicyBase<ActiveSyncMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.updateOtherDefaultPolicies)
				{
					return Strings.ConfirmationMessageSwitchActiveSyncDefaultPolicy(this.Identity.ToString());
				}
				return base.ConfirmationMessage;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsDefault)
			{
				QueryFilter extraFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, this.DataObject.Id.ObjectGuid);
				this.otherDefaultPolicies = DefaultMobileMailboxPolicyUtility<ActiveSyncMailboxPolicy>.GetDefaultPolicies((IConfigurationSession)base.DataSession, extraFilter);
				if (this.otherDefaultPolicies.Count > 0)
				{
					this.updateOtherDefaultPolicies = true;
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
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Set-ActiveSyncMailboxPolicy", "Set-MobileDeviceMailboxPolicy"));
			if (!this.DataObject.DevicePasswordEnabled && base.UserSpecifiedParameters.Contains("DevicePasswordEnabled") && !base.UserSpecifiedParameters.Contains("AllowSimpleDevicePassword") && !base.UserSpecifiedParameters.Contains("AlphanumericDevicePasswordRequired") && !base.UserSpecifiedParameters.Contains("MinDevicePasswordComplexCharacters") && !base.UserSpecifiedParameters.Contains("MinDevicePasswordLength") && !base.UserSpecifiedParameters.Contains("MaxDevicePasswordFailedAttempts") && !base.UserSpecifiedParameters.Contains("MaxInactivityTimeDeviceLock") && !base.UserSpecifiedParameters.Contains("PasswordRecoveryEnabled") && !base.UserSpecifiedParameters.Contains("DevicePasswordExpiration") && !base.UserSpecifiedParameters.Contains("DevicePasswordHistory") && !base.UserSpecifiedParameters.Contains("RequireDeviceEncryption") && !base.UserSpecifiedParameters.Contains("RequireStorageCardEncryption") && !base.UserSpecifiedParameters.Contains("DeviceEncryptionEnabled"))
			{
				this.DataObject.AllowSimpleDevicePassword = (bool)ActiveSyncMailboxPolicySchema.AllowSimpleDevicePassword.DefaultValue;
				this.DataObject.AlphanumericDevicePasswordRequired = (bool)ActiveSyncMailboxPolicySchema.AlphanumericDevicePasswordRequired.DefaultValue;
				this.DataObject.MinDevicePasswordComplexCharacters = (int)ActiveSyncMailboxPolicySchema.MinDevicePasswordComplexCharacters.DefaultValue;
				this.DataObject.MinDevicePasswordLength = (int?)ActiveSyncMailboxPolicySchema.MinDevicePasswordLength.DefaultValue;
				this.DataObject.MaxDevicePasswordFailedAttempts = (Unlimited<int>)ActiveSyncMailboxPolicySchema.MaxDevicePasswordFailedAttempts.DefaultValue;
				this.DataObject.MaxInactivityTimeDeviceLock = (Unlimited<EnhancedTimeSpan>)ActiveSyncMailboxPolicySchema.MaxInactivityTimeDeviceLock.DefaultValue;
				this.DataObject.PasswordRecoveryEnabled = (bool)MobileMailboxPolicySchema.PasswordRecoveryEnabled.DefaultValue;
				this.DataObject.DevicePasswordExpiration = (Unlimited<EnhancedTimeSpan>)ActiveSyncMailboxPolicySchema.DevicePasswordExpiration.DefaultValue;
				this.DataObject.DevicePasswordHistory = (int)ActiveSyncMailboxPolicySchema.DevicePasswordHistory.DefaultValue;
				this.DataObject.RequireDeviceEncryption = (bool)MobileMailboxPolicySchema.RequireDeviceEncryption.DefaultValue;
				this.DataObject.DeviceEncryptionEnabled = (bool)MobileMailboxPolicySchema.RequireStorageCardEncryption.DefaultValue;
				this.DataObject.RequireStorageCardEncryption = (bool)MobileMailboxPolicySchema.RequireStorageCardEncryption.DefaultValue;
			}
			base.InternalProcessRecord();
			if (this.updateOtherDefaultPolicies)
			{
				try
				{
					DefaultMailboxPolicyUtility<ActiveSyncMailboxPolicy>.ClearDefaultPolicies((IConfigurationSession)base.DataSession, this.otherDefaultPolicies);
				}
				catch (DataSourceTransientException exception)
				{
					base.WriteError(exception, ErrorCategory.ReadError, null);
				}
			}
		}
	}
}
