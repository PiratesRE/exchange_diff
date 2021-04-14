using System;

namespace Microsoft.Exchange.Diagnostics.Components.Monad
{
	public static class ExTraceGlobals
	{
		public static Trace DefaultTracer
		{
			get
			{
				if (ExTraceGlobals.defaultTracer == null)
				{
					ExTraceGlobals.defaultTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.defaultTracer;
			}
		}

		public static Trace IntegrationTracer
		{
			get
			{
				if (ExTraceGlobals.integrationTracer == null)
				{
					ExTraceGlobals.integrationTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.integrationTracer;
			}
		}

		public static Trace VerboseTracer
		{
			get
			{
				if (ExTraceGlobals.verboseTracer == null)
				{
					ExTraceGlobals.verboseTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.verboseTracer;
			}
		}

		public static Trace DataTracer
		{
			get
			{
				if (ExTraceGlobals.dataTracer == null)
				{
					ExTraceGlobals.dataTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.dataTracer;
			}
		}

		public static Trace HostTracer
		{
			get
			{
				if (ExTraceGlobals.hostTracer == null)
				{
					ExTraceGlobals.hostTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.hostTracer;
			}
		}

		private static Guid componentGuid = new Guid("b47bd400-78af-479f-aeff-39d4d6c54559");

		private static Trace defaultTracer = null;

		private static Trace integrationTracer = null;

		private static Trace verboseTracer = null;

		private static Trace dataTracer = null;

		private static Trace hostTracer = null;
	}
}
