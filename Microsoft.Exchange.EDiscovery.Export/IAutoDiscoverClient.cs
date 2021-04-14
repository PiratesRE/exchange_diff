using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IAutoDiscoverClient
	{
		List<AutoDiscoverResult> GetUserEwsEndpoints(IEnumerable<string> mailboxes);
	}
}
