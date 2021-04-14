using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "IRMConfiguration")]
	public sealed class InstallIRMConfiguration : InstallContainerTaskBase<IRMConfiguration>
	{
		protected override IConfigurable PrepareDataObject()
		{
			return IRMConfiguration.Read((IConfigurationSession)base.DataSession);
		}
	}
}
