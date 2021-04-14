using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class Event : Item<EventSchema>, IEvent, IItem, IStorageEntity, IEntity, IVersioned, IActionQueue, IActionPropagationState, IEventInternal, IPropertyChangeTracker<PropertyDefinition>
	{
		Guid? IActionPropagationState.LastExecutedAction
		{
			get
			{
				return base.GetPropertyValueOrDefault<Guid?>(base.Schema.LastExecutedInteropActionProperty);
			}
			set
			{
				base.SetPropertyValue<Guid?>(base.Schema.LastExecutedInteropActionProperty, value);
			}
		}

		ActionInfo[] IActionQueue.ActionsToAdd { get; set; }

		Guid[] IActionQueue.ActionsToRemove { get; set; }

		bool IActionQueue.HasData { get; set; }

		ActionInfo[] IActionQueue.OriginalActions { get; set; }

		public IList<Attendee> Attendees
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<Attendee>>(base.Schema.AttendeesProperty);
			}
			set
			{
				base.SetPropertyValue<IList<Attendee>>(base.Schema.AttendeesProperty, value);
			}
		}

		public Calendar Calendar
		{
			get
			{
				return base.GetPropertyValueOrDefault<Calendar>(base.Schema.CalendarProperty);
			}
			set
			{
				base.SetPropertyValue<Calendar>(base.Schema.CalendarProperty, value);
			}
		}

		public string ClientId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ClientIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ClientIdProperty, value);
			}
		}

		public bool DisallowNewTimeProposal
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.DisallowNewTimeProposalProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.DisallowNewTimeProposalProperty, value);
			}
		}

		public ExDateTime End
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.EndProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.EndProperty, value);
			}
		}

		public IList<string> ExceptionalProperties
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<string>>(base.Schema.ExceptionalPropertiesProperty);
			}
			set
			{
				base.SetPropertyValue<IList<string>>(base.Schema.ExceptionalPropertiesProperty, value);
			}
		}

		public bool HasAttendees
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.HasAttendeesProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.HasAttendeesProperty, value);
			}
		}

		public string IntendedEndTimeZoneId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.IntendedEndTimeZoneIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.IntendedEndTimeZoneIdProperty, value);
			}
		}

		public string IntendedStartTimeZoneId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.IntendedStartTimeZoneIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.IntendedStartTimeZoneIdProperty, value);
			}
		}

		public FreeBusyStatus IntendedStatus
		{
			get
			{
				return base.GetPropertyValueOrDefault<FreeBusyStatus>(base.Schema.IntendedStatusProperty);
			}
			set
			{
				base.SetPropertyValue<FreeBusyStatus>(base.Schema.IntendedStatusProperty, value);
			}
		}

		public bool IsAllDay
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsAllDayProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsAllDayProperty, value);
			}
		}

		public bool IsCancelled
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsCancelledProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsCancelledProperty, value);
			}
		}

		public bool IsDraft
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsDraftProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsDraftProperty, value);
			}
		}

		public bool IsOnlineMeeting
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsOnlineMeetingProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsOnlineMeetingProperty, value);
			}
		}

		public bool IsOrganizer
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.IsOrganizerProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.IsOrganizerProperty, value);
			}
		}

		public Location Location
		{
			get
			{
				return base.GetPropertyValueOrDefault<Location>(base.Schema.LocationProperty);
			}
			set
			{
				base.SetPropertyValue<Location>(base.Schema.LocationProperty, value);
			}
		}

		public IList<Event> Occurrences
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<Event>>(base.Schema.OccurrencesProperty);
			}
			set
			{
				base.SetPropertyValue<IList<Event>>(base.Schema.OccurrencesProperty, value);
			}
		}

		public string OnlineMeetingConfLink
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.OnlineMeetingConfLinkProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.OnlineMeetingConfLinkProperty, value);
			}
		}

		public string OnlineMeetingExternalLink
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.OnlineMeetingExternalLinkProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.OnlineMeetingExternalLinkProperty, value);
			}
		}

		public Organizer Organizer
		{
			get
			{
				return base.GetPropertyValueOrDefault<Organizer>(base.Schema.OrganizerProperty);
			}
			set
			{
				base.SetPropertyValue<Organizer>(base.Schema.OrganizerProperty, value);
			}
		}

		public PatternedRecurrence PatternedRecurrence
		{
			get
			{
				return base.GetPropertyValueOrDefault<PatternedRecurrence>(base.Schema.PatternedRecurrenceProperty);
			}
			set
			{
				base.SetPropertyValue<PatternedRecurrence>(base.Schema.PatternedRecurrenceProperty, value);
			}
		}

		public bool ResponseRequested
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ResponseRequestedProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ResponseRequestedProperty, value);
			}
		}

		public ResponseStatus ResponseStatus
		{
			get
			{
				return base.GetPropertyValueOrDefault<ResponseStatus>(base.Schema.ResponseStatusProperty);
			}
			set
			{
				base.SetPropertyValue<ResponseStatus>(base.Schema.ResponseStatusProperty, value);
			}
		}

		public string SeriesId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.SeriesIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.SeriesIdProperty, value);
			}
		}

		public Event SeriesMaster
		{
			get
			{
				return base.GetPropertyValueOrDefault<Event>(base.Schema.SeriesMasterProperty);
			}
			set
			{
				base.SetPropertyValue<Event>(base.Schema.SeriesMasterProperty, value);
			}
		}

		public string SeriesMasterId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.SeriesMasterIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.SeriesMasterIdProperty, value);
			}
		}

		public FreeBusyStatus ShowAs
		{
			get
			{
				return base.GetPropertyValueOrDefault<FreeBusyStatus>(base.Schema.ShowAsProperty);
			}
			set
			{
				base.SetPropertyValue<FreeBusyStatus>(base.Schema.ShowAsProperty, value);
			}
		}

		public ExDateTime Start
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.StartProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.StartProperty, value);
			}
		}

		public EventType Type
		{
			get
			{
				return base.GetPropertyValueOrDefault<EventType>(base.Schema.TypeProperty);
			}
			set
			{
				base.SetPropertyValue<EventType>(base.Schema.TypeProperty, value);
			}
		}

		public IList<EventPopupReminderSetting> PopupReminderSettings
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<EventPopupReminderSetting>>(base.Schema.PopupReminderSettingsProperty);
			}
			set
			{
				base.SetPropertyValue<IList<EventPopupReminderSetting>>(base.Schema.PopupReminderSettingsProperty, value);
			}
		}

		string IEventInternal.GlobalObjectId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.InternalGlobalObjectIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.InternalGlobalObjectIdProperty, value);
			}
		}

		bool IEventInternal.MarkAllPropagatedPropertiesAsException
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.InternalMarkAllPropagatedPropertiesAsExceptionProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.InternalMarkAllPropagatedPropertiesAsExceptionProperty, value);
			}
		}

		bool IEventInternal.IsReceived
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.InternalIsReceived);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.InternalIsReceived, value);
			}
		}

		int IEventInternal.InstanceCreationIndex
		{
			get
			{
				return base.GetPropertyValueOrDefault<int>(base.Schema.InternalInstanceCreationIndexProperty);
			}
			set
			{
				base.SetPropertyValue<int>(base.Schema.InternalInstanceCreationIndexProperty, value);
			}
		}

		int IEventInternal.SeriesCreationHash
		{
			get
			{
				return base.GetPropertyValueOrDefault<int>(base.Schema.InternalSeriesCreationHashProperty);
			}
			set
			{
				base.SetPropertyValue<int>(base.Schema.InternalSeriesCreationHashProperty, value);
			}
		}

		int IEventInternal.SeriesSequenceNumber
		{
			get
			{
				return base.GetPropertyValueOrDefault<int>(base.Schema.InternalSeriesSequenceNumberProperty);
			}
			set
			{
				base.SetPropertyValue<int>(base.Schema.InternalSeriesSequenceNumberProperty, value);
			}
		}

		bool IEventInternal.SeriesToInstancePropagation
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.InternalSeriesToInstancePropagationProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.InternalSeriesToInstancePropagationProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<IEvent, IList<Attendee>> Attendees = new EntityPropertyAccessor<IEvent, IList<Attendee>>(SchematizedObject<EventSchema>.SchemaInstance.AttendeesProperty, (IEvent theEvent) => theEvent.Attendees, delegate(IEvent theEvent, IList<Attendee> attendees)
			{
				theEvent.Attendees = attendees;
			});

			public static readonly EntityPropertyAccessor<IEvent, Calendar> Calendar = new EntityPropertyAccessor<IEvent, Calendar>(SchematizedObject<EventSchema>.SchemaInstance.CalendarProperty, (IEvent theEvent) => theEvent.Calendar, delegate(IEvent theEvent, Calendar calendar)
			{
				theEvent.Calendar = calendar;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> ClientId = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.ClientIdProperty, (IEvent theEvent) => theEvent.ClientId, delegate(IEvent theEvent, string clientId)
			{
				theEvent.ClientId = clientId;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> DisallowNewTimeProposal = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.DisallowNewTimeProposalProperty, (IEvent theEvent) => theEvent.DisallowNewTimeProposal, delegate(IEvent theEvent, bool disallow)
			{
				theEvent.DisallowNewTimeProposal = disallow;
			});

			public static readonly EntityPropertyAccessor<IEvent, ExDateTime> End = new EntityPropertyAccessor<IEvent, ExDateTime>(SchematizedObject<EventSchema>.SchemaInstance.EndProperty, (IEvent theEvent) => theEvent.End, delegate(IEvent theEvent, ExDateTime end)
			{
				theEvent.End = end;
			});

			public static readonly EntityPropertyAccessor<IEvent, IList<string>> ExceptionalProperties = new EntityPropertyAccessor<IEvent, IList<string>>(SchematizedObject<EventSchema>.SchemaInstance.ExceptionalPropertiesProperty, (IEvent theEvent) => theEvent.ExceptionalProperties, delegate(IEvent theEvent, IList<string> properties)
			{
				theEvent.ExceptionalProperties = properties;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> HasAttendees = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.HasAttendeesProperty, (IEvent theEvent) => theEvent.HasAttendees, delegate(IEvent theEvent, bool hasAttendees)
			{
				theEvent.HasAttendees = hasAttendees;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> IntendedEndTimeZoneId = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.IntendedEndTimeZoneIdProperty, (IEvent theEvent) => theEvent.IntendedEndTimeZoneId, delegate(IEvent theEvent, string timeZoneId)
			{
				theEvent.IntendedEndTimeZoneId = timeZoneId;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> IntendedStartTimeZoneId = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.IntendedStartTimeZoneIdProperty, (IEvent theEvent) => theEvent.IntendedStartTimeZoneId, delegate(IEvent theEvent, string timeZoneId)
			{
				theEvent.IntendedStartTimeZoneId = timeZoneId;
			});

			public static readonly EntityPropertyAccessor<IEvent, FreeBusyStatus> IntendedStatus = new EntityPropertyAccessor<IEvent, FreeBusyStatus>(SchematizedObject<EventSchema>.SchemaInstance.IntendedStatusProperty, (IEvent theEvent) => theEvent.IntendedStatus, delegate(IEvent theEvent, FreeBusyStatus status)
			{
				theEvent.IntendedStatus = status;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> IsAllDay = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.IsAllDayProperty, (IEvent theEvent) => theEvent.IsAllDay, delegate(IEvent theEvent, bool allDay)
			{
				theEvent.IsAllDay = allDay;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> IsCancelled = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.IsCancelledProperty, (IEvent theEvent) => theEvent.IsCancelled, delegate(IEvent theEvent, bool cancelled)
			{
				theEvent.IsCancelled = cancelled;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> IsDraft = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.IsDraftProperty, (IEvent theEvent) => theEvent.IsDraft, delegate(IEvent theEvent, bool isDraft)
			{
				theEvent.IsDraft = isDraft;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> IsOnlineMeeting = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.IsOnlineMeetingProperty, (IEvent theEvent) => theEvent.IsOnlineMeeting, delegate(IEvent theEvent, bool onlineMeeting)
			{
				theEvent.IsOnlineMeeting = onlineMeeting;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> IsOrganizer = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.IsOrganizerProperty, (IEvent theEvent) => theEvent.IsOrganizer, delegate(IEvent theEvent, bool organizer)
			{
				theEvent.IsOrganizer = organizer;
			});

			public static readonly EntityPropertyAccessor<IEvent, Location> Location = new EntityPropertyAccessor<IEvent, Location>(SchematizedObject<EventSchema>.SchemaInstance.LocationProperty, (IEvent theEvent) => theEvent.Location, delegate(IEvent theEvent, Location location)
			{
				theEvent.Location = location;
			});

			public static readonly EntityPropertyAccessor<IEvent, IList<Event>> Occurrences = new EntityPropertyAccessor<IEvent, IList<Event>>(SchematizedObject<EventSchema>.SchemaInstance.OccurrencesProperty, (IEvent theEvent) => theEvent.Occurrences, delegate(IEvent theEvent, IList<Event> occurrences)
			{
				theEvent.Occurrences = occurrences;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> OnlineMeetingConfLink = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.OnlineMeetingConfLinkProperty, (IEvent theEvent) => theEvent.OnlineMeetingConfLink, delegate(IEvent theEvent, string link)
			{
				theEvent.OnlineMeetingConfLink = link;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> OnlineMeetingExternalLink = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.OnlineMeetingExternalLinkProperty, (IEvent theEvent) => theEvent.OnlineMeetingExternalLink, delegate(IEvent theEvent, string link)
			{
				theEvent.OnlineMeetingExternalLink = link;
			});

			public static readonly EntityPropertyAccessor<IEvent, Organizer> Organizer = new EntityPropertyAccessor<IEvent, Organizer>(SchematizedObject<EventSchema>.SchemaInstance.OrganizerProperty, (IEvent theEvent) => theEvent.Organizer, delegate(IEvent theEvent, Organizer organizer)
			{
				theEvent.Organizer = organizer;
			});

			public static readonly EntityPropertyAccessor<IEvent, PatternedRecurrence> PatternedRecurrence = new EntityPropertyAccessor<IEvent, PatternedRecurrence>(SchematizedObject<EventSchema>.SchemaInstance.PatternedRecurrenceProperty, (IEvent theEvent) => theEvent.PatternedRecurrence, delegate(IEvent theEvent, PatternedRecurrence recurrence)
			{
				theEvent.PatternedRecurrence = recurrence;
			});

			public static readonly EntityPropertyAccessor<IEvent, bool> ResponseRequested = new EntityPropertyAccessor<IEvent, bool>(SchematizedObject<EventSchema>.SchemaInstance.ResponseRequestedProperty, (IEvent theEvent) => theEvent.ResponseRequested, delegate(IEvent theEvent, bool responseRequested)
			{
				theEvent.ResponseRequested = responseRequested;
			});

			public static readonly EntityPropertyAccessor<IEvent, ResponseStatus> ResponseStatus = new EntityPropertyAccessor<IEvent, ResponseStatus>(SchematizedObject<EventSchema>.SchemaInstance.ResponseStatusProperty, (IEvent theEvent) => theEvent.ResponseStatus, delegate(IEvent theEvent, ResponseStatus responseStatus)
			{
				theEvent.ResponseStatus = responseStatus;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> SeriesId = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.SeriesIdProperty, (IEvent theEvent) => theEvent.SeriesId, delegate(IEvent theEvent, string seriesId)
			{
				theEvent.SeriesId = seriesId;
			});

			public static readonly EntityPropertyAccessor<IEvent, string> SeriesMasterId = new EntityPropertyAccessor<IEvent, string>(SchematizedObject<EventSchema>.SchemaInstance.SeriesMasterIdProperty, (IEvent theEvent) => theEvent.SeriesMasterId, delegate(IEvent theEvent, string seriesMasterId)
			{
				theEvent.SeriesMasterId = seriesMasterId;
			});

			public static readonly EntityPropertyAccessor<IEvent, Event> SeriesMaster = new EntityPropertyAccessor<IEvent, Event>(SchematizedObject<EventSchema>.SchemaInstance.SeriesMasterProperty, (IEvent theEvent) => theEvent.SeriesMaster, delegate(IEvent theEvent, Event master)
			{
				theEvent.SeriesMaster = master;
			});

			public static readonly EntityPropertyAccessor<IEvent, FreeBusyStatus> ShowAs = new EntityPropertyAccessor<IEvent, FreeBusyStatus>(SchematizedObject<EventSchema>.SchemaInstance.ShowAsProperty, (IEvent theEvent) => theEvent.ShowAs, delegate(IEvent theEvent, FreeBusyStatus status)
			{
				theEvent.ShowAs = status;
			});

			public static readonly EntityPropertyAccessor<IEvent, ExDateTime> Start = new EntityPropertyAccessor<IEvent, ExDateTime>(SchematizedObject<EventSchema>.SchemaInstance.StartProperty, (IEvent theEvent) => theEvent.Start, delegate(IEvent theEvent, ExDateTime start)
			{
				theEvent.Start = start;
			});

			public static readonly EntityPropertyAccessor<IEvent, EventType> Type = new EntityPropertyAccessor<IEvent, EventType>(SchematizedObject<EventSchema>.SchemaInstance.TypeProperty, (IEvent theEvent) => theEvent.Type, delegate(IEvent theEvent, EventType type)
			{
				theEvent.Type = type;
			});

			internal static readonly EntityPropertyAccessor<IActionPropagationState, Guid?> LastExecutedInteropAction = new EntityPropertyAccessor<IActionPropagationState, Guid?>(SchematizedObject<EventSchema>.SchemaInstance.LastExecutedInteropActionProperty, (IActionPropagationState actionPropagationState) => actionPropagationState.LastExecutedAction, delegate(IActionPropagationState actionPropagationState, Guid? action)
			{
				actionPropagationState.LastExecutedAction = action;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, string> InternalGlobalObjectId = new EntityPropertyAccessor<IEventInternal, string>(SchematizedObject<EventSchema>.SchemaInstance.InternalGlobalObjectIdProperty, (IEventInternal theEvent) => theEvent.GlobalObjectId, delegate(IEventInternal theEvent, string globalObjectId)
			{
				theEvent.GlobalObjectId = globalObjectId;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, bool> InternalIsReceived = new EntityPropertyAccessor<IEventInternal, bool>(SchematizedObject<EventSchema>.SchemaInstance.InternalIsReceived, (IEventInternal theEvent) => theEvent.IsReceived, delegate(IEventInternal theEvent, bool isReceived)
			{
				theEvent.IsReceived = isReceived;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, bool> InternalMarkAllPropagatedPropertiesAsException = new EntityPropertyAccessor<IEventInternal, bool>(SchematizedObject<EventSchema>.SchemaInstance.InternalMarkAllPropagatedPropertiesAsExceptionProperty, (IEventInternal theEvent) => theEvent.MarkAllPropagatedPropertiesAsException, delegate(IEventInternal theEvent, bool mark)
			{
				theEvent.MarkAllPropagatedPropertiesAsException = mark;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, bool> InternalSeriesToInstancePropagation = new EntityPropertyAccessor<IEventInternal, bool>(SchematizedObject<EventSchema>.SchemaInstance.InternalSeriesToInstancePropagationProperty, (IEventInternal theEvent) => theEvent.SeriesToInstancePropagation, delegate(IEventInternal theEvent, bool seriesToInstancePropagation)
			{
				theEvent.SeriesToInstancePropagation = seriesToInstancePropagation;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, int> InternalInstanceCreationIndex = new EntityPropertyAccessor<IEventInternal, int>(SchematizedObject<EventSchema>.SchemaInstance.InternalInstanceCreationIndexProperty, (IEventInternal theEvent) => theEvent.InstanceCreationIndex, delegate(IEventInternal theEvent, int index)
			{
				theEvent.InstanceCreationIndex = index;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, int> InternalSeriesSequenceNumber = new EntityPropertyAccessor<IEventInternal, int>(SchematizedObject<EventSchema>.SchemaInstance.InternalSeriesSequenceNumberProperty, (IEventInternal theEvent) => theEvent.SeriesSequenceNumber, delegate(IEventInternal theEvent, int seriesSequenceNumber)
			{
				theEvent.SeriesSequenceNumber = seriesSequenceNumber;
			});

			internal static readonly EntityPropertyAccessor<IEventInternal, int> InternalSeriesCreationHash = new EntityPropertyAccessor<IEventInternal, int>(SchematizedObject<EventSchema>.SchemaInstance.InternalSeriesCreationHashProperty, (IEventInternal theEvent) => theEvent.SeriesCreationHash, delegate(IEventInternal theEvent, int hash)
			{
				theEvent.SeriesCreationHash = hash;
			});
		}
	}
}
