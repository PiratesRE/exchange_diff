using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Win32;

namespace Microsoft.Exchange.DxStore.Common
{
	public class DxStoreRegistryConfigProvider : IDxStoreConfigProvider, IServerNameResolver, ITopologyProvider
	{
		public DxStoreRegistryConfigProvider()
		{
		}

		public DxStoreRegistryConfigProvider(string componentName)
		{
			this.Initialize(componentName, null, null, null, false);
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ConfigTracer;
			}
		}

		public string Self { get; set; }

		public string DefaultStorageBaseDir { get; set; }

		public string DefaultManagerKeyName { get; set; }

		public string DefaultInstanceProcessFullPath { get; set; }

		public string ManagerConfigKeyName { get; set; }

		public InstanceManagerConfig ManagerConfig { get; set; }

		public void Initialize(string componentName, string self = null, string baseComponentKeyName = null, IDxStoreEventLogger eventLogger = null, bool isZeroboxMode = false)
		{
			this.componentName = componentName;
			this.Self = ((!string.IsNullOrEmpty(self)) ? self : Environment.MachineName);
			this.isZeroboxMode = isZeroboxMode;
			if (string.IsNullOrEmpty(this.DefaultStorageBaseDir))
			{
				this.DefaultStorageBaseDir = Utils.CombinePathNullSafe(isZeroboxMode ? "C:\\Exchange" : ExchangeSetupContext.InstallPath, string.Format("DxStore\\Database\\{0}", this.componentName));
				if (isZeroboxMode)
				{
					this.DefaultStorageBaseDir = this.DefaultStorageBaseDir + "\\ZeroBox\\" + this.Self;
				}
			}
			if (string.IsNullOrEmpty(baseComponentKeyName))
			{
				baseComponentKeyName = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\DxStore\\{1}", "v15", this.componentName);
				if (isZeroboxMode)
				{
					baseComponentKeyName = baseComponentKeyName + "\\Zerobox\\" + this.Self;
				}
			}
			this.eventLogger = eventLogger;
			this.ManagerConfigKeyName = baseComponentKeyName;
			this.ManagerConfig = this.GetManagerConfig();
		}

		public virtual string ResolveName(string shortServerName)
		{
			return shortServerName;
		}

		public virtual TopologyInfo GetLocalServerTopology(bool isForceRefresh = false)
		{
			return null;
		}

		public void RefreshTopology(bool isForceRefresh = false)
		{
			TopologyInfo topology = null;
			bool flag = DxStoreRegistryConfigProvider.Tracer.IsTraceEnabled(TraceType.DebugTrace);
			Utils.RunOperation(this.ManagerConfig.Identity, "RefreshTopology", delegate
			{
				topology = this.GetLocalServerTopology(isForceRefresh);
			}, this.eventLogger, LogOptions.LogException, true, new TimeSpan?(TimeSpan.FromMinutes(1.0)), new TimeSpan?(this.ManagerConfig.Settings.PeriodicExceptionLoggingDuration), null, null, null);
			if (topology != null)
			{
				if (topology.IsConfigured)
				{
					if (flag)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>(0L, "RefreshConfig found a valid topology '{0}' members: {1}", topology.Name, topology.Members.JoinWithComma("<null>"));
					}
					if (!topology.IsAllMembersVersionCompatible)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string>(0L, "RefreshConfig found that some of the members are version compatible - will be retrying in next iteration", topology.Name);
						return;
					}
					bool flag2 = false;
					InstanceGroupMemberConfig[] configuredMembers = Utils.EmptyArray<InstanceGroupMemberConfig>();
					string[] serversToRemove = Utils.EmptyArray<string>();
					InstanceGroupConfig groupConfig = this.GetGroupConfig(topology.Name, false);
					if (groupConfig != null)
					{
						if (flag)
						{
							DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>(0L, "Group {0} already exist with configured members {1}", topology.Name, (from m in groupConfig.Members
							select m.Name).JoinWithComma("<null>"));
						}
						flag2 = true;
						configuredMembers = groupConfig.Members;
						if (!groupConfig.Settings.IsAppendOnlyMembership)
						{
							serversToRemove = (from m in configuredMembers
							let isFound = topology.Members.Any((string tm) => string.Equals(tm, m.Name))
							where !isFound && !m.IsManagedExternally
							select m.Name).ToArray<string>();
						}
					}
					string[] serversToAdd = (from tm in topology.Members
					let isFound = configuredMembers.Any((InstanceGroupMemberConfig m) => string.Equals(tm, m.Name))
					where !isFound
					select tm).ToArray<string>();
					this.UpdateMembers(topology.Name, serversToRemove, serversToAdd, !flag2, !flag2);
					return;
				}
				else
				{
					InstanceGroupConfig[] allGroupConfigs = this.GetAllGroupConfigs();
					foreach (InstanceGroupConfig instanceGroupConfig in allGroupConfigs)
					{
						if (instanceGroupConfig.IsMember(instanceGroupConfig.Self, true))
						{
							string[] array2 = (from member in instanceGroupConfig.Members
							where !member.IsManagedExternally
							select member into m
							select m.Name).ToArray<string>();
							if (flag)
							{
								DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>(0L, "{0}: Removing members '{1}' from group since local node is not part of group member any more", instanceGroupConfig.Identity, array2.JoinWithComma("<null>"));
							}
							this.RemoveMembers(instanceGroupConfig.Name, array2);
						}
					}
				}
			}
		}

		public void RemoveMembers(string groupName, string[] membersToRemove)
		{
			this.UpdateMembers(groupName, membersToRemove, null, false, false);
		}

		public void UpdateMembers(string groupName, string[] serversToRemove, string[] serversToAdd, bool isEnableGroup = false, bool isSetAsDefaultGroup = false)
		{
			bool flag = DxStoreRegistryConfigProvider.Tracer.IsTraceEnabled(TraceType.DebugTrace);
			bool flag2 = false;
			lock (this.locker)
			{
				if (serversToRemove != null && serversToRemove.Length > 0)
				{
					if (flag)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>(0L, "{0}: UpdateMembers - Removing members: '{1}'", groupName, serversToRemove.JoinWithComma("<null>"));
					}
					flag2 = true;
					foreach (string memberName in serversToRemove)
					{
						this.RemoveGroupMemberConfig(groupName, memberName);
					}
				}
				if (serversToAdd != null && serversToAdd.Length > 0)
				{
					IEnumerable<InstanceGroupMemberConfig> enumerable = serversToAdd.Select(delegate(string s)
					{
						string networkAddress = this.ManagerConfig.NameResolver.ResolveNameBestEffort(s) ?? s;
						return new InstanceGroupMemberConfig
						{
							Name = s,
							NetworkAddress = networkAddress
						};
					});
					if (flag)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>(0L, "{0}: UpdateMembers - Adding members: '{1}'", groupName, serversToAdd.JoinWithComma("<null>"));
					}
					flag2 = true;
					foreach (InstanceGroupMemberConfig cfg in enumerable)
					{
						this.SetGroupMemberConfig(groupName, cfg);
					}
				}
				if (isEnableGroup)
				{
					DxStoreRegistryConfigProvider.Tracer.TraceDebug<string>(0L, "{0}: UpdateMembers - Enabling group", groupName);
					flag2 = true;
					this.EnableGroup(groupName);
				}
				if (isSetAsDefaultGroup)
				{
					DxStoreRegistryConfigProvider.Tracer.TraceDebug<string>(0L, "{0}: UpdateMembers - Setting as default group", groupName);
					flag2 = true;
					this.SetDefaultGroupName(groupName);
				}
				if (!flag2)
				{
					DxStoreRegistryConfigProvider.Tracer.TraceDebug<string>(0L, "{0}: UpdateMembers - No configuration change detected", groupName);
				}
			}
		}

		public InstanceManagerConfig GetManagerConfig()
		{
			InstanceManagerConfig instanceManagerConfig = new InstanceManagerConfig();
			instanceManagerConfig.NameResolver = this;
			instanceManagerConfig.Self = this.Self;
			instanceManagerConfig.ComponentName = this.componentName;
			instanceManagerConfig.IsZeroboxMode = this.isZeroboxMode;
			instanceManagerConfig.NetworkAddress = instanceManagerConfig.NameResolver.ResolveNameBestEffort(this.Self);
			instanceManagerConfig.DefaultTimeout = new WcfTimeout();
			using (RegistryKey registryKey = this.OpenManagerConfigKey(false))
			{
				instanceManagerConfig.BaseStorageDir = Environment.ExpandEnvironmentVariables(RegUtils.GetProperty<string>(registryKey, "BaseStorageDir", this.DefaultStorageBaseDir));
				instanceManagerConfig.InstanceMonitorInterval = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "InstanceMonitorIntervalInMSec", TimeSpan.FromSeconds(15.0));
				instanceManagerConfig.EndpointPortNumber = RegUtils.GetProperty<int>(registryKey, "EndpointPortNumber", 808);
				instanceManagerConfig.EndpointProtocolName = RegUtils.GetProperty<string>(registryKey, "EndpointProtocolName", "net.tcp");
				instanceManagerConfig.DefaultTimeout = RegUtils.GetWcfTimeoutProperty(registryKey, "DefaultTimeout", new WcfTimeout());
				instanceManagerConfig.ManagerStopTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "ManagerStopTimeoutInMSec", TimeSpan.FromMinutes(1.0));
				CommonSettings defaultSettings = this.CreateDefaultCommonSettings(instanceManagerConfig.EndpointPortNumber, instanceManagerConfig.EndpointProtocolName, instanceManagerConfig.DefaultTimeout);
				instanceManagerConfig.Settings = new CommonSettings();
				this.GetInheritableSettings(this.GetManagerSettingsKeyName(), instanceManagerConfig.Settings, defaultSettings);
			}
			return instanceManagerConfig;
		}

		public InstanceGroupConfig GetGroupConfig(string groupName, bool isFillDefaultValueIfNotExist = false)
		{
			InstanceGroupConfig instanceGroupConfig = null;
			groupName = this.ResolveGroupName(groupName);
			if (string.IsNullOrEmpty(groupName) && isFillDefaultValueIfNotExist)
			{
				groupName = "B1563499-EA40-4101-A9E6-59A8EB26FF1E";
			}
			using (RegistryKey registryKey = this.OpenGroupConfigKey(groupName, false))
			{
				if (registryKey != null || isFillDefaultValueIfNotExist)
				{
					instanceGroupConfig = new InstanceGroupConfig();
					instanceGroupConfig.NameResolver = this;
					instanceGroupConfig.Self = this.Self;
					instanceGroupConfig.ComponentName = this.componentName;
					instanceGroupConfig.IsZeroboxMode = this.isZeroboxMode;
					instanceGroupConfig.Name = groupName;
					instanceGroupConfig.IsExistInConfigProvider = (registryKey != null);
					instanceGroupConfig.Identity = string.Format("Group/{0}/{1}/{2}", this.componentName, groupName, this.Self);
					string defaultGroupName = this.GetDefaultGroupName();
					instanceGroupConfig.IsAutomaticActionsAllowed = RegUtils.GetBoolProperty(registryKey, "IsAutomaticActionsAllowed", false);
					instanceGroupConfig.IsRestartRequested = RegUtils.GetBoolProperty(registryKey, "IsRestartRequested", false);
					instanceGroupConfig.IsConfigurationManagedExternally = RegUtils.GetBoolProperty(registryKey, "IsConfigurationManagedExternally", false);
					instanceGroupConfig.ConfigInProgressExpiryTime = RegUtils.GetTimeProperty(registryKey, "ConfigInProgressExpiryTime");
					instanceGroupConfig.IsDefaultGroup = Utils.IsEqual(groupName, defaultGroupName, StringComparison.OrdinalIgnoreCase);
					instanceGroupConfig.IsConfigurationReady = (DateTimeOffset.Now > instanceGroupConfig.ConfigInProgressExpiryTime);
					instanceGroupConfig.Members = this.GetGroupMemberConfigs(groupName);
					instanceGroupConfig.Settings = this.GetGroupSettings(groupName);
				}
			}
			return instanceGroupConfig;
		}

		public CommonSettings CreateDefaultCommonSettings(int defaultEndPointPortNumber, string defaultProtocolName, WcfTimeout defaultTimeout)
		{
			CommonSettings input = new CommonSettings
			{
				InstanceProcessName = this.DefaultInstanceProcessFullPath,
				AccessEndpointPortNumber = defaultEndPointPortNumber,
				AccessEndpointProtocolName = defaultProtocolName,
				StoreAccessWcfTimeout = defaultTimeout,
				StoreAccessHttpTimeoutInMSec = 60000,
				InstanceEndpointPortNumber = defaultEndPointPortNumber,
				InstanceEndpointProtocolName = defaultProtocolName,
				StoreInstanceWcfTimeout = defaultTimeout,
				DurationToWaitBeforeRestart = TimeSpan.FromMinutes(2.0),
				InstanceHealthCheckPeriodicInterval = TimeSpan.FromSeconds(30.0),
				TruncationPeriodicCheckInterval = TimeSpan.FromSeconds(15.0),
				TruncationLimit = 1000,
				TruncationPaddingLength = 500,
				StateMachineStopTimeout = TimeSpan.FromSeconds(30.0),
				LeaderPromotionTimeout = TimeSpan.FromSeconds(30.0),
				PaxosCommandExecutionTimeout = TimeSpan.FromSeconds(30.0),
				GroupHealthCheckDuration = TimeSpan.FromSeconds(15.0),
				GroupHealthCheckAggressiveDuration = TimeSpan.FromSeconds(10.0),
				GroupStatusWaitTimeout = TimeSpan.FromSeconds(15.0),
				MaxEntriesToKeep = 10,
				MaximumAllowedInstanceNumberLag = 5,
				DefaultHealthCheckRequiredNodePercent = 51,
				MemberReconfigureTimeout = TimeSpan.FromSeconds(30.0),
				PaxosUpdateTimeout = TimeSpan.FromSeconds(30.0),
				SnapshotUpdateInterval = TimeSpan.FromSeconds(30.0),
				PeriodicExceptionLoggingDuration = TimeSpan.FromMinutes(5.0),
				PeriodicTimeoutLoggingDuration = TimeSpan.FromMinutes(5.0),
				ServiceHostCloseTimeout = TimeSpan.FromSeconds(60.0),
				MaxAllowedLagToCatchup = 200,
				DefaultSnapshotFileName = "DxStoreSnapshot.xml",
				IsAllowDynamicReconfig = false,
				IsAppendOnlyMembership = true,
				IsKillInstanceProcessWhenParentDies = true,
				AdditionalLogOptions = LogOptions.None,
				InstanceStartHoldUpDuration = TimeSpan.FromHours(1.0),
				InstanceStartHoldupDurationMaxAllowedStarts = 10,
				InstanceStartSilenceDuration = TimeSpan.FromMinutes(5.0),
				InstanceMemoryCommitSizeLimitInMb = 500,
				IsUseHttpTransportForInstanceCommunication = true,
				IsUseHttpTransportForClientCommunication = true,
				IsUseBinarySerializerForClientCommunication = false,
				IsUseEncryption = true,
				StartupDelay = TimeSpan.Zero
			};
			return this.UpdateDefaultCommonSettings(input);
		}

		public virtual CommonSettings UpdateDefaultCommonSettings(CommonSettings input)
		{
			return input;
		}

		public void GetInheritableSettings(string keyName, CommonSettings settings, CommonSettings defaultSettings)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName))
			{
				settings.InstanceProcessName = RegUtils.GetProperty<string>(registryKey, "InstanceProcessName", defaultSettings.InstanceProcessName);
				settings.TruncationLimit = RegUtils.GetProperty<int>(registryKey, "TruncationLimit", defaultSettings.TruncationLimit);
				settings.TruncationPaddingLength = RegUtils.GetProperty<int>(registryKey, "TruncationPaddingLength", defaultSettings.TruncationPaddingLength);
				settings.DurationToWaitBeforeRestart = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "DurationToWaitBeforeRestartInMSec", defaultSettings.DurationToWaitBeforeRestart);
				settings.AccessEndpointPortNumber = RegUtils.GetProperty<int>(registryKey, "AccessEndpointPortNumber", defaultSettings.AccessEndpointPortNumber);
				settings.AccessEndpointProtocolName = RegUtils.GetProperty<string>(registryKey, "AccessEndpointProtocolName", defaultSettings.AccessEndpointProtocolName);
				settings.InstanceEndpointPortNumber = RegUtils.GetProperty<int>(registryKey, "InstanceEndpointPortNumber", defaultSettings.InstanceEndpointPortNumber);
				settings.InstanceEndpointProtocolName = RegUtils.GetProperty<string>(registryKey, "InstanceEndpointProtocolName", defaultSettings.InstanceEndpointProtocolName);
				settings.IsAllowDynamicReconfig = RegUtils.GetBoolProperty(registryKey, "IsAllowDynamicReconfig", defaultSettings.IsAllowDynamicReconfig);
				settings.IsAppendOnlyMembership = RegUtils.GetBoolProperty(registryKey, "IsAppendOnlyMembership", defaultSettings.IsAppendOnlyMembership);
				settings.IsKillInstanceProcessWhenParentDies = RegUtils.GetBoolProperty(registryKey, "IsKillInstanceProcessWhenParentDies", defaultSettings.IsKillInstanceProcessWhenParentDies);
				settings.StoreAccessWcfTimeout = RegUtils.GetWcfTimeoutProperty(registryKey, "StoreAccessWcfTimeout", defaultSettings.StoreAccessWcfTimeout);
				settings.StoreAccessHttpTimeoutInMSec = RegUtils.GetProperty<int>(registryKey, "StoreAccessHttpTimeoutInMSec", defaultSettings.StoreAccessHttpTimeoutInMSec);
				settings.StoreInstanceWcfTimeout = RegUtils.GetWcfTimeoutProperty(registryKey, "StoreInstanceWcfTimeout", defaultSettings.StoreInstanceWcfTimeout);
				settings.TruncationPeriodicCheckInterval = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "TruncationPeriodicCheckIntervalInMSec", defaultSettings.TruncationPeriodicCheckInterval);
				settings.InstanceHealthCheckPeriodicInterval = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "InstanceHealthCheckPeriodicIntervalInMSec", defaultSettings.InstanceHealthCheckPeriodicInterval);
				settings.StateMachineStopTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "StateMachineStopTimeoutInMSec", defaultSettings.StateMachineStopTimeout);
				settings.LeaderPromotionTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "LeaderPromotionTimeoutInMSec", defaultSettings.LeaderPromotionTimeout);
				settings.PaxosCommandExecutionTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "PaxosCommandExecutionTimeoutInMSec", defaultSettings.PaxosCommandExecutionTimeout);
				settings.GroupHealthCheckDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "GroupHealthCheckDurationInMSec", defaultSettings.GroupHealthCheckDuration);
				settings.GroupHealthCheckAggressiveDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "GroupHealthCheckAggressiveDurationInMSec", defaultSettings.GroupHealthCheckAggressiveDuration);
				settings.GroupStatusWaitTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "GroupStatusWaitTimeoutInMSec", defaultSettings.GroupStatusWaitTimeout);
				settings.MemberReconfigureTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "MemberReconfigureTimeoutInMSec", defaultSettings.MemberReconfigureTimeout);
				settings.PaxosUpdateTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "PaxosUpdateTimeoutInMSec", defaultSettings.PaxosUpdateTimeout);
				settings.SnapshotUpdateInterval = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "SnapshotUpdateIntervalInMSec", defaultSettings.SnapshotUpdateInterval);
				settings.PeriodicExceptionLoggingDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "PeriodicExceptionLoggingDurationInMSec", defaultSettings.PeriodicExceptionLoggingDuration);
				settings.PeriodicTimeoutLoggingDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "PeriodicTimeoutLoggingDurationInMSec", defaultSettings.PeriodicTimeoutLoggingDuration);
				settings.ServiceHostCloseTimeout = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "ServiceHostCloseTimeoutInMSec", defaultSettings.ServiceHostCloseTimeout);
				settings.MaxAllowedLagToCatchup = RegUtils.GetProperty<int>(registryKey, "MaxAllowedLagToCatchup", defaultSettings.MaxAllowedLagToCatchup);
				settings.DefaultSnapshotFileName = RegUtils.GetProperty<string>(registryKey, "DefaultSnapshotFileName", defaultSettings.DefaultSnapshotFileName);
				settings.MaxEntriesToKeep = RegUtils.GetProperty<int>(registryKey, "MaxEntriesToKeep", defaultSettings.MaxEntriesToKeep);
				settings.MaximumAllowedInstanceNumberLag = RegUtils.GetProperty<int>(registryKey, "MaximumAllowedInstanceNumberLag", defaultSettings.MaximumAllowedInstanceNumberLag);
				settings.DefaultHealthCheckRequiredNodePercent = RegUtils.GetProperty<int>(registryKey, "DefaultHealthCheckRequiredNodePercent", defaultSettings.DefaultHealthCheckRequiredNodePercent);
				settings.AdditionalLogOptions = (LogOptions)RegUtils.GetProperty<int>(registryKey, "AdditionalLogOptions", defaultSettings.AdditionalLogOptionsAsInt);
				settings.InstanceStartSilenceDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "InstanceStartSilenceDurationInMSec", defaultSettings.InstanceStartSilenceDuration);
				settings.InstanceStartHoldupDurationMaxAllowedStarts = RegUtils.GetProperty<int>(registryKey, "InstanceStartHoldupDurationMaxAllowedStarts", defaultSettings.InstanceStartHoldupDurationMaxAllowedStarts);
				settings.InstanceStartHoldUpDuration = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "InstanceStartHoldUpDurationInMSec", defaultSettings.InstanceStartHoldUpDuration);
				settings.InstanceMemoryCommitSizeLimitInMb = RegUtils.GetProperty<int>(registryKey, "InstanceMemoryCommitSizeLimitInMb", defaultSettings.InstanceMemoryCommitSizeLimitInMb);
				settings.IsUseHttpTransportForInstanceCommunication = RegUtils.GetBoolProperty(registryKey, "IsUseHttpTransportForInstanceCommunication", defaultSettings.IsUseHttpTransportForInstanceCommunication);
				settings.IsUseHttpTransportForClientCommunication = RegUtils.GetBoolProperty(registryKey, "IsUseHttpTransportForClientCommunication", defaultSettings.IsUseHttpTransportForClientCommunication);
				settings.IsUseBinarySerializerForClientCommunication = RegUtils.GetBoolProperty(registryKey, "IsUseBinarySerializerForClientCommunication", defaultSettings.IsUseBinarySerializerForClientCommunication);
				settings.IsUseEncryption = RegUtils.GetBoolProperty(registryKey, "IsUseEncryption", defaultSettings.IsUseEncryption);
				settings.StartupDelay = RegUtils.GetLongPropertyAsTimeSpan(registryKey, "StartupDelayInMSec", defaultSettings.StartupDelay);
			}
		}

		public InstanceGroupSettings GetGroupSettings(string groupName)
		{
			InstanceGroupSettings instanceGroupSettings = new InstanceGroupSettings();
			this.GetInheritableSettings(this.GetGroupSettingsKeyName(groupName), instanceGroupSettings, this.ManagerConfig.Settings);
			using (RegistryKey registryKey = this.OpenGroupSettingsKey(groupName, false))
			{
				instanceGroupSettings.PaxosStorageDir = Environment.ExpandEnvironmentVariables(RegUtils.GetProperty<string>(registryKey, "PaxosStorageDir", this.ConstructDefaultStorageDir(groupName, "Paxos")));
				instanceGroupSettings.SnapshotStorageDir = Environment.ExpandEnvironmentVariables(RegUtils.GetProperty<string>(registryKey, "SnapshotStorageDir", this.ConstructDefaultStorageDir(groupName, "Snapshot")));
			}
			return instanceGroupSettings;
		}

		public InstanceGroupMemberConfig[] GetGroupMemberConfigs(string groupName)
		{
			List<InstanceGroupMemberConfig> list = new List<InstanceGroupMemberConfig>();
			string[] groupMemberNames = this.GetGroupMemberNames(groupName);
			foreach (string text in groupMemberNames)
			{
				InstanceGroupMemberConfig instanceGroupMemberConfig = new InstanceGroupMemberConfig
				{
					Name = text
				};
				using (RegistryKey registryKey = this.OpenGroupMemberConfigKey(groupName, text, false))
				{
					instanceGroupMemberConfig.NetworkAddress = RegUtils.GetProperty<string>(registryKey, "NetworkAddress", string.Empty);
					instanceGroupMemberConfig.IsWitness = RegUtils.GetBoolProperty(registryKey, "IsWitness", false);
					instanceGroupMemberConfig.IsManagedExternally = RegUtils.GetBoolProperty(registryKey, "IsManagedExternally", false);
					list.Add(instanceGroupMemberConfig);
				}
			}
			return list.ToArray();
		}

		public string GetDefaultGroupName()
		{
			string property;
			using (RegistryKey registryKey = this.OpenGroupsContainerKey(false))
			{
				property = RegUtils.GetProperty<string>(registryKey, "DefaultGroupName", string.Empty);
			}
			return property;
		}

		public void SetDefaultGroupName(string groupName)
		{
			using (RegistryKey registryKey = this.OpenGroupsContainerKey(true))
			{
				RegUtils.SetProperty<string>(registryKey, "DefaultGroupName", groupName);
			}
		}

		public void RemoveDefaultGroupName()
		{
			using (RegistryKey registryKey = this.OpenGroupsContainerKey(true))
			{
				if (registryKey != null)
				{
					registryKey.DeleteValue("DefaultGroupName");
				}
			}
		}

		public void DisableGroup(string groupName)
		{
			using (RegistryKey registryKey = this.OpenGroupConfigKey(groupName, true))
			{
				RegUtils.SetProperty<bool>(registryKey, "IsAutomaticActionsAllowed", false);
			}
		}

		public void EnableGroup(string groupName)
		{
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupConfigKey(groupName, true))
				{
					RegUtils.SetProperty<bool>(registryKey, "IsAutomaticActionsAllowed", true);
				}
			}
		}

		public void SetRestartRequired(string groupName, bool isRestartRequired)
		{
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupConfigKey(groupName, true))
				{
					RegUtils.SetProperty<bool>(registryKey, "IsRestartRequested", isRestartRequired);
				}
			}
		}

		public void SetGroupMemberConfig(string groupName, InstanceGroupMemberConfig cfg)
		{
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupMemberConfigKey(groupName, cfg.Name, true))
				{
					RegUtils.SetProperty<bool>(registryKey, "IsWitness", cfg.IsWitness);
					if (!string.IsNullOrEmpty(cfg.NetworkAddress))
					{
						RegUtils.SetProperty<string>(registryKey, "NetworkAddress", cfg.NetworkAddress);
					}
				}
			}
		}

		public string[] GetAllGroupNames()
		{
			string[] result = Utils.EmptyArray<string>();
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupsContainerKey(false))
				{
					if (registryKey != null)
					{
						result = registryKey.GetSubKeyNames();
					}
				}
			}
			return result;
		}

		public string[] GetGroupMemberNames(string groupName)
		{
			string[] result = Utils.EmptyArray<string>();
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupMembersContainerKey(groupName, false))
				{
					if (registryKey != null)
					{
						result = registryKey.GetSubKeyNames();
					}
				}
			}
			return result;
		}

		public InstanceGroupConfig[] GetAllGroupConfigs()
		{
			List<InstanceGroupConfig> list = new List<InstanceGroupConfig>();
			lock (this.locker)
			{
				string[] allGroupNames = this.GetAllGroupNames();
				list.AddRange(from groupName in allGroupNames
				select this.GetGroupConfig(groupName, false));
			}
			return list.ToArray();
		}

		public void RemoveGroupConfig(string groupName)
		{
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupsContainerKey(true))
				{
					if (registryKey != null)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string>((long)groupName.GetHashCode(), "{0}/{1}: Removing group", groupName, this.Self);
						registryKey.DeleteSubKeyTree(groupName, false);
						if (Utils.IsEqual(groupName, this.GetDefaultGroupName(), StringComparison.OrdinalIgnoreCase))
						{
							this.RemoveDefaultGroupName();
						}
					}
				}
			}
		}

		public void RemoveGroupMemberConfig(string groupName, string memberName)
		{
			lock (this.locker)
			{
				using (RegistryKey registryKey = this.OpenGroupMembersContainerKey(groupName, true))
				{
					if (registryKey != null)
					{
						DxStoreRegistryConfigProvider.Tracer.TraceDebug<string, string, string>((long)groupName.GetHashCode(), "{0}/{1}: Removing member: {2}", groupName, this.Self, memberName);
						registryKey.DeleteSubKeyTree(memberName, false);
					}
				}
			}
		}

		internal string GetManagerSettingsKeyName()
		{
			return Utils.CombinePathNullSafe(this.ManagerConfigKeyName, "Settings");
		}

		internal string GetGroupsContainerKeyName()
		{
			return Utils.CombinePathNullSafe(this.ManagerConfigKeyName, "Groups");
		}

		internal string GetGroupConfigKeyName(string groupName)
		{
			return Utils.CombinePathNullSafe(this.GetGroupsContainerKeyName(), groupName);
		}

		internal string GetGroupSettingsKeyName(string groupName)
		{
			return Utils.CombinePathNullSafe(this.GetGroupConfigKeyName(groupName), "Settings");
		}

		internal string GetGroupMembersContainerKeyName(string groupName)
		{
			return Utils.CombinePathNullSafe(this.GetGroupConfigKeyName(groupName), "Members");
		}

		internal string GetGroupMemberConfigKeyName(string groupName, string memberName)
		{
			return Utils.CombinePathNullSafe(this.GetGroupMembersContainerKeyName(groupName), memberName);
		}

		internal RegistryKey OpenKey(string keyName, bool isWritable)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(keyName, isWritable);
				if (registryKey == null && isWritable)
				{
					registryKey = Registry.LocalMachine.CreateSubKey(keyName);
				}
			}
			catch (Exception ex)
			{
				DxStoreRegistryConfigProvider.Tracer.TraceError<string, string>(0L, "Failed to open key {0} - error: {1}", keyName, ex.Message);
			}
			return registryKey;
		}

		internal RegistryKey OpenManagerConfigKey(bool isWritable = false)
		{
			return this.OpenKey(this.ManagerConfigKeyName, isWritable);
		}

		internal RegistryKey OpenGroupsContainerKey(bool isWritable = false)
		{
			return this.OpenKey(this.GetGroupsContainerKeyName(), isWritable);
		}

		internal RegistryKey OpenGroupConfigKey(string groupName, bool isWritable = false)
		{
			return this.OpenKey(this.GetGroupConfigKeyName(groupName), isWritable);
		}

		internal RegistryKey OpenGroupSettingsKey(string groupName, bool isWritable = false)
		{
			return this.OpenKey(this.GetGroupSettingsKeyName(groupName), isWritable);
		}

		internal RegistryKey OpenGroupMembersContainerKey(string groupName, bool isWritable = false)
		{
			return this.OpenKey(this.GetGroupMembersContainerKeyName(groupName), isWritable);
		}

		internal RegistryKey OpenGroupMemberConfigKey(string groupName, string memberName, bool isWritable = false)
		{
			return this.OpenKey(this.GetGroupMemberConfigKeyName(groupName, memberName), isWritable);
		}

		private string ResolveGroupName(string groupName)
		{
			if (string.IsNullOrEmpty(groupName) || Utils.IsEqual(groupName, "B1563499-EA40-4101-A9E6-59A8EB26FF1E", StringComparison.OrdinalIgnoreCase))
			{
				string defaultGroupName = this.GetDefaultGroupName();
				if (!string.IsNullOrEmpty(defaultGroupName))
				{
					groupName = defaultGroupName;
				}
			}
			return groupName;
		}

		private string ConstructDefaultStorageDir(string groupName, string storageType)
		{
			return Utils.CombinePathNullSafe(this.DefaultStorageBaseDir, string.Format("{0}\\Storage\\{1}", groupName, storageType));
		}

		private readonly object locker = new object();

		private string componentName;

		private bool isZeroboxMode;

		private IDxStoreEventLogger eventLogger;
	}
}
