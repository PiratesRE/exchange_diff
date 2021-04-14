using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHostedConnectionFilterPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<HostedConnectionFilterPolicy>
	{
		private SetHostedConnectionFilterPolicyCommand() : base("Set-HostedConnectionFilterPolicy")
		{
		}

		public SetHostedConnectionFilterPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHostedConnectionFilterPolicyCommand SetParameters(SetHostedConnectionFilterPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetHostedConnectionFilterPolicyCommand SetParameters(SetHostedConnectionFilterPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new HostedConnectionFilterPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter MakeDefault
			{
				set
				{
					base.PowerSharpParameters["MakeDefault"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDehydratedFlag
			{
				set
				{
					base.PowerSharpParameters["IgnoreDehydratedFlag"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
