using System;

namespace Microsoft.Exchange.Diagnostics.Components.SecurityCommon
{
	public static class ExTraceGlobals
	{
		public static Trace CertificateEnrollmentTracer
		{
			get
			{
				if (ExTraceGlobals.certificateEnrollmentTracer == null)
				{
					ExTraceGlobals.certificateEnrollmentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.certificateEnrollmentTracer;
			}
		}

		private static Guid componentGuid = new Guid("10DD2D62-2034-4F06-AD46-CCA14D1F30CE");

		private static Trace certificateEnrollmentTracer = null;
	}
}
