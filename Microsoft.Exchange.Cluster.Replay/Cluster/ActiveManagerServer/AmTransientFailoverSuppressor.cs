using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmTransientFailoverSuppressor
	{
		internal Dictionary<AmServerName, AmFailoverEntry> EntryMap { get; set; }

		internal static bool CheckIfMajorityNodesReachable(out int totalServersCount, out int successfulReplyCount)
		{
			return AmTransientFailoverSuppressor.CheckIfMajorityNodesReachable(null, out totalServersCount, out successfulReplyCount);
		}

		internal static bool CheckIfMajorityNodesReachable(AmServerName[] members, out int totalServersCount, out int successfulReplyCount)
		{
			bool flag = false;
			totalServersCount = 0;
			successfulReplyCount = 0;
			if (members == null)
			{
				AmLastKnownGoodConfig lastKnownGoodConfig = AmSystemManager.Instance.LastKnownGoodConfig;
				if (lastKnownGoodConfig != null && (lastKnownGoodConfig.Role == AmRole.PAM || lastKnownGoodConfig.Role == AmRole.SAM))
				{
					members = lastKnownGoodConfig.Members;
				}
			}
			if (members != null && members.Length > 0)
			{
				totalServersCount = members.Length;
				Stopwatch stopwatch = new Stopwatch();
				try
				{
					stopwatch.Start();
					AmMultiNodeRoleFetcher amMultiNodeRoleFetcher = new AmMultiNodeRoleFetcher(members.ToList<AmServerName>(), TimeSpan.FromSeconds((double)RegistryParameters.MajorityDecisionRpcTimeoutInSec), true);
					amMultiNodeRoleFetcher.Run();
					return amMultiNodeRoleFetcher.IsMajoritySuccessfulRepliesReceived(out totalServersCount, out successfulReplyCount);
				}
				finally
				{
					ReplayCrimsonEvents.MajorityNodeCheckCompleted.Log<TimeSpan, bool, int, int>(stopwatch.Elapsed, flag, totalServersCount, successfulReplyCount);
				}
			}
			ReplayCrimsonEvents.MajorityNodeNotAttemptedSinceNoMembersAvailable.Log();
			return flag;
		}

		internal AmTransientFailoverSuppressor()
		{
			this.EntryMap = new Dictionary<AmServerName, AmFailoverEntry>();
		}

		internal void Initialize()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsPAM)
			{
				lock (this.m_locker)
				{
					AmServerName[] memberServers = config.DagConfig.MemberServers;
					foreach (AmServerName amServerName in memberServers)
					{
						AmFailoverEntry amFailoverEntry = AmFailoverEntry.ReadFromPersistentStoreBestEffort(amServerName);
						if (amFailoverEntry != null)
						{
							if (!config.DagConfig.IsNodePubliclyUp(amServerName))
							{
								this.AddEntry(amFailoverEntry);
							}
							else
							{
								AmTrace.Debug("Skipped adding server to deferred failover. Removing from persistent store (server: {0})", new object[]
								{
									amServerName
								});
								amFailoverEntry.DeleteFromPersistentStoreBestEffort();
							}
						}
					}
				}
			}
		}

		internal List<AmDeferredRecoveryEntry> GetEntriesForTask()
		{
			List<AmDeferredRecoveryEntry> list = new List<AmDeferredRecoveryEntry>();
			lock (this.m_locker)
			{
				Dictionary<AmServerName, AmFailoverEntry> entryMap = AmSystemManager.Instance.TransientFailoverSuppressor.EntryMap;
				foreach (KeyValuePair<AmServerName, AmFailoverEntry> keyValuePair in entryMap)
				{
					AmFailoverEntry value = keyValuePair.Value;
					string recoveryDueTimeInUtc = (value.TimeCreated + value.Delay).ToString("o");
					string fqdn = value.ServerName.Fqdn;
					AmDeferredRecoveryEntry item = new AmDeferredRecoveryEntry(fqdn, recoveryDueTimeInUtc);
					list.Add(item);
				}
			}
			return list;
		}

		internal bool IsEntryExist(AmServerName serverName)
		{
			bool result = false;
			lock (this.m_locker)
			{
				AmFailoverEntry amFailoverEntry = null;
				result = this.EntryMap.TryGetValue(serverName, out amFailoverEntry);
			}
			return result;
		}

		internal int EntriesCount
		{
			get
			{
				int count;
				lock (this.m_locker)
				{
					count = this.EntryMap.Count;
				}
				return count;
			}
		}

		internal void AddEntry(AmDbActionReason reasonCode, AmServerName serverName)
		{
			if (AmSystemManager.Instance.Config.IsPAM)
			{
				this.AddEntry(new AmFailoverEntry(reasonCode, serverName)
				{
					TimeCreated = ExDateTime.Now,
					Delay = TimeSpan.FromSeconds((double)RegistryParameters.TransientFailoverSuppressionDelayInSec)
				});
			}
		}

		private bool AddEntry(AmFailoverEntry failoverEntry)
		{
			bool result = false;
			if (AmSystemManager.Instance.Config.IsPAM)
			{
				failoverEntry.Timer = new Timer(new TimerCallback(this.InitiateFailoverIfRequired), failoverEntry.ServerName, TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
				lock (this.m_locker)
				{
					if (!this.IsEntryExist(failoverEntry.ServerName))
					{
						result = true;
						ReplayCrimsonEvents.AddedDelayedFailoverEntry.Log<AmServerName, AmDbActionReason, ExDateTime, TimeSpan>(failoverEntry.ServerName, failoverEntry.ReasonCode, failoverEntry.TimeCreated, failoverEntry.Delay);
						this.EntryMap[failoverEntry.ServerName] = failoverEntry;
						failoverEntry.WriteToPersistentStoreBestEffort();
						failoverEntry.Timer.Change(failoverEntry.Delay, TimeSpan.FromMilliseconds(-1.0));
					}
					else
					{
						ReplayCrimsonEvents.EntryAlredayExistForDelayedFailover.Log<AmServerName, ExDateTime>(failoverEntry.ServerName, failoverEntry.TimeCreated + failoverEntry.Delay);
					}
				}
			}
			return result;
		}

		private bool RemoveEntryInternal(AmServerName serverName, bool isRemoveFromClusdb)
		{
			bool result = false;
			AmFailoverEntry amFailoverEntry = null;
			if (this.EntryMap.TryGetValue(serverName, out amFailoverEntry))
			{
				result = true;
				this.EntryMap.Remove(serverName);
				if (amFailoverEntry.Timer != null)
				{
					amFailoverEntry.Timer.Dispose();
				}
				if (isRemoveFromClusdb)
				{
					amFailoverEntry.DeleteFromPersistentStoreBestEffort();
				}
			}
			return result;
		}

		internal bool RemoveEntry(AmServerName serverName, bool isRemoveFromClusdb, string hint)
		{
			bool flag = false;
			lock (this.m_locker)
			{
				flag = this.RemoveEntryInternal(serverName, isRemoveFromClusdb);
			}
			if (flag)
			{
				ReplayCrimsonEvents.RemovingDelayedFailoverEntry.Log<AmServerName, string>(serverName, hint);
			}
			return flag;
		}

		internal void RemoveAllEntries(bool isRemoveFromClusdb)
		{
			lock (this.m_locker)
			{
				List<AmServerName> list = this.EntryMap.Keys.ToList<AmServerName>();
				list.ForEach(delegate(AmServerName serverName)
				{
					this.RemoveEntryInternal(serverName, isRemoveFromClusdb);
				});
			}
		}

		internal void InitiateFailoverIfRequired(object stateInfo)
		{
			AmServerName amServerName = (AmServerName)stateInfo;
			lock (this.m_locker)
			{
				if (AmSystemManager.Instance.Config.IsPAM)
				{
					AmFailoverEntry amFailoverEntry = null;
					if (this.EntryMap.TryGetValue(amServerName, out amFailoverEntry))
					{
						AmNodeState nodeState = AmSystemManager.Instance.Config.DagConfig.GetNodeState(amFailoverEntry.ServerName);
						if (nodeState != AmNodeState.Up)
						{
							AmEvtNodeDownForLongTime amEvtNodeDownForLongTime = new AmEvtNodeDownForLongTime(amFailoverEntry.ServerName);
							amEvtNodeDownForLongTime.Notify();
						}
						else
						{
							ReplayCrimsonEvents.DelayedFailoverSkippedSinceNodeIsUp.Log<AmServerName>(amServerName);
						}
					}
					this.RemoveEntryInternal(amServerName, true);
				}
			}
		}

		internal bool AdminRequestedForRemoval(AmServerName serverName, string hint)
		{
			return this.RemoveEntry(serverName, true, hint);
		}

		private object m_locker = new object();
	}
}
