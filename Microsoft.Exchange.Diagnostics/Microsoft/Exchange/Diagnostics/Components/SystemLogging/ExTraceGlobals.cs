using System;

namespace Microsoft.Exchange.Diagnostics.Components.SystemLogging
{
	public static class ExTraceGlobals
	{
		public static Trace SystemNetTracer
		{
			get
			{
				if (ExTraceGlobals.systemNetTracer == null)
				{
					ExTraceGlobals.systemNetTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.systemNetTracer;
			}
		}

		public static Trace SystemNetSocketTracer
		{
			get
			{
				if (ExTraceGlobals.systemNetSocketTracer == null)
				{
					ExTraceGlobals.systemNetSocketTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.systemNetSocketTracer;
			}
		}

		public static Trace SystemNetHttpListenerTracer
		{
			get
			{
				if (ExTraceGlobals.systemNetHttpListenerTracer == null)
				{
					ExTraceGlobals.systemNetHttpListenerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.systemNetHttpListenerTracer;
			}
		}

		public static Trace SystemIdentityModelTraceTracer
		{
			get
			{
				if (ExTraceGlobals.systemIdentityModelTraceTracer == null)
				{
					ExTraceGlobals.systemIdentityModelTraceTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.systemIdentityModelTraceTracer;
			}
		}

		public static Trace SystemServiceModelTraceTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelTraceTracer == null)
				{
					ExTraceGlobals.systemServiceModelTraceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.systemServiceModelTraceTracer;
			}
		}

		public static Trace SystemServiceModelMessageLoggingTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelMessageLoggingTracer == null)
				{
					ExTraceGlobals.systemServiceModelMessageLoggingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.systemServiceModelMessageLoggingTracer;
			}
		}

		public static Trace SystemServiceModelMessageLogging_LogMalformedMessagesTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelMessageLogging_LogMalformedMessagesTracer == null)
				{
					ExTraceGlobals.systemServiceModelMessageLogging_LogMalformedMessagesTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.systemServiceModelMessageLogging_LogMalformedMessagesTracer;
			}
		}

		public static Trace SystemServiceModelMessageLogging_LogMessagesAtServiceLevelTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtServiceLevelTracer == null)
				{
					ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtServiceLevelTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtServiceLevelTracer;
			}
		}

		public static Trace SystemServiceModelMessageLogging_LogMessagesAtTransportLevelTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtTransportLevelTracer == null)
				{
					ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtTransportLevelTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.systemServiceModelMessageLogging_LogMessagesAtTransportLevelTracer;
			}
		}

		public static Trace SystemServiceModelMessageLogging_LogMessageBodyTracer
		{
			get
			{
				if (ExTraceGlobals.systemServiceModelMessageLogging_LogMessageBodyTracer == null)
				{
					ExTraceGlobals.systemServiceModelMessageLogging_LogMessageBodyTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.systemServiceModelMessageLogging_LogMessageBodyTracer;
			}
		}

		private static Guid componentGuid = new Guid("F21F1E57-9689-46E5-BE7D-A84C9BCE0D94");

		private static Trace systemNetTracer = null;

		private static Trace systemNetSocketTracer = null;

		private static Trace systemNetHttpListenerTracer = null;

		private static Trace systemIdentityModelTraceTracer = null;

		private static Trace systemServiceModelTraceTracer = null;

		private static Trace systemServiceModelMessageLoggingTracer = null;

		private static Trace systemServiceModelMessageLogging_LogMalformedMessagesTracer = null;

		private static Trace systemServiceModelMessageLogging_LogMessagesAtServiceLevelTracer = null;

		private static Trace systemServiceModelMessageLogging_LogMessagesAtTransportLevelTracer = null;

		private static Trace systemServiceModelMessageLogging_LogMessageBodyTracer = null;
	}
}
