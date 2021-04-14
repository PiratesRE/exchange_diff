using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtMapiNetworkFailure : AmEvtBase
	{
		internal AmEvtMapiNetworkFailure(AmServerName nodeName)
		{
			this.NodeName = nodeName;
			this.DetectionTimeUtc = DateTime.UtcNow;
		}

		internal AmServerName NodeName { get; set; }

		internal DateTime DetectionTimeUtc { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (NodeName={1},DetectedAt={2}UTC)", base.GetType().Name, this.NodeName, this.DetectionTimeUtc);
		}
	}
}
