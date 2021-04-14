using System;

namespace Microsoft.Exchange.Diagnostics.Components.TextProcessing
{
	public static class ExTraceGlobals
	{
		public static Trace SmartTrieTracer
		{
			get
			{
				if (ExTraceGlobals.smartTrieTracer == null)
				{
					ExTraceGlobals.smartTrieTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.smartTrieTracer;
			}
		}

		public static Trace MatcherTracer
		{
			get
			{
				if (ExTraceGlobals.matcherTracer == null)
				{
					ExTraceGlobals.matcherTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.matcherTracer;
			}
		}

		public static Trace FingerprintTracer
		{
			get
			{
				if (ExTraceGlobals.fingerprintTracer == null)
				{
					ExTraceGlobals.fingerprintTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.fingerprintTracer;
			}
		}

		public static Trace BoomerangTracer
		{
			get
			{
				if (ExTraceGlobals.boomerangTracer == null)
				{
					ExTraceGlobals.boomerangTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.boomerangTracer;
			}
		}

		private static Guid componentGuid = new Guid("B15C3C00-9FF8-47B7-A975-70F1278017EF");

		private static Trace smartTrieTracer = null;

		private static Trace matcherTracer = null;

		private static Trace fingerprintTracer = null;

		private static Trace boomerangTracer = null;
	}
}
