using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtSwitchoverOnShutdown : AmEvtServerSwitchoverBase
	{
		internal AmEvtSwitchoverOnShutdown(AmServerName nodeName) : base(nodeName)
		{
		}
	}
}
