using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "IPAllowListConfig")]
	public sealed class InstallIPAllowListConfig : InstallAntispamConfig<IPAllowListConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "IPAllowListConfig";
			}
		}
	}
}
