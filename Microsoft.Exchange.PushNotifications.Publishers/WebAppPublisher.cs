using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WebAppPublisher : PushNotificationPublisher<WebAppNotification, WebAppChannel>
	{
		public WebAppPublisher(WebAppPublisherSettings publisherSettings, List<IPushNotificationMapping<WebAppNotification>> mappings = null) : this(publisherSettings, ExTraceGlobals.WebAppPublisherTracer, mappings)
		{
		}

		public WebAppPublisher(WebAppPublisherSettings publisherSettings, ITracer tracer, List<IPushNotificationMapping<WebAppNotification>> mappings = null) : base(publisherSettings, tracer, null, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private WebAppChannelSettings ChannelSettings { get; set; }

		protected override WebAppChannel CreateNotificationChannel()
		{
			return new WebAppChannel(this.ChannelSettings, base.Tracer, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WebAppPublisher>(this);
		}
	}
}
