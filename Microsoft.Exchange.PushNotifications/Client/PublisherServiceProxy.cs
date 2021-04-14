using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class PublisherServiceProxy : IPublisherServiceContract, IDisposeTrackable, IDisposable
	{
		public PublisherServiceProxy(PushNotificationsProxyPool<IPublisherServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreatePublisherProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<IPublisherServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginPublishNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<IPublisherServiceContract>.Start(this.ProxyPool, delegate(IPublisherServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginPublishNotifications(notifications, poolCallback, poolState);
			}, delegate(IPublisherServiceContract proxy, IAsyncResult result)
			{
				proxy.EndPublishNotifications(result);
			}, asyncCallback, asyncState, "PublishNotifications", 3);
		}

		public void EndPublishNotifications(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PublisherServiceProxy>(this);
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
					PushNotificationsProxyPool<IPublisherServiceContract> proxyPool = this.ProxyPool;
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
