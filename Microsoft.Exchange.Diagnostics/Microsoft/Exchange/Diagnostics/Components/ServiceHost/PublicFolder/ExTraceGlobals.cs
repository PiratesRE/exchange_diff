using System;

namespace Microsoft.Exchange.Diagnostics.Components.ServiceHost.PublicFolder
{
	public static class ExTraceGlobals
	{
		public static Trace PublicFolderSynchronizerTracer
		{
			get
			{
				if (ExTraceGlobals.publicFolderSynchronizerTracer == null)
				{
					ExTraceGlobals.publicFolderSynchronizerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.publicFolderSynchronizerTracer;
			}
		}

		private static Guid componentGuid = new Guid("59193765-72d1-4ff4-a52d-0e3de811073a");

		private static Trace publicFolderSynchronizerTracer = null;
	}
}
