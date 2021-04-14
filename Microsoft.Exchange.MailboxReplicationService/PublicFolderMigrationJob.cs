using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMigrationJob : MoveBaseJob
	{
		public SourceMailboxWrapper SourceDatabasesWrapper { get; private set; }

		protected override bool ReachedThePointOfNoReturn
		{
			get
			{
				return base.SyncStage >= SyncStage.Cleanup;
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

		public override void Initialize(TransactionalRequestJob migrationRequestJob)
		{
			MrsTracer.Service.Function("PublicFolderMigrationJob.Initialize: SourceDatabase=\"{0}\", Flags={1}", new object[]
			{
				migrationRequestJob.SourceDatabase,
				migrationRequestJob.Flags
			});
			TenantPublicFolderConfigurationCache.Instance.RemoveValue(migrationRequestJob.OrganizationId);
			this.publicFolderConfiguration = TenantPublicFolderConfigurationCache.Instance.GetValue(migrationRequestJob.OrganizationId);
			this.publicFolderMigrators = new Dictionary<Guid, PublicFolderMigrator>();
			base.Initialize(migrationRequestJob);
			if (this.publicFolderConfiguration.GetHierarchyMailboxInformation().Type != PublicFolderInformation.HierarchyType.InTransitMailboxGuid)
			{
				return;
			}
			LocalizedString sourceTracingID = LocalizedString.Empty;
			MailboxCopierFlags mailboxCopierFlags = MailboxCopierFlags.PublicFolderMigration;
			bool flag = migrationRequestJob.RequestStyle == RequestStyle.CrossOrg;
			if (flag)
			{
				sourceTracingID = MrsStrings.RPCHTTPPublicFoldersId(migrationRequestJob.RemoteMailboxServerLegacyDN);
				mailboxCopierFlags |= MailboxCopierFlags.CrossOrg;
			}
			Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
			PublicFolderMigrator value = new PublicFolderMigrator(this.publicFolderConfiguration, migrationRequestJob.FolderToMailboxMap, hierarchyMailboxGuid, mailboxCopierFlags | MailboxCopierFlags.Root, migrationRequestJob, this, sourceTracingID);
			this.publicFolderMigrators[hierarchyMailboxGuid] = value;
			foreach (FolderToMailboxMapping folderToMailboxMapping in migrationRequestJob.FolderToMailboxMap)
			{
				Guid mailboxGuid = folderToMailboxMapping.MailboxGuid;
				if (mailboxGuid != hierarchyMailboxGuid && !this.publicFolderMigrators.ContainsKey(mailboxGuid))
				{
					PublicFolderMigrator value2 = new PublicFolderMigrator(this.publicFolderConfiguration, migrationRequestJob.FolderToMailboxMap, mailboxGuid, mailboxCopierFlags, migrationRequestJob, this, sourceTracingID);
					this.publicFolderMigrators[mailboxGuid] = value2;
				}
			}
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
			List<MailboxCopierBase> list = new List<MailboxCopierBase>();
			if (this.publicFolderConfiguration.GetHierarchyMailboxInformation().Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid)
			{
				Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
				list.Add(this.publicFolderMigrators[hierarchyMailboxGuid]);
				foreach (PublicFolderMigrator publicFolderMigrator in this.publicFolderMigrators.Values)
				{
					if (publicFolderMigrator.TargetMailboxGuid != hierarchyMailboxGuid)
					{
						list.Add(publicFolderMigrator);
					}
				}
			}
			return list;
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
			PublicFolderMigrationJob.ConnectAndValidateSource(this.sourceDatabases, out mailboxServerInformation, out mailboxInformation);
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(DateTime.UtcNow));
			base.CachedRequestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.SourceConnectionFailure, null);
			MailboxServerInformation mailboxServerInformation2 = null;
			MailboxInformation mailboxInformation2 = null;
			foreach (PublicFolderMigrator publicFolderMigrator in this.publicFolderMigrators.Values)
			{
				MailboxServerInformation mailboxServerInformation3;
				MailboxInformation mailboxInformation3;
				PublicFolderMigrationJob.ConnectAndValidateDestination(publicFolderMigrator.DestMailbox, MailboxConnectFlags.ValidateOnly, out mailboxServerInformation3, out mailboxInformation3);
				bool flag = publicFolderMigrator.TargetMailboxGuid == this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
				if (flag)
				{
					MrsTracer.Service.Debug("PublicFolderMigrator for hierarchy mailbox created, with dumpster creation by {0}.", new object[]
					{
						publicFolderMigrator.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.CanStoreCreatePFDumpster) ? "Store" : "MRS"
					});
					mailboxServerInformation2 = mailboxServerInformation3;
					mailboxInformation2 = mailboxInformation3;
				}
			}
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
			Organization orgContainer = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.publicFolderConfiguration.OrganizationId), 314, "ValidateTargetMailbox", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\MRSJobs\\PublicFolderMigrationJob.cs").GetOrgContainer();
			moveFinishedReason = MrsStrings.ReportTargetPublicFolderDeploymentUnlocked;
			return orgContainer.DefaultPublicFolderMailbox.Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid;
		}

		private static void ConnectAndValidateSource(ISourceMailbox sourceDatabase, out MailboxServerInformation sourceMailboxServerInfo, out MailboxInformation sourceDatabaseInfo)
		{
			sourceDatabase.Connect(MailboxConnectFlags.None);
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
			CommonUtils.CatchKnownExceptions(delegate
			{
				Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
				if (mbxCtx.TargetMailboxGuid != hierarchyMailboxGuid)
				{
					MrsTracer.Service.Debug("Attempting to start hierarchy sync to content mailbox {0}.", new object[]
					{
						mbxCtx.TargetTracingID
					});
					PublicFolderSyncJobRpc.StartSyncHierarchy(this.publicFolderConfiguration.OrganizationId, mbxCtx.TargetMailboxGuid, mbxCtx.TargetServerInfo.MailboxServerName, true);
				}
			}, delegate(Exception e)
			{
				MrsTracer.Service.Error("Failed to start hierarchy sync to content mailbox {0} - {1}", new object[]
				{
					mbxCtx.TargetTracingID,
					e
				});
			});
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
			if (mbxCtx.IsRoot && moveIsSuccessful)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.publicFolderConfiguration.OrganizationId), 450, "CleanupDestinationMailbox", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\MRSJobs\\PublicFolderMigrationJob.cs");
				Organization orgContainer = tenantOrTopologyConfigurationSession.GetOrgContainer();
				orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
				orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid, PublicFolderInformation.HierarchyType.MailboxGuid);
				tenantOrTopologyConfigurationSession.Save(orgContainer);
			}
		}

		protected override MailboxChangesManifest EnumerateSourceHierarchyChanges(MailboxCopierBase mbxCtx, bool catchup, SyncContext syncContext)
		{
			if (catchup)
			{
				if (!mbxCtx.IsRoot)
				{
					return null;
				}
				return base.EnumerateSourceHierarchyChanges(mbxCtx, catchup, syncContext);
			}
			else
			{
				if (mbxCtx.IsRoot)
				{
					return base.EnumerateSourceHierarchyChanges(mbxCtx, catchup, syncContext);
				}
				FolderMap destinationFolderMap = base.GetRootMailboxContext().GetDestinationFolderMap(GetFolderMapFlags.None);
				EntryIdMap<FolderRecWrapper> primaryMailboxFolderRecWrappers = new EntryIdMap<FolderRecWrapper>();
				destinationFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper primaryFolderRecWrapper, FolderMap.EnumFolderContext enumFolderContext)
				{
					if (mbxCtx.IsContentAvailableInTargetMailbox(primaryFolderRecWrapper))
					{
						byte[] key = (byte[])primaryFolderRecWrapper.FolderRec[PropTag.LTID];
						primaryMailboxFolderRecWrappers[key] = primaryFolderRecWrapper;
					}
				});
				EntryIdMap<FolderRecWrapper> secondaryMailboxFolderRecWrappers = new EntryIdMap<FolderRecWrapper>();
				syncContext.TargetFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper secondaryFolderRecWrapper, FolderMap.EnumFolderContext enumFolderContext)
				{
					if (mbxCtx.IsContentAvailableInTargetMailbox(secondaryFolderRecWrapper))
					{
						byte[] key = (byte[])secondaryFolderRecWrapper.FolderRec[PropTag.LTID];
						secondaryMailboxFolderRecWrappers[key] = secondaryFolderRecWrapper;
					}
				});
				MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
				mailboxChangesManifest.ChangedFolders = new List<byte[]>();
				mailboxChangesManifest.DeletedFolders = new List<byte[]>();
				foreach (KeyValuePair<byte[], FolderRecWrapper> keyValuePair in primaryMailboxFolderRecWrappers)
				{
					FolderRecWrapper folderRecWrapper;
					if (!secondaryMailboxFolderRecWrappers.TryGetValue(keyValuePair.Key, out folderRecWrapper) || folderRecWrapper.FolderRec.LastModifyTimestamp != keyValuePair.Value.FolderRec.LastModifyTimestamp)
					{
						mailboxChangesManifest.ChangedFolders.Add(mbxCtx.SourceMailbox.GetSessionSpecificEntryId(keyValuePair.Key));
					}
				}
				foreach (KeyValuePair<byte[], FolderRecWrapper> keyValuePair2 in secondaryMailboxFolderRecWrappers)
				{
					FolderRecWrapper folderRecWrapper2;
					if (!primaryMailboxFolderRecWrappers.TryGetValue(keyValuePair2.Key, out folderRecWrapper2))
					{
						mailboxChangesManifest.DeletedFolders.Add(mbxCtx.SourceMailbox.GetSessionSpecificEntryId(keyValuePair2.Key));
					}
				}
				return mailboxChangesManifest;
			}
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
				if (this.publicFolderMigrators != null)
				{
					foreach (PublicFolderMigrator publicFolderMigrator in this.publicFolderMigrators.Values)
					{
						publicFolderMigrator.UnconfigureProviders();
					}
					this.publicFolderMigrators.Clear();
				}
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMigrationJob>(this);
		}

		private void InternalConfigureProviders(bool continueAfterConfiguringProviders)
		{
			if (this.publicFolderConfiguration.GetHierarchyMailboxInformation().Type != PublicFolderInformation.HierarchyType.InTransitMailboxGuid)
			{
				throw new PublicFolderMailboxesNotProvisionedForMigrationException();
			}
			RequestStatisticsBase cachedRequestJob = base.CachedRequestJob;
			bool flag = cachedRequestJob.RequestStyle == RequestStyle.CrossOrg;
			LocalMailboxFlags localMailboxFlags = LocalMailboxFlags.LegacyPublicFolders;
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
			foreach (MailboxCopierBase mailboxCopierBase in this.GetAllCopiers())
			{
				PublicFolderMigrator publicFolderMigrator = (PublicFolderMigrator)mailboxCopierBase;
				publicFolderMigrator.SetSourceDatabasesWrapper(this.SourceDatabasesWrapper);
			}
			base.ConfigureProviders(continueAfterConfiguringProviders);
			MailboxCopierBase rootMailboxContext = base.GetRootMailboxContext();
			foreach (MailboxCopierBase mailboxCopierBase2 in this.GetAllCopiers())
			{
				PublicFolderMigrator publicFolderMigrator2 = (PublicFolderMigrator)mailboxCopierBase2;
				if (!publicFolderMigrator2.IsRoot)
				{
					publicFolderMigrator2.SetHierarchyMailbox(rootMailboxContext.DestMailbox);
				}
				if (flag)
				{
					publicFolderMigrator2.ConfigTranslators(new PrincipalTranslator(this.SourceDatabasesWrapper.PrincipalMapper, publicFolderMigrator2.DestMailboxWrapper.PrincipalMapper), null);
				}
			}
		}

		private TenantPublicFolderConfiguration publicFolderConfiguration;

		private MapiSourceMailbox sourceDatabases;

		private Dictionary<Guid, PublicFolderMigrator> publicFolderMigrators;
	}
}
