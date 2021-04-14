using System;

namespace Microsoft.Exchange.Diagnostics.Components.MigrationWorkflowService
{
	public static class ExTraceGlobals
	{
		public static Trace MigrationWorkflowServiceTracer
		{
			get
			{
				if (ExTraceGlobals.migrationWorkflowServiceTracer == null)
				{
					ExTraceGlobals.migrationWorkflowServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.migrationWorkflowServiceTracer;
			}
		}

		public static Trace MailboxLoadBalanceTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxLoadBalanceTracer == null)
				{
					ExTraceGlobals.mailboxLoadBalanceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mailboxLoadBalanceTracer;
			}
		}

		public static Trace InjectorServiceTracer
		{
			get
			{
				if (ExTraceGlobals.injectorServiceTracer == null)
				{
					ExTraceGlobals.injectorServiceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.injectorServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("d58c9a14-d24d-45c5-9aac-14e2678adff8");

		private static Trace migrationWorkflowServiceTracer = null;

		private static Trace mailboxLoadBalanceTracer = null;

		private static Trace injectorServiceTracer = null;
	}
}
