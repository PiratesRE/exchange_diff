using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	public class ShadowRedundancyEventLogger
	{
		internal virtual void LogShadowRedundancyMessagesResubmitted(int resubmittedCount, string serverFqdn, ResubmitReason resubmitReason)
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyMessagesResubmitted, null, new object[]
			{
				resubmittedCount,
				serverFqdn,
				resubmitReason
			});
		}

		internal virtual void LogShadowRedundancyMessageResubmitSuppressed(int messageCount, string serverFqdn, string reason)
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyMessageResubmitSuppressed, null, new object[]
			{
				messageCount,
				serverFqdn,
				reason
			});
		}

		internal virtual void LogPrimaryServerDatabaseStateChanged(string serverFqdn, string oldState, string newState)
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyPrimaryServerDatabaseStateChanged, null, new object[]
			{
				serverFqdn,
				oldState,
				newState
			});
		}

		internal virtual void LogPrimaryServerHeartbeatFailed(string serverFqdn)
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyPrimaryServerHeartbeatFailed, null, new object[]
			{
				serverFqdn
			});
		}

		internal virtual void LogMessageDeferredDueToShadowFailure()
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyMessageDeferredDueToShadowFailure, null, new object[0]);
		}

		internal virtual void LogHeartbeatForcedReset(string primaryServer, TimeSpan cutoffTime)
		{
			ShadowRedundancyEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ShadowRedundancyForcedHeartbeatReset, null, new object[]
			{
				primaryServer,
				cutoffTime
			});
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ShadowRedundancyTracer.Category, TransportEventLog.GetEventSource());
	}
}
