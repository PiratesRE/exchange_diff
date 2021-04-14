using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "TransportService", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetTransportService : SetTransportServiceBase
	{
	}
}
