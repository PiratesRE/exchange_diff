using System;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TimeZoneRule : CalendarComponentBase
	{
		internal TimeZoneRule(CalendarComponentBase root) : base(root)
		{
		}

		internal NativeMethods.SystemTime TransitionDate
		{
			get
			{
				return this.transitionDate;
			}
		}

		internal int Offset
		{
			get
			{
				return this.offset;
			}
		}

		internal ushort Year
		{
			get
			{
				return (ushort)this.start.Year;
			}
		}

		internal Recurrence RecurrenceRule
		{
			get
			{
				return this.icalRecurrence;
			}
		}

		internal bool RuleHasRecurrenceUntilField
		{
			get
			{
				return this.RecurrenceRule != null && this.RecurrenceRule.UntilDateTime > DateTime.MinValue;
			}
		}

		protected override void ProcessProperty(CalendarPropertyBase calendarProperty)
		{
			PropertyId propertyId = calendarProperty.CalendarPropertyId.PropertyId;
			if (propertyId != PropertyId.DateTimeStart)
			{
				switch (propertyId)
				{
				case PropertyId.TimeZoneName:
				case PropertyId.TimeZoneOffsetFrom:
					break;
				case PropertyId.TimeZoneOffsetTo:
					this.offset = (int)((TimeSpan)calendarProperty.Value).TotalMinutes;
					return;
				default:
					return;
				}
			}
			else
			{
				this.start = (ExDateTime)((DateTime)calendarProperty.Value);
			}
		}

		protected override bool ValidateProperty(CalendarPropertyBase calendarProperty)
		{
			PropertyId propertyId = calendarProperty.CalendarPropertyId.PropertyId;
			if (propertyId == PropertyId.RecurrenceRule)
			{
				if (this.icalRecurrence != null)
				{
					base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE.TimeZoneRule.RRULE.Duplicate"));
					return false;
				}
				this.icalRecurrence = (Recurrence)calendarProperty.Value;
			}
			return true;
		}

		protected override bool ValidateProperties()
		{
			if (this.offset == 2147483647)
			{
				base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE.TimeZoneRule.TZOFFSETTO.Missing"));
				return false;
			}
			this.transitionDate = default(NativeMethods.SystemTime);
			if (this.icalRecurrence != null)
			{
				if (this.icalRecurrence.Frequency != Frequency.Yearly || this.icalRecurrence.Interval != 1 || this.icalRecurrence.ByMonth == null || this.icalRecurrence.ByMonth.Length != 1 || this.icalRecurrence.ByDayList == null || this.icalRecurrence.ByDayList.Length != 1)
				{
					base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE.TimeZoneRule"));
					return false;
				}
				this.transitionDate.Year = 0;
				this.transitionDate.Month = (ushort)this.icalRecurrence.ByMonth[0];
				this.transitionDate.DayOfWeek = (ushort)this.icalRecurrence.ByDayList[0].Day;
				short num = (short)this.icalRecurrence.ByDayList[0].OccurrenceNumber;
				if (num == -1)
				{
					num = 5;
				}
				this.transitionDate.Day = (ushort)num;
				if (this.transitionDate.Month == 0 && (this.transitionDate.DayOfWeek != 0 || this.transitionDate.Day != 0))
				{
					base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE.TimeZoneRule"));
					return false;
				}
			}
			if (this.start != ExDateTime.MaxValue)
			{
				this.transitionDate.Hour = (ushort)this.start.Hour;
				this.transitionDate.Minute = (ushort)this.start.Minute;
				this.transitionDate.Second = (ushort)this.start.Second;
				this.transitionDate.Milliseconds = (ushort)this.start.Millisecond;
			}
			if (this.start != ExDateTime.MaxValue && this.icalRecurrence == null)
			{
				this.transitionDate.Year = (ushort)this.start.Year;
				this.transitionDate.Month = (ushort)this.start.Month;
				this.transitionDate.Day = (ushort)this.start.Day;
			}
			return true;
		}

		private ExDateTime start = ExDateTime.MaxValue;

		private NativeMethods.SystemTime transitionDate;

		private int offset = int.MaxValue;

		private Recurrence icalRecurrence;
	}
}
