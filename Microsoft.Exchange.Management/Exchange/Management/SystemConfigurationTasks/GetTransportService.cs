using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "TransportService", DefaultParameterSetName = "Identity")]
	public sealed class GetTransportService : GetTransportServiceBase
	{
	}
}
