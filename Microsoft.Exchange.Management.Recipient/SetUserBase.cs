using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetUserBase<TIdentity, TPublicObject> : SetMailEnabledOrgPersonObjectTask<TIdentity, TPublicObject, ADUser> where TIdentity : IIdentityParameter, new() where TPublicObject : MailEnabledOrgPerson, new()
	{
		protected bool IsSetRandomPassword
		{
			get
			{
				return this.isSetRandomPassword;
			}
			set
			{
				this.isSetRandomPassword = value;
			}
		}

		protected bool IsChangingOnPassword
		{
			get
			{
				return this.Password != null && this.Password.Length > 0;
			}
		}

		protected bool HasSetPasswordPermission
		{
			get
			{
				return (base.CurrentTaskContext.InvocationInfo.IsCmdletInvokedWithoutPSFramework && this.DataObject.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox) || (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SetPasswordWithoutOldPassword.Enabled && base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.ExecutingUserHasResetPasswordPermission);
			}
		}

		[Parameter(Mandatory = false)]
		public Capability SKUCapability
		{
			get
			{
				return (Capability)(base.Fields["SKUCapability"] ?? Capability.None);
			}
			set
			{
				base.VerifyValues<Capability>(CapabilityHelper.AllowedSKUCapabilities, value);
				base.Fields["SKUCapability"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return (MultiValuedProperty<Capability>)(base.Fields["AddOnSKUCapability"] ?? new MultiValuedProperty<Capability>());
			}
			set
			{
				if (value != null)
				{
					base.VerifyValues<Capability>(CapabilityHelper.AllowedSKUCapabilities, value.ToArray());
				}
				base.Fields["AddOnSKUCapability"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassLiveId
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassLiveId"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BypassLiveId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NetID NetID
		{
			get
			{
				return (NetID)base.Fields["NetID"];
			}
			set
			{
				base.Fields["NetID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FederatedIdentity
		{
			get
			{
				return (string)base.Fields["FederatedIdentity"];
			}
			set
			{
				base.Fields["FederatedIdentity"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.NetID != null && !this.BypassLiveId)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorNetIDWithoutBypassWLIDInSet), ExchangeErrorCategory.Client, null);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ADUser)base.PrepareDataObject();
			if (this.DataObject.IsChanged(MailboxSchema.WindowsLiveID) && this.DataObject.WindowsLiveID != SmtpAddress.Empty)
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorEnableWindowsLiveIdForEnterpriseMailbox), ExchangeErrorCategory.Client, this.DataObject.Identity);
				}
				if (this.ShouldCheckAcceptedDomains())
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(this.ConfigurationSession, this.DataObject.OrganizationId, this.DataObject.WindowsLiveID.Domain, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
				}
			}
			if (this.DataObject.WindowsLiveID != SmtpAddress.Empty && !this.DataObject.WindowsLiveID.Equals(this.DataObject.UserPrincipalName))
			{
				this.WriteWarning(Strings.WarningChangingUserPrincipalName(this.DataObject.UserPrincipalName, this.DataObject.WindowsLiveID.ToString()));
				this.DataObject.UserPrincipalName = this.DataObject.WindowsLiveID.ToString();
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			this.IsSetRandomPassword = false;
			if (this.DataObject.IsChanged(MailboxSchema.WindowsLiveID))
			{
				this.IsSetRandomPassword = true;
				if (this.DataObject.IsChanged(MailboxSchema.NetID))
				{
					MailboxTaskHelper.IsLiveIdExists((IRecipientSession)base.DataSession, this.DataObject.WindowsLiveID, this.DataObject.NetID, new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (base.Fields.IsModified("SKUCapability"))
			{
				this.DataObject.SKUCapability = new Capability?(this.SKUCapability);
			}
			if (base.Fields.IsModified("AddOnSKUCapability"))
			{
				CapabilityHelper.SetAddOnSKUCapabilities(this.AddOnSKUCapability, this.DataObject.PersistedCapabilities);
				RecipientTaskHelper.UpgradeArchiveQuotaOnArchiveAddOnSKU(this.DataObject, this.DataObject.PersistedCapabilities);
			}
			if (this.IsChangingOnPassword && this.HasSetPasswordPermission)
			{
				((IRecipientSession)base.DataSession).SetPassword(this.DataObject, this.Password);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override bool IsObjectStateChanged()
		{
			return base.IsObjectStateChanged() || this.IsChangingOnPassword;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.FederatedIdentity != null && this.Password != null)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorFederatedIdentityandPasswordTogether), ExchangeErrorCategory.Client, this.DataObject.Identity);
			}
			if (this.DataObject.IsChanged(MailUserSchema.WindowsLiveID))
			{
				MailboxTaskHelper.IsMemberExists((IRecipientSession)base.DataSession, this.DataObject.WindowsLiveID, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.DataObject.IsModified(MailUserSchema.UserPrincipalName))
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ValidateExternalEmailAddressInAcceptedDomain.Enabled && this.ShouldCheckAcceptedDomains())
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(this.ConfigurationSession, this.DataObject.OrganizationId, RecipientTaskHelper.GetDomainPartOfUserPrincalName(this.DataObject.UserPrincipalName), new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
				}
				RecipientTaskHelper.IsUserPrincipalNameUnique(base.TenantGlobalCatalogSession, this.DataObject, this.DataObject.UserPrincipalName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (this.DataObject.IsChanged(MailboxSchema.JournalArchiveAddress) && this.DataObject.JournalArchiveAddress != SmtpAddress.NullReversePath && this.DataObject.JournalArchiveAddress != SmtpAddress.Empty)
			{
				RecipientTaskHelper.IsJournalArchiveAddressUnique(base.TenantGlobalCatalogSession, this.DataObject.OrganizationId, this.DataObject, this.DataObject.JournalArchiveAddress, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			if (this.DataObject.IsModified(MailUserSchema.SamAccountName))
			{
				RecipientTaskHelper.IsSamAccountNameUnique(this.DataObject, this.DataObject.SamAccountName, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client);
			}
			TaskLogger.LogExit();
		}

		private bool isSetRandomPassword;
	}
}
