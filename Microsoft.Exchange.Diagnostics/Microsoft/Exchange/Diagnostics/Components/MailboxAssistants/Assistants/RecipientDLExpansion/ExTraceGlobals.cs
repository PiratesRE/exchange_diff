using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.RecipientDLExpansion
{
	public static class ExTraceGlobals
	{
		public static Trace RecipientDLExpansionEventBasedAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.recipientDLExpansionEventBasedAssistantTracer == null)
				{
					ExTraceGlobals.recipientDLExpansionEventBasedAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.recipientDLExpansionEventBasedAssistantTracer;
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

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("06B6CA05-D4A0-4C19-B79E-24695A8E212D");

		private static Trace recipientDLExpansionEventBasedAssistantTracer = null;

		private static Trace pFDTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
