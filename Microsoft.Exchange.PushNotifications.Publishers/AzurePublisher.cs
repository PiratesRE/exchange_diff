using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzurePublisher : PushNotificationPublisher<AzureNotification, AzureChannel>
	{
		public AzurePublisher(AzurePublisherSettings publisherSettings, List<IPushNotificationMapping<AzureNotification>> mappings = null) : this(publisherSettings, ExTraceGlobals.AzurePublisherTracer, mappings)
		{
		}

		public AzurePublisher(AzurePublisherSettings publisherSettings, ITracer tracer, List<IPushNotificationMapping<AzureNotification>> mappings = null) : base(publisherSettings, tracer, null, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private AzureChannelSettings ChannelSettings { get; set; }

		protected override AzureChannel CreateNotificationChannel()
		{
			return new AzureChannel(this.ChannelSettings, base.Tracer, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzurePublisher>(this);
		}
	}
}
