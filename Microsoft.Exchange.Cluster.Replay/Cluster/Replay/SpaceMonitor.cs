using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Bitlocker.Utilities;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.HA.FailureItem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SpaceMonitor : TimerComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public SpaceMonitor(IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup) : base(TimeSpan.FromSeconds((double)RegistryParameters.SpaceMonitorPollerIntervalInSec), TimeSpan.FromSeconds((double)RegistryParameters.SpaceMonitorPollerIntervalInSec), "SpaceMonitor")
		{
			this.m_adConfigProvider = adConfigProvider;
			this.m_statusLookup = statusLookup;
		}

		protected override void TimerCallbackInternal()
		{
			try
			{
				this.Run();
			}
			catch (MonitoringADServiceShuttingDownException arg)
			{
				SpaceMonitor.Tracer.TraceError<MonitoringADServiceShuttingDownException>((long)this.GetHashCode(), "SpaceMonitor: Got service shutting down exception when retrieving AD config: {0}", arg);
			}
			catch (MonitoringADConfigException ex)
			{
				SpaceMonitor.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "SpaceMonitor: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.SpaceMonitorError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
		}

		private void Run()
		{
			IMonitoringADConfig config = this.m_adConfigProvider.GetConfig(true);
			if (config.ServerRole == MonitoringServerRole.Standalone)
			{
				return;
			}
			if (!config.Dag.AutoDagAutoReseedEnabled)
			{
				return;
			}
			IEnumerable<IADDatabase> enumerable = config.DatabaseMap[config.TargetServerName];
			foreach (IADDatabase db in enumerable)
			{
				this.ProcessDatabase(db, config);
			}
		}

		private void ProcessDatabase(IADDatabase db, IMonitoringADConfig adConfig)
		{
			Exception ex = null;
			try
			{
				this.NotifyLowDiskSpace(db, adConfig);
				if (this.DetectLowSpace(db, adConfig))
				{
					this.ActOnLowSpace(db);
				}
			}
			catch (DatabaseValidationException ex2)
			{
				ex = ex2;
			}
			catch (AmCommonTransientException ex3)
			{
				ex = ex3;
			}
			catch (AmServerException ex4)
			{
				ex = ex4;
			}
			catch (AmServerTransientException ex5)
			{
				ex = ex5;
			}
			catch (ADOperationException ex6)
			{
				ex = ex6;
			}
			catch (ADTransientException ex7)
			{
				ex = ex7;
			}
			if (ex != null)
			{
				SpaceMonitor.Tracer.TraceError<string>((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Got exception when processing database '{0}'. Skipping this database.", db.Name);
				ReplayCrimsonEvents.SpaceMonitorDatabaseError.LogPeriodic<string, Guid, string, Exception>(db.Guid, DiagCore.DefaultEventSuppressionInterval, db.Name, db.Guid, ex.Message, ex);
			}
		}

		internal NotificationAction NotifyLowDiskSpace(IADDatabase db, IMonitoringADConfig adConfig)
		{
			DatabaseRedundancyValidator databaseRedundancyValidator = new DatabaseRedundancyValidator(db, 1, this.m_statusLookup, adConfig, null, true);
			IHealthValidationResult healthValidationResult = databaseRedundancyValidator.Run();
			CopyStatusClientCachedEntry targetCopyStatus = healthValidationResult.TargetCopyStatus;
			if (targetCopyStatus == null || targetCopyStatus.CopyStatus == null || targetCopyStatus.Result != CopyStatusRpcResult.Success || targetCopyStatus.CopyStatus.DiskTotalSpaceBytes == 0UL)
			{
				return NotificationAction.None;
			}
			double num = (double)RegistryParameters.SpaceMonitorLowSpaceThresholdInMB / 1024.0;
			double num2 = targetCopyStatus.CopyStatus.DiskFreeSpaceBytes / 1024.0 / 1024.0 / 1024.0;
			double num3 = 0.0;
			double num4 = 0.0;
			string text = SpaceMonitor.FindVolume(new DirectoryInfo(db.EdbFilePath.PathName));
			string text2 = SpaceMonitor.FindVolume(new DirectoryInfo(db.LogFolderPath.PathName));
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && !text.Equals(text2, StringComparison.OrdinalIgnoreCase) && db.EdbFilePath.IsLocalFull)
			{
				ulong num5 = 0UL;
				ulong num6 = 0UL;
				DiskHelper.GetFreeSpace(db.EdbFilePath.PathName, out num5, out num6);
				num3 = num5 / 1024UL / 1024UL / 1024UL;
				num4 = num6 / 1024UL / 1024UL / 1024UL;
			}
			bool flag = false;
			string text3 = string.Empty;
			if (num2 <= num)
			{
				text3 = string.Format("'{0}' is low on log volume space [{1}]. Current={2:0.##} GB, Threshold={3:0.##} GB", new object[]
				{
					db.Name,
					text2,
					num2,
					num
				});
				flag = true;
			}
			if (num3 != 0.0 && num4 <= num)
			{
				text3 += string.Format("{0}'{1}' is low on EDB volume space [{2}]. Current={3:0.##} GB, Threshold={4:0.##} GB", new object[]
				{
					Environment.NewLine,
					db.Name,
					text,
					num4,
					num
				});
				flag = true;
			}
			if (flag)
			{
				if (SpaceMonitor.LastNotificationHistory.ContainsKey(db.Name) && DateTime.UtcNow - SpaceMonitor.LastNotificationHistory[db.Name] < SpaceMonitor.NotificationFrequency)
				{
					SpaceMonitor.Tracer.TraceDebug((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Database '{0}' is low on space but notification throttling is hit. LastNotify={1}, Throttling={2}mins, Message={3}", new object[]
					{
						db.Name,
						SpaceMonitor.LastNotificationHistory.ContainsKey(db.Name) ? "Never" : SpaceMonitor.LastNotificationHistory[db.Name].ToString(),
						SpaceMonitor.NotificationFrequency.TotalMinutes,
						text3
					});
					return NotificationAction.None;
				}
				Exception ex;
				if (BitlockerUtil.IsFilePathOnEncryptingVolume(db.LogFolderPath.PathName, out ex))
				{
					SpaceMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Database '{0}' is currently encrypting. Do not raise alert.", db.Name);
					return NotificationAction.None;
				}
				if (ex != null)
				{
					string text4 = string.Format("IsFilePathOnEncryptingVolume({0}) failed: {1}", db.LogFolderPath.PathName, ex.Message);
					ReplayCrimsonEvents.BitlockerQueryFailed.LogPeriodic<string, Exception>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, text4, ex);
				}
				EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeDagMgmt", "EdbAndLogVolSpace", db.Name.ToUpper(), text3, text3, ResultSeverityLevel.Critical);
				eventNotificationItem.Publish(false);
				if (SpaceMonitor.LastNotificationHistory.ContainsKey(db.Name))
				{
					SpaceMonitor.LastNotificationHistory[db.Name] = DateTime.UtcNow;
				}
				else
				{
					SpaceMonitor.LastNotificationHistory.Add(db.Name, DateTime.UtcNow);
				}
				return NotificationAction.RedRaised;
			}
			else
			{
				text3 = string.Format("{0} Status is OK - EdbFreeSpace={1:0.##} GB [{2}], LogFreeSpace={3:0.##} GB [{4}], Threshold={5:0.##} GB", new object[]
				{
					db.Name,
					num4,
					text,
					num2,
					text2,
					num
				});
				if (SpaceMonitor.LastNotificationHistory.ContainsKey(db.Name) && SpaceMonitor.LastNotificationHistory[db.Name] != DateTime.MinValue)
				{
					EventNotificationItem eventNotificationItem2 = new EventNotificationItem("MSExchangeDagMgmt", "EdbAndLogVolSpace", db.Name.ToUpper(), text3, text3, ResultSeverityLevel.Informational);
					eventNotificationItem2.Publish(false);
					SpaceMonitor.LastNotificationHistory[db.Name] = DateTime.MinValue;
					return NotificationAction.GreenRaised;
				}
				SpaceMonitor.Tracer.TraceDebug((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Database '{0}' has enough space but red notification was never raised. LastNotify={1}, Throttling={2}mins, Message={3}", new object[]
				{
					db.Name,
					SpaceMonitor.LastNotificationHistory.ContainsKey(db.Name) ? SpaceMonitor.LastNotificationHistory[db.Name].ToString() : "Never",
					SpaceMonitor.NotificationFrequency.TotalMinutes,
					text3
				});
				return NotificationAction.None;
			}
		}

		internal bool DetectLowSpace(IADDatabase db, IMonitoringADConfig adConfig)
		{
			if (db.ReplicationType != ReplicationType.Remote)
			{
				return false;
			}
			if (!FailedSuspendedCopyAutoReseedWorkflow.IsAutoReseedEnabled(db))
			{
				return false;
			}
			int num = RegistryParameters.SpaceMonitorMinHealthyCount;
			DatabaseRedundancyValidator databaseRedundancyValidator = new DatabaseRedundancyValidator(db, num, this.m_statusLookup, adConfig, null, true);
			IHealthValidationResult healthValidationResult = databaseRedundancyValidator.Run();
			CopyStatusClientCachedEntry targetCopyStatus = healthValidationResult.TargetCopyStatus;
			if (targetCopyStatus == null || targetCopyStatus.CopyStatus == null || targetCopyStatus.Result != CopyStatusRpcResult.Success || targetCopyStatus.CopyStatus.CopyStatus != CopyStatusEnum.Healthy || targetCopyStatus.CopyStatus.DiskTotalSpaceBytes == 0UL)
			{
				return false;
			}
			if (targetCopyStatus.CopyStatus.LastLogInfoIsStale || (targetCopyStatus.CopyStatus.GetCopyQueueLength() < (long)RegistryParameters.SpaceMonitorCopyQueueThreshold && targetCopyStatus.CopyStatus.GetReplayQueueLength() < (long)RegistryParameters.SpaceMonitorReplayQueueThreshold))
			{
				return false;
			}
			if (healthValidationResult.IsTargetCopyHealthy)
			{
				num++;
			}
			long num2 = (long)(targetCopyStatus.CopyStatus.DiskFreeSpaceBytes / 1048576UL);
			long num3 = (long)(targetCopyStatus.CopyStatus.DiskTotalSpaceBytes / 1048576UL);
			long num4 = 0L;
			bool flag = false;
			if (num2 <= targetCopyStatus.CopyStatus.GetCopyQueueLength() || num3 <= 0L)
			{
				flag = true;
			}
			else
			{
				num4 = (num2 - targetCopyStatus.CopyStatus.GetCopyQueueLength()) * 1048576L / 1048576L;
				if (num4 <= (long)RegistryParameters.SpaceMonitorLowSpaceThresholdInMB)
				{
					flag = true;
				}
			}
			SpaceMonitor.Tracer.TraceDebug((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Database '{0}' has {1}MB remaining = {2}MB effective. MinHealthy={3} ReportedHealthy={4}", new object[]
			{
				db.Name,
				num2,
				num4,
				num,
				healthValidationResult.HealthyCopiesCount
			});
			if (!flag)
			{
				this.m_actionSuppression.ReportSuccess(db.Guid);
				return false;
			}
			bool flag2 = !this.m_actionSuppression.ReportFailure(db.Guid, this.SuppressionWindow);
			SpaceMonitor.Tracer.TraceError<string, bool>((long)this.GetHashCode(), "SpaceMonitor.ProcessDatabase: Local copy of Database '{0}' is low on space. Action suppressed = {1}", db.Name, flag2);
			if (flag2)
			{
				return false;
			}
			ReplayCrimsonEvents.SpaceMonitorLowSpaceDetected.LogPeriodic<string, Guid, long, long, string, long, long, long, long, int, int>(db.Guid, this.m_lowSpaceLoggingPeriod, db.Name, db.Guid, num3, num2, string.Format("{0:0.000}", (double)num2 / (double)num3 * 100.0), targetCopyStatus.CopyStatus.GetCopyQueueLength(), targetCopyStatus.CopyStatus.GetReplayQueueLength(), targetCopyStatus.CopyStatus.LowestLogPresent, targetCopyStatus.CopyStatus.LastLogCopied, healthValidationResult.HealthyCopiesCount, num);
			return healthValidationResult.HealthyCopiesCount >= num;
		}

		private void ActOnLowSpace(IADDatabase db)
		{
			ExEventLog.EventTuple tuple_DatabaseSuspendedDueToLowSpace = ReplayEventLogConstants.Tuple_DatabaseSuspendedDueToLowSpace;
			tuple_DatabaseSuspendedDueToLowSpace.LogEvent(db.Name, new object[]
			{
				db.Name
			});
			FailureItemPublisherHelper.PublishFailureItem(FailureNameSpace.DagManagement, FailureTag.LowDiskSpaceStraggler, db.Guid, db.Name, null);
		}

		private static string FindVolume(DirectoryInfo directoryInfo)
		{
			DirectoryInfo directoryInfo2 = directoryInfo;
			if (directoryInfo2 == null || !directoryInfo2.Exists)
			{
				return string.Empty;
			}
			while (directoryInfo2.Parent != null)
			{
				if ((directoryInfo2.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
				{
					return directoryInfo2.FullName;
				}
				directoryInfo2 = directoryInfo2.Parent;
			}
			return directoryInfo2.FullName;
		}

		public const string NotificationItemServiceName = "MSExchangeDagMgmt";

		public const string NotificationItemComponent = "EdbAndLogVolSpace";

		private static readonly TimeSpan NotificationFrequency = TimeSpan.FromMinutes(15.0);

		private static Dictionary<string, DateTime> LastNotificationHistory = new Dictionary<string, DateTime>();

		private readonly TimeSpan SuppressionWindow = TimeSpan.FromSeconds((double)RegistryParameters.SpaceMonitorActionSuppressionWindowInSecs);

		private IMonitoringADConfigProvider m_adConfigProvider;

		private ICopyStatusClientLookup m_statusLookup;

		private readonly TimeSpan m_lowSpaceLoggingPeriod = new TimeSpan(0, 30, 0);

		private TransientDatabaseErrorSuppression m_actionSuppression = new TransientDatabaseErrorSuppression();
	}
}
