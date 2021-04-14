using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoRecurrenceProperty : XsoNestedProperty
	{
		public XsoRecurrenceProperty(TypeOfRecurrence recurrenceType, int protocolVersion) : base(new RecurrenceData(recurrenceType, protocolVersion))
		{
			this.recurrenceType = recurrenceType;
		}

		public override INestedData NestedData
		{
			get
			{
				if (base.NestedData.SubProperties.Count > 1)
				{
					return base.NestedData;
				}
				RecurrenceData recurrenceData = base.NestedData as RecurrenceData;
				Recurrence recurrence = null;
				if (this.recurrenceType == TypeOfRecurrence.Calendar)
				{
					CalendarItem calendarItem = base.XsoItem as CalendarItem;
					if (calendarItem == null)
					{
						throw new UnexpectedTypeException("CalendarItem", base.XsoItem);
					}
					recurrence = calendarItem.Recurrence;
				}
				else if (this.recurrenceType == TypeOfRecurrence.Task)
				{
					Task task = base.XsoItem as Task;
					if (task == null)
					{
						throw new UnexpectedTypeException("Task", base.XsoItem);
					}
					recurrence = task.Recurrence;
				}
				if (recurrence == null)
				{
					base.State = PropertyState.SetToDefault;
				}
				else
				{
					XsoRecurrenceProperty.Populate(recurrenceData, base.XsoItem);
				}
				return recurrenceData;
			}
		}

		public static void Populate(RecurrenceData recurrenceData, StoreObject item)
		{
			CalendarItem calendarItem = item as CalendarItem;
			Task task = item as Task;
			Recurrence recurrence;
			if (calendarItem != null)
			{
				recurrence = calendarItem.Recurrence;
			}
			else
			{
				if (task == null)
				{
					throw new UnexpectedTypeException("Task", item);
				}
				recurrence = task.Recurrence;
			}
			if (recurrence == null)
			{
				throw new ArgumentNullException("recurrence");
			}
			if (recurrenceData == null)
			{
				throw new ArgumentNullException("recurrenceData");
			}
			RecurrencePattern pattern = recurrence.Pattern;
			RecurrenceRange range = recurrence.Range;
			DailyRecurrencePattern dailyRecurrencePattern;
			WeeklyRecurrencePattern weeklyRecurrencePattern;
			MonthlyRecurrencePattern monthlyRecurrencePattern;
			YearlyRecurrencePattern yearlyRecurrencePattern;
			MonthlyThRecurrencePattern monthlyThRecurrencePattern;
			YearlyThRecurrencePattern yearlyThRecurrencePattern;
			DailyRegeneratingPattern dailyRegeneratingPattern;
			WeeklyRegeneratingPattern weeklyRegeneratingPattern;
			MonthlyRegeneratingPattern monthlyRegeneratingPattern;
			if ((dailyRecurrencePattern = (pattern as DailyRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Daily;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = (ushort)dailyRecurrencePattern.RecurrenceInterval;
			}
			else if ((weeklyRecurrencePattern = (pattern as WeeklyRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Weekly;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = (ushort)weeklyRecurrencePattern.RecurrenceInterval;
				recurrenceData.DayOfWeek = (byte)weeklyRecurrencePattern.DaysOfWeek;
				if (recurrenceData.ProtocolVersion >= 141)
				{
					recurrenceData.FirstDayOfWeek = weeklyRecurrencePattern.FirstDayOfWeek;
				}
			}
			else if ((monthlyRecurrencePattern = (pattern as MonthlyRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Monthly;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = (ushort)monthlyRecurrencePattern.RecurrenceInterval;
				recurrenceData.DayOfMonth = (byte)monthlyRecurrencePattern.DayOfMonth;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = monthlyRecurrencePattern.CalendarType;
				}
			}
			else if ((yearlyRecurrencePattern = (pattern as YearlyRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Yearly;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = 1;
				recurrenceData.DayOfMonth = (byte)yearlyRecurrencePattern.DayOfMonth;
				recurrenceData.MonthOfYear = (byte)yearlyRecurrencePattern.Month;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = yearlyRecurrencePattern.CalendarType;
					recurrenceData.IsLeapMonth = yearlyRecurrencePattern.IsLeapMonth;
				}
			}
			else if ((monthlyThRecurrencePattern = (pattern as MonthlyThRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.MonthlyTh;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = (ushort)monthlyThRecurrencePattern.RecurrenceInterval;
				recurrenceData.WeekOfMonth = (byte)((monthlyThRecurrencePattern.Order == RecurrenceOrderType.Last) ? ((RecurrenceOrderType)5) : monthlyThRecurrencePattern.Order);
				recurrenceData.DayOfWeek = (byte)monthlyThRecurrencePattern.DaysOfWeek;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = monthlyThRecurrencePattern.CalendarType;
				}
			}
			else if ((yearlyThRecurrencePattern = (pattern as YearlyThRecurrencePattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.YearlyTh;
				recurrenceData.Regenerate = false;
				recurrenceData.Interval = 1;
				recurrenceData.WeekOfMonth = (byte)((yearlyThRecurrencePattern.Order == RecurrenceOrderType.Last) ? ((RecurrenceOrderType)5) : yearlyThRecurrencePattern.Order);
				recurrenceData.DayOfWeek = (byte)yearlyThRecurrencePattern.DaysOfWeek;
				recurrenceData.MonthOfYear = (byte)yearlyThRecurrencePattern.Month;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = yearlyThRecurrencePattern.CalendarType;
					recurrenceData.IsLeapMonth = yearlyThRecurrencePattern.IsLeapMonth;
				}
			}
			else if ((dailyRegeneratingPattern = (pattern as DailyRegeneratingPattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Daily;
				recurrenceData.Regenerate = true;
				recurrenceData.Interval = (ushort)dailyRegeneratingPattern.RecurrenceInterval;
			}
			else if ((weeklyRegeneratingPattern = (pattern as WeeklyRegeneratingPattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Weekly;
				recurrenceData.Regenerate = true;
				recurrenceData.Interval = (ushort)weeklyRegeneratingPattern.RecurrenceInterval;
			}
			else if ((monthlyRegeneratingPattern = (pattern as MonthlyRegeneratingPattern)) != null)
			{
				recurrenceData.Type = RecurrenceData.RecurrenceType.Monthly;
				recurrenceData.Regenerate = true;
				recurrenceData.Interval = (ushort)monthlyRegeneratingPattern.RecurrenceInterval;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = monthlyRegeneratingPattern.CalendarType;
				}
			}
			else
			{
				YearlyRegeneratingPattern yearlyRegeneratingPattern;
				if ((yearlyRegeneratingPattern = (pattern as YearlyRegeneratingPattern)) == null)
				{
					throw new ConversionException("Unexpected Recurrence Pattern");
				}
				recurrenceData.Type = RecurrenceData.RecurrenceType.Yearly;
				recurrenceData.Regenerate = true;
				recurrenceData.Interval = (ushort)yearlyRegeneratingPattern.RecurrenceInterval;
				if (recurrenceData.ProtocolVersion >= 140)
				{
					recurrenceData.CalendarType = yearlyRegeneratingPattern.CalendarType;
					recurrenceData.IsLeapMonth = false;
				}
			}
			if (task != null)
			{
				object obj = task.TryGetProperty(TaskSchema.IsOneOff);
				if (obj != null && obj is bool)
				{
					recurrenceData.DeadOccur = (bool)obj;
				}
				else
				{
					recurrenceData.DeadOccur = false;
				}
				recurrenceData.Start = range.StartDate;
			}
			EndDateRecurrenceRange endDateRecurrenceRange;
			if ((endDateRecurrenceRange = (range as EndDateRecurrenceRange)) == null)
			{
				NumberedRecurrenceRange numberedRecurrenceRange;
				if ((numberedRecurrenceRange = (range as NumberedRecurrenceRange)) != null)
				{
					recurrenceData.Occurrences = (ushort)numberedRecurrenceRange.NumberOfOccurrences;
				}
				return;
			}
			if (calendarItem != null)
			{
				ExDateTime exDateTime = endDateRecurrenceRange.EndDate;
				exDateTime += calendarItem.Recurrence.StartOffset;
				ExDateTime until = ExTimeZone.UtcTimeZone.ConvertDateTime(exDateTime);
				recurrenceData.Until = until;
				return;
			}
			recurrenceData.Until = endDateRecurrenceRange.EndDate;
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			CalendarItem calendarItem = null;
			Task task = null;
			if (this.recurrenceType == TypeOfRecurrence.Calendar)
			{
				calendarItem = (base.XsoItem as CalendarItem);
				if (calendarItem == null)
				{
					throw new UnexpectedTypeException("CalendarItem", base.XsoItem);
				}
				if (!calendarItem.IsOrganizer())
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.CommonTracer, null, "InternalCopyFromModified::Skip Recurrence change in case of attendee.");
					return;
				}
			}
			else if (this.recurrenceType == TypeOfRecurrence.Task)
			{
				task = (base.XsoItem as Task);
				if (task == null)
				{
					throw new UnexpectedTypeException("Task", base.XsoItem);
				}
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
			Recurrence recurrence = (this.recurrenceType == TypeOfRecurrence.Calendar) ? calendarItem.Recurrence : task.Recurrence;
			RecurrencePattern recurrencePattern = XsoRecurrenceProperty.CreateRecurrencePattern(this.recurrenceType, recurrenceData, recurrence);
			ExDateTime startDate = ExDateTime.MinValue;
			ExDateTime endDate = ExDateTime.MinValue;
			if (this.recurrenceType == TypeOfRecurrence.Calendar)
			{
				ExDateTime startTime = calendarItem.StartTime;
				ExTimeZone exTimeZoneFromItem = TimeZoneHelper.GetExTimeZoneFromItem(calendarItem);
				startDate = exTimeZoneFromItem.ConvertDateTime(startTime);
				if (recurrenceData.HasUntil())
				{
					ExDateTime until = recurrenceData.Until;
					endDate = exTimeZoneFromItem.ConvertDateTime(until);
				}
			}
			else if (this.recurrenceType == TypeOfRecurrence.Task)
			{
				startDate = recurrenceData.Start;
				if (recurrenceData.HasUntil())
				{
					endDate = recurrenceData.Until;
				}
			}
			RecurrenceRange range;
			try
			{
				if (recurrenceData.HasOccurences())
				{
					range = new NumberedRecurrenceRange(startDate, (int)recurrenceData.Occurrences);
				}
				else if (recurrenceData.HasUntil())
				{
					range = new EndDateRecurrenceRange(startDate, endDate);
				}
				else
				{
					range = new NoEndRecurrenceRange(startDate);
				}
			}
			catch (ArgumentException ex)
			{
				throw new ConversionException(ex.Message);
			}
			bool ignoreCalendarTypeAndIsLeapMonth = recurrenceData.ProtocolVersion < 140;
			if (recurrence != null && recurrence.Pattern != null && recurrence.Pattern.Equals(recurrencePattern, ignoreCalendarTypeAndIsLeapMonth))
			{
				recurrencePattern = recurrence.Pattern;
			}
			try
			{
				if (this.recurrenceType == TypeOfRecurrence.Calendar)
				{
					ExTimeZone exTimeZoneFromItem2 = TimeZoneHelper.GetExTimeZoneFromItem(calendarItem);
					try
					{
						calendarItem.Recurrence = new Recurrence(recurrencePattern, range, exTimeZoneFromItem2, null);
						goto IL_295;
					}
					catch (ArgumentOutOfRangeException)
					{
						if (recurrenceData.CalendarType == CalendarType.Default)
						{
							recurrenceData.CalendarType = CalendarType.Gregorian;
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.CommonTracer, null, "Replace calendar recurrence calendar type with Gregorian.");
							recurrencePattern = XsoRecurrenceProperty.CreateRecurrencePattern(this.recurrenceType, recurrenceData, recurrence);
							calendarItem.Recurrence = new Recurrence(recurrencePattern, range, exTimeZoneFromItem2, null);
							goto IL_295;
						}
						throw;
					}
				}
				try
				{
					task.Recurrence = new Recurrence(recurrencePattern, range);
				}
				catch (ArgumentOutOfRangeException)
				{
					if (recurrenceData.CalendarType != CalendarType.Default)
					{
						throw;
					}
					recurrenceData.CalendarType = CalendarType.Gregorian;
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.CommonTracer, null, "Replace task recurrence calendar type with Gregorian.");
					recurrencePattern = XsoRecurrenceProperty.CreateRecurrencePattern(this.recurrenceType, recurrenceData, recurrence);
					task.Recurrence = new Recurrence(recurrencePattern, range);
				}
				task[TaskSchema.IsOneOff] = recurrenceData.DeadOccur;
				IL_295:;
			}
			catch (NotSupportedException ex2)
			{
				throw new ConversionException(ex2.Message);
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (this.recurrenceType != TypeOfRecurrence.Calendar)
			{
				if (this.recurrenceType == TypeOfRecurrence.Task)
				{
					Task task = base.XsoItem as Task;
					if (task == null)
					{
						throw new UnexpectedTypeException("Task", base.XsoItem);
					}
					task.Recurrence = null;
				}
				return;
			}
			CalendarItem calendarItem = base.XsoItem as CalendarItem;
			if (calendarItem == null)
			{
				throw new UnexpectedTypeException("CalendarItem", base.XsoItem);
			}
			if (calendarItem.IsOrganizer())
			{
				calendarItem.Recurrence = null;
				base.XsoItem[CalendarItemBaseSchema.TimeZone] = TimeZoneHelper.GetPromotedTimeZoneFromItem(calendarItem).LocalizableDisplayName.ToString();
				return;
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.CommonTracer, null, "Skip Recurrence change in case of attendee.");
		}

		private static RecurrencePattern CreateRecurrencePattern(TypeOfRecurrence recurrenceType, RecurrenceData recurrenceData, Recurrence oldRecurrence)
		{
			ushort recurrenceInterval = 1;
			RecurrenceData.RecurrenceType type = recurrenceData.Type;
			if (recurrenceData.HasInterval())
			{
				recurrenceInterval = recurrenceData.Interval;
			}
			if (recurrenceData.HasWeekOfMonth())
			{
				recurrenceData.WeekOfMonth = ((recurrenceData.WeekOfMonth == 5) ? byte.MaxValue : recurrenceData.WeekOfMonth);
			}
			if (recurrenceType == TypeOfRecurrence.Calendar || (recurrenceType == TypeOfRecurrence.Task && !recurrenceData.Regenerate))
			{
				switch (type)
				{
				case RecurrenceData.RecurrenceType.Daily:
					if (recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["CalendarType"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth, WeekOfMonth, MonthOfYear, CalendarType, IsLeapMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["DayOfWeek"] != null)
					{
						return new WeeklyRecurrencePattern((DaysOfWeek)recurrenceData.DayOfWeek, (int)recurrenceInterval);
					}
					return new DailyRecurrencePattern((int)recurrenceInterval);
				case RecurrenceData.RecurrenceType.Weekly:
				{
					if (recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["CalendarType"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth, WeekOfMonth, MonthOfYear, CalendarType, IsLeapMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["DayOfWeek"] == null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek is expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					WeeklyRecurrencePattern weeklyRecurrencePattern = (oldRecurrence != null) ? (oldRecurrence.Pattern as WeeklyRecurrencePattern) : null;
					return new WeeklyRecurrencePattern((DaysOfWeek)recurrenceData.DayOfWeek, (int)recurrenceInterval, (recurrenceData.ProtocolVersion < 141 && weeklyRecurrencePattern != null) ? weeklyRecurrencePattern.FirstDayOfWeek : recurrenceData.FirstDayOfWeek);
				}
				case RecurrenceData.RecurrenceType.Monthly:
					if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, WeekOfMonth, MonthOfYear, IsLeapMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["DayOfMonth"] == null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth is expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					return new MonthlyRecurrencePattern((int)recurrenceData.DayOfMonth, (int)recurrenceInterval, recurrenceData.CalendarType);
				case RecurrenceData.RecurrenceType.MonthlyTh:
				{
					if (recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth, MonthOfYear, IsLeapMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["WeekOfMonth"] == null || recurrenceData.SubProperties["DayOfWeek"] == null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth, DayOfWeek is expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					RecurrenceOrderType recurrenceOrderType = (recurrenceData.WeekOfMonth == byte.MaxValue) ? RecurrenceOrderType.Last : ((RecurrenceOrderType)recurrenceData.WeekOfMonth);
					if (!EnumValidator.IsValidValue<RecurrenceOrderType>(recurrenceOrderType))
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth value {0} is invalid", new object[]
						{
							recurrenceOrderType.ToString()
						}));
					}
					return new MonthlyThRecurrencePattern((DaysOfWeek)recurrenceData.DayOfWeek, recurrenceOrderType, (int)recurrenceInterval, recurrenceData.CalendarType);
				}
				case RecurrenceData.RecurrenceType.Yearly:
					if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, WeekOfMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["DayOfMonth"] == null || recurrenceData.SubProperties["MonthOfYear"] == null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth, MonthOfYear is expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					return new YearlyRecurrencePattern((int)recurrenceData.DayOfMonth, (int)recurrenceData.MonthOfYear, recurrenceData.IsLeapMonth, recurrenceData.CalendarType);
				case RecurrenceData.RecurrenceType.YearlyTh:
				{
					if (recurrenceData.SubProperties["DayOfMonth"] != null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfMonth is not expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					if (recurrenceData.SubProperties["WeekOfMonth"] == null || recurrenceData.SubProperties["DayOfWeek"] == null || recurrenceData.SubProperties["MonthOfYear"] == null)
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth, DayOfWeek, MonthOfYear is expected with recurrence type {0}", new object[]
						{
							type.ToString()
						}));
					}
					RecurrenceOrderType recurrenceOrderType = (recurrenceData.WeekOfMonth == byte.MaxValue) ? RecurrenceOrderType.Last : ((RecurrenceOrderType)recurrenceData.WeekOfMonth);
					if (!EnumValidator.IsValidValue<RecurrenceOrderType>(recurrenceOrderType))
					{
						throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth value {0} is invalid", new object[]
						{
							recurrenceOrderType.ToString()
						}));
					}
					return new YearlyThRecurrencePattern((DaysOfWeek)recurrenceData.DayOfWeek, recurrenceOrderType, (int)recurrenceData.MonthOfYear, recurrenceData.IsLeapMonth, recurrenceData.CalendarType);
				}
				}
				throw new ConversionException("Unexpected recurrence type, should have been caught in a higher validation layer");
			}
			switch (type)
			{
			case RecurrenceData.RecurrenceType.Daily:
				if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["CalendarType"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, DayOfMonth, WeekOfMonth, MonthOfYear, CalendarType, IsLeapMonth is not expected with recurrence type {0}", new object[]
					{
						type.ToString()
					}));
				}
				return new DailyRegeneratingPattern((int)recurrenceInterval);
			case RecurrenceData.RecurrenceType.Weekly:
				if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["CalendarType"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, DayOfMonth, WeekOfMonth, MonthOfYear, CalendarType, IsLeapMonth is not expected with recurrence type {0}", new object[]
					{
						type.ToString()
					}));
				}
				return new WeeklyRegeneratingPattern((int)recurrenceInterval);
			case RecurrenceData.RecurrenceType.Monthly:
				if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null || recurrenceData.SubProperties["IsLeapMonth"] != null)
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, DayOfMonth, WeekOfMonth, MonthOfYear, IsLeapMonth is not expected with recurrence type {0}", new object[]
					{
						type.ToString()
					}));
				}
				return new MonthlyRegeneratingPattern((int)recurrenceInterval, recurrenceData.CalendarType);
			case RecurrenceData.RecurrenceType.Yearly:
				if (recurrenceData.SubProperties["DayOfWeek"] != null || recurrenceData.SubProperties["DayOfMonth"] != null || recurrenceData.SubProperties["WeekOfMonth"] != null || recurrenceData.SubProperties["MonthOfYear"] != null)
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "DayOfWeek, DayOfMonth, WeekOfMonth, MonthOfYear is not expected with recurrence type {0}", new object[]
					{
						type.ToString()
					}));
				}
				return new YearlyRegeneratingPattern((int)recurrenceInterval, recurrenceData.CalendarType);
			}
			throw new ConversionException("Unexpected recurrence type '" + type + "', should have been caught in a higher validation layer");
		}

		private TypeOfRecurrence recurrenceType;
	}
}
