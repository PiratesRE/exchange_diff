using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class LocalUserNotificationPublisherServiceProxy : ILocalUserNotificationPublisherServiceContract, IDisposeTrackable, IDisposable
	{
		public LocalUserNotificationPublisherServiceProxy(PushNotificationsProxyPool<ILocalUserNotificationPublisherServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreateLocalUserNotificationPublisherProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<ILocalUserNotificationPublisherServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginPublishUserNotifications(LocalUserNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<ILocalUserNotificationPublisherServiceContract>.Start(this.ProxyPool, delegate(ILocalUserNotificationPublisherServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginPublishUserNotifications(notifications, poolCallback, poolState);
			}, delegate(ILocalUserNotificationPublisherServiceContract proxy, IAsyncResult result)
			{
				proxy.EndPublishUserNotifications(result);
			}, asyncCallback, asyncState, "PublishNotifications", 3);
		}

		public void EndPublishUserNotifications(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LocalUserNotificationPublisherServiceProxy>(this);
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

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					PushNotificationsProxyPool<ILocalUserNotificationPublisherServiceContract> proxyPool = this.ProxyPool;
					if (proxyPool != null)
					{
						proxyPool.Dispose();
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
				}
				this.ProxyPool = null;
				this.isDisposed = true;
			}
		}

		private DisposeTracker disposeTracker;

		private bool isDisposed;
	}
}
