using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Health;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class NativeSyncStorageProviderState : SyncStorageProviderStateBase
	{
		internal NativeSyncStorageProviderState(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, INativeStateStorage stateStorage, SyncLogSession syncLogSession, bool underRecovery) : base(subscription, syncLogSession, underRecovery)
		{
			this.stateStorage = stateStorage;
			this.syncMailboxSession = syncMailboxSession;
			this.failedFolderCreations = new Dictionary<string, Exception>(1);
		}

		internal Queue<SyncChangeEntry> ItemsToProcess
		{
			get
			{
				base.CheckDisposed();
				return this.itemsToProcess;
			}
			set
			{
				base.CheckDisposed();
				this.itemsToProcess = value;
			}
		}

		internal SyncMailboxSession SyncMailboxSession
		{
			get
			{
				base.CheckDisposed();
				return this.syncMailboxSession;
			}
		}

		internal bool TryCancel
		{
			get
			{
				base.CheckDisposed();
				return this.tryCancel;
			}
		}

		internal INativeStateStorage StateStorage
		{
			get
			{
				base.CheckDisposed();
				return this.stateStorage;
			}
		}

		internal long EnumeratedItemsSize
		{
			get
			{
				base.CheckDisposed();
				return this.enumeratedItemsSize;
			}
			set
			{
				base.CheckDisposed();
				this.enumeratedItemsSize = value;
			}
		}

		internal InboundConversionOptions ScopedInboundConversionOptions
		{
			get
			{
				base.CheckDisposed();
				if (this.scopedInboundConversionOptions == null)
				{
					this.scopedInboundConversionOptions = XSOSyncContentConversion.GetScopedInboundConversionOptions(this.ScopedADRecipientSession);
				}
				return this.scopedInboundConversionOptions;
			}
		}

		internal OutboundConversionOptions ScopedOutboundConversionOptions
		{
			get
			{
				base.CheckDisposed();
				if (this.scopedOutboundConversionOptions == null)
				{
					this.scopedOutboundConversionOptions = XSOSyncContentConversion.GetScopedOutboundConversionOptions(this.ScopedADRecipientSession);
				}
				return this.scopedOutboundConversionOptions;
			}
		}

		private IRecipientSession ScopedADRecipientSession
		{
			get
			{
				base.CheckDisposed();
				if (this.scopedIRecipientSession == null)
				{
					ADSessionSettings adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.syncMailboxSession.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId);
					ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMailboxGuid(adSettings, this.syncMailboxSession.MailboxSession.MailboxGuid, null);
					this.scopedIRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, this.syncMailboxSession.MailboxSession.PreferedCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 256, "ScopedADRecipientSession", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Worker\\Framework\\SyncEngine\\NativeSyncStorageProviderState.cs");
				}
				return this.scopedIRecipientSession;
			}
		}

		internal TimeSpan AverageBackoffTime
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.AverageBackoffTime;
			}
		}

		internal ThrottlingStatistics ThrottlingStatistics
		{
			get
			{
				base.CheckDisposed();
				return this.connectionStatistics.ThrottlingStatistics;
			}
		}

		internal abstract StoreObjectId EnsureInboxFolder(SyncChangeEntry change);

		internal StoreObjectId EnsureDefaultFolder(DefaultFolderType defaultFolderType)
		{
			base.CheckDisposed();
			StoreObjectId folderId = null;
			SyncStoreLoadManager.ThrottleAndExecuteStoreCall(this.syncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete), "GetDefaultFolderId", delegate
			{
				folderId = this.syncMailboxSession.MailboxSession.GetDefaultFolderId(defaultFolderType);
			});
			if (folderId == null)
			{
				SyncStoreLoadManager.ThrottleAndExecuteStoreCall(this.syncMailboxSession.MailboxSession, new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete), "CreateDefaultFolder", delegate
				{
					folderId = this.syncMailboxSession.MailboxSession.CreateDefaultFolder(defaultFolderType);
				});
			}
			return folderId;
		}

		internal virtual bool IsInboxFolderId(StoreObjectId itemId)
		{
			base.CheckDisposed();
			if (this.inboxFolderId == null)
			{
				this.inboxFolderId = this.EnsureDefaultFolder(DefaultFolderType.Inbox);
			}
			return object.Equals(this.inboxFolderId, itemId);
		}

		internal void SetTryCancel()
		{
			this.tryCancel = true;
		}

		internal void AddFailedFolderCreation(string cloudId, Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("exception", exception);
			if (this.failedFolderCreations.ContainsKey(cloudId))
			{
				return;
			}
			this.failedFolderCreations.Add(cloudId, exception);
		}

		internal bool TryGetFailedFolderCreation(string cloudId, out Exception exception)
		{
			exception = null;
			return !string.IsNullOrEmpty(cloudId) && this.failedFolderCreations.TryGetValue(cloudId, out exception);
		}

		internal bool TryRemoveFailedFolderCreation(string cloudId)
		{
			return this.failedFolderCreations.Remove(cloudId);
		}

		internal DefaultFolderType GetDefaultFolderTypeForSubscription()
		{
			DefaultFolderType result;
			switch (base.Subscription.FolderSupport)
			{
			case FolderSupport.InboxOnly:
				result = DefaultFolderType.Inbox;
				break;
			case FolderSupport.ContactsOnly:
				result = DefaultFolderType.Contacts;
				break;
			default:
				return DefaultFolderType.None;
			}
			return result;
		}

		internal StoreObjectId GetDefaultFolderId(SyncChangeEntry itemChange)
		{
			if (base.Subscription.AggregationType == AggregationType.PeopleConnection)
			{
				if (this.peopleConnectFolderRetriever == null)
				{
					base.SyncLogSession.LogVerbose((TSLID)1369UL, "Initializing PeopleConnectFolderRetriever.", new object[0]);
					this.peopleConnectFolderRetriever = new OscFolderCreator(this.SyncMailboxSession.MailboxSession);
				}
				IConnectSubscription connectSubscription = (IConnectSubscription)base.Subscription;
				base.SyncLogSession.LogDebugging((TSLID)1491UL, "Retrieving folder Id for network: {0}; user id: {1}.", new object[]
				{
					connectSubscription.Name,
					connectSubscription.UserId
				});
				return this.peopleConnectFolderRetriever.Create(connectSubscription.Name, connectSubscription.UserId).FolderId;
			}
			DefaultFolderType defaultFolderType = DefaultFolderType.None;
			if (itemChange.SchemaType == SchemaType.Folder && itemChange.SyncObject != null)
			{
				defaultFolderType = ((SyncFolder)itemChange.SyncObject).DefaultFolderType;
			}
			if (defaultFolderType == DefaultFolderType.None)
			{
				defaultFolderType = this.GetDefaultFolderTypeForSubscription();
			}
			DefaultFolderType defaultFolderType2 = defaultFolderType;
			StoreObjectId storeObjectId;
			if (defaultFolderType2 != DefaultFolderType.None && defaultFolderType2 != DefaultFolderType.Inbox)
			{
				if (defaultFolderType2 == DefaultFolderType.Root)
				{
					storeObjectId = null;
				}
				else
				{
					storeObjectId = this.EnsureDefaultFolder(defaultFolderType);
				}
			}
			else
			{
				storeObjectId = this.EnsureInboxFolder(itemChange);
			}
			base.SyncLogSession.LogVerbose((TSLID)1073UL, "Item {0} mapped to default folder {1} with id {2}.", new object[]
			{
				itemChange,
				defaultFolderType,
				storeObjectId
			});
			return storeObjectId;
		}

		private const int EstimatedFolderFailedCreations = 1;

		private readonly SyncMailboxSession syncMailboxSession;

		private readonly INativeStateStorage stateStorage;

		private readonly Dictionary<string, Exception> failedFolderCreations;

		private Queue<SyncChangeEntry> itemsToProcess;

		private bool tryCancel;

		private long enumeratedItemsSize;

		private IRecipientSession scopedIRecipientSession;

		private InboundConversionOptions scopedInboundConversionOptions;

		private OutboundConversionOptions scopedOutboundConversionOptions;

		private StoreObjectId inboxFolderId;

		private OscFolderCreator peopleConnectFolderRetriever;
	}
}
