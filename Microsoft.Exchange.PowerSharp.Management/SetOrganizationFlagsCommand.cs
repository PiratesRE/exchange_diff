using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOrganizationFlagsCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeConfigurationUnit>
	{
		private SetOrganizationFlagsCommand() : base("Set-OrganizationFlags")
		{
		}

		public SetOrganizationFlagsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOrganizationFlagsCommand SetParameters(SetOrganizationFlagsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationFlagsCommand SetParameters(SetOrganizationFlagsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IsFederated
			{
				set
				{
					base.PowerSharpParameters["IsFederated"] = value;
				}
			}

			public virtual SwitchParameter HideAdminAccessWarning
			{
				set
				{
					base.PowerSharpParameters["HideAdminAccessWarning"] = value;
				}
			}

			public virtual SwitchParameter SkipToUAndParentalControlCheck
			{
				set
				{
					base.PowerSharpParameters["SkipToUAndParentalControlCheck"] = value;
				}
			}

			public virtual SwitchParameter IsUpgradingOrganization
			{
				set
				{
					base.PowerSharpParameters["IsUpgradingOrganization"] = value;
				}
			}

			public virtual SwitchParameter IsPilotingOrganization
			{
				set
				{
					base.PowerSharpParameters["IsPilotingOrganization"] = value;
				}
			}

			public virtual SwitchParameter IsTemplateTenant
			{
				set
				{
					base.PowerSharpParameters["IsTemplateTenant"] = value;
				}
			}

			public virtual SwitchParameter IsUpgradeOperationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsUpgradeOperationInProgress"] = value;
				}
			}

			public virtual SwitchParameter SMTPAddressCheckWithAcceptedDomain
			{
				set
				{
					base.PowerSharpParameters["SMTPAddressCheckWithAcceptedDomain"] = value;
				}
			}

			public virtual SwitchParameter IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual SwitchParameter IsTenantAccessBlocked
			{
				set
				{
					base.PowerSharpParameters["IsTenantAccessBlocked"] = value;
				}
			}

			public virtual SwitchParameter AllowDeleteOfExternalIdentityUponRemove
			{
				set
				{
					base.PowerSharpParameters["AllowDeleteOfExternalIdentityUponRemove"] = value;
				}
			}

			public virtual SwitchParameter UseServicePlanAsCounterInstanceName
			{
				set
				{
					base.PowerSharpParameters["UseServicePlanAsCounterInstanceName"] = value;
				}
			}

			public virtual SoftDeletedFeatureStatusFlags SoftDeletedFeatureStatus
			{
				set
				{
					base.PowerSharpParameters["SoftDeletedFeatureStatus"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IsFederated
			{
				set
				{
					base.PowerSharpParameters["IsFederated"] = value;
				}
			}

			public virtual SwitchParameter HideAdminAccessWarning
			{
				set
				{
					base.PowerSharpParameters["HideAdminAccessWarning"] = value;
				}
			}

			public virtual SwitchParameter SkipToUAndParentalControlCheck
			{
				set
				{
					base.PowerSharpParameters["SkipToUAndParentalControlCheck"] = value;
				}
			}

			public virtual SwitchParameter IsUpgradingOrganization
			{
				set
				{
					base.PowerSharpParameters["IsUpgradingOrganization"] = value;
				}
			}

			public virtual SwitchParameter IsPilotingOrganization
			{
				set
				{
					base.PowerSharpParameters["IsPilotingOrganization"] = value;
				}
			}

			public virtual SwitchParameter IsTemplateTenant
			{
				set
				{
					base.PowerSharpParameters["IsTemplateTenant"] = value;
				}
			}

			public virtual SwitchParameter IsUpgradeOperationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsUpgradeOperationInProgress"] = value;
				}
			}

			public virtual SwitchParameter SMTPAddressCheckWithAcceptedDomain
			{
				set
				{
					base.PowerSharpParameters["SMTPAddressCheckWithAcceptedDomain"] = value;
				}
			}

			public virtual SwitchParameter IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual SwitchParameter IsTenantAccessBlocked
			{
				set
				{
					base.PowerSharpParameters["IsTenantAccessBlocked"] = value;
				}
			}

			public virtual SwitchParameter AllowDeleteOfExternalIdentityUponRemove
			{
				set
				{
					base.PowerSharpParameters["AllowDeleteOfExternalIdentityUponRemove"] = value;
				}
			}

			public virtual SwitchParameter UseServicePlanAsCounterInstanceName
			{
				set
				{
					base.PowerSharpParameters["UseServicePlanAsCounterInstanceName"] = value;
				}
			}

			public virtual SoftDeletedFeatureStatusFlags SoftDeletedFeatureStatus
			{
				set
				{
					base.PowerSharpParameters["SoftDeletedFeatureStatus"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
