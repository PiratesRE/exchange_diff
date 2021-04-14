using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBcsCopyValidation
	{
		public AmBcsCopyValidation(Guid dbGuid, string dbName, AmBcsChecks checksToRun, AmServerName sourceServer, AmServerName targetServer, RpcDatabaseCopyStatus2 copyStatus, IAmBcsErrorLogger errorLogger, AmBcsSkipFlags skipValidationChecks) : this(dbGuid, dbName, checksToRun, sourceServer, targetServer, copyStatus, errorLogger, skipValidationChecks, null)
		{
		}

		public AmBcsCopyValidation(Guid dbGuid, string dbName, AmBcsChecks checksToRun, AmServerName sourceServer, AmServerName targetServer, RpcDatabaseCopyStatus2 copyStatus, IAmBcsErrorLogger errorLogger, AmBcsSkipFlags skipValidationChecks, ComponentStateWrapper csw)
		{
			this.DbGuid = dbGuid;
			this.DbName = dbName;
			this.ChecksToRun = checksToRun;
			this.SourceServer = sourceServer;
			this.TargetServer = targetServer;
			this.CopyStatus = copyStatus;
			this.ComponentStateWrapper = csw;
			this.ErrorLogger = errorLogger;
			this.SkipValidationChecks = skipValidationChecks;
			AmTrace.Debug("AmBcsCopyValidation: Constructed with SkipValidationChecks='{0}'", new object[]
			{
				skipValidationChecks
			});
		}

		private Guid DbGuid { get; set; }

		private string DbName { get; set; }

		private AmServerName SourceServer { get; set; }

		private AmServerName TargetServer { get; set; }

		private RpcDatabaseCopyStatus2 CopyStatus { get; set; }

		private ComponentStateWrapper ComponentStateWrapper { get; set; }

		private AmBcsSkipFlags SkipValidationChecks { get; set; }

		private AmBcsChecks ChecksToRun { get; set; }

		public AmBcsChecks CompletedChecks { get; private set; }

		private IAmBcsErrorLogger ErrorLogger { get; set; }

		public static bool IsHealthyOrDisconnected(string dbName, RpcDatabaseCopyStatus2 status, AmServerName targetServer, ref LocalizedString error)
		{
			bool result = true;
			if (status.HAComponentOffline)
			{
				result = false;
				error = ReplayStrings.AmBcsDatabaseCopyIsHAComponentOffline(dbName, targetServer.NetbiosName);
			}
			else if (status.ActivationSuspended && status.CopyStatus != CopyStatusEnum.Suspended && status.CopyStatus != CopyStatusEnum.FailedAndSuspended && status.CopyStatus != CopyStatusEnum.Seeding)
			{
				result = false;
				error = ReplayStrings.AmBcsDatabaseCopyActivationSuspended(dbName, targetServer.NetbiosName, string.IsNullOrEmpty(status.SuspendComment) ? ReplayStrings.AmBcsNoneSpecified : status.SuspendComment);
			}
			else if (status.CopyStatus == CopyStatusEnum.Seeding)
			{
				result = false;
				error = ReplayStrings.AmBcsDatabaseCopySeeding(dbName, targetServer.NetbiosName);
			}
			else if (status.CopyStatus == CopyStatusEnum.Failed || status.CopyStatus == CopyStatusEnum.FailedAndSuspended)
			{
				result = false;
				error = ReplayStrings.AmBcsDatabaseCopyFailed(dbName, targetServer.NetbiosName, string.IsNullOrEmpty(status.ErrorMessage) ? ReplayStrings.AmBcsNoneSpecified : status.ErrorMessage);
			}
			else if (status.CopyStatus == CopyStatusEnum.Suspended)
			{
				result = false;
				error = ReplayStrings.AmBcsDatabaseCopySuspended(dbName, targetServer.NetbiosName, string.IsNullOrEmpty(status.SuspendComment) ? ReplayStrings.AmBcsNoneSpecified : status.SuspendComment);
			}
			return result;
		}

		public static bool IsTotalQueueLengthLessThanMaxThreshold(string dbName, RpcDatabaseCopyStatus2 status, AmServerName targetServer, ref LocalizedString error)
		{
			long num = Math.Max(0L, status.LastLogGenerated - status.LastLogReplayed);
			bool flag = num <= (long)AmBcsCopyValidation.TOTAL_QUEUE_MAX_THRESHOLD;
			if (!flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyTotalQueueLengthTooHigh(dbName, targetServer.NetbiosName, num, (long)AmBcsCopyValidation.TOTAL_QUEUE_MAX_THRESHOLD);
			}
			return flag;
		}

		public static bool IsRealCopyQueueLengthAcceptable(string dbName, RpcDatabaseCopyStatus2 status, int copyQueueThreshold, AmServerName targetServer, ref LocalizedString error)
		{
			long num = Math.Max(0L, status.LastLogGenerated - status.LastLogCopied);
			bool flag = num <= (long)copyQueueThreshold;
			if (!flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyQueueLengthTooHigh(dbName, targetServer.NetbiosName, num, (long)copyQueueThreshold);
			}
			return flag;
		}

		public bool RunChecks(ref LocalizedString error)
		{
			bool flag = true;
			AmBcsChecks checksToRun = this.ChecksToRun;
			error = LocalizedString.Empty;
			this.CompletedChecks = AmBcsChecks.None;
			if (flag && this.ShouldRunCheck(AmBcsChecks.IsPassiveCopy))
			{
				this.CompletedChecks |= AmBcsChecks.IsPassiveCopy;
				flag = this.IsPassiveCopy(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.ActivationEnabled))
			{
				this.CompletedChecks |= AmBcsChecks.ActivationEnabled;
				flag = this.IsActivationEnabled(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.MaxActivesUnderHighestLimit))
			{
				this.CompletedChecks |= AmBcsChecks.MaxActivesUnderHighestLimit;
				flag = this.IsMaxActivesUnderHighestLimit(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.IsHealthyOrDisconnected))
			{
				this.CompletedChecks |= AmBcsChecks.IsHealthyOrDisconnected;
				flag = this.IsHealthyOrDisconnected(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.TotalQueueLengthMaxAllowed))
			{
				this.CompletedChecks |= AmBcsChecks.TotalQueueLengthMaxAllowed;
				flag = this.IsTotalQueueLengthLessThanMaxThreshold(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.MaxActivesUnderPreferredLimit))
			{
				this.CompletedChecks |= AmBcsChecks.MaxActivesUnderPreferredLimit;
				flag = this.IsMaxActivesUnderPreferredLimit(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.CopyQueueLength))
			{
				this.CompletedChecks |= AmBcsChecks.CopyQueueLength;
				flag = this.IsCopyQueueLengthAcceptable(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.ReplayQueueLength))
			{
				this.CompletedChecks |= AmBcsChecks.ReplayQueueLength;
				flag = this.IsReplayQueueLengthAcceptable(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.IsSeedingSource))
			{
				this.CompletedChecks |= AmBcsChecks.IsSeedingSource;
				flag = this.IsPassiveSeedingSource(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.IsCatalogStatusHealthy))
			{
				this.CompletedChecks |= AmBcsChecks.IsCatalogStatusHealthy;
				flag = this.IsCatalogStatusHealthy(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.IsCatalogStatusCrawling))
			{
				this.CompletedChecks |= AmBcsChecks.IsCatalogStatusCrawling;
				flag = this.IsCatalogStatusCrawling(ref error);
			}
			if (flag && this.ShouldRunManagedAvailabilityChecks())
			{
				AmBcsChecks amBcsChecks;
				flag = this.IsManagedAvailabilityChecksSucceeded(ref error, out amBcsChecks);
				this.CompletedChecks |= amBcsChecks;
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.MaxActivesUnderHighestLimit))
			{
				this.CompletedChecks |= AmBcsChecks.MaxActivesUnderHighestLimit;
				flag = this.UpdateActiveIfMaxActivesNotExceededHighestLimit(ref error);
			}
			if (flag && this.ShouldRunCheck(AmBcsChecks.MaxActivesUnderPreferredLimit))
			{
				this.CompletedChecks |= AmBcsChecks.MaxActivesUnderPreferredLimit;
				flag = this.UpdateActiveIfMaxActivesNotExceededPreferredLimit(ref error);
			}
			return flag;
		}

		internal bool ShouldRunCheck(AmBcsChecks checkInQuestion)
		{
			bool flag = (this.ChecksToRun & checkInQuestion) == checkInQuestion;
			if (checkInQuestion == AmBcsChecks.IsPassiveCopy)
			{
				return flag;
			}
			return flag && !this.ShouldBeSkipped(checkInQuestion);
		}

		internal bool ShouldRunManagedAvailabilityChecks()
		{
			AmBcsChecks amBcsChecks = AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource | AmBcsChecks.ManagedAvailabilityAllHealthy | AmBcsChecks.ManagedAvailabilityUptoNormalHealthy | AmBcsChecks.ManagedAvailabilityAllBetterThanSource | AmBcsChecks.ManagedAvailabilitySameAsSource;
			return (this.ChecksToRun & amBcsChecks) > AmBcsChecks.None;
		}

		private bool ShouldBeSkipped(AmBcsChecks checkInQuestion)
		{
			if ((checkInQuestion & (AmBcsChecks)RegistryParameters.BcsCheckToDisable) == checkInQuestion)
			{
				AmTrace.Info("BCS Check {0} skipped since registry parameters is configured to skip it. (Reg.BcsCheckToDisable={1})", new object[]
				{
					checkInQuestion,
					RegistryParameters.BcsCheckToDisable
				});
				return true;
			}
			if (this.IsSkipFlagSpecified(AmBcsSkipFlags.LegacySkipAllChecks))
			{
				return true;
			}
			bool flag = false;
			if (!flag && this.IsSkipFlagSpecified(AmBcsSkipFlags.SkipClientExperienceChecks))
			{
				flag = this.IsCheckInSkippedList(AmBcsSkippedCheckDefinitions.SkipClientExperienceChecks, checkInQuestion);
			}
			if (!flag && this.IsSkipFlagSpecified(AmBcsSkipFlags.SkipHealthChecks))
			{
				flag = this.IsCheckInSkippedList(AmBcsSkippedCheckDefinitions.SkipHealthChecks, checkInQuestion);
			}
			if (!flag && this.IsSkipFlagSpecified(AmBcsSkipFlags.SkipLagChecks))
			{
				flag = this.IsCheckInSkippedList(AmBcsSkippedCheckDefinitions.SkipLagChecks, checkInQuestion);
			}
			if (!flag && this.IsSkipFlagSpecified(AmBcsSkipFlags.SkipMaximumActiveDatabasesChecks))
			{
				flag = this.IsCheckInSkippedList(AmBcsSkippedCheckDefinitions.SkipMaximumActiveDatabasesChecks, checkInQuestion);
			}
			return flag;
		}

		private bool IsSkipFlagSpecified(AmBcsSkipFlags flagInQuestion)
		{
			return (this.SkipValidationChecks & flagInQuestion) == flagInQuestion;
		}

		private bool IsCheckInSkippedList(IEnumerable<AmBcsChecks> checks, AmBcsChecks checkInQuestion)
		{
			return checks.Any((AmBcsChecks check) => (checkInQuestion & check) == check);
		}

		private void ReportCopyStatusFailure(AmBcsChecks checkThatFailed, LocalizedString error)
		{
			this.ErrorLogger.ReportCopyStatusFailure(this.TargetServer, checkThatFailed.ToString(), this.ChecksToRun.ToString(), error);
		}

		private bool IsPassiveCopy(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			bool flag = AmServerName.IsEqual(this.SourceServer, this.TargetServer);
			if (!flag && copyStatus != null && (copyStatus.CopyStatus == CopyStatusEnum.Mounted || copyStatus.CopyStatus == CopyStatusEnum.Mounting || copyStatus.CopyStatus == CopyStatusEnum.Dismounted || copyStatus.CopyStatus == CopyStatusEnum.Dismounting))
			{
				AmTrace.Error("IsPassiveCopy: Copy status for DB '{0}' has active copy status, but fActive is false! CopyStatus = '{1}'. Changing fActive to 'true'.", new object[]
				{
					this.DbName,
					copyStatus.CopyStatus
				});
				flag = true;
			}
			if (flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyHostedOnTarget(this.DbName, this.TargetServer.NetbiosName);
				this.ReportCopyStatusFailure(AmBcsChecks.IsPassiveCopy, error);
			}
			return !flag;
		}

		private bool IsHealthyOrDisconnected(ref LocalizedString error)
		{
			string dbName = this.DbName;
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			AmServerName targetServer = this.TargetServer;
			bool flag = AmBcsCopyValidation.IsHealthyOrDisconnected(dbName, copyStatus, targetServer, ref error);
			if (!flag)
			{
				this.ReportCopyStatusFailure(AmBcsChecks.IsHealthyOrDisconnected, error);
			}
			return flag;
		}

		private bool IsCatalogStatusHealthy(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			bool flag = copyStatus.ContentIndexStatus == ContentIndexStatusType.Healthy || copyStatus.ContentIndexStatus == ContentIndexStatusType.HealthyAndUpgrading;
			if (!flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyCatalogUnhealthy(this.DbName, this.TargetServer.NetbiosName, copyStatus.ContentIndexStatus.ToString());
				this.ReportCopyStatusFailure(AmBcsChecks.IsCatalogStatusHealthy, error);
			}
			return flag;
		}

		private bool IsCopyQueueLengthAcceptable(ref LocalizedString error)
		{
			string dbName = this.DbName;
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			AmServerName targetServer = this.TargetServer;
			bool flag = AmBcsCopyValidation.IsRealCopyQueueLengthAcceptable(dbName, copyStatus, 10, targetServer, ref error);
			if (!flag)
			{
				this.ReportCopyStatusFailure(AmBcsChecks.CopyQueueLength, error);
			}
			return flag;
		}

		private bool IsReplayQueueLengthAcceptable(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			long num = Math.Max(0L, copyStatus.LastLogCopied - copyStatus.LastLogReplayed);
			bool flag = num <= 10L;
			if (!flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyReplayQueueLengthTooHigh(this.DbName, this.TargetServer.NetbiosName, num, 10L);
				this.ReportCopyStatusFailure(AmBcsChecks.ReplayQueueLength, error);
			}
			return flag;
		}

		private bool IsTotalQueueLengthLessThanMaxThreshold(ref LocalizedString error)
		{
			string dbName = this.DbName;
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			AmServerName targetServer = this.TargetServer;
			bool flag = AmBcsCopyValidation.IsTotalQueueLengthLessThanMaxThreshold(dbName, copyStatus, targetServer, ref error);
			if (!flag)
			{
				this.ReportCopyStatusFailure(AmBcsChecks.TotalQueueLengthMaxAllowed, error);
			}
			return flag;
		}

		private bool IsPassiveSeedingSource(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			bool flag = copyStatus.CopyStatus != CopyStatusEnum.SeedingSource;
			if (!flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyIsSeedingSource(this.DbName, this.TargetServer.NetbiosName);
				this.ReportCopyStatusFailure(AmBcsChecks.IsSeedingSource, error);
			}
			return flag;
		}

		private bool IsCatalogStatusCrawling(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = this.CopyStatus;
			AmServerName targetServer = this.TargetServer;
			LocalizedString localizedString = LocalizedString.Empty;
			bool flag = this.IsCatalogStatusHealthy(ref localizedString);
			if (!flag)
			{
				flag = (copyStatus.ContentIndexStatus == ContentIndexStatusType.Crawling);
				if (!flag)
				{
					localizedString = ReplayStrings.AmBcsDatabaseCopyCatalogUnhealthy(this.DbName, targetServer.NetbiosName, copyStatus.ContentIndexStatus.ToString());
				}
			}
			if (!flag)
			{
				error = localizedString;
				this.ReportCopyStatusFailure(AmBcsChecks.IsCatalogStatusCrawling, error);
			}
			return flag;
		}

		private bool IsActivationEnabled(ref LocalizedString error)
		{
			if (AmBestCopySelectionHelper.IsActivationDisabled(this.TargetServer))
			{
				error = ReplayStrings.AmBcsTargetServerActivationDisabled(this.TargetServer.Fqdn);
				this.ReportCopyStatusFailure(AmBcsChecks.ActivationEnabled, error);
				return false;
			}
			return true;
		}

		internal bool IsManagedAvailabilityChecksSucceeded(ref LocalizedString error, out AmBcsChecks completedChecks)
		{
			bool flag = true;
			completedChecks = AmBcsChecks.None;
			List<string> list = new List<string>();
			if (this.ComponentStateWrapper == null)
			{
				return true;
			}
			if (this.ShouldRunCheck(AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource))
			{
				completedChecks |= AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource;
				flag = this.ComponentStateWrapper.IsInitiatorComponentBetterThanSource(this.TargetServer, list);
				if (!flag && list.Count > 0)
				{
					string failures = string.Join(",", list.ToArray());
					error = ReplayStrings.AmBcsManagedAvailabilityCheckFailed(this.SourceServer.NetbiosName, this.TargetServer.NetbiosName, this.ComponentStateWrapper.InitiatingComponentName, failures);
					this.ReportCopyStatusFailure(AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource, error);
				}
			}
			if (flag)
			{
				list.Clear();
				AmBcsChecks amBcsChecks = AmBcsChecks.None;
				if (this.ShouldRunCheck(AmBcsChecks.ManagedAvailabilityAllHealthy))
				{
					amBcsChecks = AmBcsChecks.ManagedAvailabilityAllHealthy;
					flag = this.ComponentStateWrapper.IsAllComponentsHealthy(this.TargetServer, list);
				}
				else if (this.ShouldRunCheck(AmBcsChecks.ManagedAvailabilityUptoNormalHealthy))
				{
					amBcsChecks = AmBcsChecks.ManagedAvailabilityUptoNormalHealthy;
					flag = this.ComponentStateWrapper.IsUptoNormalComponentsHealthy(this.TargetServer, list);
				}
				else if (this.ShouldRunCheck(AmBcsChecks.ManagedAvailabilityAllBetterThanSource))
				{
					amBcsChecks = AmBcsChecks.ManagedAvailabilityAllBetterThanSource;
					flag = this.ComponentStateWrapper.IsComponentsBettterThanSource(this.TargetServer, list);
				}
				else if (this.ShouldRunCheck(AmBcsChecks.ManagedAvailabilitySameAsSource))
				{
					amBcsChecks = AmBcsChecks.ManagedAvailabilitySameAsSource;
					flag = this.ComponentStateWrapper.IsComponentsAtleastSameAsSource(this.TargetServer, list);
				}
				if (flag)
				{
					completedChecks |= amBcsChecks;
				}
				else if (list.Count > 0)
				{
					string failures2 = string.Join(",", list.ToArray());
					error = ReplayStrings.AmBcsManagedAvailabilityCheckFailed(this.SourceServer.NetbiosName, this.TargetServer.NetbiosName, this.ComponentStateWrapper.InitiatingComponentName, failures2);
					this.ReportCopyStatusFailure(amBcsChecks, error);
				}
			}
			return flag;
		}

		internal bool IsMaxActivesUnderHighestLimit(ref LocalizedString error)
		{
			int? num;
			if (!AmBestCopySelectionHelper.IsMaxActivesUnderHighestLimit(this.TargetServer, out num))
			{
				error = ReplayStrings.AmBcsTargetServerMaxActivesReached(this.TargetServer.Fqdn, (num != null) ? num.Value.ToString() : "<null>");
				this.ReportCopyStatusFailure(AmBcsChecks.MaxActivesUnderHighestLimit, error);
				return false;
			}
			return true;
		}

		internal bool IsMaxActivesUnderPreferredLimit(ref LocalizedString error)
		{
			int? num;
			if (!AmBestCopySelectionHelper.IsMaxActivesUnderPreferredLimit(this.TargetServer, out num))
			{
				error = ReplayStrings.AmBcsTargetServerPreferredMaxActivesReached(this.TargetServer.Fqdn, (num != null) ? num.Value.ToString() : "<null>");
				this.ReportCopyStatusFailure(AmBcsChecks.MaxActivesUnderPreferredLimit, error);
				return false;
			}
			return true;
		}

		internal bool UpdateActiveIfMaxActivesNotExceededHighestLimit(ref LocalizedString error)
		{
			int? num;
			if (!AmBestCopySelectionHelper.UpdateActiveIfMaxActivesNotExceeded(this.DbGuid, this.TargetServer, (IADServer server) => server.MaximumActiveDatabases, out num))
			{
				error = ReplayStrings.AmBcsTargetServerMaxActivesReached(this.TargetServer.Fqdn, (num != null) ? num.Value.ToString() : "<null>");
				this.ReportCopyStatusFailure(AmBcsChecks.MaxActivesUnderHighestLimit, error);
				return false;
			}
			return true;
		}

		internal bool UpdateActiveIfMaxActivesNotExceededPreferredLimit(ref LocalizedString error)
		{
			int? num;
			if (!AmBestCopySelectionHelper.UpdateActiveIfMaxActivesNotExceeded(this.DbGuid, this.TargetServer, (IADServer server) => server.MaximumPreferredActiveDatabases, out num))
			{
				error = ReplayStrings.AmBcsTargetServerPreferredMaxActivesReached(this.TargetServer.Fqdn, (num != null) ? num.Value.ToString() : "<null>");
				this.ReportCopyStatusFailure(AmBcsChecks.MaxActivesUnderPreferredLimit, error);
				return false;
			}
			return true;
		}

		private const int COPY_QUEUE_THRESHOLD = 10;

		private const int REPLAY_QUEUE_THRESHOLD = 10;

		internal static readonly int TOTAL_QUEUE_MAX_THRESHOLD = RegistryParameters.BcsTotalQueueMaxThreshold;
	}
}
