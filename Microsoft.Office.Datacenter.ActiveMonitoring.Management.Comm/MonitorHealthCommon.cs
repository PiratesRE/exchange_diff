using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common
{
	public class MonitorHealthCommon
	{
		public MonitorHealthCommon(string identity, string healthSet, bool haImapctingOnly) : this(identity, healthSet, haImapctingOnly, 0, 70)
		{
		}

		public MonitorHealthCommon(string identity, string healthSet, bool haImapctingOnly, int groupSize, int minimumOnlinePercent)
		{
			this.identity = identity;
			this.healthSet = healthSet;
			this.isHaImapctingOnly = haImapctingOnly;
			this.groupSize = groupSize;
			this.minimumOnlinePercent = minimumOnlinePercent;
		}

		internal Dictionary<string, Dictionary<string, List<MonitorHealthEntry>>> ServerHealthMap
		{
			get
			{
				return this.serverHealthMap;
			}
		}

		internal int GroupSize
		{
			get
			{
				return this.groupSize;
			}
			set
			{
				this.groupSize = value;
			}
		}

		internal int MinimumOnlinePercent
		{
			get
			{
				return this.minimumOnlinePercent;
			}
			set
			{
				this.minimumOnlinePercent = value;
			}
		}

		public List<MonitorHealthEntry> GetMonitorHealthEntries(out LocalizedException exception)
		{
			if (string.IsNullOrWhiteSpace(this.identity))
			{
				throw new ArgumentNullException("The identity can't be empty.");
			}
			List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> list = null;
			List<MonitorHealthEntry> list2 = new List<MonitorHealthEntry>();
			exception = null;
			try
			{
				list = RpcGetMonitorHealthStatus.Invoke(this.identity, this.healthSet, 30000);
			}
			catch (ActiveMonitoringServerException ex)
			{
				exception = ex;
			}
			catch (ActiveMonitoringServerTransientException ex2)
			{
				exception = ex2;
			}
			if (list == null || list.Count == 0)
			{
				list = this.CreateEmptyEntry();
			}
			bool flag = !string.IsNullOrWhiteSpace(this.healthSet);
			foreach (RpcGetMonitorHealthStatus.RpcMonitorHealthEntry rpcMonitorHealthEntry in list)
			{
				if ((!this.isHaImapctingOnly || rpcMonitorHealthEntry.IsHaImpacting) && !string.Equals(rpcMonitorHealthEntry.Name, "HealthManagerHeartbeatMonitor", StringComparison.OrdinalIgnoreCase) && (!flag || string.Equals(rpcMonitorHealthEntry.HealthSetName, this.healthSet, StringComparison.OrdinalIgnoreCase)))
				{
					MonitorHealthEntry item = new MonitorHealthEntry(this.identity, rpcMonitorHealthEntry);
					list2.Add(item);
				}
			}
			return list2;
		}

		public List<ConsolidatedHealth> GetConsolidateHealthEntries()
		{
			List<ConsolidatedHealth> list = new List<ConsolidatedHealth>();
			if (this.serverHealthMap.Count <= 0)
			{
				return null;
			}
			foreach (KeyValuePair<string, Dictionary<string, List<MonitorHealthEntry>>> keyValuePair in this.serverHealthMap)
			{
				string key = keyValuePair.Key;
				Dictionary<string, List<MonitorHealthEntry>> value = keyValuePair.Value;
				foreach (KeyValuePair<string, List<MonitorHealthEntry>> keyValuePair2 in value)
				{
					string key2 = keyValuePair2.Key;
					List<MonitorHealthEntry> value2 = keyValuePair2.Value;
					string healthGroup = null;
					MonitorServerComponentState state = MonitorServerComponentState.Unknown;
					if (value2 != null && value2.Count > 0)
					{
						MonitorHealthEntry monitorHealthEntry = value2.First<MonitorHealthEntry>();
						if (monitorHealthEntry != null)
						{
							healthGroup = monitorHealthEntry.HealthGroupName;
							state = monitorHealthEntry.CurrentHealthSetState;
						}
					}
					int monitorCount = 0;
					int haImpactingMonitorCount = 0;
					if (value2 != null)
					{
						monitorCount = value2.Count<MonitorHealthEntry>();
						haImpactingMonitorCount = value2.Count((MonitorHealthEntry che) => che.IsHaImpacting);
					}
					MonitorHealthCommon.HealthSetStatistics healthSetStats = this.GetHealthSetStats(value2);
					MonitorAlertState alertValue = this.CalculatedConsolidatedHealthSetAlertValue(healthSetStats);
					DateTime lastTransitionTime = healthSetStats.LastTransitionTime;
					ConsolidatedHealth item = new ConsolidatedHealth(key, key2, healthGroup, alertValue, state, monitorCount, haImpactingMonitorCount, lastTransitionTime, value2);
					list.Add(item);
				}
			}
			return list;
		}

		internal void SetServerHealthMap(List<MonitorHealthEntry> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException("The MonitorHealth entries are null.");
			}
			Dictionary<string, List<MonitorHealthEntry>> dictionary = null;
			foreach (MonitorHealthEntry monitorHealthEntry in entries)
			{
				if (!this.serverHealthMap.TryGetValue(monitorHealthEntry.Server, out dictionary))
				{
					dictionary = new Dictionary<string, List<MonitorHealthEntry>>();
					this.serverHealthMap[monitorHealthEntry.Server] = dictionary;
				}
				List<MonitorHealthEntry> list = null;
				if (!dictionary.TryGetValue(monitorHealthEntry.HealthSetName, out list))
				{
					list = new List<MonitorHealthEntry>();
					dictionary[monitorHealthEntry.HealthSetName] = list;
				}
				list.Add(monitorHealthEntry);
			}
		}

		internal ConsolidatedHealth ConsolidateAcrossServers(Dictionary<string, ConsolidatedHealth> serverHealthMap)
		{
			MonitorHealthCommon.HealthSetStatistics healthSetStatistics = new MonitorHealthCommon.HealthSetStatistics();
			int num = 0;
			int num2 = 0;
			Dictionary<string, ConsolidatedHealth>.ValueCollection values = serverHealthMap.Values;
			string value = null;
			string healthGroup = null;
			List<ConsolidatedHealth> list = new List<ConsolidatedHealth>();
			Dictionary<string, ConsolidatedHealth>.ValueCollection values2 = serverHealthMap.Values;
			int num3 = values2.Count((ConsolidatedHealth health) => health == null);
			if (num3 > 0)
			{
				ConsolidatedHealth consolidatedHealth = serverHealthMap.Values.First((ConsolidatedHealth health) => health != null);
				if (consolidatedHealth != null)
				{
					Dictionary<string, ConsolidatedHealth> dictionary = new Dictionary<string, ConsolidatedHealth>();
					foreach (KeyValuePair<string, ConsolidatedHealth> keyValuePair in serverHealthMap)
					{
						string key = keyValuePair.Key;
						if (keyValuePair.Value == null)
						{
							ConsolidatedHealth value2 = new ConsolidatedHealth(key, consolidatedHealth.HealthSet, consolidatedHealth.HealthGroup);
							dictionary[key] = value2;
						}
					}
					foreach (KeyValuePair<string, ConsolidatedHealth> keyValuePair2 in dictionary)
					{
						serverHealthMap[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
			foreach (ConsolidatedHealth consolidatedHealth2 in values)
			{
				if (consolidatedHealth2 != null)
				{
					this.UpdateHealthStats(healthSetStatistics, consolidatedHealth2.AlertValue, consolidatedHealth2.LastTransitionTime);
					if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(consolidatedHealth2.HealthSet))
					{
						value = consolidatedHealth2.HealthSet;
						healthGroup = consolidatedHealth2.HealthGroup;
					}
					num += consolidatedHealth2.MonitorCount;
					num2 += consolidatedHealth2.HaImpactingMonitorCount;
					list.Add(consolidatedHealth2);
				}
				else
				{
					this.UpdateHealthStats(healthSetStatistics, MonitorAlertState.Unknown, DateTime.MinValue);
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				value = "Unknown";
				healthGroup = "Unknown";
			}
			MonitorAlertState alertValue = this.CalculatedConsolidatedHealthSetAlertValue(healthSetStatistics);
			DateTime lastTransitionTime = healthSetStatistics.LastTransitionTime;
			int haImpactingMonitorCount = num2;
			MonitorHealthCommon.ServerComponentStateStatistics serverComponentStats = this.GetServerComponentStats(serverHealthMap.Values);
			MonitorServerComponentState state = this.CalculatedConsolidatedServerComponentState(serverComponentStats);
			return new ConsolidatedHealth(value, healthGroup, alertValue, state, num, haImpactingMonitorCount, lastTransitionTime, list);
		}

		private List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> CreateEmptyEntry()
		{
			return new List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry>
			{
				new RpcGetMonitorHealthStatus.RpcMonitorHealthEntry
				{
					Name = "Unknown",
					HealthSetName = "Unknown",
					HealthGroupName = "Unknown",
					ServerComponentName = "Unknown",
					AlertValue = MonitorAlertState.Unknown.ToString(),
					CurrentHealthSetState = MonitorServerComponentState.Unknown.ToString()
				}
			};
		}

		private MonitorAlertState CalculatedConsolidatedHealthSetAlertValue(MonitorHealthCommon.HealthSetStatistics stats)
		{
			MonitorAlertState result = MonitorAlertState.Unknown;
			if (stats.TotalCount == 0)
			{
				result = MonitorAlertState.Healthy;
			}
			else if (stats.UnknownCount == stats.TotalCount)
			{
				result = MonitorAlertState.Unknown;
			}
			else if (stats.DisabledCount == stats.TotalCount)
			{
				result = MonitorAlertState.Disabled;
			}
			else if (stats.UnhealthyCount > 0)
			{
				result = MonitorAlertState.Unhealthy;
			}
			else if (stats.DegradedCount > 0)
			{
				result = MonitorAlertState.Degraded;
			}
			else if (stats.RepairingCount > 0)
			{
				result = MonitorAlertState.Repairing;
			}
			else if (stats.HealthyCount > 0)
			{
				int num = stats.HealthyCount + stats.DisabledCount + stats.UnknownCount;
				if (num == stats.TotalCount)
				{
					result = MonitorAlertState.Healthy;
				}
			}
			return result;
		}

		private MonitorServerComponentState CalculatedConsolidatedServerComponentState(MonitorHealthCommon.ServerComponentStateStatistics stats)
		{
			MonitorServerComponentState result = MonitorServerComponentState.Unknown;
			if (stats.TotalCount == stats.UnknownCount)
			{
				result = MonitorServerComponentState.Unknown;
			}
			else if (stats.TotalCount == stats.NotApplicableCount + stats.UnknownCount)
			{
				result = MonitorServerComponentState.NotApplicable;
			}
			else if (stats.TotalCount == stats.OnlineCount)
			{
				result = MonitorServerComponentState.Online;
			}
			else if (stats.TotalCount == stats.PartiallyOnlineCount)
			{
				result = MonitorServerComponentState.PartiallyOnline;
			}
			else if (stats.TotalCount == stats.OfflineCount)
			{
				result = MonitorServerComponentState.Offline;
			}
			else if (stats.TotalCount == stats.FunctionalCount)
			{
				result = MonitorServerComponentState.Functional;
			}
			else if (stats.TotalCount == stats.SidelinedCount)
			{
				result = MonitorServerComponentState.Sidelined;
			}
			else if (stats.OnlineCount > 0 || stats.OfflineCount > 0 || stats.PartiallyOnlineCount > 0)
			{
				int totalCount = stats.TotalCount;
				if (this.GroupSize > 0)
				{
					totalCount = this.GroupSize;
				}
				int num = totalCount * this.MinimumOnlinePercent / 100;
				if (num < 1)
				{
					num = 1;
				}
				if (stats.OnlineCount + stats.PartiallyOnlineCount >= num)
				{
					result = MonitorServerComponentState.PartiallyOnline;
				}
				else
				{
					result = MonitorServerComponentState.Offline;
				}
			}
			return result;
		}

		private MonitorHealthCommon.HealthSetStatistics GetHealthSetStats(IEnumerable<MonitorHealthEntry> entries)
		{
			MonitorHealthCommon.HealthSetStatistics healthSetStatistics = new MonitorHealthCommon.HealthSetStatistics();
			foreach (MonitorHealthEntry monitorHealthEntry in entries)
			{
				this.UpdateHealthStats(healthSetStatistics, monitorHealthEntry.AlertValue, monitorHealthEntry.LastTransitionTime);
			}
			return healthSetStatistics;
		}

		private MonitorHealthCommon.ServerComponentStateStatistics GetServerComponentStats(IEnumerable<ConsolidatedHealth> entries)
		{
			MonitorHealthCommon.ServerComponentStateStatistics serverComponentStateStatistics = new MonitorHealthCommon.ServerComponentStateStatistics();
			foreach (ConsolidatedHealth consolidatedHealth in entries)
			{
				serverComponentStateStatistics.TotalCount++;
				if (consolidatedHealth != null)
				{
					switch (consolidatedHealth.State)
					{
					case MonitorServerComponentState.NotApplicable:
						serverComponentStateStatistics.NotApplicableCount++;
						break;
					case MonitorServerComponentState.Online:
						serverComponentStateStatistics.OnlineCount++;
						break;
					case MonitorServerComponentState.PartiallyOnline:
						serverComponentStateStatistics.PartiallyOnlineCount++;
						break;
					case MonitorServerComponentState.Offline:
						serverComponentStateStatistics.OfflineCount++;
						break;
					case MonitorServerComponentState.Functional:
						serverComponentStateStatistics.FunctionalCount++;
						break;
					case MonitorServerComponentState.Sidelined:
						serverComponentStateStatistics.SidelinedCount++;
						break;
					default:
						serverComponentStateStatistics.UnknownCount++;
						break;
					}
				}
				else
				{
					serverComponentStateStatistics.UnknownCount++;
				}
			}
			return serverComponentStateStatistics;
		}

		private void UpdateHealthStats(MonitorHealthCommon.HealthSetStatistics stats, MonitorAlertState alertValue, DateTime transitionTime)
		{
			if (transitionTime > stats.LastTransitionTime)
			{
				stats.LastTransitionTime = transitionTime;
			}
			stats.TotalCount++;
			switch (alertValue)
			{
			case MonitorAlertState.Healthy:
				stats.HealthyCount++;
				return;
			case MonitorAlertState.Degraded:
				stats.DegradedCount++;
				return;
			case MonitorAlertState.Unhealthy:
				stats.UnhealthyCount++;
				return;
			case MonitorAlertState.Repairing:
				stats.RepairingCount++;
				return;
			case MonitorAlertState.Disabled:
				stats.DisabledCount++;
				return;
			default:
				stats.UnknownCount++;
				return;
			}
		}

		private readonly string identity = string.Empty;

		private readonly string healthSet = string.Empty;

		private readonly bool isHaImapctingOnly;

		private Dictionary<string, Dictionary<string, List<MonitorHealthEntry>>> serverHealthMap = new Dictionary<string, Dictionary<string, List<MonitorHealthEntry>>>();

		private int groupSize;

		private int minimumOnlinePercent = 70;

		private class HealthSetStatistics
		{
			internal DateTime LastTransitionTime { get; set; }

			internal int TotalCount { get; set; }

			internal int UnknownCount { get; set; }

			internal int HealthyCount { get; set; }

			internal int DegradedCount { get; set; }

			internal int UnhealthyCount { get; set; }

			internal int RepairingCount { get; set; }

			internal int DisabledCount { get; set; }
		}

		private class ServerComponentStateStatistics
		{
			internal int TotalCount { get; set; }

			internal int UnknownCount { get; set; }

			internal int NotApplicableCount { get; set; }

			internal int OnlineCount { get; set; }

			internal int PartiallyOnlineCount { get; set; }

			internal int OfflineCount { get; set; }

			internal int FunctionalCount { get; set; }

			internal int SidelinedCount { get; set; }
		}
	}
}
