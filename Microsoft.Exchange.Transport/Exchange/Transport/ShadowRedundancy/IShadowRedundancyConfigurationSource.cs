using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal interface IShadowRedundancyConfigurationSource
	{
		bool Enabled { get; }

		ShadowRedundancyCompatibilityVersion CompatibilityVersion { get; }

		TimeSpan ShadowMessageAutoDiscardInterval { get; }

		TimeSpan DiscardEventExpireInterval { get; }

		TimeSpan QueueMaxIdleTimeInterval { get; }

		TimeSpan ShadowServerInfoMaxIdleTimeInterval { get; }

		TimeSpan ShadowQueueCheckExpiryInterval { get; }

		TimeSpan DelayedAckCheckExpiryInterval { get; }

		bool DelayedAckSkippingEnabled { get; }

		int DelayedAckSkippingQueueLength { get; }

		TimeSpan DiscardEventsCheckExpiryInterval { get; }

		TimeSpan StringPoolCleanupInterval { get; }

		TimeSpan PrimaryServerInfoCleanupInterval { get; }

		int PrimaryServerInfoHardCleanupThreshold { get; }

		TimeSpan HeartbeatFrequency { get; }

		int HeartbeatRetryCount { get; }

		int MaxRemoteShadowAttempts { get; }

		int MaxLocalShadowAttempts { get; }

		ShadowMessagePreference ShadowMessagePreference { get; }

		bool RejectMessageOnShadowFailure { get; }

		int MaxDiscardIdsPerSmtpCommand { get; }

		TimeSpan MaxPendingHeartbeatInterval { get; }

		void Load();

		void Unload();

		void SetShadowRedundancyConfigChangeNotification(ShadowRedundancyConfigChange shadowRedundancyConfigChange);
	}
}
