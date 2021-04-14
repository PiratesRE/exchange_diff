using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ComponentStateWrapper
	{
		internal ComponentStateWrapper(string dbName, string componentName, AmServerName sourceServerName, AmDbActionCode actionCode, Dictionary<AmServerName, RpcHealthStateInfo[]> stateInfoMap)
		{
			this.DatabaseName = dbName;
			this.Initialize(componentName, sourceServerName, actionCode, stateInfoMap);
		}

		internal string InitiatingComponentName
		{
			get
			{
				if (this.InitiatingComponent != null)
				{
					return this.InitiatingComponent.Name;
				}
				return null;
			}
		}

		internal string DatabaseName { get; private set; }

		internal Component InitiatingComponent { get; set; }

		internal AmServerName SourceServerName { get; set; }

		internal AmDbActionCode ActionCode { get; set; }

		internal Dictionary<AmServerName, RpcHealthStateInfo[]> StateInfoMap { get; set; }

		private void Initialize(string componentName, AmServerName sourceServerName, AmDbActionCode actionCode, Dictionary<AmServerName, RpcHealthStateInfo[]> stateInfoMap)
		{
			if (!string.IsNullOrEmpty(componentName))
			{
				this.InitiatingComponent = new Component(componentName);
			}
			this.SourceServerName = sourceServerName;
			this.ActionCode = actionCode;
			this.StateInfoMap = stateInfoMap;
			this.consolidatedHealthTable = new Dictionary<AmServerName, Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState>>();
			if (stateInfoMap != null)
			{
				foreach (KeyValuePair<AmServerName, RpcHealthStateInfo[]> keyValuePair in stateInfoMap)
				{
					AmServerName key = keyValuePair.Key;
					RpcHealthStateInfo[] value = keyValuePair.Value;
					Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> dictionary = new Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState>();
					if (value != null && value.Length > 0)
					{
						IOrderedEnumerable<RpcHealthStateInfo> orderedEnumerable = from hs in value
						where hs != null
						orderby hs.Priority
						select hs;
						foreach (RpcHealthStateInfo rpcHealthStateInfo in orderedEnumerable)
						{
							if (rpcHealthStateInfo.DatabaseName == null || string.Equals(this.DatabaseName, rpcHealthStateInfo.DatabaseName, StringComparison.OrdinalIgnoreCase))
							{
								ComponentStateWrapper.ConsolidatedHealthState consolidatedHealthState = null;
								if (!dictionary.TryGetValue(rpcHealthStateInfo.ComponentName, out consolidatedHealthState))
								{
									consolidatedHealthState = new ComponentStateWrapper.ConsolidatedHealthState();
									consolidatedHealthState.ComponentName = rpcHealthStateInfo.ComponentName;
									consolidatedHealthState.Priority = rpcHealthStateInfo.Priority;
									dictionary[consolidatedHealthState.ComponentName] = consolidatedHealthState;
								}
								if (rpcHealthStateInfo.HealthStatus > consolidatedHealthState.HealthStatus)
								{
									consolidatedHealthState.HealthStatus = rpcHealthStateInfo.HealthStatus;
								}
							}
						}
					}
					this.consolidatedHealthTable[key] = dictionary;
				}
			}
			this.ReportComponentHealth();
		}

		private void ReportComponentHealth()
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			string text = string.Format(" {0,-15} | {1,-10} | {2,-15} \n", "Component", "Priority", "HealthStatus");
			string value = string.Format("{0}\n", new string('-', text.Length));
			foreach (KeyValuePair<AmServerName, Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState>> keyValuePair in this.consolidatedHealthTable)
			{
				stringBuilder.Clear();
				string netbiosName = keyValuePair.Key.NetbiosName;
				Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> value2 = keyValuePair.Value;
				if (value2 != null && value2.Count > 0)
				{
					stringBuilder.Append(value);
					stringBuilder.AppendFormat(text, new object[0]);
					stringBuilder.Append(value);
					foreach (ComponentStateWrapper.ConsolidatedHealthState consolidatedHealthState in value2.Values)
					{
						stringBuilder.AppendFormat(" {0,-15} | {1,-10} | {2,-15} \n", consolidatedHealthState.ComponentName, (ManagedAvailabilityPriority)consolidatedHealthState.Priority, (ServiceHealthStatus)consolidatedHealthState.HealthStatus);
					}
					stringBuilder.Append(value);
				}
				ReplayCrimsonEvents.ComponentHealthState.Log<string, string>(netbiosName, stringBuilder.ToString());
			}
			if (this.consolidatedHealthTable.Count == 0)
			{
				ReplayCrimsonEvents.ComponentHealthState.Log<string, string>("<none>", "No consolidated health entries were present.");
			}
		}

		private Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> GetConsolidatedHealthStateMap(AmServerName targetServerName)
		{
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> result = null;
			this.consolidatedHealthTable.TryGetValue(targetServerName, out result);
			return result;
		}

		private ComponentStateWrapper.ConsolidatedHealthState GetComponentHealthStateInTarget(AmServerName targetServerName, string componentName)
		{
			ComponentStateWrapper.ConsolidatedHealthState result = null;
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap = this.GetConsolidatedHealthStateMap(targetServerName);
			if (consolidatedHealthStateMap != null)
			{
				consolidatedHealthStateMap.TryGetValue(componentName, out result);
			}
			return result;
		}

		internal bool IsInitiatorComponentBetterThanSource(AmServerName targetServerName, List<string> failures)
		{
			bool result = true;
			if (this.ActionCode.IsAutomaticManagedAvailabilityFailover)
			{
				ComponentStateWrapper.ConsolidatedHealthState componentHealthStateInTarget = this.GetComponentHealthStateInTarget(this.SourceServerName, this.InitiatingComponentName);
				if (componentHealthStateInTarget != null)
				{
					ComponentStateWrapper.ConsolidatedHealthState componentHealthStateInTarget2 = this.GetComponentHealthStateInTarget(targetServerName, this.InitiatingComponentName);
					if (componentHealthStateInTarget2 != null && componentHealthStateInTarget2.HealthStatus != 1 && componentHealthStateInTarget2.HealthStatus >= componentHealthStateInTarget.HealthStatus)
					{
						result = false;
						failures.Add(string.Format("Initiating component {0} is not in a better state on target server (SrcHealth={1} TargetHealth={2}\n", this.InitiatingComponentName, componentHealthStateInTarget.HealthStatus, componentHealthStateInTarget2.HealthStatus));
					}
				}
			}
			return result;
		}

		internal bool IsAllComponentsHealthy(AmServerName targetServerName, List<string> failures)
		{
			bool result = true;
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap = this.GetConsolidatedHealthStateMap(targetServerName);
			if (consolidatedHealthStateMap != null)
			{
				foreach (KeyValuePair<string, ComponentStateWrapper.ConsolidatedHealthState> keyValuePair in consolidatedHealthStateMap)
				{
					ComponentStateWrapper.ConsolidatedHealthState value = keyValuePair.Value;
					if (value.HealthStatus != 1)
					{
						failures.Add(string.Format("Component {0} is not Healthy on target server (ConsolidatedHealthState={1})", value.ComponentName, value.HealthStatus));
						result = false;
						break;
					}
				}
			}
			return result;
		}

		internal bool IsUptoNormalComponentsHealthy(AmServerName targetServerName, List<string> failures)
		{
			bool result = true;
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap = this.GetConsolidatedHealthStateMap(targetServerName);
			if (consolidatedHealthStateMap != null)
			{
				foreach (KeyValuePair<string, ComponentStateWrapper.ConsolidatedHealthState> keyValuePair in consolidatedHealthStateMap)
				{
					ComponentStateWrapper.ConsolidatedHealthState value = keyValuePair.Value;
					if (value.Priority <= 60 && value.HealthStatus != 1)
					{
						failures.Add(string.Format("Component {0} is not Healthy on target server (ConsolidatedHealthState={1})", value.ComponentName, value.HealthStatus));
						result = false;
						break;
					}
				}
			}
			return result;
		}

		internal bool IsComponentsBettterThanSource(AmServerName targetServerName, List<string> failures)
		{
			bool result = true;
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap = this.GetConsolidatedHealthStateMap(this.SourceServerName);
			if (consolidatedHealthStateMap == null)
			{
				return true;
			}
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap2 = this.GetConsolidatedHealthStateMap(targetServerName);
			if (consolidatedHealthStateMap2 == null)
			{
				return true;
			}
			foreach (KeyValuePair<string, ComponentStateWrapper.ConsolidatedHealthState> keyValuePair in consolidatedHealthStateMap)
			{
				string key = keyValuePair.Key;
				ComponentStateWrapper.ConsolidatedHealthState value = keyValuePair.Value;
				ComponentStateWrapper.ConsolidatedHealthState consolidatedHealthState = null;
				if (consolidatedHealthStateMap2.TryGetValue(key, out consolidatedHealthState) && consolidatedHealthState != null && consolidatedHealthState.Priority <= 60 && consolidatedHealthState.HealthStatus != 1 && consolidatedHealthState.HealthStatus >= value.HealthStatus)
				{
					failures.Add(string.Format("Component {0} is not in a better state than source on target server (SrcHealth={1} TargetHealth={2}\n", key, value.HealthStatus, consolidatedHealthState.HealthStatus));
					result = false;
					break;
				}
			}
			return result;
		}

		internal bool IsComponentsAtleastSameAsSource(AmServerName targetServerName, List<string> failures)
		{
			bool result = true;
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap = this.GetConsolidatedHealthStateMap(this.SourceServerName);
			if (consolidatedHealthStateMap == null)
			{
				return true;
			}
			Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState> consolidatedHealthStateMap2 = this.GetConsolidatedHealthStateMap(targetServerName);
			if (consolidatedHealthStateMap2 == null)
			{
				return true;
			}
			foreach (KeyValuePair<string, ComponentStateWrapper.ConsolidatedHealthState> keyValuePair in consolidatedHealthStateMap)
			{
				string key = keyValuePair.Key;
				ComponentStateWrapper.ConsolidatedHealthState value = keyValuePair.Value;
				ComponentStateWrapper.ConsolidatedHealthState consolidatedHealthState = null;
				if (consolidatedHealthStateMap2.TryGetValue(key, out consolidatedHealthState) && consolidatedHealthState != null && consolidatedHealthState.Priority <= 60 && consolidatedHealthState.HealthStatus != 1 && consolidatedHealthState.HealthStatus > value.HealthStatus)
				{
					failures.Add(string.Format("Component {0} is not in a same or better state than source on target server (SrcHealth={1} TargetHealth={2}\n", key, value.HealthStatus, consolidatedHealthState.HealthStatus));
					result = false;
					break;
				}
			}
			return result;
		}

		private Dictionary<AmServerName, Dictionary<string, ComponentStateWrapper.ConsolidatedHealthState>> consolidatedHealthTable;

		internal class ConsolidatedHealthState
		{
			internal string ComponentName { get; set; }

			internal int Priority { get; set; }

			internal int HealthStatus { get; set; }
		}
	}
}
