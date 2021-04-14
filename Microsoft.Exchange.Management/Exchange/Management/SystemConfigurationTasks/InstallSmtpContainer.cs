using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "SmtpContainer")]
	public sealed class InstallSmtpContainer : InstallContainerTaskBase<SmtpContainer>
	{
		protected override ADObjectId GetBaseContainer()
		{
			return (base.DataSession as ITopologyConfigurationSession).FindLocalServer().Id;
		}
	}
}
