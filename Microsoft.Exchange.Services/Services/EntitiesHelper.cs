using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services
{
	internal class EntitiesHelper
	{
		public EntitiesHelper(CallContext context)
		{
			this.edmIdConverter = IdConverter.Instance;
			this.Context = context;
			this.ewsIdConverter = new IdConverter(this.Context);
		}

		public CallContext Context { get; private set; }

		public TEntity Execute<TInput, TEntity>(Func<TInput, CommandContext, TEntity> function, StoreSession session, BasicTypes type, TInput input) where TInput : class where TEntity : class
		{
			CommandContext arg = this.TransformEwsIdsToEntityIds<TInput>(input, type);
			TEntity tentity = function(input, arg);
			this.TransformEntityIdsToEwsIds<TEntity>(tentity, session);
			return tentity;
		}

		public void Execute(Action<string, CommandContext> action, StoreSession session, BasicTypes type, string ewsId, string changeKey = null)
		{
			this.Execute<VoidResult>(delegate(string id, CommandContext context)
			{
				action(id, context);
				return VoidResult.Value;
			}, session, type, ewsId, changeKey);
		}

		public void Execute(Action<string, CommandContext> action, StoreSession session, BaseItemId itemId)
		{
			this.Execute<VoidResult>(delegate(string id, CommandContext context)
			{
				action(id, context);
				return VoidResult.Value;
			}, session, itemId);
		}

		public TResult Execute<TResult>(Func<string, CommandContext, TResult> function, StoreSession session, BasicTypes type, string ewsId, string changeKey = null) where TResult : class
		{
			CommandContext arg2;
			string arg = this.ToEntityId(ewsId, changeKey, type, out arg2);
			TResult tresult = function(arg, arg2);
			this.TransformEntityIdsToEwsIds<TResult>(tresult, session);
			return tresult;
		}

		public TResult Execute<TResult>(Func<string, CommandContext, TResult> function, StoreSession session, BaseItemId itemId) where TResult : class
		{
			CommandContext arg2;
			string arg = this.ToEntityId(itemId, out arg2);
			TResult tresult = function(arg, arg2);
			this.TransformEntityIdsToEwsIds<TResult>(tresult, session);
			return tresult;
		}

		public ICalendarGroups GetCalendarGroups(IStoreSession session)
		{
			CalendaringContainer calendaringContainer = EntitiesHelper.GetCalendaringContainer(session);
			return calendaringContainer.CalendarGroups;
		}

		public IEvents GetEvents(BaseFolderId calendarFolderId, out StoreSession session)
		{
			IdAndSession idAndSession = this.ewsIdConverter.ConvertFolderIdToIdAndSessionReadOnly(calendarFolderId);
			session = idAndSession.Session;
			if (calendarFolderId is DistinguishedFolderId)
			{
				calendarFolderId = IdConverter.GetFolderIdFromStoreId(idAndSession.Id, new MailboxId(session.MailboxGuid));
			}
			CalendaringContainer calendaringContainer = EntitiesHelper.GetCalendaringContainer(idAndSession.Session);
			return calendaringContainer.Calendars[calendarFolderId.GetId()].Events;
		}

		private static CalendaringContainer GetCalendaringContainer(IStoreSession session)
		{
			return new CalendaringContainer(session, null);
		}

		private string ToEntityId(string ewsId, string ewsChangeKey, BasicTypes type, out CommandContext commandContext)
		{
			IdConverter.ConvertOption convertOption = string.IsNullOrEmpty(ewsChangeKey) ? (IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.NoBind) : IdConverter.ConvertOption.NoBind;
			IdAndSession idAndSession;
			switch (type)
			{
			case BasicTypes.Folder:
				idAndSession = this.ewsIdConverter.ConvertFolderIdToIdAndSession(new FolderId(ewsId, ewsChangeKey), convertOption);
				break;
			case BasicTypes.Item:
				idAndSession = this.ewsIdConverter.ConvertItemIdToIdAndSession(new ItemId(ewsId, ewsChangeKey), convertOption, type);
				break;
			case BasicTypes.Attachment:
				idAndSession = this.ewsIdConverter.ConvertItemIdToIdAndSession(new AttachmentIdType(ewsId), convertOption, type);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, "The value is not supported.");
			}
			string text;
			string result = this.edmIdConverter.ToStringId(idAndSession.Id, idAndSession.Session, out text);
			commandContext = (string.IsNullOrEmpty(text) ? null : new CommandContext
			{
				IfMatchETag = text
			});
			return result;
		}

		private string ToEntityId(BaseItemId itemId, out CommandContext commandContext)
		{
			IdConverter.ConvertOption convertOption = string.IsNullOrEmpty(itemId.GetChangeKey()) ? (IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.NoBind) : IdConverter.ConvertOption.NoBind;
			IdAndSession idAndSession = this.ewsIdConverter.ConvertItemIdToIdAndSession(itemId, convertOption, BasicTypes.Item);
			string text;
			string result = this.edmIdConverter.ToStringId(idAndSession.Id, idAndSession.Session, out text);
			commandContext = (string.IsNullOrEmpty(text) ? null : new CommandContext
			{
				IfMatchETag = text
			});
			return result;
		}

		private ConcatenatedIdAndChangeKey ToEwsConcatenatedId(string entityId, StoreSession session, string changeKey = null)
		{
			StoreId storeId = this.edmIdConverter.ToStoreId(entityId, changeKey);
			IdAndSession idAndSession = new IdAndSession(storeId, session);
			return IdConverter.GetConcatenatedId(storeId, idAndSession, null);
		}

		public TObject TransformEntityIdsToEwsIds<TObject>(TObject value, StoreSession session) where TObject : class
		{
			if (value == null)
			{
				return default(TObject);
			}
			IEntity entity = value as IEntity;
			if (entity != null)
			{
				IStorageEntity storageEntity = entity as IStorageEntity;
				ConcatenatedIdAndChangeKey concatenatedIdAndChangeKey;
				if (storageEntity != null)
				{
					concatenatedIdAndChangeKey = this.ToEwsConcatenatedId(storageEntity.Id, session, storageEntity.ChangeKey);
					storageEntity.Id = concatenatedIdAndChangeKey.Id;
					storageEntity.ChangeKey = concatenatedIdAndChangeKey.ChangeKey;
					Event @event = storageEntity as Event;
					if (@event != null && @event.IsPropertySet(@event.Schema.SeriesMasterIdProperty))
					{
						@event.SeriesMasterId = this.ToEwsConcatenatedId(@event.SeriesMasterId, session, null).Id;
					}
				}
				else
				{
					concatenatedIdAndChangeKey = this.ToEwsConcatenatedId(entity.Id, session, null);
				}
				entity.Id = concatenatedIdAndChangeKey.Id;
			}
			else
			{
				IList list = value as IList;
				if (list != null)
				{
					using (IEnumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object value2 = enumerator.Current;
							this.TransformEntityIdsToEwsIds<object>(value2, session);
						}
						return value;
					}
				}
				ExpandedEvent expandedEvent = value as ExpandedEvent;
				if (expandedEvent != null && expandedEvent.CancelledOccurrences != null)
				{
					for (int i = 0; i < expandedEvent.CancelledOccurrences.Count; i++)
					{
						string entityId = expandedEvent.CancelledOccurrences[i];
						ConcatenatedIdAndChangeKey concatenatedIdAndChangeKey2 = this.ToEwsConcatenatedId(entityId, session, null);
						expandedEvent.CancelledOccurrences[i] = concatenatedIdAndChangeKey2.Id;
					}
					this.TransformEntityIdsToEwsIds<IList<Event>>(expandedEvent.Occurrences, session);
					this.TransformEntityIdsToEwsIds<Event>(expandedEvent.RecurrenceMaster, session);
				}
			}
			return value;
		}

		private CommandContext TransformEwsIdsToEntityIds<TObject>(TObject value, BasicTypes type) where TObject : class
		{
			if (value == null)
			{
				return null;
			}
			IEntity entity = value as IEntity;
			CommandContext result = null;
			if (entity != null)
			{
				string id = entity.Id;
				IVersioned versioned = entity as IVersioned;
				string ewsChangeKey;
				if (versioned == null)
				{
					ewsChangeKey = null;
				}
				else
				{
					Event @event = versioned as Event;
					if (@event != null && !string.IsNullOrEmpty(@event.SeriesMasterId))
					{
						CommandContext commandContext;
						@event.SeriesMasterId = this.ToEntityId(@event.SeriesMasterId, null, BasicTypes.Item, out commandContext);
					}
					ewsChangeKey = versioned.ChangeKey;
				}
				if (!string.IsNullOrEmpty(id))
				{
					entity.Id = this.ToEntityId(id, ewsChangeKey, type, out result);
				}
			}
			else
			{
				IList list = value as IList;
				if (list != null)
				{
					foreach (object value2 in list)
					{
						this.TransformEwsIdsToEntityIds<object>(value2, type);
					}
				}
			}
			return result;
		}

		private readonly IdConverter edmIdConverter;

		private readonly IdConverter ewsIdConverter;
	}
}
