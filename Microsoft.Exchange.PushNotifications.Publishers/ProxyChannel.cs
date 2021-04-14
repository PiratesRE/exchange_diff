using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Client;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyChannel : PushNotificationChannel<ProxyNotification>
	{
		static ProxyChannel()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in ProxyCounters.AllCounters)
			{
				exPerformanceCounter.Reset();
			}
		}

		public ProxyChannel(ProxyChannelSettings settings, ITracer tracer, OnPremPublisherServiceProxy onPremClient = null, AzureAppConfigDataServiceProxy onPremAppDataClient = null, ProxyErrorTracker errorTracker = null) : base(settings.AppId, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			this.Settings = settings;
			this.LegacyChannel = new ProxyChannelLegacy(settings, tracer, onPremClient, errorTracker);
			this.AppDataChannel = new ProxyChannelAppData(settings, tracer, onPremAppDataClient, errorTracker, null);
		}

		private ProxyChannelSettings Settings { get; set; }

		private ProxyChannelLegacy LegacyChannel { get; set; }

		private ProxyChannelAppData AppDataChannel { get; set; }

		public override void Send(ProxyNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (notification.NotificationBatch != null && notification.NotificationBatch.Count > 0)
			{
				this.LegacyChannel.Send(notification, cancelToken);
				return;
			}
			this.AppDataChannel.Send(notification, cancelToken);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.LegacyChannel.Dispose();
				this.AppDataChannel.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyChannel>(this);
		}
	}
}
