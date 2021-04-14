using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ManagementEndpoint;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddManagementEndPointHookCommand : SyntheticCommandWithPipelineInputNoOutput<Guid>
	{
		private AddManagementEndPointHookCommand() : base("Add-ManagementEndPointHook")
		{
		}

		public AddManagementEndPointHookCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddManagementEndPointHookCommand SetParameters(AddManagementEndPointHookCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddManagementEndPointHookCommand SetParameters(AddManagementEndPointHookCommand.DomainParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddManagementEndPointHookCommand SetParameters(AddManagementEndPointHookCommand.OrganizationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual GlobalDirectoryServiceType GlobalDirectoryService
			{
				set
				{
					base.PowerSharpParameters["GlobalDirectoryService"] = value;
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

		public class DomainParameters : ParametersBase
		{
			public virtual SmtpDomain DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual bool InitialDomain
			{
				set
				{
					base.PowerSharpParameters["InitialDomain"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual GlobalDirectoryServiceType GlobalDirectoryService
			{
				set
				{
					base.PowerSharpParameters["GlobalDirectoryService"] = value;
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

		public class OrganizationParameters : ParametersBase
		{
			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual SmtpDomain PopulateCacheWithDomainName
			{
				set
				{
					base.PowerSharpParameters["PopulateCacheWithDomainName"] = value;
				}
			}

			public virtual string TenantContainerCN
			{
				set
				{
					base.PowerSharpParameters["TenantContainerCN"] = value;
				}
			}

			public virtual Guid ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual GlobalDirectoryServiceType GlobalDirectoryService
			{
				set
				{
					base.PowerSharpParameters["GlobalDirectoryService"] = value;
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
