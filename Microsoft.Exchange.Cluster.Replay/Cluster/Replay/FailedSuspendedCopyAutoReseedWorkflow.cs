using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FailedSuspendedCopyAutoReseedWorkflow : AutoReseedWorkflow
	{
		public FailedSuspendedCopyAutoReseedWorkflow(AutoReseedContext context, string workflowLaunchReason) : base(AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed, workflowLaunchReason, context)
		{
		}

		public static IDisposable SetTestHook(Action<string, Guid, uint> testResumeRpc)
		{
			return FailedSuspendedCopyAutoReseedWorkflow.hookableResumeRpc.SetTestHook(testResumeRpc);
		}

		public static IDisposable SetTestHook(Action<FailedSuspendedCopyAutoReseedWorkflow> testSeedRpc)
		{
			return FailedSuspendedCopyAutoReseedWorkflow.hookableSeedRpc.SetTestHook(testSeedRpc);
		}

		public static IDisposable SetTestHook(Func<AutoReseedContext, LocalizedString> testCheckExVolumes)
		{
			return FailedSuspendedCopyAutoReseedWorkflow.hookableCheckExVolumes.SetTestHook(testCheckExVolumes);
		}

		protected override bool IsDisabled
		{
			get
			{
				return RegistryParameters.AutoReseedDbFailedSuspendedDisabled;
			}
		}

		protected override TimeSpan GetThrottlingInterval(AutoReseedWorkflowState state)
		{
			switch (state.LastReseedRecoveryAction)
			{
			case ReseedState.Unknown:
			case ReseedState.Resume:
			case ReseedState.Completed:
				return TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Resume);
			case ReseedState.AssignSpare:
			case ReseedState.InPlaceReseed:
				return TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Reseed);
			default:
				return TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedThrottlingIntervalInSecs_Resume);
			}
		}

		public static bool IsAutoReseedEnabled(IADDatabase db)
		{
			return true;
		}

		protected override LocalizedString RunPrereqs(AutoReseedWorkflowState state)
		{
			LocalizedString result = base.RunPrereqs(state);
			if (!result.IsEmpty)
			{
				return result;
			}
			result = FailedSuspendedCopyAutoReseedWorkflow.hookableCheckExVolumes.Value(base.Context);
			if (!result.IsEmpty)
			{
				return result;
			}
			result = FailedSuspendedCopyAutoReseedWorkflow.CheckDatabaseLogPaths(base.Context);
			if (!result.IsEmpty)
			{
				return result;
			}
			if (base.Context.TargetCopyStatus.CopyStatus.ActionInitiator == ActionInitiatorType.Administrator)
			{
				base.TraceError("RunPrereqs(): The DatabaseCopy has been suspended by an Administrator so AutoReseed will not be attempted.", new object[0]);
				return ReplayStrings.AutoReseedFailedAdminSuspended;
			}
			if (base.Context.TargetCopyStatus.CopyStatus.ReseedBlocked)
			{
				string messageOrNoneString = AmExceptionHelper.GetMessageOrNoneString(base.Context.TargetCopyStatus.CopyStatus.ErrorMessage);
				base.TraceError("RunPrereqs(): The DatabaseCopy is marked as ReseedBlocked so AutoReseed will not be attempted. Database copy ErrorMessage: {0}", new object[]
				{
					messageOrNoneString
				});
				return ReplayStrings.AutoReseedFailedReseedBlocked(messageOrNoneString);
			}
			this.ResetWorkflowRecoveryActionIfNecessary(state);
			if (!state.IsLastReseedRecoveryActionPending())
			{
				base.TraceDebug("RunPrereqs(): Running the workflow for the first time, so starting with the Resume action. LastReseedRecoveryAction = {0}", new object[]
				{
					state.LastReseedRecoveryAction
				});
				state.UpdateReseedRecoveryAction(ReseedState.Resume);
			}
			if (state.LastReseedRecoveryAction == ReseedState.Resume)
			{
				result = this.RunPrereqsForResume(state);
			}
			if (state.LastReseedRecoveryAction == ReseedState.AssignSpare)
			{
				result = this.RunPrereqsForAssignSpare(state);
			}
			if (state.LastReseedRecoveryAction == ReseedState.InPlaceReseed)
			{
				result = this.RunPrereqsForInPlaceReseed(state);
			}
			return result;
		}

		protected override Exception ExecuteInternal(AutoReseedWorkflowState state)
		{
			Exception result = null;
			if (state.LastReseedRecoveryAction == ReseedState.Unknown || state.LastReseedRecoveryAction == ReseedState.Resume)
			{
				result = this.ExecuteResume(state);
			}
			else if (state.LastReseedRecoveryAction == ReseedState.AssignSpare)
			{
				result = this.ExecuteAssignSpare(state);
			}
			else if (state.LastReseedRecoveryAction == ReseedState.InPlaceReseed)
			{
				result = this.ExecuteInPlaceReseed(state);
			}
			return result;
		}

		internal static LocalizedString CheckExchangeVolumesPresent(AutoReseedContext context)
		{
			IVolumeManager volumeManager = context.VolumeManager;
			int num = (from vol in volumeManager.Volumes
			where vol.IsExchangeVolume
			select vol).Count<ExchangeVolume>();
			if (num < 1)
			{
				return ReplayStrings.AutoReseedNoExchangeVolumesConfigured(context.Dag.AutoDagVolumesRootFolderPath.PathName);
			}
			return LocalizedString.Empty;
		}

		internal static LocalizedString CheckDatabaseLogPaths(AutoReseedContext context)
		{
			string path = Path.Combine(context.Dag.AutoDagDatabasesRootFolderPath.PathName, context.TargetCopyStatus.CopyStatus.DBName);
			string path2;
			string path3;
			VolumeManager.GetDatabaseLogEdbFolderNames(context.Database.Name, out path2, out path3);
			MountedFolderPath mountedFolderPath = new MountedFolderPath(Path.Combine(path, path2));
			MountedFolderPath mountedFolderPath2 = new MountedFolderPath(Path.Combine(path, path3));
			MountedFolderPath mountedFolderPath3 = new MountedFolderPath(context.Database.LogFolderPath.PathName);
			MountedFolderPath mountedFolderPath4 = new MountedFolderPath(Path.GetDirectoryName(context.Database.EdbFilePath.PathName));
			if (!mountedFolderPath3.Equals(mountedFolderPath))
			{
				return ReplayStrings.AutoReseedInvalidLogFolderPath(mountedFolderPath3.RawString, mountedFolderPath.RawString);
			}
			if (!mountedFolderPath4.Equals(mountedFolderPath2))
			{
				return ReplayStrings.AutoReseedInvalidEdbFolderPath(mountedFolderPath4.RawString, mountedFolderPath2.RawString);
			}
			return LocalizedString.Empty;
		}

		private void ResetWorkflowRecoveryActionIfNecessary(AutoReseedWorkflowState state)
		{
			TimeSpan timeSpan;
			if (!base.GetWorkflowElapsedExecutionTime(state, out timeSpan))
			{
				base.TraceDebug("ResetWorkflowRecoveryActionIfNecessary(): Doing nothing.", new object[0]);
				return;
			}
			TimeSpan t = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedWorkflowResetIntervalInSecs);
			if (!(timeSpan >= t))
			{
				base.TraceDebug("ResetWorkflowRecoveryActionIfNecessary(): Doing nothing since elapsedExecutionTime of '{0}' is too recent.", new object[]
				{
					timeSpan
				});
				return;
			}
			if (!state.IsLastReseedRecoveryActionPending())
			{
				base.TraceDebug("ResetWorkflowRecoveryActionIfNecessary(): Successfully completed workflow is being reset after duration of '{0}'.", new object[]
				{
					timeSpan
				});
				state.ResetReseedRecoveryAction();
				return;
			}
			if (state.LastReseedRecoveryAction == ReseedState.InPlaceReseed && state.ReseedRecoveryActionRetryCount >= RegistryParameters.AutoReseedDbFailedReseedRetryCountMax)
			{
				base.TraceDebug("ResetWorkflowRecoveryActionIfNecessary(): Workflow is being reset after duration of '{0}'.", new object[]
				{
					timeSpan
				});
				ReplayCrimsonEvents.AutoReseedWorkflowReset.Log<string, Guid, string, string, DateTime>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, state.WorkflowExecutionTime);
				state.ResetReseedRecoveryAction();
				return;
			}
			base.TraceDebug("ResetWorkflowRecoveryActionIfNecessary(): Doing nothing since the workflow is still attempting to fix the copy.", new object[0]);
		}

		private bool IsVolumeRecentlyAssigned(IEnumerable<CopyStatusClientCachedEntry> targetDbSet, out MountedFolderPath volumeName)
		{
			bool flag = false;
			volumeName = MountedFolderPath.Empty;
			CopyStatusClientCachedEntry[] array = (from status in targetDbSet
			orderby status.CopyStatus.LastDatabaseVolumeNameTransitionTime descending
			select status).ToArray<CopyStatusClientCachedEntry>();
			DateTime lastDatabaseVolumeNameTransitionTime = array[0].CopyStatus.LastDatabaseVolumeNameTransitionTime;
			if (lastDatabaseVolumeNameTransitionTime != ReplayState.ZeroFileTime)
			{
				if (lastDatabaseVolumeNameTransitionTime > base.Context.TargetCopyStatus.CopyStatus.LastStatusTransitionTime)
				{
					flag = true;
					volumeName = new MountedFolderPath(array[0].CopyStatus.LastDatabaseVolumeName);
				}
				base.TraceDebug("IsVolumeRecentlyAssigned(): A previous auto-reseed (maybe of a different database copy) had assigned a new spare volume '{3}' at '{0} UTC'. LastStatusTransitionTime = '{1} UTC', recentlyAssigned = {2}", new object[]
				{
					lastDatabaseVolumeNameTransitionTime,
					base.Context.TargetCopyStatus.CopyStatus.LastStatusTransitionTime,
					flag,
					volumeName
				});
			}
			return flag;
		}

		private bool DoesMountPointNeedToBeFixed(IEnumerable<CopyStatusClientCachedEntry> targetDbSet, out MountedFolderPath volumeName)
		{
			volumeName = MountedFolderPath.Empty;
			RpcDatabaseCopyStatus2 copyStatus = base.Context.TargetCopyStatus.CopyStatus;
			if (FailedSuspendedCopyAutoReseedWorkflow.DoesCopyHaveMountPointConfigured(copyStatus))
			{
				return false;
			}
			CopyStatusClientCachedEntry copyStatusClientCachedEntry = targetDbSet.FirstOrDefault((CopyStatusClientCachedEntry status) => status.Result == CopyStatusRpcResult.Success && FailedSuspendedCopyAutoReseedWorkflow.DoesCopyHaveMountPointConfigured(status.CopyStatus));
			if (copyStatusClientCachedEntry == null)
			{
				base.TraceError("Database copy is missing a mount point, but no other copies in the grouping have mount points either.", new object[0]);
				return false;
			}
			volumeName = new MountedFolderPath(copyStatusClientCachedEntry.CopyStatus.DatabaseVolumeName);
			return true;
		}

		private void UpdateVolumeInfoCopyState(Guid dbGuid)
		{
			IReplicaInstanceManager replicaInstanceManager = base.Context.ReplicaInstanceManager;
			base.Context.VolumeManager.UpdateVolumeInfoCopyState(dbGuid, replicaInstanceManager);
		}

		private IEnumerable<CopyStatusClientCachedEntry> FindTargetDbSetFromDatabaseGroup(AutoReseedContext context)
		{
			IADDatabase database = context.Database;
			IMonitoringADConfig adConfig = context.AdConfig;
			string targetGroup = database.DatabaseGroup;
			if (string.IsNullOrWhiteSpace(targetGroup))
			{
				AutoReseedWorkflow.Tracer.TraceError<string>((long)this.GetHashCode(), "FindTargetDbSetFromDatabaseGroup: DatabaseGroup property for db '{0}' is not set. Returning 'null'.", database.Name);
				return null;
			}
			IEnumerable<IADDatabase> enumerable = adConfig.DatabaseMap[adConfig.TargetServerName];
			if (enumerable == null)
			{
				AutoReseedWorkflow.Tracer.TraceError<AmServerName>((long)this.GetHashCode(), "FindTargetDbSetFromDatabaseGroup: Server '{0}' had no databases. Returning 'null'.", adConfig.TargetServerName);
				return null;
			}
			IEnumerable<IADDatabase> source = from db in enumerable
			where StringUtil.IsEqualIgnoreCase(db.DatabaseGroup, targetGroup)
			select db;
			int num = source.Count<IADDatabase>();
			if (num != context.Dag.AutoDagDatabaseCopiesPerVolume)
			{
				AutoReseedWorkflow.Tracer.TraceError<int, string, int>((long)this.GetHashCode(), "FindTargetDbSetFromDatabaseGroup: There are only '{0}' db copies in the group for database '{1}'. Expected number is: {2}. Returning 'null'.", num, database.Name, context.Dag.AutoDagDatabaseCopiesPerVolume);
				return null;
			}
			Dictionary<Guid, IADDatabase> dbGroupMap = source.ToDictionary((IADDatabase db) => db.Guid);
			IEnumerable<CopyStatusClientCachedEntry> source2 = from status in context.CopyStatusesForTargetServer
			where dbGroupMap.ContainsKey(status.DbGuid)
			select status;
			return source2.ToArray<CopyStatusClientCachedEntry>();
		}

		private IEnumerable<CopyStatusClientCachedEntry> FindTargetDbSetFromNeighbors(AutoReseedContext context)
		{
			IEnumerable<CopyStatusClientCachedEntry> enumerable = null;
			IEnumerable<CopyStatusClientCachedEntry> enumerable2 = null;
			Dictionary<AmServerName, IEnumerable<CopyStatusClientCachedEntry>> copyStatusesForDag = context.CopyStatusesForDag;
			IEnumerable<KeyValuePair<AmServerName, IEnumerable<CopyStatusClientCachedEntry>>> enumerable3 = from kvp in copyStatusesForDag
			let server = kvp.Key
			let isTargetServer = this.IsTargetServer(server, context.TargetServerName)
			where this.ShouldSelectServer(isTargetServer)
			orderby isTargetServer descending
			select kvp;
			foreach (KeyValuePair<AmServerName, IEnumerable<CopyStatusClientCachedEntry>> keyValuePair in enumerable3)
			{
				AmServerName key = keyValuePair.Key;
				IEnumerable<CopyStatusClientCachedEntry> value = keyValuePair.Value;
				IEnumerable<CopyStatusClientCachedEntry> source = from status in value
				where status.Result == CopyStatusRpcResult.Success && FailedSuspendedCopyAutoReseedWorkflow.DoesCopyHaveMountPointConfigured(status.CopyStatus)
				select status;
				IEnumerable<IGrouping<string, CopyStatusClientCachedEntry>> databaseSets = source.GroupBy((CopyStatusClientCachedEntry status) => status.CopyStatus.DatabaseVolumeName, StringComparer.OrdinalIgnoreCase);
				IGrouping<string, CopyStatusClientCachedEntry> grouping = FailedSuspendedCopyAutoReseedWorkflow.FindDatabaseGrouping(context, databaseSets);
				if (grouping != null)
				{
					int num = grouping.Count<CopyStatusClientCachedEntry>();
					if (num == context.Dag.AutoDagDatabaseCopiesPerVolume)
					{
						enumerable2 = grouping;
						break;
					}
				}
			}
			if (enumerable2 != null)
			{
				Dictionary<Guid, CopyStatusClientCachedEntry> remoteDbMap = enumerable2.ToDictionary((CopyStatusClientCachedEntry status) => status.DbGuid);
				enumerable = from status in context.CopyStatusesForTargetServer
				where remoteDbMap.ContainsKey(status.DbGuid)
				select status;
				enumerable = enumerable.ToArray<CopyStatusClientCachedEntry>();
			}
			return enumerable;
		}

		private static IGrouping<string, CopyStatusClientCachedEntry> FindDatabaseGrouping(AutoReseedContext context, IEnumerable<IGrouping<string, CopyStatusClientCachedEntry>> databaseSets)
		{
			foreach (IGrouping<string, CopyStatusClientCachedEntry> grouping in databaseSets)
			{
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in grouping)
				{
					if (copyStatusClientCachedEntry.DbGuid.Equals(context.Database.Guid))
					{
						return grouping;
					}
				}
			}
			return null;
		}

		private int IsTargetServer(AmServerName server, AmServerName targetServer)
		{
			if (!server.Equals(targetServer))
			{
				return 0;
			}
			return 1;
		}

		private bool ShouldSelectServer(int isTargetServer)
		{
			return Convert.ToBoolean(isTargetServer) || RegistryParameters.AutoReseedDbFailedSuspendedUseNeighborsForDbGroups;
		}

		private static string GetDatabaseNames(IEnumerable<CopyStatusClientCachedEntry> targetDbSet)
		{
			IEnumerable<string> values = (from status in targetDbSet
			select status.CopyStatus.DBName).SortDatabaseNames();
			return string.Join(", ", values);
		}

		private LocalizedString RunPrereqsForResume(AutoReseedWorkflowState state)
		{
			LocalizedString result = LocalizedString.Empty;
			this.TraceBeginPrereqs(state);
			if (base.Context.TargetCopyStatus.CopyStatus.ResumeBlocked)
			{
				string messageOrNoneString = AmExceptionHelper.GetMessageOrNoneString(base.Context.TargetCopyStatus.CopyStatus.ErrorMessage);
				result = ReplayStrings.AutoReseedFailedResumeBlocked(messageOrNoneString);
				base.TraceError("RunPrereqsForResume(): The DatabaseCopy is marked as ResumeBlocked, so Resume stage is being skipped. Workflow will try AssignSpare stage next. DatabaseCopy has ErrorMessage: {0}", new object[]
				{
					messageOrNoneString
				});
				ReplayCrimsonEvents.AutoReseedWorkflowDbResumeBlocked.Log<string, Guid, string, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, messageOrNoneString);
				state.UpdateReseedRecoveryAction(ReseedState.AssignSpare);
			}
			else if (state.ReseedRecoveryActionRetryCount >= RegistryParameters.AutoReseedDbFailedResumeRetryCountMax)
			{
				int autoReseedDbFailedResumeRetryCountMax = RegistryParameters.AutoReseedDbFailedResumeRetryCountMax;
				result = ReplayStrings.AutoReseedFailedResumeRetryExceeded(autoReseedDbFailedResumeRetryCountMax);
				base.TraceError("RunPrereqsForResume(): Failing 'Resume' prereqs since ReseedRecoveryActionRetryCount ({0}) exceeds AutoReseedDbFailedResumeRetryCountMax ({1}). Workflow will try AssignSpare stage next.", new object[]
				{
					state.ReseedRecoveryActionRetryCount,
					autoReseedDbFailedResumeRetryCountMax
				});
				ReplayCrimsonEvents.AutoReseedWorkflowDbResumeRetryExceeded.Log<string, Guid, string, string, int>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, autoReseedDbFailedResumeRetryCountMax);
				state.UpdateReseedRecoveryAction(ReseedState.AssignSpare);
			}
			return result;
		}

		private LocalizedString RunPrereqsForAssignSpare(AutoReseedWorkflowState state)
		{
			this.TraceBeginPrereqs(state);
			if (state.ReseedRecoveryActionRetryCount >= RegistryParameters.AutoReseedDbFailedAssignSpareRetryCountMax)
			{
				int autoReseedDbFailedAssignSpareRetryCountMax = RegistryParameters.AutoReseedDbFailedAssignSpareRetryCountMax;
				base.TraceError("RunPrereqsForAssignSpare(): Failing 'AssignSpare' prereqs since ReseedRecoveryActionRetryCount ({0}) exceeds AutoReseedDbFailedAssignSpareRetryCountMax ({1}). Workflow will try InPlaceReseed stage next.", new object[]
				{
					state.ReseedRecoveryActionRetryCount,
					autoReseedDbFailedAssignSpareRetryCountMax
				});
				ReplayCrimsonEvents.AutoReseedWorkflowDbSpareRetryExceeded.Log<string, Guid, string, string, int>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, autoReseedDbFailedAssignSpareRetryCountMax);
				state.UpdateReseedRecoveryAction(ReseedState.InPlaceReseed);
				return ReplayStrings.AutoReseedFailedSeedRetryExceeded(autoReseedDbFailedAssignSpareRetryCountMax);
			}
			state.UpdateReseedRecoveryAction(ReseedState.AssignSpare);
			if (string.IsNullOrEmpty(base.Context.TargetCopyStatus.CopyStatus.DatabaseVolumeName) || string.IsNullOrEmpty(base.Context.TargetCopyStatus.CopyStatus.LogVolumeName))
			{
				return ReplayStrings.AutoReseedFailedToFindTargetVolumeName(AmExceptionHelper.GetMessageOrNoneString(base.Context.TargetCopyStatus.CopyStatus.VolumeInfoLastError));
			}
			MountedFolderPath mountedFolderPath = new MountedFolderPath(base.Context.TargetCopyStatus.CopyStatus.LogVolumeName);
			MountedFolderPath other = new MountedFolderPath(base.Context.TargetCopyStatus.CopyStatus.DatabaseVolumeName);
			if (!mountedFolderPath.Equals(other))
			{
				return ReplayStrings.AutoReseedLogAndDbNotOnSameVolume;
			}
			IEnumerable<CopyStatusClientCachedEntry> enumerable = this.FindTargetDbSetFromDatabaseGroup(base.Context);
			if (enumerable == null)
			{
				enumerable = this.FindTargetDbSetFromNeighbors(base.Context);
				if (enumerable == null)
				{
					return ReplayStrings.AutoReseedFailedToFindVolumeName;
				}
			}
			MountedFolderPath mountedFolderPath2;
			if (this.IsVolumeRecentlyAssigned(enumerable, out mountedFolderPath2))
			{
				ReplayCrimsonEvents.AutoReseedWorkflowDbSpareRecentlyAssigned.Log<string, Guid, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason);
				state.AssignedVolumeName = mountedFolderPath2.Path;
				state.UpdateReseedRecoveryAction(ReseedState.InPlaceReseed);
			}
			else if (this.DoesMountPointNeedToBeFixed(enumerable, out mountedFolderPath2))
			{
				base.TraceDebug("Database copy is missing a mount point, and it will be fixed up to point to volume '{0}'", new object[]
				{
					mountedFolderPath2
				});
				this.m_volumeForMissingMountPoint = mountedFolderPath2;
			}
			else
			{
				string databaseNames = FailedSuspendedCopyAutoReseedWorkflow.GetDatabaseNames(enumerable);
				if (!enumerable.All((CopyStatusClientCachedEntry status) => !status.IsActive))
				{
					return ReplayStrings.AutoReseedNotAllCopiesPassive(databaseNames);
				}
				if (!enumerable.All((CopyStatusClientCachedEntry status) => status.Result == CopyStatusRpcResult.Success && status.CopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended))
				{
					return ReplayStrings.AutoReseedNotAllCopiesOnVolumeFailedSuspended(databaseNames);
				}
			}
			this.m_targetDbSet = enumerable;
			return LocalizedString.Empty;
		}

		private TimeSpan GetWorkflowSuppressionInterval(TimeSpan suggestedSuppression)
		{
			if (suggestedSuppression > FailedSuspendedCopyAutoReseedWorkflow.OneDay)
			{
				return FailedSuspendedCopyAutoReseedWorkflow.OneDay;
			}
			return suggestedSuppression;
		}

		private LocalizedString RunPrereqsForInPlaceReseed(AutoReseedWorkflowState state)
		{
			this.TraceBeginPrereqs(state);
			int autoReseedDbFailedReseedRetryCountMax = RegistryParameters.AutoReseedDbFailedReseedRetryCountMax;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedWorkflowResetIntervalInSecs);
			timeSpan = this.GetWorkflowSuppressionInterval(timeSpan);
			if (state.ReseedRecoveryActionRetryCount >= RegistryParameters.AutoReseedDbFailedReseedRetryCountMax)
			{
				state.WriteWorkflowNextExecutionDueTime(timeSpan);
				base.TraceError("RunPrereqsForInPlaceReseed(): Failing 'InPlaceReseed' prereqs since ReseedRecoveryActionRetryCount ({0}) exceeds AutoReseedDbFailedReseedRetryCountMax ({1}). Workflow will next run after {2}.", new object[]
				{
					state.ReseedRecoveryActionRetryCount,
					autoReseedDbFailedReseedRetryCountMax,
					timeSpan
				});
				ReplayCrimsonEvents.AutoReseedWorkflowDbReseedRetryExceeded.Log<string, Guid, string, string, int, TimeSpan>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, autoReseedDbFailedReseedRetryCountMax, timeSpan);
				return ReplayStrings.AutoReseedFailedSeedRetryExceeded(autoReseedDbFailedReseedRetryCountMax);
			}
			if (base.Context.TargetCopyStatus.CopyStatus.InPlaceReseedBlocked)
			{
				IEnumerable<CopyStatusClientCachedEntry> enumerable = this.FindTargetDbSetFromDatabaseGroup(base.Context);
				if (enumerable == null)
				{
					enumerable = this.FindTargetDbSetFromNeighbors(base.Context);
					if (enumerable == null)
					{
						return ReplayStrings.AutoReseedFailedToFindVolumeName;
					}
				}
				MountedFolderPath mountedFolderPath;
				if (!this.IsVolumeRecentlyAssigned(enumerable, out mountedFolderPath))
				{
					string messageOrNoneString = AmExceptionHelper.GetMessageOrNoneString(base.Context.TargetCopyStatus.CopyStatus.ErrorMessage);
					state.WriteWorkflowNextExecutionDueTime(timeSpan);
					state.ReseedRecoveryActionRetryCount = RegistryParameters.AutoReseedDbFailedReseedRetryCountMax;
					base.TraceError("RunPrereqsForInPlaceReseed(): Failing 'InPlaceReseed' prereqs since in-place reseed is blocked. DatabaseCopy has ErrorMessage: {0}. Workflow will next run after {1}.", new object[]
					{
						messageOrNoneString,
						timeSpan
					});
					ReplayCrimsonEvents.AutoReseedWorkflowDbInPlaceReseedBlocked.Log<string, Guid, string, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, messageOrNoneString);
					return ReplayStrings.AutoReseedFailedInPlaceReseedBlocked(messageOrNoneString);
				}
			}
			string edbFilePath = base.Context.Database.EdbFilePath.PathName;
			if (File.Exists(edbFilePath) && !state.IgnoreInPlaceOverwriteDelay)
			{
				DateTime lastWriteTimeUtc = DateTime.MaxValue;
				Exception ex = MountPointUtil.HandleIOExceptions(delegate
				{
					FileInfo fileInfo = new FileInfo(edbFilePath);
					lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
				});
				if (ex != null)
				{
					lastWriteTimeUtc = base.Context.TargetCopyStatus.CopyStatus.LastStatusTransitionTime;
					base.TraceError("RunPrereqsForInPlaceReseed(): Failed to read LastWriteTimeUtc of edb file '{0}'. Using LastStatusTransitionTime of '{1}' instead. Exception: {2}", new object[]
					{
						edbFilePath,
						lastWriteTimeUtc,
						ex
					});
				}
				TimeSpan timeSpan2 = DateTime.UtcNow.Subtract(lastWriteTimeUtc);
				TimeSpan timeSpan3 = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedInPlaceReseedDelayInSecs);
				base.TraceDebug("RunPrereqsForInPlaceReseed(): Found EDB file at '{0}'. The copy was last FailedAndSuspended '{1}' duration ago. Configured reseed delay is: '{2}'.", new object[]
				{
					edbFilePath,
					timeSpan2,
					timeSpan3
				});
				if (timeSpan2 <= timeSpan3)
				{
					TimeSpan workflowSuppressionInterval = this.GetWorkflowSuppressionInterval(timeSpan3);
					state.WriteWorkflowNextExecutionDueTime(workflowSuppressionInterval);
					base.TraceError("RunPrereqsForInPlaceReseed(): EDB file exists and Reseed is being attempted too soon. Reseed will be blocked until the configured reseed delay period. Workflow will next run after {0}.", new object[]
					{
						workflowSuppressionInterval
					});
					ReplayCrimsonEvents.AutoReseedWorkflowInPlaceReseedTooSoon.Log<string, Guid, string, string, string, TimeSpan, TimeSpan, TimeSpan>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, edbFilePath, timeSpan2, timeSpan3, workflowSuppressionInterval);
					return ReplayStrings.AutoReseedInPlaceReseedTooSoon(timeSpan2.ToString(), timeSpan3.ToString());
				}
			}
			int num;
			if (!base.Context.ReseedLimiter.TryStartSeed(out num))
			{
				base.TraceError("RunPrereqsForInPlaceReseed(): Seed is being skipped for now because maximum number of concurrent seeds has been reached: {0}", new object[]
				{
					num
				});
				return ReplayStrings.AutoReseedTooManyConcurrentSeeds(num);
			}
			return LocalizedString.Empty;
		}

		private Exception ExecuteResume(AutoReseedWorkflowState state)
		{
			Exception ex = null;
			try
			{
				state.UpdateReseedRecoveryAction(ReseedState.Resume);
				this.LogBeginExecute(state);
				DatabaseCopyActionFlags arg = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation | DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState;
				FailedSuspendedCopyAutoReseedWorkflow.hookableResumeRpc.Value(AmServerName.LocalComputerName.Fqdn, base.Context.Database.Guid, (uint)arg);
			}
			catch (TaskServerException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerTransientException ex3)
			{
				ex = ex3;
			}
			this.LogExecuteCompleted(state, ex);
			return ex;
		}

		private Exception ExecuteAssignSpare(AutoReseedWorkflowState state)
		{
			Exception ex = null;
			try
			{
				bool flag = true;
				MountedFolderPath mountedFolderPath;
				if (!MountedFolderPath.IsNullOrEmpty(this.m_volumeForMissingMountPoint))
				{
					this.LogBeginExecute(state);
					flag = false;
					string name = base.Context.Database.Name;
					DatabaseSpareInfo dbInfo = new DatabaseSpareInfo(name, new MountedFolderPath(Path.Combine(base.Context.Dag.AutoDagDatabasesRootFolderPath.PathName, name)));
					ExchangeVolume exchangeVolume = base.Context.VolumeManager.FixupMountPointForDatabase(dbInfo, this.m_volumeForMissingMountPoint);
					this.UpdateVolumeInfoCopyState(base.Context.Database.Guid);
					ReplayCrimsonEvents.AutoReseedWorkflowDbMountPointMissing.Log<string, Guid, string, string, MountedFolderPath, MountedFolderPath>(name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, exchangeVolume.ExchangeVolumeMountPoint, exchangeVolume.VolumeName);
				}
				else if (this.IsVolumeRecentlyAssigned(this.m_targetDbSet, out mountedFolderPath))
				{
					flag = false;
					base.TraceDebug("Skipping assigning a new volume since a volume was recently assigned. ReseedRecoveryActionRetryCount: {0}", new object[]
					{
						state.ReseedRecoveryActionRetryCount
					});
					state.AssignedVolumeName = mountedFolderPath.Path;
				}
				if (flag)
				{
					this.LogBeginExecute(state);
					DatabaseSpareInfo[] dbInfos = (from status in this.m_targetDbSet
					select new DatabaseSpareInfo(status.CopyStatus.DBName, new MountedFolderPath(Path.Combine(base.Context.Dag.AutoDagDatabasesRootFolderPath.PathName, status.CopyStatus.DBName)))).ToArray<DatabaseSpareInfo>();
					ExchangeVolume exchangeVolume2 = base.Context.VolumeManager.AssignSpare(dbInfos);
					base.TraceDebug("Assigned spare volume: {0}", new object[]
					{
						exchangeVolume2.VolumeName.Path
					});
					foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in this.m_targetDbSet)
					{
						this.UpdateVolumeInfoCopyState(copyStatusClientCachedEntry.DbGuid);
					}
					ReplayCrimsonEvents.AutoReseedWorkflowDbFailedAssignSpareSucceeded.Log<string, Guid, string, string, MountedFolderPath, MountedFolderPath>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, exchangeVolume2.ExchangeVolumeMountPoint, exchangeVolume2.VolumeName);
					state.AssignedVolumeName = exchangeVolume2.VolumeName.Path;
				}
				else
				{
					base.TraceDebug("Re-using previously assigned volume: {0}", new object[]
					{
						state.AssignedVolumeName
					});
				}
				state.UpdateReseedRecoveryAction(ReseedState.InPlaceReseed);
			}
			catch (DatabaseVolumeInfoException ex2)
			{
				ex = ex2;
			}
			this.LogExecuteCompleted(state, ex);
			return ex;
		}

		private Exception ExecuteInPlaceReseed(AutoReseedWorkflowState state)
		{
			Exception ex = null;
			try
			{
				state.UpdateReseedRecoveryAction(ReseedState.InPlaceReseed);
				this.LogBeginExecute(state);
				FailedSuspendedCopyAutoReseedWorkflow.hookableSeedRpc.Value(this);
				ReplayCrimsonEvents.AutoReseedWorkflowDbFailedBeginReseedSucceeded.Log<string, Guid, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason);
			}
			catch (SeederServerException ex2)
			{
				ex = ex2;
			}
			catch (SeederServerTransientException ex3)
			{
				ex = ex3;
			}
			this.LogExecuteCompleted(state, ex);
			return ex;
		}

		private static void BeginSeedRpc(FailedSuspendedCopyAutoReseedWorkflow wf)
		{
			using (SeederClient seederClient = SeederClient.Create(wf.Context.TargetServerName.Fqdn, wf.Context.Database.Name, null, wf.Context.TargetServer.AdminDisplayVersion))
			{
				try
				{
					seederClient.CancelDbSeed(wf.Context.Database.Guid);
					seederClient.EndDbSeed(wf.Context.Database.Guid);
					wf.TraceDebug("Prior seed instance has been Cancelled.", new object[0]);
				}
				catch (SeederInstanceNotFoundException ex)
				{
					wf.TraceDebug("Prior seed instance cleanup failed with: {0}", new object[]
					{
						ex
					});
				}
				seederClient.PrepareDbSeedAndBegin(wf.Context.Database.Guid, false, true, false, false, true, true, string.Empty, null, string.Empty, null, null, SeederRpcFlags.SkipSettingReseedAutoReseedState);
			}
		}

		private static bool DoesCopyHaveMountPointConfigured(RpcDatabaseCopyStatus2 status)
		{
			return status.DatabasePathIsOnMountedFolder && !string.IsNullOrEmpty(status.DatabaseVolumeName);
		}

		private void TraceBeginPrereqs(AutoReseedWorkflowState state)
		{
			if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				base.TraceDebug("Starting prereqs for '{0}' stage. Attempt number: {1}", new object[]
				{
					state.LastReseedRecoveryAction,
					state.ReseedRecoveryActionRetryCount + 1
				});
			}
		}

		private void LogBeginExecute(AutoReseedWorkflowState state)
		{
			if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				base.TraceDebug("Starting Execute for '{0}' stage. Attempt number: {1}", new object[]
				{
					state.LastReseedRecoveryAction,
					state.ReseedRecoveryActionRetryCount
				});
			}
			ReplayCrimsonEvents.AutoReseedWorkflowDbFailedExecuteStage.Log<string, Guid, string, string, ReseedState, int>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, state.LastReseedRecoveryAction, state.ReseedRecoveryActionRetryCount);
		}

		private void LogExecuteCompleted(AutoReseedWorkflowState state, Exception exception)
		{
			if (exception == null)
			{
				if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					base.TraceDebug("Execution of stage '{0}' succeeded. Attempt number: {1}", new object[]
					{
						state.LastReseedRecoveryAction,
						state.ReseedRecoveryActionRetryCount
					});
				}
				return;
			}
			if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.ErrorTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				base.TraceError("Execution of stage '{0}' failed with exception: {1}", new object[]
				{
					state.LastReseedRecoveryAction,
					exception
				});
			}
			ReplayCrimsonEvents.AutoReseedWorkflowDbFailedExecutionStageFailed.Log<string, Guid, string, string, ReseedState, int, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.WorkflowLaunchReason, state.LastReseedRecoveryAction, state.ReseedRecoveryActionRetryCount, exception.Message);
		}

		private static readonly TimeSpan OneDay = TimeSpan.FromDays(1.0);

		private static Hookable<Action<string, Guid, uint>> hookableResumeRpc = Hookable<Action<string, Guid, uint>>.Create(true, delegate(string serverFqdn, Guid dbGuid, uint flags)
		{
			Dependencies.ReplayRpcClientWrapper.RequestResume2(serverFqdn, dbGuid, flags);
		});

		private static Hookable<Action<FailedSuspendedCopyAutoReseedWorkflow>> hookableSeedRpc = Hookable<Action<FailedSuspendedCopyAutoReseedWorkflow>>.Create(true, new Action<FailedSuspendedCopyAutoReseedWorkflow>(FailedSuspendedCopyAutoReseedWorkflow.BeginSeedRpc));

		private static Hookable<Func<AutoReseedContext, LocalizedString>> hookableCheckExVolumes = Hookable<Func<AutoReseedContext, LocalizedString>>.Create(true, new Func<AutoReseedContext, LocalizedString>(FailedSuspendedCopyAutoReseedWorkflow.CheckExchangeVolumesPresent));

		private IEnumerable<CopyStatusClientCachedEntry> m_targetDbSet;

		private MountedFolderPath m_volumeForMissingMountPoint;
	}
}
