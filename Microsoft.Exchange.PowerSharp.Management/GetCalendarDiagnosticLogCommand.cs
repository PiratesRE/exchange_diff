using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetCalendarDiagnosticLogCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private GetCalendarDiagnosticLogCommand() : base("Get-CalendarDiagnosticLog")
		{
		}

		public GetCalendarDiagnosticLogCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetCalendarDiagnosticLogCommand SetParameters(GetCalendarDiagnosticLogCommand.ExportToMsgParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCalendarDiagnosticLogCommand SetParameters(GetCalendarDiagnosticLogCommand.DoNotExportParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCalendarDiagnosticLogCommand SetParameters(GetCalendarDiagnosticLogCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class ExportToMsgParameterSetParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string LogLocation
			{
				set
				{
					base.PowerSharpParameters["LogLocation"] = value;
				}
			}

			public virtual string Subject
			{
				set
				{
					base.PowerSharpParameters["Subject"] = value;
				}
			}

			public virtual string MeetingID
			{
				set
				{
					base.PowerSharpParameters["MeetingID"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter Latest
			{
				set
				{
					base.PowerSharpParameters["Latest"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
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

		public class DoNotExportParameterSetParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string Subject
			{
				set
				{
					base.PowerSharpParameters["Subject"] = value;
				}
			}

			public virtual string MeetingID
			{
				set
				{
					base.PowerSharpParameters["MeetingID"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter Latest
			{
				set
				{
					base.PowerSharpParameters["Latest"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
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
			public virtual string Subject
			{
				set
				{
					base.PowerSharpParameters["Subject"] = value;
				}
			}

			public virtual string MeetingID
			{
				set
				{
					base.PowerSharpParameters["MeetingID"] = value;
				}
			}

			public virtual ExDateTime? StartDate
			{
				set
				{
					base.PowerSharpParameters["StartDate"] = value;
				}
			}

			public virtual ExDateTime? EndDate
			{
				set
				{
					base.PowerSharpParameters["EndDate"] = value;
				}
			}

			public virtual SwitchParameter Latest
			{
				set
				{
					base.PowerSharpParameters["Latest"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
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
