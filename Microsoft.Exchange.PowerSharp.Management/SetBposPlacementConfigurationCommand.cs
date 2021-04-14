using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetBposPlacementConfigurationCommand : SyntheticCommand<object>
	{
		private SetBposPlacementConfigurationCommand() : base("Set-BposPlacementConfiguration")
		{
		}

		public SetBposPlacementConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetBposPlacementConfigurationCommand SetParameters(SetBposPlacementConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Configuration
			{
				set
				{
					base.PowerSharpParameters["Configuration"] = value;
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
