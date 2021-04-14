using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPSyncStorageProviderState : SyncStorageProviderState, ISyncSourceSession
	{
		internal IMAPSyncStorageProviderState(ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery) : this(AggregationConfiguration.Instance.MaxDownloadSizePerItem, AggregationConfiguration.Instance.GetMaxDownloadItemsPerConnection(subscription.AggregationType), (long)AggregationConfiguration.Instance.GetMaxDownloadSizePerConnection(subscription.AggregationType).ToBytes(), subscription, syncLogSession, underRecovery, new EventHandler<DownloadCompleteEventArgs>(FrameworkPerfCounterHandler.Instance.OnImapSyncDownloadCompletion))
		{
			this.clientState = new IMAPClientState(this.imapSubscription.IMAPServer, this.imapSubscription.IMAPPort, this.imapSubscription.IMAPLogOnName, this.imapSubscription.LogonPasswordSecured, this.imapSubscription.ImapPathPrefix, base.SyncLogSession, this.sessionId, this.imapSubscription.SubscriptionGuid, this.imapSubscription.IMAPAuthentication, this.imapSubscription.IMAPSecurity, this.imapSubscription.AggregationType, this.maxDownloadBytesAllowed, AggregationConfiguration.Instance.RemoteConnectionTimeout, new EventHandler<DownloadCompleteEventArgs>(base.InternalOnDownloadCompletion), new EventHandler<EventArgs>(FrameworkPerfCounterHandler.Instance.OnImapSyncMessageDownloadCompletion), new EventHandler<EventArgs>(FrameworkPerfCounterHandler.Instance.OnImapSyncMessageUploadCompletion), new EventHandler<RoundtripCompleteEventArgs>(this.OnRoundtripComplete));
			base.SyncLogSession.LogDebugging((TSLID)810UL, "Built IMAPSyncStorageProviderState", new object[0]);
		}

		internal IMAPSyncStorageProviderState(ICommClient commClient, long maxSizePerItem, int maxItemsPerSession, long maxSizePerSession, ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery, EventHandler<DownloadCompleteEventArgs> downloadCompleted) : this(maxSizePerItem, maxItemsPerSession, maxSizePerSession, subscription, syncLogSession, underRecovery, downloadCompleted)
		{
			this.clientState = new IMAPClientState(commClient, this.imapSubscription.IMAPLogOnName, this.imapSubscription.LogonPasswordSecured, this.imapSubscription.ImapPathPrefix, base.SyncLogSession, this.imapSubscription.IMAPAuthentication, this.imapSubscription.IMAPSecurity);
		}

		private IMAPSyncStorageProviderState(long maxSizePerItem, int maxItemsPerSession, long maxSizePerSession, ISyncWorkerData subscription, SyncLogSession syncLogSession, bool underRecovery, EventHandler<DownloadCompleteEventArgs> downloadCompleted) : base(subscription, syncLogSession, underRecovery, downloadCompleted)
		{
			this.sessionId = SyncUtilities.GetNextSessionId();
			this.imapSubscription = (IMAPAggregationSubscription)subscription;
			this.maxDownloadSizePerItem = maxSizePerItem;
			this.maxDownloadBytesAllowed = maxSizePerSession;
			this.maxDownloadItemsPerConnection = maxItemsPerSession;
			this.cloudIdToFolderMap = new Dictionary<string, IMAPFolder>(5, StringComparer.Ordinal);
			this.cloudItemChangeMap = new Dictionary<string, string>(5);
		}

		internal string SyncWatermark
		{
			get
			{
				base.CheckDisposed();
				string result;
				this.ImapWatermark.Load(out result);
				return result;
			}
		}

		internal StringWatermark ImapWatermark
		{
			get
			{
				return (StringWatermark)base.BaseWatermark;
			}
		}

		internal IDictionary<string, string> CloudItemChangeMap
		{
			get
			{
				base.CheckDisposed();
				return this.cloudItemChangeMap;
			}
			set
			{
				base.CheckDisposed();
				this.cloudItemChangeMap = value;
			}
		}

		internal Dictionary<DefaultFolderType, string> PreprocessedDefaultMappings
		{
			get
			{
				base.CheckDisposed();
				return this.preprocessedDefaultMappings;
			}
			set
			{
				base.CheckDisposed();
				this.preprocessedDefaultMappings = value;
			}
		}

		internal SortedList<string, SyncChangeEntry> SortedFolderAddsSyncChangeEntries
		{
			get
			{
				base.CheckDisposed();
				return this.sortedFolderAddsSyncChangeEntries;
			}
			set
			{
				base.CheckDisposed();
				this.sortedFolderAddsSyncChangeEntries = value;
			}
		}

		internal Exception LastSelectFailedFolderException
		{
			get
			{
				base.CheckDisposed();
				return this.lastSelectFailedFolderException;
			}
			set
			{
				base.CheckDisposed();
				this.lastSelectFailedFolderException = value;
			}
		}

		internal long EstimatedMessageBytesToDownload
		{
			get
			{
				base.CheckDisposed();
				return this.estimatedMessageBytesToDownload;
			}
			set
			{
				base.CheckDisposed();
				this.estimatedMessageBytesToDownload = value;
			}
		}

		internal int? LowestAttemptedSequenceNumber
		{
			get
			{
				base.CheckDisposed();
				return this.lowestAttemptedSequenceNumber;
			}
			set
			{
				base.CheckDisposed();
				this.lowestAttemptedSequenceNumber = value;
			}
		}

		internal string LastAppendMessageId
		{
			get
			{
				base.CheckDisposed();
				return this.lastAppendMessageId;
			}
			set
			{
				base.CheckDisposed();
				this.lastAppendMessageId = value;
			}
		}

		internal Stream LastAppendMessageMimeStream
		{
			get
			{
				base.CheckDisposed();
				return this.lastAppendMessageMimeStream;
			}
			set
			{
				base.CheckDisposed();
				this.lastAppendMessageMimeStream = value;
			}
		}

		internal bool MoreItemsAvailable
		{
			get
			{
				base.CheckDisposed();
				return this.moreItemsAvailable;
			}
			set
			{
				base.CheckDisposed();
				this.moreItemsAvailable = value;
			}
		}

		internal IEnumerator<string> CloudFolderEnumerator
		{
			get
			{
				base.CheckDisposed();
				return this.cloudFolderEnumerator;
			}
			set
			{
				base.CheckDisposed();
				this.cloudFolderEnumerator = value;
			}
		}

		public bool CanTrackItemCount
		{
			get
			{
				base.CheckDisposed();
				return this.canTrackItemCount;
			}
			set
			{
				base.CheckDisposed();
				this.canTrackItemCount = value;
			}
		}

		internal IEnumerator<IMAPFolder> NewCloudFolderEnumerator
		{
			get
			{
				base.CheckDisposed();
				return this.newCloudFolderEnumerator;
			}
			set
			{
				base.CheckDisposed();
				this.newCloudFolderEnumerator = value;
			}
		}

		internal IEnumerator<IMAPFolder> CheckForChangesCloudFolderEnumerator
		{
			get
			{
				base.CheckDisposed();
				return this.checkForChangesCloudFolderEnumerator;
			}
			set
			{
				base.CheckDisposed();
				this.checkForChangesCloudFolderEnumerator = value;
			}
		}

		internal bool LightFetchDone
		{
			get
			{
				base.CheckDisposed();
				return this.lightFetchDone;
			}
			set
			{
				base.CheckDisposed();
				this.lightFetchDone = value;
			}
		}

		public string Protocol
		{
			get
			{
				base.CheckDisposed();
				return "IMAP";
			}
		}

		public string SessionId
		{
			get
			{
				base.CheckDisposed();
				return this.sessionId;
			}
		}

		public string Server
		{
			get
			{
				base.CheckDisposed();
				return this.imapSubscription.IMAPServer;
			}
		}

		internal IMAPClientState ClientState
		{
			get
			{
				base.CheckDisposed();
				return this.clientState;
			}
		}

		internal int CurrentFolderListLevel
		{
			get
			{
				base.CheckDisposed();
				return this.currentFolderListLevel;
			}
		}

		internal bool AnyFoldersExistAtPreviousLevel
		{
			get
			{
				base.CheckDisposed();
				return this.anyFoldersExistAtPreviousLevel;
			}
		}

		internal IDictionary<string, IMAPFolder> CloudIdToFolder
		{
			get
			{
				base.CheckDisposed();
				return this.cloudIdToFolderMap;
			}
		}

		internal IMAPFolder CurrentFolder
		{
			get
			{
				base.CheckDisposed();
				return this.currentFolder;
			}
			set
			{
				base.CheckDisposed();
				this.currentFolder = value;
			}
		}

		internal string PendingExpungeCloudFolderId
		{
			get
			{
				base.CheckDisposed();
				return this.pendingExpungeCloudFolderId;
			}
			set
			{
				base.CheckDisposed();
				this.pendingExpungeCloudFolderId = value;
			}
		}

		internal IDictionary<string, string> MessageIdToUidMap
		{
			get
			{
				base.CheckDisposed();
				return this.messageIdToUidMap;
			}
			set
			{
				base.CheckDisposed();
				this.messageIdToUidMap = value;
			}
		}

		internal IMAPSyncStorageProviderState.PostProcessor PostUidReconciliationCallback
		{
			get
			{
				base.CheckDisposed();
				return this.postUidReconciliationCallback;
			}
			set
			{
				base.CheckDisposed();
				this.postUidReconciliationCallback = value;
			}
		}

		internal IEnumerator<SyncChangeEntry> ApplyChangesEnumerator
		{
			get
			{
				base.CheckDisposed();
				return this.applyChangesEnumerator;
			}
			set
			{
				base.CheckDisposed();
				this.applyChangesEnumerator = value;
			}
		}

		internal int MaxDownloadItemsPerConnection
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadItemsPerConnection;
			}
			set
			{
				base.CheckDisposed();
				this.maxDownloadItemsPerConnection = value;
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

		internal long MaxDownloadBytesAllowed
		{
			get
			{
				base.CheckDisposed();
				return this.maxDownloadBytesAllowed;
			}
		}

		internal long TotalBytesSent
		{
			get
			{
				base.CheckDisposed();
				return this.ClientState.CommClient.TotalBytesSent;
			}
		}

		internal long TotalBytesReceived
		{
			get
			{
				base.CheckDisposed();
				return this.ClientState.CommClient.TotalBytesReceived;
			}
		}

		internal IMAPFolder LastSelectFailedFolder
		{
			get
			{
				base.CheckDisposed();
				return this.lastSelectFailedFolder;
			}
			set
			{
				base.CheckDisposed();
				this.lastSelectFailedFolder = value;
			}
		}

		internal IMAPMailFlags LastAppendMessageFlags
		{
			get
			{
				base.CheckDisposed();
				return this.lastAppendMessageFlags;
			}
			set
			{
				base.CheckDisposed();
				this.lastAppendMessageFlags = value;
			}
		}

		internal Queue<IMAPFolder> ApplyDeleteFolders
		{
			get
			{
				base.CheckDisposed();
				return this.applyDeleteFolders;
			}
			set
			{
				base.CheckDisposed();
				this.applyDeleteFolders = value;
			}
		}

		internal SyncChangeEntry LastAppendMessageChange
		{
			get
			{
				base.CheckDisposed();
				return this.lastAppendMessageChange;
			}
			set
			{
				base.CheckDisposed();
				this.lastAppendMessageChange = value;
			}
		}

		internal bool RequiresLogOff
		{
			get
			{
				base.CheckDisposed();
				return this.requiresLogOff;
			}
			set
			{
				base.CheckDisposed();
				this.requiresLogOff = value;
			}
		}

		internal bool IsListLevelsComplete
		{
			get
			{
				base.CheckDisposed();
				return this.isListLevelsComplete;
			}
			set
			{
				base.CheckDisposed();
				this.isListLevelsComplete = value;
			}
		}

		internal bool WasEnumerateEntered
		{
			get
			{
				base.CheckDisposed();
				return this.wasEnumerateEntered;
			}
			set
			{
				base.CheckDisposed();
				this.wasEnumerateEntered = value;
			}
		}

		internal void SetInitialFolderLevel()
		{
			base.CheckDisposed();
			this.currentFolderListLevel = 1;
			this.anyFoldersExistAtPreviousLevel = true;
		}

		internal void AdvanceToNextFolderLevel(bool anyFoldersAtThisLevel)
		{
			base.CheckDisposed();
			this.currentFolderListLevel++;
			this.anyFoldersExistAtPreviousLevel = anyFoldersAtThisLevel;
		}

		internal void StoreDefaultFolderMapping(DefaultFolderType folderType, string mailboxName)
		{
			base.CheckDisposed();
			int num = (int)folderType;
			string property = num.ToString(CultureInfo.InvariantCulture);
			if (base.StateStorage.ContainsProperty(property))
			{
				base.StateStorage.ChangePropertyValue(property, mailboxName);
				return;
			}
			base.StateStorage.AddProperty(property, mailboxName);
		}

		internal string GetDefaultFolderMapping(DefaultFolderType folderType)
		{
			base.CheckDisposed();
			int num = (int)folderType;
			string property = num.ToString(CultureInfo.InvariantCulture);
			string result;
			base.StateStorage.TryGetPropertyValue(property, out result);
			return result;
		}

		internal char GetSeparatorCharacter()
		{
			base.CheckDisposed();
			return this.GetSeparatorCharacter(this.CurrentFolder);
		}

		internal char GetSeparatorCharacter(IMAPFolder incomingFolder)
		{
			base.CheckDisposed();
			IMAPFolder imapfolder = incomingFolder;
			if (imapfolder != null && imapfolder.Mailbox.Separator != null)
			{
				base.SyncLogSession.LogDebugging((TSLID)1406UL, IMAPSyncStorageProvider.Tracer, "Using the specified mailbox {0} hierarchy separator '{1}'.", new object[]
				{
					imapfolder.Mailbox.Name,
					imapfolder.Mailbox.Separator.Value
				});
				return imapfolder.Mailbox.Separator.Value;
			}
			if (this.CloudIdToFolder.TryGetValue("INBOX", out imapfolder) && imapfolder.Mailbox.Separator != null)
			{
				base.SyncLogSession.LogDebugging((TSLID)1407UL, IMAPSyncStorageProvider.Tracer, "Using the INBOX mailbox hierarchy separator '{0}'.", new object[]
				{
					imapfolder.Mailbox.Separator.Value
				});
				return imapfolder.Mailbox.Separator.Value;
			}
			base.SyncLogSession.LogDebugging((TSLID)811UL, IMAPSyncStorageProvider.Tracer, "Using default hierarchy separator.", new object[0]);
			return IMAPFolder.DefaultHierarchySeparator;
		}

		internal char GetSeparatorCharacter(string cloudFolderId)
		{
			base.CheckDisposed();
			IMAPFolder incomingFolder = null;
			if (cloudFolderId != null && this.CloudIdToFolder.ContainsKey(cloudFolderId))
			{
				incomingFolder = this.CloudIdToFolder[cloudFolderId];
			}
			return this.GetSeparatorCharacter(incomingFolder);
		}

		internal void AppendToSyncWatermark(string folderName, long? uidValidity, long? uidNext)
		{
			base.CheckDisposed();
			if (this.syncWatermarkBuilder == null)
			{
				this.syncWatermarkBuilder = new StringBuilder(100);
			}
			this.syncWatermarkBuilder.Append(folderName);
			this.syncWatermarkBuilder.Append(uidValidity);
			this.syncWatermarkBuilder.Append(uidNext);
		}

		internal string ComputeSyncWatermark()
		{
			base.CheckDisposed();
			if (this.syncWatermarkBuilder == null)
			{
				return SyncUtilities.ComputeSHA512Hash(string.Empty);
			}
			return SyncUtilities.ComputeSHA512Hash(this.syncWatermarkBuilder.ToString());
		}

		internal void UpdateSubscriptionSyncWatermarkIfNeeded()
		{
			if (this.MoreItemsAvailable)
			{
				return;
			}
			string watermark = this.ComputeSyncWatermark();
			this.ImapWatermark.Save(watermark);
		}

		internal void UpdateMailboxItemCountFromCurrentFolderData()
		{
			if (this.CurrentFolder.NumberOfMessages == null)
			{
				return;
			}
			if (base.CloudStatistics.TotalItemsInSourceMailbox == null || base.CloudStatistics.TotalItemsInSourceMailbox.Value == SyncUtilities.DataNotAvailable)
			{
				base.CloudStatistics.TotalItemsInSourceMailbox = new long?(this.CurrentFolder.NumberOfMessages.Value);
				return;
			}
			base.CloudStatistics.TotalItemsInSourceMailbox += this.CurrentFolder.NumberOfMessages.Value;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.SyncLogSession.LogDebugging((TSLID)812UL, "InternalDispose({0})", new object[]
			{
				disposing
			});
			if (disposing && this.clientState != null)
			{
				this.clientState.Dispose();
				this.clientState = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IMAPSyncStorageProviderState>(this);
		}

		internal const int DefaultChangeListSize = 5;

		internal const int DefaultFolderListSize = 5;

		internal const string OrphanedCloudVersion = "ORPHANED";

		internal const int NumFetchBatchSize = 100;

		internal const string ImapVersionString = "imap4rev1";

		private const string IMAPProtocolName = "IMAP";

		private const int InitialSyncWatermarkCapacity = 100;

		internal static readonly string LastMessageSyncKey = "LastMessageSyncKey";

		private string sessionId;

		private IMAPAggregationSubscription imapSubscription;

		private IMAPClientState clientState;

		private int maxDownloadItemsPerConnection;

		private long maxDownloadSizePerItem;

		private long maxDownloadBytesAllowed;

		private bool canTrackItemCount;

		private Dictionary<DefaultFolderType, string> preprocessedDefaultMappings;

		private long estimatedMessageBytesToDownload;

		private SortedList<string, SyncChangeEntry> sortedFolderAddsSyncChangeEntries;

		private int currentFolderListLevel;

		private bool anyFoldersExistAtPreviousLevel;

		private string pendingExpungeCloudFolderId;

		private IDictionary<string, IMAPFolder> cloudIdToFolderMap;

		private IDictionary<string, string> cloudItemChangeMap;

		private IDictionary<string, string> messageIdToUidMap;

		private IMAPSyncStorageProviderState.PostProcessor postUidReconciliationCallback;

		private IMAPFolder currentFolder;

		private IMAPFolder lastSelectFailedFolder;

		private Exception lastSelectFailedFolderException;

		private int? lowestAttemptedSequenceNumber;

		private bool lightFetchDone;

		private string lastAppendMessageId;

		private IMAPMailFlags lastAppendMessageFlags;

		private Stream lastAppendMessageMimeStream;

		private IEnumerator<SyncChangeEntry> applyChangesEnumerator;

		private IEnumerator<string> cloudFolderEnumerator;

		private IEnumerator<IMAPFolder> newCloudFolderEnumerator;

		private IEnumerator<IMAPFolder> checkForChangesCloudFolderEnumerator;

		private Queue<IMAPFolder> applyDeleteFolders;

		private SyncChangeEntry lastAppendMessageChange;

		private bool moreItemsAvailable;

		private bool requiresLogOff;

		private StringBuilder syncWatermarkBuilder;

		private bool isListLevelsComplete;

		private bool wasEnumerateEntered;

		internal delegate void PostProcessor(IAsyncResult curOp, Exception exceptionDuringReconciliation);
	}
}
