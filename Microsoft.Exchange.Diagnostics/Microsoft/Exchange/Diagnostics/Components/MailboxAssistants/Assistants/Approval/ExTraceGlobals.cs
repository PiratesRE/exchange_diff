using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Approval
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace CachedStateTracer
		{
			get
			{
				if (ExTraceGlobals.cachedStateTracer == null)
				{
					ExTraceGlobals.cachedStateTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.cachedStateTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace AutoGroupTracer
		{
			get
			{
				if (ExTraceGlobals.autoGroupTracer == null)
				{
					ExTraceGlobals.autoGroupTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.autoGroupTracer;
			}
		}

		public static Trace ModeratedTransportTracer
		{
			get
			{
				if (ExTraceGlobals.moderatedTransportTracer == null)
				{
					ExTraceGlobals.moderatedTransportTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.moderatedTransportTracer;
			}
		}

		private static Guid componentGuid = new Guid("B37B1146-EBE5-4078-9F5D-4B08C52F73DE");

		private static Trace generalTracer = null;

		private static Trace cachedStateTracer = null;

		private static Trace pFDTracer = null;

		private static Trace autoGroupTracer = null;

		private static Trace moderatedTransportTracer = null;
	}
}
