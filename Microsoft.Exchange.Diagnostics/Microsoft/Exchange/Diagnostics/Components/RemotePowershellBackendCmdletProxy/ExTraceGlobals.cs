using System;

namespace Microsoft.Exchange.Diagnostics.Components.RemotePowershellBackendCmdletProxy
{
	public static class ExTraceGlobals
	{
		public static Trace RemotePowershellBackendCmdletProxyModuleTracer
		{
			get
			{
				if (ExTraceGlobals.remotePowershellBackendCmdletProxyModuleTracer == null)
				{
					ExTraceGlobals.remotePowershellBackendCmdletProxyModuleTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.remotePowershellBackendCmdletProxyModuleTracer;
			}
		}

		private static Guid componentGuid = new Guid("A4E8E535-4D59-49CC-B92D-4598367E5B35");

		private static Trace remotePowershellBackendCmdletProxyModuleTracer = null;
	}
}
