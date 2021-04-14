using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	public class CalendarViewParameters : ICalendarViewParameters
	{
		public CalendarViewParameters(ExDateTime? startTime, ExDateTime? endTime)
		{
			this.startTime = startTime;
			this.endTime = endTime;
			ExDateTime today = ExDateTime.Today;
			ExDateTime exDateTime = new ExDateTime(today.TimeZone, today.Year, today.Month, 1);
			this.EffectiveStartTime = ((startTime != null) ? startTime.Value : ((endTime != null) ? endTime.Value.AddMonths(-3) : exDateTime));
			this.EffectiveEndTime = ((endTime != null) ? this.endTime.Value : this.EffectiveStartTime.AddMonths(3));
		}

		public ExDateTime EffectiveEndTime { get; private set; }

		public ExDateTime EffectiveStartTime { get; private set; }

		public bool HasExplicitEndTime
		{
			get
			{
				return this.endTime != null;
			}
		}

		public bool HasExplicitStartTime
		{
			get
			{
				return this.startTime != null;
			}
		}

		public bool IsDefaultView
		{
			get
			{
				return !this.HasExplicitStartTime && !this.HasExplicitEndTime;
			}
		}

		private static bool IsValidFilter(QueryFilter queryFilter, ref ExDateTime? start, ref ExDateTime? end)
		{
			return CalendarViewParameters.IsValidComparisonFilter(queryFilter as ComparisonFilter, ref start, ref end) || CalendarViewParameters.IsValidAndFilter(queryFilter as AndFilter, ref start, ref end);
		}

		private static bool IsValidAndFilter(AndFilter andFilter, ref ExDateTime? start, ref ExDateTime? end)
		{
			if (andFilter == null)
			{
				return false;
			}
			foreach (QueryFilter queryFilter in andFilter.Filters)
			{
				if (!CalendarViewParameters.IsValidFilter(queryFilter, ref start, ref end))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsValidComparisonFilter(ComparisonFilter comparisonFilter, ref ExDateTime? start, ref ExDateTime? end)
		{
			if (comparisonFilter == null)
			{
				return false;
			}
			if (comparisonFilter.PropertyValue is ExDateTime)
			{
				ExDateTime value = (ExDateTime)comparisonFilter.PropertyValue;
				if (comparisonFilter.Property == CalendarItemInstanceSchema.EndTime && comparisonFilter.ComparisonOperator == ComparisonOperator.GreaterThanOrEqual)
				{
					if (start != null)
					{
						return false;
					}
					start = new ExDateTime?(value);
				}
				if (comparisonFilter.Property == CalendarItemInstanceSchema.StartTime && comparisonFilter.ComparisonOperator == ComparisonOperator.LessThanOrEqual)
				{
					if (end != null)
					{
						return false;
					}
					end = new ExDateTime?(value);
				}
			}
			return true;
		}

		public const int DefaultPeriodInMonths = 3;

		private readonly ExDateTime? endTime;

		private readonly ExDateTime? startTime;
	}
}
