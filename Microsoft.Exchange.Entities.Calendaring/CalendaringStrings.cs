using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	internal static class CalendaringStrings
	{
		static CalendaringStrings()
		{
			CalendaringStrings.stringIDs.Add(3467777808U, "ErrorOrganizerCantRespond");
			CalendaringStrings.stringIDs.Add(3656881890U, "ErrorForwardNotSupportedForNprAppointment");
			CalendaringStrings.stringIDs.Add(1984582116U, "ErrorResponseNotRequested");
			CalendaringStrings.stringIDs.Add(1952627901U, "ErrorMissingRequiredRespondParameter");
			CalendaringStrings.stringIDs.Add(2515296808U, "ErrorAllDayTimeZoneMismatch");
			CalendaringStrings.stringIDs.Add(3570207334U, "ClientIdAlreadyInUse");
			CalendaringStrings.stringIDs.Add(1428267663U, "ErrorAllDayTimesMustBeMidnight");
			CalendaringStrings.stringIDs.Add(1919329599U, "InvalidCalendarGroupName");
			CalendaringStrings.stringIDs.Add(2859986641U, "ErrorCallerCantChangeSeriesMasterId");
			CalendaringStrings.stringIDs.Add(4235503886U, "CalendarNameCannotBeEmpty");
			CalendaringStrings.stringIDs.Add(2207773804U, "UpdateEventParametersAndAttendeesCantBeSpecified");
			CalendaringStrings.stringIDs.Add(3442398301U, "ErrorProposedNewTimeNotSupportedForNpr");
			CalendaringStrings.stringIDs.Add(86332937U, "CannotDeleteDefaultCalendar");
			CalendaringStrings.stringIDs.Add(419720883U, "ErrorInvalidAttendee");
			CalendaringStrings.stringIDs.Add(1369588146U, "ErrorCallerCantSpecifyClientId");
			CalendaringStrings.stringIDs.Add(2766594189U, "MandatoryParameterClientIdNotSpecified");
			CalendaringStrings.stringIDs.Add(1170527623U, "CalendarGroupEntryUpdateFailed");
			CalendaringStrings.stringIDs.Add(3192618457U, "ErrorCantExpandSingleItem");
			CalendaringStrings.stringIDs.Add(3858227364U, "CalendarFolderUpdateFailed");
			CalendaringStrings.stringIDs.Add(1206102081U, "ErrorRespondToCancelledEvent");
			CalendaringStrings.stringIDs.Add(3539194640U, "ErrorInvalidIdentifier");
			CalendaringStrings.stringIDs.Add(3452208988U, "ErrorCallerCantSpecifySeriesId");
			CalendaringStrings.stringIDs.Add(2770247355U, "ErrorCallerMustSpecifySeriesId");
			CalendaringStrings.stringIDs.Add(2133888287U, "ErrorNotAuthorizedToCancel");
			CalendaringStrings.stringIDs.Add(3220621404U, "InvalidNewReminderSettingId");
			CalendaringStrings.stringIDs.Add(1567702372U, "ErrorNeedToSendMessagesWhenCriticalPropertiesAreChanging");
			CalendaringStrings.stringIDs.Add(2957997666U, "ErrorCallerCantSpecifySeriesMasterId");
			CalendaringStrings.stringIDs.Add(4279355164U, "ErrorMeetingMessageNotFoundOrCantBeUsed");
			CalendaringStrings.stringIDs.Add(661617243U, "ErrorCorruptedSeriesData");
			CalendaringStrings.stringIDs.Add(494393448U, "NullPopupReminderSettings");
			CalendaringStrings.stringIDs.Add(2267819751U, "ErrorOccurrencesListRequired");
			CalendaringStrings.stringIDs.Add(2237189786U, "CannotRenameDefaultCalendar");
			CalendaringStrings.stringIDs.Add(609639808U, "InvalidReminderSettingId");
		}

		public static LocalizedString ErrorOrganizerCantRespond
		{
			get
			{
				return new LocalizedString("ErrorOrganizerCantRespond", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorForwardNotSupportedForNprAppointment
		{
			get
			{
				return new LocalizedString("ErrorForwardNotSupportedForNprAppointment", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResponseNotRequested
		{
			get
			{
				return new LocalizedString("ErrorResponseNotRequested", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarGroupIsNotEmpty(StoreId groupId, Guid groupClassId, string groupName, int calendarsCount)
		{
			return new LocalizedString("CalendarGroupIsNotEmpty", CalendaringStrings.ResourceManager, new object[]
			{
				groupId,
				groupClassId,
				groupName,
				calendarsCount
			});
		}

		public static LocalizedString ErrorMissingRequiredRespondParameter
		{
			get
			{
				return new LocalizedString("ErrorMissingRequiredRespondParameter", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAllDayTimeZoneMismatch
		{
			get
			{
				return new LocalizedString("ErrorAllDayTimeZoneMismatch", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientIdAlreadyInUse
		{
			get
			{
				return new LocalizedString("ClientIdAlreadyInUse", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAllDayTimesMustBeMidnight
		{
			get
			{
				return new LocalizedString("ErrorAllDayTimesMustBeMidnight", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCalendarGroupName
		{
			get
			{
				return new LocalizedString("InvalidCalendarGroupName", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerCantChangeSeriesMasterId
		{
			get
			{
				return new LocalizedString("ErrorCallerCantChangeSeriesMasterId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPopupReminderSettingsCount(int count)
		{
			return new LocalizedString("InvalidPopupReminderSettingsCount", CalendaringStrings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString CalendarNameCannotBeEmpty
		{
			get
			{
				return new LocalizedString("CalendarNameCannotBeEmpty", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateEventParametersAndAttendeesCantBeSpecified
		{
			get
			{
				return new LocalizedString("UpdateEventParametersAndAttendeesCantBeSpecified", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeriesNotFound(string seriesId)
		{
			return new LocalizedString("SeriesNotFound", CalendaringStrings.ResourceManager, new object[]
			{
				seriesId
			});
		}

		public static LocalizedString UnableToFindUser(ADOperationErrorCode operationErrorCode)
		{
			return new LocalizedString("UnableToFindUser", CalendaringStrings.ResourceManager, new object[]
			{
				operationErrorCode
			});
		}

		public static LocalizedString ErrorProposedNewTimeNotSupportedForNpr
		{
			get
			{
				return new LocalizedString("ErrorProposedNewTimeNotSupportedForNpr", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDeleteDefaultCalendar
		{
			get
			{
				return new LocalizedString("CannotDeleteDefaultCalendar", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarNameAlreadyInUse(string name)
		{
			return new LocalizedString("CalendarNameAlreadyInUse", CalendaringStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorInvalidAttendee
		{
			get
			{
				return new LocalizedString("ErrorInvalidAttendee", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerCantSpecifyClientId
		{
			get
			{
				return new LocalizedString("ErrorCallerCantSpecifyClientId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MandatoryParameterClientIdNotSpecified
		{
			get
			{
				return new LocalizedString("MandatoryParameterClientIdNotSpecified", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarGroupEntryUpdateFailed
		{
			get
			{
				return new LocalizedString("CalendarGroupEntryUpdateFailed", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCantExpandSingleItem
		{
			get
			{
				return new LocalizedString("ErrorCantExpandSingleItem", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarFolderUpdateFailed
		{
			get
			{
				return new LocalizedString("CalendarFolderUpdateFailed", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRespondToCancelledEvent
		{
			get
			{
				return new LocalizedString("ErrorRespondToCancelledEvent", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdentifier
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdentifier", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDeleteWellKnownCalendarGroup(StoreId groupId, Guid groupClassId, string groupName)
		{
			return new LocalizedString("CannotDeleteWellKnownCalendarGroup", CalendaringStrings.ResourceManager, new object[]
			{
				groupId,
				groupClassId,
				groupName
			});
		}

		public static LocalizedString FolderNotFound(string folderType)
		{
			return new LocalizedString("FolderNotFound", CalendaringStrings.ResourceManager, new object[]
			{
				folderType
			});
		}

		public static LocalizedString ErrorCallerCantSpecifySeriesId
		{
			get
			{
				return new LocalizedString("ErrorCallerCantSpecifySeriesId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerMustSpecifySeriesId
		{
			get
			{
				return new LocalizedString("ErrorCallerMustSpecifySeriesId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotAuthorizedToCancel
		{
			get
			{
				return new LocalizedString("ErrorNotAuthorizedToCancel", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNewReminderSettingId
		{
			get
			{
				return new LocalizedString("InvalidNewReminderSettingId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNeedToSendMessagesWhenCriticalPropertiesAreChanging
		{
			get
			{
				return new LocalizedString("ErrorNeedToSendMessagesWhenCriticalPropertiesAreChanging", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerCantSpecifySeriesMasterId
		{
			get
			{
				return new LocalizedString("ErrorCallerCantSpecifySeriesMasterId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMeetingMessageNotFoundOrCantBeUsed
		{
			get
			{
				return new LocalizedString("ErrorMeetingMessageNotFoundOrCantBeUsed", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCorruptedSeriesData
		{
			get
			{
				return new LocalizedString("ErrorCorruptedSeriesData", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullPopupReminderSettings
		{
			get
			{
				return new LocalizedString("NullPopupReminderSettings", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOccurrencesListRequired
		{
			get
			{
				return new LocalizedString("ErrorOccurrencesListRequired", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRenameDefaultCalendar
		{
			get
			{
				return new LocalizedString("CannotRenameDefaultCalendar", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReminderSettingId
		{
			get
			{
				return new LocalizedString("InvalidReminderSettingId", CalendaringStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(CalendaringStrings.IDs key)
		{
			return new LocalizedString(CalendaringStrings.stringIDs[(uint)key], CalendaringStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(33);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Entities.Calendaring.CalendaringStrings", typeof(CalendaringStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorOrganizerCantRespond = 3467777808U,
			ErrorForwardNotSupportedForNprAppointment = 3656881890U,
			ErrorResponseNotRequested = 1984582116U,
			ErrorMissingRequiredRespondParameter = 1952627901U,
			ErrorAllDayTimeZoneMismatch = 2515296808U,
			ClientIdAlreadyInUse = 3570207334U,
			ErrorAllDayTimesMustBeMidnight = 1428267663U,
			InvalidCalendarGroupName = 1919329599U,
			ErrorCallerCantChangeSeriesMasterId = 2859986641U,
			CalendarNameCannotBeEmpty = 4235503886U,
			UpdateEventParametersAndAttendeesCantBeSpecified = 2207773804U,
			ErrorProposedNewTimeNotSupportedForNpr = 3442398301U,
			CannotDeleteDefaultCalendar = 86332937U,
			ErrorInvalidAttendee = 419720883U,
			ErrorCallerCantSpecifyClientId = 1369588146U,
			MandatoryParameterClientIdNotSpecified = 2766594189U,
			CalendarGroupEntryUpdateFailed = 1170527623U,
			ErrorCantExpandSingleItem = 3192618457U,
			CalendarFolderUpdateFailed = 3858227364U,
			ErrorRespondToCancelledEvent = 1206102081U,
			ErrorInvalidIdentifier = 3539194640U,
			ErrorCallerCantSpecifySeriesId = 3452208988U,
			ErrorCallerMustSpecifySeriesId = 2770247355U,
			ErrorNotAuthorizedToCancel = 2133888287U,
			InvalidNewReminderSettingId = 3220621404U,
			ErrorNeedToSendMessagesWhenCriticalPropertiesAreChanging = 1567702372U,
			ErrorCallerCantSpecifySeriesMasterId = 2957997666U,
			ErrorMeetingMessageNotFoundOrCantBeUsed = 4279355164U,
			ErrorCorruptedSeriesData = 661617243U,
			NullPopupReminderSettings = 494393448U,
			ErrorOccurrencesListRequired = 2267819751U,
			CannotRenameDefaultCalendar = 2237189786U,
			InvalidReminderSettingId = 609639808U
		}

		private enum ParamIDs
		{
			CalendarGroupIsNotEmpty,
			InvalidPopupReminderSettingsCount,
			SeriesNotFound,
			UnableToFindUser,
			CalendarNameAlreadyInUse,
			CannotDeleteWellKnownCalendarGroup,
			FolderNotFound
		}
	}
}
