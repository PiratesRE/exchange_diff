using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "IPAllowListEntry", SupportsShouldProcess = true, DefaultParameterSetName = "IPRange")]
	public sealed class AddIPAllowListEntry : AddIPListEntry<IPAllowListEntry>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("IPAddress" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageAddIPAllowListEntryIPAddress(base.IPAddress.ToString());
				}
				return Strings.ConfirmationMessageAddIPAllowListEntryIPRange(base.IPRange.ToString());
			}
		}
	}
}
