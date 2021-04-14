using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "SenderReputationConfig")]
	public sealed class NewSenderReputationConfig : InstallContainerTaskBase<SenderReputationConfig>
	{
	}
}
