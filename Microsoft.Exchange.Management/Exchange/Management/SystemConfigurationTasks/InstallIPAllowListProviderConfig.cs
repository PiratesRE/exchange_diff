using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "IPAllowListProvidersConfig")]
	public sealed class InstallIPAllowListProviderConfig : InstallAntispamConfig<IPAllowListProviderConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "IPAllowListProviderConfig";
			}
		}
	}
}
