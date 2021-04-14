using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.SharingPolicy
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

		public static Trace StoredSharingPolicyTracer
		{
			get
			{
				if (ExTraceGlobals.storedSharingPolicyTracer == null)
				{
					ExTraceGlobals.storedSharingPolicyTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.storedSharingPolicyTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		private static Guid componentGuid = new Guid("69F0F794-5732-43c6-A9AC-4393BEF4C477");

		private static Trace assistantTracer = null;

		private static Trace storedSharingPolicyTracer = null;

		private static Trace pFDTracer = null;
	}
}
