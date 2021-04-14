using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureChallengeRequestPublisher : PushNotificationPublisher<AzureChallengeRequestNotification, AzureChallengeRequestChannel>
	{
		public AzureChallengeRequestPublisher(AzureChallengeRequestPublisherSettings publisherSettings, List<IPushNotificationMapping<AzureChallengeRequestNotification>> mappings = null) : this(publisherSettings, ExTraceGlobals.AzureChallengeRequestPublisherTracer, mappings)
		{
		}

		public AzureChallengeRequestPublisher(AzureChallengeRequestPublisherSettings publisherSettings, ITracer tracer, List<IPushNotificationMapping<AzureChallengeRequestNotification>> mappings = null) : base(publisherSettings, tracer, null, mappings, null, null)
		{
			this.ChannelSettings = publisherSettings.ChannelSettings;
		}

		public event EventHandler<MissingHubEventArgs> MissingHubDetected;

		private AzureChallengeRequestChannelSettings ChannelSettings { get; set; }

		protected override AzureChallengeRequestChannel CreateNotificationChannel()
		{
			return new AzureChallengeRequestChannel(this.ChannelSettings, base.Tracer, this.MissingHubDetected, null, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureChallengeRequestPublisher>(this);
		}
	}
}
