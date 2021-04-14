using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmConfigManager : ChangePoller
	{
		internal AmConfig CurrentConfig
		{
			get
			{
				return this.m_cfg;
			}
		}

		internal AmConfigManager(IReplayAdObjectLookup adLookup) : base(true)
		{
			this.LastKnownConfig = AmConfig.UnknownConfig;
			this.RefreshConfigEvent = new AutoResetEvent(false);
			this.ForceRefreshConfigEvent = new AutoResetEvent(false);
			this.MommyCurfue = ExDateTime.MinValue;
			this.CachedFswBootTime = DateTime.MinValue;
			this.AdLookup = adLookup;
			this.m_selfDismounter = new AmSelfDismounter(this);
			this.m_selfDismounter.Start();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmConfigManagerTracer;
			}
		}

		public IReplayAdObjectLookup AdLookup { get; private set; }

		internal AmConfig LastKnownConfig
		{
			get
			{
				return this.m_cfg;
			}
			private set
			{
				this.m_cfg = value;
			}
		}

		internal AutoResetEvent RefreshConfigEvent { get; private set; }

		internal AutoResetEvent ForceRefreshConfigEvent { get; private set; }

		internal bool MommyMayIMount { get; private set; }

		internal ExDateTime MommyCurfue { get; private set; }

		internal DateTime CachedFswBootTime { get; private set; }

		internal int FswBootTimeCounter { get; private set; }

		private static TimeSpan GetRefreshInterval(bool isUnknownConfig)
		{
			if (!isUnknownConfig)
			{
				return AmConfigManager.PeriodicRefreshIntervalLong;
			}
			return AmConfigManager.PeriodicRefreshIntervalShort;
		}

		internal static IDisposable SetRefreshIntervalTestHook(Func<bool, TimeSpan> testHook)
		{
			return AmConfigManager.getRefreshIntervalTestHook.SetTestHook(testHook);
		}

		internal static DateTime ReadBootTimeFromRegistry()
		{
			long bootTimeCookie = RegistryParameters.BootTimeCookie;
			return DateTime.FromBinary(bootTimeCookie);
		}

		internal static DateTime ReadFswBootTimeFromRegistry()
		{
			long bootTimeFswCookie = RegistryParameters.BootTimeFswCookie;
			return DateTime.FromBinary(bootTimeFswCookie);
		}

		internal static void SetBootTimeToRegistry(DateTime bootTimeUtc)
		{
			long num = bootTimeUtc.ToBinary();
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", ref ex);
			if (ex != null)
			{
				throw ex;
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					registryKey.SetValue("BootTimeCookie", num, RegistryValueKind.QWord);
				}
			}
		}

		internal static void SetFswBootTimeToRegistry(DateTime bootTimeUtc)
		{
			long num = bootTimeUtc.ToBinary();
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", ref ex);
			if (ex != null)
			{
				throw ex;
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					registryKey.SetValue("BootTimeFswCookie", num, RegistryValueKind.QWord);
				}
			}
		}

		internal void RefreshConfig(bool isForceRefresh, string reason)
		{
			int num = 0;
			while (num < 5 && !this.m_fShutdown)
			{
				this.m_isClusterObjectCreationFailed = false;
				this.RefreshConfigInternal(isForceRefresh, reason);
				if (!this.m_isClusterObjectCreationFailed)
				{
					break;
				}
				Thread.Sleep(TimeSpan.FromSeconds(2.0));
				num++;
			}
		}

		internal void TriggerRefresh(bool isForce)
		{
			if (isForce)
			{
				this.ForceRefreshConfigEvent.Set();
				return;
			}
			this.RefreshConfigEvent.Set();
		}

		internal void AllowAutoMount(string reason)
		{
			AmTrace.Debug("AllowAutoMount called: {0}", new object[]
			{
				reason
			});
			this.MommyMayIMount = true;
			DateTime localBootTime = Dependencies.ManagementClassHelper.LocalBootTime;
			AmConfigManager.SetBootTimeToRegistry(localBootTime);
		}

		internal void ProhibitAutoMount(string reason)
		{
			AmTrace.Debug("ProhibitAutoMount called: {0}", new object[]
			{
				reason
			});
			this.MommyMayIMount = false;
			AmConfigManager.SetBootTimeToRegistry(DateTime.MinValue);
			AmConfigManager.SetFswBootTimeToRegistry(DateTime.MinValue);
		}

		internal void SetCurfue()
		{
			this.MommyCurfue = ExDateTime.UtcNow.Add(this.StopDagEffectiveTimeSpan);
		}

		protected override void PollerThread()
		{
			AmTrace.Entering("AmConfigManager.PollerThread", new object[0]);
			bool isForceRefresh = true;
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				this.m_shutdownEvent,
				this.ForceRefreshConfigEvent,
				this.RefreshConfigEvent
			};
			while (!this.m_fShutdown)
			{
				Exception ex = null;
				try
				{
					this.RefreshConfig(isForceRefresh, "AmConfigManager.PollerThread");
				}
				catch (ClusterException ex2)
				{
					ex = ex2;
				}
				catch (DataSourceTransientException ex3)
				{
					ex = ex3;
				}
				catch (DataSourceOperationException ex4)
				{
					ex = ex4;
				}
				catch (TransientException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					AmTrace.Error("Encountered an exception while trying to refresh configuration. (exception={0})", new object[]
					{
						ex
					});
				}
				TimeSpan timeout = AmConfigManager.getRefreshIntervalTestHook.Value(this.m_cfg.IsUnknown);
				int num = WaitHandle.WaitAny(waitHandles, timeout, false);
				isForceRefresh = false;
				if (num == 0)
				{
					AmTrace.Debug("Config manager shutdown event triggered", new object[0]);
					break;
				}
				if (num == 1)
				{
					AmTrace.Debug("Force refresh event triggerred", new object[0]);
					isForceRefresh = true;
				}
				else if (num == 2)
				{
					AmTrace.Debug("Refresh event triggerred", new object[0]);
				}
			}
			if (this.m_fShutdown)
			{
				if (!this.LastKnownConfig.IsUnknown)
				{
					this.RefreshConfig(true, "AmConfigManager shutdown");
				}
				this.m_selfDismounter.CancelDismountAllRequest();
				this.m_selfDismounter.Stop();
			}
			if (this.m_lastValidGroup != null)
			{
				this.m_lastValidGroup.Dispose();
				this.m_lastValidGroup = null;
			}
			if (AmSystemManager.Instance.ClusterMonitor != null)
			{
				AmSystemManager.Instance.ClusterMonitor.Stop();
			}
			AmTrace.Leaving("AmConfigManager.PollerThread", new object[0]);
		}

		private bool InjectUnknownIfRequired(ref AmRole role, ref bool unknownIsTriggeredByADFailure)
		{
			string text = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2842045757U, ref text);
			if (!string.IsNullOrEmpty(text))
			{
				AmTrace.Error("Fault injection injected config error: {0}", new object[]
				{
					text
				});
				role = AmRole.Unknown;
				if (string.Equals(text, "Unknown Triggered by AD Error", StringComparison.OrdinalIgnoreCase))
				{
					if (ClusterFactory.Instance.IsRunning())
					{
						using (IAmCluster amCluster = ClusterFactory.Instance.Open())
						{
							AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = new AmClusterNodeNetworkStatus();
							amClusterNodeNetworkStatus.HasADAccess = false;
							Exception ex = AmClusterNodeStatusAccessor.Write(amCluster, AmServerName.LocalComputerName, amClusterNodeNetworkStatus);
							if (ex != null)
							{
								ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<string, string>(amClusterNodeNetworkStatus.ToString(), ex.Message);
							}
						}
					}
					unknownIsTriggeredByADFailure = true;
				}
				return true;
			}
			return false;
		}

		private void RefreshConfigInternal(bool isForceRefresh, string reason)
		{
			string text = null;
			AmRole amRole = AmRole.Unknown;
			bool flag = false;
			AmServerName[] array = null;
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = null;
			IAmCluster amCluster = null;
			IAmClusterGroup amClusterGroup = null;
			IAmDbState amDbState = null;
			IAmDbState amDbState2 = null;
			AmServerName amServerName = null;
			AmDagConfig amDagConfig = null;
			ADObjectId adobjectId = null;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Exception ex = null;
			string text2 = null;
			AmTrace.Debug("Starting to refresh config (force={0}, reason={1})", new object[]
			{
				isForceRefresh,
				reason
			});
			if (isForceRefresh)
			{
				Dependencies.ADConfig.Refresh(reason);
			}
			bool flag5 = false;
			IAmClusterGroup amClusterGroup2 = null;
			IEnumerable<AmServerName> serversInMaintenance = null;
			try
			{
				try
				{
					if (this.InjectUnknownIfRequired(ref amRole, ref flag4))
					{
						goto IL_46E;
					}
					if (this.m_fShutdown)
					{
						AmTrace.Error("Active manager is shutting down. Setting the current config to Unknown", new object[0]);
						text = ReplayStrings.AmServiceShuttingDown;
						amRole = AmRole.Unknown;
						goto IL_46E;
					}
					IADServer localServer = Dependencies.ADConfig.GetLocalServer();
					if (localServer == null)
					{
						AmTrace.Error("Unable to find server object for the local server", new object[0]);
						amRole = AmRole.Unknown;
						text = ReplayStrings.ErrorFailedToFindLocalServer;
						goto IL_46E;
					}
					adobjectId = localServer.DatabaseAvailabilityGroup;
					if (adobjectId == null)
					{
						AmTrace.Debug("This machine is running in Standalone mode", new object[0]);
						amRole = AmRole.Standalone;
						if (!this.LastKnownConfig.IsStandalone)
						{
							amDbState2 = new AmStandaloneDbState();
							amDbState = amDbState2;
						}
						else
						{
							amDbState = this.LastKnownConfig.DbState;
						}
						goto IL_46E;
					}
					iaddatabaseAvailabilityGroup = Dependencies.ADConfig.GetLocalDag();
					if (iaddatabaseAvailabilityGroup == null)
					{
						AmTrace.Error("Local server is pointing to a invalid dag", new object[0]);
						text = ReplayStrings.ErrorDagMisconfiguredForAmConfig(localServer.Name, adobjectId.Name);
						amRole = AmRole.Unknown;
						goto IL_46E;
					}
					if (text2 == null || !SharedHelper.StringIEquals(text2, iaddatabaseAvailabilityGroup.Name))
					{
						text2 = iaddatabaseAvailabilityGroup.Name;
						this.UpdateLastKnownDagName(text2);
					}
					array = this.GetDagMembers(iaddatabaseAvailabilityGroup);
					if (array.Length == 0)
					{
						AmTrace.Error("Local server is part of the dag, but the dag pointing doesn't have any members", new object[0]);
						text = ReplayStrings.ErrorDagDoesNotHaveAnyMemberServers;
						amRole = AmRole.Unknown;
						goto IL_46E;
					}
					bool flag6 = ClusterFactory.Instance.IsRunning();
					bool flag7 = iaddatabaseAvailabilityGroup.DatacenterActivationMode == DatacenterActivationModeOption.DagOnly;
					if (flag7)
					{
						bool flag8 = iaddatabaseAvailabilityGroup.StoppedMailboxServers.Contains(AmServerName.LocalComputerName.Fqdn, StringComparer.OrdinalIgnoreCase);
						bool flag9 = iaddatabaseAvailabilityGroup.StartedMailboxServers.Contains(AmServerName.LocalComputerName.Fqdn, StringComparer.OrdinalIgnoreCase);
						if (flag8)
						{
							if (flag6)
							{
								AmTrace.Warning("Dag is in DAC mode and local server is in the stopped servers list- Cluster service should not be running on the local node, but it appears to be running", new object[0]);
							}
							AmTrace.Info("Dag is in DAC mode and local server is in the stopped servers list - AM should not be in any valid role", new object[0]);
							amRole = AmRole.Unknown;
							text = ReplayStrings.ServerStopped(AmServerName.LocalComputerName.Fqdn);
							goto IL_46E;
						}
						if (!flag9)
						{
							AmEvtRejoinNodeForDac amEvtRejoinNodeForDac = new AmEvtRejoinNodeForDac(iaddatabaseAvailabilityGroup, localServer);
							amEvtRejoinNodeForDac.Notify(false);
						}
					}
					if (!flag6)
					{
						AmTrace.Error("Cluster service is not running", new object[0]);
						amRole = AmRole.Unknown;
						text = ReplayStrings.ErrorClusterServiceNotRunningForAmConfig;
						goto IL_46E;
					}
					AmTrace.Debug("This server is running in DAG configuration", new object[0]);
					flag = true;
					amDagConfig = this.LastKnownConfig.DagConfig;
					if (amDagConfig != null)
					{
						flag3 = !amDagConfig.Cluster.IsRefreshRequired;
					}
					if (!isForceRefresh && flag3)
					{
						amCluster = amDagConfig.Cluster;
						amClusterGroup = this.m_lastValidGroup;
						amDbState = this.LastKnownConfig.DbState;
						flag2 = true;
					}
					else if (!this.CreateClusterObjects(out amCluster, out amClusterGroup, out amDbState, out text))
					{
						this.m_isClusterObjectCreationFailed = true;
						amRole = AmRole.Unknown;
						goto IL_46E;
					}
					amServerName = amClusterGroup.OwnerNode;
					if (AmServerName.IsNullOrEmpty(amServerName))
					{
						AmTrace.Error("Group owner is null or empty. Not able to identify the current PAM", new object[0]);
						amRole = AmRole.Unknown;
						text = ReplayStrings.ErrorInvalidPamServerName;
						goto IL_46E;
					}
					serversInMaintenance = DagHelper.EnumeratePausedNodes(amCluster);
					if (flag7)
					{
						DateTime fswBootTime = this.GetFswBootTime(amCluster, isForceRefresh);
						bool flag10 = this.DetermineAutomountConsensus(amCluster, fswBootTime);
						if (!flag10)
						{
							AmTrace.Error("Automount consensus not reached, going to Unknown AM role.", new object[0]);
							text = ReplayStrings.ErrorAutomountConsensusNotReached;
							amRole = AmRole.Unknown;
							goto IL_46E;
						}
						AmTrace.Debug("RefreshConfigInternal: The Automount consensus is true.", new object[0]);
						AmConfigManager.SetFswBootTimeToRegistry(fswBootTime);
						if (!this.CheckRemoteSiteConnected(iaddatabaseAvailabilityGroup, localServer, amDbState))
						{
							int num;
							int num2;
							if (!AmTransientFailoverSuppressor.CheckIfMajorityNodesReachable(array, out num, out num2))
							{
								AmTrace.Error("CheckRemoteSiteConnected failed and majority node check failed.", new object[0]);
								ReplayCrimsonEvents.SiteDisconnectedMajorityNotReachable.LogPeriodic<int, int>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, num, num2);
							}
							else
							{
								AmTrace.Error("CheckRemoteSiteConnected failed but majority node check succeeded.", new object[0]);
								ReplayCrimsonEvents.SiteDisconnectedMajorityReachable.LogPeriodic<int, int>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, num, num2);
							}
						}
					}
					if (AmServerName.IsEqual(AmServerName.LocalComputerName, amServerName))
					{
						amRole = AmRole.PAM;
					}
					else
					{
						amRole = AmRole.SAM;
					}
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADExternalException ex3)
				{
					ex = ex3;
				}
				catch (ADOperationException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					AmTrace.Error("AD failure triggers the Unknown role: {0}", new object[]
					{
						ex
					});
					amRole = AmRole.Unknown;
					flag4 = true;
					text = ex.Message;
				}
				IL_46E:
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4058393917U);
				if (amRole != AmRole.Standalone && AmSystemManager.Instance.ClusterMonitor != null)
				{
					AmSystemManager.Instance.ClusterMonitor.ReportHasADAccess(!flag4);
				}
				AmConfig amConfig = null;
				if (amRole != AmRole.Unknown)
				{
					if (flag)
					{
						amDagConfig = new AmDagConfig(adobjectId, array, amServerName, amCluster, iaddatabaseAvailabilityGroup.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled);
						amClusterGroup2 = this.m_lastValidGroup;
						this.m_lastValidGroup = amClusterGroup;
					}
					else
					{
						amDagConfig = null;
						amClusterGroup2 = this.m_lastValidGroup;
						this.m_lastValidGroup = null;
					}
					amConfig = new AmConfig(amRole, amDbState, amDagConfig, text);
				}
				else
				{
					if (amDbState2 != null)
					{
						amDbState2.Dispose();
						amDbState2 = null;
					}
					amConfig = new AmConfig();
					if (text != null)
					{
						amConfig.LastError = text;
					}
					amConfig.IsUnknownTriggeredByADError = flag4;
					amClusterGroup2 = this.m_lastValidGroup;
					this.m_lastValidGroup = null;
				}
				AmConfig cfg = this.m_cfg;
				if (cfg.Role == amConfig.Role)
				{
					amConfig.TimeRoleLastChanged = cfg.TimeRoleLastChanged;
				}
				if (cfg.IsPamOrSam && amConfig.IsPamOrSam && !flag3)
				{
					AmTrace.Debug("Injecting a fake unknown role before taking up the new objects (prevRole={0})", new object[]
					{
						cfg.Role
					});
					AmConfig amConfig2 = new AmConfig(ReplayStrings.ErrorAmInjectedUnknownConfig);
					AmConfigChangedFlags changeFlags = AmConfig.CheckForChanges(cfg, amConfig2);
					lock (this.m_locker)
					{
						cfg.IsCurrentConfiguration = false;
						this.m_cfg = amConfig2;
						this.ReportConfigDetails(changeFlags, cfg, serversInMaintenance);
						this.NotifyConfigChange(changeFlags, cfg, amConfig2, text2);
						cfg = this.m_cfg;
					}
				}
				AmConfigChangedFlags amConfigChangedFlags = AmConfig.CheckForChanges(cfg, amConfig);
				if (amConfigChangedFlags != AmConfigChangedFlags.None)
				{
					AmTrace.Debug("Configuration change detected. (flags={0})", new object[]
					{
						amConfigChangedFlags
					});
				}
				if (isForceRefresh || amConfigChangedFlags != AmConfigChangedFlags.None)
				{
					lock (this.m_locker)
					{
						cfg.IsCurrentConfiguration = false;
						this.m_cfg = amConfig;
						if (this.m_cfg.Role == AmRole.Unknown)
						{
							if (this.m_isFirstTimeUnknown || this.m_cfg.Role != cfg.Role)
							{
								this.m_isFirstTimeUnknown = false;
								this.m_selfDismounter.ScheduleDismountAllRequest();
							}
						}
						else
						{
							this.m_selfDismounter.CancelDismountAllRequest();
						}
						AmTrace.Info("Notifying SystemManager about the configuration change (flags={0})", new object[]
						{
							amConfigChangedFlags
						});
						this.NotifyConfigChange(amConfigChangedFlags, cfg, amConfig, text2);
						this.QueueDisposeRequestIfRequired(cfg, amConfig);
					}
					if (amConfig.IsStandalone && AmSystemManager.Instance.ClusterMonitor != null)
					{
						AmSystemManager.Instance.ClusterMonitor.Stop();
					}
				}
				else
				{
					if (amDagConfig != null)
					{
						amDagConfig.Cluster = null;
					}
					amConfig.DbState = null;
					if (amConfig.IsUnknown)
					{
						this.CheckForUnknownButGroupOwner();
					}
				}
				if (isForceRefresh || amConfigChangedFlags != AmConfigChangedFlags.None || this.m_cfg.PeriodicEventWatch.Elapsed.TotalSeconds > (double)RegistryParameters.AmPeriodicRoleReportingIntervalInSec)
				{
					this.m_cfg.PeriodicEventWatch.Reset();
					this.ReportConfigDetails(amConfigChangedFlags, cfg, serversInMaintenance);
					this.m_cfg.PeriodicEventWatch.Start();
				}
				else
				{
					TimeSpan arg = ExDateTime.Now.Subtract(this.m_cfg.TimeCreated);
					AmConfigManager.Tracer.TraceDebug<AmRole, TimeSpan>(0L, "Active manager is running as {0} for duration ({1})", amRole, arg);
				}
				flag5 = true;
				if (this.m_cfg.IsPamOrSam && AmSystemManager.Instance.ClusterMonitor != null)
				{
					AmSystemManager.Instance.ClusterMonitor.Start(amCluster);
				}
			}
			finally
			{
				if (amClusterGroup2 != null && !object.ReferenceEquals(amClusterGroup2, this.m_lastValidGroup))
				{
					if (object.ReferenceEquals(amClusterGroup2, amClusterGroup))
					{
						amClusterGroup = null;
					}
					amClusterGroup2.Dispose();
					amClusterGroup2 = null;
				}
				if (!flag5 || amRole == AmRole.Unknown)
				{
					if (!flag2)
					{
						if (amDbState != null)
						{
							amDbState.Dispose();
							amDbState = null;
						}
						if (amClusterGroup != null)
						{
							amClusterGroup.Dispose();
							amClusterGroup = null;
						}
						if (amCluster != null)
						{
							amCluster.Dispose();
							amCluster = null;
						}
					}
					if (AmSystemManager.Instance.ClusterMonitor != null)
					{
						AmSystemManager.Instance.ClusterMonitor.Stop();
					}
				}
			}
		}

		private void NotifyConfigChange(AmConfigChangedFlags changeFlags, AmConfig prevCfg, AmConfig currentCfg, string dagName)
		{
			this.UpdatePerfmonCounters(currentCfg, dagName);
			this.NotifyDxStoreManager(changeFlags);
			AmEvtConfigChanged amEvtConfigChanged = new AmEvtConfigChanged(changeFlags, prevCfg, currentCfg);
			amEvtConfigChanged.Notify(true);
		}

		private void NotifyDxStoreManager(AmConfigChangedFlags changeFlags)
		{
			IActiveManagerSettings settings = DxStoreSetting.Instance.GetSettings();
			if ((settings.DxStoreRunMode == DxStoreMode.Primary || settings.DxStoreRunMode == DxStoreMode.Shadow) && ((changeFlags & AmConfigChangedFlags.Role) == AmConfigChangedFlags.Role || (changeFlags & AmConfigChangedFlags.MemberServers) == AmConfigChangedFlags.MemberServers))
			{
				Task.Factory.StartNew(delegate()
				{
					DistributedStore.Instance.TriggerDistributedStoreManagerRefreshBestEffort(string.Format("Configuration changed ({0})", changeFlags, true), true);
				});
			}
		}

		private void UpdatePerfmonCounters(AmConfig cfg, string dagName)
		{
			ActiveManagerServerPerfmon.ActiveManagerRole.RawValue = (long)cfg.Role;
			if (cfg.Role != AmRole.Standalone)
			{
				if (string.IsNullOrEmpty(dagName))
				{
					dagName = this.GetLastKnownDagName();
				}
				if (!string.IsNullOrEmpty(dagName))
				{
					ActiveManagerDagNamePerfmonInstance instance = ActiveManagerDagNamePerfmon.GetInstance(dagName);
					if (instance != null)
					{
						instance.ActiveManagerRoleInDag.RawValue = (long)cfg.Role;
					}
				}
			}
		}

		private void UpdateLastKnownDagName(string dagName)
		{
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey(SharedHelper.AmRegKeyRoot, ref ex);
			if (ex != null)
			{
				throw ex;
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					registryKey.SetValue("LastKnownDagName", dagName, RegistryValueKind.String);
				}
			}
		}

		private string GetLastKnownDagName()
		{
			string result = null;
			Exception ex;
			IRegistryKey registryKey = Dependencies.RegistryKeyProvider.TryOpenKey(SharedHelper.AmRegKeyRoot, ref ex);
			if (ex != null)
			{
				throw ex;
			}
			if (registryKey != null)
			{
				using (registryKey)
				{
					result = (string)registryKey.GetValue("LastKnownDagName", null);
				}
			}
			return result;
		}

		private void ReportConfigDetails(AmConfigChangedFlags changeFlags, AmConfig prevCfg, IEnumerable<AmServerName> serversInMaintenance)
		{
			TimeSpan timeSpan = ExDateTime.Now.Subtract(this.m_cfg.TimeRoleLastChanged);
			string text = string.Empty;
			if (serversInMaintenance != null && serversInMaintenance.Count<AmServerName>() > 0)
			{
				string[] value = (from serverName in serversInMaintenance
				select serverName.ToString()).ToArray<string>();
				text = string.Join(",", value);
			}
			AmConfigManager.Tracer.TraceDebug<AmRole, TimeSpan, IEnumerable<AmServerName>>(0L, "Active manager is running as {0} for duration ({1}), serversInMaintenance = '{2}'", this.m_cfg.Role, timeSpan, serversInMaintenance);
			if (changeFlags != AmConfigChangedFlags.None)
			{
				ReplayCrimsonEvents.ConfigChanged.Log<AmRole, AmRole, AmConfigChangedFlags, string>(this.m_cfg.Role, prevCfg.Role, changeFlags, string.IsNullOrEmpty(this.m_cfg.LastError) ? "<none>" : this.m_cfg.LastError);
			}
			string text2 = "<unknown>";
			if (this.m_cfg.IsPamOrSam)
			{
				text2 = this.m_cfg.DagConfig.CurrentPAM.Fqdn;
			}
			else if (this.m_cfg.IsStandalone)
			{
				text2 = AmServerName.LocalComputerName.Fqdn;
			}
			ReplayCrimsonEvents.ReportCurrentRole.Log<AmRole, string, TimeSpan, AmConfigChangedFlags, string, string>(this.m_cfg.Role, text2, timeSpan, changeFlags, prevCfg.LastError, text);
		}

		private bool CreateClusterObjects(out IAmCluster cluster, out IAmClusterGroup group, out IAmDbState dbState, out string errorString)
		{
			bool flag = false;
			cluster = null;
			group = null;
			dbState = null;
			errorString = null;
			try
			{
				cluster = ClusterFactory.Instance.Open();
				if (cluster.IsLocalNodeUp)
				{
					group = cluster.FindCoreClusterGroup();
					dbState = ClusterFactory.Instance.CreateClusterDbState(cluster);
					flag = true;
				}
				else
				{
					AmTrace.Error("Local cluster node is not up yet.", new object[0]);
					errorString = ReplayStrings.ErrorLocalNodeNotUpYet;
				}
			}
			catch (AmCoreGroupRegNotFound amCoreGroupRegNotFound)
			{
				AmTrace.Error("Failed to find core cluster group, the node could just be evicted. (error={0})", new object[]
				{
					amCoreGroupRegNotFound
				});
				errorString = ReplayStrings.ErrorFailedToGetClusterCoreGroup;
			}
			catch (ClusterException ex)
			{
				AmTrace.Error("Failed to open core cluster objects. (error={0})", new object[]
				{
					ex
				});
				errorString = ReplayStrings.ErrorFailedToOpenClusterObjects;
			}
			finally
			{
				if (!flag)
				{
					if (dbState != null)
					{
						dbState.Dispose();
						dbState = null;
					}
					if (group != null)
					{
						group.Dispose();
						group = null;
					}
					if (cluster != null)
					{
						cluster.Dispose();
						cluster = null;
					}
				}
			}
			return flag;
		}

		private bool QueueDisposeRequestIfRequired(AmConfig prevCfg, AmConfig currentCfg)
		{
			bool flag = false;
			if (prevCfg != null && !prevCfg.IsUnknown)
			{
				if (currentCfg == null)
				{
					flag = true;
				}
				else
				{
					if (!object.ReferenceEquals(prevCfg.DbState, currentCfg.DbState))
					{
						flag = true;
					}
					if (prevCfg.DagConfig != null && (currentCfg.DagConfig == null || !object.ReferenceEquals(prevCfg.DagConfig.Cluster, currentCfg.DagConfig.Cluster)))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				AmSystemManager.Instance.DelayedConfigDisposer.AddEntry(prevCfg);
			}
			return flag;
		}

		private void CheckForUnknownButGroupOwner()
		{
			IAmCluster amCluster = null;
			IAmClusterGroup amClusterGroup = null;
			try
			{
				amCluster = ClusterFactory.Instance.Open();
				amClusterGroup = amCluster.FindCoreClusterGroup();
				AmServerName ownerNode = amClusterGroup.OwnerNode;
				if (!ownerNode.IsLocalComputerName)
				{
					AmConfigManager.Tracer.TraceDebug<AmServerName>(0L, "CheckForUnknownButGroupOwner is happy because group owner is {0}", ownerNode);
				}
				else
				{
					ReplayCrimsonEvents.AmUnknownRoleButGroupOwner.Log();
					AmEvtGroupOwnerButUnknown amEvtGroupOwnerButUnknown = new AmEvtGroupOwnerButUnknown();
					amEvtGroupOwnerButUnknown.Notify(true);
				}
			}
			catch (ClusterException arg)
			{
				AmConfigManager.Tracer.TraceError<ClusterException>(0L, "CheckForUnknownButGroupOwner failed: {0}", arg);
			}
			finally
			{
				if (amClusterGroup != null)
				{
					amClusterGroup.Dispose();
				}
				if (amCluster != null)
				{
					amCluster.Dispose();
				}
			}
		}

		private AmServerName[] GetDagMembers(IADDatabaseAvailabilityGroup dag)
		{
			List<AmServerName> list = new List<AmServerName>(16);
			MultiValuedProperty<ADObjectId> servers = dag.Servers;
			if (servers != null)
			{
				foreach (ADObjectId serverId in servers)
				{
					list.Add(new AmServerName(serverId));
				}
			}
			return list.ToArray();
		}

		private bool DetermineAutomountConsensus(IAmCluster cluster, DateTime fswBootTime)
		{
			bool result = false;
			if (cluster == null)
			{
				AmTrace.Debug("DetermineAutomountConsensus: returning false because the machine is not part of an active cluster.", new object[0]);
				this.ProhibitAutoMount("Machine not clustered/clussvc not running.");
				return result;
			}
			if (ExDateTime.UtcNow < this.MommyCurfue)
			{
				AmTrace.Debug("DetermineAutomountConsensus: returning false because the curfue has not yet expired.", new object[0]);
				this.ProhibitAutoMount("Curfue has not expired.");
				return result;
			}
			if (this.MommyMayIMount)
			{
				AmTrace.Debug("DetermineAutomountConsensus: returning true because MommyMayIMount is true.", new object[0]);
				return true;
			}
			AmTrace.Debug("DetermineAutomountConsensus: checking if the replay service has restarted since the MommyMayIMount bit was set.", new object[0]);
			DateTime localBootTime = Dependencies.ManagementClassHelper.LocalBootTime;
			DateTime dateTime = AmConfigManager.ReadBootTimeFromRegistry();
			AmTrace.Debug("DetermineAutomountConsensus: WMI says the boot time is {0}, and the boot time when the Mount bit was set was {1}.", new object[]
			{
				localBootTime,
				dateTime
			});
			if (localBootTime == dateTime && dateTime != DateTime.MinValue)
			{
				AmTrace.Debug("DetermineAutomountConsensus found matching boot timestamps, assuming that the replay service has restarted since setting the bit.", new object[0]);
				this.AllowAutoMount("Found matching boot timestamps, assuming that the replay service has restarted since setting the bit.");
				return true;
			}
			return this.DetermineAutomountConsensusUnanimity(cluster, fswBootTime);
		}

		private bool DetermineAutomountConsensusUnanimity(IAmCluster cluster, DateTime fswBootTime)
		{
			bool flag = true;
			bool flag2 = false;
			IAmClusterNode[] array = null;
			try
			{
				array = cluster.EnumerateNodes().ToArray<IAmClusterNode>();
				if (array.Length == 1)
				{
					AmTrace.Debug("DetermineAutomountConsensusUnanimity: There is only one node in the cluster -- this is not sufficient to allow mounts!", new object[0]);
					flag2 = this.DetermineAutomountConsensusForSingleMachine(cluster, fswBootTime);
					return flag2;
				}
				foreach (IAmClusterNode amClusterNode in array)
				{
					AmNodeState state = amClusterNode.GetState(false);
					if (!AmClusterNode.IsNodeUp(state))
					{
						AmTrace.Debug("DetermineAutomountConsensusUnanimity: node '{0}' is not up, it is '{1}'.", new object[]
						{
							amClusterNode.Name.NetbiosName,
							state
						});
						flag = false;
					}
					else
					{
						try
						{
							int num = Dependencies.AmRpcClientWrapper.RpcchGetAutomountConsensusState(amClusterNode.Name.Fqdn);
							if (num > 0)
							{
								AmTrace.Debug("DetermineAutomountConsensusUnanimity: The consensus on {0} is {1}, returning true.", new object[]
								{
									amClusterNode.Name.NetbiosName,
									num
								});
								this.AllowAutoMount(string.Format("Received confirmation from another node ({0}).", amClusterNode.Name.NetbiosName));
								flag2 = true;
								return flag2;
							}
						}
						catch (AmServerException ex)
						{
							AmTrace.Debug("DetermineAutomountConsensusUnanimity: RPC error encountered while talking to {0}: {1}", new object[]
							{
								amClusterNode.Name.Fqdn,
								ex
							});
							flag = false;
						}
					}
				}
				if (flag)
				{
					AmTrace.Debug("DetermineAutomountConsensusUnanimity: Because all of the servers are up, setting the consensus to true.", new object[0]);
					this.AllowAutoMount("All nodes are up.");
					flag2 = true;
					return flag2;
				}
				AmTrace.Debug("DetermineAutomountConsensusUnanimity: Not all of the servers are up, and no compelling reason to set consensus to true; setting the consensus to false.", new object[0]);
			}
			finally
			{
				AmTrace.Debug("DetermineAutomountConsensusUnanimity is returning {0}.", new object[]
				{
					flag2
				});
				if (array != null)
				{
					foreach (IAmClusterNode amClusterNode2 in array)
					{
						amClusterNode2.Dispose();
					}
				}
			}
			return flag2;
		}

		private bool DetermineAutomountConsensusForSingleMachine(IAmCluster cluster, DateTime fswBootTime)
		{
			bool result = false;
			AmTrace.Debug("DetermineAutomountConsensusForSingleMachine: checking if the file share witness has restarted since the MommyMayIMount bit was set.", new object[0]);
			if (fswBootTime == DateTime.MinValue)
			{
				AmTrace.Debug("DetermineAutomountConsensusForSingleMachine: An empty FSW boot time was passed in -- unsafe to allow mounting!", new object[0]);
				return result;
			}
			DateTime dateTime = AmConfigManager.ReadFswBootTimeFromRegistry();
			AmTrace.Debug("DetermineAutomountConsensusForSingleMachine: WMI says the boot time for the FSW server is {0}, and the boot time when the Mount bit was set was {1}.", new object[]
			{
				fswBootTime,
				dateTime
			});
			if (fswBootTime == dateTime && dateTime != DateTime.MinValue)
			{
				AmTrace.Debug("DetermineAutomountConsensusForSingleMachine found matching boot timestamps, assuming that only this computer has restarted since setting the bit.", new object[0]);
				this.AllowAutoMount("Found matching FSW boot timestamps, assuming that only this computer has restarted since setting the bit.");
				result = true;
			}
			return result;
		}

		private bool CheckRemoteSiteConnected(IADDatabaseAvailabilityGroup dag, IADServer localServer, IAmDbState dbState)
		{
			bool flag = false;
			ExDateTime exDateTime = ExDateTime.MinValue;
			IADServer iadserver = null;
			string text = null;
			bool flag2 = false;
			try
			{
				ExDateTime amRemoteSiteCheckDisabledTime = RegistryParameters.AmRemoteSiteCheckDisabledTime;
				if (amRemoteSiteCheckDisabledTime > ExDateTime.UtcNow)
				{
					ReplayEventLogConstants.Tuple_AmCheckRemoteSiteDisabled.LogEvent(localServer.Name, new object[]
					{
						RegistryParameters.AmRemoteSiteCheckDisabledTimeKey,
						amRemoteSiteCheckDisabledTime
					});
					AmTrace.Debug("CheckRemoteSiteConnected: check is disabled by regkey override till {0}.", new object[]
					{
						amRemoteSiteCheckDisabledTime
					});
					flag = true;
					return flag;
				}
				AmTrace.Debug("CheckRemoteSiteConnected: local server site is {0}.", new object[]
				{
					localServer.ServerSite
				});
				if (localServer.ServerSite == null)
				{
					ReplayEventLogConstants.Tuple_AmCheckRemoteSiteLocalServerSiteNull.LogEvent(localServer.Name, new object[]
					{
						localServer.Name,
						dag.Name
					});
					AmTrace.Debug("CheckRemoteSiteConnected: skipping since local server site is null {0}.", new object[]
					{
						localServer.Name
					});
					flag = true;
					return flag;
				}
				List<string> list = new List<string>();
				foreach (ADObjectId adobjectId in dag.Servers)
				{
					AmServerName amServerName = new AmServerName(adobjectId.Name, false);
					if (dag.StoppedMailboxServers.Contains(amServerName.Fqdn))
					{
						AmTrace.Debug("CheckRemoteSiteConnected: skipping stopped server {0}.", new object[]
						{
							amServerName
						});
					}
					else
					{
						IADServer iadserver2 = this.AdLookup.MiniServerLookup.ReadMiniServerByObjectId(adobjectId);
						if (iadserver2 != null)
						{
							AmTrace.Debug("CheckRemoteSiteConnected: server {0}, site {1}.", new object[]
							{
								adobjectId.Name,
								iadserver2.ServerSite
							});
							if (!localServer.ServerSite.Equals(iadserver2.ServerSite))
							{
								flag2 = true;
								ExDateTime exDateTime2;
								bool lastServerTimeStamp = dbState.GetLastServerTimeStamp(iadserver2.Name, out exDateTime2);
								AmTrace.Debug("CheckRemoteSiteConnected: remote site found. server {0}, site {1}, timeStampExists {2} lastServerTimeStamp {3}.", new object[]
								{
									iadserver2.Name,
									iadserver2.ServerSite,
									lastServerTimeStamp,
									exDateTime2
								});
								if (lastServerTimeStamp)
								{
									if (exDateTime2 > exDateTime)
									{
										exDateTime = exDateTime2;
										iadserver = iadserver2;
										text = ((iadserver2.ServerSite == null) ? null : iadserver2.ServerSite.Name);
									}
								}
								else
								{
									list.Add(iadserver2.Name);
								}
							}
						}
						else
						{
							AmTrace.Error("CheckRemoteSiteConnected: failed to read miniserver object from AD for server {0}.", new object[]
							{
								adobjectId.Name
							});
						}
					}
				}
				if (list.Count > 0)
				{
					string text2 = string.Join(",", list.ToArray());
					ReplayEventLogConstants.Tuple_AmTimeStampEntryMissingInOneOrMoreServers.LogEvent(localServer.Name, new object[]
					{
						localServer.Name,
						dag.Name,
						text2
					});
					flag = true;
					return flag;
				}
				if (!flag2)
				{
					ReplayEventLogConstants.Tuple_AmCheckRemoteSiteNotFound.LogEvent(localServer.Name, new object[]
					{
						localServer.Name,
						dag.Name
					});
					AmTrace.Debug("CheckRemoteSiteConnected: DAG not site resilient!", new object[0]);
					flag = true;
					return flag;
				}
				if (ExDateTime.UtcNow - exDateTime > TimeSpan.FromSeconds((double)RegistryParameters.AmRemoteSiteCheckAlertTimeoutInSec))
				{
					ReplayEventLogConstants.Tuple_AmCheckRemoteSiteAlert.LogEvent(localServer.Name, new object[]
					{
						localServer.Name,
						dag.Name,
						exDateTime,
						iadserver,
						text
					});
					AmTrace.Debug("CheckRemoteSiteConnected: Alert but not set AmConfig to Unknown. maxRemoteSiteTimeStamp {0}, maxRemoteServer {1}, maxRemoteSite {2}.", new object[]
					{
						exDateTime,
						iadserver,
						text
					});
					flag = false;
					return flag;
				}
				AmTrace.Debug("CheckRemoteSiteConnected: remote site connected. maxRemoteSiteTimeStamp {0}, maxRemoteServer {1}, maxRemoteSite {2}.", new object[]
				{
					exDateTime,
					iadserver,
					text
				});
				flag = true;
			}
			finally
			{
				if (flag && (this.LastKnownConfig == null || this.LastKnownConfig.IsUnknown))
				{
					ReplayEventLogConstants.Tuple_AmCheckRemoteSiteSucceeded.LogEvent(localServer.Name, new object[]
					{
						localServer.Name,
						dag.Name,
						exDateTime,
						iadserver,
						text
					});
				}
				AmTrace.Debug("CheckRemoteSiteConnected returning.{0}", new object[]
				{
					flag
				});
			}
			return flag;
		}

		private DateTime GetFswBootTime(IAmCluster cluster, bool forceRefresh)
		{
			this.FswBootTimeCounter++;
			if (forceRefresh)
			{
				this.FswBootTimeCounter = 0;
			}
			if (this.FswBootTimeCounter % 10 == 0)
			{
				AmServerName fswServerName = this.GetFswServerName(cluster);
				if (fswServerName == null)
				{
					AmTrace.Debug("GetFswBootTime: There is no determinable file share witness in the cluster.", new object[0]);
					this.CachedFswBootTime = DateTime.MinValue;
				}
				else
				{
					this.CachedFswBootTime = Dependencies.ManagementClassHelper.GetBootTime(fswServerName);
				}
			}
			return this.CachedFswBootTime;
		}

		private AmServerName GetFswServerName(IAmCluster cluster)
		{
			bool flag = DagHelper.IsQuorumTypeFileShareWitness(cluster);
			string text = string.Empty;
			AmServerName amServerName = null;
			if (flag)
			{
				using (AmClusterResource amClusterResource = cluster.OpenQuorumResource())
				{
					if (amClusterResource != null)
					{
						text = amClusterResource.GetPrivateProperty<string>("SharePath");
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				LongPath longPath;
				if (!LongPath.TryParse(text, out longPath))
				{
					AmTrace.Error("Whoa, the persisted file share witness path in the quorum resource isn't a valid path! ({0})", new object[]
					{
						text
					});
				}
				else
				{
					amServerName = new AmServerName(longPath.ServerName);
					AmTrace.Debug("AmConfigManager.GetFswHostName: The FSW server is {0}", new object[]
					{
						amServerName
					});
				}
			}
			return amServerName;
		}

		internal const string UnknownTriggeredByADError = "Unknown Triggered by AD Error";

		private const int MaxUnknownRetryCount = 5;

		private const int FswBootTimeCacheThreshold = 10;

		private static TimeSpan PeriodicRefreshIntervalShort = new TimeSpan(0, 0, 10);

		private static TimeSpan PeriodicRefreshIntervalLong = new TimeSpan(0, 0, 30);

		private readonly TimeSpan StopDagEffectiveTimeSpan = TimeSpan.FromMinutes(5.0);

		private object m_locker = new object();

		private bool m_isClusterObjectCreationFailed;

		private IAmClusterGroup m_lastValidGroup;

		private AmConfig m_cfg;

		private AmSelfDismounter m_selfDismounter;

		private bool m_isFirstTimeUnknown = true;

		private static Hookable<Func<bool, TimeSpan>> getRefreshIntervalTestHook = Hookable<Func<bool, TimeSpan>>.Create(false, new Func<bool, TimeSpan>(AmConfigManager.GetRefreshInterval));
	}
}
