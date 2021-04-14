using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal static class Globals
	{
		internal static void Initialize()
		{
		}

		internal const string CalendarRepairEventSource = "MSExchange CalendarRepairAssistant";

		internal static readonly Trace ValidatorTracer = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.ValidatorTracer;

		internal static readonly Trace ConsistencyChecksTracer = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.ConsistencyChecksTracer;

		internal static readonly Trace CompareToAttendeeTracer = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.CompareToAttendeeTracer;

		internal static readonly Trace CompareToOrganizerTracer = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.CompareToOrganizerTracer;

		internal static readonly Trace FixupTracer = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.FixupTracer;

		internal static readonly Trace TracerPfd = Microsoft.Exchange.Diagnostics.Components.Infoworker.MeetingValidator.ExTraceGlobals.PFDTracer;

		internal static ExEventLog CalendarRepairLogger = new ExEventLog(Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common.ExTraceGlobals.SingleInstanceItemTracer.Category, "MSExchange CalendarRepairAssistant");
	}
}
