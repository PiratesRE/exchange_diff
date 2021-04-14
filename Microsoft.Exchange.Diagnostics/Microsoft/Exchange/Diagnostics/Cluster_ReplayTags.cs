using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Cluster_ReplayTags
	{
		public const int ReplayApi = 0;

		public const int EseutilWrapper = 1;

		public const int State = 2;

		public const int LogReplayer = 3;

		public const int ReplicaInstance = 4;

		public const int Cmdlets = 5;

		public const int ShipLog = 6;

		public const int LogCopy = 7;

		public const int LogInspector = 8;

		public const int ReplayManager = 9;

		public const int CReplicaSeeder = 10;

		public const int NetShare = 11;

		public const int ReplicaVssWriterInterop = 12;

		public const int StateLock = 13;

		public const int FileChecker = 14;

		public const int Cluster = 15;

		public const int SeederWrapper = 16;

		public const int PFD = 17;

		public const int IncrementalReseeder = 18;

		public const int Dumpster = 19;

		public const int CLogShipContext = 20;

		public const int ClusDBWrite = 21;

		public const int ReplayConfiguration = 22;

		public const int NetPath = 23;

		public const int HealthChecks = 24;

		public const int ReplayServiceRpc = 25;

		public const int ActiveManager = 26;

		public const int SeederServer = 27;

		public const int SeederClient = 28;

		public const int LogTruncater = 29;

		public const int FailureItem = 30;

		public const int LogCopyServer = 31;

		public const int LogCopyClient = 32;

		public const int TcpChannel = 33;

		public const int TcpClient = 34;

		public const int TcpServer = 35;

		public const int RemoteDataProvider = 36;

		public const int MonitoredDatabase = 37;

		public const int NetworkManager = 38;

		public const int NetworkChannel = 39;

		public const int FaultInjection = 40;

		public const int GranularWriter = 41;

		public const int GranularReader = 42;

		public const int ThirdPartyClient = 43;

		public const int ThirdPartyManager = 44;

		public const int ThirdPartyService = 45;

		public const int ClusterEvents = 46;

		public const int AmNetworkMonitor = 47;

		public const int AmConfigManager = 48;

		public const int AmSystemManager = 49;

		public const int AmServiceMonitor = 50;

		public const int ServiceOperations = 51;

		public const int AmServerNameCache = 53;

		public const int KernelWatchdogTimer = 54;

		public const int FailureItemHealthMonitor = 55;

		public const int ReplayServiceDiagnostics = 56;

		public const int LogRepair = 57;

		public const int PassiveBlockMode = 58;

		public const int LogCopier = 59;

		public const int DiskHeartbeat = 60;

		public const int Monitoring = 61;

		public const int ServerLocatorService = 62;

		public const int ServerLocatorServiceClient = 63;

		public const int LatencyChecker = 64;

		public const int VolumeManager = 65;

		public const int AutoReseed = 66;

		public const int DiskReclaimer = 67;

		public const int ADCache = 68;

		public const int DbTracker = 69;

		public const int DatabaseCopyLayout = 70;

		public const int CompositeKey = 71;

		public const int ClusdbKey = 72;

		public const int DxStoreKey = 73;

		public static Guid guid = new Guid("404a3308-17e1-4ac3-9167-1b09c36850bd");
	}
}
