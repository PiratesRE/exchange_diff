using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyHeartbeatHelper
	{
		public ShadowRedundancyHeartbeatHelper(NextHopSolutionKey key, IShadowRedundancyConfigurationSource configuration, ShadowRedundancyEventLogger eventLogger)
		{
			if (key.NextHopType != NextHopType.ShadowRedundancy)
			{
				throw new ArgumentException("key is not for Shadow queue");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (eventLogger == null)
			{
				throw new ArgumentNullException("eventLogger");
			}
			this.key = key;
			this.configuration = configuration;
			this.eventLogger = eventLogger;
		}

		public bool HasHeartbeatFailure
		{
			get
			{
				return this.heartbeatRetryCount != 0;
			}
		}

		public DateTime LastHeartbeatTime
		{
			get
			{
				return this.lastHeartbeatTime;
			}
		}

		public void ScheduleImmediateHeartbeat()
		{
			lock (this.syncHeartbeat)
			{
				this.heartbeatImmediately = true;
				this.CreateHeartbeatIfNecessary();
			}
		}

		public void CreateHeartbeatIfNecessary()
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Entering ShadowRedundancyHeartbeatHelper.CreateHeartbeatIfNecessary() for queue {0}", this.key);
			if (this.heartbeatTmi != null && DateTime.UtcNow - this.lastHeartbeatTime >= this.configuration.MaxPendingHeartbeatInterval)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, TimeSpan>((long)this.GetHashCode(), "Forced reset of heartbeat state for queue {0} after {1} has elapsed.", this.key, this.configuration.MaxPendingHeartbeatInterval);
				this.eventLogger.LogHeartbeatForcedReset(this.key.NextHopDomain, this.configuration.MaxPendingHeartbeatInterval);
				this.ResetHeartbeatState(this.lastHeartbeatTime);
			}
			else if (this.heartbeatTmi != null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Bailing from ShadowRedundancyHeartbeatHelper.CreateHeartbeatIfNecessary() for queue {0} because we have a TMI already", this.key);
				return;
			}
			TransportMailItem transportMailItem = null;
			lock (this.syncHeartbeat)
			{
				if (this.heartbeatTmi == null)
				{
					bool flag2;
					bool flag3;
					this.CanSendHeartbeat(out flag2, out flag3);
					if (!flag2)
					{
						return;
					}
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Creating heartbeat mailitem for queue {0}", this.key);
				this.heartbeatRetryCount = 0;
				transportMailItem = TransportMailItem.NewMailItem(LatencyComponent.Heartbeat);
				this.heartbeatTmi = transportMailItem;
				this.heartbeatImmediately = false;
			}
			transportMailItem.From = RoutingAddress.NullReversePath;
			transportMailItem.Recipients.Add(RoutingAddress.NullReversePath.ToString());
			NextHopSolutionKey nextHop = new NextHopSolutionKey(NextHopType.Heartbeat, this.key.NextHopDomain, this.key.NextHopConnector);
			transportMailItem.Recipients[0].NextHop = nextHop;
			transportMailItem.UpdateNextHopSolutionTable(nextHop, transportMailItem.Recipients[0]);
			transportMailItem.CommitLazy();
			Components.RemoteDeliveryComponent.QueueMessageForNextHop(transportMailItem);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat initiated for shadow queue '{0}'", this.key);
		}

		public void UpdateHeartbeat(DateTime heartbeatTime, NextHopSolutionKey key, bool successfulHeartbeat)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "UpdateHeartbeat called for queue {0} heartbeatTime={1} key={2}, success={3}", new object[]
			{
				this.key,
				heartbeatTime,
				key,
				successfulHeartbeat
			});
			if (successfulHeartbeat || Components.RemoteDeliveryComponent.IsPaused)
			{
				if (successfulHeartbeat)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat state cleared for shadow queue '{0}' due to successful heartbeat", this.key);
				}
				else
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat state cleared for shadow queue '{0}' due to Remote Delivery pause", this.key);
				}
				lock (this.syncHeartbeat)
				{
					this.ResetHeartbeatState(heartbeatTime);
					return;
				}
			}
			if (key.NextHopType == NextHopType.Heartbeat && key.NextHopConnector == this.key.NextHopConnector)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceError<NextHopSolutionKey>((long)this.GetHashCode(), "Heartbeat could not be detected for shadow queue '{0}'", this.key);
				lock (this.syncHeartbeat)
				{
					this.heartbeatRetryCount++;
					this.lastHeartbeatTime = heartbeatTime;
					this.heartbeatInProgress = false;
					this.StopHeartbeatTimer();
				}
			}
		}

		public void EvaluateHeartbeatAttempt(out bool sendHeartbeat, out bool abortHeartbeat)
		{
			lock (this.syncHeartbeat)
			{
				this.CanSendHeartbeat(out sendHeartbeat, out abortHeartbeat);
				if (sendHeartbeat)
				{
					this.lastHeartbeatTime = DateTime.UtcNow;
					this.heartbeatInProgress = true;
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "EvaluateHeartbeatAttempt: setting heartbeat in progress for queue {0}", this.key);
					this.StartHeartbeatTimer();
				}
				else if (abortHeartbeat)
				{
					this.ResetHeartbeatState(DateTime.UtcNow);
				}
			}
		}

		public bool CanResubmit()
		{
			return this.heartbeatRetryCount >= this.configuration.HeartbeatRetryCount;
		}

		public void ResetHeartbeat()
		{
			lock (this.syncHeartbeat)
			{
				this.ResetHeartbeatState(DateTime.UtcNow);
			}
		}

		public void NotifyConfigUpdated(IShadowRedundancyConfigurationSource oldConfiguration)
		{
			if (oldConfiguration == null)
			{
				throw new ArgumentNullException("oldConfiguration");
			}
			lock (this.syncHeartbeat)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>((long)this.GetHashCode(), "NotifyConfigUpdated() called for queue {0}", this.key);
				this.heartbeatRetryCount = 0;
			}
		}

		private void CanSendHeartbeat(out bool sendHeartbeat, out bool abortHeartbeat)
		{
			abortHeartbeat = Components.RemoteDeliveryComponent.IsPaused;
			sendHeartbeat = (!abortHeartbeat && !this.heartbeatInProgress && (DateTime.UtcNow - this.lastHeartbeatTime >= this.configuration.HeartbeatFrequency || this.heartbeatImmediately));
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "CanSendHeartbeat() called for queue {0}: Send={1} Abort={2} RemoteDelivery.IsPaused={3} heartbeatInProgress={4} lastHeartbeatTime={5} interval={6} heartbeatImmediately={7}", new object[]
			{
				this.key,
				sendHeartbeat,
				abortHeartbeat,
				Components.RemoteDeliveryComponent.IsPaused,
				this.heartbeatInProgress,
				this.lastHeartbeatTime,
				this.configuration.HeartbeatFrequency,
				this.heartbeatImmediately
			});
		}

		private void ResetHeartbeatState(DateTime heartbeatTime)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, DateTime>((long)this.GetHashCode(), "ResetHeartbeatState: resetting heartbeat state for queue {0} at {1}", this.key, heartbeatTime);
			this.heartbeatRetryCount = 0;
			this.heartbeatTmi = null;
			this.lastHeartbeatTime = heartbeatTime;
			this.heartbeatInProgress = false;
			this.StopHeartbeatTimer();
		}

		private void StartHeartbeatTimer()
		{
			this.heartbeatLatencyTimer = ShadowRedundancyManager.PerfCounters.ShadowHeartbeatLatencyCounter(this.key.NextHopDomain);
			this.heartbeatLatencyTimer.Start();
		}

		private void StopHeartbeatTimer()
		{
			if (this.heartbeatLatencyTimer != null)
			{
				this.heartbeatLatencyTimer.Stop();
				this.heartbeatLatencyTimer = null;
				return;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "StopHeartbeatTimer called with null latency timer");
		}

		private readonly IShadowRedundancyConfigurationSource configuration;

		private readonly NextHopSolutionKey key;

		private readonly ShadowRedundancyEventLogger eventLogger;

		private DateTime lastHeartbeatTime = DateTime.UtcNow;

		private int heartbeatRetryCount;

		private TransportMailItem heartbeatTmi;

		private bool heartbeatInProgress;

		private bool heartbeatImmediately;

		private ITimerCounter heartbeatLatencyTimer;

		private object syncHeartbeat = new object();
	}
}
