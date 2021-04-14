using System;

namespace Microsoft.Exchange.Diagnostics.Components.FfoCaching
{
	public static class ExTraceGlobals
	{
		public static Trace PrimingThreadTracer
		{
			get
			{
				if (ExTraceGlobals.primingThreadTracer == null)
				{
					ExTraceGlobals.primingThreadTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.primingThreadTracer;
			}
		}

		public static Trace CachingProviderTracer
		{
			get
			{
				if (ExTraceGlobals.cachingProviderTracer == null)
				{
					ExTraceGlobals.cachingProviderTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.cachingProviderTracer;
			}
		}

		public static Trace CompositeProviderTracer
		{
			get
			{
				if (ExTraceGlobals.compositeProviderTracer == null)
				{
					ExTraceGlobals.compositeProviderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.compositeProviderTracer;
			}
		}

		public static Trace PrimingStateLocalCacheTracer
		{
			get
			{
				if (ExTraceGlobals.primingStateLocalCacheTracer == null)
				{
					ExTraceGlobals.primingStateLocalCacheTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.primingStateLocalCacheTracer;
			}
		}

		private static Guid componentGuid = new Guid("880B0BC2-765E-4B89-82A0-9FFBBA7B8BE1");

		private static Trace primingThreadTracer = null;

		private static Trace cachingProviderTracer = null;

		private static Trace compositeProviderTracer = null;

		private static Trace primingStateLocalCacheTracer = null;
	}
}
