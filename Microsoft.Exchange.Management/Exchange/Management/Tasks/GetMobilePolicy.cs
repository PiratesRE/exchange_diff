using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("get", "MobileDeviceMailboxPolicy", DefaultParameterSetName = "Identity")]
	public class GetMobilePolicy : GetMailboxPolicyBase<MobileMailboxPolicy>
	{
	}
}
