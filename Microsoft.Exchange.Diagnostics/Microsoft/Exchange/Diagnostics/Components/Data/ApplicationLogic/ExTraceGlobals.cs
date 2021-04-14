using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace FreeBusyTracer
		{
			get
			{
				if (ExTraceGlobals.freeBusyTracer == null)
				{
					ExTraceGlobals.freeBusyTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.freeBusyTracer;
			}
		}

		public static Trace ExtensionTracer
		{
			get
			{
				if (ExTraceGlobals.extensionTracer == null)
				{
					ExTraceGlobals.extensionTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.extensionTracer;
			}
		}

		public static Trace PeopleConnectConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.peopleConnectConfigurationTracer == null)
				{
					ExTraceGlobals.peopleConnectConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.peopleConnectConfigurationTracer;
			}
		}

		public static Trace MailboxFileStoreTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxFileStoreTracer == null)
				{
					ExTraceGlobals.mailboxFileStoreTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.mailboxFileStoreTracer;
			}
		}

		public static Trace CafeTracer
		{
			get
			{
				if (ExTraceGlobals.cafeTracer == null)
				{
					ExTraceGlobals.cafeTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.cafeTracer;
			}
		}

		public static Trace DiagnosticHandlersTracer
		{
			get
			{
				if (ExTraceGlobals.diagnosticHandlersTracer == null)
				{
					ExTraceGlobals.diagnosticHandlersTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.diagnosticHandlersTracer;
			}
		}

		public static Trace E4ETracer
		{
			get
			{
				if (ExTraceGlobals.e4ETracer == null)
				{
					ExTraceGlobals.e4ETracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.e4ETracer;
			}
		}

		public static Trace SyncCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.syncCalendarTracer == null)
				{
					ExTraceGlobals.syncCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.syncCalendarTracer;
			}
		}

		private static Guid componentGuid = new Guid("A9F57445-AB0E-47ff-90F3-9593E8D23B6F");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace freeBusyTracer = null;

		private static Trace extensionTracer = null;

		private static Trace peopleConnectConfigurationTracer = null;

		private static Trace mailboxFileStoreTracer = null;

		private static Trace cafeTracer = null;

		private static Trace diagnosticHandlersTracer = null;

		private static Trace e4ETracer = null;

		private static Trace syncCalendarTracer = null;
	}
}
