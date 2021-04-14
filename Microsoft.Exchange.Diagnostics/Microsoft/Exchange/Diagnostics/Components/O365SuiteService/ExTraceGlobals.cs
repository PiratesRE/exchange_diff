using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.O365SuiteService
{
	public static class ExTraceGlobals
	{
		public static Trace BriefTracer
		{
			get
			{
				if (ExTraceGlobals.briefTracer == null)
				{
					ExTraceGlobals.briefTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.briefTracer;
			}
		}

		public static Trace VerboseTracer
		{
			get
			{
				if (ExTraceGlobals.verboseTracer == null)
				{
					ExTraceGlobals.verboseTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.verboseTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace ExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.exceptionTracer == null)
				{
					ExTraceGlobals.exceptionTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.exceptionTracer;
			}
		}

		private static Guid componentGuid = new Guid("AF620BE4-41C6-4931-ABD7-B83FE584538D");

		private static Trace briefTracer = null;

		private static Trace verboseTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace exceptionTracer = null;
	}
}
