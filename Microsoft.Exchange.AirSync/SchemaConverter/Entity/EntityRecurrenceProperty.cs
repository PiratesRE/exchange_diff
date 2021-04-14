using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	internal class EntityRecurrenceProperty : EntityNestedProperty
	{
		public EntityRecurrenceProperty(TypeOfRecurrence typeOfRecurrence, int protocolVersion, EntityPropertyDefinition propertyDefinition, PropertyType type = PropertyType.ReadWrite) : base(propertyDefinition, type)
		{
			base.NestedData = new RecurrenceData(typeOfRecurrence, protocolVersion);
		}

		public override INestedData NestedData
		{
			get
			{
				if (base.State == PropertyState.Uninitialized)
				{
					return base.NestedData;
				}
				if (base.CalendaringEvent.PatternedRecurrence == null)
				{
					base.State = PropertyState.SetToDefault;
					return base.NestedData;
				}
				if (base.NestedData.SubProperties.Count > 2)
				{
					return base.NestedData;
				}
				RecurrenceData recurrenceData = base.NestedData as RecurrenceData;
				if (recurrenceData == null)
				{
					throw new UnexpectedTypeException("RecurrenceData", base.NestedData);
				}
				EntityRecurrenceProperty.PopulateRecurrencePattern(recurrenceData, base.CalendaringEvent.PatternedRecurrence.Pattern);
				EntityRecurrenceProperty.PopulateRecurrenceRange(recurrenceData, base.CalendaringEvent.PatternedRecurrence.Range);
				return recurrenceData;
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			RecurrenceData recurrenceData = nestedProperty.NestedData as RecurrenceData;
			if (recurrenceData == null)
			{
				throw new UnexpectedTypeException("RecurrenceData", nestedProperty.NestedData);
			}
			recurrenceData.Validate();
			PatternedRecurrence patternedRecurrence = new PatternedRecurrence();
			patternedRecurrence.Pattern = EntityRecurrenceProperty.CreateRecurrencePattern(recurrenceData);
			patternedRecurrence.Range = EntityRecurrenceProperty.CreateRecurrenceRange(base.CalendaringEvent.Start, recurrenceData);
			base.EntityPropertyDefinition.Setter(base.Item, patternedRecurrence);
		}

		private static RecurrencePattern CreateRecurrencePattern(RecurrenceData recurrenceData)
		{
			RecurrencePattern recurrencePattern;
			switch (recurrenceData.Type)
			{
			case RecurrenceData.RecurrenceType.Daily:
				if (recurrenceData.SubProperties["DayOfWeek"] != null)
				{
					recurrencePattern = new WeeklyRecurrencePattern();
					((WeeklyRecurrencePattern)recurrencePattern).DaysOfWeek = recurrenceData.DaysOfWeek;
					goto IL_15E;
				}
				recurrencePattern = new DailyRecurrencePattern();
				goto IL_15E;
			case RecurrenceData.RecurrenceType.Weekly:
				recurrencePattern = new WeeklyRecurrencePattern();
				((WeeklyRecurrencePattern)recurrencePattern).DaysOfWeek = recurrenceData.DaysOfWeek;
				goto IL_15E;
			case RecurrenceData.RecurrenceType.Monthly:
				recurrencePattern = new AbsoluteMonthlyRecurrencePattern();
				((AbsoluteMonthlyRecurrencePattern)recurrencePattern).DayOfMonth = (int)recurrenceData.DayOfMonth;
				goto IL_15E;
			case RecurrenceData.RecurrenceType.MonthlyTh:
				recurrencePattern = new RelativeMonthlyRecurrencePattern();
				((RelativeMonthlyRecurrencePattern)recurrencePattern).DaysOfWeek = recurrenceData.DaysOfWeek;
				((RelativeMonthlyRecurrencePattern)recurrencePattern).Index = recurrenceData.WeekIndex;
				goto IL_15E;
			case RecurrenceData.RecurrenceType.Yearly:
				recurrencePattern = new AbsoluteYearlyRecurrencePattern();
				((AbsoluteYearlyRecurrencePattern)recurrencePattern).DayOfMonth = (int)recurrenceData.DayOfMonth;
				((AbsoluteYearlyRecurrencePattern)recurrencePattern).Month = (int)recurrenceData.MonthOfYear;
				goto IL_15E;
			case RecurrenceData.RecurrenceType.YearlyTh:
				recurrencePattern = new RelativeYearlyRecurrencePattern();
				((RelativeYearlyRecurrencePattern)recurrencePattern).DaysOfWeek = recurrenceData.DaysOfWeek;
				((RelativeYearlyRecurrencePattern)recurrencePattern).Index = recurrenceData.WeekIndex;
				((RelativeYearlyRecurrencePattern)recurrencePattern).Month = (int)recurrenceData.MonthOfYear;
				goto IL_15E;
			}
			throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Unexpected recurrence type {0}, should have been caught in a higher validation layer", new object[]
			{
				recurrenceData.Type
			}));
			IL_15E:
			recurrencePattern.Interval = (int)(recurrenceData.HasInterval() ? recurrenceData.Interval : 1);
			return recurrencePattern;
		}

		private static RecurrenceRange CreateRecurrenceRange(ExDateTime startDate, RecurrenceData recurrenceData)
		{
			RecurrenceRange recurrenceRange = null;
			try
			{
				if (recurrenceData.HasOccurences())
				{
					recurrenceRange = new NumberedRecurrenceRange();
					((NumberedRecurrenceRange)recurrenceRange).NumberOfOccurrences = (int)recurrenceData.Occurrences;
				}
				else if (recurrenceData.HasUntil())
				{
					recurrenceRange = new EndDateRecurrenceRange();
					((EndDateRecurrenceRange)recurrenceRange).EndDate = recurrenceData.Until;
				}
				else
				{
					recurrenceRange = new NoEndRecurrenceRange();
				}
				recurrenceRange.StartDate = startDate;
			}
			catch (ArgumentException ex)
			{
				throw new ConversionException(ex.Message);
			}
			return recurrenceRange;
		}

		private static void PopulateRecurrencePattern(RecurrenceData recurrenceData, RecurrencePattern pattern)
		{
			recurrenceData.Interval = (ushort)pattern.Interval;
			DailyRecurrencePattern dailyRecurrencePattern = pattern as DailyRecurrencePattern;
			if (dailyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Daily;
				return;
			}
			WeeklyRecurrencePattern weeklyRecurrencePattern = pattern as WeeklyRecurrencePattern;
			if (weeklyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Weekly;
				recurrenceData.DaysOfWeek = weeklyRecurrencePattern.DaysOfWeek;
				return;
			}
			AbsoluteMonthlyRecurrencePattern absoluteMonthlyRecurrencePattern = pattern as AbsoluteMonthlyRecurrencePattern;
			if (absoluteMonthlyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Monthly;
				recurrenceData.DayOfMonth = (byte)absoluteMonthlyRecurrencePattern.DayOfMonth;
				return;
			}
			RelativeMonthlyRecurrencePattern relativeMonthlyRecurrencePattern = pattern as RelativeMonthlyRecurrencePattern;
			if (relativeMonthlyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.MonthlyTh;
				recurrenceData.WeekIndex = relativeMonthlyRecurrencePattern.Index;
				recurrenceData.DaysOfWeek = relativeMonthlyRecurrencePattern.DaysOfWeek;
				return;
			}
			AbsoluteYearlyRecurrencePattern absoluteYearlyRecurrencePattern = pattern as AbsoluteYearlyRecurrencePattern;
			if (absoluteYearlyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Yearly;
				recurrenceData.DayOfMonth = (byte)absoluteYearlyRecurrencePattern.DayOfMonth;
				recurrenceData.MonthOfYear = (byte)absoluteYearlyRecurrencePattern.Month;
				return;
			}
			RelativeYearlyRecurrencePattern relativeYearlyRecurrencePattern = pattern as RelativeYearlyRecurrencePattern;
			if (relativeYearlyRecurrencePattern != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.YearlyTh;
				recurrenceData.WeekIndex = relativeYearlyRecurrencePattern.Index;
				recurrenceData.DaysOfWeek = relativeYearlyRecurrencePattern.DaysOfWeek;
				recurrenceData.MonthOfYear = (byte)relativeYearlyRecurrencePattern.Month;
				return;
			}
			throw new ConversionException("Unexpected Recurrence Pattern");
		}

		private static void PopulateRecurrenceRange(RecurrenceData recurrenceData, RecurrenceRange range)
		{
			EndDateRecurrenceRange endDateRecurrenceRange = range as EndDateRecurrenceRange;
			NumberedRecurrenceRange numberedRecurrenceRange = range as NumberedRecurrenceRange;
			if (endDateRecurrenceRange != null)
			{
				recurrenceData.Until = endDateRecurrenceRange.EndDate;
				return;
			}
			if (numberedRecurrenceRange != null)
			{
				recurrenceData.Occurrences = (ushort)numberedRecurrenceRange.NumberOfOccurrences;
			}
		}
	}
}
