using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Recurrence
	{
		public Recurrence(RecurrencePattern pattern, RecurrenceRange range) : this(pattern, range, default(TimeSpan), default(TimeSpan), null, null, false, null)
		{
		}

		public Recurrence(RecurrencePattern pattern, RecurrenceRange range, ExTimeZone createExTimeZone, ExTimeZone readExTimeZone) : this(pattern, range, default(TimeSpan), default(TimeSpan), createExTimeZone, readExTimeZone, true, null)
		{
		}

		internal Recurrence(RecurrencePattern pattern, RecurrenceRange range, ExDateTime? endDateOverride) : this(pattern, range, default(TimeSpan), default(TimeSpan), null, null, false, endDateOverride)
		{
		}

		internal Recurrence(RecurrencePattern pattern, RecurrenceRange range, TimeSpan startOffset, TimeSpan endOffset, ExTimeZone createExTimeZone, ExTimeZone readExTimeZone, ExDateTime? endDateOverride) : this(pattern, range, startOffset, endOffset, createExTimeZone, readExTimeZone, createExTimeZone != null, endDateOverride)
		{
		}

		protected Recurrence(RecurrencePattern pattern, RecurrenceRange range, TimeSpan startOffset, TimeSpan endOffset, ExTimeZone createExTimeZone, ExTimeZone readExTimeZone) : this(pattern, range, startOffset, endOffset, createExTimeZone, readExTimeZone, createExTimeZone != null, null)
		{
		}

		private Recurrence(RecurrencePattern pattern, RecurrenceRange range, TimeSpan startOffset, TimeSpan endOffset, ExTimeZone createExTimeZone, ExTimeZone readExTimeZone, bool hasTimeZone, ExDateTime? endDateOverride)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("Pattern");
			}
			if (range == null)
			{
				throw new ArgumentNullException("Range");
			}
			if (hasTimeZone)
			{
				if (createExTimeZone == null)
				{
					throw new ArgumentException("timeZone");
				}
				range.StartDate.CheckExpectedTimeZone(createExTimeZone);
				this.createExTimeZone = createExTimeZone;
			}
			else
			{
				if (createExTimeZone != null)
				{
					throw new ArgumentException("timeZone");
				}
				hasTimeZone = true;
				this.createExTimeZone = range.StartDate.TimeZone;
			}
			this.hasTimeZone = hasTimeZone;
			this.readExTimeZone = (readExTimeZone ?? ExTimeZone.UtcTimeZone);
			this.pattern = pattern;
			this.range = range;
			this.startOffset = TimeSpan.FromMinutes(Math.Truncate(startOffset.TotalMinutes));
			this.endOffset = TimeSpan.FromMinutes(Math.Truncate(endOffset.TotalMinutes));
			ExDateTime exDateTime = this.createExTimeZone.ConvertDateTime(this.range.StartDate);
			if (this.range is NumberedRecurrenceRange)
			{
				this.range = new NumberedRecurrenceRange(exDateTime, ((NumberedRecurrenceRange)this.range).NumberOfOccurrences);
			}
			else if (this.range is EndDateRecurrenceRange)
			{
				ExDateTime exDateTime2 = this.createExTimeZone.ConvertDateTime(((EndDateRecurrenceRange)this.range).EndDate);
				this.range = new EndDateRecurrenceRange(exDateTime, exDateTime2);
			}
			else
			{
				this.range = new NoEndRecurrenceRange(exDateTime);
			}
			ExDateTime exDateTime3 = Recurrence.MinimumDateForRecurrenceStart;
			exDateTime3 = this.createExTimeZone.Assign(exDateTime3);
			this.maxSupportedDate = this.createExTimeZone.Assign(this.maxSupportedDate);
			if (this.Pattern is IMonthlyPatternInfo)
			{
				ExCalendar calendar = Recurrence.GetCalendar(((IMonthlyPatternInfo)this.Pattern).CalendarType);
				ExDateTime exDateTime4 = this.createExTimeZone.Assign(calendar.MinSupportedDateTime);
				ExDateTime exDateTime5 = this.createExTimeZone.Assign(calendar.MaxSupportedDateTime);
				exDateTime3 = ((exDateTime4 > exDateTime3) ? exDateTime4 : exDateTime3);
				this.maxSupportedDate = ((exDateTime5 < this.maxSupportedDate) ? exDateTime5 : this.maxSupportedDate);
			}
			exDateTime = this.GetNthOccurrence(exDateTime, 1);
			if (exDateTime <= exDateTime3)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, Start Date for the RecurrenceRange ({0}) is beyond minimum allowed date in recurring calendar item ({1})", exDateTime, exDateTime3.IncrementDays(1));
				throw new RecurrenceStartDateTooSmallException(exDateTime, ServerStrings.ExStartDateCantBeLessThanMinimum(exDateTime, exDateTime3.IncrementDays(1)));
			}
			if (exDateTime >= this.maxSupportedDate)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, StartDate for RecurrenceRange ({0}) is beyond maximum allowed date in recurring calendar item ({1})", exDateTime, this.maxSupportedDate.IncrementDays(-1));
				throw new RecurrenceEndDateTooBigException(exDateTime, ServerStrings.ExStartDateCantBeGreaterThanMaximum(exDateTime, this.maxSupportedDate.IncrementDays(-1)));
			}
			EndDateRecurrenceRange endDateRecurrenceRange;
			NumberedRecurrenceRange numberedRecurrenceRange;
			if ((endDateRecurrenceRange = (this.Range as EndDateRecurrenceRange)) != null)
			{
				this.endDate = endDateRecurrenceRange.EndDate;
				if (hasTimeZone)
				{
					this.endDate = this.createExTimeZone.ConvertDateTime(this.endDate);
				}
				if (this.endDate >= this.maxSupportedDate)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, EndDate for EndDateRecurrenceRange ({0}) is beyond maximum allowed date in recurring calendar item ({1})", this.endDate, this.maxSupportedDate.IncrementDays(-1));
					throw new RecurrenceEndDateTooBigException(this.endDate, ServerStrings.ExEndDateCantExceedMaxDate(this.endDate, this.maxSupportedDate.IncrementDays(-1)));
				}
				if (exDateTime > this.endDate)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError<RecurrenceRange, RecurrencePattern>((long)this.GetHashCode(), "Recurrence::Recurrence, Recurrence does not have any occurrences within the range. range:{0} pattern:{1}", range, pattern);
					throw new RecurrenceHasNoOccurrenceException(exDateTime, this.endDate, ServerStrings.ExNoOccurrencesInRecurrence);
				}
				this.range = new EndDateRecurrenceRange(exDateTime, this.endDate);
				this.numberOfOccurrences = this.GetNumberOfOccurrencesSinceStart(this.endDate);
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime, ExDateTime, int>((long)this.GetHashCode(), "Recurrence::Recurrence, EndDate Recurrence has effecitve startDate {0} and endDate {1} with {2} occurrences", exDateTime, this.endDate, this.numberOfOccurrences);
			}
			else if ((numberedRecurrenceRange = (this.Range as NumberedRecurrenceRange)) != null)
			{
				this.numberOfOccurrences = numberedRecurrenceRange.NumberOfOccurrences;
				if (this.range.StartDate != exDateTime)
				{
					this.range = new NumberedRecurrenceRange(exDateTime, this.numberOfOccurrences);
				}
				if (endDateOverride != null)
				{
					if (endDateOverride.Value >= this.maxSupportedDate)
					{
						ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, EndDate for NumberedRecurrenceRange ({0}) is beyond maximum allowed date in recurring calendar item ({1})", endDateOverride.Value, this.maxSupportedDate.IncrementDays(-1));
						throw new RecurrenceEndDateTooBigException(endDateOverride.Value, ServerStrings.ExEndDateCantExceedMaxDate(endDateOverride.Value, this.maxSupportedDate.IncrementDays(-1)));
					}
					if (endDateOverride.Value < exDateTime)
					{
						ExTraceGlobals.RecurrenceTracer.TraceError<RecurrenceRange, RecurrencePattern>((long)this.GetHashCode(), "Recurrence::Recurrence, Recurrence does not have any occurrences within the range. range:{0} pattern:{1}", range, pattern);
						throw new RecurrenceHasNoOccurrenceException(exDateTime, endDateOverride.Value, ServerStrings.ExNoOccurrencesInRecurrence);
					}
					if (endDateOverride.Value < this.GetNthOccurrence(exDateTime, this.numberOfOccurrences))
					{
						this.endDate = this.GetNthOccurrence(this.Range.StartDate, numberedRecurrenceRange.NumberOfOccurrences);
					}
					else
					{
						this.endDate = endDateOverride.Value;
					}
				}
				else
				{
					this.endDate = this.GetNthOccurrence(this.Range.StartDate, numberedRecurrenceRange.NumberOfOccurrences);
				}
				if (this.endDate >= this.maxSupportedDate)
				{
					ExTraceGlobals.RecurrenceTracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, EndDate for EndDateRecurrenceRange ({0}) is beyond maximum allowed date in recurring calendar item ({1})", this.endDate, this.maxSupportedDate.IncrementDays(-1));
					throw new RecurrenceEndDateTooBigException(this.endDate, ServerStrings.ExEndDateCantExceedMaxDate(this.endDate, this.maxSupportedDate.IncrementDays(-1)));
				}
				ExTraceGlobals.RecurrenceTracer.Information<int, ExDateTime, ExDateTime>((long)this.GetHashCode(), "Recurrence::Recurrence, Numbered Recurrence with {0} occurrences has effective startDate {1} and endDate {2}", this.numberOfOccurrences, exDateTime, this.endDate);
			}
			else
			{
				if (!(this.Pattern is RegeneratingPattern) && this.range.StartDate != exDateTime)
				{
					this.range = new NoEndRecurrenceRange(exDateTime);
				}
				this.endDate = ((Recurrence.MaximumDateForRecurrenceEnd > this.maxSupportedDate) ? this.maxSupportedDate : Recurrence.MaximumDateForRecurrenceEnd);
				this.numberOfOccurrences = this.GetNumberOfOccurrencesSinceStart(this.endDate);
				ExTraceGlobals.RecurrenceTracer.Information<ExDateTime, ExDateTime, int>((long)this.GetHashCode(), "Recurrence::Recurrence, NoEnd recurrence has effective startDate {0}, endDate {1} and numberOfOccurrences{2}", exDateTime, this.endDate, this.numberOfOccurrences);
			}
			TimeSpan timeSpan = this.MinTimeBetweenTwoOccurrences();
			if (this.endOffset - this.startOffset > timeSpan)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<TimeSpan, TimeSpan>((long)this.GetHashCode(), "Recurrence::Recurrence, Duration between startTime and endTime ({0}) of the recurrence is greater then minimum duration between two occurrences ({1}).", this.endOffset, timeSpan);
				throw new OccurrenceTimeSpanTooBigException(this.endOffset - this.startOffset, timeSpan, ServerStrings.ExMeetingCantCrossOtherOccurrences(this.endOffset - this.startOffset, timeSpan));
			}
		}

		public static StorePropertyDefinition[] RequiredRecurrenceProperties
		{
			get
			{
				return InternalRecurrence.RequiredRecurrenceProperties;
			}
		}

		public RecurrencePattern Pattern
		{
			get
			{
				return this.pattern;
			}
		}

		public RecurrenceRange Range
		{
			get
			{
				return this.range;
			}
		}

		public bool HasTimeZone
		{
			get
			{
				return this.hasTimeZone;
			}
		}

		public ExTimeZone CreatedExTimeZone
		{
			get
			{
				return this.createExTimeZone;
			}
		}

		public virtual ExTimeZone ReadExTimeZone
		{
			get
			{
				return this.readExTimeZone;
			}
		}

		public TimeSpan StartOffset
		{
			get
			{
				return this.startOffset;
			}
			protected internal set
			{
				this.startOffset = value;
			}
		}

		public TimeSpan EndOffset
		{
			get
			{
				return this.endOffset;
			}
			protected internal set
			{
				this.endOffset = value;
			}
		}

		public ExDateTime EndDate
		{
			get
			{
				return this.endDate;
			}
		}

		protected virtual bool GenerateTimeInDay
		{
			get
			{
				return true;
			}
		}

		protected int NumberOfOccurrences
		{
			get
			{
				return this.numberOfOccurrences;
			}
		}

		public static bool TryFromMasterPropertyBag(PropertyBag masterPropertyBag, MailboxSession session, out Recurrence recurrence)
		{
			try
			{
				VersionedId masterItemId = masterPropertyBag.TryGetProperty(InternalSchema.ItemId) as VersionedId;
				RecurringItemLatencyInformation latencyInformation = new RecurringItemLatencyInformation();
				recurrence = InternalRecurrence.FromMasterPropertyBag(masterPropertyBag, session, masterItemId, CalendarItem.DefaultCodePage, latencyInformation);
				return recurrence != null;
			}
			catch (RecurrenceException arg)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError<string, RecurrenceException>(0L, "Recurrence::TryFromMasterPropertyBag - The recurrence blob data is invalid (Session Display Name: {0}). Exception: {1}", session.DisplayName, arg);
			}
			catch (CorruptDataException arg2)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceError<string, CorruptDataException>(0L, "Recurrence::TryFromMasterPropertyBag - The data in the master property bag is corrupt (Session Display Name: {0}). Exception: {1}", session.DisplayName, arg2);
			}
			recurrence = null;
			return false;
		}

		public override string ToString()
		{
			return string.Format("Pattern: {0}, Range: {1}", this.Pattern.ToString(), this.Range.ToString());
		}

		public virtual OccurrenceInfo GetNextOccurrence(OccurrenceInfo occurrenceInfo)
		{
			ExDateTime exDateTime = this.CreatedExTimeZone.ConvertDateTime(occurrenceInfo.OriginalStartTime).Date;
			exDateTime = this.GetNextOccurrenceDateId(exDateTime);
			if (exDateTime < ExDateTime.MaxValue && exDateTime <= this.endDate)
			{
				return this.GetOccurrenceInfoByDateId(exDateTime);
			}
			throw new ArgumentOutOfRangeException("occurrenceInfo");
		}

		public virtual OccurrenceInfo GetPreviousOccurrence(OccurrenceInfo occurrenceInfo)
		{
			ExDateTime exDateTime = occurrenceInfo.OccurrenceDateId;
			exDateTime = this.GetPreviousOccurrenceDateId(exDateTime);
			if (exDateTime < ExDateTime.MaxValue && exDateTime >= this.range.StartDate)
			{
				return this.GetOccurrenceInfoByDateId(exDateTime);
			}
			throw new ArgumentOutOfRangeException("occurrenceInfo");
		}

		public OccurrenceInfo GetFirstOccurrenceAfterDate(ExDateTime startDate)
		{
			ExDateTime date = (startDate.TimeZone == this.CreatedExTimeZone) ? startDate : this.CreatedExTimeZone.ConvertDateTime(startDate);
			ExDateTime nextOccurrenceDateId = this.GetNextOccurrenceDateId(date);
			if (nextOccurrenceDateId < ExDateTime.MaxValue)
			{
				return this.GetOccurrenceInfoByDateId(nextOccurrenceDateId);
			}
			return null;
		}

		public OccurrenceInfo GetLastOccurrenceBeforeDate(ExDateTime date)
		{
			ExDateTime date2 = (date.TimeZone == this.CreatedExTimeZone) ? date : this.CreatedExTimeZone.ConvertDateTime(date);
			if (date2.Date == this.Range.StartDate.Date)
			{
				return this.GetFirstOccurrence();
			}
			ExDateTime previousOccurrenceDateId = this.GetPreviousOccurrenceDateId(date2);
			if (previousOccurrenceDateId > ExDateTime.MinValue)
			{
				return this.GetOccurrenceInfoByDateId(previousOccurrenceDateId);
			}
			return null;
		}

		public virtual OccurrenceInfo GetFirstOccurrence()
		{
			ExDateTime nextOccurrenceDateId = this.GetNextOccurrenceDateId(ExDateTime.MinValue);
			if (nextOccurrenceDateId == ExDateTime.MaxValue)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "Recurrence::Recurrence, No occurrence found in currence recurrence.");
				throw new RecurrenceHasNoOccurrenceException(this.Range.StartDate, this.endDate, ServerStrings.ExNoOccurrencesInRecurrence);
			}
			return this.GetOccurrenceInfoByDateId(nextOccurrenceDateId);
		}

		public virtual OccurrenceInfo GetLastOccurrence()
		{
			OccurrenceInfo occurrenceInfo = this.InternalGetLastOccurrence();
			if (occurrenceInfo == null)
			{
				throw new RecurrenceHasNoOccurrenceException(this.Range.StartDate, this.endDate, ServerStrings.ExNoOccurrencesInRecurrence);
			}
			return occurrenceInfo;
		}

		public IList<OccurrenceInfo> GetRecentOccurrencesInfoList(ExDateTime referenceTime, int count)
		{
			ExDateTime exDateTime = (referenceTime.TimeZone == this.CreatedExTimeZone) ? referenceTime : this.CreatedExTimeZone.ConvertDateTime(referenceTime);
			ExDateTime[] array = new ExDateTime[count];
			int num = count / 2;
			int num2 = count - num;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			bool flag = true;
			bool flag2 = true;
			ExDateTime exDateTime2 = exDateTime.Date;
			ExDateTime exDateTime3 = (exDateTime.Date >= ExDateTime.MaxValue.IncrementDays(-1)) ? ExDateTime.MaxValue : exDateTime.Date.IncrementDays(1);
			while (num3 < count && (flag || flag2))
			{
				if (num5 < num2 && flag2)
				{
					exDateTime3 = this.GetPreviousOccurrenceDateId(exDateTime3);
					if (exDateTime3 == ExDateTime.MinValue)
					{
						num2 = num5;
						num = count - num2;
						flag2 = false;
						continue;
					}
					num5++;
					int num6 = (-num5 + count) % count;
					array[num6] = exDateTime3;
				}
				else if (num4 < num && flag)
				{
					exDateTime2 = this.GetNextOccurrenceDateId(exDateTime2);
					if (exDateTime2 == ExDateTime.MaxValue)
					{
						num = num4;
						num2 = count - num;
						flag = false;
						continue;
					}
					array[num4] = exDateTime2;
					num4++;
				}
				num3++;
			}
			List<OccurrenceInfo> list = new List<OccurrenceInfo>(num3);
			for (int i = 0; i < num3; i++)
			{
				int num7 = (-num5 + i + count) % count;
				list.Add(this.GetOccurrenceInfoByDateId(array[num7]));
			}
			return list;
		}

		public IList<OccurrenceInfo> GetOccurrenceInfoList(ExDateTime startView, ExDateTime endView)
		{
			return this.GetOccurrenceInfoList(startView, endView, int.MaxValue);
		}

		public IList<OccurrenceInfo> GetOccurrenceInfoList(ExDateTime startView, ExDateTime endView, int maxCount)
		{
			if (endView < startView)
			{
				throw new ArgumentException(ServerStrings.ExInvalidDateTimeRange(startView, endView));
			}
			if (!(this.Range is NoEndRecurrenceRange) && this.endDate < this.maxSupportedDate)
			{
				OccurrenceInfo occurrenceInfo = this.InternalGetLastOccurrence();
				if (occurrenceInfo == null || occurrenceInfo.EndTime < startView)
				{
					return new List<OccurrenceInfo>();
				}
			}
			ExDateTime d = this.CreatedExTimeZone.ConvertDateTime(startView);
			ExDateTime exDateTime = (d != ExDateTime.MinValue) ? d.Date : ExDateTime.MinValue;
			do
			{
				exDateTime = this.GetPreviousOccurrenceDateId(exDateTime);
			}
			while (exDateTime != ExDateTime.MinValue && this.GetOccurrenceInfoByDateId(exDateTime).EndTime >= startView);
			if (exDateTime == ExDateTime.MinValue)
			{
				exDateTime = this.GetNextOccurrenceDateId(exDateTime);
			}
			List<OccurrenceInfo> list = new List<OccurrenceInfo>();
			while (exDateTime != ExDateTime.MaxValue && list.Count <= maxCount)
			{
				OccurrenceInfo occurrenceInfoByDateId = this.GetOccurrenceInfoByDateId(exDateTime);
				if ((occurrenceInfoByDateId.StartTime < endView && occurrenceInfoByDateId.EndTime >= startView) || occurrenceInfoByDateId.StartTime == startView)
				{
					list.Add(occurrenceInfoByDateId);
				}
				else if (occurrenceInfoByDateId.StartTime >= endView)
				{
					break;
				}
				exDateTime = this.GetNextOccurrenceDateId(exDateTime);
			}
			return list;
		}

		public virtual ExDateTime[] GetDeletedOccurrences(bool convertToReadTimeZone)
		{
			return Recurrence.emptyDateTimeArray;
		}

		public virtual ExDateTime[] GetDeletedOccurrences()
		{
			return Recurrence.emptyDateTimeArray;
		}

		public virtual IList<OccurrenceInfo> GetModifiedOccurrences()
		{
			return Recurrence.emptyOccurrenceInfoArray;
		}

		public override bool Equals(object value)
		{
			Recurrence recurrence = value as Recurrence;
			return recurrence != null && this.Pattern.Equals(recurrence.Pattern) && this.Range.Equals(recurrence.Range);
		}

		public override int GetHashCode()
		{
			return this.Pattern.GetHashCode() + this.Range.GetHashCode();
		}

		public RecurrenceInfo GetRecurrenceInfo()
		{
			return RecurrenceInfo.GetInfo(this);
		}

		public bool IsValidOccurrenceId(ExDateTime occurrenceId)
		{
			return !(occurrenceId.Date != occurrenceId) && !(occurrenceId > this.EndDate) && !(occurrenceId < this.Range.StartDate) && !(this.GetNthOccurrence(occurrenceId, 1) != occurrenceId);
		}

		public virtual bool IsOccurrenceDeleted(ExDateTime occurrenceId)
		{
			throw new NotImplementedException();
		}

		internal static int GetNumberOfYearsBetween(ExDateTime firstDate, ExDateTime secondDate, ExCalendar calendar)
		{
			if (firstDate > secondDate)
			{
				throw new ArgumentException("firstDate", "firstDate should be less then second date");
			}
			int year = calendar.GetYear(firstDate);
			int era = calendar.GetEra(firstDate);
			int year2 = calendar.GetYear(secondDate);
			int era2 = calendar.GetEra(secondDate);
			int num = secondDate.Year - firstDate.Year;
			int num2 = num - 1;
			int num3 = num * 13 / 12;
			if (era == era2)
			{
				return year2 - year;
			}
			int result;
			for (;;)
			{
				try
				{
					ExDateTime time = calendar.AddYears(firstDate, num);
					int year3 = calendar.GetYear(time);
					int era3 = calendar.GetEra(time);
					if (era3 < era2)
					{
						num2 = num;
					}
					else
					{
						if (era3 <= era2)
						{
							num += year2 - year3;
							result = num;
							break;
						}
						num3 = num;
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					num3 = num - 1;
				}
				num = (num2 + num3 + 1) / 2;
			}
			return result;
		}

		internal static int GetNumberOfMonthsBetween(ExDateTime firstDate, ExDateTime secondDate, ExCalendar calendar)
		{
			if (firstDate > secondDate)
			{
				string message = string.Format("First date {0} should be less than the second date {1}", firstDate, secondDate);
				throw new ArgumentException(message, "firstDate");
			}
			int era = calendar.GetEra(firstDate);
			int year = calendar.GetYear(firstDate);
			int month = calendar.GetMonth(firstDate);
			int era2 = calendar.GetEra(secondDate);
			int year2 = calendar.GetYear(secondDate);
			int month2 = calendar.GetMonth(secondDate);
			int numberOfYearsBetween = Recurrence.GetNumberOfYearsBetween(firstDate, secondDate, calendar);
			int num = numberOfYearsBetween * 12 + month2 - month;
			int num2 = numberOfYearsBetween * 13 + month2 - month;
			int num3 = num;
			if (era == era2 && year == year2)
			{
				return month2 - month;
			}
			int result;
			for (;;)
			{
				try
				{
					ExDateTime time = calendar.AddMonths(firstDate, num3);
					int era3 = calendar.GetEra(time);
					int year3 = calendar.GetYear(time);
					int month3 = calendar.GetMonth(time);
					if (era3 < era2 || (era3 == era2 && year3 < year2))
					{
						num = num3;
					}
					else
					{
						if (era3 <= era2 && (era3 != era2 || year3 <= year2))
						{
							num3 += month2 - month3;
							result = num3;
							break;
						}
						num2 = num3;
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					num2 = num3 - 1;
				}
				num3 = (num + num2 + 1) / 2;
			}
			return result;
		}

		internal static bool IsGregorianCompatible(CalendarType calendarType)
		{
			switch (calendarType)
			{
			case CalendarType.Default:
			case CalendarType.Gregorian:
			case CalendarType.Gregorian_us:
			case CalendarType.Japan:
			case CalendarType.Taiwan:
			case CalendarType.Korea:
			case CalendarType.Thai:
			case CalendarType.GregorianMeFrench:
			case CalendarType.GregorianArabic:
			case CalendarType.GregorianXlitEnglish:
			case CalendarType.GregorianXlitFrench:
				return true;
			}
			return false;
		}

		internal static ExCalendar GetCalendar(CalendarType calendarType)
		{
			if (Recurrence.IsGregorianCompatible(calendarType))
			{
				return Recurrence.ExGregorianCalendar;
			}
			if (calendarType <= CalendarType.ChineseLunar)
			{
				switch (calendarType)
				{
				case CalendarType.Hijri:
					return Recurrence.InternalGetHijriCalendar();
				case CalendarType.Thai:
					break;
				case CalendarType.Hebrew:
					return Recurrence.ExHebrewCalendar;
				default:
					switch (calendarType)
					{
					case CalendarType.JapanLunar:
						return Recurrence.ExJapaneseLunarCalendar;
					case CalendarType.ChineseLunar:
						return Recurrence.ExChineseLunarCalendar;
					}
					break;
				}
			}
			else
			{
				if (calendarType == CalendarType.KoreaLunar)
				{
					return Recurrence.ExKoreanLunarCalendar;
				}
				if (calendarType != CalendarType.Umalqura)
				{
				}
			}
			throw new NotSupportedException("CalendarType not supported");
		}

		internal static LocalizedString GetCalendarName(CalendarType calendarType)
		{
			switch (calendarType)
			{
			case CalendarType.Hijri:
				return ClientStrings.Hijri;
			case CalendarType.Thai:
				break;
			case CalendarType.Hebrew:
				return ClientStrings.HebrewLunar;
			default:
				switch (calendarType)
				{
				case CalendarType.JapanLunar:
					return ClientStrings.JapaneseLunar;
				case CalendarType.ChineseLunar:
					return ClientStrings.ChineseLunar;
				default:
					if (calendarType == CalendarType.KoreaLunar)
					{
						return ClientStrings.KoreanLunar;
					}
					break;
				}
				break;
			}
			ExDiagnostics.FailFast("GetCalendarName should not have been called for this case", false);
			throw new InvalidOperationException();
		}

		internal static RecurrencePattern TransformRecurrenceToRegenerating(RecurrencePattern pattern)
		{
			if (pattern is DailyRecurrencePattern)
			{
				return new DailyRegeneratingPattern(((IntervalRecurrencePattern)pattern).RecurrenceInterval);
			}
			if (pattern is WeeklyRecurrencePattern)
			{
				return new WeeklyRegeneratingPattern(((IntervalRecurrencePattern)pattern).RecurrenceInterval);
			}
			if (pattern is MonthlyRecurrencePattern || pattern is MonthlyThRecurrencePattern)
			{
				return new MonthlyRegeneratingPattern(((IntervalRecurrencePattern)pattern).RecurrenceInterval, ((IMonthlyPatternInfo)pattern).CalendarType);
			}
			if (pattern is YearlyRecurrencePattern)
			{
				YearlyRecurrencePattern yearlyRecurrencePattern = (YearlyRecurrencePattern)pattern;
				return new YearlyRegeneratingPattern(yearlyRecurrencePattern.Months / 12);
			}
			throw new RecurrenceFormatException(ServerStrings.ExUnknownPattern(pattern), null);
		}

		internal virtual LocalizedString GenerateWhen(bool addTimeZoneInfo)
		{
			LocalizedString localizedString = new LocalizedString(string.Empty);
			double totalMinutes = this.EndOffset.TotalMinutes;
			TimeSpan t = this.EndOffset - this.StartOffset;
			double totalHours = t.TotalHours;
			int minutes = t.Minutes;
			if (this.startOffset < Recurrence.ZeroTimeSpan || this.endOffset < Recurrence.ZeroTimeSpan)
			{
				return LocalizedString.Empty;
			}
			ExDateTime exDateTime = new ExDateTime(this.CreatedExTimeZone, new DateTime(this.startOffset.Ticks + 1728000000000L));
			if (!(this.Range is NoEndRecurrenceRange))
			{
				if (exDateTime.TimeOfDay.TotalMinutes == 0.0 && (t.Ticks == 864000000000L || t.Ticks == 0L))
				{
					localizedString = ClientStrings.WhenRecurringWithEndDateNoTimeInDay(this.pattern.When(), this.Range.StartDate, this.EndDate);
				}
				else if (t.Days == 0)
				{
					if (t.Hours == 0 && t.Minutes == 0)
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateNoDuration(this.pattern.When(), this.Range.StartDate, this.EndDate, exDateTime);
					}
					else
					{
						localizedString = ClientStrings.WhenRecurringWithEndDate(this.pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, exDateTime + t);
					}
				}
				else if (t.Days == 1)
				{
					if (t.Minutes == 0)
					{
						if (t.Hours == 0)
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateOneDayNoDuration(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime);
						}
						else if (t.Hours == 1)
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateOneDayOneHour(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime);
						}
						else
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateOneDayHours(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Hours);
						}
					}
					else if (t.Hours == 0)
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateOneDayMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Minutes);
					}
					else if (t.Hours == 1)
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateOneDayOneHourMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Minutes);
					}
					else
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateOneDayHoursMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Hours, t.Minutes);
					}
				}
				else if (t.Days > 1)
				{
					if (t.Minutes == 0)
					{
						if (t.Hours == 0)
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateDaysNoDuration(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days);
						}
						else if (t.Hours == 1)
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateDaysOneHour(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days);
						}
						else
						{
							localizedString = ClientStrings.WhenRecurringWithEndDateDaysHours(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days, t.Hours);
						}
					}
					else if (t.Hours == 0)
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateDaysMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days, t.Minutes);
					}
					else if (t.Hours == 1)
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateDaysOneHourMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days, t.Minutes);
					}
					else
					{
						localizedString = ClientStrings.WhenRecurringWithEndDateDaysHoursMinutes(this.Pattern.When(), this.Range.StartDate, this.EndDate, exDateTime, t.Days, t.Hours, t.Minutes);
					}
				}
			}
			else if (exDateTime.TimeOfDay.TotalMinutes == 0.0 && (t.Ticks == 864000000000L || t.Ticks == 0L))
			{
				localizedString = ClientStrings.WhenRecurringNoEndDateNoTimeInDay(this.Pattern.When(), this.Range.StartDate);
			}
			else if (t.Days == 0)
			{
				if (t.Hours == 0 && t.Minutes == 0)
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateNoDuration(this.pattern.When(), this.Range.StartDate, exDateTime);
				}
				else
				{
					localizedString = ClientStrings.WhenRecurringNoEndDate(this.pattern.When(), this.Range.StartDate, exDateTime, exDateTime + t);
				}
			}
			else if (t.Days == 1)
			{
				if (t.Minutes == 0)
				{
					if (t.Hours == 0)
					{
						localizedString = ClientStrings.WhenRecurringNoEndDateOneDayNoDuration(this.Pattern.When(), this.Range.StartDate, exDateTime);
					}
					else if (t.Hours == 1)
					{
						localizedString = ClientStrings.WhenRecurringNoEndDateOneDayOneHour(this.Pattern.When(), this.Range.StartDate, exDateTime);
					}
					else
					{
						localizedString = ClientStrings.WhenRecurringNoEndDateOneDayHours(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Hours);
					}
				}
				else if (t.Hours == 0)
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateOneDayMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Minutes);
				}
				else if (t.Hours == 1)
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateOneDayOneHourMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Minutes);
				}
				else
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateOneDayHoursMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Hours, t.Minutes);
				}
			}
			else if (t.Minutes == 0)
			{
				if (t.Hours == 0)
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateDaysNoDuration(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days);
				}
				else if (t.Hours == 1)
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateDaysOneHour(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days);
				}
				else
				{
					localizedString = ClientStrings.WhenRecurringNoEndDateDaysHours(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days, t.Hours);
				}
			}
			else if (t.Hours == 0)
			{
				localizedString = ClientStrings.WhenRecurringNoEndDateDaysMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days, t.Minutes);
			}
			else if (t.Hours == 1)
			{
				localizedString = ClientStrings.WhenRecurringNoEndDateDaysOneHourMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days, t.Minutes);
			}
			else
			{
				localizedString = ClientStrings.WhenRecurringNoEndDateDaysHoursMinutes(this.Pattern.When(), this.Range.StartDate, exDateTime, t.Days, t.Hours, t.Minutes);
			}
			if (this.Range is NumberedRecurrenceRange)
			{
				ExDateTime now = ExDateTime.GetNow(this.CreatedExTimeZone);
				int num;
				if (now.Date < this.range.StartDate || (now.Date == this.range.StartDate && now.TimeOfDay < this.StartOffset))
				{
					num = 0;
				}
				else
				{
					num = this.GetNumberOfOccurrencesSinceStart((now.TimeOfDay < this.StartOffset) ? now.Date.IncrementDays(-1) : now.Date);
				}
				int num2 = ((NumberedRecurrenceRange)this.Range).NumberOfOccurrences - num;
				if (num2 > 0)
				{
					LocalizedString str = (num2 == 1) ? ClientStrings.WhenOneMoreOccurrence : ClientStrings.WhenNMoreOccurrences(num2);
					localizedString = ClientStrings.JointStrings(localizedString, str);
				}
			}
			bool flag = object.Equals(this.CreatedExTimeZone, this.ReadExTimeZone);
			if (!flag && this.CreatedExTimeZone != null && this.ReadExTimeZone != null)
			{
				string text = this.CreatedExTimeZone.LocalizableDisplayName.ToString();
				string value = this.ReadExTimeZone.LocalizableDisplayName.ToString();
				flag = text.Equals(value, StringComparison.Ordinal);
			}
			if (addTimeZoneInfo || !flag)
			{
				localizedString = ClientStrings.JointStrings(localizedString, this.CreatedExTimeZone.LocalizableDisplayName);
			}
			return localizedString;
		}

		internal virtual OccurrenceInfo GetOccurrenceInfoByDateId(ExDateTime occurrenceId)
		{
			ExDateTime exDateTime;
			ExDateTime endTime;
			this.ComputeStartAndEndForInstance(occurrenceId, out exDateTime, out endTime);
			return new OccurrenceInfo(null, occurrenceId, exDateTime, exDateTime, endTime);
		}

		internal TimeSpan MinTimeBetweenTwoOccurrences()
		{
			if (this.Pattern is DailyRecurrencePattern)
			{
				return TimeSpan.FromDays((double)((DailyRecurrencePattern)this.Pattern).RecurrenceInterval);
			}
			if (this.Pattern is WeeklyRecurrencePattern)
			{
				DaysOfWeek daysOfWeek = ((WeeklyRecurrencePattern)this.Pattern).DaysOfWeek;
				DayOfWeek firstDayOfWeek = ((WeeklyRecurrencePattern)this.Pattern).FirstDayOfWeek;
				if (Recurrence.OccurrencesPerWeek(daysOfWeek) > 1)
				{
					int num = int.MaxValue;
					int num2 = (int)((((WeeklyRecurrencePattern)this.Pattern).RecurrenceInterval > 1) ? (firstDayOfWeek + 7) : (firstDayOfWeek + 14));
					int i = (int)firstDayOfWeek;
					int num3 = int.MinValue;
					while (i < num2)
					{
						if ((1 << i % 7 & (int)daysOfWeek) != 0)
						{
							if (num3 == -2147483648)
							{
								num3 = i;
							}
							else
							{
								num = Math.Min(i - num3, num);
								num3 = i;
							}
						}
						i++;
					}
					return TimeSpan.FromDays((double)num);
				}
				return TimeSpan.FromDays((double)(((WeeklyRecurrencePattern)this.Pattern).RecurrenceInterval * 7));
			}
			else
			{
				if (this.Pattern is MonthlyRecurrencePattern || this.Pattern is MonthlyThRecurrencePattern)
				{
					return TimeSpan.FromDays((double)(((IntervalRecurrencePattern)this.Pattern).RecurrenceInterval * 28));
				}
				return TimeSpan.FromDays(365.0);
			}
		}

		internal bool HasFutureOccurrences(ExDateTime now)
		{
			return this.Range is NoEndRecurrenceRange || this.GetLastOccurrence().StartTime > now;
		}

		internal bool HasPastOccurrences(ExDateTime now)
		{
			return this.GetFirstOccurrence().StartTime <= now;
		}

		protected virtual ExDateTime GetNextOccurrenceDateId(ExDateTime date)
		{
			date = date.Date;
			if (date >= this.EndDate)
			{
				return ExDateTime.MaxValue;
			}
			if (date < this.Range.StartDate)
			{
				date = this.Range.StartDate;
			}
			else if (date < this.maxSupportedDate)
			{
				date = date.IncrementDays(1);
			}
			ExDateTime exDateTime = this.GetNthOccurrence(date, 1);
			if (exDateTime > this.endDate)
			{
				exDateTime = ExDateTime.MaxValue;
			}
			return exDateTime;
		}

		protected virtual ExDateTime GetPreviousOccurrenceDateId(ExDateTime date)
		{
			date = date.Date;
			if (date <= this.Range.StartDate)
			{
				return ExDateTime.MinValue;
			}
			if (date > this.EndDate)
			{
				date = this.EndDate;
			}
			else
			{
				date = date.IncrementDays(-1);
			}
			int numberOfOccurrencesSinceStart = this.GetNumberOfOccurrencesSinceStart(date);
			if (numberOfOccurrencesSinceStart < 1)
			{
				return ExDateTime.MinValue;
			}
			ExDateTime exDateTime = this.GetNthOccurrence(this.Range.StartDate, numberOfOccurrencesSinceStart);
			if (exDateTime < this.Range.StartDate)
			{
				exDateTime = ExDateTime.MinValue;
			}
			return exDateTime;
		}

		protected ExDateTime GetNthOccurrence(ExDateTime date, int nthOcc)
		{
			try
			{
				if (this.Pattern is RegeneratingPattern)
				{
					int num = nthOcc - 1;
					int recurrenceInterval = ((IntervalRecurrencePattern)this.Pattern).RecurrenceInterval;
					ExDateTime exDateTime;
					if (this.Pattern is DailyRegeneratingPattern)
					{
						exDateTime = date.IncrementDays(num * recurrenceInterval);
					}
					else if (this.Pattern is WeeklyRegeneratingPattern)
					{
						exDateTime = date.IncrementDays(num * 7 * recurrenceInterval);
					}
					else if (this.Pattern is MonthlyRegeneratingPattern)
					{
						ExCalendar calendar = Recurrence.GetCalendar(((MonthlyRegeneratingPattern)this.Pattern).CalendarType);
						exDateTime = calendar.AddMonths(date, num * recurrenceInterval);
					}
					else
					{
						if (!(this.Pattern is YearlyRegeneratingPattern))
						{
							throw new NotSupportedException();
						}
						ExCalendar calendar2 = Recurrence.GetCalendar(((YearlyRegeneratingPattern)this.Pattern).CalendarType);
						exDateTime = calendar2.AddYears(date, num * recurrenceInterval);
					}
					return exDateTime.Date;
				}
				if (this.Pattern is DailyRecurrencePattern)
				{
					int days = (((DateTime)date).AddDays(-1.0) - ((DateTime)this.Range.StartDate).Date).Days;
					int recurrenceInterval2 = ((DailyRecurrencePattern)this.Pattern).RecurrenceInterval;
					int num2 = Convert.ToInt32(Math.Floor((double)days / (double)recurrenceInterval2));
					return this.Range.StartDate.IncrementDays(recurrenceInterval2 * (nthOcc + num2)).Date;
				}
				if (this.Pattern is WeeklyRecurrencePattern)
				{
					WeeklyRecurrencePattern weeklyRecurrencePattern = this.Pattern as WeeklyRecurrencePattern;
					TimeSpan t = (DateTime)date - (DateTime)this.Range.StartDate;
					int num3 = this.Range.StartDate.DayOfWeek - weeklyRecurrencePattern.FirstDayOfWeek;
					if (num3 < 0)
					{
						num3 += 7;
					}
					if (num3 != 0)
					{
						t += TimeSpan.FromDays((double)num3);
					}
					if (t.Days % (7 * weeklyRecurrencePattern.RecurrenceInterval) < 7)
					{
						int num4 = Recurrence.OccurrencesBetween(date.DayOfWeek, weeklyRecurrencePattern.FirstDayOfWeek, weeklyRecurrencePattern.DaysOfWeek);
						if (num4 >= nthOcc)
						{
							return Recurrence.GetOccurrenceInCurrentWeek(date, weeklyRecurrencePattern.FirstDayOfWeek, weeklyRecurrencePattern.DaysOfWeek, nthOcc);
						}
						nthOcc -= num4;
					}
					int num5 = Recurrence.OccurrencesPerWeek(weeklyRecurrencePattern.DaysOfWeek);
					int num6 = 1 + nthOcc / num5;
					int num7 = nthOcc % num5;
					num6 = ((num7 == 0) ? (num6 - 1) : num6);
					num7 = ((num7 == 0) ? num5 : num7);
					ExDateTime curDate = this.Range.StartDate.IncrementDays(7 * weeklyRecurrencePattern.RecurrenceInterval * (num6 + t.Days / (7 * weeklyRecurrencePattern.RecurrenceInterval)) - num3);
					return Recurrence.GetOccurrenceInCurrentWeek(curDate, weeklyRecurrencePattern.FirstDayOfWeek, weeklyRecurrencePattern.DaysOfWeek, num7);
				}
				if (this.Pattern is MonthlyRecurrencePattern || this.Pattern is MonthlyThRecurrencePattern)
				{
					MonthlyRecurrencePattern monthlyRecurrencePattern = this.Pattern as MonthlyRecurrencePattern;
					MonthlyThRecurrencePattern monthlyThRecurrencePattern = this.Pattern as MonthlyThRecurrencePattern;
					int recurrenceInterval3 = ((IntervalRecurrencePattern)this.Pattern).RecurrenceInterval;
					ExCalendar calendar3 = Recurrence.GetCalendar((monthlyRecurrencePattern != null) ? monthlyRecurrencePattern.CalendarType : monthlyThRecurrencePattern.CalendarType);
					int numberOfMonthsBetween = Recurrence.GetNumberOfMonthsBetween(this.Range.StartDate, date, calendar3);
					if (numberOfMonthsBetween % recurrenceInterval3 == 0 && ((monthlyRecurrencePattern != null && calendar3.GetDayOfMonth(date) <= monthlyRecurrencePattern.DayOfMonth) || (monthlyThRecurrencePattern != null && Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(date, monthlyThRecurrencePattern.DaysOfWeek, monthlyThRecurrencePattern.Order, calendar3) >= date)))
					{
						nthOcc--;
					}
					int months = recurrenceInterval3 * (nthOcc + numberOfMonthsBetween / recurrenceInterval3);
					ExDateTime date2 = calendar3.AddMonths(this.Range.StartDate, months);
					if (monthlyRecurrencePattern != null)
					{
						return Recurrence.AdjustToDayOfMonthInCurrentMonth(date2, monthlyRecurrencePattern.DayOfMonth, calendar3);
					}
					return Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(date2, monthlyThRecurrencePattern.DaysOfWeek, monthlyThRecurrencePattern.Order, calendar3);
				}
				else if (this.Pattern is YearlyThRecurrencePattern || this.Pattern is YearlyRecurrencePattern)
				{
					YearlyThRecurrencePattern yearlyThRecurrencePattern = this.Pattern as YearlyThRecurrencePattern;
					YearlyRecurrencePattern yearlyRecurrencePattern = this.Pattern as YearlyRecurrencePattern;
					IYearlyPatternInfo yearlyPatternInfo = this.Pattern as IYearlyPatternInfo;
					bool flag = (yearlyRecurrencePattern == null) ? yearlyThRecurrencePattern.IsLeapMonth : yearlyRecurrencePattern.IsLeapMonth;
					int num8 = (yearlyRecurrencePattern == null) ? yearlyThRecurrencePattern.Month : yearlyRecurrencePattern.Month;
					ExCalendar calendar4 = Recurrence.GetCalendar((yearlyRecurrencePattern != null) ? yearlyRecurrencePattern.CalendarType : yearlyThRecurrencePattern.CalendarType);
					int month = calendar4.GetMonth(date);
					int num9 = calendar4.GetLeapMonth(calendar4.GetYear(date), calendar4.GetEra(date));
					num9 = ((num9 == 0) ? int.MaxValue : num9);
					int num10 = num8;
					if (num9 <= num10 || (flag && num9 == num10 + 1))
					{
						num10++;
					}
					bool flag2 = false;
					if (month > num10)
					{
						flag2 = true;
					}
					else if (month == num10)
					{
						if (yearlyRecurrencePattern != null && calendar4.GetDayOfMonth(date) > yearlyRecurrencePattern.DayOfMonth)
						{
							flag2 = true;
						}
						else if (yearlyThRecurrencePattern != null && Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(date, yearlyThRecurrencePattern.DaysOfWeek, yearlyThRecurrencePattern.Order, calendar4) < date)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						nthOcc++;
					}
					date = calendar4.AddMonths(date, 1 - month);
					int recurrenceInterval4 = (this.Pattern as IntervalRecurrencePattern).RecurrenceInterval;
					date = calendar4.AddYears(date, (nthOcc - 1) * recurrenceInterval4);
					month = calendar4.GetMonth(date);
					num9 = calendar4.GetLeapMonth(calendar4.GetYear(date), calendar4.GetEra(date));
					num9 = ((num9 == 0) ? int.MaxValue : num9);
					num10 = yearlyPatternInfo.Month;
					if (num9 <= num10 || (flag && num9 == num10 + 1))
					{
						num10++;
					}
					if (yearlyThRecurrencePattern != null)
					{
						return Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(calendar4.AddMonths(date, num10 - calendar4.GetMonth(date)), yearlyThRecurrencePattern.DaysOfWeek, yearlyThRecurrencePattern.Order, calendar4);
					}
					return Recurrence.AdjustToDayOfMonthInCurrentMonth(calendar4.AddMonths(date, num10 - calendar4.GetMonth(date)), yearlyRecurrencePattern.DayOfMonth, calendar4);
				}
			}
			catch (ArgumentOutOfRangeException)
			{
			}
			return ExDateTime.MaxValue;
		}

		protected void ComputeStartAndEndForInstance(ExDateTime occurrenceId, out ExDateTime startTime, out ExDateTime endTime)
		{
			occurrenceId.CheckExpectedTimeZone(this.CreatedExTimeZone);
			startTime = new ExDateTime(this.CreatedExTimeZone, occurrenceId.LocalTime.Date.Add(this.StartOffset));
			startTime = this.ReadExTimeZone.ConvertDateTime(startTime);
			endTime = new ExDateTime(this.CreatedExTimeZone, occurrenceId.LocalTime.Date.Add(this.EndOffset));
			endTime = this.ReadExTimeZone.ConvertDateTime(endTime);
			if (startTime > endTime)
			{
				endTime = startTime;
			}
		}

		protected int GetNumberOfOccurrencesSinceStart(ExDateTime currentDate)
		{
			TimeSpan t = (DateTime)currentDate - (DateTime)this.Range.StartDate;
			int num = 0;
			if (this.Pattern is DailyRecurrencePattern)
			{
				int recurrenceInterval = ((DailyRecurrencePattern)this.Pattern).RecurrenceInterval;
				num += 1 + t.Days / recurrenceInterval;
			}
			else if (this.Pattern is WeeklyRecurrencePattern)
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = this.Pattern as WeeklyRecurrencePattern;
				int num2 = this.Range.StartDate.DayOfWeek - weeklyRecurrencePattern.FirstDayOfWeek;
				if (num2 < 0)
				{
					num2 += 7;
				}
				if (num2 != 0)
				{
					t += TimeSpan.FromDays((double)num2);
					num -= Recurrence.OccurrencesBetween(weeklyRecurrencePattern.FirstDayOfWeek, this.Range.StartDate.DayOfWeek, weeklyRecurrencePattern.DaysOfWeek);
				}
				num += Recurrence.OccurrencesPerWeek(weeklyRecurrencePattern.DaysOfWeek) * (t.Days / (7 * weeklyRecurrencePattern.RecurrenceInterval));
				if (t.Days % (7 * weeklyRecurrencePattern.RecurrenceInterval) < 7)
				{
					if (currentDate.DayOfWeek != weeklyRecurrencePattern.FirstDayOfWeek)
					{
						num += Recurrence.OccurrencesBetween(weeklyRecurrencePattern.FirstDayOfWeek, currentDate.DayOfWeek, weeklyRecurrencePattern.DaysOfWeek);
					}
					if ((1 << (int)currentDate.DayOfWeek & (int)weeklyRecurrencePattern.DaysOfWeek) != 0)
					{
						num++;
					}
				}
				else
				{
					num += Recurrence.OccurrencesPerWeek(weeklyRecurrencePattern.DaysOfWeek);
				}
			}
			else if (this.Pattern is MonthlyRecurrencePattern || this.Pattern is MonthlyThRecurrencePattern)
			{
				MonthlyRecurrencePattern monthlyRecurrencePattern = this.Pattern as MonthlyRecurrencePattern;
				MonthlyThRecurrencePattern monthlyThRecurrencePattern = this.Pattern as MonthlyThRecurrencePattern;
				int num3 = (monthlyRecurrencePattern != null) ? monthlyRecurrencePattern.RecurrenceInterval : monthlyThRecurrencePattern.RecurrenceInterval;
				ExCalendar calendar = Recurrence.GetCalendar((monthlyRecurrencePattern != null) ? monthlyRecurrencePattern.CalendarType : monthlyThRecurrencePattern.CalendarType);
				int numberOfMonthsBetween = Recurrence.GetNumberOfMonthsBetween(this.Range.StartDate, currentDate, calendar);
				if (numberOfMonthsBetween % num3 == 0)
				{
					if ((monthlyRecurrencePattern != null && calendar.GetDayOfMonth(currentDate) >= Math.Min(monthlyRecurrencePattern.DayOfMonth, calendar.GetDaysInMonth(calendar.GetYear(currentDate), calendar.GetMonth(currentDate), calendar.GetEra(currentDate)))) || (monthlyThRecurrencePattern != null && Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(currentDate, monthlyThRecurrencePattern.DaysOfWeek, monthlyThRecurrencePattern.Order, calendar) <= currentDate))
					{
						num++;
					}
				}
				else
				{
					num++;
				}
				num += numberOfMonthsBetween / num3;
			}
			else if (this.Pattern is YearlyThRecurrencePattern || this.Pattern is YearlyRecurrencePattern)
			{
				YearlyThRecurrencePattern yearlyThRecurrencePattern = this.Pattern as YearlyThRecurrencePattern;
				YearlyRecurrencePattern yearlyRecurrencePattern = this.Pattern as YearlyRecurrencePattern;
				bool flag = (yearlyRecurrencePattern == null) ? yearlyThRecurrencePattern.IsLeapMonth : yearlyRecurrencePattern.IsLeapMonth;
				int num4 = (yearlyRecurrencePattern == null) ? yearlyThRecurrencePattern.Month : yearlyRecurrencePattern.Month;
				ExCalendar calendar2 = Recurrence.GetCalendar((yearlyRecurrencePattern == null) ? yearlyThRecurrencePattern.CalendarType : yearlyRecurrencePattern.CalendarType);
				int month = calendar2.GetMonth(currentDate);
				int num5 = calendar2.GetLeapMonth(calendar2.GetYear(currentDate), calendar2.GetEra(currentDate));
				num5 = ((num5 == 0) ? int.MaxValue : num5);
				int num6 = num4;
				if (num5 <= num6 || (flag && num5 == num6 + 1))
				{
					num6++;
				}
				bool flag2 = false;
				if (month > num6)
				{
					flag2 = true;
				}
				else if (month == num6)
				{
					if (yearlyRecurrencePattern != null)
					{
						int num7 = calendar2.GetDaysInMonth(calendar2.GetYear(currentDate), month);
						if (yearlyRecurrencePattern.DayOfMonth < num7)
						{
							num7 = yearlyRecurrencePattern.DayOfMonth;
						}
						if (calendar2.GetDayOfMonth(currentDate) >= num7)
						{
							flag2 = true;
						}
					}
					else if (yearlyThRecurrencePattern != null && Recurrence.AdjustToDayOfWeekWithOrderInCurrentMonth(currentDate, yearlyThRecurrencePattern.DaysOfWeek, yearlyThRecurrencePattern.Order, calendar2) <= currentDate)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					num++;
				}
				int recurrenceInterval2 = (this.Pattern as IntervalRecurrencePattern).RecurrenceInterval;
				num += Recurrence.GetNumberOfYearsBetween(this.Range.StartDate, currentDate, calendar2) / recurrenceInterval2;
			}
			return num;
		}

		private static ExDateTime GetOccurrenceInCurrentWeek(ExDateTime curDate, DayOfWeek firstDayOfWeek, DaysOfWeek mask, int nthOcc)
		{
			return Recurrence.GetOccurrenceInCurrentWeek(curDate, firstDayOfWeek, mask, nthOcc, 1);
		}

		private static ExDateTime GetOccurrenceInCurrentWeek(ExDateTime curDate, DayOfWeek endDayOfWeek, DaysOfWeek mask, int nthOcc, int step)
		{
			while ((1 << (int)curDate.DayOfWeek & (int)mask) == 0 || nthOcc-- != 1)
			{
				curDate = curDate.IncrementDays(step);
				if (curDate.DayOfWeek == endDayOfWeek)
				{
					return ExDateTime.MaxValue;
				}
			}
			return curDate.Date;
		}

		private static int OccurrencesPerWeek(DaysOfWeek daysOfWeek)
		{
			return Recurrence.OccurrencesBetween(DayOfWeek.Sunday, DayOfWeek.Sunday, daysOfWeek);
		}

		private static int OccurrencesBetween(DayOfWeek curDay, DayOfWeek firstDay, DaysOfWeek mask)
		{
			int num = 0;
			do
			{
				if ((1 << (int)curDay & (int)mask) != 0)
				{
					num++;
				}
				curDay++;
				curDay %= (DayOfWeek)7;
			}
			while (firstDay != curDay);
			return num;
		}

		private static ExDateTime AdjustToDayOfMonthInCurrentMonth(ExDateTime date, int dayOfMonth, ExCalendar calendar)
		{
			ExDateTime time = calendar.AddDays(date, dayOfMonth - calendar.GetDayOfMonth(date));
			if (calendar.GetMonth(time) != calendar.GetMonth(date))
			{
				time = calendar.AddDays(time, -calendar.GetDayOfMonth(time));
			}
			return time.Date;
		}

		private static ExDateTime AdjustToDayOfWeekWithOrderInCurrentMonth(ExDateTime date, DaysOfWeek daysOfWeek, RecurrenceOrderType order, ExCalendar calendar)
		{
			date = calendar.AddDays(date, 1 - calendar.GetDayOfMonth(date));
			if (order != RecurrenceOrderType.Last)
			{
				int num = Recurrence.OccurrencesPerWeek(daysOfWeek);
				int num2 = (order - RecurrenceOrderType.First) / num;
				date = calendar.AddDays(date, 7 * num2);
				int nthOcc = order - (RecurrenceOrderType)(num2 * num);
				return Recurrence.GetOccurrenceInCurrentWeek(date, date.DayOfWeek, daysOfWeek, nthOcc);
			}
			if (calendar.AddMonths(calendar.MaxSupportedDateTime.Date, -1) > date)
			{
				date = calendar.AddDays(calendar.AddMonths(date, 1), -1);
			}
			else
			{
				date = calendar.MaxSupportedDateTime.Date;
			}
			return Recurrence.GetOccurrenceInCurrentWeek(date, date.DayOfWeek, daysOfWeek, 1, -1);
		}

		private OccurrenceInfo InternalGetLastOccurrence()
		{
			if (this.endDate >= this.maxSupportedDate)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "Recurrence::InternalGetLastOccurrence called for NoEndDateRecurrenceRange recurrence");
				throw new InvalidOperationException("InternalGetLastOccurrence called for NoEndDateRecurrenceRange recurrence");
			}
			ExDateTime previousOccurrenceDateId = this.GetPreviousOccurrenceDateId(this.endDate.IncrementDays(1));
			if (previousOccurrenceDateId == ExDateTime.MinValue)
			{
				ExTraceGlobals.RecurrenceTracer.TraceError((long)this.GetHashCode(), "Recurrence::InternalGetLastOccurrence, No occurrence found.");
				return null;
			}
			return this.GetOccurrenceInfoByDateId(previousOccurrenceDateId);
		}

		private static ExCalendar InternalGetHijriCalendar()
		{
			((HijriCalendar)Recurrence.ExHijiriCalendar.InnerCalendar).HijriAdjustment = -1;
			return Recurrence.ExHijiriCalendar;
		}

		public static readonly ExDateTime MinimumDateForRecurrenceStart = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 4, 1, 0, 0, 0);

		public static readonly ExDateTime MaximumDateForRecurrenceEnd = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 4500, 9, 1, 0, 0, 0);

		private static readonly OccurrenceInfo[] emptyOccurrenceInfoArray = Array<OccurrenceInfo>.Empty;

		private static readonly ExDateTime[] emptyDateTimeArray = Array<ExDateTime>.Empty;

		private static readonly TimeSpan ZeroTimeSpan = new TimeSpan(0L);

		private RecurrencePattern pattern;

		private RecurrenceRange range;

		private ExDateTime endDate;

		private int numberOfOccurrences;

		private ExTimeZone createExTimeZone;

		private ExTimeZone readExTimeZone;

		private bool hasTimeZone;

		private TimeSpan startOffset;

		private TimeSpan endOffset;

		private ExDateTime maxSupportedDate = Recurrence.MaximumDateForRecurrenceEnd;

		protected static readonly ExCalendar ExGregorianCalendar = new ExCalendar(new GregorianCalendar());

		protected static readonly ExCalendar ExKoreanLunarCalendar = new ExCalendar(new KoreanLunisolarCalendar());

		protected static readonly ExCalendar ExChineseLunarCalendar = new ExCalendar(new ChineseLunisolarCalendar());

		protected static readonly ExCalendar ExJapaneseLunarCalendar = new ExCalendar(new JapaneseLunisolarCalendar());

		protected static readonly ExCalendar ExHebrewCalendar = new ExCalendar(new HebrewCalendar());

		protected static readonly ExCalendar ExHijiriCalendar = new ExCalendar(new HijriCalendar());
	}
}
