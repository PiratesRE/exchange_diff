using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureHubCreationPublisher : PushNotificationPublisher<AzureHubCreationNotification, AzureHubCreationChannel>
	{
		public AzureHubCreationPublisher(AzureHubCreationPublisherSettings publisherSettings, List<IPushNotificationMapping<AzureHubCreationNotification>> mappings = null) : this(publisherSettings, ExTraceGlobals.AzureHubCreationPublisherTracer, mappings)
		{
		}

		public AzureHubCreationPublisher(AzureHubCreationPublisherSettings publisherSettings, ITracer tracer, List<IPushNotificationMapping<AzureHubCreationNotification>> mappings = null) : base(publisherSettings, tracer, null, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		private AzureHubCreationChannelSettings ChannelSettings { get; set; }

		protected override AzureHubCreationChannel CreateNotificationChannel()
		{
			return new AzureHubCreationChannel(this.ChannelSettings, base.Tracer, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureHubCreationPublisher>(this);
		}
	}
}
