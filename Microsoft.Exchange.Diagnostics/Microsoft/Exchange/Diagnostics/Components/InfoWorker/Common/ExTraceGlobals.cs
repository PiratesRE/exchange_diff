using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common
{
	public static class ExTraceGlobals
	{
		public static Trace SingleInstanceItemTracer
		{
			get
			{
				if (ExTraceGlobals.singleInstanceItemTracer == null)
				{
					ExTraceGlobals.singleInstanceItemTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.singleInstanceItemTracer;
			}
		}

		public static Trace MeetingSuggestionsTracer
		{
			get
			{
				if (ExTraceGlobals.meetingSuggestionsTracer == null)
				{
					ExTraceGlobals.meetingSuggestionsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.meetingSuggestionsTracer;
			}
		}

		public static Trace WorkingHoursTracer
		{
			get
			{
				if (ExTraceGlobals.workingHoursTracer == null)
				{
					ExTraceGlobals.workingHoursTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.workingHoursTracer;
			}
		}

		public static Trace OOFTracer
		{
			get
			{
				if (ExTraceGlobals.oOFTracer == null)
				{
					ExTraceGlobals.oOFTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.oOFTracer;
			}
		}

		public static Trace ELCTracer
		{
			get
			{
				if (ExTraceGlobals.eLCTracer == null)
				{
					ExTraceGlobals.eLCTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.eLCTracer;
			}
		}

		public static Trace ResourceBookingTracer
		{
			get
			{
				if (ExTraceGlobals.resourceBookingTracer == null)
				{
					ExTraceGlobals.resourceBookingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.resourceBookingTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace TraceContextTracer
		{
			get
			{
				if (ExTraceGlobals.traceContextTracer == null)
				{
					ExTraceGlobals.traceContextTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.traceContextTracer;
			}
		}

		public static Trace AutoTaggingTracer
		{
			get
			{
				if (ExTraceGlobals.autoTaggingTracer == null)
				{
					ExTraceGlobals.autoTaggingTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.autoTaggingTracer;
			}
		}

		public static Trace SearchTracer
		{
			get
			{
				if (ExTraceGlobals.searchTracer == null)
				{
					ExTraceGlobals.searchTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.searchTracer;
			}
		}

		public static Trace MWITracer
		{
			get
			{
				if (ExTraceGlobals.mWITracer == null)
				{
					ExTraceGlobals.mWITracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.mWITracer;
			}
		}

		public static Trace TopNTracer
		{
			get
			{
				if (ExTraceGlobals.topNTracer == null)
				{
					ExTraceGlobals.topNTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.topNTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace PublicFolderFreeBusyDataTracer
		{
			get
			{
				if (ExTraceGlobals.publicFolderFreeBusyDataTracer == null)
				{
					ExTraceGlobals.publicFolderFreeBusyDataTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.publicFolderFreeBusyDataTracer;
			}
		}

		public static Trace UserPhotosTracer
		{
			get
			{
				if (ExTraceGlobals.userPhotosTracer == null)
				{
					ExTraceGlobals.userPhotosTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.userPhotosTracer;
			}
		}

		private static Guid componentGuid = new Guid("3A8BB7C6-6298-45eb-BE95-1A3AF02F7FFA");

		private static Trace singleInstanceItemTracer = null;

		private static Trace meetingSuggestionsTracer = null;

		private static Trace workingHoursTracer = null;

		private static Trace oOFTracer = null;

		private static Trace eLCTracer = null;

		private static Trace resourceBookingTracer = null;

		private static Trace pFDTracer = null;

		private static Trace traceContextTracer = null;

		private static Trace autoTaggingTracer = null;

		private static Trace searchTracer = null;

		private static Trace mWITracer = null;

		private static Trace topNTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace publicFolderFreeBusyDataTracer = null;

		private static Trace userPhotosTracer = null;
	}
}
