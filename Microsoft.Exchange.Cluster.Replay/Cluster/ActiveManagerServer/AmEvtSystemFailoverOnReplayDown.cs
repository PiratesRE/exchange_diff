using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtSystemFailoverOnReplayDown : AmEvtBase
	{
		internal AmEvtSystemFailoverOnReplayDown(AmServerName serverName)
		{
			this.ServerName = serverName;
		}

		internal AmServerName ServerName { get; private set; }
	}
}
