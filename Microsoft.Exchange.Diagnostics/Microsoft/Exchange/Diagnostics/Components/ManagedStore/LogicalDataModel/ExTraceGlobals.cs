using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel
{
	public static class ExTraceGlobals
	{
		public static Trace FolderTracer
		{
			get
			{
				if (ExTraceGlobals.folderTracer == null)
				{
					ExTraceGlobals.folderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.folderTracer;
			}
		}

		public static Trace EventsTracer
		{
			get
			{
				if (ExTraceGlobals.eventsTracer == null)
				{
					ExTraceGlobals.eventsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.eventsTracer;
			}
		}

		public static Trace ConversationsSummaryTracer
		{
			get
			{
				if (ExTraceGlobals.conversationsSummaryTracer == null)
				{
					ExTraceGlobals.conversationsSummaryTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.conversationsSummaryTracer;
			}
		}

		public static Trace ConversationsDetailedTracer
		{
			get
			{
				if (ExTraceGlobals.conversationsDetailedTracer == null)
				{
					ExTraceGlobals.conversationsDetailedTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.conversationsDetailedTracer;
			}
		}

		public static Trace SearchFolderSearchCriteriaTracer
		{
			get
			{
				if (ExTraceGlobals.searchFolderSearchCriteriaTracer == null)
				{
					ExTraceGlobals.searchFolderSearchCriteriaTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.searchFolderSearchCriteriaTracer;
			}
		}

		public static Trace SearchFolderPopulationTracer
		{
			get
			{
				if (ExTraceGlobals.searchFolderPopulationTracer == null)
				{
					ExTraceGlobals.searchFolderPopulationTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.searchFolderPopulationTracer;
			}
		}

		public static Trace CategorizationsTracer
		{
			get
			{
				if (ExTraceGlobals.categorizationsTracer == null)
				{
					ExTraceGlobals.categorizationsTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.categorizationsTracer;
			}
		}

		public static Trace GetViewsPropertiesTracer
		{
			get
			{
				if (ExTraceGlobals.getViewsPropertiesTracer == null)
				{
					ExTraceGlobals.getViewsPropertiesTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.getViewsPropertiesTracer;
			}
		}

		public static Trace DatabaseSizeCheckTracer
		{
			get
			{
				if (ExTraceGlobals.databaseSizeCheckTracer == null)
				{
					ExTraceGlobals.databaseSizeCheckTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.databaseSizeCheckTracer;
			}
		}

		public static Trace SearchFolderAgeOutTracer
		{
			get
			{
				if (ExTraceGlobals.searchFolderAgeOutTracer == null)
				{
					ExTraceGlobals.searchFolderAgeOutTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.searchFolderAgeOutTracer;
			}
		}

		public static Trace ReadEventsTracer
		{
			get
			{
				if (ExTraceGlobals.readEventsTracer == null)
				{
					ExTraceGlobals.readEventsTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.readEventsTracer;
			}
		}

		public static Trace EventCounterBoundsTracer
		{
			get
			{
				if (ExTraceGlobals.eventCounterBoundsTracer == null)
				{
					ExTraceGlobals.eventCounterBoundsTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.eventCounterBoundsTracer;
			}
		}

		public static Trace QuotaTracer
		{
			get
			{
				if (ExTraceGlobals.quotaTracer == null)
				{
					ExTraceGlobals.quotaTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.quotaTracer;
			}
		}

		public static Trace SubobjectCleanupTracer
		{
			get
			{
				if (ExTraceGlobals.subobjectCleanupTracer == null)
				{
					ExTraceGlobals.subobjectCleanupTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.subobjectCleanupTracer;
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

		private static Guid componentGuid = new Guid("702edbba-c134-43b8-b01d-6aed04823af3");

		private static Trace folderTracer = null;

		private static Trace eventsTracer = null;

		private static Trace conversationsSummaryTracer = null;

		private static Trace conversationsDetailedTracer = null;

		private static Trace searchFolderSearchCriteriaTracer = null;

		private static Trace searchFolderPopulationTracer = null;

		private static Trace categorizationsTracer = null;

		private static Trace getViewsPropertiesTracer = null;

		private static Trace databaseSizeCheckTracer = null;

		private static Trace searchFolderAgeOutTracer = null;

		private static Trace readEventsTracer = null;

		private static Trace eventCounterBoundsTracer = null;

		private static Trace quotaTracer = null;

		private static Trace subobjectCleanupTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
