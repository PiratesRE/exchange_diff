using System;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal enum ExchangeTopologyScope
	{
		Complete,
		ServerAndSiteTopology,
		ADAndExchangeServerAndSiteTopology,
		ADAndExchangeServerAndSiteAndVirtualDirectoryTopology,
		Max = 3
	}
}
