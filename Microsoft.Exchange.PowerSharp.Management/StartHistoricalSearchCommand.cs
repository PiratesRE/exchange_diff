using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class StartHistoricalSearchCommand : SyntheticCommandWithPipelineInput<HistoricalSearch, HistoricalSearch>
	{
		private StartHistoricalSearchCommand() : base("Start-HistoricalSearch")
		{
		}

		public StartHistoricalSearchCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual StartHistoricalSearchCommand SetParameters(StartHistoricalSearchCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual HistoricalSearchReportType ReportType
			{
				set
				{
					base.PowerSharpParameters["ReportType"] = value;
				}
			}

			public virtual string ReportTitle
			{
				set
				{
					base.PowerSharpParameters["ReportTitle"] = value;
				}
			}

			public virtual MultiValuedProperty<string> NotifyAddress
			{
				set
				{
					base.PowerSharpParameters["NotifyAddress"] = value;
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

			public virtual string DeliveryStatus
			{
				set
				{
					base.PowerSharpParameters["DeliveryStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> SenderAddress
			{
				set
				{
					base.PowerSharpParameters["SenderAddress"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RecipientAddress
			{
				set
				{
					base.PowerSharpParameters["RecipientAddress"] = value;
				}
			}

			public virtual string OriginalClientIP
			{
				set
				{
					base.PowerSharpParameters["OriginalClientIP"] = value;
				}
			}

			public virtual MultiValuedProperty<string> MessageID
			{
				set
				{
					base.PowerSharpParameters["MessageID"] = value;
				}
			}

			public virtual MultiValuedProperty<Guid> DLPPolicy
			{
				set
				{
					base.PowerSharpParameters["DLPPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<Guid> TransportRule
			{
				set
				{
					base.PowerSharpParameters["TransportRule"] = value;
				}
			}

			public virtual CultureInfo Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual MessageDirection Direction
			{
				set
				{
					base.PowerSharpParameters["Direction"] = value;
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
