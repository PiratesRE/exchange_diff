using System;

namespace Microsoft.Exchange.Diagnostics.Components.ConnectionFiltering
{
	public static class ExTraceGlobals
	{
		public static Trace ErrorTracer
		{
			get
			{
				if (ExTraceGlobals.errorTracer == null)
				{
					ExTraceGlobals.errorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.errorTracer;
			}
		}

		public static Trace FactoryTracer
		{
			get
			{
				if (ExTraceGlobals.factoryTracer == null)
				{
					ExTraceGlobals.factoryTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.factoryTracer;
			}
		}

		public static Trace OnConnectTracer
		{
			get
			{
				if (ExTraceGlobals.onConnectTracer == null)
				{
					ExTraceGlobals.onConnectTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.onConnectTracer;
			}
		}

		public static Trace OnMailFromTracer
		{
			get
			{
				if (ExTraceGlobals.onMailFromTracer == null)
				{
					ExTraceGlobals.onMailFromTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.onMailFromTracer;
			}
		}

		public static Trace OnRcptToTracer
		{
			get
			{
				if (ExTraceGlobals.onRcptToTracer == null)
				{
					ExTraceGlobals.onRcptToTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.onRcptToTracer;
			}
		}

		public static Trace OnEOHTracer
		{
			get
			{
				if (ExTraceGlobals.onEOHTracer == null)
				{
					ExTraceGlobals.onEOHTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.onEOHTracer;
			}
		}

		public static Trace DNSTracer
		{
			get
			{
				if (ExTraceGlobals.dNSTracer == null)
				{
					ExTraceGlobals.dNSTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.dNSTracer;
			}
		}

		public static Trace IPAllowDenyTracer
		{
			get
			{
				if (ExTraceGlobals.iPAllowDenyTracer == null)
				{
					ExTraceGlobals.iPAllowDenyTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.iPAllowDenyTracer;
			}
		}

		private static Guid componentGuid = new Guid("F0A7EB4B-2EE5-478f-A589-5273CAC4E5EE");

		private static Trace errorTracer = null;

		private static Trace factoryTracer = null;

		private static Trace onConnectTracer = null;

		private static Trace onMailFromTracer = null;

		private static Trace onRcptToTracer = null;

		private static Trace onEOHTracer = null;

		private static Trace dNSTracer = null;

		private static Trace iPAllowDenyTracer = null;
	}
}
