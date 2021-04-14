using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum OwaEventHandlerErrorCode
	{
		None,
		ConflictResolution,
		ItemNotFound,
		MissingMeetingFields,
		UnexpectedError,
		MeetingCriticalUpdateProperties,
		WebPartSegmentationError,
		WebPartFirstAccessError,
		WebPartContentsPermissionsError,
		WebPartUnsupportedFolderTypeError,
		WebPartUnsupportedItemError,
		WebPartFolderPathError,
		WebPartTaskFolderError,
		WebPartCommandParameterError,
		WebPartInvalidParameterError,
		WebPartActionPermissionsError,
		WebPartNoMatchOnSmtpAddressError,
		WebPartCalendarFolderError,
		AddContactsToItemError,
		SendByEmailError,
		MailboxInTransitError,
		ComplianceLabelNotFoundError,
		WebPartAccessPublicFolderViaOwaBasicError,
		WebPartExplicitLogonPublicFolderViaOwaBasicError,
		ExistentNotificationPipeError,
		MailboxFailoverWithoutRedirection,
		MailboxFailoverWithRedirection,
		FolderNameExists,
		UserNotIMEnabled,
		IMOperationNotAllowedToSelf,
		ErrorEarlyBrowserOnPublishedCalendar,
		NotSet
	}
}
