using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal interface IShadowRedundancyPerformanceCounters
	{
		long RedundantMessageDiscardEvents { get; }

		bool IsValid(ShadowRedundancyCounterId shadowRedundancyCounterName);

		void IncrementCounter(ShadowRedundancyCounterId shadowRedundancyCounterName);

		void IncrementCounterBy(ShadowRedundancyCounterId shadowRedundancyCounterName, long value);

		void DecrementCounter(ShadowRedundancyCounterId shadowRedundancyCounterName);

		void DecrementCounterBy(ShadowRedundancyCounterId shadowRedundancyCounterName, long value);

		void DelayedAckExpired(long messageCount);

		void DelayedAckDeliveredAfterExpiry(long messageCount);

		void UpdateShadowQueueLength(string hostname, int changeAmount);

		ITimerCounter ShadowSelectionLatencyCounter();

		ITimerCounter ShadowNegotiationLatencyCounter();

		IAverageCounter ShadowSuccessfulNegotiationLatencyCounter();

		ITimerCounter ShadowHeartbeatLatencyCounter(string hostname);

		void ShadowFailure(string hostname);

		void HeartbeatFailure(string hostname);

		void TrackMessageMadeRedundant(bool success);

		void SubmitMessagesFromShadowQueue(string hostname, int count);

		void SmtpTimeout();

		void SmtpClientFailureAfterAccept();

		void MessageShadowed(string shadowServer, bool remote);
	}
}
