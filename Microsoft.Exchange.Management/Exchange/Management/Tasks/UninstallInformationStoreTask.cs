using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("uninstall", "InformationStore")]
	public sealed class UninstallInformationStoreTask : RemoveSystemConfigurationObjectTask<InformationStoreIdParameter, InformationStore>
	{
	}
}
