using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxCalendarConfigurationSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition WorkDays = WorkingHoursSchema.WorkDays;

		public static readonly SimplePropertyDefinition WorkingHoursStartTime = WorkingHoursSchema.WorkingHoursStartTime;

		public static readonly SimplePropertyDefinition WorkingHoursEndTime = WorkingHoursSchema.WorkingHoursEndTime;

		public static readonly SimplePropertyDefinition WorkingHoursTimeZone = WorkingHoursSchema.WorkingHoursTimeZone;

		public static readonly SimplePropertyDefinition RawWeekStartDay = new SimplePropertyDefinition("rawweekstartday", ExchangeObjectVersion.Exchange2007, typeof(DayOfWeek), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition WeekStartDay = new SimplePropertyDefinition("weekstartday", ExchangeObjectVersion.Exchange2007, typeof(DayOfWeek), PropertyDefinitionFlags.Calculated, DayOfWeek.Sunday, DayOfWeek.Sunday, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			MailboxCalendarConfigurationSchema.RawWeekStartDay
		}, null, new GetterDelegate(MailboxCalendarConfiguration.WeekStartDayGetter), new SetterDelegate(MailboxCalendarConfiguration.WeekStartDaySetter));

		public static readonly SimplePropertyDefinition ShowWeekNumbers = new SimplePropertyDefinition("showweeknumbers", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition RawFirstWeekOfYear = new SimplePropertyDefinition("rawfirstweekofyear", ExchangeObjectVersion.Exchange2007, typeof(FirstWeekRules), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition FirstWeekOfYear = new SimplePropertyDefinition("firstweekofyear", ExchangeObjectVersion.Exchange2007, typeof(FirstWeekRules), PropertyDefinitionFlags.Calculated, FirstWeekRules.FirstDay, FirstWeekRules.FirstDay, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			MailboxCalendarConfigurationSchema.RawFirstWeekOfYear
		}, null, new GetterDelegate(MailboxCalendarConfiguration.FirstWeekOfYearGetter), new SetterDelegate(MailboxCalendarConfiguration.FirstWeekOfYearSetter));

		public static readonly SimplePropertyDefinition Language = new SimplePropertyDefinition("language", ExchangeObjectVersion.Exchange2010, typeof(CultureInfo), PropertyDefinitionFlags.None, null, null, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateNonNeutralCulture))
		}, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateNonNeutralCulture))
		});

		public static readonly SimplePropertyDefinition TimeIncrement = new SimplePropertyDefinition("hourincrement", ExchangeObjectVersion.Exchange2007, typeof(HourIncrement), PropertyDefinitionFlags.None, HourIncrement.ThirtyMinutes, HourIncrement.ThirtyMinutes, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition RemindersEnabled = new SimplePropertyDefinition("enablereminders", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, true, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition ReminderSoundEnabled = new SimplePropertyDefinition("enableremindersound", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, true, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition DefaultReminderTime = new SimplePropertyDefinition("piRemindDefault", ExchangeObjectVersion.Exchange2007, typeof(TimeSpan), PropertyDefinitionFlags.None, new TimeSpan(0, 0, 15, 0), new TimeSpan(0, 0, 15, 0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new CalendarReminderTimeSpanConstraint()
		});

		public static readonly SimplePropertyDefinition WeatherEnabled = new SimplePropertyDefinition("WeatherEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, true, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition WeatherUnit = new SimplePropertyDefinition("WeatherUnit", ExchangeObjectVersion.Exchange2007, typeof(WeatherTemperatureUnit), PropertyDefinitionFlags.None, WeatherTemperatureUnit.Default, WeatherTemperatureUnit.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition WeatherLocations = new SimplePropertyDefinition("WeatherLocations", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
