using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal delegate IEnumerable<INextHopServer> FindRelatedBridgeHeads(NextHopSolutionKey nextHopSolutionKey);
}
