using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Monitoring.Reporting;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetPhysicalAvailabilityReportCommand : SyntheticCommandWithPipelineInput<OrganizationIdParameter, OrganizationIdParameter>
	{
		private GetPhysicalAvailabilityReportCommand() : base("Get-PhysicalAvailabilityReport")
		{
		}

		public GetPhysicalAvailabilityReportCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetPhysicalAvailabilityReportCommand SetParameters(GetPhysicalAvailabilityReportCommand.ReportingPeriodSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetPhysicalAvailabilityReportCommand SetParameters(GetPhysicalAvailabilityReportCommand.StartEndDateSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetPhysicalAvailabilityReportCommand SetParameters(GetPhysicalAvailabilityReportCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ReportingPeriodSetParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual ServerIdParameter ExchangeServer
			{
				set
				{
					base.PowerSharpParameters["ExchangeServer"] = value;
				}
			}

			public virtual ReportingPeriod ReportingPeriod
			{
				set
				{
					base.PowerSharpParameters["ReportingPeriod"] = value;
				}
			}

			public virtual SwitchParameter DailyStatistics
			{
				set
				{
					base.PowerSharpParameters["DailyStatistics"] = value;
				}
			}

			public virtual Fqdn ReportingServer
			{
				set
				{
					base.PowerSharpParameters["ReportingServer"] = value;
				}
			}

			public virtual string ReportingDatabase
			{
				set
				{
					base.PowerSharpParameters["ReportingDatabase"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class StartEndDateSetParameters : ParametersBase
		{
			public virtual DatabaseIdParameter Database
			{
				set
				{
					base.PowerSharpParameters["Database"] = value;
				}
			}

			public virtual ServerIdParameter ExchangeServer
			{
				set
				{
					base.PowerSharpParameters["ExchangeServer"] = value;
				}
			}

			public virtual DateTime StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual DateTime EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter DailyStatistics
			{
				set
				{
					base.PowerSharpParameters["DailyStatistics"] = value;
				}
			}

			public virtual Fqdn ReportingServer
			{
				set
				{
					base.PowerSharpParameters["ReportingServer"] = value;
				}
			}

			public virtual string ReportingDatabase
			{
				set
				{
					base.PowerSharpParameters["ReportingDatabase"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn ReportingServer
			{
				set
				{
					base.PowerSharpParameters["ReportingServer"] = value;
				}
			}

			public virtual string ReportingDatabase
			{
				set
				{
					base.PowerSharpParameters["ReportingDatabase"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
