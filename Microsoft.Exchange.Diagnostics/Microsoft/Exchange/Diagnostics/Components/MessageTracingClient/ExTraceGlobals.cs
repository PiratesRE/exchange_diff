using System;

namespace Microsoft.Exchange.Diagnostics.Components.MessageTracingClient
{
	public static class ExTraceGlobals
	{
		public static Trace ParserTracer
		{
			get
			{
				if (ExTraceGlobals.parserTracer == null)
				{
					ExTraceGlobals.parserTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.parserTracer;
			}
		}

		public static Trace WriterTracer
		{
			get
			{
				if (ExTraceGlobals.writerTracer == null)
				{
					ExTraceGlobals.writerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.writerTracer;
			}
		}

		public static Trace ReaderTracer
		{
			get
			{
				if (ExTraceGlobals.readerTracer == null)
				{
					ExTraceGlobals.readerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.readerTracer;
			}
		}

		public static Trace LogMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.logMonitorTracer == null)
				{
					ExTraceGlobals.logMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.logMonitorTracer;
			}
		}

		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace TransportQueueTracer
		{
			get
			{
				if (ExTraceGlobals.transportQueueTracer == null)
				{
					ExTraceGlobals.transportQueueTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.transportQueueTracer;
			}
		}

		private static Guid componentGuid = new Guid("0402AB9A-3D53-4353-AC55-9A9491E5A22A");

		private static Trace parserTracer = null;

		private static Trace writerTracer = null;

		private static Trace readerTracer = null;

		private static Trace logMonitorTracer = null;

		private static Trace generalTracer = null;

		private static Trace transportQueueTracer = null;
	}
}
