using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface IManagementClassHelper
	{
		DateTime LocalBootTime { get; }

		DateTime GetBootTime(AmServerName machineName);

		string LocalComputerFqdn { get; }

		string LocalDomainName { get; }

		string LocalMachineName { get; }
	}
}
