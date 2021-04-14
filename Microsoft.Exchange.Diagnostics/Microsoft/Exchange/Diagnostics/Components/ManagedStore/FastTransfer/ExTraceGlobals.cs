using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer
{
	public static class ExTraceGlobals
	{
		public static Trace SourceSendTracer
		{
			get
			{
				if (ExTraceGlobals.sourceSendTracer == null)
				{
					ExTraceGlobals.sourceSendTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.sourceSendTracer;
			}
		}

		public static Trace IcsDownloadTracer
		{
			get
			{
				if (ExTraceGlobals.icsDownloadTracer == null)
				{
					ExTraceGlobals.icsDownloadTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.icsDownloadTracer;
			}
		}

		public static Trace IcsDownloadStateTracer
		{
			get
			{
				if (ExTraceGlobals.icsDownloadStateTracer == null)
				{
					ExTraceGlobals.icsDownloadStateTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.icsDownloadStateTracer;
			}
		}

		public static Trace IcsUploadStateTracer
		{
			get
			{
				if (ExTraceGlobals.icsUploadStateTracer == null)
				{
					ExTraceGlobals.icsUploadStateTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.icsUploadStateTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("e8d090ac-ab71-4752-b432-0b86b6e380e6");

		private static Trace sourceSendTracer = null;

		private static Trace icsDownloadTracer = null;

		private static Trace icsDownloadStateTracer = null;

		private static Trace icsUploadStateTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
