using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtNodeStateChanged : AmEvtBase
	{
		internal AmEvtNodeStateChanged(AmServerName nodeName, AmNodeState nodeState)
		{
			this.NodeName = nodeName;
			this.State = nodeState;
		}

		internal AmServerName NodeName { get; set; }

		internal AmNodeState State { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (NodeName={1}, NodeState={2})", base.GetType().Name, this.NodeName, this.State);
		}
	}
}
