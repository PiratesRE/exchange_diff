using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetHealthReportCommand : SyntheticCommandWithPipelineInput<ConsolidatedHealth, ConsolidatedHealth>
	{
		private GetHealthReportCommand() : base("Get-HealthReport")
		{
		}

		public GetHealthReportCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetHealthReportCommand SetParameters(GetHealthReportCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter RollupGroup
			{
				set
				{
					base.PowerSharpParameters["RollupGroup"] = value;
				}
			}

			public virtual int GroupSize
			{
				set
				{
					base.PowerSharpParameters["GroupSize"] = value;
				}
			}

			public virtual int MinimumOnlinePercent
			{
				set
				{
					base.PowerSharpParameters["MinimumOnlinePercent"] = value;
				}
			}

			public virtual ServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual string HealthSet
			{
				set
				{
					base.PowerSharpParameters["HealthSet"] = value;
				}
			}

			public virtual SwitchParameter HaImpactingOnly
			{
				set
				{
					base.PowerSharpParameters["HaImpactingOnly"] = value;
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
