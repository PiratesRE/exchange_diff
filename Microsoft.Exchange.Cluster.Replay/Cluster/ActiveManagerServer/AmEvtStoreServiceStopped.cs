using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtStoreServiceStopped : AmEvtBase
	{
		internal AmEvtStoreServiceStopped(AmServerName nodeName)
		{
			this.NodeName = nodeName;
			this.IsGracefulStop = true;
		}

		internal AmServerName NodeName { get; set; }

		internal bool IsGracefulStop { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (NodeName={1}, IsGracefulStop={2})", base.GetType().Name, this.NodeName, this.IsGracefulStop);
		}
	}
}
