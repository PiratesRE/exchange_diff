using System;

namespace Microsoft.Exchange.Diagnostics.Components.Notifications.Broker
{
	public static class ExTraceGlobals
	{
		public static Trace ClientTracer
		{
			get
			{
				if (ExTraceGlobals.clientTracer == null)
				{
					ExTraceGlobals.clientTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.clientTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace MailboxChangeTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxChangeTracer == null)
				{
					ExTraceGlobals.mailboxChangeTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.mailboxChangeTracer;
			}
		}

		public static Trace SubscriptionsTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionsTracer == null)
				{
					ExTraceGlobals.subscriptionsTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.subscriptionsTracer;
			}
		}

		public static Trace RemoteConduitTracer
		{
			get
			{
				if (ExTraceGlobals.remoteConduitTracer == null)
				{
					ExTraceGlobals.remoteConduitTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.remoteConduitTracer;
			}
		}

		public static Trace GeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.generatorTracer == null)
				{
					ExTraceGlobals.generatorTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.generatorTracer;
			}
		}

		private static Guid componentGuid = new Guid("f16b990e-bd72-4a46-b231-b1ed417eaa17");

		private static Trace clientTracer = null;

		private static Trace serviceTracer = null;

		private static Trace mailboxChangeTracer = null;

		private static Trace subscriptionsTracer = null;

		private static Trace remoteConduitTracer = null;

		private static Trace generatorTracer = null;
	}
}
