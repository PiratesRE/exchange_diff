using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Subscription : IDisposeTrackable, IDisposable
	{
		private Subscription(StoreSession storeSession, StoreObjectId storeObjectId, NotificationHandler handler, bool passthruCallback)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			this.storeSession = storeSession;
			this.itemId = storeObjectId;
			this.handler = handler;
			this.passthruCallback = passthruCallback;
			if (this.passthruCallback)
			{
				this.sink = new SubscriptionSink(this);
			}
			else
			{
				this.sink = new SubscriptionSink(storeSession.SubscriptionsManager, handler != null);
				storeSession.SubscriptionsManager.RegisterSubscription(this);
			}
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private Subscription(StoreSession storeSession, StoreObjectId storeObjectId, AdviseFlags flags, NotificationHandler handler, NotificationCallbackMode callbackMode, bool passthruCallback) : this(storeSession, storeObjectId, handler, passthruCallback)
		{
			if (passthruCallback)
			{
				callbackMode = NotificationCallbackMode.Sync;
			}
			try
			{
				MapiNotificationHandler mapiNotificationHandler = new MapiNotificationHandler(this.sink.OnNotify);
				if (callbackMode == NotificationCallbackMode.Async)
				{
					mapiNotificationHandler = new MapiNotificationHandler(this.OnNotify);
					this.waitCallback = new WaitCallback(this.WaitCallbackProc);
				}
				this.adviseId = this.storeSession.Mailbox.Advise((storeObjectId == null) ? null : storeObjectId.ProviderLevelItemId, flags, mapiNotificationHandler, callbackMode);
				this.notificationSource = this.storeSession.Mailbox;
			}
			catch
			{
				this.Dispose();
				throw;
			}
		}

		private Subscription(IQueryResult queryResult, NotificationHandler handler, bool passthruCallback) : this(queryResult.StoreSession, null, handler, passthruCallback)
		{
			this.queryResult = queryResult;
			this.notificationSource = (INotificationSource)queryResult;
			try
			{
				this.adviseId = queryResult.Advise(this.sink, false);
			}
			catch
			{
				this.Dispose();
				throw;
			}
		}

		public NotificationHandler Handler
		{
			get
			{
				this.CheckDisposed("Handler::get");
				return this.handler;
			}
		}

		public StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed("StoreSession::get");
				return this.storeSession;
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				this.CheckDisposed("ItemId::get");
				return this.itemId;
			}
		}

		public bool HasDroppedNotification
		{
			get
			{
				this.CheckDisposed("HasDroppedNotification::get");
				return this.sink.HasDroppedNotification;
			}
		}

		public bool HasPendingNotifications
		{
			get
			{
				this.CheckDisposed("HasPendingNotifications::get");
				return this.sink.Count > 0;
			}
		}

		internal SubscriptionSink Sink
		{
			get
			{
				return this.sink;
			}
		}

		public static Subscription Create(StoreSession session, NotificationHandler handler, NotificationType notificationType, StoreId id)
		{
			return Subscription.Create(session, handler, notificationType, id, true, false);
		}

		public static Subscription Create(StoreSession session, NotificationHandler handler, NotificationType notificationType, StoreId id, bool isSyncCallback)
		{
			return Subscription.Create(session, handler, notificationType, id, isSyncCallback, false);
		}

		public static Subscription Create(StoreSession session, NotificationHandler handler, NotificationType notificationType, StoreId id, bool isSyncCallback, bool passthruCallback)
		{
			EnumValidator.ThrowIfInvalid<NotificationType>(notificationType, "notificationType");
			if (session == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "session");
				throw new ArgumentNullException("session");
			}
			if (handler == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "handler");
				throw new ArgumentNullException("handler");
			}
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "id");
				throw new ArgumentNullException("id");
			}
			if ((notificationType & NotificationType.ConnectionDropped) == NotificationType.ConnectionDropped)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Subscription::Create. ConnectionDropped not valid on object notifications.");
				throw new InvalidOperationException("ConnectionDropped not valid on object notifications.");
			}
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			NotificationCallbackMode callbackMode = isSyncCallback ? NotificationCallbackMode.Sync : NotificationCallbackMode.Async;
			return Subscription.InternalCreate(session, handler, notificationType, storeObjectId, callbackMode, passthruCallback);
		}

		public static Subscription CreateMailboxSubscription(StoreSession session, NotificationHandler handler, NotificationType notificationType)
		{
			return Subscription.CreateMailboxSubscription(session, handler, notificationType, false);
		}

		public static Subscription CreateMailboxSubscription(StoreSession session, NotificationHandler handler, NotificationType notificationType, bool passthruCallback)
		{
			EnumValidator.ThrowIfInvalid<NotificationType>(notificationType, "notificationType");
			if (session == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "session");
				throw new ArgumentNullException("session");
			}
			if (handler == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "handler");
				throw new ArgumentNullException("handler");
			}
			if ((notificationType & NotificationType.ConnectionDropped) == NotificationType.ConnectionDropped && notificationType != NotificationType.ConnectionDropped)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Subscription::Create. ConnectionDropped cannot be combined with other types of notification.");
				throw new InvalidOperationException("ConnectionDropped cannot be combined with other types of notification.");
			}
			return Subscription.InternalCreate(session, handler, notificationType, null, NotificationCallbackMode.Sync, passthruCallback);
		}

		public static Subscription Create(IQueryResult queryResult, NotificationHandler handler)
		{
			return Subscription.Create(queryResult, handler, false);
		}

		public static Subscription Create(IQueryResult queryResult, NotificationHandler handler, bool passthruCallback)
		{
			if (queryResult == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "queryResult");
				throw new ArgumentNullException("queryResult");
			}
			if (handler == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Subscription::Create. {0} should not be null.", "handler");
				throw new ArgumentNullException("handler");
			}
			return Subscription.InternalCreate(queryResult, handler, passthruCallback);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Subscription>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void OnNotify(MapiNotification notification)
		{
			Notification state;
			if (this.TryCreateXsoNotification(notification, out state))
			{
				ThreadPool.QueueUserWorkItem(this.waitCallback, state);
			}
		}

		internal bool TryCreateXsoNotification(MapiNotification notification, out Notification xsoNotification)
		{
			xsoNotification = null;
			if (notification.NotificationType == AdviseFlags.TableModified)
			{
				xsoNotification = Subscription.CreateQueryNotification(notification, this.queryResult.Columns);
			}
			else
			{
				xsoNotification = Subscription.CreateNotification(notification);
			}
			return xsoNotification != null;
		}

		internal void InvokeHandler(Notification notification)
		{
			this.handler(notification);
		}

		private static Subscription InternalCreate(StoreSession session, NotificationHandler handler, NotificationType notificationType, StoreObjectId storeObjectId, NotificationCallbackMode callbackMode, bool passthruCallback)
		{
			Subscription.CheckSubscriptionLimit(session);
			AdviseFlags adviseFlags = (AdviseFlags)0;
			if ((notificationType & NotificationType.NewMail) == NotificationType.NewMail)
			{
				adviseFlags |= AdviseFlags.NewMail;
			}
			if ((notificationType & NotificationType.Created) == NotificationType.Created)
			{
				adviseFlags |= AdviseFlags.ObjectCreated;
			}
			if ((notificationType & NotificationType.Deleted) == NotificationType.Deleted)
			{
				adviseFlags |= AdviseFlags.ObjectDeleted;
			}
			if ((notificationType & NotificationType.Modified) == NotificationType.Modified)
			{
				adviseFlags |= AdviseFlags.ObjectModified;
			}
			if ((notificationType & NotificationType.Moved) == NotificationType.Moved)
			{
				adviseFlags |= AdviseFlags.ObjectMoved;
			}
			if ((notificationType & NotificationType.Copied) == NotificationType.Copied)
			{
				adviseFlags |= AdviseFlags.ObjectCopied;
			}
			if ((notificationType & NotificationType.SearchComplete) == NotificationType.SearchComplete)
			{
				adviseFlags |= AdviseFlags.SearchComplete;
			}
			if ((notificationType & NotificationType.ConnectionDropped) == NotificationType.ConnectionDropped)
			{
				adviseFlags |= AdviseFlags.ConnectionDropped;
			}
			return new Subscription(session, storeObjectId, adviseFlags, handler, callbackMode, passthruCallback);
		}

		private static Subscription InternalCreate(IQueryResult queryResult, NotificationHandler handler, bool passthruCallback)
		{
			Subscription.CheckSubscriptionLimit(queryResult.StoreSession);
			return new Subscription(queryResult, handler, passthruCallback);
		}

		private static void CheckSubscriptionLimit(StoreSession session)
		{
			if (session.SubscriptionsManager.SubscriptionCount >= StorageLimits.Instance.NotificationsMaxSubscriptions)
			{
				MailboxSession mailboxSession = session as MailboxSession;
				LocalizedString message;
				if (mailboxSession != null)
				{
					message = ServerStrings.ExTooManySubscriptions(mailboxSession.MailboxOwner.LegacyDn, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerLegacyDn);
				}
				else
				{
					message = ServerStrings.ExTooManySubscriptionsOnPublicStore(session.ServerFullyQualifiedDomainName);
				}
				throw new TooManySubscriptionsException(message);
			}
		}

		private static Notification CreateQueryNotification(MapiNotification notification, ColumnPropertyDefinitions columns)
		{
			MapiTableNotification mapiTableNotification = notification as MapiTableNotification;
			QueryNotificationType eventType;
			switch (mapiTableNotification.TableEvent)
			{
			case TableEvent.TableChanged:
				eventType = QueryNotificationType.QueryResultChanged;
				break;
			case TableEvent.TableError:
				eventType = QueryNotificationType.Error;
				break;
			case TableEvent.TableRowAdded:
				eventType = QueryNotificationType.RowAdded;
				break;
			case TableEvent.TableRowDeleted:
			case TableEvent.TableRowDeletedExtended:
				eventType = QueryNotificationType.RowDeleted;
				break;
			case TableEvent.TableRowModified:
				eventType = QueryNotificationType.RowModified;
				break;
			case TableEvent.TableSortDone:
				eventType = QueryNotificationType.SortDone;
				break;
			case TableEvent.TableRestrictDone:
				eventType = QueryNotificationType.RestrictDone;
				break;
			case TableEvent.TableSetColDone:
				eventType = QueryNotificationType.SetColumnDone;
				break;
			case TableEvent.TableReload:
				eventType = QueryNotificationType.Reload;
				break;
			default:
				return null;
			}
			int hresult = mapiTableNotification.HResult;
			byte[] index = mapiTableNotification.Index.GetBytes() ?? Array<byte>.Empty;
			byte[] prior = mapiTableNotification.Prior.GetBytes() ?? Array<byte>.Empty;
			ICollection<PropertyDefinition> propertyDefinitions;
			object[] row;
			if (mapiTableNotification.Row != null && mapiTableNotification.Row.Length > 0)
			{
				if (mapiTableNotification.TableEvent == TableEvent.TableRowDeletedExtended)
				{
					PropTag[] array = new PropTag[mapiTableNotification.Row.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = mapiTableNotification.Row[i].PropTag;
					}
					propertyDefinitions = Subscription.PropertiesForRowDeletedExtended;
					ICollection<PropertyDefinition> columns2 = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, null, null, array);
					QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(null, columns2);
					queryResultPropertyBag.SetQueryResultRow(mapiTableNotification.Row);
					row = queryResultPropertyBag.GetProperties(propertyDefinitions);
				}
				else
				{
					if (!QueryResult.DoPropertyValuesMatchColumns(columns, mapiTableNotification.Row))
					{
						ExTraceGlobals.StorageTracer.TraceDebug(0L, "Subcription::CreateQueryNotification. The notification data does not match the columns the client knows about. Dropping notification.");
						return null;
					}
					QueryResultPropertyBag queryResultPropertyBag2 = new QueryResultPropertyBag(null, columns.SimplePropertyDefinitions);
					queryResultPropertyBag2.SetQueryResultRow(mapiTableNotification.Row);
					propertyDefinitions = columns.PropertyDefinitions;
					row = queryResultPropertyBag2.GetProperties(columns.PropertyDefinitions);
				}
			}
			else
			{
				propertyDefinitions = Array<UnresolvedPropertyDefinition>.Empty;
				row = Array<object>.Empty;
			}
			return new QueryNotification(eventType, hresult, index, prior, propertyDefinitions, row);
		}

		private static Notification CreateNotification(MapiNotification notification)
		{
			Notification result;
			if (notification.NotificationType == AdviseFlags.NewMail)
			{
				MapiNewMailNotification mapiNewMailNotification = notification as MapiNewMailNotification;
				result = new NewMailNotification(StoreObjectId.FromProviderSpecificId(mapiNewMailNotification.EntryId, ObjectClass.GetObjectType(mapiNewMailNotification.MessageClass)), StoreObjectId.FromProviderSpecificId(mapiNewMailNotification.ParentId, StoreObjectType.Folder), mapiNewMailNotification.MessageClass, (MessageFlags)mapiNewMailNotification.MessageFlags);
			}
			else if (notification.NotificationType == AdviseFlags.SearchComplete)
			{
				result = new ObjectNotification(null, null, null, null, (NotificationObjectType)0, null, NotificationType.SearchComplete);
			}
			else if (notification.NotificationType == AdviseFlags.ConnectionDropped)
			{
				MapiConnectionDroppedNotification mapiConnectionDroppedNotification = notification as MapiConnectionDroppedNotification;
				result = new ConnectionDroppedNotification(mapiConnectionDroppedNotification.ServerDN, mapiConnectionDroppedNotification.UserDN, mapiConnectionDroppedNotification.TickDeath);
			}
			else
			{
				MapiObjectNotification mapiObjectNotification = notification as MapiObjectNotification;
				if (mapiObjectNotification == null)
				{
					throw new InvalidOperationException(ServerStrings.ExNotSupportedNotificationType((uint)notification.NotificationType));
				}
				AdviseFlags notificationType = notification.NotificationType;
				NotificationType type;
				if (notificationType <= AdviseFlags.ObjectDeleted)
				{
					if (notificationType == AdviseFlags.ObjectCreated)
					{
						type = NotificationType.Created;
						goto IL_10A;
					}
					if (notificationType == AdviseFlags.ObjectDeleted)
					{
						type = NotificationType.Deleted;
						goto IL_10A;
					}
				}
				else
				{
					if (notificationType == AdviseFlags.ObjectModified)
					{
						type = NotificationType.Modified;
						goto IL_10A;
					}
					if (notificationType == AdviseFlags.ObjectMoved)
					{
						type = NotificationType.Moved;
						goto IL_10A;
					}
					if (notificationType == AdviseFlags.ObjectCopied)
					{
						type = NotificationType.Copied;
						goto IL_10A;
					}
				}
				throw new InvalidOperationException(ServerStrings.ExNotSupportedNotificationType((uint)notification.NotificationType));
				IL_10A:
				UnresolvedPropertyDefinition[] propertyDefinitions;
				if (mapiObjectNotification.Tags != null)
				{
					propertyDefinitions = PropertyTagCache.UnresolvedPropertyDefinitionsFromPropTags(mapiObjectNotification.Tags);
				}
				else
				{
					propertyDefinitions = Array<UnresolvedPropertyDefinition>.Empty;
				}
				result = new ObjectNotification((mapiObjectNotification.EntryId == null) ? null : StoreObjectId.FromProviderSpecificId(mapiObjectNotification.EntryId, StoreObjectType.Unknown), (mapiObjectNotification.ParentId == null) ? null : StoreObjectId.FromProviderSpecificId(mapiObjectNotification.ParentId, StoreObjectType.Folder), (mapiObjectNotification.OldId == null) ? null : StoreObjectId.FromProviderSpecificId(mapiObjectNotification.OldId, StoreObjectType.Unknown), (mapiObjectNotification.OldParentId == null) ? null : StoreObjectId.FromProviderSpecificId(mapiObjectNotification.OldParentId, StoreObjectType.Folder), (NotificationObjectType)mapiObjectNotification.ObjectType, propertyDefinitions, type);
			}
			return result;
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			lock (this.disposeLock)
			{
				if (this.isDisposed)
				{
					return;
				}
				this.isDisposed = true;
			}
			this.InternalDispose(disposing);
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					if (this.storeSession != null && !this.storeSession.IsDisposed)
					{
						try
						{
							if (!this.passthruCallback)
							{
								this.storeSession.SubscriptionsManager.UnRegisterSubscription(this);
							}
							if (this.adviseId != null && this.notificationSource != null && !this.notificationSource.IsDisposedOrDead)
							{
								this.notificationSource.Unadvise(this.adviseId);
							}
						}
						catch (StoragePermanentException arg)
						{
							ExTraceGlobals.SessionTracer.Information<StoragePermanentException>((long)this.GetHashCode(), "Subscription::InternalDispose. Exception ignored during subscription Dispose, {0}.", arg);
						}
						catch (StorageTransientException arg2)
						{
							ExTraceGlobals.SessionTracer.Information<StorageTransientException>((long)this.GetHashCode(), "Subscription::InternalDispose. Exception ignored during subscription Dispose, {0}.", arg2);
						}
					}
				}
				finally
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
			}
		}

		private void WaitCallbackProc(object obj)
		{
			this.handler((Notification)obj);
		}

		private readonly StoreObjectId itemId;

		private readonly StoreSession storeSession;

		private readonly NotificationHandler handler;

		private readonly DisposeTracker disposeTracker;

		private readonly object disposeLock = new object();

		private readonly SubscriptionSink sink;

		private readonly bool passthruCallback;

		private readonly INotificationSource notificationSource;

		private readonly IQueryResult queryResult;

		private readonly WaitCallback waitCallback;

		private readonly object adviseId;

		private bool isDisposed;

		private static readonly PropertyDefinition[] PropertiesForRowDeletedExtended = new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationId
		};
	}
}
