using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IProxyHubSelector
	{
		bool TrySelectHubServers(IReadOnlyMailItem mailItem, out IEnumerable<INextHopServer> hubServers);

		bool TrySelectHubServersForClientProxy(MiniRecipient recipient, out IEnumerable<INextHopServer> hubServers);
	}
}
