using System;

namespace Microsoft.Exchange.Transport.Common
{
	internal static class TransportMimeConstants
	{
		internal const string XMSExchangeCrossTenantOriginalSenderLatencyHeader = "X-MS-Exchange-CrossTenant-OriginalSenderLatency";

		internal const string XMSExchangeTransportFromEntityHeader = "X-MS-Exchange-Transport-FromEntityHeader";

		internal const string XMSExchangeOrganizationFromEntityHeader = "X-MS-Exchange-Organization-FromEntityHeader";

		internal const string XMSExchangeOrganizationMessageHighPrecisionLatencyInProgressHeader = "X-MS-Exchange-Organization-MessageHighPrecisionLatencyInProgress";

		internal const string XMSExchangeOrganizationMessageTreeLatencyInProgressHeader = "X-MS-Exchange-Organization-OrderedPrecisionLatencyInProgress";

		internal const string XMSExchangeOrganizationMessageLatencyHeader = "X-MS-Exchange-Organization-MessageLatency";
	}
}
