using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal static class ScopeMappingEndpointManagerExtensions
	{
		internal static ScopeMappingEndpoint GetEndpoint(this ScopeMappingEndpointManager endpointManager)
		{
			return LocalEndpointManager.Instance.ScopeMappingLocalEndpoint;
		}
	}
}
