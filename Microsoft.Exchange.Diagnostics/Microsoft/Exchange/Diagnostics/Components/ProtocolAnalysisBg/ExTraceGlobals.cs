using System;

namespace Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysisBg
{
	public static class ExTraceGlobals
	{
		public static Trace FactoryTracer
		{
			get
			{
				if (ExTraceGlobals.factoryTracer == null)
				{
					ExTraceGlobals.factoryTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.factoryTracer;
			}
		}

		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		public static Trace DatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.databaseTracer == null)
				{
					ExTraceGlobals.databaseTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.databaseTracer;
			}
		}

		public static Trace OnOpenProxyDetectionTracer
		{
			get
			{
				if (ExTraceGlobals.onOpenProxyDetectionTracer == null)
				{
					ExTraceGlobals.onOpenProxyDetectionTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.onOpenProxyDetectionTracer;
			}
		}

		public static Trace OnDnsQueryTracer
		{
			get
			{
				if (ExTraceGlobals.onDnsQueryTracer == null)
				{
					ExTraceGlobals.onDnsQueryTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.onDnsQueryTracer;
			}
		}

		public static Trace OnSenderBlockingTracer
		{
			get
			{
				if (ExTraceGlobals.onSenderBlockingTracer == null)
				{
					ExTraceGlobals.onSenderBlockingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.onSenderBlockingTracer;
			}
		}

		private static Guid componentGuid = new Guid("E30C077B-EBAD-4AC8-B2BF-197C3329952F");

		private static Trace factoryTracer = null;

		private static Trace agentTracer = null;

		private static Trace databaseTracer = null;

		private static Trace onOpenProxyDetectionTracer = null;

		private static Trace onDnsQueryTracer = null;

		private static Trace onSenderBlockingTracer = null;
	}
}
