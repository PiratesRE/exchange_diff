using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RemovePerfCountersCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private RemovePerfCountersCommand() : base("Remove-PerfCounters")
		{
		}

		public RemovePerfCountersCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RemovePerfCountersCommand SetParameters(RemovePerfCountersCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string DefinitionFileName
			{
				set
				{
					base.PowerSharpParameters["DefinitionFileName"] = value;
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
