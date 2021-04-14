using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxCalendarConfiguration : UserConfigurationObject, IWorkingHours
	{
		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxCalendarConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public DaysOfWeek WorkDays
		{
			get
			{
				return (DaysOfWeek)this[MailboxCalendarConfigurationSchema.WorkDays];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WorkDays] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan WorkingHoursStartTime
		{
			get
			{
				return (TimeSpan)this[MailboxCalendarConfigurationSchema.WorkingHoursStartTime];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WorkingHoursStartTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan WorkingHoursEndTime
		{
			get
			{
				return (TimeSpan)this[MailboxCalendarConfigurationSchema.WorkingHoursEndTime];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WorkingHoursEndTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExTimeZoneValue WorkingHoursTimeZone
		{
			get
			{
				return (ExTimeZoneValue)this[MailboxCalendarConfigurationSchema.WorkingHoursTimeZone];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WorkingHoursTimeZone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DayOfWeek WeekStartDay
		{
			get
			{
				return (DayOfWeek)this[MailboxCalendarConfigurationSchema.WeekStartDay];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WeekStartDay] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShowWeekNumbers
		{
			get
			{
				return (bool)this[MailboxCalendarConfigurationSchema.ShowWeekNumbers];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.ShowWeekNumbers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FirstWeekRules FirstWeekOfYear
		{
			get
			{
				return (FirstWeekRules)this[MailboxCalendarConfigurationSchema.FirstWeekOfYear];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.FirstWeekOfYear] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HourIncrement TimeIncrement
		{
			get
			{
				return (HourIncrement)this[MailboxCalendarConfigurationSchema.TimeIncrement];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.TimeIncrement] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemindersEnabled
		{
			get
			{
				return (bool)this[MailboxCalendarConfigurationSchema.RemindersEnabled];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.RemindersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReminderSoundEnabled
		{
			get
			{
				return (bool)this[MailboxCalendarConfigurationSchema.ReminderSoundEnabled];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.ReminderSoundEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TimeSpan DefaultReminderTime
		{
			get
			{
				return (TimeSpan)this[MailboxCalendarConfigurationSchema.DefaultReminderTime];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.DefaultReminderTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WeatherEnabled
		{
			get
			{
				return (bool)this[MailboxCalendarConfigurationSchema.WeatherEnabled];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WeatherEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public WeatherTemperatureUnit WeatherUnit
		{
			get
			{
				return (WeatherTemperatureUnit)this[MailboxCalendarConfigurationSchema.WeatherUnit];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WeatherUnit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> WeatherLocations
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxCalendarConfigurationSchema.WeatherLocations];
			}
			set
			{
				this[MailboxCalendarConfigurationSchema.WeatherLocations] = value;
			}
		}

		internal static void WeekStartDaySetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[MailboxCalendarConfigurationSchema.RawWeekStartDay] = value;
		}

		internal static object WeekStartDayGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[MailboxCalendarConfigurationSchema.RawWeekStartDay];
			if (obj != null && obj is DayOfWeek)
			{
				DayOfWeek dayOfWeek = (DayOfWeek)obj;
				if (dayOfWeek >= DayOfWeek.Sunday && dayOfWeek <= DayOfWeek.Saturday)
				{
					return dayOfWeek;
				}
			}
			CultureInfo cultureInfo = (CultureInfo)propertyBag[MailboxCalendarConfigurationSchema.Language];
			if (cultureInfo != null)
			{
				return (DayOfWeek)cultureInfo.DateTimeFormat.FirstDayOfWeek;
			}
			return DayOfWeek.Sunday;
		}

		internal static void FirstWeekOfYearSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[MailboxCalendarConfigurationSchema.RawFirstWeekOfYear] = value;
		}

		internal static object FirstWeekOfYearGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[MailboxCalendarConfigurationSchema.RawFirstWeekOfYear];
			if (obj != null && obj is FirstWeekRules)
			{
				FirstWeekRules firstWeekRules = (FirstWeekRules)obj;
				if (firstWeekRules >= FirstWeekRules.LegacyNotSet && firstWeekRules <= FirstWeekRules.FirstFullWeek)
				{
					return firstWeekRules;
				}
			}
			CultureInfo cultureInfo = (CultureInfo)propertyBag[MailboxCalendarConfigurationSchema.Language];
			if (cultureInfo != null)
			{
				return cultureInfo.DateTimeFormat.CalendarWeekRule.ToFirstWeekRules();
			}
			return FirstWeekRules.FirstDay;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.WorkingHoursStartTime > this.WorkingHoursEndTime)
			{
				errors.Add(new ObjectValidationError(ServerStrings.ErrorWorkingHoursEndTimeSmaller, this.Identity, string.Empty));
			}
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			base.Principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			using (WorkingHoursAdapter<MailboxCalendarConfiguration> workingHoursAdapter = new WorkingHoursAdapter<MailboxCalendarConfiguration>(session.MailboxSession))
			{
				using (UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxCalendarConfiguration.mailboxProperties))
				{
					using (UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration> userConfigurationDictionaryAdapter2 = new UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration>(session.MailboxSession, "Calendar", new GetUserConfigurationDelegate(UserConfigurationHelper.GetCalendarConfiguration), MailboxCalendarConfiguration.calendarProperties))
					{
						workingHoursAdapter.Fill(this);
						userConfigurationDictionaryAdapter.Fill(this);
						userConfigurationDictionaryAdapter2.Fill(this);
					}
				}
			}
			if (base.Principal.PreferredCultures.Any<CultureInfo>())
			{
				this[MailboxCalendarConfigurationSchema.Language] = base.Principal.PreferredCultures.First<CultureInfo>();
			}
			return this;
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			using (WorkingHoursAdapter<MailboxCalendarConfiguration> workingHoursAdapter = new WorkingHoursAdapter<MailboxCalendarConfiguration>(session.MailboxSession))
			{
				using (UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxCalendarConfiguration.mailboxProperties))
				{
					using (UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration> userConfigurationDictionaryAdapter2 = new UserConfigurationDictionaryAdapter<MailboxCalendarConfiguration>(session.MailboxSession, "Calendar", new GetUserConfigurationDelegate(UserConfigurationHelper.GetCalendarConfiguration), MailboxCalendarConfiguration.calendarProperties))
					{
						workingHoursAdapter.Save(this);
						userConfigurationDictionaryAdapter.Save(this);
						userConfigurationDictionaryAdapter2.Save(this);
						base.ResetChangeTracking();
					}
				}
			}
		}

		private static MailboxCalendarConfigurationSchema schema = ObjectSchema.GetInstance<MailboxCalendarConfigurationSchema>();

		private static SimplePropertyDefinition[] mailboxProperties = new SimplePropertyDefinition[]
		{
			MailboxCalendarConfigurationSchema.WeekStartDay,
			MailboxCalendarConfigurationSchema.ShowWeekNumbers,
			MailboxCalendarConfigurationSchema.FirstWeekOfYear,
			MailboxCalendarConfigurationSchema.TimeIncrement,
			MailboxCalendarConfigurationSchema.RemindersEnabled,
			MailboxCalendarConfigurationSchema.ReminderSoundEnabled,
			MailboxCalendarConfigurationSchema.WeatherEnabled,
			MailboxCalendarConfigurationSchema.WeatherUnit,
			MailboxCalendarConfigurationSchema.WeatherLocations
		};

		private static SimplePropertyDefinition[] calendarProperties = new SimplePropertyDefinition[]
		{
			MailboxCalendarConfigurationSchema.DefaultReminderTime
		};
	}
}
