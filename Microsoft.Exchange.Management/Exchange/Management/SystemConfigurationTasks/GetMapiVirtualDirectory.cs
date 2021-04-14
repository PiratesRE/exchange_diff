using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MapiVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetMapiVirtualDirectory : GetExchangeVirtualDirectory<ADMapiVirtualDirectory>
	{
		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}
	}
}
