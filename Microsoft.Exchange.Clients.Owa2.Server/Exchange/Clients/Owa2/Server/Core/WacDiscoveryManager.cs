using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class WacDiscoveryManager
	{
		private WacDiscoveryManager(Uri wacDiscoveryEndPoint)
		{
			if (wacDiscoveryEndPoint == null)
			{
				this.wacDiscoveryResult = new WacDiscoveryResultFailure(new WacDiscoveryFailureException("The WAC Discovery URL is null."));
				return;
			}
			this.wacDiscoveryResult = new WacDiscoveryResultFailure(new WacDiscoveryFailureException("WAC discovery has not yet succeeded with endpoint " + wacDiscoveryEndPoint));
			this.wacDiscoveryClient = new WacDiscoveryClient(wacDiscoveryEndPoint);
			this.currentFailureRetryTimeSpan = WacConfiguration.Instance.DiscoveryDataRetrievalErrorBaseRefreshInterval;
			this.wacDiscoveryDataRefreshTimer = new Timer(new TimerCallback(this.RefreshWacDiscoveryData));
			this.RefreshWacDiscoveryData(null);
		}

		public static WacDiscoveryManager Instance
		{
			get
			{
				if (WacDiscoveryManager.wacDiscoveryManager == null)
				{
					lock (WacDiscoveryManager.wacDiscoveryManagerConstructorLock)
					{
						if (WacDiscoveryManager.wacDiscoveryManager == null)
						{
							Uri wacDiscoveryEndPoint = WacConfiguration.Instance.WacDiscoveryEndPoint;
							WacDiscoveryManager.wacDiscoveryManager = new WacDiscoveryManager(wacDiscoveryEndPoint);
						}
					}
				}
				return WacDiscoveryManager.wacDiscoveryManager;
			}
		}

		public WacDiscoveryResultBase WacDiscoveryResult
		{
			get
			{
				WacDiscoveryResultBase result;
				lock (this.wacDiscoveryDataLock)
				{
					result = this.wacDiscoveryResult;
				}
				return result;
			}
			private set
			{
				lock (this.wacDiscoveryDataLock)
				{
					this.wacDiscoveryResult = value;
				}
			}
		}

		public Uri WacDiscoveryEndPoint
		{
			get
			{
				if (this.wacDiscoveryClient != null)
				{
					return this.wacDiscoveryClient.WacDiscoveryEndPoint;
				}
				return null;
			}
		}

		private void RefreshWacDiscoveryData(object data)
		{
			WacDiscoveryResultBase wacDiscoveryResultBase = this.wacDiscoveryClient.FetchDiscoveryResults();
			if (wacDiscoveryResultBase != null)
			{
				this.WacDiscoveryResult = wacDiscoveryResultBase;
				this.currentFailureRetryTimeSpan = WacConfiguration.Instance.DiscoveryDataRefreshInterval;
			}
			if (this.WacDiscoveryResult != null)
			{
				this.wacDiscoveryDataRefreshTimer.Change((long)WacConfiguration.Instance.DiscoveryDataRefreshInterval.TotalMilliseconds, -1L);
				return;
			}
			this.currentFailureRetryTimeSpan = this.currentFailureRetryTimeSpan.Add(this.currentFailureRetryTimeSpan);
			if (this.currentFailureRetryTimeSpan > WacConfiguration.Instance.DiscoveryDataRefreshInterval)
			{
				this.currentFailureRetryTimeSpan = WacConfiguration.Instance.DiscoveryDataRefreshInterval;
			}
			this.wacDiscoveryDataRefreshTimer.Change((long)this.currentFailureRetryTimeSpan.TotalMilliseconds, -1L);
		}

		private static readonly object wacDiscoveryManagerConstructorLock = new object();

		private readonly object wacDiscoveryDataLock = new object();

		private readonly WacDiscoveryClient wacDiscoveryClient;

		private readonly Timer wacDiscoveryDataRefreshTimer;

		private static WacDiscoveryManager wacDiscoveryManager;

		private WacDiscoveryResultBase wacDiscoveryResult;

		private TimeSpan currentFailureRetryTimeSpan;
	}
}
