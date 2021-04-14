using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WnsPublisher : PushNotificationPublisher<WnsNotification, WnsChannel>
	{
		public WnsPublisher(WnsPublisherSettings publisherSettings, IThrottlingManager throttlingManager, List<IPushNotificationMapping<WnsNotification>> mappings = null) : this(publisherSettings, throttlingManager, ExTraceGlobals.WnsPublisherTracer, mappings)
		{
		}

		public WnsPublisher(WnsPublisherSettings publisherSettings, IThrottlingManager throttlingManager, ITracer tracer, List<IPushNotificationMapping<WnsNotification>> mappings = null) : base(publisherSettings, tracer, throttlingManager, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private WnsChannelSettings ChannelSettings { get; set; }

		protected override WnsChannel CreateNotificationChannel()
		{
			return new WnsChannel(this.ChannelSettings, base.Tracer, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WnsPublisher>(this);
		}
	}
}
