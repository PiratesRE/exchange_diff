using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators
{
	internal class EventTranslator : ItemTranslator<ICalendarItemBase, Event, EventSchema>
	{
		protected EventTranslator(IEnumerable<ITranslationRule<ICalendarItemBase, Event>> additionalRules = null) : base(EventTranslator.CreateTranslationRules().AddRules(additionalRules))
		{
			this.propertyChangeMetadataConverter = new PropertyChangeMetadataConverter(base.StoragePropertyGroupsToEdmProperties);
			ITranslationRule<ICalendarItemBase, IEvent> internalRule = CalendarItemAccessors.PropertyChangeMetadata.MapTo(Event.Accessors.ExceptionalProperties, this.propertyChangeMetadataConverter);
			base.Strategy.Add(internalRule.OfType<ICalendarItemBase, IEvent, CalendarItemInstance, Event>());
		}

		public new static EventTranslator Instance
		{
			get
			{
				return EventTranslator.SingleTonInstance;
			}
		}

		public override Event ConvertToEntity(ICalendarItemBase master, ICalendarItemBase instance)
		{
			Event @event = EventTranslator.Instance.ConvertToEntity(master);
			Event event2 = EventTranslator.Instance.ConvertToEntity(instance);
			@event.SeriesMasterId = @event.Id;
			@event.ChangeKey = event2.ChangeKey;
			@event.End = event2.End;
			@event.Id = event2.Id;
			@event.StoreId = event2.StoreId;
			@event.PopupReminderSettings = event2.PopupReminderSettings;
			@event.Start = event2.Start;
			@event.Type = event2.Type;
			IEventInternal eventInternal = @event;
			IEventInternal eventInternal2 = event2;
			eventInternal.GlobalObjectId = eventInternal2.GlobalObjectId;
			eventInternal.InstanceCreationIndex = eventInternal2.InstanceCreationIndex;
			IActionPropagationState actionPropagationState = @event;
			IActionPropagationState actionPropagationState2 = event2;
			actionPropagationState.LastExecutedAction = actionPropagationState2.LastExecutedAction;
			PropertyChangeMetadata propertyChangeMetadata;
			if (CalendarItemAccessors.PropertyChangeMetadata.TryGetValue(instance, out propertyChangeMetadata))
			{
				IEnumerable<PropertyChangeMetadata.PropertyGroup> overriddenGroups = propertyChangeMetadata.GetOverriddenGroups();
				foreach (ITranslationRule<ICalendarItemBase, Event> translationRule2 in from translationRule in base.Strategy.EnumerateRules<ICalendarItemBase, Event>()
				let storageRule = translationRule as IStorageTranslationRule<ICalendarItemBase, Event>
				where storageRule != null && overriddenGroups.Contains(storageRule.StoragePropertyGroup)
				select translationRule)
				{
					translationRule2.FromLeftToRightType(instance, @event);
				}
			}
			return @event;
		}

		public override void SetPropertiesFromStoragePropertyValuesOnEntity(IDictionary<PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, Event destinationEntity)
		{
			base.SetPropertiesFromStoragePropertyValuesOnEntity(propertyIndices, values, session, destinationEntity);
			EventTranslator.SeriesMasterIdRule.FromPropertyValues(propertyIndices, values, session, destinationEntity);
		}

		internal IList<string> GetOverridenProperties(PropertyChangeMetadata metadata)
		{
			return this.propertyChangeMetadataConverter.Convert(metadata);
		}

		private static List<ITranslationRule<ICalendarItemBase, Event>> CreateTranslationRules()
		{
			ITranslationRule<ICalendarItemBase, IActionPropagationState> lastExecutedInteropActionRule = CalendarItemAccessors.LastExecutedInteropAction.MapTo(Event.Accessors.LastExecutedInteropAction);
			TranslationStrategy<ICalendarItem, PropertyDefinition, Event> internalRule = new TranslationStrategy<ICalendarItem, PropertyDefinition, Event>(new ITranslationRule<ICalendarItem, Event>[]
			{
				new PatternedRecurrenceRule(),
				CalendarItemAccessors.SeriesMasterDataPropagationOperation.MapTo(Event.Accessors.InternalSeriesToInstancePropagation),
				CalendarItemAccessors.MarkAllPropagatedPropertiesAsException.MapTo(Event.Accessors.InternalMarkAllPropagatedPropertiesAsException)
			});
			TranslationStrategy<ICalendarItemSeries, PropertyDefinition, Event> internalRule2 = new TranslationStrategy<ICalendarItemSeries, PropertyDefinition, Event>(new ITranslationRule<ICalendarItemSeries, Event>[]
			{
				CalendarItemAccessors.SeriesCreationHash.MapTo(Event.Accessors.InternalSeriesCreationHash),
				CalendarItemAccessors.SeriesSequenceNumber.MapTo(Event.Accessors.InternalSeriesSequenceNumber),
				new ActionQueueRules()
			});
			DateTimeHelper dateTimeHelper = new DateTimeHelper();
			FreeBusyConverter converter = default(FreeBusyConverter);
			return new List<ITranslationRule<ICalendarItemBase, Event>>
			{
				new DraftStateRules(),
				CalendarItemAccessors.Attendees.MapTo(Event.Accessors.Attendees),
				CalendarItemAccessors.DisallowNewTimeProposal.MapTo(Event.Accessors.DisallowNewTimeProposal),
				CalendarItemAccessors.EndTime.MapTo(CalendarItemAccessors.EndTimeZone, Event.Accessors.End, Event.Accessors.IntendedEndTimeZoneId, dateTimeHelper),
				CalendarItemAccessors.GlobalObjectId.MapTo(Event.Accessors.InternalGlobalObjectId, default(GlobalObjectIdConverter)),
				CalendarItemAccessors.HasAttendees.MapTo(Event.Accessors.HasAttendees),
				CalendarItemAccessors.IntendedFreeBusyStatus.MapTo(Event.Accessors.IntendedStatus, converter),
				CalendarItemAccessors.IsAllDay.MapTo(Event.Accessors.IsAllDay),
				CalendarItemAccessors.IsCancelled.MapTo(Event.Accessors.IsCancelled),
				CalendarItemAccessors.IsOnlineMeeting.MapTo(Event.Accessors.IsOnlineMeeting),
				CalendarItemAccessors.IsOrganizer.MapTo(Event.Accessors.IsOrganizer),
				CalendarItemAccessors.Location.MapTo(Event.Accessors.Location),
				CalendarItemAccessors.OnlineMeetingConfLink.MapTo(Event.Accessors.OnlineMeetingConfLink),
				CalendarItemAccessors.OnlineMeetingExternalLink.MapTo(Event.Accessors.OnlineMeetingExternalLink),
				CalendarItemAccessors.Organizer.MapTo(Event.Accessors.Organizer),
				CalendarItemAccessors.ResponseRequested.MapTo(Event.Accessors.ResponseRequested),
				new ResponseStatusRule(),
				EventTranslator.SeriesIdRule,
				CalendarItemAccessors.FreeBusyStatus.MapTo(Event.Accessors.ShowAs, converter),
				CalendarItemAccessors.StartTime.MapTo(CalendarItemAccessors.StartTimeZone, Event.Accessors.Start, Event.Accessors.IntendedStartTimeZoneId, dateTimeHelper),
				EventTranslator.EventTypeTranslationRule,
				CalendarItemAccessors.IsReceivedAccessor.MapTo(Event.Accessors.InternalIsReceived),
				new DelegatedTranslationRule<ICalendarItemBase, Event>(delegate(ICalendarItemBase calendarItemBase, Event theEvent)
				{
					if (!(calendarItemBase is ICalendarItemOccurrence))
					{
						lastExecutedInteropActionRule.FromLeftToRightType(calendarItemBase, theEvent);
					}
				}, new Action<ICalendarItemBase, Event>(lastExecutedInteropActionRule.FromRightToLeftType)),
				internalRule.OfType<ICalendarItemBase, Event, ICalendarItem, Event>(),
				internalRule2.OfType<ICalendarItemBase, Event, ICalendarItemSeries, Event>(),
				new EventPopupReminderSettingsRules(),
				EventTranslator.SeriesMasterIdRule
			};
		}

		public static readonly SeriesMasterIdTranslationRule SeriesMasterIdRule = new SeriesMasterIdTranslationRule(null);

		public static readonly SeriesIdTranslationRule SeriesIdRule = new SeriesIdTranslationRule();

		public static readonly EventTypeTranslationRule EventTypeTranslationRule = new EventTypeTranslationRule();

		private static readonly EventTranslator SingleTonInstance = new EventTranslator(null);

		private readonly PropertyChangeMetadataConverter propertyChangeMetadataConverter;
	}
}
