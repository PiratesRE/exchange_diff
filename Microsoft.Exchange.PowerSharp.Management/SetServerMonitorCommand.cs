using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetServerMonitorCommand : SyntheticCommandWithPipelineInputNoOutput<ServerIdParameter>
	{
		private SetServerMonitorCommand() : base("Set-ServerMonitor")
		{
		}

		public SetServerMonitorCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetServerMonitorCommand SetParameters(SetServerMonitorCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string TargetResource
			{
				set
				{
					base.PowerSharpParameters["TargetResource"] = value;
				}
			}

			public virtual bool Repairing
			{
				set
				{
					base.PowerSharpParameters["Repairing"] = value;
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
