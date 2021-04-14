using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemFailoverOnReplayDownTracker : AmThrottledActionTracker<FailoverData>
	{
		internal AmSystemFailoverOnReplayDownTracker() : base("ServerLevelFailover", 1)
		{
			base.MaxHistorySize = RegistryParameters.OnReplDownMaxAllowedFailoversPerNodeInADay;
			if (base.MaxHistorySize < 1)
			{
				base.MaxHistorySize = 1;
			}
		}

		internal void MarkReplayDown(AmServerName node, bool isForce = false)
		{
			lock (this.locker)
			{
				ExDateTime replayDownFromTime = this.GetReplayDownFromTime(node);
				if (isForce || replayDownFromTime == ExDateTime.MinValue)
				{
					this.replayDownMap[node] = ExDateTime.Now;
				}
			}
		}

		internal void MarkReplayUp(AmServerName node)
		{
			lock (this.locker)
			{
				if (this.replayDownMap.Remove(node))
				{
					this.RemoveTimer(node);
				}
			}
		}

		internal ExDateTime GetReplayDownFromTime(AmServerName node)
		{
			ExDateTime result;
			lock (this.locker)
			{
				ExDateTime minValue;
				if (!this.replayDownMap.TryGetValue(node, out minValue))
				{
					minValue = ExDateTime.MinValue;
				}
				result = minValue;
			}
			return result;
		}

		internal override void Cleanup()
		{
			lock (this.locker)
			{
				this.replayDownMap.Clear();
				this.RemoveAllTimers();
				base.Cleanup();
			}
		}

		internal bool IsFailoverScheduled(AmServerName serverName)
		{
			bool result;
			lock (this.locker)
			{
				TimerComponent timerComponent;
				this.scheduledFailovers.TryGetValue(serverName, out timerComponent);
				result = (timerComponent != null);
			}
			return result;
		}

		internal void ScheduleFailover(AmServerName serverName)
		{
			lock (this.locker)
			{
				if (!this.IsFailoverScheduled(serverName))
				{
					ExDateTime replayDownFromTime = this.GetReplayDownFromTime(serverName);
					if (replayDownFromTime != ExDateTime.MinValue)
					{
						int onReplDownConfirmDurationBeforeFailoverInSecs = RegistryParameters.OnReplDownConfirmDurationBeforeFailoverInSecs;
						AmTrace.Debug("AmSystemFailoverOnReplayDownTracker finds {0} down. Timer scheduled in {1} sec", new object[]
						{
							serverName.NetbiosName,
							onReplDownConfirmDurationBeforeFailoverInSecs
						});
						AmSystemFailoverOnReplayDownTimer amSystemFailoverOnReplayDownTimer = new AmSystemFailoverOnReplayDownTimer(serverName, TimeSpan.FromSeconds((double)onReplDownConfirmDurationBeforeFailoverInSecs));
						this.scheduledFailovers[serverName] = amSystemFailoverOnReplayDownTimer;
						amSystemFailoverOnReplayDownTimer.Start();
					}
				}
			}
		}

		internal void AddFailoverEntry(AmServerName serverName)
		{
			base.AddEntry(serverName, new FailoverData());
		}

		internal AmThrottledActionTracker<FailoverData>.ThrottlingShapshot GetThrottlingSnapshot(AmServerName serverName)
		{
			TimeSpan minDurationBetweenActionsPerNode = TimeSpan.FromSeconds((double)RegistryParameters.OnReplDownDurationBetweenFailoversInSecs);
			TimeSpan maxCheckDurationPerNode = TimeSpan.FromDays(1.0);
			int onReplDownMaxAllowedFailoversPerNodeInADay = RegistryParameters.OnReplDownMaxAllowedFailoversPerNodeInADay;
			TimeSpan minDurationBetweenActionsAcrossDag = TimeSpan.FromSeconds((double)RegistryParameters.OnReplDownDurationBetweenFailoversInSecs);
			TimeSpan maxCheckDurationAcrossDag = TimeSpan.FromDays(1.0);
			int onReplDownMaxAllowedFailoversAcrossDagInADay = RegistryParameters.OnReplDownMaxAllowedFailoversAcrossDagInADay;
			return base.GetThrottlingSnapshot(serverName, minDurationBetweenActionsPerNode, maxCheckDurationPerNode, onReplDownMaxAllowedFailoversPerNodeInADay, minDurationBetweenActionsAcrossDag, maxCheckDurationAcrossDag, onReplDownMaxAllowedFailoversAcrossDagInADay);
		}

		internal void RemoveTimer(AmServerName serverName)
		{
			lock (this.locker)
			{
				TimerComponent state = null;
				if (this.scheduledFailovers.TryGetValue(serverName, out state))
				{
					this.scheduledFailovers[serverName] = null;
					ThreadPool.QueueUserWorkItem(delegate(object timerObject)
					{
						if (timerObject != null)
						{
							((TimerComponent)timerObject).Dispose();
						}
					}, state);
				}
			}
		}

		internal void RemoveAllTimers()
		{
			TimerComponent[] array = null;
			lock (this.locker)
			{
				array = this.scheduledFailovers.Values.ToArray<TimerComponent>();
				this.scheduledFailovers.Clear();
			}
			foreach (TimerComponent timerComponent in array)
			{
				if (timerComponent != null)
				{
					timerComponent.Dispose();
				}
			}
		}

		internal static void RemoveFailoverEntryFromClusterDatabase(string serverName)
		{
			AmThrottledActionTracker<FailoverData>.RemoveEntryFromClusdb(serverName, "ServerLevelFailover");
		}

		private const string ServerLevelFailoverAction = "ServerLevelFailover";

		private readonly object locker = new object();

		private readonly Dictionary<AmServerName, ExDateTime> replayDownMap = new Dictionary<AmServerName, ExDateTime>();

		private readonly Dictionary<AmServerName, TimerComponent> scheduledFailovers = new Dictionary<AmServerName, TimerComponent>();
	}
}
