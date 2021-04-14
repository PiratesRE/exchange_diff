using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationSink : BaseObject
	{
		private NotificationSink(NotificationQueue notificationQueue, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandleValue, StoreId? folderId, Encoding string8Encoding)
		{
			this.parentQueue = notificationQueue;
			this.notificationHandler = notificationHandler;
			this.returnNotificationHandleValue = returnNotificationHandleValue;
			this.rootFolderId = folderId;
			this.string8Encoding = string8Encoding;
		}

		private NotificationSink(NotificationQueue notificationQueue, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandleValue, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, Encoding string8Encoding) : this(notificationQueue, notificationHandler, returnNotificationHandleValue, new StoreId?(folderId), string8Encoding)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				NotificationType notificationType = this.ToXsoNotificationType(flags, eventFlags, folderId, messageId, out this.folderId, out this.objectId);
				if (notificationType == NotificationType.NewMail && folderId == default(StoreId) && !wantGlobalScope)
				{
					wantGlobalScope = true;
				}
				if (wantGlobalScope && folderId != default(StoreId))
				{
					throw new RopExecutionException("When specifying wantGlobalScope to true, the FID must be zero.", (ErrorCode)2147746050U);
				}
				if (this.NotificationServerObjectId != null)
				{
					this.subscription = Subscription.Create(this.Logon.Session, new NotificationHandler(this.Handle), notificationType, this.NotificationServerObjectId, true, true);
				}
				else
				{
					this.subscription = Subscription.CreateMailboxSubscription(this.Logon.Session, new NotificationHandler(this.Handle), notificationType, true);
				}
				disposeGuard.Success();
			}
		}

		private NotificationSink(NotificationQueue notificationQueue, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandleValue, View view, TableFlags tableFlags, StoreId? folderId, Encoding string8Encoding) : this(notificationQueue, notificationHandler, returnNotificationHandleValue, folderId, string8Encoding)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.view = view;
				if ((byte)(tableFlags & NotificationSink.NonNotificationTableFlags) != 0)
				{
					throw new ArgumentException(string.Format("tableFlags = {0}", tableFlags));
				}
				if ((byte)(tableFlags & TableFlags.NoNotifications) != 0)
				{
					this.subscription = null;
				}
				else
				{
					tableFlags &= ~NotificationSink.NonNotificationTableFlags;
					if ((tableFlags | NotificationSink.NotificationTableFlags) != NotificationSink.NotificationTableFlags)
					{
						throw new RopExecutionException(string.Format("We have flags which has not been processed yet. TableFlags = {0}.", tableFlags), (ErrorCode)2147746050U);
					}
					if (view != null && view.DataSource != null)
					{
						this.subscription = Subscription.Create(view.DataSource.QueryResult, new NotificationHandler(this.Handle), true);
					}
				}
				disposeGuard.Success();
			}
		}

		private StoreObjectId NotificationServerObjectId
		{
			get
			{
				if (this.objectId != null)
				{
					return this.objectId;
				}
				return this.folderId;
			}
		}

		private Logon Logon
		{
			get
			{
				return this.parentQueue.Logon;
			}
		}

		public static NotificationSink CreateObjectNotificationSink(NotificationQueue notificationQueue, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandleValue, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, Encoding string8Encoding)
		{
			return new NotificationSink(notificationQueue, notificationHandler, returnNotificationHandleValue, flags, eventFlags, wantGlobalScope, folderId, messageId, string8Encoding);
		}

		public static NotificationSink CreateQueryNotificationSink(NotificationHandler notificationHandler, NotificationQueue notificationQueue, ServerObjectHandle returnNotificationHandleValue, View view, TableFlags tableFlags, StoreId? folderId, Encoding string8Encoding)
		{
			return new NotificationSink(notificationQueue, notificationHandler, returnNotificationHandleValue, view, tableFlags, folderId, string8Encoding);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationSink>(this);
		}

		protected override void InternalDispose()
		{
			this.parentQueue.UnRegister(this);
			Util.DisposeIfPresent(this.subscription);
			base.InternalDispose();
		}

		private NotificationType ToXsoNotificationType(NotificationFlags flags, NotificationEventFlags eventFlags, StoreId folderId, StoreId messageId, out StoreObjectId xsoFolderId, out StoreObjectId xsoMessageId)
		{
			xsoFolderId = null;
			xsoMessageId = null;
			if (folderId != default(StoreId))
			{
				xsoFolderId = this.Logon.Session.IdConverter.CreateFolderId(folderId);
			}
			if (messageId != default(StoreId))
			{
				xsoMessageId = this.Logon.Session.IdConverter.CreateMessageId(folderId, messageId);
			}
			if (eventFlags != NotificationEventFlags.None)
			{
				Feature.Stubbed(87114, string.Format("Other NotificationEventFlags {0}", flags));
				eventFlags = NotificationEventFlags.None;
			}
			NotificationType notificationType = (NotificationType)0;
			int num = 0;
			foreach (NotificationFlags notificationFlags in NotificationSink.notificationFlags)
			{
				if ((ushort)(flags & notificationFlags) != 0)
				{
					notificationType |= NotificationSink.notificationTypes[num];
					flags &= ~notificationFlags;
				}
				num++;
			}
			if (flags != (NotificationFlags)0)
			{
				throw Feature.NotImplemented(68088, string.Format("Other NotificationFlags {0}", flags));
			}
			return notificationType;
		}

		private void Handle(Notification xsoNotification)
		{
			if (base.IsDisposed)
			{
				return;
			}
			this.Logon.Connection.ExecuteInContext<Notification>(xsoNotification, delegate(Notification innerNotification)
			{
				this.parentQueue.Enqueue(this.returnNotificationHandleValue, this.Logon, this.string8Encoding, innerNotification, this.rootFolderId, this.view);
				if (!this.Logon.HasActiveAsyncOperation && !Activity.IsForeground)
				{
					this.notificationHandler.InvokeCallback();
				}
			});
		}

		public static readonly TableFlags NonNotificationTableFlags = ~TableFlags.NoNotifications;

		public static readonly TableFlags NotificationTableFlags = TableFlags.NoNotifications;

		private static readonly NotificationType[] notificationTypes = new NotificationType[]
		{
			NotificationType.Copied,
			NotificationType.Created,
			NotificationType.Deleted,
			NotificationType.Modified,
			NotificationType.Moved,
			NotificationType.SearchComplete,
			NotificationType.NewMail,
			NotificationType.Query
		};

		private static readonly NotificationFlags[] notificationFlags = new NotificationFlags[]
		{
			NotificationFlags.ObjectCopied,
			NotificationFlags.ObjectCreated,
			NotificationFlags.ObjectDeleted,
			NotificationFlags.ObjectModified,
			NotificationFlags.ObjectMoved,
			NotificationFlags.SearchComplete,
			NotificationFlags.NewMail,
			NotificationFlags.TableModified
		};

		private readonly ServerObjectHandle returnNotificationHandleValue;

		private readonly NotificationQueue parentQueue;

		private readonly NotificationHandler notificationHandler;

		private readonly View view;

		private readonly StoreObjectId folderId;

		private readonly StoreObjectId objectId;

		private readonly Subscription subscription;

		private readonly Encoding string8Encoding;

		private readonly StoreId? rootFolderId;
	}
}
