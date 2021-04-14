using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	internal class ContactCommon
	{
		static ContactCommon()
		{
			for (int i = 0; i < ContactCommon.propertyStringTable.Length; i++)
			{
				if (ContactCommon.propertyStringTable[i] != null)
				{
					ContactCommon.propertyEnumTable.Add(ContactCommon.propertyStringTable[i], (PropertyId)i);
				}
			}
			for (int j = 0; j < ContactCommon.parameterStringTable.Length; j++)
			{
				if (ContactCommon.parameterStringTable[j] != null)
				{
					ContactCommon.parameterEnumTable.Add(ContactCommon.parameterStringTable[j], (ParameterId)j);
				}
			}
			for (int k = 0; k < ContactCommon.typeStringTable.Length; k++)
			{
				if (ContactCommon.typeStringTable[k] != null)
				{
					ContactCommon.valueEnumTable.Add(ContactCommon.typeStringTable[k], (ContactValueType)k);
				}
			}
		}

		public static string GetPropertyString(PropertyId p)
		{
			if (p < PropertyId.Unknown || (ulong)p >= (ulong)((long)ContactCommon.propertyStringTable.Length))
			{
				throw new ArgumentOutOfRangeException();
			}
			return ContactCommon.propertyStringTable[(int)((UIntPtr)p)];
		}

		public static string GetValueTypeString(ContactValueType p)
		{
			if (p < ContactValueType.Unknown || (ulong)p >= (ulong)((long)ContactCommon.typeStringTable.Length))
			{
				throw new ArgumentOutOfRangeException();
			}
			return ContactCommon.typeStringTable[(int)((UIntPtr)p)];
		}

		public static string GetParameterString(ParameterId p)
		{
			if (p < ParameterId.Unknown || (ulong)p >= (ulong)((long)ContactCommon.parameterStringTable.Length))
			{
				throw new ArgumentOutOfRangeException();
			}
			return ContactCommon.parameterStringTable[(int)((UIntPtr)p)];
		}

		public static PropertyId GetPropertyEnum(string p)
		{
			PropertyId result;
			if (ContactCommon.propertyEnumTable.TryGetValue(p, out result))
			{
				return result;
			}
			return PropertyId.Unknown;
		}

		public static ContactValueType GetValueTypeEnum(string c)
		{
			ContactValueType result;
			if (ContactCommon.valueEnumTable.TryGetValue(c, out result))
			{
				return result;
			}
			return ContactValueType.Unknown;
		}

		public static ParameterId GetParameterEnum(string p)
		{
			ParameterId result;
			if (p != null && ContactCommon.parameterEnumTable.TryGetValue(p, out result))
			{
				return result;
			}
			return ParameterId.Unknown;
		}

		public static ContactValueType GetDefaultValueType(PropertyId p)
		{
			if (p < PropertyId.Unknown || (ulong)p >= (ulong)((long)ContactCommon.defaultTypeTable.Length))
			{
				throw new ArgumentOutOfRangeException();
			}
			return ContactCommon.defaultTypeTable[(int)((UIntPtr)p)];
		}

		public static DateTime ParseDate(string s, ComplianceTracker tracker)
		{
			DateTime result;
			if (!DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) && !DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
			{
				return ContactCommon.ParseDateTime(s, tracker);
			}
			return result;
		}

		public static DateTime ParseTime(string s, ComplianceTracker tracker)
		{
			int length = s.Length;
			if (length < 6)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateTimeLength);
				return ContactCommon.MinDateTime;
			}
			string format = "HHmmss";
			int num = 6;
			if (s[2] == ':')
			{
				format = "HH:mm:ss";
				num = 8;
			}
			if (length < num)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateTimeLength);
				return ContactCommon.MinDateTime;
			}
			return ContactCommon.InternalParseDateTime(s, length, format, num, tracker);
		}

		public static DateTime ParseDateTime(string s, ComplianceTracker tracker)
		{
			int length = s.Length;
			if (length < 15)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateTimeLength);
				return ContactCommon.MinDateTime;
			}
			string text = "yyyyMMdd";
			int num = 8;
			if (s[4] == '-')
			{
				text = "yyyy-MM-dd";
				num = 10;
			}
			if (s[num + 3] == ':')
			{
				text += "\\THH:mm:ss";
				num += 9;
			}
			else
			{
				text += "\\THHmmss";
				num += 7;
			}
			if (length < num)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateTimeLength);
				return ContactCommon.MinDateTime;
			}
			return ContactCommon.InternalParseDateTime(s, length, text, num, tracker);
		}

		public static TimeSpan ParseUtcOffset(string s, ComplianceTracker tracker)
		{
			int length = s.Length;
			if (length != 5 && length != 6)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidUtcOffsetLength);
				return TimeSpan.Zero;
			}
			bool flag = false;
			if (s[0] == '-')
			{
				flag = true;
			}
			else if (s[0] != '+')
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedPlusMinus);
				return TimeSpan.Zero;
			}
			DateTime dateTime;
			if (!DateTime.TryParseExact(s.Substring(1), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) && !DateTime.TryParseExact(s.Substring(1), "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
				return TimeSpan.Zero;
			}
			TimeSpan result = new TimeSpan(dateTime.Hour, dateTime.Minute, 0);
			if (flag)
			{
				return result.Negate();
			}
			return result;
		}

		public static string FormatDate(DateTime s)
		{
			return s.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
		}

		public static string FormatDateTime(DateTime s)
		{
			string format = ContactCommon.RetrieveDateTimeFormatString(s.Millisecond != 0, s.Kind == DateTimeKind.Utc);
			return s.ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		public static string FormatTime(DateTime s)
		{
			string format = ContactCommon.RetrieveTimeFormatString(s.Millisecond != 0, s.Kind == DateTimeKind.Utc);
			return s.ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		public static string FormatUtcOffset(TimeSpan ts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
			{
				return "+00:00";
			}
			if (ts.Ticks > 0L)
			{
				stringBuilder.Append('+');
			}
			else
			{
				stringBuilder.Append('-');
				ts = ((ts == TimeSpan.MinValue) ? TimeSpan.MaxValue : ts.Negate());
			}
			if (ts.Hours < 10)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(ts.Hours.ToString());
			stringBuilder.Append(':');
			if (ts.Minutes < 10)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(ts.Minutes.ToString());
			return stringBuilder.ToString();
		}

		private static DateTime InternalParseDateTime(string s, int length, string format, int formatLength, ComplianceTracker tracker)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			if (length > formatLength)
			{
				if (s[formatLength] == ',')
				{
					int num = formatLength + 1;
					while (num < length && char.IsDigit(s[num]))
					{
						num++;
					}
					if (num == formatLength + 1)
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateFormat);
						return ContactCommon.MinDateTime;
					}
					text2 = s.Substring(formatLength + 1, num - (formatLength + 1));
					if (num < length)
					{
						text = s.Substring(num);
					}
				}
				else
				{
					text = s.Substring(formatLength);
				}
				s = s.Substring(0, formatLength);
			}
			DateTime dateTime;
			if (!DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime))
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateFormat);
				return ContactCommon.MinDateTime;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				if (text2.Length > 3)
				{
					text2 = text2.Substring(0, 3);
				}
				int num2 = 0;
				if (!int.TryParse(text2, out num2))
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateFormat);
					return ContactCommon.MinDateTime;
				}
				for (int i = text2.Length; i < 3; i++)
				{
					num2 *= 10;
				}
				dateTime += new TimeSpan(0, 0, 0, 0, num2);
			}
			if (!string.IsNullOrEmpty(text) && text != "Z")
			{
				dateTime += ContactCommon.ParseUtcOffset(text, tracker);
			}
			return dateTime;
		}

		private static string RetrieveDateTimeFormatString(bool hasNonZeroMillisecond, bool isUtc)
		{
			if (hasNonZeroMillisecond)
			{
				if (!isUtc)
				{
					return "yyyy-MM-dd\\THH:mm:ss\\,fff";
				}
				return "yyyy-MM-dd\\THH:mm:ss\\,fff\\Z";
			}
			else
			{
				if (!isUtc)
				{
					return "yyyy-MM-dd\\THH:mm:ss";
				}
				return "yyyy-MM-dd\\THH:mm:ss\\Z";
			}
		}

		private static string RetrieveTimeFormatString(bool hasNonZeroMillisecond, bool isUtc)
		{
			if (hasNonZeroMillisecond)
			{
				if (!isUtc)
				{
					return "HH:mm:ss\\,fff";
				}
				return "HH:mm:ss\\,fff\\Z";
			}
			else
			{
				if (!isUtc)
				{
					return "HH:mm:ss";
				}
				return "HH:mm:ss\\Z";
			}
		}

		private const string DateFormat = "yyyyMMdd";

		private const string DateFormatDash = "yyyy-MM-dd";

		private const string TimeSeparator = "\\T";

		private const string TimeFormat = "HHmmss";

		private const string TimeFormatColon = "HH:mm:ss";

		private const string TimeMsFormat = "\\,fff";

		private const string TimeOffset = "HHmm";

		private const string TimeOffsetColon = "HH:mm";

		private const string UtcSuffix = "\\Z";

		private const string TimeFormatColonMillisecUtc = "HH:mm:ss\\,fff\\Z";

		private const string TimeFormatColonMillisec = "HH:mm:ss\\,fff";

		private const string TimeFormatColonUtc = "HH:mm:ss\\Z";

		private const string DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss";

		private const string DateTimeFormatMillisec = "yyyy-MM-dd\\THH:mm:ss\\,fff";

		private const string DateTimeFormatMillisecUtc = "yyyy-MM-dd\\THH:mm:ss\\,fff\\Z";

		private const string DateTimeFormatUtc = "yyyy-MM-dd\\THH:mm:ss\\Z";

		private const DateTimeStyles VCardDateTimeStyle = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;

		private static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

		private static string[] propertyStringTable = new string[]
		{
			null,
			"PROFILE",
			"NAME",
			"SOURCE",
			"FN",
			"N",
			"NICKNAME",
			"PHOTO",
			"BDAY",
			"ADR",
			"LABEL",
			"TEL",
			"EMAIL",
			"MAILER",
			"TZ",
			"GEO",
			"TITLE",
			"ROLE",
			"LOGO",
			"AGENT",
			"ORG",
			"CATEGORIES",
			"NOTE",
			"PRODID",
			"REV",
			"SORT-STRING",
			"SOUND",
			"UID",
			"URL",
			"VERSION",
			"CLASS",
			"KEY"
		};

		private static string[] parameterStringTable = new string[]
		{
			null,
			"TYPE",
			"LANGUAGE",
			"ENCODING",
			"VALUE",
			"CHARSET"
		};

		private static string[] typeStringTable = new string[]
		{
			null,
			"BINARY",
			"BOOLEAN",
			"DATE",
			"DATE-TIME",
			"FLOAT",
			"INTEGER",
			"TEXT",
			"TIME",
			"URI",
			"UTC-OFFSET",
			"VCARD",
			"PHONE-NUMBER"
		};

		private static ContactValueType[] defaultTypeTable = new ContactValueType[]
		{
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Uri,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Binary,
			ContactValueType.Date,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.PhoneNumber,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.UtcOffset,
			ContactValueType.Float,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Binary,
			ContactValueType.VCard,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.DateTime,
			ContactValueType.Text,
			ContactValueType.Binary,
			ContactValueType.Text,
			ContactValueType.Uri,
			ContactValueType.Text,
			ContactValueType.Text,
			ContactValueType.Binary
		};

		private static Dictionary<string, PropertyId> propertyEnumTable = new Dictionary<string, PropertyId>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, ParameterId> parameterEnumTable = new Dictionary<string, ParameterId>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, ContactValueType> valueEnumTable = new Dictionary<string, ContactValueType>(StringComparer.OrdinalIgnoreCase);
	}
}
