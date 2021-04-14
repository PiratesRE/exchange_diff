using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public enum CalendarActionError
	{
		None,
		CalendarActionInvalidGroupName,
		CalendarActionCannotCreateGroup,
		CalendarActionInvalidGroupId,
		CalendarActionCannotSaveGroup,
		CalendarActionInvalidGroupTypeForDeletion,
		CalendarActionCannotDeleteGroupStillHasChildren,
		CalendarActionFolderIdNotCalendarFolder,
		CalendarActionUnableToChangeColor,
		CalendarActionInvalidCalendarName,
		CalendarActionUnableToCreateCalendarFolder,
		CalendarActionUnableToRenameCalendar,
		CalendarActionUnableToRenameCalendarGroup,
		CalendarActionUnableToDeleteCalendarGroup,
		CalendarActionCalendarAlreadyExists,
		CalendarActionUnableToCreateCalendarNode,
		CalendarActionInvalidItemId,
		CalendarActionFolderIdIsDefaultCalendar,
		CalendarActionCannotRename,
		CalendarActionCannotRenameCalendarNode,
		CalendarActionCannotDeleteCalendar,
		CalendarActionInvalidCalendarNodeOrder,
		CalendarActionUnableToUpdateCalendarNode,
		CalendarActionUnableToFindGroupWithId,
		AddSharedCalendarFailed,
		CalendarActionUnableToSubscribeToCalendar,
		CalendarActionCannotSubscribeToOwnCalendar,
		CalendarActionCannotSubscribeToDistributionList,
		CalendarActionInvalidUrlFormat,
		CalendarActionCalendarAlreadyPublished,
		CalendarActionCannotSaveCalendar,
		CalendarActionUnableToFindCalendarFolder,
		CalendarActionNonExistentMailbox,
		CalendarActionUnexpectedError,
		CalendarActionDelegateManagementError,
		CalendarActionSendSharingInviteError,
		CalendarActionInvalidCalendarEmailAddress,
		CalendarActionUnresolvedCalendarEmailAddress,
		CalendarActionNotInBirthdayCalendarFlight
	}
}
