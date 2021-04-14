using System;

namespace Microsoft.Exchange.Diagnostics.Components.GenericRus
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

		public static Trace RusClientTracer
		{
			get
			{
				if (ExTraceGlobals.rusClientTracer == null)
				{
					ExTraceGlobals.rusClientTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.rusClientTracer;
			}
		}

		public static Trace RusServiceTracer
		{
			get
			{
				if (ExTraceGlobals.rusServiceTracer == null)
				{
					ExTraceGlobals.rusServiceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.rusServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("F6193284-D059-4D1B-AB4B-C2A778A8BAB9");

		private static Trace commonTracer = null;

		private static Trace rusClientTracer = null;

		private static Trace rusServiceTracer = null;
	}
}
