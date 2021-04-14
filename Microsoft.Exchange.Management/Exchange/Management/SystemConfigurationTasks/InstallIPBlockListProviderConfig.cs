using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "IPBlockListProvidersConfig")]
	public sealed class InstallIPBlockListProviderConfig : InstallAntispamConfig<IPBlockListProviderConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "IPBlockListProviderConfig";
			}
		}
	}
}
