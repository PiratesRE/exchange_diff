using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewHostedConnectionFilterPolicyCommand : SyntheticCommandWithPipelineInput<HostedConnectionFilterPolicy, HostedConnectionFilterPolicy>
	{
		private NewHostedConnectionFilterPolicyCommand() : base("New-HostedConnectionFilterPolicy")
		{
		}

		public NewHostedConnectionFilterPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewHostedConnectionFilterPolicyCommand SetParameters(NewHostedConnectionFilterPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string AdminDisplayName
			{
				set
				{
					base.PowerSharpParameters["AdminDisplayName"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> IPAllowList
			{
				set
				{
					base.PowerSharpParameters["IPAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> IPBlockList
			{
				set
				{
					base.PowerSharpParameters["IPBlockList"] = value;
				}
			}

			public virtual bool EnableSafeList
			{
				set
				{
					base.PowerSharpParameters["EnableSafeList"] = value;
				}
			}

			public virtual DirectoryBasedEdgeBlockMode DirectoryBasedEdgeBlockMode
			{
				set
				{
					base.PowerSharpParameters["DirectoryBasedEdgeBlockMode"] = value;
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
