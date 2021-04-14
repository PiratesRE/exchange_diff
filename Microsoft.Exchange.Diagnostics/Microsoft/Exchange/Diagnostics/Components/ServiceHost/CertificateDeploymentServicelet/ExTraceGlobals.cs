using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.CertificateDeploymentServicelet
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceletTracer
		{
			get
			{
				if (ExTraceGlobals.serviceletTracer == null)
				{
					ExTraceGlobals.serviceletTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceletTracer;
			}
		}

		private static Guid componentGuid = new Guid("1772CAFB-E211-4b08-89C9-CF49BDACE986");

		private static Trace serviceletTracer = null;
	}
}
