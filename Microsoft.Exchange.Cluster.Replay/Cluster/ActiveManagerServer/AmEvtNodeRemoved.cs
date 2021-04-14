using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtNodeRemoved : AmEvtBase
	{
		internal AmEvtNodeRemoved(AmServerName nodeName)
		{
			this.NodeName = nodeName;
		}

		internal AmServerName NodeName { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (NodeName={1})", base.GetType().Name, this.NodeName);
		}
	}
}
