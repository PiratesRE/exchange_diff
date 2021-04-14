using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class CalendarItemType : ItemType
	{
		public string UID
		{
			get
			{
				return this.uIDField;
			}
			set
			{
				this.uIDField = value;
			}
		}

		public DateTime RecurrenceId
		{
			get
			{
				return this.recurrenceIdField;
			}
			set
			{
				this.recurrenceIdField = value;
			}
		}

		[XmlIgnore]
		public bool RecurrenceIdSpecified
		{
			get
			{
				return this.recurrenceIdFieldSpecified;
			}
			set
			{
				this.recurrenceIdFieldSpecified = value;
			}
		}

		public DateTime DateTimeStamp
		{
			get
			{
				return this.dateTimeStampField;
			}
			set
			{
				this.dateTimeStampField = value;
			}
		}

		[XmlIgnore]
		public bool DateTimeStampSpecified
		{
			get
			{
				return this.dateTimeStampFieldSpecified;
			}
			set
			{
				this.dateTimeStampFieldSpecified = value;
			}
		}

		public DateTime Start
		{
			get
			{
				return this.startField;
			}
			set
			{
				this.startField = value;
			}
		}

		[XmlIgnore]
		public bool StartSpecified
		{
			get
			{
				return this.startFieldSpecified;
			}
			set
			{
				this.startFieldSpecified = value;
			}
		}

		public DateTime End
		{
			get
			{
				return this.endField;
			}
			set
			{
				this.endField = value;
			}
		}

		[XmlIgnore]
		public bool EndSpecified
		{
			get
			{
				return this.endFieldSpecified;
			}
			set
			{
				this.endFieldSpecified = value;
			}
		}

		public DateTime OriginalStart
		{
			get
			{
				return this.originalStartField;
			}
			set
			{
				this.originalStartField = value;
			}
		}

		[XmlIgnore]
		public bool OriginalStartSpecified
		{
			get
			{
				return this.originalStartFieldSpecified;
			}
			set
			{
				this.originalStartFieldSpecified = value;
			}
		}

		public bool IsAllDayEvent
		{
			get
			{
				return this.isAllDayEventField;
			}
			set
			{
				this.isAllDayEventField = value;
			}
		}

		[XmlIgnore]
		public bool IsAllDayEventSpecified
		{
			get
			{
				return this.isAllDayEventFieldSpecified;
			}
			set
			{
				this.isAllDayEventFieldSpecified = value;
			}
		}

		public LegacyFreeBusyType LegacyFreeBusyStatus
		{
			get
			{
				return this.legacyFreeBusyStatusField;
			}
			set
			{
				this.legacyFreeBusyStatusField = value;
			}
		}

		[XmlIgnore]
		public bool LegacyFreeBusyStatusSpecified
		{
			get
			{
				return this.legacyFreeBusyStatusFieldSpecified;
			}
			set
			{
				this.legacyFreeBusyStatusFieldSpecified = value;
			}
		}

		public string Location
		{
			get
			{
				return this.locationField;
			}
			set
			{
				this.locationField = value;
			}
		}

		public string When
		{
			get
			{
				return this.whenField;
			}
			set
			{
				this.whenField = value;
			}
		}

		public bool IsMeeting
		{
			get
			{
				return this.isMeetingField;
			}
			set
			{
				this.isMeetingField = value;
			}
		}

		[XmlIgnore]
		public bool IsMeetingSpecified
		{
			get
			{
				return this.isMeetingFieldSpecified;
			}
			set
			{
				this.isMeetingFieldSpecified = value;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return this.isCancelledField;
			}
			set
			{
				this.isCancelledField = value;
			}
		}

		[XmlIgnore]
		public bool IsCancelledSpecified
		{
			get
			{
				return this.isCancelledFieldSpecified;
			}
			set
			{
				this.isCancelledFieldSpecified = value;
			}
		}

		public bool IsRecurring
		{
			get
			{
				return this.isRecurringField;
			}
			set
			{
				this.isRecurringField = value;
			}
		}

		[XmlIgnore]
		public bool IsRecurringSpecified
		{
			get
			{
				return this.isRecurringFieldSpecified;
			}
			set
			{
				this.isRecurringFieldSpecified = value;
			}
		}

		public bool MeetingRequestWasSent
		{
			get
			{
				return this.meetingRequestWasSentField;
			}
			set
			{
				this.meetingRequestWasSentField = value;
			}
		}

		[XmlIgnore]
		public bool MeetingRequestWasSentSpecified
		{
			get
			{
				return this.meetingRequestWasSentFieldSpecified;
			}
			set
			{
				this.meetingRequestWasSentFieldSpecified = value;
			}
		}

		public bool IsResponseRequested
		{
			get
			{
				return this.isResponseRequestedField;
			}
			set
			{
				this.isResponseRequestedField = value;
			}
		}

		[XmlIgnore]
		public bool IsResponseRequestedSpecified
		{
			get
			{
				return this.isResponseRequestedFieldSpecified;
			}
			set
			{
				this.isResponseRequestedFieldSpecified = value;
			}
		}

		[XmlElement("CalendarItemType")]
		public CalendarItemTypeType CalendarItemType1
		{
			get
			{
				return this.calendarItemType1Field;
			}
			set
			{
				this.calendarItemType1Field = value;
			}
		}

		[XmlIgnore]
		public bool CalendarItemType1Specified
		{
			get
			{
				return this.calendarItemType1FieldSpecified;
			}
			set
			{
				this.calendarItemType1FieldSpecified = value;
			}
		}

		public ResponseTypeType MyResponseType
		{
			get
			{
				return this.myResponseTypeField;
			}
			set
			{
				this.myResponseTypeField = value;
			}
		}

		[XmlIgnore]
		public bool MyResponseTypeSpecified
		{
			get
			{
				return this.myResponseTypeFieldSpecified;
			}
			set
			{
				this.myResponseTypeFieldSpecified = value;
			}
		}

		public SingleRecipientType Organizer
		{
			get
			{
				return this.organizerField;
			}
			set
			{
				this.organizerField = value;
			}
		}

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] RequiredAttendees
		{
			get
			{
				return this.requiredAttendeesField;
			}
			set
			{
				this.requiredAttendeesField = value;
			}
		}

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] OptionalAttendees
		{
			get
			{
				return this.optionalAttendeesField;
			}
			set
			{
				this.optionalAttendeesField = value;
			}
		}

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] Resources
		{
			get
			{
				return this.resourcesField;
			}
			set
			{
				this.resourcesField = value;
			}
		}

		public int ConflictingMeetingCount
		{
			get
			{
				return this.conflictingMeetingCountField;
			}
			set
			{
				this.conflictingMeetingCountField = value;
			}
		}

		[XmlIgnore]
		public bool ConflictingMeetingCountSpecified
		{
			get
			{
				return this.conflictingMeetingCountFieldSpecified;
			}
			set
			{
				this.conflictingMeetingCountFieldSpecified = value;
			}
		}

		public int AdjacentMeetingCount
		{
			get
			{
				return this.adjacentMeetingCountField;
			}
			set
			{
				this.adjacentMeetingCountField = value;
			}
		}

		[XmlIgnore]
		public bool AdjacentMeetingCountSpecified
		{
			get
			{
				return this.adjacentMeetingCountFieldSpecified;
			}
			set
			{
				this.adjacentMeetingCountFieldSpecified = value;
			}
		}

		public NonEmptyArrayOfAllItemsType ConflictingMeetings
		{
			get
			{
				return this.conflictingMeetingsField;
			}
			set
			{
				this.conflictingMeetingsField = value;
			}
		}

		public NonEmptyArrayOfAllItemsType AdjacentMeetings
		{
			get
			{
				return this.adjacentMeetingsField;
			}
			set
			{
				this.adjacentMeetingsField = value;
			}
		}

		public string Duration
		{
			get
			{
				return this.durationField;
			}
			set
			{
				this.durationField = value;
			}
		}

		public string TimeZone
		{
			get
			{
				return this.timeZoneField;
			}
			set
			{
				this.timeZoneField = value;
			}
		}

		public DateTime AppointmentReplyTime
		{
			get
			{
				return this.appointmentReplyTimeField;
			}
			set
			{
				this.appointmentReplyTimeField = value;
			}
		}

		[XmlIgnore]
		public bool AppointmentReplyTimeSpecified
		{
			get
			{
				return this.appointmentReplyTimeFieldSpecified;
			}
			set
			{
				this.appointmentReplyTimeFieldSpecified = value;
			}
		}

		public int AppointmentSequenceNumber
		{
			get
			{
				return this.appointmentSequenceNumberField;
			}
			set
			{
				this.appointmentSequenceNumberField = value;
			}
		}

		[XmlIgnore]
		public bool AppointmentSequenceNumberSpecified
		{
			get
			{
				return this.appointmentSequenceNumberFieldSpecified;
			}
			set
			{
				this.appointmentSequenceNumberFieldSpecified = value;
			}
		}

		public int AppointmentState
		{
			get
			{
				return this.appointmentStateField;
			}
			set
			{
				this.appointmentStateField = value;
			}
		}

		[XmlIgnore]
		public bool AppointmentStateSpecified
		{
			get
			{
				return this.appointmentStateFieldSpecified;
			}
			set
			{
				this.appointmentStateFieldSpecified = value;
			}
		}

		public RecurrenceType Recurrence
		{
			get
			{
				return this.recurrenceField;
			}
			set
			{
				this.recurrenceField = value;
			}
		}

		public OccurrenceInfoType FirstOccurrence
		{
			get
			{
				return this.firstOccurrenceField;
			}
			set
			{
				this.firstOccurrenceField = value;
			}
		}

		public OccurrenceInfoType LastOccurrence
		{
			get
			{
				return this.lastOccurrenceField;
			}
			set
			{
				this.lastOccurrenceField = value;
			}
		}

		[XmlArrayItem("Occurrence", IsNullable = false)]
		public OccurrenceInfoType[] ModifiedOccurrences
		{
			get
			{
				return this.modifiedOccurrencesField;
			}
			set
			{
				this.modifiedOccurrencesField = value;
			}
		}

		[XmlArrayItem("DeletedOccurrence", IsNullable = false)]
		public DeletedOccurrenceInfoType[] DeletedOccurrences
		{
			get
			{
				return this.deletedOccurrencesField;
			}
			set
			{
				this.deletedOccurrencesField = value;
			}
		}

		public TimeZoneType MeetingTimeZone
		{
			get
			{
				return this.meetingTimeZoneField;
			}
			set
			{
				this.meetingTimeZoneField = value;
			}
		}

		public TimeZoneDefinitionType StartTimeZone
		{
			get
			{
				return this.startTimeZoneField;
			}
			set
			{
				this.startTimeZoneField = value;
			}
		}

		public TimeZoneDefinitionType EndTimeZone
		{
			get
			{
				return this.endTimeZoneField;
			}
			set
			{
				this.endTimeZoneField = value;
			}
		}

		public int ConferenceType
		{
			get
			{
				return this.conferenceTypeField;
			}
			set
			{
				this.conferenceTypeField = value;
			}
		}

		[XmlIgnore]
		public bool ConferenceTypeSpecified
		{
			get
			{
				return this.conferenceTypeFieldSpecified;
			}
			set
			{
				this.conferenceTypeFieldSpecified = value;
			}
		}

		public bool AllowNewTimeProposal
		{
			get
			{
				return this.allowNewTimeProposalField;
			}
			set
			{
				this.allowNewTimeProposalField = value;
			}
		}

		[XmlIgnore]
		public bool AllowNewTimeProposalSpecified
		{
			get
			{
				return this.allowNewTimeProposalFieldSpecified;
			}
			set
			{
				this.allowNewTimeProposalFieldSpecified = value;
			}
		}

		public bool IsOnlineMeeting
		{
			get
			{
				return this.isOnlineMeetingField;
			}
			set
			{
				this.isOnlineMeetingField = value;
			}
		}

		[XmlIgnore]
		public bool IsOnlineMeetingSpecified
		{
			get
			{
				return this.isOnlineMeetingFieldSpecified;
			}
			set
			{
				this.isOnlineMeetingFieldSpecified = value;
			}
		}

		public string MeetingWorkspaceUrl
		{
			get
			{
				return this.meetingWorkspaceUrlField;
			}
			set
			{
				this.meetingWorkspaceUrlField = value;
			}
		}

		public string NetShowUrl
		{
			get
			{
				return this.netShowUrlField;
			}
			set
			{
				this.netShowUrlField = value;
			}
		}

		public EnhancedLocationType EnhancedLocation
		{
			get
			{
				return this.enhancedLocationField;
			}
			set
			{
				this.enhancedLocationField = value;
			}
		}

		public DateTime StartWallClock
		{
			get
			{
				return this.startWallClockField;
			}
			set
			{
				this.startWallClockField = value;
			}
		}

		[XmlIgnore]
		public bool StartWallClockSpecified
		{
			get
			{
				return this.startWallClockFieldSpecified;
			}
			set
			{
				this.startWallClockFieldSpecified = value;
			}
		}

		public DateTime EndWallClock
		{
			get
			{
				return this.endWallClockField;
			}
			set
			{
				this.endWallClockField = value;
			}
		}

		[XmlIgnore]
		public bool EndWallClockSpecified
		{
			get
			{
				return this.endWallClockFieldSpecified;
			}
			set
			{
				this.endWallClockFieldSpecified = value;
			}
		}

		public string StartTimeZoneId
		{
			get
			{
				return this.startTimeZoneIdField;
			}
			set
			{
				this.startTimeZoneIdField = value;
			}
		}

		public string EndTimeZoneId
		{
			get
			{
				return this.endTimeZoneIdField;
			}
			set
			{
				this.endTimeZoneIdField = value;
			}
		}

		public LegacyFreeBusyType IntendedFreeBusyStatus
		{
			get
			{
				return this.intendedFreeBusyStatusField;
			}
			set
			{
				this.intendedFreeBusyStatusField = value;
			}
		}

		[XmlIgnore]
		public bool IntendedFreeBusyStatusSpecified
		{
			get
			{
				return this.intendedFreeBusyStatusFieldSpecified;
			}
			set
			{
				this.intendedFreeBusyStatusFieldSpecified = value;
			}
		}

		public string JoinOnlineMeetingUrl
		{
			get
			{
				return this.joinOnlineMeetingUrlField;
			}
			set
			{
				this.joinOnlineMeetingUrlField = value;
			}
		}

		public OnlineMeetingSettingsType OnlineMeetingSettings
		{
			get
			{
				return this.onlineMeetingSettingsField;
			}
			set
			{
				this.onlineMeetingSettingsField = value;
			}
		}

		public bool IsOrganizer
		{
			get
			{
				return this.isOrganizerField;
			}
			set
			{
				this.isOrganizerField = value;
			}
		}

		[XmlIgnore]
		public bool IsOrganizerSpecified
		{
			get
			{
				return this.isOrganizerFieldSpecified;
			}
			set
			{
				this.isOrganizerFieldSpecified = value;
			}
		}

		private string uIDField;

		private DateTime recurrenceIdField;

		private bool recurrenceIdFieldSpecified;

		private DateTime dateTimeStampField;

		private bool dateTimeStampFieldSpecified;

		private DateTime startField;

		private bool startFieldSpecified;

		private DateTime endField;

		private bool endFieldSpecified;

		private DateTime originalStartField;

		private bool originalStartFieldSpecified;

		private bool isAllDayEventField;

		private bool isAllDayEventFieldSpecified;

		private LegacyFreeBusyType legacyFreeBusyStatusField;

		private bool legacyFreeBusyStatusFieldSpecified;

		private string locationField;

		private string whenField;

		private bool isMeetingField;

		private bool isMeetingFieldSpecified;

		private bool isCancelledField;

		private bool isCancelledFieldSpecified;

		private bool isRecurringField;

		private bool isRecurringFieldSpecified;

		private bool meetingRequestWasSentField;

		private bool meetingRequestWasSentFieldSpecified;

		private bool isResponseRequestedField;

		private bool isResponseRequestedFieldSpecified;

		private CalendarItemTypeType calendarItemType1Field;

		private bool calendarItemType1FieldSpecified;

		private ResponseTypeType myResponseTypeField;

		private bool myResponseTypeFieldSpecified;

		private SingleRecipientType organizerField;

		private AttendeeType[] requiredAttendeesField;

		private AttendeeType[] optionalAttendeesField;

		private AttendeeType[] resourcesField;

		private int conflictingMeetingCountField;

		private bool conflictingMeetingCountFieldSpecified;

		private int adjacentMeetingCountField;

		private bool adjacentMeetingCountFieldSpecified;

		private NonEmptyArrayOfAllItemsType conflictingMeetingsField;

		private NonEmptyArrayOfAllItemsType adjacentMeetingsField;

		private string durationField;

		private string timeZoneField;

		private DateTime appointmentReplyTimeField;

		private bool appointmentReplyTimeFieldSpecified;

		private int appointmentSequenceNumberField;

		private bool appointmentSequenceNumberFieldSpecified;

		private int appointmentStateField;

		private bool appointmentStateFieldSpecified;

		private RecurrenceType recurrenceField;

		private OccurrenceInfoType firstOccurrenceField;

		private OccurrenceInfoType lastOccurrenceField;

		private OccurrenceInfoType[] modifiedOccurrencesField;

		private DeletedOccurrenceInfoType[] deletedOccurrencesField;

		private TimeZoneType meetingTimeZoneField;

		private TimeZoneDefinitionType startTimeZoneField;

		private TimeZoneDefinitionType endTimeZoneField;

		private int conferenceTypeField;

		private bool conferenceTypeFieldSpecified;

		private bool allowNewTimeProposalField;

		private bool allowNewTimeProposalFieldSpecified;

		private bool isOnlineMeetingField;

		private bool isOnlineMeetingFieldSpecified;

		private string meetingWorkspaceUrlField;

		private string netShowUrlField;

		private EnhancedLocationType enhancedLocationField;

		private DateTime startWallClockField;

		private bool startWallClockFieldSpecified;

		private DateTime endWallClockField;

		private bool endWallClockFieldSpecified;

		private string startTimeZoneIdField;

		private string endTimeZoneIdField;

		private LegacyFreeBusyType intendedFreeBusyStatusField;

		private bool intendedFreeBusyStatusFieldSpecified;

		private string joinOnlineMeetingUrlField;

		private OnlineMeetingSettingsType onlineMeetingSettingsField;

		private bool isOrganizerField;

		private bool isOrganizerFieldSpecified;
	}
}
