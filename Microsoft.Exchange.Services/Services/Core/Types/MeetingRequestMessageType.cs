using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MeetingRequestMessageType : MeetingMessageType
	{
		[IgnoreDataMember]
		[XmlElement("MeetingRequestType")]
		public RequestType MeetingRequestType
		{
			get
			{
				if (!this.MeetingRequestTypeSpecified)
				{
					return RequestType.None;
				}
				return EnumUtilities.Parse<RequestType>(this.MeetingRequestTypeString);
			}
			set
			{
				this.MeetingRequestTypeString = EnumUtilities.ToString<RequestType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "MeetingRequestType", EmitDefaultValue = false, Order = 1)]
		public string MeetingRequestTypeString
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingRequestSchema.MeetingRequestType);
			}
			set
			{
				this[MeetingRequestSchema.MeetingRequestType] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool MeetingRequestTypeSpecified
		{
			get
			{
				return base.IsSet(MeetingRequestSchema.MeetingRequestType);
			}
			set
			{
			}
		}

		[XmlElement("IntendedFreeBusyStatus")]
		[IgnoreDataMember]
		public Microsoft.Exchange.InfoWorker.Common.Availability.BusyType IntendedFreeBusyStatus
		{
			get
			{
				if (!this.IntendedFreeBusyStatusSpecified)
				{
					return Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Free;
				}
				return EnumUtilities.Parse<Microsoft.Exchange.InfoWorker.Common.Availability.BusyType>(this.IntendedFreeBusyStatusString);
			}
			set
			{
				this.IntendedFreeBusyStatusString = EnumUtilities.ToString<Microsoft.Exchange.InfoWorker.Common.Availability.BusyType>(value);
			}
		}

		[DataMember(Name = "IntendedFreeBusyStatus", EmitDefaultValue = false, Order = 2)]
		[XmlIgnore]
		public string IntendedFreeBusyStatusString
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingRequestSchema.IntendedFreeBusyStatus);
			}
			set
			{
				this[MeetingRequestSchema.IntendedFreeBusyStatus] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IntendedFreeBusyStatusSpecified
		{
			get
			{
				return base.IsSet(MeetingRequestSchema.IntendedFreeBusyStatus);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		[DateTimeString]
		public string Start
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingRequestSchema.Start);
			}
			set
			{
				this[MeetingRequestSchema.Start] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool StartSpecified
		{
			get
			{
				return base.IsSet(MeetingRequestSchema.Start);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		[DateTimeString]
		public string End
		{
			get
			{
				return base.GetValueOrDefault<string>(MeetingRequestSchema.End);
			}
			set
			{
				this[MeetingRequestSchema.End] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool EndSpecified
		{
			get
			{
				return base.IsSet(MeetingRequestSchema.End);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		[DateTimeString]
		public string OriginalStart
		{
			get
			{
				return base.GetValueOrDefault<string>(CalendarItemSchema.OriginalStart);
			}
			set
			{
				this[CalendarItemSchema.OriginalStart] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool OriginalStartSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.OriginalStart);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public bool? IsAllDayEvent
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.IsAllDayEvent);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IsAllDayEvent] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsAllDayEventSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsAllDayEvent);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public Microsoft.Exchange.InfoWorker.Common.Availability.BusyType LegacyFreeBusyStatus
		{
			get
			{
				if (!this.LegacyFreeBusyStatusSpecified)
				{
					return Microsoft.Exchange.InfoWorker.Common.Availability.BusyType.Free;
				}
				return EnumUtilities.Parse<Microsoft.Exchange.InfoWorker.Common.Availability.BusyType>(this.LegacyFreeBusyStatusString);
			}
			set
			{
				this.LegacyFreeBusyStatusString = EnumUtilities.ToString<Microsoft.Exchange.InfoWorker.Common.Availability.BusyType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "FreeBusyType", EmitDefaultValue = false, Order = 7)]
		public string LegacyFreeBusyStatusString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.LegacyFreeBusyStatus);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.LegacyFreeBusyStatus] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool LegacyFreeBusyStatusSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.LegacyFreeBusyStatus);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		public string Location
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(MeetingRequestSchema.Location);
			}
			set
			{
				base.PropertyBag[MeetingRequestSchema.Location] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public string When
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.When);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.When] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public bool? IsMeeting
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.IsMeeting);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IsMeeting] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsMeetingSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsMeeting);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public bool? IsCancelled
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.IsCancelled);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IsCancelled] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsCancelledSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsCancelled);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 11)]
		public bool? IsRecurring
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.IsRecurring);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IsRecurring] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsRecurringSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsRecurring);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 12)]
		public bool? MeetingRequestWasSent
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.MeetingRequestWasSent);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.MeetingRequestWasSent] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MeetingRequestWasSentSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.MeetingRequestWasSent);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlElement("CalendarItemType")]
		public CalendarItemTypeType CalendarItemType
		{
			get
			{
				if (!this.CalendarItemTypeSpecified)
				{
					return CalendarItemTypeType.Single;
				}
				return EnumUtilities.Parse<CalendarItemTypeType>(this.CalendarItemTypeString);
			}
			set
			{
				this.CalendarItemTypeString = EnumUtilities.ToString<CalendarItemTypeType>(value);
			}
		}

		[DataMember(Name = "CalendarItemType", EmitDefaultValue = false, Order = 13)]
		[XmlIgnore]
		public string CalendarItemTypeString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.CalendarItemType);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.CalendarItemType] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool CalendarItemTypeSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.CalendarItemType);
			}
			set
			{
			}
		}

		[XmlElement]
		[DataMember(Name = "MyResponseType", EmitDefaultValue = false, Order = 14)]
		public string MyResponseType
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.MyResponseType);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.MyResponseType] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool MyResponseTypeSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.MyResponseType);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 15)]
		public SingleRecipientType Organizer
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(CalendarItemSchema.Organizer);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.Organizer] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 16)]
		[XmlArrayItem("Attendee", IsNullable = false)]
		public EwsAttendeeType[] RequiredAttendees
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.RequiredAttendees);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.RequiredAttendees] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 17)]
		[XmlArrayItem("Attendee", IsNullable = false)]
		public EwsAttendeeType[] OptionalAttendees
		{
			get
			{
				return base.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.OptionalAttendees);
			}
			set
			{
				this[CalendarItemSchema.AttendeeSpecific.OptionalAttendees] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 18)]
		[XmlArrayItem("Attendee", IsNullable = false)]
		public EwsAttendeeType[] Resources
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.Resources);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.Resources] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 19)]
		public int? ConflictingMeetingCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(CalendarItemSchema.ConflictingMeetingCount);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ConflictingMeetingCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ConflictingMeetingCountSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.ConflictingMeetingCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		public int? AdjacentMeetingCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(CalendarItemSchema.AdjacentMeetingCount);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AdjacentMeetingCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AdjacentMeetingCountSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AdjacentMeetingCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 21)]
		public NonEmptyArrayOfAllItemsType ConflictingMeetings
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<NonEmptyArrayOfAllItemsType>(CalendarItemSchema.ConflictingMeetings);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ConflictingMeetings] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 22)]
		public NonEmptyArrayOfAllItemsType AdjacentMeetings
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<NonEmptyArrayOfAllItemsType>(CalendarItemSchema.AdjacentMeetings);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AdjacentMeetings] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 23)]
		public string Duration
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.Duration);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.Duration] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 24)]
		public string TimeZone
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.TimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.TimeZone] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 25)]
		public string AppointmentReplyTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AppointmentReplyTime);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AppointmentReplyTime] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AppointmentReplyTimeSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AppointmentReplyTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 26)]
		public int? AppointmentSequenceNumber
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(CalendarItemSchema.AppointmentSequenceNumber);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AppointmentSequenceNumber] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AppointmentSequenceNumberSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AppointmentSequenceNumber);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 27)]
		public int? AppointmentState
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(CalendarItemSchema.AppointmentState);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AppointmentState] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AppointmentStateSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AppointmentState);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 28)]
		public RecurrenceType Recurrence
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RecurrenceType>(CalendarItemSchema.OrganizerSpecific.Recurrence);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 29)]
		public OccurrenceInfoType FirstOccurrence
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<OccurrenceInfoType>(CalendarItemSchema.FirstOccurrence);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.FirstOccurrence] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 30)]
		public OccurrenceInfoType LastOccurrence
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<OccurrenceInfoType>(CalendarItemSchema.LastOccurrence);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.LastOccurrence] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 31)]
		[XmlArrayItem("Occurrence", IsNullable = false)]
		public OccurrenceInfoType[] ModifiedOccurrences
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<OccurrenceInfoType[]>(CalendarItemSchema.ModifiedOccurrences);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ModifiedOccurrences] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 32)]
		[XmlArrayItem("DeletedOccurrence", IsNullable = false)]
		public DeletedOccurrenceInfoType[] DeletedOccurrences
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<DeletedOccurrenceInfoType[]>(CalendarItemSchema.DeletedOccurrences);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.DeletedOccurrences] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 33)]
		public TimeZoneType MeetingTimeZone
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<TimeZoneType>(CalendarItemSchema.MeetingTimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.MeetingTimeZone] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 34)]
		public TimeZoneDefinitionType StartTimeZone
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.OrganizerSpecific.StartTimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.StartTimeZone] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 35)]
		public TimeZoneDefinitionType EndTimeZone
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.OrganizerSpecific.EndTimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.EndTimeZone] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 36)]
		public int? ConferenceType
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(CalendarItemSchema.AttendeeSpecific.ConferenceType);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.ConferenceType] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ConferenceTypeSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AttendeeSpecific.ConferenceType);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 37)]
		public bool? AllowNewTimeProposal
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AllowNewTimeProposalSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 38)]
		public bool? IsOnlineMeeting
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool?>(CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsOnlineMeetingSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 39)]
		public string MeetingWorkspaceUrl
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AttendeeSpecific.MeetingWorkspaceUrl);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.MeetingWorkspaceUrl] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 40)]
		public string NetShowUrl
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AttendeeSpecific.NetShowUrl);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.NetShowUrl] = value;
			}
		}

		[DataMember(Name = "Location", EmitDefaultValue = false, Order = 41)]
		public EnhancedLocationType EnhancedLocation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EnhancedLocationType>(MeetingRequestSchema.EnhancedLocation);
			}
			set
			{
				base.PropertyBag[MeetingRequestSchema.EnhancedLocation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 42)]
		public ChangeHighlightsType ChangeHighlights
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ChangeHighlightsType>(MeetingRequestSchema.ChangeHighlights);
			}
			set
			{
				base.PropertyBag[MeetingRequestSchema.ChangeHighlights] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 43)]
		public string StartWallClock
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.StartWallClock);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.StartWallClock] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 44)]
		public string EndWallClock
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.EndWallClock);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.EndWallClock] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 45)]
		public string StartTimeZoneId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.StartTimeZoneId);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.StartTimeZoneId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 46)]
		public string EndTimeZoneId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.EndTimeZoneId);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.EndTimeZoneId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 47)]
		[XmlIgnore]
		public AttendeeCountsType AttendeeCounts
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<AttendeeCountsType>(CalendarItemSchema.AttendeeSpecific.AttendeeCounts);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AttendeeSpecific.AttendeeCounts] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.MeetingRequest;
			}
		}
	}
}
