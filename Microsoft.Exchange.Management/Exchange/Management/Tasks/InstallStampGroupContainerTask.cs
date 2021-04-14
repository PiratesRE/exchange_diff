using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "StampGroupContainer")]
	public sealed class InstallStampGroupContainerTask : InstallContainerTaskBase<StampGroupContainer>
	{
	}
}
