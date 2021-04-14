using System;

namespace Microsoft.Exchange.Diagnostics.Components.MapiHttpClient
{
	public static class ExTraceGlobals
	{
		public static Trace ClientAsyncOperationTracer
		{
			get
			{
				if (ExTraceGlobals.clientAsyncOperationTracer == null)
				{
					ExTraceGlobals.clientAsyncOperationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.clientAsyncOperationTracer;
			}
		}

		public static Trace ClientSessionContextTracer
		{
			get
			{
				if (ExTraceGlobals.clientSessionContextTracer == null)
				{
					ExTraceGlobals.clientSessionContextTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.clientSessionContextTracer;
			}
		}

		private static Guid componentGuid = new Guid("377C2744-C481-4C11-9B16-82C9C5E65362");

		private static Trace clientAsyncOperationTracer = null;

		private static Trace clientSessionContextTracer = null;
	}
}
