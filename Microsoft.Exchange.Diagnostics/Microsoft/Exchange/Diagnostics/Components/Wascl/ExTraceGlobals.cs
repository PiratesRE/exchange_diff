using System;

namespace Microsoft.Exchange.Diagnostics.Components.Wascl
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace CoreTracer
		{
			get
			{
				if (ExTraceGlobals.coreTracer == null)
				{
					ExTraceGlobals.coreTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.coreTracer;
			}
		}

		public static Trace VerdictTracer
		{
			get
			{
				if (ExTraceGlobals.verdictTracer == null)
				{
					ExTraceGlobals.verdictTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.verdictTracer;
			}
		}

		public static Trace ExternalCallTracer
		{
			get
			{
				if (ExTraceGlobals.externalCallTracer == null)
				{
					ExTraceGlobals.externalCallTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.externalCallTracer;
			}
		}

		public static Trace APITracer
		{
			get
			{
				if (ExTraceGlobals.aPITracer == null)
				{
					ExTraceGlobals.aPITracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.aPITracer;
			}
		}

		public static Trace CryptoHelperTracer
		{
			get
			{
				if (ExTraceGlobals.cryptoHelperTracer == null)
				{
					ExTraceGlobals.cryptoHelperTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.cryptoHelperTracer;
			}
		}

		public static Trace OSETracer
		{
			get
			{
				if (ExTraceGlobals.oSETracer == null)
				{
					ExTraceGlobals.oSETracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.oSETracer;
			}
		}

		private static Guid componentGuid = new Guid("48076FB3-30FE-460B-975D-934742F529EA");

		private static Trace generalTracer = null;

		private static Trace coreTracer = null;

		private static Trace verdictTracer = null;

		private static Trace externalCallTracer = null;

		private static Trace aPITracer = null;

		private static Trace cryptoHelperTracer = null;

		private static Trace oSETracer = null;
	}
}
