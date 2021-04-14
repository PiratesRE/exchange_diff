using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	internal class CalendarCommon
	{
		static CalendarCommon()
		{
			CalendarCommon.parameterStringTable.Add(ParameterId.AlternateRepresentation, "ALTREP");
			CalendarCommon.parameterStringTable.Add(ParameterId.CommonName, "CN");
			CalendarCommon.parameterStringTable.Add(ParameterId.CalendarUserType, "CUTYPE");
			CalendarCommon.parameterStringTable.Add(ParameterId.Delegator, "DELEGATED-FROM");
			CalendarCommon.parameterStringTable.Add(ParameterId.Delegatee, "DELEGATED-TO");
			CalendarCommon.parameterStringTable.Add(ParameterId.Directory, "DIR");
			CalendarCommon.parameterStringTable.Add(ParameterId.Encoding, "ENCODING");
			CalendarCommon.parameterStringTable.Add(ParameterId.FormatType, "FMTTYPE");
			CalendarCommon.parameterStringTable.Add(ParameterId.FreeBusyType, "FBTYPE");
			CalendarCommon.parameterStringTable.Add(ParameterId.Language, "LANGUAGE");
			CalendarCommon.parameterStringTable.Add(ParameterId.Membership, "MEMBER");
			CalendarCommon.parameterStringTable.Add(ParameterId.ParticipationStatus, "PARTSTAT");
			CalendarCommon.parameterStringTable.Add(ParameterId.RecurrenceRange, "RANGE");
			CalendarCommon.parameterStringTable.Add(ParameterId.TriggerRelationship, "RELATED");
			CalendarCommon.parameterStringTable.Add(ParameterId.RelationshipType, "RELTYPE");
			CalendarCommon.parameterStringTable.Add(ParameterId.ParticipationRole, "ROLE");
			CalendarCommon.parameterStringTable.Add(ParameterId.RsvpExpectation, "RSVP");
			CalendarCommon.parameterStringTable.Add(ParameterId.SentBy, "SENT-BY");
			CalendarCommon.parameterStringTable.Add(ParameterId.TimeZoneId, "TZID");
			CalendarCommon.parameterStringTable.Add(ParameterId.ValueType, "VALUE");
			CalendarCommon.componentStringTable.Add(ComponentId.VCalendar, "VCALENDAR");
			CalendarCommon.componentStringTable.Add(ComponentId.VEvent, "VEVENT");
			CalendarCommon.componentStringTable.Add(ComponentId.VTodo, "VTODO");
			CalendarCommon.componentStringTable.Add(ComponentId.VJournal, "VJOURNAL");
			CalendarCommon.componentStringTable.Add(ComponentId.VFreeBusy, "VFREEBUSY");
			CalendarCommon.componentStringTable.Add(ComponentId.VTimeZone, "VTIMEZONE");
			CalendarCommon.componentStringTable.Add(ComponentId.VAlarm, "VALARM");
			CalendarCommon.componentStringTable.Add(ComponentId.Standard, "STANDARD");
			CalendarCommon.componentStringTable.Add(ComponentId.Daylight, "DAYLIGHT");
			CalendarCommon.componentEnumTable.Add("NONE", ComponentId.None);
			CalendarCommon.componentEnumTable.Add("VCALENDAR", ComponentId.VCalendar);
			CalendarCommon.componentEnumTable.Add("VEVENT", ComponentId.VEvent);
			CalendarCommon.componentEnumTable.Add("VTODO", ComponentId.VTodo);
			CalendarCommon.componentEnumTable.Add("VJOURNAL", ComponentId.VJournal);
			CalendarCommon.componentEnumTable.Add("VFREEBUSY", ComponentId.VFreeBusy);
			CalendarCommon.componentEnumTable.Add("VTIMEZONE", ComponentId.VTimeZone);
			CalendarCommon.componentEnumTable.Add("VALARM", ComponentId.VAlarm);
			CalendarCommon.componentEnumTable.Add("STANDARD", ComponentId.Standard);
			CalendarCommon.componentEnumTable.Add("DAYLIGHT", ComponentId.Daylight);
			CalendarCommon.propertyEnumTable.Add("PRODID", PropertyId.ProductId);
			CalendarCommon.propertyEnumTable.Add("VERSION", PropertyId.Version);
			CalendarCommon.propertyEnumTable.Add("CALSCALE", PropertyId.CalendarScale);
			CalendarCommon.propertyEnumTable.Add("METHOD", PropertyId.Method);
			CalendarCommon.propertyEnumTable.Add("ATTACH", PropertyId.Attachment);
			CalendarCommon.propertyEnumTable.Add("CATEGORIES", PropertyId.Categories);
			CalendarCommon.propertyEnumTable.Add("CLASS", PropertyId.Class);
			CalendarCommon.propertyEnumTable.Add("COMMENT", PropertyId.Comment);
			CalendarCommon.propertyEnumTable.Add("DESCRIPTION", PropertyId.Description);
			CalendarCommon.propertyEnumTable.Add("GEO", PropertyId.GeographicPosition);
			CalendarCommon.propertyEnumTable.Add("LOCATION", PropertyId.Location);
			CalendarCommon.propertyEnumTable.Add("PERCENT-COMPLETE", PropertyId.PercentComplete);
			CalendarCommon.propertyEnumTable.Add("PRIORITY", PropertyId.Priority);
			CalendarCommon.propertyEnumTable.Add("RESOURCES", PropertyId.Resources);
			CalendarCommon.propertyEnumTable.Add("STATUS", PropertyId.Status);
			CalendarCommon.propertyEnumTable.Add("SUMMARY", PropertyId.Summary);
			CalendarCommon.propertyEnumTable.Add("COMPLETED", PropertyId.Completed);
			CalendarCommon.propertyEnumTable.Add("DTEND", PropertyId.DateTimeEnd);
			CalendarCommon.propertyEnumTable.Add("DUE", PropertyId.DateTimeDue);
			CalendarCommon.propertyEnumTable.Add("DTSTART", PropertyId.DateTimeStart);
			CalendarCommon.propertyEnumTable.Add("DURATION", PropertyId.Duration);
			CalendarCommon.propertyEnumTable.Add("FREEBUSY", PropertyId.FreeBusy);
			CalendarCommon.propertyEnumTable.Add("TRANSP", PropertyId.Transparency);
			CalendarCommon.propertyEnumTable.Add("TZID", PropertyId.TimeZoneId);
			CalendarCommon.propertyEnumTable.Add("TZNAME", PropertyId.TimeZoneName);
			CalendarCommon.propertyEnumTable.Add("TZOFFSETFROM", PropertyId.TimeZoneOffsetFrom);
			CalendarCommon.propertyEnumTable.Add("TZOFFSETTO", PropertyId.TimeZoneOffsetTo);
			CalendarCommon.propertyEnumTable.Add("TZURL", PropertyId.TimeZoneUrl);
			CalendarCommon.propertyEnumTable.Add("ATTENDEE", PropertyId.Attendee);
			CalendarCommon.propertyEnumTable.Add("CONTACT", PropertyId.Contact);
			CalendarCommon.propertyEnumTable.Add("ORGANIZER", PropertyId.Organizer);
			CalendarCommon.propertyEnumTable.Add("RECURRENCE-ID", PropertyId.RecurrenceId);
			CalendarCommon.propertyEnumTable.Add("RELATED-TO", PropertyId.RelatedTo);
			CalendarCommon.propertyEnumTable.Add("URL", PropertyId.Url);
			CalendarCommon.propertyEnumTable.Add("UID", PropertyId.Uid);
			CalendarCommon.propertyEnumTable.Add("EXDATE", PropertyId.ExceptionDate);
			CalendarCommon.propertyEnumTable.Add("EXRULE", PropertyId.ExceptionRule);
			CalendarCommon.propertyEnumTable.Add("RDATE", PropertyId.RecurrenceDate);
			CalendarCommon.propertyEnumTable.Add("RRULE", PropertyId.RecurrenceRule);
			CalendarCommon.propertyEnumTable.Add("ACTION", PropertyId.Action);
			CalendarCommon.propertyEnumTable.Add("REPEAT", PropertyId.Repeat);
			CalendarCommon.propertyEnumTable.Add("TRIGGER", PropertyId.Trigger);
			CalendarCommon.propertyEnumTable.Add("CREATED", PropertyId.Created);
			CalendarCommon.propertyEnumTable.Add("DTSTAMP", PropertyId.DateTimeStamp);
			CalendarCommon.propertyEnumTable.Add("LAST-MODIFIED", PropertyId.LastModified);
			CalendarCommon.propertyEnumTable.Add("SEQUENCE", PropertyId.Sequence);
			CalendarCommon.propertyEnumTable.Add("REQUEST-STATUS", PropertyId.RequestStatus);
			CalendarCommon.parameterEnumTable.Add("ALTREP", ParameterId.AlternateRepresentation);
			CalendarCommon.parameterEnumTable.Add("CN", ParameterId.CommonName);
			CalendarCommon.parameterEnumTable.Add("CUTYPE", ParameterId.CalendarUserType);
			CalendarCommon.parameterEnumTable.Add("DELEGATED-FROM", ParameterId.Delegator);
			CalendarCommon.parameterEnumTable.Add("DELEGATED-TO", ParameterId.Delegatee);
			CalendarCommon.parameterEnumTable.Add("DIR", ParameterId.Directory);
			CalendarCommon.parameterEnumTable.Add("ENCODING", ParameterId.Encoding);
			CalendarCommon.parameterEnumTable.Add("FMTTYPE", ParameterId.FormatType);
			CalendarCommon.parameterEnumTable.Add("FBTYPE", ParameterId.FreeBusyType);
			CalendarCommon.parameterEnumTable.Add("LANGUAGE", ParameterId.Language);
			CalendarCommon.parameterEnumTable.Add("MEMBER", ParameterId.Membership);
			CalendarCommon.parameterEnumTable.Add("PARTSTAT", ParameterId.ParticipationStatus);
			CalendarCommon.parameterEnumTable.Add("RANGE", ParameterId.RecurrenceRange);
			CalendarCommon.parameterEnumTable.Add("RELATED", ParameterId.TriggerRelationship);
			CalendarCommon.parameterEnumTable.Add("RELTYPE", ParameterId.RelationshipType);
			CalendarCommon.parameterEnumTable.Add("ROLE", ParameterId.ParticipationRole);
			CalendarCommon.parameterEnumTable.Add("RSVP", ParameterId.RsvpExpectation);
			CalendarCommon.parameterEnumTable.Add("SENT-BY", ParameterId.SentBy);
			CalendarCommon.parameterEnumTable.Add("TZID", ParameterId.TimeZoneId);
			CalendarCommon.parameterEnumTable.Add("VALUE", ParameterId.ValueType);
			CalendarCommon.valueEnumTable.Add("BINARY", CalendarValueType.Binary);
			CalendarCommon.valueEnumTable.Add("BOOLEAN", CalendarValueType.Boolean);
			CalendarCommon.valueEnumTable.Add("CAL-ADDRESS", CalendarValueType.CalAddress);
			CalendarCommon.valueEnumTable.Add("DATE", CalendarValueType.Date);
			CalendarCommon.valueEnumTable.Add("DATE-TIME", CalendarValueType.DateTime);
			CalendarCommon.valueEnumTable.Add("DURATION", CalendarValueType.Duration);
			CalendarCommon.valueEnumTable.Add("FLOAT", CalendarValueType.Float);
			CalendarCommon.valueEnumTable.Add("INTEGER", CalendarValueType.Integer);
			CalendarCommon.valueEnumTable.Add("PERIOD", CalendarValueType.Period);
			CalendarCommon.valueEnumTable.Add("RECUR", CalendarValueType.Recurrence);
			CalendarCommon.valueEnumTable.Add("TEXT", CalendarValueType.Text);
			CalendarCommon.valueEnumTable.Add("TIME", CalendarValueType.Time);
			CalendarCommon.valueEnumTable.Add("URI", CalendarValueType.Uri);
			CalendarCommon.valueEnumTable.Add("UTC-OFFSET", CalendarValueType.UtcOffset);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Attachment, CalendarValueType.Uri);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.PercentComplete, CalendarValueType.Integer);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Priority, CalendarValueType.Integer);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Completed, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeEnd, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeStart, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeDue, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Duration, CalendarValueType.Duration);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.FreeBusy, CalendarValueType.Period);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneOffsetFrom, CalendarValueType.UtcOffset);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneOffsetTo, CalendarValueType.UtcOffset);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneUrl, CalendarValueType.Uri);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Attendee, CalendarValueType.CalAddress);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Organizer, CalendarValueType.CalAddress);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceId, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Url, CalendarValueType.Uri);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.ExceptionDate, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.ExceptionRule, CalendarValueType.Recurrence);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceDate, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceRule, CalendarValueType.Recurrence);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Repeat, CalendarValueType.Integer);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Trigger, CalendarValueType.Duration);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Created, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeStamp, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.LastModified, CalendarValueType.DateTime);
			CalendarCommon.defaultValueTypeTable.Add(PropertyId.Sequence, CalendarValueType.Integer);
		}

		public static string GetPropertyString(PropertyId p)
		{
			if (p < PropertyId.Unknown || (ulong)p >= (ulong)((long)CalendarCommon.propertyStringTable.Length))
			{
				return null;
			}
			return CalendarCommon.propertyStringTable[(int)((UIntPtr)p)];
		}

		public static string GetComponentString(ComponentId c)
		{
			return CalendarCommon.componentStringTable[c];
		}

		public static string GetParameterString(ParameterId p)
		{
			return CalendarCommon.parameterStringTable[p];
		}

		public static PropertyId GetPropertyEnum(string p)
		{
			PropertyId result;
			if (CalendarCommon.propertyEnumTable.TryGetValue(p, out result))
			{
				return result;
			}
			return PropertyId.Unknown;
		}

		public static ComponentId GetComponentEnum(string c)
		{
			ComponentId result;
			if (CalendarCommon.componentEnumTable.TryGetValue(c, out result))
			{
				return result;
			}
			return ComponentId.Unknown;
		}

		public static ParameterId GetParameterEnum(string p)
		{
			ParameterId result;
			if (CalendarCommon.parameterEnumTable.TryGetValue(p, out result))
			{
				return result;
			}
			return ParameterId.Unknown;
		}

		public static CalendarValueType GetValueTypeEnum(string v)
		{
			CalendarValueType result;
			if (CalendarCommon.valueEnumTable.TryGetValue(v, out result))
			{
				return result;
			}
			return CalendarValueType.Unknown;
		}

		public static CalendarValueType GetDefaultValueType(PropertyId p)
		{
			CalendarValueType result;
			if (CalendarCommon.defaultValueTypeTable.TryGetValue(p, out result))
			{
				return result;
			}
			return CalendarValueType.Text;
		}

		public static DateTime ParseDate(string s, ComplianceTracker tracker)
		{
			DateTime result;
			if (!DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateFormat);
				return CalendarCommon.MinDateTime;
			}
			return result;
		}

		public static DateTime ParseDateTime(string s, ComplianceTracker tracker)
		{
			int length = s.Length;
			if (length != 15 && length != 16)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidDateTimeLength);
				return CalendarCommon.MinDateTime;
			}
			DateTime dateTime = CalendarCommon.ParseDate(s.Substring(0, 8), tracker);
			if (s[8] != 'T')
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedTAfterDate);
				return CalendarCommon.MinDateTime;
			}
			CalendarTime calendarTime = new CalendarTime(s.Substring(9), tracker);
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, calendarTime.Time.Hours, calendarTime.Time.Minutes, calendarTime.Time.Seconds, calendarTime.IsUtc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
		}

		public static TimeSpan ParseDuration(string s, ComplianceTracker tracker)
		{
			int num = 0;
			int length = s.Length;
			CalendarCommon.DurationParseStates durationParseStates = CalendarCommon.DurationParseStates.Start;
			StringBuilder stringBuilder = null;
			int num2 = 0;
			int num3 = 0;
			int hours = 0;
			int minutes = 0;
			int seconds = 0;
			bool negative = false;
			while (num < length && durationParseStates != CalendarCommon.DurationParseStates.End)
			{
				char c = s[num];
				if ((int)c >= ContentLineParser.Dictionary.Length)
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
					return TimeSpan.Zero;
				}
				switch (durationParseStates)
				{
				case CalendarCommon.DurationParseStates.Start:
					if (c == 'P')
					{
						durationParseStates = CalendarCommon.DurationParseStates.S1;
					}
					else if (c == '+')
					{
						durationParseStates = CalendarCommon.DurationParseStates.Sign;
					}
					else
					{
						if (c != '-')
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
						negative = true;
						durationParseStates = CalendarCommon.DurationParseStates.Sign;
					}
					break;
				case CalendarCommon.DurationParseStates.Sign:
					if (c != 'P')
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedP);
						return TimeSpan.Zero;
					}
					durationParseStates = CalendarCommon.DurationParseStates.S1;
					break;
				case CalendarCommon.DurationParseStates.S1:
					if ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) != 0)
					{
						stringBuilder = new StringBuilder();
						stringBuilder.Append(c);
						durationParseStates = CalendarCommon.DurationParseStates.S2;
					}
					else
					{
						if (c != 'T')
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
						durationParseStates = CalendarCommon.DurationParseStates.T1;
					}
					break;
				case CalendarCommon.DurationParseStates.S2:
					while ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) != 0)
					{
						stringBuilder.Append(c);
						if (++num == length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedWOrD);
							return TimeSpan.Zero;
						}
						c = s[num];
						if ((int)c >= ContentLineParser.Dictionary.Length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
					}
					if (c == 'W')
					{
						if (!int.TryParse(stringBuilder.ToString(), out num3))
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
						}
						durationParseStates = CalendarCommon.DurationParseStates.End;
					}
					else
					{
						if (c != 'D')
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
						if (!int.TryParse(stringBuilder.ToString(), out num2))
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
						}
						durationParseStates = CalendarCommon.DurationParseStates.D1;
					}
					break;
				case CalendarCommon.DurationParseStates.D1:
					if (c != 'T')
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedT);
						return TimeSpan.Zero;
					}
					durationParseStates = CalendarCommon.DurationParseStates.T1;
					break;
				case CalendarCommon.DurationParseStates.T1:
					if ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) == 0)
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
						return TimeSpan.Zero;
					}
					stringBuilder = new StringBuilder();
					stringBuilder.Append(c);
					durationParseStates = CalendarCommon.DurationParseStates.T2;
					break;
				case CalendarCommon.DurationParseStates.T2:
					while ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) != 0)
					{
						stringBuilder.Append(c);
						if (++num == length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedHMS);
							return TimeSpan.Zero;
						}
						c = s[num];
						if ((int)c >= ContentLineParser.Dictionary.Length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
					}
					if (c == 'H')
					{
						if (!int.TryParse(stringBuilder.ToString(), out hours))
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
						}
						durationParseStates = CalendarCommon.DurationParseStates.H1;
					}
					else if (c == 'M')
					{
						if (!int.TryParse(stringBuilder.ToString(), out minutes))
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
						}
						durationParseStates = CalendarCommon.DurationParseStates.M1;
					}
					else
					{
						if (c != 'S')
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
						if (!int.TryParse(stringBuilder.ToString(), out seconds))
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
						}
						durationParseStates = CalendarCommon.DurationParseStates.End;
					}
					break;
				case CalendarCommon.DurationParseStates.H1:
					if ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) == 0)
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
						return TimeSpan.Zero;
					}
					stringBuilder = new StringBuilder();
					stringBuilder.Append(c);
					durationParseStates = CalendarCommon.DurationParseStates.H2;
					break;
				case CalendarCommon.DurationParseStates.H2:
					while ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) != 0)
					{
						stringBuilder.Append(c);
						if (++num == length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedM);
							return TimeSpan.Zero;
						}
						c = s[num];
						if ((int)c >= ContentLineParser.Dictionary.Length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
					}
					if (c != 'M')
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
						return TimeSpan.Zero;
					}
					if (!int.TryParse(stringBuilder.ToString(), out minutes))
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
					}
					durationParseStates = CalendarCommon.DurationParseStates.M1;
					break;
				case CalendarCommon.DurationParseStates.M1:
					if ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) == 0)
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
						return TimeSpan.Zero;
					}
					stringBuilder = new StringBuilder();
					stringBuilder.Append(c);
					durationParseStates = CalendarCommon.DurationParseStates.M2;
					break;
				case CalendarCommon.DurationParseStates.M2:
					while ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) != 0)
					{
						stringBuilder.Append(c);
						if (++num == length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedS);
							return TimeSpan.Zero;
						}
						c = s[num];
						if ((int)c >= ContentLineParser.Dictionary.Length)
						{
							tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
							return TimeSpan.Zero;
						}
					}
					if (c != 'S')
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
						return TimeSpan.Zero;
					}
					if (!int.TryParse(stringBuilder.ToString(), out seconds))
					{
						tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
					}
					durationParseStates = CalendarCommon.DurationParseStates.End;
					break;
				default:
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
					return TimeSpan.Zero;
				}
				num++;
			}
			if (num != length)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.DurationDataNotEndedProperly);
				return TimeSpan.Zero;
			}
			if (num3 != 0)
			{
				num2 += num3 * 7;
			}
			return CalendarCommon.CreateTimeSpan(tracker, num2, hours, minutes, seconds, negative);
		}

		public static TimeSpan ParseUtcOffset(string s, ComplianceTracker tracker)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int length = s.Length;
			if (length != 5 && length != 7)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidUtcOffsetLength);
				return TimeSpan.Zero;
			}
			char c = s[0];
			bool flag;
			if (c == '+')
			{
				flag = false;
			}
			else
			{
				if (c != '-')
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedPlusMinus);
					return TimeSpan.Zero;
				}
				flag = true;
			}
			for (int i = 1; i < length; i++)
			{
				c = s[i];
				if ((int)c >= ContentLineParser.Dictionary.Length)
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidCharacter);
					return TimeSpan.Zero;
				}
				if ((byte)(ContentLineParser.Tokens.Digit & ContentLineParser.Dictionary[(int)c]) == 0)
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidToken);
					return TimeSpan.Zero;
				}
			}
			if (!int.TryParse(s.Substring(1, 2), out num) || num < 0 || num > 23)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			if (!int.TryParse(s.Substring(3, 2), out num2) || num2 < 0 || num2 > 59)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			if (length == 7 && (!int.TryParse(s.Substring(5, 2), out num3) || num3 < 0 || num3 > 59))
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			if (num == 0 && num2 == 0 && num3 == 0 && flag)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return CalendarCommon.CreateTimeSpan(tracker, 0, num, num2, num3, flag);
		}

		public static string FormatDate(DateTime s)
		{
			return s.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
		}

		public static string FormatDateTime(DateTime s)
		{
			return s.ToString((s.Kind == DateTimeKind.Utc) ? "yyyyMMdd\\THHmmss\\Z" : "yyyyMMdd\\THHmmss", CultureInfo.InvariantCulture);
		}

		public static string FormatDuration(TimeSpan ts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ts.Ticks < 0L)
			{
				stringBuilder.Append('-');
				ts = ((ts == TimeSpan.MinValue) ? TimeSpan.MaxValue : ts.Negate());
			}
			stringBuilder.Append('P');
			if (ts.Days >= 7 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0 && ts.Days % 7 == 0)
			{
				stringBuilder.Append((ts.Days / 7).ToString());
				stringBuilder.Append("W");
			}
			else
			{
				if (ts.Days > 0)
				{
					stringBuilder.Append(ts.Days.ToString());
					stringBuilder.Append('D');
				}
				if (ts.Hours != 0 || ts.Minutes != 0 || ts.Seconds != 0)
				{
					stringBuilder.Append('T');
				}
				if (ts.Hours != 0)
				{
					stringBuilder.Append(ts.Hours.ToString());
					stringBuilder.Append('H');
					if (ts.Minutes != 0 || ts.Seconds != 0)
					{
						stringBuilder.Append(ts.Minutes.ToString());
						stringBuilder.Append('M');
						if (ts.Seconds != 0)
						{
							stringBuilder.Append(ts.Seconds.ToString());
							stringBuilder.Append('S');
						}
					}
				}
				else if (ts.Minutes != 0)
				{
					stringBuilder.Append(ts.Minutes.ToString());
					stringBuilder.Append('M');
					if (ts.Seconds != 0)
					{
						stringBuilder.Append(ts.Seconds.ToString());
						stringBuilder.Append('S');
					}
				}
				else if (ts.Seconds != 0)
				{
					stringBuilder.Append(ts.Seconds.ToString());
					stringBuilder.Append('S');
				}
			}
			return stringBuilder.ToString();
		}

		public static string FormatUtcOffset(TimeSpan ts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
			{
				return "+0000";
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
			if (ts.Minutes < 10)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(ts.Minutes.ToString());
			if (ts.Seconds != 0)
			{
				if (ts.Seconds < 10)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(ts.Seconds.ToString());
			}
			return stringBuilder.ToString();
		}

		private static TimeSpan CreateTimeSpan(ComplianceTracker tracker, int days, int hours, int minutes, int seconds, bool negative)
		{
			TimeSpan result;
			try
			{
				result = new TimeSpan(days, hours, minutes, seconds);
			}
			catch (ArgumentOutOfRangeException)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidTimeFormat);
				return TimeSpan.Zero;
			}
			if (negative)
			{
				result = result.Negate();
			}
			return result;
		}

		private const string DateFormat = "yyyyMMdd";

		private const string DateTimeFormat = "yyyyMMdd\\THHmmss";

		private const string DateTimeFormatUtc = "yyyyMMdd\\THHmmss\\Z";

		internal static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

		private static Dictionary<ParameterId, string> parameterStringTable = new Dictionary<ParameterId, string>(new CalendarCommon.ParameterIdComparer());

		private static Dictionary<ComponentId, string> componentStringTable = new Dictionary<ComponentId, string>(new CalendarCommon.ComponentIdComparer());

		private static Dictionary<PropertyId, CalendarValueType> defaultValueTypeTable = new Dictionary<PropertyId, CalendarValueType>(new CalendarCommon.PropertyIdComparer());

		private static string[] propertyStringTable = new string[]
		{
			null,
			"PRODID",
			"VERSION",
			"CALSCALE",
			"METHOD",
			"ATTACH",
			"CATEGORIES",
			"CLASS",
			"COMMENT",
			"DESCRIPTION",
			"GEO",
			"LOCATION",
			"PERCENT-COMPLETE",
			"PRIORITY",
			"RESOURCES",
			"STATUS",
			"SUMMARY",
			"COMPLETED",
			"DTEND",
			"DUE",
			"DTSTART",
			"DURATION",
			"FREEBUSY",
			"TRANSP",
			"TZID",
			"TZNAME",
			"TZOFFSETFROM",
			"TZOFFSETTO",
			"TZURL",
			"ATTENDEE",
			"CONTACT",
			"ORGANIZER",
			"RECURRENCE-ID",
			"RELATED-TO",
			"URL",
			"UID",
			"EXDATE",
			"EXRULE",
			"RDATE",
			"RRULE",
			"ACTION",
			"REPEAT",
			"TRIGGER",
			"CREATED",
			"DTSTAMP",
			"LAST-MODIFIED",
			"SEQUENCE",
			"REQUEST-STATUS"
		};

		private static Dictionary<string, ComponentId> componentEnumTable = new Dictionary<string, ComponentId>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, PropertyId> propertyEnumTable = new Dictionary<string, PropertyId>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, ParameterId> parameterEnumTable = new Dictionary<string, ParameterId>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, CalendarValueType> valueEnumTable = new Dictionary<string, CalendarValueType>(StringComparer.OrdinalIgnoreCase);

		internal enum DurationParseStates
		{
			Start,
			Sign,
			S1,
			S2,
			D1,
			T1,
			T2,
			H1,
			H2,
			M1,
			M2,
			End
		}

		private class ComponentIdComparer : IEqualityComparer<ComponentId>
		{
			public bool Equals(ComponentId x, ComponentId y)
			{
				return x == y;
			}

			public int GetHashCode(ComponentId obj)
			{
				return (int)obj;
			}
		}

		private class PropertyIdComparer : IEqualityComparer<PropertyId>
		{
			public bool Equals(PropertyId x, PropertyId y)
			{
				return x == y;
			}

			public int GetHashCode(PropertyId obj)
			{
				return (int)obj;
			}
		}

		private class ParameterIdComparer : IEqualityComparer<ParameterId>
		{
			public bool Equals(ParameterId x, ParameterId y)
			{
				return x == y;
			}

			public int GetHashCode(ParameterId obj)
			{
				return (int)obj;
			}
		}
	}
}
