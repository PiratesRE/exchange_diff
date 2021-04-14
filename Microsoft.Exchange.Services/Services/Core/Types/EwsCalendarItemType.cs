using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "CalendarItem")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "CalendarItemType")]
	[Serializable]
	public class EwsCalendarItemType : ItemType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string UID
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.ICalendarUid);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ICalendarUid] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string RecurrenceId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.ICalendarRecurrenceId);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ICalendarRecurrenceId] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool RecurrenceIdSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.ICalendarRecurrenceId);
			}
			set
			{
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string DateTimeStamp
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.ICalendarDateTimeStamp);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.ICalendarDateTimeStamp] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DateTimeStampSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.ICalendarDateTimeStamp);
			}
			set
			{
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string Start
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.Start);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.Start] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool StartSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.Start);
			}
			set
			{
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string End
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.End);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.End] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool EndSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.End);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		[DateTimeString]
		public string OriginalStart
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.OriginalStart);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OriginalStart] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
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

		[DataMember(EmitDefaultValue = true, Order = 7)]
		public bool? IsAllDayEvent
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsAllDayEvent);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.IsAllDayEvent, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
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

		[XmlElement]
		[IgnoreDataMember]
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
		[DataMember(Name = "FreeBusyType", EmitDefaultValue = false, Order = 8)]
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
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.Location);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.Location] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
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

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public bool? IsMeeting
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsMeeting);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.IsMeeting, value);
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

		[DataMember(EmitDefaultValue = false, Order = 11)]
		public bool? IsCancelled
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsCancelled);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.IsCancelled, value);
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

		[DataMember(EmitDefaultValue = false, Order = 12)]
		public bool? IsRecurring
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsRecurring);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.IsRecurring, value);
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

		[DataMember(EmitDefaultValue = false, Order = 13)]
		public bool? MeetingRequestWasSent
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.MeetingRequestWasSent);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.MeetingRequestWasSent, value);
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

		[DataMember(EmitDefaultValue = false, Order = 14)]
		public bool? IsResponseRequested
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.IsResponseRequested))
				{
					return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.IsResponseRequested);
				}
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.AttendeeSpecific.IsResponseRequested);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.IsResponseRequested, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsResponseRequestedSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.OrganizerSpecific.IsResponseRequested) || base.IsSet(CalendarItemSchema.AttendeeSpecific.IsResponseRequested);
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

		[XmlIgnore]
		[DataMember(Name = "CalendarItemType", EmitDefaultValue = false, Order = 15)]
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

		[XmlIgnore]
		[IgnoreDataMember]
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

		[IgnoreDataMember]
		[XmlElement]
		public ResponseTypeType MyResponseType
		{
			get
			{
				if (!this.MyResponseTypeSpecified)
				{
					return ResponseTypeType.Unknown;
				}
				return EnumUtilities.Parse<ResponseTypeType>(this.MyResponseTypeString);
			}
			set
			{
				this.MyResponseTypeString = EnumUtilities.ToString<ResponseTypeType>(value);
			}
		}

		[DataMember(Name = "ResponseType", EmitDefaultValue = false, Order = 16)]
		[XmlIgnore]
		public string MyResponseTypeString
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

		[XmlIgnore]
		[IgnoreDataMember]
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

		[DataMember(EmitDefaultValue = false, Order = 17)]
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

		[DataMember(EmitDefaultValue = false, Order = 18)]
		[XmlArrayItem("Attendee", IsNullable = false)]
		public EwsAttendeeType[] RequiredAttendees
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.RequiredAttendees))
				{
					return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.OrganizerSpecific.RequiredAttendees);
				}
				return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.RequiredAttendees);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.RequiredAttendees] = value;
			}
		}

		[XmlArrayItem("Attendee", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 19)]
		public EwsAttendeeType[] OptionalAttendees
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.OptionalAttendees))
				{
					return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.OrganizerSpecific.OptionalAttendees);
				}
				return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.OptionalAttendees);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.OptionalAttendees] = value;
			}
		}

		[XmlArrayItem("Attendee", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 20)]
		public EwsAttendeeType[] Resources
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.Resources))
				{
					return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.OrganizerSpecific.Resources);
				}
				return base.PropertyBag.GetValueOrDefault<EwsAttendeeType[]>(CalendarItemSchema.AttendeeSpecific.Resources);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.Resources] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 21)]
		public int? ConflictingMeetingCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.ConflictingMeetingCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(CalendarItemSchema.ConflictingMeetingCount, value);
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

		[DataMember(EmitDefaultValue = false, Order = 22)]
		public int? AdjacentMeetingCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.AdjacentMeetingCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(CalendarItemSchema.AdjacentMeetingCount, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
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

		[DataMember(EmitDefaultValue = false, Order = 23)]
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

		[DataMember(EmitDefaultValue = false, Order = 24)]
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

		[DataMember(EmitDefaultValue = false, Order = 25)]
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

		[DataMember(EmitDefaultValue = false, Order = 26)]
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
		[DataMember(EmitDefaultValue = false, Order = 27)]
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

		[DataMember(EmitDefaultValue = false, Order = 28)]
		public int? AppointmentSequenceNumber
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.AppointmentSequenceNumber);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(CalendarItemSchema.AppointmentSequenceNumber, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
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

		[DataMember(EmitDefaultValue = false, Order = 29)]
		public int? AppointmentState
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.AppointmentState);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(CalendarItemSchema.AppointmentState, value);
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

		[DataMember(EmitDefaultValue = false, Order = 30)]
		public RecurrenceType Recurrence
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.Recurrence))
				{
					return base.PropertyBag.GetValueOrDefault<RecurrenceType>(CalendarItemSchema.OrganizerSpecific.Recurrence);
				}
				return base.PropertyBag.GetValueOrDefault<RecurrenceType>(CalendarItemSchema.AttendeeSpecific.Recurrence);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 31)]
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

		[DataMember(EmitDefaultValue = false, Order = 32)]
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

		[XmlArrayItem("Occurrence", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 33)]
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

		[XmlArrayItem("DeletedOccurrence", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 34)]
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

		[DataMember(EmitDefaultValue = false, Order = 35)]
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

		[DataMember(EmitDefaultValue = false, Order = 36)]
		public TimeZoneDefinitionType StartTimeZone
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.StartTimeZone))
				{
					return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.OrganizerSpecific.StartTimeZone);
				}
				return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.AttendeeSpecific.StartTimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.StartTimeZone] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 37)]
		public TimeZoneDefinitionType EndTimeZone
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.EndTimeZone))
				{
					return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.OrganizerSpecific.EndTimeZone);
				}
				return base.PropertyBag.GetValueOrDefault<TimeZoneDefinitionType>(CalendarItemSchema.AttendeeSpecific.EndTimeZone);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.EndTimeZone] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 38)]
		public int? ConferenceType
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.ConferenceType))
				{
					return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.OrganizerSpecific.ConferenceType);
				}
				return base.PropertyBag.GetNullableValue<int>(CalendarItemSchema.AttendeeSpecific.ConferenceType);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(CalendarItemSchema.OrganizerSpecific.ConferenceType, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ConferenceTypeSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.OrganizerSpecific.ConferenceType) || base.IsSet(CalendarItemSchema.AttendeeSpecific.ConferenceType);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 39)]
		public bool? AllowNewTimeProposal
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal))
				{
					return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal);
				}
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AllowNewTimeProposalSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal) || base.IsSet(CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 40)]
		public bool? IsOnlineMeeting
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.IsOnlineMeeting))
				{
					return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.IsOnlineMeeting);
				}
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(CalendarItemSchema.OrganizerSpecific.IsOnlineMeeting, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsOnlineMeetingSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.OrganizerSpecific.IsOnlineMeeting) || base.IsSet(CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 41)]
		public string MeetingWorkspaceUrl
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.MeetingWorkspaceUrl))
				{
					return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.OrganizerSpecific.MeetingWorkspaceUrl);
				}
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AttendeeSpecific.MeetingWorkspaceUrl);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.MeetingWorkspaceUrl] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 42)]
		public string NetShowUrl
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.NetShowUrl))
				{
					return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.OrganizerSpecific.NetShowUrl);
				}
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AttendeeSpecific.NetShowUrl);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.NetShowUrl] = value;
			}
		}

		[DataMember(Name = "Location", EmitDefaultValue = false, Order = 43)]
		public EnhancedLocationType EnhancedLocation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EnhancedLocationType>(CalendarItemSchema.EnhancedLocation);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.EnhancedLocation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 44)]
		[DateTimeString]
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

		[DataMember(EmitDefaultValue = false, Order = 45)]
		[DateTimeString]
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

		[DataMember(EmitDefaultValue = false, Order = 46)]
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

		[DataMember(EmitDefaultValue = false, Order = 47)]
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

		[DataMember(Name = "IntendedFreeBusyStatus", EmitDefaultValue = false, Order = 48)]
		[XmlIgnore]
		public string IntendedFreeBusyStatusString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.IntendedFreeBusyStatus);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IntendedFreeBusyStatus] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 49)]
		public string JoinOnlineMeetingUrl
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.JoinOnlineMeetingUrl);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 50)]
		public OnlineMeetingSettingsType OnlineMeetingSettings
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<OnlineMeetingSettingsType>(CalendarItemSchema.OnlineMeetingSettings);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 51)]
		public bool? IsOrganizer
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsOrganizer);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.IsOrganizer] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsOrganizerSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IsOrganizer);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IntendedFreeBusyStatusSpecified
		{
			get
			{
				return base.IsSet(CalendarItemSchema.IntendedFreeBusyStatus);
			}
			set
			{
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.CalendarItem;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 52)]
		public string AppointmentReplyName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(CalendarItemSchema.AppointmentReplyName);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.AppointmentReplyName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 53)]
		[XmlIgnore]
		public bool? IsSeriesCancelled
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(CalendarItemSchema.IsSeriesCancelled);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 54)]
		public InboxReminderType[] InboxReminders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<InboxReminderType[]>(CalendarItemSchema.InboxReminders);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.InboxReminders] = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 55)]
		public AttendeeCountsType AttendeeCounts
		{
			get
			{
				if (base.IsSet(CalendarItemSchema.OrganizerSpecific.AttendeeCounts))
				{
					return base.PropertyBag.GetValueOrDefault<AttendeeCountsType>(CalendarItemSchema.OrganizerSpecific.AttendeeCounts);
				}
				return base.PropertyBag.GetValueOrDefault<AttendeeCountsType>(CalendarItemSchema.AttendeeSpecific.AttendeeCounts);
			}
			set
			{
				base.PropertyBag[CalendarItemSchema.OrganizerSpecific.AttendeeCounts] = value;
			}
		}
	}
}
