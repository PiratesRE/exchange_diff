using System;

namespace Microsoft.Exchange.Diagnostics.Components.StsUpdate
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

		public static Trace DatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.databaseTracer == null)
				{
					ExTraceGlobals.databaseTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.databaseTracer;
			}
		}

		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		public static Trace OnDownloadTracer
		{
			get
			{
				if (ExTraceGlobals.onDownloadTracer == null)
				{
					ExTraceGlobals.onDownloadTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.onDownloadTracer;
			}
		}

		public static Trace OnRequestTracer
		{
			get
			{
				if (ExTraceGlobals.onRequestTracer == null)
				{
					ExTraceGlobals.onRequestTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.onRequestTracer;
			}
		}

		private static Guid componentGuid = new Guid("C5F72F2A-EF44-4286-9AB2-14D106DFB8F1");

		private static Trace factoryTracer = null;

		private static Trace databaseTracer = null;

		private static Trace agentTracer = null;

		private static Trace onDownloadTracer = null;

		private static Trace onRequestTracer = null;
	}
}
