using System;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class HolidaySchedule
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Greeting
		{
			get
			{
				return this.introductoryGreeting;
			}
			set
			{
				this.introductoryGreeting = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return this.scheduleDate;
			}
			set
			{
				this.scheduleDate = value;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return this.endDate;
			}
			set
			{
				this.endDate = value;
			}
		}

		static HolidaySchedule()
		{
			DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
			HolidaySchedule.shortPatterns = new string[]
			{
				dateTimeFormat.ShortDatePattern
			};
		}

		private HolidaySchedule()
		{
		}

		public HolidaySchedule(string holidayName, string greeting, DateTime start, DateTime end)
		{
			this.name = holidayName;
			this.introductoryGreeting = greeting;
			this.scheduleDate = start;
			this.endDate = end;
			this.Validate();
		}

		public HolidaySchedule(PSObject importedObject)
		{
			this.name = (string)importedObject.Properties["Name"].Value;
			this.introductoryGreeting = (string)importedObject.Properties["Greeting"].Value;
			string datestring = (string)importedObject.Properties["StartDate"].Value;
			this.scheduleDate = HolidaySchedule.StringToDate(datestring, false);
			datestring = (string)importedObject.Properties["EndDate"].Value;
			this.endDate = HolidaySchedule.StringToDate(datestring, false);
			this.Validate();
		}

		public static HolidaySchedule Parse(string schedule)
		{
			return HolidaySchedule.HolidayFromString(schedule, false);
		}

		public static HolidaySchedule ParseADString(string schedule)
		{
			return HolidaySchedule.HolidayFromString(schedule, true);
		}

		private static HolidaySchedule HolidayFromString(string schedule, bool fromAD)
		{
			if (schedule == null || schedule.Length == 0)
			{
				throw new FormatException(DataStrings.InvalidHolidayScheduleFormat);
			}
			string[] array = schedule.Split(new char[]
			{
				','
			});
			if (array == null || array.Length < 3 || array.Length > 4)
			{
				throw new FormatException(DataStrings.InvalidHolidayScheduleFormat);
			}
			DateTime dateTime = DateTime.MinValue;
			dateTime = HolidaySchedule.StringToDate(array[2], fromAD);
			DateTime end = dateTime;
			if (array.Length > 3)
			{
				end = HolidaySchedule.StringToDate(array[3], fromAD);
			}
			return new HolidaySchedule(array[0], array[1], dateTime, end);
		}

		public override string ToString()
		{
			return this.HolidayToString(false);
		}

		public string ToADString()
		{
			return this.HolidayToString(true);
		}

		private string HolidayToString(bool toADString)
		{
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				this.Name,
				this.Greeting,
				this.DateToString(this.StartDate, toADString),
				this.DateToString(this.EndDate, toADString)
			});
		}

		private string DateToString(DateTime dt, bool toADString)
		{
			string result;
			if (toADString)
			{
				result = dt.ToString("yyyyMMddHHmmss'.0Z'", DateTimeFormatInfo.InvariantInfo);
			}
			else
			{
				result = dt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
			}
			return result;
		}

		public void Validate()
		{
			HolidaySchedule.ValidateName(this.name);
			HolidaySchedule.ValidateGreeting(this.introductoryGreeting);
			HolidaySchedule.ValidateSchedule(this.scheduleDate, this.endDate);
		}

		private static DateTime StringToDate(string datestring, bool fromADString)
		{
			DateTime result;
			try
			{
				if (fromADString)
				{
					result = DateTime.ParseExact(datestring, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
				}
				else
				{
					result = DateTime.ParseExact(datestring, HolidaySchedule.shortPatterns, null, DateTimeStyles.NoCurrentDateDefault);
				}
			}
			catch (FormatException innerException)
			{
				throw new FormatException(DataStrings.InvalidDateFormat(datestring, HolidaySchedule.shortPatterns[0]), innerException);
			}
			return result;
		}

		private static void ValidateName(string holidayName)
		{
			if (holidayName == null || holidayName.Length == 0)
			{
				throw new StrongTypeFormatException(DataStrings.ConstraintViolationStringLengthIsEmpty, "Name");
			}
			if (holidayName.Length > 64)
			{
				throw new StrongTypeFormatException(DataStrings.ConstraintViolationStringLengthTooLong(64, holidayName.Length), "Name");
			}
			if (holidayName.IndexOf(",") != -1)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidCharInString("HolidayName", ","), "Name");
			}
		}

		private static void ValidateGreeting(string greeting)
		{
			if (string.IsNullOrEmpty(greeting))
			{
				throw new StrongTypeFormatException(DataStrings.ConstraintViolationStringLengthIsEmpty, "Greeting");
			}
			if (greeting.IndexOf(",") != -1)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidCharInString("Greeting", ","), "Greeting");
			}
		}

		private static void ValidateSchedule(DateTime start, DateTime end)
		{
			if (start > end)
			{
				throw new FormatException(DataStrings.ScheduleDateInvalid(start, end));
			}
		}

		public override bool Equals(object obj)
		{
			HolidaySchedule holidaySchedule = obj as HolidaySchedule;
			return holidaySchedule != null && string.Equals(this.ToADString(), holidaySchedule.ToADString(), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.ToADString().ToLowerInvariant().GetHashCode();
		}

		private const int NAME = 0;

		private const int GREETING = 1;

		private const int START = 2;

		private const int END = 3;

		private const string strName = "Name";

		private const string strGreeting = "Greeting";

		private const string strStart = "StartDate";

		private const string strEnd = "EndDate";

		private string name;

		private string introductoryGreeting;

		private DateTime scheduleDate;

		private DateTime endDate;

		private static string[] shortPatterns;
	}
}
