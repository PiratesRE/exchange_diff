using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public sealed class RecurrenceUtilities
	{
		internal RecurrenceUtilities(Recurrence recurrence, TextWriter output)
		{
			this.recurrence = recurrence;
			this.output = output;
			this.recurrenceType = this.MapRecurrenceType();
		}

		public OwaRecurrenceType MapRecurrenceType()
		{
			OwaRecurrenceType result = OwaRecurrenceType.None;
			if (this.recurrence == null)
			{
				result = OwaRecurrenceType.None;
			}
			else if (this.recurrence.Pattern is DailyRecurrencePattern)
			{
				result = OwaRecurrenceType.Daily;
			}
			else if (this.recurrence.Pattern is WeeklyRecurrencePattern)
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)this.recurrence.Pattern;
				if (weeklyRecurrencePattern.DaysOfWeek == DaysOfWeek.Weekdays)
				{
					result = (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday);
				}
				else
				{
					result = OwaRecurrenceType.Weekly;
				}
			}
			else if (this.recurrence.Pattern is MonthlyRecurrencePattern)
			{
				result = OwaRecurrenceType.Monthly;
			}
			else if (this.recurrence.Pattern is MonthlyThRecurrencePattern)
			{
				result = (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh);
			}
			else if (this.recurrence.Pattern is YearlyRecurrencePattern)
			{
				result = OwaRecurrenceType.Yearly;
			}
			else if (this.recurrence.Pattern is YearlyThRecurrencePattern)
			{
				result = (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh);
			}
			else if (this.recurrence.Pattern is DailyRegeneratingPattern)
			{
				result = (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyRegenerating);
			}
			else if (this.recurrence.Pattern is WeeklyRegeneratingPattern)
			{
				result = (OwaRecurrenceType.Weekly | OwaRecurrenceType.WeeklyRegenerating);
			}
			else if (this.recurrence.Pattern is MonthlyRegeneratingPattern)
			{
				result = (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyRegenerating);
			}
			else if (this.recurrence.Pattern is YearlyRegeneratingPattern)
			{
				result = (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyRegenerating);
			}
			return result;
		}

		public OwaRecurrenceType ItemRecurrenceType
		{
			get
			{
				return this.recurrenceType;
			}
		}

		public int RecurrenceInterval()
		{
			if (this.recurrenceType != OwaRecurrenceType.None && this.recurrence != null && this.recurrence.Pattern is IntervalRecurrencePattern)
			{
				return ((IntervalRecurrencePattern)this.recurrence.Pattern).RecurrenceInterval;
			}
			return 1;
		}

		public int RegeneratingInterval()
		{
			if (this.recurrenceType != OwaRecurrenceType.None && this.recurrence != null && this.recurrence.Pattern is RegeneratingPattern)
			{
				return ((RegeneratingPattern)this.recurrence.Pattern).RecurrenceInterval;
			}
			return 1;
		}

		public int RecurrenceDays()
		{
			if (this.recurrence == null)
			{
				return 0;
			}
			OwaRecurrenceType owaRecurrenceType = this.recurrenceType;
			if (owaRecurrenceType == OwaRecurrenceType.Weekly)
			{
				return (int)((WeeklyRecurrencePattern)this.recurrence.Pattern).DaysOfWeek;
			}
			if (owaRecurrenceType == (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
			{
				return (int)((MonthlyThRecurrencePattern)this.recurrence.Pattern).DaysOfWeek;
			}
			if (owaRecurrenceType != (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
			{
				return 0;
			}
			return (int)((YearlyThRecurrencePattern)this.recurrence.Pattern).DaysOfWeek;
		}

		public int RecurrenceDay()
		{
			if (this.recurrence == null)
			{
				return 1;
			}
			OwaRecurrenceType owaRecurrenceType = this.recurrenceType;
			if (owaRecurrenceType == OwaRecurrenceType.Monthly)
			{
				return ((MonthlyRecurrencePattern)this.recurrence.Pattern).DayOfMonth;
			}
			if (owaRecurrenceType != OwaRecurrenceType.Yearly)
			{
				return 1;
			}
			return ((YearlyRecurrencePattern)this.recurrence.Pattern).DayOfMonth;
		}

		public int RecurrenceMonth()
		{
			if (this.recurrence == null)
			{
				return 1;
			}
			OwaRecurrenceType owaRecurrenceType = this.recurrenceType;
			if (owaRecurrenceType == OwaRecurrenceType.Yearly)
			{
				return ((YearlyRecurrencePattern)this.recurrence.Pattern).Month;
			}
			if (owaRecurrenceType != (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
			{
				return 1;
			}
			return ((YearlyThRecurrencePattern)this.recurrence.Pattern).Month;
		}

		public int RecurrenceOrder()
		{
			if (this.recurrence == null)
			{
				return 0;
			}
			OwaRecurrenceType owaRecurrenceType = this.recurrenceType;
			if (owaRecurrenceType == (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
			{
				return (int)((MonthlyThRecurrencePattern)this.recurrence.Pattern).Order;
			}
			if (owaRecurrenceType != (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
			{
				return 0;
			}
			return (int)((YearlyThRecurrencePattern)this.recurrence.Pattern).Order;
		}

		public RecurrenceRangeType ItemRecurrenceRangeType()
		{
			if (this.recurrence == null)
			{
				return RecurrenceRangeType.NoEnd;
			}
			if (this.recurrenceType == OwaRecurrenceType.None)
			{
				return RecurrenceRangeType.NoEnd;
			}
			if (this.recurrence.Range is NumberedRecurrenceRange)
			{
				return RecurrenceRangeType.Numbered;
			}
			if (this.recurrence.Range is EndDateRecurrenceRange)
			{
				return RecurrenceRangeType.EndDate;
			}
			return RecurrenceRangeType.NoEnd;
		}

		public void RenderRecurrenceRangeStart()
		{
			ExDateTime dateTime;
			if (this.recurrenceType != OwaRecurrenceType.None)
			{
				dateTime = this.recurrence.Range.StartDate;
			}
			else
			{
				dateTime = DateTimeUtilities.GetLocalTime();
			}
			RenderingUtilities.RenderDateTimeScriptObject(this.output, dateTime);
		}

		public int RecurrenceRangeOccurences()
		{
			if (this.recurrenceType != OwaRecurrenceType.None && this.ItemRecurrenceRangeType() == RecurrenceRangeType.Numbered)
			{
				return ((NumberedRecurrenceRange)this.recurrence.Range).NumberOfOccurrences;
			}
			return 10;
		}

		public void RenderRecurrenceRangeEnd()
		{
			ExDateTime dateTime;
			if (this.recurrenceType != OwaRecurrenceType.None && this.ItemRecurrenceRangeType() == RecurrenceRangeType.EndDate)
			{
				dateTime = ((EndDateRecurrenceRange)this.recurrence.Range).EndDate;
			}
			else
			{
				dateTime = DateTimeUtilities.GetLocalTime();
			}
			RenderingUtilities.RenderDateTimeScriptObject(this.output, dateTime);
		}

		private Recurrence recurrence;

		private OwaRecurrenceType recurrenceType = OwaRecurrenceType.None;

		private TextWriter output;
	}
}
