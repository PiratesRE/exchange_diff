using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class HealthReportHelper
	{
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

		internal void ProcessEntry(MonitorHealthEntry healthEntry)
		{
			Dictionary<string, List<MonitorHealthEntry>> dictionary = null;
			if (!this.serverHealthMap.TryGetValue(healthEntry.Server, out dictionary))
			{
				dictionary = new Dictionary<string, List<MonitorHealthEntry>>();
				this.serverHealthMap[healthEntry.Server] = dictionary;
			}
			List<MonitorHealthEntry> list = null;
			if (!dictionary.TryGetValue(healthEntry.HealthSetName, out list))
			{
				list = new List<MonitorHealthEntry>();
				dictionary[healthEntry.HealthSetName] = list;
			}
			list.Add(healthEntry);
		}

		internal void ProcessHealth(Action<ConsolidatedHealth> action)
		{
			int num = 0;
			foreach (KeyValuePair<string, Dictionary<string, List<MonitorHealthEntry>>> keyValuePair in this.serverHealthMap)
			{
				Dictionary<string, List<MonitorHealthEntry>> value = keyValuePair.Value;
				if (value != null)
				{
					num += value.Count;
				}
			}
			foreach (KeyValuePair<string, Dictionary<string, List<MonitorHealthEntry>>> keyValuePair2 in this.serverHealthMap)
			{
				string key = keyValuePair2.Key;
				Dictionary<string, List<MonitorHealthEntry>> value2 = keyValuePair2.Value;
				foreach (KeyValuePair<string, List<MonitorHealthEntry>> keyValuePair3 in value2)
				{
					string key2 = keyValuePair3.Key;
					List<MonitorHealthEntry> value3 = keyValuePair3.Value;
					string healthGroup = null;
					MonitorServerComponentState state = MonitorServerComponentState.Unknown;
					if (value3 != null && value3.Count > 0)
					{
						MonitorHealthEntry monitorHealthEntry = value3.First<MonitorHealthEntry>();
						if (monitorHealthEntry != null)
						{
							healthGroup = monitorHealthEntry.HealthGroupName;
							state = monitorHealthEntry.CurrentHealthSetState;
						}
					}
					int monitorCount = 0;
					int haImpactingMonitorCount = 0;
					if (value3 != null)
					{
						monitorCount = value3.Count<MonitorHealthEntry>();
						haImpactingMonitorCount = value3.Count((MonitorHealthEntry che) => che.IsHaImpacting);
					}
					HealthReportHelper.HealthSetStatistics healthSetStats = this.GetHealthSetStats(value3);
					MonitorAlertState alertValue = this.CalculatedConsolidatedHealthSetAlertValue(healthSetStats);
					DateTime lastTransitionTime = healthSetStats.LastTransitionTime;
					ConsolidatedHealth obj = new ConsolidatedHealth(key, key2, healthGroup, alertValue, state, monitorCount, haImpactingMonitorCount, lastTransitionTime, value3);
					action(obj);
				}
			}
		}

		internal ConsolidatedHealth ConsolidateAcrossServers(Dictionary<string, ConsolidatedHealth> serverHealthMap)
		{
			HealthReportHelper.HealthSetStatistics healthSetStatistics = new HealthReportHelper.HealthSetStatistics();
			int num = 0;
			int num2 = 0;
			Dictionary<string, ConsolidatedHealth>.ValueCollection values = serverHealthMap.Values;
			string text = null;
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
							ConsolidatedHealth value = new ConsolidatedHealth(key, consolidatedHealth.HealthSet, consolidatedHealth.HealthGroup);
							dictionary[key] = value;
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
					if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(consolidatedHealth2.HealthSet))
					{
						text = consolidatedHealth2.HealthSet;
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
			if (string.IsNullOrEmpty(text))
			{
				text = "Unknown";
				healthGroup = "Unknown";
			}
			MonitorAlertState alertValue = this.CalculatedConsolidatedHealthSetAlertValue(healthSetStatistics);
			DateTime lastTransitionTime = healthSetStatistics.LastTransitionTime;
			int haImpactingMonitorCount = num2;
			HealthReportHelper.ServerComponentStateStatistics serverComponentStats = this.GetServerComponentStats(serverHealthMap.Values);
			MonitorServerComponentState state = this.CalculatedConsolidatedServerComponentState(serverComponentStats);
			return new ConsolidatedHealth(text, healthGroup, alertValue, state, num, haImpactingMonitorCount, lastTransitionTime, list);
		}

		private MonitorAlertState CalculatedConsolidatedHealthSetAlertValue(HealthReportHelper.HealthSetStatistics stats)
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

		private MonitorServerComponentState CalculatedConsolidatedServerComponentState(HealthReportHelper.ServerComponentStateStatistics stats)
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

		private HealthReportHelper.HealthSetStatistics GetHealthSetStats(IEnumerable<MonitorHealthEntry> entries)
		{
			HealthReportHelper.HealthSetStatistics healthSetStatistics = new HealthReportHelper.HealthSetStatistics();
			foreach (MonitorHealthEntry monitorHealthEntry in entries)
			{
				this.UpdateHealthStats(healthSetStatistics, monitorHealthEntry.AlertValue, monitorHealthEntry.LastTransitionTime);
			}
			return healthSetStatistics;
		}

		private void UpdateHealthStats(HealthReportHelper.HealthSetStatistics stats, MonitorAlertState alertValue, DateTime transitionTime)
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

		private HealthReportHelper.ServerComponentStateStatistics GetServerComponentStats(IEnumerable<ConsolidatedHealth> entries)
		{
			HealthReportHelper.ServerComponentStateStatistics serverComponentStateStatistics = new HealthReportHelper.ServerComponentStateStatistics();
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
