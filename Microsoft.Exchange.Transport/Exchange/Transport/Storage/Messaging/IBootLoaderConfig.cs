using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IBootLoaderConfig
	{
		bool BootLoaderMessageTrackingEnabled { get; }

		TimeSpan MessageDropTimeout { get; }

		TimeSpan MessageExpirationGracePeriod { get; }

		TimeSpan PoisonMessageRetentionPeriod { get; }

		bool PoisonCountPublishingEnabled { get; }

		int PoisonCountLookbackHours { get; }
	}
}
