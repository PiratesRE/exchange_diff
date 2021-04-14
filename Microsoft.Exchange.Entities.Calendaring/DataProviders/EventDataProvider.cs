using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.Serialization;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.Translators;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class EventDataProvider : StorageItemDataProvider<IStoreSession, Event, ICalendarItemBase>
	{
		public EventDataProvider(IStorageEntitySetScope<IStoreSession> scope, StoreId calendarFolderId) : base(scope, calendarFolderId, ExTraceGlobals.EventDataProviderTracer)
		{
		}

		protected override IStorageTranslator<ICalendarItemBase, Event> Translator
		{
			get
			{
				return EventTranslator.Instance;
			}
		}

		private static Trace InstanceQueryTrace
		{
			get
			{
				return ExTraceGlobals.InstancesQueryTracer;
			}
		}

		public static ExDateTime EnforceMidnightTime(ExDateTime time, MidnightEnforcementOption option = MidnightEnforcementOption.Throw)
		{
			if (!(time != time.Date))
			{
				return time;
			}
			switch (option)
			{
			case MidnightEnforcementOption.Throw:
				throw new InvalidRequestException(CalendaringStrings.ErrorAllDayTimesMustBeMidnight);
			case MidnightEnforcementOption.MoveBackward:
				return time.Date;
			case MidnightEnforcementOption.MoveForward:
				return time.Date.AddDays(1.0);
			default:
				throw new ArgumentOutOfRangeException("option");
			}
		}

		public virtual IEnumerable<Event> GetCalendarView(ExDateTime startTime, ExDateTime endTime, bool includeSeriesMasters, params Microsoft.Exchange.Data.PropertyDefinition[] propertiesToLoad)
		{
			IEnumerable<Event> result;
			using (ICalendarFolder calendarFolder = base.XsoFactory.BindToCalendarFolder(base.Session, base.ContainerFolderId))
			{
				Dictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices = this.GetPropertyIndices(propertiesToLoad);
				object[][] syncView = calendarFolder.GetSyncView(startTime, endTime, CalendarViewBatchingStrategy.CreateNoneBatchingInstance(), propertiesToLoad, true);
				IEnumerable<Event> enumerable = this.ReadQueryResults(syncView, propertyIndices);
				result = (includeSeriesMasters ? enumerable : this.FilterOutType(enumerable, EventType.SeriesMaster));
			}
			return result;
		}

		public virtual void RespondToEvent(StoreId id, RespondToEventParameters parameters, Event updateToEvent = null)
		{
			if (parameters == null)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorMissingRequiredRespondParameter);
			}
			MeetingResponse meetingResponse = null;
			try
			{
				try
				{
					using (ICalendarItemBase calendarItemBase = this.Bind(id))
					{
						calendarItemBase.OpenAsReadWrite();
						if (updateToEvent != null)
						{
							EventTranslator.Instance.SetPropertiesFromEntityOnStorageObject(updateToEvent, calendarItemBase);
						}
						meetingResponse = calendarItemBase.RespondToMeetingRequest(default(ResponseTypeConverter).Convert(parameters.Response), true, true, parameters.ProposedStartTime, parameters.ProposedEndTime);
					}
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new AccessDeniedException(Strings.ErrorAccessDenied, innerException);
				}
				if (parameters.SendResponse)
				{
					EventWorkflowParametersTranslator<RespondToEventParameters, RespondToEventParametersSchema>.Instance.SetPropertiesFromEntityOnStorageObject(parameters, meetingResponse);
					MeetingMessage.SendLocalOrRemote(meetingResponse, true, true);
				}
			}
			finally
			{
				if (meetingResponse != null)
				{
					meetingResponse.Dispose();
				}
			}
		}

		public override Event ConvertToEntity(ICalendarItemBase storeObject)
		{
			ICalendarItem calendarItem = storeObject as ICalendarItem;
			if (calendarItem == null || string.IsNullOrEmpty(storeObject.SeriesId))
			{
				return base.ConvertToEntity(storeObject);
			}
			Event result;
			using (ICalendarItemBase calendarItemBase = this.BindToMasterFromInstance(calendarItem))
			{
				result = ((calendarItemBase != null) ? this.Translator.ConvertToEntity(calendarItemBase, storeObject) : base.ConvertToEntity(storeObject));
			}
			return result;
		}

		public void CancelEvent(StoreId id, CancelEventParameters parameters, int? seriesSequenceNumber = null, bool deleteAfterCancelling = true, Event updateToEvent = null, byte[] masterGoid = null)
		{
			using (ICalendarItemBase calendarItemBase = this.Bind(id))
			{
				if (calendarItemBase.IsMeeting && !calendarItemBase.IsOrganizer())
				{
					throw new InvalidRequestException(CalendaringStrings.ErrorNotAuthorizedToCancel);
				}
				calendarItemBase.OpenAsReadWrite();
				if (updateToEvent != null)
				{
					EventTranslator.Instance.SetPropertiesFromEntityOnStorageObject(updateToEvent, calendarItemBase);
				}
				if (calendarItemBase.IsMeeting && calendarItemBase.AttendeeCollection != null && calendarItemBase.AttendeeCollection.Count > 0)
				{
					using (MeetingCancellation meetingCancellation = ((CalendarItemBase)calendarItemBase).CancelMeeting(seriesSequenceNumber, masterGoid))
					{
						if (parameters != null)
						{
							EventWorkflowParametersTranslator<CancelEventParameters, CancelEventParametersSchema>.Instance.SetPropertiesFromEntityOnStorageObject(parameters, meetingCancellation);
						}
						MeetingMessage.SendLocalOrRemote(meetingCancellation, true, true);
						goto IL_95;
					}
				}
				if (updateToEvent != null)
				{
					calendarItemBase.SaveWithConflictCheck(SaveMode.ResolveConflicts);
				}
				IL_95:;
			}
			if (deleteAfterCancelling)
			{
				this.Delete(id, DeleteItemFlags.MoveToDeletedItems | DeleteItemFlags.CancelCalendarItem);
			}
		}

		public bool ForEachSeriesItem(Event theEvent, Func<Event, bool> instanceAction, Func<IStorePropertyBag, Event> convertToEvent, Action<Event> seriesAction = null, SortBy sortOrder = null, Microsoft.Exchange.Data.PropertyDefinition identityProperty = null, params Microsoft.Exchange.Data.PropertyDefinition[] additionalPropertiesToQuery)
		{
			if (theEvent.Type != EventType.SeriesMaster)
			{
				throw new InvalidOperationException("You can only call this method for a series master");
			}
			foreach (Event @event in this.GetSeriesEventsData(theEvent, identityProperty, convertToEvent, sortOrder, additionalPropertiesToQuery))
			{
				if (@event.Type == EventType.SeriesMaster)
				{
					if (seriesAction != null)
					{
						seriesAction(@event);
					}
				}
				else if (instanceAction != null && !instanceAction(@event))
				{
					return false;
				}
			}
			return true;
		}

		public string CreateOccurrenceViewPropertiesBlob(Event master)
		{
			List<Event> occurrencesWithViewProperties = new List<Event>();
			this.ForEachSeriesItem(master, delegate(Event instance)
			{
				occurrencesWithViewProperties.Add(instance);
				return true;
			}, new Func<IStorePropertyBag, Event>(this.GetOccurrenceWithViewProperties), null, null, CalendarItemBaseSchema.SeriesId, new Microsoft.Exchange.Data.PropertyDefinition[]
			{
				CalendarItemInstanceSchema.StartTime,
				CalendarItemInstanceSchema.EndTime,
				ItemSchema.Subject,
				CalendarItemBaseSchema.Location,
				ItemSchema.Sensitivity,
				CalendarItemBaseSchema.IsAllDayEvent,
				CalendarItemBaseSchema.GlobalObjectId
			});
			return EntitySerializer.Serialize<List<Event>>(occurrencesWithViewProperties);
		}

		public void ForwardEvent(StoreId id, ForwardEventParameters parameters, Event updateToEvent = null, int? seriesSequenceNumber = null, string occurrencesViewPropertiesBlob = null, CommandContext commandContext = null)
		{
			using (ICalendarItemBase calendarItemBase = this.Bind(id))
			{
				if (updateToEvent != null)
				{
					calendarItemBase.OpenAsReadWrite();
					this.UpdateOnly(updateToEvent, calendarItemBase, base.GetSaveMode(updateToEvent.ChangeKey, commandContext));
				}
				CalendarItemBase calendarItemBase2 = (CalendarItemBase)calendarItemBase;
				BodyFormat targetFormat = BodyFormat.TextPlain;
				if (parameters != null && parameters.Notes != null)
				{
					targetFormat = parameters.Notes.ContentType.ToStorageType();
				}
				ReplyForwardConfiguration replyForwardParameters = new ReplyForwardConfiguration(targetFormat)
				{
					ShouldSuppressReadReceipt = false
				};
				MailboxSession mailboxSession = base.Session as MailboxSession;
				using (MessageItem messageItem = calendarItemBase2.CreateForward(mailboxSession, CalendarItemBase.GetDraftsFolderIdOrThrow(mailboxSession), replyForwardParameters, seriesSequenceNumber, occurrencesViewPropertiesBlob))
				{
					EventWorkflowParametersTranslator<ForwardEventParameters, ForwardEventParametersSchema>.Instance.SetPropertiesFromEntityOnStorageObject(parameters, messageItem);
					foreach (Recipient<RecipientSchema> recipient in parameters.Forwardees)
					{
						messageItem.Recipients.Add(new Participant(recipient.Name, recipient.EmailAddress, "SMTP"));
					}
					MeetingMessage.SendLocalOrRemote(messageItem, true, true);
				}
			}
		}

		public override void Validate(Event entity, bool isNew)
		{
			if (entity.IsPropertySet(entity.Schema.PopupReminderSettingsProperty))
			{
				this.ValidatePopupReminderSettings(entity, isNew);
			}
			base.Validate(entity, isNew);
		}

		public virtual ICalendarItemBase BindToMasterFromSeriesId(string seriesId)
		{
			StoreId seriesMasterIdFromSeriesId = this.GetSeriesMasterIdFromSeriesId(seriesId);
			if (seriesMasterIdFromSeriesId == null)
			{
				return null;
			}
			return this.Bind(seriesMasterIdFromSeriesId);
		}

		public override void Delete(StoreId id, DeleteItemFlags flags)
		{
			base.Delete(id, flags);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			if (storeObjectId is OccurrenceStoreObjectId)
			{
				this.TryLogCalendarEventActivity(ActivityId.UpdateCalendarEvent, storeObjectId);
			}
		}

		internal void TryLogCalendarEventActivity(ActivityId activityId, StoreObjectId itemId)
		{
			if (base.Session.ActivitySession != null && this.ShouldLogActivityForAggregation())
			{
				base.Session.ActivitySession.CaptureCalendarEventActivity(activityId, itemId);
			}
		}

		internal bool TryGetPropertyFromPropertyIndices(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, Microsoft.Exchange.Data.PropertyDefinition property, out object value)
		{
			int index;
			object obj;
			if (propertyIndices.TryGetValue(property, out index) && (obj = values[index]) != null && obj.GetType() == property.Type)
			{
				value = obj;
				return true;
			}
			value = null;
			return false;
		}

		protected internal override ICalendarItemBase BindToStoreObject(StoreId id)
		{
			return base.XsoFactory.BindToCalendarItemBase(base.Session, id, null);
		}

		protected virtual bool ShouldLogActivityForAggregation()
		{
			return base.Session.MailboxOwner.MailboxInfo.IsAggregated;
		}

		protected virtual ICalendarItemBase BindToMasterFromInstance(ICalendarItem instance)
		{
			ICalendarItemBase result;
			if (!this.TryBindToMasterFromSeriesMasterId(instance, out result))
			{
				result = this.BindToMasterFromSeriesId(instance.SeriesId);
			}
			return result;
		}

		protected virtual IEnumerable<Event> FilterOutType(IEnumerable<Event> events, EventType type)
		{
			return from theEvent in events
			where theEvent.Type != type
			select theEvent;
		}

		protected virtual bool TryBindToMasterFromSeriesMasterId(ICalendarItem instance, out ICalendarItemBase master)
		{
			master = null;
			StoreId id;
			if (!this.TryGetSeriesMasterId(instance, out id))
			{
				return false;
			}
			bool result;
			try
			{
				master = this.Bind(id);
				result = true;
			}
			catch (ObjectNotFoundException arg)
			{
				base.Trace.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "Error while binding master based on SeriesMasterId. {0}", arg);
				result = false;
			}
			return result;
		}

		protected virtual bool TryGetSeriesMasterId(ICalendarItem item, out StoreId seriesMasterId)
		{
			string text;
			if (EventTranslator.SeriesMasterIdRule.TryGetValue(item, out text) && !string.IsNullOrEmpty(text))
			{
				seriesMasterId = base.IdConverter.ToStoreObjectId(text);
				return true;
			}
			seriesMasterId = null;
			return false;
		}

		protected override ICalendarItemBase CreateNewStoreObject()
		{
			return base.XsoFactory.CreateCalendarItem(base.Session, base.ContainerFolderId);
		}

		protected virtual StoreId GetSeriesMasterIdFromSeriesId(string seriesId)
		{
			Event theEvent = new Event
			{
				SeriesId = seriesId,
				Type = EventType.SeriesMaster
			};
			Event masterFromStore = null;
			this.ForEachSeriesItem(theEvent, null, (IStorePropertyBag bag) => EventExtensions.GetBasicSeriesEventData(bag, base.Scope), delegate(Event master)
			{
				masterFromStore = master;
			}, null, null, new Microsoft.Exchange.Data.PropertyDefinition[0]);
			if (masterFromStore == null)
			{
				return null;
			}
			return base.IdConverter.ToStoreObjectId(masterFromStore.Id);
		}

		protected override void ValidateChanges(Event sourceEntity, ICalendarItemBase targetStoreObject)
		{
			bool flag;
			if (sourceEntity.IsPropertySet(sourceEntity.Schema.IsAllDayProperty))
			{
				flag = sourceEntity.IsAllDay;
				if (flag != targetStoreObject.IsAllDayEvent)
				{
					sourceEntity.ThrowIfPropertyNotSet(sourceEntity.Schema.StartProperty);
					sourceEntity.ThrowIfPropertyNotSet(sourceEntity.Schema.EndProperty);
				}
			}
			else
			{
				flag = targetStoreObject.IsAllDayEvent;
			}
			if (flag)
			{
				if (sourceEntity.IsPropertySet(sourceEntity.Schema.StartProperty))
				{
					EventDataProvider.EnforceMidnightTime(sourceEntity.Start, MidnightEnforcementOption.Throw);
					ExTimeZone exTimeZone;
					if (sourceEntity.IsPropertySet(sourceEntity.Schema.EndProperty))
					{
						EventDataProvider.EnforceMidnightTime(sourceEntity.End, MidnightEnforcementOption.Throw);
						exTimeZone = sourceEntity.End.TimeZone;
					}
					else
					{
						exTimeZone = targetStoreObject.EndTimeZone;
					}
					if (sourceEntity.Start.TimeZone != exTimeZone)
					{
						throw new InvalidRequestException(CalendaringStrings.ErrorAllDayTimeZoneMismatch);
					}
				}
				else if (sourceEntity.IsPropertySet(sourceEntity.Schema.EndProperty))
				{
					EventDataProvider.EnforceMidnightTime(sourceEntity.End, MidnightEnforcementOption.Throw);
					if (sourceEntity.End.TimeZone != targetStoreObject.StartTimeZone)
					{
						throw new InvalidRequestException(CalendaringStrings.ErrorAllDayTimeZoneMismatch);
					}
				}
			}
			base.ValidateChanges(sourceEntity, targetStoreObject);
		}

		protected override IEnumerable<Event> ReadQueryResults(object[][] rows, Dictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices)
		{
			List<Event> list = new List<Event>();
			Dictionary<string, Event> dictionary = new Dictionary<string, Event>();
			foreach (object[] values in rows)
			{
				Event @event = this.Translator.ConvertToEntity(propertyIndices, values, base.Session);
				list.Add(@event);
				object obj;
				if (@event.Type == EventType.SeriesMaster && this.TryGetPropertyFromPropertyIndices(propertyIndices, values, CalendarItemBaseSchema.SeriesId, out obj) && !dictionary.ContainsKey(@event.SeriesId))
				{
					dictionary.Add(@event.SeriesId, @event);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Event event2 = list[j];
				Event master2;
				if (event2.Type == EventType.Exception && dictionary.TryGetValue(event2.SeriesId, out master2))
				{
					IList<string> overridenProperties = new List<string>();
					object obj2;
					if (this.TryGetPropertyFromPropertyIndices(propertyIndices, rows[j], CalendarItemInstanceSchema.PropertyChangeMetadataRaw, out obj2))
					{
						PropertyChangeMetadata metadata = PropertyChangeMetadata.Parse((byte[])obj2);
						overridenProperties = EventTranslator.Instance.GetOverridenProperties(metadata);
					}
					master2.CopyMasterPropertiesTo(event2, false, (Event occurrence, Event master, Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition property) => master.IsPropertySet(property) && !overridenProperties.Contains(property.Name), false);
				}
			}
			return list;
		}

		private static ComparisonFilter CreateComparisonFilterForSeriesSearch(Event entity, Microsoft.Exchange.Data.PropertyDefinition identityProperty)
		{
			ComparisonFilter result;
			if (identityProperty.Equals(CalendarItemBaseSchema.SeriesId))
			{
				result = new ComparisonFilter(ComparisonOperator.Equal, CalendarItemBaseSchema.SeriesId, entity.SeriesId);
			}
			else
			{
				if (!identityProperty.Equals(CalendarItemBaseSchema.EventClientId))
				{
					throw new ArgumentOutOfRangeException("identityProperty");
				}
				result = new ComparisonFilter(ComparisonOperator.Equal, CalendarItemBaseSchema.EventClientId, entity.ClientId);
			}
			return result;
		}

		private static SortBy[] CreateSortOrderForSeriesSearch(Microsoft.Exchange.Data.PropertyDefinition identityProperty, SortBy additionalSortBy)
		{
			List<SortBy> list = new List<SortBy>(2)
			{
				new SortBy(identityProperty, SortOrder.Ascending)
			};
			if (additionalSortBy != null)
			{
				list.Add(additionalSortBy);
			}
			return list.ToArray();
		}

		private static bool IsSeriesIdMatching(string seriesId, IStorePropertyBag rowPropertyBag, Microsoft.Exchange.Data.PropertyDefinition identityProperty)
		{
			return string.Equals(rowPropertyBag.GetValueOrDefault<string>(identityProperty, null), seriesId, StringComparison.OrdinalIgnoreCase);
		}

		private void ValidatePopupReminderSettings(Event entity, bool isNew)
		{
			IList<EventPopupReminderSetting> popupReminderSettings = entity.PopupReminderSettings;
			if (popupReminderSettings == null)
			{
				throw new NullPopupReminderSettingsException();
			}
			if (popupReminderSettings.Count != 1)
			{
				throw new InvalidPopupReminderSettingsCountException(popupReminderSettings.Count);
			}
			EventPopupReminderSetting eventPopupReminderSetting = popupReminderSettings[0];
			if (eventPopupReminderSetting == null)
			{
				throw new NullPopupReminderSettingsException();
			}
			if (isNew)
			{
				if (!string.IsNullOrEmpty(eventPopupReminderSetting.Id))
				{
					throw new InvalidNewReminderSettingIdException();
				}
			}
			else if (eventPopupReminderSetting.Id != EventPopupReminderSettingsRules.GetDefaultPopupReminderSettingId(entity))
			{
				throw new InvalidReminderSettingIdException();
			}
		}

		private IEnumerable<Event> GetSeriesEventsData(Event theEvent, Microsoft.Exchange.Data.PropertyDefinition identityProperty, Func<IStorePropertyBag, Event> convertToEvent, SortBy sortOrder, params Microsoft.Exchange.Data.PropertyDefinition[] additionalPropertiesToQuery)
		{
			if (identityProperty == null)
			{
				identityProperty = CalendarItemBaseSchema.SeriesId;
			}
			ComparisonFilter condition = EventDataProvider.CreateComparisonFilterForSeriesSearch(theEvent, identityProperty);
			string identityBeingSearched = condition.PropertyValue.ToString();
			SortBy[] internalSortOrder = EventDataProvider.CreateSortOrderForSeriesSearch(condition.Property, sortOrder);
			ICollection<Microsoft.Exchange.Data.PropertyDefinition> propertiesToQuery = new List<Microsoft.Exchange.Data.PropertyDefinition>(EventDataProvider.InstanceRequiredPropertiesToQuery);
			propertiesToQuery.Add(identityProperty);
			if (additionalPropertiesToQuery != null)
			{
				propertiesToQuery = propertiesToQuery.Union(additionalPropertiesToQuery);
			}
			using (ICalendarFolder calendarFolder = base.XsoFactory.BindToCalendarFolder(base.Session, base.ContainerFolderId))
			{
				using (IQueryResult queryResult = calendarFolder.IItemQuery(ItemQueryType.None, null, internalSortOrder, propertiesToQuery))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, condition);
					for (;;)
					{
						IStorePropertyBag[] rows = queryResult.GetPropertyBags(50);
						EventDataProvider.InstanceQueryTrace.TraceDebug<int, string>((long)theEvent.GetHashCode(), "EventExtension::GetDataForSeries retrieved {0} instances for series {1}.", rows.Length, identityBeingSearched);
						foreach (IStorePropertyBag rowPropertyBag in rows)
						{
							string itemClass = rowPropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, null);
							if (ObjectClass.IsCalendarItem(itemClass) || ObjectClass.IsCalendarItemSeries(itemClass))
							{
								if (!EventDataProvider.IsSeriesIdMatching(identityBeingSearched, rowPropertyBag, condition.Property))
								{
									goto IL_211;
								}
								yield return convertToEvent(rowPropertyBag);
							}
							else
							{
								EventDataProvider.InstanceQueryTrace.TraceDebug<string>((long)theEvent.GetHashCode(), "EventExtension::GetDataForSeries will skip item with class {0}.", itemClass);
							}
						}
						if (rows.Length <= 0)
						{
							goto Block_10;
						}
					}
					IL_211:
					EventDataProvider.InstanceQueryTrace.TraceDebug<string>((long)theEvent.GetHashCode(), "EventExtension::GetDataForSeries found all the items related to series {0} and will return.", identityBeingSearched);
					yield break;
					Block_10:;
				}
			}
			yield break;
		}

		private Event GetOccurrenceWithViewProperties(IStorePropertyBag propertyBag)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, base.Scope);
			basicSeriesEventData.Start = propertyBag.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.StartTime, ExDateTime.MinValue);
			basicSeriesEventData.End = propertyBag.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.EndTime, ExDateTime.MaxValue);
			basicSeriesEventData.Subject = propertyBag.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
			basicSeriesEventData.Location = new Location
			{
				DisplayName = propertyBag.GetValueOrDefault<string>(CalendarItemBaseSchema.Location, string.Empty)
			};
			basicSeriesEventData.Sensitivity = default(SensitivityConverter).StorageToEntitiesConverter.Convert(propertyBag.GetValueOrDefault<Microsoft.Exchange.Data.Storage.Sensitivity>(ItemSchema.Sensitivity, Microsoft.Exchange.Data.Storage.Sensitivity.Normal));
			basicSeriesEventData.IsAllDay = propertyBag.GetValueOrDefault<bool>(CalendarItemBaseSchema.IsAllDayEvent, false);
			IEventInternal eventInternal = basicSeriesEventData;
			eventInternal.GlobalObjectId = new GlobalObjectId(propertyBag.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.GlobalObjectId, null)).ToString();
			return basicSeriesEventData;
		}

		internal const DeleteItemFlags DeleteFlags = DeleteItemFlags.MoveToDeletedItems | DeleteItemFlags.CancelCalendarItem;

		private static readonly Microsoft.Exchange.Data.PropertyDefinition[] InstanceRequiredPropertiesToQuery = new Microsoft.Exchange.Data.PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.SeriesId
		};
	}
}
