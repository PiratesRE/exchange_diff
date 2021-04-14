using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class AzureDeviceRegistrationServiceProxy : IAzureDeviceRegistrationServiceContract, IDisposeTrackable, IDisposable
	{
		public AzureDeviceRegistrationServiceProxy(PushNotificationsProxyPool<IAzureDeviceRegistrationServiceContract> proxyPool = null)
		{
			this.ProxyPool = (proxyPool ?? PushNotificationsProxyPoolFactory.CreateAzureDeviceRegistrationProxyPool());
			this.disposeTracker = this.GetDisposeTracker();
			this.isDisposed = false;
		}

		private PushNotificationsProxyPool<IAzureDeviceRegistrationServiceContract> ProxyPool { get; set; }

		public IAsyncResult BeginDeviceRegistration(AzureDeviceRegistrationInfo hubDefinition, AsyncCallback asyncCallback, object asyncState)
		{
			return RetriableAsyncOperation<IAzureDeviceRegistrationServiceContract>.Start(this.ProxyPool, delegate(IAzureDeviceRegistrationServiceContract proxy, AsyncCallback poolCallback, object poolState)
			{
				proxy.BeginDeviceRegistration(hubDefinition, poolCallback, poolState);
			}, delegate(IAzureDeviceRegistrationServiceContract proxy, IAsyncResult result)
			{
				proxy.EndDeviceRegistration(result);
			}, asyncCallback, asyncState, "RegisterDevice", 3);
		}

		public void EndDeviceRegistration(IAsyncResult result)
		{
			BasicAsyncResult basicAsyncResult = result as BasicAsyncResult;
			ArgumentValidator.ThrowIfNull("result should be a BasicAsyncResult instance", basicAsyncResult);
			basicAsyncResult.End();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AzureDeviceRegistrationServiceProxy>(this);
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
					PushNotificationsProxyPool<IAzureDeviceRegistrationServiceContract> proxyPool = this.ProxyPool;
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
