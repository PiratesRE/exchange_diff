using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_PhysicalAccessTags
	{
		public const int DbInteractionSummary = 0;

		public const int DbInteractionIntermediate = 1;

		public const int DbInteractionDetail = 2;

		public const int DbInitialization = 3;

		public const int DirtyObjects = 4;

		public const int DbIO = 5;

		public const int BadPlanDetection = 6;

		public const int CategorizedTableOperator = 15;

		public const int SnapshotOperation = 16;

		public const int FaultInjection = 20;

		public const int JetInformation = 651;

		public const int JetErrors = 652;

		public const int JetAsserts = 653;

		public const int JetAPI = 654;

		public const int JetInitTerm = 655;

		public const int JetBufferManager = 656;

		public const int JetBufferManagerHashedLatches = 657;

		public const int JetIO = 658;

		public const int JetMemory = 659;

		public const int JetVersionStore = 660;

		public const int JetVersionStoreOOM = 661;

		public const int JetVersionCleanup = 662;

		public const int JetCatalog = 663;

		public const int JetDDLRead = 664;

		public const int JetDDLWrite = 665;

		public const int JetDMLRead = 666;

		public const int JetDMLWrite = 667;

		public const int JetDMLConflicts = 668;

		public const int JetInstances = 669;

		public const int JetDatabases = 670;

		public const int JetSessions = 671;

		public const int JetCursors = 672;

		public const int JetCursorNavigation = 673;

		public const int JetCursorPageRefs = 674;

		public const int JetBtree = 675;

		public const int JetSpace = 676;

		public const int JetFCBs = 677;

		public const int JetTransactions = 678;

		public const int JetLogging = 679;

		public const int JetRecovery = 680;

		public const int JetBackup = 681;

		public const int JetRestore = 682;

		public const int JetOLD = 683;

		public const int JetEventlog = 684;

		public const int JetBufferManagerMaintTasks = 685;

		public const int JetSpaceManagement = 686;

		public const int JetSpaceInternal = 687;

		public const int JetIOQueue = 688;

		public const int JetDiskVolumeManagement = 689;

		public const int JetCallbacks = 690;

		public const int JetIOProblems = 691;

		public const int JetUpgrade = 692;

		public const int JetBufMgrCacheState = 693;

		public const int JetBufMgrDirtyState = 694;

		public static Guid guid = new Guid("40c22f16-f297-499a-b812-a5a352295610");
	}
}
