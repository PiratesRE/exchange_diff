using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class AzureChallengeRequestServiceProxy : IAzureChallengeRequestServiceContract, IDisposeTrackable, IDisposable
	{
		public AzureChallengeRequestServiceProxy(PushNotificationsProxyPool<IAzureChallengeRequestServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreateAzureChallengeRequestProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<IAzureChallengeRequestServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginChallengeRequest(AzureChallengeRequestInfo issueSecret, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<IAzureChallengeRequestServiceContract>.Start(this.ProxyPool, delegate(IAzureChallengeRequestServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginChallengeRequest(issueSecret, poolCallback, poolState);
			}, delegate(IAzureChallengeRequestServiceContract proxy, IAsyncResult result)
			{
				proxy.EndChallengeRequest(result);
			}, asyncCallback, asyncState, "IssueRegistrationSecret", 3);
		}

		public void EndChallengeRequest(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AzureChallengeRequestServiceProxy>(this);
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
					PushNotificationsProxyPool<IAzureChallengeRequestServiceContract> proxyPool = this.ProxyPool;
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
