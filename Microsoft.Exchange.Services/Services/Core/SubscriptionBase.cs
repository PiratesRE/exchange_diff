using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SubscriptionBase : IDisposable
	{
		public SubscriptionBase(SubscriptionRequestBase subscriptionRequest, IdAndSession[] folderIds, Guid subscriptionOwnerObjectGuid) : this(subscriptionRequest, folderIds)
		{
			this.ownerObjectGuid = subscriptionOwnerObjectGuid;
		}

		protected SubscriptionBase(SubscriptionRequestBase subscriptionRequest, IdAndSession[] folderIds)
		{
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<int>((long)this.GetHashCode(), "SubscriptionBase constructor called. Hashcode: {0}.", this.GetHashCode());
			this.budgetKey = CallContext.Current.Budget.Owner;
			this.mailboxVersion = CallContext.Current.GetAccessingPrincipalServerVersion();
			StoreSession storeSessionForOperation = this.GetStoreSessionForOperation(folderIds);
			if (storeSessionForOperation is MailboxSession)
			{
				this.mailboxId = new MailboxId((MailboxSession)storeSessionForOperation);
			}
			Guid mailboxGuid = (this.MailboxId != null) ? new Guid(this.MailboxId.MailboxGuid) : Guid.Empty;
			this.subscriptionId = new SubscriptionId(Guid.NewGuid(), mailboxGuid).ToString();
			StoreId[] array = null;
			if (folderIds != null)
			{
				array = new StoreId[folderIds.Length];
				for (int i = 0; i < folderIds.Length; i++)
				{
					array[i] = folderIds[i].Id;
				}
			}
			this.eventQueue = this.CreateEventQueue(subscriptionRequest, storeSessionForOperation, array);
			this.lastWatermarkSent = this.eventQueue.CurrentWatermark.ToBase64String();
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, BudgetKey>((long)this.GetHashCode(), "SubscriptionBase constructor. Subscription: {0}. BudgetKey: {1}", this.TraceIdentifier, this.budgetKey);
			if (CallContext.Current.AccessingPrincipal == null)
			{
				this.CreatorSmtpAddress = string.Empty;
				return;
			}
			RemoteUserMailboxPrincipal remoteUserMailboxPrincipal = CallContext.Current.AccessingPrincipal as RemoteUserMailboxPrincipal;
			if (remoteUserMailboxPrincipal != null)
			{
				this.CreatorSmtpAddress = remoteUserMailboxPrincipal.PrimarySmtpAddress.ToString();
				return;
			}
			this.CreatorSmtpAddress = CallContext.Current.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
		}

		private static EventWatermark ConvertToEventWatermark(string watermark)
		{
			if (!string.IsNullOrEmpty(watermark))
			{
				try
				{
					return EventWatermark.Deserialize(watermark);
				}
				catch (CorruptDataException innerException)
				{
					throw new InvalidWatermarkException(innerException);
				}
			}
			return null;
		}

		private static void AddFoldersToMonitor(EventCondition eventCondition, StoreId[] folderIds)
		{
			if (folderIds != null)
			{
				for (int i = 0; i < folderIds.Length; i++)
				{
					StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(folderIds[i]);
					eventCondition.ContainerFolderIds.Add(asStoreObjectId);
					eventCondition.ObjectIds.Add(asStoreObjectId);
				}
			}
		}

		private EventQueue CreateEventQueue(SubscriptionRequestBase subscriptionRequest, StoreSession session, StoreId[] folderIds)
		{
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.CreateEventQueue called. Subscription: {0}.", this.TraceIdentifier);
			EventWatermark eventWatermark = SubscriptionBase.ConvertToEventWatermark(subscriptionRequest.Watermark);
			this.EventCondition = this.CreateEventCondition(subscriptionRequest, folderIds);
			this.EventCondition.EventSubtree = ((subscriptionRequest.SubscribeToAllFolders && this.mailboxVersion >= Server.E14SP1MinVersion) ? EventSubtreeFlag.IPMSubtree : EventSubtreeFlag.All);
			EventQueue result;
			if (eventWatermark == null)
			{
				result = EventQueue.Create(session, this.EventCondition, this.EventQueueSize);
			}
			else
			{
				result = EventQueue.Create(session, this.EventCondition, this.EventQueueSize, eventWatermark);
			}
			return result;
		}

		protected StoreSession GetStoreSessionForOperation(IdAndSession[] folderIds)
		{
			StoreSession result;
			if (folderIds != null && folderIds.Length != 0)
			{
				result = folderIds[0].Session;
			}
			else
			{
				result = CallContext.Current.SessionCache.GetMailboxIdentityMailboxSession();
			}
			return result;
		}

		private EventCondition CreateEventCondition(SubscriptionRequestBase subscriptionRequest, StoreId[] folderIds)
		{
			EventCondition eventCondition = new EventCondition();
			eventCondition.ObjectType = (EventObjectType.Item | EventObjectType.Folder);
			SubscriptionBase.AddFoldersToMonitor(eventCondition, folderIds);
			this.SetEventTypesToMonitor(eventCondition, subscriptionRequest.EventTypes);
			return eventCondition;
		}

		private void SetEventTypesToMonitor(EventCondition eventCondition, EventType[] eventTypes)
		{
			eventCondition.EventType = EventType.None;
			for (int i = 0; i < eventTypes.Length; i++)
			{
				eventCondition.EventType |= (EventType)eventTypes[i];
			}
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, EventType>((long)this.GetHashCode(), "SubscriptionBase.CreateEvent. Subscription: {0}. EventType: {1}", this.TraceIdentifier, eventCondition.EventType);
		}

		public string SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		public BudgetKey BudgetKey
		{
			get
			{
				return this.budgetKey;
			}
		}

		public bool CheckForEventsLater()
		{
			bool result;
			lock (this.lockObject)
			{
				if (this.isDisposed)
				{
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.CheckForEventsLater. InvalidSubscriptionException. Subscription: {0}.", this.TraceIdentifier);
					throw new InvalidSubscriptionException();
				}
				result = !this.eventQueue.IsQueueEmptyAndUpToDate();
			}
			return result;
		}

		internal string LastWatermarkSent
		{
			get
			{
				return this.lastWatermarkSent;
			}
		}

		internal MailboxId MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		internal EventQueue EventQueue
		{
			get
			{
				return this.eventQueue;
			}
		}

		internal EventCondition EventCondition { get; set; }

		internal string CreatorSmtpAddress { get; private set; }

		internal string TraceIdentifier
		{
			get
			{
				if (this.MailboxId != null)
				{
					return string.Format("{0}:{1}", this.MailboxId.SmtpAddress, this.SubscriptionId);
				}
				return this.SubscriptionId;
			}
		}

		protected abstract int EventQueueSize { get; }

		public virtual EwsNotificationType GetEvents(string theLastWatermarkSent)
		{
			int num;
			return this.GetEvents(theLastWatermarkSent, 50, out num);
		}

		public virtual EwsNotificationType GetEvents(string theLastWatermarkSent, int maxEventCount, out int eventCount)
		{
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.GetEvents called. Before lock. Subscription: {0}.", this.TraceIdentifier);
			EwsNotificationType result;
			lock (this.lockObject)
			{
				if (this.isDisposed)
				{
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.GetEvents. InvalidSubscriptionException. Subscription: {0}.", this.TraceIdentifier);
					throw new InvalidSubscriptionException();
				}
				this.VerifyLastWatermarkSent(theLastWatermarkSent);
				List<Event> list;
				try
				{
					list = this.CreateEventList(maxEventCount);
					eventCount = list.Count;
				}
				catch (Exception arg)
				{
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "SubscriptionBase::GetEvents Subscription {0} failed with exception {1}", this.TraceIdentifier, arg);
					PerformanceMonitor.UpdateFailedSubscriptionCounter();
					throw;
				}
				string lastWatermark = this.GetLastWatermark(list);
				this.TraceEvents(list, lastWatermark);
				EwsNotificationType ewsNotificationType = this.CreateNotifications(list, lastWatermark);
				this.lastWatermarkSent = lastWatermark;
				ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<int, EwsNotificationType>((long)this.GetHashCode(), "SubscriptionBase.GetEvents. Hashcode: {0}. Notifications: {1}", this.GetHashCode(), ewsNotificationType);
				result = ewsNotificationType;
			}
			return result;
		}

		private void VerifyLastWatermarkSent(string theLastWatermarkSent)
		{
			if (string.CompareOrdinal(theLastWatermarkSent, this.lastWatermarkSent) != 0)
			{
				ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.GetEvents. InvalidWatermarkException. Subscription: {0}.", this.TraceIdentifier);
				throw new InvalidWatermarkException();
			}
		}

		private List<Event> CreateEventList(int maximumEvents)
		{
			List<Event> list = new List<Event>();
			Event @event;
			do
			{
				@event = this.eventQueue.GetEvent();
				FaultInjection.GenerateFault((FaultInjection.LIDs)3534105917U);
				if (@event != null)
				{
					list.Add(@event);
				}
			}
			while (@event != null && list.Count < maximumEvents);
			if (list.Count > 1)
			{
				Dictionary<Tuple<StoreObjectId, StoreObjectId>, Event> dictionary = new Dictionary<Tuple<StoreObjectId, StoreObjectId>, Event>(list.Count / 2 + 1);
				for (int i = list.Count - 1; i >= 0; i--)
				{
					Event event2 = list[i];
					if (event2.EventType == EventType.ObjectModified)
					{
						Tuple<StoreObjectId, StoreObjectId> key = Tuple.Create<StoreObjectId, StoreObjectId>(event2.ObjectId, event2.ParentObjectId);
						if (dictionary.ContainsKey(key))
						{
							list.RemoveAt(i);
						}
						else
						{
							dictionary.Add(key, event2);
						}
					}
				}
			}
			return list;
		}

		private EwsNotificationType CreateNotifications(List<Event> events, string newLastWatermarkSent)
		{
			EwsNotificationType ewsNotificationType = new EwsNotificationType();
			SubscriptionBase.NotificationBuilder notificationBuilder = new SubscriptionBase.NotificationBuilder(this, ewsNotificationType);
			if (events.Count > 0)
			{
				for (int i = 0; i < events.Count; i++)
				{
					notificationBuilder.AddEvent(events[i]);
				}
			}
			else
			{
				notificationBuilder.AddStatusEvent(newLastWatermarkSent);
			}
			return ewsNotificationType;
		}

		private string GetLastWatermark(List<Event> events)
		{
			EventWatermark eventWatermark;
			if (events.Count > 0)
			{
				eventWatermark = events[events.Count - 1].EventWatermark;
			}
			else
			{
				eventWatermark = this.eventQueue.CurrentWatermark;
			}
			return eventWatermark.ToBase64String();
		}

		public virtual bool CheckCallerHasRights(CallContext callContext)
		{
			Guid objectGuid = callContext.EffectiveCaller.ObjectGuid;
			if (objectGuid == Guid.Empty)
			{
				ExTraceGlobals.SubscriptionBaseTracer.TraceDebug((long)this.GetHashCode(), "[SubscriptionBase::CheckCallerHasRights] Passed callContext.EffectiveCaller.ObjectGuid is Guid.Empty.  Cannot be owner.");
				return false;
			}
			return objectGuid == this.ownerObjectGuid;
		}

		protected void TraceEvents(List<Event> events, string lastWatermark)
		{
			if (!ExTraceGlobals.SubscriptionBaseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (events.Count > 0)
			{
				ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "TraceEvents: Watermark {0}. {1} events retrieved from notification queue", lastWatermark, events.Count);
				for (int i = 0; i < events.Count; i++)
				{
					Event @event = events[i];
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug((long)this.GetHashCode(), "TraceEvents: Event[{0}] {1} {2} Id:{3} Parent:{4}", new object[]
					{
						i,
						@event.ObjectType,
						@event.EventType,
						@event.ObjectId,
						@event.ParentObjectId
					});
				}
				return;
			}
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "TraceEvents: Watermark {0}. No events in notification queue", lastWatermark, events.Count);
		}

		public virtual bool IsExpired
		{
			get
			{
				return false;
			}
		}

		public virtual bool UseWatermarks
		{
			get
			{
				return true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, bool, bool>((long)this.GetHashCode(), "SubscriptionBase.Dispose() called. Before lock. Subscription: {0}. IsDisposed: {1} IsDisposing: {2}", this.TraceIdentifier, this.isDisposed, isDisposing);
			if (!this.isDisposed)
			{
				if (isDisposing)
				{
					lock (this.lockObject)
					{
						ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "SubscriptionBase.Dispose(). After lock. Subscription: {0}. IsDisposed: {1}", this.TraceIdentifier, this.isDisposed);
						if (this.eventQueue != null)
						{
							this.eventQueue.Dispose();
							this.eventQueue = null;
							ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionBase.Dispose(). After EventQueue dispose. Subscription: {0}.", this.TraceIdentifier);
						}
						this.isDisposed = true;
						return;
					}
				}
				this.isDisposed = true;
			}
		}

		private const int MaximumNotificationEventCount = 50;

		private string subscriptionId;

		protected EventQueue eventQueue;

		private string lastWatermarkSent;

		private MailboxId mailboxId;

		private Guid ownerObjectGuid;

		protected bool isDisposed;

		private BudgetKey budgetKey;

		private int mailboxVersion;

		protected object lockObject = new object();

		private sealed class NotificationBuilder
		{
			internal NotificationBuilder(SubscriptionBase subscription, EwsNotificationType notificationObject)
			{
				this.notificationObject = notificationObject;
				this.subscription = subscription;
				this.supportsExchange2010SchemaVersion = ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2010);
				this.notificationObject.SubscriptionId = this.subscription.SubscriptionId;
				this.mailboxId = subscription.MailboxId;
				this.emitWatermarks = subscription.UseWatermarks;
				if (this.emitWatermarks)
				{
					this.notificationObject.PreviousWatermark = subscription.LastWatermarkSent;
					this.notificationObject.MoreEvents = subscription.CheckForEventsLater();
				}
			}

			internal void AddStatusEvent(string watermark)
			{
				BaseNotificationEventType baseNotificationEventType = new BaseNotificationEventType(NotificationTypeEnum.StatusEvent);
				baseNotificationEventType.Watermark = watermark;
				this.notificationObject.AddNotificationEvent(baseNotificationEventType);
			}

			internal void AddEvent(Event xsoEvent)
			{
				foreach (EventType eventType in SubscriptionBase.NotificationBuilder.allEventTypes)
				{
					if ((xsoEvent.EventType & eventType) == eventType && (this.subscription.EventCondition.EventType & eventType) == eventType)
					{
						BaseNotificationEventType baseNotificationEventType = null;
						EventType eventType2 = eventType;
						if (eventType2 <= EventType.ObjectModified)
						{
							switch (eventType2)
							{
							case EventType.NewMail:
								baseNotificationEventType = this.BuildObjectChangedEvent(NotificationTypeEnum.NewMailEvent, xsoEvent);
								break;
							case EventType.ObjectCreated:
								baseNotificationEventType = this.BuildObjectChangedEvent(NotificationTypeEnum.CreatedEvent, xsoEvent);
								break;
							case EventType.NewMail | EventType.ObjectCreated:
								break;
							case EventType.ObjectDeleted:
								baseNotificationEventType = this.BuildObjectChangedEvent(NotificationTypeEnum.DeletedEvent, xsoEvent);
								break;
							default:
								if (eventType2 == EventType.ObjectModified)
								{
									baseNotificationEventType = this.BuildModifiedEvent(xsoEvent);
								}
								break;
							}
						}
						else if (eventType2 != EventType.ObjectMoved)
						{
							if (eventType2 != EventType.ObjectCopied)
							{
								if (eventType2 == EventType.FreeBusyChanged)
								{
									baseNotificationEventType = this.BuildObjectChangedEvent(NotificationTypeEnum.FreeBusyChangedEvent, xsoEvent);
								}
							}
							else
							{
								baseNotificationEventType = this.BuildMovedCopiedEvent(NotificationTypeEnum.CopiedEvent, xsoEvent);
							}
						}
						else
						{
							baseNotificationEventType = this.BuildMovedCopiedEvent(NotificationTypeEnum.MovedEvent, xsoEvent);
						}
						if (baseNotificationEventType != null)
						{
							this.notificationObject.AddNotificationEvent(baseNotificationEventType);
						}
					}
				}
			}

			private BaseNotificationEventType BuildModifiedEvent(Event xsoEvent)
			{
				ModifiedEventType modifiedEventType = new ModifiedEventType();
				this.AddWatermarkAndTimeStamp(xsoEvent, modifiedEventType);
				if (xsoEvent.ObjectType == EventObjectType.Folder)
				{
					modifiedEventType.FolderId = this.CreateFolderId(xsoEvent.ObjectId);
					if (xsoEvent.UnreadItemCount >= 0L)
					{
						modifiedEventType.UnreadCount = (int)xsoEvent.UnreadItemCount;
					}
				}
				else
				{
					modifiedEventType.ItemId = this.CreateItemId(xsoEvent.ObjectId, xsoEvent.ParentObjectId);
				}
				modifiedEventType.ParentFolderId = this.CreateFolderId(xsoEvent.ParentObjectId);
				return modifiedEventType;
			}

			private BaseNotificationEventType BuildMovedCopiedEvent(NotificationTypeEnum eventType, Event xsoEvent)
			{
				MovedCopiedEventType movedCopiedEventType = new MovedCopiedEventType(eventType);
				this.AddWatermarkAndTimeStamp(xsoEvent, movedCopiedEventType);
				if (xsoEvent.ObjectType == EventObjectType.Folder)
				{
					movedCopiedEventType.FolderId = this.CreateFolderId(xsoEvent.ObjectId);
					movedCopiedEventType.OldFolderId = this.CreateFolderId(xsoEvent.OldObjectId);
				}
				else
				{
					movedCopiedEventType.ItemId = this.CreateItemId(xsoEvent.ObjectId, xsoEvent.ParentObjectId);
					movedCopiedEventType.OldItemId = this.CreateItemId(xsoEvent.OldObjectId, xsoEvent.OldParentObjectId);
				}
				movedCopiedEventType.ParentFolderId = this.CreateFolderId(xsoEvent.ParentObjectId);
				movedCopiedEventType.OldParentFolderId = this.CreateFolderId(xsoEvent.OldParentObjectId);
				return movedCopiedEventType;
			}

			private BaseNotificationEventType BuildObjectChangedEvent(NotificationTypeEnum eventType, Event xsoEvent)
			{
				BaseObjectChangedEventType baseObjectChangedEventType = new BaseObjectChangedEventType(eventType);
				this.AddWatermarkAndTimeStamp(xsoEvent, baseObjectChangedEventType);
				if (xsoEvent.ObjectType == EventObjectType.Folder)
				{
					baseObjectChangedEventType.FolderId = this.CreateFolderId(xsoEvent.ObjectId);
				}
				else
				{
					baseObjectChangedEventType.ItemId = this.CreateItemId(xsoEvent.ObjectId, xsoEvent.ParentObjectId);
				}
				baseObjectChangedEventType.ParentFolderId = this.CreateFolderId(xsoEvent.ParentObjectId);
				return baseObjectChangedEventType;
			}

			private void AddWatermarkAndTimeStamp(Event notificationEvent, BaseObjectChangedEventType newEvent)
			{
				if (this.emitWatermarks)
				{
					newEvent.Watermark = notificationEvent.EventWatermark.ToBase64String();
				}
				newEvent.TimeStamp = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(notificationEvent.TimeStamp, this.supportsExchange2010SchemaVersion);
			}

			private FolderId CreateFolderId(StoreObjectId xsoStoreId)
			{
				ConcatenatedIdAndChangeKey concatenatedIdAndChangeKey;
				if (this.mailboxId != null)
				{
					concatenatedIdAndChangeKey = IdConverter.GetConcatenatedId(xsoStoreId, this.mailboxId, null);
				}
				else
				{
					concatenatedIdAndChangeKey = IdConverter.GetConcatenatedIdForPublicFolder(xsoStoreId);
				}
				return new FolderId(concatenatedIdAndChangeKey.Id, concatenatedIdAndChangeKey.ChangeKey);
			}

			private ItemId CreateItemId(StoreObjectId xsoStoreId, StoreObjectId xsoParentFolderId)
			{
				ConcatenatedIdAndChangeKey concatenatedIdAndChangeKey;
				if (this.mailboxId != null)
				{
					concatenatedIdAndChangeKey = IdConverter.GetConcatenatedId(xsoStoreId, this.mailboxId, null);
				}
				else
				{
					concatenatedIdAndChangeKey = IdConverter.GetConcatenatedIdForPublicFolderItem(xsoStoreId, xsoParentFolderId, null);
				}
				return new ItemId(concatenatedIdAndChangeKey.Id, concatenatedIdAndChangeKey.ChangeKey);
			}

			private static readonly EventType[] allEventTypes = (EventType[])Enum.GetValues(typeof(EventType));

			private EwsNotificationType notificationObject;

			private SubscriptionBase subscription;

			private bool supportsExchange2010SchemaVersion;

			private bool emitWatermarks;

			private MailboxId mailboxId;
		}
	}
}
