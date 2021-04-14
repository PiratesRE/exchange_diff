using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LiveIDAuthentication;
using Microsoft.Exchange.Net.Logging;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncStorageProviderState : SyncStorageProviderState, ISyncSourceSession
	{
		internal DeltaSyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery, IWebProxy proxyServer, int remoteConnectionTimeout, EventHandler<DownloadCompleteEventArgs> downloadCompletedEventHandler, EventHandler<EventArgs> messageDownloadedEventHandler, EventHandler<EventArgs> messageUploadedEventHandler, int maxDownloadItemsPerConnection, long maxDownloadSizePerConnection, long maxDownloadSizePerItem, ProtocolLog httpProtocolLog, TimeSpan deltaSyncSettingsUpdateInterval, int maxDownloadItemsInFirstDeltaSyncConnection, DeltaSyncClientFactory deltaSyncClientFactory) : base(subscription, syncLogSession, underRecovery, downloadCompletedEventHandler)
		{
			if (this.DeltaSyncSubscription.LogonPasswordSecured != null && this.DeltaSyncSubscription.LogonPasswordSecured.Length > 0)
			{
				this.deltaSyncUserAccount = DeltaSyncUserAccount.CreateDeltaSyncUserForPassportAuth(this.DeltaSyncSubscription.LogonName, SyncUtilities.SecureStringToString(this.DeltaSyncSubscription.LogonPasswordSecured));
				if (!string.IsNullOrEmpty(this.DeltaSyncSubscription.AuthPolicy))
				{
					this.deltaSyncUserAccount.AuthPolicy = this.DeltaSyncSubscription.AuthPolicy;
				}
				if (!string.IsNullOrEmpty(this.DeltaSyncSubscription.AuthToken) && !string.IsNullOrEmpty(this.DeltaSyncSubscription.Puid))
				{
					this.deltaSyncUserAccount.AuthToken = new AuthenticationToken(this.DeltaSyncSubscription.AuthToken, this.DeltaSyncSubscription.AuthTokenExpirationTime, null, this.DeltaSyncSubscription.Puid);
				}
			}
			else
			{
				string puid = this.DeltaSyncSubscription.Puid ?? string.Empty;
				this.deltaSyncUserAccount = DeltaSyncUserAccount.CreateDeltaSyncUserForTrustedPartnerAuthWithPuid(this.DeltaSyncSubscription.LogonName, puid);
			}
			this.deltaSyncUserAccount.DeltaSyncServer = this.DeltaSyncSubscription.IncommingServerUrl;
			this.deltaSyncClient = deltaSyncClientFactory.Create(this.deltaSyncUserAccount, remoteConnectionTimeout, proxyServer, maxDownloadSizePerItem, httpProtocolLog, syncLogSession, new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete));
			this.deltaSyncClient.SubscribeDownloadCompletedEvent(new EventHandler<DownloadCompleteEventArgs>(base.InternalOnDownloadCompletion));
			this.MessageDownloaded += messageDownloadedEventHandler;
			this.MessageUploaded += messageUploadedEventHandler;
			this.maxEmailChangesEnumerated = ((maxDownloadItemsPerConnection > 0) ? maxDownloadItemsPerConnection : int.MaxValue);
			this.maxDownloadSizePerConnection = ((maxDownloadSizePerConnection > 0L) ? maxDownloadSizePerConnection : long.MaxValue);
			this.maxDownloadSizePerItem = ((maxDownloadSizePerItem > 0L) ? maxDownloadSizePerItem : long.MaxValue);
			this.maxDownloadItemsInFirstDeltaSyncConnection = maxDownloadItemsInFirstDeltaSyncConnection;
			this.deltaSyncSettingsUpdateInterval = deltaSyncSettingsUpdateInterval;
		}

		private event EventHandler<EventArgs> MessageDownloaded;

		private event EventHandler<EventArgs> MessageUploaded;

		string ISyncSourceSession.Protocol
		{
			get
			{
				base.CheckDisposed();
				return "DeltaSync";
			}
		}

		string ISyncSourceSession.SessionId
		{
			get
			{
				base.CheckDisposed();
				return string.Empty;
			}
		}

		string ISyncSourceSession.Server
		{
			get
			{
				base.CheckDisposed();
				return string.Empty;
			}
		}

		internal static ConflictResolution ConflictResolution
		{
			get
			{
				return DeltaSyncStorageProviderState.conflictResolution;
			}
		}

		internal IDeltaSyncClient DeltaSyncClient
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncClient;
			}
		}

		internal DeltaSyncResultData DeltaSyncResultData
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncResultData;
			}
			set
			{
				base.CheckDisposed();
				this.deltaSyncResultData = value;
			}
		}

		internal string LatestFolderSyncKey
		{
			get
			{
				base.CheckDisposed();
				return this.latestFolderSyncKey;
			}
			set
			{
				base.CheckDisposed();
				this.latestFolderSyncKey = value;
			}
		}

		internal string LatestEmailSyncKey
		{
			get
			{
				base.CheckDisposed();
				return this.latestEmailSyncKey;
			}
			set
			{
				base.CheckDisposed();
				this.latestEmailSyncKey = value;
			}
		}

		internal long MaxMessageSize
		{
			get
			{
				base.CheckDisposed();
				return this.DeltaSyncSubscription.MaxMessageSize;
			}
		}

		internal bool HasLatestSettings
		{
			get
			{
				base.CheckDisposed();
				return this.LastSettingsSyncTime != null && ExDateTime.UtcNow - this.LastSettingsSyncTime.Value <= this.deltaSyncSettingsUpdateInterval;
			}
		}

		internal List<DeltaSyncOperation> DeltaSyncOperations
		{
			get
			{
				base.CheckDisposed();
				return this.deltaSyncOperations;
			}
			set
			{
				base.CheckDisposed();
				this.deltaSyncOperations = value;
			}
		}

		internal Dictionary<string, SyncChangeEntry> EmailIdMapping
		{
			get
			{
				base.CheckDisposed();
				return this.emailIdMapping;
			}
			set
			{
				base.CheckDisposed();
				this.emailIdMapping = value;
			}
		}

		internal Dictionary<string, SyncChangeEntry> FolderAddList
		{
			get
			{
				base.CheckDisposed();
				return this.folderAddList;
			}
			set
			{
				base.CheckDisposed();
				this.folderAddList = value;
			}
		}

		internal Dictionary<StoreObjectId, string> NativeToTempFolderIdMapping
		{
			get
			{
				base.CheckDisposed();
				return this.nativeToTempFolderIdMapping;
			}
			set
			{
				base.CheckDisposed();
				this.nativeToTempFolderIdMapping = value;
			}
		}

		internal Dictionary<StoreObjectId, string> NativeToCloudFolderIdMapping
		{
			get
			{
				base.CheckDisposed();
				return this.nativeToCloudFolderIdMapping;
			}
			set
			{
				base.CheckDisposed();
				this.nativeToCloudFolderIdMapping = value;
			}
		}

		internal Dictionary<string, SyncChangeEntry> ChangeList
		{
			get
			{
				base.CheckDisposed();
				return this.changeList;
			}
			set
			{
				base.CheckDisposed();
				this.changeList = value;
			}
		}

		internal List<SyncChangeEntry> EmailAddList
		{
			get
			{
				base.CheckDisposed();
				return this.emailAddList;
			}
			set
			{
				base.CheckDisposed();
				this.emailAddList = value;
			}
		}

		internal int EmailIndex
		{
			get
			{
				base.CheckDisposed();
				return this.emailIndex;
			}
			set
			{
				base.CheckDisposed();
				this.emailIndex = value;
			}
		}

		internal bool ChangesLoaded
		{
			get
			{
				base.CheckDisposed();
				return this.changesLoaded;
			}
			set
			{
				base.CheckDisposed();
				this.changesLoaded = value;
			}
		}

		internal long MaxDownloadSizePerItem
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadSizePerItem;
			}
		}

		internal long MaxDownloadSizePerConnection
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadSizePerConnection;
			}
		}

		internal int MaxEmailChangesEnumeratedInThisSync
		{
			get
			{
				base.CheckDisposed();
				if (this.deltaSyncUserAccount.EmailSyncKey == DeltaSyncCommon.DefaultSyncKey)
				{
					base.SyncLogSession.LogVerbose((TSLID)3000UL, "First Sync Connection (EmailSyncKey = DefaultSyncKey) - using maxDownloadItemsInFirstDeltaSyncConnection {0} instead of maxEmailChangesEnumerated {1}.", new object[]
					{
						this.maxDownloadItemsInFirstDeltaSyncConnection,
						this.maxEmailChangesEnumerated
					});
					return this.maxDownloadItemsInFirstDeltaSyncConnection;
				}
				return this.maxEmailChangesEnumerated;
			}
		}

		internal int CloudItemsSynced
		{
			get
			{
				base.CheckDisposed();
				return this.cloudItemsSynced;
			}
			set
			{
				base.CheckDisposed();
				this.cloudItemsSynced = value;
			}
		}

		internal bool CloudMoreItemsAvailable
		{
			get
			{
				base.CheckDisposed();
				return this.cloudMoreItemsAvailable;
			}
			set
			{
				base.CheckDisposed();
				this.cloudMoreItemsAvailable = value;
			}
		}

		internal DeltaSyncAggregationSubscription DeltaSyncSubscription
		{
			get
			{
				base.CheckDisposed();
				return (DeltaSyncAggregationSubscription)base.Subscription;
			}
		}

		internal bool HasFolderAndEmailCollectionCached
		{
			get
			{
				base.CheckDisposed();
				return this.hasFolderAndEmailCollectionCached;
			}
		}

		internal Collection CachedFolderCollection
		{
			get
			{
				base.CheckDisposed();
				return this.cachedFolderCollection;
			}
		}

		internal Collection CachedEmailCollection
		{
			get
			{
				base.CheckDisposed();
				return this.cachedEmailCollection;
			}
		}

		internal DeltaSyncWatermark DeltaSyncWatermark
		{
			get
			{
				return (DeltaSyncWatermark)base.BaseWatermark;
			}
		}

		private ExDateTime? LastSettingsSyncTime
		{
			get
			{
				string s;
				ExDateTime value;
				if (base.StateStorage.TryGetPropertyValue("LastSettingsSyncTime", out s) && ExDateTime.TryParse(ExTimeZone.UtcTimeZone, s, out value))
				{
					return new ExDateTime?(value);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					base.StateStorage.TryRemoveProperty("LastSettingsSyncTime");
					return;
				}
				string value2 = value.Value.ToUtc().ToString();
				if (base.StateStorage.ContainsProperty("LastSettingsSyncTime"))
				{
					base.StateStorage.ChangePropertyValue("LastSettingsSyncTime", value2);
					return;
				}
				base.StateStorage.AddProperty("LastSettingsSyncTime", value2);
			}
		}

		public override string ToString()
		{
			return base.ToString() + ", " + this.deltaSyncUserAccount.ToString();
		}

		internal void ForceGetSettingsInNextSync()
		{
			base.CheckDisposed();
			this.LastSettingsSyncTime = null;
		}

		internal void RecordSettingsSyncTime()
		{
			base.CheckDisposed();
			this.LastSettingsSyncTime = new ExDateTime?(ExDateTime.UtcNow);
		}

		internal void UpdateWaterMark()
		{
			base.CheckDisposed();
			if (this.latestEmailSyncKey == null && this.latestFolderSyncKey == null)
			{
				throw new InvalidOperationException("At least one of the Sync Keys (EmailSyncKey,FolderSyncKey) should be not null");
			}
			string folderSyncKey = this.latestFolderSyncKey ?? DeltaSyncCommon.DefaultSyncKey;
			string emailSyncKey = this.latestEmailSyncKey ?? DeltaSyncCommon.DefaultSyncKey;
			this.DeltaSyncWatermark.Save(folderSyncKey, emailSyncKey);
		}

		internal void UpdateDeltaSyncClientWithWaterMark()
		{
			base.CheckDisposed();
			string text;
			string text2;
			this.DeltaSyncWatermark.Load(out text, out text2);
			this.deltaSyncUserAccount.FolderSyncKey = text;
			base.SyncLogSession.LogVerbose((TSLID)645UL, "DS Client Folder Sync Key Updated: [{0}]", new object[]
			{
				text
			});
			this.deltaSyncUserAccount.EmailSyncKey = text2;
			base.SyncLogSession.LogVerbose((TSLID)646UL, "DS Client Email Sync Key Updated: [{0}]", new object[]
			{
				text2
			});
		}

		internal void CacheDeltaSyncServerUrl()
		{
			base.CheckDisposed();
			this.DeltaSyncSubscription.IncommingServerUrl = this.deltaSyncUserAccount.DeltaSyncServer;
		}

		internal void CacheGetChangesResults(Collection folderCollection, Collection emailCollection)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("folderCollection", folderCollection);
			SyncUtilities.ThrowIfArgumentNull("emailCollection", emailCollection);
			this.cachedFolderCollection = folderCollection;
			this.cachedEmailCollection = emailCollection;
			this.hasFolderAndEmailCollectionCached = true;
		}

		internal void TriggerMessageDownloadedEvent(object sender, EventArgs eventArgs)
		{
			base.CheckDisposed();
			if (this.MessageDownloaded != null)
			{
				this.MessageDownloaded(sender, eventArgs);
			}
		}

		internal void TriggerMessageUploadedEvent(object sender, EventArgs eventArgs)
		{
			base.CheckDisposed();
			if (this.MessageUploaded != null)
			{
				this.MessageUploaded(sender, eventArgs);
			}
		}

		internal void ClearApplyChangesState()
		{
			base.CheckDisposed();
			if (this.changesLoaded)
			{
				if (this.changeList != null)
				{
					DeltaSyncStorageProviderState.DisposeSyncObjects(this.changeList.Values);
					this.changeList.Clear();
					this.changeList = null;
				}
				if (this.folderAddList != null)
				{
					DeltaSyncStorageProviderState.DisposeSyncObjects(this.folderAddList.Values);
					this.folderAddList.Clear();
					this.folderAddList = null;
				}
				if (this.emailAddList != null)
				{
					DeltaSyncStorageProviderState.DisposeSyncObjects(this.emailAddList);
					this.emailAddList.Clear();
					this.emailAddList = null;
				}
				if (this.deltaSyncOperations != null)
				{
					this.deltaSyncOperations.Clear();
					this.deltaSyncOperations = null;
				}
				if (this.nativeToCloudFolderIdMapping != null)
				{
					this.nativeToCloudFolderIdMapping.Clear();
					this.nativeToCloudFolderIdMapping = null;
				}
				if (this.nativeToTempFolderIdMapping != null)
				{
					this.nativeToTempFolderIdMapping.Clear();
					this.nativeToTempFolderIdMapping = null;
				}
				if (this.emailIdMapping != null)
				{
					DeltaSyncStorageProviderState.DisposeSyncObjects(this.emailIdMapping.Values);
					this.emailIdMapping.Clear();
					this.emailIdMapping = null;
				}
				this.emailIndex = -1;
				base.ItemRetriever = null;
				base.ItemRetrieverState = null;
				this.changesLoaded = false;
			}
		}

		internal void UpdateSubscriptionSettings(DeltaSyncSettings deltaSyncSettings)
		{
			base.CheckDisposed();
			this.DeltaSyncSubscription.MaxNumberOfEmailAdds = deltaSyncSettings.MaxNumberOfEmailAdds;
			this.DeltaSyncSubscription.MaxNumberOfFolderAdds = deltaSyncSettings.MaxNumberOfFolderAdds;
			this.DeltaSyncSubscription.MaxObjectInSync = deltaSyncSettings.MaxObjectsInSync;
			this.DeltaSyncSubscription.MinSettingPollInterval = deltaSyncSettings.MinSettingsPollInterval;
			this.DeltaSyncSubscription.MinSyncPollInterval = deltaSyncSettings.MinSyncPollInterval;
			this.DeltaSyncSubscription.SyncMultiplier = deltaSyncSettings.SyncMultiplier;
			this.DeltaSyncSubscription.MaxAttachments = deltaSyncSettings.MaxAttachments;
			this.DeltaSyncSubscription.MaxMessageSize = deltaSyncSettings.MaxMessageSize;
			this.DeltaSyncSubscription.MaxRecipients = deltaSyncSettings.MaxRecipients;
			switch (deltaSyncSettings.AccountStatus)
			{
			case AccountStatusType.OK:
				this.DeltaSyncSubscription.AccountStatus = DeltaSyncAccountStatus.Normal;
				return;
			case AccountStatusType.Blocked:
				this.DeltaSyncSubscription.AccountStatus = DeltaSyncAccountStatus.Blocked;
				return;
			case AccountStatusType.RequiresHIP:
				this.DeltaSyncSubscription.AccountStatus = DeltaSyncAccountStatus.HipRequired;
				return;
			default:
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "unknown AccountStatus: {0}", new object[]
				{
					deltaSyncSettings.AccountStatus
				}));
			}
		}

		internal void UpdateSyncChangeEntry(SyncChangeEntry syncEntry, Exception exception)
		{
			if (exception is TransientException)
			{
				base.HasTransientSyncErrors = true;
				syncEntry.Exception = SyncTransientException.CreateItemLevelException(exception);
				return;
			}
			base.HasPermanentSyncErrors = true;
			syncEntry.Exception = SyncPermanentException.CreateItemLevelException(exception);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.deltaSyncClient != null)
			{
				this.deltaSyncClient.Dispose();
				this.deltaSyncClient = null;
				this.ClearApplyChangesState();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeltaSyncStorageProviderState>(this);
		}

		private static void DisposeSyncObjects(ICollection<SyncChangeEntry> collection)
		{
			if (collection != null)
			{
				foreach (SyncChangeEntry syncChangeEntry in collection)
				{
					if (syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
			}
		}

		private const string DeltaSyncComponentId = "DeltaSync";

		private const string LastSettingsSyncTimePropertyName = "LastSettingsSyncTime";

		private static ConflictResolution conflictResolution = ConflictResolution.ServerWins;

		private readonly int maxDownloadItemsInFirstDeltaSyncConnection;

		private IDeltaSyncClient deltaSyncClient;

		private DeltaSyncResultData deltaSyncResultData;

		private DeltaSyncUserAccount deltaSyncUserAccount;

		private string latestEmailSyncKey;

		private string latestFolderSyncKey;

		private Dictionary<string, SyncChangeEntry> changeList;

		private Dictionary<string, SyncChangeEntry> folderAddList;

		private Dictionary<StoreObjectId, string> nativeToCloudFolderIdMapping;

		private Dictionary<StoreObjectId, string> nativeToTempFolderIdMapping;

		private List<SyncChangeEntry> emailAddList;

		private List<DeltaSyncOperation> deltaSyncOperations;

		private Dictionary<string, SyncChangeEntry> emailIdMapping;

		private int emailIndex;

		private int maxEmailChangesEnumerated;

		private long maxDownloadSizePerConnection;

		private long maxDownloadSizePerItem;

		private int cloudItemsSynced;

		private bool cloudMoreItemsAvailable;

		private bool changesLoaded;

		private TimeSpan deltaSyncSettingsUpdateInterval;

		private bool hasFolderAndEmailCollectionCached;

		private Collection cachedFolderCollection;

		private Collection cachedEmailCollection;
	}
}
