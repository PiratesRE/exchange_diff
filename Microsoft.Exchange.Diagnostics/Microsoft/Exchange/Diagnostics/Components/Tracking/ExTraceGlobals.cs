using System;

namespace Microsoft.Exchange.Diagnostics.Components.Tracking
{
	public static class ExTraceGlobals
	{
		public static Trace TaskTracer
		{
			get
			{
				if (ExTraceGlobals.taskTracer == null)
				{
					ExTraceGlobals.taskTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.taskTracer;
			}
		}

		public static Trace ServerStatusTracer
		{
			get
			{
				if (ExTraceGlobals.serverStatusTracer == null)
				{
					ExTraceGlobals.serverStatusTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serverStatusTracer;
			}
		}

		public static Trace LogAnalysisTracer
		{
			get
			{
				if (ExTraceGlobals.logAnalysisTracer == null)
				{
					ExTraceGlobals.logAnalysisTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.logAnalysisTracer;
			}
		}

		public static Trace SearchLibraryTracer
		{
			get
			{
				if (ExTraceGlobals.searchLibraryTracer == null)
				{
					ExTraceGlobals.searchLibraryTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.searchLibraryTracer;
			}
		}

		public static Trace WebServiceTracer
		{
			get
			{
				if (ExTraceGlobals.webServiceTracer == null)
				{
					ExTraceGlobals.webServiceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.webServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("0B7BA732-EF67-4e7c-A68F-3D8593D9DC06");

		private static Trace taskTracer = null;

		private static Trace serverStatusTracer = null;

		private static Trace logAnalysisTracer = null;

		private static Trace searchLibraryTracer = null;

		private static Trace webServiceTracer = null;
	}
}
