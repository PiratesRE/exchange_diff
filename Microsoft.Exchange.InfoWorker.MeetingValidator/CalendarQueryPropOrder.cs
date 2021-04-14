using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal enum CalendarQueryPropOrder
	{
		Id,
		GlobalId,
		CleanGlobalId,
		IsMeeting,
		AppointmentState,
		CalendarItemType,
		StartTime,
		EndTime,
		Location,
		Subject,
		RecurrenceBlob,
		ResponseType,
		IsResponseRequested,
		CreationTime,
		LastModifiedTime,
		SequenceNumber,
		OwnerApptId,
		OwnerCritChgTime,
		AttendeeCritChgTime,
		AppointmentReplyTime,
		TimeZoneBlob,
		AppointmentExtractTime,
		AppointmentExtractVersion,
		LastSequenceNumber,
		InConflict,
		StartTimeZone,
		EndTimeZone,
		TimeZoneId,
		RecurringTimeZone,
		ItemClass,
		ItemVersion,
		SubjectPrefix,
		NormalizedSubject,
		DocumentId
	}
}
