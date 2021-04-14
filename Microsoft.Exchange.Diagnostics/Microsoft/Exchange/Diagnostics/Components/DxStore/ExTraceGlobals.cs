using System;

namespace Microsoft.Exchange.Diagnostics.Components.DxStore
{
	public static class ExTraceGlobals
	{
		public static Trace AccessTracer
		{
			get
			{
				if (ExTraceGlobals.accessTracer == null)
				{
					ExTraceGlobals.accessTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.accessTracer;
			}
		}

		public static Trace ManagerTracer
		{
			get
			{
				if (ExTraceGlobals.managerTracer == null)
				{
					ExTraceGlobals.managerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.managerTracer;
			}
		}

		public static Trace InstanceTracer
		{
			get
			{
				if (ExTraceGlobals.instanceTracer == null)
				{
					ExTraceGlobals.instanceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.instanceTracer;
			}
		}

		public static Trace PaxosMessageTracer
		{
			get
			{
				if (ExTraceGlobals.paxosMessageTracer == null)
				{
					ExTraceGlobals.paxosMessageTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.paxosMessageTracer;
			}
		}

		public static Trace HealthCheckerTracer
		{
			get
			{
				if (ExTraceGlobals.healthCheckerTracer == null)
				{
					ExTraceGlobals.healthCheckerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.healthCheckerTracer;
			}
		}

		public static Trace StateMachineTracer
		{
			get
			{
				if (ExTraceGlobals.stateMachineTracer == null)
				{
					ExTraceGlobals.stateMachineTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.stateMachineTracer;
			}
		}

		public static Trace TruncatorTracer
		{
			get
			{
				if (ExTraceGlobals.truncatorTracer == null)
				{
					ExTraceGlobals.truncatorTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.truncatorTracer;
			}
		}

		public static Trace SnapshotTracer
		{
			get
			{
				if (ExTraceGlobals.snapshotTracer == null)
				{
					ExTraceGlobals.snapshotTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.snapshotTracer;
			}
		}

		public static Trace LocalStoreTracer
		{
			get
			{
				if (ExTraceGlobals.localStoreTracer == null)
				{
					ExTraceGlobals.localStoreTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.localStoreTracer;
			}
		}

		public static Trace UtilsTracer
		{
			get
			{
				if (ExTraceGlobals.utilsTracer == null)
				{
					ExTraceGlobals.utilsTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.utilsTracer;
			}
		}

		public static Trace ConfigTracer
		{
			get
			{
				if (ExTraceGlobals.configTracer == null)
				{
					ExTraceGlobals.configTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.configTracer;
			}
		}

		public static Trace MeshTracer
		{
			get
			{
				if (ExTraceGlobals.meshTracer == null)
				{
					ExTraceGlobals.meshTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.meshTracer;
			}
		}

		public static Trace AccessClientTracer
		{
			get
			{
				if (ExTraceGlobals.accessClientTracer == null)
				{
					ExTraceGlobals.accessClientTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.accessClientTracer;
			}
		}

		public static Trace ManagerClientTracer
		{
			get
			{
				if (ExTraceGlobals.managerClientTracer == null)
				{
					ExTraceGlobals.managerClientTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.managerClientTracer;
			}
		}

		public static Trace InstanceClientTracer
		{
			get
			{
				if (ExTraceGlobals.instanceClientTracer == null)
				{
					ExTraceGlobals.instanceClientTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.instanceClientTracer;
			}
		}

		public static Trace StoreReadTracer
		{
			get
			{
				if (ExTraceGlobals.storeReadTracer == null)
				{
					ExTraceGlobals.storeReadTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.storeReadTracer;
			}
		}

		public static Trace StoreWriteTracer
		{
			get
			{
				if (ExTraceGlobals.storeWriteTracer == null)
				{
					ExTraceGlobals.storeWriteTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.storeWriteTracer;
			}
		}

		public static Trace AccessEntryPointTracer
		{
			get
			{
				if (ExTraceGlobals.accessEntryPointTracer == null)
				{
					ExTraceGlobals.accessEntryPointTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.accessEntryPointTracer;
			}
		}

		public static Trace ManagerEntryPointTracer
		{
			get
			{
				if (ExTraceGlobals.managerEntryPointTracer == null)
				{
					ExTraceGlobals.managerEntryPointTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.managerEntryPointTracer;
			}
		}

		public static Trace InstanceEntryPointTracer
		{
			get
			{
				if (ExTraceGlobals.instanceEntryPointTracer == null)
				{
					ExTraceGlobals.instanceEntryPointTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.instanceEntryPointTracer;
			}
		}

		public static Trace RunOperationTracer
		{
			get
			{
				if (ExTraceGlobals.runOperationTracer == null)
				{
					ExTraceGlobals.runOperationTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.runOperationTracer;
			}
		}

		public static Trace CommitAckTracer
		{
			get
			{
				if (ExTraceGlobals.commitAckTracer == null)
				{
					ExTraceGlobals.commitAckTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.commitAckTracer;
			}
		}

		public static Trace EventLoggerTracer
		{
			get
			{
				if (ExTraceGlobals.eventLoggerTracer == null)
				{
					ExTraceGlobals.eventLoggerTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.eventLoggerTracer;
			}
		}

		private static Guid componentGuid = new Guid("{3C3F940E-234C-442E-A30B-A78F146F8C10}");

		private static Trace accessTracer = null;

		private static Trace managerTracer = null;

		private static Trace instanceTracer = null;

		private static Trace paxosMessageTracer = null;

		private static Trace healthCheckerTracer = null;

		private static Trace stateMachineTracer = null;

		private static Trace truncatorTracer = null;

		private static Trace snapshotTracer = null;

		private static Trace localStoreTracer = null;

		private static Trace utilsTracer = null;

		private static Trace configTracer = null;

		private static Trace meshTracer = null;

		private static Trace accessClientTracer = null;

		private static Trace managerClientTracer = null;

		private static Trace instanceClientTracer = null;

		private static Trace storeReadTracer = null;

		private static Trace storeWriteTracer = null;

		private static Trace accessEntryPointTracer = null;

		private static Trace managerEntryPointTracer = null;

		private static Trace instanceEntryPointTracer = null;

		private static Trace runOperationTracer = null;

		private static Trace commitAckTracer = null;

		private static Trace eventLoggerTracer = null;
	}
}
