using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Bitlocker.Utilities;
using Microsoft.Exchange.Common.DiskManagement.Utilities;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DiskReclaimerManager : TimerComponent, IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DiskReclaimerTracer;
			}
		}

		public DiskReclaimerManager(IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup, IReplicaInstanceManager replicaInstanceManager) : base(TimeSpan.FromSeconds((double)RegistryParameters.DiskReclaimerDelayedStartInSecs), TimeSpan.FromSeconds((double)RegistryParameters.DiskReclaimerPollerIntervalInSecs), "DiskReclaimerManager")
		{
			DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer instance being constructed");
			this.m_adConfigProvider = adConfigProvider;
			this.m_statusLookup = statusLookup;
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_volumeManager = new VolumeManager();
		}

		public string Name
		{
			get
			{
				return "Disk Reclaimer";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.DiskReclaimer;
			}
		}

		public bool IsCritical
		{
			get
			{
				return false;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return !RegistryParameters.DiskReclaimerDisabled;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public new bool Start()
		{
			base.Start();
			return true;
		}

		internal static bool ShouldSkipDueToServerNotConfigured(IADServer server)
		{
			if (!RegistryParameters.AutoDagUseServerConfiguredProperty)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)server.GetHashCode(), "ShouldSkipDueToServerNotConfigured: Returning 'false' because regkey AutoDagUseServerConfiguredProperty is disabled.");
				return false;
			}
			if (server.AutoDagServerConfigured)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)server.GetHashCode(), "ShouldSkipDueToServerNotConfigured: Returning 'false' because server's AD property AutoDagServerConfigured is 'true'.");
				return false;
			}
			DiskReclaimerManager.Tracer.TraceDebug((long)server.GetHashCode(), "ShouldSkipDueToServerNotConfigured: Returning 'true' because server's AD property AutoDagServerConfigured is 'false'.");
			return true;
		}

		protected override void TimerCallbackInternal()
		{
			Exception ex = null;
			try
			{
				this.Run();
			}
			catch (MonitoringADServiceShuttingDownException ex2)
			{
				DiskReclaimerManager.Tracer.TraceError<MonitoringADServiceShuttingDownException>((long)this.GetHashCode(), "DiskReclaimer: Got service shutting down exception when retrieving AD config: {0}", ex2);
				ex = ex2;
			}
			catch (MonitoringADConfigException ex3)
			{
				DiskReclaimerManager.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "DiskReclaimer: Got exception when retrieving AD config: {0}", ex3);
				ex = ex3;
			}
			catch (TaskServerException ex4)
			{
				ex = ex4;
			}
			catch (TaskServerTransientException ex5)
			{
				ex = ex5;
			}
			catch (AmServerException ex6)
			{
				ex = ex6;
			}
			catch (AmServerTransientException ex7)
			{
				ex = ex7;
			}
			catch (ADOperationException ex8)
			{
				ex = ex8;
			}
			catch (ADTransientException ex9)
			{
				ex = ex9;
			}
			if (ex != null)
			{
				DiskReclaimerManager.Tracer.TraceError<Exception>((long)this.GetHashCode(), "DiskReclaimer: TimerCallbackInternal() got an exception: {0}", ex);
				ReplayCrimsonEvents.DiskReclaimerError.LogPeriodic<Exception>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, ex);
			}
		}

		private void Run()
		{
			DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Entering Run()...");
			IMonitoringADConfig config = this.m_adConfigProvider.GetConfig(true);
			if (config.ServerRole == MonitoringServerRole.Standalone)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Skipping running DiskReclaimerManager because local server is not a member of a DAG.");
				return;
			}
			if (!config.Dag.AutoDagDiskReclaimerEnabled)
			{
				DiskReclaimerManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DiskReclaimer: Skipping running DiskReclaimerManager because the local DAG '{0}' has AutoDagDiskReclaimerEnabled set to disabled.", config.Dag.Name);
				return;
			}
			if (DiskReclaimerManager.ShouldSkipDueToServerNotConfigured(config.TargetMiniServer))
			{
				DiskReclaimerManager.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "DiskReclaimer: Skipping running DiskReclaimerManager because the local server '{0}' is not yet fully configured in the AD.", config.TargetServerName);
				return;
			}
			this.m_volumeManager.Refresh(config);
			if (MountPointUtil.IsConfigureMountPointsEnabled())
			{
				bool flag = false;
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Regkey ConfigureMountPointsPostReInstall is set. Starting mountpoint configuration on all Exchange volumes.");
				ReplayCrimsonEvents.DiskReclaimerPostInstallConfigStarted.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, "ConfigureMountPointsPostReInstall");
				this.ConfigureVolumesPostReInstall(config, out flag);
				this.m_volumeManager.Refresh(config);
				if (flag)
				{
					DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Database copies might be missing in AD. ConfigureMountPointsPostReInstall will not be disabled to allow DiskReclaimer to run again.");
					return;
				}
				this.UpdateVolumeForNeverMountedActives(config);
				ReplayCrimsonEvents.DiskReclaimerPostInstallConfigCompleted.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, "ConfigureMountPointsPostReInstall");
				DiskReclaimerManager.m_lastVolumeScanTime = DateTime.MinValue;
				Exception ex;
				if ((ex = MountPointUtil.DisableConfigureMountPoints()) != null)
				{
					DiskReclaimerManager.Tracer.TraceError<Exception>((long)this.GetHashCode(), "DiskReclaimer: Failed to reset regkey ConfigureMountPointsPostReInstall after configuring mountpoints. Error: {0}", ex);
					ReplayCrimsonEvents.DiskReclaimerError.LogPeriodic<Exception>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, ex);
				}
			}
			this.m_volumeManager.DetermineMisconfiguredVolumes(config);
			if (!this.IsSafeToLookForQuarantinedDisks())
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Skip scanning for quarantined volumes...");
				return;
			}
			DiskReclaimerManager.m_lastVolumeScanTime = DateTime.UtcNow;
			ReplayCrimsonEvents.DiskReclaimerInitiated.Log();
			List<ExchangeVolume>[] array = this.m_volumeManager.DetermineVolumeSpareStatuses();
			List<ExchangeVolume> list = array[4];
			if (list == null || list.Count == 0)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "Run(): No volumes that need to be reclaimed found. Exiting.");
			}
			else
			{
				DiskReclaimerManager.Tracer.TraceDebug<int>((long)this.GetHashCode(), "DiskReclaimer: Found {0} volumes that can potentially be reclaimed.", list.Count);
				DatabaseHealthValidationRunner databaseHealthValidationRunner = new DatabaseHealthValidationRunner(config.TargetServerName.NetbiosName);
				databaseHealthValidationRunner.Initialize();
				List<string> list2 = new List<string>();
				foreach (IHealthValidationResultMinimal healthValidationResultMinimal in databaseHealthValidationRunner.RunDatabaseRedundancyChecksForCopyRemoval(true, null, true, true, -1))
				{
					if (!healthValidationResultMinimal.IsValidationSuccessful)
					{
						list2.Add(healthValidationResultMinimal.ErrorMessageWithoutFullStatus);
					}
				}
				if (list2.Count == 0)
				{
					DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: Passed copy removal redundancy check for this server. Trying to reclaim one quarantined volume...");
					IEnumerable<IADDatabase> enumerable = config.DatabaseMap[config.TargetServerName];
					IEnumerable<CopyStatusClientCachedEntry> copyStatusesByServer = this.m_statusLookup.GetCopyStatusesByServer(config.TargetServerName, enumerable, CopyStatusClientLookupFlags.None);
					using (List<ExchangeVolume>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ExchangeVolume exchangeVolume = enumerator2.Current;
							ExchangeVolumeDbStatusInfo filesInformationFromVolume = this.m_volumeManager.GetFilesInformationFromVolume(exchangeVolume, enumerable, copyStatusesByServer);
							bool flag2;
							Exception ex2;
							if (this.m_volumeManager.TryReclaimVolume(exchangeVolume, filesInformationFromVolume, out flag2, out ex2))
							{
								DiskReclaimerManager.Tracer.TraceDebug<MountedFolderPath, MountedFolderPath>((long)this.GetHashCode(), "DiskReclaimer: Successfully reclaimed volume '{0}' mounted at '{1}' as a spare.", exchangeVolume.VolumeName, exchangeVolume.ExchangeVolumeMountPoint);
								ReplayCrimsonEvents.DiskReclaimerSucceeded.Log<string, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path);
								this.m_volumeManager.DetermineVolumeSpareStatuses();
								break;
							}
							if (flag2)
							{
								DiskReclaimerManager.Tracer.TraceError<MountedFolderPath, MountedFolderPath, Exception>((long)this.GetHashCode(), "DiskReclaimer: Failed to reclaim volume '{0}' mounted at '{1}' as a spare. However, formatting the volume apparently succeeded. Error: {2}", exchangeVolume.VolumeName, exchangeVolume.ExchangeVolumeMountPoint, ex2);
								ReplayCrimsonEvents.DiskReclaimerSparingFailed.Log<string, string, bool, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path, flag2, ex2.Message);
								this.m_volumeManager.DetermineVolumeSpareStatuses();
								break;
							}
							DiskReclaimerManager.Tracer.TraceError<MountedFolderPath, MountedFolderPath, Exception>((long)this.GetHashCode(), "DiskReclaimer: Failed to format and reclaim volume '{0}' mounted at '{1}' as a spare. Error: {2}", exchangeVolume.VolumeName, exchangeVolume.ExchangeVolumeMountPoint, ex2);
							ReplayCrimsonEvents.DiskReclaimerSparingFailed.Log<string, string, bool, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path, flag2, ex2.Message);
						}
						goto IL_477;
					}
				}
				string text = string.Join("\n\n", list2);
				DiskReclaimerManager.Tracer.TraceError<string>((long)this.GetHashCode(), "DiskReclaimer: Skip sparing. One or more databases on this server failed copy removal redundancy check. Reason: {0}", text);
				ReplayCrimsonEvents.DiskReclaimerRedundancyPrereqFailed.LogPeriodic<string>(text.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, text);
			}
			IL_477:
			this.RunBitlockerMaintenance(config);
		}

		private void RunBitlockerMaintenance(IMonitoringADConfig adConfig)
		{
			if (!adConfig.Dag.AutoDagBitlockerEnabled)
			{
				DiskReclaimerManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DiskReclaimer: Skipping running RunBitlockerMaintenance because the local DAG '{0}' has AutoDagBitlockerEnabled set to disabled.", adConfig.Dag.Name);
				return;
			}
			Exception ex;
			bool flag = BitlockerUtil.IsVolumeMountedOnVirtualDisk(string.Empty, out ex);
			if (ex != null)
			{
				DiskReclaimerManager.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "RunBitlockerMaintenance(): Exception finding whether Volumes are mounted on virtual disks or not. Reason {0}", ex);
				return;
			}
			if (flag)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "RunBitlockerMaintenance(): Volumes are mounted on virtual disks. Virtul Disk Volumes cannot be encrypted.");
				return;
			}
			if (!BitlockerConfigHelper.IsBitlockerManagerEnabled())
			{
				DiskReclaimerManager.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "DiskReclaimer: Skipping running RunBitlockerMaintenance because the local Machine '{0}' has BitlockerManagerEnabled set to disabled.", adConfig.TargetServerName);
				ReplayCrimsonEvents.BitlockerFeatureDisabled.Log();
				return;
			}
			if (Util.IsOperatingSystemWin8OrHigher())
			{
				this.RunBitlockerMaintenanceforWin8EmptyVolumes(adConfig.Dag.AutoDagFIPSCompliant);
				return;
			}
			this.RunBitlockerMaintenanceforWin7EmptyVolumes(adConfig.Dag.AutoDagFIPSCompliant);
		}

		private void RunBitlockerMaintenanceforWin8EmptyVolumes(bool fipsCompliant)
		{
			if (BitlockerConfigHelper.IsBitlockerEmptyWin8VolumesUsedOnlyEncryptionFeatureEnabled())
			{
				BitlockerHelper.EncryptEmptyWin8Volumes(this.m_volumeManager, fipsCompliant);
				return;
			}
			DiskReclaimerManager.Tracer.TraceError((long)this.GetHashCode(), "DiskReclaimer: Skip encrypting Win8 empty volumes as the disabled config is set to true.}");
			ReplayCrimsonEvents.BitlockerEmptyWin8VolumesUsedOnlyEncryptionFeatureDisabled.Log();
		}

		private void RunBitlockerMaintenanceforWin7EmptyVolumes(bool fipsCompliant)
		{
			if (BitlockerConfigHelper.IsBitlockerEmptyWin7VolumesFullVolumeEncryptionFeatureEnabled())
			{
				BitlockerHelper.EncryptEmptyWin7Volumes(this.m_volumeManager, fipsCompliant);
				BitlockerHelper.LogEncryptionPercentagesForEncryptingVolumes();
				return;
			}
			DiskReclaimerManager.Tracer.TraceError((long)this.GetHashCode(), "DiskReclaimer: Skip encrypting Win7 empty volumes as the disabled config is set to true.}");
			ReplayCrimsonEvents.BitlockerEmptyWin7VolumesUsedOnlyEncryptionFeatureDisabled.Log();
		}

		private bool IsSafeToLookForQuarantinedDisks()
		{
			DateTime lastVolumeScanTime = DiskReclaimerManager.m_lastVolumeScanTime;
			DateTime t = DiskReclaimerManager.m_lastVolumeScanTime.ToUniversalTime().AddSeconds((double)RegistryParameters.DiskReclaimerSpareDelayInSecs_Short);
			if (t < DateTime.UtcNow)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: IsSafeToLookForQuarantinedDisks() - It has been at least 24 hours since DiskReclaimer looked for Quarantined volumes. Returning true.");
				return true;
			}
			return false;
		}

		private void ConfigureVolumesPostReInstall(IMonitoringADConfig adConfig, out bool databasesMissingInAD)
		{
			Exception ex = null;
			databasesMissingInAD = false;
			IEnumerable<IADDatabase> enumerable = adConfig.DatabaseMap[adConfig.TargetServerName];
			if (enumerable == null)
			{
				DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): There are no database configured in AD for this server. Skipping volume configuration.");
				ReplayCrimsonEvents.DiskReclaimerError.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, "DiskReclaimer: ConfigureVolumesPostReInstall(): There are no database configured in AD for this server. Skipping volume configuration.");
				databasesMissingInAD = true;
				return;
			}
			DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): Attempting to restore mountpoints for all Exchange volumes post ReInstall.");
			foreach (ExchangeVolume exchangeVolume in this.m_volumeManager.Volumes)
			{
				ex = null;
				if (!exchangeVolume.IsExchangeVolume)
				{
					DiskReclaimerManager.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): Volume '{0}'is not an ExchangeVolume. Skipping this volume.", exchangeVolume.VolumeName);
				}
				else if (!exchangeVolume.IsVolumeMissingDatabaseMountPoints(out ex))
				{
					DiskReclaimerManager.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): Volume '{0}' has the required number of mountpoints.", exchangeVolume.VolumeName);
				}
				else if (ex != null)
				{
					DiskReclaimerManager.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): Volume '{0}' is not valid. Error: {1}", exchangeVolume.VolumeName, ex);
					ReplayCrimsonEvents.DiskReclaimerError.LogPeriodic<Exception>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, ex);
				}
				else
				{
					ex = this.m_volumeManager.ConfigureDatabaseMountPoints(exchangeVolume, enumerable, adConfig.Dag.AutoDagDatabaseCopiesPerVolume);
					if (ex != null)
					{
						if (ex is DatabasesMissingInADException)
						{
							ReplayCrimsonEvents.DiskReclaimerPostInstallConfigUnknownDatabase.LogPeriodic<MountedFolderPath, string>(exchangeVolume.VolumeName, DiagCore.DefaultEventSuppressionInterval, exchangeVolume.VolumeName, ex.Message);
							databasesMissingInAD = true;
						}
						else if (ex is DbVolumeInvalidDirectoriesException)
						{
							ReplayCrimsonEvents.DiskReclaimerPostInstallConfigInvalidDirectories.LogPeriodic<MountedFolderPath, string>(exchangeVolume.VolumeName, DiagCore.DefaultEventSuppressionInterval, exchangeVolume.VolumeName, ex.Message);
						}
						else
						{
							ReplayCrimsonEvents.DiskReclaimerPostInstallConfigError.LogPeriodic<MountedFolderPath, string>(exchangeVolume.VolumeName, DiagCore.DefaultEventSuppressionInterval, exchangeVolume.VolumeName, ex.Message);
						}
						DiskReclaimerManager.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall(): Error configuring mountpoints for volume: '{0}'. Error: {1}", exchangeVolume.VolumeName, ex);
					}
				}
			}
			DiskReclaimerManager.Tracer.TraceDebug((long)this.GetHashCode(), "DiskReclaimer: ConfigureVolumesPostReInstall() attempting to resume database copies the server");
			ReplayCrimsonEvents.DiskReclaimerResumingDatabaseCopies.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, adConfig.TargetServerName.NetbiosName);
			foreach (IADDatabase iaddatabase in enumerable)
			{
				SeedHelper.TryResumeDatabaseCopy(iaddatabase.Guid, adConfig.TargetServerName.NetbiosName, false);
			}
		}

		private void UpdateVolumeForNeverMountedActives(IMonitoringADConfig adConfig)
		{
			IEnumerable<IADDatabase> enumerable = adConfig.DatabaseMap[adConfig.TargetServerName];
			IEnumerable<CopyStatusClientCachedEntry> copyStatusesByServer = this.m_statusLookup.GetCopyStatusesByServer(adConfig.TargetServerName, enumerable, CopyStatusClientLookupFlags.None);
			Exception ex = null;
			List<IADDatabase> neverMountedActives = this.m_volumeManager.GetNeverMountedActives(enumerable, adConfig, copyStatusesByServer);
			if (neverMountedActives != null && neverMountedActives.Count > 0)
			{
				ICollection<string> source = from db in neverMountedActives
				select db.Name;
				DiskReclaimerManager.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "DiskReclaimer: UpdateVolumeForNeverMountedActives() Number of never mounted active databases missing volume = {0}. Databases: '{1}'", neverMountedActives.Count, string.Join(",", source.ToArray<string>()));
				ReplayCrimsonEvents.DiskReclaimerNeverMountedActives.Log<int, string>(neverMountedActives.Count, string.Join(",", source.ToArray<string>()));
			}
			foreach (IADDatabase iaddatabase in neverMountedActives)
			{
				bool flag = false;
				DatabaseVolumeInfo instance = DatabaseVolumeInfo.GetInstance(iaddatabase.EdbFilePath.PathName, iaddatabase.LogFolderPath.PathName, iaddatabase.Name, adConfig.Dag.AutoDagVolumesRootFolderPath.PathName, adConfig.Dag.AutoDagDatabasesRootFolderPath.PathName, adConfig.Dag.AutoDagDatabaseCopiesPerVolume);
				if (instance.IsValid && MountPointUtil.IsDirectoryAccessibleMountPoint(instance.DatabaseVolumeMountPoint.Path, out ex))
				{
					flag = true;
				}
				else
				{
					ReplayCrimsonEvents.DiskReclaimerNeverMountedActiveMissingVolume.Log<string>(iaddatabase.Name);
					if (this.m_volumeManager.FixActiveDatabaseMountPoint(iaddatabase, enumerable, adConfig, out ex, true))
					{
						flag = true;
						this.m_volumeManager.UpdateVolumeInfoCopyState(iaddatabase.Guid, this.m_replicaInstanceManager);
					}
					else
					{
						DiskReclaimerManager.Tracer.TraceError<string, string>((long)this.GetHashCode(), "DiskReclaimer: UpdateVolumeForNeverMountedActives() failed to fix up active database: '{0}' mountpoint. Error: {1}", iaddatabase.Name, AmExceptionHelper.GetExceptionMessageOrNoneString(ex));
						ReplayCrimsonEvents.DiskReclaimerFixActiveMountPointError.Log<string, string>(iaddatabase.Name, ex.Message);
					}
				}
				if (flag)
				{
					ex = this.MountDatabaseWrapper(iaddatabase);
					if (ex != null)
					{
						DiskReclaimerManager.Tracer.TraceError<string, string>((long)this.GetHashCode(), "DiskReclaimer: UpdateVolumeForNeverMountedActives() failed to mount active database: '{0}' mountpoint. Error: {1}", iaddatabase.Name, ex.Message);
						ReplayCrimsonEvents.DiskReclaimerMountActiveDatabaseError.Log<string, string>(iaddatabase.Name, ex.Message);
					}
				}
			}
			ReplayCrimsonEvents.DiskReclaimerFixActiveMountPointCompleted.Log<string>(adConfig.TargetServerName.NetbiosName);
			this.m_volumeManager.Refresh(adConfig);
		}

		private Exception MountDatabaseWrapper(IADDatabase database)
		{
			Exception result = null;
			try
			{
				AmRpcClientHelper.MountDatabase(database, 0, 0, 0);
			}
			catch (AmServerException ex)
			{
				result = ex;
			}
			catch (AmServerTransientException ex2)
			{
				result = ex2;
			}
			return result;
		}

		public static readonly TimeSpan DiskReclaimerSpareDelayShort = TimeSpan.FromSeconds((double)RegistryParameters.DiskReclaimerSpareDelayInSecs_Short);

		public static readonly TimeSpan DiskReclaimerSpareDelayMedium = TimeSpan.FromSeconds((double)RegistryParameters.DiskReclaimerSpareDelayInSecs_Medium);

		public static readonly TimeSpan DiskReclaimerSpareDelayLong = TimeSpan.FromSeconds((double)RegistryParameters.DiskReclaimerSpareDelayInSecs_Long);

		private readonly IMonitoringADConfigProvider m_adConfigProvider;

		private readonly ICopyStatusClientLookup m_statusLookup;

		private readonly VolumeManager m_volumeManager;

		private static DateTime m_lastVolumeScanTime;

		private readonly IReplicaInstanceManager m_replicaInstanceManager;
	}
}
