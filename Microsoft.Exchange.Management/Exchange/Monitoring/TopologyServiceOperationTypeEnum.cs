using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum TopologyServiceOperationTypeEnum
	{
		GetAllTopologyVersions,
		GetTopologyVersion,
		SetConfigDC,
		ReportServerDown,
		GetServersForRole,
		Test
	}
}
