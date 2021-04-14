using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMailboxMigrationJob : MoveBaseJob
	{
		public SourceMailboxWrapper SourceDatabasesWrapper { get; private set; }

		protected override bool ReachedThePointOfNoReturn
		{
			get
			{
				return base.SyncStage >= SyncStage.CreatingInitialSyncCheckpoint;
			}
		}

		public override bool IsOnlineMove
		{
			get
			{
				return base.IsOnlineMove;
			}
			protected set
			{
				if (!value)
				{
					throw new OfflinePublicFolderMigrationNotSupportedException();
				}
				base.IsOnlineMove = value;
			}
		}

		protected override bool CanBeCanceledOrSuspended()
		{
			Organization orgContainer = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.publicFolderConfiguration.OrganizationId), 85, "CanBeCanceledOrSuspended", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\MRSJobs\\PublicFolderMailboxMigrationJob.cs").GetOrgContainer();
			return orgContainer.DefaultPublicFolderMailbox.Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid && !this.ReachedThePointOfNoReturn;
		}

		public override void Initialize(TransactionalRequestJob migrationRequestJob)
		{
			MrsTracer.Service.Function("PublicFolderMailboxMigrationJob.Initialize: SourceDatabase=\"{0}\", Flags={1}", new object[]
			{
				migrationRequestJob.SourceDatabase,
				migrationRequestJob.Flags
			});
			TenantPublicFolderConfigurationCache.Instance.RemoveValue(migrationRequestJob.OrganizationId);
			this.publicFolderConfiguration = TenantPublicFolderConfigurationCache.Instance.GetValue(migrationRequestJob.OrganizationId);
			base.Initialize(migrationRequestJob);
			LocalizedString sourceTracingID = LocalizedString.Empty;
			MailboxCopierFlags mailboxCopierFlags = MailboxCopierFlags.PublicFolderMigration;
			bool flag = migrationRequestJob.RequestStyle == RequestStyle.CrossOrg;
			if (flag)
			{
				sourceTracingID = MrsStrings.RPCHTTPPublicFoldersId(migrationRequestJob.RemoteMailboxServerLegacyDN);
				mailboxCopierFlags |= MailboxCopierFlags.CrossOrg;
			}
			this.publicFolderMailboxMigrator = new PublicFolderMailboxMigrator(this.publicFolderConfiguration, migrationRequestJob.FolderToMailboxMap, migrationRequestJob.TargetExchangeGuid, mailboxCopierFlags | MailboxCopierFlags.Root, migrationRequestJob, this, sourceTracingID);
		}

		protected override void ConfigureProviders(bool continueAfterConfiguringProviders)
		{
			this.InternalConfigureProviders(continueAfterConfiguringProviders);
		}

		protected override void UnconfigureProviders()
		{
			if (this.SourceDatabasesWrapper != null)
			{
				this.SourceDatabasesWrapper.Dispose();
				this.SourceDatabasesWrapper = null;
			}
		}

		public override List<MailboxCopierBase> GetAllCopiers()
		{
			return new List<MailboxCopierBase>
			{
				this.publicFolderMailboxMigrator
			};
		}

		protected override void ScheduleContentVerification(List<FolderSizeRec> verificationResults)
		{
		}

		protected override void VerifyFolderContents(MailboxCopierBase mbxCtx, FolderRecWrapper folderRecWrapper, List<FolderSizeRec> verificationResults)
		{
			throw new NotImplementedException();
		}

		public override void ValidateAndPopulateRequestJob(List<ReportEntry> entries)
		{
			this.InternalConfigureProviders(false);
			MailboxServerInformation mailboxServerInformation;
			MailboxInformation mailboxInformation;
			PublicFolderMailboxMigrationJob.ConnectAndValidateSource(this.sourceDatabases, out mailboxServerInformation, out mailboxInformation);
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(DateTime.UtcNow));
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.SourceConnectionFailure, null);
			MailboxServerInformation mailboxServerInformation2;
			MailboxInformation mailboxInformation2;
			PublicFolderMailboxMigrationJob.ConnectAndValidateDestination(this.publicFolderMailboxMigrator.DestMailbox, MailboxConnectFlags.ValidateOnly, out mailboxServerInformation2, out mailboxInformation2);
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulTargetConnection, new DateTime?(DateTime.UtcNow));
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.TargetConnectionFailure, null);
			if (mailboxInformation != null && mailboxInformation.ServerVersion != 0)
			{
				base.CachedRequestJob.SourceVersion = mailboxInformation.ServerVersion;
				base.CachedRequestJob.SourceServer = ((mailboxServerInformation != null) ? mailboxServerInformation.MailboxServerName : null);
			}
			if (mailboxInformation2 != null && mailboxInformation2.ServerVersion != 0)
			{
				base.CachedRequestJob.TargetVersion = mailboxInformation2.ServerVersion;
				base.CachedRequestJob.TargetServer = ((mailboxServerInformation2 != null) ? mailboxServerInformation2.MailboxServerName : null);
			}
		}

		protected override bool ValidateTargetMailbox(MailboxInformation mailboxInfo, out LocalizedString moveFinishedReason)
		{
			Organization orgContainer = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.publicFolderConfiguration.OrganizationId), 249, "ValidateTargetMailbox", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\MRSJobs\\PublicFolderMailboxMigrationJob.cs").GetOrgContainer();
			moveFinishedReason = MrsStrings.ReportTargetPublicFolderDeploymentUnlocked;
			return orgContainer.DefaultPublicFolderMailbox.Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid;
		}

		private static void ConnectAndValidateSource(ISourceMailbox sourceDatabase, out MailboxServerInformation sourceMailboxServerInfo, out MailboxInformation sourceDatabaseInfo)
		{
			sourceDatabase.Connect(MailboxConnectFlags.None);
			ParallelPublicFolderMigrationVersionChecker.ThrowIfMinimumRequiredVersionNotInstalled(((MapiSourceMailbox)sourceDatabase).ServerVersion);
			sourceMailboxServerInfo = sourceDatabase.GetMailboxServerInformation();
			sourceDatabaseInfo = sourceDatabase.GetMailboxInformation();
			MrsTracer.Service.Debug("Switching source public folder database {0} to SyncSource mode.", new object[]
			{
				sourceDatabaseInfo
			});
			bool flag;
			sourceDatabase.SetInTransitStatus(InTransitStatus.SyncSource, out flag);
			if (!flag)
			{
				throw new OfflinePublicFolderMigrationNotSupportedException();
			}
		}

		private static void ConnectAndValidateDestination(IDestinationMailbox destinationMailboxes, MailboxConnectFlags connectFlags, out MailboxServerInformation destinationHierarchyMailboxServerInfo, out MailboxInformation destinationHierarchyMailboxInfo)
		{
			destinationMailboxes.Connect(connectFlags);
			destinationHierarchyMailboxServerInfo = destinationMailboxes.GetMailboxServerInformation();
			destinationHierarchyMailboxInfo = destinationMailboxes.GetMailboxInformation();
		}

		protected override void FinalSync()
		{
			base.StartDataGuaranteeWait();
			base.ScheduleWorkItem(new Action(base.WaitForMailboxChangesToReplicate), WorkloadType.Unknown);
		}

		protected override void MigrateSecurityDescriptors()
		{
		}

		protected override void UpdateMovedMailbox()
		{
		}

		protected override void UpdateRequestOnSave(TransactionalRequestJob rj, UpdateRequestOnSaveType updateType)
		{
		}

		protected override void OnMoveCompleted(MailboxCopierBase mbxCtx)
		{
		}

		protected override void PostMoveCleanupSourceMailbox(MailboxCopierBase mbxCtx)
		{
			mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SourceMailboxCleanup;
		}

		protected override void PostMoveMarkRehomeOnRelatedRequests(MailboxCopierBase mbxCtx)
		{
			mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SetRelatedRequestsRehome;
		}

		protected override void CleanupDestinationMailbox(MailboxCopierBase mbxCtx, bool moveIsSuccessful)
		{
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.SourceDatabasesWrapper != null)
				{
					this.SourceDatabasesWrapper.Dispose();
					this.SourceDatabasesWrapper = null;
				}
				if (this.publicFolderMailboxMigrator != null)
				{
					this.publicFolderMailboxMigrator.UnconfigureProviders();
					this.publicFolderMailboxMigrator = null;
				}
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMailboxMigrationJob>(this);
		}

		private void InternalConfigureProviders(bool continueAfterConfiguringProviders)
		{
			RequestStatisticsBase cachedRequestJob = base.CachedRequestJob;
			bool flag = cachedRequestJob.RequestStyle == RequestStyle.CrossOrg;
			LocalMailboxFlags localMailboxFlags = LocalMailboxFlags.LegacyPublicFolders | LocalMailboxFlags.ParallelPublicFolderMigration;
			if (flag)
			{
				localMailboxFlags |= LocalMailboxFlags.PureMAPI;
			}
			this.sourceDatabases = new MapiSourceMailbox(localMailboxFlags);
			if (flag)
			{
				this.sourceDatabases.ConfigRPCHTTP(cachedRequestJob.RemoteMailboxLegacyDN, null, cachedRequestJob.RemoteMailboxServerLegacyDN, cachedRequestJob.OutlookAnywhereHostName, cachedRequestJob.RemoteCredential, true, cachedRequestJob.AuthenticationMethod != null && cachedRequestJob.AuthenticationMethod.Value == AuthenticationMethod.Ntlm);
			}
			else
			{
				((IMailbox)this.sourceDatabases).Config(base.GetReservation(cachedRequestJob.SourceDatabase.ObjectGuid, ReservationFlags.Read), cachedRequestJob.SourceDatabase.ObjectGuid, cachedRequestJob.SourceDatabase.ObjectGuid, CommonUtils.GetPartitionHint(cachedRequestJob.OrganizationId), cachedRequestJob.SourceDatabase.ObjectGuid, MailboxType.SourceMailbox, null);
			}
			LocalizedString tracingId = flag ? MrsStrings.RPCHTTPPublicFoldersId(cachedRequestJob.RemoteMailboxLegacyDN) : MrsStrings.PublicFoldersId(cachedRequestJob.OrganizationId.ToString());
			this.sourceDatabases.ConfigPublicFolders(cachedRequestJob.SourceDatabase);
			this.SourceDatabasesWrapper = new SourceMailboxWrapper(this.sourceDatabases, MailboxWrapperFlags.Source, tracingId);
			this.publicFolderMailboxMigrator.SetSourceDatabasesWrapper(this.SourceDatabasesWrapper);
			base.ConfigureProviders(continueAfterConfiguringProviders);
			if (flag)
			{
				this.publicFolderMailboxMigrator.ConfigTranslators(new PrincipalTranslator(this.SourceDatabasesWrapper.PrincipalMapper, this.publicFolderMailboxMigrator.DestMailboxWrapper.PrincipalMapper), null);
			}
		}

		private TenantPublicFolderConfiguration publicFolderConfiguration;

		private MapiSourceMailbox sourceDatabases;

		private PublicFolderMailboxMigrator publicFolderMailboxMigrator;
	}
}
