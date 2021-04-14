using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.BirthdayCalendar
{
	public static class ExTraceGlobals
	{
		public static Trace BirthdayAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.birthdayAssistantTracer == null)
				{
					ExTraceGlobals.birthdayAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.birthdayAssistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("62FDC2FC-DE0F-413E-9526-2ACEDC1D3D45");

		private static Trace birthdayAssistantTracer = null;
	}
}
