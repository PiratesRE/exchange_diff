using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.DxStore.Server
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class DxStoreManager : IDxStoreManager
	{
		public DxStoreManager(IDxStoreConfigProvider configProvider, IDxStoreEventLogger eventLogger)
		{
			this.EventLogger = eventLogger;
			this.ConfigProvider = configProvider;
			this.instanceMap = new Dictionary<string, Tuple<InstanceGroupConfig, DxStoreInstanceChecker>>();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ManagerTracer;
			}
		}

		public IDxStoreEventLogger EventLogger { get; set; }

		public InstanceManagerConfig Config { get; set; }

		public bool IsStopping { get; set; }

		public ManualResetEvent StopEvent { get; set; }

		public IDxStoreConfigProvider ConfigProvider { get; set; }

		public void Start()
		{
			lock (this.locker)
			{
				if (!this.isInitialized)
				{
					this.Config = this.ConfigProvider.GetManagerConfig();
					DxStoreManager.Tracer.TraceDebug<string>((long)this.Config.Identity.GetHashCode(), "{0}: Starting Manager", this.Config.Identity);
					this.EventLogger.Log(DxEventSeverity.Info, 0, "{0}: Starting DxStoreManager", new object[]
					{
						this.Config.Identity
					});
					this.configCheckerTimer = new GuardedTimer(delegate(object unused)
					{
						this.MonitorGroups("Periodic scan", false);
					}, null, TimeSpan.Zero, this.Config.InstanceMonitorInterval);
					this.isInitialized = true;
					this.EventLogger.Log(DxEventSeverity.Info, 0, "{0}: Started DxStoreManager", new object[]
					{
						this.Config.Identity
					});
				}
			}
		}

		public void Stop(TimeSpan? timeout = null)
		{
			this.IsStopping = true;
			if (!this.isInitialized)
			{
				return;
			}
			timeout = new TimeSpan?(timeout ?? this.Config.ManagerStopTimeout);
			DxStoreManager.Tracer.TraceDebug<string>((long)this.Config.Identity.GetHashCode(), "{0}: Stopping Manager", this.Config.Identity);
			Exception ex = Utils.RunOperation(this.Config.Identity, "DxStoreManager.Stop()", delegate
			{
				this.StopInternal(timeout);
			}, this.EventLogger, LogOptions.LogAll, true, timeout, null, null, null, null);
			DxStoreManager.Tracer.TraceDebug<string, string>((long)this.Config.Identity.GetHashCode(), "{0}: Stop completed {1}", this.Config.Identity, (ex != null) ? ex.ToString() : string.Empty);
		}

		public void StopInternal(TimeSpan? timeout)
		{
			lock (this.locker)
			{
				DxStoreManager.Tracer.TraceDebug<string>((long)this.Config.Identity.GetHashCode(), "{0}: Attempting to dispose config checker timer", this.Config.Identity);
				this.configCheckerTimer.Dispose(true);
				DxStoreManager.Tracer.TraceDebug<string>((long)this.Config.Identity.GetHashCode(), "{0}: Attempting to stop all instance checkers", this.Config.Identity);
				string[] array = this.instanceMap.Keys.ToArray<string>();
				foreach (string groupName in array)
				{
					this.StopInstance(groupName, false);
				}
				if (this.managerServiceHost != null)
				{
					DxStoreManager.Tracer.TraceDebug<string, string>((long)this.Config.Identity.GetHashCode(), "{0}: Closing manager service host Timeout: {1}", this.Config.Identity, (timeout != null) ? timeout.Value.ToString() : "<timeout not specified>");
					if (timeout != null)
					{
						this.managerServiceHost.Close(timeout.Value);
					}
					else
					{
						this.managerServiceHost.Close();
					}
				}
			}
		}

		public void StartInstance(string groupName, bool isForce)
		{
			lock (this.locker)
			{
				if (this.GetInstanceContainer(groupName) == null)
				{
					InstanceGroupConfig groupConfig = this.ConfigProvider.GetGroupConfig(groupName, false);
					if (groupConfig == null)
					{
						throw new DxStoreManagerGroupNotFoundException(groupName);
					}
					this.StartInstanceInternal(groupConfig, isForce);
				}
			}
		}

		public void RestartInstance(string groupName, bool isForce)
		{
			lock (this.locker)
			{
				InstanceGroupConfig groupConfig = this.ConfigProvider.GetGroupConfig(groupName, false);
				this.RestartInstanceInternal(groupConfig, isForce);
			}
		}

		public void RemoveInstance(string groupName)
		{
			lock (this.locker)
			{
				this.StopInstanceInternal(groupName, true);
				this.ConfigProvider.RemoveGroupConfig(groupName);
			}
		}

		public void StopInstance(string groupName, bool isDisable = false)
		{
			lock (this.locker)
			{
				this.StopInstanceInternal(groupName, true);
				if (isDisable)
				{
					this.ConfigProvider.DisableGroup(groupName);
				}
			}
		}

		public InstanceGroupConfig GetInstanceConfig(string groupName, bool isForce = false)
		{
			InstanceGroupConfig result = null;
			lock (this.locker)
			{
				if (!isForce)
				{
					Tuple<InstanceGroupConfig, DxStoreInstanceChecker> instanceContainer = this.GetInstanceContainer(groupName);
					if (instanceContainer != null)
					{
						result = instanceContainer.Item1;
					}
				}
				else
				{
					result = this.ConfigProvider.GetGroupConfig(groupName, true);
				}
			}
			return result;
		}

		public void TriggerRefresh(string reason, bool isForceRefreshCache)
		{
			DateTimeOffset now = DateTimeOffset.Now;
			if (now - this.lastTriggerRefreshInitiatedTime < TimeSpan.FromSeconds(30.0))
			{
				this.EventLogger.LogPeriodic("TriggerRefresh", this.Config.Settings.PeriodicExceptionLoggingDuration, DxEventSeverity.Warning, 0, "TriggerRefresh postponed to avoid refresh flood (LastRequestTime: {0})", new object[]
				{
					this.lastTriggerRefreshInitiatedTime
				});
				this.isTriggerRefreshPostponed = isForceRefreshCache;
				return;
			}
			this.lastTriggerRefreshInitiatedTime = DateTimeOffset.Now;
			this.isTriggerRefreshPostponed = false;
			Task.Factory.StartNew(delegate()
			{
				this.MonitorGroups(reason, isForceRefreshCache);
			});
		}

		public void RegisterServiceHostIfRequired()
		{
			if (this.managerServiceHost == null)
			{
				ServiceHost serviceHost = new ServiceHost(this, new Uri[0]);
				ServiceEndpoint endpoint = this.Config.GetEndpoint(this.Config.Self, true, null);
				serviceHost.AddServiceEndpoint(endpoint);
				serviceHost.Open();
				this.managerServiceHost = serviceHost;
			}
		}

		public void MonitorGroups(string reason, bool isForceRefreshCache = false)
		{
			lock (this.locker)
			{
				Utils.RunOperation(this.Config.Identity, "MonitorGroups", delegate
				{
					this.MonitorGroupsInternal(this.isTriggerRefreshPostponed || isForceRefreshCache);
				}, this.EventLogger, LogOptions.LogException | this.Config.Settings.AdditionalLogOptions, true, null, null, null, null, reason);
			}
		}

		public void MonitorGroupsInternal(bool isForceRefreshCache)
		{
			this.isTriggerRefreshPostponed = false;
			this.RegisterServiceHostIfRequired();
			this.ConfigProvider.RefreshTopology(isForceRefreshCache);
			InstanceGroupConfig[] allGroupConfigs = this.ConfigProvider.GetAllGroupConfigs();
			this.RemoveStartProcessLimitForNonExistentGroups(allGroupConfigs);
			InstanceGroupConfig[] array = allGroupConfigs;
			for (int i = 0; i < array.Length; i++)
			{
				InstanceGroupConfig instanceGroupConfig = array[i];
				InstanceGroupConfig tmpGroup = instanceGroupConfig;
				Utils.RunOperation(tmpGroup.Identity, "CheckGroup", delegate
				{
					this.CheckGroup(tmpGroup);
				}, this.EventLogger, LogOptions.LogPeriodic | LogOptions.LogExceptionFull | instanceGroupConfig.Settings.AdditionalLogOptions, true, null, null, null, null, null);
			}
			this.StopRemovedGroups(allGroupConfigs);
		}

		public void StopRemovedGroups(InstanceGroupConfig[] groups)
		{
			if (this.instanceMap.Count == 0)
			{
				return;
			}
			IEnumerable<string> enumerable = from groupName in this.instanceMap.Keys
			let isFound = groups.Any((InstanceGroupConfig g) => Utils.IsEqual(groupName, g.Name, StringComparison.OrdinalIgnoreCase))
			where !isFound
			select groupName;
			foreach (string groupName2 in enumerable)
			{
				this.StopInstanceInternal(groupName2, true);
			}
		}

		public void CheckGroup(InstanceGroupConfig group)
		{
			if (!group.IsConfigurationReady)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Tuple<InstanceGroupConfig, DxStoreInstanceChecker> instanceContainer = this.GetInstanceContainer(group.Name);
			if (instanceContainer == null)
			{
				flag = (group.IsAutomaticActionsAllowed && group.IsMembersContainSelf);
			}
			else if (!group.IsAutomaticActionsAllowed || !group.IsMembersContainSelf)
			{
				flag2 = true;
			}
			else
			{
				InstanceGroupConfig item = instanceContainer.Item1;
				DxStoreInstanceChecker item2 = instanceContainer.Item2;
				if (item2.IsRestartRequested || group.IsRestartRequested)
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				this.RestartInstanceInternal(group, false);
			}
			else if (flag2)
			{
				this.StopInstanceInternal(group.Name, false);
			}
			else if (flag)
			{
				this.StartInstanceInternal(group, false);
			}
			if (!group.IsMembersContainSelf && !group.IsConfigurationManagedExternally && DxStoreInstance.RemoveGroupStorage(this.EventLogger, group))
			{
				this.RemoveGroupConfig(group);
			}
		}

		public void RemoveGroupConfig(InstanceGroupConfig group)
		{
			this.EventLogger.Log(DxEventSeverity.Info, 0, "{0}: Removing group configuration", new object[]
			{
				group.Name
			});
			this.ConfigProvider.RemoveGroupConfig(group.Name);
		}

		public Tuple<InstanceGroupConfig, DxStoreInstanceChecker> GetInstanceContainer(string groupName)
		{
			Tuple<InstanceGroupConfig, DxStoreInstanceChecker> result;
			this.instanceMap.TryGetValue(groupName, out result);
			return result;
		}

		public bool CheckStartProcessLimits(InstanceGroupConfig group, bool isForce)
		{
			DateTimeOffset now = DateTimeOffset.Now;
			DxStoreManager.InstanceStartStats instanceStartStats;
			if (!this.instanceStartMap.TryGetValue(group.Identity, out instanceStartStats))
			{
				instanceStartStats = new DxStoreManager.InstanceStartStats();
				this.ResetStartProcessLimit(instanceStartStats, now);
				this.instanceStartMap[group.Identity] = instanceStartStats;
			}
			else
			{
				TimeSpan t = now - instanceStartStats.FirstStartRequestedTime;
				TimeSpan t2 = now - instanceStartStats.LastStartRequestedTime;
				if (t2 < group.Settings.InstanceStartSilenceDuration)
				{
					if (instanceStartStats.TotalStartRequestsFromFirstReported > group.Settings.InstanceStartHoldupDurationMaxAllowedStarts)
					{
						if (t < group.Settings.InstanceStartHoldUpDuration)
						{
							this.EventLogger.LogPeriodic(group.Identity, TimeSpan.FromMinutes(10.0), DxEventSeverity.Warning, 0, "Instance start request exceeded maximum allowed ({3}). (FirstStartTime: {0}, LastStartTime: {1}, TotalRequests: {2}", new object[]
							{
								instanceStartStats.FirstStartRequestedTime,
								instanceStartStats.LastStartRequestedTime,
								instanceStartStats.TotalStartRequestsFromFirstReported,
								isForce ? "but allowing due to force flag" : "start rejected"
							});
							if (!isForce)
							{
								return false;
							}
						}
						else
						{
							this.ResetStartProcessLimit(instanceStartStats, now);
						}
					}
					instanceStartStats.LastStartRequestedTime = now;
					instanceStartStats.TotalStartRequestsFromFirstReported++;
				}
				else
				{
					this.ResetStartProcessLimit(instanceStartStats, now);
				}
			}
			return true;
		}

		public void RemoveStartProcessLimitForNonExistentGroups(InstanceGroupConfig[] groups)
		{
			if (groups != null)
			{
				HashSet<string> hashSet = new HashSet<string>(this.instanceStartMap.Keys);
				foreach (InstanceGroupConfig instanceGroupConfig in groups)
				{
					hashSet.Remove(instanceGroupConfig.Identity);
				}
				using (HashSet<string>.Enumerator enumerator = hashSet.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string key = enumerator.Current;
						this.instanceStartMap.Remove(key);
					}
					return;
				}
			}
			this.instanceStartMap.Clear();
		}

		public void ResetStartProcessLimit(DxStoreManager.InstanceStartStats stats, DateTimeOffset now)
		{
			stats.LastStartRequestedTime = now;
			stats.FirstStartRequestedTime = now;
			stats.TotalStartRequestsFromFirstReported = 1;
		}

		private void StartInstanceInternal(InstanceGroupConfig group, bool isForce = false)
		{
			DxStoreManager.Tracer.TraceDebug<string, string, bool>((long)this.Config.Identity.GetHashCode(), "{0}: Starting instance checker {1} (group.IsRestartRequested: {2})", this.Config.Identity, group.Identity, group.IsRestartRequested);
			if (group.IsRestartRequested)
			{
				this.ConfigProvider.SetRestartRequired(group.Name, false);
				group.IsRestartRequested = false;
			}
			else if (!this.CheckStartProcessLimits(group, isForce))
			{
				return;
			}
			DxStoreInstanceChecker dxStoreInstanceChecker = new DxStoreInstanceChecker(this, group);
			this.instanceMap[group.Name] = new Tuple<InstanceGroupConfig, DxStoreInstanceChecker>(group, dxStoreInstanceChecker);
			dxStoreInstanceChecker.Start();
		}

		private void RestartInstanceInternal(InstanceGroupConfig group, bool isForce = false)
		{
			DxStoreManager.Tracer.TraceDebug<string, string>((long)this.Config.Identity.GetHashCode(), "{0}: Restarting instance checker {1}", this.Config.Identity, group.Identity);
			this.StopInstanceInternal(group.Name, false);
			this.StartInstanceInternal(group, isForce);
		}

		private void StopInstanceInternal(string groupName, bool isBestEffort = false)
		{
			Tuple<InstanceGroupConfig, DxStoreInstanceChecker> instanceContainer = this.GetInstanceContainer(groupName);
			if (instanceContainer != null)
			{
				DxStoreManager.Tracer.TraceDebug<string, string, bool>((long)this.Config.Identity.GetHashCode(), "{0}: Stopping instance checker {1} (BestEffort: {2})", this.Config.Identity, instanceContainer.Item1.Identity, isBestEffort);
				instanceContainer.Item2.Stop(isBestEffort);
				this.instanceMap.Remove(groupName);
			}
		}

		private readonly object locker = new object();

		private readonly Dictionary<string, Tuple<InstanceGroupConfig, DxStoreInstanceChecker>> instanceMap;

		private ServiceHost managerServiceHost;

		private GuardedTimer configCheckerTimer;

		private bool isInitialized;

		private DateTimeOffset lastTriggerRefreshInitiatedTime;

		private bool isTriggerRefreshPostponed;

		private Dictionary<string, DxStoreManager.InstanceStartStats> instanceStartMap = new Dictionary<string, DxStoreManager.InstanceStartStats>();

		public class InstanceStartStats
		{
			public DateTimeOffset FirstStartRequestedTime { get; set; }

			public DateTimeOffset LastStartRequestedTime { get; set; }

			public int TotalStartRequestsFromFirstReported { get; set; }
		}
	}
}
