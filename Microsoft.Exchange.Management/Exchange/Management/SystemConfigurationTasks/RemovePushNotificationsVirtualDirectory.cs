using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "PushNotificationsVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemovePushNotificationsVirtualDirectory : RemoveExchangeVirtualDirectory<ADPushNotificationsVirtualDirectory>
	{
	}
}
