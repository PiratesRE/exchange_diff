using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationHelper : DisposeTrackableBase
	{
		internal NotificationHelper(MapiNotificationHandler handler) : this(handler, false)
		{
		}

		internal NotificationHelper(MapiNotificationHandler handler, bool callbackImmediately)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(31))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string>(11490, 31, (long)this.GetHashCode(), "NotificationHelper.NotificationHelper: this={0}, handler={1}", TraceUtils.MakeHash(this), TraceUtils.MakeHash(handler));
			}
			if (!callbackImmediately)
			{
				this.waitCallback = new WaitCallback(this.WaitCallbackProc);
			}
			this.handlerReference = GCHandle.Alloc(handler, GCHandleType.Weak);
		}

		~NotificationHelper()
		{
			base.Dispose(false);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.handlerReferenceFreeLock)
			{
				this.handlerReference.Free();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationHelper>(this);
		}

		public void OnNotify(MapiNotification notification)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(31))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(15586, 31, (long)this.GetHashCode(), "NotificationHelper.OnNotify params: ulEventType={0}", notification.NotificationType.ToString());
			}
			if (this.waitCallback == null)
			{
				this.CallHandler(notification);
				return;
			}
			if (!this.NotifyOnThreadPool(notification))
			{
				ComponentTrace<MapiNetTags>.Trace<AdviseFlags>(58411, 31, (long)this.GetHashCode(), "NotificationHelper.OnNotify params: ulEventType={0}", notification.NotificationType);
			}
		}

		private void WaitCallbackProc(object obj)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(31))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(9058, 31, (long)this.GetHashCode(), "NotificationHelper.WaitCallbackProc params: object={0}", TraceUtils.MakeHash(obj));
			}
			this.CallHandler((MapiNotification)obj);
		}

		private void CallHandler(MapiNotification notification)
		{
			MapiNotificationHandler mapiNotificationHandler;
			lock (this.handlerReferenceFreeLock)
			{
				mapiNotificationHandler = (this.handlerReference.IsAllocated ? ((MapiNotificationHandler)this.handlerReference.Target) : null);
			}
			if (mapiNotificationHandler != null)
			{
				mapiNotificationHandler(notification);
			}
		}

		public bool NotifyOnThreadPool(MapiNotification notification)
		{
			return ThreadPool.QueueUserWorkItem(this.waitCallback, notification);
		}

		private GCHandle handlerReference;

		private readonly object handlerReferenceFreeLock = new object();

		private readonly WaitCallback waitCallback;
	}
}
