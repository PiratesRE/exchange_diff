using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	[Flags]
	internal enum CafeErrorPageValidationRules
	{
		None = 0,
		Accept401Response = 1
	}
}
