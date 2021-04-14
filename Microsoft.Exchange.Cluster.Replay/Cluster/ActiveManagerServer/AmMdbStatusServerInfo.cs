using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmMdbStatusServerInfo
	{
		internal AmServerName ServerName { get; set; }

		internal bool IsNodeUp { get; set; }

		internal TimeSpan TimeOut { get; set; }

		internal bool IsStoreRunning { get; set; }

		internal bool IsReplayRunning { get; set; }

		internal AmMdbStatusServerInfo(AmServerName serverName, bool isNodeUp, TimeSpan timeout)
		{
			this.ServerName = serverName;
			this.IsNodeUp = isNodeUp;
			this.TimeOut = timeout;
			this.IsReplayRunning = false;
			this.IsStoreRunning = false;
		}
	}
}
