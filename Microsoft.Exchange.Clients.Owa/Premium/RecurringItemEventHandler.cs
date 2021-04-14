using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class RecurringItemEventHandler : ItemEventHandler
	{
		protected Recurrence CreateRecurrenceFromRequest()
		{
			Recurrence result = null;
			if (base.IsParameterSet("RcrT"))
			{
				OwaRecurrenceType owaRecurrenceType = (OwaRecurrenceType)base.GetParameter("RcrT");
				RecurrencePattern recurrencePattern = null;
				OwaRecurrenceType owaRecurrenceType2 = owaRecurrenceType;
				if (owaRecurrenceType2 <= (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
				{
					if (owaRecurrenceType2 <= OwaRecurrenceType.Monthly)
					{
						switch (owaRecurrenceType2)
						{
						case OwaRecurrenceType.Daily:
							recurrencePattern = new DailyRecurrencePattern((int)base.GetParameter("RcrI"));
							break;
						case OwaRecurrenceType.None | OwaRecurrenceType.Daily:
							break;
						case OwaRecurrenceType.Weekly:
							recurrencePattern = new WeeklyRecurrencePattern((DaysOfWeek)base.GetParameter("RcrDys"), (int)base.GetParameter("RcrI"));
							break;
						default:
							if (owaRecurrenceType2 == OwaRecurrenceType.Monthly)
							{
								recurrencePattern = new MonthlyRecurrencePattern((int)base.GetParameter("RcrDy"), (int)base.GetParameter("RcrI"));
							}
							break;
						}
					}
					else if (owaRecurrenceType2 != OwaRecurrenceType.Yearly)
					{
						if (owaRecurrenceType2 != (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyEveryWeekday))
						{
							if (owaRecurrenceType2 == (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyTh))
							{
								recurrencePattern = new MonthlyThRecurrencePattern((DaysOfWeek)base.GetParameter("RcrDys"), (RecurrenceOrderType)base.GetParameter("RcrO"), (int)base.GetParameter("RcrI"));
							}
						}
						else
						{
							recurrencePattern = new WeeklyRecurrencePattern(DaysOfWeek.Weekdays);
						}
					}
					else
					{
						recurrencePattern = new YearlyRecurrencePattern((int)base.GetParameter("RcrDy"), (int)base.GetParameter("RcrM"));
					}
				}
				else if (owaRecurrenceType2 <= (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyRegenerating))
				{
					if (owaRecurrenceType2 != (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyTh))
					{
						if (owaRecurrenceType2 == (OwaRecurrenceType.Daily | OwaRecurrenceType.DailyRegenerating))
						{
							recurrencePattern = new DailyRegeneratingPattern((int)base.GetParameter("RgrI"));
						}
					}
					else
					{
						recurrencePattern = new YearlyThRecurrencePattern((DaysOfWeek)base.GetParameter("RcrDys"), (RecurrenceOrderType)base.GetParameter("RcrO"), (int)base.GetParameter("RcrM"));
					}
				}
				else if (owaRecurrenceType2 != (OwaRecurrenceType.Weekly | OwaRecurrenceType.WeeklyRegenerating))
				{
					if (owaRecurrenceType2 != (OwaRecurrenceType.Monthly | OwaRecurrenceType.MonthlyRegenerating))
					{
						if (owaRecurrenceType2 == (OwaRecurrenceType.Yearly | OwaRecurrenceType.YearlyRegenerating))
						{
							recurrencePattern = new YearlyRegeneratingPattern((int)base.GetParameter("RgrI"));
						}
					}
					else
					{
						recurrencePattern = new MonthlyRegeneratingPattern((int)base.GetParameter("RgrI"));
					}
				}
				else
				{
					recurrencePattern = new WeeklyRegeneratingPattern((int)base.GetParameter("RgrI"));
				}
				if (owaRecurrenceType != OwaRecurrenceType.None)
				{
					RecurrenceRangeType recurrenceRangeType = (RecurrenceRangeType)base.GetParameter("RcrRngT");
					ExDateTime startDate = (ExDateTime)base.GetParameter("RcrRngS");
					RecurrenceRange recurrenceRange;
					switch (recurrenceRangeType)
					{
					case RecurrenceRangeType.Numbered:
						recurrenceRange = new NumberedRecurrenceRange(startDate, (int)base.GetParameter("RcrRngO"));
						goto IL_2C8;
					case RecurrenceRangeType.EndDate:
						recurrenceRange = new EndDateRecurrenceRange(startDate, (ExDateTime)base.GetParameter("RcrRngE"));
						goto IL_2C8;
					}
					recurrenceRange = new NoEndRecurrenceRange(startDate);
					IL_2C8:
					if (recurrencePattern != null && recurrenceRange != null)
					{
						result = new Recurrence(recurrencePattern, recurrenceRange);
					}
				}
			}
			return result;
		}

		public const string Recurrence = "RcrT";

		public const string RecurrenceInterval = "RcrI";

		public const string RegeneratingInterval = "RgrI";

		public const string RecurrenceDays = "RcrDys";

		public const string RecurrenceDay = "RcrDy";

		public const string RecurrenceMonth = "RcrM";

		public const string RecurrenceOrder = "RcrO";

		public const string RecurrenceRangeT = "RcrRngT";

		public const string RecurrenceRangeStart = "RcrRngS";

		public const string RecurrenceRangeOccurences = "RcrRngO";

		public const string RecurrenceRangeEndDate = "RcrRngE";
	}
}
