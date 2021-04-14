using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.PublicFolder;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderSynchronizerContext : DisposeTrackableBase
	{
		public PublicFolderSynchronizerContext(OrganizationId organizationId, Guid contentMailboxGuid, bool isSingleFolderSync, bool executeReconcileFolders, Guid correlationId)
		{
			if (executeReconcileFolders && isSingleFolderSync)
			{
				throw new ArgumentException("Reconcile Folders can't be executed wit single folder syncs", "executeReconcileFolders");
			}
			this.OrganizationId = (organizationId ?? OrganizationId.ForestWideOrgId);
			this.ContentMailboxGuid = contentMailboxGuid;
			this.isSingleFolderSync = isSingleFolderSync;
			this.correlationId = correlationId;
			this.executeReconcileFolders = executeReconcileFolders;
			if (ExEnvironment.IsTest)
			{
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(organizationId);
			}
			this.isHierarchyReady = this.ReadIsHierarchyReadyFlag();
		}

		public OrganizationId OrganizationId { get; private set; }

		public Guid ContentMailboxGuid { get; private set; }

		public bool IsSingleFolderSync
		{
			get
			{
				return this.isSingleFolderSync;
			}
		}

		public bool ExecuteReconcileFolders
		{
			get
			{
				return this.executeReconcileFolders;
			}
		}

		public bool IsLoggerInitialized
		{
			get
			{
				return this.logger != null;
			}
		}

		public bool IsHierarchyReady
		{
			get
			{
				return this.isHierarchyReady;
			}
		}

		public ISourceMailbox SourceMailbox
		{
			get
			{
				if (this.sourceMailbox == null)
				{
					this.sourceMailbox = this.GetSourceMailbox();
				}
				return this.sourceMailbox;
			}
		}

		public IDestinationMailbox DestinationMailbox
		{
			get
			{
				if (this.destinationMailbox == null)
				{
					ExchangePrincipal exchangePrincipal;
					if (!PublicFolderSession.TryGetPublicFolderMailboxPrincipal(this.OrganizationId, this.ContentMailboxGuid, false, out exchangePrincipal))
					{
						throw new StoragePermanentException(PublicFolderSession.GetNoPublicFoldersProvisionedError(this.OrganizationId));
					}
					this.destinationMailbox = new StorageDestinationMailbox(LocalMailboxFlags.None);
					this.destinationMailbox.Config(null, exchangePrincipal.MailboxInfo.MailboxGuid, exchangePrincipal.MailboxInfo.MailboxGuid, TenantPartitionHint.FromOrganizationId(this.OrganizationId), exchangePrincipal.MailboxInfo.GetDatabaseGuid(), MailboxType.DestMailboxIntraOrg, null);
					this.destinationMailbox.Connect(MailboxConnectFlags.PublicFolderHierarchyReplication);
					if (!this.destinationMailbox.MailboxExists())
					{
						using (PublicFolderSession.OpenAsAdmin(this.OrganizationId, null, this.ContentMailboxGuid, null, CultureInfo.CurrentCulture, "Client=PublicFolderSystem;Action=PublicFolderHierarchyReplication", null))
						{
						}
						this.destinationMailbox.Connect(MailboxConnectFlags.PublicFolderHierarchyReplication);
					}
				}
				return this.destinationMailbox;
			}
		}

		public WellKnownPublicFolders SourceWellKnownFolders
		{
			get
			{
				if (this.sourceWellKnownFolders == null)
				{
					this.sourceWellKnownFolders = new WellKnownPublicFolders(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.SourceMailbox.GetProps(WellKnownPublicFolders.EntryIdPropTags)));
				}
				return this.sourceWellKnownFolders;
			}
		}

		public WellKnownPublicFolders DestinationWellKnownFolders
		{
			get
			{
				if (this.destinationWellKnownFolders == null)
				{
					this.destinationWellKnownFolders = new WellKnownPublicFolders(DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(this.DestinationMailbox.GetProps(WellKnownPublicFolders.EntryIdPropTags)));
				}
				return this.destinationWellKnownFolders;
			}
		}

		public Guid CorrelationId
		{
			get
			{
				return this.correlationId;
			}
		}

		public FolderOperationCounter FolderOperationCount
		{
			get
			{
				return this.folderOperationCount;
			}
		}

		public SyncStateCounter SyncStateCounter
		{
			get
			{
				return this.syncStateCounter;
			}
		}

		public PublicFolderSynchronizerLogger Logger
		{
			get
			{
				if (this.isSingleFolderSync)
				{
					return null;
				}
				if (this.logger == null)
				{
					this.logger = new PublicFolderSynchronizerLogger(this.DestinationMailboxSession, this.FolderOperationCount, this.CorrelationId);
				}
				return this.logger;
			}
		}

		public LatencyInfo MRSProxyLatencyInfo
		{
			get
			{
				return ((RemoteMailbox)this.SourceMailbox).MrsProxyClient.LatencyInfo;
			}
		}

		public PublicFolderSession DestinationMailboxSession
		{
			get
			{
				if (this.destinationMailboxSession == null)
				{
					ExchangePrincipal publicFolderMailboxPrincipal;
					if (!PublicFolderSession.TryGetPublicFolderMailboxPrincipal(this.OrganizationId, this.ContentMailboxGuid, false, out publicFolderMailboxPrincipal))
					{
						throw new StoragePermanentException(PublicFolderSession.GetNoPublicFoldersProvisionedError(this.OrganizationId));
					}
					this.destinationMailboxSession = PublicFolderSession.OpenAsAdmin(null, publicFolderMailboxPrincipal, null, CultureInfo.CurrentCulture, "Client=PublicFolderSystem;Action=PublicFolderHierarchyReplication", null);
				}
				return this.destinationMailboxSession;
			}
		}

		public byte[] MapSourceToDestinationFolderId(byte[] folderId, out bool isWellKnownFolder)
		{
			WellKnownPublicFolders.FolderType? folderType;
			isWellKnownFolder = ((folderId.Length > 22) ? this.SourceWellKnownFolders.GetFolderType(folderId, out folderType) : this.SourceWellKnownFolders.TryGetFolderTypeFromLongTermId(folderId, out folderType));
			if (isWellKnownFolder)
			{
				return this.DestinationWellKnownFolders.GetFolderId(folderType.Value);
			}
			return this.DestinationMailbox.GetSessionSpecificEntryId(folderId);
		}

		public void ResetDestinationMailboxConnection()
		{
			if (this.destinationMailbox != null)
			{
				this.destinationMailbox.Dispose();
				this.destinationMailbox = null;
			}
			if (this.logger != null)
			{
				this.logger.Dispose();
				this.logger = null;
			}
			if (this.destinationMailboxSession != null)
			{
				this.destinationMailboxSession.Dispose();
				this.destinationMailboxSession = null;
			}
		}

		public void ResetSourceMailboxConnection()
		{
			if (this.sourceMailbox != null)
			{
				this.sourceMailbox.Dispose();
				this.sourceMailbox = null;
			}
		}

		public IRecipientSession GetADSession()
		{
			ADSessionSettings sessionSettings = this.OrganizationId.ToADSessionSettings();
			return DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 421, "GetADSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ServiceHost\\Servicelets\\JobQueue\\Program\\PublicFolderSynchronizerContext.cs");
		}

		public ADRecipient GetContentMailboxADRecipient(IRecipientSession session = null)
		{
			if (session == null)
			{
				session = this.GetADSession();
			}
			return session.FindByExchangeGuid(this.ContentMailboxGuid);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.ResetSourceMailboxConnection();
				this.ResetDestinationMailboxConnection();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderSynchronizerContext>(this);
		}

		private bool ReadIsHierarchyReadyFlag()
		{
			ADRecipient contentMailboxADRecipient = this.GetContentMailboxADRecipient(null);
			return contentMailboxADRecipient != null && (bool)contentMailboxADRecipient[ADRecipientSchema.IsHierarchyReady];
		}

		private ISourceMailbox GetSourceMailbox()
		{
			ISourceMailbox result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ExchangePrincipal exchangePrincipal;
				if (!PublicFolderSession.TryGetPublicFolderMailboxPrincipal(this.OrganizationId, PublicFolderSession.HierarchyMailboxGuidAlias, true, out exchangePrincipal))
				{
					throw new StoragePermanentException(PublicFolderSession.GetNoPublicFoldersProvisionedError(this.OrganizationId));
				}
				ISourceMailbox sourceMailbox = new RemoteSourceMailbox(exchangePrincipal.MailboxInfo.Location.ServerFqdn, null, null, this.isSingleFolderSync ? ProxyControlFlags.SkipWLMThrottling : ProxyControlFlags.None, PublicFolderSynchronizerContext.RequiredCapabilities, false, LocalMailboxFlags.None);
				disposeGuard.Add<ISourceMailbox>(sourceMailbox);
				TenantPartitionHint partitionHint = CommonUtils.GetPartitionHint(this.OrganizationId);
				if (this.Logger != null)
				{
					this.Logger.LogEvent(LogEventType.Verbose, string.Format("Connecting to Primary Hierarchy. [Mailbox:{0}; Server:{1}; Database:{2}; PartitionHint:{3}]", new object[]
					{
						exchangePrincipal.MailboxInfo.MailboxGuid,
						exchangePrincipal.MailboxInfo.Location.ServerFqdn,
						exchangePrincipal.MailboxInfo.GetDatabaseGuid(),
						partitionHint
					}));
				}
				sourceMailbox.Config(null, exchangePrincipal.MailboxInfo.MailboxGuid, exchangePrincipal.MailboxInfo.MailboxGuid, partitionHint, exchangePrincipal.MailboxInfo.GetDatabaseGuid(), MailboxType.SourceMailbox, null);
				sourceMailbox.Connect(MailboxConnectFlags.PublicFolderHierarchyReplication);
				disposeGuard.Success();
				result = sourceMailbox;
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.PublicFolderSynchronizerTracer;

		private static readonly MRSProxyCapabilities[] RequiredCapabilities = new MRSProxyCapabilities[]
		{
			MRSProxyCapabilities.PublicFolderMigration
		};

		private static readonly BudgetKey BudgetKey = new UnthrottledBudgetKey("PublicFolderSync", BudgetType.ResourceTracking);

		private readonly bool isSingleFolderSync;

		private readonly bool executeReconcileFolders;

		private readonly FolderOperationCounter folderOperationCount = new FolderOperationCounter();

		private readonly SyncStateCounter syncStateCounter = new SyncStateCounter();

		private readonly Guid correlationId;

		private readonly bool isHierarchyReady;

		private ISourceMailbox sourceMailbox;

		private IDestinationMailbox destinationMailbox;

		private WellKnownPublicFolders sourceWellKnownFolders;

		private WellKnownPublicFolders destinationWellKnownFolders;

		private PublicFolderSynchronizerLogger logger;

		private PublicFolderSession destinationMailboxSession;
	}
}
