using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability
{
	public static class ExTraceGlobals
	{
		public static Trace InitializeTracer
		{
			get
			{
				if (ExTraceGlobals.initializeTracer == null)
				{
					ExTraceGlobals.initializeTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.initializeTracer;
			}
		}

		public static Trace SecurityTracer
		{
			get
			{
				if (ExTraceGlobals.securityTracer == null)
				{
					ExTraceGlobals.securityTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.securityTracer;
			}
		}

		public static Trace CalendarViewTracer
		{
			get
			{
				if (ExTraceGlobals.calendarViewTracer == null)
				{
					ExTraceGlobals.calendarViewTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.calendarViewTracer;
			}
		}

		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace PublicFolderRequestTracer
		{
			get
			{
				if (ExTraceGlobals.publicFolderRequestTracer == null)
				{
					ExTraceGlobals.publicFolderRequestTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.publicFolderRequestTracer;
			}
		}

		public static Trace IntraSiteCalendarRequestTracer
		{
			get
			{
				if (ExTraceGlobals.intraSiteCalendarRequestTracer == null)
				{
					ExTraceGlobals.intraSiteCalendarRequestTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.intraSiteCalendarRequestTracer;
			}
		}

		public static Trace MeetingSuggestionsTracer
		{
			get
			{
				if (ExTraceGlobals.meetingSuggestionsTracer == null)
				{
					ExTraceGlobals.meetingSuggestionsTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.meetingSuggestionsTracer;
			}
		}

		public static Trace AutoDiscoverTracer
		{
			get
			{
				if (ExTraceGlobals.autoDiscoverTracer == null)
				{
					ExTraceGlobals.autoDiscoverTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.autoDiscoverTracer;
			}
		}

		public static Trace MailboxConnectionCacheTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxConnectionCacheTracer == null)
				{
					ExTraceGlobals.mailboxConnectionCacheTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.mailboxConnectionCacheTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace DnsReaderTracer
		{
			get
			{
				if (ExTraceGlobals.dnsReaderTracer == null)
				{
					ExTraceGlobals.dnsReaderTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.dnsReaderTracer;
			}
		}

		public static Trace MessageTracer
		{
			get
			{
				if (ExTraceGlobals.messageTracer == null)
				{
					ExTraceGlobals.messageTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.messageTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("A7F9AB97-3B1B-4e10-B58F-E58136B9DA0A");

		private static Trace initializeTracer = null;

		private static Trace securityTracer = null;

		private static Trace calendarViewTracer = null;

		private static Trace configurationTracer = null;

		private static Trace publicFolderRequestTracer = null;

		private static Trace intraSiteCalendarRequestTracer = null;

		private static Trace meetingSuggestionsTracer = null;

		private static Trace autoDiscoverTracer = null;

		private static Trace mailboxConnectionCacheTracer = null;

		private static Trace pFDTracer = null;

		private static Trace dnsReaderTracer = null;

		private static Trace messageTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
