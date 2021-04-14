using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureDeviceRegistrationPublisher : PushNotificationPublisher<AzureDeviceRegistrationNotification, AzureDeviceRegistrationChannel>
	{
		public AzureDeviceRegistrationPublisher(AzureDeviceRegistrationPublisherSettings publisherSettings, List<IPushNotificationMapping<AzureDeviceRegistrationNotification>> mappings = null) : this(publisherSettings, ExTraceGlobals.AzureDeviceRegistrationPublisherTracer, mappings)
		{
		}

		public AzureDeviceRegistrationPublisher(AzureDeviceRegistrationPublisherSettings publisherSettings, ITracer tracer, List<IPushNotificationMapping<AzureDeviceRegistrationNotification>> mappings = null) : base(publisherSettings, tracer, null, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		public event EventHandler<MissingHubEventArgs> MissingHubDetected;

		private AzureDeviceRegistrationChannelSettings ChannelSettings { get; set; }

		protected override AzureDeviceRegistrationChannel CreateNotificationChannel()
		{
			return new AzureDeviceRegistrationChannel(this.ChannelSettings, base.Tracer, this.MissingHubDetected, null, null, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureDeviceRegistrationPublisher>(this);
		}
	}
}
