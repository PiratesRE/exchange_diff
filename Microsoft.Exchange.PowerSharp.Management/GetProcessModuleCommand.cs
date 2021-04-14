using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetProcessModuleCommand : SyntheticCommandWithPipelineInputNoOutput<int>
	{
		private GetProcessModuleCommand() : base("Get-ProcessModule")
		{
		}

		public GetProcessModuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetProcessModuleCommand SetParameters(GetProcessModuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual int ProcessId
			{
				set
				{
					base.PowerSharpParameters["ProcessId"] = value;
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
		}
	}
}
