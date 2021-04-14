using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueReportSchema
	{
		internal static readonly HygienePropertyDefinition ReportProperty = new HygienePropertyDefinition("AsyncQueueReport", typeof(string));

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition ProcessStartDatetimeProperty = AsyncQueueLogSchema.ProcessStartDatetimeProperty;

		internal static readonly HygienePropertyDefinition ProcessEndDatetimeProperty = AsyncQueueLogSchema.ProcessEndDatetimeProperty;
	}
}
