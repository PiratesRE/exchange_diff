using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetCalendarDiagnosticAnalysisCommand : SyntheticCommandWithPipelineInputNoOutput<CalendarLog>
	{
		private GetCalendarDiagnosticAnalysisCommand() : base("Get-CalendarDiagnosticAnalysis")
		{
		}

		public GetCalendarDiagnosticAnalysisCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetCalendarDiagnosticAnalysisCommand SetParameters(GetCalendarDiagnosticAnalysisCommand.DefaultSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCalendarDiagnosticAnalysisCommand SetParameters(GetCalendarDiagnosticAnalysisCommand.LocationSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCalendarDiagnosticAnalysisCommand SetParameters(GetCalendarDiagnosticAnalysisCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultSetParameters : ParametersBase
		{
			public virtual CalendarLog CalendarLogs
			{
				set
				{
					base.PowerSharpParameters["CalendarLogs"] = value;
				}
			}

			public virtual string GlobalObjectId
			{
				set
				{
					base.PowerSharpParameters["GlobalObjectId"] = value;
				}
			}

			public virtual AnalysisDetailLevel DetailLevel
			{
				set
				{
					base.PowerSharpParameters["DetailLevel"] = value;
				}
			}

			public virtual OutputType OutputAs
			{
				set
				{
					base.PowerSharpParameters["OutputAs"] = value;
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

		public class LocationSetParameters : ParametersBase
		{
			public virtual string LogLocation
			{
				set
				{
					base.PowerSharpParameters["LogLocation"] = value;
				}
			}

			public virtual string GlobalObjectId
			{
				set
				{
					base.PowerSharpParameters["GlobalObjectId"] = value;
				}
			}

			public virtual AnalysisDetailLevel DetailLevel
			{
				set
				{
					base.PowerSharpParameters["DetailLevel"] = value;
				}
			}

			public virtual OutputType OutputAs
			{
				set
				{
					base.PowerSharpParameters["OutputAs"] = value;
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
			public virtual string GlobalObjectId
			{
				set
				{
					base.PowerSharpParameters["GlobalObjectId"] = value;
				}
			}

			public virtual AnalysisDetailLevel DetailLevel
			{
				set
				{
					base.PowerSharpParameters["DetailLevel"] = value;
				}
			}

			public virtual OutputType OutputAs
			{
				set
				{
					base.PowerSharpParameters["OutputAs"] = value;
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
