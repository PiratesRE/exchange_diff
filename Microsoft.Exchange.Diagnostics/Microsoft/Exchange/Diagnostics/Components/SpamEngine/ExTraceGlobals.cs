using System;

namespace Microsoft.Exchange.Diagnostics.Components.SpamEngine
{
	public static class ExTraceGlobals
	{
		public static Trace RulesEngineTracer
		{
			get
			{
				if (ExTraceGlobals.rulesEngineTracer == null)
				{
					ExTraceGlobals.rulesEngineTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.rulesEngineTracer;
			}
		}

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace BackScatterTracer
		{
			get
			{
				if (ExTraceGlobals.backScatterTracer == null)
				{
					ExTraceGlobals.backScatterTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.backScatterTracer;
			}
		}

		public static Trace SenderAuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.senderAuthenticationTracer == null)
				{
					ExTraceGlobals.senderAuthenticationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.senderAuthenticationTracer;
			}
		}

		public static Trace UriScanTracer
		{
			get
			{
				if (ExTraceGlobals.uriScanTracer == null)
				{
					ExTraceGlobals.uriScanTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.uriScanTracer;
			}
		}

		public static Trace DnsChecksTracer
		{
			get
			{
				if (ExTraceGlobals.dnsChecksTracer == null)
				{
					ExTraceGlobals.dnsChecksTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.dnsChecksTracer;
			}
		}

		public static Trace DkimTracer
		{
			get
			{
				if (ExTraceGlobals.dkimTracer == null)
				{
					ExTraceGlobals.dkimTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.dkimTracer;
			}
		}

		public static Trace DmarcTracer
		{
			get
			{
				if (ExTraceGlobals.dmarcTracer == null)
				{
					ExTraceGlobals.dmarcTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.dmarcTracer;
			}
		}

		private static Guid componentGuid = new Guid("D47F7E4B-8F27-43fa-9BE6-DDF69C7AE225");

		private static Trace rulesEngineTracer = null;

		private static Trace commonTracer = null;

		private static Trace backScatterTracer = null;

		private static Trace senderAuthenticationTracer = null;

		private static Trace uriScanTracer = null;

		private static Trace dnsChecksTracer = null;

		private static Trace dkimTracer = null;

		private static Trace dmarcTracer = null;
	}
}
