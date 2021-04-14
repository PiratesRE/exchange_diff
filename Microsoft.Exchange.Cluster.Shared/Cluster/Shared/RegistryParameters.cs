using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class RegistryParameters
	{
		internal static void TestLoadRegistryParameters()
		{
			RegistryParameters.s_parameters.LoadInitialValues();
			RegistryParameters.TestLoadRegistryValues();
		}

		internal static int BcsCheckToDisable
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("BcsCheckToDisable");
			}
		}

		internal static int ListMdbStatusRpcTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ListMdbStatusRpcTimeoutInSec");
			}
		}

		internal static int MdbStatusFetcherServerUpTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MdbStatusFetcherServerUpTimeoutInSec");
			}
		}

		internal static ExDateTime StoreKillBugcheckDisabledTime
		{
			get
			{
				return DateTimeHelper.ToLocalExDateTime(RegistryParameters.s_parameters.GetValue<DateTime>("StoreKillBugcheckDisabledTime"));
			}
		}

		internal static ClusterNotifyFlags NetworkClusterNotificationMask
		{
			get
			{
				RegistryParameters.LoadRegistryValues();
				return RegistryParameters.m_networkClusterNotificationMask;
			}
		}

		internal static long BootTimeCookie
		{
			get
			{
				RegistryParameters.LoadRegistryValues();
				return RegistryParameters.m_bootTimeCookie;
			}
		}

		internal static long BootTimeFswCookie
		{
			get
			{
				RegistryParameters.LoadRegistryValues();
				return RegistryParameters.m_bootTimeFswCookie;
			}
		}

		internal static WatchdogAction FailureItemWatchdogAction
		{
			get
			{
				RegistryParameters.LoadRegistryValues();
				return RegistryParameters.m_failureItemWatchdogAction;
			}
		}

		internal static bool EnableKernelWatchdogTimer
		{
			get
			{
				RegistryParameters.LoadRegistryValues();
				return RegistryParameters.m_enableKernelWatchdogTimer;
			}
		}

		internal static bool IsTransientFailoverSuppressionEnabled
		{
			get
			{
				return RegistryParameters.TransientFailoverSuppressionDelayInSec > 0;
			}
		}

		internal static int SelfDismountAllDelayInSec
		{
			get
			{
				int delayInSec = RegistryParameters.TransientFailoverSuppressionDelayInSec + 60;
				RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
				{
					delayInSec = (int)key.GetValue("SelfDismountAllDelayInSec", delayInSec);
				});
				return delayInSec;
			}
		}

		internal static ExDateTime AmRemoteSiteCheckDisabledTime
		{
			get
			{
				string disabledTime = null;
				RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
				{
					disabledTime = (string)key.GetValue(RegistryParameters.AmRemoteSiteCheckDisabledTimeKey, string.Empty);
				});
				ExDateTime result;
				if (ExDateTime.TryParse(disabledTime, out result))
				{
					return result;
				}
				return ExDateTime.MinValue;
			}
		}

		private static void LoadRegistryValues()
		{
			RegistryParameters.LoadRegistryValues(false);
		}

		private static void TestLoadRegistryValues()
		{
			RegistryParameters.LoadRegistryValues(true);
		}

		internal static void TryGetRegistryParameters(string registryKey, Action<IRegistryKey> operation)
		{
			Exception ex;
			using (IRegistryKey registryKey2 = SharedDependencies.RegistryKeyProvider.TryOpenKey(registryKey, ref ex))
			{
				if (ex == null)
				{
					operation(registryKey2);
				}
			}
		}

		private static WatchdogAction GetFailureItemWatchdogAction(IRegistryKey key)
		{
			RegistryParameters.m_failureItemWatchdogAction = (WatchdogAction)((int)key.GetValue("FailureItemWatchdogAction", (int)RegistryParameters.m_failureItemWatchdogAction));
			return RegistryParameters.m_failureItemWatchdogAction;
		}

		internal static WatchdogAction GetFailureItemWatchdogAction()
		{
			RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
			{
				RegistryParameters.GetFailureItemWatchdogAction(key);
			});
			return RegistryParameters.m_failureItemWatchdogAction;
		}

		internal static int GetApiLatencyInSec(string apiName)
		{
			string propertyName = "ApiDelayLatencyInSec_" + apiName;
			int latency = 0;
			RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
			{
				latency = (int)key.GetValue(propertyName, latency);
			});
			return latency;
		}

		internal static int GetApiSimulatedErrorCode(string apiName)
		{
			string propertyName = "ApiSimulatedErrorCode_" + apiName;
			int errorCode = 0;
			RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
			{
				errorCode = (int)key.GetValue(propertyName, errorCode);
			});
			return errorCode;
		}

		internal static bool GetIsKillClusterServiceOnClusApiHang()
		{
			return RegistryParameters.IsKillClusterServiceOnClusApiHang;
		}

		internal static bool GetIsLogApiLatencyFailure()
		{
			return RegistryParameters.IsLogApiLatencyFailure;
		}

		internal static int GetTestWithFakeNetwork()
		{
			return RegistryParameters.TestWithFakeNetwork;
		}

		internal static bool IsManagedStore()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS"))
			{
				if (registryKey != null)
				{
					string text = registryKey.GetValue("ImagePath") as string;
					if (text != null)
					{
						text = text.Trim(new char[]
						{
							'"'
						});
						return text.EndsWith("microsoft.exchange.store.exe", StringComparison.OrdinalIgnoreCase) || text.EndsWith("microsoft.exchange.store.service.exe", StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			return false;
		}

		private static void LoadRegistryValues(bool forceReload)
		{
			if (!forceReload && RegistryParameters.m_gLoadedRegistryValues)
			{
				return;
			}
			RegistryParameters.TryGetRegistryParameters("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", delegate(IRegistryKey key)
			{
				RegistryParameters.m_networkClusterNotificationMask = (ClusterNotifyFlags)key.GetValue("NetworkClusterNotificationMask", RegistryParameters.m_networkClusterNotificationMask);
				RegistryParameters.m_bootTimeCookie = RegistryParameters.GetRegistryParameterLong(key, "BootTimeCookie", RegistryParameters.m_bootTimeCookie, 0L, null);
				RegistryParameters.m_bootTimeFswCookie = RegistryParameters.GetRegistryParameterLong(key, "BootTimeFswCookie", RegistryParameters.m_bootTimeFswCookie, 0L, null);
				RegistryParameters.GetFailureItemWatchdogAction(key);
				RegistryParameters.m_enableKernelWatchdogTimer = ((int)key.GetValue("78341438-9b4a-4554-bbff-fd3ac2b5bbe3", 0) > 0);
				RegistryParameters.m_gLoadedRegistryValues = true;
			});
		}

		private static int GetRegistryParameterInt(IRegistryKey key, string paramName, int defaultValue, int minimumValue)
		{
			return RegistryParameters.GetRegistryParameterInt(key, paramName, defaultValue, minimumValue, null);
		}

		private static int GetRegistryParameterInt(IRegistryKey key, string paramName, int defaultValue, int minimumValue, int? maximumValue)
		{
			int num = (int)key.GetValue(paramName, defaultValue);
			if (num < minimumValue)
			{
				num = minimumValue;
			}
			else if (maximumValue != null && num > maximumValue.Value)
			{
				num = maximumValue.Value;
			}
			return num;
		}

		private static long GetRegistryParameterLong(IRegistryKey key, string paramName, long defaultValue, long minimumValue, long? maximumValue)
		{
			object value = key.GetValue(paramName, defaultValue);
			long num = (long)value;
			if (num < minimumValue)
			{
				num = minimumValue;
			}
			else if (maximumValue != null && num > maximumValue.Value)
			{
				num = maximumValue.Value;
			}
			return num;
		}

		internal static int AcllDismountOrKillTimeoutInSec2
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AcllDismountOrKillTimeoutInSec2");
			}
		}

		internal static int AcllLockAutoReleaseAfterDurationMs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AcllLockAutoReleaseAfterDurationMs");
			}
		}

		internal static int AcllSuspendLockTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AcllSuspendLockTimeoutInSec");
			}
		}

		internal static int ADConfigRefreshDefaultTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ADConfigRefreshDefaultTimeoutInSec");
			}
		}

		internal static int AdObjectCacheHitTtlInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AdObjectCacheHitTtlInSec");
			}
		}

		internal static int AdObjectCacheMissTtlInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AdObjectCacheMissTtlInSec");
			}
		}

		internal static int ADReplicationSleepInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ADReplicationSleepInSec");
			}
		}

		internal static int AmConfigObjectDisposeDelayInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmConfigObjectDisposeDelayInSec");
			}
		}

		internal static int AmDeferredDatabaseStateRestorerIntervalInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmDeferredDatabaseStateRestorerIntervalInMSec");
			}
		}

		internal static bool AmDisableBatchOperations
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AmDisableBatchOperations");
			}
		}

		internal static int AmDismountOrKillTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmDismountOrKillTimeoutInSec");
			}
		}

		internal static bool AmEnableCrimsonLoggingPeriodicEventProcessing
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AmEnableCrimsonLoggingPeriodicEventProcessing");
			}
		}

		internal static int AmPerfCounterUpdateIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmPerfCounterUpdateIntervalInSec");
			}
		}

		internal static bool AmPeriodicDatabaseAnalyzerEnabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AmPeriodicDatabaseAnalyzerEnabled");
			}
		}

		internal static int AmPeriodicDatabaseAnalyzerIntervalInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmPeriodicDatabaseAnalyzerIntervalInMSec");
			}
		}

		internal static int AmPeriodicRoleReportingIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmPeriodicRoleReportingIntervalInSec");
			}
		}

		internal static int AmRemoteSiteCheckAlertTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmRemoteSiteCheckAlertTimeoutInSec");
			}
		}

		internal static int AmServerNameCacheTTLInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmServerNameCacheTTLInSec");
			}
		}

		internal static int AmSystemEventAssertOnHangTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmSystemEventAssertOnHangTimeoutInMSec");
			}
		}

		internal static int AmSystemManagerEventWaitTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AmSystemManagerEventWaitTimeoutInMSec");
			}
		}

		internal static bool AutoDagUseServerConfiguredProperty
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoDagUseServerConfiguredProperty");
			}
		}

		internal static int AutoReseedCiBehindBacklog
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiBehindBacklog");
			}
		}

		internal static bool AutoReseedCiBehindDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedCiBehindDisabled");
			}
		}

		internal static int AutoReseedCiBehindRetryCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiBehindRetryCount");
			}
		}

		internal static bool AutoReseedCiFailedSuspendedDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedCiFailedSuspendedDisabled");
			}
		}

		internal static int AutoReseedCiMaxConcurrentSeeds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiMaxConcurrentSeeds");
			}
		}

		internal static int AutoReseedCiPeriodicIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiPeriodicIntervalInSecs");
			}
		}

		internal static bool AutoReseedCiRebuildFailedSuspendedDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedCiRebuildFailedSuspendedDisabled");
			}
		}

		internal static int AutoReseedCiRebuildFailedSuspendedPeriodicIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiRebuildFailedSuspendedPeriodicIntervalInSecs");
			}
		}

		internal static int AutoReseedCiCatalogOnUpgradeIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiCatalogOnUpgradeIntervalInSecs");
			}
		}

		internal static int AutoReseedCiRebuildFailedSuspendedSuppressionInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiRebuildFailedSuspendedSuppressionInSecs");
			}
		}

		internal static int AutoReseedCiRebuildFailedSuspendedThrottlingIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiRebuildFailedSuspendedThrottlingIntervalInSecs");
			}
		}

		internal static int AutoReseedCiSuppressionInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiSuppressionInSecs");
			}
		}

		internal static int AutoReseedCiThrottlingIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedCiThrottlingIntervalInSecs");
			}
		}

		internal static bool AutoReseedCiUpgradeDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedCiUpgradeDisabled");
			}
		}

		internal static int AutoReseedDbFailedAssignSpareRetryCountMax
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedAssignSpareRetryCountMax");
			}
		}

		internal static int AutoReseedDbFailedInPlaceReseedDelayInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedInPlaceReseedDelayInSecs");
			}
		}

		internal static int AutoReseedDbFailedPeriodicIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedPeriodicIntervalInSecs");
			}
		}

		internal static int AutoReseedDbFailedReseedRetryCountMax
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedReseedRetryCountMax");
			}
		}

		internal static int AutoReseedDbFailedResumeRetryCountMax
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedResumeRetryCountMax");
			}
		}

		internal static bool AutoReseedDbFailedSuspendedDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedDbFailedSuspendedDisabled");
			}
		}

		internal static int AutoReseedDbFailedSuspendedPeriodicIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedSuspendedPeriodicIntervalInSecs");
			}
		}

		internal static int AutoReseedDbFailedSuspendedSuppressionInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedSuspendedSuppressionInSecs");
			}
		}

		internal static int AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Reseed
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Reseed");
			}
		}

		internal static int AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Resume
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Resume");
			}
		}

		internal static bool AutoReseedDbFailedSuspendedUseNeighborsForDbGroups
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedDbFailedSuspendedUseNeighborsForDbGroups");
			}
		}

		internal static int AutoReseedDbFailedSuspendedWorkflowResetIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbFailedSuspendedWorkflowResetIntervalInSecs");
			}
		}

		internal static bool AutoReseedDbFailedWorkflowDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedDbFailedWorkflowDisabled");
			}
		}

		internal static int AutoReseedDbHealthySuppressionInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbHealthySuppressionInSecs");
			}
		}

		internal static int AutoReseedDbMaxConcurrentSeeds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbMaxConcurrentSeeds");
			}
		}

		internal static bool AutoReseedDbNeverMountedDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedDbNeverMountedDisabled");
			}
		}

		internal static int AutoReseedDbNeverMountedThrottlingIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedDbNeverMountedThrottlingIntervalInSecs");
			}
		}

		internal static bool AutoReseedManagerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("AutoReseedManagerDisabled");
			}
		}

		internal static int AutoReseedManagerPollerIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedManagerPollerIntervalInSecs");
			}
		}

		internal static int AutoReseedVolumeAssignmentCacheTTLInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoReseedVolumeAssignmentCacheTTLInSecs");
			}
		}

		internal static int BCSGetCopyStatusRPCTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("BCSGetCopyStatusRPCTimeoutInMSec");
			}
		}

		internal static int BcsTotalQueueMaxThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("BcsTotalQueueMaxThreshold");
			}
		}

		internal static int CheckCatalogReadyIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CheckCatalogReadyIntervalInSec");
			}
		}

		internal static int CICurrentnessThresholdInSeconds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CICurrentnessThresholdInSeconds");
			}
		}

		internal static int CISuspendResumeTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CISuspendResumeTimeoutInSec");
			}
		}

		internal static int ClusApiHangActionLatencyAllowedInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusApiHangActionLatencyAllowedInSec");
			}
		}

		internal static int ClusApiHangReportLongLatencyDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusApiHangReportLongLatencyDurationInSec");
			}
		}

		internal static int ClusApiLatencyAllowedInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusApiLatencyAllowedInSec");
			}
		}

		internal static int ClusdbHungNodesConfirmDurationInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusdbHungNodesConfirmDurationInMSec");
			}
		}

		internal static int ClusterBatchWriterIntervalInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusterBatchWriterIntervalInMsec");
			}
		}

		internal static int ConfigInitializedCheckTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ConfigInitializedCheckTimeoutInSec");
			}
		}

		internal static int ConfigUpdaterTimerIntervalSlow
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ConfigUpdaterTimerIntervalSlow");
			}
		}

		internal static int CopyStatusPollerIntervalInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CopyStatusPollerIntervalInMsec");
			}
		}

		internal static int CopyQueueAlertThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CopyQueueAlertThreshold");
			}
		}

		internal static bool CopyStatusClientCachingDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("CopyStatusClientCachingDisabled");
			}
		}

		internal static int CorruptLogRequiredRange
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CorruptLogRequiredRange");
			}
		}

		internal static int CrimsonPeriodicLoggingIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("CrimsonPeriodicLoggingIntervalInSec");
			}
		}

		internal static int DatabaseCheckInspectorQueueLengthFailedThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseCheckInspectorQueueLengthFailedThreshold");
			}
		}

		internal static int DatabaseCheckInspectorQueueLengthWarningThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseCheckInspectorQueueLengthWarningThreshold");
			}
		}

		internal static int DatabaseHealthCheckAtLeastNCopies
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckAtLeastNCopies");
			}
		}

		internal static int DatabaseHealthCheckGreenPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckGreenPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckGreenTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckRedPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckRedPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckOneCopyGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckOneCopyGreenTransitionSuppressionInSec");
			}
		}

		internal static string DatabaseHealthCheckSkipDatabasesRegex
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<string>("DatabaseHealthCheckSkipDatabasesRegex");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusGreenPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusGreenPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusGreenTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusRedPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusRedPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusServerLevelMinStaleCopies
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusServerLevelMinStaleCopies");
			}
		}

		internal static int DatabaseHealthCheckStaleStatusServerLevelRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckStaleStatusServerLevelRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckTwoCopyGreenPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckTwoCopyGreenPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckTwoCopyGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckTwoCopyGreenTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckTwoCopyRedPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckTwoCopyRedPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckTwoCopyRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckTwoCopyRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckDelayInRaisingDatabasePotentialRedundancyAlertDueToServiceStartInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckDelayInRaisingDatabasePotentialRedundancyAlertDueToServiceStartInSec");
			}
		}

		internal static int DatabaseHealthCheckCopyConnectedErrorThresholdInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckCopyConnectedErrorThresholdInSec");
			}
		}

		internal static int DatabaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin");
			}
		}

		internal static int DatabaseHealthCheckPotentialOneCopyRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckPotentialOneCopyRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckPotentialOneCopyGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckPotentialOneCopyGreenTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckPotentialOneCopyRedPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckPotentialOneCopyRedPeriodicIntervalInSec");
			}
		}

		internal static int DatabaseHealthCheckServerLevelPotentialOneCopyRedTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckServerLevelPotentialOneCopyRedTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckServerLevelPotentialOneCopyGreenTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckServerLevelPotentialOneCopyGreenTransitionSuppressionInSec");
			}
		}

		internal static int DatabaseHealthCheckServerLevelPotentialOneCopyRedPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthCheckServerLevelPotentialOneCopyRedPeriodicIntervalInSec");
			}
		}

		internal static bool DatabaseHealthCheckSiteAlertsDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DatabaseHealthCheckSiteAlertsDisabled");
			}
		}

		internal static bool DatabaseHealthMonitorDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DatabaseHealthMonitorDisabled");
			}
		}

		internal static int DatabaseHealthMonitorPeriodicIntervalInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseHealthMonitorPeriodicIntervalInMsec");
			}
		}

		internal static bool DatabaseHealthTrackerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DatabaseHealthTrackerDisabled");
			}
		}

		internal static bool DatabaseStateTrackerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DatabaseStateTrackerDisabled");
			}
		}

		internal static int DatabaseStateTrackerInitTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseStateTrackerInitTimeoutInSec");
			}
		}

		internal static int DatabaseType
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DatabaseType");
			}
		}

		internal static int DbQueueMgrStopLimitInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DbQueueMgrStopLimitInSecs");
			}
		}

		internal static bool DisableActivationDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableActivationDisabled");
			}
		}

		internal static int DisableBugcheckOnHungIo
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DisableBugcheckOnHungIo");
			}
		}

		internal static bool DisableDatabaseScan
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableDatabaseScan");
			}
		}

		internal static bool DisableEdbLogDirectoryCreation
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableEdbLogDirectoryCreation");
			}
		}

		internal static bool DisableFailureItemProcessing
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableFailureItemProcessing");
			}
		}

		internal static bool DisableGranularReplication
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableGranularReplication");
			}
		}

		internal static bool DisableGranularReplicationOverflow
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableGranularReplicationOverflow");
			}
		}

		internal static bool DisableISeedStreamingPageReader
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableISeedStreamingPageReader");
			}
		}

		internal static bool DisableJetFailureItemPublish
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableJetFailureItemPublish");
			}
		}

		internal static bool DisableNetworkSigning
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableNetworkSigning");
			}
		}

		internal static bool DisablePriorityBoost
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisablePriorityBoost");
			}
		}

		internal static bool DisableSetBrokenFailureItemSuppression
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableSetBrokenFailureItemSuppression");
			}
		}

		internal static bool DisableSocketStream
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableSocketStream");
			}
		}

		internal static bool DisableSourceLogVerifier
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableSourceLogVerifier");
			}
		}

		internal static int DiskReclaimerDelayedStartInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DiskReclaimerDelayedStartInSecs");
			}
		}

		internal static bool DiskReclaimerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DiskReclaimerDisabled");
			}
		}

		internal static int DiskReclaimerPollerIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DiskReclaimerPollerIntervalInSecs");
			}
		}

		internal static int DiskReclaimerSpareDelayInSecs_Long
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DiskReclaimerSpareDelayInSecs_Long");
			}
		}

		internal static int DiskReclaimerSpareDelayInSecs_Medium
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DiskReclaimerSpareDelayInSecs_Medium");
			}
		}

		internal static int DiskReclaimerSpareDelayInSecs_Short
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DiskReclaimerSpareDelayInSecs_Short");
			}
		}

		internal static int DumpsterInfoCacheTTLInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterInfoCacheTTLInSec");
			}
		}

		internal static int DumpsterRedeliveryEndBufferSeconds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryEndBufferSeconds");
			}
		}

		internal static int DumpsterRedeliveryExpirationDurationInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryExpirationDurationInSecs");
			}
		}

		internal static bool DumpsterRedeliveryIgnoreBackoff
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DumpsterRedeliveryIgnoreBackoff");
			}
		}

		internal static int DumpsterRedeliveryManagerTimerIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryManagerTimerIntervalInSecs");
			}
		}

		internal static int DumpsterRedeliveryMaxTimeRangeInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryMaxTimeRangeInSecs");
			}
		}

		internal static int DumpsterRedeliveryPrimaryRetryDurationInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryPrimaryRetryDurationInSecs");
			}
		}

		internal static int DumpsterRedeliveryStartBufferSeconds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRedeliveryStartBufferSeconds");
			}
		}

		internal static int DumpsterRpcTimeoutInMSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DumpsterRpcTimeoutInMSecs");
			}
		}

		internal static int EnableNetworkChecksums
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("EnableNetworkChecksums");
			}
		}

		internal static bool EnableSupportApi
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("EnableSupportApi");
			}
		}

		internal static bool EnableV1IncReseed
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("EnableV1IncReseed");
			}
		}

		internal static bool EnableVssWriter
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("EnableVssWriter");
			}
		}

		internal static bool EnableWatsonDumpOnTooMuchMemory
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("EnableWatsonDumpOnTooMuchMemory");
			}
		}

		internal static bool EnforceDbFolderUnderMountPoint
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("EnforceDbFolderUnderMountPoint");
			}
		}

		internal static int ExtraReplayLagAllowedMinutes
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ExtraReplayLagAllowedMinutes");
			}
		}

		internal static int FailureItemHangDetectionIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemHangDetectionIntervalInSec");
			}
		}

		internal static int FailureItemManagerDatabaseListUpdaterIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemManagerDatabaseListUpdaterIntervalInSec");
			}
		}

		internal static int FailureItemLocalDatabaseOperationTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemLocalDatabaseOperationTimeoutInSec");
			}
		}

		internal static int FailureItemProcessingAllowedLatencyInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemProcessingAllowedLatencyInSec");
			}
		}

		internal static int FailureItemProcessingDelayInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemProcessingDelayInMSec");
			}
		}

		internal static int FailureItemStromCoolingDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemStromCoolingDurationInSec");
			}
		}

		internal static int FailureItemWatchdogEngageDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FailureItemWatchdogEngageDurationInSec");
			}
		}

		internal static int FileInUseRetryLimitInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FileInUseRetryLimitInSecs");
			}
		}

		internal static bool FilesystemMaintainsOrder
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("FilesystemMaintainsOrder");
			}
		}

		internal static int FullServerReseedRetryIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("FullServerReseedRetryIntervalInSec");
			}
		}

		internal static bool GetActiveCopiesForDatabaseAvailabilityGroupUseCache
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("GetActiveCopiesForDatabaseAvailabilityGroupUseCache");
			}
		}

		internal static int GetCopyStatusRpcCacheTTLInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("GetCopyStatusRpcCacheTTLInSec");
			}
		}

		internal static int GetCopyStatusServerCachedEntryStaleTimeoutSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("GetCopyStatusServerCachedEntryStaleTimeoutSec");
			}
		}

		internal static bool GetCopyStatusServerTimeoutEnabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("GetCopyStatusServerTimeoutEnabled");
			}
		}

		internal static int GetCopyStatusServerTimeoutSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("GetCopyStatusServerTimeoutSec");
			}
		}

		internal static int GetMailboxDatabaseCopyStatusRPCTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("GetMailboxDatabaseCopyStatusRPCTimeoutInMSec");
			}
		}

		internal static bool HealthStateTrackerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("HealthStateTrackerDisabled");
			}
		}

		internal static int HealthStateTrackerLookupDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("HealthStateTrackerLookupDurationInSec");
			}
		}

		internal static int HighAvailabilityWebServiceMexPort
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("HighAvailabilityWebServiceMexPort");
			}
		}

		internal static int HighAvailabilityWebServicePort
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("HighAvailabilityWebServicePort");
			}
		}

		internal static int HungCopyLimitInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("HungCopyLimitInSec");
			}
		}

		internal static bool IgnoreCatalogHealthSetByCI
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("IgnoreCatalogHealthSetByCI");
			}
		}

		internal static int IncSeedThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("IncSeedThreshold");
			}
		}

		internal static int IOBufferPoolPreallocationOverride
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("IOBufferPoolPreallocationOverride");
			}
		}

		internal static int IOSizeInBytes
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("IOSizeInBytes");
			}
		}

		internal static bool IsApiLatencyTestEnabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("IsApiLatencyTestEnabled");
			}
		}

		internal static bool IsKillClusterServiceOnClusApiHang
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("IsKillClusterServiceOnClusApiHang");
			}
		}

		internal static bool IsLogApiLatencyFailure
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("IsLogApiLatencyFailure");
			}
		}

		internal static bool KillStoreInsteadOfWatsonOnTimeout
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("KillStoreInsteadOfWatsonOnTimeout");
			}
		}

		internal static int LastLogUpdateThresholdInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LastLogUpdateThresholdInSec");
			}
		}

		internal static bool ListMdbStatusMonitorDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("ListMdbStatusMonitorDisabled");
			}
		}

		internal static int ListMdbStatusRecoveryLimitInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ListMdbStatusRecoveryLimitInSec");
			}
		}

		internal static int ListMdbStatusFailureSuppressionWindowInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ListMdbStatusFailureSuppressionWindowInSec");
			}
		}

		internal static int LogCopierHungIoLimitInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopierHungIoLimitInMsec");
			}
		}

		internal static int LogCopierStalledToFailedThresholdInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopierStalledToFailedThresholdInSecs");
			}
		}

		internal static int LogCopyBufferedIo
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopyBufferedIo");
			}
		}

		internal static int LogCopyDelayInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopyDelayInMsec");
			}
		}

		internal static int LogCopyNetworkTransferSize
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopyNetworkTransferSize");
			}
		}

		internal static int LogCopyPull
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogCopyPull");
			}
		}

		internal static int LogInspectorDelayInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogInspectorDelayInMsec");
			}
		}

		internal static int LogInspectorReadSize
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogInspectorReadSize");
			}
		}

		internal static int LogReplayerDelayInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerDelayInMsec");
			}
		}

		internal static int LogReplayerIdleStoreRpcIntervalInMSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerIdleStoreRpcIntervalInMSecs");
			}
		}

		internal static int LogReplayerMaximumLogsForReplayLag
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerMaximumLogsForReplayLag");
			}
		}

		internal static int LogReplayerMaxLogsToScanInOneIteration
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerMaxLogsToScanInOneIteration");
			}
		}

		internal static int LogReplayerPauseDurationInMSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerPauseDurationInMSecs");
			}
		}

		internal static int LogReplayerResumeThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerResumeThreshold");
			}
		}

		internal static int LogReplayerScanMoreLogsWhenReplayWithinThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerScanMoreLogsWhenReplayWithinThreshold");
			}
		}

		internal static int LogReplayerSuspendThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayerSuspendThreshold");
			}
		}

		internal static int LogReplayQueueHighPlayDownDisableSuppressionWindowInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayQueueHighPlayDownDisableSuppressionWindowInSecs");
			}
		}

		internal static int LogReplayQueueHighPlayDownEnableSuppressionWindowInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogReplayQueueHighPlayDownEnableSuppressionWindowInSecs");
			}
		}

		internal static int LogShipACLLTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogShipACLLTimeoutInMsec");
			}
		}

		internal static int LogShipCompressionDisable
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogShipCompressionDisable");
			}
		}

		internal static int LogShipTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogShipTimeoutInMsec");
			}
		}

		internal static int LogTruncationExtendedPreservation
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogTruncationExtendedPreservation");
			}
		}

		internal static bool LogTruncationKeepAllLogsForLagCopy
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("LogTruncationKeepAllLogsForLagCopy");
			}
		}

		internal static int LogTruncationOpenContextTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogTruncationOpenContextTimeoutInSec");
			}
		}

		internal static int LogTruncationTimerDuration
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("LogTruncationTimerDuration");
			}
		}

		internal static int MajorityDecisionRpcTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MajorityDecisionRpcTimeoutInSec");
			}
		}

		internal static int MaxADReplicationWaitInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaxADReplicationWaitInSec");
			}
		}

		internal static int MaxAutoDatabaseMountDial
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaxAutoDatabaseMountDial");
			}
		}

		internal static int MaxBlockModeConsumerDepthInBytes
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaxBlockModeConsumerDepthInBytes");
			}
		}

		internal static int MaximumGCHandleCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaximumGCHandleCount");
			}
		}

		internal static int MaximumProcessHandleCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaximumProcessHandleCount");
			}
		}

		internal static int MaximumProcessPrivateMemoryMB
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaximumProcessPrivateMemoryMB");
			}
		}

		internal static int MaxLogFilesToSeed
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaxLogFilesToSeed");
			}
		}

		internal static int MemoryLimitBaseInMB
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MemoryLimitBaseInMB");
			}
		}

		internal static int MemoryLimitPerDBInMB
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MemoryLimitPerDBInMB");
			}
		}

		internal static int MaximumProcessThreadCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaximumProcessThreadCount");
			}
		}

		internal static int MaximumRpcThreadCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MaximumRpcThreadCount");
			}
		}

		internal static int MdbStatusFetcherServerDownTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MdbStatusFetcherServerDownTimeoutInSec");
			}
		}

		internal static int StartupLogScanTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StartupLogScanTimeoutInSec");
			}
		}

		internal static int MdbStatusFetcherTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MdbStatusFetcherTimeoutInSec");
			}
		}

		internal static bool MonitorGCHandleCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("MonitorGCHandleCount");
			}
		}

		internal static int MonitoringADConfigManagerIntervalInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringADConfigManagerIntervalInMsec");
			}
		}

		internal static int MonitoringADConfigStaleTimeoutLongInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringADConfigStaleTimeoutLongInSec");
			}
		}

		internal static int MonitoringADConfigStaleTimeoutShortInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringADConfigStaleTimeoutShortInSec");
			}
		}

		internal static int MonitoringADGetConfigTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringADGetConfigTimeoutInSec");
			}
		}

		internal static bool MonitoringComponentDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("MonitoringComponentDisabled");
			}
		}

		internal static int MonitoringDHTInitLastUpdateTimeDiffInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTInitLastUpdateTimeDiffInSec");
			}
		}

		internal static int MonitoringDHTMissingObjectCleanupAgeThresholdInDays
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTMissingObjectCleanupAgeThresholdInDays");
			}
		}

		internal static int MonitoringDHTPeriodicIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTPeriodicIntervalInSec");
			}
		}

		internal static int MonitoringDHTPrimaryPeriodicSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTPrimaryPeriodicSuppressionInSec");
			}
		}

		internal static int MonitoringDHTPrimaryPublishPeriodicSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTPrimaryPublishPeriodicSuppressionInSec");
			}
		}

		internal static int MonitoringDHTPrimaryTransitionSuppressionInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringDHTPrimaryTransitionSuppressionInSec");
			}
		}

		internal static int MonitoringWebServicePort
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringWebServicePort");
			}
		}

		internal static int MonitoringWebServiceClientOpenTimeoutInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringWebServiceClientOpenTimeoutInSecs");
			}
		}

		internal static int MonitoringWebServiceClientCloseTimeoutInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringWebServiceClientCloseTimeoutInSecs");
			}
		}

		internal static int MonitoringWebServiceClientSendTimeoutInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringWebServiceClientSendTimeoutInSecs");
			}
		}

		internal static int MonitoringWebServiceClientReceiveTimeoutInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MonitoringWebServiceClientReceiveTimeoutInSecs");
			}
		}

		internal static int MountTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("MountTimeoutInSec");
			}
		}

		internal static int NetworkManagerStartupTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NetworkManagerStartupTimeoutInSec");
			}
		}

		internal static int NetworkStatusPollingPeriodInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NetworkStatusPollingPeriodInSecs");
			}
		}

		internal static int NodeActionDelayBetweenIterationsInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NodeActionDelayBetweenIterationsInSec");
			}
		}

		internal static int NodeActionInProgressWaitDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NodeActionInProgressWaitDurationInSec");
			}
		}

		internal static int NodeActionNodeStateJoiningWaitDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NodeActionNodeStateJoiningWaitDurationInSec");
			}
		}

		internal static int NumThreadsPerPamDbOperation
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("NumThreadsPerPamDbOperation");
			}
		}

		internal static int OpenClusterTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("OpenClusterTimeoutInSec");
			}
		}

		internal static int OnReplDownConfirmDurationBeforeFailoverInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("OnReplDownConfirmDurationBeforeFailoverInSecs");
			}
		}

		internal static int OnReplDownDurationBetweenFailoversInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("OnReplDownDurationBetweenFailoversInSecs");
			}
		}

		internal static int OnReplDownMaxAllowedFailoversAcrossDagInADay
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("OnReplDownMaxAllowedFailoversAcrossDagInADay");
			}
		}

		internal static int OnReplDownMaxAllowedFailoversPerNodeInADay
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("OnReplDownMaxAllowedFailoversPerNodeInADay");
			}
		}

		internal static bool OnReplDownFailoverEnabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("OnReplDownFailoverEnabled");
			}
		}

		internal static int PamLastLogRpcTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamLastLogRpcTimeoutInMsec");
			}
		}

		internal static int PamLastLogUpdaterIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamLastLogUpdaterIntervalInSec");
			}
		}

		internal static int PamMonitorCheckPeriodInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamMonitorCheckPeriodInSec");
			}
		}

		internal static int PamMonitorMoveClusterGroupTimeout
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamMonitorMoveClusterGroupTimeout");
			}
		}

		internal static int PamMonitorRecoveryDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamMonitorRecoveryDurationInSec");
			}
		}

		internal static int PamMonitorRoleCheckTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamMonitorRoleCheckTimeoutInSec");
			}
		}

		internal static int PamToSamDismountRpcTimeoutMediumInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamToSamDismountRpcTimeoutMediumInSec");
			}
		}

		internal static int PamToSamDismountRpcTimeoutShortInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PamToSamDismountRpcTimeoutShortInSec");
			}
		}

		internal static int PerfCounterUpdateIntervalInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("PerfCounterUpdateIntervalInMSec");
			}
		}

		internal static int QueryLogRangeTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("QueryLogRangeTimeoutInMsec");
			}
		}

		internal static int RegistryMonitorPollingIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("RegistryMonitorPollingIntervalInSec");
			}
		}

		internal static int RemoteClusterCallTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("RemoteClusterCallTimeoutInSec");
			}
		}

		internal static int RemoteDataProviderSelfCheckInterval
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("RemoteDataProviderSelfCheckInterval");
			}
		}

		internal static int RemoteRegistryTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("RemoteRegistryTimeoutInMsec");
			}
		}

		internal static bool ReplayLagManagerDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("ReplayLagManagerDisabled");
			}
		}

		internal static int ReplayLagManagerDisableLagSuppressionWindowInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayLagManagerDisableLagSuppressionWindowInSecs");
			}
		}

		internal static int ReplayLagManagerEnableLagSuppressionWindowInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayLagManagerEnableLagSuppressionWindowInSecs");
			}
		}

		internal static int ReplayLagManagerNumAvailableCopies
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayLagManagerNumAvailableCopies");
			}
		}

		internal static int ReplayLagManagerPollerIntervalInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayLagManagerPollerIntervalInMsec");
			}
		}

		internal static int ReplayLagLowSpacePlaydownThresholdInMB
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayLagLowSpacePlaydownThresholdInMB");
			}
		}

		internal static int ReplayServiceDiagnosticsIntervalMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayServiceDiagnosticsIntervalMsec");
			}
		}

		internal static int ReplayQueueAlertThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplayQueueAlertThreshold");
			}
		}

		internal static int ReplicaInstanceManagerNumThreadsPerDbCopy
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplicaInstanceManagerNumThreadsPerDbCopy");
			}
		}

		internal static int ReplicaProgressNumberOfLogsThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ReplicaProgressNumberOfLogsThreshold");
			}
		}

		internal static int RpcKillServiceTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("RpcKillServiceTimeoutInMSec");
			}
		}

		internal static int SeedCatalogProgressIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SeedCatalogProgressIntervalInSec");
			}
		}

		internal static int SeederInstanceStaleDuration
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SeederInstanceStaleDuration");
			}
		}

		internal static int SeedingNetworkTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SeedingNetworkTimeoutInMsec");
			}
		}

		internal static int SeedingNetworkTransferSize
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SeedingNetworkTransferSize");
			}
		}

		internal static int SkipIncReseedPagePatch
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SkipIncReseedPagePatch");
			}
		}

		internal static int SkippedLogsDeleteAfterAgeInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SkippedLogsDeleteAfterAgeInSecs");
			}
		}

		internal static int SkippedLogsDeletionIntervalSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SkippedLogsDeletionIntervalSecs");
			}
		}

		internal static int SlowIoThresholdInMs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SlowIoThresholdInMs");
			}
		}

		internal static int SpaceMonitorActionSuppressionWindowInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorActionSuppressionWindowInSecs");
			}
		}

		internal static bool SpaceMonitorDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("SpaceMonitorDisabled");
			}
		}

		internal static int SpaceMonitorCopyQueueThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorCopyQueueThreshold");
			}
		}

		internal static int SpaceMonitorReplayQueueThreshold
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorReplayQueueThreshold");
			}
		}

		internal static int SpaceMonitorLowSpaceThresholdInMB
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorLowSpaceThresholdInMB");
			}
		}

		internal static int SpaceMonitorMinHealthyCount
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorMinHealthyCount");
			}
		}

		internal static int SpaceMonitorPollerIntervalInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SpaceMonitorPollerIntervalInSec");
			}
		}

		internal static int StoreCrashControlCodeAckTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StoreCrashControlCodeAckTimeoutInMSec");
			}
		}

		internal static int StoreKillBugcheckTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StoreKillBugcheckTimeoutInMSec");
			}
		}

		internal static int StoreRpcConnectivityTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StoreRpcConnectivityTimeoutInSec");
			}
		}

		internal static int StoreRpcGenericTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StoreRpcGenericTimeoutInSec");
			}
		}

		internal static int StoreWatsonDumpTimeoutInMSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("StoreWatsonDumpTimeoutInMSec");
			}
		}

		internal static int SuspendLockTimeoutInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("SuspendLockTimeoutInMsec");
			}
		}

		internal static int TcpChannelIdleLimitInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TcpChannelIdleLimitInSec");
			}
		}

		internal static int TestDelayCatalogSeedSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestDelayCatalogSeedSec");
			}
		}

		internal static int TestDisableWatson
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestDisableWatson");
			}
		}

		internal static int TestMemoryLeak
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestMemoryLeak");
			}
		}

		internal static int TestServiceStartupDelay
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestServiceStartupDelay");
			}
		}

		internal static int TestStoreConnectivityTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestStoreConnectivityTimeoutInSec");
			}
		}

		internal static int TestWithFakeNetwork
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TestWithFakeNetwork");
			}
		}

		internal static int TransientFailoverSuppressionDelayInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("TransientFailoverSuppressionDelayInSec");
			}
		}

		internal static bool TreatLogCopyPartnerAsDownlevel
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("TreatLogCopyPartnerAsDownlevel");
			}
		}

		internal static bool UnboundedDatalossDisableClusterInput
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("UnboundedDatalossDisableClusterInput");
			}
		}

		internal static bool UnboundedDatalossDisableReplicationInput
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("UnboundedDatalossDisableReplicationInput");
			}
		}

		internal static int UnboundedDatalossSafeGuardDurationInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("UnboundedDatalossSafeGuardDurationInSec");
			}
		}

		internal static int WaitForCatalogReadyTimeoutInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("WaitForCatalogReadyTimeoutInSec");
			}
		}

		internal static int WatchDogTimeoutForWatsonDumpInSec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("WatchDogTimeoutForWatsonDumpInSec");
			}
		}

		internal static bool WatsonOnBlockModeConsumerOverflow
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("WatsonOnBlockModeConsumerOverflow");
			}
		}

		internal static bool WcfEnableMexEndpoint
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("WcfEnableMexEndpoint");
			}
		}

		internal static int WcfMaxConcurrentCalls
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("WcfMaxConcurrentCalls");
			}
		}

		internal static int WcfMaxConcurrentInstances
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("WcfMaxConcurrentInstances");
			}
		}

		internal static int WcfMaxConcurrentSessions
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("WcfMaxConcurrentSessions");
			}
		}

		internal static int ClusdbPeriodicCleanupStartDelayInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusdbPeriodicCleanupStartDelayInSecs");
			}
		}

		internal static int ClusdbPeriodicCleanupIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("ClusdbPeriodicCleanupIntervalInSecs");
			}
		}

		internal static bool BitlockerWin8EmptyUsedOnlyDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("BitlockerWin8EmptyUsedOnlyDisabled");
			}
		}

		internal static bool BitlockerWin7EmptyFullVolumeDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("BitlockerWin7EmptyFullVolumeDisabled");
			}
		}

		internal static bool BitlockerWin8UsedOnlyDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("BitlockerWin8UsedOnlyDisabled");
			}
		}

		internal static bool BitlockerFeatureDisabled
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("BitlockerFeatureDisabled");
			}
		}

		internal static int DistributedStorePerfTrackerFlushInMs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStorePerfTrackerFlushInMs");
			}
		}

		internal static int DistributedStoreApiExecutionPeriodicLogDurationInMs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreApiExecutionPeriodicLogDurationInMs");
			}
		}

		internal static bool DistributedStoreIsLogShadowApiResult
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreIsLogShadowApiResult");
			}
		}

		internal static bool DistributedStoreIsLogApiSuccess
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreIsLogApiSuccess");
			}
		}

		internal static bool DistributedStoreIsLogApiExecutionCallstack
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreIsLogApiExecutionCallstack");
			}
		}

		internal static int DistributedStoreShadowMaxAllowedWriteQueueLength
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreShadowMaxAllowedWriteQueueLength");
			}
		}

		internal static int DistributedStoreShadowMaxAllowedReadQueueLength
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreShadowMaxAllowedReadQueueLength");
			}
		}

		internal static int AutoMounterFirstStartupDelayInMsec
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("AutoMounterFirstStartupDelayInMsec");
			}
		}

		internal static bool DistributedStoreDisableDualClientMode
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreDisableDualClientMode");
			}
		}

		internal static bool DisableDxStoreManager
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DisableDxStoreManager");
			}
		}

		internal static int DistributedStoreConsistencyCheckPeriodicIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreConsistencyCheckPeriodicIntervalInSecs");
			}
		}

		internal static int DistributedStoreConsistencyVerifyIntervalInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreConsistencyVerifyIntervalInSecs");
			}
		}

		internal static int DistributedStoreConsistencyStartupDelayInSecs
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreConsistencyStartupDelayInSecs");
			}
		}

		internal static bool DistributedStoreDisableDxStoreFixUp
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreDisableDxStoreFixUp");
			}
		}

		internal static int DistributedStoreDiffReportVerboseFlags
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreDiffReportVerboseFlags");
			}
		}

		internal static int DistributedStoreDiffVerboseReportMaxCharsPerLine
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreDiffVerboseReportMaxCharsPerLine");
			}
		}

		internal static int DistributedStoreDagVersionCheckerDurationInSeconds
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreDagVersionCheckerDurationInSeconds");
			}
		}

		internal static int DistributedStoreStartupMinimumRequiredVersionAcrossDag
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<int>("DistributedStoreStartupMinimumRequiredVersionAcrossDag");
			}
		}

		internal static bool DistributedStoreIsLogPerformanceForSingleStore
		{
			get
			{
				return RegistryParameters.s_parameters.GetValue<bool>("DistributedStoreIsLogPerformanceForSingleStore");
			}
		}

		internal const string StoreKillBugcheckDisabledTimeKey = "StoreKillBugcheckDisabledTime";

		private const WatchdogAction DefaultFailureItemWatchdogAction = WatchdogAction.BugCheck;

		private const string EnableKernelWatchdogTimerPropertyGuid = "78341438-9b4a-4554-bbff-fd3ac2b5bbe3";

		internal const int PamMonitorRecoveryDurationInSecMinimum = 5;

		internal const int JetDatabaseType = 0;

		internal const int SqlDatabaseType = 1;

		internal const string RegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		internal const string StoreServiceRegistryKey = "SYSTEM\\CurrentControlSet\\services\\MSExchangeIS\\ParametersSystem";

		private static readonly RegistryParameterValues s_parameters = new RegistryParameterValues();

		private static ClusterNotifyFlags m_networkClusterNotificationMask = ~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY);

		private static long m_bootTimeCookie;

		private static long m_bootTimeFswCookie;

		private static WatchdogAction m_failureItemWatchdogAction = WatchdogAction.BugCheck;

		private static bool m_enableKernelWatchdogTimer = false;

		internal static string AmRemoteSiteCheckDisabledTimeKey = "AmRemoteSiteCheckDisabledTime";

		private static bool m_gLoadedRegistryValues = false;
	}
}
