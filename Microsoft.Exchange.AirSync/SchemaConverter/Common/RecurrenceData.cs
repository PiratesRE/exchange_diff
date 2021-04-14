using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class RecurrenceData : INestedData
	{
		public RecurrenceData(TypeOfRecurrence type, int protocolVersion)
		{
			this.subProperties = new Hashtable();
			this.recurrenceType = type;
			this.protocolVersion = protocolVersion;
			this.Clear();
		}

		public int ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
		}

		public byte DayOfMonth
		{
			get
			{
				return byte.Parse((string)this.subProperties["DayOfMonth"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["DayOfMonth"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public byte DayOfWeek
		{
			get
			{
				return byte.Parse((string)this.subProperties["DayOfWeek"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["DayOfWeek"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ISet<DayOfWeek> DaysOfWeek
		{
			get
			{
				DaysOfWeek dayOfWeek = (DaysOfWeek)this.DayOfWeek;
				return RecurrenceData.dayOfWeekConverter.Convert(dayOfWeek);
			}
			set
			{
				DaysOfWeek daysOfWeek = RecurrenceData.dayOfWeekConverter.Convert(value);
				this.DayOfWeek = (byte)daysOfWeek;
			}
		}

		public bool DeadOccur
		{
			get
			{
				return Convert.ToBoolean(byte.Parse((string)this.subProperties["DeadOccur"], CultureInfo.InvariantCulture));
			}
			set
			{
				this.subProperties["DeadOccur"] = Convert.ToByte(value).ToString(CultureInfo.InvariantCulture);
			}
		}

		public ushort Interval
		{
			get
			{
				return ushort.Parse((string)this.subProperties["Interval"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["Interval"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public string[] Keys
		{
			get
			{
				return this.keys;
			}
		}

		public byte MonthOfYear
		{
			get
			{
				return byte.Parse((string)this.subProperties["MonthOfYear"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["MonthOfYear"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ushort Occurrences
		{
			get
			{
				return ushort.Parse((string)this.subProperties["Occurrences"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["Occurrences"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool Regenerate
		{
			get
			{
				return Convert.ToBoolean(byte.Parse((string)this.subProperties["Regenerate"], CultureInfo.InvariantCulture));
			}
			set
			{
				this.subProperties["Regenerate"] = Convert.ToByte(value).ToString(CultureInfo.InvariantCulture);
			}
		}

		public ExDateTime Start
		{
			get
			{
				ExDateTime result;
				if (!ExDateTime.TryParseExact((string)this.subProperties["Start"], this.formatString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidStartDateTimeInRecurrenceData"
					};
				}
				return result;
			}
			set
			{
				this.subProperties["Start"] = value.ToString(this.formatString, DateTimeFormatInfo.InvariantInfo);
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public RecurrenceData.RecurrenceType Type
		{
			get
			{
				RecurrenceData.RecurrenceType recurrenceType = (RecurrenceData.RecurrenceType)byte.Parse((string)this.subProperties["Type"], CultureInfo.InvariantCulture);
				if (!EnumValidator.IsValidValue<RecurrenceData.RecurrenceType>(recurrenceType))
				{
					throw new ConversionException("RecurrenceType value is invalid.");
				}
				return recurrenceType;
			}
			set
			{
				IDictionary dictionary = this.subProperties;
				object key = "Type";
				int num = (int)value;
				dictionary[key] = num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ExDateTime Until
		{
			get
			{
				if (this.protocolVersion >= 160 && this.recurrenceType == TypeOfRecurrence.Calendar)
				{
					return TimeZoneConverter.Parse((string)this.subProperties["Until"], "RecurrenceDataUntil");
				}
				ExDateTime result;
				if (!ExDateTime.TryParseExact((string)this.subProperties["Until"], this.formatString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidUntilDateTimeInRecurrenceData"
					};
				}
				return result;
			}
			set
			{
				if (this.protocolVersion < 160 || this.recurrenceType != TypeOfRecurrence.Calendar)
				{
					this.subProperties["Until"] = value.ToString(this.formatString, DateTimeFormatInfo.InvariantInfo);
					return;
				}
				this.subProperties["Until"] = TimeZoneConverter.ToString(value);
			}
		}

		public byte WeekOfMonth
		{
			get
			{
				return byte.Parse((string)this.subProperties["WeekOfMonth"], CultureInfo.InvariantCulture);
			}
			set
			{
				this.subProperties["WeekOfMonth"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public CalendarType CalendarType
		{
			get
			{
				if (this.subProperties["CalendarType"] == null)
				{
					return CalendarType.Default;
				}
				CalendarType calendarType = (CalendarType)byte.Parse((string)this.subProperties["CalendarType"], CultureInfo.InvariantCulture);
				if (!EnumValidator.IsValidValue<CalendarType>(calendarType))
				{
					throw new ConversionException("CalendarType value is invalid.");
				}
				return calendarType;
			}
			set
			{
				IDictionary dictionary = this.subProperties;
				object key = "CalendarType";
				int num = (int)value;
				dictionary[key] = num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool IsLeapMonth
		{
			get
			{
				return this.subProperties["IsLeapMonth"] != null && ((string)this.subProperties["IsLeapMonth"]).Equals("1", StringComparison.OrdinalIgnoreCase);
			}
			set
			{
				this.subProperties["IsLeapMonth"] = (value ? "1" : "0");
			}
		}

		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				if (this.subProperties["FirstDayOfWeek"] == null)
				{
					MailboxSession mailboxSession = Command.CurrentCommand.MailboxSession;
					DayOfWeek firstDayOfWeek = mailboxSession.PreferedCulture.DateTimeFormat.FirstDayOfWeek;
					UserConfigurationManager userConfigurationManager = mailboxSession.UserConfigurationManager;
					if (userConfigurationManager == null)
					{
						return firstDayOfWeek;
					}
					try
					{
						using (UserConfiguration mailboxConfiguration = userConfigurationManager.GetMailboxConfiguration("OWA.UserOptions", UserConfigurationTypes.Dictionary))
						{
							IDictionary dictionary = mailboxConfiguration.GetDictionary();
							if (!dictionary.Contains("weekstartday"))
							{
								return firstDayOfWeek;
							}
							object obj = dictionary["weekstartday"];
							if (obj == null || !(obj is int))
							{
								return firstDayOfWeek;
							}
							if (!EnumValidator.IsValidValue<DayOfWeek>((DayOfWeek)obj))
							{
								return firstDayOfWeek;
							}
							return (DayOfWeek)obj;
						}
					}
					catch (ObjectNotFoundException arg)
					{
						AirSyncDiagnostics.TraceError<ObjectNotFoundException>(ExTraceGlobals.RequestsTracer, null, "ObjectNotFoundException when getting FirstDayOfWeekProperty from MailboxConfiguration: {0}", arg);
						return firstDayOfWeek;
					}
					catch (CorruptDataException arg2)
					{
						AirSyncDiagnostics.TraceError<CorruptDataException>(ExTraceGlobals.RequestsTracer, null, "CorruptDataException when getting FirstDayOfWeekProperty from MailboxConfiguration: {0}", arg2);
						return firstDayOfWeek;
					}
					catch (AccessDeniedException arg3)
					{
						AirSyncDiagnostics.TraceError<AccessDeniedException>(ExTraceGlobals.RequestsTracer, null, "AccessDeniedException when getting FirstDayOfWeekProperty from MailboxConfiguration: {0}", arg3);
						return firstDayOfWeek;
					}
				}
				DayOfWeek dayOfWeek = (DayOfWeek)byte.Parse((string)this.subProperties["FirstDayOfWeek"], CultureInfo.InvariantCulture);
				if (!EnumValidator.IsValidValue<DayOfWeek>(dayOfWeek))
				{
					throw new ConversionException("FirstDayOfWeek value is invalid.");
				}
				return dayOfWeek;
			}
			set
			{
				IDictionary dictionary = this.subProperties;
				object key = "FirstDayOfWeek";
				int num = (int)value;
				dictionary[key] = num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public RecurrenceOrderType RecurrenceOrderType
		{
			get
			{
				if (this.WeekOfMonth != 5)
				{
					return (RecurrenceOrderType)this.WeekOfMonth;
				}
				return RecurrenceOrderType.Last;
			}
		}

		public WeekIndex WeekIndex
		{
			get
			{
				return RecurrenceData.weekIndexConverter.Convert(this.RecurrenceOrderType);
			}
			set
			{
				switch (value)
				{
				case WeekIndex.First:
					this.WeekOfMonth = 1;
					return;
				case WeekIndex.Second:
					this.WeekOfMonth = 2;
					return;
				case WeekIndex.Third:
					this.WeekOfMonth = 3;
					return;
				case WeekIndex.Fourth:
					this.WeekOfMonth = 4;
					return;
				case WeekIndex.Last:
					this.WeekOfMonth = 5;
					return;
				default:
					throw new InvalidOperationException(string.Format("Unable to set invalid value {0} to WeekOfMonth.", value));
				}
			}
		}

		public static string GetEmailNamespaceForKey(int keyIndex)
		{
			if (keyIndex < RecurrenceData.calKeys.Length)
			{
				return "Email:";
			}
			if (keyIndex < RecurrenceData.calKeys141.Length)
			{
				return "Email2:";
			}
			throw new InvalidOperationException(string.Format("keyIndex value {0} is invalid.", keyIndex));
		}

		public bool HasInterval()
		{
			return this.subProperties["Interval"] != null && this.Interval != 0;
		}

		public bool HasOccurences()
		{
			return null != this.subProperties["Occurrences"];
		}

		public bool HasUntil()
		{
			return null != this.subProperties["Until"];
		}

		public bool HasWeekOfMonth()
		{
			return null != this.subProperties["WeekOfMonth"];
		}

		public void Validate()
		{
			bool flag = this.recurrenceType == TypeOfRecurrence.Calendar || (this.recurrenceType == TypeOfRecurrence.Task && !this.Regenerate);
			switch (this.Type)
			{
			case RecurrenceData.RecurrenceType.Daily:
			case RecurrenceData.RecurrenceType.Weekly:
				this.CheckNull("DayOfMonth");
				this.CheckNull("WeekOfMonth");
				this.CheckNull("MonthOfYear");
				this.CheckNull("CalendarType");
				this.CheckNull("IsLeapMonth");
				if (!flag)
				{
					this.CheckNull("DayOfWeek");
					return;
				}
				if (this.Type == RecurrenceData.RecurrenceType.Weekly)
				{
					this.CheckNotNull("DayOfWeek");
					return;
				}
				return;
			case RecurrenceData.RecurrenceType.Monthly:
				this.CheckNull("DayOfWeek");
				this.CheckNull("WeekOfMonth");
				this.CheckNull("MonthOfYear");
				this.CheckNull("IsLeapMonth");
				if (flag)
				{
					this.CheckNotNull("DayOfMonth");
					return;
				}
				this.CheckNull("DayOfMonth");
				return;
			case RecurrenceData.RecurrenceType.MonthlyTh:
				this.CheckNull("DayOfMonth");
				this.CheckNull("MonthOfYear");
				this.CheckNull("IsLeapMonth");
				if (flag)
				{
					this.CheckNotNull("WeekOfMonth");
					this.CheckNotNull("DayOfWeek");
				}
				else
				{
					this.CheckNull("WeekOfMonth");
					this.CheckNull("DayOfWeek");
				}
				if (!EnumValidator.IsValidValue<RecurrenceOrderType>(this.RecurrenceOrderType))
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth value {0} is invalid", new object[]
					{
						this.RecurrenceOrderType
					}));
				}
				return;
			case RecurrenceData.RecurrenceType.Yearly:
				this.CheckNull("DayOfWeek");
				this.CheckNull("WeekOfMonth");
				if (flag)
				{
					this.CheckNotNull("DayOfMonth");
					this.CheckNotNull("MonthOfYear");
					return;
				}
				this.CheckNull("WeekOfMonth");
				this.CheckNull("MonthOfYear");
				return;
			case RecurrenceData.RecurrenceType.YearlyTh:
				this.CheckNull("DayOfMonth");
				if (!flag)
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Unsupported recurrence {0}, should have been caught in a higher validation layer", new object[]
					{
						this.Type
					}));
				}
				this.CheckNotNull("WeekOfMonth");
				this.CheckNotNull("DayOfWeek");
				this.CheckNotNull("MonthOfYear");
				if (!EnumValidator.IsValidValue<RecurrenceOrderType>(this.RecurrenceOrderType))
				{
					throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "WeekOfMonth value {0} is invalid", new object[]
					{
						this.RecurrenceOrderType
					}));
				}
				return;
			}
			throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "Unexpected recurrence type {0}, should have been caught in a higher validation layer", new object[]
			{
				this.Type
			}));
		}

		public void Clear()
		{
			this.subProperties.Clear();
			switch (this.recurrenceType)
			{
			case TypeOfRecurrence.Calendar:
				this.formatString = "yyyyMMdd\\THHmmss\\Z";
				if (this.protocolVersion < 140)
				{
					this.keys = RecurrenceData.calKeys;
					return;
				}
				if (this.protocolVersion == 140)
				{
					this.keys = RecurrenceData.calKeys14;
					return;
				}
				this.keys = RecurrenceData.calKeys141;
				return;
			case TypeOfRecurrence.Task:
				this.formatString = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";
				if (this.protocolVersion < 140)
				{
					this.keys = RecurrenceData.taskKeys;
				}
				else if (this.protocolVersion == 140)
				{
					this.keys = RecurrenceData.taskKeys14;
				}
				else
				{
					this.keys = RecurrenceData.taskKeys141;
				}
				this.DeadOccur = false;
				this.Regenerate = false;
				return;
			default:
				return;
			}
		}

		private void CheckNull(string propertyName)
		{
			if (this.SubProperties[propertyName] != null)
			{
				throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "{0} is not expected with recurrence type {1}", new object[]
				{
					propertyName,
					this.Type
				}));
			}
		}

		private void CheckNotNull(string propertyName)
		{
			if (this.SubProperties[propertyName] == null)
			{
				throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "{0} is expected with recurrence type {1}", new object[]
				{
					propertyName,
					this.Type
				}));
			}
		}

		public const int MinProperties = 2;

		public const string CalFormatString = "yyyyMMdd\\THHmmss\\Z";

		public const string TaskFormatString = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

		private const string ConfigurationName = "OWA.UserOptions";

		private const string FirstDayOfWeekPropertyName = "weekstartday";

		private const byte lastWeekOfAMonth = 5;

		private static readonly string[] calKeys = new string[]
		{
			"Type",
			"Interval",
			"Until",
			"Occurrences",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear"
		};

		private static readonly string[] calKeys14 = new string[]
		{
			"Type",
			"Interval",
			"Until",
			"Occurrences",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear",
			"CalendarType",
			"IsLeapMonth"
		};

		private static readonly string[] calKeys141 = new string[]
		{
			"Type",
			"Interval",
			"Until",
			"Occurrences",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear",
			"CalendarType",
			"IsLeapMonth",
			"FirstDayOfWeek"
		};

		private static readonly string[] taskKeys = new string[]
		{
			"Regenerate",
			"DeadOccur",
			"Type",
			"Start",
			"Occurrences",
			"Until",
			"Interval",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear"
		};

		private static readonly string[] taskKeys14 = new string[]
		{
			"Regenerate",
			"DeadOccur",
			"Type",
			"Start",
			"Occurrences",
			"Until",
			"Interval",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear",
			"CalendarType",
			"IsLeapMonth"
		};

		private static readonly string[] taskKeys141 = new string[]
		{
			"Regenerate",
			"DeadOccur",
			"Type",
			"Start",
			"Occurrences",
			"Until",
			"Interval",
			"WeekOfMonth",
			"DayOfMonth",
			"DayOfWeek",
			"MonthOfYear",
			"CalendarType",
			"IsLeapMonth",
			"FirstDayOfWeek"
		};

		private static readonly DayOfWeekConverter dayOfWeekConverter = default(DayOfWeekConverter);

		private static readonly WeekIndexConverter weekIndexConverter = default(WeekIndexConverter);

		private int protocolVersion;

		private string formatString;

		private string[] keys;

		private IDictionary subProperties;

		private TypeOfRecurrence recurrenceType;

		public enum RecurrenceType
		{
			Daily,
			Weekly,
			Monthly,
			MonthlyTh,
			Yearly = 5,
			YearlyTh
		}
	}
}
