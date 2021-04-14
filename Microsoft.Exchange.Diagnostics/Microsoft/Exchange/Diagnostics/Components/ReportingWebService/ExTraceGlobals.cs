using System;

namespace Microsoft.Exchange.Diagnostics.Components.ReportingWebService
{
	public static class ExTraceGlobals
	{
		public static Trace ReportingWebServiceTracer
		{
			get
			{
				if (ExTraceGlobals.reportingWebServiceTracer == null)
				{
					ExTraceGlobals.reportingWebServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.reportingWebServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("E1C7EC5C-4B42-427A-9033-BC062E34DD7F");

		private static Trace reportingWebServiceTracer = null;
	}
}
