using System;

namespace Microsoft.Exchange.Diagnostics.Components.CosmosProxy
{
	public static class ExTraceGlobals
	{
		public static Trace SenderTracer
		{
			get
			{
				if (ExTraceGlobals.senderTracer == null)
				{
					ExTraceGlobals.senderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.senderTracer;
			}
		}

		public static Trace ReceiverTracer
		{
			get
			{
				if (ExTraceGlobals.receiverTracer == null)
				{
					ExTraceGlobals.receiverTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.receiverTracer;
			}
		}

		public static Trace DownloaderTracer
		{
			get
			{
				if (ExTraceGlobals.downloaderTracer == null)
				{
					ExTraceGlobals.downloaderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.downloaderTracer;
			}
		}

		private static Guid componentGuid = new Guid("18229B60-53AF-4337-9F63-BACE4AB588AD");

		private static Trace senderTracer = null;

		private static Trace receiverTracer = null;

		private static Trace downloaderTracer = null;
	}
}
