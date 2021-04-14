using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ManagedStore.AdminInterface
{
	public static class ExTraceGlobals
	{
		public static Trace AdminRpcTracer
		{
			get
			{
				if (ExTraceGlobals.adminRpcTracer == null)
				{
					ExTraceGlobals.adminRpcTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.adminRpcTracer;
			}
		}

		public static Trace MailboxSignatureTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxSignatureTracer == null)
				{
					ExTraceGlobals.mailboxSignatureTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mailboxSignatureTracer;
			}
		}

		public static Trace MultiMailboxSearchTracer
		{
			get
			{
				if (ExTraceGlobals.multiMailboxSearchTracer == null)
				{
					ExTraceGlobals.multiMailboxSearchTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.multiMailboxSearchTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		private static Guid componentGuid = new Guid("40a87a6b-f69b-4c8e-b8c9-1835d09acfe3");

		private static Trace adminRpcTracer = null;

		private static Trace mailboxSignatureTracer = null;

		private static Trace multiMailboxSearchTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
