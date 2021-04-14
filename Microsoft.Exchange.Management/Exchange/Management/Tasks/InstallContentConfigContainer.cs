using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ContentConfigContainer")]
	public sealed class InstallContentConfigContainer : InstallContainerTaskBase<ContentConfigContainer>
	{
	}
}
