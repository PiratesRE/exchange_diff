using System;

namespace Microsoft.Exchange.Diagnostics.Components.ADRecipientCache
{
	public static class ExTraceGlobals
	{
		public static Trace ADLookupTracer
		{
			get
			{
				if (ExTraceGlobals.aDLookupTracer == null)
				{
					ExTraceGlobals.aDLookupTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aDLookupTracer;
			}
		}

		public static Trace CacheLookupTracer
		{
			get
			{
				if (ExTraceGlobals.cacheLookupTracer == null)
				{
					ExTraceGlobals.cacheLookupTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.cacheLookupTracer;
			}
		}

		private static Guid componentGuid = new Guid("48868D1B-4502-4c8e-8293-E81776D01BCE");

		private static Trace aDLookupTracer = null;

		private static Trace cacheLookupTracer = null;
	}
}
