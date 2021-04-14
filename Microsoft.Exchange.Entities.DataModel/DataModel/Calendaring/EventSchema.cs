using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class EventSchema : ItemSchema
	{
		public EventSchema()
		{
			base.RegisterPropertyDefinition(EventSchema.StaticAttendeesProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticCalendarProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticDisallowNewTimeProposalProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticEndProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticHasAttendeesProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIntendedEndTimeZoneIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIntendedStartTimeZoneIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIntendedStatusProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIsAllDayProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIsCancelledProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIsDraftProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIsOnlineMeetingProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticIsOrganizerProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticLastExecutedInteropActionProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticLocationProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticOccurrencesProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticOnlineMeetingConfLinkProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticOnlineMeetingExternalLinkProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticOrganizerProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticPatternedRecurrenceProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticResponseRequestedProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticResponseStatusProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticSeriesIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticSeriesMasterProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticSeriesMasterIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticShowAsProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticStartProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticTypeProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticPopupReminderSettingsProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticInternalGlobalObjectIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticInternalIsReceived);
			base.RegisterPropertyDefinition(EventSchema.StaticInternalMarkAllPropagatedPropertiesAsExceptionProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticInternalSeriesToInstancePropagationProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticClientIdProperty);
			base.RegisterPropertyDefinition(EventSchema.StaticInternalInstanceCreationIndexProperty);
		}

		public TypedPropertyDefinition<IList<Attendee>> AttendeesProperty
		{
			get
			{
				return EventSchema.StaticAttendeesProperty;
			}
		}

		public TypedPropertyDefinition<Calendar> CalendarProperty
		{
			get
			{
				return EventSchema.StaticCalendarProperty;
			}
		}

		public TypedPropertyDefinition<bool> DisallowNewTimeProposalProperty
		{
			get
			{
				return EventSchema.StaticDisallowNewTimeProposalProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> EndProperty
		{
			get
			{
				return EventSchema.StaticEndProperty;
			}
		}

		public TypedPropertyDefinition<IList<string>> ExceptionalPropertiesProperty
		{
			get
			{
				return EventSchema.StaticExceptionalPropertiesProperty;
			}
		}

		public TypedPropertyDefinition<bool> HasAttendeesProperty
		{
			get
			{
				return EventSchema.StaticHasAttendeesProperty;
			}
		}

		public TypedPropertyDefinition<string> IntendedEndTimeZoneIdProperty
		{
			get
			{
				return EventSchema.StaticIntendedEndTimeZoneIdProperty;
			}
		}

		public TypedPropertyDefinition<string> IntendedStartTimeZoneIdProperty
		{
			get
			{
				return EventSchema.StaticIntendedStartTimeZoneIdProperty;
			}
		}

		public TypedPropertyDefinition<FreeBusyStatus> IntendedStatusProperty
		{
			get
			{
				return EventSchema.StaticIntendedStatusProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsAllDayProperty
		{
			get
			{
				return EventSchema.StaticIsAllDayProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsCancelledProperty
		{
			get
			{
				return EventSchema.StaticIsCancelledProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsDraftProperty
		{
			get
			{
				return EventSchema.StaticIsDraftProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsOnlineMeetingProperty
		{
			get
			{
				return EventSchema.StaticIsOnlineMeetingProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsOrganizerProperty
		{
			get
			{
				return EventSchema.StaticIsOrganizerProperty;
			}
		}

		public TypedPropertyDefinition<Location> LocationProperty
		{
			get
			{
				return EventSchema.StaticLocationProperty;
			}
		}

		public TypedPropertyDefinition<IList<Event>> OccurrencesProperty
		{
			get
			{
				return EventSchema.StaticOccurrencesProperty;
			}
		}

		public TypedPropertyDefinition<string> OnlineMeetingConfLinkProperty
		{
			get
			{
				return EventSchema.StaticOnlineMeetingConfLinkProperty;
			}
		}

		public TypedPropertyDefinition<string> OnlineMeetingExternalLinkProperty
		{
			get
			{
				return EventSchema.StaticOnlineMeetingExternalLinkProperty;
			}
		}

		public TypedPropertyDefinition<Organizer> OrganizerProperty
		{
			get
			{
				return EventSchema.StaticOrganizerProperty;
			}
		}

		public TypedPropertyDefinition<PatternedRecurrence> PatternedRecurrenceProperty
		{
			get
			{
				return EventSchema.StaticPatternedRecurrenceProperty;
			}
		}

		public TypedPropertyDefinition<bool> ResponseRequestedProperty
		{
			get
			{
				return EventSchema.StaticResponseRequestedProperty;
			}
		}

		public TypedPropertyDefinition<ResponseStatus> ResponseStatusProperty
		{
			get
			{
				return EventSchema.StaticResponseStatusProperty;
			}
		}

		public TypedPropertyDefinition<string> SeriesIdProperty
		{
			get
			{
				return EventSchema.StaticSeriesIdProperty;
			}
		}

		public TypedPropertyDefinition<Event> SeriesMasterProperty
		{
			get
			{
				return EventSchema.StaticSeriesMasterProperty;
			}
		}

		public TypedPropertyDefinition<string> SeriesMasterIdProperty
		{
			get
			{
				return EventSchema.StaticSeriesMasterIdProperty;
			}
		}

		public TypedPropertyDefinition<FreeBusyStatus> ShowAsProperty
		{
			get
			{
				return EventSchema.StaticShowAsProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> StartProperty
		{
			get
			{
				return EventSchema.StaticStartProperty;
			}
		}

		public TypedPropertyDefinition<EventType> TypeProperty
		{
			get
			{
				return EventSchema.StaticTypeProperty;
			}
		}

		public TypedPropertyDefinition<IList<EventPopupReminderSetting>> PopupReminderSettingsProperty
		{
			get
			{
				return EventSchema.StaticPopupReminderSettingsProperty;
			}
		}

		internal TypedPropertyDefinition<Guid?> LastExecutedInteropActionProperty
		{
			get
			{
				return EventSchema.StaticLastExecutedInteropActionProperty;
			}
		}

		internal TypedPropertyDefinition<string> InternalGlobalObjectIdProperty
		{
			get
			{
				return EventSchema.StaticInternalGlobalObjectIdProperty;
			}
		}

		internal TypedPropertyDefinition<bool> InternalMarkAllPropagatedPropertiesAsExceptionProperty
		{
			get
			{
				return EventSchema.StaticInternalMarkAllPropagatedPropertiesAsExceptionProperty;
			}
		}

		internal TypedPropertyDefinition<bool> InternalSeriesToInstancePropagationProperty
		{
			get
			{
				return EventSchema.StaticInternalSeriesToInstancePropagationProperty;
			}
		}

		internal TypedPropertyDefinition<bool> InternalIsReceived
		{
			get
			{
				return EventSchema.StaticInternalIsReceived;
			}
		}

		internal TypedPropertyDefinition<string> ClientIdProperty
		{
			get
			{
				return EventSchema.StaticClientIdProperty;
			}
		}

		internal TypedPropertyDefinition<int> InternalSeriesCreationHashProperty
		{
			get
			{
				return EventSchema.StaticInternalSeriesCreationHashProperty;
			}
		}

		internal TypedPropertyDefinition<int> InternalSeriesSequenceNumberProperty
		{
			get
			{
				return EventSchema.StaticInternalSeriesSequenceNumberProperty;
			}
		}

		internal TypedPropertyDefinition<int> InternalInstanceCreationIndexProperty
		{
			get
			{
				return EventSchema.StaticInternalInstanceCreationIndexProperty;
			}
		}

		private static readonly TypedPropertyDefinition<IList<Attendee>> StaticAttendeesProperty = new TypedPropertyDefinition<IList<Attendee>>("Event.Attendees", null, true);

		private static readonly TypedPropertyDefinition<Calendar> StaticCalendarProperty = new TypedPropertyDefinition<Calendar>("Event.Calendar", null, true);

		private static readonly TypedPropertyDefinition<string> StaticClientIdProperty = new TypedPropertyDefinition<string>("Event.ClientId", null, true);

		private static readonly TypedPropertyDefinition<bool> StaticDisallowNewTimeProposalProperty = new TypedPropertyDefinition<bool>("Event.DisallowNewTimeProposal", false, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticEndProperty = new TypedPropertyDefinition<ExDateTime>("Event.End", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<IList<string>> StaticExceptionalPropertiesProperty = new TypedPropertyDefinition<IList<string>>("Event.ExceptionalProperties", null, true);

		private static readonly TypedPropertyDefinition<bool> StaticHasAttendeesProperty = new TypedPropertyDefinition<bool>("Event.HasAttendee", false, true);

		private static readonly TypedPropertyDefinition<string> StaticIntendedEndTimeZoneIdProperty = new TypedPropertyDefinition<string>("Event.IntendedEndTimeZoneId", null, true);

		private static readonly TypedPropertyDefinition<string> StaticIntendedStartTimeZoneIdProperty = new TypedPropertyDefinition<string>("Event.IntendedStartTimeZoneId", null, true);

		private static readonly TypedPropertyDefinition<FreeBusyStatus> StaticIntendedStatusProperty = new TypedPropertyDefinition<FreeBusyStatus>("Event.IntendedStatus", FreeBusyStatus.Free, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsAllDayProperty = new TypedPropertyDefinition<bool>("Event.IsAllDay", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsCancelledProperty = new TypedPropertyDefinition<bool>("Event.IsCancelled", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsDraftProperty = new TypedPropertyDefinition<bool>("Event.IsDraft", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsOnlineMeetingProperty = new TypedPropertyDefinition<bool>("Event.IsOnlineMeeting", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsOrganizerProperty = new TypedPropertyDefinition<bool>("Event.IsOrganizer", false, true);

		private static readonly TypedPropertyDefinition<Guid?> StaticLastExecutedInteropActionProperty = new TypedPropertyDefinition<Guid?>("Event.LastExecutedInteropAction", null, true);

		private static readonly TypedPropertyDefinition<Location> StaticLocationProperty = new TypedPropertyDefinition<Location>("Event.Location", null, true);

		private static readonly TypedPropertyDefinition<IList<Event>> StaticOccurrencesProperty = new TypedPropertyDefinition<IList<Event>>("Event.Occurrences", null, true);

		private static readonly TypedPropertyDefinition<Organizer> StaticOrganizerProperty = new TypedPropertyDefinition<Organizer>("Event.Organizer", null, true);

		private static readonly TypedPropertyDefinition<string> StaticOnlineMeetingConfLinkProperty = new TypedPropertyDefinition<string>("Event.OnlineMeetingConfLink", null, true);

		private static readonly TypedPropertyDefinition<string> StaticOnlineMeetingExternalLinkProperty = new TypedPropertyDefinition<string>("Event.OnlineMeetingExternalLink", null, true);

		private static readonly TypedPropertyDefinition<PatternedRecurrence> StaticPatternedRecurrenceProperty = new TypedPropertyDefinition<PatternedRecurrence>("Event.PatternedRecurrence", null, true);

		private static readonly TypedPropertyDefinition<bool> StaticResponseRequestedProperty = new TypedPropertyDefinition<bool>("Event.ResponseRequested", false, true);

		private static readonly TypedPropertyDefinition<ResponseStatus> StaticResponseStatusProperty = new TypedPropertyDefinition<ResponseStatus>("Event.ResponseStatus", null, true);

		private static readonly TypedPropertyDefinition<string> StaticSeriesIdProperty = new TypedPropertyDefinition<string>("Event.SeriesId", null, true);

		private static readonly TypedPropertyDefinition<Event> StaticSeriesMasterProperty = new TypedPropertyDefinition<Event>("Event.SeriesMaster", null, true);

		private static readonly TypedPropertyDefinition<string> StaticSeriesMasterIdProperty = new TypedPropertyDefinition<string>("Event.SeriesMasterId", null, true);

		private static readonly TypedPropertyDefinition<FreeBusyStatus> StaticShowAsProperty = new TypedPropertyDefinition<FreeBusyStatus>("Event.ShowAs", FreeBusyStatus.Free, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticStartProperty = new TypedPropertyDefinition<ExDateTime>("Event.Start", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<EventType> StaticTypeProperty = new TypedPropertyDefinition<EventType>("Event.Type", EventType.SingleInstance, true);

		private static readonly TypedPropertyDefinition<IList<EventPopupReminderSetting>> StaticPopupReminderSettingsProperty = new TypedPropertyDefinition<IList<EventPopupReminderSetting>>("Event.PopupReminderSettings", null, true);

		private static readonly TypedPropertyDefinition<string> StaticInternalGlobalObjectIdProperty = new TypedPropertyDefinition<string>("Event.InternalGlobalObjectId", null, true);

		private static readonly TypedPropertyDefinition<bool> StaticInternalMarkAllPropagatedPropertiesAsExceptionProperty = new TypedPropertyDefinition<bool>("Event.InternalMarkAllPropagatedPropertiesAsException", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticInternalSeriesToInstancePropagationProperty = new TypedPropertyDefinition<bool>("Event.InternalSeriesToInstancePropagation", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticInternalIsReceived = new TypedPropertyDefinition<bool>("InternalEvent.IsReceived", false, true);

		private static readonly TypedPropertyDefinition<int> StaticInternalInstanceCreationIndexProperty = new TypedPropertyDefinition<int>("InternalEvent.InstanceCreationIndex", 0, true);

		private static readonly TypedPropertyDefinition<int> StaticInternalSeriesCreationHashProperty = new TypedPropertyDefinition<int>("InternalEvent.SeriesCreationHash", 0, true);

		private static readonly TypedPropertyDefinition<int> StaticInternalSeriesSequenceNumberProperty = new TypedPropertyDefinition<int>("InternalEvent.SeriesSequenceNumber", 0, true);
	}
}
