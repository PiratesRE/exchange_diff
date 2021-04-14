using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PushNotificationsVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetPushNotificationsVirtualDirectory : GetExchangeServiceVirtualDirectory<ADPushNotificationsVirtualDirectory>
	{
	}
}
