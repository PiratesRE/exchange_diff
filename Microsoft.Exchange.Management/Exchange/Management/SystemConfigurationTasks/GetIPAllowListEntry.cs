using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IPAllowListEntry", DefaultParameterSetName = "Identity")]
	public sealed class GetIPAllowListEntry : GetIPListEntry<IPAllowListEntry>
	{
	}
}
