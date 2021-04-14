using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoReseedWorkflowLauncher
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AutoReseedTracer;
			}
		}

		public AutoReseedWorkflowLauncher()
		{
			this.m_suppression = new AutoReseedWorkflowSuppression();
		}

		public void BeginAutoReseedIfNecessary(AutoReseedContext context)
		{
			Guid guid = context.Database.Guid;
			string name = context.Database.Name;
			if (context.TargetCopyStatus == null)
			{
				AutoReseedWorkflowLauncher.Tracer.TraceError<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' has null TargetCopyStatus. Skipping.", name, context.TargetServerName);
				return;
			}
			if (context.TargetCopyStatus.Result != CopyStatusRpcResult.Success)
			{
				AutoReseedWorkflowLauncher.Tracer.TraceError((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Skipping since GetCopyStatus RPC to database copy '{0}\\{1}' failed. Result: {2}. Error: {3}.", new object[]
				{
					name,
					context.TargetServerName,
					context.TargetCopyStatus.Result,
					context.TargetCopyStatus.LastException
				});
				return;
			}
			this.RunNeverMountedActiveWorkflow(context);
			this.RunHealthyCopyWorkflowIfNecessary(context);
			ExtendedErrorInfo extendedErrorInfo = context.TargetCopyStatus.CopyStatus.ExtendedErrorInfo;
			if (context.TargetCopyStatus.CopyStatus.CopyStatus == CopyStatusEnum.Failed && (extendedErrorInfo == null || extendedErrorInfo.FailureException == null || !(extendedErrorInfo.FailureException is ReplayServiceRpcCopyStatusTimeoutException)))
			{
				if (context.TargetCopyStatus.IsActive)
				{
					AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is 'Failed' but active. Skipping.", name, context.TargetServerName);
					return;
				}
				if (this.m_suppression.ReportWorkflowLaunchConditionMet(AutoReseedWorkflowType.FailedCopy, guid, CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None, 1))
				{
					AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName, TimeSpan>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' has been 'Failed' for at least {2}. Launching the FailedCopyWorkflow workflow.", name, context.TargetServerName, AutoReseedWorkflowSuppression.s_dbFailedSuppresionInterval);
					FailedCopyWorkflow failedCopyWorkflow = new FailedCopyWorkflow(context, context.TargetCopyStatus.CopyStatus.ErrorMessage);
					failedCopyWorkflow.Execute();
					return;
				}
				AutoReseedWorkflowLauncher.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is 'Failed' but launching the recovery workflow is being skipped due to either initial suppression of {2}, or periodic suppression of {3}.", new object[]
				{
					name,
					context.TargetServerName,
					AutoReseedWorkflowSuppression.s_dbFailedSuppresionInterval,
					AutoReseedWorkflowSuppression.s_dbFailedSuppresionInterval
				});
				return;
			}
			else
			{
				if (context.TargetCopyStatus.CopyStatus.CopyStatus != CopyStatusEnum.FailedAndSuspended)
				{
					if (context.TargetCopyStatus.CopyStatus.ContentIndexStatus == ContentIndexStatusType.FailedAndSuspended)
					{
						if (context.CopyStatusesForTargetDatabase.All((CopyStatusClientCachedEntry status) => status.Result == CopyStatusRpcResult.Success && (status.CopyStatus.ContentIndexStatus == ContentIndexStatusType.Disabled || status.CopyStatus.ContentIndexStatus == ContentIndexStatusType.Suspended || status.CopyStatus.ContentIndexStatus == ContentIndexStatusType.FailedAndSuspended)))
						{
							AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, Guid, string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: AutoReseed workflow launcher detected all catalogs failed for database '{0}' [{1}]: {2}.", context.Database.Name, context.Database.Guid, context.TargetCopyStatus.CopyStatus.ErrorMessage);
							ReplayCrimsonEvents.AutoReseedWorkflowAllCatalogFailed.Log<string, Guid, string, string>(context.Database.Name, context.Database.Guid, "FailedSuspendedCatalogRebuildWorkflow", context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage);
							if (this.m_suppression.ReportWorkflowLaunchConditionMet(AutoReseedWorkflowType.FailedSuspendedCatalogRebuild, guid, CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None, 1))
							{
								AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, TimeSpan>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database '{0}' has no catalogs in healthy state for at least {1}. Launching the recovery workflow.", name, AutoReseedWorkflowSuppression.s_ciRebuildSuppresionInterval);
								FailedSuspendedCatalogRebuildWorkflow failedSuspendedCatalogRebuildWorkflow = new FailedSuspendedCatalogRebuildWorkflow(context, context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage);
								failedSuspendedCatalogRebuildWorkflow.Execute();
								return;
							}
							AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database '{0}' has no catalogs in healthy state but launching the recovery workflow is being skipped due to either initial suppression of {1}, or periodic suppression of {2}.", name, AutoReseedWorkflowSuppression.s_ciRebuildSuppresionInterval, AutoReseedWorkflowSuppression.s_ciRebuildRetryInterval);
							return;
						}
					}
					if (!this.TryLaunchCatalogAutoReseedWorkflow(context, name))
					{
						this.m_suppression.ReportNoWorkflowsNeedToLaunch(guid);
					}
					return;
				}
				if (context.TargetCopyStatus.IsActive)
				{
					AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is 'FailedAndSuspended' but active. Skipping.", name, context.TargetServerName);
					return;
				}
				if (this.m_suppression.ReportWorkflowLaunchConditionMet(AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed, guid, CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None, 1))
				{
					AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName, TimeSpan>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' has been FailedAndSuspended for at least {2}. Launching the recovery workflow.", name, context.TargetServerName, AutoReseedWorkflowSuppression.s_dbReseedSuppresionInterval);
					FailedSuspendedCopyAutoReseedWorkflow failedSuspendedCopyAutoReseedWorkflow = new FailedSuspendedCopyAutoReseedWorkflow(context, context.TargetCopyStatus.CopyStatus.ErrorMessage);
					failedSuspendedCopyAutoReseedWorkflow.Execute();
					return;
				}
				AutoReseedWorkflowLauncher.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is FailedAndSuspended but launching the recovery workflow is being skipped due to either initial suppression of {2}, or periodic suppression of {3}.", new object[]
				{
					name,
					context.TargetServerName,
					AutoReseedWorkflowSuppression.s_dbReseedSuppresionInterval,
					AutoReseedWorkflowSuppression.s_dbReseedRetryInterval
				});
				return;
			}
		}

		private bool TryLaunchCatalogAutoReseedWorkflow(AutoReseedContext context, string dbName)
		{
			string text = string.Empty;
			Trace tracer = AutoReseedWorkflowLauncher.Tracer;
			long id = (long)this.GetHashCode();
			string formatString = "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' ContentIndexStatus is {1}, ContentIndexBacklog is {2}, ContentIndexRetryQueueSize is {3}, ContentIndexVersion is {4}, LatestQueryVersion is {5}, any target DB with version==LastQueryVersion is {6}.";
			object[] array = new object[7];
			array[0] = dbName;
			array[1] = context.TargetCopyStatus.CopyStatus.ContentIndexStatus;
			array[2] = context.TargetCopyStatus.CopyStatus.ContentIndexBacklog;
			array[3] = context.TargetCopyStatus.CopyStatus.ContentIndexRetryQueueSize;
			array[4] = context.TargetCopyStatus.CopyStatus.ContentIndexVersion;
			object[] array2 = array;
			int num = 5;
			VersionInfo latest = VersionInfo.Latest;
			array2[num] = latest.QueryVersion;
			array[6] = context.CopyStatusesForTargetDatabase.Any(delegate(CopyStatusClientCachedEntry status)
			{
				if (status.Result == CopyStatusRpcResult.Success && status.CopyStatus.ContentIndexVersion != null)
				{
					int value3 = status.CopyStatus.ContentIndexVersion.Value;
					VersionInfo latest4 = VersionInfo.Latest;
					return value3 == latest4.QueryVersion;
				}
				return false;
			});
			tracer.TraceDebug(id, formatString, array);
			CatalogAutoReseedWorkflow.CatalogAutoReseedReason catalogAutoReseedReason;
			if (context.TargetCopyStatus.CopyStatus.ContentIndexStatus == ContentIndexStatusType.FailedAndSuspended)
			{
				if (context.TargetCopyStatus.CopyStatus.ContentIndexErrorCode != null)
				{
					catalogAutoReseedReason = this.MapErrorCodeToReseedReason(context.TargetCopyStatus.CopyStatus.ContentIndexErrorCode.Value);
				}
				else
				{
					catalogAutoReseedReason = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Unknown;
				}
				text = context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage;
				AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, CatalogAutoReseedWorkflow.CatalogAutoReseedReason, string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' has Failed and suspended catalog, will reseed with reason {1}, errorMessage is {2}.", dbName, catalogAutoReseedReason, text);
			}
			else if (context.TargetCopyStatus.CopyStatus.ContentIndexBacklog != null && context.TargetCopyStatus.CopyStatus.ContentIndexBacklog.Value > RegistryParameters.AutoReseedCiBehindBacklog)
			{
				catalogAutoReseedReason = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindBacklog;
				text = ReplayStrings.AutoReseedCatalogIsBehindBacklog(context.TargetCopyStatus.CopyStatus.ContentIndexBacklog.GetValueOrDefault());
				AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, int, string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' will reseed. Reason is BehindBacklog, AutoReseedCiBehindBacklog is {1}, errorMessage is {2}.", dbName, RegistryParameters.AutoReseedCiBehindBacklog, text);
			}
			else
			{
				if (context.TargetCopyStatus.CopyStatus.ContentIndexRetryQueueSize == null || context.TargetCopyStatus.CopyStatus.ContentIndexRetryQueueSize.Value <= RegistryParameters.AutoReseedCiBehindRetryCount)
				{
					if (context.TargetCopyStatus.CopyStatus.ContentIndexVersion != null)
					{
						int value = context.TargetCopyStatus.CopyStatus.ContentIndexVersion.Value;
						VersionInfo latest2 = VersionInfo.Latest;
						if (value < latest2.QueryVersion)
						{
							if (context.CopyStatusesForTargetDatabase.Any(delegate(CopyStatusClientCachedEntry status)
							{
								if (status.Result == CopyStatusRpcResult.Success && status.CopyStatus.ContentIndexVersion != null)
								{
									int value3 = status.CopyStatus.ContentIndexVersion.Value;
									VersionInfo latest4 = VersionInfo.Latest;
									return value3 == latest4.QueryVersion;
								}
								return false;
							}))
							{
								catalogAutoReseedReason = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Upgrade;
								int value2 = context.TargetCopyStatus.CopyStatus.ContentIndexVersion.Value;
								VersionInfo latest3 = VersionInfo.Latest;
								text = ReplayStrings.AutoReseedCatalogToUpgrade(value2, latest3.QueryVersion);
								AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' will reseed with reason Upgrade, errorMessage is {1}", dbName, text);
								goto IL_330;
							}
						}
					}
					AutoReseedWorkflowLauncher.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' has no reason to reseed.", dbName);
					return false;
				}
				catalogAutoReseedReason = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindRetry;
				text = ReplayStrings.AutoReseedCatalogIsBehindRetry(context.TargetCopyStatus.CopyStatus.ContentIndexRetryQueueSize.GetValueOrDefault());
				AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, int, string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.TryLaunchCatalogAutoReseedWorkflow: Database '{0}' will reseed. Reason is BehindBacklog, AutoReseedCiBehindBacklog is {1}, errorMessage is {2}.", dbName, RegistryParameters.AutoReseedCiBehindBacklog, text);
			}
			IL_330:
			AutoReseedWorkflowLauncher.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: AutoReseed workflow launcher detected a {0} catalog for database '{1}' [{2}]: {3}.", new object[]
			{
				context.TargetCopyStatus.CopyStatus.CopyStatus,
				context.Database.Name,
				context.Database.Guid,
				text
			});
			ReplayCrimsonEvents.AutoReseedWorkflowDetectedFailedCatalog.Log<string, Guid, string, string>(context.Database.Name, context.Database.Guid, "CatalogAutoReseedWorkflow", text);
			if (this.m_suppression.ReportWorkflowLaunchConditionMet(AutoReseedWorkflowType.CatalogAutoReseed, context.Database.Guid, catalogAutoReseedReason, context.TargetCopyStatus.CopyStatus.ActivationPreference))
			{
				AutoReseedWorkflowLauncher.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' has catalog in {2} state for at least {3}. Launching the recovery workflow. Reason is {4}, ActivationPreference is {5}.", new object[]
				{
					context.Database.Name,
					context.TargetServerName,
					context.TargetCopyStatus.CopyStatus.CopyStatus,
					AutoReseedWorkflowSuppression.s_ciReseedSuppresionInterval,
					catalogAutoReseedReason,
					context.TargetCopyStatus.CopyStatus.ActivationPreference
				});
				CatalogAutoReseedWorkflow catalogAutoReseedWorkflow = new CatalogAutoReseedWorkflow(context, catalogAutoReseedReason, text);
				catalogAutoReseedWorkflow.Execute();
			}
			else
			{
				AutoReseedWorkflowLauncher.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' has catalog in {2} state but launching the recovery workflow is being skipped due to either initial suppression of {3}, or periodic suppression of {4}. Reason is {5}, ActivationPreference is {6}", new object[]
				{
					context.Database.Name,
					context.TargetServerName,
					context.TargetCopyStatus.CopyStatus.CopyStatus,
					AutoReseedWorkflowSuppression.s_ciReseedSuppresionInterval,
					AutoReseedWorkflowSuppression.s_ciReseedRetryInterval,
					catalogAutoReseedReason,
					context.TargetCopyStatus.CopyStatus.ActivationPreference
				});
			}
			return true;
		}

		private void RunHealthyCopyWorkflowIfNecessary(AutoReseedContext context)
		{
			Guid guid = context.Database.Guid;
			string name = context.Database.Name;
			bool flag = false;
			if (context.TargetCopyStatus.CopyStatus.CopyStatus == CopyStatusEnum.Healthy)
			{
				try
				{
					AutoReseedWorkflowState autoReseedWorkflowState = new AutoReseedWorkflowState(guid, AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed);
					AutoReseedWorkflowState autoReseedWorkflowState2 = new AutoReseedWorkflowState(guid, AutoReseedWorkflowType.ManualReseed);
					AutoReseedWorkflowState autoReseedWorkflowState3 = new AutoReseedWorkflowState(guid, AutoReseedWorkflowType.ManualResume);
					if (autoReseedWorkflowState2.IsLastReseedRecoveryActionPending() || autoReseedWorkflowState3.IsLastReseedRecoveryActionPending() || autoReseedWorkflowState.IsLastReseedRecoveryActionPending())
					{
						flag = true;
						if (this.m_suppression.ReportHealthyWorkflowLaunchConditionMet(guid))
						{
							AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is Healthy with some prior recovery action having been completed. Launching the HealthyCopyCompletedSeedWorkflow workflow.", name, context.TargetServerName);
							HealthyCopyCompletedSeedWorkflow healthyCopyCompletedSeedWorkflow = new HealthyCopyCompletedSeedWorkflow(context);
							healthyCopyCompletedSeedWorkflow.Execute();
						}
						else
						{
							AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName, TimeSpan>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is Healthy with some prior recovery action having been completed but launching the workflow is being skipped due suppression of {2}.", name, context.TargetServerName, AutoReseedWorkflowSuppression.s_dbHealthySuppressionInterval);
						}
					}
				}
				catch (RegistryParameterException arg)
				{
					AutoReseedWorkflowLauncher.Tracer.TraceError<string, AmServerName, RegistryParameterException>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Couldn't launch/execute HealthyCopyCompletedSeedWorkflow for database copy '{0}\\{1}' to potentially reset the AutoReseed state. Error: {2}", name, context.TargetServerName, arg);
				}
			}
			if (!flag)
			{
				this.m_suppression.ReportHealthyWorkflowNotNeeded(guid);
			}
		}

		private void RunNeverMountedActiveWorkflow(AutoReseedContext context)
		{
			string name = context.Database.Name;
			if (context.TargetCopyStatus.CopyStatus.CopyStatus != CopyStatusEnum.Dismounted)
			{
				AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.RunNeverMountedActiveWorkflow: Database copy '{0}\\{1}' is NOT 'Dismounted'. Skip RunNeverMountedActiveWorkflow.", name, context.TargetServerName);
				return;
			}
			if (context.Database.DatabaseCreated)
			{
				AutoReseedWorkflowLauncher.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database copy '{0}\\{1}' is 'Dismounted' but database has previously been mounted. Skipping.", name, context.TargetServerName);
				return;
			}
			AutoReseedWorkflowLauncher.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutoReseedWorkflowLauncher.BeginAutoReseedIfNecessary: Database '{0}' has never been mounted before. Launching the recovery workflow.", name);
			NeverMountedActiveWorkflow neverMountedActiveWorkflow = new NeverMountedActiveWorkflow(context, ReplayStrings.AutoReseedNeverMountedWorkflowReason);
			neverMountedActiveWorkflow.Execute();
		}

		private CatalogAutoReseedWorkflow.CatalogAutoReseedReason MapErrorCodeToReseedReason(int errorCode)
		{
			CatalogAutoReseedWorkflow.CatalogAutoReseedReason result;
			try
			{
				switch (errorCode)
				{
				case 18:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingStarts;
					break;
				case 19:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingCompletes;
					break;
				case 20:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.EventsMissingWithNotificationsWatermark;
					break;
				case 21:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithNotificationsWatermark;
					break;
				case 22:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithTooManyNotificationEvents;
					break;
				case 23:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnPassive;
					break;
				default:
					result = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None;
					break;
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(string.Format("The parameter {0} is not of type IndexStatusErrorCode.", errorCode));
			}
			return result;
		}

		private AutoReseedWorkflowSuppression m_suppression;
	}
}
