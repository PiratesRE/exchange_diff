using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	internal static class AvailabilityGlobals
	{
		public static readonly Guid ComponentGuid = new Guid("{A12F4C36-83F1-4142-BE14-09FE7E782E16}");

		public static readonly ExEventLog Logger = new ExEventLog(AvailabilityGlobals.ComponentGuid, "MSExchange Availability");

		private static readonly Trace InitializeTracer = ExTraceGlobals.InitializeTracer;
	}
}
