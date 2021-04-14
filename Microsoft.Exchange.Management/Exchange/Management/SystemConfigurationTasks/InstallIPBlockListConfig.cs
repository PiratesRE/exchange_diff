using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "IPBlockListConfig")]
	public sealed class InstallIPBlockListConfig : InstallAntispamConfig<IPBlockListConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "IPBlockListConfig";
			}
		}
	}
}
