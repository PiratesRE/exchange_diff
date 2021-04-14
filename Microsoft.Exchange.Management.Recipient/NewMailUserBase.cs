using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class NewMailUserBase : NewUserBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("WindowsLiveID" == base.ParameterSetName || "MicrosoftOnlineServicesID" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewMailuserWithWindowsLiveId(base.Name.ToString(), base.WindowsLiveID.SmtpAddress.ToString(), base.RecipientContainerId.ToString());
				}
				return Strings.ConfirmationMessageNewMailUser(base.Name.ToString(), this.ExternalEmailAddress.ToString(), this.UserPrincipalName.ToString(), base.RecipientContainerId.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = true, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = true, ParameterSetName = "WindowsLiveID")]
		public override SecureString Password
		{
			get
			{
				return base.Password;
			}
			set
			{
				base.Password = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		public override string UserPrincipalName
		{
			get
			{
				return base.UserPrincipalName;
			}
			set
			{
				base.UserPrincipalName = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "FederatedUser")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = true, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = true, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return this.DataObject.ExternalEmailAddress;
			}
			set
			{
				this.DataObject.ExternalEmailAddress = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		public virtual bool UsePreferMessageFormat
		{
			get
			{
				return this.DataObject.UsePreferMessageFormat;
			}
			set
			{
				this.DataObject.UsePreferMessageFormat = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public virtual MessageFormat MessageFormat
		{
			get
			{
				return this.DataObject.MessageFormat;
			}
			set
			{
				this.DataObject.MessageFormat = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public virtual MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return this.DataObject.MessageBodyFormat;
			}
			set
			{
				this.DataObject.MessageBodyFormat = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public virtual MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return this.DataObject.MacAttachmentFormat;
			}
			set
			{
				this.DataObject.MacAttachmentFormat = value;
			}
		}

		[Parameter]
		public bool RemotePowerShellEnabled
		{
			get
			{
				return (bool)base.Fields[MailUserSchema.RemotePowerShellEnabled];
			}
			set
			{
				base.Fields[MailUserSchema.RemotePowerShellEnabled] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveID")]
		[Parameter(Mandatory = false, ParameterSetName = "DisabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "WindowsLiveCustomDomains")]
		[Parameter(Mandatory = false, ParameterSetName = "ImportLiveId")]
		[Parameter(Mandatory = false, ParameterSetName = "MicrosoftOnlineServicesID")]
		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		public CountryInfo UsageLocation
		{
			get
			{
				return this.DataObject.UsageLocation;
			}
			set
			{
				this.DataObject.UsageLocation = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)base.Fields[ADRecipientSchema.MailboxProvisioningConstraint];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningConstraint] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)base.Fields[ADRecipientSchema.MailboxProvisioningPreferences];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailboxProvisioningPreferences] = value;
			}
		}

		public NewMailUserBase()
		{
		}

		protected override void StampDefaultValues(ADUser dataObject)
		{
			dataObject.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.StampDefaultValues(dataObject);
			dataObject.StampDefaultValues(RecipientType.MailUser);
		}

		protected override void PrepareUserObject(ADUser user)
		{
			TaskLogger.LogEnter();
			if (base.WindowsLiveID == null && base.SoftDeletedObject == null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled && !RecipientTaskHelper.SMTPAddressCheckWithAcceptedDomain(this.ConfigurationSession, user.OrganizationId, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorWindowsLiveIdRequired(user.Name)), ExchangeErrorCategory.Client, null);
			}
			if (base.WindowsLiveID != null && base.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				if (this.ExternalEmailAddress == null)
				{
					user.ExternalEmailAddress = ProxyAddress.Parse(base.WindowsLiveID.SmtpAddress.ToString());
				}
				user.UserPrincipalName = base.WindowsLiveID.SmtpAddress.ToString();
				base.IsSetRandomPassword = (base.SoftDeletedObject == null || base.IsSetRandomPassword);
			}
			if (string.IsNullOrEmpty(user.UserPrincipalName))
			{
				user.UserPrincipalName = RecipientTaskHelper.GenerateUniqueUserPrincipalName(base.TenantGlobalCatalogSession, user.Name, this.ConfigurationSession.GetDefaultAcceptedDomain().DomainName.Domain, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (base.SoftDeletedObject == null)
			{
				if (base.Fields.IsModified(MailUserSchema.RemotePowerShellEnabled))
				{
					user.RemotePowerShellEnabled = this.RemotePowerShellEnabled;
				}
				else
				{
					user.RemotePowerShellEnabled = true;
				}
				MailUserTaskHelper.ValidateExternalEmailAddress(user, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			}
			if (base.Fields.IsChanged(ADRecipientSchema.MailboxProvisioningConstraint))
			{
				user.MailboxProvisioningConstraint = this.MailboxProvisioningConstraint;
			}
			if (base.Fields.IsChanged(ADRecipientSchema.MailboxProvisioningPreferences))
			{
				user.MailboxProvisioningPreferences = this.MailboxProvisioningPreferences;
			}
			if (user.MailboxProvisioningConstraint != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(new MailboxProvisioningConstraint[]
				{
					user.MailboxProvisioningConstraint
				}, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (user.MailboxProvisioningPreferences != null)
			{
				MailboxTaskHelper.ValidateMailboxProvisioningConstraintEntries(user.MailboxProvisioningPreferences, base.DomainController, delegate(string message)
				{
					base.WriteVerbose(new LocalizedString(message));
				}, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			base.PrepareUserObject(user);
			TaskLogger.LogExit();
		}

		protected override void StampChangesAfterSettingPassword()
		{
			base.StampChangesAfterSettingPassword();
			if (base.ParameterSetName == "DisabledUser")
			{
				this.DataObject.UserAccountControl = (UserAccountControlFlags.AccountDisabled | UserAccountControlFlags.NormalAccount);
			}
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(MailUser).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailUser.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalProcessRecord()
		{
			if (this.DataObject.IsInLitigationHoldOrInplaceHold)
			{
				RecoverableItemsQuotaHelper.IncreaseRecoverableItemsQuotaIfNeeded(this.DataObject);
			}
			base.InternalProcessRecord();
		}
	}
}
