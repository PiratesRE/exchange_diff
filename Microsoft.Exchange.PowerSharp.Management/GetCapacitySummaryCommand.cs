using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.MailboxLoadBalanceClient;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetCapacitySummaryCommand : SyntheticCommandWithPipelineInput<CapacitySummary, CapacitySummary>
	{
		private GetCapacitySummaryCommand() : base("Get-CapacitySummary")
		{
		}

		public GetCapacitySummaryCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetCapacitySummaryCommand SetParameters(GetCapacitySummaryCommand.DatabaseSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCapacitySummaryCommand SetParameters(GetCapacitySummaryCommand.DagSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCapacitySummaryCommand SetParameters(GetCapacitySummaryCommand.ForestSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCapacitySummaryCommand SetParameters(GetCapacitySummaryCommand.ServerSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DatabaseSetParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual SwitchParameter Refresh
			{
				set
				{
					base.PowerSharpParameters["Refresh"] = value;
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

		public class DagSetParameters : ParametersBase
		{
			public virtual DatabaseAvailabilityGroupIdParameter DatabaseAvailabilityGroup
			{
				set
				{
					base.PowerSharpParameters["DatabaseAvailabilityGroup"] = value;
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

		public class ForestSetParameters : ParametersBase
		{
			public virtual SwitchParameter Forest
			{
				set
				{
					base.PowerSharpParameters["Forest"] = value;
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

		public class ServerSetParameters : ParametersBase
		{
			public virtual SwitchParameter Refresh
			{
				set
				{
					base.PowerSharpParameters["Refresh"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
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
