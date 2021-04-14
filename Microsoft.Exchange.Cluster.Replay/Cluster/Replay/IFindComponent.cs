using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IFindComponent
	{
		MonitoredDatabase FindMonitoredDatabase(string nodeName, Guid dbGuid);

		LogCopier FindLogCopier(string nodeName, Guid dbGuid);
	}
}
