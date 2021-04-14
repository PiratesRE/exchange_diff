using System;

namespace Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator
{
	public static class ExTraceGlobals
	{
		public static Trace ValidatorTracer
		{
			get
			{
				if (ExTraceGlobals.validatorTracer == null)
				{
					ExTraceGlobals.validatorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.validatorTracer;
			}
		}

		public static Trace ConsistencyChecksTracer
		{
			get
			{
				if (ExTraceGlobals.consistencyChecksTracer == null)
				{
					ExTraceGlobals.consistencyChecksTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.consistencyChecksTracer;
			}
		}

		public static Trace CompareToAttendeeTracer
		{
			get
			{
				if (ExTraceGlobals.compareToAttendeeTracer == null)
				{
					ExTraceGlobals.compareToAttendeeTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.compareToAttendeeTracer;
			}
		}

		public static Trace CompareToOrganizerTracer
		{
			get
			{
				if (ExTraceGlobals.compareToOrganizerTracer == null)
				{
					ExTraceGlobals.compareToOrganizerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.compareToOrganizerTracer;
			}
		}

		public static Trace FixupTracer
		{
			get
			{
				if (ExTraceGlobals.fixupTracer == null)
				{
					ExTraceGlobals.fixupTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.fixupTracer;
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

		private static Guid componentGuid = new Guid("7CCC3078-AE21-4CF6-B241-3EE7A8439681");

		private static Trace validatorTracer = null;

		private static Trace consistencyChecksTracer = null;

		private static Trace compareToAttendeeTracer = null;

		private static Trace compareToOrganizerTracer = null;

		private static Trace fixupTracer = null;

		private static Trace pFDTracer = null;
	}
}
