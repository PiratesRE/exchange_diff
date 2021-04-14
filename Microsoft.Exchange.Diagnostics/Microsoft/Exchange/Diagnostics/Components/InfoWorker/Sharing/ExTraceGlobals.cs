using System;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing
{
	public static class ExTraceGlobals
	{
		public static Trace SharingEngineTracer
		{
			get
			{
				if (ExTraceGlobals.sharingEngineTracer == null)
				{
					ExTraceGlobals.sharingEngineTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.sharingEngineTracer;
			}
		}

		public static Trace AppointmentTranslatorTracer
		{
			get
			{
				if (ExTraceGlobals.appointmentTranslatorTracer == null)
				{
					ExTraceGlobals.appointmentTranslatorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.appointmentTranslatorTracer;
			}
		}

		public static Trace ExchangeServiceTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeServiceTracer == null)
				{
					ExTraceGlobals.exchangeServiceTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.exchangeServiceTracer;
			}
		}

		public static Trace LocalFolderTracer
		{
			get
			{
				if (ExTraceGlobals.localFolderTracer == null)
				{
					ExTraceGlobals.localFolderTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.localFolderTracer;
			}
		}

		public static Trace SharingKeyHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.sharingKeyHandlerTracer == null)
				{
					ExTraceGlobals.sharingKeyHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.sharingKeyHandlerTracer;
			}
		}

		private static Guid componentGuid = new Guid("A15553C6-31A1-4a7a-8526-8FABE6841235");

		private static Trace sharingEngineTracer = null;

		private static Trace appointmentTranslatorTracer = null;

		private static Trace exchangeServiceTracer = null;

		private static Trace localFolderTracer = null;

		private static Trace sharingKeyHandlerTracer = null;
	}
}
