using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "IPBlockListEntry", DefaultParameterSetName = "Identity")]
	public sealed class GetIPBlockListEntry : GetIPListEntry<IPBlockListEntry>
	{
	}
}
