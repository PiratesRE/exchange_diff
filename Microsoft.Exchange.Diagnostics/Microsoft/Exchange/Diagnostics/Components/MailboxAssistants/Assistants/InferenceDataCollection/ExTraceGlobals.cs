using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.InferenceDataCollection
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		private static Guid componentGuid = new Guid("C36CD9F9-4104-4C85-B1FD-2B432FEE369C");

		private static Trace generalTracer = null;
	}
}
