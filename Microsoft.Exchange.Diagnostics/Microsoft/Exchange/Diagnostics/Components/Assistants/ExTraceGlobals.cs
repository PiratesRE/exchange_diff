using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Assistants
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantsRpcServerTracer
		{
			get
			{
				if (ExTraceGlobals.assistantsRpcServerTracer == null)
				{
					ExTraceGlobals.assistantsRpcServerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantsRpcServerTracer;
			}
		}

		public static Trace DatabaseInfoTracer
		{
			get
			{
				if (ExTraceGlobals.databaseInfoTracer == null)
				{
					ExTraceGlobals.databaseInfoTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.databaseInfoTracer;
			}
		}

		public static Trace DatabaseManagerTracer
		{
			get
			{
				if (ExTraceGlobals.databaseManagerTracer == null)
				{
					ExTraceGlobals.databaseManagerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.databaseManagerTracer;
			}
		}

		public static Trace ErrorHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.errorHandlerTracer == null)
				{
					ExTraceGlobals.errorHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.errorHandlerTracer;
			}
		}

		public static Trace EventAccessTracer
		{
			get
			{
				if (ExTraceGlobals.eventAccessTracer == null)
				{
					ExTraceGlobals.eventAccessTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.eventAccessTracer;
			}
		}

		public static Trace EventControllerTracer
		{
			get
			{
				if (ExTraceGlobals.eventControllerTracer == null)
				{
					ExTraceGlobals.eventControllerTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.eventControllerTracer;
			}
		}

		public static Trace EventDispatcherTracer
		{
			get
			{
				if (ExTraceGlobals.eventDispatcherTracer == null)
				{
					ExTraceGlobals.eventDispatcherTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.eventDispatcherTracer;
			}
		}

		public static Trace EventBasedAssistantCollectionTracer
		{
			get
			{
				if (ExTraceGlobals.eventBasedAssistantCollectionTracer == null)
				{
					ExTraceGlobals.eventBasedAssistantCollectionTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.eventBasedAssistantCollectionTracer;
			}
		}

		public static Trace TimeBasedAssistantControllerTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedAssistantControllerTracer == null)
				{
					ExTraceGlobals.timeBasedAssistantControllerTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.timeBasedAssistantControllerTracer;
			}
		}

		public static Trace TimeBasedDatabaseDriverTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedDatabaseDriverTracer == null)
				{
					ExTraceGlobals.timeBasedDatabaseDriverTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.timeBasedDatabaseDriverTracer;
			}
		}

		public static Trace TimeBasedDatabaseJobTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedDatabaseJobTracer == null)
				{
					ExTraceGlobals.timeBasedDatabaseJobTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.timeBasedDatabaseJobTracer;
			}
		}

		public static Trace TimeBasedDatabaseWindowJobTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedDatabaseWindowJobTracer == null)
				{
					ExTraceGlobals.timeBasedDatabaseWindowJobTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.timeBasedDatabaseWindowJobTracer;
			}
		}

		public static Trace TimeBasedDatabaseDemandJobTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedDatabaseDemandJobTracer == null)
				{
					ExTraceGlobals.timeBasedDatabaseDemandJobTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.timeBasedDatabaseDemandJobTracer;
			}
		}

		public static Trace TimeBasedDriverManagerTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedDriverManagerTracer == null)
				{
					ExTraceGlobals.timeBasedDriverManagerTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.timeBasedDriverManagerTracer;
			}
		}

		public static Trace OnlineDatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.onlineDatabaseTracer == null)
				{
					ExTraceGlobals.onlineDatabaseTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.onlineDatabaseTracer;
			}
		}

		public static Trace PoisonControlTracer
		{
			get
			{
				if (ExTraceGlobals.poisonControlTracer == null)
				{
					ExTraceGlobals.poisonControlTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.poisonControlTracer;
			}
		}

		public static Trace ThrottleTracer
		{
			get
			{
				if (ExTraceGlobals.throttleTracer == null)
				{
					ExTraceGlobals.throttleTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.throttleTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace GovernorTracer
		{
			get
			{
				if (ExTraceGlobals.governorTracer == null)
				{
					ExTraceGlobals.governorTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.governorTracer;
			}
		}

		public static Trace QueueProcessorTracer
		{
			get
			{
				if (ExTraceGlobals.queueProcessorTracer == null)
				{
					ExTraceGlobals.queueProcessorTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.queueProcessorTracer;
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

		public static Trace ProbeTimeBasedAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.probeTimeBasedAssistantTracer == null)
				{
					ExTraceGlobals.probeTimeBasedAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.probeTimeBasedAssistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("EDC33045-05FB-4abb-A608-AEE572BC3C5F");

		private static Trace assistantsRpcServerTracer = null;

		private static Trace databaseInfoTracer = null;

		private static Trace databaseManagerTracer = null;

		private static Trace errorHandlerTracer = null;

		private static Trace eventAccessTracer = null;

		private static Trace eventControllerTracer = null;

		private static Trace eventDispatcherTracer = null;

		private static Trace eventBasedAssistantCollectionTracer = null;

		private static Trace timeBasedAssistantControllerTracer = null;

		private static Trace timeBasedDatabaseDriverTracer = null;

		private static Trace timeBasedDatabaseJobTracer = null;

		private static Trace timeBasedDatabaseWindowJobTracer = null;

		private static Trace timeBasedDatabaseDemandJobTracer = null;

		private static Trace timeBasedDriverManagerTracer = null;

		private static Trace onlineDatabaseTracer = null;

		private static Trace poisonControlTracer = null;

		private static Trace throttleTracer = null;

		private static Trace pFDTracer = null;

		private static Trace governorTracer = null;

		private static Trace queueProcessorTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace probeTimeBasedAssistantTracer = null;
	}
}
