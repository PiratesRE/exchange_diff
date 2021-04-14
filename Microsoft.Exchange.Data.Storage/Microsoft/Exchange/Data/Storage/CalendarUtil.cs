using System;
using System.Globalization;
using Microsoft.Exchange.Calendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class CalendarUtil
	{
		internal static bool? BooleanFromString(string value)
		{
			if (string.Compare(value, "TRUE", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return new bool?(true);
			}
			if (string.Compare(value, "FALSE", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return new bool?(false);
			}
			return null;
		}

		internal static string RemoveDoubleQuotes(string text)
		{
			return text.Replace("\"", string.Empty);
		}

		internal static string AddMailToPrefix(string emailAddress)
		{
			if (emailAddress.ToUpperInvariant().StartsWith("MAILTO:"))
			{
				return emailAddress;
			}
			return "MAILTO:" + emailAddress;
		}

		internal static string RemoveMailToPrefix(string emailAddress)
		{
			if (emailAddress.ToUpperInvariant().StartsWith("MAILTO:"))
			{
				return emailAddress.Substring("MAILTO:".Length);
			}
			return emailAddress;
		}

		internal static BusyType BusyTypeFromString(string busyStatusString)
		{
			return CalendarUtil.BusyTypeFromStringOrDefault(busyStatusString, BusyType.Busy);
		}

		internal static BusyType BusyTypeFromStringOrDefault(string busyStatusString, BusyType defaultValue)
		{
			if (string.Compare(busyStatusString, "FREE", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return BusyType.Free;
			}
			if (string.Compare(busyStatusString, "TENTATIVE", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return BusyType.Tentative;
			}
			if (string.Compare(busyStatusString, "OOF", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return BusyType.OOF;
			}
			return defaultValue;
		}

		internal static BusyType BusyTypeFromTranspStringOrDefault(string transpString, BusyType defaultValue)
		{
			if (string.Compare(transpString, "TRANSPARENT", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return BusyType.Free;
			}
			if (string.Compare(transpString, "OPAQUE", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return BusyType.Busy;
			}
			return defaultValue;
		}

		internal static string BusyTypeToString(BusyType busyType)
		{
			switch (busyType)
			{
			case BusyType.Free:
			case BusyType.WorkingElseWhere:
				return "FREE";
			case BusyType.Tentative:
				return "TENTATIVE";
			case BusyType.OOF:
				return "OOF";
			}
			return "BUSY";
		}

		internal static string ParticipationStatusFromItemClass(string itemClass)
		{
			string result = "NEEDS-ACTION";
			if (string.Compare(itemClass, "IPM.Schedule.Meeting.Resp.Pos", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				result = "ACCEPTED";
			}
			else if (string.Compare(itemClass, "IPM.Schedule.Meeting.Resp.Neg", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				result = "DECLINED";
			}
			else if (string.Compare(itemClass, "IPM.Schedule.Meeting.Resp.Tent", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				result = "TENTATIVE";
			}
			return result;
		}

		internal static string ItemClassFromParticipationStatus(string status)
		{
			string result = "IPM.Schedule.Meeting.Resp.Tent";
			if (status != null)
			{
				if (string.Compare(status, "ACCEPTED", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = "IPM.Schedule.Meeting.Resp.Pos";
				}
				else if (string.Compare(status, "DECLINED", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = "IPM.Schedule.Meeting.Resp.Neg";
				}
				else if (string.Compare(status, "TENTATIVE", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = "IPM.Schedule.Meeting.Resp.Tent";
				}
			}
			return result;
		}

		internal static string ItemClassFromMethod(CalendarMethod method)
		{
			string result = null;
			if (method <= CalendarMethod.Cancel)
			{
				switch (method)
				{
				case CalendarMethod.Publish:
					result = "IPM.Appointment";
					break;
				case CalendarMethod.Request:
					result = "IPM.Schedule.Meeting.Request";
					break;
				case CalendarMethod.Publish | CalendarMethod.Request:
				case CalendarMethod.Reply:
					break;
				default:
					if (method == CalendarMethod.Cancel)
					{
						result = "IPM.Schedule.Meeting.Canceled";
					}
					break;
				}
			}
			else if (method != CalendarMethod.Refresh)
			{
				if (method != CalendarMethod.Counter)
				{
				}
			}
			else
			{
				result = "IPM.Note";
			}
			return result;
		}

		internal static ResponseType ResponseTypeFromParticipationStatus(string status)
		{
			ResponseType result = ResponseType.None;
			if (status != null)
			{
				if (string.Compare(status, "ACCEPTED", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = ResponseType.Accept;
				}
				else if (string.Compare(status, "DECLINED", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = ResponseType.Decline;
				}
				else if (string.Compare(status, "TENTATIVE", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					result = ResponseType.Tentative;
				}
			}
			return result;
		}

		internal static string ParticipationStatusFromResponseType(ResponseType responseType)
		{
			string result = "NEEDS-ACTION";
			switch (responseType)
			{
			case ResponseType.Tentative:
				result = "TENTATIVE";
				break;
			case ResponseType.Accept:
				result = "ACCEPTED";
				break;
			case ResponseType.Decline:
				result = "DECLINED";
				break;
			}
			return result;
		}

		internal static bool CanConvertToMeetingMessage(Item item)
		{
			return CalendarUtil.GetICalMethod(item) != CalendarMethod.None || ObjectClass.IsFailedInboundICal(item.ClassName);
		}

		internal static CalendarMethod GetICalMethod(Item item)
		{
			CalendarMethod result = CalendarMethod.None;
			string className = item.ClassName;
			if (ObjectClass.IsMeetingRequest(className))
			{
				result = CalendarMethod.Request;
			}
			else if (ObjectClass.IsMeetingCancellation(className))
			{
				result = CalendarMethod.Cancel;
			}
			else if (ObjectClass.IsMeetingResponse(className))
			{
				if (item.GetValueOrDefault<bool>(InternalSchema.AppointmentCounterProposal))
				{
					result = CalendarMethod.Counter;
				}
				else
				{
					result = CalendarMethod.Reply;
				}
			}
			else if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(className))
			{
				result = CalendarMethod.Publish;
			}
			return result;
		}

		internal static bool IsReplyOrCounter(CalendarMethod calendarMethod)
		{
			return calendarMethod == CalendarMethod.Reply || calendarMethod == CalendarMethod.Counter;
		}

		internal static string CalendarMethodToString(CalendarMethod calendarMethod)
		{
			switch (calendarMethod)
			{
			case CalendarMethod.Publish:
				return "PUBLISH";
			case CalendarMethod.Request:
				return "REQUEST";
			case CalendarMethod.Publish | CalendarMethod.Request:
				break;
			case CalendarMethod.Reply:
				return "REPLY";
			default:
				if (calendarMethod == CalendarMethod.Cancel)
				{
					return "CANCEL";
				}
				if (calendarMethod == CalendarMethod.Counter)
				{
					return "COUNTER";
				}
				break;
			}
			throw new ArgumentException("Not supported method: " + calendarMethod.ToString());
		}

		internal static string CalendarTypeToString(CalendarType calendarType)
		{
			EnumValidator<CalendarType>.ThrowIfInvalid(calendarType, "calendarType");
			return calendarType.ToString().ToUpper();
		}

		internal static CalendarType? CalendarTypeFromString(string value)
		{
			CalendarType value2;
			if (EnumValidator<CalendarType>.TryParse(value, EnumParseOptions.IgnoreCase, out value2))
			{
				return new CalendarType?(value2);
			}
			return null;
		}

		internal static string GetSubjectFromFreeBusyStatus(BusyType busyType, CultureInfo culture)
		{
			switch (busyType)
			{
			case BusyType.Free:
				return ClientStrings.Free.ToString(culture);
			case BusyType.Tentative:
				return ClientStrings.Tentative.ToString(culture);
			case BusyType.Busy:
				return ClientStrings.Busy.ToString(culture);
			case BusyType.OOF:
				return ClientStrings.OOF.ToString(culture);
			default:
				ExTraceGlobals.ICalTracer.TraceDebug<BusyType>(0L, "CalendarUtil::GetSubjectFromFreeBusy. Found invalid BusyType '{0}'.", busyType);
				return ClientStrings.Tentative.ToString(culture);
			}
		}

		internal static ICalendarIcalConversionSettings GetCalendarIcalConversionSettings()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			return snapshot.DataStorage.CalendarIcalConversionSettings;
		}

		internal const string UpperCaseMailToPrefix = "MAILTO:";

		internal static string NotSupportedInboundIcal = "not supported calendar message.ics";

		internal static class StandardICalTokens
		{
			internal const string ICalRule = "RRULE";

			internal const string ICalExDate = "EXDATE";

			internal const string ICalRecur = "RECUR";

			internal const string ICalDateTime = "DATE-TIME";

			internal const string AltRep = "ALTREP";

			internal const string ByDayList = "BYDAYLIST";
		}

		internal static class XICalProperties
		{
			internal const string ExchangeApptId = "X-MICROSOFT-CDO-OWNERAPPTID";

			internal const string ExchangeBusyStatus = "X-MICROSOFT-CDO-BUSYSTATUS";

			internal const string ExchangeCharmId = "X-MICROSOFT-CHARMID";

			internal const string ExchangeIntendedStatus = "X-MICROSOFT-CDO-INTENDEDSTATUS";

			internal const string ExchangeIsOrganizer = "X-MICROSOFT-ISORGANIZER";

			internal const string ExchangeAllDayEvent = "X-MICROSOFT-CDO-ALLDAYEVENT";

			internal const string ExchangeImportance = "X-MICROSOFT-CDO-IMPORTANCE";

			internal const string ExchangeInstanceType = "X-MICROSOFT-CDO-INSTTYPE";

			internal const string ExchangeApptSequence = "X-MICROSOFT-CDO-APPT-SEQUENCE";

			internal const string ExchangeCalScale = "X-MICROSOFT-CALSCALE";

			internal const string ExchangeRule = "X-MICROSOFT-RRULE";

			internal const string ExchangeExDate = "X-MICROSOFT-EXDATE";

			internal const string ExchangeIsLeap = "X-MICROSOFT-ISLEAPMONTH";

			internal const string ExchangeDisallowCounter = "X-MICROSOFT-DISALLOW-COUNTER";

			internal const string XAltDesc = "X-ALT-DESC";

			internal const string XMsOlkOriginalStart = "X-MS-OLK-ORIGINALSTART";

			internal const string XMsOlkOriginalEnd = "X-MS-OLK-ORIGINALEND";

			internal const string XCalendarName = "X-WR-CALNAME";
		}

		internal static class ICalClass
		{
			internal const string ClassPublic = "PUBLIC";

			internal const string ClassPrivate = "PRIVATE";

			internal const string ClassPersonal = "PERSONAL";

			internal const string ClassConfidential = "CONFIDENTIAL";
		}

		internal static class ICalStatus
		{
			internal const string StatusTentative = "TENTATIVE";

			internal const string StatusConfirmed = "CONFIRMED";

			internal const string StatusCancelled = "CANCELLED";
		}

		internal static class ICalBusyType
		{
			internal const string BusyTypeFree = "FREE";

			internal const string BusyTypeTentative = "TENTATIVE";

			internal const string BusyTypeOof = "OOF";

			internal const string BusyTypeBusy = "BUSY";
		}

		internal static class ICalTransp
		{
			internal const string TranspTransparent = "TRANSPARENT";

			internal const string TranspOpaque = "OPAQUE";
		}

		internal static class ICalMethod
		{
			internal const string MethodPublish = "PUBLISH";

			internal const string MethodRequest = "REQUEST";

			internal const string MethodReply = "REPLY";

			internal const string MethodCancel = "CANCEL";

			internal const string MethodRefresh = "REFRESH";

			internal const string MethodCounter = "COUNTER";
		}

		internal static class ICalParticipationStatus
		{
			internal const string PartstatNeedsAction = "NEEDS-ACTION";

			internal const string PartstatAccepted = "ACCEPTED";

			internal const string PartstatDeclined = "DECLINED";

			internal const string PartstatTentative = "TENTATIVE";

			internal const string PartstatDelegated = "DELEGATED";
		}

		internal static class ICalParticipationRole
		{
			internal const string PartRoleChair = "CHAIR";

			internal const string PartRoleRequired = "REQ-PARTICIPANT";

			internal const string PartRoleOptional = "OPT-PARTICIPANT";

			internal const string PartRoleNonParticipant = "NON-PARTICIPANT";
		}

		internal static class ICalCalendarUserType
		{
			internal const string CalendarUserTypeResource = "RESOURCE";

			internal const string CalendarUserTypeRoom = "ROOM";
		}

		internal static class ICalTaskStatusType
		{
			internal const string TaskStatusTypeCompleted = "COMPLETED";

			internal const string TaskStatusTypeNeedsAction = "NEEDS-ACTION";

			internal const string TaskStatusTypeInProcess = "IN-PROCESS";

			internal const string TaskStatusTypeCancelled = "CANCELLED";
		}
	}
}
