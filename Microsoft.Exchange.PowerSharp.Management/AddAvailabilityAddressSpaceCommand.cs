using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddAvailabilityAddressSpaceCommand : SyntheticCommandWithPipelineInput<AvailabilityAddressSpace, AvailabilityAddressSpace>
	{
		private AddAvailabilityAddressSpaceCommand() : base("Add-AvailabilityAddressSpace")
		{
		}

		public AddAvailabilityAddressSpaceCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddAvailabilityAddressSpaceCommand SetParameters(AddAvailabilityAddressSpaceCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ForestName
			{
				set
				{
					base.PowerSharpParameters["ForestName"] = value;
				}
			}

			public virtual AvailabilityAccessMethod AccessMethod
			{
				set
				{
					base.PowerSharpParameters["AccessMethod"] = value;
				}
			}

			public virtual bool UseServiceAccount
			{
				set
				{
					base.PowerSharpParameters["UseServiceAccount"] = value;
				}
			}

			public virtual PSCredential Credentials
			{
				set
				{
					base.PowerSharpParameters["Credentials"] = value;
				}
			}

			public virtual Uri ProxyUrl
			{
				set
				{
					base.PowerSharpParameters["ProxyUrl"] = value;
				}
			}

			public virtual Uri TargetAutodiscoverEpr
			{
				set
				{
					base.PowerSharpParameters["TargetAutodiscoverEpr"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
