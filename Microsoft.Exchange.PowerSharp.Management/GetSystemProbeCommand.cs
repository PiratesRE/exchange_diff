using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetSystemProbeCommand : SyntheticCommandWithPipelineInput<Guid, Guid>
	{
		private GetSystemProbeCommand() : base("Get-SystemProbe")
		{
		}

		public GetSystemProbeCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetSystemProbeCommand SetParameters(GetSystemProbeCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual DateTimeOffset? StartTime
			{
				set
				{
					base.PowerSharpParameters["StartTime"] = value;
				}
			}

			public virtual DateTimeOffset? EndTime
			{
				set
				{
					base.PowerSharpParameters["EndTime"] = value;
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
