using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxCalendarConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxCalendarConfiguration>
	{
		private SetMailboxCalendarConfigurationCommand() : base("Set-MailboxCalendarConfiguration")
		{
		}

		public SetMailboxCalendarConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxCalendarConfigurationCommand SetParameters(SetMailboxCalendarConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxCalendarConfigurationCommand SetParameters(SetMailboxCalendarConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DaysOfWeek WorkDays
			{
				set
				{
					base.PowerSharpParameters["WorkDays"] = value;
				}
			}

			public virtual TimeSpan WorkingHoursStartTime
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursStartTime"] = value;
				}
			}

			public virtual TimeSpan WorkingHoursEndTime
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursEndTime"] = value;
				}
			}

			public virtual ExTimeZoneValue WorkingHoursTimeZone
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursTimeZone"] = value;
				}
			}

			public virtual Microsoft.Exchange.Data.Storage.Management.DayOfWeek WeekStartDay
			{
				set
				{
					base.PowerSharpParameters["WeekStartDay"] = value;
				}
			}

			public virtual bool ShowWeekNumbers
			{
				set
				{
					base.PowerSharpParameters["ShowWeekNumbers"] = value;
				}
			}

			public virtual FirstWeekRules FirstWeekOfYear
			{
				set
				{
					base.PowerSharpParameters["FirstWeekOfYear"] = value;
				}
			}

			public virtual HourIncrement TimeIncrement
			{
				set
				{
					base.PowerSharpParameters["TimeIncrement"] = value;
				}
			}

			public virtual bool RemindersEnabled
			{
				set
				{
					base.PowerSharpParameters["RemindersEnabled"] = value;
				}
			}

			public virtual bool ReminderSoundEnabled
			{
				set
				{
					base.PowerSharpParameters["ReminderSoundEnabled"] = value;
				}
			}

			public virtual TimeSpan DefaultReminderTime
			{
				set
				{
					base.PowerSharpParameters["DefaultReminderTime"] = value;
				}
			}

			public virtual bool WeatherEnabled
			{
				set
				{
					base.PowerSharpParameters["WeatherEnabled"] = value;
				}
			}

			public virtual WeatherTemperatureUnit WeatherUnit
			{
				set
				{
					base.PowerSharpParameters["WeatherUnit"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WeatherLocations
			{
				set
				{
					base.PowerSharpParameters["WeatherLocations"] = value;
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

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual DaysOfWeek WorkDays
			{
				set
				{
					base.PowerSharpParameters["WorkDays"] = value;
				}
			}

			public virtual TimeSpan WorkingHoursStartTime
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursStartTime"] = value;
				}
			}

			public virtual TimeSpan WorkingHoursEndTime
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursEndTime"] = value;
				}
			}

			public virtual ExTimeZoneValue WorkingHoursTimeZone
			{
				set
				{
					base.PowerSharpParameters["WorkingHoursTimeZone"] = value;
				}
			}

			public virtual Microsoft.Exchange.Data.Storage.Management.DayOfWeek WeekStartDay
			{
				set
				{
					base.PowerSharpParameters["WeekStartDay"] = value;
				}
			}

			public virtual bool ShowWeekNumbers
			{
				set
				{
					base.PowerSharpParameters["ShowWeekNumbers"] = value;
				}
			}

			public virtual FirstWeekRules FirstWeekOfYear
			{
				set
				{
					base.PowerSharpParameters["FirstWeekOfYear"] = value;
				}
			}

			public virtual HourIncrement TimeIncrement
			{
				set
				{
					base.PowerSharpParameters["TimeIncrement"] = value;
				}
			}

			public virtual bool RemindersEnabled
			{
				set
				{
					base.PowerSharpParameters["RemindersEnabled"] = value;
				}
			}

			public virtual bool ReminderSoundEnabled
			{
				set
				{
					base.PowerSharpParameters["ReminderSoundEnabled"] = value;
				}
			}

			public virtual TimeSpan DefaultReminderTime
			{
				set
				{
					base.PowerSharpParameters["DefaultReminderTime"] = value;
				}
			}

			public virtual bool WeatherEnabled
			{
				set
				{
					base.PowerSharpParameters["WeatherEnabled"] = value;
				}
			}

			public virtual WeatherTemperatureUnit WeatherUnit
			{
				set
				{
					base.PowerSharpParameters["WeatherUnit"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WeatherLocations
			{
				set
				{
					base.PowerSharpParameters["WeatherLocations"] = value;
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

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
