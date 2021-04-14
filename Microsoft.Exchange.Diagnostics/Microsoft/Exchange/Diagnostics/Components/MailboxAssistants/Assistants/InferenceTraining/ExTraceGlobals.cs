using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.InferenceTraining
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantTracer
		{
			get
			{
				if (ExTraceGlobals.assistantTracer == null)
				{
					ExTraceGlobals.assistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("FB0FDD38-E81F-49B7-AEF0-80A9433ED4C2");

		private static Trace assistantTracer = null;
	}
}
