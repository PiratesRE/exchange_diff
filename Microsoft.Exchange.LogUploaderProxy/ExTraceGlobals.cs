using System;
using Microsoft.Exchange.Diagnostics.Components.MessageTracingClient;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public static class ExTraceGlobals
	{
		public static Trace ParserTracer
		{
			get
			{
				if (ExTraceGlobals.parserTracer == null)
				{
					ExTraceGlobals.parserTracer = new Trace(ExTraceGlobals.ParserTracer);
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
					ExTraceGlobals.writerTracer = new Trace(ExTraceGlobals.WriterTracer);
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
					ExTraceGlobals.readerTracer = new Trace(ExTraceGlobals.ReaderTracer);
				}
				return ExTraceGlobals.readerTracer;
			}
		}

		public static Trace LogMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.logmonitorTracer == null)
				{
					ExTraceGlobals.logmonitorTracer = new Trace(ExTraceGlobals.LogMonitorTracer);
				}
				return ExTraceGlobals.logmonitorTracer;
			}
		}

		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.GeneralTracer);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace TransportQueueTracer
		{
			get
			{
				if (ExTraceGlobals.transportqueueTracer == null)
				{
					ExTraceGlobals.transportqueueTracer = new Trace(ExTraceGlobals.TransportQueueTracer);
				}
				return ExTraceGlobals.transportqueueTracer;
			}
		}

		private static Trace parserTracer;

		private static Trace writerTracer;

		private static Trace readerTracer;

		private static Trace logmonitorTracer;

		private static Trace generalTracer;

		private static Trace transportqueueTracer;
	}
}
