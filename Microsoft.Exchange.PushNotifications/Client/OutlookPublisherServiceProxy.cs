using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class OutlookPublisherServiceProxy : IOutlookPublisherServiceContract, IDisposeTrackable, IDisposable
	{
		public OutlookPublisherServiceProxy(PushNotificationsProxyPool<IOutlookPublisherServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreateOutlookPublisherProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<IOutlookPublisherServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginPublishOutlookNotifications(OutlookNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<IOutlookPublisherServiceContract>.Start(this.ProxyPool, delegate(IOutlookPublisherServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginPublishOutlookNotifications(notifications, poolCallback, poolState);
			}, delegate(IOutlookPublisherServiceContract proxy, IAsyncResult result)
			{
				proxy.EndPublishOutlookNotifications(result);
			}, asyncCallback, asyncState, "PublishNotifications", 3);
		}

		public void EndPublishOutlookNotifications(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<OutlookPublisherServiceProxy>(this);
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
					PushNotificationsProxyPool<IOutlookPublisherServiceContract> proxyPool = this.ProxyPool;
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
