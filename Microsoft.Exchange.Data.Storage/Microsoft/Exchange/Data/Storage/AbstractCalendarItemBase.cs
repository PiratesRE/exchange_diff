using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractCalendarItemBase : AbstractItem, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual double? Accuracy
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool AllowNewTimeProposal
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual double? Altitude
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual double? AltitudeAccuracy
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int AppointmentLastSequenceNumber
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string AppointmentReplyName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime AppointmentReplyTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual int AppointmentSequenceNumber
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IAttendeeCollection AttendeeCollection
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime AttendeeCriticalChangeTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool AttendeesChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual CalendarItemType CalendarItemType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string CalendarOriginatorId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual byte[] CleanGlobalObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ClientIntentFlags ClientIntent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ConferenceInfo
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ConferenceTelURI
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime EndTime
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExTimeZone EndTimeZone
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime EndWallClock
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual Reminders<EventTimeBasedInboxReminder> EventTimeBasedInboxReminders
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual BusyType FreeBusyStatus
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual GlobalObjectId GlobalObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsAllDayEvent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsCalendarItemTypeOccurrenceOrException
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsCancelled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsEvent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsForwardAllowed
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsMeeting
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsOrganizerExternal
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ItemClass
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual double? Latitude
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Location
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationAnnotation
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationCity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationCountry
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationDisplayName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationPostalCode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual LocationSource LocationSource
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationStreet
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LocationUri
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual double? Longitude
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool MeetingRequestWasSent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string OnlineMeetingConfLink
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string OnlineMeetingExternalLink
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string OnlineMeetingInternalLink
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual Participant Organizer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual byte[] OutlookUserPropsPropDefStream
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int? OwnerAppointmentId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime OwnerCriticalChangeTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool ResponseRequested
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ResponseType ResponseType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string SeriesId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ClientId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime StartTime
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExTimeZone StartTimeZone
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ExDateTime StartWallClock
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Subject
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UCCapabilities
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UCInband
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UCMeetingSetting
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UCMeetingSettingSent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UCOpenedConferenceID
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string When
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsReminderSet
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int ReminderMinutesBeforeStart
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual RemindersState<EventTimeBasedInboxReminderState> EventTimeBasedInboxRemindersState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsOrganizer()
		{
			throw new NotImplementedException();
		}

		public virtual MeetingResponse RespondToMeetingRequest(ResponseType responseType)
		{
			throw new NotImplementedException();
		}

		public virtual MeetingResponse RespondToMeetingRequest(ResponseType responseType, bool autoCaptureClientIntent, bool intendToSendResponse)
		{
			throw new NotImplementedException();
		}

		public virtual MeetingResponse RespondToMeetingRequest(ResponseType responseType, bool autoCaptureClientIntent, bool intendToSendResponse, ExDateTime? proposedStart, ExDateTime? proposedEnd)
		{
			throw new NotImplementedException();
		}

		public virtual MeetingResponse RespondToMeetingRequest(ResponseType responseType, string subjectPrefix, ExDateTime? proposedStart, ExDateTime? proposedEnd)
		{
			throw new NotImplementedException();
		}

		public virtual void SendMeetingMessages(bool isToAllAttendees, int? seriesSequenceNumber = null, bool autoCaptureClientIntent = false, bool copyToSentItems = true, string occurrencesViewPropertiesBlob = null, byte[] masterGoid = null)
		{
			throw new NotImplementedException();
		}

		public void SaveWithConflictCheck(SaveMode saveMode)
		{
			throw new NotImplementedException();
		}
	}
}
