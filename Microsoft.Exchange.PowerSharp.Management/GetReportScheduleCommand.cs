using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.FfoReporting;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetReportScheduleCommand : SyntheticCommandWithPipelineInput<ReportSchedule, ReportSchedule>
	{
		private GetReportScheduleCommand() : base("Get-ReportSchedule")
		{
		}

		public GetReportScheduleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetReportScheduleCommand SetParameters(GetReportScheduleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Guid? ScheduleID
			{
				set
				{
					base.PowerSharpParameters["ScheduleID"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
