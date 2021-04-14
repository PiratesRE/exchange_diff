using System;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.MultiMailboxSearch
{
	public static class ExTraceGlobals
	{
		public static Trace LocalSearchTracer
		{
			get
			{
				if (ExTraceGlobals.localSearchTracer == null)
				{
					ExTraceGlobals.localSearchTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.localSearchTracer;
			}
		}

		public static Trace MailboxGroupGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxGroupGeneratorTracer == null)
				{
					ExTraceGlobals.mailboxGroupGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mailboxGroupGeneratorTracer;
			}
		}

		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace AutoDiscoverTracer
		{
			get
			{
				if (ExTraceGlobals.autoDiscoverTracer == null)
				{
					ExTraceGlobals.autoDiscoverTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.autoDiscoverTracer;
			}
		}

		private static Guid componentGuid = new Guid("6a7f7e5b-18a1-4e29-b0c0-2514adb49e41");

		private static Trace localSearchTracer = null;

		private static Trace mailboxGroupGeneratorTracer = null;

		private static Trace generalTracer = null;

		private static Trace autoDiscoverTracer = null;
	}
}
