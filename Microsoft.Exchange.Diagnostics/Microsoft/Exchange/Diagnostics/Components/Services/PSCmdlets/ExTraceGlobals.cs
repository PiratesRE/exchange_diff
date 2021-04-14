using System;

namespace Microsoft.Exchange.Diagnostics.Components.Services.PSCmdlets
{
	public static class ExTraceGlobals
	{
		public static Trace MessageInspectorTracer
		{
			get
			{
				if (ExTraceGlobals.messageInspectorTracer == null)
				{
					ExTraceGlobals.messageInspectorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.messageInspectorTracer;
			}
		}

		public static Trace KnownExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.knownExceptionTracer == null)
				{
					ExTraceGlobals.knownExceptionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.knownExceptionTracer;
			}
		}

		public static Trace PowerShellTracer
		{
			get
			{
				if (ExTraceGlobals.powerShellTracer == null)
				{
					ExTraceGlobals.powerShellTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.powerShellTracer;
			}
		}

		public static Trace RbacAuthorizationTracer
		{
			get
			{
				if (ExTraceGlobals.rbacAuthorizationTracer == null)
				{
					ExTraceGlobals.rbacAuthorizationTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.rbacAuthorizationTracer;
			}
		}

		private static Guid componentGuid = new Guid("0df9c122-5f11-416d-9ed1-7b6dd48beb8e");

		private static Trace messageInspectorTracer = null;

		private static Trace knownExceptionTracer = null;

		private static Trace powerShellTracer = null;

		private static Trace rbacAuthorizationTracer = null;
	}
}
