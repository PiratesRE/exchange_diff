using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class AzureHubCreationServiceProxy : IAzureHubCreationServiceContract, IDisposeTrackable, IDisposable
	{
		public AzureHubCreationServiceProxy(PushNotificationsProxyPool<IAzureHubCreationServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreateAzureHubCreationProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<IAzureHubCreationServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginCreateHub(AzureHubDefinition hubDefinition, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<IAzureHubCreationServiceContract>.Start(this.ProxyPool, delegate(IAzureHubCreationServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginCreateHub(hubDefinition, poolCallback, poolState);
			}, delegate(IAzureHubCreationServiceContract proxy, IAsyncResult result)
			{
				proxy.EndCreateHub(result);
			}, asyncCallback, asyncState, "CreateHub", 3);
		}

		public void EndCreateHub(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AzureHubCreationServiceProxy>(this);
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
					PushNotificationsProxyPool<IAzureHubCreationServiceContract> proxyPool = this.ProxyPool;
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
