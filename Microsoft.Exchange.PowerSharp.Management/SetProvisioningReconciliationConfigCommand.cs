using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetProvisioningReconciliationConfigCommand : SyntheticCommandWithPipelineInputNoOutput<ProvisioningReconciliationConfig>
	{
		private SetProvisioningReconciliationConfigCommand() : base("Set-ProvisioningReconciliationConfig")
		{
		}

		public SetProvisioningReconciliationConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetProvisioningReconciliationConfigCommand SetParameters(SetProvisioningReconciliationConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<ReconciliationCookie> ReconciliationCookies
			{
				set
				{
					base.PowerSharpParameters["ReconciliationCookies"] = value;
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
