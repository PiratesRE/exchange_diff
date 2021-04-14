using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class DateTimeFormatInfo : ICloneable, IFormatProvider
	{
		[SecuritySafeCritical]
		private static bool InitPreferExistingTokens()
		{
			return DateTime.LegacyParseMode();
		}

		private string CultureName
		{
			get
			{
				if (this.m_name == null)
				{
					this.m_name = this.m_cultureData.CultureName;
				}
				return this.m_name;
			}
		}

		private CultureInfo Culture
		{
			get
			{
				if (this.m_cultureInfo == null)
				{
					this.m_cultureInfo = CultureInfo.GetCultureInfo(this.CultureName);
				}
				return this.m_cultureInfo;
			}
		}

		private string LanguageName
		{
			[SecurityCritical]
			get
			{
				if (this.m_langName == null)
				{
					this.m_langName = this.m_cultureData.SISO639LANGNAME;
				}
				return this.m_langName;
			}
		}

		private string[] internalGetAbbreviatedDayOfWeekNames()
		{
			if (this.abbreviatedDayNames == null)
			{
				this.abbreviatedDayNames = this.m_cultureData.AbbreviatedDayNames(this.Calendar.ID);
			}
			return this.abbreviatedDayNames;
		}

		private string[] internalGetSuperShortDayNames()
		{
			if (this.m_superShortDayNames == null)
			{
				this.m_superShortDayNames = this.m_cultureData.SuperShortDayNames(this.Calendar.ID);
			}
			return this.m_superShortDayNames;
		}

		private string[] internalGetDayOfWeekNames()
		{
			if (this.dayNames == null)
			{
				this.dayNames = this.m_cultureData.DayNames(this.Calendar.ID);
			}
			return this.dayNames;
		}

		private string[] internalGetAbbreviatedMonthNames()
		{
			if (this.abbreviatedMonthNames == null)
			{
				this.abbreviatedMonthNames = this.m_cultureData.AbbreviatedMonthNames(this.Calendar.ID);
			}
			return this.abbreviatedMonthNames;
		}

		private string[] internalGetMonthNames()
		{
			if (this.monthNames == null)
			{
				this.monthNames = this.m_cultureData.MonthNames(this.Calendar.ID);
			}
			return this.monthNames;
		}

		[__DynamicallyInvokable]
		public DateTimeFormatInfo() : this(CultureInfo.InvariantCulture.m_cultureData, GregorianCalendar.GetDefaultInstance())
		{
		}

		internal DateTimeFormatInfo(CultureData cultureData, Calendar cal)
		{
			this.m_cultureData = cultureData;
			this.Calendar = cal;
		}

		[SecuritySafeCritical]
		private void InitializeOverridableProperties(CultureData cultureData, int calendarID)
		{
			if (this.firstDayOfWeek == -1)
			{
				this.firstDayOfWeek = cultureData.IFIRSTDAYOFWEEK;
			}
			if (this.calendarWeekRule == -1)
			{
				this.calendarWeekRule = cultureData.IFIRSTWEEKOFYEAR;
			}
			if (this.amDesignator == null)
			{
				this.amDesignator = cultureData.SAM1159;
			}
			if (this.pmDesignator == null)
			{
				this.pmDesignator = cultureData.SPM2359;
			}
			if (this.timeSeparator == null)
			{
				this.timeSeparator = cultureData.TimeSeparator;
			}
			if (this.dateSeparator == null)
			{
				this.dateSeparator = cultureData.DateSeparator(calendarID);
			}
			this.allLongTimePatterns = this.m_cultureData.LongTimes;
			this.allShortTimePatterns = this.m_cultureData.ShortTimes;
			this.allLongDatePatterns = cultureData.LongDates(calendarID);
			this.allShortDatePatterns = cultureData.ShortDates(calendarID);
			this.allYearMonthPatterns = cultureData.YearMonths(calendarID);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			if (this.m_name != null)
			{
				this.m_cultureData = CultureData.GetCultureData(this.m_name, this.m_useUserOverride);
				if (this.m_cultureData == null)
				{
					throw new CultureNotFoundException("m_name", this.m_name, Environment.GetResourceString("Argument_CultureNotSupported"));
				}
			}
			else
			{
				this.m_cultureData = CultureData.GetCultureData(this.CultureID, this.m_useUserOverride);
			}
			if (this.calendar == null)
			{
				this.calendar = (Calendar)GregorianCalendar.GetDefaultInstance().Clone();
				this.calendar.SetReadOnlyState(this.m_isReadOnly);
			}
			else
			{
				CultureInfo.CheckDomainSafetyObject(this.calendar, this);
			}
			this.InitializeOverridableProperties(this.m_cultureData, this.calendar.ID);
			bool isReadOnly = this.m_isReadOnly;
			this.m_isReadOnly = false;
			if (this.longDatePattern != null)
			{
				this.LongDatePattern = this.longDatePattern;
			}
			if (this.shortDatePattern != null)
			{
				this.ShortDatePattern = this.shortDatePattern;
			}
			if (this.yearMonthPattern != null)
			{
				this.YearMonthPattern = this.yearMonthPattern;
			}
			if (this.longTimePattern != null)
			{
				this.LongTimePattern = this.longTimePattern;
			}
			if (this.shortTimePattern != null)
			{
				this.ShortTimePattern = this.shortTimePattern;
			}
			this.m_isReadOnly = isReadOnly;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.CultureID = this.m_cultureData.ILANGUAGE;
			this.m_useUserOverride = this.m_cultureData.UseUserOverride;
			this.m_name = this.CultureName;
			if (DateTimeFormatInfo.s_calendarNativeNames == null)
			{
				DateTimeFormatInfo.s_calendarNativeNames = new Hashtable();
			}
			object obj = this.LongTimePattern;
			obj = this.LongDatePattern;
			obj = this.ShortTimePattern;
			obj = this.ShortDatePattern;
			obj = this.YearMonthPattern;
			obj = this.AllLongTimePatterns;
			obj = this.AllLongDatePatterns;
			obj = this.AllShortTimePatterns;
			obj = this.AllShortDatePatterns;
			obj = this.AllYearMonthPatterns;
		}

		[__DynamicallyInvokable]
		public static DateTimeFormatInfo InvariantInfo
		{
			[__DynamicallyInvokable]
			get
			{
				if (DateTimeFormatInfo.invariantInfo == null)
				{
					DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
					dateTimeFormatInfo.Calendar.SetReadOnlyState(true);
					dateTimeFormatInfo.m_isReadOnly = true;
					DateTimeFormatInfo.invariantInfo = dateTimeFormatInfo;
				}
				return DateTimeFormatInfo.invariantInfo;
			}
		}

		[__DynamicallyInvokable]
		public static DateTimeFormatInfo CurrentInfo
		{
			[__DynamicallyInvokable]
			get
			{
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				if (!currentCulture.m_isInherited)
				{
					DateTimeFormatInfo dateTimeInfo = currentCulture.dateTimeInfo;
					if (dateTimeInfo != null)
					{
						return dateTimeInfo;
					}
				}
				return (DateTimeFormatInfo)currentCulture.GetFormat(typeof(DateTimeFormatInfo));
			}
		}

		[__DynamicallyInvokable]
		public static DateTimeFormatInfo GetInstance(IFormatProvider provider)
		{
			CultureInfo cultureInfo = provider as CultureInfo;
			if (cultureInfo != null && !cultureInfo.m_isInherited)
			{
				return cultureInfo.DateTimeFormat;
			}
			DateTimeFormatInfo dateTimeFormatInfo = provider as DateTimeFormatInfo;
			if (dateTimeFormatInfo != null)
			{
				return dateTimeFormatInfo;
			}
			if (provider != null)
			{
				dateTimeFormatInfo = (provider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo);
				if (dateTimeFormatInfo != null)
				{
					return dateTimeFormatInfo;
				}
			}
			return DateTimeFormatInfo.CurrentInfo;
		}

		[__DynamicallyInvokable]
		public object GetFormat(Type formatType)
		{
			if (!(formatType == typeof(DateTimeFormatInfo)))
			{
				return null;
			}
			return this;
		}

		[__DynamicallyInvokable]
		public object Clone()
		{
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)base.MemberwiseClone();
			dateTimeFormatInfo.calendar = (Calendar)this.Calendar.Clone();
			dateTimeFormatInfo.m_isReadOnly = false;
			return dateTimeFormatInfo;
		}

		[__DynamicallyInvokable]
		public string AMDesignator
		{
			[__DynamicallyInvokable]
			get
			{
				return this.amDesignator;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.ClearTokenHashTable();
				this.amDesignator = value;
			}
		}

		[__DynamicallyInvokable]
		public Calendar Calendar
		{
			[__DynamicallyInvokable]
			get
			{
				return this.calendar;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
				}
				if (value == this.calendar)
				{
					return;
				}
				CultureInfo.CheckDomainSafetyObject(value, this);
				for (int i = 0; i < this.OptionalCalendars.Length; i++)
				{
					if (this.OptionalCalendars[i] == value.ID)
					{
						if (this.calendar != null)
						{
							this.m_eraNames = null;
							this.m_abbrevEraNames = null;
							this.m_abbrevEnglishEraNames = null;
							this.monthDayPattern = null;
							this.dayNames = null;
							this.abbreviatedDayNames = null;
							this.m_superShortDayNames = null;
							this.monthNames = null;
							this.abbreviatedMonthNames = null;
							this.genitiveMonthNames = null;
							this.m_genitiveAbbreviatedMonthNames = null;
							this.leapYearMonthNames = null;
							this.formatFlags = DateTimeFormatFlags.NotInitialized;
							this.allShortDatePatterns = null;
							this.allLongDatePatterns = null;
							this.allYearMonthPatterns = null;
							this.dateTimeOffsetPattern = null;
							this.longDatePattern = null;
							this.shortDatePattern = null;
							this.yearMonthPattern = null;
							this.fullDateTimePattern = null;
							this.generalShortTimePattern = null;
							this.generalLongTimePattern = null;
							this.dateSeparator = null;
							this.ClearTokenHashTable();
						}
						this.calendar = value;
						this.InitializeOverridableProperties(this.m_cultureData, this.calendar.ID);
						return;
					}
				}
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Argument_InvalidCalendar"));
			}
		}

		private int[] OptionalCalendars
		{
			get
			{
				if (this.optionalCalendars == null)
				{
					this.optionalCalendars = this.m_cultureData.CalendarIds;
				}
				return this.optionalCalendars;
			}
		}

		[__DynamicallyInvokable]
		public int GetEra(string eraName)
		{
			if (eraName == null)
			{
				throw new ArgumentNullException("eraName", Environment.GetResourceString("ArgumentNull_String"));
			}
			if (eraName.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < this.EraNames.Length; i++)
			{
				if (this.m_eraNames[i].Length > 0 && string.Compare(eraName, this.m_eraNames[i], this.Culture, CompareOptions.IgnoreCase) == 0)
				{
					return i + 1;
				}
			}
			for (int j = 0; j < this.AbbreviatedEraNames.Length; j++)
			{
				if (string.Compare(eraName, this.m_abbrevEraNames[j], this.Culture, CompareOptions.IgnoreCase) == 0)
				{
					return j + 1;
				}
			}
			for (int k = 0; k < this.AbbreviatedEnglishEraNames.Length; k++)
			{
				if (string.Compare(eraName, this.m_abbrevEnglishEraNames[k], StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return k + 1;
				}
			}
			return -1;
		}

		internal string[] EraNames
		{
			get
			{
				if (this.m_eraNames == null)
				{
					this.m_eraNames = this.m_cultureData.EraNames(this.Calendar.ID);
				}
				return this.m_eraNames;
			}
		}

		[__DynamicallyInvokable]
		public string GetEraName(int era)
		{
			if (era == 0)
			{
				era = this.Calendar.CurrentEraValue;
			}
			if (--era < this.EraNames.Length && era >= 0)
			{
				return this.m_eraNames[era];
			}
			throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("ArgumentOutOfRange_InvalidEraValue"));
		}

		internal string[] AbbreviatedEraNames
		{
			get
			{
				if (this.m_abbrevEraNames == null)
				{
					this.m_abbrevEraNames = this.m_cultureData.AbbrevEraNames(this.Calendar.ID);
				}
				return this.m_abbrevEraNames;
			}
		}

		[__DynamicallyInvokable]
		public string GetAbbreviatedEraName(int era)
		{
			if (this.AbbreviatedEraNames.Length == 0)
			{
				return this.GetEraName(era);
			}
			if (era == 0)
			{
				era = this.Calendar.CurrentEraValue;
			}
			if (--era < this.m_abbrevEraNames.Length && era >= 0)
			{
				return this.m_abbrevEraNames[era];
			}
			throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("ArgumentOutOfRange_InvalidEraValue"));
		}

		internal string[] AbbreviatedEnglishEraNames
		{
			get
			{
				if (this.m_abbrevEnglishEraNames == null)
				{
					this.m_abbrevEnglishEraNames = this.m_cultureData.AbbreviatedEnglishEraNames(this.Calendar.ID);
				}
				return this.m_abbrevEnglishEraNames;
			}
		}

		public string DateSeparator
		{
			get
			{
				return this.dateSeparator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.ClearTokenHashTable();
				this.dateSeparator = value;
			}
		}

		[__DynamicallyInvokable]
		public DayOfWeek FirstDayOfWeek
		{
			[__DynamicallyInvokable]
			get
			{
				return (DayOfWeek)this.firstDayOfWeek;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value >= DayOfWeek.Sunday && value <= DayOfWeek.Saturday)
				{
					this.firstDayOfWeek = (int)value;
					return;
				}
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					DayOfWeek.Sunday,
					DayOfWeek.Saturday
				}));
			}
		}

		[__DynamicallyInvokable]
		public CalendarWeekRule CalendarWeekRule
		{
			[__DynamicallyInvokable]
			get
			{
				return (CalendarWeekRule)this.calendarWeekRule;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value >= CalendarWeekRule.FirstDay && value <= CalendarWeekRule.FirstFourDayWeek)
				{
					this.calendarWeekRule = (int)value;
					return;
				}
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					CalendarWeekRule.FirstDay,
					CalendarWeekRule.FirstFourDayWeek
				}));
			}
		}

		[__DynamicallyInvokable]
		public string FullDateTimePattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.fullDateTimePattern == null)
				{
					this.fullDateTimePattern = this.LongDatePattern + " " + this.LongTimePattern;
				}
				return this.fullDateTimePattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.fullDateTimePattern = value;
			}
		}

		[__DynamicallyInvokable]
		public string LongDatePattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.longDatePattern == null)
				{
					this.longDatePattern = this.UnclonedLongDatePatterns[0];
				}
				return this.longDatePattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.longDatePattern = value;
				this.ClearTokenHashTable();
				this.fullDateTimePattern = null;
			}
		}

		[__DynamicallyInvokable]
		public string LongTimePattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.longTimePattern == null)
				{
					this.longTimePattern = this.UnclonedLongTimePatterns[0];
				}
				return this.longTimePattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.longTimePattern = value;
				this.ClearTokenHashTable();
				this.fullDateTimePattern = null;
				this.generalLongTimePattern = null;
				this.dateTimeOffsetPattern = null;
			}
		}

		[__DynamicallyInvokable]
		public string MonthDayPattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.monthDayPattern == null)
				{
					this.monthDayPattern = this.m_cultureData.MonthDay(this.Calendar.ID);
				}
				return this.monthDayPattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.monthDayPattern = value;
			}
		}

		[__DynamicallyInvokable]
		public string PMDesignator
		{
			[__DynamicallyInvokable]
			get
			{
				return this.pmDesignator;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.ClearTokenHashTable();
				this.pmDesignator = value;
			}
		}

		[__DynamicallyInvokable]
		public string RFC1123Pattern
		{
			[__DynamicallyInvokable]
			get
			{
				return "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
			}
		}

		[__DynamicallyInvokable]
		public string ShortDatePattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.shortDatePattern == null)
				{
					this.shortDatePattern = this.UnclonedShortDatePatterns[0];
				}
				return this.shortDatePattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.shortDatePattern = value;
				this.ClearTokenHashTable();
				this.generalLongTimePattern = null;
				this.generalShortTimePattern = null;
				this.dateTimeOffsetPattern = null;
			}
		}

		[__DynamicallyInvokable]
		public string ShortTimePattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.shortTimePattern == null)
				{
					this.shortTimePattern = this.UnclonedShortTimePatterns[0];
				}
				return this.shortTimePattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.shortTimePattern = value;
				this.ClearTokenHashTable();
				this.generalShortTimePattern = null;
			}
		}

		[__DynamicallyInvokable]
		public string SortableDateTimePattern
		{
			[__DynamicallyInvokable]
			get
			{
				return "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
			}
		}

		internal string GeneralShortTimePattern
		{
			get
			{
				if (this.generalShortTimePattern == null)
				{
					this.generalShortTimePattern = this.ShortDatePattern + " " + this.ShortTimePattern;
				}
				return this.generalShortTimePattern;
			}
		}

		internal string GeneralLongTimePattern
		{
			get
			{
				if (this.generalLongTimePattern == null)
				{
					this.generalLongTimePattern = this.ShortDatePattern + " " + this.LongTimePattern;
				}
				return this.generalLongTimePattern;
			}
		}

		internal string DateTimeOffsetPattern
		{
			get
			{
				if (this.dateTimeOffsetPattern == null)
				{
					this.dateTimeOffsetPattern = this.ShortDatePattern + " " + this.LongTimePattern;
					bool flag = false;
					bool flag2 = false;
					char c = '\'';
					int num = 0;
					while (!flag && num < this.LongTimePattern.Length)
					{
						char c2 = this.LongTimePattern[num];
						if (c2 <= '%')
						{
							if (c2 == '"')
							{
								goto IL_6D;
							}
							if (c2 == '%')
							{
								goto IL_97;
							}
						}
						else
						{
							if (c2 == '\'')
							{
								goto IL_6D;
							}
							if (c2 == '\\')
							{
								goto IL_97;
							}
							if (c2 == 'z')
							{
								flag = !flag2;
							}
						}
						IL_9B:
						num++;
						continue;
						IL_6D:
						if (flag2 && c == this.LongTimePattern[num])
						{
							flag2 = false;
							goto IL_9B;
						}
						if (!flag2)
						{
							c = this.LongTimePattern[num];
							flag2 = true;
							goto IL_9B;
						}
						goto IL_9B;
						IL_97:
						num++;
						goto IL_9B;
					}
					if (!flag)
					{
						this.dateTimeOffsetPattern += " zzz";
					}
				}
				return this.dateTimeOffsetPattern;
			}
		}

		public string TimeSeparator
		{
			get
			{
				return this.timeSeparator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.ClearTokenHashTable();
				this.timeSeparator = value;
			}
		}

		[__DynamicallyInvokable]
		public string UniversalSortableDateTimePattern
		{
			[__DynamicallyInvokable]
			get
			{
				return "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
			}
		}

		[__DynamicallyInvokable]
		public string YearMonthPattern
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.yearMonthPattern == null)
				{
					this.yearMonthPattern = this.UnclonedYearMonthPatterns[0];
				}
				return this.yearMonthPattern;
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
				}
				this.yearMonthPattern = value;
				this.ClearTokenHashTable();
			}
		}

		private static void CheckNullValue(string[] values, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (values[i] == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_ArrayValue"));
				}
			}
		}

		[__DynamicallyInvokable]
		public string[] AbbreviatedDayNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetAbbreviatedDayOfWeekNames().Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 7)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						7
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length);
				this.ClearTokenHashTable();
				this.abbreviatedDayNames = value;
			}
		}

		[ComVisible(false)]
		[__DynamicallyInvokable]
		public string[] ShortestDayNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetSuperShortDayNames().Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 7)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						7
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length);
				this.m_superShortDayNames = value;
			}
		}

		[__DynamicallyInvokable]
		public string[] DayNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetDayOfWeekNames().Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 7)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						7
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length);
				this.ClearTokenHashTable();
				this.dayNames = value;
			}
		}

		[__DynamicallyInvokable]
		public string[] AbbreviatedMonthNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetAbbreviatedMonthNames().Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 13)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						13
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length - 1);
				this.ClearTokenHashTable();
				this.abbreviatedMonthNames = value;
			}
		}

		[__DynamicallyInvokable]
		public string[] MonthNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetMonthNames().Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 13)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						13
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length - 1);
				this.monthNames = value;
				this.ClearTokenHashTable();
			}
		}

		internal bool HasSpacesInMonthNames
		{
			get
			{
				return (this.FormatFlags & DateTimeFormatFlags.UseSpacesInMonthNames) > DateTimeFormatFlags.None;
			}
		}

		internal bool HasSpacesInDayNames
		{
			get
			{
				return (this.FormatFlags & DateTimeFormatFlags.UseSpacesInDayNames) > DateTimeFormatFlags.None;
			}
		}

		internal string internalGetMonthName(int month, MonthNameStyles style, bool abbreviated)
		{
			string[] array;
			if (style != MonthNameStyles.Genitive)
			{
				if (style != MonthNameStyles.LeapYear)
				{
					array = (abbreviated ? this.internalGetAbbreviatedMonthNames() : this.internalGetMonthNames());
				}
				else
				{
					array = this.internalGetLeapYearMonthNames();
				}
			}
			else
			{
				array = this.internalGetGenitiveMonthNames(abbreviated);
			}
			if (month < 1 || month > array.Length)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					1,
					array.Length
				}));
			}
			return array[month - 1];
		}

		private string[] internalGetGenitiveMonthNames(bool abbreviated)
		{
			if (abbreviated)
			{
				if (this.m_genitiveAbbreviatedMonthNames == null)
				{
					this.m_genitiveAbbreviatedMonthNames = this.m_cultureData.AbbreviatedGenitiveMonthNames(this.Calendar.ID);
				}
				return this.m_genitiveAbbreviatedMonthNames;
			}
			if (this.genitiveMonthNames == null)
			{
				this.genitiveMonthNames = this.m_cultureData.GenitiveMonthNames(this.Calendar.ID);
			}
			return this.genitiveMonthNames;
		}

		internal string[] internalGetLeapYearMonthNames()
		{
			if (this.leapYearMonthNames == null)
			{
				this.leapYearMonthNames = this.m_cultureData.LeapYearMonthNames(this.Calendar.ID);
			}
			return this.leapYearMonthNames;
		}

		[__DynamicallyInvokable]
		public string GetAbbreviatedDayName(DayOfWeek dayofweek)
		{
			if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException("dayofweek", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					DayOfWeek.Sunday,
					DayOfWeek.Saturday
				}));
			}
			return this.internalGetAbbreviatedDayOfWeekNames()[(int)dayofweek];
		}

		[ComVisible(false)]
		public string GetShortestDayName(DayOfWeek dayOfWeek)
		{
			if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException("dayOfWeek", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					DayOfWeek.Sunday,
					DayOfWeek.Saturday
				}));
			}
			return this.internalGetSuperShortDayNames()[(int)dayOfWeek];
		}

		private static string[] GetCombinedPatterns(string[] patterns1, string[] patterns2, string connectString)
		{
			string[] array = new string[patterns1.Length * patterns2.Length];
			int num = 0;
			for (int i = 0; i < patterns1.Length; i++)
			{
				for (int j = 0; j < patterns2.Length; j++)
				{
					array[num++] = patterns1[i] + connectString + patterns2[j];
				}
			}
			return array;
		}

		public string[] GetAllDateTimePatterns()
		{
			List<string> list = new List<string>(132);
			for (int i = 0; i < DateTimeFormat.allStandardFormats.Length; i++)
			{
				string[] allDateTimePatterns = this.GetAllDateTimePatterns(DateTimeFormat.allStandardFormats[i]);
				for (int j = 0; j < allDateTimePatterns.Length; j++)
				{
					list.Add(allDateTimePatterns[j]);
				}
			}
			return list.ToArray();
		}

		public string[] GetAllDateTimePatterns(char format)
		{
			if (format <= 'U')
			{
				switch (format)
				{
				case 'D':
					return this.AllLongDatePatterns;
				case 'E':
					goto IL_1AF;
				case 'F':
					break;
				case 'G':
					return DateTimeFormatInfo.GetCombinedPatterns(this.AllShortDatePatterns, this.AllLongTimePatterns, " ");
				default:
					switch (format)
					{
					case 'M':
						goto IL_13D;
					case 'N':
					case 'P':
					case 'Q':
					case 'S':
						goto IL_1AF;
					case 'O':
						goto IL_14F;
					case 'R':
						goto IL_160;
					case 'T':
						return this.AllLongTimePatterns;
					case 'U':
						break;
					default:
						goto IL_1AF;
					}
					break;
				}
				return DateTimeFormatInfo.GetCombinedPatterns(this.AllLongDatePatterns, this.AllLongTimePatterns, " ");
			}
			if (format != 'Y')
			{
				switch (format)
				{
				case 'd':
					return this.AllShortDatePatterns;
				case 'e':
					goto IL_1AF;
				case 'f':
					return DateTimeFormatInfo.GetCombinedPatterns(this.AllLongDatePatterns, this.AllShortTimePatterns, " ");
				case 'g':
					return DateTimeFormatInfo.GetCombinedPatterns(this.AllShortDatePatterns, this.AllShortTimePatterns, " ");
				default:
					switch (format)
					{
					case 'm':
						goto IL_13D;
					case 'n':
					case 'p':
					case 'q':
					case 'v':
					case 'w':
					case 'x':
						goto IL_1AF;
					case 'o':
						goto IL_14F;
					case 'r':
						goto IL_160;
					case 's':
						return new string[]
						{
							"yyyy'-'MM'-'dd'T'HH':'mm':'ss"
						};
					case 't':
						return this.AllShortTimePatterns;
					case 'u':
						return new string[]
						{
							this.UniversalSortableDateTimePattern
						};
					case 'y':
						break;
					default:
						goto IL_1AF;
					}
					break;
				}
			}
			return this.AllYearMonthPatterns;
			IL_13D:
			return new string[]
			{
				this.MonthDayPattern
			};
			IL_14F:
			return new string[]
			{
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"
			};
			IL_160:
			return new string[]
			{
				"ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"
			};
			IL_1AF:
			throw new ArgumentException(Environment.GetResourceString("Format_BadFormatSpecifier"), "format");
		}

		[__DynamicallyInvokable]
		public string GetDayName(DayOfWeek dayofweek)
		{
			if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException("dayofweek", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					DayOfWeek.Sunday,
					DayOfWeek.Saturday
				}));
			}
			return this.internalGetDayOfWeekNames()[(int)dayofweek];
		}

		[__DynamicallyInvokable]
		public string GetAbbreviatedMonthName(int month)
		{
			if (month < 1 || month > 13)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					1,
					13
				}));
			}
			return this.internalGetAbbreviatedMonthNames()[month - 1];
		}

		[__DynamicallyInvokable]
		public string GetMonthName(int month)
		{
			if (month < 1 || month > 13)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					1,
					13
				}));
			}
			return this.internalGetMonthNames()[month - 1];
		}

		private static string[] GetMergedPatterns(string[] patterns, string defaultPattern)
		{
			if (defaultPattern == patterns[0])
			{
				return (string[])patterns.Clone();
			}
			int num = 0;
			while (num < patterns.Length && !(defaultPattern == patterns[num]))
			{
				num++;
			}
			string[] array;
			if (num < patterns.Length)
			{
				array = (string[])patterns.Clone();
				array[num] = array[0];
			}
			else
			{
				array = new string[patterns.Length + 1];
				Array.Copy(patterns, 0, array, 1, patterns.Length);
			}
			array[0] = defaultPattern;
			return array;
		}

		private string[] AllYearMonthPatterns
		{
			get
			{
				return DateTimeFormatInfo.GetMergedPatterns(this.UnclonedYearMonthPatterns, this.YearMonthPattern);
			}
		}

		private string[] AllShortDatePatterns
		{
			get
			{
				return DateTimeFormatInfo.GetMergedPatterns(this.UnclonedShortDatePatterns, this.ShortDatePattern);
			}
		}

		private string[] AllShortTimePatterns
		{
			get
			{
				return DateTimeFormatInfo.GetMergedPatterns(this.UnclonedShortTimePatterns, this.ShortTimePattern);
			}
		}

		private string[] AllLongDatePatterns
		{
			get
			{
				return DateTimeFormatInfo.GetMergedPatterns(this.UnclonedLongDatePatterns, this.LongDatePattern);
			}
		}

		private string[] AllLongTimePatterns
		{
			get
			{
				return DateTimeFormatInfo.GetMergedPatterns(this.UnclonedLongTimePatterns, this.LongTimePattern);
			}
		}

		private string[] UnclonedYearMonthPatterns
		{
			get
			{
				if (this.allYearMonthPatterns == null)
				{
					this.allYearMonthPatterns = this.m_cultureData.YearMonths(this.Calendar.ID);
				}
				return this.allYearMonthPatterns;
			}
		}

		private string[] UnclonedShortDatePatterns
		{
			get
			{
				if (this.allShortDatePatterns == null)
				{
					this.allShortDatePatterns = this.m_cultureData.ShortDates(this.Calendar.ID);
				}
				return this.allShortDatePatterns;
			}
		}

		private string[] UnclonedLongDatePatterns
		{
			get
			{
				if (this.allLongDatePatterns == null)
				{
					this.allLongDatePatterns = this.m_cultureData.LongDates(this.Calendar.ID);
				}
				return this.allLongDatePatterns;
			}
		}

		private string[] UnclonedShortTimePatterns
		{
			get
			{
				if (this.allShortTimePatterns == null)
				{
					this.allShortTimePatterns = this.m_cultureData.ShortTimes;
				}
				return this.allShortTimePatterns;
			}
		}

		private string[] UnclonedLongTimePatterns
		{
			get
			{
				if (this.allLongTimePatterns == null)
				{
					this.allLongTimePatterns = this.m_cultureData.LongTimes;
				}
				return this.allLongTimePatterns;
			}
		}

		[__DynamicallyInvokable]
		public static DateTimeFormatInfo ReadOnly(DateTimeFormatInfo dtfi)
		{
			if (dtfi == null)
			{
				throw new ArgumentNullException("dtfi", Environment.GetResourceString("ArgumentNull_Obj"));
			}
			if (dtfi.IsReadOnly)
			{
				return dtfi;
			}
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)dtfi.MemberwiseClone();
			dateTimeFormatInfo.calendar = Calendar.ReadOnly(dtfi.Calendar);
			dateTimeFormatInfo.m_isReadOnly = true;
			return dateTimeFormatInfo;
		}

		[__DynamicallyInvokable]
		public bool IsReadOnly
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_isReadOnly;
			}
		}

		[ComVisible(false)]
		public string NativeCalendarName
		{
			get
			{
				return this.m_cultureData.CalendarName(this.Calendar.ID);
			}
		}

		[ComVisible(false)]
		public void SetAllDateTimePatterns(string[] patterns, char format)
		{
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
			}
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns", Environment.GetResourceString("ArgumentNull_Array"));
			}
			if (patterns.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ArrayZeroError"), "patterns");
			}
			for (int i = 0; i < patterns.Length; i++)
			{
				if (patterns[i] == null)
				{
					throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_ArrayValue"));
				}
			}
			if (format <= 'Y')
			{
				if (format == 'D')
				{
					this.allLongDatePatterns = patterns;
					this.longDatePattern = this.allLongDatePatterns[0];
					goto IL_11E;
				}
				if (format == 'T')
				{
					this.allLongTimePatterns = patterns;
					this.longTimePattern = this.allLongTimePatterns[0];
					goto IL_11E;
				}
				if (format != 'Y')
				{
					goto IL_109;
				}
			}
			else
			{
				if (format == 'd')
				{
					this.allShortDatePatterns = patterns;
					this.shortDatePattern = this.allShortDatePatterns[0];
					goto IL_11E;
				}
				if (format == 't')
				{
					this.allShortTimePatterns = patterns;
					this.shortTimePattern = this.allShortTimePatterns[0];
					goto IL_11E;
				}
				if (format != 'y')
				{
					goto IL_109;
				}
			}
			this.allYearMonthPatterns = patterns;
			this.yearMonthPattern = this.allYearMonthPatterns[0];
			goto IL_11E;
			IL_109:
			throw new ArgumentException(Environment.GetResourceString("Format_BadFormatSpecifier"), "format");
			IL_11E:
			this.ClearTokenHashTable();
		}

		[ComVisible(false)]
		[__DynamicallyInvokable]
		public string[] AbbreviatedMonthGenitiveNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetGenitiveMonthNames(true).Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 13)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						13
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length - 1);
				this.ClearTokenHashTable();
				this.m_genitiveAbbreviatedMonthNames = value;
			}
		}

		[ComVisible(false)]
		[__DynamicallyInvokable]
		public string[] MonthGenitiveNames
		{
			[__DynamicallyInvokable]
			get
			{
				return (string[])this.internalGetGenitiveMonthNames(false).Clone();
			}
			[__DynamicallyInvokable]
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
				}
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Array"));
				}
				if (value.Length != 13)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidArrayLength", new object[]
					{
						13
					}), "value");
				}
				DateTimeFormatInfo.CheckNullValue(value, value.Length - 1);
				this.genitiveMonthNames = value;
				this.ClearTokenHashTable();
			}
		}

		internal string FullTimeSpanPositivePattern
		{
			get
			{
				if (this.m_fullTimeSpanPositivePattern == null)
				{
					CultureData cultureData;
					if (this.m_cultureData.UseUserOverride)
					{
						cultureData = CultureData.GetCultureData(this.m_cultureData.CultureName, false);
					}
					else
					{
						cultureData = this.m_cultureData;
					}
					string numberDecimalSeparator = new NumberFormatInfo(cultureData).NumberDecimalSeparator;
					this.m_fullTimeSpanPositivePattern = "d':'h':'mm':'ss'" + numberDecimalSeparator + "'FFFFFFF";
				}
				return this.m_fullTimeSpanPositivePattern;
			}
		}

		internal string FullTimeSpanNegativePattern
		{
			get
			{
				if (this.m_fullTimeSpanNegativePattern == null)
				{
					this.m_fullTimeSpanNegativePattern = "'-'" + this.FullTimeSpanPositivePattern;
				}
				return this.m_fullTimeSpanNegativePattern;
			}
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				if (this.m_compareInfo == null)
				{
					this.m_compareInfo = CompareInfo.GetCompareInfo(this.m_cultureData.SCOMPAREINFO);
				}
				return this.m_compareInfo;
			}
		}

		internal static void ValidateStyles(DateTimeStyles style, string parameterName)
		{
			if ((style & ~(DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind)) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeStyles"), parameterName);
			}
			if ((style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None && (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_ConflictingDateTimeStyles"), parameterName);
			}
			if ((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None && (style & (DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal)) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_ConflictingDateTimeRoundtripStyles"), parameterName);
			}
		}

		internal DateTimeFormatFlags FormatFlags
		{
			get
			{
				if (this.formatFlags == DateTimeFormatFlags.NotInitialized)
				{
					this.formatFlags = DateTimeFormatFlags.None;
					this.formatFlags |= (DateTimeFormatFlags)DateTimeFormatInfoScanner.GetFormatFlagGenitiveMonth(this.MonthNames, this.internalGetGenitiveMonthNames(false), this.AbbreviatedMonthNames, this.internalGetGenitiveMonthNames(true));
					this.formatFlags |= (DateTimeFormatFlags)DateTimeFormatInfoScanner.GetFormatFlagUseSpaceInMonthNames(this.MonthNames, this.internalGetGenitiveMonthNames(false), this.AbbreviatedMonthNames, this.internalGetGenitiveMonthNames(true));
					this.formatFlags |= (DateTimeFormatFlags)DateTimeFormatInfoScanner.GetFormatFlagUseSpaceInDayNames(this.DayNames, this.AbbreviatedDayNames);
					this.formatFlags |= (DateTimeFormatFlags)DateTimeFormatInfoScanner.GetFormatFlagUseHebrewCalendar(this.Calendar.ID);
				}
				return this.formatFlags;
			}
		}

		internal bool HasForceTwoDigitYears
		{
			get
			{
				int id = this.calendar.ID;
				return id - 3 <= 1;
			}
		}

		internal bool HasYearMonthAdjustment
		{
			get
			{
				return (this.FormatFlags & DateTimeFormatFlags.UseHebrewRule) > DateTimeFormatFlags.None;
			}
		}

		internal bool YearMonthAdjustment(ref int year, ref int month, bool parsedMonthName)
		{
			if ((this.FormatFlags & DateTimeFormatFlags.UseHebrewRule) != DateTimeFormatFlags.None)
			{
				if (year < 1000)
				{
					year += 5000;
				}
				if (year < this.Calendar.GetYear(this.Calendar.MinSupportedDateTime) || year > this.Calendar.GetYear(this.Calendar.MaxSupportedDateTime))
				{
					return false;
				}
				if (parsedMonthName && !this.Calendar.IsLeapYear(year))
				{
					if (month >= 8)
					{
						month--;
					}
					else if (month == 7)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static DateTimeFormatInfo GetJapaneseCalendarDTFI()
		{
			DateTimeFormatInfo dateTimeFormat = DateTimeFormatInfo.s_jajpDTFI;
			if (dateTimeFormat == null)
			{
				dateTimeFormat = new CultureInfo("ja-JP", false).DateTimeFormat;
				dateTimeFormat.Calendar = JapaneseCalendar.GetDefaultInstance();
				DateTimeFormatInfo.s_jajpDTFI = dateTimeFormat;
			}
			return dateTimeFormat;
		}

		internal static DateTimeFormatInfo GetTaiwanCalendarDTFI()
		{
			DateTimeFormatInfo dateTimeFormat = DateTimeFormatInfo.s_zhtwDTFI;
			if (dateTimeFormat == null)
			{
				dateTimeFormat = new CultureInfo("zh-TW", false).DateTimeFormat;
				dateTimeFormat.Calendar = TaiwanCalendar.GetDefaultInstance();
				DateTimeFormatInfo.s_zhtwDTFI = dateTimeFormat;
			}
			return dateTimeFormat;
		}

		private void ClearTokenHashTable()
		{
			this.m_dtfiTokenHash = null;
			this.formatFlags = DateTimeFormatFlags.NotInitialized;
		}

		[SecurityCritical]
		internal TokenHashValue[] CreateTokenHashTable()
		{
			TokenHashValue[] array = this.m_dtfiTokenHash;
			if (array == null)
			{
				array = new TokenHashValue[199];
				bool flag = this.LanguageName.Equals("ko");
				string b = this.TimeSeparator.Trim();
				if ("," != b)
				{
					this.InsertHash(array, ",", TokenType.IgnorableSymbol, 0);
				}
				if ("." != b)
				{
					this.InsertHash(array, ".", TokenType.IgnorableSymbol, 0);
				}
				if ("시" != b && "時" != b && "时" != b)
				{
					this.InsertHash(array, this.TimeSeparator, TokenType.SEP_Time, 0);
				}
				this.InsertHash(array, this.AMDesignator, (TokenType)1027, 0);
				this.InsertHash(array, this.PMDesignator, (TokenType)1284, 1);
				if (this.LanguageName.Equals("sq"))
				{
					this.InsertHash(array, "." + this.AMDesignator, (TokenType)1027, 0);
					this.InsertHash(array, "." + this.PMDesignator, (TokenType)1284, 1);
				}
				this.InsertHash(array, "年", TokenType.SEP_YearSuff, 0);
				this.InsertHash(array, "년", TokenType.SEP_YearSuff, 0);
				this.InsertHash(array, "月", TokenType.SEP_MonthSuff, 0);
				this.InsertHash(array, "월", TokenType.SEP_MonthSuff, 0);
				this.InsertHash(array, "日", TokenType.SEP_DaySuff, 0);
				this.InsertHash(array, "일", TokenType.SEP_DaySuff, 0);
				this.InsertHash(array, "時", TokenType.SEP_HourSuff, 0);
				this.InsertHash(array, "时", TokenType.SEP_HourSuff, 0);
				this.InsertHash(array, "分", TokenType.SEP_MinuteSuff, 0);
				this.InsertHash(array, "秒", TokenType.SEP_SecondSuff, 0);
				if (!AppContextSwitches.EnforceLegacyJapaneseDateParsing && this.Calendar.ID == 3)
				{
					this.InsertHash(array, "元", TokenType.YearNumberToken, 1);
					this.InsertHash(array, "(", TokenType.IgnorableSymbol, 0);
					this.InsertHash(array, ")", TokenType.IgnorableSymbol, 0);
				}
				if (flag)
				{
					this.InsertHash(array, "시", TokenType.SEP_HourSuff, 0);
					this.InsertHash(array, "분", TokenType.SEP_MinuteSuff, 0);
					this.InsertHash(array, "초", TokenType.SEP_SecondSuff, 0);
				}
				if (this.LanguageName.Equals("ky"))
				{
					this.InsertHash(array, "-", TokenType.IgnorableSymbol, 0);
				}
				else
				{
					this.InsertHash(array, "-", TokenType.SEP_DateOrOffset, 0);
				}
				DateTimeFormatInfoScanner dateTimeFormatInfoScanner = new DateTimeFormatInfoScanner();
				string[] array2 = this.m_dateWords = dateTimeFormatInfoScanner.GetDateWordsOfDTFI(this);
				DateTimeFormatFlags dateTimeFormatFlags = this.FormatFlags;
				bool flag2 = false;
				if (array2 != null)
				{
					for (int i = 0; i < array2.Length; i++)
					{
						char c = array2[i][0];
						if (c != '')
						{
							if (c != '')
							{
								this.InsertHash(array, array2[i], TokenType.DateWordToken, 0);
								if (this.LanguageName.Equals("eu"))
								{
									this.InsertHash(array, "." + array2[i], TokenType.DateWordToken, 0);
								}
							}
							else
							{
								string text = array2[i].Substring(1);
								this.InsertHash(array, text, TokenType.IgnorableSymbol, 0);
								if (this.DateSeparator.Trim(null).Equals(text))
								{
									flag2 = true;
								}
							}
						}
						else
						{
							string monthPostfix = array2[i].Substring(1);
							this.AddMonthNames(array, monthPostfix);
						}
					}
				}
				if (!flag2)
				{
					this.InsertHash(array, this.DateSeparator, TokenType.SEP_Date, 0);
				}
				this.AddMonthNames(array, null);
				for (int j = 1; j <= 13; j++)
				{
					this.InsertHash(array, this.GetAbbreviatedMonthName(j), TokenType.MonthToken, j);
				}
				if ((this.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != DateTimeFormatFlags.None)
				{
					for (int k = 1; k <= 13; k++)
					{
						string str = this.internalGetMonthName(k, MonthNameStyles.Genitive, false);
						this.InsertHash(array, str, TokenType.MonthToken, k);
					}
				}
				if ((this.FormatFlags & DateTimeFormatFlags.UseLeapYearMonth) != DateTimeFormatFlags.None)
				{
					for (int l = 1; l <= 13; l++)
					{
						string str2 = this.internalGetMonthName(l, MonthNameStyles.LeapYear, false);
						this.InsertHash(array, str2, TokenType.MonthToken, l);
					}
				}
				for (int m = 0; m < 7; m++)
				{
					string str3 = this.GetDayName((DayOfWeek)m);
					this.InsertHash(array, str3, TokenType.DayOfWeekToken, m);
					str3 = this.GetAbbreviatedDayName((DayOfWeek)m);
					this.InsertHash(array, str3, TokenType.DayOfWeekToken, m);
				}
				int[] eras = this.calendar.Eras;
				for (int n = 1; n <= eras.Length; n++)
				{
					this.InsertHash(array, this.GetEraName(n), TokenType.EraToken, n);
					this.InsertHash(array, this.GetAbbreviatedEraName(n), TokenType.EraToken, n);
				}
				if (this.LanguageName.Equals("ja"))
				{
					for (int num = 0; num < 7; num++)
					{
						string str4 = "(" + this.GetAbbreviatedDayName((DayOfWeek)num) + ")";
						this.InsertHash(array, str4, TokenType.DayOfWeekToken, num);
					}
					if (this.Calendar.GetType() != typeof(JapaneseCalendar))
					{
						DateTimeFormatInfo japaneseCalendarDTFI = DateTimeFormatInfo.GetJapaneseCalendarDTFI();
						for (int num2 = 1; num2 <= japaneseCalendarDTFI.Calendar.Eras.Length; num2++)
						{
							this.InsertHash(array, japaneseCalendarDTFI.GetEraName(num2), TokenType.JapaneseEraToken, num2);
							this.InsertHash(array, japaneseCalendarDTFI.GetAbbreviatedEraName(num2), TokenType.JapaneseEraToken, num2);
							this.InsertHash(array, japaneseCalendarDTFI.AbbreviatedEnglishEraNames[num2 - 1], TokenType.JapaneseEraToken, num2);
						}
					}
				}
				else if (this.CultureName.Equals("zh-TW"))
				{
					DateTimeFormatInfo taiwanCalendarDTFI = DateTimeFormatInfo.GetTaiwanCalendarDTFI();
					for (int num3 = 1; num3 <= taiwanCalendarDTFI.Calendar.Eras.Length; num3++)
					{
						if (taiwanCalendarDTFI.GetEraName(num3).Length > 0)
						{
							this.InsertHash(array, taiwanCalendarDTFI.GetEraName(num3), TokenType.TEraToken, num3);
						}
					}
				}
				this.InsertHash(array, DateTimeFormatInfo.InvariantInfo.AMDesignator, (TokenType)1027, 0);
				this.InsertHash(array, DateTimeFormatInfo.InvariantInfo.PMDesignator, (TokenType)1284, 1);
				for (int num4 = 1; num4 <= 12; num4++)
				{
					string str5 = DateTimeFormatInfo.InvariantInfo.GetMonthName(num4);
					this.InsertHash(array, str5, TokenType.MonthToken, num4);
					str5 = DateTimeFormatInfo.InvariantInfo.GetAbbreviatedMonthName(num4);
					this.InsertHash(array, str5, TokenType.MonthToken, num4);
				}
				for (int num5 = 0; num5 < 7; num5++)
				{
					string str6 = DateTimeFormatInfo.InvariantInfo.GetDayName((DayOfWeek)num5);
					this.InsertHash(array, str6, TokenType.DayOfWeekToken, num5);
					str6 = DateTimeFormatInfo.InvariantInfo.GetAbbreviatedDayName((DayOfWeek)num5);
					this.InsertHash(array, str6, TokenType.DayOfWeekToken, num5);
				}
				for (int num6 = 0; num6 < this.AbbreviatedEnglishEraNames.Length; num6++)
				{
					this.InsertHash(array, this.AbbreviatedEnglishEraNames[num6], TokenType.EraToken, num6 + 1);
				}
				this.InsertHash(array, "T", TokenType.SEP_LocalTimeMark, 0);
				this.InsertHash(array, "GMT", TokenType.TimeZoneToken, 0);
				this.InsertHash(array, "Z", TokenType.TimeZoneToken, 0);
				this.InsertHash(array, "/", TokenType.SEP_Date, 0);
				this.InsertHash(array, ":", TokenType.SEP_Time, 0);
				this.m_dtfiTokenHash = array;
			}
			return array;
		}

		private void AddMonthNames(TokenHashValue[] temp, string monthPostfix)
		{
			for (int i = 1; i <= 13; i++)
			{
				string text = this.GetMonthName(i);
				if (text.Length > 0)
				{
					if (monthPostfix != null)
					{
						this.InsertHash(temp, text + monthPostfix, TokenType.MonthToken, i);
					}
					else
					{
						this.InsertHash(temp, text, TokenType.MonthToken, i);
					}
				}
				text = this.GetAbbreviatedMonthName(i);
				this.InsertHash(temp, text, TokenType.MonthToken, i);
			}
		}

		private static bool TryParseHebrewNumber(ref __DTString str, out bool badFormat, out int number)
		{
			number = -1;
			badFormat = false;
			int index = str.Index;
			if (!HebrewNumber.IsDigit(str.Value[index]))
			{
				return false;
			}
			HebrewNumberParsingContext hebrewNumberParsingContext = new HebrewNumberParsingContext(0);
			HebrewNumberParsingState hebrewNumberParsingState;
			for (;;)
			{
				hebrewNumberParsingState = HebrewNumber.ParseByChar(str.Value[index++], ref hebrewNumberParsingContext);
				if (hebrewNumberParsingState <= HebrewNumberParsingState.NotHebrewDigit)
				{
					break;
				}
				if (index >= str.Value.Length || hebrewNumberParsingState == HebrewNumberParsingState.FoundEndOfHebrewNumber)
				{
					goto IL_5A;
				}
			}
			return false;
			IL_5A:
			if (hebrewNumberParsingState != HebrewNumberParsingState.FoundEndOfHebrewNumber)
			{
				return false;
			}
			str.Advance(index - str.Index);
			number = hebrewNumberParsingContext.result;
			return true;
		}

		private static bool IsHebrewChar(char ch)
		{
			return ch >= '֐' && ch <= '׿';
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsAllowedJapaneseTokenFollowedByNonSpaceLetter(string tokenString, char nextCh)
		{
			return !AppContextSwitches.EnforceLegacyJapaneseDateParsing && this.Calendar.ID == 3 && (nextCh == "元"[0] || (tokenString == "元" && nextCh == "年"[0]));
		}

		[SecurityCritical]
		internal bool Tokenize(TokenType TokenMask, out TokenType tokenType, out int tokenValue, ref __DTString str)
		{
			tokenType = TokenType.UnknownToken;
			tokenValue = 0;
			char c = str.m_current;
			bool flag = char.IsLetter(c);
			if (flag)
			{
				c = char.ToLower(c, this.Culture);
				bool flag2;
				if (DateTimeFormatInfo.IsHebrewChar(c) && TokenMask == TokenType.RegularTokenMask && DateTimeFormatInfo.TryParseHebrewNumber(ref str, out flag2, out tokenValue))
				{
					if (flag2)
					{
						tokenType = TokenType.UnknownToken;
						return false;
					}
					tokenType = TokenType.HebrewNumber;
					return true;
				}
			}
			int num = (int)(c % 'Ç');
			int num2 = (int)('\u0001' + c % 'Å');
			int num3 = str.len - str.Index;
			int num4 = 0;
			TokenHashValue[] array = this.m_dtfiTokenHash;
			if (array == null)
			{
				array = this.CreateTokenHashTable();
			}
			TokenHashValue tokenHashValue;
			int count;
			int count2;
			for (;;)
			{
				tokenHashValue = array[num];
				if (tokenHashValue == null)
				{
					return false;
				}
				if ((tokenHashValue.tokenType & TokenMask) > (TokenType)0 && tokenHashValue.tokenString.Length <= num3)
				{
					if (string.Compare(str.Value, str.Index, tokenHashValue.tokenString, 0, tokenHashValue.tokenString.Length, this.Culture, CompareOptions.IgnoreCase) == 0)
					{
						break;
					}
					if (tokenHashValue.tokenType == TokenType.MonthToken && this.HasSpacesInMonthNames)
					{
						count = 0;
						if (str.MatchSpecifiedWords(tokenHashValue.tokenString, true, ref count))
						{
							goto Block_17;
						}
					}
					else if (tokenHashValue.tokenType == TokenType.DayOfWeekToken && this.HasSpacesInDayNames)
					{
						count2 = 0;
						if (str.MatchSpecifiedWords(tokenHashValue.tokenString, true, ref count2))
						{
							goto Block_20;
						}
					}
				}
				num4++;
				num += num2;
				if (num >= 199)
				{
					num -= 199;
				}
				if (num4 >= 199)
				{
					return false;
				}
			}
			int index;
			if (flag && (index = str.Index + tokenHashValue.tokenString.Length) < str.len)
			{
				char c2 = str.Value[index];
				if (char.IsLetter(c2) && !this.IsAllowedJapaneseTokenFollowedByNonSpaceLetter(tokenHashValue.tokenString, c2))
				{
					return false;
				}
			}
			tokenType = (tokenHashValue.tokenType & TokenMask);
			tokenValue = tokenHashValue.tokenValue;
			str.Advance(tokenHashValue.tokenString.Length);
			return true;
			Block_17:
			tokenType = (tokenHashValue.tokenType & TokenMask);
			tokenValue = tokenHashValue.tokenValue;
			str.Advance(count);
			return true;
			Block_20:
			tokenType = (tokenHashValue.tokenType & TokenMask);
			tokenValue = tokenHashValue.tokenValue;
			str.Advance(count2);
			return true;
		}

		private void InsertAtCurrentHashNode(TokenHashValue[] hashTable, string str, char ch, TokenType tokenType, int tokenValue, int pos, int hashcode, int hashProbe)
		{
			TokenHashValue tokenHashValue = hashTable[hashcode];
			hashTable[hashcode] = new TokenHashValue(str, tokenType, tokenValue);
			while (++pos < 199)
			{
				hashcode += hashProbe;
				if (hashcode >= 199)
				{
					hashcode -= 199;
				}
				TokenHashValue tokenHashValue2 = hashTable[hashcode];
				if (tokenHashValue2 == null || char.ToLower(tokenHashValue2.tokenString[0], this.Culture) == ch)
				{
					hashTable[hashcode] = tokenHashValue;
					if (tokenHashValue2 == null)
					{
						return;
					}
					tokenHashValue = tokenHashValue2;
				}
			}
		}

		private void InsertHash(TokenHashValue[] hashTable, string str, TokenType tokenType, int tokenValue)
		{
			if (str == null || str.Length == 0)
			{
				return;
			}
			int num = 0;
			if (char.IsWhiteSpace(str[0]) || char.IsWhiteSpace(str[str.Length - 1]))
			{
				str = str.Trim(null);
				if (str.Length == 0)
				{
					return;
				}
			}
			char c = char.ToLower(str[0], this.Culture);
			int num2 = (int)(c % 'Ç');
			int num3 = (int)('\u0001' + c % 'Å');
			for (;;)
			{
				TokenHashValue tokenHashValue = hashTable[num2];
				if (tokenHashValue == null)
				{
					break;
				}
				if (str.Length >= tokenHashValue.tokenString.Length && string.Compare(str, 0, tokenHashValue.tokenString, 0, tokenHashValue.tokenString.Length, this.Culture, CompareOptions.IgnoreCase) == 0)
				{
					if (str.Length > tokenHashValue.tokenString.Length)
					{
						goto Block_7;
					}
					int tokenType2 = (int)tokenHashValue.tokenType;
					if (DateTimeFormatInfo.preferExistingTokens || BinaryCompatibility.TargetsAtLeast_Desktop_V4_5_1)
					{
						if (((tokenType2 & 255) == 0 && (tokenType & TokenType.RegularTokenMask) != (TokenType)0) || ((tokenType2 & 65280) == 0 && (tokenType & TokenType.SeparatorTokenMask) != (TokenType)0))
						{
							tokenHashValue.tokenType |= tokenType;
							if (tokenValue != 0)
							{
								tokenHashValue.tokenValue = tokenValue;
							}
						}
					}
					else if (((tokenType | (TokenType)tokenType2) & TokenType.RegularTokenMask) == tokenType || ((tokenType | (TokenType)tokenType2) & TokenType.SeparatorTokenMask) == tokenType)
					{
						tokenHashValue.tokenType |= tokenType;
						if (tokenValue != 0)
						{
							tokenHashValue.tokenValue = tokenValue;
						}
					}
				}
				num++;
				num2 += num3;
				if (num2 >= 199)
				{
					num2 -= 199;
				}
				if (num >= 199)
				{
					return;
				}
			}
			hashTable[num2] = new TokenHashValue(str, tokenType, tokenValue);
			return;
			Block_7:
			this.InsertAtCurrentHashNode(hashTable, str, c, tokenType, tokenValue, num, num2, num3);
		}

		private static volatile DateTimeFormatInfo invariantInfo;

		[NonSerialized]
		private CultureData m_cultureData;

		[OptionalField(VersionAdded = 2)]
		internal string m_name;

		[NonSerialized]
		private string m_langName;

		[NonSerialized]
		private CompareInfo m_compareInfo;

		[NonSerialized]
		private CultureInfo m_cultureInfo;

		internal string amDesignator;

		internal string pmDesignator;

		[OptionalField(VersionAdded = 1)]
		internal string dateSeparator;

		[OptionalField(VersionAdded = 1)]
		internal string generalShortTimePattern;

		[OptionalField(VersionAdded = 1)]
		internal string generalLongTimePattern;

		[OptionalField(VersionAdded = 1)]
		internal string timeSeparator;

		internal string monthDayPattern;

		[OptionalField(VersionAdded = 2)]
		internal string dateTimeOffsetPattern;

		internal const string rfc1123Pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";

		internal const string sortableDateTimePattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

		internal const string universalSortableDateTimePattern = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";

		internal Calendar calendar;

		internal int firstDayOfWeek = -1;

		internal int calendarWeekRule = -1;

		[OptionalField(VersionAdded = 1)]
		internal string fullDateTimePattern;

		internal string[] abbreviatedDayNames;

		[OptionalField(VersionAdded = 2)]
		internal string[] m_superShortDayNames;

		internal string[] dayNames;

		internal string[] abbreviatedMonthNames;

		internal string[] monthNames;

		[OptionalField(VersionAdded = 2)]
		internal string[] genitiveMonthNames;

		[OptionalField(VersionAdded = 2)]
		internal string[] m_genitiveAbbreviatedMonthNames;

		[OptionalField(VersionAdded = 2)]
		internal string[] leapYearMonthNames;

		internal string longDatePattern;

		internal string shortDatePattern;

		internal string yearMonthPattern;

		internal string longTimePattern;

		internal string shortTimePattern;

		[OptionalField(VersionAdded = 3)]
		private string[] allYearMonthPatterns;

		internal string[] allShortDatePatterns;

		internal string[] allLongDatePatterns;

		internal string[] allShortTimePatterns;

		internal string[] allLongTimePatterns;

		internal string[] m_eraNames;

		internal string[] m_abbrevEraNames;

		internal string[] m_abbrevEnglishEraNames;

		internal int[] optionalCalendars;

		private const int DEFAULT_ALL_DATETIMES_SIZE = 132;

		internal bool m_isReadOnly;

		[OptionalField(VersionAdded = 2)]
		internal DateTimeFormatFlags formatFlags = DateTimeFormatFlags.NotInitialized;

		internal static bool preferExistingTokens = DateTimeFormatInfo.InitPreferExistingTokens();

		[OptionalField(VersionAdded = 1)]
		private int CultureID;

		[OptionalField(VersionAdded = 1)]
		private bool m_useUserOverride;

		[OptionalField(VersionAdded = 1)]
		private bool bUseCalendarInfo;

		[OptionalField(VersionAdded = 1)]
		private int nDataItem;

		[OptionalField(VersionAdded = 2)]
		internal bool m_isDefaultCalendar;

		[OptionalField(VersionAdded = 2)]
		private static volatile Hashtable s_calendarNativeNames;

		[OptionalField(VersionAdded = 1)]
		internal string[] m_dateWords;

		private static char[] MonthSpaces = new char[]
		{
			' ',
			'\u00a0'
		};

		[NonSerialized]
		private string m_fullTimeSpanPositivePattern;

		[NonSerialized]
		private string m_fullTimeSpanNegativePattern;

		internal const DateTimeStyles InvalidDateTimeStyles = ~(DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind);

		[NonSerialized]
		private TokenHashValue[] m_dtfiTokenHash;

		private const int TOKEN_HASH_SIZE = 199;

		private const int SECOND_PRIME = 197;

		private const string dateSeparatorOrTimeZoneOffset = "-";

		private const string invariantDateSeparator = "/";

		private const string invariantTimeSeparator = ":";

		internal const string IgnorablePeriod = ".";

		internal const string IgnorableComma = ",";

		internal const string CJKYearSuff = "年";

		internal const string CJKMonthSuff = "月";

		internal const string CJKDaySuff = "日";

		internal const string KoreanYearSuff = "년";

		internal const string KoreanMonthSuff = "월";

		internal const string KoreanDaySuff = "일";

		internal const string KoreanHourSuff = "시";

		internal const string KoreanMinuteSuff = "분";

		internal const string KoreanSecondSuff = "초";

		internal const string CJKHourSuff = "時";

		internal const string ChineseHourSuff = "时";

		internal const string CJKMinuteSuff = "分";

		internal const string CJKSecondSuff = "秒";

		internal const string JapaneseEraStart = "元";

		internal const string LocalTimeMark = "T";

		internal const string KoreanLangName = "ko";

		internal const string JapaneseLangName = "ja";

		internal const string EnglishLangName = "en";

		private static volatile DateTimeFormatInfo s_jajpDTFI;

		private static volatile DateTimeFormatInfo s_zhtwDTFI;
	}
}
