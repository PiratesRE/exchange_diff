using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess
{
	public static class ExTraceGlobals
	{
		public static Trace DbInteractionSummaryTracer
		{
			get
			{
				if (ExTraceGlobals.dbInteractionSummaryTracer == null)
				{
					ExTraceGlobals.dbInteractionSummaryTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dbInteractionSummaryTracer;
			}
		}

		public static Trace DbInteractionIntermediateTracer
		{
			get
			{
				if (ExTraceGlobals.dbInteractionIntermediateTracer == null)
				{
					ExTraceGlobals.dbInteractionIntermediateTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dbInteractionIntermediateTracer;
			}
		}

		public static Trace DbInteractionDetailTracer
		{
			get
			{
				if (ExTraceGlobals.dbInteractionDetailTracer == null)
				{
					ExTraceGlobals.dbInteractionDetailTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.dbInteractionDetailTracer;
			}
		}

		public static Trace DbInitializationTracer
		{
			get
			{
				if (ExTraceGlobals.dbInitializationTracer == null)
				{
					ExTraceGlobals.dbInitializationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.dbInitializationTracer;
			}
		}

		public static Trace DirtyObjectsTracer
		{
			get
			{
				if (ExTraceGlobals.dirtyObjectsTracer == null)
				{
					ExTraceGlobals.dirtyObjectsTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.dirtyObjectsTracer;
			}
		}

		public static Trace DbIOTracer
		{
			get
			{
				if (ExTraceGlobals.dbIOTracer == null)
				{
					ExTraceGlobals.dbIOTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.dbIOTracer;
			}
		}

		public static Trace BadPlanDetectionTracer
		{
			get
			{
				if (ExTraceGlobals.badPlanDetectionTracer == null)
				{
					ExTraceGlobals.badPlanDetectionTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.badPlanDetectionTracer;
			}
		}

		public static Trace CategorizedTableOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.categorizedTableOperatorTracer == null)
				{
					ExTraceGlobals.categorizedTableOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.categorizedTableOperatorTracer;
			}
		}

		public static Trace SnapshotOperationTracer
		{
			get
			{
				if (ExTraceGlobals.snapshotOperationTracer == null)
				{
					ExTraceGlobals.snapshotOperationTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.snapshotOperationTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace JetInformationTracer
		{
			get
			{
				if (ExTraceGlobals.jetInformationTracer == null)
				{
					ExTraceGlobals.jetInformationTracer = new Trace(ExTraceGlobals.componentGuid, 651);
				}
				return ExTraceGlobals.jetInformationTracer;
			}
		}

		public static Trace JetErrorsTracer
		{
			get
			{
				if (ExTraceGlobals.jetErrorsTracer == null)
				{
					ExTraceGlobals.jetErrorsTracer = new Trace(ExTraceGlobals.componentGuid, 652);
				}
				return ExTraceGlobals.jetErrorsTracer;
			}
		}

		public static Trace JetAssertsTracer
		{
			get
			{
				if (ExTraceGlobals.jetAssertsTracer == null)
				{
					ExTraceGlobals.jetAssertsTracer = new Trace(ExTraceGlobals.componentGuid, 653);
				}
				return ExTraceGlobals.jetAssertsTracer;
			}
		}

		public static Trace JetAPITracer
		{
			get
			{
				if (ExTraceGlobals.jetAPITracer == null)
				{
					ExTraceGlobals.jetAPITracer = new Trace(ExTraceGlobals.componentGuid, 654);
				}
				return ExTraceGlobals.jetAPITracer;
			}
		}

		public static Trace JetInitTermTracer
		{
			get
			{
				if (ExTraceGlobals.jetInitTermTracer == null)
				{
					ExTraceGlobals.jetInitTermTracer = new Trace(ExTraceGlobals.componentGuid, 655);
				}
				return ExTraceGlobals.jetInitTermTracer;
			}
		}

		public static Trace JetBufferManagerTracer
		{
			get
			{
				if (ExTraceGlobals.jetBufferManagerTracer == null)
				{
					ExTraceGlobals.jetBufferManagerTracer = new Trace(ExTraceGlobals.componentGuid, 656);
				}
				return ExTraceGlobals.jetBufferManagerTracer;
			}
		}

		public static Trace JetBufferManagerHashedLatchesTracer
		{
			get
			{
				if (ExTraceGlobals.jetBufferManagerHashedLatchesTracer == null)
				{
					ExTraceGlobals.jetBufferManagerHashedLatchesTracer = new Trace(ExTraceGlobals.componentGuid, 657);
				}
				return ExTraceGlobals.jetBufferManagerHashedLatchesTracer;
			}
		}

		public static Trace JetIOTracer
		{
			get
			{
				if (ExTraceGlobals.jetIOTracer == null)
				{
					ExTraceGlobals.jetIOTracer = new Trace(ExTraceGlobals.componentGuid, 658);
				}
				return ExTraceGlobals.jetIOTracer;
			}
		}

		public static Trace JetMemoryTracer
		{
			get
			{
				if (ExTraceGlobals.jetMemoryTracer == null)
				{
					ExTraceGlobals.jetMemoryTracer = new Trace(ExTraceGlobals.componentGuid, 659);
				}
				return ExTraceGlobals.jetMemoryTracer;
			}
		}

		public static Trace JetVersionStoreTracer
		{
			get
			{
				if (ExTraceGlobals.jetVersionStoreTracer == null)
				{
					ExTraceGlobals.jetVersionStoreTracer = new Trace(ExTraceGlobals.componentGuid, 660);
				}
				return ExTraceGlobals.jetVersionStoreTracer;
			}
		}

		public static Trace JetVersionStoreOOMTracer
		{
			get
			{
				if (ExTraceGlobals.jetVersionStoreOOMTracer == null)
				{
					ExTraceGlobals.jetVersionStoreOOMTracer = new Trace(ExTraceGlobals.componentGuid, 661);
				}
				return ExTraceGlobals.jetVersionStoreOOMTracer;
			}
		}

		public static Trace JetVersionCleanupTracer
		{
			get
			{
				if (ExTraceGlobals.jetVersionCleanupTracer == null)
				{
					ExTraceGlobals.jetVersionCleanupTracer = new Trace(ExTraceGlobals.componentGuid, 662);
				}
				return ExTraceGlobals.jetVersionCleanupTracer;
			}
		}

		public static Trace JetCatalogTracer
		{
			get
			{
				if (ExTraceGlobals.jetCatalogTracer == null)
				{
					ExTraceGlobals.jetCatalogTracer = new Trace(ExTraceGlobals.componentGuid, 663);
				}
				return ExTraceGlobals.jetCatalogTracer;
			}
		}

		public static Trace JetDDLReadTracer
		{
			get
			{
				if (ExTraceGlobals.jetDDLReadTracer == null)
				{
					ExTraceGlobals.jetDDLReadTracer = new Trace(ExTraceGlobals.componentGuid, 664);
				}
				return ExTraceGlobals.jetDDLReadTracer;
			}
		}

		public static Trace JetDDLWriteTracer
		{
			get
			{
				if (ExTraceGlobals.jetDDLWriteTracer == null)
				{
					ExTraceGlobals.jetDDLWriteTracer = new Trace(ExTraceGlobals.componentGuid, 665);
				}
				return ExTraceGlobals.jetDDLWriteTracer;
			}
		}

		public static Trace JetDMLReadTracer
		{
			get
			{
				if (ExTraceGlobals.jetDMLReadTracer == null)
				{
					ExTraceGlobals.jetDMLReadTracer = new Trace(ExTraceGlobals.componentGuid, 666);
				}
				return ExTraceGlobals.jetDMLReadTracer;
			}
		}

		public static Trace JetDMLWriteTracer
		{
			get
			{
				if (ExTraceGlobals.jetDMLWriteTracer == null)
				{
					ExTraceGlobals.jetDMLWriteTracer = new Trace(ExTraceGlobals.componentGuid, 667);
				}
				return ExTraceGlobals.jetDMLWriteTracer;
			}
		}

		public static Trace JetDMLConflictsTracer
		{
			get
			{
				if (ExTraceGlobals.jetDMLConflictsTracer == null)
				{
					ExTraceGlobals.jetDMLConflictsTracer = new Trace(ExTraceGlobals.componentGuid, 668);
				}
				return ExTraceGlobals.jetDMLConflictsTracer;
			}
		}

		public static Trace JetInstancesTracer
		{
			get
			{
				if (ExTraceGlobals.jetInstancesTracer == null)
				{
					ExTraceGlobals.jetInstancesTracer = new Trace(ExTraceGlobals.componentGuid, 669);
				}
				return ExTraceGlobals.jetInstancesTracer;
			}
		}

		public static Trace JetDatabasesTracer
		{
			get
			{
				if (ExTraceGlobals.jetDatabasesTracer == null)
				{
					ExTraceGlobals.jetDatabasesTracer = new Trace(ExTraceGlobals.componentGuid, 670);
				}
				return ExTraceGlobals.jetDatabasesTracer;
			}
		}

		public static Trace JetSessionsTracer
		{
			get
			{
				if (ExTraceGlobals.jetSessionsTracer == null)
				{
					ExTraceGlobals.jetSessionsTracer = new Trace(ExTraceGlobals.componentGuid, 671);
				}
				return ExTraceGlobals.jetSessionsTracer;
			}
		}

		public static Trace JetCursorsTracer
		{
			get
			{
				if (ExTraceGlobals.jetCursorsTracer == null)
				{
					ExTraceGlobals.jetCursorsTracer = new Trace(ExTraceGlobals.componentGuid, 672);
				}
				return ExTraceGlobals.jetCursorsTracer;
			}
		}

		public static Trace JetCursorNavigationTracer
		{
			get
			{
				if (ExTraceGlobals.jetCursorNavigationTracer == null)
				{
					ExTraceGlobals.jetCursorNavigationTracer = new Trace(ExTraceGlobals.componentGuid, 673);
				}
				return ExTraceGlobals.jetCursorNavigationTracer;
			}
		}

		public static Trace JetCursorPageRefsTracer
		{
			get
			{
				if (ExTraceGlobals.jetCursorPageRefsTracer == null)
				{
					ExTraceGlobals.jetCursorPageRefsTracer = new Trace(ExTraceGlobals.componentGuid, 674);
				}
				return ExTraceGlobals.jetCursorPageRefsTracer;
			}
		}

		public static Trace JetBtreeTracer
		{
			get
			{
				if (ExTraceGlobals.jetBtreeTracer == null)
				{
					ExTraceGlobals.jetBtreeTracer = new Trace(ExTraceGlobals.componentGuid, 675);
				}
				return ExTraceGlobals.jetBtreeTracer;
			}
		}

		public static Trace JetSpaceTracer
		{
			get
			{
				if (ExTraceGlobals.jetSpaceTracer == null)
				{
					ExTraceGlobals.jetSpaceTracer = new Trace(ExTraceGlobals.componentGuid, 676);
				}
				return ExTraceGlobals.jetSpaceTracer;
			}
		}

		public static Trace JetFCBsTracer
		{
			get
			{
				if (ExTraceGlobals.jetFCBsTracer == null)
				{
					ExTraceGlobals.jetFCBsTracer = new Trace(ExTraceGlobals.componentGuid, 677);
				}
				return ExTraceGlobals.jetFCBsTracer;
			}
		}

		public static Trace JetTransactionsTracer
		{
			get
			{
				if (ExTraceGlobals.jetTransactionsTracer == null)
				{
					ExTraceGlobals.jetTransactionsTracer = new Trace(ExTraceGlobals.componentGuid, 678);
				}
				return ExTraceGlobals.jetTransactionsTracer;
			}
		}

		public static Trace JetLoggingTracer
		{
			get
			{
				if (ExTraceGlobals.jetLoggingTracer == null)
				{
					ExTraceGlobals.jetLoggingTracer = new Trace(ExTraceGlobals.componentGuid, 679);
				}
				return ExTraceGlobals.jetLoggingTracer;
			}
		}

		public static Trace JetRecoveryTracer
		{
			get
			{
				if (ExTraceGlobals.jetRecoveryTracer == null)
				{
					ExTraceGlobals.jetRecoveryTracer = new Trace(ExTraceGlobals.componentGuid, 680);
				}
				return ExTraceGlobals.jetRecoveryTracer;
			}
		}

		public static Trace JetBackupTracer
		{
			get
			{
				if (ExTraceGlobals.jetBackupTracer == null)
				{
					ExTraceGlobals.jetBackupTracer = new Trace(ExTraceGlobals.componentGuid, 681);
				}
				return ExTraceGlobals.jetBackupTracer;
			}
		}

		public static Trace JetRestoreTracer
		{
			get
			{
				if (ExTraceGlobals.jetRestoreTracer == null)
				{
					ExTraceGlobals.jetRestoreTracer = new Trace(ExTraceGlobals.componentGuid, 682);
				}
				return ExTraceGlobals.jetRestoreTracer;
			}
		}

		public static Trace JetOLDTracer
		{
			get
			{
				if (ExTraceGlobals.jetOLDTracer == null)
				{
					ExTraceGlobals.jetOLDTracer = new Trace(ExTraceGlobals.componentGuid, 683);
				}
				return ExTraceGlobals.jetOLDTracer;
			}
		}

		public static Trace JetEventlogTracer
		{
			get
			{
				if (ExTraceGlobals.jetEventlogTracer == null)
				{
					ExTraceGlobals.jetEventlogTracer = new Trace(ExTraceGlobals.componentGuid, 684);
				}
				return ExTraceGlobals.jetEventlogTracer;
			}
		}

		public static Trace JetBufferManagerMaintTasksTracer
		{
			get
			{
				if (ExTraceGlobals.jetBufferManagerMaintTasksTracer == null)
				{
					ExTraceGlobals.jetBufferManagerMaintTasksTracer = new Trace(ExTraceGlobals.componentGuid, 685);
				}
				return ExTraceGlobals.jetBufferManagerMaintTasksTracer;
			}
		}

		public static Trace JetSpaceManagementTracer
		{
			get
			{
				if (ExTraceGlobals.jetSpaceManagementTracer == null)
				{
					ExTraceGlobals.jetSpaceManagementTracer = new Trace(ExTraceGlobals.componentGuid, 686);
				}
				return ExTraceGlobals.jetSpaceManagementTracer;
			}
		}

		public static Trace JetSpaceInternalTracer
		{
			get
			{
				if (ExTraceGlobals.jetSpaceInternalTracer == null)
				{
					ExTraceGlobals.jetSpaceInternalTracer = new Trace(ExTraceGlobals.componentGuid, 687);
				}
				return ExTraceGlobals.jetSpaceInternalTracer;
			}
		}

		public static Trace JetIOQueueTracer
		{
			get
			{
				if (ExTraceGlobals.jetIOQueueTracer == null)
				{
					ExTraceGlobals.jetIOQueueTracer = new Trace(ExTraceGlobals.componentGuid, 688);
				}
				return ExTraceGlobals.jetIOQueueTracer;
			}
		}

		public static Trace JetDiskVolumeManagementTracer
		{
			get
			{
				if (ExTraceGlobals.jetDiskVolumeManagementTracer == null)
				{
					ExTraceGlobals.jetDiskVolumeManagementTracer = new Trace(ExTraceGlobals.componentGuid, 689);
				}
				return ExTraceGlobals.jetDiskVolumeManagementTracer;
			}
		}

		public static Trace JetCallbacksTracer
		{
			get
			{
				if (ExTraceGlobals.jetCallbacksTracer == null)
				{
					ExTraceGlobals.jetCallbacksTracer = new Trace(ExTraceGlobals.componentGuid, 690);
				}
				return ExTraceGlobals.jetCallbacksTracer;
			}
		}

		public static Trace JetIOProblemsTracer
		{
			get
			{
				if (ExTraceGlobals.jetIOProblemsTracer == null)
				{
					ExTraceGlobals.jetIOProblemsTracer = new Trace(ExTraceGlobals.componentGuid, 691);
				}
				return ExTraceGlobals.jetIOProblemsTracer;
			}
		}

		public static Trace JetUpgradeTracer
		{
			get
			{
				if (ExTraceGlobals.jetUpgradeTracer == null)
				{
					ExTraceGlobals.jetUpgradeTracer = new Trace(ExTraceGlobals.componentGuid, 692);
				}
				return ExTraceGlobals.jetUpgradeTracer;
			}
		}

		public static Trace JetBufMgrCacheStateTracer
		{
			get
			{
				if (ExTraceGlobals.jetBufMgrCacheStateTracer == null)
				{
					ExTraceGlobals.jetBufMgrCacheStateTracer = new Trace(ExTraceGlobals.componentGuid, 693);
				}
				return ExTraceGlobals.jetBufMgrCacheStateTracer;
			}
		}

		public static Trace JetBufMgrDirtyStateTracer
		{
			get
			{
				if (ExTraceGlobals.jetBufMgrDirtyStateTracer == null)
				{
					ExTraceGlobals.jetBufMgrDirtyStateTracer = new Trace(ExTraceGlobals.componentGuid, 694);
				}
				return ExTraceGlobals.jetBufMgrDirtyStateTracer;
			}
		}

		private static Guid componentGuid = new Guid("40c22f16-f297-499a-b812-a5a352295610");

		private static Trace dbInteractionSummaryTracer = null;

		private static Trace dbInteractionIntermediateTracer = null;

		private static Trace dbInteractionDetailTracer = null;

		private static Trace dbInitializationTracer = null;

		private static Trace dirtyObjectsTracer = null;

		private static Trace dbIOTracer = null;

		private static Trace badPlanDetectionTracer = null;

		private static Trace categorizedTableOperatorTracer = null;

		private static Trace snapshotOperationTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace jetInformationTracer = null;

		private static Trace jetErrorsTracer = null;

		private static Trace jetAssertsTracer = null;

		private static Trace jetAPITracer = null;

		private static Trace jetInitTermTracer = null;

		private static Trace jetBufferManagerTracer = null;

		private static Trace jetBufferManagerHashedLatchesTracer = null;

		private static Trace jetIOTracer = null;

		private static Trace jetMemoryTracer = null;

		private static Trace jetVersionStoreTracer = null;

		private static Trace jetVersionStoreOOMTracer = null;

		private static Trace jetVersionCleanupTracer = null;

		private static Trace jetCatalogTracer = null;

		private static Trace jetDDLReadTracer = null;

		private static Trace jetDDLWriteTracer = null;

		private static Trace jetDMLReadTracer = null;

		private static Trace jetDMLWriteTracer = null;

		private static Trace jetDMLConflictsTracer = null;

		private static Trace jetInstancesTracer = null;

		private static Trace jetDatabasesTracer = null;

		private static Trace jetSessionsTracer = null;

		private static Trace jetCursorsTracer = null;

		private static Trace jetCursorNavigationTracer = null;

		private static Trace jetCursorPageRefsTracer = null;

		private static Trace jetBtreeTracer = null;

		private static Trace jetSpaceTracer = null;

		private static Trace jetFCBsTracer = null;

		private static Trace jetTransactionsTracer = null;

		private static Trace jetLoggingTracer = null;

		private static Trace jetRecoveryTracer = null;

		private static Trace jetBackupTracer = null;

		private static Trace jetRestoreTracer = null;

		private static Trace jetOLDTracer = null;

		private static Trace jetEventlogTracer = null;

		private static Trace jetBufferManagerMaintTasksTracer = null;

		private static Trace jetSpaceManagementTracer = null;

		private static Trace jetSpaceInternalTracer = null;

		private static Trace jetIOQueueTracer = null;

		private static Trace jetDiskVolumeManagementTracer = null;

		private static Trace jetCallbacksTracer = null;

		private static Trace jetIOProblemsTracer = null;

		private static Trace jetUpgradeTracer = null;

		private static Trace jetBufMgrCacheStateTracer = null;

		private static Trace jetBufMgrDirtyStateTracer = null;
	}
}
