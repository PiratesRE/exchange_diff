using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NodeActionTracker
	{
		private void CheckForNodesJoining(ITaskOutputHelper output, AmServerName nodeName, NodeAction nodeAction, TimeSpan maxWaitDurationForJoining)
		{
			if (maxWaitDurationForJoining == TimeSpan.Zero)
			{
				return;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.NodeActionDelayBetweenIterationsInSec);
			if (maxWaitDurationForJoining < timeSpan)
			{
				timeSpan = maxWaitDurationForJoining;
			}
			bool flag = true;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (;;)
			{
				Dictionary<AmServerName, AmNodeState> allClusterNodeStates = this.GetAllClusterNodeStates(output);
				KeyValuePair<AmServerName, AmNodeState>[] array = (from kv in allClusterNodeStates
				where kv.Value == AmNodeState.Joining
				select kv).ToArray<KeyValuePair<AmServerName, AmNodeState>>();
				if (array.Length == 0)
				{
					break;
				}
				string nodeStatesAsSingleString = this.GetNodeStatesAsSingleString(array);
				if (flag)
				{
					output.AppendLogMessage("Delaying the action '{1}' for node '{0} since some nodes are still in joining state. (Nodes: {2})", new object[]
					{
						nodeName,
						nodeAction,
						nodeStatesAsSingleString
					});
					ReplayCrimsonEvents.ClusterNodeActionBlockedDueToJoiningNodes.Log<AmServerName, NodeAction, string>(nodeName, nodeAction, nodeStatesAsSingleString);
					flag = false;
				}
				if (stopwatch.Elapsed > maxWaitDurationForJoining)
				{
					return;
				}
				Thread.Sleep(timeSpan);
			}
		}

		internal void PerformNodeAction(ITaskOutputHelper output, AmServerName nodeName, NodeAction nodeAction, Action clusterAction)
		{
			TimeSpan maxWaitDurationForJoining = TimeSpan.FromSeconds((double)RegistryParameters.NodeActionNodeStateJoiningWaitDurationInSec);
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.NodeActionInProgressWaitDurationInSec);
			TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)RegistryParameters.NodeActionDelayBetweenIterationsInSec);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			this.CheckForNodesJoining(output, nodeName, nodeAction, maxWaitDurationForJoining);
			TimeSpan elapsed = stopwatch.Elapsed;
			if (timeSpan < timeSpan2)
			{
				timeSpan2 = timeSpan;
			}
			bool flag = true;
			TimeSpan inProgressDuration = TimeSpan.Zero;
			Exception exception = null;
			try
			{
				Stopwatch stopwatch2 = new Stopwatch();
				stopwatch2.Start();
				ExDateTime t = ExDateTime.Now.Add(timeSpan);
				for (;;)
				{
					List<NodeActionTracker.NodeActionInfo> list = new List<NodeActionTracker.NodeActionInfo>(15);
					lock (this.locker)
					{
						ExDateTime now = ExDateTime.Now;
						if (this.nodeActionMap.Count > 0)
						{
							foreach (NodeActionTracker.NodeActionInfo nodeActionInfo in this.nodeActionMap.Values)
							{
								if (now - nodeActionInfo.StartTime < timeSpan)
								{
									list.Add(nodeActionInfo);
								}
							}
						}
						if (list.Count == 0 || now > t)
						{
							NodeActionTracker.NodeActionInfo value = new NodeActionTracker.NodeActionInfo(nodeName, nodeAction);
							this.nodeActionMap[nodeName] = value;
							break;
						}
					}
					string nodeInfoListAsString = this.GetNodeInfoListAsString(list);
					if (flag)
					{
						output.AppendLogMessage("Blocking '{1}' action for node '{0}'. InProgress: {2}", new object[]
						{
							nodeName,
							nodeAction,
							nodeInfoListAsString
						});
						ReplayCrimsonEvents.ClusterNodeActionBlockedDueToInProgress.Log<AmServerName, NodeAction, string>(nodeName, nodeAction, nodeInfoListAsString);
						flag = false;
					}
					Thread.Sleep(timeSpan2);
				}
				inProgressDuration = stopwatch2.Elapsed;
				this.LogStatusStarting(output, nodeName, nodeAction, elapsed, inProgressDuration);
				clusterAction();
			}
			catch (Exception ex)
			{
				exception = ex;
				throw;
			}
			finally
			{
				lock (this.locker)
				{
					this.nodeActionMap.Remove(nodeName);
				}
				this.LogStatusCompleted(output, nodeName, nodeAction, exception);
			}
		}

		private void LogStatusStarting(ITaskOutputHelper output, AmServerName nodeName, NodeAction nodeAction, TimeSpan joiningDuration, TimeSpan inProgressDuration)
		{
			string allNodesStillInList = this.GetAllNodesStillInList(nodeName);
			if (!string.IsNullOrEmpty(allNodesStillInList))
			{
				output.AppendLogMessage("There are stale node action entries that are still present when attemping action '{1}' for node '{0}'. Stale: {2}", new object[]
				{
					nodeName,
					nodeAction,
					allNodesStillInList
				});
			}
			string nodeStatesAsSingleString = this.GetNodeStatesAsSingleString(this.GetAllClusterNodeStates(output));
			output.AppendLogMessage("State of the cluster nodes before performing action '{1}' for node '{0}': {2}", new object[]
			{
				nodeName,
				nodeAction,
				nodeStatesAsSingleString
			});
			ReplayCrimsonEvents.ClusterNodeActionStarted.Log<AmServerName, NodeAction, TimeSpan, TimeSpan, string, string>(nodeName, nodeAction, joiningDuration, inProgressDuration, allNodesStillInList, nodeStatesAsSingleString);
		}

		private void LogStatusCompleted(ITaskOutputHelper output, AmServerName nodeName, NodeAction nodeAction, Exception exception)
		{
			string allNodesStillInList = this.GetAllNodesStillInList(nodeName);
			if (!string.IsNullOrEmpty(allNodesStillInList))
			{
				output.AppendLogMessage("Stale node action entries that are still present after attemping action '{1}' for node '{0}'. Stale: {2}", new object[]
				{
					nodeName,
					nodeAction,
					allNodesStillInList
				});
			}
			string nodeStatesAsSingleString = this.GetNodeStatesAsSingleString(this.GetAllClusterNodeStates(output));
			output.AppendLogMessage("State of the cluster nodes after performing action '{1}' for node '{0}': {2}", new object[]
			{
				nodeName,
				nodeAction,
				nodeStatesAsSingleString
			});
			ReplayCrimsonEvents.ClusterNodeActionCompleted.Log<AmServerName, NodeAction, string, string, string>(nodeName, nodeAction, allNodesStillInList, nodeStatesAsSingleString, (exception != null) ? exception.Message : "<none>");
		}

		protected virtual Dictionary<AmServerName, AmNodeState> GetAllClusterNodeStates(ITaskOutputHelper output)
		{
			Dictionary<AmServerName, AmNodeState> dictionary = new Dictionary<AmServerName, AmNodeState>(15);
			try
			{
				using (IAmCluster amCluster = ClusterFactory.Instance.Open())
				{
					foreach (IAmClusterNode amClusterNode in amCluster.EnumerateNodes())
					{
						using (amClusterNode)
						{
							AmNodeState state = amClusterNode.GetState(false);
							dictionary[amClusterNode.Name] = state;
						}
					}
				}
			}
			catch (ClusterException ex)
			{
				output.AppendLogMessage("GetAllClusterNodeStates() failed with error {0}", new object[]
				{
					ex.Message
				});
			}
			return dictionary;
		}

		private string GetNodeStatesAsSingleString(IEnumerable<KeyValuePair<AmServerName, AmNodeState>> nodeStates)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<AmServerName, AmNodeState> keyValuePair in nodeStates)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat("{0} => {1}", keyValuePair.Key.NetbiosName, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private string GetAllNodesStillInList(AmServerName nodeToExclude)
		{
			string result = string.Empty;
			lock (this.locker)
			{
				if (this.nodeActionMap.Count > 0)
				{
					result = this.GetNodeInfoListAsString(from nodeInfo in this.nodeActionMap.Values
					where !AmServerName.IsEqual(nodeInfo.Name, nodeToExclude)
					select nodeInfo);
				}
			}
			return result;
		}

		private string GetNodeInfoListAsString(IEnumerable<NodeActionTracker.NodeActionInfo> nodeList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (NodeActionTracker.NodeActionInfo nodeActionInfo in nodeList)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(nodeActionInfo.ToString());
			}
			return stringBuilder.ToString();
		}

		private object locker = new object();

		private Dictionary<AmServerName, NodeActionTracker.NodeActionInfo> nodeActionMap = new Dictionary<AmServerName, NodeActionTracker.NodeActionInfo>(15);

		private class NodeActionInfo
		{
			internal NodeActionInfo(AmServerName name, NodeAction actionInProgress) : this(name, actionInProgress, ExDateTime.Now)
			{
			}

			internal NodeActionInfo(AmServerName name, NodeAction actionInProgress, ExDateTime startTime)
			{
				this.Name = name;
				this.ActionInProgress = actionInProgress;
				this.StartTime = startTime;
			}

			internal AmServerName Name { get; set; }

			internal NodeAction ActionInProgress { get; set; }

			internal ExDateTime StartTime { get; set; }

			public override string ToString()
			{
				return string.Format("Node: {0} ActionInProgress: {1} StartTime: {2}", this.Name.NetbiosName, this.ActionInProgress, this.StartTime.ToString("o"));
			}
		}
	}
}
