using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "Container")]
	public sealed class InstallContainerTask : InstallContainerTaskBase<Container>
	{
	}
}
