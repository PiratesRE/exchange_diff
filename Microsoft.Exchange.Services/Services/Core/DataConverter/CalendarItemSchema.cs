using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarItemSchema : Schema
	{
		static CalendarItemSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				CalendarItemSchema.ICalendarUid,
				CalendarItemSchema.ICalendarRecurrenceId,
				CalendarItemSchema.ICalendarDateTimeStamp,
				CalendarItemSchema.Start,
				CalendarItemSchema.End,
				CalendarItemSchema.OriginalStart,
				CalendarItemSchema.IsAllDayEvent,
				CalendarItemSchema.LegacyFreeBusyStatus,
				CalendarItemSchema.Location,
				CalendarItemSchema.EnhancedLocation,
				CalendarItemSchema.When,
				CalendarItemSchema.IsMeeting,
				CalendarItemSchema.IsCancelled,
				CalendarItemSchema.IsRecurring,
				CalendarItemSchema.MeetingRequestWasSent,
				CalendarItemSchema.OrganizerSpecific.IsResponseRequested,
				CalendarItemSchema.CalendarItemType,
				CalendarItemSchema.MyResponseType,
				CalendarItemSchema.Organizer,
				CalendarItemSchema.OrganizerSpecific.RequiredAttendees,
				CalendarItemSchema.OrganizerSpecific.OptionalAttendees,
				CalendarItemSchema.OrganizerSpecific.Resources,
				CalendarItemSchema.ConflictingMeetingCount,
				CalendarItemSchema.AdjacentMeetingCount,
				CalendarItemSchema.ConflictingMeetings,
				CalendarItemSchema.AdjacentMeetings,
				CalendarItemSchema.Duration,
				CalendarItemSchema.TimeZone,
				CalendarItemSchema.AppointmentReplyTime,
				CalendarItemSchema.AppointmentSequenceNumber,
				CalendarItemSchema.AppointmentState,
				CalendarItemSchema.OrganizerSpecific.Recurrence,
				CalendarItemSchema.FirstOccurrence,
				CalendarItemSchema.LastOccurrence,
				CalendarItemSchema.ModifiedOccurrences,
				CalendarItemSchema.DeletedOccurrences,
				CalendarItemSchema.MeetingTimeZone,
				CalendarItemSchema.OrganizerSpecific.StartTimeZone,
				CalendarItemSchema.OrganizerSpecific.EndTimeZone,
				CalendarItemSchema.OrganizerSpecific.ConferenceType,
				CalendarItemSchema.OrganizerSpecific.AllowNewTimeProposal,
				CalendarItemSchema.OrganizerSpecific.IsOnlineMeeting,
				CalendarItemSchema.OrganizerSpecific.MeetingWorkspaceUrl,
				CalendarItemSchema.OrganizerSpecific.NetShowUrl,
				CalendarItemSchema.StartWallClock,
				CalendarItemSchema.EndWallClock,
				CalendarItemSchema.StartTimeZoneId,
				CalendarItemSchema.EndTimeZoneId,
				CalendarItemSchema.IntendedFreeBusyStatus,
				CalendarItemSchema.JoinOnlineMeetingUrl,
				CalendarItemSchema.OnlineMeetingSettings,
				CalendarItemSchema.IsOrganizer,
				CalendarItemSchema.AppointmentReplyName,
				CalendarItemSchema.IsSeriesCancelled,
				CalendarItemSchema.InboxReminders,
				CalendarItemSchema.OrganizerSpecific.AttendeeCounts
			};
			XmlElementInformation[] xmlElements2 = new XmlElementInformation[]
			{
				CalendarItemSchema.ICalendarUid,
				CalendarItemSchema.ICalendarRecurrenceId,
				CalendarItemSchema.ICalendarDateTimeStamp,
				CalendarItemSchema.Start,
				CalendarItemSchema.End,
				CalendarItemSchema.OriginalStart,
				CalendarItemSchema.IsAllDayEvent,
				CalendarItemSchema.LegacyFreeBusyStatus,
				CalendarItemSchema.Location,
				CalendarItemSchema.EnhancedLocation,
				CalendarItemSchema.When,
				CalendarItemSchema.IsMeeting,
				CalendarItemSchema.IsCancelled,
				CalendarItemSchema.IsRecurring,
				CalendarItemSchema.MeetingRequestWasSent,
				CalendarItemSchema.AttendeeSpecific.IsResponseRequested,
				CalendarItemSchema.CalendarItemType,
				CalendarItemSchema.MyResponseType,
				CalendarItemSchema.Organizer,
				CalendarItemSchema.AttendeeSpecific.RequiredAttendees,
				CalendarItemSchema.AttendeeSpecific.OptionalAttendees,
				CalendarItemSchema.AttendeeSpecific.Resources,
				CalendarItemSchema.ConflictingMeetingCount,
				CalendarItemSchema.AdjacentMeetingCount,
				CalendarItemSchema.ConflictingMeetings,
				CalendarItemSchema.AdjacentMeetings,
				CalendarItemSchema.Duration,
				CalendarItemSchema.TimeZone,
				CalendarItemSchema.AppointmentReplyTime,
				CalendarItemSchema.AppointmentSequenceNumber,
				CalendarItemSchema.AppointmentState,
				CalendarItemSchema.AttendeeSpecific.Recurrence,
				CalendarItemSchema.FirstOccurrence,
				CalendarItemSchema.LastOccurrence,
				CalendarItemSchema.ModifiedOccurrences,
				CalendarItemSchema.DeletedOccurrences,
				CalendarItemSchema.MeetingTimeZone,
				CalendarItemSchema.AttendeeSpecific.StartTimeZone,
				CalendarItemSchema.AttendeeSpecific.EndTimeZone,
				CalendarItemSchema.AttendeeSpecific.ConferenceType,
				CalendarItemSchema.AttendeeSpecific.AllowNewTimeProposal,
				CalendarItemSchema.AttendeeSpecific.IsOnlineMeeting,
				CalendarItemSchema.AttendeeSpecific.MeetingWorkspaceUrl,
				CalendarItemSchema.AttendeeSpecific.NetShowUrl,
				CalendarItemSchema.StartWallClock,
				CalendarItemSchema.EndWallClock,
				CalendarItemSchema.StartTimeZoneId,
				CalendarItemSchema.EndTimeZoneId,
				CalendarItemSchema.IntendedFreeBusyStatus,
				CalendarItemSchema.JoinOnlineMeetingUrl,
				CalendarItemSchema.OnlineMeetingSettings,
				CalendarItemSchema.IsOrganizer,
				CalendarItemSchema.AppointmentReplyName,
				CalendarItemSchema.IsSeriesCancelled,
				CalendarItemSchema.InboxReminders,
				CalendarItemSchema.AttendeeSpecific.AttendeeCounts
			};
			CalendarItemSchema.schemaForOrganizer_Exchange2010AndEarlier = new CalendarItemSchema(xmlElements, ExchangeVersion.Exchange2010);
			CalendarItemSchema.schemaForAttendee_Exchange2010AndEarlier = new CalendarItemSchema(xmlElements2, ExchangeVersion.Exchange2010);
			CalendarItemSchema.schemaForOrganizer_Exchange2010SP1AndLater = new CalendarItemSchema(xmlElements, ExchangeVersion.Exchange2010SP1);
			CalendarItemSchema.schemaForAttendee_Exchange2010SP1AndLater = new CalendarItemSchema(xmlElements2, ExchangeVersion.Exchange2010SP1);
		}

		private CalendarItemSchema(XmlElementInformation[] xmlElements, ExchangeVersion exchangeVersion) : base(xmlElements)
		{
			IList<PropertyInformation> propertyInformationListByShapeEnum = base.GetPropertyInformationListByShapeEnum(ShapeEnum.AllProperties);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.OrganizerSpecific.StartTimeZone);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.OrganizerSpecific.EndTimeZone);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.AttendeeSpecific.StartTimeZone);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.AttendeeSpecific.EndTimeZone);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.EnhancedLocation);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.AdjacentMeetingCount);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.AdjacentMeetings);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.ConflictingMeetingCount);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.ConflictingMeetings);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.OrganizerSpecific.AttendeeCounts);
			propertyInformationListByShapeEnum.Remove(CalendarItemSchema.AttendeeSpecific.AttendeeCounts);
		}

		public static Schema GetSchema(bool forOrganizer)
		{
			if (forOrganizer)
			{
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
				{
					return CalendarItemSchema.schemaForOrganizer_Exchange2010AndEarlier;
				}
				return CalendarItemSchema.schemaForOrganizer_Exchange2010SP1AndLater;
			}
			else
			{
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
				{
					return CalendarItemSchema.schemaForAttendee_Exchange2010AndEarlier;
				}
				return CalendarItemSchema.schemaForAttendee_Exchange2010SP1AndLater;
			}
		}

		public static readonly PropertyInformation AdjacentMeetingCount = new PropertyInformation(PropertyUriEnum.AdjacentMeetingCount, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AdjacentOrConflictingMeetingsProperty.CreateCommandForAdjacentMeetingCount), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation AdjacentMeetings = new PropertyInformation(PropertyUriEnum.AdjacentMeetings, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AdjacentOrConflictingMeetingsProperty.CreateCommandForAdjacentMeetings), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation AppointmentReplyTime = new PropertyInformation(PropertyUriEnum.AppointmentReplyTime, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.AppointmentReplyTime, new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation AppointmentSequenceNumber = new PropertyInformation(PropertyUriEnum.AppointmentSequenceNumber, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.AppointmentSequenceNumber, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation AppointmentState = new PropertyInformation(PropertyUriEnum.AppointmentState, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.AppointmentState, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CalendarItemType = new PropertyInformation(PropertyUriEnum.CalendarItemType, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.CalendarItemType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ConflictingMeetingCount = new PropertyInformation(PropertyUriEnum.ConflictingMeetingCount, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AdjacentOrConflictingMeetingsProperty.CreateCommandForConflictingMeetingCount), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation ConflictingMeetings = new PropertyInformation(PropertyUriEnum.ConflictingMeetings, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AdjacentOrConflictingMeetingsProperty.CreateCommandForConflictingMeetings), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation DeletedOccurrences = new PropertyInformation(PropertyUriEnum.DeletedOccurrences, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(OccurrenceInfoProperty.CreateCommandForDeletedOccurrences), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation Duration = new PropertyInformation(PropertyUriEnum.Duration.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.Duration.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime
		}, new PropertyUri(PropertyUriEnum.Duration), new PropertyCommand.CreatePropertyCommand(DurationProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation End = new PropertyInformation(PropertyUriEnum.End, ExchangeVersion.Exchange2007, CalendarItemInstanceSchema.EndTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForEnd), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation FirstOccurrence = new PropertyInformation(PropertyUriEnum.FirstOccurrence, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(OccurrenceInfoProperty.CreateCommandForFirstOccurrence), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation ICalendarDateTimeStamp = new PropertyInformation(PropertyUriEnum.DateTimeStamp.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.DateTimeStamp.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, ICalendar.DateTimeStampProperty.PropertiesToLoad, new PropertyUri(PropertyUriEnum.DateTimeStamp), new PropertyCommand.CreatePropertyCommand(ICalendar.DateTimeStampProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ICalendarRecurrenceId = new PropertyInformation(PropertyUriEnum.RecurrenceId.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.RecurrenceId.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007SP1, ICalendar.RecurrenceIdProperty.PropertiesToLoad, new PropertyUri(PropertyUriEnum.RecurrenceId), new PropertyCommand.CreatePropertyCommand(ICalendar.RecurrenceIdProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ICalendarUid = new PropertyInformation(PropertyUriEnum.UID, ExchangeVersion.Exchange2007SP1, ICalendar.UidProperty.PropertyToLoad, new PropertyCommand.CreatePropertyCommand(ICalendar.UidProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation IsAllDayEvent = new PropertyInformation(PropertyUriEnum.IsAllDayEvent, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.MapiIsAllDayEvent, new PropertyCommand.CreatePropertyCommand(IsAllDayEventProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation IsCancelled = new PropertyInformation(PropertyUriEnum.IsCancelled, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(IsCancelledProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsSeriesCancelled = new PropertyInformation(PropertyUriEnum.IsSeriesCancelled, ExchangeVersion.Exchange2012, CalendarItemOccurrenceSchema.IsSeriesCancelled, new PropertyCommand.CreatePropertyCommand(IsSeriesCancelledProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsMeeting = new PropertyInformation(PropertyUriEnum.IsMeeting, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.IsMeeting, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsRecurring = new PropertyInformation(PropertyUriEnum.IsRecurring, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.IsRecurring, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation LastOccurrence = new PropertyInformation(PropertyUriEnum.LastOccurrence, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(OccurrenceInfoProperty.CreateCommandForLastOccurrence), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation LegacyFreeBusyStatus = new PropertyInformation(PropertyUriEnum.LegacyFreeBusyStatus, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.FreeBusyStatus, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Location = new PropertyInformation(PropertyUriEnum.Location, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.Location, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation EnhancedLocation = new PropertyInformation(PropertyUriEnum.EnhancedLocation.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.EnhancedLocation.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, EnhancedLocationProperty.LocationProperties, new PropertyUri(PropertyUriEnum.EnhancedLocation), new PropertyCommand.CreatePropertyCommand(EnhancedLocationProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation MeetingRequestWasSent = new PropertyInformation(PropertyUriEnum.MeetingRequestWasSent, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.MeetingRequestWasSent, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation MeetingTimeZone = new PropertyInformation(PropertyUriEnum.MeetingTimeZone, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(MeetingTimeZoneProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation ModifiedOccurrences = new PropertyInformation(PropertyUriEnum.ModifiedOccurrences, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(OccurrenceInfoProperty.CreateCommandForModifiedOccurrences), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation MyResponseType = new PropertyInformation(PropertyUriEnum.MyResponseType, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.ResponseType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Organizer = new PropertyInformation(PropertyUriEnum.Organizer.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.Organizer.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, new PropertyDefinition[]
		{
			CalendarItemBaseSchema.OrganizerDisplayName,
			CalendarItemBaseSchema.OrganizerEmailAddress,
			CalendarItemBaseSchema.OrganizerType
		}, new PropertyUri(PropertyUriEnum.Organizer), new PropertyCommand.CreatePropertyCommand(OrganizerProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation OriginalStart = new PropertyInformation(PropertyUriEnum.OriginalStart, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(OriginalStartProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation StartWallClock = new PropertyInformation(PropertyUriEnum.StartWallClock, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.StartWallClock, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForStartWallClock), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EndWallClock = new PropertyInformation(PropertyUriEnum.EndWallClock, ExchangeVersion.Exchange2012, CalendarItemInstanceSchema.EndWallClock, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForEndWallClock), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation StartTimeZoneId = new PropertyInformation(PropertyUriEnum.StartTimeZoneId, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.StartTimeZoneId, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EndTimeZoneId = new PropertyInformation(PropertyUriEnum.EndTimeZoneId, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.EndTimeZoneId, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Start = new PropertyInformation(PropertyUriEnum.Start, ExchangeVersion.Exchange2007, CalendarItemInstanceSchema.StartTime, new PropertyCommand.CreatePropertyCommand(StartEndProperty.CreateCommandForStart), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation TimeZone = new PropertyInformation(PropertyUriEnum.TimeZone, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.TimeZone, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation When = new PropertyInformation(PropertyUriEnum.When, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.When, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation IntendedFreeBusyStatus = new PropertyInformation(PropertyUriEnum.IntendedFreeBusyStatus, ExchangeVersion.Exchange2012, MeetingRequestSchema.IntendedFreeBusyStatus, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation JoinOnlineMeetingUrl = new PropertyInformation(PropertyUriEnum.JoinOnlineMeetingUrl, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.OnlineMeetingExternalLink, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation OnlineMeetingSettings = new PropertyInformation(PropertyUriEnum.OnlineMeetingSettings, ExchangeVersion.Exchange2012, null, new PropertyCommand.CreatePropertyCommand(OnlineMeetingSettingsProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation IsOrganizer = new PropertyInformation(PropertyUriEnum.IsOrganizer, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.IsOrganizer, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation AppointmentReplyName = new PropertyInformation(PropertyUriEnum.AppointmentReplyName, ExchangeVersion.Exchange2012, CalendarItemBaseSchema.AppointmentReplyName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation InboxReminders = new PropertyInformation(PropertyUriEnum.InboxReminders, ExchangeVersion.Exchange2012, null, new PropertyCommand.CreatePropertyCommand(InboxRemindersProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		private static Schema schemaForOrganizer_Exchange2010AndEarlier;

		private static Schema schemaForAttendee_Exchange2010AndEarlier;

		private static Schema schemaForOrganizer_Exchange2010SP1AndLater;

		private static Schema schemaForAttendee_Exchange2010SP1AndLater;

		internal static class OrganizerSpecific
		{
			public static readonly PropertyInformation AllowNewTimeProposal = new PropertyInformation(PropertyUriEnum.AllowNewTimeProposal, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.DisallowNewTimeProposal, new PropertyCommand.CreatePropertyCommand(AllowNewTimeProposalProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation ConferenceType = new PropertyInformation(PropertyUriEnum.ConferenceType, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.ConferenceType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation IsOnlineMeeting = new PropertyInformation(PropertyUriEnum.IsOnlineMeeting, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.IsOnlineMeeting, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation IsResponseRequested = new PropertyInformation("IsResponseRequested", ExchangeVersion.Exchange2007, ItemSchema.IsResponseRequested, new PropertyUri(PropertyUriEnum.CalendarIsResponseRequested), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation MeetingWorkspaceUrl = new PropertyInformation(PropertyUriEnum.MeetingWorkspaceUrl, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.MeetingWorkspaceUrl, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation NetShowUrl = new PropertyInformation(PropertyUriEnum.NetShowUrl, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.NetShowURL, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation OptionalAttendees = new PropertyInformation(PropertyUriEnum.OptionalAttendees, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForOptionalAttendees), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation Recurrence = new PropertyInformation(PropertyUriEnum.Recurrence, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(CalendarItemRecurrenceProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation RequiredAttendees = new PropertyInformation(PropertyUriEnum.RequiredAttendees, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForRequiredAttendees), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation Resources = new PropertyInformation(PropertyUriEnum.Resources, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForResources), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation StartTimeZone = new PropertyInformation(PropertyUriEnum.StartTimeZone, ExchangeVersion.Exchange2010, TimeZoneProperty.StartTimeZonePropertyDefinition, new PropertyCommand.CreatePropertyCommand(TimeZoneProperty.CreateCommandForStart), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation EndTimeZone = new PropertyInformation(PropertyUriEnum.EndTimeZone, ExchangeVersion.Exchange2010, TimeZoneProperty.EndTimeZonePropertyDefinition, new PropertyCommand.CreatePropertyCommand(TimeZoneProperty.CreateCommandForEnd), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

			public static readonly PropertyInformation AttendeeCounts = new PropertyInformation(PropertyUriEnum.AttendeeCounts, ExchangeVersion.Exchange2013, null, new PropertyCommand.CreatePropertyCommand(AttendeeCountsProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);
		}

		internal static class AttendeeSpecific
		{
			public static readonly PropertyInformation AllowNewTimeProposal = new PropertyInformation(PropertyUriEnum.AllowNewTimeProposal, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.DisallowNewTimeProposal, new PropertyCommand.CreatePropertyCommand(AllowNewTimeProposalProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation ConferenceType = new PropertyInformation(PropertyUriEnum.ConferenceType, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.ConferenceType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation IsOnlineMeeting = new PropertyInformation(PropertyUriEnum.IsOnlineMeeting, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.IsOnlineMeeting, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation IsResponseRequested = new PropertyInformation("IsResponseRequested", ExchangeVersion.Exchange2007, ItemSchema.IsResponseRequested, new PropertyUri(PropertyUriEnum.CalendarIsResponseRequested), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation MeetingWorkspaceUrl = new PropertyInformation(PropertyUriEnum.MeetingWorkspaceUrl, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.MeetingWorkspaceUrl, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation NetShowUrl = new PropertyInformation(PropertyUriEnum.NetShowUrl, ExchangeVersion.Exchange2007, CalendarItemBaseSchema.NetShowURL, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation OptionalAttendees = new PropertyInformation(PropertyUriEnum.OptionalAttendees, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForOptionalAttendees), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation Recurrence = new PropertyInformation(PropertyUriEnum.Recurrence, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(CalendarItemRecurrenceProperty.CreateCommand), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation RequiredAttendees = new PropertyInformation(PropertyUriEnum.RequiredAttendees, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForRequiredAttendees), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation Resources = new PropertyInformation(PropertyUriEnum.Resources, ExchangeVersion.Exchange2007, null, new PropertyCommand.CreatePropertyCommand(AttendeesProperty.CreateCommandForResources), PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

			public static readonly PropertyInformation StartTimeZone = new PropertyInformation(PropertyUriEnum.StartTimeZone, ExchangeVersion.Exchange2010, TimeZoneProperty.StartTimeZonePropertyDefinition, new PropertyCommand.CreatePropertyCommand(TimeZoneProperty.CreateCommandForStart), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation EndTimeZone = new PropertyInformation(PropertyUriEnum.EndTimeZone, ExchangeVersion.Exchange2010, TimeZoneProperty.EndTimeZonePropertyDefinition, new PropertyCommand.CreatePropertyCommand(TimeZoneProperty.CreateCommandForEnd), PropertyInformationAttributes.ImplementsReadOnlyCommands);

			public static readonly PropertyInformation AttendeeCounts = new PropertyInformation(PropertyUriEnum.AttendeeCounts, ExchangeVersion.Exchange2013, null, new PropertyCommand.CreatePropertyCommand(AttendeeCountsProperty.CreateCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);
		}
	}
}
