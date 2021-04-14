using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.DiscoverySearch
{
	public static class ExTraceGlobals
	{
		public static Trace DiscoverySearchEventBasedAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.discoverySearchEventBasedAssistantTracer == null)
				{
					ExTraceGlobals.discoverySearchEventBasedAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.discoverySearchEventBasedAssistantTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		private static Guid componentGuid = new Guid("27e7e37b-4dd9-40a7-9a27-99c623e088f3");

		private static Trace discoverySearchEventBasedAssistantTracer = null;

		private static Trace pFDTracer = null;
	}
}
