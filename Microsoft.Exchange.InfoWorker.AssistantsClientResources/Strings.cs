using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.AssistantsClientResources
{
	public static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(394366253U, "descRecurringNotAllowed");
			Strings.stringIDs.Add(1148026803U, "descSingleBooked");
			Strings.stringIDs.Add(566335833U, "descDeclined");
			Strings.stringIDs.Add(95666746U, "descDelegateConflictList");
			Strings.stringIDs.Add(3021003899U, "descDelegateRecurring");
			Strings.stringIDs.Add(167053646U, "descAccepted");
			Strings.stringIDs.Add(1075460866U, "descCredit");
			Strings.stringIDs.Add(2827051240U, "descRoleNotAllowed");
			Strings.stringIDs.Add(736995773U, "descInThePast");
			Strings.stringIDs.Add(3503731055U, "RetainUntil");
			Strings.stringIDs.Add(3589522754U, "descRecurringBooked");
			Strings.stringIDs.Add(1579274145U, "descRecurringSomeAccepted");
			Strings.stringIDs.Add(3779075844U, "descTahomaBlackMediumFontTag");
			Strings.stringIDs.Add(3990825802U, "descSingleAccepted");
			Strings.stringIDs.Add(3328869470U, "descCorruptWorkingHours");
			Strings.stringIDs.Add(3716547363U, "SMSLowConfidenceTranscription");
			Strings.stringIDs.Add(2683960550U, "descAcknowledgeReceived");
			Strings.stringIDs.Add(2847829128U, "descOutOfPolicyDelegateMessage");
			Strings.stringIDs.Add(1043033118U, "descSingleConflicts");
			Strings.stringIDs.Add(2938636641U, "descRecurringConflicts");
			Strings.stringIDs.Add(4274878084U, "descTimeZoneInfo");
			Strings.stringIDs.Add(4066812006U, "descSingleBookedSomeAccepted");
			Strings.stringIDs.Add(445569887U, "descMeetingTimeLabel");
			Strings.stringIDs.Add(1687944893U, "descBody");
			Strings.stringIDs.Add(2293492535U, "descDelegateConflicts");
			Strings.stringIDs.Add(203356656U, "descDeclinedAll");
			Strings.stringIDs.Add(3332843108U, "SMSProtectedVoicemail");
			Strings.stringIDs.Add(1630794581U, "descArialGreySmallFontTag");
			Strings.stringIDs.Add(918712133U, "descDelegateNoPerm");
			Strings.stringIDs.Add(3244013810U, "descTahomaGreyMediumFontTag");
			Strings.stringIDs.Add(2931808677U, "descAcceptedAll");
			Strings.stringIDs.Add(2892660168U, "descInPolicyDelegateMessage");
			Strings.stringIDs.Add(1177578330U, "SMSEmptyCallerId");
			Strings.stringIDs.Add(4066928937U, "descMeetingOrganizerAndTimeLabel");
			Strings.stringIDs.Add(3209486321U, "descAcknowledgeTentativeAccept");
		}

		public static LocalizedString descRecurringNotAllowed
		{
			get
			{
				return new LocalizedString("descRecurringNotAllowed", "Ex10BBBE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descSingleBooked
		{
			get
			{
				return new LocalizedString("descSingleBooked", "ExD58370", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDeclined
		{
			get
			{
				return new LocalizedString("descDeclined", "Ex0F0ED6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateConflictList
		{
			get
			{
				return new LocalizedString("descDelegateConflictList", "Ex0CAD29", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSMissedCallWithCallerInfo(string callerName, string callerId)
		{
			return new LocalizedString("SMSMissedCallWithCallerInfo", "Ex3EA246", false, true, Strings.ResourceManager, new object[]
			{
				callerName,
				callerId
			});
		}

		public static LocalizedString descDelegateRecurring
		{
			get
			{
				return new LocalizedString("descDelegateRecurring", "ExBC122E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAccepted
		{
			get
			{
				return new LocalizedString("descAccepted", "Ex0043C4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateTimeSingleDay(string date, string startTime, string endTime)
		{
			return new LocalizedString("DateTimeSingleDay", "", false, false, Strings.ResourceManager, new object[]
			{
				date,
				startTime,
				endTime
			});
		}

		public static LocalizedString descCredit
		{
			get
			{
				return new LocalizedString("descCredit", "ExBEF798", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateEndDate(string date)
		{
			return new LocalizedString("descDelegateEndDate", "Ex0CA0C8", false, true, Strings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString descAcceptedThrough(string shortDate)
		{
			return new LocalizedString("descAcceptedThrough", "ExF11589", false, true, Strings.ResourceManager, new object[]
			{
				shortDate
			});
		}

		public static LocalizedString descRoleNotAllowed
		{
			get
			{
				return new LocalizedString("descRoleNotAllowed", "ExE9E956", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSNewVoicemailWithCallerInfo(int durationInSecs, string callerName, string callerId)
		{
			return new LocalizedString("SMSNewVoicemailWithCallerInfo", "Ex03119B", false, true, Strings.ResourceManager, new object[]
			{
				durationInSecs,
				callerName,
				callerId
			});
		}

		public static LocalizedString descInThePast
		{
			get
			{
				return new LocalizedString("descInThePast", "Ex4F8870", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetainUntil
		{
			get
			{
				return new LocalizedString("RetainUntil", "Ex77FB38", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecurringBooked
		{
			get
			{
				return new LocalizedString("descRecurringBooked", "ExEC240C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecurringSomeAccepted
		{
			get
			{
				return new LocalizedString("descRecurringSomeAccepted", "Ex13707D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTahomaBlackMediumFontTag
		{
			get
			{
				return new LocalizedString("descTahomaBlackMediumFontTag", "ExE2D423", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descBookingWindow(string days, string date)
		{
			return new LocalizedString("descBookingWindow", "ExCD069B", false, true, Strings.ResourceManager, new object[]
			{
				days,
				date
			});
		}

		public static LocalizedString descSingleAccepted
		{
			get
			{
				return new LocalizedString("descSingleAccepted", "Ex7B2BBB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCorruptWorkingHours
		{
			get
			{
				return new LocalizedString("descCorruptWorkingHours", "Ex4268E5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSLowConfidenceTranscription
		{
			get
			{
				return new LocalizedString("SMSLowConfidenceTranscription", "Ex02DF8B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descToList(string start, string end)
		{
			return new LocalizedString("descToList", "Ex373ED4", false, true, Strings.ResourceManager, new object[]
			{
				start,
				end
			});
		}

		public static LocalizedString EventReminderSubject(string subject)
		{
			return new LocalizedString("EventReminderSubject", "", false, false, Strings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString descAcknowledgeReceived
		{
			get
			{
				return new LocalizedString("descAcknowledgeReceived", "ExC60BEC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSMissedCallWithCallerId(string callerId)
		{
			return new LocalizedString("SMSMissedCallWithCallerId", "Ex31FFD4", false, true, Strings.ResourceManager, new object[]
			{
				callerId
			});
		}

		public static LocalizedString descOutOfPolicyDelegateMessage
		{
			get
			{
				return new LocalizedString("descOutOfPolicyDelegateMessage", "Ex3110DC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCommaList(string first, string last)
		{
			return new LocalizedString("descCommaList", "Ex9FA536", false, true, Strings.ResourceManager, new object[]
			{
				first,
				last
			});
		}

		public static LocalizedString descRecurringEndDateWindow(string window, string endDate)
		{
			return new LocalizedString("descRecurringEndDateWindow", "Ex957418", false, true, Strings.ResourceManager, new object[]
			{
				window,
				endDate
			});
		}

		public static LocalizedString descOn(string list)
		{
			return new LocalizedString("descOn", "Ex9DD11E", false, true, Strings.ResourceManager, new object[]
			{
				list
			});
		}

		public static LocalizedString descWorkingHours(string startHour, string endHour, string days)
		{
			return new LocalizedString("descWorkingHours", "Ex9A64E4", false, true, Strings.ResourceManager, new object[]
			{
				startHour,
				endHour,
				days
			});
		}

		public static LocalizedString descSingleConflicts
		{
			get
			{
				return new LocalizedString("descSingleConflicts", "ExE3CD6B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecurringAccepted(string endDate)
		{
			return new LocalizedString("descRecurringAccepted", "Ex1B2ABC", false, true, Strings.ResourceManager, new object[]
			{
				endDate
			});
		}

		public static LocalizedString DateTimeMultiDay(string startDate, string startTime, string endDate, string endTime)
		{
			return new LocalizedString("DateTimeMultiDay", "", false, false, Strings.ResourceManager, new object[]
			{
				startDate,
				startTime,
				endDate,
				endTime
			});
		}

		public static LocalizedString descDelegateWorkHours(string hours)
		{
			return new LocalizedString("descDelegateWorkHours", "ExABB9A7", false, true, Strings.ResourceManager, new object[]
			{
				hours
			});
		}

		public static LocalizedString descRecurringConflicts
		{
			get
			{
				return new LocalizedString("descRecurringConflicts", "Ex5E68AA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTimeZoneInfo
		{
			get
			{
				return new LocalizedString("descTimeZoneInfo", "Ex8A8335", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSNewVoicemailWithCallerId(int durationInSecs, string callerId)
		{
			return new LocalizedString("SMSNewVoicemailWithCallerId", "Ex1F8B83", false, true, Strings.ResourceManager, new object[]
			{
				durationInSecs,
				callerId
			});
		}

		public static LocalizedString descToLong(string maxDuration)
		{
			return new LocalizedString("descToLong", "Ex3D5C9A", false, true, Strings.ResourceManager, new object[]
			{
				maxDuration
			});
		}

		public static LocalizedString descSingleBookedSomeAccepted
		{
			get
			{
				return new LocalizedString("descSingleBookedSomeAccepted", "ExD1C45A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMeetingTimeLabel
		{
			get
			{
				return new LocalizedString("descMeetingTimeLabel", "Ex7955C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDeclinedInstance(string shortDate)
		{
			return new LocalizedString("descDeclinedInstance", "ExABCDE1", false, true, Strings.ResourceManager, new object[]
			{
				shortDate
			});
		}

		public static LocalizedString descBody
		{
			get
			{
				return new LocalizedString("descBody", "Ex0DF370", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateConflicts
		{
			get
			{
				return new LocalizedString("descDelegateConflicts", "ExB336BA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventReminderSummary(string dateTime, string location)
		{
			return new LocalizedString("EventReminderSummary", "", false, false, Strings.ResourceManager, new object[]
			{
				dateTime,
				location
			});
		}

		public static LocalizedString descDelegatePleaseVerify(string user)
		{
			return new LocalizedString("descDelegatePleaseVerify", "Ex51BE8F", false, true, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString descDeclinedAll
		{
			get
			{
				return new LocalizedString("descDeclinedAll", "Ex80AB69", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventReminderSummaryNoLocation(string dateTime)
		{
			return new LocalizedString("EventReminderSummaryNoLocation", "", false, false, Strings.ResourceManager, new object[]
			{
				dateTime
			});
		}

		public static LocalizedString ReminderMessageBody(string summary, string customMessage)
		{
			return new LocalizedString("ReminderMessageBody", "", false, false, Strings.ResourceManager, new object[]
			{
				summary,
				customMessage
			});
		}

		public static LocalizedString descAndList(string first, string last)
		{
			return new LocalizedString("descAndList", "Ex3D8456", false, true, Strings.ResourceManager, new object[]
			{
				first,
				last
			});
		}

		public static LocalizedString SMSProtectedVoicemail
		{
			get
			{
				return new LocalizedString("SMSProtectedVoicemail", "ExBEA9CA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descArialGreySmallFontTag
		{
			get
			{
				return new LocalizedString("descArialGreySmallFontTag", "ExBEEFA8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateNoPerm
		{
			get
			{
				return new LocalizedString("descDelegateNoPerm", "Ex0A52A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateNoEndDate(string date)
		{
			return new LocalizedString("descDelegateNoEndDate", "Ex3ED270", false, true, Strings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString descTahomaGreyMediumFontTag
		{
			get
			{
				return new LocalizedString("descTahomaGreyMediumFontTag", "ExC1F40F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAcceptedAll
		{
			get
			{
				return new LocalizedString("descAcceptedAll", "ExBB42F6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecurringEndDate(string endDate)
		{
			return new LocalizedString("descRecurringEndDate", "ExB13AC3", false, true, Strings.ResourceManager, new object[]
			{
				endDate
			});
		}

		public static LocalizedString descInPolicyDelegateMessage
		{
			get
			{
				return new LocalizedString("descInPolicyDelegateMessage", "Ex6D23AD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMSEmptyCallerId
		{
			get
			{
				return new LocalizedString("SMSEmptyCallerId", "ExDF7549", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDelegateTooLong(string minutes)
		{
			return new LocalizedString("descDelegateTooLong", "ExCDBC94", false, true, Strings.ResourceManager, new object[]
			{
				minutes
			});
		}

		public static LocalizedString descMeetingOrganizerAndTimeLabel
		{
			get
			{
				return new LocalizedString("descMeetingOrganizerAndTimeLabel", "ExE98BCC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAcknowledgeTentativeAccept
		{
			get
			{
				return new LocalizedString("descAcknowledgeTentativeAccept", "ExD300A0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(35);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.InfoWorker.AssistantsClientResources.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			descRecurringNotAllowed = 394366253U,
			descSingleBooked = 1148026803U,
			descDeclined = 566335833U,
			descDelegateConflictList = 95666746U,
			descDelegateRecurring = 3021003899U,
			descAccepted = 167053646U,
			descCredit = 1075460866U,
			descRoleNotAllowed = 2827051240U,
			descInThePast = 736995773U,
			RetainUntil = 3503731055U,
			descRecurringBooked = 3589522754U,
			descRecurringSomeAccepted = 1579274145U,
			descTahomaBlackMediumFontTag = 3779075844U,
			descSingleAccepted = 3990825802U,
			descCorruptWorkingHours = 3328869470U,
			SMSLowConfidenceTranscription = 3716547363U,
			descAcknowledgeReceived = 2683960550U,
			descOutOfPolicyDelegateMessage = 2847829128U,
			descSingleConflicts = 1043033118U,
			descRecurringConflicts = 2938636641U,
			descTimeZoneInfo = 4274878084U,
			descSingleBookedSomeAccepted = 4066812006U,
			descMeetingTimeLabel = 445569887U,
			descBody = 1687944893U,
			descDelegateConflicts = 2293492535U,
			descDeclinedAll = 203356656U,
			SMSProtectedVoicemail = 3332843108U,
			descArialGreySmallFontTag = 1630794581U,
			descDelegateNoPerm = 918712133U,
			descTahomaGreyMediumFontTag = 3244013810U,
			descAcceptedAll = 2931808677U,
			descInPolicyDelegateMessage = 2892660168U,
			SMSEmptyCallerId = 1177578330U,
			descMeetingOrganizerAndTimeLabel = 4066928937U,
			descAcknowledgeTentativeAccept = 3209486321U
		}

		private enum ParamIDs
		{
			SMSMissedCallWithCallerInfo,
			DateTimeSingleDay,
			descDelegateEndDate,
			descAcceptedThrough,
			SMSNewVoicemailWithCallerInfo,
			descBookingWindow,
			descToList,
			EventReminderSubject,
			SMSMissedCallWithCallerId,
			descCommaList,
			descRecurringEndDateWindow,
			descOn,
			descWorkingHours,
			descRecurringAccepted,
			DateTimeMultiDay,
			descDelegateWorkHours,
			SMSNewVoicemailWithCallerId,
			descToLong,
			descDeclinedInstance,
			EventReminderSummary,
			descDelegatePleaseVerify,
			EventReminderSummaryNoLocation,
			ReminderMessageBody,
			descAndList,
			descDelegateNoEndDate,
			descRecurringEndDate,
			descDelegateTooLong
		}
	}
}
