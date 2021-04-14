using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICalendarItemBase : IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		double? Accuracy { get; set; }

		bool AllowNewTimeProposal { get; }

		double? Altitude { get; set; }

		double? AltitudeAccuracy { get; set; }

		int AppointmentLastSequenceNumber { get; set; }

		string AppointmentReplyName { get; }

		ExDateTime AppointmentReplyTime { get; }

		int AppointmentSequenceNumber { get; }

		IAttendeeCollection AttendeeCollection { get; }

		ExDateTime AttendeeCriticalChangeTime { get; }

		bool AttendeesChanged { get; }

		CalendarItemType CalendarItemType { get; }

		string CalendarOriginatorId { get; }

		byte[] CleanGlobalObjectId { get; }

		ClientIntentFlags ClientIntent { get; set; }

		string ConferenceInfo { get; set; }

		string ConferenceTelURI { get; set; }

		ExDateTime EndTime { get; set; }

		ExTimeZone EndTimeZone { get; set; }

		ExDateTime EndWallClock { get; }

		Reminders<EventTimeBasedInboxReminder> EventTimeBasedInboxReminders { get; set; }

		BusyType FreeBusyStatus { get; set; }

		GlobalObjectId GlobalObjectId { get; }

		bool IsAllDayEvent { get; set; }

		bool IsCalendarItemTypeOccurrenceOrException { get; }

		bool IsCancelled { get; }

		bool IsEvent { get; }

		bool IsForwardAllowed { get; }

		bool IsMeeting { get; set; }

		bool IsOrganizerExternal { get; }

		double? Latitude { get; set; }

		string Location { get; set; }

		string LocationAnnotation { get; set; }

		string LocationCity { get; set; }

		string LocationCountry { get; set; }

		string LocationDisplayName { get; set; }

		string LocationPostalCode { get; set; }

		LocationSource LocationSource { get; set; }

		string LocationState { get; set; }

		string LocationStreet { get; set; }

		string LocationUri { get; set; }

		double? Longitude { get; set; }

		bool MeetingRequestWasSent { get; }

		string OnlineMeetingConfLink { get; set; }

		string OnlineMeetingExternalLink { get; set; }

		string OnlineMeetingInternalLink { get; set; }

		Participant Organizer { get; }

		byte[] OutlookUserPropsPropDefStream { get; set; }

		int? OwnerAppointmentId { get; }

		ExDateTime OwnerCriticalChangeTime { get; }

		bool ResponseRequested { get; set; }

		ResponseType ResponseType { get; set; }

		string SeriesId { get; set; }

		string ClientId { get; set; }

		ExDateTime StartTime { get; set; }

		ExTimeZone StartTimeZone { get; set; }

		ExDateTime StartWallClock { get; }

		string Subject { get; set; }

		string UCCapabilities { get; set; }

		string UCInband { get; set; }

		string UCMeetingSetting { get; set; }

		string UCMeetingSettingSent { get; set; }

		string UCOpenedConferenceID { get; set; }

		string When { get; set; }

		bool IsReminderSet { get; set; }

		int ReminderMinutesBeforeStart { get; set; }

		RemindersState<EventTimeBasedInboxReminderState> EventTimeBasedInboxRemindersState { get; set; }

		string ItemClass { get; }

		bool IsOrganizer();

		MeetingResponse RespondToMeetingRequest(ResponseType responseType);

		MeetingResponse RespondToMeetingRequest(ResponseType responseType, bool autoCaptureClientIntent, bool intendToSendResponse, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null);

		MeetingResponse RespondToMeetingRequest(ResponseType responseType, string subjectPrefix, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null);

		void SendMeetingMessages(bool isToAllAttendees, int? seriesSequenceNumber = null, bool autoCaptureClientIntent = false, bool copyToSentItems = true, string occurrencesViewPropertiesBlob = null, byte[] masterGoid = null);

		void SaveWithConflictCheck(SaveMode saveMode);
	}
}
