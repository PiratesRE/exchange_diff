using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.DirectoryProcessorAssistant.GroupMetricsGenerator
{
	public static class ExTraceGlobals
	{
		public static Trace GroupMetricsTracer
		{
			get
			{
				if (ExTraceGlobals.groupMetricsTracer == null)
				{
					ExTraceGlobals.groupMetricsTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.groupMetricsTracer;
			}
		}

		private static Guid componentGuid = new Guid("a968ca69-504c-4430-bef4-84f9480d8937");

		private static Trace groupMetricsTracer = null;
	}
}
