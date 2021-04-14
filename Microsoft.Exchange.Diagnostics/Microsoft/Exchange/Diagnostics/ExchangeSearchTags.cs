using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ExchangeSearchTags
	{
		public const int General = 0;

		public const int CatchUpNotificationCrawler = 1;

		public const int ChunkSourceFunctions = 2;

		public const int Crawler = 3;

		public const int CSrchProject = 4;

		public const int DataSourceFunctions = 5;

		public const int Driver = 6;

		public const int FilterEnumerator = 7;

		public const int FTEAdminComInterop = 8;

		public const int IndexablePropertyCache = 9;

		public const int MapiIterator = 10;

		public const int NotificationProcessing = 11;

		public const int NotificationQueue = 12;

		public const int NotificationWatcher = 13;

		public const int PHFunctions = 14;

		public const int RetryEngine = 16;

		public const int ThrottleController = 17;

		public const int PropertyStoreCache = 18;

		public const int ActiveManager = 19;

		public const int CatalogHealth = 20;

		public const int SearchCatalogClient = 21;

		public const int SearchCatalogServer = 22;

		public const int MailboxDeletion = 23;

		public const int PropertyStore = 24;

		public const int StoreMonitor = 25;

		public const int FaultInjection = 26;

		public const int MailboxIndexingHelper = 27;

		public const int CatalogState = 28;

		public const int FileExtensionCache = 29;

		public const int MsFteSqlMonitor = 30;

		public const int ServerConnections = 31;

		public const int LogonCache = 32;

		public const int Logon = 33;

		public const int CatalogReconciler = 34;

		public const int CatalogReconcileResult = 35;

		public const int AllCatalogReconciler = 36;

		public const int MailboxReconcileResult = 37;

		public const int NewFilterMonitor = 38;

		public const int InMemoryDefault = 39;

		public const int TestExchangeSearch = 40;

		public const int BatchThrottler = 41;

		public const int ThrottleParameters = 42;

		public const int ThrottleDataProvider = 43;

		public const int RegistryParameter = 44;

		public const int LatencySampler = 45;

		public const int MovingAverage = 46;

		public const int CoreComponent = 50;

		public const int CoreComponentRegistry = 51;

		public const int CoreGeneral = 52;

		public const int FastFeeder = 53;

		public const int MdbNotificationsFeeder = 54;

		public const int Service = 55;

		public const int Engine = 56;

		public const int MdbFeedingController = 57;

		public const int IndexManagement = 58;

		public const int CoreFailureMonitor = 59;

		public const int MdbCrawlerFeeder = 60;

		public const int MdbDocumentAdapter = 61;

		public const int CoreDocumentModel = 62;

		public const int PipelineLoader = 63;

		public const int CorePipeline = 64;

		public const int QueueManager = 65;

		public const int CrawlerWatermarkManager = 66;

		public const int FailedItemStorage = 67;

		public const int MdbWatcher = 68;

		public const int MdbRetryFeeder = 69;

		public const int MdbSessionCache = 70;

		public const int RetrieverOperator = 71;

		public const int StreamManager = 72;

		public const int StreamChannel = 73;

		public const int AnnotationToken = 74;

		public const int TransportOperator = 75;

		public const int IndexRoutingAgent = 76;

		public const int IndexDeliveryAgent = 77;

		public const int TransportFlowFeeder = 78;

		public const int QueryExecutor = 79;

		public const int ErrorOperator = 80;

		public const int NotificationsWatermarkManager = 81;

		public const int IndexStatusStore = 82;

		public const int IndexStatusProvider = 83;

		public const int FastIoExtension = 84;

		public const int XSOMailboxSession = 85;

		public const int PostDocParserOperator = 86;

		public const int RecordManagerOperator = 87;

		public const int OperatorDiagnostics = 88;

		public const int SearchRpcClient = 89;

		public const int SearchRpcServer = 90;

		public const int DocumentTrackerOperator = 91;

		public const int ErrorBypassOperator = 92;

		public const int FeederThrottling = 93;

		public const int WatermarkStorage = 94;

		public const int DiagnosticOperator = 95;

		public const int InstantSearch = 96;

		public const int TopNManagementClient = 97;

		public const int SearchDictionary = 98;

		public static Guid guid = new Guid("c3ea5adf-c135-45e7-9dff-e1dc3bd67123");
	}
}
