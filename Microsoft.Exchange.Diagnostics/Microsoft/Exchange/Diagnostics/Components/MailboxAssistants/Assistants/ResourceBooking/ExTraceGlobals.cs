using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.ResourceBooking
{
	public static class ExTraceGlobals
	{
		public static Trace ResourceBookingAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.resourceBookingAssistantTracer == null)
				{
					ExTraceGlobals.resourceBookingAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.resourceBookingAssistantTracer;
			}
		}

		public static Trace ResourceBookingProcessingTracer
		{
			get
			{
				if (ExTraceGlobals.resourceBookingProcessingTracer == null)
				{
					ExTraceGlobals.resourceBookingProcessingTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.resourceBookingProcessingTracer;
			}
		}

		public static Trace BookingPolicyTracer
		{
			get
			{
				if (ExTraceGlobals.bookingPolicyTracer == null)
				{
					ExTraceGlobals.bookingPolicyTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.bookingPolicyTracer;
			}
		}

		public static Trace ResourceCheckTracer
		{
			get
			{
				if (ExTraceGlobals.resourceCheckTracer == null)
				{
					ExTraceGlobals.resourceCheckTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.resourceCheckTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		private static Guid componentGuid = new Guid("CAC67919-4E0C-4416-B371-9AC12F9B1AED");

		private static Trace resourceBookingAssistantTracer = null;

		private static Trace resourceBookingProcessingTracer = null;

		private static Trace bookingPolicyTracer = null;

		private static Trace resourceCheckTracer = null;

		private static Trace pFDTracer = null;
	}
}
