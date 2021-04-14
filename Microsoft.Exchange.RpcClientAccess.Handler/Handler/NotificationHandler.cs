using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationHandler : INotificationHandler
	{
		internal NotificationHandler(ConnectionHandler connectionHandler)
		{
			this.connectionHandler = connectionHandler;
		}

		public bool HasPendingNotifications()
		{
			return this.connectionHandler.ForAnyLogon((Logon logon) => !logon.NotificationQueue.IsEmpty);
		}

		public void CollectNotifications(NotificationCollector collector)
		{
			if (this.connectionHandler.ForAnyLogon((Logon logon) => logon.HasActiveAsyncOperation))
			{
				return;
			}
			this.connectionHandler.ForAnyLogon(delegate(Logon logon)
			{
				ServerObjectHandle notificationHandle;
				byte logonId;
				Encoding string8Encoding;
				Notification notification;
				while (logon.NotificationQueue.Peek(out notificationHandle, out logonId, out string8Encoding, out notification))
				{
					if (!collector.TryAddNotification(notificationHandle, logonId, string8Encoding, notification))
					{
						return true;
					}
					logon.NotificationQueue.Dequeue();
				}
				return false;
			});
		}

		public void RegisterCallback(Action callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback", "Callback cannot be null");
			}
			if (this.HasPendingNotifications())
			{
				ExTraceGlobals.AsyncRpcTracer.TraceDebug<IConnection, bool, Action>(Activity.TraceId, "RegisteredCallback. Invoke callback immediately. Connection = {0}, HasPendingNotifications = {1}, callback = {2}.", this.connectionHandler.Connection, true, callback);
				callback();
				return;
			}
			lock (this.callbackLock)
			{
				this.registeredCallback = callback;
				if (this.HasPendingNotifications())
				{
					ExTraceGlobals.AsyncRpcTracer.TraceDebug<IConnection, bool, Action>(Activity.TraceId, "RegisteredCallback. Invoke callback immediately after registering the callback. Connection = {0}, HasPendingNotifications = {1}, registeredCallback = {2}.", this.connectionHandler.Connection, true, this.registeredCallback);
					this.InvokeCallback();
				}
			}
		}

		public void CancelCallback()
		{
			ExTraceGlobals.AsyncRpcTracer.TraceDebug<IConnection, bool>(Activity.TraceId, "CancelCallback. Connection = {0}, registeredCallback = {1}.", this.connectionHandler.Connection, this.registeredCallback != null);
			lock (this.callbackLock)
			{
				this.registeredCallback = null;
			}
		}

		internal void InvokeCallback()
		{
			lock (this.callbackLock)
			{
				if (this.registeredCallback != null)
				{
					ExTraceGlobals.AsyncRpcTracer.TraceInformation<IConnection, Action>(0, Activity.TraceId, "InvokeCallback. Invoke callback. Connection = {0}, registeredCallback = {1}.", this.connectionHandler.Connection, this.registeredCallback);
					Action action = this.registeredCallback;
					this.registeredCallback = null;
					action();
				}
				else
				{
					ExTraceGlobals.AsyncRpcTracer.TraceDebug<IConnection>(0, Activity.TraceId, "InvokeCallback. Invoke callback but there is NO registered callback. Connection = {0}.", this.connectionHandler.Connection);
				}
			}
		}

		private readonly ConnectionHandler connectionHandler;

		private object callbackLock = new object();

		private Action registeredCallback;
	}
}
