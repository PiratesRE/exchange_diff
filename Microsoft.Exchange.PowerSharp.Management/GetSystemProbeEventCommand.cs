using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetSystemProbeEventCommand : SyntheticCommandWithPipelineInputNoOutput<Guid>
	{
		private GetSystemProbeEventCommand() : base("Get-SystemProbeEvent")
		{
		}

		public GetSystemProbeEventCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetSystemProbeEventCommand SetParameters(GetSystemProbeEventCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Guid Guid
			{
				set
				{
					base.PowerSharpParameters["Guid"] = value;
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
