using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmThrottledActionTracker<TData> where TData : class, IActionData, new()
	{
		internal string ActionName { get; private set; }

		internal int MaxHistorySize { get; set; }

		internal AmThrottledActionTracker(string actionName, int maxHistorySize = 1)
		{
			this.ActionName = actionName;
			this.MaxHistorySize = maxHistorySize;
		}

		internal static string ConstructRegKeyName(string serverName)
		{
			return string.Format("ExchangeActiveManager\\SystemState\\{0}\\Throttling", serverName);
		}

		internal void InitializeFromClusdb()
		{
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				lock (this.locker)
				{
					this.InitializeFromClusdbInternal();
				}
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.FailoverOnReplDownClusdbInitializeFailed.Log<string, int, string>(this.ActionName, this.MaxHistorySize, (ex != null) ? ex.Message : "<null>");
			}
		}

		private void InitializeFromClusdbInternal()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsPAM)
			{
				AmDagConfig dagConfig = config.DagConfig;
				using (IClusterDB clusterDB = ClusterDB.Open())
				{
					foreach (AmServerName amServerName in dagConfig.MemberServers)
					{
						string keyName = AmThrottledActionTracker<TData>.ConstructRegKeyName(amServerName.NetbiosName);
						string[] value = clusterDB.GetValue<string[]>(keyName, this.ActionName, null);
						if (value != null && value.Length > 0)
						{
							LinkedList<TData> linkedList = new LinkedList<TData>();
							foreach (string text in value)
							{
								int num = text.IndexOf('=');
								if (num != -1)
								{
									string s = text.Substring(0, num);
									string dataStr = null;
									if (num < text.Length - 1)
									{
										dataStr = text.Substring(num + 1);
									}
									ExDateTime actionTime = ExDateTime.Parse(s);
									TData value2 = Activator.CreateInstance<TData>();
									value2.Initialize(actionTime, dataStr);
									linkedList.AddFirst(value2);
								}
							}
							this.actionHistory[amServerName] = linkedList;
						}
					}
				}
			}
		}

		internal AmThrottledActionTracker<TData>.ThrottlingShapshot GetThrottlingSnapshot(AmServerName server, TimeSpan minDurationBetweenActionsPerNode, TimeSpan maxCheckDurationPerNode, int maxAllowedActionsPerNode, TimeSpan minDurationBetweenActionsAcrossDag, TimeSpan maxCheckDurationAcrossDag, int maxAllowedActionsAcrossDag)
		{
			AmThrottledActionTracker<TData>.ThrottlingShapshot throttlingShapshot = new AmThrottledActionTracker<TData>.ThrottlingShapshot(server, minDurationBetweenActionsPerNode, maxCheckDurationPerNode, maxAllowedActionsPerNode, minDurationBetweenActionsAcrossDag, maxCheckDurationAcrossDag, maxAllowedActionsAcrossDag);
			ExDateTime now = ExDateTime.Now;
			throttlingShapshot.CalculationBaseTime = now;
			lock (this.locker)
			{
				throttlingShapshot.MostRecentActionDataForNode = this.GetMostRecentActionData(server);
				throttlingShapshot.MostRecentActionDataAcrossDag = this.GetMostRecentActionDataAcrossDag();
				throttlingShapshot.ActionsCountPerNode = this.GetActionsCountForTimeSpan(server, maxCheckDurationPerNode, now);
				throttlingShapshot.ActionsCountAcrossDag = this.GetActionsForTimeSpanAcrossDag(maxCheckDurationAcrossDag, now);
			}
			throttlingShapshot.IsActionCalledTooSoonPerNode = false;
			if (minDurationBetweenActionsPerNode > TimeSpan.Zero)
			{
				throttlingShapshot.IsActionCalledTooSoonPerNode = this.IsActionCalledTooSoon(throttlingShapshot.MostRecentActionDataForNode, minDurationBetweenActionsPerNode, now);
			}
			throttlingShapshot.IsActionCalledTooSoonAcrossDag = false;
			if (minDurationBetweenActionsAcrossDag > TimeSpan.Zero)
			{
				throttlingShapshot.IsActionCalledTooSoonAcrossDag = this.IsActionCalledTooSoon(throttlingShapshot.MostRecentActionDataAcrossDag, minDurationBetweenActionsAcrossDag, now);
			}
			throttlingShapshot.IsMaxActionsPerNodeExceeded = false;
			if (maxAllowedActionsPerNode > 0 && throttlingShapshot.ActionsCountPerNode >= maxAllowedActionsPerNode)
			{
				throttlingShapshot.IsMaxActionsPerNodeExceeded = true;
			}
			throttlingShapshot.IsMaxActionsAcrossDagExceeded = false;
			if (maxAllowedActionsAcrossDag > 0 && throttlingShapshot.ActionsCountAcrossDag >= maxAllowedActionsAcrossDag)
			{
				throttlingShapshot.IsMaxActionsAcrossDagExceeded = true;
			}
			return throttlingShapshot;
		}

		internal bool IsActionCalledTooSoon(TData actionData, TimeSpan duration, ExDateTime now)
		{
			bool result = false;
			ExDateTime t = now - duration;
			if (actionData != null && actionData.Time != ExDateTime.MinValue && actionData.Time > t)
			{
				result = true;
			}
			return result;
		}

		internal ExDateTime GetMostRecentActionTime(AmServerName node)
		{
			ExDateTime result = ExDateTime.MinValue;
			TData mostRecentActionData = this.GetMostRecentActionData(node);
			if (mostRecentActionData != null)
			{
				result = mostRecentActionData.Time;
			}
			return result;
		}

		internal TData GetMostRecentActionData(AmServerName node)
		{
			TData result = default(TData);
			lock (this.locker)
			{
				LinkedList<TData> linkedList;
				if (this.actionHistory.TryGetValue(node, out linkedList) && linkedList != null && linkedList.Count > 0)
				{
					result = linkedList.First<TData>();
				}
			}
			return result;
		}

		internal int GetActionsCountForTimeSpan(AmServerName node, TimeSpan timeSpan, ExDateTime now)
		{
			int num = 0;
			ExDateTime t = now - timeSpan;
			lock (this.locker)
			{
				LinkedList<TData> linkedList;
				if (this.actionHistory.TryGetValue(node, out linkedList) && linkedList != null)
				{
					foreach (TData tdata in linkedList)
					{
						if (tdata != null && tdata.Time >= t)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		internal ExDateTime GetMostRecentActionTimeAcrossDag()
		{
			ExDateTime result = ExDateTime.MinValue;
			TData mostRecentActionDataAcrossDag = this.GetMostRecentActionDataAcrossDag();
			if (mostRecentActionDataAcrossDag != null)
			{
				result = mostRecentActionDataAcrossDag.Time;
			}
			return result;
		}

		internal TData GetMostRecentActionDataAcrossDag()
		{
			TData tdata = default(TData);
			lock (this.locker)
			{
				foreach (KeyValuePair<AmServerName, LinkedList<TData>> keyValuePair in this.actionHistory)
				{
					LinkedList<TData> value = keyValuePair.Value;
					if (value != null)
					{
						TData tdata2 = value.First<TData>();
						if (tdata2 != null && (tdata == null || tdata2.Time > tdata.Time))
						{
							tdata = tdata2;
						}
					}
				}
			}
			return tdata;
		}

		internal int GetActionsForTimeSpanAcrossDag(TimeSpan timeSpan, ExDateTime now)
		{
			int num = 0;
			ExDateTime t = now - timeSpan;
			lock (this.locker)
			{
				foreach (KeyValuePair<AmServerName, LinkedList<TData>> keyValuePair in this.actionHistory)
				{
					LinkedList<TData> value = keyValuePair.Value;
					if (value != null)
					{
						int num2 = 0;
						foreach (TData tdata in value)
						{
							if (tdata != null && tdata.Time >= t)
							{
								num2++;
							}
						}
						num += num2;
					}
				}
			}
			return num;
		}

		internal void AddEntry(AmServerName node, TData actionData)
		{
			lock (this.locker)
			{
				LinkedList<TData> linkedList;
				if (!this.actionHistory.TryGetValue(node, out linkedList))
				{
					linkedList = new LinkedList<TData>();
					this.actionHistory.Add(node, linkedList);
				}
				linkedList.AddFirst(actionData);
				if (linkedList.Count > this.MaxHistorySize)
				{
					linkedList.RemoveLast();
				}
				this.Persist(node);
			}
		}

		internal virtual void Cleanup()
		{
			lock (this.locker)
			{
				this.actionHistory.Clear();
			}
		}

		internal static void RemoveEntryFromClusdb(string serverName, string actionName)
		{
			using (IClusterDB clusterDB = ClusterDB.Open())
			{
				string keyName = AmThrottledActionTracker<TData>.ConstructRegKeyName(serverName);
				clusterDB.DeleteValue(keyName, actionName);
			}
		}

		private void Persist(AmServerName node)
		{
			LinkedList<TData> linkedList;
			if (this.actionHistory.TryGetValue(node, out linkedList) && linkedList != null && linkedList.Count > 0)
			{
				using (IClusterDB clusterDB = ClusterDB.Open())
				{
					List<string> list = new List<string>();
					foreach (TData tdata in linkedList)
					{
						string arg = tdata.Time.UniversalTime.ToString("o");
						string dataStr = tdata.DataStr;
						string item = string.Format("{0}{1}{2}", arg, '=', dataStr);
						list.Add(item);
					}
					string[] propetyValue = list.ToArray();
					string keyName = AmThrottledActionTracker<TData>.ConstructRegKeyName(node.NetbiosName);
					clusterDB.SetValue<string[]>(keyName, this.ActionName, propetyValue);
				}
			}
		}

		private const string ThrottledActionsKeyNameFormat = "ExchangeActiveManager\\SystemState\\{0}\\Throttling";

		private const char SeparatorChar = '=';

		private readonly object locker = new object();

		private readonly Dictionary<AmServerName, LinkedList<TData>> actionHistory = new Dictionary<AmServerName, LinkedList<TData>>();

		internal class ThrottlingShapshot
		{
			internal AmServerName Server { get; set; }

			internal TimeSpan MinDurationBetweenActionsPerNode { get; private set; }

			internal int MaxAllowedActionsPerNode { get; private set; }

			internal TimeSpan MaxCheckDurationPerNode { get; private set; }

			internal TimeSpan MinDurationBetweenActionsAcrossDag { get; private set; }

			internal int MaxAllowedActionsAcrossDag { get; private set; }

			internal TimeSpan MaxCheckDurationAcrossDag { get; private set; }

			internal TData MostRecentActionDataForNode { get; set; }

			internal TData MostRecentActionDataAcrossDag { get; set; }

			internal int ActionsCountPerNode { get; set; }

			internal int ActionsCountAcrossDag { get; set; }

			internal ExDateTime CalculationBaseTime { get; set; }

			internal bool IsActionCalledTooSoonPerNode { get; set; }

			internal bool IsActionCalledTooSoonAcrossDag { get; set; }

			internal bool IsMaxActionsPerNodeExceeded { get; set; }

			internal bool IsMaxActionsAcrossDagExceeded { get; set; }

			internal ThrottlingShapshot(AmServerName server, TimeSpan minDurationBetweenActionsPerNode, TimeSpan maxCheckDurationPerNode, int maxAllowedActionsPerNode, TimeSpan minDurationBetweenActionsAcrossDag, TimeSpan maxCheckDurationAcrossDag, int maxAllowedActionsAcrossDag)
			{
				this.Server = server;
				this.MinDurationBetweenActionsPerNode = minDurationBetweenActionsPerNode;
				this.MaxCheckDurationPerNode = maxCheckDurationPerNode;
				this.MaxAllowedActionsPerNode = maxAllowedActionsPerNode;
				this.MinDurationBetweenActionsAcrossDag = minDurationBetweenActionsAcrossDag;
				this.MaxCheckDurationAcrossDag = maxCheckDurationAcrossDag;
				this.MaxAllowedActionsAcrossDag = maxAllowedActionsAcrossDag;
			}

			internal void LogResults(ReplayCrimsonEvent crimsonEvent, TimeSpan suppressDuration)
			{
				if (suppressDuration != TimeSpan.Zero)
				{
					crimsonEvent.LogPeriodicGeneric(this.Server, suppressDuration, new object[]
					{
						this.Server.NetbiosName,
						this.IsActionCalledTooSoonPerNode,
						this.IsActionCalledTooSoonAcrossDag,
						this.IsMaxActionsPerNodeExceeded,
						this.IsMaxActionsAcrossDagExceeded,
						this.GetStringOrNull(this.MostRecentActionDataForNode),
						this.GetStringOrNull(this.MostRecentActionDataAcrossDag),
						this.ActionsCountPerNode,
						this.ActionsCountAcrossDag,
						this.MinDurationBetweenActionsPerNode,
						this.MaxCheckDurationPerNode,
						this.MaxAllowedActionsPerNode,
						this.MinDurationBetweenActionsAcrossDag,
						this.MaxCheckDurationAcrossDag,
						this.MaxAllowedActionsAcrossDag
					});
					return;
				}
				crimsonEvent.LogGeneric(new object[]
				{
					this.Server.NetbiosName,
					this.GetStringOrNull(this.MostRecentActionDataForNode),
					this.GetStringOrNull(this.MostRecentActionDataAcrossDag),
					this.ActionsCountPerNode,
					this.ActionsCountAcrossDag,
					this.MinDurationBetweenActionsPerNode,
					this.MaxCheckDurationPerNode,
					this.MaxAllowedActionsPerNode,
					this.MinDurationBetweenActionsAcrossDag,
					this.MaxCheckDurationAcrossDag,
					this.MaxAllowedActionsAcrossDag
				});
			}

			private string GetStringOrNull(object obj)
			{
				if (obj != null)
				{
					return obj.ToString();
				}
				return "<null>";
			}
		}
	}
}
