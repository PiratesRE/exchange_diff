using System;

namespace Microsoft.Exchange.Diagnostics.Components.SubmissionService
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace StoreDriverSubmissionTracer
		{
			get
			{
				if (ExTraceGlobals.storeDriverSubmissionTracer == null)
				{
					ExTraceGlobals.storeDriverSubmissionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.storeDriverSubmissionTracer;
			}
		}

		private static Guid componentGuid = new Guid("ef777296-2ff4-4617-8abd-5490cfb2d5c6");

		private static Trace serviceTracer = null;

		private static Trace storeDriverSubmissionTracer = null;
	}
}
