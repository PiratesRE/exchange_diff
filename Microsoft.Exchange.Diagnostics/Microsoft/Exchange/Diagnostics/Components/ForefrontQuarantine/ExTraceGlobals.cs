using System;

namespace Microsoft.Exchange.Diagnostics.Components.ForefrontQuarantine
{
	public static class ExTraceGlobals
	{
		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		public static Trace StoreTracer
		{
			get
			{
				if (ExTraceGlobals.storeTracer == null)
				{
					ExTraceGlobals.storeTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.storeTracer;
			}
		}

		public static Trace ManagerTracer
		{
			get
			{
				if (ExTraceGlobals.managerTracer == null)
				{
					ExTraceGlobals.managerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.managerTracer;
			}
		}

		public static Trace CleanupTracer
		{
			get
			{
				if (ExTraceGlobals.cleanupTracer == null)
				{
					ExTraceGlobals.cleanupTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.cleanupTracer;
			}
		}

		public static Trace SpamDigestWSTracer
		{
			get
			{
				if (ExTraceGlobals.spamDigestWSTracer == null)
				{
					ExTraceGlobals.spamDigestWSTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.spamDigestWSTracer;
			}
		}

		public static Trace SpamDigestCommonTracer
		{
			get
			{
				if (ExTraceGlobals.spamDigestCommonTracer == null)
				{
					ExTraceGlobals.spamDigestCommonTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.spamDigestCommonTracer;
			}
		}

		public static Trace SpamDigestGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.spamDigestGeneratorTracer == null)
				{
					ExTraceGlobals.spamDigestGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.spamDigestGeneratorTracer;
			}
		}

		public static Trace SpamDigestBackgroundJobTracer
		{
			get
			{
				if (ExTraceGlobals.spamDigestBackgroundJobTracer == null)
				{
					ExTraceGlobals.spamDigestBackgroundJobTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.spamDigestBackgroundJobTracer;
			}
		}

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		private static Guid componentGuid = new Guid("10B884FD-372F-490D-A233-7C2C4CB8F104");

		private static Trace agentTracer = null;

		private static Trace storeTracer = null;

		private static Trace managerTracer = null;

		private static Trace cleanupTracer = null;

		private static Trace spamDigestWSTracer = null;

		private static Trace spamDigestCommonTracer = null;

		private static Trace spamDigestGeneratorTracer = null;

		private static Trace spamDigestBackgroundJobTracer = null;

		private static Trace commonTracer = null;
	}
}
