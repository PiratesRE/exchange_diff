using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class CalendarItemType : ItemType
	{
		public string UID;

		public DateTime RecurrenceId;

		[XmlIgnore]
		public bool RecurrenceIdSpecified;

		public DateTime DateTimeStamp;

		[XmlIgnore]
		public bool DateTimeStampSpecified;

		public DateTime Start;

		[XmlIgnore]
		public bool StartSpecified;

		public DateTime End;

		[XmlIgnore]
		public bool EndSpecified;

		public DateTime OriginalStart;

		[XmlIgnore]
		public bool OriginalStartSpecified;

		public bool IsAllDayEvent;

		[XmlIgnore]
		public bool IsAllDayEventSpecified;

		public LegacyFreeBusyType LegacyFreeBusyStatus;

		[XmlIgnore]
		public bool LegacyFreeBusyStatusSpecified;

		public string Location;

		public string When;

		public bool IsMeeting;

		[XmlIgnore]
		public bool IsMeetingSpecified;

		public bool IsCancelled;

		[XmlIgnore]
		public bool IsCancelledSpecified;

		public bool IsRecurring;

		[XmlIgnore]
		public bool IsRecurringSpecified;

		public bool MeetingRequestWasSent;

		[XmlIgnore]
		public bool MeetingRequestWasSentSpecified;

		public bool IsResponseRequested;

		[XmlIgnore]
		public bool IsResponseRequestedSpecified;

		[XmlElement("CalendarItemType")]
		public CalendarItemTypeType CalendarItemType1;

		[XmlIgnore]
		public bool CalendarItemType1Specified;

		public ResponseTypeType MyResponseType;

		[XmlIgnore]
		public bool MyResponseTypeSpecified;

		public SingleRecipientType Organizer;

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] RequiredAttendees;

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] OptionalAttendees;

		[XmlArrayItem("Attendee", IsNullable = false)]
		public AttendeeType[] Resources;

		public int ConflictingMeetingCount;

		[XmlIgnore]
		public bool ConflictingMeetingCountSpecified;

		public int AdjacentMeetingCount;

		[XmlIgnore]
		public bool AdjacentMeetingCountSpecified;

		public NonEmptyArrayOfAllItemsType ConflictingMeetings;

		public NonEmptyArrayOfAllItemsType AdjacentMeetings;

		public string Duration;

		public string TimeZone;

		public DateTime AppointmentReplyTime;

		[XmlIgnore]
		public bool AppointmentReplyTimeSpecified;

		public int AppointmentSequenceNumber;

		[XmlIgnore]
		public bool AppointmentSequenceNumberSpecified;

		public int AppointmentState;

		[XmlIgnore]
		public bool AppointmentStateSpecified;

		public RecurrenceType Recurrence;

		public OccurrenceInfoType FirstOccurrence;

		public OccurrenceInfoType LastOccurrence;

		[XmlArrayItem("Occurrence", IsNullable = false)]
		public OccurrenceInfoType[] ModifiedOccurrences;

		[XmlArrayItem("DeletedOccurrence", IsNullable = false)]
		public DeletedOccurrenceInfoType[] DeletedOccurrences;

		public TimeZoneType MeetingTimeZone;

		public TimeZoneDefinitionType StartTimeZone;

		public TimeZoneDefinitionType EndTimeZone;

		public int ConferenceType;

		[XmlIgnore]
		public bool ConferenceTypeSpecified;

		public bool AllowNewTimeProposal;

		[XmlIgnore]
		public bool AllowNewTimeProposalSpecified;

		public bool IsOnlineMeeting;

		[XmlIgnore]
		public bool IsOnlineMeetingSpecified;

		public string MeetingWorkspaceUrl;

		public string NetShowUrl;

		public EnhancedLocationType EnhancedLocation;

		public DateTime StartWallClock;

		[XmlIgnore]
		public bool StartWallClockSpecified;

		public DateTime EndWallClock;

		[XmlIgnore]
		public bool EndWallClockSpecified;

		public string StartTimeZoneId;

		public string EndTimeZoneId;

		public LegacyFreeBusyType IntendedFreeBusyStatus;

		[XmlIgnore]
		public bool IntendedFreeBusyStatusSpecified;

		public string JoinOnlineMeetingUrl;

		public OnlineMeetingSettingsType OnlineMeetingSettings;

		public bool IsOrganizer;

		[XmlIgnore]
		public bool IsOrganizerSpecified;
	}
}
