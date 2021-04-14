using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ContentAggregationTags
	{
		public const int Common = 0;

		public const int Pop3Client = 1;

		public const int DeltaSyncStorageProvider = 2;

		public const int FeedParser = 3;

		public const int ContentGeneration = 4;

		public const int SubscriptionSubmissionRpc = 5;

		public const int RssServerLock = 6;

		public const int SubscriptionSubmit = 7;

		public const int SubscriptionSubmissionServer = 8;

		public const int Scheduler = 9;

		public const int SubscriptionManager = 10;

		public const int WebFeedProtocolHandler = 11;

		public const int DeliveryAgent = 12;

		public const int HtmlFixer = 13;

		public const int Pop3ProtocolHandler = 14;

		public const int Pop3StorageProvider = 15;

		public const int SubscriptionTask = 17;

		public const int SyncEngine = 18;

		public const int TransportSyncStorageProvider = 19;

		public const int StateStorage = 20;

		public const int SyncLog = 21;

		public const int XSOSyncStorageProvider = 22;

		public const int SubscriptionEventbasedAssistant = 23;

		public const int CacheManager = 24;

		public const int CacheManagerLookup = 25;

		public const int TokenManager = 26;

		public const int SubscriptionCompletionServer = 27;

		public const int SubscriptionCompletionClient = 28;

		public const int EventLog = 29;

		public const int ProtocolHandler = 30;

		public const int AggregationComponent = 31;

		public const int IMAPSyncStorageProvider = 32;

		public const int IMAPClient = 33;

		public const int FaultInjection = 34;

		public const int DavClient = 35;

		public const int DavSyncStorageProvider = 36;

		public const int SyncPoisonHandler = 37;

		public const int NativeSyncStorageProvider = 38;

		public const int SendAs = 39;

		public const int StatefulHubPicker = 40;

		public const int RemoteAccountPolicy = 41;

		public const int DataAccessLayer = 42;

		public const int SystemMailboxSessionPool = 43;

		public const int SubscriptionCacheMessage = 44;

		public const int SubscriptionQueue = 45;

		public const int SubscriptionCacheRpcServer = 46;

		public const int ContentAggregationConfig = 47;

		public const int AggregationConfiguration = 48;

		public const int SubscriptionAgentManager = 49;

		public const int SyncHealthLogManager = 50;

		public const int TransportSyncManagerSvc = 51;

		public const int GlobalDatabaseHandler = 52;

		public const int DatabaseManager = 53;

		public const int MailboxManager = 54;

		public const int MailboxTableManager = 55;

		public const int SubscriptionNotificationServer = 56;

		public const int SubscriptionNotificationClient = 57;

		public const int FacebookProvider = 58;

		public const int LinkedInProvider = 59;

		public const int SubscriptionRemove = 60;

		public static Guid guid = new Guid("B29C4959-0C49-4bfa-BDDD-9B6E961420AC");
	}
}
