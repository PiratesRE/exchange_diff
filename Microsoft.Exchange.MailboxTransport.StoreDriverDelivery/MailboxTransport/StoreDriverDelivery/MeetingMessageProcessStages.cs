using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal enum MeetingMessageProcessStages
	{
		Unprocessed,
		DelegateMessageFound,
		ErrorObtainingCalendarItem,
		ExternalMsgProcessingDisabled,
		ProcessMeetingRequest,
		ProcessMeetingResponse,
		ProcessMeetingCancellation,
		ProcessMFN,
		MessageReceipientIsOrganizer,
		RUMAbortDelivery,
		CalendarAssistantAddNewItemsFalse,
		CalendarAssistantActiveFalse,
		ResourceMailboxFound,
		HijackedMeetingFound,
		ParticipantMatchFailure,
		HijackedAppointmentFound,
		JunkNewMeetingRequestFound
	}
}
