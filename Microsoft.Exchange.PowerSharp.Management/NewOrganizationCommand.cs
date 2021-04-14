using System;
using System.Collections;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewOrganizationCommand : SyntheticCommandWithPipelineInputNoOutput<SwitchParameter>
	{
		private NewOrganizationCommand() : base("New-Organization")
		{
		}

		public NewOrganizationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewOrganizationCommand SetParameters(NewOrganizationCommand.SharedConfigurationParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewOrganizationCommand SetParameters(NewOrganizationCommand.DatacenterParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewOrganizationCommand SetParameters(NewOrganizationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class SharedConfigurationParameterSetParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SmtpDomain DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual SwitchParameter CreateSharedConfiguration
			{
				set
				{
					base.PowerSharpParameters["CreateSharedConfiguration"] = value;
				}
			}

			public virtual WindowsLiveId Administrator
			{
				set
				{
					base.PowerSharpParameters["Administrator"] = value;
				}
			}

			public virtual NetID AdministratorNetID
			{
				set
				{
					base.PowerSharpParameters["AdministratorNetID"] = value;
				}
			}

			public virtual SecureString AdministratorPassword
			{
				set
				{
					base.PowerSharpParameters["AdministratorPassword"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual string DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual bool OutBoundOnly
			{
				set
				{
					base.PowerSharpParameters["OutBoundOnly"] = value;
				}
			}

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual byte RMSOnlineConfig
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineConfig"] = value;
				}
			}

			public virtual Hashtable RMSOnlineKeys
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeys"] = value;
				}
			}

			public virtual SwitchParameter EnableFileLogging
			{
				set
				{
					base.PowerSharpParameters["EnableFileLogging"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenter
			{
				set
				{
					base.PowerSharpParameters["IsDatacenter"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenterDedicated
			{
				set
				{
					base.PowerSharpParameters["IsDatacenterDedicated"] = value;
				}
			}

			public virtual SwitchParameter IsPartnerHosted
			{
				set
				{
					base.PowerSharpParameters["IsPartnerHosted"] = value;
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

		public class DatacenterParameterSetParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SmtpDomain DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual SwitchParameter HotmailMigration
			{
				set
				{
					base.PowerSharpParameters["HotmailMigration"] = value;
				}
			}

			public virtual WindowsLiveId Administrator
			{
				set
				{
					base.PowerSharpParameters["Administrator"] = value;
				}
			}

			public virtual NetID AdministratorNetID
			{
				set
				{
					base.PowerSharpParameters["AdministratorNetID"] = value;
				}
			}

			public virtual SecureString AdministratorPassword
			{
				set
				{
					base.PowerSharpParameters["AdministratorPassword"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual string DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual bool OutBoundOnly
			{
				set
				{
					base.PowerSharpParameters["OutBoundOnly"] = value;
				}
			}

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual byte RMSOnlineConfig
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineConfig"] = value;
				}
			}

			public virtual Hashtable RMSOnlineKeys
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeys"] = value;
				}
			}

			public virtual SwitchParameter EnableFileLogging
			{
				set
				{
					base.PowerSharpParameters["EnableFileLogging"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenter
			{
				set
				{
					base.PowerSharpParameters["IsDatacenter"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenterDedicated
			{
				set
				{
					base.PowerSharpParameters["IsDatacenterDedicated"] = value;
				}
			}

			public virtual SwitchParameter IsPartnerHosted
			{
				set
				{
					base.PowerSharpParameters["IsPartnerHosted"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual WindowsLiveId Administrator
			{
				set
				{
					base.PowerSharpParameters["Administrator"] = value;
				}
			}

			public virtual NetID AdministratorNetID
			{
				set
				{
					base.PowerSharpParameters["AdministratorNetID"] = value;
				}
			}

			public virtual SecureString AdministratorPassword
			{
				set
				{
					base.PowerSharpParameters["AdministratorPassword"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual string DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual bool OutBoundOnly
			{
				set
				{
					base.PowerSharpParameters["OutBoundOnly"] = value;
				}
			}

			public virtual AuthenticationType AuthenticationType
			{
				set
				{
					base.PowerSharpParameters["AuthenticationType"] = value;
				}
			}

			public virtual LiveIdInstanceType LiveIdInstanceType
			{
				set
				{
					base.PowerSharpParameters["LiveIdInstanceType"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual byte RMSOnlineConfig
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineConfig"] = value;
				}
			}

			public virtual Hashtable RMSOnlineKeys
			{
				set
				{
					base.PowerSharpParameters["RMSOnlineKeys"] = value;
				}
			}

			public virtual SwitchParameter EnableFileLogging
			{
				set
				{
					base.PowerSharpParameters["EnableFileLogging"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenter
			{
				set
				{
					base.PowerSharpParameters["IsDatacenter"] = value;
				}
			}

			public virtual SwitchParameter IsDatacenterDedicated
			{
				set
				{
					base.PowerSharpParameters["IsDatacenterDedicated"] = value;
				}
			}

			public virtual SwitchParameter IsPartnerHosted
			{
				set
				{
					base.PowerSharpParameters["IsPartnerHosted"] = value;
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
