using System;

namespace Microsoft.Exchange.Diagnostics.Components.RulesPublisher
{
	public static class ExTraceGlobals
	{
		public static Trace RulesPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.rulesPublisherTracer == null)
				{
					ExTraceGlobals.rulesPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rulesPublisherTracer;
			}
		}

		public static Trace IPListPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.iPListPublisherTracer == null)
				{
					ExTraceGlobals.iPListPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.iPListPublisherTracer;
			}
		}

		public static Trace RulesProfilerTracer
		{
			get
			{
				if (ExTraceGlobals.rulesProfilerTracer == null)
				{
					ExTraceGlobals.rulesProfilerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.rulesProfilerTracer;
			}
		}

		public static Trace SpamDataBlobPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.spamDataBlobPublisherTracer == null)
				{
					ExTraceGlobals.spamDataBlobPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.spamDataBlobPublisherTracer;
			}
		}

		private static Guid componentGuid = new Guid("0082B730-63A3-475E-B53C-ACCA6AFDC400");

		private static Trace rulesPublisherTracer = null;

		private static Trace iPListPublisherTracer = null;

		private static Trace rulesProfilerTracer = null;

		private static Trace spamDataBlobPublisherTracer = null;
	}
}
