using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class StartSetupProcessCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private StartSetupProcessCommand() : base("Start-SetupProcess")
		{
		}

		public StartSetupProcessCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual StartSetupProcessCommand SetParameters(StartSetupProcessCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string Args
			{
				set
				{
					base.PowerSharpParameters["Args"] = value;
				}
			}

			public virtual int Timeout
			{
				set
				{
					base.PowerSharpParameters["Timeout"] = value;
				}
			}

			public virtual int IgnoreExitCode
			{
				set
				{
					base.PowerSharpParameters["IgnoreExitCode"] = value;
				}
			}

			public virtual uint RetryCount
			{
				set
				{
					base.PowerSharpParameters["RetryCount"] = value;
				}
			}

			public virtual uint RetryDelay
			{
				set
				{
					base.PowerSharpParameters["RetryDelay"] = value;
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
