using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class ProxyPublisher : PushNotificationPublisher<ProxyNotification, ProxyChannel>
	{
		public ProxyPublisher(ProxyPublisherSettings publisherSettings) : this(publisherSettings, ExTraceGlobals.ProxyPublisherTracer)
		{
		}

		public ProxyPublisher(ProxyPublisherSettings publisherSettings, ITracer tracer)
		{
			List<IPushNotificationMapping<ProxyNotification>> mappings = null;
			base..ctor(publisherSettings, tracer, null, mappings, null, null);
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private ProxyChannelSettings ChannelSettings { get; set; }

		protected override ProxyChannel CreateNotificationChannel()
		{
			return new ProxyChannel(this.ChannelSettings, base.Tracer, null, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyPublisher>(this);
		}
	}
}
