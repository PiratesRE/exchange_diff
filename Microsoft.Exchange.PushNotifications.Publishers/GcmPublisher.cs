using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class GcmPublisher : PushNotificationPublisher<GcmNotification, GcmChannel>
	{
		public GcmPublisher(GcmPublisherSettings publisherSettings, IThrottlingManager throttlingManager, List<IPushNotificationMapping<GcmNotification>> mappings = null) : this(publisherSettings, throttlingManager, ExTraceGlobals.GcmPublisherTracer, mappings)
		{
		}

		public GcmPublisher(GcmPublisherSettings publisherSettings, IThrottlingManager throttlingManager, ITracer tracer, List<IPushNotificationMapping<GcmNotification>> mappings = null) : base(publisherSettings, tracer, throttlingManager, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private GcmChannelSettings ChannelSettings { get; set; }

		protected override GcmChannel CreateNotificationChannel()
		{
			return new GcmChannel(this.ChannelSettings, base.Tracer, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<GcmPublisher>(this);
		}
	}
}
