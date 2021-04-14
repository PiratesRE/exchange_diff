using System;

namespace Microsoft.Exchange.Diagnostics.Components.IPListGenerators
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace IPListGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.iPListGeneratorTracer == null)
				{
					ExTraceGlobals.iPListGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.iPListGeneratorTracer;
			}
		}

		public static Trace RWBLListGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.rWBLListGeneratorTracer == null)
				{
					ExTraceGlobals.rWBLListGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.rWBLListGeneratorTracer;
			}
		}

		public static Trace TBLListGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.tBLListGeneratorTracer == null)
				{
					ExTraceGlobals.tBLListGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.tBLListGeneratorTracer;
			}
		}

		private static Guid componentGuid = new Guid("4A1C4EB6-CEAC-42f3-A708-3FF1536B0DD7");

		private static Trace commonTracer = null;

		private static Trace iPListGeneratorTracer = null;

		private static Trace rWBLListGeneratorTracer = null;

		private static Trace tBLListGeneratorTracer = null;
	}
}
