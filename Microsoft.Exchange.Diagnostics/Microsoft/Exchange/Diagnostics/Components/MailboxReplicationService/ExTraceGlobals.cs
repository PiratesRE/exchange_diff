using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService
{
	public static class ExTraceGlobals
	{
		public static Trace MailboxReplicationServiceTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationServiceTracer == null)
				{
					ExTraceGlobals.mailboxReplicationServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.mailboxReplicationServiceTracer;
			}
		}

		public static Trace MailboxReplicationServiceProviderTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationServiceProviderTracer == null)
				{
					ExTraceGlobals.mailboxReplicationServiceProviderTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mailboxReplicationServiceProviderTracer;
			}
		}

		public static Trace MailboxReplicationProxyClientTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationProxyClientTracer == null)
				{
					ExTraceGlobals.mailboxReplicationProxyClientTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.mailboxReplicationProxyClientTracer;
			}
		}

		public static Trace MailboxReplicationProxyServiceTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationProxyServiceTracer == null)
				{
					ExTraceGlobals.mailboxReplicationProxyServiceTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.mailboxReplicationProxyServiceTracer;
			}
		}

		public static Trace MailboxReplicationCmdletTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationCmdletTracer == null)
				{
					ExTraceGlobals.mailboxReplicationCmdletTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.mailboxReplicationCmdletTracer;
			}
		}

		public static Trace MailboxReplicationUpdateMovedMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationUpdateMovedMailboxTracer == null)
				{
					ExTraceGlobals.mailboxReplicationUpdateMovedMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.mailboxReplicationUpdateMovedMailboxTracer;
			}
		}

		public static Trace MailboxReplicationServiceThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationServiceThrottlingTracer == null)
				{
					ExTraceGlobals.mailboxReplicationServiceThrottlingTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.mailboxReplicationServiceThrottlingTracer;
			}
		}

		public static Trace MailboxReplicationAuthorizationTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationAuthorizationTracer == null)
				{
					ExTraceGlobals.mailboxReplicationAuthorizationTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.mailboxReplicationAuthorizationTracer;
			}
		}

		public static Trace MailboxReplicationCommonTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationCommonTracer == null)
				{
					ExTraceGlobals.mailboxReplicationCommonTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.mailboxReplicationCommonTracer;
			}
		}

		public static Trace MailboxReplicationResourceHealthTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReplicationResourceHealthTracer == null)
				{
					ExTraceGlobals.mailboxReplicationResourceHealthTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.mailboxReplicationResourceHealthTracer;
			}
		}

		private static Guid componentGuid = new Guid("1141b405-c15a-48ef-a440-ab2f44a9cdac");

		private static Trace mailboxReplicationServiceTracer = null;

		private static Trace mailboxReplicationServiceProviderTracer = null;

		private static Trace mailboxReplicationProxyClientTracer = null;

		private static Trace mailboxReplicationProxyServiceTracer = null;

		private static Trace mailboxReplicationCmdletTracer = null;

		private static Trace mailboxReplicationUpdateMovedMailboxTracer = null;

		private static Trace mailboxReplicationServiceThrottlingTracer = null;

		private static Trace mailboxReplicationAuthorizationTracer = null;

		private static Trace mailboxReplicationCommonTracer = null;

		private static Trace mailboxReplicationResourceHealthTracer = null;
	}
}
