using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OrganizationFlags", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOrganizationFlags : SetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IsFederated
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsFederated] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsFederated] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter HideAdminAccessWarning
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.HideAdminAccessWarning] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.HideAdminAccessWarning] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipToUAndParentalControlCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.SkipToUAndParentalControlCheck] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.SkipToUAndParentalControlCheck] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsUpgradingOrganization
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsUpgradingOrganization] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsUpgradingOrganization] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsPilotingOrganization
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsPilotingOrganization] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsPilotingOrganization] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsTemplateTenant
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsTemplateTenant] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsTemplateTenant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsUpgradeOperationInProgress
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsUpgradeOperationInProgress] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsUpgradeOperationInProgress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SMTPAddressCheckWithAcceptedDomain
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.SMTPAddressCheckWithAcceptedDomain] ?? true);
			}
			set
			{
				base.Fields[OrganizationSchema.SMTPAddressCheckWithAcceptedDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsLicensingEnforced
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsLicensingEnforced] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsLicensingEnforced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsTenantAccessBlocked
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.IsTenantAccessBlocked] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsTenantAccessBlocked] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowDeleteOfExternalIdentityUponRemove
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseServicePlanAsCounterInstanceName
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.UseServicePlanAsCounterInstanceName] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.UseServicePlanAsCounterInstanceName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SoftDeletedFeatureStatusFlags SoftDeletedFeatureStatus
		{
			get
			{
				return (SoftDeletedFeatureStatusFlags)base.Fields[OrganizationSchema.SoftDeletedFeatureStatus];
			}
			set
			{
				base.Fields[OrganizationSchema.SoftDeletedFeatureStatus] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.tenantCU = (ExchangeConfigurationUnit)base.PrepareDataObject();
			if (base.Fields.IsModified(OrganizationSchema.IsFederated) && !this.IsFederated && this.tenantCU.IsFederated)
			{
				base.WriteError(new CannotDisableFederationException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (base.Fields.IsModified(OrganizationSchema.IsFederated))
			{
				this.tenantCU.IsFederated = this.IsFederated;
			}
			if (base.Fields.IsModified(OrganizationSchema.SkipToUAndParentalControlCheck))
			{
				this.tenantCU.SkipToUAndParentalControlCheck = this.SkipToUAndParentalControlCheck;
			}
			if (base.Fields.IsModified(OrganizationSchema.HideAdminAccessWarning))
			{
				this.tenantCU.HideAdminAccessWarning = this.HideAdminAccessWarning;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsUpgradingOrganization))
			{
				this.tenantCU.IsUpgradingOrganization = this.IsUpgradingOrganization;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsPilotingOrganization))
			{
				this.tenantCU.IsPilotingOrganization = this.IsPilotingOrganization;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsUpgradeOperationInProgress))
			{
				this.tenantCU.IsUpgradeOperationInProgress = this.IsUpgradeOperationInProgress;
			}
			if (base.Fields.IsModified(OrganizationSchema.SMTPAddressCheckWithAcceptedDomain))
			{
				this.tenantCU.SMTPAddressCheckWithAcceptedDomain = this.SMTPAddressCheckWithAcceptedDomain;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsLicensingEnforced))
			{
				this.tenantCU.IsLicensingEnforced = this.IsLicensingEnforced;
			}
			if (base.Fields.IsModified(OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove))
			{
				this.tenantCU.AllowDeleteOfExternalIdentityUponRemove = this.AllowDeleteOfExternalIdentityUponRemove;
			}
			if (base.Fields.IsModified(OrganizationSchema.UseServicePlanAsCounterInstanceName))
			{
				this.tenantCU.UseServicePlanAsCounterInstanceName = this.UseServicePlanAsCounterInstanceName;
			}
			if (base.Fields.IsModified(OrganizationSchema.SoftDeletedFeatureStatus))
			{
				this.tenantCU.SoftDeletedFeatureStatus = this.SoftDeletedFeatureStatus;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsTenantAccessBlocked))
			{
				this.tenantCU.IsTenantAccessBlocked = this.IsTenantAccessBlocked;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsTemplateTenant))
			{
				this.tenantCU.IsTemplateTenant = this.IsTemplateTenant;
			}
			return this.tenantCU;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOrganizationFlags(this.tenantCU.Name);
			}
		}

		private ExchangeConfigurationUnit tenantCU;
	}
}
