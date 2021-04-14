using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OfflineAddressBook", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOfflineAddressBook : SetOfflineAddressBookInternal
	{
	}
}
