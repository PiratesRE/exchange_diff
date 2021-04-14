using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CatalogAutoReseedWorkflow : AutoReseedWorkflow
	{
		public CatalogAutoReseedWorkflow(AutoReseedContext context, CatalogAutoReseedWorkflow.CatalogAutoReseedReason catalogReseedReason, string workflowLaunchReason) : base(AutoReseedWorkflowType.CatalogAutoReseed, workflowLaunchReason, context)
		{
			this.catalogReseedReason = catalogReseedReason;
		}

		public static IDisposable SetTestHook(Func<CatalogAutoReseedWorkflow, Exception> reseedAction)
		{
			return CatalogAutoReseedWorkflow.hookableReseedAction.SetTestHook(reseedAction);
		}

		protected override TimeSpan GetThrottlingInterval(AutoReseedWorkflowState state)
		{
			return TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiThrottlingIntervalInSecs);
		}

		internal string SourceName
		{
			get
			{
				return this.sourceName;
			}
		}

		internal CatalogAutoReseedWorkflow.CatalogAutoReseedReason ReseedReason
		{
			get
			{
				return this.catalogReseedReason;
			}
		}

		protected override bool IsDisabled
		{
			get
			{
				switch (this.catalogReseedReason)
				{
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingStarts:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingCompletes:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.EventsMissingWithNotificationsWatermark:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithNotificationsWatermark:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithTooManyNotificationEvents:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnPassive:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Unknown:
					return RegistryParameters.AutoReseedCiFailedSuspendedDisabled;
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindBacklog:
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindRetry:
					return RegistryParameters.AutoReseedCiBehindDisabled;
				case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Upgrade:
					return RegistryParameters.AutoReseedCiUpgradeDisabled;
				default:
					throw new ArgumentOutOfRangeException("catalogReseedReason");
				}
			}
		}

		protected override LocalizedString RunPrereqs(AutoReseedWorkflowState state)
		{
			LocalizedString result = base.RunPrereqs(state);
			if (!result.IsEmpty)
			{
				return result;
			}
			int num = base.Context.CopyStatusesForTargetDatabase.Count((CopyStatusClientCachedEntry status) => status.Result == CopyStatusRpcResult.Success && status.CopyStatus.ContentIndexStatus == ContentIndexStatusType.Healthy);
			if (num == 0)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, Guid, string>((long)this.GetHashCode(), "CatalogAutoReseedWorkflow detected all catalogs failed for database '{0}' [{1}]: {2}.", base.Context.Database.Name, base.Context.Database.Guid, base.Context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage);
				ReplayCrimsonEvents.AutoReseedWorkflowAllCatalogFailed.Log<string, Guid, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.Context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage);
				return ReplayStrings.AutoReseedAllCatalogFailed(base.Context.Database.Name);
			}
			if (num == 1)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "CatalogAutoReseedWorkflow detected only one catalog copy is healthy for database '{0}' [{1}].", base.Context.Database.Name, base.Context.Database.Guid);
				ReplayCrimsonEvents.AutoReseedWorkflowSingleCatalogHealthy.Log<string, Guid, string, string>(base.Context.Database.Name, base.Context.Database.Guid, base.WorkflowName, base.Context.TargetCopyStatus.CopyStatus.ContentIndexErrorMessage);
			}
			int num2;
			if (!base.Context.ReseedLimiter.TryStartCiSeed(out num2))
			{
				base.TraceError("CatalogAutoReseedWorkflow is being skipped for now because maximum number of concurrent seeds has been reached: {0}", new object[]
				{
					num2
				});
				return ReplayStrings.AutoReseedTooManyConcurrentSeeds(num2);
			}
			return LocalizedString.Empty;
		}

		protected override Exception ExecuteInternal(AutoReseedWorkflowState state)
		{
			int num = int.MaxValue;
			bool skipBehindCatalog = false;
			if (this.catalogReseedReason == CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindBacklog || this.catalogReseedReason == CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindRetry)
			{
				num = this.WeighCiCopyStatus(base.Context.TargetCopyStatus, false);
				skipBehindCatalog = true;
			}
			foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in base.Context.CopyStatusesForTargetDatabase)
			{
				if (!copyStatusClientCachedEntry.ServerContacted.Equals(base.Context.TargetServerName) && copyStatusClientCachedEntry.Result == CopyStatusRpcResult.Success && copyStatusClientCachedEntry.CopyStatus.ContentIndexStatus == ContentIndexStatusType.Healthy && (copyStatusClientCachedEntry.CopyStatus.CopyStatus == CopyStatusEnum.Mounted || copyStatusClientCachedEntry.CopyStatus.CopyStatus == CopyStatusEnum.Healthy || copyStatusClientCachedEntry.CopyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndHealthy))
				{
					if (this.catalogReseedReason == CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Upgrade)
					{
						if (copyStatusClientCachedEntry.CopyStatus.ContentIndexVersion == null)
						{
							continue;
						}
						int value = copyStatusClientCachedEntry.CopyStatus.ContentIndexVersion.Value;
						VersionInfo latest = VersionInfo.Latest;
						if (value != latest.QueryVersion)
						{
							continue;
						}
					}
					int num2 = this.WeighCiCopyStatus(copyStatusClientCachedEntry, skipBehindCatalog);
					if (num2 < num)
					{
						this.sourceName = copyStatusClientCachedEntry.ServerContacted.Fqdn;
						num = num2;
					}
				}
			}
			AutoReseedWorkflow.Tracer.TraceDebug<string, string, AmServerName>((long)this.GetHashCode(), "CatalogAutoReseedWorkflow: Selected '{0}' as source server for content index of database copy '{1}\\{2}'.", this.sourceName, base.Context.Database.Name, base.Context.TargetServerName);
			if (string.IsNullOrEmpty(this.sourceName))
			{
				return new AutoReseedCatalogSourceException(base.Context.Database.Name, base.Context.TargetServerName.NetbiosName);
			}
			if (base.Context.TargetCopyStatus.IsActive)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, AmServerName>((long)this.GetHashCode(), "CatalogAutoReseedWorkflow: Database copy '{0}\\{1}' is active. Fail over.", base.Context.Database.Name, base.Context.TargetServerName);
				new DatabaseFailureItem(FailureNameSpace.ContentIndex, FailureTag.CatalogReseed, base.Context.Database.Guid)
				{
					InstanceName = base.Context.Database.Name
				}.Publish();
				return new AutoReseedCatalogActiveException(base.Context.Database.Name, base.Context.TargetServerName.NetbiosName);
			}
			return CatalogAutoReseedWorkflow.hookableReseedAction.Value(this);
		}

		private static Exception ReseedCatalog(CatalogAutoReseedWorkflow workflow)
		{
			Exception result = null;
			using (SeederClient seederClient = SeederClient.Create(workflow.Context.TargetServerName.Fqdn, workflow.Context.Database.Name, null, workflow.Context.TargetServer.AdminDisplayVersion))
			{
				bool flag = false;
				for (int i = 0; i <= 1; i++)
				{
					result = null;
					try
					{
						if (flag)
						{
							seederClient.EndDbSeed(workflow.Context.Database.Guid);
						}
						AutoReseedWorkflow.Tracer.TraceDebug((long)workflow.GetHashCode(), "CatalogAutoReseedWorkflow: Attempt({0}) to reseed catalog for database copy '{1}\\{2}' from {3}.", new object[]
						{
							i,
							workflow.Context.Database.Name,
							workflow.Context.TargetServerName,
							string.IsNullOrEmpty(workflow.sourceName) ? "Active" : workflow.sourceName
						});
						SeederRpcFlags reseedRPCReason = CatalogAutoReseedWorkflow.GetReseedRPCReason(workflow.catalogReseedReason);
						seederClient.PrepareDbSeedAndBegin(workflow.Context.Database.Guid, false, false, false, false, false, true, string.Empty, null, workflow.sourceName, null, null, reseedRPCReason);
						break;
					}
					catch (SeederInstanceAlreadyInProgressException ex)
					{
						result = ex;
						break;
					}
					catch (SeederInstanceAlreadyFailedException ex2)
					{
						result = ex2;
						flag = true;
					}
					catch (SeederServerException ex3)
					{
						result = ex3;
					}
					catch (SeederServerTransientException ex4)
					{
						result = ex4;
					}
					if (!string.IsNullOrEmpty(workflow.sourceName))
					{
						workflow.sourceName = string.Empty;
					}
				}
			}
			return result;
		}

		private static SeederRpcFlags GetReseedRPCReason(CatalogAutoReseedWorkflow.CatalogAutoReseedReason reason)
		{
			switch (reason)
			{
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindBacklog:
				return SeederRpcFlags.CIAutoReseedReasonBehindBacklog;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.BehindRetry:
				return SeederRpcFlags.CIAutoReseedReasonBehindRetry;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Upgrade:
				return SeederRpcFlags.CIAutoReseedReasonUpgrade;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingStarts:
				return SeederRpcFlags.CatalogCorruptionWhenFeedingStarts;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CatalogCorruptionWhenFeedingCompletes:
				return SeederRpcFlags.CatalogCorruptionWhenFeedingCompletes;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.EventsMissingWithNotificationsWatermark:
				return SeederRpcFlags.EventsMissingWithNotificationsWatermark;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithNotificationsWatermark:
				return SeederRpcFlags.CrawlOnNonPreferredActiveWithNotificationsWatermark;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnNonPreferredActiveWithTooManyNotificationEvents:
				return SeederRpcFlags.CrawlOnNonPreferredActiveWithTooManyNotificationEvents;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.CrawlOnPassive:
				return SeederRpcFlags.CrawlOnPassive;
			case CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Unknown:
				return SeederRpcFlags.Unknown;
			default:
				return SeederRpcFlags.None;
			}
		}

		private int WeighCiCopyStatus(CopyStatusClientCachedEntry entry, bool skipBehindCatalog)
		{
			if (entry.CopyStatus.ContentIndexBacklog == null || entry.CopyStatus.ContentIndexRetryQueueSize == null)
			{
				return int.MaxValue;
			}
			if (skipBehindCatalog && (entry.CopyStatus.ContentIndexBacklog.Value > RegistryParameters.AutoReseedCiBehindBacklog || entry.CopyStatus.ContentIndexRetryQueueSize.Value > RegistryParameters.AutoReseedCiBehindRetryCount))
			{
				return int.MaxValue;
			}
			return entry.CopyStatus.ContentIndexBacklog.Value * 100 + entry.CopyStatus.ContentIndexRetryQueueSize.Value;
		}

		private readonly CatalogAutoReseedWorkflow.CatalogAutoReseedReason catalogReseedReason;

		private string sourceName;

		private static readonly Hookable<Func<CatalogAutoReseedWorkflow, Exception>> hookableReseedAction = Hookable<Func<CatalogAutoReseedWorkflow, Exception>>.Create(true, new Func<CatalogAutoReseedWorkflow, Exception>(CatalogAutoReseedWorkflow.ReseedCatalog));

		internal enum CatalogAutoReseedReason
		{
			None,
			BehindBacklog,
			BehindRetry,
			Upgrade,
			CatalogCorruptionWhenFeedingStarts,
			CatalogCorruptionWhenFeedingCompletes,
			EventsMissingWithNotificationsWatermark,
			CrawlOnNonPreferredActiveWithNotificationsWatermark,
			CrawlOnNonPreferredActiveWithTooManyNotificationEvents,
			CrawlOnPassive,
			Unknown
		}
	}
}
