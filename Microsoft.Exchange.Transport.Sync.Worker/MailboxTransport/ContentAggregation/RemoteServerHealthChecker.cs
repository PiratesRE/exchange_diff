using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoteServerHealthChecker : IRemoteServerHealthChecker
	{
		private RemoteServerHealthChecker()
		{
		}

		public static RemoteServerHealthChecker Instance
		{
			get
			{
				return RemoteServerHealthChecker.instance;
			}
		}

		public RemoteServerHealthState GetRemoteServerHealthState(ISyncWorkerData subscription)
		{
			if (subscription.SyncPhase == SyncPhase.Initial || subscription.SyncPhase == SyncPhase.Finalization)
			{
				return RemoteServerHealthState.Clean;
			}
			string incomingServerName = subscription.IncomingServerName;
			bool isPartnerProtocol = subscription.IsPartnerProtocol;
			return AggregationComponent.CalculateRemoteServerHealth(incomingServerName, isPartnerProtocol);
		}

		public bool IsRemoteServerSlow(SyncEngineState syncEngineState, ISyncWorkerData subscription, out RemoteServerTooSlowException exception)
		{
			if (!subscription.IsPartnerProtocol)
			{
				TimeSpan? timeSpan = null;
				if (syncEngineState.CloudProviderState.AverageSuccessfulRoundtripTime >= AggregationConfiguration.Instance.RemoteRoundtripTimeThreshold)
				{
					timeSpan = new TimeSpan?(syncEngineState.CloudProviderState.AverageSuccessfulRoundtripTime);
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1360UL, SyncEngine.Tracer, "Avg Successful Roundtrip time {0} exceeds threshold {1} and marking the sync with transient failure.", new object[]
					{
						timeSpan,
						AggregationConfiguration.Instance.RemoteRoundtripTimeThreshold
					});
				}
				else if (syncEngineState.CloudProviderState.AverageUnsuccessfulRoundtripTime >= AggregationConfiguration.Instance.RemoteRoundtripTimeThreshold)
				{
					timeSpan = new TimeSpan?(syncEngineState.CloudProviderState.AverageUnsuccessfulRoundtripTime);
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1361UL, SyncEngine.Tracer, "Avg Unsuccessful Roundtrip time {0} exceeds threshold {1} and marking the sync with transient failure.", new object[]
					{
						timeSpan,
						AggregationConfiguration.Instance.RemoteRoundtripTimeThreshold
					});
				}
				if (timeSpan != null)
				{
					exception = new RemoteServerTooSlowException(subscription.IncomingServerName, subscription.IncomingServerPort, timeSpan.Value, AggregationConfiguration.Instance.RemoteRoundtripTimeThreshold);
					return true;
				}
			}
			exception = null;
			return false;
		}

		private static readonly RemoteServerHealthChecker instance = new RemoteServerHealthChecker();
	}
}
