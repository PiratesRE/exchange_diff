using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("install", "GlobalSettingsContainer")]
	public sealed class InstallGlobalSettingsContainer : InstallContainerTaskBase<ADContainer>
	{
	}
}
